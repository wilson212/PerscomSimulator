namespace Perscom
{
    partial class UnitTypeManagerForm
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
            this.sidePanel = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.applyButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.shortNameBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.promotionPoolSelect = new System.Windows.Forms.ComboBox();
            this.referencesLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.unitNameBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.echelonTypeSelect = new System.Windows.Forms.ComboBox();
            this.templateNameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.billetsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addBilletMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeBilletMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateBilletMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyFromMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.subUnitsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.adjustUnitCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeSubUnitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.finalizeButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.sidePanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.billetsContextMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.subUnitsContextMenu.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidePanel
            // 
            this.sidePanel.Controls.Add(this.treeView1);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 75);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Padding = new System.Windows.Forms.Padding(5, 10, 0, 0);
            this.sidePanel.Size = new System.Drawing.Size(240, 512);
            this.sidePanel.TabIndex = 8;
            this.sidePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sidePanel_Paint);
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Location = new System.Drawing.Point(10, 7);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(214, 391);
            this.treeView1.TabIndex = 0;
            this.treeView1.TabStop = false;
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(114, 22);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 12;
            this.deleteButton.TabStop = false;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(31, 22);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 23);
            this.newButton.TabIndex = 11;
            this.newButton.TabStop = false;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.applyButton);
            this.mainPanel.Controls.Add(this.groupBox3);
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(240, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(844, 512);
            this.mainPanel.TabIndex = 9;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(197, 471);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(431, 30);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.shortNameBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.promotionPoolSelect);
            this.groupBox3.Controls.Add(this.referencesLabel);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.unitNameBox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.echelonTypeSelect);
            this.groupBox3.Controls.Add(this.templateNameBox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(16, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(340, 259);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Details";
            // 
            // shortNameBox
            // 
            this.shortNameBox.Location = new System.Drawing.Point(129, 170);
            this.shortNameBox.Name = "shortNameBox";
            this.shortNameBox.Size = new System.Drawing.Size(185, 20);
            this.shortNameBox.TabIndex = 35;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Short Name Format: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 207);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Promotion Pool:";
            // 
            // promotionPoolSelect
            // 
            this.promotionPoolSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.promotionPoolSelect.FormattingEnabled = true;
            this.promotionPoolSelect.Location = new System.Drawing.Point(129, 204);
            this.promotionPoolSelect.Name = "promotionPoolSelect";
            this.promotionPoolSelect.Size = new System.Drawing.Size(189, 21);
            this.promotionPoolSelect.TabIndex = 33;
            // 
            // referencesLabel
            // 
            this.referencesLabel.AutoSize = true;
            this.referencesLabel.Location = new System.Drawing.Point(221, 29);
            this.referencesLabel.Name = "referencesLabel";
            this.referencesLabel.Size = new System.Drawing.Size(13, 13);
            this.referencesLabel.TabIndex = 32;
            this.referencesLabel.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(106, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Template References:  ";
            // 
            // unitNameBox
            // 
            this.unitNameBox.Location = new System.Drawing.Point(129, 137);
            this.unitNameBox.Name = "unitNameBox";
            this.unitNameBox.Size = new System.Drawing.Size(185, 20);
            this.unitNameBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Unit Name Format: ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Echelon Level:";
            // 
            // echelonTypeSelect
            // 
            this.echelonTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.echelonTypeSelect.FormattingEnabled = true;
            this.echelonTypeSelect.Location = new System.Drawing.Point(129, 66);
            this.echelonTypeSelect.Name = "echelonTypeSelect";
            this.echelonTypeSelect.Size = new System.Drawing.Size(189, 21);
            this.echelonTypeSelect.TabIndex = 1;
            this.echelonTypeSelect.SelectedIndexChanged += new System.EventHandler(this.echelonTypeSelect_SelectedIndexChanged);
            // 
            // templateNameBox
            // 
            this.templateNameBox.Location = new System.Drawing.Point(129, 104);
            this.templateNameBox.Name = "templateNameBox";
            this.templateNameBox.Size = new System.Drawing.Size(185, 20);
            this.templateNameBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Template Name: ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView2);
            this.groupBox2.Location = new System.Drawing.Point(369, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(463, 430);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Billets";
            // 
            // listView2
            // 
            this.listView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
            this.listView2.ContextMenuStrip = this.billetsContextMenu;
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.Location = new System.Drawing.Point(3, 16);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(457, 411);
            this.listView2.TabIndex = 1;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.DoubleClick += new System.EventHandler(this.listView2_DoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Rank";
            this.columnHeader2.Width = 75;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 320;
            // 
            // billetsContextMenu
            // 
            this.billetsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBilletMenuItem,
            this.removeBilletMenuItem,
            this.duplicateBilletMenuItem,
            this.toolStripSeparator3,
            this.copyFromMenuItem,
            this.toolStripSeparator2,
            this.viewModeToolStripMenuItem});
            this.billetsContextMenu.Name = "billetsContextMenu";
            this.billetsContextMenu.Size = new System.Drawing.Size(154, 126);
            this.billetsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.billetsContextMenu_Opening);
            // 
            // addBilletMenuItem
            // 
            this.addBilletMenuItem.Name = "addBilletMenuItem";
            this.addBilletMenuItem.Size = new System.Drawing.Size(153, 22);
            this.addBilletMenuItem.Text = "Add Billet";
            this.addBilletMenuItem.Click += new System.EventHandler(this.addBilletMenuItem_Click);
            // 
            // removeBilletMenuItem
            // 
            this.removeBilletMenuItem.Name = "removeBilletMenuItem";
            this.removeBilletMenuItem.Size = new System.Drawing.Size(153, 22);
            this.removeBilletMenuItem.Text = "Remove Billet";
            this.removeBilletMenuItem.Click += new System.EventHandler(this.removeBilletMenuItem_Click);
            // 
            // duplicateBilletMenuItem
            // 
            this.duplicateBilletMenuItem.Name = "duplicateBilletMenuItem";
            this.duplicateBilletMenuItem.Size = new System.Drawing.Size(153, 22);
            this.duplicateBilletMenuItem.Text = "Duplicate Billet";
            this.duplicateBilletMenuItem.Click += new System.EventHandler(this.duplicateBilletMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(150, 6);
            // 
            // copyFromMenuItem
            // 
            this.copyFromMenuItem.Name = "copyFromMenuItem";
            this.copyFromMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyFromMenuItem.Text = "Copy From";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(150, 6);
            // 
            // viewModeToolStripMenuItem
            // 
            this.viewModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeIconsToolStripMenuItem,
            this.tilesToolStripMenuItem});
            this.viewModeToolStripMenuItem.Name = "viewModeToolStripMenuItem";
            this.viewModeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.viewModeToolStripMenuItem.Text = "View Mode";
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.Checked = true;
            this.largeIconsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.largeIconsToolStripMenuItem.Text = "Large Icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
            // 
            // tilesToolStripMenuItem
            // 
            this.tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
            this.tilesToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.tilesToolStripMenuItem.Text = "Tiles";
            this.tilesToolStripMenuItem.Click += new System.EventHandler(this.tilesToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Location = new System.Drawing.Point(16, 288);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 165);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sub Units";
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4});
            this.listView1.ContextMenuStrip = this.subUnitsContextMenu;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(3, 16);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(334, 146);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            this.listView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView1_DragEnter);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 250;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Count";
            // 
            // subUnitsContextMenu
            // 
            this.subUnitsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adjustUnitCountToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeSubUnitToolStripMenuItem});
            this.subUnitsContextMenu.Name = "subUnitsContextMenu";
            this.subUnitsContextMenu.Size = new System.Drawing.Size(170, 54);
            this.subUnitsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.subUnitsContextMenu_Opening);
            // 
            // adjustUnitCountToolStripMenuItem
            // 
            this.adjustUnitCountToolStripMenuItem.Name = "adjustUnitCountToolStripMenuItem";
            this.adjustUnitCountToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.adjustUnitCountToolStripMenuItem.Text = "Adjust Unit Count";
            this.adjustUnitCountToolStripMenuItem.Click += new System.EventHandler(this.adjustUnitCountToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
            // 
            // removeSubUnitToolStripMenuItem
            // 
            this.removeSubUnitToolStripMenuItem.Name = "removeSubUnitToolStripMenuItem";
            this.removeSubUnitToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.removeSubUnitToolStripMenuItem.Text = "Remove Sub Unit";
            this.removeSubUnitToolStripMenuItem.Click += new System.EventHandler(this.removeSubUnitToolStripMenuItem_Click);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.newButton);
            this.bottomPanel.Controls.Add(this.deleteButton);
            this.bottomPanel.Controls.Add(this.finalizeButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 587);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(1084, 60);
            this.bottomPanel.TabIndex = 20;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // finalizeButton
            // 
            this.finalizeButton.Location = new System.Drawing.Point(883, 18);
            this.finalizeButton.Name = "finalizeButton";
            this.finalizeButton.Size = new System.Drawing.Size(150, 30);
            this.finalizeButton.TabIndex = 13;
            this.finalizeButton.Text = "Finalize";
            this.finalizeButton.UseVisualStyleBackColor = true;
            this.finalizeButton.Click += new System.EventHandler(this.finalizeButton_Click);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.label6);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1084, 75);
            this.headerPanel.TabIndex = 4;
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
            this.label6.Size = new System.Drawing.Size(242, 37);
            this.label6.TabIndex = 0;
            this.label6.Text = "Unit Template Manager";
            // 
            // UnitTypeManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1084, 647);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.bottomPanel);
            this.Name = "UnitTypeManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unit Template Manager";
            this.sidePanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.billetsContextMenu.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.subUnitsContextMenu.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox templateNameBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox echelonTypeSelect;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ContextMenuStrip billetsContextMenu;
        private System.Windows.Forms.ContextMenuStrip subUnitsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addBilletMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeBilletMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adjustUnitCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeSubUnitToolStripMenuItem;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button finalizeButton;
        private System.Windows.Forms.TextBox unitNameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label referencesLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox promotionPoolSelect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem viewModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
        private System.Windows.Forms.TextBox shortNameBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem copyFromMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateBilletMenuItem;
    }
}