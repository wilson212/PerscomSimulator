namespace Perscom
{
    partial class SoldierGeneratorEditorForm
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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.totalProbLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lengthCheckBox = new System.Windows.Forms.CheckBox();
            this.addButton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label8 = new System.Windows.Forms.Label();
            this.rankSelect = new System.Windows.Forms.ComboBox();
            this.existProbLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.existTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.newProbLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.newTrackBar = new System.Windows.Forms.TrackBar();
            this.newCheckBox = new System.Windows.Forms.CheckBox();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.promotableCheckBox = new System.Windows.Forms.CheckBox();
            this.orderedCheckBox = new System.Windows.Forms.CheckBox();
            this.lockedCheckBox = new System.Windows.Forms.CheckBox();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainPanel.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.existTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newTrackBar)).BeginInit();
            this.sidePanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox4);
            this.mainPanel.Controls.Add(this.groupBox3);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(240, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(844, 487);
            this.mainPanel.TabIndex = 16;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.totalProbLabel);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.nameBox);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(21, 21);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(395, 165);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Details";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(270, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(33, 13);
            this.label11.TabIndex = 33;
            this.label11.Text = "/ 100";
            // 
            // totalProbLabel
            // 
            this.totalProbLabel.ForeColor = System.Drawing.Color.Red;
            this.totalProbLabel.Location = new System.Drawing.Point(238, 43);
            this.totalProbLabel.Name = "totalProbLabel";
            this.totalProbLabel.Size = new System.Drawing.Size(32, 13);
            this.totalProbLabel.TabIndex = 32;
            this.totalProbLabel.Text = "0";
            this.totalProbLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(85, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Current Cumulative Probability:";
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(140, 89);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(233, 20);
            this.nameBox.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Generator Name: ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lockedCheckBox);
            this.groupBox3.Controls.Add(this.orderedCheckBox);
            this.groupBox3.Controls.Add(this.promotableCheckBox);
            this.groupBox3.Controls.Add(this.lengthCheckBox);
            this.groupBox3.Controls.Add(this.addButton);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.rankSelect);
            this.groupBox3.Controls.Add(this.existProbLabel);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.existTrackBar);
            this.groupBox3.Location = new System.Drawing.Point(428, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(396, 454);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Take existing soldier from pool";
            // 
            // lengthCheckBox
            // 
            this.lengthCheckBox.AutoSize = true;
            this.lengthCheckBox.Location = new System.Drawing.Point(145, 74);
            this.lengthCheckBox.Name = "lengthCheckBox";
            this.lengthCheckBox.Size = new System.Drawing.Size(153, 17);
            this.lengthCheckBox.TabIndex = 39;
            this.lengthCheckBox.Text = "Recalculate Career Length";
            this.lengthCheckBox.UseVisualStyleBackColor = true;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(130, 259);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(150, 30);
            this.addButton.TabIndex = 38;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.listView1);
            this.groupBox5.Location = new System.Drawing.Point(8, 307);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(380, 140);
            this.groupBox5.TabIndex = 34;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Existing Pools";
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader2});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(3, 16);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(374, 121);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Rank";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Prob.";
            this.columnHeader2.Width = 40;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeItemToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(145, 26);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // removeItemToolStripMenuItem
            // 
            this.removeItemToolStripMenuItem.Name = "removeItemToolStripMenuItem";
            this.removeItemToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.removeItemToolStripMenuItem.Text = "Remove Item";
            this.removeItemToolStripMenuItem.Click += new System.EventHandler(this.removeItemToolStripMenuItem_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(36, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Rank:";
            // 
            // rankSelect
            // 
            this.rankSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankSelect.FormattingEnabled = true;
            this.rankSelect.Location = new System.Drawing.Point(145, 35);
            this.rankSelect.Name = "rankSelect";
            this.rankSelect.Size = new System.Drawing.Size(222, 21);
            this.rankSelect.TabIndex = 34;
            // 
            // existProbLabel
            // 
            this.existProbLabel.AutoSize = true;
            this.existProbLabel.Location = new System.Drawing.Point(334, 184);
            this.existProbLabel.Name = "existProbLabel";
            this.existProbLabel.Size = new System.Drawing.Size(33, 13);
            this.existProbLabel.TabIndex = 33;
            this.existProbLabel.Text = "100%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Probability";
            // 
            // existTrackBar
            // 
            this.existTrackBar.Location = new System.Drawing.Point(27, 203);
            this.existTrackBar.Maximum = 100;
            this.existTrackBar.Minimum = 1;
            this.existTrackBar.Name = "existTrackBar";
            this.existTrackBar.Size = new System.Drawing.Size(340, 45);
            this.existTrackBar.TabIndex = 31;
            this.existTrackBar.TickFrequency = 5;
            this.existTrackBar.Value = 1;
            this.existTrackBar.ValueChanged += new System.EventHandler(this.existTrackBar_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.applyButton);
            this.groupBox1.Location = new System.Drawing.Point(21, 416);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 59);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(122, 19);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(150, 30);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.newProbLabel);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.newTrackBar);
            this.groupBox2.Controls.Add(this.newCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(21, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 218);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Generate Settings";
            // 
            // newProbLabel
            // 
            this.newProbLabel.AutoSize = true;
            this.newProbLabel.Location = new System.Drawing.Point(334, 74);
            this.newProbLabel.Name = "newProbLabel";
            this.newProbLabel.Size = new System.Drawing.Size(33, 13);
            this.newProbLabel.TabIndex = 33;
            this.newProbLabel.Text = "100%";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Probability";
            // 
            // newTrackBar
            // 
            this.newTrackBar.Location = new System.Drawing.Point(27, 93);
            this.newTrackBar.Maximum = 100;
            this.newTrackBar.Name = "newTrackBar";
            this.newTrackBar.Size = new System.Drawing.Size(340, 45);
            this.newTrackBar.TabIndex = 31;
            this.newTrackBar.TickFrequency = 5;
            this.newTrackBar.Value = 1;
            this.newTrackBar.ValueChanged += new System.EventHandler(this.newTrackBar_ValueChanged);
            // 
            // newCheckBox
            // 
            this.newCheckBox.AutoSize = true;
            this.newCheckBox.Location = new System.Drawing.Point(27, 36);
            this.newCheckBox.Name = "newCheckBox";
            this.newCheckBox.Size = new System.Drawing.Size(122, 17);
            this.newCheckBox.TabIndex = 30;
            this.newCheckBox.Text = "Create New Soldiers";
            this.newCheckBox.UseVisualStyleBackColor = true;
            this.newCheckBox.CheckedChanged += new System.EventHandler(this.newCheckBox_CheckedChanged);
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
            this.sidePanel.Size = new System.Drawing.Size(240, 487);
            this.sidePanel.TabIndex = 15;
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
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.label6);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1084, 75);
            this.headerPanel.TabIndex = 14;
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
            this.label6.Text = "Soldier Spawn Generator Editor";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "RC";
            this.columnHeader3.Width = 27;
            // 
            // promotableCheckBox
            // 
            this.promotableCheckBox.AutoSize = true;
            this.promotableCheckBox.Location = new System.Drawing.Point(145, 97);
            this.promotableCheckBox.Name = "promotableCheckBox";
            this.promotableCheckBox.Size = new System.Drawing.Size(121, 17);
            this.promotableCheckBox.TabIndex = 40;
            this.promotableCheckBox.Text = "Must Be Promotable";
            this.promotableCheckBox.UseVisualStyleBackColor = true;
            // 
            // orderedCheckBox
            // 
            this.orderedCheckBox.AutoSize = true;
            this.orderedCheckBox.Location = new System.Drawing.Point(145, 120);
            this.orderedCheckBox.Name = "orderedCheckBox";
            this.orderedCheckBox.Size = new System.Drawing.Size(110, 17);
            this.orderedCheckBox.TabIndex = 41;
            this.orderedCheckBox.Text = "Order By Seniority";
            this.orderedCheckBox.UseVisualStyleBackColor = true;
            // 
            // lockedCheckBox
            // 
            this.lockedCheckBox.AutoSize = true;
            this.lockedCheckBox.Location = new System.Drawing.Point(145, 143);
            this.lockedCheckBox.Name = "lockedCheckBox";
            this.lockedCheckBox.Size = new System.Drawing.Size(177, 17);
            this.lockedCheckBox.TabIndex = 42;
            this.lockedCheckBox.Text = "Must Not Be Locked In To Billet";
            this.lockedCheckBox.UseVisualStyleBackColor = true;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "P";
            this.columnHeader4.Width = 25;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "O";
            this.columnHeader5.Width = 25;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "NL";
            this.columnHeader6.Width = 27;
            // 
            // SoldierGeneratorEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1084, 562);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "SoldierGeneratorEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SpawnGeneratorForm";
            this.mainPanel.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.existTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newTrackBar)).EndInit();
            this.sidePanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label newProbLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar newTrackBar;
        private System.Windows.Forms.CheckBox newCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label existProbLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar existTrackBar;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox rankSelect;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label totalProbLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox lengthCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeItemToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox promotableCheckBox;
        private System.Windows.Forms.CheckBox orderedCheckBox;
        private System.Windows.Forms.CheckBox lockedCheckBox;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}