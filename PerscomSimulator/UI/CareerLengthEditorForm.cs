using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public partial class CareerLengthEditorForm : Form
    {
        private CareerSpawnRate SelectedRate { get; set; }

        public CareerLengthEditorForm()
        {
            // Setup form controls
            InitializeComponent();
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            treeView1.Dock = DockStyle.Left;
            treeView1.Width = sidePanel.Width - (2 + sidePanel.Padding.Left);

            // Fill the rank tree
            FillTree();

            // Fill Rank Types
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
                rankTypeSelect.Items.Add(type);

            rankTypeSelect.SelectedIndex = 0;

            // Disable inputs
            ToggleFields(false);
        }

        /// <summary>
        /// Fills the treeView with the existing probabilites
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
            List<CareerSpawnRate> rates;
            using (AppDatabase db = new AppDatabase())
            {
                rates = db.CareerSpawnRates.ToList();
            }

            // Keep track of the total spawn probability
            int total = 0;

            // Load and fill the rank trees
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                string name = Enum.GetName(typeof(RankType), type);
                TreeNode parent = new TreeNode(name);
                parent.Tag = type;

                var list = rates.Where(x => x.Type == type).OrderBy(x => x.MinCareerLength);

                // Add files in this directory
                foreach (var rate in list)
                {
                    TreeNode subNode = new TreeNode(rate.ToString());
                    subNode.Tag = rate;
                    parent.Nodes.Add(subNode);

                    // Do we add this probability
                    if (rankTypeSelect.SelectedIndex == (int)type)
                    {
                        total += rate.Probability;
                    }
                }

                // Was this expanded before?
                int id = treeView1.Nodes.Add(parent);
                if (expanded.ContainsKey(type) && expanded[type])
                    treeView1.Nodes[id].Expand();
            }

            // Update label
            totalProbLabel.Text = total.ToString();
            totalProbLabel.ForeColor = (total == 1000) ? Color.Black : Color.Red;

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
            probInput.Enabled = enabled;
            minInput.Enabled = enabled;
            maxInput.Enabled = enabled;
            applyButton.Enabled = enabled;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Reset
            SelectedRate = new CareerSpawnRate();

            // Reset Details
            probInput.Value = 1;
            minInput.Value = 1;
            maxInput.Value = 2;

            ToggleFields(true);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Ignore parent nodes
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Nodes.Count > 0) return;

            // Get selected node and rank
            var selected = treeView1.SelectedNode;
            SelectedRate = selected.Tag as CareerSpawnRate;

            // Ignore if null
            if (SelectedRate == null) return;

            // Remove node
            treeView1.Nodes.Remove(selected);

            // Update probability
            int total = Int32.Parse(totalProbLabel.Text);
            total -= SelectedRate.Probability;

            // Delete rank
            using (AppDatabase db = new AppDatabase())
            {
                db.CareerSpawnRates.Remove(SelectedRate);
            }

            // Update label
            totalProbLabel.Text = total.ToString();
            totalProbLabel.ForeColor = (total == 1000) ? Color.Black : Color.Red;

            // Reset form
            newButton_Click(sender, e);
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Ignore
            if (SelectedRate == null) return;

            using (AppDatabase db = new AppDatabase())
            {
                // Fill Rank Details
                SelectedRate.Type = (RankType)rankTypeSelect.SelectedItem;
                SelectedRate.Probability = (int)probInput.Value;
                SelectedRate.MinCareerLength = (int)minInput.Value;
                SelectedRate.MaxCareerLength = (int)maxInput.Value;

                if (SelectedRate.Id == 0)
                {
                    // Insert New
                    db.CareerSpawnRates.Add(SelectedRate);
                }
                else
                {
                    // Update
                    db.CareerSpawnRates.Update(SelectedRate);
                }
            }

            // Refill tree
            FillTree();
        }

        private void rankTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            rankTypeSelect.Enabled = false;

            // Fill cumulative rate
            int total = 0;
            TreeNode parent = treeView1.Nodes[(int)rankTypeSelect.SelectedItem];
            foreach (var node in parent.Nodes)
            {
                TreeNode n = (TreeNode)node;
                var rate = (CareerSpawnRate)n.Tag;
                total += rate.Probability;
            }

            // Update label
            totalProbLabel.Text = total.ToString();
            totalProbLabel.ForeColor = (total == 1000) ? Color.Black : Color.Red;

            rankTypeSelect.Enabled = true;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Ignore parent nodes
            if (e.Node == null || e.Node.Nodes.Count > 0 || e.Node.Parent == null) return;

            // Disable treeView and input fields
            treeView1.Enabled = false;
            ToggleFields(false);

            // Context menu
            // Get selected node and rank
            var selected = e.Node;
            SelectedRate = (CareerSpawnRate)selected.Tag;

            // Fill Rank Details
            rankTypeSelect.SelectedIndex = (int)SelectedRate.Type;
            probInput.Value = SelectedRate.Probability;
            minInput.Value = SelectedRate.MinCareerLength;
            maxInput.Value = SelectedRate.MaxCareerLength;

            // Enable fields
            ToggleFields(true);

            treeView1.Enabled = true;
        }

        #region Panel Border Painting

        private void sidePanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Gray, 1);

            // Create points that define line.
            Point point1 = new Point(sidePanel.Width - 1, 0);
            Point point2 = new Point(sidePanel.Width - 1, sidePanel.Height);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
            base.OnPaint(e);
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
            Pen greyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(greyPen, point1, point2);

            base.OnPaint(e);
        }

        #endregion
    }
}
