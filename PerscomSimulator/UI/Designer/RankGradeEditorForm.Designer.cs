namespace Perscom
{
    partial class RankGradeEditorForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RankGradeEditorForm));
            fluentTheme1 = new Telerik.WinControls.Themes.FluentTheme();
            bottomPanel = new System.Windows.Forms.Panel();
            CloseButton = new Telerik.WinControls.UI.RadButton();
            radTreeView1 = new Telerik.WinControls.UI.RadTreeView();
            TreeContextMenu = new Telerik.WinControls.UI.RadContextMenu(components);
            addGradeMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            wizardMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            aiMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radMenuSeparatorItem1 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            deleteGradeMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radLabel4 = new Telerik.WinControls.UI.RadLabel();
            applyButton = new Telerik.WinControls.UI.RadButton();
            radLabel15 = new Telerik.WinControls.UI.RadLabel();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            prevTimeInGradeSpinner = new Telerik.WinControls.UI.RadSpinEditor();
            radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            stipendSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel6 = new Telerik.WinControls.UI.RadLabel();
            branchingCheckBox = new Telerik.WinControls.UI.RadCheckBox();
            boardButton = new Telerik.WinControls.UI.RadButton();
            selectionTypeDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            radLabel5 = new Telerik.WinControls.UI.RadLabel();
            lockInTimeSpinner = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel3 = new Telerik.WinControls.UI.RadLabel();
            minTimeInGradeSpinner = new Telerik.WinControls.UI.RadSpinEditor();
            maxTimeInGradeSpinner = new Telerik.WinControls.UI.RadSpinEditor();
            radContextMenuManager1 = new Telerik.WinControls.UI.RadContextMenuManager();
            RanksContextMenu = new Telerik.WinControls.UI.RadContextMenu(components);
            addRankMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radMenuSeparatorItem2 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            deleteRankMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radPanel1 = new Telerik.WinControls.UI.RadPanel();
            radRankSelector4 = new RadRankSelector();
            radRankSelector3 = new RadRankSelector();
            radRankSelector2 = new RadRankSelector();
            radRankSelector1 = new RadRankSelector();
            radPanel2 = new Telerik.WinControls.UI.RadPanel();
            headerPanel = new System.Windows.Forms.Panel();
            headerLabel = new System.Windows.Forms.ShadowLabel();
            DescriptionGroupBox = new Telerik.WinControls.UI.RadGroupBox();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CloseButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)applyButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel15).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)prevTimeInGradeSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radGroupBox1).BeginInit();
            radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)stipendSpinEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)branchingCheckBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)selectionTypeDropDownList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lockInTimeSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)minTimeInGradeSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxTimeInGradeSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radPanel1).BeginInit();
            radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radPanel2).BeginInit();
            radPanel2.SuspendLayout();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DescriptionGroupBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(CloseButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 607);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(767, 50);
            bottomPanel.TabIndex = 15;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // closeButton
            // 
            CloseButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            CloseButton.Location = new System.Drawing.Point(303, 11);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new System.Drawing.Size(150, 28);
            CloseButton.TabIndex = 91;
            CloseButton.Text = "Close";
            CloseButton.ThemeName = "Fluent";
            CloseButton.Click += CloseButton_Click;
            // 
            // unitTreeView
            // 
            radTreeView1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            radTreeView1.Dock = System.Windows.Forms.DockStyle.Top;
            radTreeView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            radTreeView1.ForeColor = System.Drawing.Color.Black;
            radTreeView1.ItemHeight = 28;
            radTreeView1.LineColor = System.Drawing.Color.FromArgb(204, 204, 204);
            radTreeView1.LineStyle = Telerik.WinControls.UI.TreeLineStyle.Solid;
            radTreeView1.Location = new System.Drawing.Point(0, 0);
            radTreeView1.Name = "radTreeView1";
            radTreeView1.RadContextMenu = TreeContextMenu;
            radTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            radTreeView1.Size = new System.Drawing.Size(226, 498);
            radTreeView1.TabIndex = 16;
            radTreeView1.ThemeName = "Fluent";
            radTreeView1.SelectedNodeChanged += radTreeView1_SelectedNodeChanged;
            // 
            // TreeContextMenu
            // 
            TreeContextMenu.Items.AddRange(new Telerik.WinControls.RadItem[] { addGradeMenuItem, wizardMenuItem, aiMenuItem, radMenuSeparatorItem1, deleteGradeMenuItem });
            // 
            // addGradeMenuItem
            // 
            addGradeMenuItem.Name = "addGradeMenuItem";
            addGradeMenuItem.Text = "Add Rank Grade";
            // 
            // wizardMenuItem
            // 
            wizardMenuItem.Name = "wizardMenuItem";
            wizardMenuItem.Text = "Rank Wizard";
            // 
            // aiMenuItem
            // 
            aiMenuItem.Name = "aiMenuItem";
            aiMenuItem.Text = "Ask the AI";
            // 
            // radMenuSeparatorItem1
            // 
            radMenuSeparatorItem1.Name = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.Text = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deleteGradeMenuItem
            // 
            deleteGradeMenuItem.Name = "deleteGradeMenuItem";
            deleteGradeMenuItem.Text = "Delete Rank Grade";
            // 
            // radLabel4
            // 
            radLabel4.Location = new System.Drawing.Point(26, 505);
            radLabel4.Name = "radLabel4";
            radLabel4.Size = new System.Drawing.Size(171, 18);
            radLabel4.TabIndex = 17;
            radLabel4.Text = "Right click to open context menu";
            // 
            // applyButton
            // 
            applyButton.Location = new System.Drawing.Point(245, 564);
            applyButton.Name = "applyButton";
            applyButton.Size = new System.Drawing.Size(508, 32);
            applyButton.TabIndex = 19;
            applyButton.Text = "Apply Changes";
            applyButton.ThemeName = "Fluent";
            applyButton.Click += ApplyButton_Click;
            // 
            // radLabel15
            // 
            radLabel15.Location = new System.Drawing.Point(25, 93);
            radLabel15.Name = "radLabel15";
            radLabel15.Size = new System.Drawing.Size(191, 18);
            radLabel15.TabIndex = 3;
            radLabel15.Text = "Previous Time In Grade Requirement:";
            // 
            // positionNameLabel
            // 
            radLabel1.Location = new System.Drawing.Point(289, 34);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(146, 18);
            radLabel1.TabIndex = 5;
            radLabel1.Text = "Max Allowed Time In Grade:";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(289, 93);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(195, 18);
            radLabel2.TabIndex = 6;
            radLabel2.Text = "Minimum Time In Grade (Retirement):";
            // 
            // prevTimeInGradeSpinner
            // 
            prevTimeInGradeSpinner.Location = new System.Drawing.Point(25, 117);
            prevTimeInGradeSpinner.Name = "prevTimeInGradeSpinner";
            prevTimeInGradeSpinner.NullableValue = new decimal(new int[] { 18, 0, 0, 0 });
            prevTimeInGradeSpinner.Size = new System.Drawing.Size(194, 24);
            prevTimeInGradeSpinner.TabIndex = 14;
            prevTimeInGradeSpinner.ThemeName = "Fluent";
            prevTimeInGradeSpinner.Value = new decimal(new int[] { 18, 0, 0, 0 });
            // 
            // radGroupBox1
            // 
            radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            radGroupBox1.Controls.Add(stipendSpinEditor);
            radGroupBox1.Controls.Add(radLabel6);
            radGroupBox1.Controls.Add(branchingCheckBox);
            radGroupBox1.Controls.Add(boardButton);
            radGroupBox1.Controls.Add(selectionTypeDropDownList);
            radGroupBox1.Controls.Add(radLabel5);
            radGroupBox1.Controls.Add(lockInTimeSpinner);
            radGroupBox1.Controls.Add(radLabel3);
            radGroupBox1.Controls.Add(minTimeInGradeSpinner);
            radGroupBox1.Controls.Add(maxTimeInGradeSpinner);
            radGroupBox1.Controls.Add(prevTimeInGradeSpinner);
            radGroupBox1.Controls.Add(radLabel2);
            radGroupBox1.Controls.Add(radLabel1);
            radGroupBox1.Controls.Add(radLabel15);
            radGroupBox1.HeaderMargin = new System.Windows.Forms.Padding(3);
            radGroupBox1.HeaderText = "Grade Details";
            radGroupBox1.Location = new System.Drawing.Point(245, 126);
            radGroupBox1.Name = "radGroupBox1";
            radGroupBox1.Size = new System.Drawing.Size(508, 265);
            radGroupBox1.TabIndex = 17;
            radGroupBox1.Text = "Grade Details";
            radGroupBox1.ThemeName = "Fluent";
            // 
            // stipendSpinEditor
            // 
            stipendSpinEditor.DecimalPlaces = 2;
            stipendSpinEditor.Location = new System.Drawing.Point(287, 176);
            stipendSpinEditor.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            stipendSpinEditor.Name = "stipendSpinEditor";
            stipendSpinEditor.Size = new System.Drawing.Size(194, 24);
            stipendSpinEditor.TabIndex = 25;
            stipendSpinEditor.ThemeName = "Fluent";
            // 
            // radLabel6
            // 
            radLabel6.Location = new System.Drawing.Point(287, 152);
            radLabel6.Name = "radLabel6";
            radLabel6.Size = new System.Drawing.Size(47, 18);
            radLabel6.TabIndex = 24;
            radLabel6.Text = "Stipend:";
            // 
            // branchingCheckBox
            // 
            branchingCheckBox.Location = new System.Drawing.Point(37, 229);
            branchingCheckBox.Name = "branchingCheckBox";
            branchingCheckBox.Size = new System.Drawing.Size(144, 18);
            branchingCheckBox.TabIndex = 4;
            branchingCheckBox.Text = "Is Split Lane Rank Grade";
            branchingCheckBox.ThemeName = "Fluent";
            // 
            // boardButton
            // 
            boardButton.Location = new System.Drawing.Point(287, 221);
            boardButton.Name = "boardButton";
            boardButton.Size = new System.Drawing.Size(196, 32);
            boardButton.TabIndex = 23;
            boardButton.Text = "Rank Grade Promotion Board";
            boardButton.ThemeName = "Fluent";
            boardButton.Click += BoardButton_Click;
            // 
            // selectionTypeDropDownList
            // 
            selectionTypeDropDownList.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            selectionTypeDropDownList.Location = new System.Drawing.Point(25, 58);
            selectionTypeDropDownList.Name = "selectionTypeDropDownList";
            selectionTypeDropDownList.Size = new System.Drawing.Size(194, 24);
            selectionTypeDropDownList.TabIndex = 22;
            selectionTypeDropDownList.ThemeName = "Fluent";
            selectionTypeDropDownList.SelectedIndexChanged += SelectionTypeDropDownListSelectedIndexChanged;
            // 
            // radLabel5
            // 
            radLabel5.Location = new System.Drawing.Point(25, 34);
            radLabel5.Name = "radLabel5";
            radLabel5.Size = new System.Drawing.Size(177, 18);
            radLabel5.TabIndex = 21;
            radLabel5.Text = "Promotable Status (To this Grade):";
            radLabel5.ThemeName = "Fluent";
            // 
            // lockInTimeSpinner
            // 
            lockInTimeSpinner.Location = new System.Drawing.Point(25, 176);
            lockInTimeSpinner.Name = "lockInTimeSpinner";
            lockInTimeSpinner.Size = new System.Drawing.Size(194, 24);
            lockInTimeSpinner.TabIndex = 18;
            lockInTimeSpinner.ThemeName = "Fluent";
            // 
            // radLabel3
            // 
            radLabel3.Location = new System.Drawing.Point(25, 152);
            radLabel3.Name = "radLabel3";
            radLabel3.Size = new System.Drawing.Size(71, 18);
            radLabel3.TabIndex = 17;
            radLabel3.Text = "Lock In Time:";
            // 
            // minTimeInGradeSpinner
            // 
            minTimeInGradeSpinner.Location = new System.Drawing.Point(289, 117);
            minTimeInGradeSpinner.Name = "minTimeInGradeSpinner";
            minTimeInGradeSpinner.NullableValue = new decimal(new int[] { 12, 0, 0, 0 });
            minTimeInGradeSpinner.Size = new System.Drawing.Size(194, 24);
            minTimeInGradeSpinner.TabIndex = 16;
            minTimeInGradeSpinner.ThemeName = "Fluent";
            minTimeInGradeSpinner.Value = new decimal(new int[] { 12, 0, 0, 0 });
            // 
            // maxTimeInGradeSpinner
            // 
            maxTimeInGradeSpinner.Location = new System.Drawing.Point(289, 58);
            maxTimeInGradeSpinner.Name = "maxTimeInGradeSpinner";
            maxTimeInGradeSpinner.Size = new System.Drawing.Size(194, 24);
            maxTimeInGradeSpinner.TabIndex = 15;
            maxTimeInGradeSpinner.ThemeName = "Fluent";
            // 
            // RanksContextMenu
            // 
            RanksContextMenu.Items.AddRange(new Telerik.WinControls.RadItem[] { addRankMenuItem, radMenuSeparatorItem2, deleteRankMenuItem });
            RanksContextMenu.ThemeName = "Fluent";
            // 
            // addRankMenuItem
            // 
            addRankMenuItem.Name = "addRankMenuItem";
            addRankMenuItem.Text = "Add Rank";
            // 
            // radMenuSeparatorItem2
            // 
            radMenuSeparatorItem2.Name = "radMenuSeparatorItem2";
            radMenuSeparatorItem2.Text = "radMenuSeparatorItem2";
            radMenuSeparatorItem2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deleteRankMenuItem
            // 
            deleteRankMenuItem.Name = "deleteRankMenuItem";
            deleteRankMenuItem.Text = "Delete Rank";
            // 
            // radPanel1
            // 
            radPanel1.Controls.Add(radRankSelector4);
            radPanel1.Controls.Add(radRankSelector3);
            radPanel1.Controls.Add(radRankSelector2);
            radPanel1.Controls.Add(radRankSelector1);
            radPanel1.Location = new System.Drawing.Point(245, 401);
            radPanel1.Name = "radPanel1";
            radPanel1.Size = new System.Drawing.Size(508, 154);
            radPanel1.TabIndex = 20;
            radPanel1.ThemeName = "Fluent";
            // 
            // radRankSelector4
            // 
            radRankSelector4.BackColor = System.Drawing.Color.White;
            radRankSelector4.ImagePadding = 4;
            radRankSelector4.Location = new System.Drawing.Point(378, 9);
            radRankSelector4.Name = "radRankSelector4";
            radRankSelector4.OutlineColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            radRankSelector4.OutlineWidth = 0;
            radRankSelector4.Rank = null;
            radRankSelector4.ShadowColor = System.Drawing.Color.FromArgb(120, 0, 0, 0);
            radRankSelector4.ShadowRadius = 1;
            radRankSelector4.Size = new System.Drawing.Size(124, 140);
            radRankSelector4.TabIndex = 7;
            // 
            // radRankSelector3
            // 
            radRankSelector3.BackColor = System.Drawing.Color.White;
            radRankSelector3.ImagePadding = 4;
            radRankSelector3.Location = new System.Drawing.Point(254, 9);
            radRankSelector3.Name = "radRankSelector3";
            radRankSelector3.OutlineColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            radRankSelector3.OutlineWidth = 0;
            radRankSelector3.Rank = null;
            radRankSelector3.ShadowColor = System.Drawing.Color.FromArgb(120, 0, 0, 0);
            radRankSelector3.ShadowRadius = 1;
            radRankSelector3.Size = new System.Drawing.Size(124, 140);
            radRankSelector3.TabIndex = 6;
            // 
            // radRankSelector2
            // 
            radRankSelector2.BackColor = System.Drawing.Color.White;
            radRankSelector2.ImagePadding = 4;
            radRankSelector2.Location = new System.Drawing.Point(130, 9);
            radRankSelector2.Name = "radRankSelector2";
            radRankSelector2.OutlineColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            radRankSelector2.OutlineWidth = 0;
            radRankSelector2.Rank = null;
            radRankSelector2.ShadowColor = System.Drawing.Color.FromArgb(120, 0, 0, 0);
            radRankSelector2.ShadowRadius = 1;
            radRankSelector2.Size = new System.Drawing.Size(124, 140);
            radRankSelector2.TabIndex = 5;
            // 
            // radRankSelector1
            // 
            radRankSelector1.BackColor = System.Drawing.Color.White;
            radRankSelector1.ImagePadding = 4;
            radRankSelector1.Location = new System.Drawing.Point(6, 9);
            radRankSelector1.Name = "radRankSelector1";
            radRankSelector1.OutlineColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            radRankSelector1.OutlineWidth = 0;
            radRankSelector1.Rank = null;
            radRankSelector1.ShadowColor = System.Drawing.Color.FromArgb(120, 0, 0, 0);
            radRankSelector1.ShadowRadius = 1;
            radRankSelector1.Size = new System.Drawing.Size(124, 140);
            radRankSelector1.TabIndex = 4;
            // 
            // radPanel2
            // 
            radPanel2.Controls.Add(radLabel4);
            radPanel2.Controls.Add(radTreeView1);
            radPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            radPanel2.Location = new System.Drawing.Point(0, 75);
            radPanel2.Name = "radPanel2";
            radPanel2.Size = new System.Drawing.Size(226, 532);
            radPanel2.TabIndex = 21;
            radPanel2.ThemeName = "Fluent";
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(headerLabel);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(767, 75);
            headerPanel.TabIndex = 14;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // headerLabel
            // 
            headerLabel.BackColor = System.Drawing.Color.Transparent;
            headerLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            headerLabel.ForeColor = System.Drawing.SystemColors.Control;
            headerLabel.Location = new System.Drawing.Point(26, 22);
            headerLabel.Name = "label6";
            headerLabel.ShadowDirection = 60;
            headerLabel.ShadowOpacity = 180;
            headerLabel.ShadowSoftness = 3F;
            headerLabel.Size = new System.Drawing.Size(717, 37);
            headerLabel.TabIndex = 0;
            headerLabel.Text = "Rank And Grade Editor for Faction Name";
            // 
            // descriptionGroupBox
            // 
            DescriptionGroupBox.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            DescriptionGroupBox.HeaderAlignment = Telerik.WinControls.UI.HeaderAlignment.Center;
            DescriptionGroupBox.HeaderMargin = new System.Windows.Forms.Padding(3, 8, 3, 1);
            DescriptionGroupBox.HeaderText = "Enlisted Grade 8";
            DescriptionGroupBox.HeaderTextAlignment = System.Drawing.ContentAlignment.TopCenter;
            DescriptionGroupBox.Location = new System.Drawing.Point(245, 81);
            DescriptionGroupBox.Name = "DescriptionGroupBox";
            DescriptionGroupBox.Size = new System.Drawing.Size(508, 36);
            DescriptionGroupBox.TabIndex = 22;
            DescriptionGroupBox.Text = "Enlisted Grade 8";
            DescriptionGroupBox.ThemeName = "Fluent";
            // 
            // RankGradeEditorForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(767, 657);
            Controls.Add(DescriptionGroupBox);
            Controls.Add(radPanel2);
            Controls.Add(applyButton);
            Controls.Add(radPanel1);
            Controls.Add(radGroupBox1);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "RankGradeEditorForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Rank and Grade Editor";
            ThemeName = "Fluent";
            FormClosing += RankGradeEditorForm_FormClosing;
            Load += RankGradeEditorForm_Load;
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)CloseButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel4).EndInit();
            ((System.ComponentModel.ISupportInitialize)applyButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel15).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)prevTimeInGradeSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)radGroupBox1).EndInit();
            radGroupBox1.ResumeLayout(false);
            radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)stipendSpinEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel6).EndInit();
            ((System.ComponentModel.ISupportInitialize)branchingCheckBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)selectionTypeDropDownList).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).EndInit();
            ((System.ComponentModel.ISupportInitialize)lockInTimeSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).EndInit();
            ((System.ComponentModel.ISupportInitialize)minTimeInGradeSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxTimeInGradeSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)radPanel1).EndInit();
            radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radPanel2).EndInit();
            radPanel2.ResumeLayout(false);
            radPanel2.PerformLayout();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DescriptionGroupBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.Themes.FluentTheme fluentTheme1;
        private System.Windows.Forms.Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton CloseButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel headerLabel;
        private Telerik.WinControls.UI.RadTreeView radTreeView1;
        private Telerik.WinControls.UI.RadButton applyButton;
        private Telerik.WinControls.UI.RadLabel radLabel15;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadSpinEditor prevTimeInGradeSpinner;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadSpinEditor lockInTimeSpinner;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadSpinEditor minTimeInGradeSpinner;
        private Telerik.WinControls.UI.RadSpinEditor maxTimeInGradeSpinner;
        private Telerik.WinControls.UI.RadContextMenu TreeContextMenu;
        private Telerik.WinControls.UI.RadContextMenuManager radContextMenuManager1;
        private Telerik.WinControls.UI.RadContextMenu RanksContextMenu;
        private Telerik.WinControls.UI.RadMenuItem addGradeMenuItem;
        private Telerik.WinControls.UI.RadMenuItem wizardMenuItem;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem1;
        private Telerik.WinControls.UI.RadMenuItem deleteGradeMenuItem;
        private Telerik.WinControls.UI.RadMenuItem addRankMenuItem;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem2;
        private Telerik.WinControls.UI.RadMenuItem deleteRankMenuItem;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadDropDownList selectionTypeDropDownList;
        private Telerik.WinControls.UI.RadGroupBox DescriptionGroupBox;
        private Telerik.WinControls.UI.RadButton boardButton;
        private Telerik.WinControls.UI.RadCheckBox branchingCheckBox;
        private Telerik.WinControls.UI.RadMenuItem aiMenuItem;
        private RadRankSelector radRankSelector4;
        private RadRankSelector radRankSelector3;
        private RadRankSelector radRankSelector2;
        private RadRankSelector radRankSelector1;
        private Telerik.WinControls.UI.RadSpinEditor stipendSpinEditor;
        private Telerik.WinControls.UI.RadLabel radLabel6;
    }
}
