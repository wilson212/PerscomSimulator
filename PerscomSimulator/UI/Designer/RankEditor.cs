using Perscom.Database;
using System;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class RankEditor : RadForm
    {
        /// <summary>
        /// The rank being edited.
        /// </summary>
        private Rank _rank;

        /// <summary>
        /// The RankClassification this rank belongs to.
        /// </summary>
        private RankClassification _rankClassification;

        /// <summary>
        /// Creates a new RankEditor for a new rank under the given classification.
        /// </summary>
        public RankEditor(RankClassification classification, Rank rank = null)
        {
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);
            _rankClassification = classification;

            // Creating a new rank?
            if (rank == null)
            {
                using var db = new AppDatabase();
                _rank = db.Ranks.Create();
                _rank.RankClassificationId = classification.Id;
                _rank.Name = string.Empty;
                _rank.Abbreviation = string.Empty;
                _rank.StipendMode = StipendMode.Inherit;
                _rank.Stipend = 0;
                _rank.Precedence = 0;
                _rank.IsPositional = false;
                _rank.Image = string.Empty;

                // Set default UI state
                inheritRadioButton.IsChecked = true;
            }
            else
            {
                _rank = rank;
                LoadRankIntoForm();
            }
        }

        /// <summary>
        /// Populates all form controls from the current _rank entity.
        /// </summary>
        private void LoadRankIntoForm()
        {
            rankNameTextBox.Text = _rank.Name;
            rankAbbrTextBox.Text = _rank.Abbreviation;
            precedenceSpinEditor.Value = _rank.Precedence;
            isPositionalCheckBox.IsChecked = _rank.IsPositional;
            stipendAmountSpinEditor.Value = (decimal)_rank.Stipend;

            // Set stipend mode radio buttons
            switch (_rank.StipendMode)
            {
                case StipendMode.Inherit:
                    inheritRadioButton.IsChecked = true;
                    break;
                case StipendMode.Override:
                    overrideRadioButton.IsChecked = true;
                    break;
                case StipendMode.Offset:
                    offsetRadioButton.IsChecked = true;
                    break;
            }

            // Load next rank into the selector control
            if (_rank.NextRankId.HasValue && _rank.NextRank != null)
            {
                nextRankSelector.SetRank(_rank.NextRank);
            }
            else
            {
                nextRankSelector.SetRank(null);
            }

            // Does rank have an image? If so, set it
            if (!String.IsNullOrWhiteSpace(_rank.Image))
            {
                string imagePath = System.IO.Path.Combine(Program.RootPath, "Images", _rank.Image);
                var image = ImageAccessor.GetSvgImage(imagePath);
                if (image != null)
                {
                    rankImageSelector.SetImage(image);
                }
            }
        }

        /// <summary>
        /// Returns the currently selected StipendMode from the radio buttons.
        /// </summary>
        private StipendMode GetSelectedStipendMode()
        {
            if (overrideRadioButton.IsChecked)
                return StipendMode.Override;
            if (offsetRadioButton.IsChecked)
                return StipendMode.Offset;
            return StipendMode.Inherit;
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooterGray(bottomPanel, e);
            base.OnPaint(e);
        }

        /// <summary>
        /// Opens the RankSelectForm to allow the user to pick a "Next Rank" for split rank lanes.
        /// </summary>
        private void nextRankSelector_Click(object sender, EventArgs e)
        {
            using var form = new RankSelectForm(
                _rankClassification.FactionId,
                _rank.Name,
                excludeRankId: _rank.Id > 0 ? _rank.Id : null,
                currentNextRank: _rank.NextRank
            );

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Rank selectedRank = form.SelectedRank;

                //_rank.NextRank = selectedRank;
                _rank.NextRankId = selectedRank?.Id;

                // Update the visual selector control
                nextRankSelector.SetRank(selectedRank);
            }
        }

        /// <summary>
        /// Validates form input and saves the rank to the database using CrossLite UoW.
        /// </summary>
        private void saveButton_Click(object sender, EventArgs e)
        {
            // Validate required fields
            string name = rankNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                RadMessageBox.Show("Please enter a name for this Rank.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            string abbreviation = rankAbbrTextBox.Text.Trim();
            if (string.IsNullOrEmpty(abbreviation))
            {
                RadMessageBox.Show("Please enter an abbreviation for this Rank.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // Map form values to entity
            _rank.Name = name;
            _rank.Abbreviation = abbreviation;
            _rank.Precedence = (int)precedenceSpinEditor.Value;
            _rank.IsPositional = isPositionalCheckBox.IsChecked;
            _rank.StipendMode = GetSelectedStipendMode();
            _rank.Stipend = (double)stipendAmountSpinEditor.Value;

            // Persist to database
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                if (_rank.Id > 0)
                {
                    // Update existing rank
                    db.Ranks.Update(_rank);
                }
                else
                {
                    // Insert new rank
                    db.Ranks.Add(_rank);
                }

                transaction.Commit();

                RadMessageBox.Show("Rank saved successfully.", "Success", MessageBoxButtons.OK, RadMessageIcon.Info);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to save rank: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void rankImageSelector_OnImageChanged(object sender, EventArgs e)
        {
            

            // Update the rank entity
            _rank.Image = rankImageSelector.SelectedImagePath;

            // Update the control
        }
    }
}
