using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrossLite;
using Perscom.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class OccupationBoardListForm : RadForm
    {
        /// <summary>
        /// The occupation whose board overrides we are managing
        /// </summary>
        private Occupation _occupation;

        /// <summary>
        /// In-memory list of boards currently displayed in the grid,
        /// kept in sync with the grid rows by index.
        /// </summary>
        private List<PromotionBoard> _boards = new();

        public OccupationBoardListForm(Occupation occupation)
        {
            if (occupation == null)
                throw new ArgumentNullException(nameof(occupation));

            // Setup controls and themes
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);
            boardContextMenu.ThemeName = Program.ThemeName;

            _occupation = occupation;
            headerLabel.Text = $"Promotion Board Overrides for {_occupation}";

            // Wire events
            boardContextMenu.DropDownOpening += BoardContextMenu_DropDownOpening;
            deleteMenuItem.Click += DeleteMenuItem_Click;

            // Load existing boards
            LoadBoards();
        }

        /// <summary>
        /// Loads all PromotionBoards scoped to this occupation from the database
        /// and populates the grid.
        /// </summary>
        private void LoadBoards()
        {
            promoBoardsGridView.Rows.Clear();
            _boards.Clear();

            using var db = new AppDatabase();
            var boards = db.PromotionBoards
                .Where(b => b.OccupationId == _occupation.Id)
                .ToList();

            foreach (var board in boards)
            {
                _boards.Add(board);

                // Determine scope display text
                string scope;
                if (board.RankId.HasValue)
                {
                    var rank = db.Ranks.Find(board.RankId.Value);
                    scope = $"Rank: {rank?.Name ?? "Unknown"}";
                }
                else if (board.RankClassificationId.HasValue)
                {
                    var classification = db.RankClassifications.Find(board.RankClassificationId.Value);
                    scope = $"Pay Grade: {classification?.ToString() ?? "Unknown"}";
                }
                else
                {
                    scope = "Unknown";
                }

                promoBoardsGridView.Rows.Add(
                    board.Name,
                    scope,
                    Enum.GetName(typeof(PromotionBoardType), board.Type),
                    $"{board.PassThreshold}%"
                );
            }
            
            // Adjust the width of the "Board Name" column to fit the scroll bar
            RadGridViewHelper.AdjustColumnForScrollBar(promoBoardsGridView, "column1", 230);
        }

        #region Event Handlers

        /// <summary>
        /// Dynamically populates the Classification Override and Rank Override
        /// sub-menus each time the context menu opens, excluding items that
        /// already have a board for this occupation.
        /// </summary>
        private void BoardContextMenu_DropDownOpening(object sender, CancelEventArgs e)
        {
            // Enable/disable delete based on selection
            bool hasSelection = promoBoardsGridView.SelectedRows.Count > 0
                                && promoBoardsGridView.SelectedRows[0].Index >= 0;
            deleteMenuItem.Enabled = hasSelection;

            // Clear previous dynamic sub-items
            classificationMenuItem.Items.Clear();
            rankMenuItem.Items.Clear();

            using var db = new AppDatabase();

            // Get IDs of classifications and ranks that already have a board for this occupation
            var existingBoards = db.PromotionBoards
                .Where(b => b.OccupationId == _occupation.Id)
                .ToList();

            var usedClassificationIds = new HashSet<int>(
                existingBoards
                    .Where(b => b.RankClassificationId.HasValue && !b.RankId.HasValue)
                    .Select(b => b.RankClassificationId.Value)
            );

            var usedRankIds = new HashSet<int>(
                existingBoards
                    .Where(b => b.RankId.HasValue)
                    .Select(b => b.RankId.Value)
            );

            // --- Classification Overrides (level 3) ---
            var classifications = db.RankClassifications
                .Where(rc => rc.FactionId == _occupation.FactionId && rc.Type == _occupation.Type)
                .OrderBy(rc => rc.PayGrade)
                .ToList();

            foreach (var classification in classifications)
            {
                if (usedClassificationIds.Contains(classification.Id))
                    continue;

                var item = new RadMenuItem(classification.ToString())
                {
                    Tag = classification
                };
                item.Click += ClassificationSubItem_Click;
                classificationMenuItem.Items.Add(item);
            }

            if (classificationMenuItem.Items.Count == 0)
            {
                var emptyItem = new RadMenuItem("(All classifications covered)") { Enabled = false };
                classificationMenuItem.Items.Add(emptyItem);
            }

            // --- Rank Overrides (level 3) ---
            var classificationIds = classifications.Select(rc => rc.Id).ToList();
            var ranks = db.Ranks
                .Where(r => r.RankClassificationId.In(classificationIds))
                .OrderBy(r => r.RankClassificationId)
                .ToList();

            foreach (var rank in ranks)
            {
                if (usedRankIds.Contains(rank.Id))
                    continue;

                var item = new RadMenuItem($"{rank.Name} ({rank.Abbreviation})")
                {
                    Tag = rank
                };
                item.Click += RankSubItem_Click;
                rankMenuItem.Items.Add(item);
            }

            if (rankMenuItem.Items.Count == 0)
            {
                var emptyItem = new RadMenuItem("(All ranks covered)") { Enabled = false };
                rankMenuItem.Items.Add(emptyItem);
            }
        }

        /// <summary>
        /// Handles clicking a classification sub-menu item to create a new
        /// classification-scoped board for this occupation.
        /// </summary>
        private void ClassificationSubItem_Click(object sender, EventArgs e)
        {
            if (sender is not RadMenuItem menuItem || menuItem.Tag is not RankClassification classification)
                return;

            using var form = new PromotionBoardEditor(classification, _occupation);
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK)
                LoadBoards();
        }

        /// <summary>
        /// Handles clicking a rank sub-menu item to create a new
        /// rank-scoped board for this occupation.
        /// </summary>
        private void RankSubItem_Click(object sender, EventArgs e)
        {
            if (sender is not RadMenuItem menuItem || menuItem.Tag is not Rank rank)
                return;

            using var form = new PromotionBoardEditor(rank, _occupation);
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK)
                LoadBoards();
        }

        /// <summary>
        /// Double-click a row to open the PromotionBoardEditor for that board.
        /// </summary>
        private void PromoBoardsGridView_DoubleClick(object sender, EventArgs e)
        {
            if (promoBoardsGridView.SelectedRows.Count == 0) return;

            int rowIndex = promoBoardsGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= _boards.Count) return;

            var board = _boards[rowIndex];

            using var form = new PromotionBoardEditor(board);
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK || result == DialogResult.Abort)
                LoadBoards();
        }

        /// <summary>
        /// Deletes the selected board from the database and refreshes the grid.
        /// </summary>
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            if (promoBoardsGridView.SelectedRows.Count == 0) return;

            int rowIndex = promoBoardsGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= _boards.Count) return;

            var board = _boards[rowIndex];

            var result = MessageBox.Show(
                $"Are you sure you want to delete the promotion board \"{board.Name}\"?\n" +
                "This will also remove all associated weights and scores.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                db.PromotionBoardWeights.RemoveWhere(w => w.PromotionBoardId == board.Id);
                db.PromotionBoardAddScores.RemoveWhere(s => s.PromotionBoardId == board.Id);
                db.PromotionBoards.Remove(board);
                transaction.Commit();

                LoadBoards();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Failed to delete promotion board: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Prevents the selected cell border and "pop" out
        /// </summary>
        private void GridView_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            RadGridViewHelper.RemoveSelectedCellBorderAndPop(e);
        }

        #endregion

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

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
