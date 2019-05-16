using CrossLite.QueryBuilder;
using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
        /// Gets the total months of stats that were logged in the database
        /// </summary>
        protected double TotalMonthsLogged { get; set; }

        /// <summary>
        /// Gets the current Simulation date
        /// </summary>
        protected DateTime CurrentDate => CurrentIterationDate.Date;

        protected IterationDate CurrentIterationDate { get; set; }

        /// <summary>
        /// Contains an enumerable array of RankTypes
        /// </summary>
        protected static RankType[] RankTypes { get; set; } = Enum.GetValues(typeof(RankType)).Cast<RankType>().ToArray();

        private Dictionary<int, Rank> Ranks { get; set; }

        private Dictionary<int, ListViewGroup> Groups = new Dictionary<int, ListViewGroup>();

        /// <summary>
        /// UnitTemplateId => [RankType => [Rank.Grade => RankGradeStatistics]]
        /// </summary>
        public Dictionary<int, Dictionary<RankType, Dictionary<int, RankGradeStatistics>>> RankStatistics
        {
            get;
            protected set;
        }

        /// <summary>
        /// UnitTemplateId => [RankType => [SpecialtyId => [Rank.Grade => SpecialtyGradeStatistics]]]
        /// </summary>
        public Dictionary<int, Dictionary<RankType, Dictionary<int, Dictionary<int, RankGradeStatistics>>>> SpecialtyStatistics
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
            panel3.BackColor = Color.FromArgb(40, 40, 40);
            panel4.BackColor = Color.FromArgb(40, 40, 40);

            // Set chart colors
            chart3.Series[0].BorderColor = FormStyling.LINE_COLOR_DARK;
            chart3.Series[0].Color = FormStyling.LINE_COLOR_LIGHT;
            chart4.Series[0].BorderColor = FormStyling.LINE_COLOR_DARK;
            chart4.Series[0].Color = FormStyling.LINE_COLOR_LIGHT;
            chart4.Series[1].BorderColor = Color.DarkRed;
            chart4.Series[1].Color = FormStyling.LINE_COLOR_LIGHT;

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
            promotionPieChart.Series[0]["PieLabelStyle"] = "Outside";
            promotionPieChart.Series[0]["PieLineColor"] = "Black";
            promotionPieChart.ChartAreas[0].Area3DStyle.Enable3D = true;
            promotionPieChart.ChartAreas[0].Area3DStyle.Inclination = 60;

            // Load data from database
            Database = db;
            CurrentIterationDate = db.Query<IterationDate>("SELECT * FROM IterationDate ORDER BY Id DESC LIMIT 1").FirstOrDefault();
            TotalMonthsLogged = db.ExecuteScalar<int>("SELECT COUNT(*) FROM IterationDate WHERE Logged=1");
            labelYearsRan.Text = (TotalMonthsLogged / 12) + " years";

            // Create dictionaries
            CreateDictionaries();

            // Load Unit Types
            foreach (var template in Database.UnitTemplates.OrderByDescending(x => x.Echelon.HierarchyLevel))
                unitSelect.Items.Add(template);

            // Load Specialties
            specialtySelect.Items.Add("<< Not Specific >>");
            foreach (var spec in Database.Specialties.OrderBy(x => x.Code))
                specialtySelect.Items.Add(spec);

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
            specialtySelect.SelectedIndex = 0;
            rankTypeBox.SelectedIndex = 0;
            rankTypeBox1.SelectedIndex = 0;
            rankTypeBox2.SelectedIndex = 0;
            rankTypeBox3.SelectedIndex = 0;
            rankTypeBox4.SelectedIndex = 0;
            rankTypeBox5.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndex = 0;

            TreeNode root = new TreeNode();
            root.Text = "Loading... Please Wait";
            treeView1.Nodes.Add(root);

            // Add billit catagories
            foreach (var cat in Database.BilletCatagories.OrderByDescending(x => x.ZIndex))
            {
                var group = new ListViewGroup(cat.Name);
                group.Tag = cat;
                listView2.Groups.Add(group);
                Groups.Add(cat.Id, group);
            }

            // Fill image list for Spawn Settings
            ImageList myImageList1 = new ImageList();
            myImageList1.ImageSize = new Size(64, 64);
            myImageList1.ColorDepth = ColorDepth.Depth32Bit;

            // Fill images
            Ranks = Database.Ranks.ToDictionary(x => x.Id, x => x);
            foreach (var rank in Ranks)
            {
                Image picture = ImageAccessor.GetImage(Path.Combine("Large", rank.Value.Image)) ?? new Bitmap(64, 64);
                myImageList1.Images.Add(rank.Value.Image, picture);

                // Add Greyscale image
                picture = ToolStripRenderer.CreateDisabledImage(picture);
                myImageList1.Images.Add(String.Concat(rank.Value.Image, "_empty"), picture);
            }
            listView2.LargeImageList = myImageList1;
        }

        private void BuildUnitTree(Unit unit)
        {
            TreeNode root = new TreeNode();
            root.Text = unit.Name;
            root.Tag = unit;

            string query = "SELECT * FROM `UnitAttachment` WHERE `ParentId`=" + unit.Id;
            var attachments = Database.Query<UnitAttachment>(query);
            PopulateTree(root, attachments);

            // An exception is thrown here if we close the Window too quickly
            try
            {
                // Since we are cross-threaded, invoke changes
                treeView1.Invoke((MethodInvoker)delegate
                {
                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(root);

                    foreach (TreeNode tn in treeView1.Nodes)
                    {
                        tn.Expand();
                    }
                });
            }
            catch (InvalidOperationException) { }
        }

        public void PopulateTree(TreeNode parent, IEnumerable<UnitAttachment> attachments)
        {
            // An exception is thrown here if we clost the Window too quickly
            try
            {
                foreach (var att in attachments)
                {
                    string query = "SELECT * FROM `Unit` WHERE `Id`=" + att.ChildId;
                    Unit unit = Database.Query<Unit>(query).First();
                    var child = new TreeNode()
                    {
                        Text = unit.Name,
                        Tag = unit
                    };

                    query = "SELECT * FROM `UnitAttachment` WHERE `ParentId`=" + unit.Id;
                    var children = Database.Query<UnitAttachment>(query);

                    // Since we are cross-threaded, invoke changes
                    treeView1.Invoke((MethodInvoker)delegate
                    {
                        parent.Nodes.Add(child);
                    });
                }
            }
            catch (ObjectDisposedException)
            {

            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Get selected Node
            foreach (TreeNode child in e.Node.Nodes)
            {
                // Grab child unit
                Unit childUnit = (Unit)child.Tag;

                // Grab attachments
                string query = "SELECT * FROM `UnitAttachment` WHERE `ParentId`=" + childUnit.Id;
                var attachments = Database.Query<UnitAttachment>(query);

                // Load each childs children
                PopulateTree(child, attachments);
            }
        }

        private void CreateDictionaries()
        {
            RankStatistics = new Dictionary<int, Dictionary<RankType, Dictionary<int, RankGradeStatistics>>>();
            SpecialtyStatistics = new Dictionary<int, Dictionary<RankType, Dictionary<int, Dictionary<int, RankGradeStatistics>>>>();

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
                        new Dictionary<RankType, Dictionary<int, Dictionary<int, RankGradeStatistics>>>()
                    );

                // Ensure key exists
                if (!SpecialtyStatistics[stat.UnitTemplateId].ContainsKey(stat.RankType))
                    SpecialtyStatistics[stat.UnitTemplateId].Add(stat.RankType, new Dictionary<int, Dictionary<int, RankGradeStatistics>>());

                // Ensure key exists
                if (!SpecialtyStatistics[stat.UnitTemplateId][stat.RankType].ContainsKey(stat.SpecialtyId))
                    SpecialtyStatistics[stat.UnitTemplateId][stat.RankType].Add(stat.SpecialtyId, new Dictionary<int, RankGradeStatistics>());

                // Ensure key exists
                if (!SpecialtyStatistics[stat.UnitTemplateId][stat.RankType][stat.SpecialtyId].ContainsKey(stat.RankGrade))
                    SpecialtyStatistics[stat.UnitTemplateId][stat.RankType][stat.SpecialtyId].Add(stat.RankGrade, new RankGradeStatistics());

                // Add stat
                SpecialtyStatistics[stat.UnitTemplateId][stat.RankType][stat.SpecialtyId][stat.RankGrade] = stat;
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

            // Grab selected specialty filtering
            Specialty specialty = specialtySelect.SelectedItem as Specialty;

            // Grab filtered soldier list
            var stats = (specialty == null)
                ? RankStatistics[template.Id][rank.Type]
                : SpecialtyStatistics[template.Id][rank.Type][specialty.Id];

            // Make sure the rank exists in the simulation
            if (!stats.ContainsKey(rank.Grade))
                return 0;

            // Total number of soldiers who did NOT make this rank/grade
            int totalPriorGradeRetirements = 0;

            // Total number of soldiers who DID make this rank/grade
            int totalAtThisRank = stats[rank.Grade].TotalSoldiersOutgoing;

            // Add up the total number of soldiers who did NOT make this
            // grade in the specified rank type
            foreach (Rank r in RankCache.GetPrevousGrades(rank.Type, rank.Grade))
            {
                // If no soldiers ever made this rank/grade, skip
                if (!stats.ContainsKey(r.Grade))
                    continue;

                // Add total retirements at this grade to the total cumulative retirements
                var info = stats[r.Grade];
                totalPriorGradeRetirements +=  info.TotalRetirements;
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
            // Get the selected unit template
            UnitTemplate template = unitSelect.SelectedItem as UnitTemplate;
            if (template == null) return 0;

            // Grab selected specialty filtering
            Specialty specialty = specialtySelect.SelectedItem as Specialty;

            // Grab filtered soldier list
            var stats = (specialty == null)
                ? RankStatistics[template.Id][rank.Type]
                : SpecialtyStatistics[template.Id][rank.Type][specialty.Id];

            // Make sure the rank exists in the simulation
            if (!stats.ContainsKey(rank.Grade))
                return 0;

            // Load the unit, so the soldier counts can be fetched
            UnitStatistics unitStats = UnitBuilder.GetUnitStatistics(template);
            int positions = unitStats.SoldierCountsByGrade[rank.Type][rank.Grade];
            double cumulative = positions * TotalMonthsLogged;

            // Total number of soldiers who DID make this rank/grade
            int totalDeficit = stats[rank.Grade].Deficit;

            return (cumulative == 0 || totalDeficit == 0)
                ? 0
                : Math.Round((totalDeficit / cumulative) * 100, 2);
        }

        private void DrawTabPageReport(int index)
        {
            switch (index)
            {
                case 0:
                    FillTab0Report();
                    break;
                case 1:
                    FillTab1Report();
                    break;
                case 2:
                    FillTab2Report();
                    break;
                case 3:
                    FillTab3Report();
                    break;
                case 4:
                    FillTab4Report();
                    break;
                case 5:
                    FillTab5Report();
                    break;
            }
        }

        #region Report Functions

        private void FillTab0Report()
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
            RankType type = (rankTypeBox.SelectedIndex == -1) ? RankType.Enlisted : (RankType)rankTypeBox.SelectedItem;

            // Setup the enlisted pie chart
            foreach (var item in stats.SoldierCountsByRank[type].OrderByDescending(x => RankCache.RanksById[x.Key].Grade))
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
        }

        /// <summary>
        /// Time In Grade Report Tab
        /// </summary>
        private void FillTab1Report()
        {
            // === TAB 1 ====================================================
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Grab selected specialty filtering
            Specialty specialty = specialtySelect.SelectedItem as Specialty;

            // Clear chart
            chart1.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox1.SelectedItem;
            var ranks = RankCache.GetRankListByType(type);

            // Check for Specialty. If we have one, switch the rank type to match
            if (specialty != null && specialty.Type != type)
            {
                rankTypeBox1.SelectedIndex = (int)specialty.Type;
                return;
            }

            // Grab filtered soldier list
            var soldierData = (specialty == null) 
                ? RankStatistics[selected.Id][type]
                : SpecialtyStatistics[selected.Id][type][specialty.Id];

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
                point.Color = (i % 2 == 1) ? FormStyling.CHART_COLOR_DARK : FormStyling.CHART_COLOR_LIGHT;
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

            // Grab selected specialty filtering
            Specialty specialty = specialtySelect.SelectedItem as Specialty;

            // Clear chart
            chart2.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox2.SelectedItem;
            var ranks = RankCache.GetRankListByType(type);

            // Check for Specialty. If we have one, switch the rank type to match
            if (specialty != null && specialty.Type != type)
            {
                // This will Invoke this method again, so return afterwards
                rankTypeBox2.SelectedIndex = (int)specialty.Type;
                return;
            }

            // Grab filtered soldier list
            var soldierData = (specialty == null)
                ? RankStatistics[selected.Id][type]
                : SpecialtyStatistics[selected.Id][type][specialty.Id];

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
                point.Color = (i % 2 == 1) ? FormStyling.CHART_COLOR_DARK : FormStyling.CHART_COLOR_LIGHT;
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

            // Grab selected specialty filtering
            Specialty specialty = specialtySelect.SelectedItem as Specialty;

            // Clear chart
            chart3.Series[0].Points.Clear();
            RankType type = (RankType)rankTypeBox3.SelectedItem;
            var ranks = RankCache.GetRankListByType(type);

            // Check for Specialty. If we have one, switch the rank type to match
            if (specialty != null && specialty.Type != type)
            {
                // This will Invoke this method again, so return afterwards
                rankTypeBox3.SelectedIndex = (int)specialty.Type;
                return;
            }

            // Grab filtered soldier list
            var soldierData = (specialty == null)
                ? RankStatistics[selected.Id][type]
                : SpecialtyStatistics[selected.Id][type][specialty.Id];

            // Plot the tptal number of retirements by rank/grade
            foreach (var stat in soldierData.OrderBy(x => x.Key))
            {
                Rank rank = ranks[stat.Key].FirstOrDefault();
                if (rank == null)
                    continue;

                int i = chart3.Series[0].Points.AddY(stat.Value.TotalRetirements);
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

            // Grab selected specialty filtering
            Specialty specialty = specialtySelect.SelectedItem as Specialty;

            // Clear chart
            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();
            RankType type = (RankType)rankTypeBox4.SelectedItem;
            var ranks = RankCache.GetRankListByType(type);

            // Check for Specialty. If we have one, switch the rank type to match
            if (specialty != null && specialty.Type != type)
            {
                // This will Invoke this method again, so return afterwards
                rankTypeBox4.SelectedIndex = (int)specialty.Type;
                return;
            }

            // Grab filtered soldier list
            var soldierData = (specialty == null)
                ? RankStatistics[selected.Id][type]
                : SpecialtyStatistics[selected.Id][type][specialty.Id];
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
                rate = (double)stats.PromotablePercentage;
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

        private void FillTab5Report()
        {
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Grab selected rank
            Rank rank = rankSelectionBox.SelectedItem as Rank;
            if (rank == null) return;

            // Grab selected specialty filtering
            Specialty specialty = specialtySelect.SelectedItem as Specialty;

            // Get selected item
            Series series = promotionPieChart.Series[0];
            RankType type = (RankType)rankTypeBox5.SelectedItem;
            double totalYears = (yearlyRadioButton.Checked) ? TotalMonthsLogged / 12 : 1;

            // Check for Specialty. If we have one, switch the rank type to match
            if (specialty != null && specialty.Type != type)
            {
                // This will Invoke this method again, so return afterwards
                rankTypeBox5.SelectedIndex = (int)specialty.Type;
                return;
            }

            // Grab filtered soldier list
            var soldierData = (specialty == null)
                ? RankStatistics[selected.Id][type]
                : SpecialtyStatistics[selected.Id][type][specialty.Id];

            // Clear Chart
            series.Points.Clear();
            promotionPieChart.Titles[1].Text = rank.Name;

            // Make sure grade exists
            if (soldierData.ContainsKey(rank.Grade))
            {
                // Grab stats
                var stats = soldierData[rank.Grade];
                if (stats.TotalSoldiersOutgoing == 0)
                {
                    ResetTab5Labels();
                    return;
                }

                // Get promotion rate and deficit
                var rate = TotalPromotionRate(rank);
                var deficit = GetAverageDeficitRate(rank);

                // Plot soldiers promoted
                double ratio = Math.Round(stats.PromotionsToNextGrade / totalYears, 0);
                int i = series.Points.AddY(ratio);
                series.Points[i].LegendText = "Promoted";

                // Plot soldiers retired
                ratio = Math.Round(stats.TotalRetirements / totalYears, 0);
                i = series.Points.AddY(ratio);
                series.Points[i].LegendText = "Retired";

                // Plos transfered soldiers
                ratio = Math.Round(stats.TransfersFrom / totalYears, 0);
                i = series.Points.AddY(ratio);
                series.Points[i].LegendText = "Transfered Branches";

                // Set label texts for statistical data
                labelTotalSelectRate.Text = String.Format("{0}%", rate);
                labelAvgDeficitRate.Text = String.Format("{0}%", deficit);
                labelRankTotalSelected.Text = String.Format("{0:N0}", stats.TotalSoldiersIncoming);
                labelRankPromotions.Text = String.Format("{0:N0}", stats.PromotionsToNextGrade);
                labelRankRetirements.Text = String.Format("{0:N0}", stats.TotalRetirements);
                labelAvgTiS_Promoted.Text = String.Format("{0} years", Math.Round(stats.PromotedAverageTimeInService / 12, 1));
                labelAvgTiS_Retirement.Text = String.Format("{0} years", Math.Round(stats.RetiredAverageTimeInService / 12, 1));
                labelAvgTiG_Promotion.Text = String.Format("{0} months", Math.Round(stats.PromotedAverageTimeInGrade));
                labelAvgTiG_Retirement.Text = String.Format("{0} months", Math.Round(stats.RetiredAverageTimeInGrade));

                labelTransfersInto.Text = String.Format("{0:N0}", stats.TransfersInto);
                labelTransferIntoRate.Text = String.Format("{0}%", stats.TransferIntoRate);

                labelTransfersOut.Text = String.Format("{0:N0}", stats.TransfersFrom);
                labelAvgTiS_Transfered.Text = String.Format("{0} years", Math.Round(stats.TransfersFromAverageTimeInService / 12, 1));
                labelAvgTiG_Transfered.Text = String.Format("{0} months", Math.Round(stats.TransfersFromAverageTimeInGrade));
            }
            else
            {
                ResetTab5Labels();
            }
        }

        private void ResetTab5Labels()
        {
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

            labelTransfersInto.Text = "0";
            labelTransferIntoRate.Text = "0%";

            labelTransfersOut.Text = "0";
            labelAvgTiS_Transfered.Text = "0";
            labelAvgTiG_Transfered.Text = "0";
        }

        #endregion Report Functions

        #region Tab Controls Events

        private void unitSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawTabPageReport(tabControl1.SelectedIndex);
        }

        private void specialtySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) return;

            DrawTabPageReport(tabControl1.SelectedIndex);
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
            foreach (var item in stats.SoldierCountsByRank[type].OrderByDescending(x => RankCache.RanksById[x.Key].Grade))
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

            // Reset labels
            ResetTab5Labels();

            // Fill rank list
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
            FillTab5Report();
        }

        #endregion Tab Controls Events

        #region Soldier Data View

        /// <summary>
        /// Event triggered when a soldier view row is clicked
        /// </summary>
        private async void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var row = dataGridView1.SelectedRows[0];
            var soldier = (SoldierFormWrapper)row.Tag;

            // Show Task Form!
            TaskForm.Show(this, "Loading", "Loading soldier statistics... Please Wait", false);

            // New thread
            await Task.Run(() =>
            {
                using (SoldierViewForm form = new SoldierViewForm(soldier, CurrentDate))
                {
                    TaskForm.CloseForm();

                    // Show soldier dialog
                    form.ShowDialog();

                    // Invalidate the row, so that if the name changed, we can update it with
                    // the new name!
                    dataGridView1.Invoke((MethodInvoker)delegate
                    {
                        int index = dataGridView1.SelectedRows[0].Index;
                        row.SetValues(new object[]
                        {
                            soldier.CurrentRankIcon,
                            soldier.Name,
                            Math.Round((double)soldier.TimeInService / 12, 2).ToString(),
                            soldier.TimeInGrade
                        });
                        dataGridView1.InvalidateRow(index);
                    });
                }
            });
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
            var soldiers = query.From(nameof(Soldier))
                .Where("Retired").NotEqualTo(1)
                .SelectAll()
                .ExecuteQuery<Soldier>();

            foreach (Soldier s in soldiers)
            {
                SoldierFormWrapper soldier = new SoldierFormWrapper(s, CurrentDate);
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Tag = soldier;
                row.SetValues(new object[]
                {
                    soldier.CurrentRankIcon ?? new Bitmap(1, 1),
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
            FormStyling.StyleFormHeader(headerPanel, e);
            //base.OnPaint(e);
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

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            DrawTabPageReport(e.TabPageIndex);
        }

        #endregion Form Events

        private async void SimResultViewForm_Shown(object sender, EventArgs e)
        {
            // Run the simulation
            await Task.Run(() =>
            {
                // Fill units
                Unit unit = Database.Query<Unit>("SELECT * FROM `Unit` ORDER BY `Id` LIMIT 1").FirstOrDefault();
                BuildUnitTree(unit);
            });
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Invoke as we might be in a new thread
            // (Happens after a soldier is viewied ;))
            treeView1.Invoke((MethodInvoker)delegate
            {
                // Get selected Node
                TreeNode node = treeView1.SelectedNode;
                if (node == null) return;

                // Grab selected unit from Node tag
                Unit unit = node.Tag as Unit;
                if (unit == null) return;

                // Prepare update
                listView2.BeginUpdate();

                // Clear list view items
                listView2.Items.Clear();

                // Clear billits from groups
                foreach (var item in Groups)
                {
                    item.Value.Items.Clear();
                }

                // Grab a list of soldiers and positions
                var q = new SelectQueryBuilder(Database);
                var positions = q.From(nameof(Position))
                    .Where("t1.UnitId", Comparison.Equals, unit.Id)
                    .SelectAll()
                    .InnerJoin(nameof(Billet)).On("Id").Equals(nameof(Position), "BilletId")
                    .Select("RankId", "ZIndex", "BilletCatagoryId")
                    .OrderBy("BilletCatagoryId", Sorting.Descending)
                    .OrderBy("ZIndex", Sorting.Descending)
                    .ExecuteQuery();

                // Add each positon to the list
                foreach (var pos in positions)
                {
                    // Define and parse variables
                    int posId = int.Parse(pos["Id"].ToString());
                    int rankId = int.Parse(pos["RankId"].ToString());
                    Rank rank = RankCache.RanksById[rankId];
                    int gId = int.Parse(pos["BilletCatagoryId"].ToString());

                    // Query two, pull the soldier if the position has one
                    var query2 = new SelectQueryBuilder(Database);
                    var s = query2.From(nameof(Assignment))
                        .Where("PositionId", Comparison.Equals, posId)
                        .Select("SoldierId")
                        .InnerJoin(nameof(Soldier)).On("Id").Equals(nameof(Assignment), "SoldierId")
                        .Select("FirstName", "LastName", "RankId")
                        .ExecuteQuery();

                    // Position is empty
                    var row = s.FirstOrDefault();
                    if (row == default(Dictionary<string, object>))
                    {
                        // Add billit to listViewGroup
                        ListViewItem item = new ListViewItem("Position is Empty");
                        item.SubItems.Add(rank.Name);
                        item.SubItems.Add(pos["Name"].ToString());
                        item.ImageKey = String.Concat(Ranks[rank.Id].Image, "_empty");
                        item.Tag = new PositionResult() { SoldierId = 0, PositionId = posId };

                        // Add to list next
                        Groups[gId].Items.Add(item);
                        listView2.Items.Add(item);
                    }
                    else
                    {
                        // Show soldier rank and name!
                        string name = String.Concat(row["FirstName"], " ", row["LastName"]);
                        rankId = int.Parse(row["RankId"].ToString());
                        rank = RankCache.RanksById[rankId];

                        // Add billit to listViewGroup
                        ListViewItem item = new ListViewItem(name);
                        item.SubItems.Add(rank.Name);
                        item.SubItems.Add(pos["Name"].ToString());
                        item.ImageKey = Ranks[rank.Id].Image;
                        item.Tag = new PositionResult()
                        {
                            SoldierId = int.Parse(row["SoldierId"].ToString()),
                            PositionId = posId
                        };

                        // Add to list next
                        Groups[gId].Items.Add(item);
                        listView2.Items.Add(item);
                    }
                }

                // End update
                listView2.EndUpdate();
            });
        }

        private async void listView2_DoubleClick(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView2.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            if (listView2.SelectedItems[0].Tag == null) return;
            var pos = (PositionResult)listView2.SelectedItems[0].Tag;
            if (pos == null || pos.SoldierId == 0) return;

            // Show Task Form!
            TaskForm.Show(this, "Loading", "Loading soldier statistics... Please Wait", false);

            await Task.Run(() =>
            {
                // Fetch Soldier
                var query = "SELECT * FROM `Soldier` WHERE `Id`=" + pos.SoldierId;
                var s = Database.Query<Soldier>(query).FirstOrDefault();
                if (s == null)
                {
                    TaskForm.CloseForm();
                    return;
                }

                // Create Wrapper
                SoldierFormWrapper soldier = new SoldierFormWrapper(s, CurrentDate);
                using (SoldierViewForm form = new SoldierViewForm(soldier, CurrentDate))
                {
                    TaskForm.CloseForm();

                    // Show soldier dialog
                    form.ShowDialog();

                    // Redraw!
                    treeView1_AfterSelect(sender, default(TreeViewEventArgs));
                }
            });
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // Ensure we have a selected item
            viewSoldierToolStripMenuItem.Enabled = (listView2.SelectedItems.Count > 0);
            viewPositionStatisticsToolStripMenuItem.Enabled = (listView2.SelectedItems.Count > 0);
        }

        private void viewSoldierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView2_DoubleClick(sender, e);
        }

        private async void viewPositionStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView2.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            if (listView2.SelectedItems[0].Tag == null) return;
            var pos = (PositionResult)listView2.SelectedItems[0].Tag;
            if (pos ==null) return;

            // Show Task Form!
            TaskForm.Show(this, "Loading", "Loading position statistics... Please Wait", false);

            await Task.Run(() =>
            {
                try
                {
                    // Create Wrapper
                    using (var form = new PositionStatsForm(Database, pos.PositionId, CurrentIterationDate))
                    {
                        TaskForm.CloseForm();

                        // Show soldier dialog
                        form.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    TaskForm.CloseForm();
                    ExceptionHandler.ShowException(ex);
                }
            });
        }

        private async void viewPositionHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView2.SelectedItems.Count == 0)
                return;

            // Grab Billet from selected item tag
            if (listView2.SelectedItems[0].Tag == null) return;
            var pos = (PositionResult)listView2.SelectedItems[0].Tag;
            if (pos == null) return;

            // Show Task Form!
            TaskForm.Show(this, "Loading", "Loading position History... Please Wait", false);

            await Task.Run(() =>
            {
                try
                {
                    // Create Wrapper
                    using (var form = new PositionHistoryForm(Database, pos.PositionId, CurrentIterationDate))
                    {
                        TaskForm.CloseForm();

                        // Show soldier dialog
                        form.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    TaskForm.CloseForm();
                    ExceptionHandler.ShowException(ex);
                }
            });
        }

        private void yearlyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            FillTab5Report();
        }

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
                TotalRecords = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Soldier WHERE Retired=0");
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

        protected class PositionResult
        {
            public int SoldierId { get; set; }

            public int PositionId { get; set; }
        }
    }
}
