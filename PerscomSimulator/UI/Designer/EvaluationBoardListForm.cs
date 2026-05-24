using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using CrossLite;
using Perscom.Simulation;
using Perscom.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class EvaluationBoardListForm : RadForm
    {
        /// <summary>
        /// The faction whose evaluation boards we are managing
        /// </summary>
        private Faction _faction;
        
        /// <summary>
        /// The height of each row in the grid, in pixels.
        /// </summary>
        private int _rowHeight = 50;

        /// <summary>
        /// In-memory list of boards currently displayed in the grid,
        /// kept in sync with the grid rows by index.
        /// </summary>
        private List<EvaluationBoard> _boards = new();

        public EvaluationBoardListForm(Faction faction)
        {
            if (faction == null)
                throw new ArgumentNullException(nameof(faction));

            // Setup controls and themes
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);
            radContextMenu1.ThemeName = Program.ThemeName;

            _faction = faction;
            headerLabel.Text = $"Evaluation Boards for {_faction}";

            // Wire events
            boardsGridView.DoubleClick += BoardsGridView_DoubleClick;
            boardsGridView.CellFormatting += GridView_CellFormatting;
            deleteMenuItem.Click += DeleteMenuItem_Click;

            // Load existing boards
            LoadBoards();
        }

        /// <summary>
        /// Loads all EvaluationBoards scoped to this faction from the database
        /// and populates the grid.
        /// </summary>
        private void LoadBoards()
        {
            boardsGridView.Rows.Clear();
            boardsGridView.TableElement.RowHeight = _rowHeight;
            _boards.Clear();

            using var db = new AppDatabase();
            var boards = db.EvaluationBoards
                .Where(b => b.FactionId == _faction.Id)
                .ToList();

            foreach (var board in boards)
            {
                _boards.Add(board);

                // Pool Type display
                string poolType = Enum.GetName(typeof(PoolSelection), board.PoolSelection) ?? "Unknown";

                // Candidate Ranks — render composite image
                var boardRanks = db.EvaluationBoardRanks
                    .Where(r => r.EvaluationBoardId == board.Id)
                    .ToList();

                var rankIds = boardRanks.Select(r => r.RankId).ToList();
                var ranks = db.Ranks
                    .Where(r => r.Id.In(rankIds))
                    .OrderBy(r => r.Precedence)
                    .ToList();

                Bitmap rankImage = RenderRankComposite(ranks, 120, _rowHeight - 5);

                // References — count how many PositionBlueprints use this board
                var references = db.PositionBlueprints.CountWhere(x => x.EvaluationBoardId == board.Id);
                boardsGridView.Rows.Add(
                    board.Name,
                    poolType,
                    rankImage,
                    references.ToString()
                );
            }

            // Adjust the width of the "References" column to fit the scroll bar
            RadGridViewHelper.AdjustColumnForScrollBar(boardsGridView, "column4", 140);
        }

        /// <summary>
        /// Renders a list of ranks into a single composite bitmap, similar to
        /// <see cref="RadClassificationRankDisplay"/>.
        /// </summary>
        private Bitmap RenderRankComposite(List<Rank> ranks, int width, int height)
        {
            if (ranks == null || ranks.Count == 0)
                return null;

            const double overlapFraction = 0.35;
            const int padding = 2;

            int areaW = Math.Max(1, width - (padding * 2));
            int areaH = Math.Max(1, height - (padding * 2));

            // Collect SVG images
            var svgImages = new List<(Rank rank, RadSvgImage svg)>();
            foreach (var rank in ranks)
            {
                if (string.IsNullOrEmpty(rank.Image)) continue;
                var svg = ImageAccessor.GetSvgImage(rank.Image);
                if (svg != null)
                    svgImages.Add((rank, svg));
            }

            if (svgImages.Count == 0)
                return null;

            try
            {
                var composite = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(composite))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    if (svgImages.Count == 1)
                    {
                        var svg = svgImages[0].svg;
                        var scaledSize = Imager.ScaleToFit(svg.Size, new Size(areaW, areaH));
                        var bmp = svg.GetRasterImage(scaledSize);
                        if (bmp != null && bmp.Width > 0 && bmp.Height > 0)
                        {
                            int x = (width - scaledSize.Width) / 2;
                            int y = (height - scaledSize.Height) / 2;
                            g.DrawImage(bmp, x, y, scaledSize.Width, scaledSize.Height);
                        }
                    }
                    else
                    {
                        int n = svgImages.Count;
                        double overlap = Math.Clamp(overlapFraction, 0.0, 0.9);
                        Size slotSize = new Size(Math.Max(1, areaH), areaH);

                        var scaledItems = new List<(RadSvgImage svg, Size scaled)>();
                        foreach (var (rank, svg) in svgImages)
                        {
                            var scaled = Imager.ScaleToFit(svg.Size, slotSize);
                            if (scaled.Width <= 0 || scaled.Height <= 0)
                                scaled = new Size(1, 1);
                            scaledItems.Add((svg, scaled));
                        }

                        int maxScaledWidth = scaledItems.Max(s => s.scaled.Width);
                        int step = Math.Max(1, (int)(maxScaledWidth * (1.0 - overlap)));
                        int compositeW = maxScaledWidth + (n - 1) * step;
                        int startX = Math.Max(padding, (width - compositeW) / 2);

                        for (int i = 0; i < scaledItems.Count; i++)
                        {
                            var (svg, scaled) = scaledItems[i];
                            var bmp = svg.GetRasterImage(scaled);
                            if (bmp == null || bmp.Width <= 0 || bmp.Height <= 0) continue;

                            int x = startX + (i * step);
                            int y = (height - scaled.Height) / 2;
                            g.DrawImage(bmp, x, y, scaled.Width, scaled.Height);
                        }
                    }
                }

                return composite;
            }
            catch
            {
                return null;
            }
        }

        #region Event Handlers

        /// <summary>
        /// Double-click a row to open the EvaluationBoardForm for that board.
        /// </summary>
        private void BoardsGridView_DoubleClick(object sender, EventArgs e)
        {
            if (boardsGridView.SelectedRows.Count == 0) return;

            int rowIndex = boardsGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= _boards.Count) return;

            var board = _boards[rowIndex];

            using var form = new EvaluationBoardForm(board);
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK || result == DialogResult.Abort)
                LoadBoards();
        }

        /// <summary>
        /// Deletes the selected board from the database and refreshes the grid.
        /// </summary>
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            if (boardsGridView.SelectedRows.Count == 0) return;

            int rowIndex = boardsGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= _boards.Count) return;

            var board = _boards[rowIndex];
            
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();
            
            // We can't delete a board if it's referenced by any PositionBlueprints
            var references = db.PositionBlueprints.Where(x => x.EvaluationBoardId == board.Id).ToList();
            if (references.Count > 0)
            {
                string names = string.Join(", ", references.Select(x => x.Name));
                RadMessageBox.Show(
                    $"You cannot delete \"{board.Name}\" because it is used in " + references.Count + " position blueprints\n" +
                    "The following blueprints reference this board: " + names,
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }
            else
            {
                var result = RadMessageBox.Show(
                    $"Are you sure you want to delete the evaluation board \"{board.Name}\"?\n" +
                    "This will also remove all associated scores, filters, groups, sorting rules, and allowed ranks.",
                    "Confirm Delete", MessageBoxButtons.YesNo, RadMessageIcon.Exclamation);

                if (result != DialogResult.Yes) return;
            }

            try
            {
                db.EvaluationBoards.Remove(board);
                transaction.Commit();

                LoadBoards();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to delete evaluation board: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
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

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
