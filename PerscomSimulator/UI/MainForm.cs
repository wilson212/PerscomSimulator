﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The cancellation token for the simulator
        /// </summary>
        protected CancellationTokenSource CancelToken { get; set; }

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
            this.Height = 750;

            // Header background color
            headerPanel.BackColor = FormStyling.THEME_COLOR_DARK;
            panel3.BackColor = Color.FromArgb(40, 40, 40);
            panel4.BackColor = Color.FromArgb(40, 40, 40);

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

            // Load Unit Types
            using (AppDatabase db = new AppDatabase())
            {
                foreach (var template in db.UnitTemplates.OrderByDescending(x => x.Echelon.HierarchyLevel))
                    unitSelect.Items.Add(template);
            }

            // Fill in ranks
            //Ranks.Load();
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                rankTypeBox.Items.Add(type);
            }
        }

        private async void FillTab0Report()
        {
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Reset pie chart always!
            foreach (var s in unitRankPieChart.Series)
            {
                s.Points.Clear();
            }

            foreach (var s in unitPersonelPieChart.Series)
            {
                s.Points.Clear();
            }

            // Disable the simulate button, as this can cause errors
            generateButton.Enabled = false;

            // Load the unit, so the soldier counts can be fetched
            UnitStatistics stats = await Task.Run(() => UnitBuilder.GetUnitStatistics(selected));

            labelTotalSoldiers.Text = "Total Unit Soldiers: " + stats.TotalSoldiers;
            RankType type = (rankTypeBox.SelectedIndex == -1) ? RankType.Enlisted : (RankType)rankTypeBox.SelectedItem;

            // Setup the enlisted pie chart
            Series series = unitRankPieChart.Series[0];
            foreach (var item in stats.SoldierCountsByRank[type].OrderByDescending(x => RankCache.RanksById[x.Key].Grade))
            {
                if (item.Value > 0)
                {
                    int i = series.Points.AddY(item.Value);
                    Rank rank = RankCache.RanksById[item.Key];
                    series.Points[i].LegendText = rank.Name + ": #VALY";
                }
            }

            // Switch pie charts
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

            // Re-enable the simulate button
            if (!SimulationRan)
                generateButton.Enabled = true;
        }

        #region Tab Controls Events

        private void unitSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTab0Report();
        }

        private async void rankTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected unit template
            UnitTemplate selected = unitSelect.SelectedItem as UnitTemplate;
            if (selected == null) return;

            // Reset pie chart always!
            Series series = unitRankPieChart.Series[0];
            series.Points.Clear();

            // Load the unit, so the soldier counts can be fetched
            UnitStatistics stats = await Task.Run(() => UnitBuilder.GetUnitStatistics(selected));
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

        #endregion Tab Controls Events

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
            if (unitSelect.Items.Count > 0)
                unitSelect.SelectedIndex = 0;

            if (rankTypeBox.Items.Count > 0)
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
            UnitBuilder.ClearCache();

            try
            {
                using (AppDatabase db = new AppDatabase())
                using (SimDatabase simDb = SimDatabase.CreateNew(db, "SimulationResult.db"))
                {
                    // Show the TaskForm
                    TaskForm.Show(this, "Running Simulation", "Building Units...", true);
                    TaskForm.Cancelled += TaskForm_Cancelled;
                    CancelToken = new CancellationTokenSource();

                    // Run the simulation
                    await Task.Run(() =>
                    {
                        // Fill cache
                        SimulationCache.Load(simDb);

                        // Buil units
                        UnitWrapper unit;
                        using (var trans = simDb.BeginTransaction())
                        {
                            // Load the Unit and Soldier xml files
                            UnitTemplate template = db.UnitTemplates.OrderByDescending(x => x.Echelon.HierarchyLevel).FirstOrDefault();
                            unit = UnitBuilder.BuildUnit(simDb, template, TaskForm.Progress, CancelToken.Token);
                            trans.Commit();
                        }

                        // Create settings
                        SimulatorSettings settings = new SimulatorSettings()
                        {
                            ProcessEnlisted = enlistedMenuItem.Checked,
                            ProcessOfficers = officerMenuItem.Checked,
                            ProcessWarrant = warrantMenuItem.Checked
                        };

                        // Create an update
                        TaskProgressUpdate update = new TaskProgressUpdate();
                        update.HeaderText = "Preparing to Run Simulation...";
                        update.MessageText = "Building simulator cache dictionaries...";
                        TaskForm.Progress.Report(update);

                        // Run the simulator, properly disposing afterwards to clear memory
                        using (var Simulation = new Simulator(simDb, unit, settings))
                        {
                            Simulation.Run((int)yearsOfSimulate.Value, (int)yearsToSkip.Value, TaskForm.Progress, CancelToken.Token);
                        }

                        // Clear up memory
                        SimulationCache.Clear();
                        unit = null;
                    });

                    SimulationRan = true;

                    // Finally, close the task form
                    TaskForm.Cancelled -= TaskForm_Cancelled;
                    TaskForm.CloseForm();

                    // Finally, call garbage collector
                    GC.Collect();

                    // Show form
                    using (SimResultViewForm form = new SimResultViewForm(simDb))
                    {
                        form.ShowDialog();
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                TaskForm.Cancelled -= TaskForm_Cancelled;
                TaskForm.CloseForm();
                MessageBox.Show(ex.Message, "Operation Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                TaskForm.Cancelled -= TaskForm_Cancelled;
                TaskForm.CloseForm();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExceptionHandler.GenerateExceptionLog(ex);
            }
            finally
            {
                // Enable Button
                //generateButton.Enabled = true;
            }
        }

        /// <summary>
        /// Event fired when the Cancel button is pushed during a simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskForm_Cancelled(object sender, CancelEventArgs e)
        {
            CancelToken.Cancel();
        }

        #endregion Form Events

        #region Menu Items

        private async void openReportMenuItem_Click(object sender, EventArgs e)
        {
            // Check for an existing report!
            if (!File.Exists(Path.Combine(Program.RootPath, "Data", "SimulationResult.db")))
            {
                MessageBox.Show("No simulation result database found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show Task Form!
            TaskForm.Show(this, "Loading", "Opening existing simulation report... Please Wait", false);

            // Open in a new thread
            await Task.Run(() => 
            { 
                using (AppDatabase db = new AppDatabase())
                using (SimDatabase simDb = SimDatabase.Open("SimulationResult.db"))
                {
                    using (SimResultViewForm form = new SimResultViewForm(simDb))
                    {
                        TaskForm.CloseForm();
                        form.ShowDialog();
                    }
                }
            });
        }

        /// <summary>
        /// Exit application menu button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeMenuItem_Click(object sender, EventArgs e) => this.Close();

        private void soldierConfigMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new CareerGeneratorEditorForm())
            {
                form.ShowDialog();
            }
        }

        private void manageGensMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new RandomizedProcedureForm())
            {
                form.ShowDialog();
            }
        }

        private void manageOrderedMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new OrderedProcedureForm())
            {
                form.ShowDialog();
            }
        }

        private void manageSpecialsMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new SpecialtyEditorForm())
            {
                form.ShowDialog();
            }
        }

        private void manageExpMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new ExperienceForm())
            {
                form.ShowDialog();
            }
        }

        private void manageRanksMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new RankEditorForm())
            {
                form.ShowDialog();
            }
        }

        private void manageTemplatesMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new UnitTypeManagerForm())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Grab selected template
                    var selected = (UnitTemplate)unitSelect.SelectedItem;
                    unitSelect.Items.Clear();
                    bool set = false;

                    // Clear unit stats
                    UnitBuilder.ClearUnitStats();

                    // Redraw menu
                    using (AppDatabase db = new AppDatabase())
                    {
                        foreach (var template in db.UnitTemplates.OrderByDescending(x => x.Echelon.HierarchyLevel))
                        {
                            // Add template
                            unitSelect.Items.Add(template);

                            // Was this template selected before?
                            if (selected.Id == template.Id)
                            {
                                // This will trigger new Pie Charts on the first tab
                                unitSelect.SelectedIndex = unitSelect.Items.Count - 1;
                                set = true;
                            }
                        }
                    }

                    // Update charts
                    if (!set)
                        FillTab0Report();
                }
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
    }
}
