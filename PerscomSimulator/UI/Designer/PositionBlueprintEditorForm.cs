using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CrossLite;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class PositionBlueprintEditorForm : RadForm
    {
        /// <summary>
        /// Stores a snapshot of the form field values at the time of load or last Apply.
        /// Used to detect unsaved changes when the user tries to navigate away.
        /// </summary>
        private class FormSnapshot
        {
            public string Name;
            public int CatagoryIndex;
            public int FlagIndex;
            public int PromoPoolIndex;
            public int TargetRankIndex;
            public int RankTypeIndex;
            public int SupervisorIndex;
            public int OccupationIndex;
            public decimal Stature;
            public decimal Prestige;
            public decimal MinTourLength;
            public decimal MaxTourLength;
            public decimal ZIndex;
            public bool CanRetireEarly;
            public bool CanBePromotedEarly;
            public bool Waiverable;
            public bool CanLateralEarly;
            public bool DemoteOverRanked;
            public bool BlockAutoPromote;
            public int SelectionProcedureIndex;

            // Performance model trackbar values
            public int Leadership;
            public int Composure;
            public int Marksmanship;
            public int Fitness;
            public int Teamwork;
            public int Discipline;
            public int Agreeableness;
            public int Ambition;
            public int Conscientiousness;
            public int Extraversion;
            public int Mindfulness;
            public int Courage;
            public int Intellect;
            public int Adaptability;
            
            public List<int> AllowedRankIds;
            public List<int> AllowedOccupationIds;
        }

        /// <summary>
        /// Captures a snapshot of the current form field values.
        /// </summary>
        private FormSnapshot _lastSavedSnapshot;

        /// <summary>
        /// The UnitBlueprint that owns the position blueprints being edited.
        /// </summary>
        private UnitBlueprint OwnerUnit { get; set; }

        /// <summary>
        /// The currently selected PositionBlueprint being edited, or null if creating new.
        /// </summary>
        private PositionBlueprint SelectedBlueprint { get; set; }

        /// <summary>
        /// Indicates whether the form is in "Edit" mode (true) or "Add New" mode (false).
        /// </summary>
        private bool IsEditMode { get; set; }

        /// <summary>
        /// Maps AttributeType to its corresponding RadTrackBar control.
        /// </summary>
        private Dictionary<AttributeType, RadTrackBar> _trackBarMap;

        /// <summary>
        /// Constructor for creating a NEW PositionBlueprint under the given UnitBlueprint.
        /// </summary>
        public PositionBlueprintEditorForm(UnitBlueprint ownerUnit)
        {
            if (ownerUnit == null)
                throw new ArgumentNullException(nameof(ownerUnit));

            OwnerUnit = ownerUnit;
            IsEditMode = false;

            InitializeForm();

            labelHeader.Text = $"Add New Position Blueprint for {OwnerUnit.Name}";
        }

        /// <summary>
        /// Constructor for editing an EXISTING PositionBlueprint.
        /// </summary>
        public PositionBlueprintEditorForm(UnitBlueprint ownerUnit, PositionBlueprint existing)
        {
            if (ownerUnit == null)
                throw new ArgumentNullException(nameof(ownerUnit));
            if (existing == null)
                throw new ArgumentNullException(nameof(existing));

            OwnerUnit = ownerUnit;
            IsEditMode = true;

            InitializeForm();

            labelHeader.Text = $"Edit Position Blueprint: {existing.Name}";

            // Load the existing blueprint into the form
            SelectBlueprint(existing);
        }

        /// <summary>
        /// Shared initialization logic for both constructors.
        /// </summary>
        private void InitializeForm()
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            // Build the trackbar map for performance model
            _trackBarMap = new Dictionary<AttributeType, RadTrackBar>
            {
                { AttributeType.Leadership, leadershipTrackBar },
                { AttributeType.Composure, composureTrackBar },
                { AttributeType.Marksmanship, marksmanTrackBar },
                { AttributeType.Fitness, fitnessTrackBar },
                { AttributeType.Teamwork, teamworkTrackBar },
                { AttributeType.Discipline, disciplineTrackBar },
                { AttributeType.Agreeableness, agreeableTrackBar },
                { AttributeType.Ambition, ambitionTrackBar },
                { AttributeType.Conscientiousness, conTrackBar },
                { AttributeType.Extraversion, extraversionTrackBar },
                { AttributeType.Mindfullness, awarenessTrackBar },
                { AttributeType.Courage, courageTrackBar },
                { AttributeType.Intellect, intelTrackBar },
                { AttributeType.Adaptability, adaptTrackBar }
            };

            // Configure trackbars: range 0-20
            foreach (var trackBar in _trackBarMap.Values)
            {
                trackBar.Minimum = 0;
                trackBar.Maximum = 20;
                trackBar.Value = 0;
                trackBar.LargeTickFrequency = 5;
                trackBar.SmallTickFrequency = 1;
            }

            // Populate dropdowns from database
            PopulateDropdowns();

            // Set default values
            ResetFields();

            // Wire up change-detection events for dirty-state styling
            nameTextBox1.TextChanged += (s, ev) => UpdateSaveButtonStyle();
            catagoryDropDownList.SelectedIndexChanged += (s, ev) => UpdateSaveButtonStyle();
            flagDropDownList.SelectedIndexChanged += (s, ev) => UpdateSaveButtonStyle();
            promoPoolDropDownList.SelectedIndexChanged += (s, ev) => UpdateSaveButtonStyle();
            targetRankDropDownList.SelectedIndexChanged += (s, ev) => UpdateSaveButtonStyle();
            rankTypeDropDownList.SelectedIndexChanged += (s, ev) => OnRankTypeChanged();
            supervisorDropDownList.SelectedIndexChanged += (s, ev) => UpdateSaveButtonStyle();
            occupationDropDownList.SelectedIndexChanged += (s, ev) => UpdateSaveButtonStyle();
            statureSpinEditor.ValueChanged += (s, ev) => UpdateSaveButtonStyle();
            prestigeSpinEditor.ValueChanged += (s, ev) => UpdateSaveButtonStyle();
            minTourLengthSpinEditor.ValueChanged += (s, ev) => UpdateSaveButtonStyle();
            maxTourLengthSpinEditor.ValueChanged += (s, ev) => UpdateSaveButtonStyle();
            zIndexSpinEditor.ValueChanged += (s, ev) => UpdateSaveButtonStyle();
            radCheckBox1.ToggleStateChanged += (s, ev) => UpdateSaveButtonStyle();
            radCheckBox3.ToggleStateChanged += (s, ev) => UpdateSaveButtonStyle();
            radCheckBox4.ToggleStateChanged += (s, ev) => UpdateSaveButtonStyle();
            radCheckBox5.ToggleStateChanged += (s, ev) => UpdateSaveButtonStyle();
            demoteCheckBox.ToggleStateChanged += (s, ev) => UpdateSaveButtonStyle();
            blockAutoPromoteCheckBox.ToggleStateChanged += (s, ev) => UpdateSaveButtonStyle();
            selectionProcedureDropDownList.SelectedIndexChanged += (s, ev) => UpdateSaveButtonStyle();

            // Wire up trackbar change events
            foreach (var trackBar in _trackBarMap.Values)
            {
                trackBar.ValueChanged += (s, ev) => UpdateSaveButtonStyle();
            }

            // Update the rank display when target rank changes
            targetRankDropDownList.SelectedIndexChanged += (s, ev) => UpdateRankDisplay();
            allowedRanksDropDownList.ItemCheckedChanged += (s, ev) =>
            {
                UpdateRankDisplay();
                UpdateSaveButtonStyle();
            };
            allowedOccupationsDropDownList.ItemCheckedChanged += (s, ev) => UpdateSaveButtonStyle();

            // Initial button state
            UpdateSaveButtonStyle();
        }

        /// <summary>
        /// Populates all dropdown lists with data from the database.
        /// </summary>
        private void PopulateDropdowns()
        {
            try
            {
                using var db = new AppDatabase();

                // Position Categories
                var categories = db.PositionCatagories.ToList();
                foreach (var cat in categories)
                {
                    catagoryDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = cat.Name,
                        Tag = cat
                    });
                }

                // Position Flags
                foreach (PositionFlag flag in Enum.GetValues(typeof(PositionFlag)))
                {
                    flagDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = Enum.GetName(typeof(PositionFlag), flag),
                        Tag = flag
                    });
                }

                // Echelons (Promotion Pool)
                var echelons = db.Echelons.OrderBy(e => e.HierarchyLevel).ToList();
                foreach (var echelon in echelons)
                {
                    promoPoolDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = echelon.Name,
                        Tag = echelon
                    });
                }

                // Rank Types
                foreach (RankType type in Enum.GetValues(typeof(RankType)))
                {
                    rankTypeDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = Enum.GetName(typeof(RankType), type),
                        Tag = type
                    });
                }

                // Occupations (filtered by faction)
                var occupations = db.Occupations
                    .Where(o => o.FactionId == OwnerUnit.FactionId)
                    .ToList();
                foreach (var occ in occupations)
                {
                    occupationDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = occ.ToString(),
                        Tag = occ
                    });
                }
                
                // Allowed Occupations (checked dropdown)
                foreach (var occ in occupations)
                {
                    allowedOccupationsDropDownList.Items.Add(new RadCheckedListDataItem
                    {
                        Text = occ.ToString(),
                        Tag = occ
                    });
                }

                // Supervisor positions (other positions in the same unit blueprint)
                supervisorDropDownList.Items.Add(new RadListDataItem
                {
                    Text = "(None)",
                    Tag = null
                });

                var siblingPositions = db.PositionBlueprints
                    .Where(p => p.UnitBlueprintId == OwnerUnit.Id)
                    .ToList();
                foreach (var pos in siblingPositions)
                {
                    supervisorDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = pos.Name,
                        Tag = pos
                    });
                }
                
                // Selection Procedures
                foreach (SelectionProcedure proc in Enum.GetValues(typeof(SelectionProcedure)))
                {
                    selectionProcedureDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = Enum.GetName(typeof(SelectionProcedure), proc),
                        Tag = proc
                    });
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Failed to load dropdown data: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Populates the target rank dropdown based on the selected RankType.
        /// </summary>
        private void PopulateRanksForType(RankType type)
        {
            targetRankDropDownList.Items.Clear();
            allowedRanksDropDownList.Items.Clear(); // <-- ADD

            try
            {
                using var db = new AppDatabase();

                var classifications = db.RankClassifications
                    .Where(rc => rc.Type == type && rc.FactionId == OwnerUnit.FactionId)
                    .Select(rc => rc.Id)
                    .ToList();

                var ranks = db.Ranks
                    .Where(r => r.RankClassificationId.In(classifications))
                    .ToList()  // materialize into memory first
                    .OrderBy(r => r.Classification?.PayGrade ?? 0)
                    .ThenBy(r => r.Precedence)
                    .ToList();

                foreach (var rank in ranks)
                {
                    var text = $"{rank.Classification} - {rank.Name}";

                    targetRankDropDownList.Items.Add(new RadListDataItem
                    {
                        Text = text,
                        Tag = rank
                    });

                    // Also add to the checked dropdown
                    allowedRanksDropDownList.Items.Add(new RadCheckedListDataItem
                    {
                        Text = text,
                        Tag = rank
                    });
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Failed to load ranks: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Called when the Rank Type dropdown changes. Repopulates the target rank dropdown.
        /// </summary>
        private void OnRankTypeChanged()
        {
            if (rankTypeDropDownList.SelectedItem?.Tag is RankType type)
            {
                PopulateRanksForType(type);
            }
            UpdateSaveButtonStyle();
        }

        /// <summary>
        /// Updates the RadClassificationRankDisplay based on the currently selected target rank.
        /// </summary>
        private void UpdateRankDisplay()
        {
            var displayRanks = new List<Rank>();

            // Add the target rank if selected
            if (targetRankDropDownList.SelectedItem?.Tag is Rank targetRank)
            {
                displayRanks.Add(targetRank);
            }

            // Add all checked ranks from the allowed ranks dropdown
            foreach (var item in allowedRanksDropDownList.CheckedItems)
            {
                if (item.Tag is Rank checkedRank && !displayRanks.Any(r => r.Id == checkedRank.Id))
                {
                    displayRanks.Add(checkedRank);
                }
            }

            if (displayRanks.Count > 0)
            {
                var sorted = displayRanks
                    .OrderBy(r => r.Classification?.PayGrade ?? 0)
                    .ThenBy(r => r.Precedence)
                    .ToList();
                radClassificationRankDisplay1.SetRanks(sorted);
            }
            else
            {
                radClassificationRankDisplay1.SetRanks((RankClassification)null);
            }
        }

        /// <summary>
        /// Resets all form fields to their default values.
        /// </summary>
        private void ResetFields()
        {
            SelectedBlueprint = null;
            nameTextBox1.Text = string.Empty;
            catagoryDropDownList.SelectedIndex = catagoryDropDownList.Items.Count > 0 ? 0 : -1;
            flagDropDownList.SelectedIndex = 0;
            promoPoolDropDownList.SelectedIndex = promoPoolDropDownList.Items.Count > 0 ? 0 : -1;
            rankTypeDropDownList.SelectedIndex = 0;
            selectionProcedureDropDownList.SelectedIndex = 0;
            
            // Force-populate ranks for the initially selected rank type
            OnRankTypeChanged();
            
            targetRankDropDownList.SelectedIndex = targetRankDropDownList.Items.Count > 0 ? 0 : -1;
            supervisorDropDownList.SelectedIndex = 0;
            occupationDropDownList.SelectedIndex = occupationDropDownList.Items.Count > 0 ? 0 : -1;
            statureSpinEditor.Value = 1;
            prestigeSpinEditor.Value = 50;
            minTourLengthSpinEditor.Value = 0;
            maxTourLengthSpinEditor.Value = 0;
            zIndexSpinEditor.Value = 1;
            radCheckBox1.IsChecked = true;   // CanRetireEarly
            radCheckBox3.IsChecked = true;   // Waiverable
            radCheckBox4.IsChecked = true;   // CanBePromotedEarly
            radCheckBox5.IsChecked = false;  // CanLateralEarly
            demoteCheckBox.IsChecked = false;
            blockAutoPromoteCheckBox.IsChecked = false;

            // Reset trackbars
            foreach (var trackBar in _trackBarMap.Values)
            {
                trackBar.Value = 0;
            }

            _lastSavedSnapshot = null;
            UpdateSaveButtonStyle();
        }

        /// <summary>
        /// Updates the UI to reflect the details of the specified PositionBlueprint.
        /// </summary>
        private void SelectBlueprint(PositionBlueprint blueprint)
        {
            SelectedBlueprint = blueprint;

            nameTextBox1.Text = blueprint.Name;

            // Category
            for (int i = 0; i < catagoryDropDownList.Items.Count; i++)
            {
                if (catagoryDropDownList.Items[i].Tag is PositionCatagory cat && cat.Id == blueprint.CatagoryId)
                {
                    catagoryDropDownList.SelectedIndex = i;
                    break;
                }
            }

            // Flag
            for (int i = 0; i < flagDropDownList.Items.Count; i++)
            {
                if (flagDropDownList.Items[i].Tag is PositionFlag flag && flag == blueprint.Flag)
                {
                    flagDropDownList.SelectedIndex = i;
                    break;
                }
            }

            // Promotion Pool (Echelon)
            for (int i = 0; i < promoPoolDropDownList.Items.Count; i++)
            {
                if (promoPoolDropDownList.Items[i].Tag is Echelon ech && ech.Id == blueprint.PromotionEchelonId)
                {
                    promoPoolDropDownList.SelectedIndex = i;
                    break;
                }
            }
            
            // Selection Procedure
            for (int i = 0; i < selectionProcedureDropDownList.Items.Count; i++)
            {
                if (selectionProcedureDropDownList.Items[i].Tag is SelectionProcedure proc && proc == blueprint.SelectionMethod)
                {
                    selectionProcedureDropDownList.SelectedIndex = i;
                    break;
                }
            }

            // Rank Type — determine from the target rank's classification
            try
            {
                using var db = new AppDatabase();
                var targetRank = db.Ranks.Find(r => r.Id == blueprint.TargetRankId);
                if (targetRank != null)
                {
                    var classification = db.RankClassifications
                        .Find(rc => rc.Id == targetRank.RankClassificationId);

                    if (classification != null)
                    {
                        for (int i = 0; i < rankTypeDropDownList.Items.Count; i++)
                        {
                            if (rankTypeDropDownList.Items[i].Tag is RankType type && type == classification.Type)
                            {
                                rankTypeDropDownList.SelectedIndex = i;
                                PopulateRanksForType(classification.Type);
                                break;
                            }
                        }
                    }
                }
                
                // Load allowed ranks from PositionBlueprintRank
                if (blueprint.Id > 0)
                {
                    var allowedRankIds = db.PositionRanks
                        .Where(pr => pr.PositionBlueprintId == blueprint.Id)
                        .Select(pr => pr.RankId)
                        .ToList();

                    foreach (var item in allowedRanksDropDownList.Items)
                    {
                        if (item is RadCheckedListDataItem checkedItem && checkedItem.Tag is Rank r)
                        {
                            checkedItem.Checked = allowedRankIds.Contains(r.Id);
                        }
                    }
                    
                    // Load allowed occupations from PositionOccupations
                    var allowedOccIds = db.PositionOccupations
                        .Where(po => po.PositionBlueprintId == blueprint.Id)
                        .Select(po => po.OccupationId)
                        .ToList();

                    foreach (var item in allowedOccupationsDropDownList.Items)
                    {
                        if (item is RadCheckedListDataItem checkedItem && checkedItem.Tag is Occupation occ)
                        {
                            checkedItem.Checked = allowedOccIds.Contains(occ.Id);
                        }
                    }
                }

                // Target Rank
                for (int i = 0; i < targetRankDropDownList.Items.Count; i++)
                {
                    if (targetRankDropDownList.Items[i].Tag is Rank rank && rank.Id == blueprint.TargetRankId)
                    {
                        targetRankDropDownList.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Failed to load rank data: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }

            // Supervisor
            supervisorDropDownList.SelectedIndex = 0; // Default to (None)
            if (blueprint.SupervisorPositionBlueprintId.HasValue)
            {
                for (int i = 0; i < supervisorDropDownList.Items.Count; i++)
                {
                    if (supervisorDropDownList.Items[i].Tag is PositionBlueprint sup
                        && sup.Id == blueprint.SupervisorPositionBlueprintId.Value)
                    {
                        supervisorDropDownList.SelectedIndex = i;
                        break;
                    }
                }
            }
            
            // Remove self from the supervisor dropdown
            if (blueprint.Id > 0)
            {
                var selfItem = supervisorDropDownList.Items
                    .FirstOrDefault(i => i.Tag is PositionBlueprint pb && pb.Id == SelectedBlueprint.Id);
                if (selfItem != null)
                {
                    supervisorDropDownList.Items.Remove(selfItem);
                }
            }

            // Occupation
            for (int i = 0; i < occupationDropDownList.Items.Count; i++)
            {
                if (occupationDropDownList.Items[i].Tag is Occupation occ && occ.Id == blueprint.OccupationId)
                {
                    occupationDropDownList.SelectedIndex = i;
                    break;
                }
            }

            // Spin editors
            statureSpinEditor.Value = blueprint.Stature;
            prestigeSpinEditor.Value = blueprint.Prestige;
            minTourLengthSpinEditor.Value = blueprint.MinTourLength;
            maxTourLengthSpinEditor.Value = blueprint.MaxTourLength;
            zIndexSpinEditor.Value = blueprint.ZIndex;

            // Checkboxes
            radCheckBox1.IsChecked = blueprint.CanRetireEarly;
            radCheckBox3.IsChecked = blueprint.Waiverable;
            radCheckBox4.IsChecked = blueprint.CanBePromotedEarly;
            radCheckBox5.IsChecked = blueprint.CanLateralEarly;
            demoteCheckBox.IsChecked = blueprint.DemoteOverRanked;
            blockAutoPromoteCheckBox.IsChecked = !blueprint.AutoPromoteInRankRange;

            // Load performance model into trackbars
            LoadPerformanceModel(blueprint);

            // Update rank display
            UpdateRankDisplay();

            // Snapshot the clean state
            _lastSavedSnapshot = CaptureSnapshot();
            UpdateSaveButtonStyle();
        }

        /// <summary>
        /// Loads the PositionPerformanceModel records for the given blueprint into the trackbars.
        /// </summary>
        private void LoadPerformanceModel(PositionBlueprint blueprint)
        {
            // Reset all trackbars to 0
            foreach (var trackBar in _trackBarMap.Values)
            {
                trackBar.Value = 0;
            }

            if (blueprint.Id == 0) return;

            try
            {
                using var db = new AppDatabase();
                var models = db.PositionPerformanceModels
                    .Where(m => m.PositionBlueprintId == blueprint.Id)
                    .ToList();

                foreach (var model in models)
                {
                    if (_trackBarMap.TryGetValue(model.Attribute, out var trackBar))
                    {
                        trackBar.Value = Math.Clamp(model.ExpectedLevel, 0, 20);
                    }
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Failed to load performance model: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

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

        #region Snapshot & Dirty State

        /// <summary>
        /// Captures the current form field values into a snapshot.
        /// </summary>
        private FormSnapshot CaptureSnapshot()
        {
            return new FormSnapshot
            {
                Name = nameTextBox1.Text,
                CatagoryIndex = catagoryDropDownList.SelectedIndex,
                FlagIndex = flagDropDownList.SelectedIndex,
                PromoPoolIndex = promoPoolDropDownList.SelectedIndex,
                TargetRankIndex = targetRankDropDownList.SelectedIndex,
                RankTypeIndex = rankTypeDropDownList.SelectedIndex,
                SupervisorIndex = supervisorDropDownList.SelectedIndex,
                OccupationIndex = occupationDropDownList.SelectedIndex,
                Stature = statureSpinEditor.Value,
                Prestige = prestigeSpinEditor.Value,
                MinTourLength = minTourLengthSpinEditor.Value,
                MaxTourLength = maxTourLengthSpinEditor.Value,
                ZIndex = zIndexSpinEditor.Value,
                CanRetireEarly = radCheckBox1.IsChecked,
                Waiverable = radCheckBox3.IsChecked,
                CanBePromotedEarly = radCheckBox4.IsChecked,
                CanLateralEarly = radCheckBox5.IsChecked,
                DemoteOverRanked = demoteCheckBox.IsChecked,
                BlockAutoPromote = blockAutoPromoteCheckBox.IsChecked,
                SelectionProcedureIndex = selectionProcedureDropDownList.SelectedIndex,
                Leadership = (int)leadershipTrackBar.Value,
                Composure = (int)composureTrackBar.Value,
                Marksmanship = (int)marksmanTrackBar.Value,
                Fitness = (int)fitnessTrackBar.Value,
                Teamwork = (int)teamworkTrackBar.Value,
                Discipline = (int)disciplineTrackBar.Value,
                Agreeableness = (int)agreeableTrackBar.Value,
                Ambition = (int)ambitionTrackBar.Value,
                Conscientiousness = (int)conTrackBar.Value,
                Extraversion = (int)extraversionTrackBar.Value,
                Mindfulness = (int)awarenessTrackBar.Value,
                Courage = (int)courageTrackBar.Value,
                Intellect = (int)intelTrackBar.Value,
                Adaptability = (int)adaptTrackBar.Value,
                AllowedRankIds = allowedRanksDropDownList.CheckedItems
                    .Where(i => i.Tag is Rank)
                    .Select(i => ((Rank)i.Tag).Id)
                    .OrderBy(id => id)
                    .ToList(),
                AllowedOccupationIds = allowedOccupationsDropDownList.CheckedItems
                    .Where(i => i.Tag is Occupation)
                    .Select(i => ((Occupation)i.Tag).Id)
                    .OrderBy(id => id)
                    .ToList(),
            };
        }

        /// <summary>
        /// Returns true if the current form values differ from the last saved snapshot.
        /// </summary>
        private bool HasUnsavedChanges()
        {
            if (_lastSavedSnapshot == null)
            {
                // In "Add New" mode, consider dirty if the name is non-empty
                return !string.IsNullOrWhiteSpace(nameTextBox1.Text);
            }

            var current = CaptureSnapshot();
            return current.Name != _lastSavedSnapshot.Name
                   || current.CatagoryIndex != _lastSavedSnapshot.CatagoryIndex
                   || current.FlagIndex != _lastSavedSnapshot.FlagIndex
                   || current.PromoPoolIndex != _lastSavedSnapshot.PromoPoolIndex
                   || current.TargetRankIndex != _lastSavedSnapshot.TargetRankIndex
                   || current.RankTypeIndex != _lastSavedSnapshot.RankTypeIndex
                   || current.SupervisorIndex != _lastSavedSnapshot.SupervisorIndex
                   || current.OccupationIndex != _lastSavedSnapshot.OccupationIndex
                   || current.Stature != _lastSavedSnapshot.Stature
                   || current.Prestige != _lastSavedSnapshot.Prestige
                   || current.MinTourLength != _lastSavedSnapshot.MinTourLength
                   || current.MaxTourLength != _lastSavedSnapshot.MaxTourLength
                   || current.ZIndex != _lastSavedSnapshot.ZIndex
                   || current.CanRetireEarly != _lastSavedSnapshot.CanRetireEarly
                   || current.Waiverable != _lastSavedSnapshot.Waiverable
                   || current.CanBePromotedEarly != _lastSavedSnapshot.CanBePromotedEarly
                   || current.CanLateralEarly != _lastSavedSnapshot.CanLateralEarly
                   || current.DemoteOverRanked != _lastSavedSnapshot.DemoteOverRanked
                   || current.BlockAutoPromote != _lastSavedSnapshot.BlockAutoPromote
                   || current.SelectionProcedureIndex != _lastSavedSnapshot.SelectionProcedureIndex
                   || current.Leadership != _lastSavedSnapshot.Leadership
                   || current.Composure != _lastSavedSnapshot.Composure
                   || current.Marksmanship != _lastSavedSnapshot.Marksmanship
                   || current.Fitness != _lastSavedSnapshot.Fitness
                   || current.Teamwork != _lastSavedSnapshot.Teamwork
                   || current.Discipline != _lastSavedSnapshot.Discipline
                   || current.Agreeableness != _lastSavedSnapshot.Agreeableness
                   || current.Ambition != _lastSavedSnapshot.Ambition
                   || current.Conscientiousness != _lastSavedSnapshot.Conscientiousness
                   || current.Extraversion != _lastSavedSnapshot.Extraversion
                   || current.Mindfulness != _lastSavedSnapshot.Mindfulness
                   || current.Courage != _lastSavedSnapshot.Courage
                   || current.Intellect != _lastSavedSnapshot.Intellect
                   || current.Adaptability != _lastSavedSnapshot.Adaptability
                   || !current.AllowedOccupationIds.SequenceEqual(_lastSavedSnapshot.AllowedOccupationIds)
                   || !current.AllowedRankIds.SequenceEqual(_lastSavedSnapshot.AllowedRankIds);
        }

        /// <summary>
        /// Updates the Save button's color based on whether there are unsaved changes.
        /// Blue when dirty, FluentDefault (disabled) when clean.
        /// </summary>
        private void UpdateSaveButtonStyle()
        {
            if (HasUnsavedChanges())
            {
                FormStyling.StyleButtonBlue(saveButton);
                saveButton.Enabled = true;
            }
            else
            {
                FormStyling.StyleButtonFluentDefault(saveButton);
                saveButton.Enabled = false;
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates the form fields before saving. Returns true if valid.
        /// </summary>
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox1.Text))
            {
                RadMessageBox.Show("Position name is required.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return false;
            }

            if (targetRankDropDownList.SelectedItem?.Tag is not Rank)
            {
                RadMessageBox.Show("Please select a target rank.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return false;
            }

            if (catagoryDropDownList.SelectedItem?.Tag is not PositionCatagory)
            {
                RadMessageBox.Show("Please select a position category.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return false;
            }

            if (promoPoolDropDownList.SelectedItem?.Tag is not Echelon)
            {
                RadMessageBox.Show("Please select a promotion pool echelon.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return false;
            }

            if (occupationDropDownList.SelectedItem?.Tag is not Occupation)
            {
                RadMessageBox.Show("Please select an occupation.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return false;
            }

            return true;
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Save button click. Saves the PositionBlueprint and its
        /// PerformanceModel to the database using the CrossLite Unit of Work pattern.
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                bool isNew = (SelectedBlueprint == null || SelectedBlueprint.Id == 0);

                if (isNew)
                {
                    SelectedBlueprint = db.PositionBlueprints.Create();
                    SelectedBlueprint.UnitBlueprintId = OwnerUnit.Id;
                }

                // Map form fields to entity
                SelectedBlueprint.Name = nameTextBox1.Text.Trim();
                SelectedBlueprint.CatagoryId = ((PositionCatagory)catagoryDropDownList.SelectedItem.Tag).Id;
                SelectedBlueprint.Flag = (PositionFlag)flagDropDownList.SelectedItem.Tag;
                SelectedBlueprint.PromotionEchelonId = ((Echelon)promoPoolDropDownList.SelectedItem.Tag).Id;
                SelectedBlueprint.TargetRankId = ((Rank)targetRankDropDownList.SelectedItem.Tag).Id;
                SelectedBlueprint.OccupationId = ((Occupation)occupationDropDownList.SelectedItem.Tag).Id;
                SelectedBlueprint.Stature = (int)statureSpinEditor.Value;
                SelectedBlueprint.Prestige = (int)prestigeSpinEditor.Value;
                SelectedBlueprint.MinTourLength = (int)minTourLengthSpinEditor.Value;
                SelectedBlueprint.MaxTourLength = (int)maxTourLengthSpinEditor.Value;
                SelectedBlueprint.ZIndex = (int)zIndexSpinEditor.Value;
                SelectedBlueprint.CanRetireEarly = radCheckBox1.IsChecked;
                SelectedBlueprint.Waiverable = radCheckBox3.IsChecked;
                SelectedBlueprint.CanBePromotedEarly = radCheckBox4.IsChecked;
                SelectedBlueprint.CanLateralEarly = radCheckBox5.IsChecked;
                SelectedBlueprint.DemoteOverRanked = demoteCheckBox.IsChecked;
                SelectedBlueprint.AutoPromoteInRankRange = !blockAutoPromoteCheckBox.IsChecked;
                SelectedBlueprint.SelectionMethod = (SelectionProcedure)selectionProcedureDropDownList.SelectedItem.Tag;

                // Supervisor (nullable)
                if (supervisorDropDownList.SelectedItem?.Tag is PositionBlueprint supervisor)
                {
                    SelectedBlueprint.SupervisorPositionBlueprintId = supervisor.Id;
                }
                else
                {
                    SelectedBlueprint.SupervisorPositionBlueprintId = null;
                }

                if (isNew)
                {
                    db.PositionBlueprints.Add(SelectedBlueprint);
                }
                else
                {
                    db.PositionBlueprints.Update(SelectedBlueprint);
                }

                // Save Performance Model
                SavePerformanceModel(db);
                
                // Save allowed ranks (PositionBlueprintRank)
                SaveAllowedRanks(db);
                
                // Save allowed occupations (PositionOccupations)
                SaveAllowedOccupations(db);

                transaction.Commit();

                // Update snapshot
                _lastSavedSnapshot = CaptureSnapshot();
                UpdateSaveButtonStyle();

                // Update header for edit mode
                IsEditMode = true;
                labelHeader.Text = $"Edit Position Blueprint: {SelectedBlueprint.Name}";

                this.DialogResult = DialogResult.OK;

                RadMessageBox.Show("Position blueprint saved successfully.",
                    "Success", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to save position blueprint: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Saves the performance model (trackbar values) for the current blueprint.
        /// Deletes existing records and re-inserts them.
        /// </summary>
        private void SavePerformanceModel(AppDatabase db)
        {
            if (SelectedBlueprint == null || SelectedBlueprint.Id == 0) return;

            // Delete existing performance models for this blueprint
            db.PositionPerformanceModels.RemoveWhere(m => m.PositionBlueprintId == SelectedBlueprint.Id);

            // Insert new performance models for each trackbar with a non-zero value
            foreach (var kvp in _trackBarMap)
            {
                int value = (int)kvp.Value.Value;
                if (value > 0)
                {
                    var model = db.PositionPerformanceModels.Create();
                    model.PositionBlueprintId = SelectedBlueprint.Id;
                    model.Attribute = kvp.Key;
                    model.ExpectedLevel = value;
                    db.PositionPerformanceModels.Add(model);
                }
            }
        }
        
        private void SaveAllowedRanks(AppDatabase db)
        {
            if (SelectedBlueprint == null || SelectedBlueprint.Id == 0) return;

            // Delete existing
            db.PositionRanks.RemoveWhere(pr => pr.PositionBlueprintId == SelectedBlueprint.Id);

            // Insert checked items
            foreach (var item in allowedRanksDropDownList.CheckedItems)
            {
                if (item.Tag is Rank rank)
                {
                    var record = db.PositionRanks.Create();
                    record.PositionBlueprintId = SelectedBlueprint.Id;
                    record.RankId = rank.Id;
                    db.PositionRanks.Add(record);
                }
            }
        }
        
        private void SaveAllowedOccupations(AppDatabase db)
        {
            if (SelectedBlueprint == null || SelectedBlueprint.Id == 0) return;

            // Delete existing
            db.PositionOccupations.RemoveWhere(po => po.PositionBlueprintId == SelectedBlueprint.Id);

            // Insert checked items
            foreach (var item in allowedOccupationsDropDownList.CheckedItems)
            {
                if (item.Tag is Occupation occ)
                {
                    var record = db.PositionOccupations.Create();
                    record.PositionBlueprintId = SelectedBlueprint.Id;
                    record.OccupationId = occ.Id;
                    db.PositionOccupations.Add(record);
                }
            }
        }

        /// <summary>
        /// Handles the FormClosing event. Prompts the user to confirm if there are unsaved changes.
        /// </summary>
        private void PositionBlueprintEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasUnsavedChanges())
            {
                var result = RadMessageBox.Show(
                    "You have unsaved changes. Are you sure you want to close without saving?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion
    }
}
