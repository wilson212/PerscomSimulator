using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Perscom.Database;

namespace Perscom
{
    public partial class SoldierGeneratorEditorForm : Form
    {
        private SoldierGenerator Selected { get; set; }

        private List<SoldierGeneratorSetting> Settings { get; set; } = new List<SoldierGeneratorSetting>();

        public SoldierGeneratorEditorForm()
        {
            // Setup form controls
            InitializeComponent();
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            treeView1.Dock = DockStyle.Left;
            treeView1.Width = sidePanel.Width - (2 + sidePanel.Padding.Left);

            // Fill Ranks
            using (AppDatabase db = new AppDatabase())
            {
                var ranks = db.Ranks.OrderBy(x => x.Type).ThenBy(x => x.Grade);
                foreach (var rank in ranks)
                {
                    rankSelect.Items.Add(rank);
                }

                if (ranks.Count() > 0)
                    rankSelect.SelectedIndex = 0;
            }

            // Fill the generator tree
            FillTree();

            // Disable inputs
            ToggleFields(false);
        }

        private void FillTree()
        {
            // Disable tree
            treeView1.Enabled = false;
            treeView1.BeginUpdate();

            // Always Clear Nodes first!
            treeView1.Nodes.Clear();

            // Load all ranks
            List<SoldierGenerator> generators;
            using (AppDatabase db = new AppDatabase())
            {
                generators = db.SoldierGenerators.ToList();
            }

            // Load and fill the rank trees
            foreach (var generator in generators)
            {
                TreeNode parent = new TreeNode(generator.Name);
                parent.Tag = generator;
                treeView1.Nodes.Add(parent);
            }

            // Enable tree
            treeView1.EndUpdate();
            treeView1.Enabled = true;
        }

        private void FillListView()
        {
            // Reset listview
            listView1.Items.Clear();

            // Quit here if this is a new Generator
            if (Selected == null || Selected.Id == 0)
                return;

            // Prepare update
            listView1.BeginUpdate();

            // Create ListView items
            foreach (var setting in Settings)
            {
                Rank rank = RankCache.RanksById[setting.RankId];
                ListViewItem item = new ListViewItem(rank.Name);
                item.SubItems.Add(setting.NewCareerLength ? "1" : "0");
                item.SubItems.Add(setting.MustBePromotable ? "1" : "0");
                item.SubItems.Add(setting.OrderedBySeniority ? "1" : "0");
                item.SubItems.Add(setting.NotLockedInBillet? "1" : "0");
                item.SubItems.Add(setting.Probability.ToString());
                item.Tag = setting;
                listView1.Items.Add(item);
            }

            // End update
            listView1.EndUpdate();
        }

        private void UpdateProbabiltyLabel()
        {
            int count = Settings.Sum(x => x.Probability);
            int combined = count + newTrackBar.Value;
            int total = (newCheckBox.Checked) ? combined : count;

            totalProbLabel.Text =  total.ToString();
            totalProbLabel.ForeColor = (total == 100) ? Color.Black : Color.Red;
        }

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleFields(bool enabled)
        {
            nameBox.Enabled = enabled;
            rankSelect.Enabled = enabled;
            listView1.Enabled = enabled;

            addButton.Enabled = enabled;
            applyButton.Enabled = enabled;

            existTrackBar.Enabled = enabled;
            lengthCheckBox.Enabled = enabled;
            promotableCheckBox.Enabled = enabled;
            orderedCheckBox.Enabled = enabled;
            lockedCheckBox.Enabled = enabled;

            newCheckBox.Enabled = enabled;
            newTrackBar.Enabled = (enabled && newCheckBox.Checked) ? true : false;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Ignore parent nodes
            if (e.Node == null) return;

            // Disable treeView and input fields
            treeView1.Enabled = false;
            ToggleFields(false);

            // Context menu
            // Get selected node and rank
            var selected = e.Node;
            Selected = (SoldierGenerator)selected.Tag;

            // Fill Fields
            nameBox.Text = Selected.Name;
            newCheckBox.Checked = Selected.CreatesNewSoldiers;
            newTrackBar.Value = Selected.NewSoldierProbability;

            // Add generator settings
            Settings.Clear();
            Settings.AddRange(Selected.SpawnSettings);

            // Fill The ListView
            FillListView();

            // Fill label
            UpdateProbabiltyLabel();

            // Enable fields
            ToggleFields(true);
            treeView1.Enabled = true;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Create a new instance
            Selected = new SoldierGenerator();
            Settings.Clear();

            // Reset field values
            nameBox.Text = "";
            newCheckBox.Checked = true;
            newTrackBar.Value = 100;
            lengthCheckBox.Checked = false;

            // Clear listView items and enable fields
            FillListView();
            UpdateProbabiltyLabel();
            ToggleFields(true);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Ignore parent nodes
            if (treeView1.SelectedNode == null) return;

            // Get selected node and rank
            var selected = treeView1.SelectedNode;
            Selected = selected.Tag as SoldierGenerator;

            // Ignore if null
            if (Selected == null) return;

            // Remove node
            treeView1.Nodes.Remove(selected);

            // Delete generator
            using (AppDatabase db = new AppDatabase())
            {
                db.SoldierGenerators.Remove(Selected);
            }

            // Update label
            totalProbLabel.Text = "0";
            totalProbLabel.ForeColor = Color.Red;

            // Reset form
            newButton_Click(sender, e);
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Check for validation errors
            if (String.IsNullOrWhiteSpace(nameBox.Text))
            {
                ShowErrorMessage("Invalid generator name entered!");
                return;
            }

            // Update entity
            Selected.Name = nameBox.Text;
            Selected.CreatesNewSoldiers = newCheckBox.Checked;
            Selected.NewSoldierProbability = newTrackBar.Value;

            // Make sure we have some sort of spawn ability
            int count = Settings.Sum(x => x.Probability);
            int combined = count + newTrackBar.Value;
            if ((!newCheckBox.Checked && count == 0) || combined == 0)
            {
                ShowErrorMessage("Generator has zero probability!");
                return;
            }

            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    // Add or update record
                    if (Selected.Id == 0)
                        db.SoldierGenerators.Add(Selected);
                    else
                        db.SoldierGenerators.Update(Selected);

                    // Add, update and remove settings
                    var existing = Selected.SpawnSettings.ToList();

                    // Remove
                    foreach (var item in existing.Except(Settings))
                    {
                        item.GeneratorId = Selected.Id;
                        db.SoldierGeneratorSettings.Remove(item);
                    }

                    // Add
                    foreach (var item in Settings.Except(existing))
                    {
                        item.GeneratorId = Selected.Id;
                        db.SoldierGeneratorSettings.Add(item);
                    }

                    // Update
                    foreach (var item in Settings.Intersect(existing))
                    {
                        item.GeneratorId = Selected.Id;
                        db.SoldierGeneratorSettings.Update(item);
                    }

                    // Commit
                    trans.Commit();

                    // Refill Tree
                    FillTree();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ExceptionHandler.ShowException(ex);
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // Ensure fields are selected!
            if (rankSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No rank was selected!");
                return;
            }
            else if (existTrackBar.Value == 0)
            {
                ShowErrorMessage("Probability must be greater than zero!");
                return;
            }

            // Add new Setting
            var setting = new SoldierGeneratorSetting();
            setting.GeneratorId = Selected.Id;
            setting.RankId = ((Rank)rankSelect.SelectedItem).Id;
            setting.Probability = existTrackBar.Value;
            setting.NewCareerLength = lengthCheckBox.Checked;
            setting.MustBePromotable = promotableCheckBox.Checked;
            setting.OrderedBySeniority = orderedCheckBox.Checked;
            setting.NotLockedInBillet = lockedCheckBox.Checked;

            // Ensure this is not a duplicate setting
            int count = Settings.Where(x => x.RankId == setting.RankId).Count();
            if (count > 0)
            {
                ShowErrorMessage("Selected rank already exists in the existing pool!");
                return;
            }

            // Add setting
            Settings.Add(setting);

            // Fill List view and Update label
            FillListView();
            UpdateProbabiltyLabel();
        }

        private void newCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            newTrackBar.Enabled = newCheckBox.Checked;
            if (!newCheckBox.Checked && newTrackBar.Value > 0)
            {
                // Setting value will raise event to update probability
                newTrackBar.Value = 0;
            }
        }

        private void newTrackBar_ValueChanged(object sender, EventArgs e)
        {
            newProbLabel.Text = $"{newTrackBar.Value}%";
            UpdateProbabiltyLabel();
        }

        private void existTrackBar_ValueChanged(object sender, EventArgs e)
        {
            existProbLabel.Text = $"{existTrackBar.Value}%";
            UpdateProbabiltyLabel();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            removeItemToolStripMenuItem.Enabled = (listView1.SelectedItems.Count > 0);
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Grab Billet from selected item tag
            var setting = listView1.SelectedItems[0].Tag as SoldierGeneratorSetting;
            if (setting == null) return;

            // Now we must remove the matching Settings...
            // SoldierGeneratorSetting.Equals will not work in this case, since we overriden
            // the Equals method for comparing database Row ID's
            for (int i = Settings.Count - 1; i >= 0; i--)
            {
                SoldierGeneratorSetting current = Settings[i];
                if (current.IsDuplicateOf(setting))
                    Settings.RemoveAt(i);
            }

            // Now redraw the listView
            FillListView();

            // Update probability label
            UpdateProbabiltyLabel();
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
