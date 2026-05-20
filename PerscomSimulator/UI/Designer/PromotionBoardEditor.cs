using Perscom.Database;
using Perscom.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class PromotionBoardEditor : RadForm
    {
        /// <summary>
        /// The promotion board entity being edited (null until first save for new boards)
        /// </summary>
        private PromotionBoard Board { get; set; }

        /// <summary>
        /// Indicates whether we are creating a new board or editing an existing one
        /// </summary>
        private bool IsNewBoard { get; set; }

        /// <summary>
        /// The Rank this board is scoped to (null if scoped to RankClassification instead)
        /// </summary>
        private Rank ScopedRank { get; set; }

        /// <summary>
        /// The RankClassification this board is scoped to (null if scoped to a specific Rank)
        /// </summary>
        private RankClassification ScopedClassification { get; set; }

        /// <summary>
        /// The optional Occupation override (null = default board for this rank/classification)
        /// </summary>
        private Occupation ScopedOccupation { get; set; }

        /// <summary>
        /// In-memory list of attribute weights for the grid
        /// </summary>
        private List<PromotionBoardWeight> Weights { get; set; } = [];

        /// <summary>
        /// Represents a collection of additional scores associated with the promotion board.
        /// </summary>
        private List<PromotionBoardAddScore> AdditionalScores { get; set; } = [];

        /// <summary>
        /// Master constructor — all public constructors funnel into this one.
        /// Exactly one of <paramref name="rank"/> or <paramref name="rankClassification"/> 
        /// must be non-null (unless editing an existing board).
        /// </summary>
        private PromotionBoardEditor(Rank rank, RankClassification rankClassification,
            Occupation occupation, PromotionBoard existing)
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            // For some reason these aren't a part of the forms Controls
            gradedItemsContextMenu.ThemeName = Program.ThemeName;
            additionalContextMenu.ThemeName = Program.ThemeName;

            // Button styling
            FormStyling.StyleButtonDarkBlue(saveButton);
            FormStyling.StyleButtonRed(deleteButton);

            // Fill board type dropdown
            foreach (PromotionBoardType item in Enum.GetValues(typeof(PromotionBoardType)))
            {
                boardTypeDropDownList.Items.Add(new RadListDataItem
                {
                    Tag = item,
                    Text = Enum.GetName(typeof(PromotionBoardType), item)
                });
            }
            boardTypeDropDownList.SelectedIndex = 0;

            // Existing board?
            if (existing != null)
            {
                // --- EDIT MODE ---
                IsNewBoard = false;
                Board = existing;
                deleteButton.Visible = true;

                // Resolve the scope from the existing entity's FK values
                using var db = new AppDatabase();
                if (Board.RankId.HasValue)
                    ScopedRank = db.Ranks.Find(Board.RankId.Value);

                if (Board.RankClassificationId.HasValue)
                    ScopedClassification = db.RankClassifications.Find(Board.RankClassificationId.Value);

                if (Board.OccupationId.HasValue)
                    ScopedOccupation = db.Occupations.Find(Board.OccupationId.Value);

                // Load existing board values into controls
                LoadBoardIntoForm();
            }
            else
            {
                // --- NEW BOARD MODE ---
                IsNewBoard = true;
                deleteButton.Visible = false;

                ScopedRank = rank;
                ScopedClassification = rankClassification;
                ScopedOccupation = occupation;

                // If a Rank was provided but no explicit classification, 
                // grab the classification from the rank for display purposes
                if (ScopedRank != null && ScopedClassification == null)
                {
                    ScopedClassification = ScopedRank.Classification;
                }
            }

            // Lock down the "Board Details" section — user cannot change scope
            SetupScopeDisplay();

            RecalculateTotalPoints();

            // Register for events
            addAttrMenuItem.Click += AddAttrMenuItem_Click;
            editAttrMenuItem.Click += EditAttrMenuItem_Click;
            deleteAttrMenuItem.Click += DeleteAttrMenuItem_Click;
            addScoreMenuItem.Click += AddScoreMenuItem_Click;
            editScoreMenuItem.Click += EditScoreMenuItem_Click;
            deleteScoreMenuItem.Click += DeleteScoreMenuItem_Click;

            // Context menu enable/disable logic
            gradedItemsContextMenu.DropDownOpening += GradedItemsContextMenu_DropDownOpening;
            additionalContextMenu.DropDownOpening += AdditionalContextMenu_DropDownOpening;
        }

        private void DeleteScoreMenuItem_Click(object sender, EventArgs e)
        {
            if (addScoresGridView.SelectedRows.Count == 0) return;

            int rowIndex = addScoresGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= AdditionalScores.Count) return;

            var result = MessageBox.Show(
                "Are you sure you want to remove this score entry?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            AdditionalScores.RemoveAt(rowIndex);
            addScoresGridView.Rows.RemoveAt(rowIndex);

            RecalculateTotalPoints();
        }

        private void DeleteAttrMenuItem_Click(object sender, EventArgs e)
        {
            if (attrWeightsGridView.SelectedRows.Count == 0) return;

            int rowIndex = attrWeightsGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Weights.Count) return;

            var result = MessageBox.Show(
                "Are you sure you want to remove this attribute weight?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            Weights.RemoveAt(rowIndex);
            attrWeightsGridView.Rows.RemoveAt(rowIndex);

            RecalculateTotalPoints();
        }

        private void EditScoreMenuItem_Click(object sender, EventArgs e)
        {
            if (addScoresGridView.SelectedRows.Count == 0) return;

            int rowIndex = addScoresGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= AdditionalScores.Count) return;

            var current = AdditionalScores[rowIndex];

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

        private void AddScoreMenuItem_Click(object sender, EventArgs e)
        {
            using var frm = new GradedScoreForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                var score = new PromotionBoardAddScore
                {
                    Selector = frm.SelectedMethod,
                    SelectorId = (int)frm.SelectedValue,
                    Operator = frm.SelectedOperator,
                    ExpectedLevel = frm.RequiredValue,
                    Points = frm.PointsValue
                };
                AdditionalScores.Add(score);

                string functionText = $"{score.Selector}.{(SoldierFunction)score.SelectorId}";

                addScoresGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), score.Operator),
                    score.ExpectedLevel.ToString(),
                    score.Points.ToString()
                );

                RecalculateTotalPoints();
            }
        }

        /// <summary>
        /// Handles the click event for editing a selected attribute menu item.
        /// Allows editing the selected item's properties within the grid, including its attribute
        /// and weight, while excluding already used attributes from selection.
        /// </summary>
        /// <param name="sender">The source of the event, typically the menu item that triggered the method.</param>
        /// <param name="e">Provides the event data associated with the click action.</param>
        private void EditAttrMenuItem_Click(object sender, EventArgs e)
        {
            if (attrWeightsGridView.SelectedRows.Count == 0) return;

            int rowIndex = attrWeightsGridView.SelectedRows[0].Index;
            if (rowIndex < 0 || rowIndex >= Weights.Count) return;

            var current = Weights[rowIndex];

            // Exclude all used attributes except the one currently being edited
            var excludedAttributes = Weights
                .Where(w => w.Attribute != current.Attribute)
                .Select(w => w.Attribute);

            using var frm = new GradedAttributeForm(current.Attribute, current.ExpectedLevel, current.Points, excludedAttributes);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                current.Attribute = frm.SelectedAttribute;
                current.ExpectedLevel = frm.ExpectedLevel;
                current.Points = frm.MaxPoints;

                attrWeightsGridView.Rows[rowIndex].Cells["column1"].Value = Enum.GetName(typeof(AttributeType), current.Attribute);
                attrWeightsGridView.Rows[rowIndex].Cells["column2"].Value = current.ExpectedLevel.ToString();
                attrWeightsGridView.Rows[rowIndex].Cells["column3"].Value = current.Points.ToString();

                RecalculateTotalPoints();
            }
        }

        /// <summary>
        /// Handles the event triggered when the "Add Attribute" menu item is clicked.
        /// Prompts the user to select an attribute and configure its weight,
        /// then adds the new attribute along with its weight to the list of weights
        /// and updates the attribute weights grid and total points.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object containing event data.</param>
        private void AddAttrMenuItem_Click(object sender, EventArgs e)
        {
            var usedAttributes = Weights.Select(w => w.Attribute);

            using var frm = new GradedAttributeForm(usedAttributes);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                var weight = new PromotionBoardWeight
                {
                    Attribute = frm.SelectedAttribute,
                    ExpectedLevel = frm.ExpectedLevel,
                    Points = frm.MaxPoints
                };
                Weights.Add(weight);

                attrWeightsGridView.Rows.Add(
                    Enum.GetName(typeof(AttributeType), weight.Attribute),
                    weight.ExpectedLevel.ToString(),
                    weight.Points.ToString(),
                    "0%"
                );

                RecalculateTotalPoints();
            }
        }

        #region Public Constructors

        /// <summary>
        /// Creates a new board scoped to a specific Rank
        /// </summary>
        public PromotionBoardEditor(Rank rank)
            : this(rank, null, null, null) { }

        /// <summary>
        /// Creates a new board scoped to a specific Rank with an Occupation override
        /// </summary>
        public PromotionBoardEditor(Rank rank, Occupation occupation)
            : this(rank, null, occupation, null) { }

        /// <summary>
        /// Creates a new board scoped to a RankClassification (pay-grade fallback)
        /// </summary>
        public PromotionBoardEditor(RankClassification classification)
            : this(null, classification, null, null) { }

        /// <summary>
        /// Creates a new board scoped to a RankClassification with an Occupation override
        /// </summary>
        public PromotionBoardEditor(RankClassification classification, Occupation occupation)
            : this(null, classification, occupation, null) { }

        /// <summary>
        /// Edits an existing PromotionBoard entity
        /// </summary>
        public PromotionBoardEditor(PromotionBoard existing)
            : this(null, null, null, existing) { }

        #endregion

        #region Scope Display (Read-Only)

        /// <summary>
        /// Configures the "Board Details" group box to display the locked-in scope.
        /// All scope controls are set to read-only / disabled so the user cannot change them.
        /// </summary>
        private void SetupScopeDisplay()
        {
            if (ScopedClassification != null)
            {
                radLabel2.Text = $"Promotion To Pay Grade: {ScopedClassification}";
                rankPictureDisplayBox.SetClassification(ScopedClassification);
            }

            // --- Occupation dropdown: display-only ---
            occupationDropDownList.Items.Clear();
            if (ScopedOccupation != null)
            {
                occupationDropDownList.Items.Add(new RadListDataItem
                {
                    Tag = ScopedOccupation,
                    Text = ScopedOccupation.ToString()
                });
                occupationDropDownList.SelectedIndex = 0;
            }
            else
            {
                occupationDropDownList.Items.Add(new RadListDataItem
                {
                    Tag = null,
                    Text = "(Default — No Occupation Override)"
                });
                occupationDropDownList.SelectedIndex = 0;
            }
            occupationDropDownList.ReadOnly = true; //  Locked

            // --- "Apply to all ranks in pay grade" checkbox: display-only ---
            // Checked = scoped to classification without a specific rank
            applyAllRanksInGradeCheckBox.Checked = (ScopedRank == null && ScopedClassification != null);
            applyAllRanksInGradeCheckBox.Enabled = false; // locked

            // Update header label
            string scopeText = ScopedRank != null
                ? $"Promotion Board Editor for {ScopedRank.Name}"
                : $"Promotion Board Editor for {ScopedClassification}";
            if (ScopedOccupation != null)
                scopeText += $" ({ScopedOccupation.Code} {ScopedOccupation.Name})";
            headerLabel.Text = scopeText;
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// Loads an existing board's values into the form controls
        /// </summary>
        private void LoadBoardIntoForm()
        {
            if (Board == null) return;

            boardTypeDropDownList.SelectedIndex = (int)Board.Type;

            passFailCheckBox.Checked = Board.IsPassFail;
            percentageTrackBar.Value = Board.PassThreshold;

            // Time in Grade
            bool hasTig = Board.TimeInGradeFactor > 0 || Board.TimeInGradeMaxPoints > 0;
            tigCheckBox.Checked = hasTig;
            if (hasTig)
            {
                factorScaleTrackBar.Value = Board.TimeInGradeFactor;
                tigMaxSpinEditor.Value = Board.TimeInGradeMaxPoints;
            }

            // Form Rating
            bool hasForm = Board.FormRatingMaxPoints > 0;
            formScaleCheckBox.Checked = hasForm;
            if (hasForm)
            {
                formRatingRadSpinEditor.Value = Board.FormRatingMaxPoints;
            }
            
            // Expiry
            bool hasExpiry = Board.PromotableLength > 0;
            expiresCheckBox.Checked = hasExpiry;
            if (hasExpiry)
            {
                promtableLenSpinEditor.Value = Board.PromotableLength;
            }

            // Load existing weights and scores
            LoadExistingWeights();

            // Load existing additional scores
            LoadExistingAddScores();
        }

        /// <summary>
        /// Loads existing PromotionBoardWeight records from the database
        /// </summary>
        private void LoadExistingWeights()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            var existingWeights = db.PromotionBoardWeights.FindAll(Board.Id).ToArray();

            foreach (var weight in existingWeights)
            {
                Weights.Add(weight);

                attrWeightsGridView.Rows.Add(
                    Enum.GetName(typeof(AttributeType), weight.Attribute),
                    weight.ExpectedLevel.ToString(),
                    weight.Points.ToString(),
                    "0%"
                );
            }
        }

        /// <summary>
        /// Loads existing PromotionBoardAddScore records from the database
        /// </summary>
        private void LoadExistingAddScores()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            var existing = db.PromotionBoardAddScores.FindAll(Board.Id).ToList();

            foreach (var score in existing)
            {
                AdditionalScores.Add(score);

                string functionText = $"{score.Selector}.{(SoldierFunction)score.SelectorId}";

                addScoresGridView.Rows.Add(
                    functionText,
                    Enum.GetName(typeof(ComparisonOperator), score.Operator),
                    score.ExpectedLevel.ToString(),
                    score.Points.ToString()
                );
            }
        }

        #endregion

        #region Calculations

        private void RecalculateTotalPoints()
        {
            int total = Weights.Sum(w => w.Points);
            total += AdditionalScores.Sum(s => s.Points);

            if (tigCheckBox.Checked)
                total += (int)tigMaxSpinEditor.Value;

            if (formScaleCheckBox.Checked)
                total += (int)formRatingRadSpinEditor.Value;

            TotalPointsSpinEditor.Value = total;
            RecalculatePercentages();
        }

        private void RecalculatePercentages()
        {
            int totalPoints = (int)TotalPointsSpinEditor.Value;

            for (int i = 0; i < Weights.Count && i < attrWeightsGridView.Rows.Count; i++)
            {
                double pct = totalPoints > 0
                    ? (Weights[i].Points / (double)totalPoints) * 100.0
                    : 0;
                attrWeightsGridView.Rows[i].Cells["column4"].Value = $"{pct:F1}%";
            }

            UpdateAdditionalScoresGrid(totalPoints);
        }

        private void UpdateAdditionalScoresGrid(int totalPoints)
        {

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
            FormStyling.StyleFormFooterDarker(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion

        #region Control Toggle Logic

        private void tigCheckBox_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            factorScaleTrackBar.Enabled = tigCheckBox.Checked;
            tigMaxSpinEditor.Enabled = tigCheckBox.Checked;
            RecalculateTotalPoints();
        }

        private void formRatingCheckBox_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            formRatingRadSpinEditor.Enabled = formScaleCheckBox.Checked;
            if (!formScaleCheckBox.Checked)
                formRatingRadSpinEditor.Value = 0; // This will trigger the ValueChanged event, which will recalculate total points
            else
                RecalculateTotalPoints();
        }
        
        private void expiresCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            promtableLenSpinEditor.Enabled = expiresCheckBox.Checked;
            promtableLenSpinEditor.Value = !expiresCheckBox.Checked ? 0 : 12;
        }

        private void boardTypeDropDownList_SelectedIndexChanged(object sender,
            Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (boardTypeDropDownList.SelectedIndex > 0)
            {
                passFailCheckBox.Checked = true;
                passFailCheckBox.Enabled = false;
            }
            else
            {
                passFailCheckBox.Enabled = true;
            }
        }

        private void PassFailCheckBox_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            percentageTrackBar.Enabled = passFailCheckBox.Checked;
        }

        private void TigMaxSpinEditor_ValueChanged(object sender, EventArgs e)
            => RecalculateTotalPoints();

        private void formRatingSpinEditor_ValueChanged(object sender, EventArgs e)
            => RecalculateTotalPoints();

        private void attrWeightsGridView_DoubleClick(object sender, EventArgs e)
        {
            EditAttrMenuItem_Click(sender, e);
        }

        private void GradedItemsContextMenu_DropDownOpening(object sender, CancelEventArgs e)
        {
            bool hasSelection = attrWeightsGridView.SelectedRows.Count > 0
                                && attrWeightsGridView.SelectedRows[0].Index >= 0;

            editAttrMenuItem.Enabled = hasSelection;
            deleteAttrMenuItem.Enabled = hasSelection;
        }

        private void AdditionalContextMenu_DropDownOpening(object sender, CancelEventArgs e)
        {
            bool hasSelection = addScoresGridView.SelectedRows.Count > 0
                                && addScoresGridView.SelectedRows[0].Index >= 0;

            editScoreMenuItem.Enabled = hasSelection;
            deleteScoreMenuItem.Enabled = hasSelection;
        }

        #endregion

        #region Save / Cancel / Delete

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                if (IsNewBoard)
                    Board = db.PromotionBoards.Create();

                // Set the locked scope from constructor params
                if (ScopedRank != null)
                {
                    Board.RankId = ScopedRank.Id;
                    Board.RankClassificationId = null;
                }
                else if (ScopedClassification != null)
                {
                    Board.RankId = null;
                    Board.RankClassificationId = ScopedClassification.Id;
                }

                Board.OccupationId = ScopedOccupation?.Id;

                // Auto-generate a name from the scope
                Board.Name = ScopedRank != null
                    ? $"{ScopedRank.Abbreviation} Promotion Board"
                    : $"{ScopedClassification} Promotion Board";

                if (ScopedOccupation != null)
                    Board.Name += $" ({ScopedOccupation.Code})";

                Board.Type = (PromotionBoardType)boardTypeDropDownList.SelectedItem.Tag;
                Board.IsPassFail = passFailCheckBox.Checked;
                Board.PassThreshold = (int)percentageTrackBar.Value;
                
                // Form Rating
                if (formScaleCheckBox.Checked)
                {
                    Board.FormRatingMaxPoints = (int)formRatingRadSpinEditor.Value;
                }
                else
                {
                    Board.FormRatingMaxPoints = 0;
                }

                // Promotable Length
                Board.PromotableLength = (int)promtableLenSpinEditor.Value;

                if (tigCheckBox.Checked)
                {
                    Board.TimeInGradeFactor = (int)factorScaleTrackBar.Value;
                    Board.TimeInGradeMaxPoints = (int)tigMaxSpinEditor.Value;
                }
                else
                {
                    Board.TimeInGradeFactor = 0;
                    Board.TimeInGradeMaxPoints = 0;
                }

                // Insert or Update
                if (IsNewBoard)
                {
                    db.PromotionBoards.Add(Board);
                    IsNewBoard = false;
                }
                else
                {
                    db.PromotionBoards.Update(Board);
                }

                // Before re-inserting weights and scores:
                db.PromotionBoardWeights.Remove(w => w.PromotionBoardId == Board.Id);
                db.PromotionBoardAddScores.Remove(s => s.PromotionBoardId == Board.Id);

                // Delete old weights, re-insert current
                foreach (var weight in Weights.Where(w => w.ExpectedLevel > 0))
                {
                    var entity = db.PromotionBoardWeights.Create();
                    entity.PromotionBoardId = Board.Id;
                    entity.Attribute = weight.Attribute;
                    entity.ExpectedLevel = weight.ExpectedLevel;
                    entity.Points = weight.Points;
                    db.PromotionBoardWeights.Add(entity);
                }

                // Delete old additional scores, re-insert current
                foreach (var score in AdditionalScores)
                {
                    var entity = db.PromotionBoardAddScores.Create();
                    entity.PromotionBoardId = Board.Id;
                    entity.Selector = score.Selector;
                    entity.SelectorId = score.SelectorId;
                    entity.Operator = score.Operator;
                    entity.ExpectedLevel = score.ExpectedLevel;
                    entity.Points = score.Points;
                    db.PromotionBoardAddScores.Add(entity);
                }

                transaction.Commit();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Failed to save promotion board: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Board == null || IsNewBoard) return;

            var result = MessageBox.Show(
                "Are you sure you want to delete this promotion board?\n" +
                "This will also remove all associated weights and promotable candidates.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                // Clear all references to this board and delete the board itself
                db.PromotionBoardWeights.Remove(w => w.PromotionBoardId == Board.Id);
                db.PromotionBoardAddScores.Remove(s => s.PromotionBoardId == Board.Id);
                db.PromotionBoards.Remove(Board);

                // Commit the transaction
                transaction.Commit();
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Failed to delete promotion board: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        /// <summary>
        /// Prevents the selected cell border and "pop" out
        /// </summary>
        private void GridView_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            if (e.CellElement.IsCurrent)
            {
                // 1. Keep the border enabled so the layout engine doesn't recalculate the size
                e.CellElement.DrawBorder = true;

                // 2. Make the focus border invisible
                e.CellElement.BorderColor = Color.Transparent;

                // 3. Force the style to match standard cells so it doesn't try to draw a 3D focus box
                e.CellElement.BorderBoxStyle = BorderBoxStyle.SingleBorder;
                e.CellElement.BorderGradientStyle = GradientStyles.Solid;
            }
            else
            {
                // Restore default theme behavior for non-current cells
                e.CellElement.ResetValue(LightVisualElement.DrawBorderProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BorderColorProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BorderBoxStyleProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BorderGradientStyleProperty, ValueResetFlags.Local);
            }
        }

        /// <summary>
        /// Clears the selected row on the grid view when an external control is selected
        /// </summary>
        private void GridView_Leave(object sender, EventArgs e)
        {
            var gridView = (RadGridView)sender;

            // Clears all highlighted selections
            gridView.ClearSelection();

            // Removes the internal "active" pointer so the theme completely lets go of the row
            gridView.CurrentRow = null;
        }
    }
}