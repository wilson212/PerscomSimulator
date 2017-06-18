using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CrossLite;
using NodaTime;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public partial class SoldierViewForm : Form
    {
        protected SoldierWrapper2 Soldier { get; set; }

        public SoldierViewForm(SoldierWrapper2 soldier, DateTime currentDate)
        {
            InitializeComponent();
            this.Soldier = soldier;

            // Setup the styling of the panels
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            panel1.BackColor = MainForm.CHART_COLOR_DARK;
            panel2.BackColor = MainForm.CHART_COLOR_DARK;
            panel4.BackColor = MainForm.CHART_COLOR_DARK;
            panel5.BackColor = MainForm.CHART_COLOR_DARK;

            // Round corners on panels
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 5, 5));
            panel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel2.Width, panel2.Height, 5, 5));
            panel4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel4.Width, panel4.Height, 5, 5));
            panel5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel5.Width, panel5.Height, 5, 5));

            // Extract data
            var position = soldier.Position;
            var unit = position.Unit;
            var assignment = soldier.Soldier.Assignments.First();
            var spec = soldier.Soldier.Specialty;

            // Begin filling label text values
            nameLabel.Text = soldier.Name;
            rankLabel.Text = soldier.Soldier.Rank.Name;
            pictureBox1.Image = soldier.RankImage;
            entryLabel.Text = soldier.Soldier.EntryServiceDate.Date.ToShortDateString();
            specLabel.Text = $"{spec.Code} - {spec.Name}";

            // Build current Unit name
            var unitNameBuilder = new StringBuilder();
            var unitCodeBuilder = new StringBuilder();
            while (unit != null)
            {
                unitNameBuilder.Append(unit.Name);
                unitCodeBuilder.Append(unit.UnitCode);
                unit = unit.Attachments.Where(x => x.ChildId == unit.Id).Select(x => x.ParentUnit).FirstOrDefault();
                if (unit != null)
                    unitNameBuilder.Append(", ");
            }
            unitLabel.Text = unitNameBuilder.ToString();

            // Time in Service
            LocalDate startDate = new LocalDate(
                soldier.EntryServiceDate.Year,
                soldier.EntryServiceDate.Month, 
                1
            );
            LocalDate endDate = startDate.PlusYears(currentDate.Year - startDate.Year).PlusMonths(currentDate.Month - startDate.Month);
            Period timeFrame = Period.Between(startDate, endDate);
            tisLabel.Text = $"{timeFrame.Years} year(s) and {timeFrame.Months} month(s)";
            labelPosition.Text = soldier.Position?.ToString() ?? "";

            // AddCurrent Assignment to the Assignment history
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView2);
            row.SetValues(new object[]
            {
                    assignment.AssignedOn.Date.ToShortDateString(),
                    position.Name,
                    unitCodeBuilder.ToString().TrimStart(new[] { ',', ' ' }),
                    "--"
            });
            dataGridView2.Rows.Add(row);

            // Time to retire label
            startDate = new LocalDate(currentDate.Year, currentDate.Month, 1);
            endDate = new LocalDate(soldier.ExitServiceDate.Date.Year, soldier.ExitServiceDate.Date.Month, 1);
            timeFrame = Period.Between(startDate, endDate);
            if (timeFrame.Years != 0)
                ttrLabel.Text = $"{timeFrame.Years} year(s) and {timeFrame.Months} month(s)";
            else
                ttrLabel.Text = $"{timeFrame.Months} month(s)";

            // Time in Billet label
            var ad = assignment.AssignedOn;
            startDate = new LocalDate(ad.Date.Year, ad.Date.Month, 1);
            endDate = new LocalDate(currentDate.Year, currentDate.Month, 1);
            timeFrame = Period.Between(startDate, endDate);
            if (timeFrame.Years != 0)
                labelTIB.Text = $"{timeFrame.Years} year(s) and {timeFrame.Months} month(s)";
            else
                labelTIB.Text = $"{timeFrame.Months} month(s)";

            // Time in Grade
            DateTime lastPromo = soldier.EntryServiceDate;

            // Fill Past Assignments
            foreach (Promotion info in soldier.Soldier.Promotions.OrderByDescending(x => x.IterationId))
            {
                Rank toRank = info.ToRank;
                Rank fromRank = info.FromRank;
                char code = char.ToUpper(RankCache.GetCodeByRankType(info.ToRank.Type));
                string desc = String.Empty;

                if (toRank.Grade == soldier.Rank.Grade && toRank.Type == soldier.Rank.Type)
                {
                    lastPromo = info.Date.Date;
                }

                // Get description of the move
                if (toRank.Grade > fromRank.Grade || toRank.Type != fromRank.Type)
                {
                    desc = $"Promoted to {info.ToRank.Name} ({code}-{toRank.Grade})";
                }
                else if (toRank.Grade < fromRank.Grade)
                {
                    desc = $"Demoted to {info.ToRank.Name} ({code}-{toRank.Grade})";
                }
                else
                {
                    desc = $"Laterally Promoted to {info.ToRank.Name} ({code}-{toRank.Grade})";
                }

                row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.SetValues(new object[]
                {
                    info.Date.Date.ToShortDateString(),
                    desc,
                    Math.Round((double)info.TimeInService / 12, 2).ToString(),
                    info.PreviousTimeInGrade
                });
                dataGridView1.Rows.Add(row);
            }

            // Fill TIG label
            startDate = new LocalDate(lastPromo.Year, lastPromo.Month, 1);
            endDate = new LocalDate(currentDate.Year, currentDate.Month, 1);
            timeFrame = Period.Between(startDate, endDate);
            if (timeFrame.Years > 0)
                tigLabel.Text = $"{timeFrame.Years} year(s) and {timeFrame.Months} month(s)";
            else
                tigLabel.Text = $"{timeFrame.Months} month(s)";

            // Fill Past Assignments
            foreach (var pos in soldier.Soldier.PastAssignments.OrderByDescending(x => x.Id))
            {
                int monthsHeld = pos.RemovedIteration - pos.StartIteration;
                unitCodeBuilder.Clear();

                // build unitname
                unit = pos.Position.Unit;
                while (unit != null)
                {
                    unitCodeBuilder.Append(unit.UnitCode);
                    unit = unit.Attachments.Where(x => x.ChildId == unit.Id).Select(x => x.ParentUnit).FirstOrDefault();
                }

                row = new DataGridViewRow();
                row.CreateCells(dataGridView2);
                row.SetValues(new object[]
                {
                    pos.StartDate.Date.ToShortDateString(),
                    pos.Position.Name,
                    unitCodeBuilder.ToString().TrimStart(new[] { ',', ' ' }),
                    monthsHeld
                });
                dataGridView2.Rows.Add(row);
            }

            // Fill Past Assignments
            foreach (var pos in soldier.Soldier.SpecialtyAssignments.OrderByDescending(x => x.Id))
            {
                spec = pos.Specialty;
                row = new DataGridViewRow();
                row.CreateCells(dataGridView3);
                row.SetValues(new object[]
                {
                    pos.AssignedOn.Date.ToShortDateString(),
                    $"{spec.Code} - {spec.Name}"
                });
                dataGridView3.Rows.Add(row);
            }

            // Hide selection on the data view grid
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView2.DefaultCellStyle.SelectionBackColor = dataGridView2.DefaultCellStyle.BackColor;
            dataGridView2.DefaultCellStyle.SelectionForeColor = dataGridView2.DefaultCellStyle.ForeColor;
            dataGridView3.DefaultCellStyle.SelectionBackColor = dataGridView3.DefaultCellStyle.BackColor;
            dataGridView3.DefaultCellStyle.SelectionForeColor = dataGridView3.DefaultCellStyle.ForeColor;
        }

        /// <summary>
        /// Event fired when the Change Name link is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkChangeName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (SoldierEditForm frm = new SoldierEditForm(this.Soldier.Soldier))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    nameLabel.Text = this.Soldier.Name;
                }
            }
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel2.ClientRectangle,
                Color.Gray,
                Color.Black,
                90F))
            {
                e.Graphics.FillRectangle(brush, panel2.ClientRectangle);
            }
        }
    }
}
