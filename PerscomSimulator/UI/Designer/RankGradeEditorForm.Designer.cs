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
            radLabel4 = new Telerik.WinControls.UI.RadLabel();
            TreeContextMenu = new Telerik.WinControls.UI.RadContextMenu(components);
            addGradeMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            wizardMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radMenuSeparatorItem1 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            deleteGradeMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            applyButton = new Telerik.WinControls.UI.RadButton();
            radLabel15 = new Telerik.WinControls.UI.RadLabel();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            PrevTIGReq = new Telerik.WinControls.UI.RadSpinEditor();
            radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            branchingCheckBox = new Telerik.WinControls.UI.RadCheckBox();
            boardButton = new Telerik.WinControls.UI.RadButton();
            SelectionDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            radLabel5 = new Telerik.WinControls.UI.RadLabel();
            LockInTime = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel3 = new Telerik.WinControls.UI.RadLabel();
            MinTIG = new Telerik.WinControls.UI.RadSpinEditor();
            MaxTIG = new Telerik.WinControls.UI.RadSpinEditor();
            radContextMenuManager1 = new Telerik.WinControls.UI.RadContextMenuManager();
            RanksContextMenu = new Telerik.WinControls.UI.RadContextMenu(components);
            addRankMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radMenuSeparatorItem2 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            deleteRankMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radPanel1 = new Telerik.WinControls.UI.RadPanel();
            radRankSelector3 = new RadRankSelector();
            radRankSelector2 = new RadRankSelector();
            radRankSelector1 = new RadRankSelector();
            radPanel2 = new Telerik.WinControls.UI.RadPanel();
            headerPanel = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.ShadowLabel();
            DescriptionGroupBox = new Telerik.WinControls.UI.RadGroupBox();
            radRankSelector4 = new RadRankSelector();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CloseButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).BeginInit();
            radTreeView1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radLabel4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)applyButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel15).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PrevTIGReq).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radGroupBox1).BeginInit();
            radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)branchingCheckBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SelectionDropDownList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LockInTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MinTIG).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxTIG).BeginInit();
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
            bottomPanel.Controls.Add(CloseButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 587);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(757, 50);
            bottomPanel.TabIndex = 15;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // CloseButton
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
            // radTreeView1
            // 
            radTreeView1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            radTreeView1.Controls.Add(radLabel4);
            radTreeView1.Dock = System.Windows.Forms.DockStyle.Left;
            radTreeView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            radTreeView1.ForeColor = System.Drawing.Color.Black;
            radTreeView1.ItemHeight = 28;
            radTreeView1.LineColor = System.Drawing.Color.FromArgb(204, 204, 204);
            radTreeView1.LineStyle = Telerik.WinControls.UI.TreeLineStyle.Solid;
            radTreeView1.Location = new System.Drawing.Point(0, 0);
            radTreeView1.Name = "radTreeView1";
            radTreeView1.RadContextMenu = TreeContextMenu;
            radTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            radTreeView1.Size = new System.Drawing.Size(220, 512);
            radTreeView1.TabIndex = 16;
            radTreeView1.ThemeName = "Fluent";
            radTreeView1.SelectedNodeChanged += radTreeView1_SelectedNodeChanged;
            // 
            // radLabel4
            // 
            radLabel4.Location = new System.Drawing.Point(22, 476);
            radLabel4.Name = "radLabel4";
            radLabel4.Size = new System.Drawing.Size(171, 18);
            radLabel4.TabIndex = 17;
            radLabel4.Text = "Right click to open context menu";
            // 
            // TreeContextMenu
            // 
            TreeContextMenu.Items.AddRange(new Telerik.WinControls.RadItem[] { addGradeMenuItem, wizardMenuItem, radMenuSeparatorItem1, deleteGradeMenuItem });
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
            // applyButton
            // 
            applyButton.Location = new System.Drawing.Point(245, 544);
            applyButton.Name = "applyButton";
            applyButton.Size = new System.Drawing.Size(498, 32);
            applyButton.TabIndex = 19;
            applyButton.Text = "Apply Changes";
            applyButton.ThemeName = "Fluent";
            // 
            // radLabel15
            // 
            radLabel15.Location = new System.Drawing.Point(16, 93);
            radLabel15.Name = "radLabel15";
            radLabel15.Size = new System.Drawing.Size(191, 18);
            radLabel15.TabIndex = 3;
            radLabel15.Text = "Previous Time In Grade Requirement:";
            // 
            // radLabel1
            // 
            radLabel1.Location = new System.Drawing.Point(280, 34);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(146, 18);
            radLabel1.TabIndex = 5;
            radLabel1.Text = "Max Allowed Time In Grade:";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(280, 93);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(195, 18);
            radLabel2.TabIndex = 6;
            radLabel2.Text = "Minimum Time In Grade (Retirement):";
            // 
            // PrevTIGReq
            // 
            PrevTIGReq.Location = new System.Drawing.Point(16, 117);
            PrevTIGReq.Name = "PrevTIGReq";
            PrevTIGReq.NullableValue = new decimal(new int[] { 18, 0, 0, 0 });
            PrevTIGReq.Size = new System.Drawing.Size(194, 24);
            PrevTIGReq.TabIndex = 14;
            PrevTIGReq.ThemeName = "Fluent";
            PrevTIGReq.Value = new decimal(new int[] { 18, 0, 0, 0 });
            // 
            // radGroupBox1
            // 
            radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            radGroupBox1.Controls.Add(branchingCheckBox);
            radGroupBox1.Controls.Add(boardButton);
            radGroupBox1.Controls.Add(SelectionDropDownList);
            radGroupBox1.Controls.Add(radLabel5);
            radGroupBox1.Controls.Add(LockInTime);
            radGroupBox1.Controls.Add(radLabel3);
            radGroupBox1.Controls.Add(MinTIG);
            radGroupBox1.Controls.Add(MaxTIG);
            radGroupBox1.Controls.Add(PrevTIGReq);
            radGroupBox1.Controls.Add(radLabel2);
            radGroupBox1.Controls.Add(radLabel1);
            radGroupBox1.Controls.Add(radLabel15);
            radGroupBox1.HeaderMargin = new System.Windows.Forms.Padding(3);
            radGroupBox1.HeaderText = "Grade Details";
            radGroupBox1.Location = new System.Drawing.Point(245, 127);
            radGroupBox1.Name = "radGroupBox1";
            radGroupBox1.Size = new System.Drawing.Size(498, 245);
            radGroupBox1.TabIndex = 17;
            radGroupBox1.Text = "Grade Details";
            radGroupBox1.ThemeName = "Fluent";
            // 
            // branchingCheckBox
            // 
            branchingCheckBox.Location = new System.Drawing.Point(177, 215);
            branchingCheckBox.Name = "branchingCheckBox";
            branchingCheckBox.Size = new System.Drawing.Size(145, 18);
            branchingCheckBox.TabIndex = 4;
            branchingCheckBox.Text = "Is Branching Rank Grade";
            branchingCheckBox.ThemeName = "Fluent";
            // 
            // boardButton
            // 
            boardButton.Location = new System.Drawing.Point(278, 168);
            boardButton.Name = "boardButton";
            boardButton.Size = new System.Drawing.Size(196, 32);
            boardButton.TabIndex = 23;
            boardButton.Text = "Generic Promotion Board";
            boardButton.ThemeName = "Fluent";
            // 
            // SelectionDropDownList
            // 
            SelectionDropDownList.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            SelectionDropDownList.Location = new System.Drawing.Point(16, 58);
            SelectionDropDownList.Name = "SelectionDropDownList";
            SelectionDropDownList.Size = new System.Drawing.Size(194, 24);
            SelectionDropDownList.TabIndex = 22;
            SelectionDropDownList.ThemeName = "Fluent";
            SelectionDropDownList.SelectedIndexChanged += SelectionDropDownList_SelectedIndexChanged;
            // 
            // radLabel5
            // 
            radLabel5.Location = new System.Drawing.Point(16, 34);
            radLabel5.Name = "radLabel5";
            radLabel5.Size = new System.Drawing.Size(177, 18);
            radLabel5.TabIndex = 21;
            radLabel5.Text = "Promotable Status (To this Grade):";
            radLabel5.ThemeName = "Fluent";
            // 
            // LockInTime
            // 
            LockInTime.Location = new System.Drawing.Point(16, 176);
            LockInTime.Name = "LockInTime";
            LockInTime.Size = new System.Drawing.Size(194, 24);
            LockInTime.TabIndex = 18;
            LockInTime.ThemeName = "Fluent";
            // 
            // radLabel3
            // 
            radLabel3.Location = new System.Drawing.Point(16, 152);
            radLabel3.Name = "radLabel3";
            radLabel3.Size = new System.Drawing.Size(71, 18);
            radLabel3.TabIndex = 17;
            radLabel3.Text = "Lock In Time:";
            // 
            // MinTIG
            // 
            MinTIG.Location = new System.Drawing.Point(280, 117);
            MinTIG.Name = "MinTIG";
            MinTIG.NullableValue = new decimal(new int[] { 12, 0, 0, 0 });
            MinTIG.Size = new System.Drawing.Size(194, 24);
            MinTIG.TabIndex = 16;
            MinTIG.ThemeName = "Fluent";
            MinTIG.Value = new decimal(new int[] { 12, 0, 0, 0 });
            // 
            // MaxTIG
            // 
            MaxTIG.Location = new System.Drawing.Point(280, 58);
            MaxTIG.Name = "MaxTIG";
            MaxTIG.Size = new System.Drawing.Size(194, 24);
            MaxTIG.TabIndex = 15;
            MaxTIG.ThemeName = "Fluent";
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
            radPanel1.Location = new System.Drawing.Point(245, 381);
            radPanel1.Name = "radPanel1";
            radPanel1.Size = new System.Drawing.Size(498, 154);
            radPanel1.TabIndex = 20;
            radPanel1.ThemeName = "Fluent";
            // 
            // radRankSelector3
            // 
            radRankSelector3.BackColor = System.Drawing.Color.White;
            radRankSelector3.Location = new System.Drawing.Point(254, 14);
            radRankSelector3.Name = "radRankSelector3";
            radRankSelector3.Rank = null;
            radRankSelector3.RankTitle = "Rank 3";
            radRankSelector3.Size = new System.Drawing.Size(108, 128);
            radRankSelector3.TabIndex = 2;
            // 
            // radRankSelector2
            // 
            radRankSelector2.BackColor = System.Drawing.Color.White;
            radRankSelector2.Location = new System.Drawing.Point(137, 14);
            radRankSelector2.Name = "radRankSelector2";
            radRankSelector2.Rank = null;
            radRankSelector2.RankTitle = "Rank 2";
            radRankSelector2.Size = new System.Drawing.Size(108, 128);
            radRankSelector2.TabIndex = 1;
            // 
            // radRankSelector1
            // 
            radRankSelector1.BackColor = System.Drawing.Color.White;
            radRankSelector1.Location = new System.Drawing.Point(20, 14);
            radRankSelector1.Name = "radRankSelector1";
            radRankSelector1.Rank = null;
            radRankSelector1.Size = new System.Drawing.Size(108, 128);
            radRankSelector1.TabIndex = 0;
            // 
            // radPanel2
            // 
            radPanel2.Controls.Add(radTreeView1);
            radPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            radPanel2.Location = new System.Drawing.Point(0, 75);
            radPanel2.Name = "radPanel2";
            radPanel2.Size = new System.Drawing.Size(226, 512);
            radPanel2.TabIndex = 21;
            radPanel2.ThemeName = "Fluent";
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(label6);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(757, 75);
            headerPanel.TabIndex = 14;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // label6
            // 
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.SystemColors.Control;
            label6.Location = new System.Drawing.Point(26, 22);
            label6.Name = "label6";
            label6.ShadowDirection = 90;
            label6.ShadowOpacity = 225;
            label6.ShadowSoftness = 3F;
            label6.Size = new System.Drawing.Size(717, 37);
            label6.TabIndex = 0;
            label6.Text = "Rank And Grade Editor for Faction Name";
            // 
            // DescriptionGroupBox
            // 
            DescriptionGroupBox.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            DescriptionGroupBox.HeaderAlignment = Telerik.WinControls.UI.HeaderAlignment.Center;
            DescriptionGroupBox.HeaderMargin = new System.Windows.Forms.Padding(3, 8, 3, 1);
            DescriptionGroupBox.HeaderText = "Enlisted Grade 8";
            DescriptionGroupBox.HeaderTextAlignment = System.Drawing.ContentAlignment.TopCenter;
            DescriptionGroupBox.Location = new System.Drawing.Point(245, 81);
            DescriptionGroupBox.Name = "DescriptionGroupBox";
            DescriptionGroupBox.Size = new System.Drawing.Size(498, 38);
            DescriptionGroupBox.TabIndex = 22;
            DescriptionGroupBox.Text = "Enlisted Grade 8";
            DescriptionGroupBox.ThemeName = "Fluent";
            // 
            // radRankSelector4
            // 
            radRankSelector4.BackColor = System.Drawing.Color.White;
            radRankSelector4.Location = new System.Drawing.Point(371, 14);
            radRankSelector4.Name = "radRankSelector4";
            radRankSelector4.Rank = null;
            radRankSelector4.RankTitle = "Rank 4";
            radRankSelector4.Size = new System.Drawing.Size(108, 128);
            radRankSelector4.TabIndex = 3;
            // 
            // RankGradeEditorForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(757, 637);
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
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)CloseButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).EndInit();
            radTreeView1.ResumeLayout(false);
            radTreeView1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)radLabel4).EndInit();
            ((System.ComponentModel.ISupportInitialize)applyButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel15).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)PrevTIGReq).EndInit();
            ((System.ComponentModel.ISupportInitialize)radGroupBox1).EndInit();
            radGroupBox1.ResumeLayout(false);
            radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)branchingCheckBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)SelectionDropDownList).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).EndInit();
            ((System.ComponentModel.ISupportInitialize)LockInTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).EndInit();
            ((System.ComponentModel.ISupportInitialize)MinTIG).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxTIG).EndInit();
            ((System.ComponentModel.ISupportInitialize)radPanel1).EndInit();
            radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radPanel2).EndInit();
            radPanel2.ResumeLayout(false);
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
        private System.Windows.Forms.ShadowLabel label6;
        private Telerik.WinControls.UI.RadTreeView radTreeView1;
        private Telerik.WinControls.UI.RadButton applyButton;
        private Telerik.WinControls.UI.RadLabel radLabel15;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadSpinEditor PrevTIGReq;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadSpinEditor LockInTime;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadSpinEditor MinTIG;
        private Telerik.WinControls.UI.RadSpinEditor MaxTIG;
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
        private Telerik.WinControls.UI.RadDropDownList SelectionDropDownList;
        private Telerik.WinControls.UI.RadGroupBox DescriptionGroupBox;
        private Telerik.WinControls.UI.RadButton boardButton;
        private RadRankSelector radRankSelector3;
        private RadRankSelector radRankSelector2;
        private RadRankSelector radRankSelector1;
        private Telerik.WinControls.UI.RadCheckBox branchingCheckBox;
        private RadRankSelector radRankSelector4;
    }
}
