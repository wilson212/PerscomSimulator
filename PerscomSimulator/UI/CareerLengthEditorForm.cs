using System;
using System.Drawing;
using System.Windows.Forms;
using Perscom.Database;

namespace Perscom
{
    public partial class CareerLengthEditorForm : Form
    {
        private CareerLengthRange SelectedRate { get; set; }

        public CareerLengthEditorForm(CareerLengthRange rate)
        {
            // Setup form controls
            InitializeComponent();
            SelectedRate = rate;

            // Fill default values
            probInput.SetValueInRange(rate.Probability);
            minInput.SetValueInRange(rate.MinCareerLength);
            maxInput.SetValueInRange(rate.MaxCareerLength);
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Check for any changes
            var newRate = new CareerLengthRange()
            {
                Probability = (int)probInput.Value,
                MinCareerLength = (int)minInput.Value,
                MaxCareerLength = (int)maxInput.Value
            };

            // Just cancel and close if no changes were made
            if (SelectedRate.IsDuplicateOf(newRate))
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            // Fill Rank Details
            SelectedRate.Probability = (int)probInput.Value;
            SelectedRate.MinCareerLength = (int)minInput.Value;
            SelectedRate.MaxCareerLength = (int)maxInput.Value;

            // Return OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region Panel Border Painting

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        #endregion

        private void minInput_Enter(object sender, EventArgs e)
        {
            minInput.Select(0, minInput.Text.Length);
        }

        private void probInput_Enter(object sender, EventArgs e)
        {
            probInput.Select(0, probInput.Text.Length);
        }

        private void maxInput_Enter(object sender, EventArgs e)
        {
            maxInput.Select(0, maxInput.Text.Length);
        }
    }
}
