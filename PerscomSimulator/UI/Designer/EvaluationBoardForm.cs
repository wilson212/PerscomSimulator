using CrossLite.QueryBuilder;
using Perscom.Database;
using Perscom.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CrossLite;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class EvaluationBoardForm : RadForm
    {
        /// <summary>
        /// The evaluation board entity being edited (null until first save for new boards)
        /// </summary>
        private EvaluationBoard Board { get; set; }
        
        /// <summary>
        /// The PositionBlueprint that provides faction context and rank info
        /// </summary>
        private PositionBlueprint Blueprint { get; set; }

        /// <summary>
        /// Indicates whether we are creating a new board or editing an existing one
        /// </summary>
        private bool IsNewBoard { get; set; }

        /// <summary>
        /// The Rank required by the position this board fills
        /// </summary>
        private List<Rank> PositionRanks { get; set; } = [];

        /// <summary>
        /// In-memory list of merit scores for the grid
        /// </summary>
        private List<EvaluationBoardScore> Scores { get; set; } = [];

        /// <summary>
        /// In-memory list of selection filters
        /// </summary>
        private List<SelectionFilter> Filters { get; set; } = [];

        /// <summary>
        /// In-memory list of selection groupings
        /// </summary>
        private List<SelectionGroup> Groups { get; set; } = [];

        /// <summary>
        /// In-memory list of selection sorting rules
        /// </summary>
        private List<SelectionSorting> Sortings { get; set; } = [];

        /// <summary>
        /// In-memory list of allowed candidate rank IDs
        /// </summary>
        private List<int> AllowedRankIds { get; set; } = [];

        /// <summary>
        /// Master constructor — all public constructors funnel into this one.
        /// </summary>
        public EvaluationBoardForm(PositionBlueprint positionBlueprint, EvaluationBoard existing)
        {
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);
            FormStyling.StyleButtonDarkBlue(saveButton);

            Blueprint = positionBlueprint;

            if (existing != null)
            {
                IsNewBoard = false;
                Board = existing;
                LoadBoardIntoForm();
            }
            else
            {
                IsNewBoard = true;
            }

            // Derive ranks from the blueprint
            using var db = new AppDatabase();
            if (Blueprint != null)
            {
                PositionRanks = [ Blueprint.TargetRank ];
            }
            else if (existing != null)
            {
                // Load all the different ranks and store those
                var rankIds = db.PositionBlueprints
                    .FindAll(b => b.EvaluationBoardId == existing.Id)
                    .Select(x => x.TargetRankId);

                PositionRanks = [.. db.Ranks.Where(x => x.Id.In(rankIds))];
            }
            else
            {
                throw new Exception("Both positionBlueprint and existing cannot be null");
            }

            // Setup the rank displays
            SetupScopeDisplay();

            // Recalculate total points based on existing scores
            RecalculateTotalPoints();

            // Register context menu events for Scores
            addScoreMenuItem.Click += AddScoreMenuItem_Click;
            deleteScoreMenuItem.Click += DeleteScoreMenuItem_Click;

            // Register context menu events for Filters
            addFilterMenuItem.Click += AddFilterMenuItem_Click;
            deleteFilterMenuItem.Click += DeleteFilterMenuItem_Click;

            // Register context menu events for Grouping
            addGroupMenuItem.Click += AddGroupMenuItem_Click;
            deleteGroupMenuItem.Click += DeleteGroupMenuItem_Click;

            // Register context menu events for Sorting
            addSortMenuItem.Click += AddSortMenuItem_Click;
            deleteSortMenuItem.Click += DeleteSortMenuItem_Click;

            // Save button
            saveButton.Click += SaveButton_Click;

            // Checkbox toggle events
            factorPerformanceCheckBox.ToggleStateChanged += FactorPerformanceCheckBox_ToggleStateChanged;
            passFailCheckBox.ToggleStateChanged += PassFailCheckBox_ToggleStateChanged;

            // Grid formatting and leave events
            addScoresGridView.CellFormatting += GridView_CellFormatting;
            addScoresGridView.Leave += GridView_Leave;
            filtersGridView.CellFormatting += GridView_CellFormatting;
            filtersGridView.Leave += GridView_Leave;
            groupingGridView.CellFormatting += GridView_CellFormatting;
            groupingGridView.Leave += GridView_Leave;
            sortingGridView.CellFormatting += GridView_CellFormatting;
            sortingGridView.Leave += GridView_Leave;

            // Double-click to edit
            addScoresGridView.DoubleClick += AddScoresGridView_DoubleClick;
            filtersGridView.DoubleClick += FiltersGridView_DoubleClick;
            groupingGridView.DoubleClick += GroupingGridView_DoubleClick;
            sortingGridView.DoubleClick += SortingGridView_DoubleClick;

            // Clear grid focus on init
            RadGridViewHelper.ClearFocusAndSelection(addScoresGridView);
            RadGridViewHelper.ClearFocusAndSelection(filtersGridView);
            RadGridViewHelper.ClearFocusAndSelection(groupingGridView);
            RadGridViewHelper.ClearFocusAndSelection(sortingGridView);
        }

        #region Public Constructors

        /// <summary>
        /// Creates a new EvaluationBoard for a given PositionBlueprint
        /// </summary>
        public EvaluationBoardForm(PositionBlueprint positionBlueprint)
            : this(positionBlueprint, null) { }

        /// <summary>
        /// Edits an existing EvaluationBoard, with PositionBlueprint for faction context
        /// </summary>
        public EvaluationBoardForm(EvaluationBoard existing)
            : this(null, existing) { }

        #endregion

        #region Scope Display

        /// <summary>
        /// Configures the "Board Details" group box to display the rank displays
        /// and populate the allowed ranks dropdown.
        /// </summary>
        private void SetupScopeDisplay()
        {
            if (PositionRanks.Count > 0)
                positionRankDisplay.SetRanks(PositionRanks);

            // Populate allowed ranks dropdown — filtered by faction
            using var db = new AppDatabase();

            int factionId = 0;
            if (Blueprint != null)
            {
                var unitBlueprint = db.UnitBlueprints.Find(Blueprint.UnitBlueprintId);
                factionId = unitBlueprint.FactionId;
            }
            else if (Board != null)
            {
                factionId = Board.FactionId;
            }

            // Get classification IDs for this faction
            var classificationIds = db.RankClassifications
                .Where(rc => rc.FactionId == factionId)
                .Select(rc => rc.Id)
                .ToList();

            // Only show ranks belonging to this faction
            var factionRanks = db.Ranks
                .Where(r => classificationIds.Contains(r.RankClassificationId))
                .ToArray();

            allowedRanksDropDownList.Items.Clear();
            foreach (var rank in factionRanks)
            {
                var item = new RadCheckedListDataItem
                {
                    Tag = rank,
                    Text = rank.Name,
                    Checked = AllowedRankIds.Contains(rank.Id)
                };
                allowedRanksDropDownList.Items.Add(item);
            }

            // Pool selection radio buttons
            if (Board != null)
            {
                if (Board.PoolSelection == PoolSelection.Collective)
                    collectiveRadioButton.IsChecked = true;
                else
                    groupByRankRadioButton.IsChecked = true;
            }

            label6.Text = Board != null
                ? $"Evaluation Board: {Board.Name}"
                : "Evaluation Board (New)";
            
            nameTextBox.Text = Board != null
                ? Board.Name
                : string.Empty;

            UpdateCandidateRankDisplay();
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// Loads an existing board's values into the form controls
        /// </summary>
        private void LoadBoardIntoForm()
        {
            if (Board == null) return;

            // Pool selection
            if (Board.PoolSelection == PoolSelection.Collective)
                collectiveRadioButton.IsChecked = true;
            else
                groupByRankRadioButton.IsChecked = true;

            // Load existing scores
            LoadExistingScores();

            // Load existing filters
            LoadExistingFilters();

            // Load existing groups
            LoadExistingGroups();

            // Load existing sorting
            LoadExistingSorting();

            // Load allowed ranks
            LoadExistingAllowedRanks();
        }

        /// <summary>
        /// Loads existing EvaluationBoardScore records from the database
        /// </summary>
        private void LoadExistingScores()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            var existing = db.EvaluationBoardScores
                .Where(s => s.EvaluationBoardId == Board.Id)
                .ToList();

            foreach (var score in existing)
            {
                Scores.Add(score);

                string functionText = $"{score.Selector}.{(SoldierFunction)score.SelectorId}";

                addScoresGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), score.Operator),
                    score.ExpectedLevel.ToString(),
                    score.Points.ToString(),
                    "0%"
                );
            }
        }

        /// <summary>
        /// Loads existing SelectionFilter records from the database
        /// </summary>
        private void LoadExistingFilters()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            var existing = db.SelectionFilters
                .Where(f => f.EvaluationBoardId == Board.Id)
                .ToList();

            foreach (var filter in existing)
            {
                Filters.Add(filter);

                string functionText = $"{filter.Selector}.{(SoldierFunction)filter.SelectorId}";

                filtersGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), filter.Operator),
                    filter.RightValue.ToString()
                );
            }
        }

        /// <summary>
        /// Loads existing SelectionGroup records from the database
        /// </summary>
        private void LoadExistingGroups()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            var existing = db.SelectionGroups
                .Where(g => g.EvaluationBoardId == Board.Id)
                .ToList();

            foreach (var group in existing)
            {
                Groups.Add(group);

                string functionText = $"{group.Selector}.{(SoldierFunction)group.SelectorId}";

                groupingGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), group.Operator),
                    group.RightValue.ToString()
                );
            }
        }

        /// <summary>
        /// Loads existing SelectionSorting records from the database
        /// </summary>
        private void LoadExistingSorting()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            var existing = db.SelectionSortings
                .Where(s => s.EvaluationBoardId == Board.Id)
                .ToList();

            foreach (var sort in existing)
            {
                Sortings.Add(sort);

                string functionText = $"{sort.Selector}.{(SoldierFunction)sort.SelectorId}";

                sortingGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(Sorting), sort.Direction),
                    ""
                );
            }
        }

        /// <summary>
        /// Loads existing EvaluationBoardRank records from the database
        /// </summary>
        /// <summary>
        /// Loads existing EvaluationBoardRank records from the database.
        /// Only populates AllowedRankIds with the ranks saved for this board,
        /// so that SetupScopeDisplay() checks only those ranks in the dropdown.
        /// </summary>
        private void LoadExistingAllowedRanks()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            AllowedRankIds = db.EvaluationBoardRanks
                .Where(r => r.EvaluationBoardId == Board.Id)
                .Select(r => r.RankId)
                .ToList();
        }

        #endregion

        #region Score Context Menu Handlers

        private void AddScoreMenuItem_Click(object sender, EventArgs e)
        {
            using var frm = new GradedScoreForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                var score = new EvaluationBoardScore
                {
                    Selector = frm.SelectedMethod,
                    SelectorId = (int)frm.SelectedValue,
                    Operator = frm.SelectedOperator,
                    ExpectedLevel = frm.RequiredValue,
                    Points = frm.PointsValue
                };
                Scores.Add(score);

                string functionText = $"{score.Selector}.{(SoldierFunction)score.SelectorId}";

                addScoresGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), score.Operator),
                    score.ExpectedLevel.ToString(),
                    score.Points.ToString(),
                    "0%"
                );

                RecalculateTotalPoints();
            }
        }

        private void DeleteScoreMenuItem_Click(object sender, EventArgs e)
        {
            if (addScoresGridView.SelectedRows.Count == 0) return;

            int rowIndex = addScoresGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Scores.Count) return;

            var result = MessageBox.Show(
                "Are you sure you want to remove this score entry?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            Scores.RemoveAt(rowIndex);
            addScoresGridView.Rows.RemoveAt(rowIndex);

            RecalculateTotalPoints();
        }

        #endregion

        #region Filter Context Menu Handlers

        private void AddFilterMenuItem_Click(object sender, EventArgs e)
        {
            using var frm = new GradedScoreForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                var filter = new SelectionFilter
                {
                    Precedence = Filters.Count,
                    Selector = frm.SelectedMethod,
                    SelectorId = (int)frm.SelectedValue,
                    Operator = frm.SelectedOperator,
                    RightValue = frm.RequiredValue
                };
                Filters.Add(filter);

                string functionText = $"{filter.Selector}.{(SoldierFunction)filter.SelectorId}";

                filtersGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), filter.Operator),
                    filter.RightValue.ToString()
                );
            }
        }

        private void DeleteFilterMenuItem_Click(object sender, EventArgs e)
        {
            if (filtersGridView.SelectedRows.Count == 0) return;

            int rowIndex = filtersGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Filters.Count) return;

            var result = MessageBox.Show(
                "Are you sure you want to remove this filter entry?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            Filters.RemoveAt(rowIndex);
            filtersGridView.Rows.RemoveAt(rowIndex);
        }

        #endregion

        #region Grouping Context Menu Handlers

        private void AddGroupMenuItem_Click(object sender, EventArgs e)
        {
            using var frm = new GradedScoreForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                var group = new SelectionGroup
                {
                    Precedence = Groups.Count,
                    Selector = frm.SelectedMethod,
                    SelectorId = (int)frm.SelectedValue,
                    Operator = frm.SelectedOperator,
                    RightValue = frm.RequiredValue
                };
                Groups.Add(group);

                string functionText = $"{group.Selector}.{(SoldierFunction)group.SelectorId}";

                groupingGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), group.Operator),
                    group.RightValue.ToString()
                );
            }
        }

        private void DeleteGroupMenuItem_Click(object sender, EventArgs e)
        {
            if (groupingGridView.SelectedRows.Count == 0) return;

            int rowIndex = groupingGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Groups.Count) return;

            var result = MessageBox.Show(
                "Are you sure you want to remove this grouping entry?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            Groups.RemoveAt(rowIndex);
            groupingGridView.Rows.RemoveAt(rowIndex);
        }

        #endregion

        #region Sorting Context Menu Handlers

        private void AddSortMenuItem_Click(object sender, EventArgs e)
        {
            using var frm = new GradedScoreForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                var sort = new SelectionSorting
                {
                    Precedence = Sortings.Count,
                    Selector = frm.SelectedMethod,
                    SelectorId = (int)frm.SelectedValue,
                    Direction = Sorting.Ascending
                };
                Sortings.Add(sort);

                string functionText = $"{sort.Selector}.{(SoldierFunction)sort.SelectorId}";

                sortingGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(Sorting), sort.Direction),
                    ""
                );
            }
        }

        private void DeleteSortMenuItem_Click(object sender, EventArgs e)
        {
            if (sortingGridView.SelectedRows.Count == 0) return;

            int rowIndex = sortingGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Sortings.Count) return;

            var result = MessageBox.Show(
                "Are you sure you want to remove this sorting entry?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            Sortings.RemoveAt(rowIndex);
            sortingGridView.Rows.RemoveAt(rowIndex);
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Recalculates the total points for the evaluation board by summing up
        /// all score entries. Updates the total points display and percentages.
        /// </summary>
        private void RecalculateTotalPoints()
        {
            int total = Scores.Sum(s => s.Points);

            if (factorPerformanceCheckBox.Checked)
                total += (int)factorPerformanceSpinEditor.Value;

            totalPointsSpinEditor.Value = total;
            RecalculatePercentages();
        }

        /// <summary>
        /// Recalculates and updates the percentage values for each score in the grid.
        /// </summary>
        private void RecalculatePercentages()
        {
            int totalPoints = (int)totalPointsSpinEditor.Value;

            for (int i = 0; i < Scores.Count && i < addScoresGridView.Rows.Count; i++)
            {
                double pct = totalPoints > 0
                    ? (Scores[i].Points / (double)totalPoints) * 100.0
                    : 0;
                addScoresGridView.Rows[i].Cells["column4"].Value = $"{Scores[i].Points}";
                addScoresGridView.Rows[i].Cells["column5"].Value = $"{pct:F1}%";
            }

            // Adjust the width of the "Function" column to fit the content
            RadGridViewHelper.AdjustColumnForScrollBar(addScoresGridView, "column1", 235);
        }

        #endregion

        #region Control Toggle Logic

        private void FactorPerformanceCheckBox_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            factorScaleTrackBar.Enabled = factorPerformanceCheckBox.Checked;
            factorPerformanceSpinEditor.Enabled = factorPerformanceCheckBox.Checked;
            RecalculateTotalPoints();
        }

        private void PassFailCheckBox_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            percentageModelTrackBar.Enabled = passFailCheckBox.Checked;
        }

        private void AddScoresGridView_DoubleClick(object sender, EventArgs e)
        {
            if (addScoresGridView.SelectedRows.Count == 0) return;

            int rowIndex = addScoresGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Scores.Count) return;

            var current = Scores[rowIndex];

            using var frm = new GradedScoreForm(
                current.Selector, (SoldierFunction)current.SelectorId, current.Operator,
                current.ExpectedLevel, current.Points);

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                current.Selector = frm.SelectedMethod;
                current.SelectorId = (int)frm.SelectedValue;
                current.Operator = frm.SelectedOperator;
                current.ExpectedLevel = frm.RequiredValue;
                current.Points = frm.PointsValue;

                string functionText = $"{current.Selector}.{(SoldierFunction)current.SelectorId}";

                addScoresGridView.Rows[rowIndex].Cells["column1"].Value = functionText;
                addScoresGridView.Rows[rowIndex].Cells["column2"].Value = Enum.GetName(typeof(ComparisonOperator), current.Operator);
                addScoresGridView.Rows[rowIndex].Cells["column3"].Value = current.ExpectedLevel.ToString();
                addScoresGridView.Rows[rowIndex].Cells["column4"].Value = current.Points.ToString();

                RecalculateTotalPoints();
            }
        }

        private void FiltersGridView_DoubleClick(object sender, EventArgs e)
        {
            if (filtersGridView.SelectedRows.Count == 0) return;

            int rowIndex = filtersGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Filters.Count) return;

            var current = Filters[rowIndex];

            using var frm = new GradedScoreForm(
                current.Selector, (SoldierFunction)current.SelectorId, current.Operator,
                current.RightValue, 0);

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                current.Selector = frm.SelectedMethod;
                current.SelectorId = (int)frm.SelectedValue;
                current.Operator = frm.SelectedOperator;
                current.RightValue = frm.RequiredValue;

                string functionText = $"{current.Selector}.{(SoldierFunction)current.SelectorId}";

                filtersGridView.Rows[rowIndex].Cells["column1"].Value = functionText;
                filtersGridView.Rows[rowIndex].Cells["column2"].Value = Enum.GetName(typeof(ComparisonOperator), current.Operator);
                filtersGridView.Rows[rowIndex].Cells["column3"].Value = current.RightValue.ToString();
            }
        }

        private void GroupingGridView_DoubleClick(object sender, EventArgs e)
        {
            if (groupingGridView.SelectedRows.Count == 0) return;

            int rowIndex = groupingGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Groups.Count) return;

            var current = Groups[rowIndex];

            using var frm = new GradedScoreForm(
                current.Selector, (SoldierFunction)current.SelectorId, current.Operator,
                current.RightValue, 0);

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                current.Selector = frm.SelectedMethod;
                current.SelectorId = (int)frm.SelectedValue;
                current.Operator = frm.SelectedOperator;
                current.RightValue = frm.RequiredValue;

                string functionText = $"{current.Selector}.{(SoldierFunction)current.SelectorId}";

                groupingGridView.Rows[rowIndex].Cells["column1"].Value = functionText;
                groupingGridView.Rows[rowIndex].Cells["column2"].Value = Enum.GetName(typeof(ComparisonOperator), current.Operator);
                groupingGridView.Rows[rowIndex].Cells["column3"].Value = current.RightValue.ToString();
            }
        }

        private void SortingGridView_DoubleClick(object sender, EventArgs e)
        {
            if (sortingGridView.SelectedRows.Count == 0) return;

            int rowIndex = sortingGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Sortings.Count) return;

            var current = Sortings[rowIndex];

            using var frm = new GradedScoreForm(
                current.Selector, (SoldierFunction)current.SelectorId, ComparisonOperator.Equals,
                0, 0);

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                current.Selector = frm.SelectedMethod;
                current.SelectorId = (int)frm.SelectedValue;

                string functionText = $"{current.Selector}.{(SoldierFunction)current.SelectorId}";

                sortingGridView.Rows[rowIndex].Cells["column1"].Value = functionText;
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Handles the click event for the Save button, performing validation, saving the current data to the database,
        /// and managing transactions. Displays a validation message if required fields are not provided.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Save button.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                RadMessageBox.Show("Please enter a board name.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }
            
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                if (IsNewBoard)
                    Board = db.EvaluationBoards.Create();

                // Set board properties
                Board.Name = nameTextBox.Text.Trim();
                Board.PoolSelection = collectiveRadioButton.IsChecked
                    ? PoolSelection.Collective
                    : PoolSelection.OrderedPriority;

                // Insert or Update
                if (IsNewBoard)
                {
                    // Set FactionId from the PositionBlueprint's UnitBlueprint
                    if (Blueprint != null)
                    {
                        var unitBlueprint = db.UnitBlueprints.Find(Blueprint.UnitBlueprintId);
                        Board.FactionId = unitBlueprint.FactionId;
                    }
                    
                    db.EvaluationBoards.Add(Board);
                    IsNewBoard = false;
                }
                else
                {
                    db.EvaluationBoards.Update(Board);
                }

                // --- Scores: delete old, re-insert current ---
                db.EvaluationBoardScores.RemoveWhere(s => s.EvaluationBoardId == Board.Id);

                foreach (var score in Scores)
                {
                    var entity = db.EvaluationBoardScores.Create();
                    entity.EvaluationBoardId = Board.Id;
                    entity.Selector = score.Selector;
                    entity.SelectorId = score.SelectorId;
                    entity.Operator = score.Operator;
                    entity.ExpectedLevel = score.ExpectedLevel;
                    entity.Points = score.Points;
                    db.EvaluationBoardScores.Add(entity);
                }

                // --- Filters: delete old, re-insert current ---
                db.SelectionFilters.RemoveWhere(f => f.EvaluationBoardId == Board.Id);

                for (int i = 0; i < Filters.Count; i++)
                {
                    var filter = Filters[i];
                    var entity = db.SelectionFilters.Create();
                    entity.EvaluationBoardId = Board.Id;
                    entity.Precedence = i;
                    entity.Selector = filter.Selector;
                    entity.SelectorId = filter.SelectorId;
                    entity.Operator = filter.Operator;
                    entity.RightValue = filter.RightValue;
                    db.SelectionFilters.Add(entity);
                }

                // --- Groups: delete old, re-insert current ---
                db.SelectionGroups.RemoveWhere(g => g.EvaluationBoardId == Board.Id);

                for (int i = 0; i < Groups.Count; i++)
                {
                    var group = Groups[i];
                    var entity = db.SelectionGroups.Create();
                    entity.EvaluationBoardId = Board.Id;
                    entity.Precedence = i;
                    entity.Selector = group.Selector;
                    entity.SelectorId = group.SelectorId;
                    entity.Operator = group.Operator;
                    entity.RightValue = group.RightValue;
                    db.SelectionGroups.Add(entity);
                }

                // --- Sorting: delete old, re-insert current ---
                db.SelectionSortings.RemoveWhere(s => s.EvaluationBoardId == Board.Id);

                for (int i = 0; i < Sortings.Count; i++)
                {
                    var sort = Sortings[i];
                    var entity = db.SelectionSortings.Create();
                    entity.EvaluationBoardId = Board.Id;
                    entity.Precedence = i;
                    entity.Selector = sort.Selector;
                    entity.SelectorId = sort.SelectorId;
                    entity.Direction = sort.Direction;
                    db.SelectionSortings.Add(entity);
                }

                // --- Allowed Ranks: delete old, re-insert checked ---
                db.EvaluationBoardRanks.RemoveWhere(r => r.EvaluationBoardId == Board.Id);

                foreach (RadCheckedListDataItem item in allowedRanksDropDownList.Items)
                {
                    if (item.Checked && item.Tag is Rank rank)
                    {
                        var entity = db.EvaluationBoardRanks.Create();
                        entity.EvaluationBoardId = Board.Id;
                        entity.RankId = rank.Id;
                        db.EvaluationBoardRanks.Add(entity);
                    }
                }

                transaction.Commit();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Failed to save evaluation board: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Grid Helpers

        /// <summary>
        /// Prevents the selected cell border and "pop" out
        /// </summary>
        private void GridView_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            RadGridViewHelper.RemoveSelectedCellBorderAndPop(e);
        }

        /// <summary>
        /// Clears the selected row on the grid view when an external control is selected
        /// </summary>
        private void GridView_Leave(object sender, EventArgs e)
        {
            var gridView = (RadGridView)sender;
            RadGridViewHelper.ClearFocusAndSelection(gridView);
        }

        #endregion

        private void AllowedRanksDropDownList_ItemCheckedChanged(object sender, RadCheckedListDataItemEventArgs e)
        {
            UpdateCandidateRankDisplay();
        }

        private void UpdateCandidateRankDisplay()
        {
            var checkedRanks = allowedRanksDropDownList.Items
                .OfType<RadCheckedListDataItem>()
                .Where(item => item.Checked && item.Tag is Rank)
                .Select(item => (Rank)item.Tag)
                .ToList();

            if (checkedRanks.Count > 0)
                candidateRankDisplay.SetRanks(checkedRanks);
            else
                candidateRankDisplay.SetRanks([]);
        }

        /// <summary>
        /// Adds the darker border line color between the header panel and the contents
        /// panel
        /// </summary>
        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
        }

        private void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
        }
    }
}