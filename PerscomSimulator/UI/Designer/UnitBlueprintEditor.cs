using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class UnitBlueprintEditor : RadForm
    {
        /// <summary>
        /// Stores a snapshot of the form field values at the time of load or last Apply.
        /// Used to detect unsaved changes when the user tries to navigate away.
        /// </summary>
        private class FormSnapshot
        {
            public string Name;
            public string UnitNameFormat;
            public string UnitCodeFormat;
            public int PromotionPoolIndex;
            public int EchelonIndex;
            public Dictionary<int, int> SubUnitMap = new(); // ChildId -> Count
        }

        /// <summary>
        /// Captures a snapshot of the current form field values.
        /// </summary>
        private FormSnapshot _lastSavedSnapshot;

        /// <summary>
        /// Represents the currently selected node in the RadTreeView control.
        /// Used to track and manage the active selection state,
        /// allowing for operations such as enabling or disabling menu items
        /// and binding the form inputs to the selected unit blueprint.
        /// </summary>
        private RadTreeNode _selectedNode;

        /// <summary>
        /// Represents the faction currently selected in the unit blueprint editor form.
        /// This property provides access to the faction's details, such as its name and identifier,
        /// and is used throughout the form to configure unit blueprints and UI elements
        /// specific to the selected faction.
        /// </summary>
        private Faction SelectedFaction { get; set; }

        /// <summary>
        /// The currently selected UnitBlueprint being edited, or null if none is selected.
        /// </summary>
        private UnitBlueprint SelectedUnit { get; set; }

        /// <summary>
        /// In-memory list of child unit blueprint attachments for the currently selected unit.
        /// Key = child UnitBlueprint, Value = count.
        /// </summary>
        private Dictionary<UnitBlueprint, int> SubUnits { get; set; } = new Dictionary<UnitBlueprint, int>();

        public UnitBlueprintEditor(Faction selectedFaction)
        {
            if (selectedFaction == null)
                throw new ArgumentNullException(nameof(selectedFaction));

            SelectedFaction = selectedFaction;

            // Create components and apply theme
            InitializeComponent();
            unitTreeView.TreeViewElement.DrawBorder = false;

            // Panel Styling, show only left border
            radPanel2.PanelElement.PanelBorder.BoxStyle = BorderBoxStyle.FourBorders;
            radPanel2.PanelElement.PanelBorder.TopWidth = 0;
            radPanel2.PanelElement.PanelBorder.BottomWidth = 0;
            radPanel2.PanelElement.PanelBorder.RightWidth = 0;

            // Set context menu themes
            unitContextMenu.ThemeName = Program.ThemeName;
            positionContextMenu.ThemeName = Program.ThemeName;
            subunitContextMenu.ThemeName = Program.ThemeName;

            // Button styling
            FormStyling.ApplyControlsTheme(Controls);
            FormStyling.StyleButtonBlue(addPosButton);

            // Fill the echelon and promotion pool dropdowns
            using (var db = new AppDatabase())
            {
                var echelons = db.Echelons.ToList().OrderBy(x => x.HierarchyLevel);
                foreach (var echelon in echelons)
                {
                    echelonDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = echelon.Name,
                        Tag = echelon
                    });

                    promoPoolDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = echelon.Name,
                        Tag = echelon
                    });
                }
            }

            // Set defaults
            if (echelonDropDownList.Items.Count > 0)
                echelonDropDownList.SelectedIndex = 0;

            if (promoPoolDropDownList.Items.Count > 0)
                promoPoolDropDownList.SelectedIndex = 0;

            // Set the header label to include the faction name
            headerLabel.Text = $"Unit Blueprint Designer for {SelectedFaction.Name}";

            // Register context menu event handlers
            addBlueprintMenuItem.Click += AddBlueprintMenuItem_Click;
            deleteUnitMenuItem.Click += DeleteUnitMenuItem_Click;

            addPosMenuItem.Click += AddPosMenuItem_Click;
            deletePosMenuItem.Click += DeletePosMenuItem_Click;
            duplicatePosMenuItem.Click += DuplicatePosMenuItem_Click;
            copyOneMenuItem.Click += CopyOneMenuItem_Click;
            duplicateAllMenuItem.Click += DuplicateAllMenuItem_Click;
            importPosJsonMenuItem.Click += ImportPosJsonMenuItem_Click;

            editCountMenuItem.Click += EditCountMenuItem_Click;
            deleteSubMenuItem.Click += DeleteSubMenuItem_Click;

            // Disable context menu items until valid selections exist
            deleteUnitMenuItem.Enabled = false;
            deletePosMenuItem.Enabled = false;

            // Wire up context menu opening events to toggle item states
            unitContextMenu.DropDownOpening += UnitContextMenu_DropDownOpening;
            positionContextMenu.DropDownOpening += PositionContextMenu_DropDownOpening;
            subunitContextMenu.DropDownOpening += SubunitContextMenu_DropDownOpening;

            // Wire up change-detection events for dirty-state styling
            blueprintNameTextBox.TextChanged += (s, ev) => UpdateApplyButtonStyle();
            nameFormatTextBox.TextChanged += (s, ev) => UpdateApplyButtonStyle();
            shortFormatTextBox.TextChanged += (s, ev) => UpdateApplyButtonStyle();
            echelonDropDownList.SelectedIndexChanged += (s, ev) => UpdateApplyButtonStyle();
            promoPoolDropDownList.SelectedIndexChanged += (s, ev) => UpdateApplyButtonStyle();

            // Set default state
            ResetFields(true);

            // Hook into Telerik's internal drag-drop service for cross-control drops
            unitTreeView.TreeViewElement.DragDropService.PreviewDragOver += DragDropService_PreviewDragOver;
            unitTreeView.TreeViewElement.DragDropService.PreviewDragDrop += DragDropService_PreviewDragDrop;
        }

        #region Form Styling

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

        #region Event Handlers

        private void UnitBlueprintEditor_Load(object sender, EventArgs e)
        {
            LoadBlueprintsFromDatabase();
        }

        /// <summary>
        /// Handles the event triggered when the selected node in the RadTreeView changes.
        /// </summary>
        private void UnitTreeView_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            // Check for unsaved changes before allowing navigation
            if (HasUnsavedChanges())
            {
                var result = RadMessageBox.Show(
                    "You have unsaved changes. Are you sure you want to switch without applying?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);

                if (result != DialogResult.Yes)
                {
                    // Revert selection back to the previous node
                    unitTreeView.SelectedNodeChanged -= UnitTreeView_SelectedNodeChanged;
                    unitTreeView.SelectedNode = _selectedNode;
                    unitTreeView.SelectedNodeChanged += UnitTreeView_SelectedNodeChanged;
                    return;
                }
            }

            var node = e.Node;
            _selectedNode = node;

            // Bind form inputs to selected unit blueprint
            if (node != null && node.Tag is UnitBlueprint blueprint)
            {
                SelectBlueprint(blueprint);
            }
            else
            {
                // Root node (Echelon) or null selected — reset
                ResetFields(false);
            }
        }

        /// <summary>
        /// Handles double-clicking a PositionBlueprint in the list view.
        /// Opens the PositionBlueprintEditorForm in edit mode for the selected blueprint.
        /// </summary>
        private void PositionBlueprintListView_ItemMouseDoubleClick(object sender, ListViewItemEventArgs e)
        {
            if (e.Item?.Tag is not PositionBlueprint position)
                return;

            if (SelectedUnit == null || SelectedUnit.Id == 0)
                return;

            using var form = new PositionBlueprintEditorForm(SelectedUnit, position);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                FillPositionBlueprintsListView();
            }
        }

        /// <summary>
        /// Handles the Apply button click. Saves the currently selected UnitBlueprint's
        /// field values back to the database using the CrossLite Unit of Work pattern.
        /// </summary>
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(blueprintNameTextBox.Text))
            {
                RadMessageBox.Show("Invalid blueprint name entered!",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(nameFormatTextBox.Text))
            {
                RadMessageBox.Show("Invalid unit name format entered!",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            if (echelonDropDownList.SelectedItem?.Tag is not Echelon selectedEchelon)
            {
                RadMessageBox.Show("No echelon level was selected!",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            if (promoPoolDropDownList.SelectedItem?.Tag is not Echelon selectedPool)
            {
                RadMessageBox.Show("No promotion pool was selected!",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // Ensure no sub units have a higher echelon than the current unit
            var invalidSubUnits = SubUnits
                .Select(x => x.Key)
                .Where(x => x.Echelon.HierarchyLevel >= selectedEchelon.HierarchyLevel)
                .ToArray();

            if (invalidSubUnits.Length > 0)
            {
                RadMessageBox.Show(
                    $"A sub unit blueprint ({invalidSubUnits[0].Name}) has an equal or higher hierarchy level than the current blueprint!",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // Determine if this is a new or existing blueprint
            bool isNew = (SelectedUnit == null || SelectedUnit.Id == 0);

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                if (isNew)
                {
                    // Create new UnitBlueprint
                    SelectedUnit = db.UnitBlueprints.Create();
                    SelectedUnit.FactionId = SelectedFaction.Id;
                }

                // Update fields from form
                SelectedUnit.Name = blueprintNameTextBox.Text;
                SelectedUnit.UnitNameFormat = nameFormatTextBox.Text;
                SelectedUnit.UnitCodeFormat = shortFormatTextBox.Text;
                SelectedUnit.EchelonId = selectedEchelon.Id;
                SelectedUnit.PromotionPoolId = selectedPool.Id;

                if (isNew)
                {
                    db.UnitBlueprints.Add(SelectedUnit);

                    // Add all sub units
                    foreach (var kvp in SubUnits)
                    {
                        var attachment = new UnitBlueprintAttachment
                        {
                            ParentId = SelectedUnit.Id,
                            ChildId = kvp.Key.Id,
                            Count = kvp.Value
                        };
                        db.UnitTypeAttachments.Add(attachment);
                    }
                }
                else
                {
                    db.UnitBlueprints.Update(SelectedUnit);

                    // Sync subunit attachments: delete all, re-insert from the in-memory state
                    db.UnitTypeAttachments.RemoveWhere(a => a.ParentId == SelectedUnit.Id);

                    foreach (var kvp in SubUnits)
                    {
                        var attachment = new UnitBlueprintAttachment
                        {
                            ParentId = SelectedUnit.Id,
                            ChildId = kvp.Key.Id,
                            Count = kvp.Value
                        };
                        db.UnitTypeAttachments.Add(attachment);
                    }
                }

                transaction.Commit();

                // Update the tree node text
                if (unitTreeView.SelectedNode != null && unitTreeView.SelectedNode.Tag is UnitBlueprint)
                {
                    unitTreeView.SelectedNode.Text = SelectedUnit.Name;
                    unitTreeView.SelectedNode.Tag = SelectedUnit;
                }

                // If it was a new blueprint, reload the tree to place it under the correct echelon
                if (isNew)
                {
                    LoadBlueprintsFromDatabase();
                }

                // Update the clean snapshot after a successful save
                _lastSavedSnapshot = CaptureSnapshot();
                UpdateApplyButtonStyle();

                RadMessageBox.Show("Unit blueprint saved successfully.",
                    "Success", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to save unit blueprint: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Add Position Blueprint button click.
        /// </summary>
        private void AddPosButton_Click(object sender, EventArgs e) => AddPosMenuItem_Click(sender, e);

        /// <summary>
        /// Handles the FormClosing event. Prompts the user to confirm closing
        /// if there are unsaved changes.
        /// </summary>
        private void UnitBlueprintEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasUnsavedChanges())
            {
                var result = RadMessageBox.Show(
                    "You have unsaved changes. Are you sure you want to close without applying?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        #endregion

        #region Unit Context Menu (Tree View)

        /// <summary>
        /// Toggles menu item enabled states when the unit context menu opens.
        /// "Add New" is enabled when a root (Echelon) node or a blueprint node is selected.
        /// "Delete" is only enabled when a blueprint (child) node is selected.
        /// </summary>
        private void UnitContextMenu_DropDownOpening(object sender, EventArgs e)
        {
            var node = unitTreeView.SelectedNode;
            bool isBlueprintNode = node != null && node.Tag is UnitBlueprint;
            bool isEchelonNode = node != null && node.Tag is Echelon;

            // Only allow adding when an echelon root node is selected
            addBlueprintMenuItem.Enabled = isEchelonNode;
            deleteUnitMenuItem.Enabled = isBlueprintNode;
        }

        /// <summary>
        /// Handles the "Add New" menu item click on the tree view context menu.
        /// Creates a new blank UnitBlueprint, persists it to the database, and
        /// adds it to the tree under the appropriate echelon node.
        /// </summary>
        private void AddBlueprintMenuItem_Click(object sender, EventArgs e)
        {
            // Check for unsaved changes first
            if (HasUnsavedChanges())
            {
                var unsavedResult = RadMessageBox.Show(
                    "You have unsaved changes. Are you sure you want to continue without applying?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);

                if (unsavedResult != DialogResult.Yes)
                    return;
            }

            // Must have an echelon node selected
            var selectedNode = unitTreeView.SelectedNode;
            if (selectedNode == null || selectedNode.Tag is not Echelon selectedEchelon)
            {
                RadMessageBox.Show("Please select an echelon level to add the blueprint under.",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                var newBlueprint = db.UnitBlueprints.Create();
                newBlueprint.FactionId = SelectedFaction.Id;
                newBlueprint.Name = "New Unit Blueprint";
                newBlueprint.UnitNameFormat = "";
                newBlueprint.UnitCodeFormat = "";
                newBlueprint.EchelonId = selectedEchelon.Id;
                newBlueprint.PromotionPoolId = selectedEchelon.Id;

                db.UnitBlueprints.Add(newBlueprint);
                transaction.Commit();

                // Reload the tree and select the new node
                LoadBlueprintsFromDatabase();

                // Find and select the newly created node
                foreach (RadTreeNode rootNode in unitTreeView.Nodes)
                {
                    foreach (RadTreeNode childNode in rootNode.Nodes)
                    {
                        if (childNode.Tag is UnitBlueprint bp && bp.Id == newBlueprint.Id)
                        {
                            unitTreeView.SelectedNode = childNode;
                            _selectedNode = childNode;
                            SelectBlueprint(newBlueprint);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to create unit blueprint: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Handles the "Delete" menu item click on the tree view context menu.
        /// Deletes the currently selected UnitBlueprint from the database.
        /// </summary>
        private void DeleteUnitMenuItem_Click(object sender, EventArgs e)
        {
            var selectedNode = unitTreeView.SelectedNode;
            if (selectedNode == null || selectedNode.Tag is not UnitBlueprint blueprint)
                return;

            // Confirm deletion
            var result = RadMessageBox.Show(
                $"Are you sure you want to delete the unit blueprint \"{blueprint.Name}\"? " +
                "This will also delete all associated position blueprints and attachments.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Question);

            if (result != DialogResult.Yes)
                return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                db.UnitBlueprints.Remove(blueprint);
                transaction.Commit();

                // Reset form and reload tree
                ResetFields(true);
                LoadBlueprintsFromDatabase();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to delete unit blueprint: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        #endregion

        #region Position Context Menu (radListView1)

        /// <summary>
        /// Toggles menu item enabled states when the position context menu opens.
        /// Items that require a selected position are disabled when nothing is selected.
        /// </summary>
        private void PositionContextMenu_DropDownOpening(object sender, EventArgs e)
        {
            bool hasSelection = positionBlueprintListView.SelectedItem != null;
            bool hasUnit = SelectedUnit != null && SelectedUnit.Id > 0;
            bool hasPositions = positionBlueprintListView.Items.Count > 0;

            addPosMenuItem.Enabled = hasUnit;
            deletePosMenuItem.Enabled = hasSelection;
            duplicatePosMenuItem.Enabled = hasSelection;

            // These are enabled by default (whenever a unit is selected)
            importPosJsonMenuItem.Enabled = hasUnit;
            copyOneMenuItem.Enabled = hasUnit;

            // "Duplicate All From Another Unit" only enabled if there are NO position blueprints currently
            duplicateAllMenuItem.Enabled = hasUnit && !hasPositions;

            // Rebuild "Copy From" sub-menu with other blueprints' positions
            RefreshCopyFromMenuItems();

            // Rebuild "Duplicate All From" sub-menu (only if enabled)
            if (duplicateAllMenuItem.Enabled)
            {
                RefreshDuplicateAllMenuItems();
            }
            else
            {
                duplicateAllMenuItem.Items.Clear();
            }
        }

        /// <summary>
        /// Handles the "Create New Position" menu item click.
        /// </summary>
        private void AddPosMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedUnit == null || SelectedUnit.Id == 0)
            {
                RadMessageBox.Show("Please save the unit blueprint before adding position blueprints.",
                    "Validation", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // Open PositionBlueprintEditor dialog for SelectedUnit
            using var form = new PositionBlueprintEditorForm(SelectedUnit);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                FillPositionBlueprintsListView();
            }
        }

        /// <summary>
        /// Handles the "Delete" menu item click on the position context menu.
        /// Removes the selected PositionBlueprint from the database.
        /// </summary>
        private void DeletePosMenuItem_Click(object sender, EventArgs e)
        {
            if (positionBlueprintListView.SelectedItem == null)
                return;

            var position = positionBlueprintListView.SelectedItem.Tag as PositionBlueprint;
            if (position == null) return;

            var result = RadMessageBox.Show(
                $"Are you sure you want to delete the position blueprint \"{position}\"?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Question);

            if (result != DialogResult.Yes)
                return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                db.PositionBlueprints.Remove(position);
                transaction.Commit();

                FillPositionBlueprintsListView();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to delete position blueprint: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Handles the "Duplicate Selected Position" menu item click.
        /// Creates a copy of the selected PositionBlueprint under the same UnitBlueprint.
        /// </summary>
        private void DuplicatePosMenuItem_Click(object sender, EventArgs e)
        {
            if (positionBlueprintListView.SelectedItem == null)
                return;

            var position = positionBlueprintListView.SelectedItem.Tag as PositionBlueprint;
            if (position == null) return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                DuplicatePositionBlueprint(db, position, SelectedUnit);
                transaction.Commit();

                FillPositionBlueprintsListView();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to duplicate position blueprint: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Handles the "Import Blueprint JSON" menu item click.
        /// </summary>
        private void ImportPosJsonMenuItem_Click(object sender, EventArgs e)
        {
            if (positionBlueprintListView.SelectedItem == null)
                return;

            // TODO: Implement JSON import for position blueprints
            RadMessageBox.Show("JSON import is not yet implemented.",
                "Not Implemented", MessageBoxButtons.OK, RadMessageIcon.Info);
        }

        /// <summary>
        /// Handles the "Copy From" menu item click.
        /// This is a parent menu item — individual sub-items are wired up in
        /// <see cref="RefreshCopyFromMenuItems"/>.
        /// </summary>
        private void CopyOneMenuItem_Click(object sender, EventArgs e)
        {
            // Parent menu item — no direct action. Sub-items handle the logic.
        }

        /// <summary>
        /// Handles the "Duplicate All From" menu item click.
        /// This is a parent menu item — individual sub-items are wired up in
        /// <see cref="RefreshDuplicateAllMenuItems"/>.
        /// </summary>
        private void DuplicateAllMenuItem_Click(object sender, EventArgs e)
        {
            // Parent menu item — no direct action. Sub-items handle the logic.
        }

        /// <summary>
        /// Rebuilds the "Copy From" sub-menu with position blueprints from other
        /// UnitBlueprints of the same echelon, allowing the user to copy a single
        /// position blueprint into the current unit.
        /// </summary>
        private void RefreshCopyFromMenuItems()
        {
            copyOneMenuItem.Items.Clear();

            if (SelectedUnit == null || SelectedUnit.Id == 0) return;
            if (echelonDropDownList.SelectedItem?.Tag is not Echelon selectedEchelon) return;

            using var db = new AppDatabase();

            var otherBlueprints = db.UnitBlueprints
                .Where(ub => ub.FactionId == SelectedFaction.Id
                              && ub.EchelonId == selectedEchelon.Id
                              && ub.Id != SelectedUnit.Id)
                .OrderBy(ub => ub.Name)
                .ToList();

            foreach (var otherBp in otherBlueprints)
            {
                var parentItem = new RadMenuItem(otherBp.Name);

                foreach (var pos in otherBp.PositionBlueprints)
                {
                    var subItem = new RadMenuItem(pos.ToString()) { Tag = pos };
                    subItem.Click += CopyOneSubItem_Click;
                    parentItem.Items.Add(subItem);
                }

                if (parentItem.Items.Count > 0)
                    copyOneMenuItem.Items.Add(parentItem);
            }
        }

        /// <summary>
        /// Handles clicking a specific position blueprint from the "Copy From" sub-menu.
        /// Duplicates that single position into the current UnitBlueprint.
        /// </summary>
        private void CopyOneSubItem_Click(object sender, EventArgs e)
        {
            if (sender is not RadMenuItem menuItem) return;
            if (menuItem.Tag is not PositionBlueprint sourcePosition) return;

            var result = RadMessageBox.Show(
                $"Are you sure you want to copy the position \"{sourcePosition}\" into this unit?",
                "Confirm Copy",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Question);

            if (result != DialogResult.Yes) return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                DuplicatePositionBlueprint(db, sourcePosition, SelectedUnit);
                transaction.Commit();

                FillPositionBlueprintsListView();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to copy position blueprint: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Rebuilds the "Duplicate All From" sub-menu with other UnitBlueprints
        /// of the same echelon, allowing the user to copy ALL position blueprints
        /// from another unit into the current one.
        /// </summary>
        /// <summary>
        /// Rebuilds the "Duplicate All From" sub-menu with other UnitBlueprints
        /// of the same echelon. Level 2 = UnitBlueprint names, Level 3 = individual
        /// PositionBlueprints within each unit. Clicking a position copies ALL positions
        /// from that source unit into the current unit (clearing existing ones first).
        /// </summary>
        private void RefreshDuplicateAllMenuItems()
        {
            duplicateAllMenuItem.Items.Clear();

            if (SelectedUnit == null || SelectedUnit.Id == 0) return;
            if (echelonDropDownList.SelectedItem?.Tag is not Echelon selectedEchelon) return;

            using var db = new AppDatabase();

            var otherBlueprints = db.UnitBlueprints
                .Where(ub => ub.FactionId == SelectedFaction.Id
                             && ub.EchelonId == selectedEchelon.Id
                             && ub.Id != SelectedUnit.Id)
                .OrderBy(ub => ub.Name)
                .ToList();

            foreach (var otherBp in otherBlueprints)
            {
                var parentItem = new RadMenuItem(otherBp.Name);

                foreach (var pos in otherBp.PositionBlueprints)
                {
                    var subItem = new RadMenuItem(pos.ToString()) { Tag = otherBp };
                    subItem.Click += DuplicateAllSubItem_Click;
                    parentItem.Items.Add(subItem);
                }

                if (parentItem.Items.Count > 0)
                    duplicateAllMenuItem.Items.Add(parentItem);
            }
        }

        /// <summary>
        /// Handles clicking a specific UnitBlueprint from the "Duplicate All From" sub-menu.
        /// Copies ALL position blueprints from the source unit into the current unit,
        /// after clearing the current unit's existing positions.
        /// </summary>
        /// <summary>
        /// Handles clicking a specific PositionBlueprint from the "Duplicate All From" sub-menu.
        /// Copies ALL position blueprints from the source unit (the parent menu item's unit)
        /// into the current unit, after clearing any existing positions.
        /// </summary>
        private void DuplicateAllSubItem_Click(object sender, EventArgs e)
        {
            if (sender is not RadMenuItem menuItem) return;
            if (menuItem.Tag is not UnitBlueprint sourceBlueprint) return;

            var result = RadMessageBox.Show(
                $"Are you sure you want to copy all positions from \"{sourceBlueprint.Name}\"? " +
                "This will remove all current position blueprints first!",
                "Confirm Duplicate All",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Exclamation);

            if (result != DialogResult.Yes) return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                // Remove existing position blueprints
                var existingPositions = SelectedUnit.PositionBlueprints.ToList();
                foreach (var pos in existingPositions)
                {
                    db.PositionBlueprints.Remove(pos);
                }

                // Copy all positions from the source blueprint
                foreach (var pos in sourceBlueprint.PositionBlueprints)
                {
                    DuplicatePositionBlueprint(db, pos, SelectedUnit);
                }

                transaction.Commit();

                FillPositionBlueprintsListView();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to duplicate positions: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        #endregion

        #region Subunit Context Menu (radListView2)

        /// <summary>
        /// Toggles menu item enabled states when the subunit context menu opens.
        /// </summary>
        private void SubunitContextMenu_DropDownOpening(object sender, EventArgs e)
        {
            bool hasSelection = subUnitsGridView.SelectedRows.Count > 0;
            editCountMenuItem.Enabled = hasSelection;
            deleteSubMenuItem.Enabled = hasSelection;
        }

        /// <summary>
        /// Handles the "Adjust Count" menu item click on the subunit context menu.
        /// Prompts the user to enter a new count for the selected child unit attachment.
        /// </summary>
        private void EditCountMenuItem_Click(object sender, EventArgs e)
        {
            if (subUnitsGridView.SelectedRows.Count == 0)
                return;

            if (subUnitsGridView.SelectedRows[0].Tag is not KeyValuePair<UnitBlueprint, int> kvp)
                return;

            // Use a simple input dialog to get the new count
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter the new count for \"{kvp.Key.Name}\":",
                "Adjust Count",
                kvp.Value.ToString());

            if (string.IsNullOrWhiteSpace(input))
                return;

            if (!int.TryParse(input, out int newCount) || newCount < 1)
            {
                RadMessageBox.Show("Please enter a valid positive integer.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // Update the in-memory dictionary
            SubUnits[kvp.Key] = newCount;

            // Refresh the list view
            FillChildUnitsListView();
        }

        /// <summary>
        /// Handles the "Remove Subunit" menu item click on the subunit context menu.
        /// Removes the selected child unit attachment from the in-memory dictionary.
        /// </summary>
        private void DeleteSubMenuItem_Click(object sender, EventArgs e)
        {
            if (subUnitsGridView.SelectedRows.Count == 0)
                return;

            if (subUnitsGridView.SelectedRows[0].Tag is not KeyValuePair<UnitBlueprint, int> kvp)
                return;

            // Remove from in-memory dictionary
            SubUnits.Remove(kvp.Key);

            // Refresh the list view
            FillChildUnitsListView();
        }

        #endregion

        /// <summary>
        /// Loads all existing UnitBlueprints for the selected faction from the database
        /// and populates the tree view under the appropriate echelon root nodes.
        /// </summary>
        private void LoadBlueprintsFromDatabase()
        {
            // Clear existing nodes
            unitTreeView.Nodes.Clear();

            try
            {
                using var db = new AppDatabase();

                // Fetch echelons that have unit blueprints for this faction
                var echelons = db.Echelons
                    .Where(e => e.HierarchyLevel < 99)
                    .OrderByDescending(x => x.HierarchyLevel)
                    .ToList();

                var blueprints = db.UnitBlueprints
                    .Where(ub => ub.FactionId == SelectedFaction.Id)
                    .OrderBy(ub => ub.Name)
                    .ToList();

                foreach (var echelon in echelons)
                {
                    var echelonBlueprints = blueprints
                        .Where(ub => ub.EchelonId == echelon.Id)
                        .ToList();

                    var parentNode = new RadTreeNode(echelon.Name)
                    {
                        Tag = echelon
                    };

                    foreach (var blueprint in echelonBlueprints)
                    {
                        var childNode = new RadTreeNode(blueprint.Name)
                        {
                            Tag = blueprint
                        };
                        parentNode.Nodes.Add(childNode);
                    }

                    unitTreeView.Nodes.Add(parentNode);
                    parentNode.Expand();
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Failed to load unit blueprints: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Resets the fields and selections in the user interface to their default values.
        /// </summary>
        private void ResetFields(bool clearSelectedNode)
        {
            if (clearSelectedNode)
            {
                unitTreeView.SelectedNode = null;
            }

            SelectedUnit = null;
            blueprintNameTextBox.Text = string.Empty;
            nameFormatTextBox.Text = string.Empty;
            shortFormatTextBox.Text = string.Empty;

            if (echelonDropDownList.Items.Count > 0)
                echelonDropDownList.SelectedIndex = 0;

            if (promoPoolDropDownList.Items.Count > 0)
                promoPoolDropDownList.SelectedIndex = 0;

            SubUnits.Clear();
            _lastSavedSnapshot = null;

            // Clear list views
            positionBlueprintListView.Items.Clear();
            subUnitsGridView.Rows.Clear();

            // Disable editing controls
            SetFormEnabled(false);

            // Ensure apply button is disabled (no changes possible when nothing is selected)
            UpdateApplyButtonStyle();
        }

        /// <summary>
        /// Enables or disables the editing controls based on whether a unit blueprint is selected.
        /// </summary>
        private void SetFormEnabled(bool enabled)
        {
            blueprintNameTextBox.Enabled = enabled;
            nameFormatTextBox.Enabled = enabled;
            shortFormatTextBox.Enabled = enabled;
            echelonDropDownList.Enabled = enabled;
            promoPoolDropDownList.Enabled = enabled;
            addPosButton.Enabled = enabled;
            positionBlueprintListView.Enabled = enabled;
            subUnitsGridView.Enabled = enabled;
            orgChartButton.Enabled = enabled;
        }

        /// <summary>
        /// Updates the UI to reflect the details of the specified unit blueprint.
        /// </summary>
        private void SelectBlueprint(UnitBlueprint blueprint)
        {
            SelectedUnit = blueprint;

            // Set form fields from the blueprint
            blueprintNameTextBox.Text = blueprint.Name;
            nameFormatTextBox.Text = blueprint.UnitNameFormat;
            shortFormatTextBox.Text = blueprint.UnitCodeFormat;

            // Set echelon dropdown
            for (int i = 0; i < echelonDropDownList.Items.Count; i++)
            {
                if (echelonDropDownList.Items[i].Tag is Echelon ec && ec.Id == blueprint.EchelonId)
                {
                    echelonDropDownList.SelectedIndex = i;
                    break;
                }
            }

            // Set promotion pool dropdown
            for (int i = 0; i < promoPoolDropDownList.Items.Count; i++)
            {
                if (promoPoolDropDownList.Items[i].Tag is Echelon ec && ec.Id == blueprint.PromotionPoolId)
                {
                    promoPoolDropDownList.SelectedIndex = i;
                    break;
                }
            }

            // Load child unit attachments
            SubUnits.Clear();
            if (blueprint.Id > 0)
            {
                foreach (var attachment in blueprint.SubUnitBlueprints)
                {
                    if (attachment.ParentId == blueprint.Id)
                        SubUnits[attachment.Child] = attachment.Count;
                }
            }

            // Fill list views
            FillPositionBlueprintsListView();
            FillChildUnitsListView();

            // Snapshot the clean state
            _lastSavedSnapshot = CaptureSnapshot();

            // Enable editing controls
            SetFormEnabled(true);

            // Now update apply button style (will disable it since snapshot matches)
            UpdateApplyButtonStyle();
        }

        /// <summary>
        /// Fills the Position Blueprints RadListView (positionBlueprintListView) with the positions
        /// belonging to the currently selected UnitBlueprint.
        /// </summary>
        private void FillPositionBlueprintsListView()
        {
            positionBlueprintListView.Items.Clear();
            positionBlueprintListView.Groups.Clear();

            if (SelectedUnit == null || SelectedUnit.Id == 0) return;

            // Enable grouping
            positionBlueprintListView.EnableGrouping = true;
            positionBlueprintListView.ShowGroups = true;

            // Build a dictionary of groups keyed by category name
            var groupMap = new Dictionary<string, ListViewDataItemGroup>();

            foreach (var position in SelectedUnit.PositionBlueprints)
            {
                // Get the category name (fallback to "Uncategorized")
                string categoryName = position.Catagory?.Name ?? "Uncategorized";

                // Create group if it doesn't exist yet
                if (!groupMap.TryGetValue(categoryName, out var group))
                {
                    group = new ListViewDataItemGroup(categoryName);
                    groupMap[categoryName] = group;
                    positionBlueprintListView.Groups.Add(group);
                }

                var item = new ListViewDataItem();
                item.Text = position.ToString();
                item.Tag = position;
                item.Group = group;
                positionBlueprintListView.Items.Add(item);
            }
        }

        /// <summary>
        /// Fills the Child Unit Blueprints RadListView (subUnitsGridView) with the sub-unit
        /// attachments for the currently selected UnitBlueprint.
        /// </summary>
        private void FillChildUnitsListView()
        {
            subUnitsGridView.Rows.Clear();

            foreach (var kvp in SubUnits)
            {
                var row = subUnitsGridView.Rows.AddNew();
                row.Cells["column1"].Value = kvp.Key.Name;
                row.Cells["column2"].Value = kvp.Value.ToString();
                row.Tag = kvp;
            }
        }

        /// <summary>
        /// Captures the current form field values into a snapshot.
        /// </summary>
        private FormSnapshot CaptureSnapshot()
        {
            return new FormSnapshot
            {
                Name = blueprintNameTextBox.Text,
                UnitNameFormat = nameFormatTextBox.Text,
                UnitCodeFormat = shortFormatTextBox.Text,
                EchelonIndex = echelonDropDownList.SelectedIndex,
                PromotionPoolIndex = promoPoolDropDownList.SelectedIndex,
                SubUnitMap = SubUnits.ToDictionary(kvp => kvp.Key.Id, kvp => kvp.Value)
            };
        }

        /// <summary>
        /// Returns true if the current form values differ from the last saved snapshot.
        /// </summary>
        private bool HasUnsavedChanges()
        {
            if (_lastSavedSnapshot == null || SelectedUnit == null)
                return false;

            var current = CaptureSnapshot();

            // Compare SubUnits
            var currentSubs = SubUnits.ToDictionary(kvp => kvp.Key.Id, kvp => kvp.Value);
            if (currentSubs.Count != _lastSavedSnapshot.SubUnitMap.Count)
                return true;

            foreach (var kvp in currentSubs)
            {
                if (!_lastSavedSnapshot.SubUnitMap.TryGetValue(kvp.Key, out var savedCount) || savedCount != kvp.Value)
                    return true;
            }

            return current.Name != _lastSavedSnapshot.Name
                   || current.UnitNameFormat != _lastSavedSnapshot.UnitNameFormat
                   || current.UnitCodeFormat != _lastSavedSnapshot.UnitCodeFormat
                   || current.EchelonIndex != _lastSavedSnapshot.EchelonIndex
                   || current.PromotionPoolIndex != _lastSavedSnapshot.PromotionPoolIndex;
        }

        /// <summary>
        /// Updates the Apply button's color based on whether there are unsaved changes.
        /// Orange/Green when dirty, Blue when clean.
        /// </summary>
        private void UpdateApplyButtonStyle()
        {
            if (HasUnsavedChanges())
            {
                FormStyling.StyleButtonBlue(applyButton);
                applyButton.Enabled = true;
            }
            else
            {
                FormStyling.StyleButtonFluentDefault(applyButton);
                applyButton.Enabled = false;
            }
        }

        /// <summary>
        /// Creates a duplicate of the specified PositionBlueprint and adds it to the database
        /// under the given target UnitBlueprint.
        /// </summary>
        /// <param name="db">The active AppDatabase connection.</param>
        /// <param name="source">The PositionBlueprint to copy from.</param>
        /// <param name="targetUnit">The UnitBlueprint to attach the new copy to.</param>
        private void DuplicatePositionBlueprint(AppDatabase db, PositionBlueprint source, UnitBlueprint targetUnit)
        {
            var copy = db.PositionBlueprints.Create();

            // Copy all relevant fields from source
            copy.UnitBlueprintId = targetUnit.Id;
            copy.Name = source.Name;
            copy.TargetRankId = source.TargetRankId;
            copy.Stature = source.Stature;
            copy.MaxTourLength = source.MaxTourLength;
            copy.MinTourLength = source.MinTourLength;
            copy.PromotionPool = source.PromotionPool;
            copy.CanRetireEarly = source.CanRetireEarly;
            copy.Waiverable = source.Waiverable;
            copy.ZIndex = source.ZIndex;
            copy.SelectionMethod = source.SelectionMethod;
            copy.CatagoryId = source.CatagoryId;
            copy.DemoteOverRanked = source.DemoteOverRanked;
            copy.AutoPromoteInRankRange = source.AutoPromoteInRankRange;
            copy.Flag = source.Flag;
            copy.CanBePromotedEarly = source.CanBePromotedEarly;
            copy.CanLateralEarly = source.CanLateralEarly;
            copy.ExperienceLogic = source.ExperienceLogic;
            copy.InverseSpecialtyRequirements = source.InverseSpecialtyRequirements;

            db.PositionBlueprints.Add(copy);

            // TODO: Copy child relationships (specialties, requirements, careers, etc.)
            // if your PositionBlueprint has the same child entity sets as the old Billet.
            // Follow the same pattern from the old DuplicateBillet method.
        }

        #region Drag Drop Events

        private void UnitTreeView_DragStarting(object sender, RadTreeViewDragCancelEventArgs e)
        {
            // Only allow dragging UnitBlueprint nodes (not echelon root nodes)
            if (e.Node?.Tag is not UnitBlueprint)
            {
                e.Cancel = true;
            }
        }

        private void DragDropService_PreviewDragOver(object sender, RadDragOverEventArgs e)
        {
            if (IsOverSubUnitsGrid(e.HitTarget))
            {
                e.CanDrop = true;
            }
        }

        private bool IsOverSubUnitsGrid(object hitTarget)
        {
            if (hitTarget is RadElement element)
            {
                var control = element.ElementTree?.Control;
                return control == subUnitsGridView;
            }
            return hitTarget == subUnitsGridView;
        }

        private void DragDropService_PreviewDragDrop(object sender, RadDropEventArgs e)
        {
            // Only handle drops on the subUnitsGridView
            if (!IsOverSubUnitsGrid(e.HitTarget))
                return;

            // Get the dragged tree node
            var draggedNode = (e.DragInstance as TreeNodeElement)?.Data;
            if (draggedNode?.Tag is not UnitBlueprint draggedBlueprint)
                return;

            // Mark as handled so the tree doesn't try to move the node
            e.Handled = true;

            // Don't allow adding self as a sub-unit
            if (SelectedUnit != null && draggedBlueprint.Id == SelectedUnit.Id)
            {
                RadMessageBox.Show("A unit blueprint cannot be a sub-unit of itself.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // Check if already in the SubUnits dictionary
            if (SubUnits.ContainsKey(draggedBlueprint))
            {
                RadMessageBox.Show(
                    $"\"{draggedBlueprint.Name}\" is already a sub-unit. Use the context menu to adjust its count.",
                    "Already Added", MessageBoxButtons.OK, RadMessageIcon.Info);
                return;
            }

            // Add to in-memory dictionary with count = 1
            SubUnits[draggedBlueprint] = 1;

            // Refresh the grid
            FillChildUnitsListView();

            // Mark form as dirty
            UpdateApplyButtonStyle();
        }

        #endregion
    }
}
