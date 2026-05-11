using Perscom.Database;
using Perscom.Simulation;
using System;
using System.IO;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class RankGradeEditorForm : RadForm
    {
        private Faction SelectedFaction { get; set; }

        public RankGradeEditorForm(Faction selectedFaction)
        {
            if (selectedFaction == null)
            {
                
            }

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
            FormStyling.StyleButtonFluentBlue(applyButton);
            FormStyling.StyleButtonDarkBlue(CloseButton);

            // Fill the selection dropdown with enum values
            foreach (PayGradeSelection item in Enum.GetValues(typeof(PayGradeSelection)))
            {
                var radItem = new RadListDataItem()
                {
                    Tag = item,
                    Text = Enum.GetName(typeof(PayGradeSelection), item)
                };
                SelectionDropDownList.Items.Add(radItem);
            }

            // Set default values and indexes
            ResetFields(true);

            // Register event handlers
            addGradeMenuItem.Click += AddGradeMenuItem_Click;
            wizardMenuItem.Click += WizardMenuItem_Click;
            deleteGradeMenuItem.Click += DeleteGradeMenuItem_Click;
            aiMenuItem.Click += AiMenuItem_Click;
        }

        private void AiMenuItem_Click(object sender, EventArgs e)
        {
            AdvisorChatForm.SetFactionId(SelectedFaction.Id);
            AdvisorChatForm.Open(this);
        }

        private void RankGradeEditorForm_Load(object sender, EventArgs e)
        {
            // Load existing grades from the database or simulation context if needed
            // This could be implemented to populate the tree view with existing grades
        }

        /// <summary>
        /// Resets the fields and selections in the user interface to their default values.
        /// </summary>
        /// <remarks>This method clears the selected node in the tree view, resets text and numeric fields
        /// to their  default states, and clears the rows in the ranks grid view. It is typically used to initialize  or
        /// reset the form to a clean state.</remarks>
        private void ResetFields(bool clearSelectedNode)
        {
            // Reset any fields or selections if necessary
            if (clearSelectedNode)
            {
                radTreeView1.SelectedNode = null;
            }

            DescriptionGroupBox.Text = "Please Add or Select a Rank Grade";
            SelectionDropDownList.SelectedIndex = 0;
            PrevTIGReq.Value = 0;
            MaxTIG.Value = 0;
            MinTIG.Value = 0;
            LockInTime.Value = 0;
        }

        /// <summary>
        /// Updates the UI to reflect the details of the specified rank classification.
        /// </summary>
        /// <remarks>This method updates various UI elements, such as text fields, dropdowns, and grid
        /// views,  to display the information associated with the provided rank classification.  It resets any previous
        /// selections and populates the grid view with the ranks associated  with the classification.</remarks>
        /// <param name="classification">The <see cref="RankClassification"/> object containing the details of the rank classification to display.</param>
        private void SelectClassification(RankClassification classification)
        {
            // Reset any fields or selections if necessary
            DescriptionGroupBox.Text = $"{classification.Type} Grade {classification.PayGrade}";
            SelectionDropDownList.SelectedIndex = 0;
            PrevTIGReq.Value = classification.PreviousTimeInGradeRequirement;
            MaxTIG.Value = classification.MaxTimeInGrade;
            MinTIG.Value = classification.MinTimeInGrade;
            LockInTime.Value = classification.LockInTime;
        }

        #region Form Styling

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooterGray(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion

        #region Events

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the event triggered when the selected node in the RadTreeView changes.
        /// </summary>
        /// <remarks>This method updates the enabled state of menu items based on whether the selected
        /// node is a root node or a child node. The <see cref="addGradeMenuItem"/> is enabled only for root nodes,
        /// while the <see cref="deleteGradeMenuItem"/> is enabled for non-root nodes. The <see cref="wizardMenuItem"/>
        /// is always enabled.</remarks>
        /// <param name="sender">The source of the event, typically the RadTreeView control.</param>
        /// <param name="e">An object containing event data, including the newly selected node.</param>
        private void radTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            var node = e.Node;
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

        private void SelectionDropDownList_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            //BoardDropDownList.Enabled = (SelectionDropDownList.SelectedIndex == 1);
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
            if (highestNode == selectedNode)
            {
                parentNode.Nodes.Remove(selectedNode);
                radTreeView1.SelectedNode = parentNode;
                ResetFields(false);
            }
            else
            {
                RadMessageBox.Show("Only the highest grade in this category can be deleted.", "Delete Grade", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
        }

        private void WizardMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the click event for the "Add Grade" menu item. 
        /// </summary>
        /// <remarks>This method is invoked when the user selects the "Add Grade" option from the menu.</remarks>
        /// <param name="sender">The source of the event, typically the menu item that was clicked.</param>
        /// <param name="e">An <see cref="EventArgs"/> instance containing the event data.</param>
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

            // Create new RankClassification
            var newGrade = new RankClassification
            {
                Type = rankType,
                PayGrade = newPayGrade,
                Selection = PayGradeSelection.Automatic
                // Other properties can be set to defaults or customized as needed
            };

            // Create new tree node and add to tree
            var newNode = new RadTreeNode(newGrade.ToString())
            {
                Tag = newGrade,
            };
            selectedNode.Nodes.Add(newNode);
            radTreeView1.SelectedNode = newNode;

            SelectClassification(newGrade);
        }

        #endregion

        private void RankGradeEditorForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
