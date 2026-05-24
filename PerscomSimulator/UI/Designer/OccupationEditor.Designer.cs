namespace Perscom
{
    partial class OccupationEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OccupationEditor));
            radPanel2 = new Telerik.WinControls.UI.RadPanel();
            radLabel4 = new Telerik.WinControls.UI.RadLabel();
            radTreeView1 = new Telerik.WinControls.UI.RadTreeView();
            TreeContextMenu = new Telerik.WinControls.UI.RadContextMenu(components);
            addMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radMenuSeparatorItem1 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            deleteMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            bottomPanel = new System.Windows.Forms.Panel();
            CloseButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new System.Windows.Forms.Panel();
            headerLabel = new System.Windows.Forms.ShadowLabel();
            descriptionGroupBox = new Telerik.WinControls.UI.RadGroupBox();
            radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            codeTextBox = new Telerik.WinControls.UI.RadTextBox();
            nameTextBox = new Telerik.WinControls.UI.RadTextBox();
            stipendSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel6 = new Telerik.WinControls.UI.RadLabel();
            boardButton = new Telerik.WinControls.UI.RadButton();
            radLabel5 = new Telerik.WinControls.UI.RadLabel();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            applyButton = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)radPanel2).BeginInit();
            radPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radLabel4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).BeginInit();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CloseButton).BeginInit();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)descriptionGroupBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radGroupBox1).BeginInit();
            radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)codeTextBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nameTextBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)stipendSpinEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)applyButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // radPanel2
            // 
            radPanel2.Controls.Add(radLabel4);
            radPanel2.Controls.Add(radTreeView1);
            radPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            radPanel2.Location = new System.Drawing.Point(0, 75);
            radPanel2.Name = "radPanel2";
            radPanel2.Size = new System.Drawing.Size(226, 486);
            radPanel2.TabIndex = 24;
            radPanel2.ThemeName = "Fluent";
            // 
            // radLabel4
            // 
            radLabel4.Location = new System.Drawing.Point(26, 505);
            radLabel4.Name = "radLabel4";
            radLabel4.Size = new System.Drawing.Size(171, 18);
            radLabel4.TabIndex = 17;
            radLabel4.Text = "Right click to open context menu";
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
            TreeContextMenu.Items.AddRange(new Telerik.WinControls.RadItem[] { addMenuItem, radMenuSeparatorItem1, deleteMenuItem });
            // 
            // addMenuItem
            // 
            addMenuItem.Name = "addMenuItem";
            addMenuItem.Text = "Add Occupation";
            addMenuItem.UseCompatibleTextRendering = false;
            // 
            // radMenuSeparatorItem1
            // 
            radMenuSeparatorItem1.Name = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.Text = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            radMenuSeparatorItem1.UseCompatibleTextRendering = false;
            // 
            // deleteMenuItem
            // 
            deleteMenuItem.Name = "deleteMenuItem";
            deleteMenuItem.Text = "Delete Occupation";
            deleteMenuItem.UseCompatibleTextRendering = false;
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(CloseButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 561);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(767, 50);
            bottomPanel.TabIndex = 23;
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
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(headerLabel);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(767, 75);
            headerPanel.TabIndex = 22;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // headerLabel
            // 
            headerLabel.BackColor = System.Drawing.Color.Transparent;
            headerLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            headerLabel.ForeColor = System.Drawing.SystemColors.Control;
            headerLabel.Location = new System.Drawing.Point(26, 22);
            headerLabel.Name = "headerLabel";
            headerLabel.ShadowDirection = 60;
            headerLabel.ShadowOpacity = 180;
            headerLabel.ShadowSoftness = 3F;
            headerLabel.Size = new System.Drawing.Size(717, 37);
            headerLabel.TabIndex = 0;
            headerLabel.Text = "Occupation Editor";
            // 
            // descriptionGroupBox
            // 
            descriptionGroupBox.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            descriptionGroupBox.HeaderAlignment = Telerik.WinControls.UI.HeaderAlignment.Center;
            descriptionGroupBox.HeaderMargin = new System.Windows.Forms.Padding(3, 8, 3, 1);
            descriptionGroupBox.HeaderText = "Enlisted Occupation";
            descriptionGroupBox.HeaderTextAlignment = System.Drawing.ContentAlignment.TopCenter;
            descriptionGroupBox.Location = new System.Drawing.Point(245, 81);
            descriptionGroupBox.Name = "descriptionGroupBox";
            descriptionGroupBox.Size = new System.Drawing.Size(508, 36);
            descriptionGroupBox.TabIndex = 25;
            descriptionGroupBox.Text = "Enlisted Occupation";
            descriptionGroupBox.ThemeName = "Fluent";
            // 
            // radGroupBox1
            // 
            radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            radGroupBox1.Controls.Add(codeTextBox);
            radGroupBox1.Controls.Add(nameTextBox);
            radGroupBox1.Controls.Add(stipendSpinEditor);
            radGroupBox1.Controls.Add(radLabel6);
            radGroupBox1.Controls.Add(boardButton);
            radGroupBox1.Controls.Add(radLabel5);
            radGroupBox1.Controls.Add(radLabel1);
            radGroupBox1.HeaderMargin = new System.Windows.Forms.Padding(3);
            radGroupBox1.HeaderText = "Grade Details";
            radGroupBox1.Location = new System.Drawing.Point(247, 126);
            radGroupBox1.Name = "radGroupBox1";
            radGroupBox1.Size = new System.Drawing.Size(508, 385);
            radGroupBox1.TabIndex = 26;
            radGroupBox1.Text = "Grade Details";
            radGroupBox1.ThemeName = "Fluent";
            // 
            // codeTextBox
            // 
            codeTextBox.Location = new System.Drawing.Point(274, 58);
            codeTextBox.Name = "codeTextBox";
            codeTextBox.Size = new System.Drawing.Size(210, 24);
            codeTextBox.TabIndex = 27;
            codeTextBox.Text = "Please enter an MOS code";
            codeTextBox.ThemeName = "Fluent";
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new System.Drawing.Point(24, 58);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new System.Drawing.Size(210, 24);
            nameTextBox.TabIndex = 26;
            nameTextBox.Text = "Please enter a name";
            nameTextBox.ThemeName = "Fluent";
            // 
            // stipendSpinEditor
            // 
            stipendSpinEditor.DecimalPlaces = 2;
            stipendSpinEditor.Location = new System.Drawing.Point(274, 154);
            stipendSpinEditor.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            stipendSpinEditor.Name = "stipendSpinEditor";
            stipendSpinEditor.Size = new System.Drawing.Size(194, 24);
            stipendSpinEditor.TabIndex = 25;
            stipendSpinEditor.ThemeName = "Fluent";
            // 
            // radLabel6
            // 
            radLabel6.Location = new System.Drawing.Point(274, 130);
            radLabel6.Name = "radLabel6";
            radLabel6.Size = new System.Drawing.Size(81, 18);
            radLabel6.TabIndex = 24;
            radLabel6.Text = "Bonus Stipend:";
            // 
            // boardButton
            // 
            boardButton.Location = new System.Drawing.Point(278, 199);
            boardButton.Name = "boardButton";
            boardButton.Size = new System.Drawing.Size(196, 32);
            boardButton.TabIndex = 23;
            boardButton.Text = "Promotion Board Overrides";
            boardButton.ThemeName = "Fluent";
            boardButton.Click += BoardButton_Click;
            // 
            // radLabel5
            // 
            radLabel5.Location = new System.Drawing.Point(24, 34);
            radLabel5.Name = "radLabel5";
            radLabel5.Size = new System.Drawing.Size(147, 18);
            radLabel5.TabIndex = 21;
            radLabel5.Text = "Occupation Specialty Name:";
            radLabel5.ThemeName = "Fluent";
            // 
            // positionNameLabel
            // 
            radLabel1.Location = new System.Drawing.Point(274, 34);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(63, 18);
            radLabel1.TabIndex = 5;
            radLabel1.Text = "MOS Code:";
            // 
            // applyButton
            // 
            applyButton.Location = new System.Drawing.Point(247, 521);
            applyButton.Name = "applyButton";
            applyButton.Size = new System.Drawing.Size(508, 32);
            applyButton.TabIndex = 27;
            applyButton.Text = "Apply Changes";
            applyButton.ThemeName = "Fluent";
            applyButton.Click += ApplyButton_Click;
            // 
            // OccupationEditor
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(767, 611);
            Controls.Add(applyButton);
            Controls.Add(radGroupBox1);
            Controls.Add(descriptionGroupBox);
            Controls.Add(radPanel2);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "OccupationEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "OccupationEditor";
            ThemeName = "Fluent";
            FormClosing += OccupationEditor_FormClosing;
            Load += OccupationEditor_Load;
            ((System.ComponentModel.ISupportInitialize)radPanel2).EndInit();
            radPanel2.ResumeLayout(false);
            radPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)radLabel4).EndInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).EndInit();
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)CloseButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)descriptionGroupBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)radGroupBox1).EndInit();
            radGroupBox1.ResumeLayout(false);
            radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)codeTextBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)nameTextBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)stipendSpinEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel6).EndInit();
            ((System.ComponentModel.ISupportInitialize)boardButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel5).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)applyButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel2;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadTreeView radTreeView1;
        private System.Windows.Forms.Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton CloseButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel headerLabel;
        private Telerik.WinControls.UI.RadGroupBox descriptionGroupBox;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadSpinEditor stipendSpinEditor;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadButton boardButton;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox codeTextBox;
        private Telerik.WinControls.UI.RadTextBox nameTextBox;
        private Telerik.WinControls.UI.RadButtonTextBox radButtonTextBox1;
        private Telerik.WinControls.UI.RadButton applyButton;
        private Telerik.WinControls.UI.RadContextMenu TreeContextMenu;
        private Telerik.WinControls.UI.RadMenuItem addMenuItem;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem1;
        private Telerik.WinControls.UI.RadMenuItem deleteMenuItem;
    }
}
