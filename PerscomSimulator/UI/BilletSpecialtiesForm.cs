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
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            bottomPanel.BackColor = MainForm.THEME_COLOR_GRAY;

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
            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
            Pen greyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(greyPen, point1, point2);

            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Gray, 1);

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(bottomPanel.Width, 0);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
            base.OnPaint(e);
        }

        #endregion
    }
}
