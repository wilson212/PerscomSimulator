using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perscom.Database;

namespace Perscom
{
    public partial class SoldierGeneratorEditorForm : Form
    {
        private SoldierGenerator SelectedGenerator { get; set; }

        private SoldierGeneratorCareer SelectedNewCareer { get; set; }

        private List<SoldierGeneratorPool> SpawnPools { get; set; } = new List<SoldierGeneratorPool>();

        private Dictionary<int, Rank> Ranks { get; set; }

        private List<CareerGenerator> CareerGens { get; set; }

        private int CurrentProbability { get; set; } = 0;

        public SoldierGeneratorEditorForm()
        {
            // Setup form controls
            InitializeComponent();
            treeView1.Dock = DockStyle.Left;
            treeView1.Width = sidePanel.Width - (2 + sidePanel.Padding.Left);

            // Initialize the tile size.
            listView1.TileSize = new Size(390, 64);

            // Fill Ranks and Career Generators
            using (AppDatabase db = new AppDatabase())
            {
                // Fill Ranks
                Ranks = db.Ranks.ToDictionary(x => x.Id, x => x);
                CareerGens = new List<CareerGenerator>(db.CareerGenerators);
            }

            // Fill drop down
            foreach (var length in CareerGens)
            {
                careerGeneratorBox.Items.Add(length);
            }

            if (CareerGens.Count > 0)
                careerGeneratorBox.SelectedIndex = 0;

            // Fill image list for Spawn Settings
            ImageList myImageList1 = new ImageList();
            myImageList1.ImageSize = new Size(64, 64);
            myImageList1.ColorDepth = ColorDepth.Depth32Bit;

            // Fill images
            foreach (var rank in Ranks)
            {
                Bitmap picture = ImageAccessor.GetImage(Path.Combine("Large", rank.Value.Image)) ?? new Bitmap(64, 64);
                myImageList1.Images.Add(rank.Value.Image, picture);
            }
            listView1.LargeImageList = myImageList1;

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

            // Prepare update
            listView1.BeginUpdate();

            var builder = new StringBuilder();

            // Create ListView items
            foreach (var setting in SpawnPools)
            {
                // Clear old string
                builder.Clear();

                // Create listview item
                Rank rank = RankCache.RanksById[setting.RankId];
                ListViewItem item = new ListViewItem(rank.Name);
                item.SubItems.Add($"Probability: {setting.Probability}%");

                // Add career length change line
                if (setting.NewCareerLength)
                {
                    var name = setting.TemporaryCareer?.Name ?? setting.CareerGenerator?.Name;
                    item.SubItems.Add("New Career Generator: " + name);
                }
                else
                {
                    item.SubItems.Add("No Career Length Changes");
                }

                // Add promotable flag
                if (setting.MustBePromotable)
                {
                    builder.Append("Must be Promtable");
                }

                // Add not locked in billet flag
                if (setting.NotLockedInBillet)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append("Not Locked In Billet");
                }

                // Add orderby clause
                if (setting.SoldierSorting != null && setting.SoldierSorting.Count() > 0)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append("Sorted");
                }
                else if (setting.TemporarySoldierSorting != null && setting.TemporarySoldierSorting.Count > 0)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append("Sorted");
                }

                // Add flag string to the 3rd subitem line
                if (builder.Length > 0)
                    item.SubItems.Add(builder.ToString());
                else
                    item.SubItems.Add("No Flags");

                item.ImageKey = Ranks[rank.Id].Image;
                item.Tag = setting;
                listView1.Items.Add(item);
            }
            

            // End update
            listView1.EndUpdate();
        }

        private void UpdateProbabiltyLabel()
        {
            int count = SpawnPools.Sum(x => x.Probability);
            int combined = count + newSoldierProbability.Value;
            int total = (newSoldiersCheckBox.Checked) ? combined : count;

            totalProbLabel.Text =  total.ToString();
            totalProbLabel.ForeColor = (total == 100) ? Color.Black : Color.Red;

            CurrentProbability = total;
        }

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleFields(bool enabled)
        {
            nameBox.Enabled = enabled;
            listView1.Enabled = enabled;

            applyButton.Enabled = enabled;

            newSoldiersCheckBox.Enabled = enabled;
            newSoldierProbability.Enabled = (enabled && newSoldiersCheckBox.Checked) ? true : false;
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
            SelectedGenerator = (SoldierGenerator)selected.Tag;

            // Fill Fields
            nameBox.Text = SelectedGenerator.Name;
            newSoldiersCheckBox.Checked = SelectedGenerator.CreatesNewSoldiers;
            newSoldierProbability.Value = SelectedGenerator.NewSoldierProbability;

            // Add generator settings
            SpawnPools.Clear();
            SpawnPools.AddRange(SelectedGenerator.SpawnPools);

            // Fill The ListView
            FillListView();

            // New soldier generator?
            if (SelectedGenerator.CreatesNewSoldiers)
            {
                // Find generator
                var item = SelectedGenerator.NewSoldierCareer?.FirstOrDefault();
                if (item != null && item != default(SoldierGeneratorCareer))
                {
                    var careerId = item.CareerGeneratorId;
                    var index = CareerGens.FindIndex(x => x.Id == careerId);
                    if (index > -1)
                        careerGeneratorBox.SelectedIndex = index;
                }

                SelectedNewCareer = item;
            }

            // Fill label
            UpdateProbabiltyLabel();

            // Enable fields
            ToggleFields(true);
            treeView1.Enabled = true;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Create a new instance
            SelectedGenerator = new SoldierGenerator();
            SpawnPools.Clear();

            // Reset field values
            nameBox.Text = "";
            newSoldiersCheckBox.Checked = true;
            newSoldierProbability.Value = 100;

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
            SelectedGenerator = selected.Tag as SoldierGenerator;

            // Ignore if null
            if (SelectedGenerator == null) return;

            // TODO
            var spawns = SelectedGenerator.BilletSpawns;
            if (spawns != null)
            {
                int count = spawns.Count();
                if (count > 0)
                {
                    ShowErrorMessage($"Cannot remove the selected Soldier Generator because it is used by {count} Billets!");
                    return;
                }
            }

            // Delete generator
            using (AppDatabase db = new AppDatabase())
            {
                db.SoldierGenerators.Remove(SelectedGenerator);
            }

            // Remove node
            treeView1.Nodes.Remove(selected);

            // Update label
            totalProbLabel.Text = "0";
            totalProbLabel.ForeColor = Color.Red;

            // Reset form
            newButton_Click(sender, e);

            // Fill tree!
            FillTree();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Check for validation errors
            if (String.IsNullOrWhiteSpace(nameBox.Text))
            {
                ShowErrorMessage("Invalid generator name entered!");
                return;
            }

            // Ensure we have a career selected!
            if (newSoldiersCheckBox.Checked)
            {
                if (careerGeneratorBox.SelectedIndex < 0)
                {
                    ShowErrorMessage("No Soldier Career Generator selected!");
                    return;
                }
            }

            // Make sure we have some sort of spawn ability
            int count = SpawnPools.Sum(x => x.Probability);
            int combined = count + newSoldierProbability.Value;
            if ((!newSoldiersCheckBox.Checked && count == 0) || combined == 0)
            {
                ShowErrorMessage("Generator has zero probability!");
                return;
            }

            // Update entity
            SelectedGenerator.Name = nameBox.Text;
            SelectedGenerator.CreatesNewSoldiers = newSoldiersCheckBox.Checked;
            SelectedGenerator.NewSoldierProbability = newSoldierProbability.Value;

            // Updated entity in database
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    // Add or update record
                    if (SelectedGenerator.Id == 0)
                        db.SoldierGenerators.Add(SelectedGenerator);
                    else
                        db.SoldierGenerators.Update(SelectedGenerator);

                    // Add, update and remove spawn pools
                    var existing = SelectedGenerator.SpawnPools.ToList();

                    // Remove spawn pools
                    foreach (var item in existing.Except(SpawnPools))
                    {
                        item.GeneratorId = SelectedGenerator.Id;
                        db.SoldierGeneratorPools.Remove(item);
                    }

                    // Add spawn pools
                    foreach (var item in SpawnPools.Except(existing))
                    {
                        item.GeneratorId = SelectedGenerator.Id;
                        db.SoldierGeneratorPools.Add(item);

                        // Do we need to add a new Career Adjustment?
                        if (item.TemporaryCareer != null && item.TemporaryCareer.Id > 0)
                        {
                            var temp = new SoldierCareerAdjustment()
                            {
                                SoldierGeneratorPool = item,
                                CareerGenerator = item.TemporaryCareer
                            };
                            db.SoldierCareerAdjustments.Add(temp);
                        }

                        // Do we need to add sorting?
                        if (item.TemporarySoldierSorting != null)
                        {
                            foreach (var sort in item.TemporarySoldierSorting)
                            {
                                sort.SoldierPool = item;
                                db.SoldierPoolSorting.Add(sort);
                            }
                        }
                    }

                    // Update spawn pools
                    foreach (var item in SpawnPools.Intersect(existing))
                    {
                        item.GeneratorId = SelectedGenerator.Id;
                        db.SoldierGeneratorPools.Update(item);

                        // Do we need to add a new Career Adjustment?
                        if (item.TemporaryCareer != null && item.TemporaryCareer.Id > 0)
                        {
                            var temp = new SoldierCareerAdjustment()
                            {
                                SoldierGeneratorPool = item,
                                CareerGenerator = item.TemporaryCareer
                            };
                            db.SoldierCareerAdjustments.AddOrUpdate(temp);
                        }
                        else if (item.TemporaryCareer == null)
                        {
                            // Delete just to be safe
                            db.Execute($"DELETE FROM `SoldierCareerAdjustment` WHERE `SoldierGeneratorPoolId`={item.Id}");
                        }

                        // Do we need to add sorting?
                        if (item.TemporarySoldierSorting != null)
                        {
                            // Remove all old ones!
                            db.Execute($"DELETE FROM `SoldierPoolSorting` WHERE `SoldierGeneratorPoolId`={item.Id}");

                            // Add new
                            foreach (var sort in item.TemporarySoldierSorting)
                            {
                                sort.SoldierPool = item;
                                db.SoldierPoolSorting.Add(sort);
                            }
                        }
                    }

                    // Change new soldier Career?
                    if (SelectedGenerator.CreatesNewSoldiers)
                    {
                        var career = CareerGens[careerGeneratorBox.SelectedIndex];
                        if (SelectedNewCareer == null)
                        {
                            // Add!
                            SelectedNewCareer = new SoldierGeneratorCareer
                            {
                                SoldierGenerator = SelectedGenerator,
                                CareerGenerator = career
                            };

                            db.SoldierGeneratorCareers.Add(SelectedNewCareer);
                        }
                        else if (career.Id != SelectedNewCareer.CareerGeneratorId)
                        {
                            // Update!
                            SelectedNewCareer.CareerGenerator = career;
                            db.SoldierGeneratorCareers.Update(SelectedNewCareer);
                        }
                    }
                    else if (SelectedNewCareer != null)
                    {
                        // Remove
                        db.SoldierGeneratorCareers.Remove(SelectedNewCareer);
                        SelectedNewCareer = null;
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
                    this.Close();
                }
            }
        }

        private void newCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            newSoldierProbability.Enabled = newSoldiersCheckBox.Checked;
            careerGeneratorBox.Enabled = newSoldiersCheckBox.Checked;
            if (!newSoldiersCheckBox.Checked && newSoldierProbability.Value > 0)
            {
                // Setting value will raise event to update probability
                newSoldierProbability.Value = 0;
            }
        }

        private void newTrackBar_ValueChanged(object sender, EventArgs e)
        {
            newProbLabel.Text = $"{newSoldierProbability.Value}%";
            UpdateProbabiltyLabel();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            removeItemToolStripMenuItem.Enabled = (listView1.SelectedItems.Count > 0);
        }

        private void addSoldierPoolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var setting = new SoldierGeneratorPool();
            using (var form = new SoldierGeneratorPoolForm(setting))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Ensure this is not a duplicate setting
                    if (SpawnPools.Where(x => x.RankId == setting.RankId).Any())
                    {
                        ShowErrorMessage("Selected rank already exists in the existing pool!");
                        return;
                    }

                    // Add setting
                    SpawnPools.Add(setting);

                    // Now redraw the listView
                    FillListView();

                    // Update probability label
                    UpdateProbabiltyLabel();
                }
            }
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Grab Billet from selected item tag
            var setting = listView1.SelectedItems[0].Tag as SoldierGeneratorPool;
            if (setting == null) return;

            // Now we must remove the matching Settings...
            // SoldierGeneratorSetting.Equals will not work in this case, since we overriden
            // the Equals method for comparing database Row ID's
            for (int i = SpawnPools.Count - 1; i >= 0; i--)
            {
                SoldierGeneratorPool current = SpawnPools[i];
                if (current.IsDuplicateOf(setting))
                    SpawnPools.RemoveAt(i);
            }

            // Now redraw the listView
            FillListView();

            // Update probability label
            UpdateProbabiltyLabel();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            var setting = listView1.SelectedItems[0].Tag as SoldierGeneratorPool;
            if (setting == null) return;

            using (var form = new SoldierGeneratorPoolForm(setting))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Now redraw the listView
                    FillListView();

                    // Update probability label
                    UpdateProbabiltyLabel();
                }
            }
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
    }
}
