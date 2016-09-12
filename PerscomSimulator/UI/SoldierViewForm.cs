using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NodaTime;

namespace Perscom
{
    public partial class SoldierViewForm : Form
    {
        public SoldierViewForm(SoldierWrapper soldier, DateTime currentDate)
        {
            InitializeComponent();
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;

            // Setup the styling of the panels
            panel1.BackColor = MainForm.CHART_COLOR_DARK;
            panel2.BackColor = MainForm.CHART_COLOR_DARK;

            // Round corners on panels
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 5, 5));
            panel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 5, 5));

            // Begin filling label text values
            nameLabel.Text = soldier.Name;
            rankLabel.Text = soldier.Soldier.RankInfo.Name;
            pictureBox1.Image = soldier.RankImage;
            entryLabel.Text = soldier.ServiceEntryDate.ToShortDateString();

            // Time in Service
            LocalDate entry = new LocalDate(soldier.ServiceEntryDate.Year, soldier.ServiceEntryDate.Month, 1);
            LocalDate current = entry.PlusYears(currentDate.Year - entry.Year).PlusMonths(currentDate.Month - entry.Month);
            Period timeFram = Period.Between(entry, current);
            tisLabel.Text = $"{timeFram.Years} year(s) and {timeFram.Months} month(s)";

            // Time to retire
            current = new LocalDate(currentDate.Year, currentDate.Month, 1);
            entry = new LocalDate(soldier.Soldier.ExitServiceDate.Year, soldier.Soldier.ExitServiceDate.Month, 1);
            timeFram = Period.Between(current, entry);
            if (timeFram.Years > 0)
                ttrLabel.Text = $"{timeFram.Years} year(s) and {timeFram.Months} month(s)";
            else
                ttrLabel.Text = $"{timeFram.Months} month(s)";

            // Time in Grade
            DateTime lastPromo = soldier.Soldier.LastPromotionDate;
            current = new LocalDate(currentDate.Year, currentDate.Month, 1);
            entry = new LocalDate(lastPromo.Year, lastPromo.Month, 1);
            timeFram = Period.Between(entry, current);
            if (timeFram.Years > 0)
                tigLabel.Text = $"{timeFram.Years} year(s) and {timeFram.Months} month(s)";
            else
                tigLabel.Text = $"{timeFram.Months} month(s)";


            // Fill Data grid with promotion data
            foreach (Promotion info in soldier.Soldier.Promotions.OrderByDescending(x => x.ToRank.Grade))
            {
                char code = char.ToUpper(info.ToRank.TypeCode);
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.SetValues(new object[]
                {
                    info.Date.ToShortDateString(),
                    $"Promoted to {info.ToRank.Name} ({code}-{info.ToRank.Grade})",
                    Math.Round((double)info.TimeInService / 12, 2).ToString(),
                    info.PreviousTimeInGrade
                });
                dataGridView1.Rows.Add(row);
            }

            // Hide selection on the data view grid
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
        }

        /// <summary>
        /// Adds drop shadow to all the contained CustomPanels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dropShadow(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            Color[] shadow = new Color[3];
            shadow[0] = Color.FromArgb(255,181, 181, 181);
            shadow[1] = Color.FromArgb(155,195, 195, 195);
            shadow[2] = Color.FromArgb(55,211, 211, 211);
            Pen pen = new Pen(shadow[0]);
            using (pen)
            {
                foreach (CustomPanel p in panel.Controls.OfType<CustomPanel>())
                {
                    Point pt = p.Location;
                    pt.Y += p.Height;
                    pt.X += 1;
                    for (var sp = 0; sp < 3; sp++)
                    {
                        pen.Color = shadow[sp];
                        e.Graphics.DrawLine(pen, pt.X, pt.Y, pt.X + p.Width - 2, pt.Y);
                        pt.Y++;
                    }
                }
            }
        }

        /// <summary>
        /// Rounds the corners of a control
        /// </summary>
        /// <param name="nLeftRect"></param>
        /// <param name="nTopRect"></param>
        /// <param name="nRightRect"></param>
        /// <param name="nBottomRect"></param>
        /// <param name="nWidthEllipse"></param>
        /// <param name="nHeightEllipse"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

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
    }
}
