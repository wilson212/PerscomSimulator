using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public partial class RankEditorForm : Form
    {
        /// <summary>
        /// The current selected node's rank
        /// </summary>
        private Database.Rank SelectedRank { get; set; } = new Database.Rank();

        /// <summary>
        /// Creates a new instance of RankEditorForm
        /// </summary>
        public RankEditorForm()
        {
            // Setup form controls
            InitializeComponent();
            treeView1.Dock = DockStyle.Left;
            treeView1.Width = sidePanel.Width - (2 + sidePanel.Padding.Left);

            // Fill the rank tree
            FillTree();

            // Fill images
            FillComboBoxes();

            // Disable inputs
            ToggleFields(false);
        }

        /// <summary>
        /// Fills the treeView with the existing ranks
        /// </summary>
        private void FillTree()
        {
            // Disable tree
            treeView1.Enabled = false;
            treeView1.BeginUpdate();

            // Get a list of expanded trees
            var expanded = new Dictionary<RankType, bool>();
            foreach (var node in treeView1.Nodes)
            {
                TreeNode nd = (TreeNode)node;
                var type = (RankType)nd.Tag;
                expanded.Add(type, nd.IsExpanded);
            }

            // Always Clear Nodes first!
            treeView1.Nodes.Clear();

            // Load all ranks
            List<Database.Rank> ranks;
            using (AppDatabase db = new AppDatabase())
            {
                ranks = db.Ranks.ToList();
            }

            // Load and fill the rank trees
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                string name = Enum.GetName(typeof(RankType), type);
                TreeNode parent = new TreeNode(name);
                parent.Tag = type;

                var list = ranks.Where(x => x.Type == type)
                    .OrderByDescending(x => x.Grade)
                    .ThenByDescending(x => x.Precedence);

                // Add files in this directory
                foreach (var rank in list)
                {
                    TreeNode subNode = new TreeNode($"[{rank.Grade}] {rank.Name}");
                    subNode.Tag = rank;
                    parent.Nodes.Add(subNode);
                }

                // Was this expanded before?
                int id = treeView1.Nodes.Add(parent);
                if (expanded.ContainsKey(type) && expanded[type])
                    treeView1.Nodes[id].Expand();
            }

            // Enable tree
            treeView1.EndUpdate();
            treeView1.Enabled = true;
        }

        /// <summary>
        /// Called only on form initialization. Fills the rank comboboxes
        /// </summary>
        private void FillComboBoxes()
        {
            // Fill the rank types
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                rankTypeSelect.Items.Add(type);
            }

            // Load image directory
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Program.RootPath, "Images", "Large"));
            if (!directory.Exists)
                directory.Create();

            // Fill images
            imageSelect.Items.Add("None");
            foreach (var image in directory.GetFiles())
            {
                imageSelect.Items.Add(image.Name);
            }

            // select default rank type
            rankTypeSelect.SelectedIndex = 0;
        }

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleFields(bool enabled)
        {
            imageSelect.Enabled = enabled;
            rankTypeSelect.Enabled = enabled;
            rankNameBox.Enabled = enabled;
            rankAbbrBox.Enabled = enabled;
            rankGradeBox.Enabled = enabled;
            maxTigBox.Enabled = enabled;
            minTigBox.Enabled = enabled;
            promotableBox.Enabled = enabled;
            autoPromoteCheckBox.Enabled = enabled;
            applyButton.Enabled = enabled;
        }

        #region Panel Border Painting

        private void sidePanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormSidePanel(sidePanel, e);
            base.OnPaint(e);
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        #endregion

        #region Form Events

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Ignore
            if (SelectedRank == null) return;

            // Perform validation!
            else if (maxTigBox.Value > 0 && minTigBox.Value > maxTigBox.Value)
            {
                ShowErrorMessage("Minimum time in grade is greater than the Maximum!");
                return;
            }
            else if (maxTigBox.Value > 0 && promotableBox.Value >= maxTigBox.Value)
            {
                ShowErrorMessage("Promotable time in grade is greater or equal to the Maximum!");
                return;
            }
            else if (String.IsNullOrWhiteSpace(rankNameBox.Text))
            {
                ShowErrorMessage("Invalid rank name entered!");
                return;
            }

            using (AppDatabase db = new AppDatabase())
            {
                // Fill Rank Details
                SelectedRank.Type = (RankType)rankTypeSelect.SelectedItem;
                SelectedRank.Name = rankNameBox.Text;
                SelectedRank.Abbreviation = rankAbbrBox.Text;
                SelectedRank.Grade = (int)rankGradeBox.Value;
                SelectedRank.Precedence = (int)precedenceBox.Value;
                SelectedRank.Image = imageSelect.SelectedItem.ToString();
                SelectedRank.MaxTimeInGrade = (int)maxTigBox.Value;
                SelectedRank.MinTimeInGrade = (int)minTigBox.Value;
                SelectedRank.PromotableAt = (int)promotableBox.Value;
                SelectedRank.AutoPromote = autoPromoteCheckBox.Checked;

                if (SelectedRank.Id == 0)
                {
                    // Insert New
                    db.Ranks.Add(SelectedRank);
                }
                else
                {
                    // Update
                    db.Ranks.Update(SelectedRank);
                }

                // Reset Rank Cache!
                RankCache.Load(db);
            }

            // Refill tree
            FillTree();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Ignore parent nodes
            if (e.Node == null || e.Node.Nodes.Count > 0) return;

            // Disable treeView and input fields
            treeView1.Enabled = false;
            ToggleFields(false);

            // Context menu
            // Get selected node and rank
            var selected = e.Node;
            SelectedRank = selected.Tag as Rank;
            if (SelectedRank == null) return;

            // Get image index
            var imageIndex = imageSelect.Items.IndexOf(SelectedRank.Image);
            if (imageIndex < 0) imageIndex = 0;

            // Fill Rank Details
            rankTypeSelect.SelectedIndex = (int)SelectedRank.Type;
            imageSelect.SelectedIndex = imageIndex;
            rankNameBox.Text = SelectedRank.Name;
            rankAbbrBox.Text = SelectedRank.Abbreviation;
            rankGradeBox.Value = SelectedRank.Grade;
            precedenceBox.Value = SelectedRank.Precedence;
            maxTigBox.Value = SelectedRank.MaxTimeInGrade;
            minTigBox.Value = SelectedRank.MinTimeInGrade;
            promotableBox.Value = SelectedRank.PromotableAt;
            autoPromoteCheckBox.Checked = SelectedRank.AutoPromote;

            // Enable fields
            ToggleFields(true);

            treeView1.Enabled = true;
        }

        private void imageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
            // DONOT dispose image since it is a WeakRefernce!
            //

            // Set new image if it exists
            if (imageSelect.SelectedItem != null)
            {
                var image = ImageAccessor.GetImage(Path.Combine("large", imageSelect.SelectedItem.ToString()));
                rankPicture.Image = image;
            }
        }

        private void rankType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Reset
            SelectedRank = new Database.Rank();

            // Reset Rank Details
            imageSelect.SelectedIndex = 0;
            rankTypeSelect.SelectedIndex = 0;
            rankNameBox.Text = "";
            rankAbbrBox.Text = "";
            rankGradeBox.Value = 1;
            precedenceBox.Value = 1;
            maxTigBox.Value = 0;
            minTigBox.Value = 0;
            promotableBox.Value = 12;
            autoPromoteCheckBox.Checked = false;

            ToggleFields(true);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Ignore parent nodes
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Nodes.Count > 0) return;

            // Get selected node and rank
            var selected = treeView1.SelectedNode;
            SelectedRank = selected.Tag as Database.Rank;

            // Ignore if null
            if (SelectedRank == null) return;

            // Remove node
            treeView1.Nodes.Remove(selected);

            // Delete rank
            using (AppDatabase db = new AppDatabase())
            {
                db.Ranks.Remove(SelectedRank);
            }

            // Reset form
            newButton_Click(sender, e);
        }

        private void rankGradeBox_Enter(object sender, EventArgs e)
        {
            rankGradeBox.Select(0, rankGradeBox.Text.Length);
        }

        private void precedenceBox_Enter(object sender, EventArgs e)
        {
            precedenceBox.Select(0, precedenceBox.Text.Length);
        }

        private void minTigBox_Enter(object sender, EventArgs e)
        {
            minTigBox.Select(0, minTigBox.Text.Length);
        }

        private void maxTigBox_Enter(object sender, EventArgs e)
        {
            maxTigBox.Select(0, maxTigBox.Text.Length);
        }

        private void promotableBox_Enter(object sender, EventArgs e)
        {
            promotableBox.Select(0, promotableBox.Text.Length);
        }

        private void rankNameBox_Enter(object sender, EventArgs e)
        {

        }
    }

    #endregion
}
