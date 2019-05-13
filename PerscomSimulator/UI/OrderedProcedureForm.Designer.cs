namespace Perscom
{
    partial class OrderedProcedureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderedProcedureForm));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.ListViewWithReordering();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSoldierPoolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.billetRankSelect = new System.Windows.Forms.ComboBox();
            this.rankPicture = new System.Windows.Forms.PictureBox();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.careerGeneratorBox = new System.Windows.Forms.ComboBox();
            this.newProbLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.newSoldierProbability = new System.Windows.Forms.TrackBar();
            this.newSoldiersCheckBox = new System.Windows.Forms.CheckBox();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.headerPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newSoldierProbability)).BeginInit();
            this.sidePanel.SuspendLayout();
            this.SuspendLayout();
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
            this.headerPanel.TabIndex = 15;
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
            this.label6.Size = new System.Drawing.Size(516, 37);
            this.label6.TabIndex = 0;
            this.label6.Text = "Billet Ordered (Priority) Selection Procedure Editor";
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox6);
            this.mainPanel.Controls.Add(this.groupBox4);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(240, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(844, 486);
            this.mainPanel.TabIndex = 18;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.listView1);
            this.groupBox6.Location = new System.Drawing.Point(430, 21);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(396, 389);
            this.groupBox6.TabIndex = 36;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Ordered Procedure Pools by Priority (Top to Bottom)";
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.AllowReorder = true;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader1});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.LineColor = System.Drawing.Color.Gray;
            this.listView1.Location = new System.Drawing.Point(3, 16);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(390, 370);
            this.listView1.TabIndex = 2;
            this.listView1.TileSize = new System.Drawing.Size(168, 200);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Rank";
            this.columnHeader7.Width = 200;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "RC";
            this.columnHeader8.Width = 27;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "P";
            this.columnHeader9.Width = 25;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "O";
            this.columnHeader10.Width = 25;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "NL";
            this.columnHeader11.Width = 27;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Prob.";
            this.columnHeader12.Width = 40;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSoldierPoolToolStripMenuItem,
            this.removeItemToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(184, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // addSoldierPoolToolStripMenuItem
            // 
            this.addSoldierPoolToolStripMenuItem.Name = "addSoldierPoolToolStripMenuItem";
            this.addSoldierPoolToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.addSoldierPoolToolStripMenuItem.Text = "Add Soldier Pool";
            this.addSoldierPoolToolStripMenuItem.Click += new System.EventHandler(this.addSoldierPoolToolStripMenuItem_Click);
            // 
            // removeItemToolStripMenuItem
            // 
            this.removeItemToolStripMenuItem.Name = "removeItemToolStripMenuItem";
            this.removeItemToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.removeItemToolStripMenuItem.Text = "Remove Soldier Pool";
            this.removeItemToolStripMenuItem.Click += new System.EventHandler(this.removeItemToolStripMenuItem_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.billetRankSelect);
            this.groupBox4.Controls.Add(this.rankPicture);
            this.groupBox4.Controls.Add(this.nameBox);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(18, 21);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(395, 165);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Details";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(137, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 30;
            this.label8.Text = "Billet Rank:";
            // 
            // billetRankSelect
            // 
            this.billetRankSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.billetRankSelect.FormattingEnabled = true;
            this.billetRankSelect.Location = new System.Drawing.Point(140, 60);
            this.billetRankSelect.Name = "billetRankSelect";
            this.billetRankSelect.Size = new System.Drawing.Size(233, 21);
            this.billetRankSelect.TabIndex = 28;
            this.billetRankSelect.SelectedIndexChanged += new System.EventHandler(this.billetRankSelect_SelectedIndexChanged);
            // 
            // rankPicture
            // 
            this.rankPicture.Location = new System.Drawing.Point(27, 26);
            this.rankPicture.Name = "rankPicture";
            this.rankPicture.Size = new System.Drawing.Size(64, 64);
            this.rankPicture.TabIndex = 29;
            this.rankPicture.TabStop = false;
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(140, 113);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(233, 20);
            this.nameBox.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Procedure Name: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.applyButton);
            this.groupBox1.Location = new System.Drawing.Point(18, 416);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(805, 59);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(177, 19);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(450, 30);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.careerGeneratorBox);
            this.groupBox2.Controls.Add(this.newProbLabel);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.newSoldierProbability);
            this.groupBox2.Controls.Add(this.newSoldiersCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(18, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 218);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Entry Level Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "New Soldier Career:";
            // 
            // careerGeneratorBox
            // 
            this.careerGeneratorBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.careerGeneratorBox.Enabled = false;
            this.careerGeneratorBox.FormattingEnabled = true;
            this.careerGeneratorBox.Location = new System.Drawing.Point(145, 73);
            this.careerGeneratorBox.Name = "careerGeneratorBox";
            this.careerGeneratorBox.Size = new System.Drawing.Size(222, 21);
            this.careerGeneratorBox.TabIndex = 38;
            // 
            // newProbLabel
            // 
            this.newProbLabel.AutoSize = true;
            this.newProbLabel.Location = new System.Drawing.Point(334, 128);
            this.newProbLabel.Name = "newProbLabel";
            this.newProbLabel.Size = new System.Drawing.Size(21, 13);
            this.newProbLabel.TabIndex = 33;
            this.newProbLabel.Text = "0%";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Probability";
            // 
            // newSoldierProbability
            // 
            this.newSoldierProbability.Location = new System.Drawing.Point(27, 147);
            this.newSoldierProbability.Maximum = 100;
            this.newSoldierProbability.Name = "newSoldierProbability";
            this.newSoldierProbability.Size = new System.Drawing.Size(340, 45);
            this.newSoldierProbability.TabIndex = 31;
            this.newSoldierProbability.TickFrequency = 5;
            this.newSoldierProbability.ValueChanged += new System.EventHandler(this.newSoldierProbability_ValueChanged);
            // 
            // newSoldiersCheckBox
            // 
            this.newSoldiersCheckBox.AutoSize = true;
            this.newSoldiersCheckBox.Location = new System.Drawing.Point(27, 36);
            this.newSoldiersCheckBox.Name = "newSoldiersCheckBox";
            this.newSoldiersCheckBox.Size = new System.Drawing.Size(293, 17);
            this.newSoldiersCheckBox.TabIndex = 30;
            this.newSoldiersCheckBox.Text = "Create new soldier if all procedures fail to produce soldier";
            this.newSoldiersCheckBox.UseVisualStyleBackColor = true;
            this.newSoldiersCheckBox.CheckedChanged += new System.EventHandler(this.newSoldiersCheckBox_CheckedChanged);
            // 
            // sidePanel
            // 
            this.sidePanel.Controls.Add(this.deleteButton);
            this.sidePanel.Controls.Add(this.newButton);
            this.sidePanel.Controls.Add(this.treeView1);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 75);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Padding = new System.Windows.Forms.Padding(5, 10, 0, 0);
            this.sidePanel.Size = new System.Drawing.Size(240, 486);
            this.sidePanel.TabIndex = 17;
            this.sidePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sidePanel_Paint);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(132, 435);
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
            this.newButton.Location = new System.Drawing.Point(34, 435);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 23);
            this.newButton.TabIndex = 11;
            this.newButton.TabStop = false;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Location = new System.Drawing.Point(10, 7);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(214, 421);
            this.treeView1.TabIndex = 0;
            this.treeView1.TabStop = false;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // OrderedProcedureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1084, 561);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.Controls.Add(this.headerPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderedProcedureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ordered Procedure Form";
            this.headerPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newSoldierProbability)).EndInit();
            this.sidePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.ListViewWithReordering listView1;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox careerGeneratorBox;
        private System.Windows.Forms.Label newProbLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar newSoldierProbability;
        private System.Windows.Forms.CheckBox newSoldiersCheckBox;
        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox billetRankSelect;
        private System.Windows.Forms.PictureBox rankPicture;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addSoldierPoolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeItemToolStripMenuItem;
    }
}