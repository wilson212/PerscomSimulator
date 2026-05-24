using Perscom.Database;
using Perscom.Simulation;
using Perscom.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.Windows.Diagrams.Core;

namespace Perscom
{
    public partial class UnitOrgChartForm : RadForm
    {
        private UnitBlueprint _unitBlueprint;

        // Shape dimensions
        private const int ShapeWidth = 150;
        private const int ShapeHeight = 120;
        private const int IconSize = 56;

        // Layout spacing
        private const double HorizontalGap = 30d;
        private const double VerticalGap = 80d;
        private const double SideOffsetX = 280d;

        private const int MaxColumnsPerRow = 4; // Max positions per row inside a group box
        private const int VerticalThreshold = 3;  // Switch to vertical when positions exceed this AND multiple categories share the row
        private const int VerticalColumns = 1;    // Number of columns in vertical mode (1 = single stack, 2 = two-column)

        public UnitOrgChartForm(UnitBlueprint unitBlueprint)
        {
            _unitBlueprint = unitBlueprint ?? throw new ArgumentNullException(nameof(unitBlueprint));
            InitializeComponent();

            labelHeader.Text = $"Organization Chart For — {unitBlueprint.Name}";
            FormStyling.ApplyControlsTheme(Controls);
            FormStyling.StyleButtonGreen(exportButton);
        }

        private void UnitOrgChartForm_Load(object sender, EventArgs e)
        {
            InitializeDiagram();
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
        }

        private void InitializeDiagram()
        {
            radDiagram.IsInformationAdornerVisible = false;
            radDiagram.ActiveTool = MouseTool.PanTool;
            radDiagram.IsSnapToGridEnabled = false;
            radDiagram.IsSnapToItemsEnabled = false;
            radDiagram.RouteConnections = false;
            radDiagram.BackgroundGrid.Visibility = ElementVisibility.Hidden;
            radDiagram.BackgroundPageGrid.Visibility = ElementVisibility.Hidden;
            radDiagram.IsSettingsPaneEnabled = false;

            BuildOrgChart();

            radDiagram.Zoom = 0.85;
        }

        /// <summary>
        /// Master method that builds the entire org chart using manual positioning.
        /// Positions are grouped by PositionCatagory, then laid out level-by-level
        /// according to OrgChartLevel (OrgChartPosition enum) and OrgChartAlignment.
        /// </summary>
        private void BuildOrgChart()
        {
            var positions = _unitBlueprint.PositionBlueprints.ToList();
            if (positions.Count == 0) return;

            var categoryGroups = positions
                .GroupBy(p => p.Catagory)
                .Where(g => g.Key != null)
                .ToList();

            var levelGroups = categoryGroups
                .GroupBy(g => g.Key.OrgChartLevel)
                .OrderBy(lg => (int)lg.Key)
                .ToList();

            var allShapes = new Dictionary<int, RadDiagramShape>();
            var connectedPairs = new HashSet<(int sourceId, int targetId)>();
            RadDiagramShape previousTrunkShape = null;

            // ── Title Header ──
            string factionName = _unitBlueprint.Faction?.Name;
            string titleText = !string.IsNullOrEmpty(factionName)
                ? $"Organization Chart for {_unitBlueprint.Name} ({factionName})"
                : $"Organization Chart for {_unitBlueprint.Name}";

            double canvasCenterX = 600;
            double boxPadding = 10;
            double labelInsideHeight = 24;
            double categoryGap = 40;
            double titleWidth = 600;
            double titleHeight = 36;
            double titleX = canvasCenterX - (titleWidth / 2.0);
            double titleY = 10;

            var titleShape = CreateLabelShape(titleText, titleX, titleY, (int)titleWidth);
            titleShape.Height = titleHeight;
            titleShape.ForeColor = Color.Black;
            titleShape.DiagramShapeElement.Font = new Font("Segoe UI Semibold", 14f, FontStyle.Bold);
            radDiagram.AddShape(titleShape);

            double currentY = titleY + titleHeight + 30;

            foreach (var level in levelGroups)
            {
                var byAlignment = level.ToLookup(g => g.Key.OrgChartAlignment);

                var leftCats = byAlignment[OrgChartAlignment.LeftSide].OrderBy(g => g.Key.ZIndex).ToList();
                var rightCats = byAlignment[OrgChartAlignment.RightSide].OrderBy(g => g.Key.ZIndex).ToList();
                var centerCats = byAlignment[OrgChartAlignment.Center].OrderBy(g => g.Key.ZIndex).ToList();
                var splitLeftCats = byAlignment[OrgChartAlignment.SplitLeft].OrderBy(g => g.Key.ZIndex).ToList();
                var splitRightCats = byAlignment[OrgChartAlignment.SplitRight].OrderBy(g => g.Key.ZIndex).ToList();
                var splitCenterCats = byAlignment[OrgChartAlignment.SplitCenter].OrderBy(g => g.Key.ZIndex).ToList();

                bool hasSplits = splitLeftCats.Count > 0 || splitRightCats.Count > 0 || splitCenterCats.Count > 0;
                double levelMaxHeight = 0;

                // ── Left side (branches off trunk, same Y as center) ──
                double h = LayoutCategoryRow(leftCats, canvasCenterX - SideOffsetX,
                    currentY, boxPadding, labelInsideHeight, categoryGap,
                    allShapes, connectedPairs, previousTrunkShape, true);
                if (h > 0) levelMaxHeight = Math.Max(levelMaxHeight, h);

                // ── Right side (branches off trunk, same Y as center) ──
                h = LayoutCategoryRow(rightCats, canvasCenterX + SideOffsetX,
                    currentY, boxPadding, labelInsideHeight, categoryGap,
                    allShapes, connectedPairs, previousTrunkShape, true);
                if (h > 0) levelMaxHeight = Math.Max(levelMaxHeight, h);

                // ── Center (command trunk — creates hub, updates previousTrunkShape) ──
                h = LayoutCategoryRow(centerCats, canvasCenterX,
                    currentY, boxPadding, labelInsideHeight, categoryGap,
                    allShapes, connectedPairs, previousTrunkShape, false);

                if (h > 0)
                {
                    var hub = CreateHubShape(canvasCenterX, (currentY - boxPadding) + h);
                    hub.Tag = centerCats.SelectMany(g => g).FirstOrDefault();
                    radDiagram.AddShape(hub);
                    previousTrunkShape = hub;
                    levelMaxHeight = Math.Max(levelMaxHeight, h);
                }

                // ── Split categories get their own sub-row BELOW center/side ──
                if (hasSplits)
                {
                    bool hasCenterOrSide = centerCats.Count > 0 || leftCats.Count > 0 || rightCats.Count > 0;
                    double splitY;
                    if (hasCenterOrSide)
                    {
                        splitY = currentY + Math.Max(levelMaxHeight, ShapeHeight) + VerticalGap;
                    }
                    else
                    {
                        // No center/side row on this level — place splits directly at currentY
                        splitY = currentY;
                    }

                    double splitMaxHeight = 0;

                    bool anySplitHadCenterBox = false;

                    // SplitLeft
                    var (hLeft, cLeft) = LayoutSplitRow(splitLeftCats, SplitMode.Left, canvasCenterX,
                                            splitY, boxPadding, labelInsideHeight, categoryGap,
                                            allShapes, connectedPairs, previousTrunkShape);
                    if (hLeft > 0) splitMaxHeight = Math.Max(splitMaxHeight, hLeft);
                    if (cLeft) anySplitHadCenterBox = true;

                    // SplitRight
                    var (hRight, cRight) = LayoutSplitRow(splitRightCats, SplitMode.Right, canvasCenterX,
                                            splitY, boxPadding, labelInsideHeight, categoryGap,
                                            allShapes, connectedPairs, previousTrunkShape);
                    if (hRight > 0) splitMaxHeight = Math.Max(splitMaxHeight, hRight);
                    if (cRight) anySplitHadCenterBox = true;

                    // SplitCenter
                    var (hCenter, cCenter) = LayoutSplitRow(splitCenterCats, SplitMode.Center, canvasCenterX,
                                            splitY, boxPadding, labelInsideHeight, categoryGap,
                                            allShapes, connectedPairs, previousTrunkShape);
                    if (hCenter > 0) splitMaxHeight = Math.Max(splitMaxHeight, hCenter);
                    if (cCenter) anySplitHadCenterBox = true;

                    // ── KEY FIX: Create a hub at the bottom of the split row ──
                    // This ensures the NEXT level's categories connect BELOW this split level,
                    // not to the old hub from a previous level.
                    if (splitMaxHeight > 0)
                    {
                        double hubY = splitY + Math.Max(splitMaxHeight, ShapeHeight);
                        var splitHub = CreateHubShape(canvasCenterX - 0.5, hubY);
                        radDiagram.AddShape(splitHub);

                        // Only draw trunk-through line when no center box exists on this split level
                        if (previousTrunkShape != null && !anySplitHadCenterBox)
                        {
                            ConnectShapes(previousTrunkShape, splitHub);
                        }

                        previousTrunkShape = splitHub;
                    }

                    // Total level height — use the actual hub position relative to currentY
                    levelMaxHeight = (splitY - currentY) + Math.Max(splitMaxHeight, ShapeHeight);
                }

                // Only advance Y if this level had center or split categories
                // Side-only levels float alongside the trunk without consuming vertical space
                bool hadCenterOrSplit = centerCats.Count > 0 || hasSplits;
                if (hadCenterOrSplit)
                {
                    currentY += Math.Max(levelMaxHeight, ShapeHeight) + VerticalGap;
                }
            }

            // Supervisor connections
            foreach (var pos in positions)
            {
                if (pos.SupervisorPositionBlueprintId.HasValue
                    && allShapes.TryGetValue(pos.SupervisorPositionBlueprintId.Value, out var parentShape)
                    && allShapes.TryGetValue(pos.Id, out var childShape))
                {
                    if (!connectedPairs.Contains((pos.SupervisorPositionBlueprintId.Value, pos.Id)))
                        ConnectShapes(parentShape, childShape, Color.Gray);
                }
            }
        }

        /// <summary>
        /// Creates a styled group box shape (rounded rect with subtle border).
        /// </summary>
        private RadDiagramShape CreateGroupBox(double x, double y, double width, double height)
        {
            var groupBox = new RadDiagramShape();
            groupBox.IsConnectorsManipulationEnabled = false;
            groupBox.IsRotationEnabled = false;
            groupBox.IsResizingEnabled = false;
            groupBox.IsDraggingEnabled = false;
            groupBox.Shape = new RoundRectShape(6);
            groupBox.Width = width;
            groupBox.Height = height;
            groupBox.Position = new Telerik.Windows.Diagrams.Core.Point(x, y);
            groupBox.BackColor = Color.FromArgb(20, 255, 255, 255);
            groupBox.DiagramShapeElement.DrawFill = true;
            groupBox.DiagramShapeElement.DrawBorder = true;
            groupBox.DiagramShapeElement.BorderColor = Color.FromArgb(100, 180, 180, 180);
            groupBox.DiagramShapeElement.BorderWidth = 1.5f;
            groupBox.Text = "";
            groupBox.Tag = null;
            return groupBox;
        }

        /// <summary>
        /// Creates an invisible 1x1 hub shape used as a trunk anchor point for connections.
        /// </summary>
        private RadDiagramShape CreateHubShape(double x, double y)
        {
            var hub = new RadDiagramShape();
            hub.IsConnectorsManipulationEnabled = false;
            hub.IsRotationEnabled = false;
            hub.IsResizingEnabled = false;
            hub.IsDraggingEnabled = false;
            hub.Width = 1;
            hub.Height = 1;
            hub.Position = new Telerik.Windows.Diagrams.Core.Point(x, y);
            hub.BackColor = Color.Transparent;
            hub.DiagramShapeElement.DrawFill = false;
            hub.DiagramShapeElement.DrawBorder = false;
            hub.Text = "";
            hub.Tag = null;
            return hub;
        }

        /// <summary>
        /// Lays out category group boxes split on either side of the trunk line at canvasCenterX.
        /// The split mode controls how odd-numbered category lists are distributed.
        /// Does NOT create a hub or update previousTrunkShape — split categories branch off
        /// the command trunk but do not interrupt it.
        /// Returns the max box height for level height calculation.
        /// </summary>
        private (double maxHeight, bool hasCenterBox) LayoutSplitRow(
            List<IGrouping<PositionCatagory, PositionBlueprint>> categories,
            SplitMode mode,
            double canvasCenterX,
            double currentY,
            double boxPadding,
            double labelInsideHeight,
            double categoryGap,
            Dictionary<int, RadDiagramShape> allShapes,
            HashSet<(int, int)> connectedPairs,
            RadDiagramShape previousTrunkShape)
        {
            if (categories.Count == 0) return (0, false);

            bool multipleCategories = categories.Count > 1;
            double trunkGap = 40; // gap left in the center for the trunk line to pass through

            // ── First pass: calculate each category's box dimensions ──
            var boxWidths = new List<double>();
            var boxHeights = new List<double>();
            var sortedLists = new List<List<PositionBlueprint>>();
            var colCounts = new List<int>();

            foreach (var catGroup in categories)
            {
                var sorted = SortPositionsByRank(catGroup.ToList());
                sortedLists.Add(sorted);

                int cols;
                if (multipleCategories && sorted.Count > VerticalThreshold)
                    cols = VerticalColumns;
                else
                    cols = Math.Min(sorted.Count, MaxColumnsPerRow);

                colCounts.Add(cols);

                int rows = (int)Math.Ceiling(sorted.Count / (double)cols);
                double tw = cols * ShapeWidth + (cols - 1) * HorizontalGap;
                double th = rows * ShapeHeight + (rows - 1) * HorizontalGap;

                boxWidths.Add(tw + (boxPadding * 2));
                boxHeights.Add(labelInsideHeight + th + (boxPadding * 2));
            }

            // ── Distribute categories into left, center, and right lists ──
            var leftIndices = new List<int>();
            var rightIndices = new List<int>();
            int? centerIndex = null;

            int count = categories.Count;

            if (count == 1)
            {
                // Single category: always center it (even for SplitLeft/SplitRight)
                centerIndex = 0;
            }
            else if (count % 2 == 0)
            {
                // Even count: split evenly regardless of mode
                int half = count / 2;
                for (int i = 0; i < half; i++)
                    leftIndices.Add(i);
                for (int i = half; i < count; i++)
                    rightIndices.Add(i);
            }
            else
            {
                // Odd count: mode determines where the extra box goes
                int half = count / 2;

                switch (mode)
                {
                    case SplitMode.Left:
                        // Extra box goes to the left side
                        // Left gets half+1, right gets half
                        for (int i = 0; i <= half; i++)
                            leftIndices.Add(i);
                        for (int i = half + 1; i < count; i++)
                            rightIndices.Add(i);
                        break;

                    case SplitMode.Right:
                        // Extra box goes to the right side
                        // Left gets half, right gets half+1
                        for (int i = 0; i < half; i++)
                            leftIndices.Add(i);
                        for (int i = half; i < count; i++)
                            rightIndices.Add(i);
                        break;

                    case SplitMode.Center:
                        // Middle box sits on the center line
                        for (int i = 0; i < half; i++)
                            leftIndices.Add(i);
                        centerIndex = half;
                        for (int i = half + 1; i < count; i++)
                            rightIndices.Add(i);
                        break;
                }
            }

            double maxBoxH = 0;

            // ── Calculate left side total width ──
            double leftTotalWidth = 0;
            for (int i = 0; i < leftIndices.Count; i++)
            {
                leftTotalWidth += boxWidths[leftIndices[i]];
                if (i < leftIndices.Count - 1)
                    leftTotalWidth += categoryGap;
            }

            // ── Calculate right side total width ──
            double rightTotalWidth = 0;
            for (int i = 0; i < rightIndices.Count; i++)
            {
                rightTotalWidth += boxWidths[rightIndices[i]];
                if (i < rightIndices.Count - 1)
                    rightTotalWidth += categoryGap;
            }

            // ── Calculate center box width (if any) ──
            double centerBoxWidth = centerIndex.HasValue ? boxWidths[centerIndex.Value] : 0;

            // ── Place LEFT boxes: grow leftward from the center gap ──
            {
                double leftEdge = canvasCenterX - (trunkGap / 2.0);
                if (centerIndex.HasValue)
                    leftEdge = canvasCenterX - (centerBoxWidth / 2.0) - categoryGap;

                // Start from the rightmost left box (closest to center) and go left
                double currentBoxX = leftEdge;
                for (int i = leftIndices.Count - 1; i >= 0; i--)
                {
                    int idx = leftIndices[i];
                    double boxW = boxWidths[idx];
                    double boxH = boxHeights[idx];
                    double boxX = currentBoxX - boxW;
                    double boxY = currentY - boxPadding;
                    maxBoxH = Math.Max(maxBoxH, boxH);

                    PlaceCategoryBox(idx, boxX, boxY, boxW, boxH,
                        categories, sortedLists, colCounts,
                        currentY, boxPadding, labelInsideHeight,
                        allShapes, connectedPairs, previousTrunkShape,
                        isSideBranch: mode != SplitMode.Center,
                        isSideEdge: mode == SplitMode.Center);

                    currentBoxX = boxX - categoryGap;
                }
            }

            // ── Place RIGHT boxes: grow rightward from the center gap ──
            {
                double rightEdge = canvasCenterX + (trunkGap / 2.0);
                if (centerIndex.HasValue)
                    rightEdge = canvasCenterX + (centerBoxWidth / 2.0) + categoryGap;

                double currentBoxX = rightEdge;
                for (int i = 0; i < rightIndices.Count; i++)
                {
                    int idx = rightIndices[i];
                    double boxW = boxWidths[idx];
                    double boxH = boxHeights[idx];
                    double boxX = currentBoxX;
                    double boxY = currentY - boxPadding;
                    maxBoxH = Math.Max(maxBoxH, boxH);

                    PlaceCategoryBox(idx, boxX, boxY, boxW, boxH,
                        categories, sortedLists, colCounts,
                        currentY, boxPadding, labelInsideHeight,
                        allShapes, connectedPairs, previousTrunkShape,
                        isSideBranch: mode != SplitMode.Center,
                        isSideEdge: mode == SplitMode.Center);

                    currentBoxX = boxX + boxW + categoryGap;
                }
            }

            // ── Place CENTER box (SplitCenter with odd count, or single category) ──
            if (centerIndex.HasValue)
            {
                int idx = centerIndex.Value;
                double boxW = boxWidths[idx];
                double boxH = boxHeights[idx];
                double boxX = canvasCenterX - (boxW / 2.0);
                double boxY = currentY - boxPadding;
                maxBoxH = Math.Max(maxBoxH, boxH);

                PlaceCategoryBox(idx, boxX, boxY, boxW, boxH,
                    categories, sortedLists, colCounts,
                    currentY, boxPadding, labelInsideHeight,
                    allShapes, connectedPairs, previousTrunkShape,
                    isSideBranch: false,
                    isSideEdge: false);
            }

            return (maxBoxH, centerIndex.HasValue);
        }

        /// <summary>
        /// Places a single category group box at the specified position, including its label,
        /// position shapes inside, and connection from the trunk.
        /// </summary>
        private void PlaceCategoryBox(
            int idx,
            double boxX, double boxY, double boxW, double boxH,
            List<IGrouping<PositionCatagory, PositionBlueprint>> categories,
            List<List<PositionBlueprint>> sortedLists,
            List<int> colCounts,
            double currentY,
            double boxPadding,
            double labelInsideHeight,
            Dictionary<int, RadDiagramShape> allShapes,
            HashSet<(int, int)> connectedPairs,
            RadDiagramShape previousTrunkShape,
            bool isSideBranch = false,
            bool isSideEdge = false)
        {
            var catGroup = categories[idx];
            var sortedPositions = sortedLists[idx];
            int cols = colCounts[idx];

            var groupBox = CreateGroupBox(boxX, boxY, boxW, boxH);
            radDiagram.AddShape(groupBox);

            var labelShape = CreateLabelShape(catGroup.Key.Name, boxX, boxY + 2, (int)boxW);
            radDiagram.AddShape(labelShape);

            // Place positions in a grid
            double posStartY = currentY + labelInsideHeight;
            double posStartX = boxX + boxPadding;

            for (int i = 0; i < sortedPositions.Count; i++)
            {
                int col = i % cols;
                int row = i / cols;

                double x = posStartX + col * (ShapeWidth + HorizontalGap);
                double y = posStartY + row * (ShapeHeight + HorizontalGap);

                var shape = CreatePositionShape(sortedPositions[i]);
                shape.Position = new Telerik.Windows.Diagrams.Core.Point(x, y);
                radDiagram.AddShape(shape);
                allShapes[sortedPositions[i].Id] = shape;
            }

            if (previousTrunkShape != null)
            {
                if (isSideEdge)
                    ConnectShapesSideEdge(previousTrunkShape, groupBox);
                else if (isSideBranch)
                    ConnectShapesSideBranch(previousTrunkShape, groupBox);
                else
                    ConnectShapes(previousTrunkShape, groupBox);

                foreach (var pos in sortedPositions)
                {
                    if (previousTrunkShape.Tag is PositionBlueprint srcPos)
                        connectedPairs.Add((srcPos.Id, pos.Id));
                }
            }
        }

        /// <summary>
        /// Lays out a row of categories in the organizational chart. Each category is represented
        /// by a group of position blueprints and is positioned based on calculated dimensions
        /// and spacing. The method supports multiple categories, calculates the dimensions for
        /// each, and places them horizontally while maintaining alignment using a shared Y-coordinate.
        /// </summary>
        private double LayoutCategoryRow(
            List<IGrouping<PositionCatagory, PositionBlueprint>> categories,
            double anchorX,
            double currentY,
            double boxPadding,
            double labelInsideHeight,
            double categoryGap,
            Dictionary<int, RadDiagramShape> allShapes,
            HashSet<(int, int)> connectedPairs,
            RadDiagramShape previousTrunkShape,
            bool isSideBranch = false)
        {
            if (categories.Count == 0) return 0;


            bool multipleCategories = categories.Count > 1;

            // First pass: calculate box dimensions
            var boxWidths = new List<double>();
            var boxHeights = new List<double>();
            var sortedLists = new List<List<PositionBlueprint>>();
            var colCounts = new List<int>();

            foreach (var catGroup in categories)
            {
                var sorted = SortPositionsByRank(catGroup.ToList());
                sortedLists.Add(sorted);

                int cols;
                if (multipleCategories && sorted.Count > VerticalThreshold)
                    cols = VerticalColumns;
                else
                    cols = Math.Min(sorted.Count, MaxColumnsPerRow);

                colCounts.Add(cols);

                int rows = (int)Math.Ceiling(sorted.Count / (double)cols);
                double tw = cols * ShapeWidth + (cols - 1) * HorizontalGap;
                double th = rows * ShapeHeight + (rows - 1) * HorizontalGap;

                boxWidths.Add(tw + (boxPadding * 2));
                boxHeights.Add(labelInsideHeight + th + (boxPadding * 2));
            }

            double combinedWidth = boxWidths.Sum() + (boxWidths.Count - 1) * categoryGap;
            double rowStartX = anchorX - (combinedWidth / 2.0);
            double currentBoxX = rowStartX;
            double maxBoxH = 0;

            // Second pass: place boxes
            for (int idx = 0; idx < categories.Count; idx++)
            {
                double boxX = currentBoxX;
                double boxY = currentY - boxPadding;
                double boxW = boxWidths[idx];
                double boxH = boxHeights[idx];
                maxBoxH = Math.Max(maxBoxH, boxH);

                PlaceCategoryBox(idx, boxX, boxY, boxW, boxH,
                    categories, sortedLists, colCounts,
                    currentY, boxPadding, labelInsideHeight,
                    allShapes, connectedPairs, previousTrunkShape, isSideBranch);

                currentBoxX += boxW + categoryGap;
            }

            return maxBoxH;
        }

        /// <summary>
        /// Sorts positions by RankType (Officer first, then Warrant, then Enlisted),
        /// then by PayGrade descending within each type.
        /// </summary>
        private List<PositionBlueprint> SortPositionsByRank(List<PositionBlueprint> positions)
        {
            return positions
                .OrderByDescending(p => GetRankTypeOrder(p))
                .ThenByDescending(p => p.TargetRank?.PayGrade ?? 0)
                .ThenByDescending(p => p.Stature)
                .ToList();
        }

        /// <summary>
        /// Returns a sort value for RankType: Officer=2, Warrant=1, Enlisted=0
        /// </summary>
        private int GetRankTypeOrder(PositionBlueprint pos)
        {
            if (pos.TargetRank?.Classification == null) return -1;
            return pos.TargetRank.Classification.Type switch
            {
                RankType.Officer => 2,
                RankType.Warrant => 1,
                _ => 0
            };
        }

        /// <summary>
        /// Creates a label shape (non-interactive) for category group names.
        /// </summary>
        private RadDiagramShape CreateLabelShape(string text, double x, double y, int width)
        {
            var label = new RadDiagramShape();
            label.IsConnectorsManipulationEnabled = false;
            label.IsRotationEnabled = false;
            label.IsResizingEnabled = false;
            label.IsDraggingEnabled = false;
            label.Width = Math.Max(width, 150);
            label.Height = 22;
            label.Position = new Telerik.Windows.Diagrams.Core.Point(x, y);
            label.Text = text;
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.FromArgb(80, 80, 80);
            label.DiagramShapeElement.DrawBorder = false;
            label.DiagramShapeElement.DrawFill = false;
            label.DiagramShapeElement.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Italic);
            label.DiagramShapeElement.TextAlignment = ContentAlignment.MiddleCenter;
            return label;
        }

        /// <summary>
        /// Creates a single diagram shape for a PositionBlueprint.
        /// </summary>
        private RadDiagramShape CreatePositionShape(PositionBlueprint position)
        {
            var shape = new RadDiagramShape();
            shape.IsConnectorsManipulationEnabled = false;
            shape.IsRotationEnabled = false;
            shape.IsResizingEnabled = false;
            shape.IsDraggingEnabled = false;
            shape.Shape = new RoundRectShape(4);
            shape.Width = ShapeWidth;
            shape.Height = ShapeHeight;
            shape.Name = position.Name;
            shape.Tag = position;

            if (position.TargetRank?.Classification != null)
            {
                shape.BackColor = position.TargetRank.Classification.Type switch
                {
                    RankType.Officer => Color.FromArgb(34, 60, 34),
                    RankType.Warrant => Color.FromArgb(60, 50, 20),
                    _ => Color.FromArgb(45, 45, 48)
                };
            }
            else
            {
                shape.BackColor = Color.FromArgb(45, 45, 48);
            }

            shape.ForeColor = Color.White;

            var composite = RenderPositionCard(position, ShapeWidth, ShapeHeight, IconSize);
            if (composite != null)
            {
                shape.DiagramShapeElement.Image = composite;
                shape.DiagramShapeElement.ImageLayout = ImageLayout.Center;
                shape.Text = "";
            }
            else
            {
                shape.DiagramShapeElement.TextAlignment = ContentAlignment.MiddleCenter;
                shape.Text = position.Name;
            }

            return shape;
        }

        /// <summary>
        /// Renders a composite bitmap: rank insignia top-center with outline + drop shadow,
        /// position name centered below, rank name in smaller italic gray text beneath.
        /// </summary>
        private Bitmap RenderPositionCard(PositionBlueprint position, int cardWidth, int cardHeight, int iconSize)
        {
            int shadowRadius = 3, shadowOffset = 2, outlineWidth = 1;
            var shadowColor = Color.FromArgb(120, 0, 0, 0);
            var outlineColor = Color.FromArgb(180, 0, 0, 0);

            var composite = new Bitmap(cardWidth, cardHeight);
            using (var g = Graphics.FromImage(composite))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                int iconY = 6;

                // Draw rank image
                if (position.TargetRank != null
                    && !string.IsNullOrWhiteSpace(position.TargetRank.Image))
                {
                    var svgImage = ImageAccessor.GetSvgImage(position.TargetRank.Image);
                    if (svgImage != null)
                    {
                        int margin = Math.Max(outlineWidth, shadowRadius + shadowOffset);
                        var paddedSize = new System.Drawing.Size(
                            Math.Max(1, iconSize - (margin * 2)),
                            Math.Max(1, iconSize - (margin * 2)));

                        var scaledSize = Imager.ScaleToFit(svgImage.Size, paddedSize);
                        if (scaledSize.Width <= 0 || scaledSize.Height <= 0)
                            scaledSize = new System.Drawing.Size(1, 1);

                        var bmp = svgImage.GetRasterImage(scaledSize);
                        if (bmp != null && bmp.Width > 0 && bmp.Height > 0)
                        {
                            int x = (cardWidth - scaledSize.Width) / 2;
                            int y = iconY + (iconSize - scaledSize.Height) / 2;

                            Imager.DrawDropShadow(g, bmp, x, y,
                                scaledSize.Width, scaledSize.Height,
                                shadowRadius, shadowOffset, shadowColor);
                            Imager.DrawOutline(g, bmp, x, y,
                                scaledSize.Width, scaledSize.Height,
                                outlineWidth, outlineColor);
                            g.DrawImage(bmp, x, y, scaledSize.Width, scaledSize.Height);
                        }
                    }
                }

                // Position name below icon
                int textY = iconY + iconSize + 4;
                int textAreaHeight = cardHeight - textY - 4;

                using var nameFont = new Font("Segoe UI Semibold", 8.5f);
                using var nameBrush = new SolidBrush(Color.White);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                var nameRect = new RectangleF(4, textY, cardWidth - 8, textAreaHeight / 2f);
                g.DrawString(position.Name, nameFont, nameBrush, nameRect, sf);

                // Rank name below position name
                if (position.TargetRank != null)
                {
                    using var rankFont = new Font("Segoe UI", 7f, FontStyle.Italic);
                    using var rankBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
                    var rankRect = new RectangleF(
                        4, textY + (textAreaHeight / 2f), cardWidth - 8, textAreaHeight / 2f);
                    g.DrawString(position.TargetRank.Name, rankFont, rankBrush, rankRect, sf);
                }
            }

            return composite;
        }

        /// <summary>
        /// Creates an orthogonal (right-angle) connection between two shapes.
        /// Uses explicit StartPoint/EndPoint with two waypoints to create
        /// clean 90-degree connector lines with no diagonal segments.
        /// </summary>
        private void ConnectShapes(
            RadDiagramShape source,
            RadDiagramShape target,
            Color? color = null,
            ConnectionType type = ConnectionType.Polyline)
        {
            var connection = new RadDiagramConnection();
            connection.ConnectionType = type;

            if (color.HasValue)
            {
                connection.ForeColor = color.Value;
            }

            // Calculate anchor points
            double srcCenterX = source.Position.X + (source.Width / 2.0);
            double srcBottomY = source.Position.Y + source.Height;
            double tgtCenterX = target.Position.X + (target.Width / 2.0);
            double tgtTopY = target.Position.Y;

            // Set explicit start and end points
            connection.StartPoint = new Telerik.Windows.Diagrams.Core.Point(srcCenterX, srcBottomY);
            connection.EndPoint = new Telerik.Windows.Diagrams.Core.Point(tgtCenterX, tgtTopY);

            // Only add elbow waypoints if there's a horizontal offset
            // (straight vertical lines don't need waypoints)
            if (Math.Abs(srcCenterX - tgtCenterX) > 1.0)
            {
                double midY;
                if (tgtTopY <= srcBottomY)
                {
                    midY = Math.Min(srcBottomY, tgtTopY) - 20;
                }
                else
                {
                    midY = srcBottomY + ((tgtTopY - srcBottomY) / 2.0);
                }

                connection.ConnectionPoints.Add(new Telerik.Windows.Diagrams.Core.Point(srcCenterX, midY));
                connection.ConnectionPoints.Add(new Telerik.Windows.Diagrams.Core.Point(tgtCenterX, midY));
            }

            radDiagram.AddConnection(connection);
        }

        /// <summary>
        /// Creates a connection that goes straight down from source, then horizontally
        /// to the target at a Y just above the target's top edge.
        /// Used for side/split branches where the midpoint calculation would
        /// route through other boxes.
        /// </summary>
        private void ConnectShapesSideBranch(
            RadDiagramShape source,
            RadDiagramShape target,
            Color? color = null,
            ConnectionType type = ConnectionType.Polyline)
        {
            var connection = new RadDiagramConnection();
            connection.ConnectionType = type;

            if (color.HasValue)
                connection.ForeColor = color.Value;

            double srcCenterX = source.Position.X + (source.Width / 2.0);
            double srcBottomY = source.Position.Y + source.Height;
            double tgtCenterX = target.Position.X + (target.Width / 2.0);
            double tgtTopY = target.Position.Y;

            // Route the horizontal segment just above the target (10px above)
            double turnY = tgtTopY - 10;
            // But don't go above the source
            turnY = Math.Max(turnY, srcBottomY + 5);

            connection.StartPoint = new Telerik.Windows.Diagrams.Core.Point(srcCenterX, srcBottomY);
            connection.EndPoint = new Telerik.Windows.Diagrams.Core.Point(tgtCenterX, tgtTopY);

            connection.ConnectionPoints.Add(new Telerik.Windows.Diagrams.Core.Point(srcCenterX, turnY));
            connection.ConnectionPoints.Add(new Telerik.Windows.Diagrams.Core.Point(tgtCenterX, turnY));

            radDiagram.AddConnection(connection);
        }
        
        /// <summary>
        /// Creates a connection from the trunk that goes straight down, then horizontally
        /// into the SIDE of the target box (at its vertical midpoint).
        /// Used for SplitCenter left/right boxes where the line should enter the box edge.
        /// </summary>
        private void ConnectShapesSideEdge(
            RadDiagramShape source,
            RadDiagramShape target,
            Color? color = null,
            ConnectionType type = ConnectionType.Polyline)
        {
            var connection = new RadDiagramConnection();
            connection.ConnectionType = type;

            if (color.HasValue)
                connection.ForeColor = color.Value;

            double srcCenterX = source.Position.X + (source.Width / 2.0);
            double srcBottomY = source.Position.Y + source.Height;

            // Determine which side of the target box to connect to
            double tgtCenterX = target.Position.X + (target.Width / 2.0);
            double tgtMidY = target.Position.Y + (target.Height / 2.0);

            double tgtEdgeX;
            if (tgtCenterX < srcCenterX)
            {
                // Target is to the LEFT of trunk — connect to its RIGHT edge
                tgtEdgeX = target.Position.X + target.Width;
            }
            else
            {
                // Target is to the RIGHT of trunk — connect to its LEFT edge
                tgtEdgeX = target.Position.X;
            }

            connection.StartPoint = new Telerik.Windows.Diagrams.Core.Point(srcCenterX, srcBottomY);
            connection.EndPoint = new Telerik.Windows.Diagrams.Core.Point(tgtEdgeX, tgtMidY);

            // Route: go down from trunk to the target's midY, then horizontally to the side edge
            connection.ConnectionPoints.Add(new Telerik.Windows.Diagrams.Core.Point(srcCenterX, tgtMidY));
            connection.ConnectionPoints.Add(new Telerik.Windows.Diagrams.Core.Point(tgtEdgeX, tgtMidY));

            radDiagram.AddConnection(connection);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            saveDialog.Title = "Export Diagram";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var image = radDiagram.ExportToImage();
                image.Save(saveDialog.FileName);
            }
        }

        /// <summary>
        /// Controls how category boxes are distributed around the center trunk line
        /// when using a Split alignment.
        /// </summary>
        private enum SplitMode
        {
            /// <summary>Left-biased: odd count puts the extra box on the left side.</summary>
            Left,
            /// <summary>Right-biased: odd count puts the extra box on the right side.</summary>
            Right,
            /// <summary>Center-biased: odd count places the middle box on the center line.</summary>
            Center
        }
    }
}
