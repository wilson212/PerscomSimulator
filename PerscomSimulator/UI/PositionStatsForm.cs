using Perscom.Database;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Perscom
{
    public partial class PositionStatsForm : Form
    {
        protected IterationDate Date { get; set; }

        protected double TotalYears { get; set; }

        protected Billet Billet { get; set; }

        protected AbstractBilletStatistics PositionStats { get; set; }

        protected AbstractBilletStatistics BilletStats { get; set; }

        protected int PositionCount { get; set; }

        public PositionStatsForm(SimDatabase db, int PositionId, IterationDate date)
        {
            InitializeComponent();

            pieChart.Series[0].Label = "#VALY (#PERCENT{P0})";
            pieChart.Series[0]["PieLabelStyle"] = "Outside";
            pieChart.Series[0]["PieLineColor"] = "Black";
            pieChart.ChartAreas[0].Area3DStyle.Enable3D = true;
            pieChart.ChartAreas[0].Area3DStyle.Inclination = 60;

            // Grab billet ID
            int billetId = db.ExecuteScalar<int>("SELECT BilletId FROM Position WHERE Id=@P0", PositionId);

            // Grab billet
            Billet = db.Query<Billet>("SELECT * FROM Billet WHERE Id=@P0", billetId).First();

            // Grab billet stats
            BilletStats = db.Query<BilletStatistics>("SELECT * FROM BilletStatistics WHERE BilletId=@P0", billetId).First();

            // Grab position count
            PositionCount = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Position WHERE BilletId=@P0", billetId);

            // Grab Position stats
            PositionStats = db.Query<PositionStatistics>("SELECT * FROM PositionStatistics WHERE PositionId=@P0", PositionId).First();

            // Grab Unit Template Name
            string query = "SELECT u.Name FROM Billet AS b JOIN UnitTemplate AS u ON b.UnitTypeId = u.Id WHERE b.Id=@P0";
            string unitName = db.ExecuteScalar<string>(query, billetId);

            // Fill vars
            pieChart.Titles[1].Text = $"{Billet.Name} ({unitName})";
            Date = date;
            TotalYears = Date.Id / 12;

            // Fill initial chart data
            FillPieChart();
            FillLabels();
        }

        private void FillPieChart()
        {
            AbstractBilletStatistics stats = (positionRadioButton.Checked) ? PositionStats : BilletStats;
            Series series = pieChart.Series[0];

            // Clear Chart
            series.Points.Clear();

            // Add new pie chart data
            if (outgoingRadioButton.Checked)
            {
                // Soldier Outgoing Ratio
                pieChart.Titles[0].Text = "Soldier Outgoing Ratio";

                // Plot soldiers promoted
                int i = series.Points.AddY(stats.TotalSoldiersPromotedOut);
                series.Points[i].LegendText = "Promoted";

                // Plot soldiers retired
                i = series.Points.AddY(stats.TotalSoldiersRetireOut);
                series.Points[i].LegendText = "Retired";

                // Plot soldiers transferred out
                if (stats.TotalSoldiersTransferredOut > 0)
                {
                    i = series.Points.AddY(stats.TotalSoldiersTransferredOut);
                    series.Points[i].LegendText = "Transferred Branches";
                }

                // Plos transfered soldiers
                i = series.Points.AddY(stats.TotalSoldiersLateralOut);
                series.Points[i].LegendText = "Laterally Transfered";
            }
            else
            {
                // Soldier Incoming Ratio
                pieChart.Titles[0].Text = "Soldier Incoming Ratio";

                // Plot soldiers promoted
                int i = series.Points.AddY(stats.TotalSoldiersPromotedIn);
                series.Points[i].LegendText = "Promoted";

                // Plos transfered soldiers
                if (stats.TotalSoldiersLateralIn > 0)
                {
                    i = series.Points.AddY(stats.TotalSoldiersLateralIn);
                    series.Points[i].LegendText = "Laterally Transfered";
                }

                // Plot soldiers transferred out
                i = series.Points.AddY(stats.TotalSoldiersTransferredIn);
                series.Points[i].LegendText = "Transferred Branches";
            }
        }

        private void FillLabels()
        {
            AbstractBilletStatistics stats = (positionRadioButton.Checked) ? PositionStats : BilletStats;

            // Set label texts for statistical data
            labelTotalIncoming.Text = String.Format("{0:N0}", stats.TotalSoldiersIncoming);
            labelPromotionsIn.Text = String.Format("{0:N0}", stats.TotalSoldiersPromotedIn);
            labelLateralsIn.Text = String.Format("{0:N0}", stats.TotalSoldiersLateralIn);
            labelTransffersIn.Text = String.Format("{0:N0}", stats.TotalSoldiersTransferredIn);
            labelAverageTIG.Text = String.Format("{0} months", Math.Round(stats.AverageTimeInGradeIncoming));
            labelAverageTiS.Text = String.Format("{0} years", Math.Round(stats.AverageTimeInServiceIncoming / 12, 1));

            labelTotalOutgoing.Text = String.Format("{0:N0}", stats.TotalSoldiersOutgoing);
            labelPromotionsOut.Text = String.Format("{0:N0}", stats.TotalSoldiersPromotedOut);
            labelLateralsOut.Text = String.Format("{0:N0}", stats.TotalSoldiersLateralOut);
            labelTransffersOut.Text = String.Format("{0:N0}", stats.TotalSoldiersTransferredOut);
            labelRetirements.Text = String.Format("{0:N0}", stats.TotalSoldiersRetireOut);

            // Get defecit data
            labelAverageDeficitRateE.Text = String.Format("{0}%", GetAverageDeficit(stats.EmptyDeficit));
            labelAverageDeficitRateSI.Text = String.Format("{0}%", GetAverageDeficit(stats.StandInDeficit));

            // Get average time in position
            labelAverageTimeInPosition.Text = String.Format("{0} months", stats.AverageTimeInPosition);
        }

        private double GetAverageDeficit(int totalDeficit)
        {
            double totalMonthsRan = (TotalYears * 12);
            int positions = (positionRadioButton.Checked) ? 1 : PositionCount;
            double cumulative = positions * totalMonthsRan;

            return Math.Round((totalDeficit / cumulative) * 100, 2);
        }

        private void positionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            FillLabels();
            FillPieChart();
        }

        private void incomingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            FillPieChart();
        }

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
    }
}
