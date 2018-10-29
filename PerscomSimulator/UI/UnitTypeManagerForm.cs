using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrossLite;
using CrossLite.QueryBuilder;
using Perscom.Database;

namespace Perscom
{
    public partial class UnitTypeManagerForm : Form
    {
        private UnitTemplate SelectedTemplate { get; set; } = new UnitTemplate();

        private List<Billet> Billets { get; set; } = new List<Billet>();

        private Dictionary<int, ListViewGroup> Groups = new Dictionary<int, ListViewGroup>();

        private Dictionary<int, Rank> Ranks { get; set; }

        /// <summary>
        /// UnitTemplate => Count
        /// </summary>
        private Dictionary<UnitTemplate, int> SubUnits { get; set; } = new Dictionary<UnitTemplate, int>();

        public UnitTypeManagerForm()
        {
            // Setup form controls
            InitializeComponent();
            treeView1.Dock = DockStyle.Left;
            treeView1.Width = sidePanel.Width - (2 + sidePanel.Padding.Left);

            // Initialize the tile size.
            listView2.TileSize = new Size(400, 64);

            // Disable fields
            ToggleFields(false);

            // Fill the echelon select box
            using (AppDatabase db = new AppDatabase())
            {
                // Fill the echelon select box
                var list = db.Echelons.Where(x => x.HierarchyLevel < 99).OrderBy(x => x.HierarchyLevel).ToArray();
                echelonTypeSelect.Items.AddRange(list);

                list = db.Echelons.OrderByDescending(x => x.HierarchyLevel).ToArray();
                promotionPoolSelect.Items.AddRange(list);

                // Set drop downs to an option
                echelonTypeSelect.SelectedIndex = 0;
                promotionPoolSelect.SelectedIndex = 0;

                // Fill Ranks
                Ranks = db.Ranks.ToDictionary(x => x.Id, x => x);

                //
                foreach (var cat in db.BilletCatagories.OrderByDescending(x => x.ZIndex))
                {
                    var group = new ListViewGroup(cat.Name);
                    group.Tag = cat;
                    listView2.Groups.Add(group);
                    Groups.Add(cat.Id, group);
                }
            }

            // Fill image list for Billets
            ImageList myImageList1 = new ImageList();
            myImageList1.ImageSize = new Size(64, 64);
            myImageList1.ColorDepth = ColorDepth.Depth32Bit;

            // Fill images
            foreach (var rank in Ranks)
            {
                Bitmap picture = ImageAccessor.GetImage(Path.Combine("Large", rank.Value.Image)) ?? new Bitmap(64, 64);
                myImageList1.Images.Add(rank.Value.Image, picture);
            }
            listView2.LargeImageList = myImageList1;

            // Fill treeView
            FillTree();
            treeView1.Scrollable = true;
        }

        /// <summary>
        /// Fills the Billets listView
        /// </summary>
        private void FillBilletsListView()
        {
            // Clear existing Billets
            listView2.Enabled = false;
            listView2.BeginUpdate();
            listView2.Items.Clear();

            // Clear billits from groups
            foreach (var item in Groups)
            {
                item.Value.Items.Clear();
            }

            // Fill Billets
            foreach (var billet in Billets.OrderByDescending(x => x.ZIndex))
            {
                // Add billit to listViewGroup
                ListViewItem item = new ListViewItem();
                item.Tag = billet;
                item.Text = billet.Name;
                item.ImageKey = Ranks[billet.RankId].Image;
                Groups[billet.BilletCatagoryId].Items.Add(item);

                // Add to list next
                listView2.Items.Add(item);
            }

            // Re-draw listView
            listView2.EndUpdate();
            listView2.Enabled = true;
        }

        /// <summary>
        /// Fills the Sub Unit listView
        /// </summary>
        private void FillUnitsListView()
        {
            // Clear existing sub units
            listView1.BeginUpdate();
            listView1.Items.Clear();

            // Fill Sub Units
            foreach (var unit in SubUnits)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = unit;
                item.Text = unit.Key.Name;
                item.SubItems.Add(unit.Value.ToString());
                listView1.Items.Add(item);
            }


            // Re-draw listView
            listView1.EndUpdate();
        }

        /// <summary>
        /// Fills the treeView with the existing ranks
        /// </summary>
        private void FillTree()
        {
            // Disable tree
            treeView1.Enabled = false;
            treeView1.BeginUpdate();

            // Get a list of expanded trees, so we can
            // re-expand them after re-loading the treeView
            var expanded = new Dictionary<int, bool>(treeView1.Nodes.Count);
            foreach (var node in treeView1.Nodes)
            {
                TreeNode nd = (TreeNode)node;
                var type = (Echelon)nd.Tag;
                expanded.Add(type.Id, nd.IsExpanded);
            }

            // Always Clear Nodes first!
            treeView1.Nodes.Clear();

            // Load all ranks
            List<Echelon> echelons;
            using (AppDatabase db = new AppDatabase())
            {
                echelons = db.Echelons
                    .Where(x => x.HierarchyLevel < 99 && x.UnitTypes.Count() > 0)
                    .OrderByDescending(x => x.HierarchyLevel)
                    .ToList();
            }

            // Load and fill the rank trees
            foreach (Echelon e in echelons)
            {
                TreeNode parent = new TreeNode(e.Name);
                parent.Tag = e;

                // Add echelon UnitTemplate's to the treeView
                foreach (var template in e.UnitTypes)
                {
                    // Skip units that only use this echelon as a promotion pool
                    if (template.EchelonId != e.Id) continue;

                    TreeNode subNode = new TreeNode(template.Name);
                    subNode.Tag = template;
                    parent.Nodes.Add(subNode);
                }

                // Expand each treeNode that was extended before this redraw
                int id = treeView1.Nodes.Add(parent);
                if (expanded.ContainsKey(e.Id) && expanded[e.Id])
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
            echelonTypeSelect.Enabled = enabled;
            templateNameBox.Enabled = enabled;
            unitNameBox.Enabled = enabled;
            shortNameBox.Enabled = enabled;
            promotionPoolSelect.Enabled = enabled;
            listView1.Enabled = enabled;
            listView2.Enabled = enabled;
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

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion

        #region Button Events

        /// <summary>
        /// New button click event
        /// </summary>
        private void newButton_Click(object sender, EventArgs e)
        {
            // Create a new Unit Template
            SelectedTemplate = new UnitTemplate();
            Billets.Clear();
            SubUnits.Clear();

            // Clear listViews and fields
            templateNameBox.Text = "";
            unitNameBox.Text = "";
            referencesLabel.Text = "0";
            FillBilletsListView();
            FillUnitsListView();

            // Enable fields
            ToggleFields(true);

            // Prepare form for a new UnitTemplate.
            listView1.Enabled = false;
            listView2.Enabled = false;
            applyButton.Text = "Create Unit Template";
        }

        /// <summary>
        /// Delete button click event. Deletes the current selected
        /// UnitTemplate from the treeView
        /// </summary>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Remove treeNode
            TreeNode selected = treeView1.SelectedNode;

            // Ignore if the selected node has children or is null
            if (selected == null || selected.Nodes.Count > 0 || selected.Parent == null)
                return;

            // Verify
            DialogResult res = MessageBox.Show(
                $"Are you sure you want to delete the Unit Template ({selected.Text})?",
                "Delete Unit template", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                // Remove the Unit Template from the database
                var template = selected.Tag as UnitTemplate;
                using (AppDatabase db = new AppDatabase())
                {
                    db.UnitTemplates.Remove(template);
                }

                // Did we just delete the current open template?
                if (template.Equals(SelectedTemplate))
                {
                    // Create a new Unit Template
                    SelectedTemplate = new UnitTemplate();
                    Billets.Clear();
                    SubUnits.Clear();

                    // Clear listViews and fields
                    templateNameBox.Text = "";
                    unitNameBox.Text = "";
                    referencesLabel.Text = "0";
                    FillBilletsListView();
                    FillUnitsListView();

                    // Disable fields
                    ToggleFields(false);

                    // Prepare form for a new UnitTemplate.
                    applyButton.Text = "Create Unit Template";
                }

                // Redraw treeView
                FillTree();
            }
        }

        /// <summary>
        /// Apply button click event. Saves all changes to the current selected
        /// <see cref="UnitTemplate"/>
        /// </summary>
        private void applyButton_Click(object sender, EventArgs e)
        {
            // Check for validation errors
            if (echelonTypeSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No echelon rank was selected!");
                return;
            }
            else if (String.IsNullOrWhiteSpace(templateNameBox.Text))
            {
                ShowErrorMessage("Invalid template name entered!");
                return;
            }
            else if (String.IsNullOrWhiteSpace(unitNameBox.Text))
            {
                ShowErrorMessage("Invalid default unit name entered!");
                return;
            }

            // Disable appy button
            applyButton.Enabled = false;

            // Fill template details
            Echelon echelon = (Echelon)echelonTypeSelect.SelectedItem;
            Echelon pool = (Echelon)promotionPoolSelect.SelectedItem;
            SelectedTemplate.Echelon = echelon;
            SelectedTemplate.Name = templateNameBox.Text;
            SelectedTemplate.UnitNameFormat = unitNameBox.Text;
            SelectedTemplate.UnitCodeFormat = shortNameBox.Text;
            SelectedTemplate.PromotionPool = pool;

            // Ensure that no sub units are of higher echelon that our current unit,
            // as to prevent endless loops
            var items = SubUnits
                .Select(x => x.Key)
                .Where(x => x.Echelon.HierarchyLevel > echelon.HierarchyLevel)
                .ToArray();

            // Alert the user if any sub units are a higher level
            if (items.Count() > 0)
            {
                ShowErrorMessage($"A sub unit template ({items[0].Name}) has a higher hierarchy level that the current template!");
                return;
            }

            // Open connection, and start a transaction
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    // Update or Insert data
                    if (SelectedTemplate.Id == 0)
                    {
                        // New Template
                        db.UnitTemplates.Add(SelectedTemplate);

                        // Add sub units
                        foreach (var unit in SubUnits)
                        {
                            var item = new UnitTemplateAttachment();
                            item.ParentId = SelectedTemplate.Id;
                            item.ChildId = unit.Key.Id;
                            item.Count = unit.Value;
                            db.UnitTypeAttachments.Add(item);
                        }
                    }
                    else
                    {
                        // Update Existing Template
                        db.UnitTemplates.Update(SelectedTemplate);

                        // Get a list of sub units that do not exist in the current list
                        var formVals = SubUnits.Select(x => x.Key).ToList();
                        var current = SelectedTemplate.UnitTemplateAttachments.Select(x => x.Child).ToList();

                        // Add the added sub units, if any
                        foreach (var item in formVals.Except(current))
                        {
                            var attachement = new UnitTemplateAttachment();
                            attachement.ParentId = SelectedTemplate.Id;
                            attachement.ChildId = item.Id;
                            attachement.Count = SubUnits[item];
                            db.UnitTypeAttachments.Add(attachement);
                        }

                        // Remove the sub units that were removed
                        foreach (var item in current.Except(formVals))
                        {
                            var query = new DeleteQueryBuilder(db);
                            query.From(nameof(UnitTemplateAttachment))
                                .Where("ParentId", Comparison.Equals, SelectedTemplate.Id)
                                .And("ChildId", Comparison.Equals, item.Id);
                            query.Execute();
                        }

                        // Update existing
                        foreach (var item in current.Intersect(formVals))
                        {
                            var query = new UpdateQueryBuilder(nameof(UnitTemplateAttachment), db);
                            query.Set("Count", SubUnits[item])
                                .Where("ParentId", Comparison.Equals, SelectedTemplate.Id)
                                .And("ChildId", Comparison.Equals, item.Id);
                            query.Execute();
                        }
                    }

                    // Apply changes
                    trans.Commit();

                    // Update treeView
                    FillTree();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ExceptionHandler.ShowException(ex);
                }
            }

            // Enable appy button
            applyButton.Enabled = true;
            applyButton.Text = "Apply Changes";
            listView1.Enabled = true;
            listView2.Enabled = true;
        }

        #endregion Button Events

        #region Context Menu Events

        private void addBilletMenuItem_Click(object sender, EventArgs e)
        {
            Billet newBillet = new Billet();
            using (var form = new BilletEditorForm(SelectedTemplate, newBillet))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Add the new Billet to the list
                    Billets.Add(newBillet);

                    // Refill List Views
                    FillBilletsListView();
                }
            }
        }

        private void removeBilletMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView2.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            var billet = listView2.SelectedItems[0].Tag as Billet;
            if (billet == null) return;

            // Remove billet from both list, and database!
            using (AppDatabase db = new AppDatabase())
            {
                Billets.Remove(billet);
                db.Billets.Remove(billet);
            }

            // Now redraw the billets listView
            FillBilletsListView();
        }

        private void duplicateBilletMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView2.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            var billet = listView2.SelectedItems[0].Tag as Billet;
            if (billet == null) return;

            try
            {
                // Disable menu
                billetsContextMenu.Enabled = false;

                // Update table cache
                EntityCache.GetTableMap(typeof(BilletRequirement)).BuildInstanceForeignKeys = false;
                EntityCache.GetTableMap(typeof(BilletSpawnSetting)).BuildInstanceForeignKeys = false;
                EntityCache.GetTableMap(typeof(BilletSpecialty)).BuildInstanceForeignKeys = false;

                // Open database and begin transaction
                using (AppDatabase db = new AppDatabase())
                using (var trans = db.BeginTransaction())
                {
                    // Copy billet
                    DuplicateBillet(db, billet);

                    // Commit changes
                    trans.Commit();

                    // Update billet view
                    FillBilletsListView();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.ShowException(ex);
            }
            finally
            {
                // Enable menu
                billetsContextMenu.Enabled = true;

                // Update table cache
                EntityCache.GetTableMap(typeof(BilletRequirement)).BuildInstanceForeignKeys = true;
                EntityCache.GetTableMap(typeof(BilletSpawnSetting)).BuildInstanceForeignKeys = true;
                EntityCache.GetTableMap(typeof(BilletSpecialty)).BuildInstanceForeignKeys = true;
            }
        }

        private void adjustUnitCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;
        }

        private void removeSubUnitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;

            // Grab UnitTemplate from selected item tag
            var template = (KeyValuePair<UnitTemplate, int>)listView1.SelectedItems[0].Tag;

            // Remove the unit tempalate from the list
            SubUnits.Remove(template.Key);

            // Now redraw the sub units listView
            FillUnitsListView();
        }

        #endregion Context Menu Events

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            // We only allow TreeNodes to be dragged here
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            // Grab the UnitTemplate from the node
            UnitTemplate template = draggedNode.Tag as UnitTemplate;
            if (template == null)
                return;

            // Ensure we are not just copying ourselves. This could cause infinite loops.
            if (template.Equals(SelectedTemplate))
            {
                ShowErrorMessage("You cannot place the same unit template under itself!");
                return;
            }
            

            // Add or update the sub unit count
            if (SubUnits.ContainsKey(template))
            {
                SubUnits[template] += 1;
            }
            else
            {
                SubUnits.Add(template, 1);
            }

            FillUnitsListView();
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Skip if no nodes are selected!
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Nodes.Count > 0 || treeView1.SelectedNode.Parent == null)
            {
                return;
            }

            treeView1.DoDragDrop(e.Item, DragDropEffects.Copy);
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Ignore parent nodes
            if (e.Node == null || e.Node.Nodes.Count > 0) return;

            // Disable treeView and input fields
            treeView1.Enabled = false;
            ToggleFields(false);

            // Get selected node and rank
            var selected = e.Node;
            SelectedTemplate = (UnitTemplate)selected.Tag;

            // Get echelon index
            int index = echelonTypeSelect.Items.IndexOf(SelectedTemplate.Echelon);

            // Fill Template Details
            echelonTypeSelect.SelectedIndex = (index < 0) ? 0 : index; ;
            templateNameBox.Text = SelectedTemplate.Name;
            unitNameBox.Text = SelectedTemplate.UnitNameFormat;
            shortNameBox.Text = SelectedTemplate.UnitCodeFormat;
            referencesLabel.Text = SelectedTemplate.UnitTemplateAttachments.Count().ToString();

            // Get promotion pool index
            index = promotionPoolSelect.Items.IndexOf(SelectedTemplate.PromotionPool);
            promotionPoolSelect.SelectedIndex = (index < 0) ? 0 : index;

            // Add Sub Units
            Billets.Clear();
            Billets.AddRange(SelectedTemplate.Billets);

            // Add Billets
            SubUnits.Clear();
            foreach (var unit in SelectedTemplate.UnitTemplateAttachments)
            {
                if (unit.ParentId == SelectedTemplate.Id)
                    SubUnits.Add(unit.Child, unit.Count);
            }

            // Fill List Views
            FillBilletsListView();
            FillUnitsListView();
            RefreshBilletCopyMenuItems();

            // Enable fields
            ToggleFields(true);
            applyButton.Text = "Apply Changes";

            treeView1.Enabled = true;
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView2.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            var billet = listView2.SelectedItems[0].Tag as Billet;
            if (billet == null) return;

            // Get the selected billet

            using (var form = new BilletEditorForm(SelectedTemplate, billet))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Refill List Views
                    FillBilletsListView();
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Enables items on the billet context menu
        /// </summary>
        private void billetsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            removeBilletMenuItem.Enabled = (listView2.SelectedItems.Count > 0);
            duplicateBilletMenuItem.Enabled = (listView2.SelectedItems.Count > 0);
        }

        /// <summary>
        /// Enables items on the sub units context menu
        /// </summary>
        private void subUnitsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            adjustUnitCountToolStripMenuItem.Enabled = (listView1.SelectedItems.Count > 0);
            removeSubUnitToolStripMenuItem.Enabled = (listView1.SelectedItems.Count > 0);
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (largeIconsToolStripMenuItem.Checked) return;

            largeIconsToolStripMenuItem.Checked = true;
            tilesToolStripMenuItem.Checked = false;
            listView2.View = View.LargeIcon;
        }

        private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tilesToolStripMenuItem.Checked) return;

            tilesToolStripMenuItem.Checked = true;
            largeIconsToolStripMenuItem.Checked = false;
            listView2.View = View.Tile;
        }

        private void finalizeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void echelonTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Refresh items
            RefreshBilletCopyMenuItems();
        }

        private void RefreshBilletCopyMenuItems()
        {
            // Grab selected echelon
            Echelon echelon = (Echelon)echelonTypeSelect.SelectedItem;
            if (echelon == null) return;

            // Clear old items
            copyFromMenuItem.DropDownItems.Clear();
            copyFromMenuItem.Enabled = false;

            using (AppDatabase db = new AppDatabase())
            {
                // Fetch all Unit Templates from this echelon
                var builder = new SelectQueryBuilder(db);
                builder.From(nameof(UnitTemplate))
                    .SelectAll()
                    .Where(nameof(UnitTemplate.EchelonId), Comparison.Equals, echelon.Id);

                // Add UnitTemplate as a menu item
                foreach (var ec in builder.ExecuteQuery<UnitTemplate>())
                {
                    // Dont add ourselves!
                    if (SelectedTemplate.Id == ec.Id)
                        continue;

                    // Add the template as an option
                    ToolStripMenuItem item = new ToolStripMenuItem(ec.Name);
                    item.Tag = ec;
                    item.Click += Item_Click;
                    copyFromMenuItem.DropDownItems.Add(item);
                }
            }

            // Only enable if we have items
            if (copyFromMenuItem.DropDownItems.Count > 0)
                copyFromMenuItem.Enabled = true;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            // Grab menu item
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem == null) return;

            // Grab template
            UnitTemplate template = menuItem.Tag as UnitTemplate;
            if (template == null) return;

            // As user to verify
            var res = MessageBox.Show(
                $"Are you sure you want to copy all billets from {template.Name}? This will clear all current billets!",
                "Verify", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
            );
            if (res != DialogResult.Yes) return;

            try
            {
                // Disable menu
                billetsContextMenu.Enabled = false;

                // Update table cache
                EntityCache.GetTableMap(typeof(BilletRequirement)).BuildInstanceForeignKeys = false;
                EntityCache.GetTableMap(typeof(BilletSpawnSetting)).BuildInstanceForeignKeys = false;
                EntityCache.GetTableMap(typeof(BilletSpecialty)).BuildInstanceForeignKeys = false;

                // Open database and begin transaction
                using (AppDatabase db = new AppDatabase())
                using (var trans = db.BeginTransaction())
                {
                    // Remove old billets
                    foreach (var b in Billets)
                    {
                        db.Billets.Remove(b);
                    }
                    Billets.Clear();

                    // Copy all billets
                    foreach (var billet in template.Billets)
                    {
                        DuplicateBillet(db, billet);
                    }

                    // Commit changes
                    trans.Commit();

                    // Update billet view
                    FillBilletsListView();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.ShowException(ex);
            }
            finally
            {
                // Enable menu
                billetsContextMenu.Enabled = true;

                // Update table cache
                EntityCache.GetTableMap(typeof(BilletRequirement)).BuildInstanceForeignKeys = true;
                EntityCache.GetTableMap(typeof(BilletSpawnSetting)).BuildInstanceForeignKeys = true;
                EntityCache.GetTableMap(typeof(BilletSpecialty)).BuildInstanceForeignKeys = true;
            }
        }

        /// <summary>
        /// Creates a duplicated of the specified billet, and adds it to the database
        /// </summary>
        /// <param name="db"></param>
        /// <param name="billet"></param>
        private void DuplicateBillet(AppDatabase db, Billet billet)
        {
            // Create new copy of billet
            Billet b = new Billet()
            {
                BilletCatagoryId = billet.BilletCatagoryId,
                CanRetireEarly = billet.CanRetireEarly,
                InverseRequirements = billet.InverseRequirements,
                LateralOnly = billet.LateralOnly,
                MaxRankId = billet.MaxRankId,
                MaxTourLength = billet.MaxTourLength,
                MinTourLength = billet.MinTourLength,
                Name = billet.Name,
                PreferNonRepeats = billet.PreferNonRepeats,
                PromotionPoolId = billet.PromotionPoolId,
                RankId = billet.RankId,
                Repeatable = billet.Repeatable,
                Stature = billet.Stature,
                UnitTypeId = SelectedTemplate.Id,
                ZIndex = billet.ZIndex
            };

            // Add billet to database
            db.Billets.Add(b);

            // Add billet requirements
            foreach (var item in billet.Requirements)
            {
                var req = new BilletRequirement()
                {
                    BilletId = b.Id,
                    SpecialtyId = item.SpecialtyId
                };
                db.BilletRequirements.Add(req);
            }

            // Add billet spawn settings
            foreach (var item in billet.SpawnSettings)
            {
                var req = new BilletSpawnSetting()
                {
                    Billet = b,
                    GeneratorId = item.GeneratorId,
                    SpecialtyId = item.SpecialtyId
                };
                db.BilletSpawnSettings.Add(req);
            }

            // Add billet specialties
            foreach (var item in billet.Specialties)
            {
                var spec = new BilletSpecialty()
                {
                    Billet = b,
                    SpecialtyId = item.SpecialtyId
                };
                db.BilletSpecialties.Add(spec);
            }

            // Add billet to internal list
            Billets.Add(b);
        }
    }
}
