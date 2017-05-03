using System;
using System.Drawing;
using System.Windows.Forms;

namespace Perscom
{
    public partial class SoldierEditForm : Form
    {
        protected Soldier Soldier { get; set; }

        /// <summary>
        /// SoldierEditForm constructor
        /// </summary>
        /// <param name="soldier"></param>
        public SoldierEditForm(Soldier soldier)
        {
            InitializeComponent();
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            bottomPanel.BackColor = MainForm.THEME_COLOR_GRAY;

            this.Soldier = soldier;
            textBox1.Text = soldier.FirstName;
            textBox2.Text = soldier.LastName;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Soldier.FirstName = textBox1.Text.Trim();
            Soldier.LastName = textBox2.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region Panel Border Painting

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

        #endregion
    }
}
