using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class OccupationEditor : RadForm
    {
        /// <summary>
        /// Stores a snapshot of the form field values at the time of load or last Apply.
        /// Used to detect unsaved changes when the user tries to navigate away.
        /// </summary>
        private class FormSnapshot
        {
            public string Name;
            public string Code;
            public decimal Stipend;
        }

        /// <summary>
        /// Captures a snapshot of the current form field values.
        /// </summary>
        private FormSnapshot _lastSavedSnapshot;

        /// <summary>
        /// Represents the currently selected node in the RadTreeView control.
        /// Used to track and manage the active selection state,
        /// allowing for operations such as enabling or disabling menu items
        /// and binding the form inputs to the selected occ.
        /// </summary>
        private RadTreeNode _selectedNode;

        /// <summary>
        /// Represents the occupation currently selected
        /// </summary>
        private Occupation SelectedOccupation { get; set; }

        /// <summary>
        /// Represents the faction currently selected in the rank and grade editor form.
        /// This property provides access to the faction's details, such as its name and identifier,
        /// and is used throughout the form to configure rank occcupations and UI elements
        /// specific to the selected faction.
        /// </summary>
        private Faction SelectedFaction { get; set; }

        public OccupationEditor(Faction selectedFaction)
        {
            if (selectedFaction == null)
                throw new ArgumentNullException(nameof(selectedFaction));

            SelectedFaction = selectedFaction;

            // Create components and apply theme
            InitializeComponent();
            radTreeView1.TreeViewElement.DrawBorder = false;
            radTreeView1.Nodes.Add(new RadTreeNode("Enlisted") { Tag = RankType.Enlisted });
            radTreeView1.Nodes.Add(new RadTreeNode("Officer") { Tag = RankType.Officer });
            radTreeView1.Nodes.Add(new RadTreeNode("Warrant") { Tag = RankType.Warrant });

            // Panel Styling, show only left border
            radPanel2.PanelElement.PanelBorder.BoxStyle = BorderBoxStyle.FourBorders;
            radPanel2.PanelElement.PanelBorder.TopWidth = 0;
            radPanel2.PanelElement.PanelBorder.BottomWidth = 0;
            radPanel2.PanelElement.PanelBorder.RightWidth = 0;

            // Button styling
            FormStyling.ApplyControlsTheme(Controls);
            
            // Set default values and indexes
            ResetFields(true);

            // Set the header label to include the faction name
            headerLabel.Text = $"Occupation Editor for {SelectedFaction.Name}";
            
            // Wire up change-detection events for dirty-state styling
            nameTextBox.TextChanged += (s, ev) => UpdateApplyButtonStyle();
            codeTextBox.TextChanged += (s, ev) => UpdateApplyButtonStyle();
            stipendSpinEditor.ValueChanged += (s, ev) => UpdateApplyButtonStyle();

            // Register event handlers
            addMenuItem.Click += AddMenuItem_Click;
            deleteMenuItem.Click += DeleteMenuItem_Click;

            // Disable context menu items until a valid node is selected
            addMenuItem.Enabled = false;
            deleteMenuItem.Enabled = false;
        }

        /// <summary>
        /// Finds the root tree node that corresponds to the given <see cref="RankType"/>.
        /// </summary>
        private RadTreeNode FindRootNodeByType(RankType type)
        {
            foreach (RadTreeNode node in radTreeView1.Nodes)
            {
                if (node.Tag is RankType nodeType && nodeType == type)
                    return node;
            }
            return null;
        }

        /// <summary>
        /// Resets the fields and selections in the user interface to their default values.
        /// </summary>
        private void ResetFields(bool clearSelectedNode)
        {
            if (clearSelectedNode)
            {
                radTreeView1.SelectedNode = null;
            }

            SelectedOccupation = null;
            descriptionGroupBox.Text = "Please Add or Select an Occupation";
            stipendSpinEditor.Value = 0;
            nameTextBox.Text = "";
            codeTextBox.Text = "";
            _lastSavedSnapshot = null;

            // Disable editing controls
            SetFormEnabled(false);
            
            // Ensure apply button is disabled
            UpdateApplyButtonStyle();
        }

        /// <summary>
        /// Enables or disables the editing controls based on whether a rank grade is selected.
        /// </summary>
        private void SetFormEnabled(bool enabled)
        {
            // Textboxes
            nameTextBox.Enabled = enabled;
            codeTextBox.Enabled = enabled;

            // Spinners
            stipendSpinEditor.Enabled = enabled;

            // Buttons
            boardButton.Enabled = enabled;
        }

        /// <summary>
        /// Updates the UI to reflect the details of the specified occupation.
        /// </summary>
        private void SelectOccupation(Occupation occ)
        {
            SelectedOccupation = occ;
            descriptionGroupBox.Text = $"{occ.Type} Occupation: {occ.ToString()}";

            stipendSpinEditor.Value = (decimal)occ.Stipend;
            nameTextBox.Text = occ.Name;
            codeTextBox.Text = occ.Code;

            // Snapshot the clean state
            _lastSavedSnapshot = CaptureSnapshot();

            // Enable editing controls
            SetFormEnabled(true);
            
            // Now update apply button style (will disable it since snapshot matches)
            UpdateApplyButtonStyle();
        }

        /// <summary>
        /// Captures the current form field values into a snapshot.
        /// </summary>
        private FormSnapshot CaptureSnapshot()
        {
            return new FormSnapshot
            {
                Name = nameTextBox.Text,
                Code = codeTextBox.Text,
                Stipend = stipendSpinEditor.Value,
            };
        }

        /// <summary>
        /// Returns true if the current form values differ from the last saved snapshot.
        /// </summary>
        private bool HasUnsavedChanges()
        {
            if (_lastSavedSnapshot == null || SelectedOccupation == null)
                return false;

            var current = CaptureSnapshot();
            return current.Name != _lastSavedSnapshot.Name
                   || current.Code != _lastSavedSnapshot.Code
                   || current.Stipend != _lastSavedSnapshot.Stipend;
        }
        
        /// <summary>
        /// Updates the Apply button's color based on whether there are unsaved changes.
        /// Blue when dirty, FluentDefault (disabled) when clean.
        /// </summary>
        private void UpdateApplyButtonStyle()
        {
            if (HasUnsavedChanges())
            {
                FormStyling.StyleButtonBlue(applyButton);
                applyButton.Enabled = true;
            }
            else
            {
                FormStyling.StyleButtonFluentDefault(applyButton);
                applyButton.Enabled = false;
            }
        }

        /// <summary>
        /// Loads all existing Occupationss for the selected faction from the database
        /// and populates the tree view under the appropriate root nodes.
        /// </summary>
        private void LoadOccupationsFromDatabase()
        {
            // Clear existing child nodes from root nodes
            foreach (RadTreeNode rootNode in radTreeView1.Nodes)
            {
                rootNode.Nodes.Clear();
            }

            try
            {
                using var db = new AppDatabase();

                // Fetch all RankClassifications for this faction, ordered by PayGrade
                var occcupations = db.Occupations
                    .Where(rc => rc.FactionId == SelectedFaction.Id)
                    .OrderBy(rc => rc.Code)
                    .ToList();

                foreach (var occ in occcupations)
                {
                    // Find the matching root node by RankType
                    RadTreeNode parentNode = FindRootNodeByType(occ.Type);
                    if (parentNode == null) continue;

                    var childNode = new RadTreeNode(occ.ToString())
                    {
                        Tag = occ
                    };
                    parentNode.Nodes.Add(childNode);
                }

                // Expand all root nodes
                foreach (RadTreeNode rootNode in radTreeView1.Nodes)
                {
                    rootNode.Expand();
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Failed to load occupations: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        #region Events

        private void OccupationEditor_Load(object sender, EventArgs e)
        {
            LoadOccupationsFromDatabase();
        }

        /// <summary>
        /// Handles the FormClosing event. Prompts the user to confirm closing the form if there are unsaved changes.
        /// </summary>
        /// <param name="sender">The source of the event, typically the form being closed.</param>
        /// <param name="e">Provides data for the FormClosing event, including the option to cancel the close operation.</param>
        private void OccupationEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasUnsavedChanges())
            {
                var result = RadMessageBox.Show(
                    "You have unsaved changes. Are you sure you want to close without applying?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Handles the event triggered when the selected node in the RadTreeView changes.
        /// </summary>
        private void radTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            // Check for unsaved changes before allowing navigation
            if (HasUnsavedChanges())
            {
                var result = RadMessageBox.Show(
                    "You have unsaved changes. Are you sure you want to switch without applying?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);

                if (result != DialogResult.Yes)
                {
                    // Revert selection back to the previous node
                    radTreeView1.SelectedNodeChanged -= radTreeView1_SelectedNodeChanged;
                    radTreeView1.SelectedNode = _selectedNode;
                    radTreeView1.SelectedNodeChanged += radTreeView1_SelectedNodeChanged;
                    return;
                }
            }

            var node = e.Node;
            _selectedNode = node;
            bool isRoot = node != null && node.Parent == null;

            addMenuItem.Enabled = isRoot;
            deleteMenuItem.Enabled = !isRoot;

            // Bind form inputs to selected occ
            if (node != null && node.Tag is Occupation occ)
            {
                SelectOccupation(occ);
            }
            else
            {
                ResetFields(!isRoot);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Apply button click. Saves the currently selected occupation's
        /// field values back to the database using the CrossLite Unit of Work pattern.
        /// </summary>
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (SelectedOccupation == null)
            {
                RadMessageBox.Show("No occupation selected to save.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                // Update the occ from form fields
                SelectedOccupation.Name = nameTextBox.Text;
                SelectedOccupation.Code = codeTextBox.Text;
                SelectedOccupation.Stipend = (double)stipendSpinEditor.Value;

                if (SelectedOccupation.Id == 0)
                {
                    // New occ — insert
                    SelectedOccupation.FactionId = SelectedFaction.Id;
                    db.Occupations.Add(SelectedOccupation);
                }
                else
                {
                    // Existing occ — update
                    db.Occupations.Update(SelectedOccupation);
                }

                transaction.Commit();

                // Update the tree node text
                if (radTreeView1.SelectedNode != null && radTreeView1.SelectedNode.Tag is RankClassification)
                {
                    radTreeView1.SelectedNode.Text = SelectedOccupation.Name;
                    radTreeView1.SelectedNode.Tag = SelectedOccupation;
                }

                // Update the clean snapshot after a successful save
                _lastSavedSnapshot = CaptureSnapshot();
                _selectedNode.Text = SelectedOccupation.ToString();
                UpdateApplyButtonStyle();

                // Update description box
                descriptionGroupBox.Text = $"{SelectedOccupation.Type} Occupation: {SelectedOccupation.ToString()}";

                RadMessageBox.Show("Occupation saved successfully.",
                    "Success", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to save occupation: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Handles the click event for the "Delete Occupation" menu item.
        /// </summary>
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            var selectedNode = radTreeView1.SelectedNode;
            if (selectedNode == null || selectedNode.Parent == null)
                return; // Only allow deleting child nodes

            var parentNode = selectedNode.Parent;
            if (!(selectedNode.Tag is Occupation occ))
                return;

            // Get count
            //var count = occ.

            // Confirm deletion
            var result = RadMessageBox.Show(
                $"Are you sure you want to delete the occupation {occ.Name}? This will also delete all associated position blueprints.",
                "Confirm Delete", MessageBoxButtons.YesNo, RadMessageIcon.Exclamation);

            if (result != DialogResult.Yes)
                return;

            // Delete from database if persisted
            if (occ.Id > 0)
            {
                using var db = new AppDatabase();
                using var transaction = db.BeginTransaction();

                try
                {
                    db.Occupations.Remove(occ);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    RadMessageBox.Show($"Failed to delete occupation: {ex.Message}",
                        "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }
            }

            parentNode.Nodes.Remove(selectedNode);
            radTreeView1.SelectedNode = parentNode;
            _selectedNode = parentNode;
            ResetFields(false);
        }

        /// <summary>
        /// Handles the click event for the "Add Occupation" menu item.
        /// </summary>
        private void AddMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure a root node is selected
            var selectedNode = radTreeView1.SelectedNode;
            if (selectedNode == null || selectedNode.Parent != null)
                return;

            // Find the rank type
            RankType rankType = (RankType)selectedNode.Tag;

            // Create and persist the new RankClassification
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                var occ = db.Occupations.Create();
                occ.FactionId = SelectedFaction.Id;
                occ.Type = rankType;
                occ.Stipend = 0d;
                occ.Name = "New Occupation";
                occ.Code = "NEW";

                db.Occupations.Add(occ);
                transaction.Commit();

                // Create new tree node and add to tree
                var newNode = new RadTreeNode(occ.ToString())
                {
                    Tag = occ,
                };
                selectedNode.Nodes.Add(newNode);
                selectedNode.Expand();
                radTreeView1.SelectedNode = newNode;
                _selectedNode = newNode;

                SelectOccupation(occ);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to add occupation: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void BoardButton_Click(object sender, EventArgs e)
        {
            if (SelectedOccupation == null || SelectedOccupation.Id <= 0) return;

            using var form = new OccupationBoardListForm(SelectedOccupation);
            form.ShowDialog(this);
        }

        #endregion

        #region Form Styling

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion
    }
}
