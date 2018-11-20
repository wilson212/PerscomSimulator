namespace Perscom
{
    partial class PositionStatsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PositionStatsForm));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.billetRadioButton = new System.Windows.Forms.RadioButton();
            this.positionRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelLateralsIn = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelPromotionsIn = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.labelAverageTimeInPosition = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelAverageDeficitRateE = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.labelAverageDeficitRateSI = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.labelAverageTiS = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelAverageTIG = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelTotalIncoming = new System.Windows.Forms.Label();
            this.pieChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.labelLateralsOut = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.labelPromotionsOut = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.labelTotalOutgoing = new System.Windows.Forms.Label();
            this.labelRetirements = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.outgoingRadioButton = new System.Windows.Forms.RadioButton();
            this.incomingRadioButton = new System.Windows.Forms.RadioButton();
            this.mainPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pieChart)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox3);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.pieChart);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(786, 499);
            this.mainPanel.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.billetRadioButton);
            this.groupBox1.Controls.Add(this.positionRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(15, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(745, 43);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // billetRadioButton
            // 
            this.billetRadioButton.AutoSize = true;
            this.billetRadioButton.Location = new System.Drawing.Point(394, 16);
            this.billetRadioButton.Name = "billetRadioButton";
            this.billetRadioButton.Size = new System.Drawing.Size(166, 17);
            this.billetRadioButton.TabIndex = 1;
            this.billetRadioButton.Text = "View Global Position Statistics";
            this.billetRadioButton.UseVisualStyleBackColor = true;
            // 
            // positionRadioButton
            // 
            this.positionRadioButton.AutoSize = true;
            this.positionRadioButton.Checked = true;
            this.positionRadioButton.Location = new System.Drawing.Point(185, 16);
            this.positionRadioButton.Name = "positionRadioButton";
            this.positionRadioButton.Size = new System.Drawing.Size(178, 17);
            this.positionRadioButton.TabIndex = 0;
            this.positionRadioButton.TabStop = true;
            this.positionRadioButton.Text = "View Selected Position Statistics";
            this.positionRadioButton.UseVisualStyleBackColor = true;
            this.positionRadioButton.CheckedChanged += new System.EventHandler(this.positionRadioButton_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelRetirements);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.labelLateralsOut);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.labelPromotionsOut);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label26);
            this.groupBox2.Controls.Add(this.labelTotalOutgoing);
            this.groupBox2.Controls.Add(this.labelLateralsIn);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.labelPromotionsIn);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.labelAverageTimeInPosition);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.labelAverageDeficitRateE);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.labelAverageDeficitRateSI);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.labelAverageTiS);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.labelAverageTIG);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.labelTotalIncoming);
            this.groupBox2.Location = new System.Drawing.Point(480, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(280, 382);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Billet Statistical Data";
            // 
            // labelLateralsIn
            // 
            this.labelLateralsIn.AutoSize = true;
            this.labelLateralsIn.Location = new System.Drawing.Point(173, 179);
            this.labelLateralsIn.Name = "labelLateralsIn";
            this.labelLateralsIn.Size = new System.Drawing.Size(13, 13);
            this.labelLateralsIn.TabIndex = 57;
            this.labelLateralsIn.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 56;
            this.label2.Text = "Lateral Promotions In: ";
            // 
            // labelPromotionsIn
            // 
            this.labelPromotionsIn.AutoSize = true;
            this.labelPromotionsIn.Location = new System.Drawing.Point(173, 157);
            this.labelPromotionsIn.Name = "labelPromotionsIn";
            this.labelPromotionsIn.Size = new System.Drawing.Size(13, 13);
            this.labelPromotionsIn.TabIndex = 55;
            this.labelPromotionsIn.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 157);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(102, 13);
            this.label11.TabIndex = 54;
            this.label11.Text = "Newly Promoted In: ";
            // 
            // labelAverageTimeInPosition
            // 
            this.labelAverageTimeInPosition.AutoSize = true;
            this.labelAverageTimeInPosition.Location = new System.Drawing.Point(173, 101);
            this.labelAverageTimeInPosition.Name = "labelAverageTimeInPosition";
            this.labelAverageTimeInPosition.Size = new System.Drawing.Size(51, 13);
            this.labelAverageTimeInPosition.TabIndex = 53;
            this.labelAverageTimeInPosition.Text = "2 Months";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 13);
            this.label4.TabIndex = 52;
            this.label4.Text = "Average Time in Position: ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 57);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(128, 13);
            this.label12.TabIndex = 51;
            this.label12.Text = "Average Deficit Rate (E): ";
            // 
            // labelAverageDeficitRateE
            // 
            this.labelAverageDeficitRateE.AutoSize = true;
            this.labelAverageDeficitRateE.Location = new System.Drawing.Point(172, 57);
            this.labelAverageDeficitRateE.Name = "labelAverageDeficitRateE";
            this.labelAverageDeficitRateE.Size = new System.Drawing.Size(21, 13);
            this.labelAverageDeficitRateE.TabIndex = 50;
            this.labelAverageDeficitRateE.Text = "0%";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(25, 79);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(131, 13);
            this.label16.TabIndex = 49;
            this.label16.Text = "Average Deficit Rate (SI): ";
            // 
            // labelAverageDeficitRateSI
            // 
            this.labelAverageDeficitRateSI.AutoSize = true;
            this.labelAverageDeficitRateSI.Location = new System.Drawing.Point(172, 79);
            this.labelAverageDeficitRateSI.Name = "labelAverageDeficitRateSI";
            this.labelAverageDeficitRateSI.Size = new System.Drawing.Size(21, 13);
            this.labelAverageDeficitRateSI.TabIndex = 48;
            this.labelAverageDeficitRateSI.Text = "0%";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(24, 35);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(91, 13);
            this.label18.TabIndex = 47;
            this.label18.Text = "Position Data: ";
            // 
            // labelAverageTiS
            // 
            this.labelAverageTiS.AutoSize = true;
            this.labelAverageTiS.Location = new System.Drawing.Point(173, 224);
            this.labelAverageTiS.Name = "labelAverageTiS";
            this.labelAverageTiS.Size = new System.Drawing.Size(51, 13);
            this.labelAverageTiS.TabIndex = 45;
            this.labelAverageTiS.Text = "2 Months";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 223);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(129, 13);
            this.label9.TabIndex = 44;
            this.label9.Text = "Average Time in Service: ";
            // 
            // labelAverageTIG
            // 
            this.labelAverageTIG.AutoSize = true;
            this.labelAverageTIG.Location = new System.Drawing.Point(173, 202);
            this.labelAverageTIG.Name = "labelAverageTIG";
            this.labelAverageTIG.Size = new System.Drawing.Size(51, 13);
            this.labelAverageTIG.TabIndex = 43;
            this.labelAverageTIG.Text = "2 Months";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "Average Time in Grade: ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(23, 135);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 13);
            this.label13.TabIndex = 15;
            this.label13.Text = "Total Incoming: ";
            // 
            // labelTotalIncoming
            // 
            this.labelTotalIncoming.AutoSize = true;
            this.labelTotalIncoming.Location = new System.Drawing.Point(172, 135);
            this.labelTotalIncoming.Name = "labelTotalIncoming";
            this.labelTotalIncoming.Size = new System.Drawing.Size(13, 13);
            this.labelTotalIncoming.TabIndex = 14;
            this.labelTotalIncoming.Text = "0";
            // 
            // pieChart
            // 
            chartArea1.Name = "ChartArea1";
            this.pieChart.ChartAreas.Add(chartArea1);
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            this.pieChart.Legends.Add(legend1);
            this.pieChart.Location = new System.Drawing.Point(15, 81);
            this.pieChart.Name = "pieChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.YValuesPerPoint = 4;
            this.pieChart.Series.Add(series1);
            this.pieChart.Size = new System.Drawing.Size(433, 338);
            this.pieChart.TabIndex = 3;
            this.pieChart.Text = "chart3";
            title1.Name = "Title1";
            title1.Text = "Soldier Incoming Ratio";
            title2.Name = "Title2";
            this.pieChart.Titles.Add(title1);
            this.pieChart.Titles.Add(title2);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.closeButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 574);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(786, 60);
            this.bottomPanel.TabIndex = 13;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(325, 18);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(150, 30);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.label6);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(786, 75);
            this.headerPanel.TabIndex = 12;
            this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.headerPanel_Paint);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(26, 22);
            this.label6.Name = "label6";
            this.label6.ShadowDirection = 90;
            this.label6.ShadowOpacity = 225;
            this.label6.ShadowSoftness = 3F;
            this.label6.Size = new System.Drawing.Size(787, 37);
            this.label6.TabIndex = 0;
            this.label6.Text = "Position Statistics";
            // 
            // labelLateralsOut
            // 
            this.labelLateralsOut.AutoSize = true;
            this.labelLateralsOut.Location = new System.Drawing.Point(173, 302);
            this.labelLateralsOut.Name = "labelLateralsOut";
            this.labelLateralsOut.Size = new System.Drawing.Size(13, 13);
            this.labelLateralsOut.TabIndex = 67;
            this.labelLateralsOut.Text = "0";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(25, 302);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(120, 13);
            this.label19.TabIndex = 66;
            this.label19.Text = "Lateral Promotions Out: ";
            // 
            // labelPromotionsOut
            // 
            this.labelPromotionsOut.AutoSize = true;
            this.labelPromotionsOut.Location = new System.Drawing.Point(173, 279);
            this.labelPromotionsOut.Name = "labelPromotionsOut";
            this.labelPromotionsOut.Size = new System.Drawing.Size(13, 13);
            this.labelPromotionsOut.TabIndex = 65;
            this.labelPromotionsOut.Text = "0";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(25, 279);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(85, 13);
            this.label21.TabIndex = 64;
            this.label21.Text = "Promotions Out: ";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(23, 256);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(99, 13);
            this.label26.TabIndex = 59;
            this.label26.Text = "Total Outgoing: ";
            // 
            // labelTotalOutgoing
            // 
            this.labelTotalOutgoing.AutoSize = true;
            this.labelTotalOutgoing.Location = new System.Drawing.Point(173, 256);
            this.labelTotalOutgoing.Name = "labelTotalOutgoing";
            this.labelTotalOutgoing.Size = new System.Drawing.Size(13, 13);
            this.labelTotalOutgoing.TabIndex = 58;
            this.labelTotalOutgoing.Text = "0";
            // 
            // labelRetirements
            // 
            this.labelRetirements.AutoSize = true;
            this.labelRetirements.Location = new System.Drawing.Point(173, 325);
            this.labelRetirements.Name = "labelRetirements";
            this.labelRetirements.Size = new System.Drawing.Size(13, 13);
            this.labelRetirements.TabIndex = 69;
            this.labelRetirements.Text = "0";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(25, 325);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(69, 13);
            this.label23.TabIndex = 68;
            this.label23.Text = "Retirements: ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.outgoingRadioButton);
            this.groupBox3.Controls.Add(this.incomingRadioButton);
            this.groupBox3.Location = new System.Drawing.Point(15, 420);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(451, 43);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            // 
            // outgoingRadioButton
            // 
            this.outgoingRadioButton.AutoSize = true;
            this.outgoingRadioButton.Location = new System.Drawing.Point(240, 16);
            this.outgoingRadioButton.Name = "outgoingRadioButton";
            this.outgoingRadioButton.Size = new System.Drawing.Size(68, 17);
            this.outgoingRadioButton.TabIndex = 1;
            this.outgoingRadioButton.Text = "Outgoing";
            this.outgoingRadioButton.UseVisualStyleBackColor = true;
            // 
            // incomingRadioButton
            // 
            this.incomingRadioButton.AutoSize = true;
            this.incomingRadioButton.Checked = true;
            this.incomingRadioButton.Location = new System.Drawing.Point(142, 16);
            this.incomingRadioButton.Name = "incomingRadioButton";
            this.incomingRadioButton.Size = new System.Drawing.Size(68, 17);
            this.incomingRadioButton.TabIndex = 0;
            this.incomingRadioButton.TabStop = true;
            this.incomingRadioButton.Text = "Incoming";
            this.incomingRadioButton.UseVisualStyleBackColor = true;
            this.incomingRadioButton.CheckedChanged += new System.EventHandler(this.incomingRadioButton_CheckedChanged);
            // 
            // PositionStatsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(786, 634);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PositionStatsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Position Statistics";
            this.mainPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pieChart)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton billetRadioButton;
        private System.Windows.Forms.RadioButton positionRadioButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelTotalIncoming;
        private System.Windows.Forms.DataVisualization.Charting.Chart pieChart;
        private System.Windows.Forms.Label labelAverageTIG;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelLateralsIn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelPromotionsIn;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelAverageTimeInPosition;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelAverageDeficitRateE;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label labelAverageDeficitRateSI;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label labelAverageTiS;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelRetirements;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label labelLateralsOut;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label labelPromotionsOut;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label labelTotalOutgoing;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton outgoingRadioButton;
        private System.Windows.Forms.RadioButton incomingRadioButton;
    }
}