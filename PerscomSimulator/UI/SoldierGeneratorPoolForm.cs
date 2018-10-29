using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrossLite.QueryBuilder;
using Perscom.Database;

namespace Perscom
{
    public partial class SoldierGeneratorPoolForm : Form
    {
        protected SoldierGeneratorPool Selected { get; set; }

        private CareerGenerator SelectedNewCareer { get; set; }

        private List<CareerGenerator> CareerGens { get; set; }

        public SoldierGeneratorPoolForm(SoldierGeneratorPool setting)
        {
            // Setup form controls
            InitializeComponent();

            // Save settings
            Selected = setting;

            // Fill Ranks
            using (AppDatabase db = new AppDatabase())
            {
                // Fill Ranks
                var ranks = db.Ranks.OrderBy(x => x.Type).ThenBy(x => x.Grade);
                foreach (var rank in ranks)
                    rankSelect.Items.Add(rank);

                if (ranks.Count() > 0)
                    rankSelect.SelectedIndex = 0;

                // Fill career generators
                CareerGens = new List<CareerGenerator>(db.CareerGenerators);

                // Fill drop down
                foreach (var length in CareerGens)
                    careerGeneratorBox.Items.Add(length);

                if (CareerGens.Count > 0)
                    careerGeneratorBox.SelectedIndex = 0;

                // Get selected career adjustment
                if (setting.NewCareerLength || setting.TemporaryCareer != null)
                {
                    var item = setting.CareerGenerator;
                    if (item != null && item != default(CareerGenerator))
                    {
                        SelectedNewCareer = item;
                        newCareerCheckBox.Checked = true;
                        careerGeneratorBox.SelectedIndex = CareerGens.FindIndex(x => x.Id == item.Id);
                    }
                    else if (setting.TemporaryCareer != null)
                    {
                        SelectedNewCareer = setting.TemporaryCareer;
                        newCareerCheckBox.Checked = true;
                        careerGeneratorBox.SelectedIndex = CareerGens.FindIndex(x => x.Id == setting.TemporaryCareer.Id);
                    }
                }
            }

            // Set form values for existing settings
            if (setting.RankId != 0)
            {
                // Get rank index
                var index = rankSelect.Items.IndexOf(setting.Rank);
                if (index >= 0)
                {
                    rankSelect.SelectedIndex = index;
                }

                // Set probability
                existTrackBar.Value = setting.Probability;

                // Misc Settings
                promotableCheckBox.Checked = setting.MustBePromotable;
                lockedCheckBox.Checked = setting.NotLockedInBillet;
            }

            foreach (var val in Enum.GetValues(typeof(SoldierSorting)))
            {
                firstOrderBox.Items.Add(val);
                thenOrderBox.Items.Add(val);
            }

            // Apply ordering
            firstOrderCheckBox.Checked = (setting.FirstOrderedBy != SoldierSorting.None);
            firstOrderBox.SelectedIndex = (int)setting.FirstOrderedBy;
            firstOrderDirection.SelectedIndex = (int)setting.FirstOrder;
            thenOrderCheckBox.Checked = (setting.ThenOrderedBy != SoldierSorting.None);
            thenOrderBox.SelectedIndex = (int)setting.ThenOrderedBy;
            thenOrderDirection.SelectedIndex = (int)setting.ThenOrder;
        }

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleFields(bool enabled)
        {
            rankSelect.Enabled = enabled;

            existTrackBar.Enabled = enabled;
            newCareerCheckBox.Enabled = enabled;
            promotableCheckBox.Enabled = enabled;
            firstOrderCheckBox.Enabled = enabled;
            lockedCheckBox.Enabled = enabled;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Ensure fields are selected!
            if (rankSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No rank was selected!");
                return;
            }
            else if (existTrackBar.Value == 0)
            {
                ShowErrorMessage("Probability must be greater than zero!");
                return;
            }

            // Add new Setting
            Selected.RankId = ((Rank)rankSelect.SelectedItem).Id;
            Selected.Probability = existTrackBar.Value;
            Selected.NewCareerLength = newCareerCheckBox.Checked;
            Selected.MustBePromotable = promotableCheckBox.Checked;
            Selected.NotLockedInBillet = lockedCheckBox.Checked;
            Selected.FirstOrderedBy = firstOrderCheckBox.Checked
                ? (SoldierSorting)Enum.Parse(typeof(SoldierSorting), firstOrderBox.SelectedItem.ToString()) 
                : 0;
            Selected.ThenOrderedBy = thenOrderCheckBox.Checked
                ? (SoldierSorting)Enum.Parse(typeof(SoldierSorting), thenOrderBox.SelectedItem.ToString())
                : 0;
            Selected.FirstOrder = (firstOrderDirection.SelectedIndex == 0) ? Sorting.Ascending : Sorting.Descending;
            Selected.ThenOrder = (thenOrderDirection.SelectedIndex == 0) ? Sorting.Ascending : Sorting.Descending;

            if (newCareerCheckBox.Checked)
            {
                var item = careerGeneratorBox.SelectedItem as CareerGenerator;
                Selected.TemporaryCareer = item;
            }
            else
            {
                Selected.TemporaryCareer = null;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void existTrackBar_ValueChanged(object sender, EventArgs e)
        {
            existProbLabel.Text = $"{existTrackBar.Value}%";
        }

        private void rankSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rankSelect.SelectedIndex < 0) return;

            // Get selected rank
            Rank rank = (Rank)rankSelect.SelectedItem;

            // Set new image if it exists
            var image = ImageAccessor.GetImage(Path.Combine("large", rank.Image));
            rankPicture.Image = image;
        }

        #region Panel Border Painting

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

        private void firstOrderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = firstOrderCheckBox.Checked;
            firstOrderBox.Enabled = enabled;
            firstOrderDirection.Enabled = enabled;

            // if the first order box is not enabled, uncheck "Then order by"
            // and prevent it from being checked
            if (!enabled)
                thenOrderCheckBox.Checked = false;

            thenOrderCheckBox.Enabled = enabled;
        }

        private void thenOrderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = thenOrderCheckBox.Checked;
            thenOrderBox.Enabled = enabled;
            thenOrderDirection.Enabled = enabled;
        }

        private void firstOrderBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the first order is "None", we don't allow a second ordering!
            if (firstOrderBox.SelectedIndex == 0)
            {
                thenOrderCheckBox.Checked = false;
                thenOrderCheckBox.Enabled = false;
            }
            else
            {
                firstOrderCheckBox_CheckedChanged(sender, e);
            }
        }

        private void newCareerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            careerGeneratorBox.Enabled = newCareerCheckBox.Checked;
        }
    }
}
