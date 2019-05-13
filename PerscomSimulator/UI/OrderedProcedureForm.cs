using Perscom.Database;
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

namespace Perscom
{
    public partial class OrderedProcedureForm : Form
    {
        private OrderedProcedure SelectedProcedure { get; set; }

        private OrderedProcedureCareer SelectedNewCareer { get; set; }

        private List<OrderedPool> ProcedurePools { get; set; } = new List<OrderedPool>();

        private Dictionary<int, Rank> Ranks { get; set; }

        private List<CareerGenerator> CareerGens { get; set; }

        public OrderedProcedureForm()
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
                // Grab Career generators and Ranks
                CareerGens = new List<CareerGenerator>(db.CareerGenerators);
                Ranks = RankCache.RanksById;
                
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
                billetRankSelect.Items.Add(rank.Value);
                Bitmap picture = ImageAccessor.GetImage(Path.Combine("Large", rank.Value.Image)) ?? new Bitmap(64, 64);
                myImageList1.Images.Add(rank.Value.Image, picture);
            }
            listView1.LargeImageList = myImageList1;

            // Select default rank
            if (billetRankSelect.Items.Count > 0)
                billetRankSelect.SelectedIndex = 0;

            // Fill the procedure tree
            FillTree();

            // Disable inputs
            ToggleAllFields(false);
        }

        private void FillTree()
        {
            // Disable tree
            treeView1.Enabled = false;
            treeView1.BeginUpdate();

            // Always Clear Nodes first!
            treeView1.Nodes.Clear();

            // Load all ranks
            using (AppDatabase db = new AppDatabase())
            {
                // Load and fill the rank trees
                foreach (var procedure in db.OrderedProcedures)
                {
                    TreeNode parent = new TreeNode(procedure.Name);
                    parent.Tag = procedure;
                    treeView1.Nodes.Add(parent);
                }
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
            foreach (var setting in ProcedurePools)
            {
                // Clear old string
                builder.Clear();

                // Create listview item
                ListViewItem item = null;
                Rank rank = RankCache.RanksById[setting.RankId];
                if (setting.UseRankGrade)
                {
                    var ranks = RankCache.RanksByGrade[rank.Type][rank.Grade];
                    foreach (var r in ranks)
                    {
                        if (builder.Length > 0)
                            builder.Append(", ");
                        builder.Append(r.Name);
                    }

                    item = new ListViewItem(builder.ToString());
                    builder.Clear();
                }
                else
                {
                    item = new ListViewItem(rank.Name);
                }

                // Add probability
                item.SubItems.Add($"Probability: {setting.Probability}%");

                // Add career length change line
                if (setting.NewCareerLength)
                {
                    var name = setting.CareerGenerator?.Name;
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

                // Add filter clause
                if (setting.SoldierFilters != null && setting.SoldierFilters.Count() > 0)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append("Filtered");
                }

                // Add grouped clause
                if (setting.SoldierGroups != null && setting.SoldierGroups.Count() > 0)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append("Grouped");
                }

                // Add orderby clause
                if (setting.SoldierSorting != null && setting.SoldierSorting.Count() > 0)
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

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleAllFields(bool enabled)
        {
            nameBox.Enabled = enabled;
            listView1.Enabled = enabled;

            applyButton.Enabled = enabled;

            newSoldiersCheckBox.Enabled = enabled;
            newSoldierProbability.Enabled = (enabled && newSoldiersCheckBox.Checked) ? true : false;

            listView1.Enabled = enabled;
        }

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleNewFieldsOnly(bool enabled)
        {
            nameBox.Enabled = enabled;
            listView1.Enabled = false;

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
            ToggleAllFields(false);

            // Context menu
            // Get selected node and rank
            var selected = e.Node;
            SelectedProcedure = (OrderedProcedure)selected.Tag;

            // Fill Fields
            nameBox.Text = SelectedProcedure.Name;
            newSoldiersCheckBox.Checked = SelectedProcedure.CreatesNewSoldiers;
            newSoldierProbability.Value = SelectedProcedure.NewSoldierProbability;

            // Add generator settings
            ProcedurePools.Clear();
            ProcedurePools.AddRange(SelectedProcedure.ProcedurePools.OrderBy(x => x.Precedence));

            // Fill The ListView
            FillListView();

            // New soldier generator?
            if (SelectedProcedure.CreatesNewSoldiers)
            {
                // Find generator
                var item = SelectedProcedure.NewSoldierCareer?.FirstOrDefault();
                if (item != null && item != default(OrderedProcedureCareer))
                {
                    var careerId = item.CareerGeneratorId;
                    var index = CareerGens.FindIndex(x => x.Id == careerId);
                    if (index > -1)
                        careerGeneratorBox.SelectedIndex = index;
                }

                SelectedNewCareer = item;
            }

            // Enable fields
            ToggleAllFields(true);
            treeView1.Enabled = true;
        }


        private void newSoldiersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            newSoldierProbability.Enabled = newSoldiersCheckBox.Checked;
            careerGeneratorBox.Enabled = newSoldiersCheckBox.Checked;
            if (!newSoldiersCheckBox.Checked && newSoldierProbability.Value > 0)
            {
                // Setting value will raise event to update probability
                newSoldierProbability.Value = 0;
            }
        }

        private void newSoldierProbability_ValueChanged(object sender, EventArgs e)
        {
            newProbLabel.Text = $"{newSoldierProbability.Value}%";
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Create a new instance
            SelectedProcedure = new OrderedProcedure();
            ProcedurePools.Clear();

            // Reset field values
            nameBox.Text = "";
            newSoldiersCheckBox.Checked = true;
            newSoldierProbability.Value = 100;

            // Clear listView items and enable fields
            FillListView();
            ToggleAllFields(false);
            ToggleNewFieldsOnly(true);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Ignore parent nodes
            if (treeView1.SelectedNode == null) return;

            // Get selected node and rank
            var selected = treeView1.SelectedNode;
            SelectedProcedure = selected.Tag as OrderedProcedure;

            // Ignore if null
            if (SelectedProcedure == null) return;

            // TODO
            var billets = SelectedProcedure.Billets;
            if (billets != null)
            {
                int count = billets.Count();
                if (count > 0)
                {
                    ShowErrorMessage($"Cannot remove the selected procedure because it is used by {count} Billets!");
                    return;
                }
            }

            // Confirm
            string message = $"Are you sure you want to delete procedure \"{SelectedProcedure.Name}\"";
            var result = MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            // Delete generator
            using (AppDatabase db = new AppDatabase())
            {
                db.OrderedProcedures.Remove(SelectedProcedure);
            }

            // Remove node
            treeView1.Nodes.Remove(selected);

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
                ShowErrorMessage("Invalid procedure name entered!");
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

            // Disable fields while saving
            ToggleAllFields(false);

            // Update entity
            SelectedProcedure.RankId = ((Rank)billetRankSelect.SelectedItem).Id;
            SelectedProcedure.Name = nameBox.Text;
            SelectedProcedure.CreatesNewSoldiers = newSoldiersCheckBox.Checked;
            SelectedProcedure.NewSoldierProbability = newSoldierProbability.Value;

            // Updated entity in database
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    // Add or update record
                    if (SelectedProcedure.Id == 0)
                        db.OrderedProcedures.Add(SelectedProcedure);
                    else
                        db.OrderedProcedures.Update(SelectedProcedure);

                    // Update procedure pool precedences
                    int i = 0;
                    foreach (OrderedPool pool in ProcedurePools)
                    {
                        // Set values
                        pool.OrderedProcedureId = SelectedProcedure.Id;
                        pool.Precedence = i++;

                        // Update record
                        db.OrderedPools.Update(pool);
                    }

                    // Commit changes
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // STOP
                    trans.Rollback();

                    // Show exception and close form
                    ExceptionHandler.ShowException(ex);
                    this.Close();
                }
            }

            // Enable all fields
            ToggleAllFields(true);

            // Fill Tree
            FillTree();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            removeItemToolStripMenuItem.Enabled = (listView1.SelectedItems.Count > 0);
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

        private void addSoldierPoolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a new pool item
            var item = new OrderedPool();
            item.OrderedProcedure = SelectedProcedure;
            item.Precedence = listView1.Items.Count;

            // Open editor
            using (var form = new OrderedPoolForm(item))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Add item
                    ProcedurePools.Add(item);

                    // Re-draw listview
                    FillListView();
                }
            }
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Grab Billet from selected item tag
            var setting = listView1.SelectedItems[0].Tag as OrderedPool;
            if (setting == null) return;

            // Now we must remove the matching Settings...
            // SoldierGeneratorSetting.Equals will not work in this case, since we overriden
            // the Equals method for comparing database Row ID's
            for (int i = ProcedurePools.Count - 1; i >= 0; i--)
            {
                OrderedPool current = ProcedurePools[i];
                if (current.Id == setting.Id)
                    ProcedurePools.RemoveAt(i);
            }

            // Delete from database
            using (AppDatabase db = new AppDatabase())
            {
                db.OrderedPools.Remove(setting);
            }

            // Now redraw the listView
            FillListView();
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            // Clear old items
            ProcedurePools.Clear();

            // Re-order the internal list
            foreach (ListViewItem item in listView1.Items)
            {
                var pool = (OrderedPool)item.Tag;
                ProcedurePools.Add(pool);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            var setting = listView1.SelectedItems[0].Tag as OrderedPool;
            if (setting == null) return;

            using (var form = new OrderedPoolForm(setting))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Now redraw the listView
                    FillListView();
                }
            }
        }

        private void billetRankSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (billetRankSelect.SelectedIndex < 0) return;

            // Get selected rank
            Rank rank = (Rank)billetRankSelect.SelectedItem;

            // Set new image if it exists
            var image = ImageAccessor.GetImage(Path.Combine("large", rank.Image));
            rankPicture.Image = image;
        }
    }
}
