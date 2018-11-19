using Perscom.Database;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Perscom
{
    public partial class ExperienceSelectForm : Form
    {
        private BilletExperience Selected { get; set; }

        public ExperienceSelectForm(BilletExperience item)
        {
            InitializeComponent();

            // Fill drop down
            Selected = item ?? throw new ArgumentException("Item cannot be null", "item");
            FillSelectBox();
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
                        numericUpDown1.SetValueInRange(Selected.Rate);
                    }
                }
            }
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
            if (item == null) return;

            Selected.ExperienceId = item.Id;
            Selected.Rate = (int)numericUpDown1.Value;

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
