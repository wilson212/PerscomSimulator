namespace Perscom
{
    partial class RankEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RankEditor));
            bottomPanel = new System.Windows.Forms.Panel();
            deleteButton = new Telerik.WinControls.UI.RadButton();
            saveButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.ShadowLabel();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            radLabel3 = new Telerik.WinControls.UI.RadLabel();
            isPositionalCheckBox = new Telerik.WinControls.UI.RadCheckBox();
            radLabel4 = new Telerik.WinControls.UI.RadLabel();
            rankNameTextBox = new Telerik.WinControls.UI.RadTextBox();
            rankAbbrTextBox = new Telerik.WinControls.UI.RadTextBox();
            precedenceSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            rankImageSelector = new RankImageSelector();
            nextRankSelector = new RadRankSelector();
            stipendAmountSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel5 = new Telerik.WinControls.UI.RadLabel();
            radLabel6 = new Telerik.WinControls.UI.RadLabel();
            overrideRadioButton = new Telerik.WinControls.UI.RadRadioButton();
            offsetRadioButton = new Telerik.WinControls.UI.RadRadioButton();
            inheritRadioButton = new Telerik.WinControls.UI.RadRadioButton();
            radPictureBox1 = new Telerik.WinControls.UI.RadPictureBox();
            boardButton = new Telerik.WinControls.UI.RadButton();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)deleteButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)saveButton).BeginInit();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)isPositionalCheckBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rankNameTextBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rankAbbrTextBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)precedenceSpinEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)stipendAmountSpinEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)overrideRadioButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)offsetRadioButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inheritRadioButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radPictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(deleteButton);
            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 618);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(507, 50);
            bottomPanel.TabIndex = 17;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // deleteButton
            // 
            deleteButton.Location = new System.Drawing.Point(19, 14);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new System.Drawing.Size(110, 24);
            deleteButton.TabIndex = 1;
            deleteButton.Text = "Delete Rank";
            deleteButton.ThemeName = "Fluent";
            deleteButton.Click += deleteButton_Click;
            // 
            // saveButton
            // 
            saveButton.Location = new System.Drawing.Point(375, 14);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(110, 24);
            saveButton.TabIndex = 0;
            saveButton.Text = "Save";
            saveButton.ThemeName = "Fluent";
            saveButton.Click += saveButton_Click;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(label6);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(507, 75);
            headerPanel.TabIndex = 16;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // headerLabel
            // 
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.SystemColors.Control;
            label6.Location = new System.Drawing.Point(26, 22);
            label6.Name = "label6";
            label6.ShadowDirection = 60;
            label6.ShadowOpacity = 180;
            label6.ShadowSoftness = 3F;
            label6.Size = new System.Drawing.Size(237, 37);
            label6.TabIndex = 0;
            label6.Text = "Rank Editor";
            // 
            // radLabel1
            // 
            radLabel1.Location = new System.Drawing.Point(32, 276);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(69, 18);
            radLabel1.TabIndex = 18;
            radLabel1.Text = "Rank Name: ";
            radLabel1.ThemeName = "Fluent";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(32, 327);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(103, 18);
            radLabel2.TabIndex = 19;
            radLabel2.Text = "Rank Abbreviation: ";
            radLabel2.ThemeName = "Fluent";
            // 
            // radLabel3
            // 
            radLabel3.Location = new System.Drawing.Point(32, 373);
            radLabel3.Name = "radLabel3";
            radLabel3.Size = new System.Drawing.Size(69, 18);
            radLabel3.TabIndex = 20;
            radLabel3.Text = "Precedence: ";
            radLabel3.ThemeName = "Fluent";
            // 
            // isPositionalCheckBox
            // 
            isPositionalCheckBox.Location = new System.Drawing.Point(94, 519);
            isPositionalCheckBox.Name = "isPositionalCheckBox";
            isPositionalCheckBox.Size = new System.Drawing.Size(318, 18);
            isPositionalCheckBox.TabIndex = 21;
            isPositionalCheckBox.Text = "Is Positional Rank (Only earned and maintaned by Position)";
            isPositionalCheckBox.ThemeName = "Fluent";
            // 
            // radLabel4
            // 
            radLabel4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            radLabel4.Location = new System.Drawing.Point(124, 240);
            radLabel4.Name = "radLabel4";
            radLabel4.Size = new System.Drawing.Size(259, 18);
            radLabel4.TabIndex = 23;
            radLabel4.Text = "Only select Next Rank if using Split Rank Lanes ";
            radLabel4.ThemeName = "Fluent";
            // 
            // rankNameTextBox
            // 
            rankNameTextBox.Location = new System.Drawing.Point(187, 278);
            rankNameTextBox.Name = "rankNameTextBox";
            rankNameTextBox.Size = new System.Drawing.Size(275, 24);
            rankNameTextBox.TabIndex = 24;
            rankNameTextBox.ThemeName = "Fluent";
            // 
            // rankAbbrTextBox
            // 
            rankAbbrTextBox.Location = new System.Drawing.Point(187, 326);
            rankAbbrTextBox.Name = "rankAbbrTextBox";
            rankAbbrTextBox.Size = new System.Drawing.Size(275, 24);
            rankAbbrTextBox.TabIndex = 25;
            rankAbbrTextBox.ThemeName = "Fluent";
            // 
            // precedenceSpinEditor
            // 
            precedenceSpinEditor.Location = new System.Drawing.Point(187, 372);
            precedenceSpinEditor.Name = "precedenceSpinEditor";
            precedenceSpinEditor.Size = new System.Drawing.Size(276, 24);
            precedenceSpinEditor.TabIndex = 26;
            precedenceSpinEditor.ThemeName = "Fluent";
            // 
            // rankImageSelector
            // 
            rankImageSelector.BackColor = System.Drawing.SystemColors.ControlLightLight;
            rankImageSelector.ImagePadding = 4;
            rankImageSelector.Location = new System.Drawing.Point(37, 81);
            rankImageSelector.Name = "rankImageSelector";
            rankImageSelector.OutlineColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            rankImageSelector.RankText = "This Rank Insignia";
            rankImageSelector.InstructionText = "Click to Change";
            rankImageSelector.ShadowColor = System.Drawing.Color.FromArgb(120, 0, 0, 0);
            rankImageSelector.Size = new System.Drawing.Size(156, 156);
            rankImageSelector.TabIndex = 29;
            rankImageSelector.OnImageChanged += rankImageSelector_OnImageChanged;
            // 
            // nextRankSelector
            // 
            nextRankSelector.BackColor = System.Drawing.Color.White;
            nextRankSelector.ImagePadding = 4;
            nextRankSelector.Location = new System.Drawing.Point(314, 81);
            nextRankSelector.Name = "nextRankSelector";
            nextRankSelector.OutlineColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            nextRankSelector.Rank = null;
            nextRankSelector.RankName = "Click to Select";
            nextRankSelector.RankTitle = "Next Rank";
            nextRankSelector.ShadowColor = System.Drawing.Color.FromArgb(120, 0, 0, 0);
            nextRankSelector.ShowNextRank = false;
            nextRankSelector.Size = new System.Drawing.Size(156, 156);
            nextRankSelector.TabIndex = 30;
            nextRankSelector.OnClick += nextRankSelector_Click;
            // 
            // stipendAmountSpinEditor
            // 
            stipendAmountSpinEditor.Location = new System.Drawing.Point(187, 462);
            stipendAmountSpinEditor.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            stipendAmountSpinEditor.Name = "stipendAmountSpinEditor";
            stipendAmountSpinEditor.Size = new System.Drawing.Size(276, 24);
            stipendAmountSpinEditor.TabIndex = 32;
            stipendAmountSpinEditor.ThemeName = "Fluent";
            stipendAmountSpinEditor.ThousandsSeparator = true;
            // 
            // radLabel5
            // 
            radLabel5.Location = new System.Drawing.Point(32, 463);
            radLabel5.Name = "radLabel5";
            radLabel5.Size = new System.Drawing.Size(93, 18);
            radLabel5.TabIndex = 31;
            radLabel5.Text = "Stipend Amount: ";
            radLabel5.ThemeName = "Fluent";
            // 
            // radLabel6
            // 
            radLabel6.Location = new System.Drawing.Point(32, 424);
            radLabel6.Name = "radLabel6";
            radLabel6.Size = new System.Drawing.Size(83, 18);
            radLabel6.TabIndex = 33;
            radLabel6.Text = "Stipend Mode: ";
            radLabel6.ThemeName = "Fluent";
            // 
            // overrideRadioButton
            // 
            overrideRadioButton.Location = new System.Drawing.Point(287, 424);
            overrideRadioButton.Name = "overrideRadioButton";
            overrideRadioButton.Size = new System.Drawing.Size(64, 18);
            overrideRadioButton.TabIndex = 34;
            overrideRadioButton.TabStop = false;
            overrideRadioButton.Text = "Override";
            overrideRadioButton.ThemeName = "Fluent";
            // 
            // offsetRadioButton
            // 
            offsetRadioButton.Location = new System.Drawing.Point(398, 424);
            offsetRadioButton.Name = "offsetRadioButton";
            offsetRadioButton.Size = new System.Drawing.Size(51, 18);
            offsetRadioButton.TabIndex = 35;
            offsetRadioButton.TabStop = false;
            offsetRadioButton.Text = "Offset";
            offsetRadioButton.ThemeName = "Fluent";
            // 
            // inheritRadioButton
            // 
            inheritRadioButton.CheckState = System.Windows.Forms.CheckState.Checked;
            inheritRadioButton.Location = new System.Drawing.Point(187, 424);
            inheritRadioButton.Name = "inheritRadioButton";
            inheritRadioButton.Size = new System.Drawing.Size(53, 18);
            inheritRadioButton.TabIndex = 36;
            inheritRadioButton.Text = "Inherit";
            inheritRadioButton.ThemeName = "Fluent";
            // 
            // radPictureBox1
            // 
            radPictureBox1.Image = Properties.Resources.go_next;
            radPictureBox1.Location = new System.Drawing.Point(214, 136);
            radPictureBox1.Name = "radPictureBox1";
            radPictureBox1.Size = new System.Drawing.Size(78, 48);
            radPictureBox1.TabIndex = 37;
            // 
            // boardButton
            // 
            boardButton.Location = new System.Drawing.Point(148, 563);
            boardButton.Name = "boardButton";
            boardButton.Size = new System.Drawing.Size(210, 32);
            boardButton.TabIndex = 38;
            boardButton.Text = "Promotion Board Settings";
            boardButton.ThemeName = "Fluent";
            boardButton.Click += boardButton_Click;
            // 
            // RankEditor
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(507, 668);
            Controls.Add(boardButton);
            Controls.Add(radPictureBox1);
            Controls.Add(inheritRadioButton);
            Controls.Add(offsetRadioButton);
            Controls.Add(overrideRadioButton);
            Controls.Add(radLabel6);
            Controls.Add(stipendAmountSpinEditor);
            Controls.Add(radLabel5);
            Controls.Add(nextRankSelector);
            Controls.Add(rankImageSelector);
            Controls.Add(precedenceSpinEditor);
            Controls.Add(rankAbbrTextBox);
            Controls.Add(rankNameTextBox);
            Controls.Add(radLabel4);
            Controls.Add(isPositionalCheckBox);
            Controls.Add(radLabel3);
            Controls.Add(radLabel2);
            Controls.Add(radLabel1);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RankEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Rank Editor";
            ThemeName = "Fluent";
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)deleteButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)saveButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).EndInit();
            ((System.ComponentModel.ISupportInitialize)isPositionalCheckBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel4).EndInit();
            ((System.ComponentModel.ISupportInitialize)rankNameTextBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)rankAbbrTextBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)precedenceSpinEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)stipendAmountSpinEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel6).EndInit();
            ((System.ComponentModel.ISupportInitialize)overrideRadioButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)offsetRadioButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)inheritRadioButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)radPictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private Telerik.WinControls.UI.RadButton saveButton;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadCheckBox isPositionalCheckBox;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadTextBox rankNameTextBox;
        private Telerik.WinControls.UI.RadTextBox rankAbbrTextBox;
        private Telerik.WinControls.UI.RadSpinEditor precedenceSpinEditor;
        private RankImageSelector rankImageSelector;
        private RadRankSelector nextRankSelector;
        private Telerik.WinControls.UI.RadSpinEditor stipendAmountSpinEditor;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadRadioButton overrideRadioButton;
        private Telerik.WinControls.UI.RadRadioButton offsetRadioButton;
        private Telerik.WinControls.UI.RadRadioButton inheritRadioButton;
        private Telerik.WinControls.UI.RadPictureBox radPictureBox1;
        private Telerik.WinControls.UI.RadButton boardButton;
        private Telerik.WinControls.UI.RadButton deleteButton;
    }
}
