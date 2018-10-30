namespace Perscom
{
    partial class SoldierGeneratorPoolForm
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.existProbLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.existTrackBar = new System.Windows.Forms.TrackBar();
            this.careerGeneratorBox = new System.Windows.Forms.ComboBox();
            this.lockedCheckBox = new System.Windows.Forms.CheckBox();
            this.promotableCheckBox = new System.Windows.Forms.CheckBox();
            this.newCareerCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rankSelect = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rankPicture = new System.Windows.Forms.PictureBox();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListViewWithReordering();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.mainPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.existTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.groupBox3);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(944, 386);
            this.mainPanel.TabIndex = 11;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.existProbLabel);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.existTrackBar);
            this.groupBox3.Controls.Add(this.careerGeneratorBox);
            this.groupBox3.Controls.Add(this.lockedCheckBox);
            this.groupBox3.Controls.Add(this.promotableCheckBox);
            this.groupBox3.Controls.Add(this.newCareerCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(17, 141);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(475, 231);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // existProbLabel
            // 
            this.existProbLabel.AutoSize = true;
            this.existProbLabel.Location = new System.Drawing.Point(374, 39);
            this.existProbLabel.Name = "existProbLabel";
            this.existProbLabel.Size = new System.Drawing.Size(21, 13);
            this.existProbLabel.TabIndex = 46;
            this.existProbLabel.Text = "0%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(76, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 45;
            this.label5.Text = "Probability";
            // 
            // existTrackBar
            // 
            this.existTrackBar.Location = new System.Drawing.Point(67, 58);
            this.existTrackBar.Maximum = 100;
            this.existTrackBar.Minimum = 1;
            this.existTrackBar.Name = "existTrackBar";
            this.existTrackBar.Size = new System.Drawing.Size(340, 45);
            this.existTrackBar.TabIndex = 44;
            this.existTrackBar.TickFrequency = 5;
            this.existTrackBar.Value = 1;
            this.existTrackBar.ValueChanged += new System.EventHandler(this.existTrackBar_ValueChanged);
            // 
            // careerGeneratorBox
            // 
            this.careerGeneratorBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.careerGeneratorBox.Enabled = false;
            this.careerGeneratorBox.FormattingEnabled = true;
            this.careerGeneratorBox.Location = new System.Drawing.Point(174, 127);
            this.careerGeneratorBox.Name = "careerGeneratorBox";
            this.careerGeneratorBox.Size = new System.Drawing.Size(265, 21);
            this.careerGeneratorBox.TabIndex = 43;
            // 
            // lockedCheckBox
            // 
            this.lockedCheckBox.AutoSize = true;
            this.lockedCheckBox.Location = new System.Drawing.Point(35, 196);
            this.lockedCheckBox.Name = "lockedCheckBox";
            this.lockedCheckBox.Size = new System.Drawing.Size(212, 17);
            this.lockedCheckBox.TabIndex = 42;
            this.lockedCheckBox.Text = "Soldier Must Not Be Locked In To Billet";
            this.lockedCheckBox.UseVisualStyleBackColor = true;
            // 
            // promotableCheckBox
            // 
            this.promotableCheckBox.AutoSize = true;
            this.promotableCheckBox.Location = new System.Drawing.Point(35, 173);
            this.promotableCheckBox.Name = "promotableCheckBox";
            this.promotableCheckBox.Size = new System.Drawing.Size(156, 17);
            this.promotableCheckBox.TabIndex = 40;
            this.promotableCheckBox.Text = "Soldier Must Be Promotable";
            this.promotableCheckBox.UseVisualStyleBackColor = true;
            // 
            // newCareerCheckBox
            // 
            this.newCareerCheckBox.AutoSize = true;
            this.newCareerCheckBox.Location = new System.Drawing.Point(35, 129);
            this.newCareerCheckBox.Name = "newCareerCheckBox";
            this.newCareerCheckBox.Size = new System.Drawing.Size(121, 17);
            this.newCareerCheckBox.TabIndex = 39;
            this.newCareerCheckBox.Text = "New Career Length:";
            this.newCareerCheckBox.UseVisualStyleBackColor = true;
            this.newCareerCheckBox.CheckedChanged += new System.EventHandler(this.newCareerCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rankPicture);
            this.groupBox1.Controls.Add(this.rankSelect);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(17, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(475, 109);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rank Pool";
            // 
            // rankSelect
            // 
            this.rankSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankSelect.FormattingEnabled = true;
            this.rankSelect.Location = new System.Drawing.Point(174, 47);
            this.rankSelect.Name = "rankSelect";
            this.rankSelect.Size = new System.Drawing.Size(265, 21);
            this.rankSelect.TabIndex = 34;
            this.rankSelect.SelectedIndexChanged += new System.EventHandler(this.rankSelect_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(108, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Rank:";
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.saveButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 461);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(944, 60);
            this.bottomPanel.TabIndex = 10;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(409, 18);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(150, 30);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save Changes";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView1);
            this.groupBox2.Location = new System.Drawing.Point(510, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(417, 352);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selection Sorting";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewSortingToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeItemToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(165, 54);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // addNewSortingToolStripMenuItem
            // 
            this.addNewSortingToolStripMenuItem.Name = "addNewSortingToolStripMenuItem";
            this.addNewSortingToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.addNewSortingToolStripMenuItem.Text = "Add New Sorting";
            this.addNewSortingToolStripMenuItem.Click += new System.EventHandler(this.addNewSortingToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
            // 
            // removeItemToolStripMenuItem
            // 
            this.removeItemToolStripMenuItem.Name = "removeItemToolStripMenuItem";
            this.removeItemToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.removeItemToolStripMenuItem.Text = "Remove Item";
            this.removeItemToolStripMenuItem.Click += new System.EventHandler(this.removeItemToolStripMenuItem_Click);
            // 
            // rankPicture
            // 
            this.rankPicture.Location = new System.Drawing.Point(20, 27);
            this.rankPicture.Name = "rankPicture";
            this.rankPicture.Size = new System.Drawing.Size(64, 64);
            this.rankPicture.TabIndex = 14;
            this.rankPicture.TabStop = false;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.label6);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(944, 75);
            this.headerPanel.TabIndex = 9;
            this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.headerPanel_Paint);
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(3, 16);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Scrollable = false;
            this.listView1.Size = new System.Drawing.Size(411, 333);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView1_ColumnWidthChanging);
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 65;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "";
            this.columnHeader2.Width = 220;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "";
            this.columnHeader3.Width = 120;
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
            this.label6.Size = new System.Drawing.Size(466, 37);
            this.label6.TabIndex = 0;
            this.label6.Text = "Generate Soldier From Existing Soldier Pool";
            // 
            // SoldierGeneratorPoolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(944, 521);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoldierGeneratorPoolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Soldier Generator Pool";
            this.mainPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.existTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox rankPicture;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox careerGeneratorBox;
        private System.Windows.Forms.CheckBox lockedCheckBox;
        private System.Windows.Forms.CheckBox promotableCheckBox;
        private System.Windows.Forms.CheckBox newCareerCheckBox;
        private System.Windows.Forms.ComboBox rankSelect;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label existProbLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar existTrackBar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListViewWithReordering listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addNewSortingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeItemToolStripMenuItem;
    }
}