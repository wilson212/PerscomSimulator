using CrossLite.QueryBuilder;
using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perscom
{
    public partial class BilletSortingForm : Form
    {
        private BilletExperienceSorting Selected { get; set; }

        public BilletSortingForm(BilletExperienceSorting item)
        {
            InitializeComponent();

            // Fill drop down
            Selected = item ?? throw new ArgumentException("Item cannot be null", "item");

            // Set form values
            sortingDirectionBox.SelectedIndex = (item.Direction == Sorting.Ascending) ? 0 : 1;

            // Fill box
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

            // Save values
            Selected.ExperienceId = item.Id;
            Selected.Direction = (Sorting)sortingDirectionBox.SelectedIndex;

            // Close form
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
