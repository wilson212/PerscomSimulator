using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public partial class SpecialtyEditorForm : Form
    {
        private Specialty Selected { get; set; }

        public SpecialtyEditorForm()
        {
            // Setup form controls
            InitializeComponent();
            treeView1.Dock = DockStyle.Left;
            treeView1.Width = sidePanel.Width - (2 + sidePanel.Padding.Left);

            // Fill the rank tree
            FillTree();

            // Fill the rank types
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                rankTypeSelect.Items.Add(type);
            }

            // select default rank type
            rankTypeSelect.SelectedIndex = 0;

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
            List<Specialty> specs;
            using (AppDatabase db = new AppDatabase())
            {
                specs = db.Specialties.ToList();
            }

            // Load and fill the rank trees
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                string name = Enum.GetName(typeof(RankType), type);
                TreeNode parent = new TreeNode(name);
                parent.Tag = type;

                var list = specs.Where(x => x.Type == type).OrderBy(x => x.Code);

                // Add files in this directory
                foreach (var rank in list)
                {
                    TreeNode subNode = new TreeNode(rank.ToString());
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
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleFields(bool enabled)
        {
            rankTypeSelect.Enabled = enabled;
            nameTextBox.Enabled = enabled;
            codeTextBox.Enabled = enabled;
            applyButton.Enabled = enabled;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Ignore
            if (Selected == null) return;

            // Check for validation errors
            if (rankTypeSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No rank type was selected!");
                return;
            }
            else if (String.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                ShowErrorMessage("Invalid specialty name entered!");
                return;
            }
            else if (!Regex.IsMatch(codeTextBox.Text, "[0-9]{2,4}[A-Z]{0,1}", RegexOptions.IgnoreCase))
            {
                ShowErrorMessage("Invalid specialty code entered!");
                return;
            }

            using (AppDatabase db = new AppDatabase())
            {
                // Fill Rank Details
                Selected.Type = (RankType)rankTypeSelect.SelectedItem;
                Selected.Name = nameTextBox.Text;
                Selected.Code = codeTextBox.Text;

                if (Selected.Id == 0)
                {
                    // Insert New
                    db.Specialties.Add(Selected);
                }
                else
                {
                    // Update
                    db.Specialties.Update(Selected);
                }
            }

            // Refill tree
            FillTree();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Reset
            Selected = new Specialty();

            // Reset Rank Details
            rankTypeSelect.SelectedIndex = 0;
            nameTextBox.Text = "";
            codeTextBox.Text = "";

            ToggleFields(true);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Ignore parent nodes
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Nodes.Count > 0) return;

            // Get selected node and rank
            var selected = treeView1.SelectedNode;
            Selected = selected.Tag as Specialty;

            // Ignore if null
            if (Selected == null) return;

            // Remove node
            treeView1.Nodes.Remove(selected);

            // Delete rank
            using (AppDatabase db = new AppDatabase())
            {
                db.Specialties.Remove(Selected);
            }

            // Reset form
            newButton_Click(sender, e);
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Ignore parent nodes
            if (e.Node == null || e.Node.Parent == null || e.Node.Nodes.Count > 0) return;

            // Disable treeView and input fields
            treeView1.Enabled = false;
            ToggleFields(false);

            // Context menu
            // Get selected node and rank
            var selected = e.Node;
            Selected = (Specialty)selected.Tag;

            // Fill Rank Details
            rankTypeSelect.SelectedIndex = (int)Selected.Type;
            nameTextBox.Text = Selected.Name;
            codeTextBox.Text = Selected.Code;

            // Enable fields
            ToggleFields(true);

            treeView1.Enabled = true;
        }
    }
}
