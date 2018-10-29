using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Perscom.Database;

namespace Perscom
{
    public partial class BilletSpecialtiesForm : Form
    {
        public List<Specialty> SelectedItems { get; set; }

        public BilletSpecialtiesForm(List<Specialty> currentItems)
        {
            // Setup form controls
            InitializeComponent();

            // Set internal
            SelectedItems = currentItems;

            // Fill Items
            using (AppDatabase db = new AppDatabase())
            {
                foreach (var spec in db.Specialties)
                {
                    ListViewItem item = new ListViewItem("");
                    item.SubItems.Add(spec.Code);
                    item.SubItems.Add(spec.Name);
                    item.Tag = spec;
                    item.Checked = SelectedItems.Contains(spec);

                    listView1.Items.Add(item);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            Specialty spec = e.Item.Tag as Specialty;
            if (spec == null) return;

            if (e.Item.Checked)
                SelectedItems.Add(spec);
            else
                SelectedItems.Remove(spec);
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
