using CrossLite.QueryBuilder;
using Perscom.Database;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Perscom
{
    public partial class ExperienceGroupingForm : Form
    {
        private BilletExperienceGroup Selected { get; set; }

        public ExperienceGroupingForm(BilletExperienceGroup group)
        {
            InitializeComponent();

            // Fill drop down
            Selected = group ?? throw new ArgumentException("Group cannot be null", "group");
            FillSelectBox();

            // Add operator types
            foreach (var val in Enum.GetValues(typeof(ConditionOperator)).Cast<ConditionOperator>())
            {
                operatorSelectBox.Items.Add(val);

                // Is this what we are editing?
                if (Selected.Operator == val)
                {
                    operatorSelectBox.SelectedIndex = operatorSelectBox.Items.Count - 1;
                }
            }

            // Set indexies
            numericUpDown1.SetValueInRange(group.Value);
        }

        private void FillSelectBox()
        {
            // Clear old items first!
            experienceSelect.Items.Clear();

            // Add items
            using (AppDatabase db = new AppDatabase())
            {
                foreach (var exp in db.Experience.OrderBy(x => x.Name))
                {
                    experienceSelect.Items.Add(exp);

                    // Is this what we are editing?
                    if (Selected.ExperienceId == exp.Id)
                    {
                        experienceSelect.SelectedIndex = experienceSelect.Items.Count - 1;
                    }
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using (ExperienceForm form = new ExperienceForm())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    FillSelectBox();
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Grab selected item
            var item = experienceSelect.SelectedItem as Experience;
            if (item == null)
            {
                ShowErrorMessage("An experience item has not been selected!");
                return;
            }

            Selected.ExperienceId = item.Id;
            Selected.Operator = (ConditionOperator)operatorSelectBox.SelectedItem;
            Selected.Value = (int)numericUpDown1.Value;

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
    }
}
