using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class RankGradeEditorForm : RadForm
    {
        /// <summary>
        /// Stores a snapshot of the form field values at the time of load or last Apply.
        /// Used to detect unsaved changes when the user tries to navigate away.
        /// </summary>
        private class FormSnapshot
        {
            public int SelectionIndex;
            public decimal PrevTIG;
            public decimal MaxTIG;
            public decimal MinTIG;
            public decimal LockIn;
            public decimal Stipend;
            public bool HasSplitLanes;
        }

        /// <summary>
        /// Captures a snapshot of the current form field values.
        /// </summary>
        private FormSnapshot _lastSavedSnapshot;

        /// <summary>
        /// Represents the currently selected node in the RadTreeView control.
        /// Used to track and manage the active selection state,
        /// allowing for operations such as enabling or disabling menu items
        /// and binding the form inputs to the selected classification.
        /// </summary>
        private RadTreeNode _selectedNode;

        /// <summary>
        /// Represents the faction currently selected in the rank and grade editor form.
        /// This property provides access to the faction's details, such as its name and identifier,
        /// and is used throughout the form to configure rank classifications and UI elements
        /// specific to the selected faction.
        /// </summary>
        private Faction SelectedFaction { get; set; }

        /// <summary>
        /// The currently selected RankClassification being edited, or null if none is selected.
        /// </summary>
        private RankClassification SelectedClassification { get; set; }

        /// <summary>
        /// The four rank selector controls in an array for easy indexed access.
        /// </summary>
        private RadRankSelector[] RankSelectors { get; set; }
        
        /// <summary>
        /// The custom promotion board for the currently selected classification.
        /// </summary>
        private PromotionBoard _promotionBoard;

        /// <summary>
        /// A form that allows users to edit rank classifications and pay grades for
        /// the selected faction. Provides a tree view categorized by rank types (Enlisted,
        /// Officer, Warrant) and various UI components for grade configuration.
        /// </summary>
        public RankGradeEditorForm(Faction selectedFaction)
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

            // Store rank selectors in an array for easy access
            RankSelectors = new[] { radRankSelector1, radRankSelector2, radRankSelector3, radRankSelector4 };

            // Fill the selection dropdown with enum values
            foreach (PayGradeSelection item in Enum.GetValues(typeof(PayGradeSelection)))
            {
                var radItem = new RadListDataItem()
                {
                    Tag = item,
                    Text = Enum.GetName(typeof(PayGradeSelection), item)
                };
                selectionTypeDropDownList.Items.Add(radItem);
            }

            // Set default values and indexes
            ResetFields(true);

            // Set the header label to include the faction name
            headerLabel.Text = $"Rank And Grade Editor for {SelectedFaction.Name}";

            // Register event handlers
            addGradeMenuItem.Click += AddGradeMenuItem_Click;
            wizardMenuItem.Click += WizardMenuItem_Click;
            deleteGradeMenuItem.Click += DeleteGradeMenuItem_Click;
            aiMenuItem.Click += AiMenuItem_Click;
            
            // Disable context menu items until a valid node is selected
            addGradeMenuItem.Enabled = false;
            deleteGradeMenuItem.Enabled = false;
            
            // Wire up change-detection events for dirty-state styling
            prevTimeInGradeSpinner.ValueChanged += (s, ev) => UpdateApplyButtonStyle();
            maxTimeInGradeSpinner.ValueChanged += (s, ev) => UpdateApplyButtonStyle();
            minTimeInGradeSpinner.ValueChanged += (s, ev) => UpdateApplyButtonStyle();
            lockInTimeSpinner.ValueChanged += (s, ev) => UpdateApplyButtonStyle();
            stipendSpinEditor.ValueChanged += (s, ev) => UpdateApplyButtonStyle();
            selectionTypeDropDownList.SelectedIndexChanged += (s, ev) => UpdateApplyButtonStyle();
            branchingCheckBox.ToggleStateChanged += (s, ev) => UpdateApplyButtonStyle();

            // Wire up rank selector click events
            foreach (var selector in RankSelectors)
            {
                selector.OnClick += RankSelector_Click;
            }
        }

        private void RankGradeEditorForm_Load(object sender, EventArgs e)
        {
            LoadGradesFromDatabase();
        }

        /// <summary>
        /// Loads all existing RankClassifications for the selected faction from the database
        /// and populates the tree view under the appropriate root nodes.
        /// </summary>
        private void LoadGradesFromDatabase()
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
                var classifications = db.RankClassifications
                    .Where(rc => rc.FactionId == SelectedFaction.Id)
                    .OrderBy(rc => rc.PayGrade)
                    .ToList();

                foreach (var classification in classifications)
                {
                    // Find the matching root node by RankType
                    RadTreeNode parentNode = FindRootNodeByType(classification.Type);
                    if (parentNode == null) continue;

                    var childNode = new RadTreeNode(classification.ToString())
                    {
                        Tag = classification
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
                RadMessageBox.Show($"Failed to load rank grades: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
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

        private void AiMenuItem_Click(object sender, EventArgs e)
        {
            AdvisorChatForm.SetFactionId(SelectedFaction.Id);
            AdvisorChatForm.Open(this);
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

            SelectedClassification = null;
            DescriptionGroupBox.Text = "Please Add or Select a Rank Grade";
            selectionTypeDropDownList.SelectedIndex = 0;
            prevTimeInGradeSpinner.Value = 0;
            maxTimeInGradeSpinner.Value = 0;
            minTimeInGradeSpinner.Value = 0;
            lockInTimeSpinner.Value = 0;
            stipendSpinEditor.Value = 0;
            branchingCheckBox.IsChecked = false;
            _lastSavedSnapshot = null;

            // Clear rank selectors
            ClearRankSelectors();
            
            // Clear board
            _promotionBoard = null;

            // Disable editing controls
            SetFormEnabled(false);
            
            // Ensure apply button is disabled (no changes possible when nothing is selected)
            UpdateApplyButtonStyle();
        }

        /// <summary>
        /// Enables or disables the editing controls based on whether a rank grade is selected.
        /// </summary>
        private void SetFormEnabled(bool enabled)
        {
            // Spinners
            prevTimeInGradeSpinner.Enabled = enabled;
            maxTimeInGradeSpinner.Enabled = enabled;
            minTimeInGradeSpinner.Enabled = enabled;
            lockInTimeSpinner.Enabled = enabled;

            // Dropdown
            selectionTypeDropDownList.Enabled = enabled;

            // Checkbox
            branchingCheckBox.Enabled = enabled;

            // Buttons
            // applyButton.Enabled = enabled;  <-- REMOVE THIS LINE
            boardButton.Enabled = enabled;

            // Rank selectors
            foreach (var selector in RankSelectors)
            {
                selector.Enabled = enabled;
            }
        }

        /// <summary>
        /// Clears all rank selector controls back to their default state.
        /// </summary>
        private void ClearRankSelectors()
        {
            foreach (var selector in RankSelectors)
            {
                selector.Rank = null;
                selector.Visible = false;
            }
        }

        /// <summary>
        /// Updates the UI to reflect the details of the specified rank classification.
        /// </summary>
        private void SelectClassification(RankClassification classification)
        {
            SelectedClassification = classification;
            DescriptionGroupBox.Text = $"{classification.Type} Grade {classification.PayGrade}";

            // Set the selection dropdown to match the classification's Selection value
            for (int i = 0; i < selectionTypeDropDownList.Items.Count; i++)
            {
                if (selectionTypeDropDownList.Items[i].Tag is PayGradeSelection sel && sel == classification.Selection)
                {
                    selectionTypeDropDownList.SelectedIndex = i;
                    break;
                }
            }

            prevTimeInGradeSpinner.Value = classification.PreviousTimeInGradeRequirement;
            maxTimeInGradeSpinner.Value = classification.MaxTimeInGrade;
            minTimeInGradeSpinner.Value = classification.MinTimeInGrade;
            lockInTimeSpinner.Value = classification.LockInTime;
            stipendSpinEditor.Value = (decimal)classification.Stipend;
            branchingCheckBox.IsChecked = classification.HasSplitRankLanes;

            // Snapshot the clean state
            _lastSavedSnapshot = CaptureSnapshot();

            // Enable editing controls
            SetFormEnabled(true);
            
            // Now update apply button style (will disable it since snapshot matches)
            UpdateApplyButtonStyle();

            // Load ranks for this classification into the rank selectors
            LoadRanksAndBoardForClassification(classification);
        }

        /// <summary>
        /// Loads the ranks associated with the given classification into the RadRankSelector controls.
        /// </summary>
        private void LoadRanksAndBoardForClassification(RankClassification classification)
        {
            ClearRankSelectors();

            // Only load ranks if the classification has been persisted (has an Id)
            if (classification.Id == 0) return;

            try
            {
                using var db = new AppDatabase();

                var ranks = db.Ranks
                    .Where(r => r.RankClassificationId == classification.Id)
                    .OrderBy(r => r.Precedence)
                    .ToList();

                for (int i = 0; i < ranks.Count && i < RankSelectors.Length; i++)
                {
                    RankSelectors[i].Rank = ranks[i];
                    RankSelectors[i].RankTitle = $"Rank {i + 1}";
                    RankSelectors[i].Visible = true;
                }

                // Show the next empty slot if there's room for more ranks
                if (ranks.Count < RankSelectors.Length)
                {
                    RankSelectors[ranks.Count].Rank = null;
                    RankSelectors[ranks.Count].RankTitle = $"Rank {ranks.Count + 1}";
                    RankSelectors[ranks.Count].Visible = true;
                }

                // Center the visible selectors within the panel
                CenterRankSelectors();
                
                _promotionBoard = db.PromotionBoards.Find(
                    b => b.RankClassificationId == SelectedClassification.Id
                         && b.RankId == null
                         && b.OccupationId == null
                );

                if (_promotionBoard != null)
                {
                    FormStyling.StyleButtonBlue(boardButton);
                    boardButton.Text = "Edit Promotion Board";
                }
                else
                {
                    FormStyling.StyleButtonFluentDefault(boardButton);
                    boardButton.Text = "Add Promotion Board";
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Failed to load ranks: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

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

        #region Events

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Apply button click. Saves the currently selected classification's
        /// field values back to the database using the CrossLite Unit of Work pattern.
        /// </summary>
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (SelectedClassification == null)
            {
                RadMessageBox.Show("No rank grade selected to save.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                // ValidateAndAlertUserOnFail HasSplitRankLanes consistency with child ranks' NextRankId
                var isValidated = ValidateAndAlertUserOnFail(db);
                if (!isValidated) return;

                // Update the classification from form fields
                SelectedClassification.PreviousTimeInGradeRequirement = (int)prevTimeInGradeSpinner.Value;
                SelectedClassification.MaxTimeInGrade = (int)maxTimeInGradeSpinner.Value;
                SelectedClassification.MinTimeInGrade = (int)minTimeInGradeSpinner.Value;
                SelectedClassification.LockInTime = (int)lockInTimeSpinner.Value;
                SelectedClassification.Stipend = (double)stipendSpinEditor.Value;
                SelectedClassification.HasSplitRankLanes = branchingCheckBox.IsChecked;

                // Get the selected PayGradeSelection from the dropdown
                if (selectionTypeDropDownList.SelectedItem?.Tag is PayGradeSelection selection)
                {
                    SelectedClassification.Selection = selection;
                }

                if (SelectedClassification.Id == 0)
                {
                    // New classification — insert
                    SelectedClassification.FactionId = SelectedFaction.Id;
                    db.RankClassifications.Add(SelectedClassification);
                }
                else
                {
                    // Existing classification — update
                    db.RankClassifications.Update(SelectedClassification);
                }

                transaction.Commit();

                // Update the tree node text
                if (radTreeView1.SelectedNode != null && radTreeView1.SelectedNode.Tag is RankClassification)
                {
                    radTreeView1.SelectedNode.Text = SelectedClassification.ToString();
                    radTreeView1.SelectedNode.Tag = SelectedClassification;
                }

                // Reload ranks for the classification (in case Id was just assigned)
                LoadRanksAndBoardForClassification(SelectedClassification);

                // Update the clean snapshot after a successful save
                _lastSavedSnapshot = CaptureSnapshot();
                
                // Ensure apply button is disabled
                UpdateApplyButtonStyle();

                RadMessageBox.Show("Rank grade saved successfully.",
                    "Success", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to save rank grade: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
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

            addGradeMenuItem.Enabled = isRoot;
            deleteGradeMenuItem.Enabled = !isRoot;
            wizardMenuItem.Enabled = true;

            // Bind form inputs to selected classification
            if (node != null && node.Tag is RankClassification classification)
            {
                SelectClassification(classification);
            }
            else
            {
                ResetFields(!isRoot);
            }
        }

        private void SelectionTypeDropDownListSelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            // Enable/disable the board button based on whether PromotionBoard is selected
            if (selectionTypeDropDownList.SelectedItem?.Tag is PayGradeSelection selection)
            {
                boardButton.Enabled = (selection == PayGradeSelection.PromotionBoard);
            }
        }

        /// <summary>
        /// Handles the board button click. Opens the promotion board editor for the selected classification.
        /// </summary>
        private void BoardButton_Click(object sender, EventArgs e)
        {
            if (SelectedClassification == null || SelectedClassification.Id == 0)
            {
                RadMessageBox.Show("Please save the rank grade before editing its promotion board.",
                    "Validation", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // PromotionBoardEditor handles creation if no board exists yet
            DialogResult result = DialogResult.None;
            if (_promotionBoard != null)
            {
                using var form = new PromotionBoardEditor(_promotionBoard);
                result = form.ShowDialog(this);
            }
            else
            {
                using var form = new PromotionBoardEditor(SelectedClassification);
                result = form.ShowDialog(this);
            }

            // Requery the database to see if a board was created or deleted
            if (result == DialogResult.OK)
            {
                // Grab Board
                using var db = new AppDatabase();
                _promotionBoard = db.PromotionBoards.Find(
                    b => b.RankClassificationId == SelectedClassification.Id
                         && b.RankId == null
                         && b.OccupationId == null
                );
                
                FormStyling.StyleButtonBlue(boardButton);
                boardButton.Text = "Edit Promotion Board";
            }
            else if (result == DialogResult.Abort)
            {
                // Board Delete button was pressed
                _promotionBoard = null;
                
                FormStyling.StyleButtonFluentDefault(boardButton);
                boardButton.Text = "Add Promotion Board";
            }
        }

        /// <summary>
        /// Handles click events on the RadRankSelector controls.
        /// </summary>
        private void RankSelector_Click(object sender, EventArgs e)
        {
            if (SelectedClassification == null || SelectedClassification.Id == 0)
            {
                RadMessageBox.Show("Please save the rank grade before adding ranks.",
                    "Validation", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            var selector = sender as RadRankSelector;
            if (selector == null) return;

            RankEditor editor;

            if (selector.Rank != null)
            {
                // Edit existing rank
                editor = new RankEditor(SelectedClassification, selector.Rank);
            }
            else
            {
                // Create new rank for this classification
                editor = new RankEditor(SelectedClassification);
            }

            using (editor)
            {
                if (editor.ShowDialog(this) == DialogResult.OK)
                {
                    // Reload ranks to reflect changes
                    LoadRanksAndBoardForClassification(SelectedClassification);
                }
            }
        }

        private void DeleteGradeMenuItem_Click(object sender, EventArgs e)
        {
            var selectedNode = radTreeView1.SelectedNode;
            if (selectedNode == null || selectedNode.Parent == null)
                return; // Only allow deleting child nodes

            var parentNode = selectedNode.Parent;

            // Find the highest PayGrade among child nodes
            int maxPayGrade = 0;
            RadTreeNode highestNode = null;
            foreach (RadTreeNode child in parentNode.Nodes)
            {
                if (child.Tag is RankClassification rc)
                {
                    if (rc.PayGrade > maxPayGrade)
                    {
                        maxPayGrade = rc.PayGrade;
                        highestNode = child;
                    }
                }
            }

            // Only delete if selectedNode is the highest grade
            if (highestNode != selectedNode)
            {
                RadMessageBox.Show("Only the highest grade in this category can be deleted.",
                    "Delete Grade", MessageBoxButtons.OK, RadMessageIcon.Info);
                return;
            }

            if (!(selectedNode.Tag is RankClassification classification))
                return;

            // Confirm deletion
            var result = RadMessageBox.Show(
                $"Are you sure you want to delete {classification}? This will also delete all associated ranks.",
                "Confirm Delete", MessageBoxButtons.YesNo, RadMessageIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Delete from database if persisted
            if (classification.Id > 0)
            {
                using var db = new AppDatabase();
                using var transaction = db.BeginTransaction();

                try
                {
                    db.RankClassifications.Remove(classification);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    RadMessageBox.Show($"Failed to delete rank grade: {ex.Message}",
                        "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }
            }

            parentNode.Nodes.Remove(selectedNode);
            radTreeView1.SelectedNode = parentNode;
            _selectedNode = parentNode;
            ResetFields(false);
        }

        private void WizardMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Open the Rank Wizard dialog
        }

        /// <summary>
        /// Handles the click event for the "Add Grade" menu item.
        /// Creates a new RankClassification with the next PayGrade and persists it to the database.
        /// </summary>
        private void AddGradeMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure a root node is selected
            var selectedNode = radTreeView1.SelectedNode;
            if (selectedNode == null || selectedNode.Parent != null)
                return;

            // Find the highest PayGrade among child nodes
            int maxPayGrade = 0;
            RankType rankType = (RankType)selectedNode.Tag;

            foreach (RadTreeNode child in selectedNode.Nodes)
            {
                if (child.Tag is RankClassification rc)
                {
                    if (rc.PayGrade > maxPayGrade)
                        maxPayGrade = rc.PayGrade;
                }
            }

            // Increment PayGrade for new grade
            int newPayGrade = maxPayGrade + 1;

            // Create and persist the new RankClassification
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                var newGrade = db.RankClassifications.Create();
                newGrade.FactionId = SelectedFaction.Id;
                newGrade.Type = rankType;
                newGrade.PayGrade = newPayGrade;
                newGrade.Selection = PayGradeSelection.Automatic;
                newGrade.PreviousTimeInGradeRequirement = 12;
                newGrade.MinTimeInGrade = 0;
                newGrade.MaxTimeInGrade = 0;
                newGrade.LockInTime = 0;
                newGrade.HasSplitRankLanes = false;

                db.RankClassifications.Add(newGrade);
                transaction.Commit();

                // Create new tree node and add to tree
                var newNode = new RadTreeNode(newGrade.ToString())
                {
                    Tag = newGrade,
                };
                selectedNode.Nodes.Add(newNode);
                selectedNode.Expand();
                radTreeView1.SelectedNode = newNode;
                _selectedNode = newNode;

                SelectClassification(newGrade);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to add rank grade: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        #endregion

        /// <summary>
        /// Captures the current form field values into a snapshot.
        /// </summary>
        private FormSnapshot CaptureSnapshot()
        {
            return new FormSnapshot
            {
                SelectionIndex = selectionTypeDropDownList.SelectedIndex,
                PrevTIG = prevTimeInGradeSpinner.Value,
                MaxTIG = maxTimeInGradeSpinner.Value,
                MinTIG = minTimeInGradeSpinner.Value,
                LockIn = lockInTimeSpinner.Value,
                Stipend = stipendSpinEditor.Value,
                HasSplitLanes = branchingCheckBox.IsChecked
            };
        }

        /// <summary>
        /// Returns true if the current form values differ from the last saved snapshot.
        /// </summary>
        private bool HasUnsavedChanges()
        {
            if (_lastSavedSnapshot == null || SelectedClassification == null)
                return false;

            var current = CaptureSnapshot();
            return current.SelectionIndex != _lastSavedSnapshot.SelectionIndex
                   || current.PrevTIG != _lastSavedSnapshot.PrevTIG
                   || current.MaxTIG != _lastSavedSnapshot.MaxTIG
                   || current.MinTIG != _lastSavedSnapshot.MinTIG
                   || current.LockIn != _lastSavedSnapshot.LockIn
                   || current.Stipend != _lastSavedSnapshot.Stipend
                   || current.HasSplitLanes != _lastSavedSnapshot.HasSplitLanes;
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
        /// Horizontally centers the visible RadRankSelector controls within radPanel1.
        /// </summary>
        private void CenterRankSelectors()
        {
            const int gap = 0;  // horizontal gap between selectors (adjust if you want spacing)

            // Collect visible selectors in order
            var visible = RankSelectors.Where(s => s.Visible).ToArray();
            if (visible.Length == 0) return;

            int selectorWidth = visible[0].Width;  // all selectors are the same size
            int totalWidth = (visible.Length * selectorWidth) + ((visible.Length - 1) * gap);
            int startX = Math.Max(0, (radPanel1.Width - totalWidth) / 2);

            for (int i = 0; i < visible.Length; i++)
            {
                visible[i].Location = new System.Drawing.Point(
                    startX + (i * (selectorWidth + gap)),
                    visible[i].Location.Y  // keep the same Y position
                );
            }
        }

        /// <summary>
        /// Validates the rank classification's configuration, ensuring consistency between the
        /// "HasSplitRankLanes" setting and the child ranks' "NextRankId" values. Displays an
        /// alert message to the user if any validation issues are detected.
        /// </summary>
        /// <returns>True if the validation succeeds; otherwise, false if any validation errors occur.</returns>
        private bool ValidateAndAlertUserOnFail(AppDatabase db)
        {
            if (SelectedClassification.Id > 0)
            {
                var ranks = SelectedClassification.Ranks.ToList();
                if (ranks.Count > 0)
                {
                    bool hasSplit = branchingCheckBox.IsChecked;
                    var ranksWithNext = ranks.Where(r => r.NextRankId.HasValue).ToList();
                    var ranksWithoutNext = ranks.Where(r => !r.NextRankId.HasValue).ToList();
                    PayGradeSelection selMethod = (PayGradeSelection)selectionTypeDropDownList.SelectedItem.Tag;

                    if (!hasSplit && ranksWithNext.Count > 0)
                    {
                        string names = string.Join(", ", ranksWithNext.Select(r => r.Name));
                        var result = RadMessageBox.Show(
                            $"'Has Split Rank Lanes' is not checked, but the following rank(s) have a Next Rank set:\n\n{names}\n\nDo you want to continue saving anyway?",
                            "Configuration Warning", MessageBoxButtons.YesNo, RadMessageIcon.Exclamation);

                        if (result != DialogResult.Yes)
                            return false;
                    }
                    else if (hasSplit && ranksWithoutNext.Count > 0)
                    {
                        string names = string.Join(", ", ranksWithoutNext.Select(r => r.Name));
                        var result = RadMessageBox.Show(
                            $"'Has Split Rank Lanes' is checked, but the following rank(s) do not have a Next Rank set:\n\n{names}\n\nDo you want to continue saving anyway?",
                            "Configuration Warning", MessageBoxButtons.YesNo, RadMessageIcon.Exclamation);

                        if (result != DialogResult.Yes)
                            return false;
                    }

                    // If selection is PromotionBoard and HasSplitLanes, every non-positional rank must have its own board
                    if (hasSplit && selMethod == PayGradeSelection.PromotionBoard)
                    {
                        var missingBoardRanks = ranks
                            .Where(r => !r.IsPositional)
                            .Where(r => !db.PromotionBoards.Any(
                                b => b.RankId == r.Id
                                        && b.RankClassificationId == null
                                        && b.OccupationId == null))
                            .ToList();

                        if (missingBoardRanks.Count > 0)
                        {
                            string names = string.Join(", ", missingBoardRanks.Select(r => r.Name));
                            RadMessageBox.Show(
                                $"Selection method is 'Promotion Board' with split rank lanes enabled. " +
                                $"Each non-positional rank must have its own promotion board.\n\n" +
                                $"The following rank(s) are missing a promotion board:\n{names}",
                                "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                            return false;
                        }
                    }
                }
                else
                {
                    RadMessageBox.Show(
                        $"The selected RankClassification has no ranks", "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Handles the FormClosing event for the RankGradeEditorForm.
        /// Prompts the user to confirm closing the form if there are unsaved changes.
        /// </summary>
        /// <param name="sender">The source of the event, typically the form being closed.</param>
        /// <param name="e">Provides data for the FormClosing event, including the option to cancel the close operation.</param>
        private void RankGradeEditorForm_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}
