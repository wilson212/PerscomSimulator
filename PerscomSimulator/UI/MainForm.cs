using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Perscom.Simulation;

namespace Perscom
{
    public partial class MainForm : Form
    {
        #region Color Scheme

        public static readonly Color THEME_COLOR_DARK = Color.FromArgb(51, 53, 53);
        public static readonly Color THEME_COLOR_GRAY = Color.FromArgb(225, 225, 225);
        public static readonly Color CHART_COLOR_DARK = Color.FromArgb(34, 52, 72);
        public static readonly Color CHART_COLOR_LIGHT = Color.FromArgb(50, 82, 118);
        public static readonly Color LINE_COLOR_DARK = Color.FromArgb(39, 64, 92);
        public static readonly Color LINE_COLOR_LIGHT = Color.FromArgb(100, 50, 82, 118);

        #endregion

        public Simulator Simulation { get; protected set; }

        /// <summary>
        /// Indicates whether the simulation has been ran
        /// </summary>
        public bool SimulationRan = false;

        /// <summary>
        /// Gets a list of all Soldier objects from the last ran simulation
        /// </summary>
        protected List<SoldierWrapper> Soldiers { get; set; }

        /// <summary>
        /// Sets the number of records to display on the Soldiers Data Grid View
        /// </summary>
        const int DATA_GRID_PAGE_SIZE = 50;

        public MainForm()
        {
            // Create form controls
            InitializeComponent();

            // Header background color
            headerPanel.BackColor = THEME_COLOR_DARK;
            panel3.BackColor = Color.FromArgb(40, 40, 40);
            panel4.BackColor = Color.FromArgb(40, 40, 40);

            // Set chart colors
            chart3.Series[0].BorderColor = LINE_COLOR_DARK;
            chart3.Series[0].Color = LINE_COLOR_LIGHT;
            chart4.Series[0].BorderColor = LINE_COLOR_DARK;
            chart4.Series[0].Color = LINE_COLOR_LIGHT;
            chart4.Series[1].BorderColor = Color.DarkRed;
            chart4.Series[1].Color = LINE_COLOR_LIGHT;

            // Pie charts
            unitRankPieChart.Series[0].Label = "#PERCENT{P0}";
            unitRankPieChart.Series[0]["PieLabelStyle"] = "Outside";
            unitRankPieChart.Series[0]["PieLineColor"] = "Black";
            unitRankPieChart.ChartAreas[0].Area3DStyle.Enable3D = true;
            unitRankPieChart.ChartAreas[0].Area3DStyle.Inclination = 60;

            unitPersonelPieChart.Series[0].Label = "#PERCENT{P0}";
            unitPersonelPieChart.Series[0]["PieLabelStyle"] = "Outside";
            unitPersonelPieChart.Series[0]["PieLineColor"] = "Black";
            unitPersonelPieChart.ChartAreas[0].Area3DStyle.Enable3D = true;
            unitPersonelPieChart.ChartAreas[0].Area3DStyle.Inclination = 60;

            promotionPieChart.Series[0].Label = "#VALY (#PERCENT{P0})";
            //promotionPieChart.Series[0]["PieLabelStyle"] = "Outside";
            //promotionPieChart.Series[0]["PieLineColor"] = "Black";
            promotionPieChart.ChartAreas[0].Area3DStyle.Enable3D = true;
            promotionPieChart.ChartAreas[0].Area3DStyle.Inclination = 60;

            // Load Unit Types
            string path = Path.Combine(Program.RootPath, "Units");
            foreach (string fileName in Directory.GetFiles(path, "*.xml"))
                unitSelect.Items.Add(Path.GetFileNameWithoutExtension(fileName));

            // Fill in ranks
            Ranks.Load();
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                rankTypeBox.Items.Add(type);
                rankTypeBox1.Items.Add(type);
                rankTypeBox2.Items.Add(type);
                rankTypeBox3.Items.Add(type);
                rankTypeBox4.Items.Add(type);
                rankTypeBox5.Items.Add(type);
            }

            // Set default indexies
            rankTypeBox1.SelectedIndex = 0;
            rankTypeBox2.SelectedIndex = 0;
            rankTypeBox3.SelectedIndex = 0;
            rankTypeBox4.SelectedIndex = 0;
            rankTypeBox5.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndex = 0;
        }

        #region Report Functions

        private void FillTab1Report()
        {
            // === TAB 1 ====================================================
            // Clear chart
            chart1.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox1.SelectedItem;
            var soldierData = Simulation.Promotions[type];
            var ranks = Ranks.RankList[type];
            int lastRank = 0;

            foreach (var rank in soldierData.OrderBy(x => x.Key))
            {
                int i = chart1.Series[0].Points.AddY((double)rank.Value.AverageTimeInGrade);
                DataPoint point = chart1.Series[0].Points[i];
                point.AxisLabel = ranks[rank.Key].ToString();
                point.LegendText = ranks[rank.Key].ToString();
                point.Label = rank.Value.AverageTimeInGrade.ToString();
                point.Color = (i % 2 == 1) ? CHART_COLOR_DARK : CHART_COLOR_LIGHT;
                lastRank = rank.Key;
            }

            // Add SGM for shits
            soldierData = Simulation.Retirements[type];
            int maxRank = soldierData.Keys.OrderByDescending(x => x).FirstOrDefault();
            if (maxRank > lastRank)
            {
                int j = chart1.Series[0].Points.AddY((double)soldierData[maxRank].AverageTimeInGrade);
                DataPoint point = chart1.Series[0].Points[j];
                point.AxisLabel = ranks[maxRank].ToString();
                point.LegendText = ranks[maxRank].ToString();
                point.Label = soldierData[maxRank].AverageTimeInGrade.ToString();
                point.Color = (j % 2 == 1) ? CHART_COLOR_DARK : CHART_COLOR_LIGHT;
            }
        }

        private void FillTab2Report()
        {
            // === TAB 2 ====================================================
            // Clear chart
            chart2.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox2.SelectedItem;
            var soldierData = Simulation.Promotions[type];
            var ranks = Ranks.RankList[type];
            int lastRank = 0;
            foreach (var rank in soldierData.OrderBy(x => x.Key))
            {
                int i = chart2.Series[0].Points.AddY((double)rank.Value.AverageYearsInService);
                DataPoint point = chart2.Series[0].Points[i];
                point.AxisLabel = ranks[rank.Key].ToString();
                point.LegendText = ranks[rank.Key].ToString();
                point.Label = rank.Value.AverageYearsInService.ToString();
                lastRank = rank.Key;
                point.Color = (i % 2 == 1) ? CHART_COLOR_DARK : CHART_COLOR_LIGHT;
            }

            // Add the highest grade for the rank type as well
            soldierData = Simulation.Promotions[type];
            int maxRank = soldierData.Keys.OrderByDescending(x => x).FirstOrDefault();
            if (maxRank > lastRank)
            {
                int j = chart2.Series[0].Points.AddY((double)soldierData[maxRank].AverageYearsInService);
                DataPoint point = chart2.Series[0].Points[j];
                point.AxisLabel = ranks[maxRank].ToString();
                point.LegendText = ranks[maxRank].ToString();
                point.Label = soldierData[maxRank].AverageYearsInService.ToString();
                point.Color = (j % 2 == 1) ? CHART_COLOR_DARK : CHART_COLOR_LIGHT;
            }
        }

        private void FillTab3Report()
        {
            // === TAB 3 ====================================================
            // Clear chart
            chart3.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox3.SelectedItem;
            var soldierData = Simulation.Retirements[type];
            var ranks = Ranks.RankList[type];
            foreach (var rank in soldierData.OrderBy(x => x.Key))
            {
                int i = chart3.Series[0].Points.AddY((double)rank.Value.TotalPersonel);
                DataPoint point = chart3.Series[0].Points[i];
                point.AxisLabel = ranks[rank.Key].Name;
                point.LegendText = ranks[rank.Key].Name;
                point.Label = rank.Value.TotalPersonel.ToString();
            }
        }

        private void FillTab4Report()
        {
            // === TAB 4 ====================================================
            // Clear chart
            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();
            RankType type = (RankType)rankTypeBox4.SelectedItem;
            var ranks = Ranks.RankList[type];

            foreach (var rank in Simulation.Retirements[type].OrderBy(x => x.Key))
            {
                // Skip if this is the last grade for this rank type
                if (!Simulation.Promotions[type].ContainsKey(rank.Key)) continue;

                int p = 0;
                PromotionInfo promoted = Simulation.Promotions[type][rank.Key];
                PromotionInfo retired = rank.Value;

                // Prevent a division by zero exception here
                if (promoted.TotalPersonel > 0)
                {
                    // Total Personel that held this rank/grade (including non-promotables)
                    int total = promoted.TotalPersonel + retired.TotalPersonel;
                    double rate = Math.Round((double)promoted.TotalPersonel / total, 2) * 100;
                    p = chart4.Series[1].Points.AddY(rate);
                    chart4.Series[1].Points[p].Label = $"{rate}%";

                    // Promotable soldiers whom did, or could have been promoted but retired too soon
                    total = promoted.TotalPersonel + retired.TotalPromotable;
                    rate = Math.Round((double)promoted.TotalPersonel / total, 2) * 100;
                    p = chart4.Series[0].Points.AddY(rate);
                    chart4.Series[0].Points[p].Label = $"{rate}%";
                }
                else
                {
                    p = chart4.Series[0].Points.AddY(0.00);
                    chart4.Series[0].Points[p].Label = "0%";
                    p = chart4.Series[1].Points.AddY(0.00);
                    chart4.Series[1].Points[p].Label = "0%";
                }

                chart4.Series[0].Points[p].AxisLabel = ranks[rank.Key].Name;
                chart4.Series[0].Points[p].LegendText = ranks[rank.Key].Name;
            }
        }

        #endregion Report Functions

        #region Tab Controls Events

        private void unitSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset pie chart always!
            Series series = unitRankPieChart.Series[0];
            series.Points.Clear();

            // Load the unit, so the soldier counts can be fetched
            Unit unit = Unit.Load(unitSelect.SelectedItem.ToString());
            labelTotalSoldiers.Text = "Total Unit Soldiers: " + unit.TotalSoldiers;
            RankType type = (rankTypeBox.SelectedIndex == -1) ? RankType.Enlisted : (RankType)rankTypeBox.SelectedItem;

            // Setup the enlisted pie chart
            foreach (var item in unit.SoldierCounts[type])
            {
                int i = series.Points.AddY(item.Value);
                series.Points[i].LegendText = Ranks.RankList[type][item.Key] + ": #VALY";
            }

            // Reset pie chart always!
            series = unitPersonelPieChart.Series[0];
            series.Points.Clear();

            // Setup the different soldier type counts
            foreach (RankType rType in Enum.GetValues(typeof(RankType)))
            {
                int total = 0;
                foreach (var item in unit.SoldierCounts[rType])
                {
                    total += item.Value;
                }

                int i = series.Points.AddY(total);
                series.Points[i].LegendText = Enum.GetName(typeof(RankType), rType) + ": " + total;
            }
        }

        private void rankTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset pie chart always!
            Series series = unitRankPieChart.Series[0];
            series.Points.Clear();

            Unit unit = Unit.Load(unitSelect.SelectedItem.ToString());
            RankType type = (RankType)rankTypeBox.SelectedItem;

            // Setup the enlisted pie chart
            foreach (var item in unit.SoldierCounts[type])
            {
                int i = series.Points.AddY(item.Value);
                series.Points[i].LegendText = Ranks.RankList[type][item.Key] + ": #VALY";
            }
        }

        private void rankTypeBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SimulationRan) return;

            FillTab1Report();
        }

        private void rankTypeBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SimulationRan) return;

            FillTab2Report();
        }

        private void rankTypeBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SimulationRan) return;

            FillTab3Report();
        }

        private void rankTypeBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SimulationRan) return;

            FillTab4Report();
        }

        private void rankTypeBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Always reset the box!
            rankSelectionBox.Items.Clear();
            rankSelectionBox.SelectedItem = null;

            RankType type = (RankType)rankTypeBox5.SelectedItem;
            foreach (KeyValuePair<int, Rank> rank in Ranks.GetRankListByType(type))
            {
                rankSelectionBox.Items.Add(rank.Value);
            }
        }

        private void rankSelectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rankSelectionBox.SelectedItem == null)
                return;

            // Get selected item
            Series series = promotionPieChart.Series[0];
            Rank rank = (Rank)rankSelectionBox.SelectedItem;
            RankType type = (RankType)rankTypeBox5.SelectedItem;
            var promotions = Simulation.Promotions[type];
            var retirements = Simulation.Retirements[type];
            int totalYears = (int)(yearsOfSimulate.Value - yearsToSkip.Value);

            // Clear Chart
            series.Points.Clear();
            promotionPieChart.Titles[1].Text = rank.Name;

            if (promotions.ContainsKey(rank.Grade))
            {
                double promoted = Math.Round((double)promotions[rank.Grade].TotalPersonel / totalYears, 2);
                double retired = Math.Round((double)retirements[rank.Grade].TotalPersonel / totalYears, 2);

                int i = series.Points.AddY(promoted);
                series.Points[i].LegendText = "Promoted";

                i = series.Points.AddY(retired);
                series.Points[i].LegendText = "Retired";
            }
            else if (retirements.ContainsKey(rank.Grade))
            {
                double retired = Math.Round((double)retirements[rank.Grade].TotalPersonel / totalYears, 2);
                int i = series.Points.AddY(retired);
                series.Points[i].LegendText = "Retired";
            }
        }

        #endregion Tab Controls Events

        #region Soldier Data View

        /// <summary>
        /// Event triggered when a soldier view row is clicked
        /// </summary>
        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var soldier = (SoldierWrapper)dataGridView1.SelectedRows[0].Tag;
            using (SoldierViewForm form = new SoldierViewForm(soldier, Simulation.CurrentDate))
            {
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Event triggered when the results page is changed on the Soldier Viewer
        /// </summary>
        private void BindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            // Always clear the current rows!
            dataGridView1.Rows.Clear();
            var soldiers = ((PageOffsetList)bindingSource1.DataSource).Soldiers;

            // The desired page has changed, so fetch the page of records using the "Current" offset 
            int offset = (int)bindingSource1.Current;
            for (int i = offset; i < offset + DATA_GRID_PAGE_SIZE && i < soldiers.Count; i++)
            {
                SoldierWrapper soldier = soldiers[i];
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Tag = soldier;
                row.SetValues(new object[]
                {
                    soldier.RankIcon,
                    soldier.Name,
                    Math.Round((double)soldier.TimeInService / 12, 2).ToString(),
                    soldier.TimeInGrade
                });
                dataGridView1.Rows.Add(row);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SimulationRan) return;

            List<SoldierWrapper> newList = null;
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    newList = Soldiers;
                    break;
                case 2:
                    newList = Soldiers.Where(x => x.Soldier.RankInfo.Type == RankType.Officer).ToList();
                    break;
                case 3:
                    newList = Soldiers.Where(x => x.Soldier.RankInfo.Type == RankType.Warrant).ToList();
                    break;
                default:
                    newList = Soldiers.Where(x => x.Soldier.RankInfo.Type == RankType.Enlisted).ToList();
                    break;
            }

            // Setup the data view grid
            bindingSource1.DataSource = new PageOffsetList(newList);
            bindingSource1.MoveFirst();
        }

        #endregion Soldier Data View

        #region Form Window Events

        /// <summary>
        /// Adds the darker border line color between the header panel and the contents
        /// panel
        /// </summary>
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
        }

        /// <summary>
        /// Adds the darker border line color between the footer panel and the contents
        /// panel
        /// </summary>
        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
            Pen greyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(panel4.Width, 0);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, 1);
            point2 = new Point(panel4.Width, 1);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, 2);
            point2 = new Point(panel4.Width, 2);
            e.Graphics.DrawLine(greyPen, point1, point2);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            unitSelect.SelectedIndex = 0;
            rankTypeBox.SelectedIndex = 0;
            rankTypeBox.SelectedIndexChanged += rankTypeBox_SelectedIndexChanged;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            panel2.Left = (this.ClientSize.Width - panel2.Width) / 2;
            generateButton.Left = (this.ClientSize.Width - generateButton.Width) / 2;
        }

        private async void generateButton_Click(object sender, EventArgs e)
        {
            generateButton.Enabled = false;

            try
            {
                // Load the Unit and Soldier xml files
                Unit brigade = Unit.Load(unitSelect.SelectedItem.ToString());
                Simulation = new Simulator(brigade);
                SimulatorSettings settings = new SimulatorSettings()
                {
                    ProcessEnlisted = enlistedMenuItem.Checked,
                    ProcessOfficers = officerMenuItem.Checked,
                    ProcessWarrant = warrantMenuItem.Checked
                };

                // Run the simulation
                TaskForm.Show(this, "Running Simulation", "Running Simulation... Please Wait.", false);
                await Task.Run(() =>
                {
                    Simulation.Run((int)yearsOfSimulate.Value, (int)yearsToSkip.Value, TaskForm.Progress, settings);
                });
                SimulationRan = true;

                // Fill Charts
                FillTab1Report();
                FillTab2Report();
                FillTab3Report();
                FillTab4Report();

                // Combine officers and soldiers
                var list = new List<Soldier>();
                foreach (var typesGrades in Simulation.Soldiers)
                    foreach (var gradesSoldiers in typesGrades.Value)
                    {
                        list.AddRange(gradesSoldiers.Value);
                    }

                // Fill in the Soldier Viewer
                Soldiers = new List<SoldierWrapper>(list.Count);
                foreach (Soldier s in list.OrderBy(x => x.ServiceEntryDate))
                {
                    var soldier = new SoldierWrapper(s, Simulation.CurrentDate);
                    Soldiers.Add(soldier);
                }

                // Setup the data view grid
                toolStripComboBox1_SelectedIndexChanged(this, EventArgs.Empty);

                // Finally, close the task form
                TaskForm.CloseForm();
            }
            catch (Exception ex)
            {
                TaskForm.CloseForm();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Enable Button
                generateButton.Enabled = true;
            }
        }

        #endregion Form Events

        #region Menu Items

        /// <summary>
        /// Exit application menu button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeMenuItem_Click(object sender, EventArgs e) => this.Close();

        private void soldierConfigMenuItem_Click(object sender, EventArgs e)
        {
            using (SoldierConfigForm form = new SoldierConfigForm())
            {
                form.ShowDialog();
            }
        }

        private void openRootMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Program.RootPath);
        }

        private void enlistedMenuItem_Click(object sender, EventArgs e)
        {
            enlistedMenuItem.Checked = !enlistedMenuItem.Checked;
        }

        private void officerMenuItem_Click(object sender, EventArgs e)
        {
            officerMenuItem.Checked = !officerMenuItem.Checked;
        }

        private void warrantMenuItem_Click(object sender, EventArgs e)
        {
            warrantMenuItem.Checked = !warrantMenuItem.Checked;
        }

        #endregion Menu Items

        /// <summary>
        /// A class used to specify the total records in a <see cref="BindingNavigator"/>
        /// </summary>
        protected class PageOffsetList : IListSource
        {
            public bool ContainsListCollection { get; protected set; }

            /// <summary>
            /// Gets the total number of records
            /// </summary>
            public int TotalRecords => Soldiers.Count;

            /// <summary>
            /// Gets the internal list
            /// </summary>
            public List<SoldierWrapper> Soldiers { get; protected set; }

            public PageOffsetList(List<SoldierWrapper> soldiers)
            {
                Soldiers = soldiers;
            }

            public IList GetList()
            {
                // Return a list of page offsets based on "totalRecords" and "pageSize"
                var pageOffsets = new List<int>();
                for (int offset = 0; offset < Soldiers.Count; offset += DATA_GRID_PAGE_SIZE)
                    pageOffsets.Add(offset);
                return pageOffsets;
            }
        }
    }
}
