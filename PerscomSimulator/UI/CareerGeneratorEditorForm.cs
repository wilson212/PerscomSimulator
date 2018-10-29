using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Perscom.Database;

namespace Perscom
{
    public partial class CareerGeneratorEditorForm : Form
    {
        private CareerGenerator Selected { get; set; }

        private List<CareerLengthRange> CareerLengths { get; set; } = new List<CareerLengthRange>();

        public CareerGeneratorEditorForm()
        {
            // Setup form controls
            InitializeComponent();

            // Set chart colors
            chart1.Series[0].BorderColor = FormStyling.LINE_COLOR_DARK;
            chart1.Series[0].Color = FormStyling.LINE_COLOR_LIGHT;
            chart1.ChartAreas[0].AxisY.Maximum = 1000;

            // Fill generators
            FillTree();

            // Disable fields
            ToggleFields(false);
        }

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleFields(bool enabled)
        {
            nameBox.Enabled = enabled;
            listView1.Enabled = enabled;
            saveButton.Enabled = enabled;
        }

        /// <summary>
        /// Fills the treeView with the existing ranks
        /// </summary>
        private void FillTree()
        {
            // Disable tree
            treeView1.Enabled = false;
            treeView1.BeginUpdate();

            // Always Clear Nodes first!
            treeView1.Nodes.Clear();

            // Add also to the "Copy From" submenu also!
            copyFromToolStripMenuItem.DropDownItems.Clear();

            // Load all ranks
            List<CareerGenerator> generators;
            using (AppDatabase db = new AppDatabase())
            {
                generators = new List<CareerGenerator>(db.CareerGenerators);
            }

            // Load and fill the rank trees
            foreach (var gen in generators)
            {
                TreeNode parent = new TreeNode(gen.Name);
                parent.Tag = gen;
                treeView1.Nodes.Add(parent);

                var item = new ToolStripMenuItem();
                item.Click += Item_Click;
                item.Tag = gen;
                item.Text = gen.Name;
                copyFromToolStripMenuItem.DropDownItems.Add(item);
            }

            // Enable tree
            treeView1.EndUpdate();
            treeView1.Enabled = true;
        }

        /// <summary>
        /// Event fired when a Career Generator is clicked from the "Copy From" drop down menu items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_Click(object sender, EventArgs e)
        {
            // Grab generator
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            CareerGenerator gen = clickedItem.Tag as CareerGenerator;

            // Ensure we arent copying from ourself!
            if (gen.Id == Selected.Id) return;

            // Copy career lengths
            CareerLengths = new List<CareerLengthRange>();
            foreach (var item in gen.CareerLengths)
            {
                var newLength = new CareerLengthRange()
                {
                    MaxCareerLength = item.MaxCareerLength,
                    MinCareerLength = item.MinCareerLength,
                    Probability = item.Probability,
                };
                CareerLengths.Add(newLength);
            }

            // Finally, Fill tree
            FillCareerLengths();
        }

        private void FillCareerLengths()
        {
            // Clear existing sub units
            listView1.BeginUpdate();
            listView1.Items.Clear();

            // Clear chart
            var series = chart1.Series[0];
            series.Points.Clear();
            //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
            //series.SmartLabelStyle.IsMarkerOverlappingAllowed = false;
            //series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.Right;

            // Get total probability
            int total = CareerLengths.Sum(x => x.Probability);

            // Fill Sub Units
            foreach (var rate in CareerLengths.OrderBy(x => x.MinCareerLength))
            {
                // Add rate to the list view
                ListViewItem item = new ListViewItem();
                item.Tag = rate;
                item.Text = rate.Probability.ToString();
                item.SubItems.Add(rate.StartString());
                item.SubItems.Add(rate.EndString());
                listView1.Items.Add(item);

                // Add rate to the chart
                int i = series.Points.AddXY(rate.MinCareerLength, total);
                DataPoint point = series.Points[i];
                point.AxisLabel = rate.ToString();
                point.LegendText = rate.ToString();
                point.Label = total.ToString();

                total -= rate.Probability;
            }

            // Re-draw listView
            listView1.EndUpdate();

            // Update label
            UpdateProbabiltyLabel();
        }

        private void UpdateProbabiltyLabel()
        {
            int total = CareerLengths.Sum(x => x.Probability);
            totalProbLabel.Text = total.ToString();
            totalProbLabel.ForeColor = (total == 1000) ? Color.Black : Color.Red;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Create a new Unit Template
            Selected = new CareerGenerator();
            CareerLengths.Clear();

            // Clear listViews and fields
            nameBox.Text = "";
            referencesLabel.Text = "0";

            // Clear spawn rates
            FillCareerLengths();

            // Enable fields
            ToggleFields(true);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Ignore
            if (Selected == null) return;

            // Perform validation!
            if (String.IsNullOrWhiteSpace(nameBox.Text))
            {
                ShowErrorMessage("Invalid generator name entered!");
                return;
            }

            // Open AppDatabase Connection
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                // Fill Rank Details
                Selected.Name = nameBox.Text;
                if (Selected.Id == 0)
                {
                    // Insert New
                    db.CareerGenerators.Add(Selected);

                    // Add spawn rates
                    foreach (var rate in CareerLengths)
                    {
                        rate.Generator = Selected;
                        db.CareerLengthRange.Add(rate);
                    }
                }
                else
                {
                    // Update
                    db.CareerGenerators.Update(Selected);

                    // Add, update and remove settings
                    var existing = Selected.CareerLengths.ToList();

                    // Remove
                    foreach (var item in existing.Except(CareerLengths))
                    {
                        item.Generator = Selected;
                        db.CareerLengthRange.Remove(item);
                    }

                    // Add
                    foreach (var item in CareerLengths.Except(existing))
                    {
                        item.Generator = Selected;
                        db.CareerLengthRange.Add(item);
                    }

                    // Update
                    foreach (var item in CareerLengths.Intersect(existing))
                    {
                        item.Generator = Selected;
                        db.CareerLengthRange.Update(item);
                    }
                }

                // Commit
                trans.Commit();
            }

            // Refill tree
            FillTree();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Ignore parent nodes
            if (e.Node == null || e.Node.Nodes.Count > 0) return;

            // Disable treeView and input fields
            treeView1.Enabled = false;
            ToggleFields(false);

            // Get selected node and generator
            var selected = e.Node;
            Selected = (CareerGenerator)selected.Tag;

            // Fill Details
            nameBox.Text = Selected.Name;
            CareerLengths = new List<CareerLengthRange>(Selected.CareerLengths);

            // Fill the spawn rates
            FillCareerLengths();

            // Enable fields
            ToggleFields(true);
            treeView1.Enabled = true;
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            // Ignore if the selected node is null
            TreeNode selected = treeView1.SelectedNode;
            removeGeneratorToolStripMenuItem.Enabled = (selected != null);
        }

        private void createNewGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a new Unit Template
            Selected = new CareerGenerator();
            CareerLengths.Clear();

            // Clear listViews and fields
            nameBox.Text = "";
            referencesLabel.Text = "0";

            // Clear spawn rates
            FillCareerLengths();

            // Enable fields
            ToggleFields(true);
        }

        private void removeGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ignore if the selected node is null
            TreeNode selected = treeView1.SelectedNode;
            if (selected == null) return;

            // Verify
            DialogResult res = MessageBox.Show(
                $"Are you sure you want to delete the Career Generator ({selected.Text})?",
                "Delete Generator", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                // Remove the Unit Template from the database
                var template = selected.Tag as CareerGenerator;

                // Get the number of child sets
                int adjustments = template.CareerAdjustments?.Count() ?? 0;
                int generators = template.GeneratorCareers?.Count() ?? 0;

                // Ensure there are no conflicts
                if (adjustments > 0)
                {
                    ShowErrorMessage($"Cannot remove Career Generator: {adjustments} soldier pools referencing this career");
                    return;
                }
                else if (generators > 0)
                {
                    ShowErrorMessage($"Cannot remove Career Generator: {generators} soldier generators referencing this career");
                    return;
                }

                using (AppDatabase db = new AppDatabase())
                {
                    db.CareerGenerators.Remove(template);
                }

                // Did we just delete the current open template?
                if (template.Equals(Selected))
                {
                    // Create a new Unit Template
                    Selected = new CareerGenerator();
                    CareerLengths.Clear();

                    // Clear listViews and fields
                    nameBox.Text = "";
                    referencesLabel.Text = "0";

                    // Clear spawn rates
                    FillCareerLengths();

                    // Disable fields
                    ToggleFields(false);
                }

                // Redraw treeView
                FillTree();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool enabled = listView1.SelectedItems.Count > 0;
            deleteToolStripMenuItem.Enabled = enabled;
            clearAllToolStripMenuItem.Enabled = enabled;
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Grab Billet from selected item tag
            var rate = new CareerLengthRange();

            // Get the selected billet
            using (var form = new CareerLengthEditorForm(rate))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Ensure this is not a duplicate setting
                    if (CareerLengths.Where(x => x.IsDuplicateOf(rate)).Any())
                    {
                        ShowErrorMessage("Selected spawn rate already exists in the pool!");
                        return;
                    }

                    CareerLengths.Add(rate);

                    // Refill List Views
                    FillCareerLengths();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            var selected = listView1.SelectedItems[0].Tag as CareerLengthRange;
            if (selected == null) return;

            // Now we must remove the matching Career Spawn Rate...
            // CareerLengthRange.Equals will not work in this case, since we overriden
            // the Equals method for comparing database Row ID's
            for (int i = CareerLengths.Count - 1; i >= 0; i--)
            {
                CareerLengthRange current = CareerLengths[i];
                if (current.IsDuplicateOf(selected))
                    CareerLengths.RemoveAt(i);
            }

            // Now redraw the listView
            FillCareerLengths();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Verify
            DialogResult res = MessageBox.Show(
                $"Are you sure you want to clear all the Career Spawn Rates?",
                "Clear Spawn Rates", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                CareerLengths.Clear();
                FillCareerLengths();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            var rate = listView1.SelectedItems[0].Tag as CareerLengthRange;
            if (rate == null) return;

            // Get the selected billet
            using (var form = new CareerLengthEditorForm(rate))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Refill List Views
                    FillCareerLengths();
                }
            }
        }

        private void copyTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #region Paint Events

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
    }
}
