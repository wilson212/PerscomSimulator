using CrossLite;
using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class RankSelectForm : RadForm
    {
        /// <summary>
        /// Gets the rank selected by the user, or null if left blank.
        /// </summary>
        public Rank SelectedRank { get; private set; }

        private readonly int _factionId;
        private readonly int? _excludeRankId;
        private List<Rank> _ranks;

        /// <summary>
        /// Creates a new RankSelectForm.
        /// </summary>
        /// <param name="factionId">The faction to filter ranks by.</param>
        /// <param name="currentRankName">The name of the rank being edited (for display).</param>
        /// <param name="excludeRankId">Optional rank ID to exclude from the list (the rank itself).</param>
        /// <param name="currentNextRank">The currently assigned next rank, if any.</param>
        public RankSelectForm(int factionId, string currentRankName, int? excludeRankId = null, Rank currentNextRank = null)
        {
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            _factionId = factionId;
            _excludeRankId = excludeRankId;

            // Update the instruction label with the current rank name
            radLabel1.Text = $"Select Next Rank for {currentRankName}:";

            // Load ranks into the dropdown
            LoadRanks();

            // Pre-select the current next rank if one exists
            if (currentNextRank != null)
            {
                var match = _ranks.FirstOrDefault(r => r.Id == currentNextRank.Id);
                if (match != null)
                {
                    rankDropDownList.SelectedValue = match;
                }
            }
        }

        /// <summary>
        /// Loads all ranks for the given classification into the dropdown,
        /// excluding the rank being edited (to prevent self-referencing).
        /// </summary>
        private void LoadRanks()
        {
            using var db = new AppDatabase();

            // First get all RankClassification IDs for this faction
            var classificationIds = db.RankClassifications
                .Where(rc => rc.FactionId == _factionId)
                .Select(rc => rc.Id)
                .ToHashSet();

            _ranks = db.Ranks
                .Where(r => r.RankClassificationId.In(classificationIds))
                .ToList();

            // Exclude the current rank itself to prevent circular reference
            if (_excludeRankId.HasValue)
            {
                _ranks = _ranks.Where(r => r.Id != _excludeRankId.Value).ToList();
            }

            foreach (var rank in _ranks)
            {
                var item = new RadListDataItem()
                {
                    Tag = rank,
                    Text = rank.Name
                };
                rankDropDownList.Items.Add(item);
            }

            rankDropDownList.SelectedIndex = -1;
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            // If nothing is selected, SelectedRank stays null (meaning "no next rank")
            if (rankDropDownList.SelectedItem != null && rankDropDownList.SelectedItem.Tag is Rank rank)
            {
                SelectedRank = rank;
            }
            else
            {
                SelectedRank = null;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
