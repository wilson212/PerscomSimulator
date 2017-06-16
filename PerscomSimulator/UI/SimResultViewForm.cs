using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CrossLite.QueryBuilder;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public partial class SimResultViewForm : Form
    {

        /// <summary>
        /// Sets the number of records to display on the Soldiers Data Grid View
        /// </summary>
        const int DATA_GRID_PAGE_SIZE = 50;

        protected SimDatabase Database { get; set; }

        /// <summary>
        /// Gets the current Simulation date
        /// </summary>
        protected DateTime CurrentDate => CurrentIterationDate.Date;

        protected IterationDate CurrentIterationDate { get; set; }

        /// <summary>
        /// Contains an enumerable array of RankTypes
        /// </summary>
        protected static RankType[] RankTypes { get; set; } = Enum.GetValues(typeof(RankType)).Cast<RankType>().ToArray();

        /// <summary>
        /// UnitTemplateId => [RankType => [Rank.Grade => RankGradeStatistics]]
        /// </summary>
        public Dictionary<int, Dictionary<RankType, Dictionary<int, RankGradeStatistics>>> RankStatistics
        {
            get;
            protected set;
        }

        /// <summary>
        /// UnitTemplateId => [RankType => [Rank.Grade => [SpecialtyId => SpecialtyGradeStatistics]]]
        /// </summary>
        public Dictionary<int, Dictionary<RankType, Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>>> SpecialtyStatistics
        {
            get;
            protected set;
        }

        public SimResultViewForm(SimDatabase db)
        {
            // Create form controls
            InitializeComponent();
            this.Height = 750;

            // Header background color
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            panel3.BackColor = Color.FromArgb(40, 40, 40);
            panel4.BackColor = Color.FromArgb(40, 40, 40);

            // Set chart colors
            chart3.Series[0].BorderColor = MainForm.LINE_COLOR_DARK;
            chart3.Series[0].Color = MainForm.LINE_COLOR_LIGHT;
            chart4.Series[0].BorderColor = MainForm.LINE_COLOR_DARK;
            chart4.Series[0].Color = MainForm.LINE_COLOR_LIGHT;
            chart4.Series[1].BorderColor = Color.DarkRed;
            chart4.Series[1].Color = MainForm.LINE_COLOR_LIGHT;

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

            // Load data from database
            Database = db;
            CurrentIterationDate = db.Query<IterationDate>("SELECT * FROM IterationDate ORDER BY Id DESC LIMIT 1").FirstOrDefault();

            CreateDictionaries();

            // Load Unit Types
            foreach (var template in Database.UnitTemplates.OrderByDescending(x => x.Echelon.HierarchyLevel))
                unitSelect.Items.Add(template);

            // Fill in ranks
            //Ranks.Load();
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
            rankTypeBox.SelectedIndex = 0;
            rankTypeBox1.SelectedIndex = 0;
            rankTypeBox2.SelectedIndex = 0;
            rankTypeBox3.SelectedIndex = 0;
            rankTypeBox4.SelectedIndex = 0;
            rankTypeBox5.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndex = 0;
        }

        private void CreateDictionaries()
        {
            RankStatistics = new Dictionary<int, Dictionary<RankType, Dictionary<int, RankGradeStatistics>>>();
            SpecialtyStatistics = new Dictionary<int, Dictionary<RankType, Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>>>();

            foreach (var stat in Database.RankGradeStatistics)
            {
                // Ensure key exists
                if (!RankStatistics.ContainsKey(stat.UnitTemplateId))
                    RankStatistics.Add(stat.UnitTemplateId, new Dictionary<RankType, Dictionary<int, RankGradeStatistics>>());

                // Ensure key exists
                if (!RankStatistics[stat.UnitTemplateId].ContainsKey(stat.RankType))
                    RankStatistics[stat.UnitTemplateId].Add(stat.RankType, new Dictionary<int, RankGradeStatistics>());

                // Ensure key exists
                if (!RankStatistics[stat.UnitTemplateId][stat.RankType].ContainsKey(stat.RankGrade))
                    RankStatistics[stat.UnitTemplateId][stat.RankType].Add(stat.RankGrade, new RankGradeStatistics());

                // Add stat
                RankStatistics[stat.UnitTemplateId][stat.RankType][stat.RankGrade] = stat;
            }

            foreach (var stat in Database.SpecialtyGradeStatistics)
            {
                // Ensure key exists
                if (!SpecialtyStatistics.ContainsKey(stat.UnitTemplateId))
                    SpecialtyStatistics.Add(
                        stat.UnitTemplateId, 
                        new Dictionary<RankType, Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>>()
                    );

                // Ensure key exists
                if (!SpecialtyStatistics[stat.UnitTemplateId].ContainsKey(stat.RankType))
                    SpecialtyStatistics[stat.UnitTemplateId].Add(stat.RankType, new Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>());

                // Ensure key exists
                if (!SpecialtyStatistics[stat.UnitTemplateId][stat.RankType].ContainsKey(stat.RankGrade))
                    SpecialtyStatistics[stat.UnitTemplateId][stat.RankType].Add(stat.RankGrade, new Dictionary<int, SpecialtyGradeStatistics>());

                // Ensure key exists
                if (!SpecialtyStatistics[stat.UnitTemplateId][stat.RankType][stat.RankGrade].ContainsKey(stat.SpecialtyId))
                    SpecialtyStatistics[stat.UnitTemplateId][stat.RankType][stat.RankGrade].Add(stat.SpecialtyId, new SpecialtyGradeStatistics());

                // Add stat
                SpecialtyStatistics[stat.UnitTemplateId][stat.RankType][stat.RankGrade][stat.SpecialtyId] = stat;
            }
        }

        /// <summary>
        /// Returns the total percentage of soldiers who made it to the specified
        /// rank and grade in the simulation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public double TotalPromotionRate(Rank rank)
        {
            // Get the selected unit template
            UnitTemplate template = unitSelect.SelectedItem as UnitTemplate;
            if (template == null) return 0;

            // Make sure the rank exists in the simulation
            if (!RankStatistics[template.Id][rank.Type].ContainsKey(rank.Grade))
                return 0;

            // Total number of soldiers who did NOT make this rank/grade
            int totalPriorGradeRetirements = 0;

            // Total number of soldiers who DID make this rank/grade
            int totalAtThisRank = RankStatistics[template.Id][rank.Type][rank.Grade].TotalSoldiers;

            // Add up the total number of soldiers who did NOT make this
            // grade in the specified rank type
            foreach (Rank r in RankCache.GetPrevousGrades(rank.Type, rank.Grade))
            {
                // If no soldiers ever made this rank/grade, skip
                if (!RankStatistics[template.Id][rank.Type].ContainsKey(r.Grade))
                    continue;

                // Add total retirements at this grade to the total cumulative retirements
                var info = RankStatistics[template.Id][rank.Type][r.Grade];
                totalPriorGradeRetirements += info.TotalRetirements;
            }

            // Get the total number of soldiers up to this rank and grade point
            double totalSoldiers = totalAtThisRank + totalPriorGradeRetirements;

            // Prevent div by zero exception
            return (totalSoldiers == 0) ? 0 : Math.Round((totalAtThisRank / totalSoldiers) * 100, 2);
        }

        /// <summary>
        /// Gets the percentage of positions at the specified rank / grade that
        /// are understaffed on average
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public double GetAverageDeficitRate(Rank rank)
        {
            /*
                UnitTemplate_OLD template = UnitTemplate_OLD.Load(ProcessingUnit.TemplateName);
                int positions = template.SoldierCounts[type][grade];
                int cumulative = positions * (TotalYearsRan * 12);

                // Total number of soldiers who DID make this rank/grade
                int totalDeficit = RankStatistics[type][grade].Deficit;

                if (cumulative == 0 || totalDeficit == 0) return 0;

                return Math.Round(((double)totalDeficit / cumulative) * 100, 2);
            */
            return 0;
        }

        #region Report Functions

        /// <summary>
        /// Time In Grade Report Tab
        /// </summary>
        private void FillTab1Report()
        {
            // === TAB 1 ====================================================
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Clear chart
            chart1.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox1.SelectedItem;
            var soldierData = RankStatistics[selected.Id][type];
            var ranks = RankCache.GetRankListByType(type);

            // Plot the average time in grade for each grade
            foreach (var rank in soldierData.OrderBy(x => x.Key).Take(soldierData.Count - 1))
            {
                // Assign plot variables
                string rankName = ranks[rank.Key].First().ToString();
                double roundedVal = (double)rank.Value.PromotedAverageTimeInGrade;
                int i = chart1.Series[0].Points.AddY(roundedVal);

                // Create Plot
                DataPoint point = chart1.Series[0].Points[i];
                point.AxisLabel = rankName;
                point.LegendText = rankName;
                point.Label = roundedVal.ToString();
                point.Color = (i % 2 == 1) ? MainForm.CHART_COLOR_DARK : MainForm.CHART_COLOR_LIGHT;
            }
        }

        /// <summary>
        /// Time In Service Tab
        /// </summary>
        private void FillTab2Report()
        {
            // === TAB 2 ====================================================
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Clear chart
            chart2.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox2.SelectedItem;
            var soldierData = RankStatistics[selected.Id][type];
            var ranks = RankCache.GetRankListByType(type);

            // Plot the average time in service (years) for each grade
            foreach (var rank in soldierData.OrderBy(x => x.Key).Take(soldierData.Count - 1))
            {
                // Assign plot variables
                string rankName = ranks[rank.Key].First().ToString();
                double roundedVal = (double)rank.Value.PromotedAverageYearsInService;
                int i = chart2.Series[0].Points.AddY(roundedVal);

                // Create Plot
                DataPoint point = chart2.Series[0].Points[i];
                point.AxisLabel = rankName;
                point.LegendText = rankName;
                point.Label = roundedVal.ToString();
                point.Color = (i % 2 == 1) ? MainForm.CHART_COLOR_DARK : MainForm.CHART_COLOR_LIGHT;
            }
        }

        /// <summary>
        /// Discharges Report
        /// </summary>
        private void FillTab3Report()
        {
            // === TAB 3 ====================================================
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Clear chart
            chart3.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox3.SelectedItem;
            var soldierData = RankStatistics[selected.Id][type];
            var ranks = RankCache.GetRankListByType(type);

            // Plot the tptal number of retirements by rank/grade
            foreach (var stat in soldierData.OrderBy(x => x.Key))
            {
                Rank rank = ranks[stat.Key].First();
                int i = chart3.Series[0].Points.AddY((double)stat.Value.TotalRetirements);
                DataPoint point = chart3.Series[0].Points[i];
                point.AxisLabel = rank.Name;
                point.LegendText = rank.Name;
                point.Label = stat.Value.TotalRetirements.ToString();
            }
        }

        /// <summary>
        /// Selection Rates Chart
        /// </summary>
        private void FillTab4Report()
        {
            // === TAB 4 ====================================================
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Clear chart
            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();
            RankType type = (RankType)rankTypeBox4.SelectedItem;
            var soldierData = RankStatistics[selected.Id][type];
            var ranks = RankCache.GetRankListByType(type);
            int count = soldierData.Count;

            // Take all but the last grade!
            foreach (var rank in soldierData.OrderBy(x => x.Key).Take(count - 1))
            {
                int p = 0;
                var stats = rank.Value;

                // Total Personel that held this rank/grade (including non-promotables)
                double rate = (double)stats.PromotionRate;
                p = chart4.Series[1].Points.AddY(rate);
                chart4.Series[1].Points[p].Label = $"{rate}%";

                // Promotable soldiers whom did, or could have been promoted but retired too soon
                rate = (double)stats.SelectionRate;
                p = chart4.Series[0].Points.AddY(rate);
                chart4.Series[0].Points[p].Label = $"{rate}%";

                Rank fromRank = RankCache.GetRanksByGrade(type, rank.Key).First();
                Rank toRank = RankCache.GetRanksByGrade(type, rank.Key + 1).First();

                // Format labels
                string text = String.Format("{0} -> {1}", fromRank.Abbreviation, toRank.Abbreviation);
                chart4.Series[0].Points[p].AxisLabel = text;
                chart4.Series[0].Points[p].LegendText = text;
            }
        }

        #endregion Report Functions

        #region Tab Controls Events

        private void unitSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Reset pie chart always!
            Series series = unitRankPieChart.Series[0];
            series.Points.Clear();

            // Load the unit, so the soldier counts can be fetched
            UnitStatistics stats = UnitBuilder.GetUnitStatistics(selected);

            labelTotalSoldiers.Text = "Total Unit Soldiers: " + stats.TotalSoldiers;
            RankType type = (RankType)rankTypeBox.SelectedItem;

            // Setup the enlisted pie chart
            foreach (var item in stats.SoldierCountsByRank[type])
            {
                if (item.Value > 0)
                {
                    int i = series.Points.AddY(item.Value);
                    Rank rank = RankCache.RanksById[item.Key];
                    series.Points[i].LegendText = rank.Name + ": #VALY";
                }
            }

            // Reset pie chart always!
            series = unitPersonelPieChart.Series[0];
            series.Points.Clear();

            // Setup the different soldier type counts
            foreach (RankType rType in Enum.GetValues(typeof(RankType)))
            {
                int total = 0;
                foreach (var item in stats.SoldierCountsByRank[rType])
                {
                    total += item.Value;
                }

                int i = series.Points.AddY(total);
                series.Points[i].LegendText = Enum.GetName(typeof(RankType), rType) + ": " + total;
            }

            FillTab1Report();
            FillTab2Report();
            FillTab3Report();
            FillTab4Report();
        }

        private void rankTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Reset pie chart always!
            Series series = unitRankPieChart.Series[0];
            series.Points.Clear();

            // Load the unit, so the soldier counts can be fetched
            UnitStatistics stats = UnitBuilder.GetUnitStatistics(selected);
            RankType type = (RankType)rankTypeBox.SelectedItem;

            // Setup the enlisted pie chart
            foreach (var item in stats.SoldierCountsByGrade[type])
            {
                if (item.Value > 0)
                {
                    int i = series.Points.AddY(item.Value);
                    Rank rank = RankCache.RanksById[item.Key];
                    series.Points[i].LegendText = rank.Name + ": #VALY";
                }
            }
        }

        private void rankTypeBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTab1Report();
        }

        private void rankTypeBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTab2Report();
        }

        private void rankTypeBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTab3Report();
        }

        private void rankTypeBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTab4Report();
        }

        private void rankTypeBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Always reset the box!
            rankSelectionBox.Items.Clear();
            rankSelectionBox.SelectedItem = null;

            // ReSet label texts for statistical data
            labelTotalSelectRate.Text = "0%";
            labelAvgDeficitRate.Text = "0%";
            labelRankTotalSelected.Text = "0";
            labelRankPromotions.Text = "0";
            labelRankRetirements.Text = "0";
            labelAvgTiS_Promoted.Text = "0";
            labelAvgTiS_Retirement.Text = "0";
            labelAvgTiG_Promotion.Text = "0";
            labelAvgTiG_Retirement.Text = "0";

            RankType type = (RankType)rankTypeBox5.SelectedItem;
            foreach (var rank in RankCache.GetRankListByType(type))
            {
                rankSelectionBox.Items.Add(rank.Value.FirstOrDefault());
            }
        }

        /// <summary>
        /// Grade selection change in Tab 5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rankSelectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            if (rankSelectionBox.SelectedItem == null)
                return;

            // Get selected item
            Series series = promotionPieChart.Series[0];
            Rank rank = (Rank)rankSelectionBox.SelectedItem;
            RankType type = (RankType)rankTypeBox5.SelectedItem;
            var rankData = RankStatistics[selected.Id][type];
            int totalYears = (int)(yearsOfSimulate.Value - yearsToSkip.Value);

            // Clear Chart
            series.Points.Clear();
            promotionPieChart.Titles[1].Text = rank.Name;

            // Make sure grade exists
            if (rankData.ContainsKey(rank.Grade))
            {
                var stats = rankData[rank.Grade];
                var rate = TotalPromotionRate(rank);
                var deficit = GetAverageDeficitRate(rank);

                int i = series.Points.AddY(stats.PromotionsToNextGrade / totalYears);
                series.Points[i].LegendText = "Promoted";

                i = series.Points.AddY(stats.TotalRetirements / totalYears);
                series.Points[i].LegendText = "Retired";

                // Set label texts for statistical data
                labelTotalSelectRate.Text = String.Format("{0}%", rate);
                labelAvgDeficitRate.Text = String.Format("{0}%", deficit);
                labelRankTotalSelected.Text = String.Format("{0:N0}", stats.TotalSoldiers);
                labelRankPromotions.Text = String.Format("{0:N0}", stats.PromotionsToNextGrade);
                labelRankRetirements.Text = String.Format("{0:N0}", stats.TotalRetirements);
                labelAvgTiS_Promoted.Text = String.Format("{0} years", Math.Round(stats.PromotedAverageTimeInService / 12, 1));
                labelAvgTiS_Retirement.Text = String.Format("{0} years", Math.Round(stats.RetiredAverageTimeInService / 12, 1));
                labelAvgTiG_Promotion.Text = String.Format("{0} months", Math.Round(stats.PromotedAverageTimeInGrade));
                labelAvgTiG_Retirement.Text = String.Format("{0} months", Math.Round(stats.RetiredAverageTimeInGrade));
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

            var row = dataGridView1.SelectedRows[0];
            var soldier = (SoldierWrapper2)row.Tag;
            using (SoldierViewForm form = new SoldierViewForm(soldier, CurrentDate))
            {
                // Show soldier dialog
                form.ShowDialog();

                // Invalidate the row, so that if the name changed, we can update it with
                // the new name!
                int index = dataGridView1.SelectedRows[0].Index;
                row.SetValues(new object[]
                {
                    soldier.RankIcon,
                    soldier.Name,
                    Math.Round((double)soldier.TimeInService / 12, 2).ToString(),
                    soldier.TimeInGrade
                });
                dataGridView1.InvalidateRow(index);
            }
        }

        /// <summary>
        /// Event triggered when the results page is changed on the Soldier Viewer
        /// </summary>
        private void BindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            // Always clear the current rows!
            dataGridView1.Rows.Clear();

            // Grab soldiers
            var query = new SelectQueryBuilder(Database);
            query.Limit = DATA_GRID_PAGE_SIZE;
            query.Offset = (int)bindingSource1.Current;
            var soldiers = query.From("Soldier")
                .Where("Retired").NotEqualTo(1)
                .SelectAll()
                .ExecuteQuery<Soldier>();

            foreach (Soldier s in soldiers)
            {
                SoldierWrapper2 soldier = new SoldierWrapper2(s, CurrentDate);
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Tag = soldier;
                row.SetValues(new object[]
                {
                    soldier.RankIcon ?? new Bitmap(1, 1),
                    soldier.Name,
                    Math.Round((double)soldier.TimeInService / 12, 2).ToString(),
                    soldier.TimeInGrade
                });
                dataGridView1.Rows.Add(row);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
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
            */

            // Setup the data view grid
            bindingSource1.DataSource = new PageOffsetList(Database);
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

        #endregion Form Events

        /// <summary>
        /// A class used to specify the total records in a <see cref="BindingNavigator"/>
        /// </summary>
        protected class PageOffsetList : IListSource
        {
            public bool ContainsListCollection { get; protected set; }

            /// <summary>
            /// Gets the total number of records
            /// </summary>
            public int TotalRecords { get; set; }

            /// <summary>
            /// Creates a new instance of PageOffsetList
            /// </summary>
            /// <param name="soldiers"></param>
            public PageOffsetList(SimDatabase db)
            {
                TotalRecords = db.Soldiers.Count;
            }

            public IList GetList()
            {
                // Return a list of page offsets based on "totalRecords" and "pageSize"
                var pageOffsets = new List<int>();
                for (int offset = 0; offset < TotalRecords; offset += DATA_GRID_PAGE_SIZE)
                    pageOffsets.Add(offset);

                return pageOffsets;
            }
        }
    }
}
