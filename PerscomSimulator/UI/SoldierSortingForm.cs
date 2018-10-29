using CrossLite.QueryBuilder;
using System;
using System.Windows.Forms;

namespace Perscom
{
    public partial class SoldierSortingForm : Form
    {
        public SoldierSorting SortBy { get; protected set; }

        public Sorting Direction { get; protected set; }

        public SoldierSortingForm(SoldierSorting sortBy, Sorting direction)
        {
            InitializeComponent();

            // Add sorting types
            foreach (var val in Enum.GetValues(typeof(SoldierSorting)))
            {
                sortingTypeSelect.Items.Add(val);
            }

            // Set indexies
            sortingTypeSelect.SelectedIndex = (int)sortBy;
            sortingDirectionBox.SelectedIndex = (int)direction;
        }

        #region Panel Border Painting

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
            base.OnPaint(e);
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        #endregion

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.SortBy = (SoldierSorting)Enum.Parse(typeof(SoldierSorting), sortingTypeSelect.SelectedItem.ToString());
            this.Direction = (sortingDirectionBox.SelectedIndex == 0) ? Sorting.Ascending : Sorting.Descending;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
