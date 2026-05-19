using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class FactionEditorForm : RadForm
    {
        private Faction SelectedFaction { get; set; }

        public FactionEditorForm()
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);
            FormStyling.StyleButtonGreen(selectFlagButton);

            // Add event listeners
            addFactionMenuItem.Click += AddFactionMenuItem_Click;
            deleteFactionMenuItem.Click += DeleteFactionMenuItem_Click;

            // Attach the context menu to the tree view
            radContextMenuManager1.SetRadContextMenu(radTreeView1, radContextMenu1);

            // Wire up tree view selection
            radTreeView1.SelectedNodeChanged += RadTreeView1_SelectedNodeChanged;

            // Wire up save/cancel buttons
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;
            saveCommandBarButton.Click += SaveButton_Click;
            
            // Disable form elements until a faction is selected
            SetFormEnabled(false);

            // Load factions into the tree view
            LoadFactionTree();
        }

        /// <summary>
        /// Saves the currently selected faction's changes to the database
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (SelectedFaction == null)
            {
                RadMessageBox.Show("No faction selected to save.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            string name = radTextBox1.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                RadMessageBox.Show("Please enter a name for this Faction.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            string shortTag = radTextBox2.Text.Trim();
            if (string.IsNullOrEmpty(shortTag))
            {
                RadMessageBox.Show("Please enter a short tag for this Faction.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                SelectedFaction.Name = name;
                SelectedFaction.ShortTag = shortTag;
                SelectedFaction.Description = radTextBox3.Text.Trim();
                SelectedFaction.ThemeColorCode = ColorTranslator.ToHtml(radColorBox1.Value);

                db.Factions.Update(SelectedFaction);
                transaction.Commit();

                // Update the tree node text
                if (radTreeView1.SelectedNode != null)
                {
                    radTreeView1.SelectedNode.Text = name;
                }

                RadMessageBox.Show("Faction saved successfully.",
                    "Success", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to save faction: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Reload the selected faction to discard changes
            if (SelectedFaction != null)
            {
                LoadFactionIntoForm(SelectedFaction);
            }
        }

        private void AddFactionMenuItem_Click(object sender, EventArgs e)
        {
            CreateFaction();
        }

        private void DeleteFactionMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedFaction != null)
            {
                DeleteFaction(SelectedFaction);
            }
        }

        /// <summary>
        /// Adds the darker border line color between the header panel and the contents
        /// panel
        /// </summary>
        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooterDark(bottomPanel, e);
        }

        private void RankTileElement_Click(object sender, EventArgs e)
        {
            using (RankGradeEditorForm form = new RankGradeEditorForm(SelectedFaction))
            {
                form.ShowDialog(this);
            }
        }

        private void OccupationTileElement_Click(object sender, EventArgs e)
        {

        }

        private void TraitsTileElement_Click(object sender, EventArgs e)
        {

        }

        private void EvalBoardsTileElement_Click(object sender, EventArgs e)
        {

        }

        private void advisorButton_Click(object sender, EventArgs e)
        {
            AdvisorChatForm.Open(this);
        }

        #region Helper Functions

        /// <summary>
        /// Loads all factions from the database into the tree view
        /// </summary>
        private void LoadFactionTree()
        {
            radTreeView1.Nodes.Clear();

            try
            {
                using var db = new AppDatabase();
                foreach (var faction in db.Factions)
                {
                    var node = new RadTreeNode(faction.Name) { Tag = faction };
                    radTreeView1.Nodes.Add(node);
                }

                // Select the first node if available
                if (radTreeView1.Nodes.Count > 0)
                {
                    radTreeView1.SelectedNode = radTreeView1.Nodes[0];
                }
                else
                {
                    ClearForm();
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Enables or disables various controls and inputs in the form based on the specified flag.
        /// </summary>
        /// <param name="enabled">A boolean value indicating whether the controls should be enabled (true) or disabled (false).</param>
        private void SetFormEnabled(bool enabled)
        {
            // Basic detail inputs
            radTextBox1.Enabled = enabled;
            radTextBox2.Enabled = enabled;
            radTextBox3.Enabled = enabled;
            radColorBox1.Enabled = enabled;

            // Flag controls
            selectFlagButton.Enabled = enabled;
            flagPictureBox.Enabled = enabled;

            // Panorama tiles
            radPanorama1.Enabled = enabled;

            // Save/Cancel buttons
            saveButton.Enabled = enabled;
            cancelButton.Enabled = enabled;

            // Command bar buttons (save & delete, but not "Add New")
            saveCommandBarButton.Enabled = enabled;
            commandBarButton3.Enabled = enabled;

            // AI button
            advisorButton.Enabled = enabled;

            // Context menu delete item
            deleteFactionMenuItem.Enabled = enabled;
        }

        /// <summary>
        /// Populates the form controls with the selected faction's data
        /// </summary>
        private void LoadFactionIntoForm(Faction faction)
        {
            radTextBox1.Text = faction.Name;
            radTextBox2.Text = faction.ShortTag;
            radTextBox3.Text = faction.Description;

            // Load theme color
            if (!string.IsNullOrEmpty(faction.ThemeColorCode))
            {
                try
                {
                    radColorBox1.Value = ColorTranslator.FromHtml(faction.ThemeColorCode);
                }
                catch
                {
                    radColorBox1.Value = Color.Empty;
                }
            }
            else
            {
                radColorBox1.Value = Color.Empty;
            }

            // Load flag image
            if (!string.IsNullOrEmpty(faction.Image))
            {
                try
                {
                    flagPictureBox.Image = Image.FromFile(faction.Image);
                }
                catch
                {
                    flagPictureBox.Image = null;
                }
            }
            else
            {
                flagPictureBox.Image = null;
            }
            
            SetFormEnabled(true);
        }

        /// <summary>
        /// Clears all form fields when no faction is selected
        /// </summary>
        private void ClearForm()
        {
            SelectedFaction = null;
            radTextBox1.Text = string.Empty;
            radTextBox2.Text = string.Empty;
            radTextBox3.Text = string.Empty;
            radColorBox1.Value = Color.Empty;
            flagPictureBox.Image = null;
            SetFormEnabled(false);
        }

        private void DeleteFaction(Faction faction)
        {
            var result = RadMessageBox.Show(
                $"Are you sure you want to delete the faction \"{faction.Name}\"? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Question);

            if (result != DialogResult.Yes)
                return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                db.Factions.Remove(faction);
                transaction.Commit();

                // Reload the tree
                LoadFactionTree();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to delete faction: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void CreateFaction()
        {
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                var faction = db.Factions.Create();
                faction.Name = "New Faction";
                faction.ShortTag = "NEW";
                faction.Description = string.Empty;
                faction.ThemeColorCode = ColorTranslator.ToHtml(Color.Gray);

                db.Factions.Add(faction);
                transaction.Commit();

                // Reload tree and select the new faction
                LoadFactionTree();

                // Select the newly created node
                foreach (RadTreeNode node in radTreeView1.Nodes)
                {
                    if (node.Tag is Faction f && f.Id == faction.Id)
                    {
                        radTreeView1.SelectedNode = node;
                        AdvisorChatForm.SetFactionId(faction.Id);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to create faction: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        #endregion
        
        /// <summary>
        /// When a faction node is selected, populate the form fields
        /// </summary>
        private void RadTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node?.Tag is Faction faction)
            {
                SelectedFaction = faction;
                LoadFactionIntoForm(faction);
                AdvisorChatForm.SetFactionId(faction.Id);
            }
        }


        private void radCommandBar1_Click(object sender, EventArgs e)
        {
            
        }

        private void saveCommandBarButton_Click(object sender, EventArgs e)
        {
            // Save changes
            SaveButton_Click(sender, e);
        }

        private void commandBarButton2_Click(object sender, EventArgs e)
        {
            // Creates a new Faction
            CreateFaction();
        }

        private void commandBarButton3_Click(object sender, EventArgs e)
        {
            // Deletes the current faction
            if (SelectedFaction != null)
            {
                DeleteFaction(SelectedFaction);
            }
        }
    }
}