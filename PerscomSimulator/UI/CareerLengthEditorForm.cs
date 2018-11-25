using Perscom.Database;
using System;
using System.Windows.Forms;

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
            rangeInput.SetValueInRange(rate.MaxCareerLength - rate.MinCareerLength);
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Check for any changes
            var newRate = new CareerLengthRange()
            {
                Probability = (int)probInput.Value,
                MinCareerLength = (int)minInput.Value,
                MaxCareerLength = (int)(minInput.Value + rangeInput.Value)
            };

            // Just cancel and close if no changes were made
            if (SelectedRate.IsDuplicateOf(newRate))
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            // Fill Details
            SelectedRate.Probability = (int)probInput.Value;
            SelectedRate.MinCareerLength = (int)minInput.Value;
            SelectedRate.MaxCareerLength = (int)(minInput.Value + rangeInput.Value);

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

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
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

        private void rangeInput_Enter(object sender, EventArgs e)
        {
            rangeInput.Select(0, rangeInput.Text.Length);
        }

        private void minInput_ValueChanged(object sender, EventArgs e)
        {
            int total = (int)(minInput.Value + rangeInput.Value);
            labelMaxLength.Text = $"{total} months";
        }

        private void maxInput_ValueChanged(object sender, EventArgs e)
        {
            minInput_ValueChanged(sender, e);
        }
    }
}
