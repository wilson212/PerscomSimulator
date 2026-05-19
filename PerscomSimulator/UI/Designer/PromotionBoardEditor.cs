using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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

            // For some reason these arent apart of the forms Controls
            gradedItemsContextMenu.ThemeName = "FluentPerscomBlue";
            additionalContextMenu.ThemeName = "FluentPerscomBlue";

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

            // Context menu enable/disable logic
            gradedItemsContextMenu.DropDownOpening += GradedItemsContextMenu_DropDownOpening;
            additionalContextMenu.DropDownOpening += AdditionalContextMenu_DropDownOpening;
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

            using var frm = new GradedAttributeForm(current.Attribute, current.Weight, excludedAttributes);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                current.Attribute = frm.SelectedAttribute;
                current.Weight = frm.MaxPoints;

                attrWeightsGridView.Rows[rowIndex].Cells["column1"].Value =
                    Enum.GetName(typeof(AttributeType), current.Attribute);
                attrWeightsGridView.Rows[rowIndex].Cells["column2"].Value =
                    current.Weight.ToString();

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
                    Weight = frm.MaxPoints
                };
                Weights.Add(weight);

                attrWeightsGridView.Rows.Add(
                    Enum.GetName(typeof(AttributeType), weight.Attribute),
                    weight.Weight.ToString(),
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
            // --- Rank image + label ---
            if (ScopedRank != null)
            {
                radLabel2.Text = $"Promotion To Rank: {ScopedRank.Abbreviation} - {ScopedRank.Name}";
                if (!string.IsNullOrEmpty(ScopedRank.Image))
                    radPictureBox1.Image = ImageAccessor.GetImage(Path.Combine("Large", ScopedRank.Image));
            }
            else if (ScopedClassification != null)
            {
                radLabel2.Text = $"Promotion To Pay Grade: {ScopedClassification}";
                radPictureBox1.Image = null;
            }

            // --- Occupation dropdown: display-only ---
            radDropDownList1.Items.Clear();
            if (ScopedOccupation != null)
            {
                radDropDownList1.Items.Add(new RadListDataItem
                {
                    Tag = ScopedOccupation,
                    Text = ScopedOccupation.ToString()
                });
                radDropDownList1.SelectedIndex = 0;
            }
            else
            {
                radDropDownList1.Items.Add(new RadListDataItem
                {
                    Tag = null,
                    Text = "(Default — No Occupation Override)"
                });
                radDropDownList1.SelectedIndex = 0;
            }
            radDropDownList1.Enabled = false; // locked

            // --- "Apply to all ranks in pay grade" checkbox: display-only ---
            // Checked = scoped to classification without a specific rank
            radCheckBox1.Checked = (ScopedRank == null && ScopedClassification != null);
            radCheckBox1.Enabled = false; // locked

            // Update header label
            string scopeText = ScopedRank != null
                ? $"Promotion Board Editor — {ScopedRank.Abbreviation}"
                : $"Promotion Board Editor — {ScopedClassification}";
            if (ScopedOccupation != null)
                scopeText += $" ({ScopedOccupation.Code})";
            label6.Text = scopeText;
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

            bool hasTig = Board.TimeInGradeFactor > 0 || Board.TimeInGradeMaxPoints > 0;
            tigCheckBox.Checked = hasTig;
            if (hasTig)
            {
                factorScaleTrackBar.Value = Board.TimeInGradeFactor;
                tigMaxSpinEditor.Value = Board.TimeInGradeMaxPoints;
            }

            // Form Rating (requires FormRatingFactor / FormRatingMaxPoints columns)
            // bool hasForm = Board.FormRatingFactor > 0 || Board.FormRatingMaxPoints > 0;
            // formScaleCheckBox.Checked = hasForm;
            // if (hasForm)
            // {
            //     formRatingTrackBar.Value = Board.FormRatingFactor;
            //     precedenceSpinEditor.Value = Board.FormRatingMaxPoints;
            // }

            LoadExistingWeights();
        }

        /// <summary>
        /// Loads existing PromotionBoardWeight records from the database
        /// </summary>
        private void LoadExistingWeights()
        {
            if (Board == null) return;

            using var db = new AppDatabase();
            var existingWeights = db.PromotionBoardWeights.FindAll(Board.Id).ToArray();

            for (int i = 0; i < Weights.Count; i++)
            {
                var existing = existingWeights.FirstOrDefault(w => w.Attribute == Weights[i].Attribute);
                if (existing != null)
                {
                    Weights[i] = existing;
                    attrWeightsGridView.Rows[i].Cells["column2"].Value = existing.Weight.ToString();
                }
            }
        }

        #endregion

        #region Calculations

        private void RecalculateTotalPoints()
        {
            int total = Weights.Sum(w => w.Weight);

            if (tigCheckBox.Checked)
                total += (int)tigMaxSpinEditor.Value;

            if (formScaleCheckBox.Checked)
                total += (int)radSpinEditor1.Value;

            TotalPointsSpinEditor.Value = total;
            RecalculatePercentages();
        }

        private void RecalculatePercentages()
        {
            int totalPoints = (int)TotalPointsSpinEditor.Value;

            for (int i = 0; i < Weights.Count && i < attrWeightsGridView.Rows.Count; i++)
            {
                double pct = totalPoints > 0
                    ? (Weights[i].Weight / (double)totalPoints) * 100.0
                    : 0;
                attrWeightsGridView.Rows[i].Cells["column3"].Value = $"{pct:F1}%";
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
            FormStyling.StyleFormFooterDark(bottomPanel, e);
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
            formRatingTrackBar.Enabled = formScaleCheckBox.Checked;
            radSpinEditor1.Enabled = formScaleCheckBox.Checked;
            RecalculateTotalPoints();
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

        private void radSpinEditor1_ValueChanged(object sender, EventArgs e)
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

                // Delete old weights, re-insert current
                var oldWeights = db.PromotionBoardWeights.FindAll(Board.Id).ToArray();
                if (oldWeights.Length > 0)
                    db.PromotionBoardWeights.RemoveRange(oldWeights);

                foreach (var weight in Weights.Where(w => w.Weight > 0))
                {
                    var entity = db.PromotionBoardWeights.Create();
                    entity.PromotionBoardId = Board.Id;
                    entity.Attribute = weight.Attribute;
                    entity.Weight = weight.Weight;
                    db.PromotionBoardWeights.Add(entity);
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
                var weights = db.PromotionBoardWeights.FindAll(Board.Id).ToList();

                if (weights.Count > 0)
                    db.PromotionBoardWeights.RemoveRange(weights);

                db.PromotionBoards.Remove(Board);

                transaction.Commit();
                this.DialogResult = DialogResult.OK;
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
    }
}