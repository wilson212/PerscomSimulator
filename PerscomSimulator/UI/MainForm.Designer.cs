namespace Perscom
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.openRootMenuItem = new System.Windows.Forms.MenuItem();
            this.openReportMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.closeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.enlistedMenuItem = new System.Windows.Forms.MenuItem();
            this.officerMenuItem = new System.Windows.Forms.MenuItem();
            this.warrantMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.soldierConfigMenuItem = new System.Windows.Forms.MenuItem();
            this.manageGensMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.manageSpecialsMenuItem = new System.Windows.Forms.MenuItem();
            this.manageRanksMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.manageTemplatesMenuItem = new System.Windows.Forms.MenuItem();
            this.panel4 = new System.Windows.Forms.Panel();
            this.generateButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.unitSelect = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.yearsToSkip = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.yearsOfSimulate = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.CustomPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rankTypeBox = new System.Windows.Forms.ComboBox();
            this.labelTotalSoldiers = new System.Windows.Forms.Label();
            this.unitPersonelPieChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.unitRankPieChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.nameLabel = new System.Windows.Forms.ShadowLabel();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yearsToSkip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yearsOfSimulate)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unitPersonelPieChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitRankPieChart)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.openRootMenuItem,
            this.openReportMenuItem,
            this.menuItem5,
            this.closeMenuItem});
            this.menuItem1.Text = "File";
            // 
            // openRootMenuItem
            // 
            this.openRootMenuItem.Index = 0;
            this.openRootMenuItem.Text = "Open Program Folder";
            this.openRootMenuItem.Click += new System.EventHandler(this.openRootMenuItem_Click);
            // 
            // openReportMenuItem
            // 
            this.openReportMenuItem.Index = 1;
            this.openReportMenuItem.Text = "Open Existing Report";
            this.openReportMenuItem.Click += new System.EventHandler(this.openReportMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 2;
            this.menuItem5.Text = "-";
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Index = 3;
            this.closeMenuItem.Text = "Exit";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem3,
            this.menuItem4,
            this.soldierConfigMenuItem,
            this.manageGensMenuItem,
            this.menuItem6,
            this.manageSpecialsMenuItem,
            this.manageRanksMenuItem,
            this.menuItem7,
            this.manageTemplatesMenuItem});
            this.menuItem2.Text = "Settings";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 0;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.enlistedMenuItem,
            this.officerMenuItem,
            this.warrantMenuItem});
            this.menuItem3.Text = "Parse Soldier Types";
            // 
            // enlistedMenuItem
            // 
            this.enlistedMenuItem.Checked = true;
            this.enlistedMenuItem.Index = 0;
            this.enlistedMenuItem.Text = "Enlisted";
            this.enlistedMenuItem.Click += new System.EventHandler(this.enlistedMenuItem_Click);
            // 
            // officerMenuItem
            // 
            this.officerMenuItem.Checked = true;
            this.officerMenuItem.Index = 1;
            this.officerMenuItem.Text = "Officer";
            this.officerMenuItem.Click += new System.EventHandler(this.officerMenuItem_Click);
            // 
            // warrantMenuItem
            // 
            this.warrantMenuItem.Checked = true;
            this.warrantMenuItem.Index = 2;
            this.warrantMenuItem.Text = "Warrant";
            this.warrantMenuItem.Click += new System.EventHandler(this.warrantMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            this.menuItem4.Text = "-";
            // 
            // soldierConfigMenuItem
            // 
            this.soldierConfigMenuItem.Index = 2;
            this.soldierConfigMenuItem.Text = "Manage Soldier Career Lengths";
            this.soldierConfigMenuItem.Click += new System.EventHandler(this.soldierConfigMenuItem_Click);
            // 
            // manageGensMenuItem
            // 
            this.manageGensMenuItem.Index = 3;
            this.manageGensMenuItem.Text = "Manage Soldier Generators";
            this.manageGensMenuItem.Click += new System.EventHandler(this.manageGensMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 4;
            this.menuItem6.Text = "-";
            // 
            // manageSpecialsMenuItem
            // 
            this.manageSpecialsMenuItem.Index = 5;
            this.manageSpecialsMenuItem.Text = "Manage Specialties";
            this.manageSpecialsMenuItem.Click += new System.EventHandler(this.manageSpecialsMenuItem_Click);
            // 
            // manageRanksMenuItem
            // 
            this.manageRanksMenuItem.Index = 6;
            this.manageRanksMenuItem.Text = "Manage Ranks";
            this.manageRanksMenuItem.Click += new System.EventHandler(this.manageRanksMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 7;
            this.menuItem7.Text = "-";
            // 
            // manageTemplatesMenuItem
            // 
            this.manageTemplatesMenuItem.Index = 8;
            this.manageTemplatesMenuItem.Text = "Manage Unit Templates";
            this.manageTemplatesMenuItem.Click += new System.EventHandler(this.manageTemplatesMenuItem_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.panel4.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.panel4.Controls.Add(this.generateButton);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 662);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(984, 50);
            this.panel4.TabIndex = 5;
            this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(417, 9);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(150, 32);
            this.generateButton.TabIndex = 2;
            this.generateButton.Text = "Begin Simulation";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.panel3.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.unitSelect);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.yearsToSkip);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.yearsOfSimulate);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 75);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(984, 40);
            this.panel3.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(862, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "years";
            // 
            // unitSelect
            // 
            this.unitSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.unitSelect.FormattingEnabled = true;
            this.unitSelect.Location = new System.Drawing.Point(159, 10);
            this.unitSelect.Name = "unitSelect";
            this.unitSelect.Size = new System.Drawing.Size(174, 21);
            this.unitSelect.TabIndex = 0;
            this.unitSelect.SelectedIndexChanged += new System.EventHandler(this.unitSelect_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Location = new System.Drawing.Point(646, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Initial Years To Skip:";
            // 
            // yearsToSkip
            // 
            this.yearsToSkip.BackColor = System.Drawing.SystemColors.Window;
            this.yearsToSkip.ForeColor = System.Drawing.SystemColors.WindowText;
            this.yearsToSkip.Location = new System.Drawing.Point(791, 8);
            this.yearsToSkip.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.yearsToSkip.Name = "yearsToSkip";
            this.yearsToSkip.Size = new System.Drawing.Size(65, 20);
            this.yearsToSkip.TabIndex = 12;
            this.yearsToSkip.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(82, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Unit Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(354, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Time to Run Simulation:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label7.Location = new System.Drawing.Point(593, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "years";
            // 
            // yearsOfSimulate
            // 
            this.yearsOfSimulate.BackColor = System.Drawing.SystemColors.Window;
            this.yearsOfSimulate.ForeColor = System.Drawing.SystemColors.WindowText;
            this.yearsOfSimulate.Location = new System.Drawing.Point(522, 8);
            this.yearsOfSimulate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.yearsOfSimulate.Name = "yearsOfSimulate";
            this.yearsOfSimulate.Size = new System.Drawing.Size(65, 20);
            this.yearsOfSimulate.TabIndex = 2;
            this.yearsOfSimulate.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Perscom.Properties.Resources.background;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 637);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BorderColor = System.Drawing.Color.Gray;
            this.panel2.BorderRounded = true;
            this.panel2.BorderRoundRadius = 5;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.BorderWidth = 1;
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Location = new System.Drawing.Point(50, 53);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(874, 495);
            this.panel2.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(14, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(843, 471);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rankTypeBox);
            this.tabPage1.Controls.Add(this.labelTotalSoldiers);
            this.tabPage1.Controls.Add(this.unitPersonelPieChart);
            this.tabPage1.Controls.Add(this.unitRankPieChart);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(835, 445);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Unit Breakdown";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rankTypeBox
            // 
            this.rankTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankTypeBox.FormattingEnabled = true;
            this.rankTypeBox.Location = new System.Drawing.Point(107, 40);
            this.rankTypeBox.Name = "rankTypeBox";
            this.rankTypeBox.Size = new System.Drawing.Size(174, 21);
            this.rankTypeBox.TabIndex = 3;
            this.rankTypeBox.SelectedIndexChanged += new System.EventHandler(this.rankTypeBox_SelectedIndexChanged);
            // 
            // labelTotalSoldiers
            // 
            this.labelTotalSoldiers.Location = new System.Drawing.Point(470, 40);
            this.labelTotalSoldiers.Name = "labelTotalSoldiers";
            this.labelTotalSoldiers.Size = new System.Drawing.Size(348, 23);
            this.labelTotalSoldiers.TabIndex = 2;
            this.labelTotalSoldiers.Text = "Total Unit Soldiers: ";
            this.labelTotalSoldiers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // unitPersonelPieChart
            // 
            chartArea1.Name = "ChartArea1";
            this.unitPersonelPieChart.ChartAreas.Add(chartArea1);
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            this.unitPersonelPieChart.Legends.Add(legend1);
            this.unitPersonelPieChart.Location = new System.Drawing.Point(473, 90);
            this.unitPersonelPieChart.Name = "unitPersonelPieChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.YValuesPerPoint = 4;
            this.unitPersonelPieChart.Series.Add(series1);
            this.unitPersonelPieChart.Size = new System.Drawing.Size(345, 330);
            this.unitPersonelPieChart.TabIndex = 1;
            this.unitPersonelPieChart.Text = "chart3";
            title1.Name = "Title1";
            title1.Text = "Soldier Type Breakdown";
            title1.Visible = false;
            this.unitPersonelPieChart.Titles.Add(title1);
            // 
            // unitRankPieChart
            // 
            chartArea2.Name = "ChartArea1";
            this.unitRankPieChart.ChartAreas.Add(chartArea2);
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend2.Name = "Legend1";
            this.unitRankPieChart.Legends.Add(legend2);
            this.unitRankPieChart.Location = new System.Drawing.Point(17, 90);
            this.unitRankPieChart.Name = "unitRankPieChart";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            series2.YValuesPerPoint = 4;
            this.unitRankPieChart.Series.Add(series2);
            this.unitRankPieChart.Size = new System.Drawing.Size(345, 330);
            this.unitRankPieChart.TabIndex = 0;
            this.unitRankPieChart.Text = "chart3";
            title2.Name = "Title1";
            title2.Text = "Enlisted Breakdown";
            title2.Visible = false;
            this.unitRankPieChart.Titles.Add(title2);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.nameLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(984, 75);
            this.headerPanel.TabIndex = 0;
            this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.headerPanel_Paint);
            // 
            // nameLabel
            // 
            this.nameLabel.BackColor = System.Drawing.Color.Transparent;
            this.nameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.nameLabel.Location = new System.Drawing.Point(325, 21);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.ShadowDirection = 90;
            this.nameLabel.ShadowOpacity = 225;
            this.nameLabel.ShadowSoftness = 3F;
            this.nameLabel.Size = new System.Drawing.Size(335, 34);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Perscom Movement Simulator";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(984, 712);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(16, 750);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Military Personnel  Movement Simulator";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yearsToSkip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yearsOfSimulate)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.unitPersonelPieChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitRankPieChart)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox unitSelect;
        private System.Windows.Forms.NumericUpDown yearsOfSimulate;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataVisualization.Charting.Chart unitRankPieChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart unitPersonelPieChart;
        private System.Windows.Forms.Label labelTotalSoldiers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown yearsToSkip;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem closeMenuItem;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem soldierConfigMenuItem;
        private System.Windows.Forms.MenuItem openRootMenuItem;
        private System.Windows.Forms.CustomPanel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ShadowLabel nameLabel;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem enlistedMenuItem;
        private System.Windows.Forms.MenuItem officerMenuItem;
        private System.Windows.Forms.MenuItem warrantMenuItem;
        private System.Windows.Forms.ComboBox rankTypeBox;
        private System.Windows.Forms.MenuItem manageRanksMenuItem;
        private System.Windows.Forms.MenuItem manageTemplatesMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem manageSpecialsMenuItem;
        private System.Windows.Forms.MenuItem manageGensMenuItem;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem openReportMenuItem;
        private System.Windows.Forms.MenuItem menuItem5;
    }
}

