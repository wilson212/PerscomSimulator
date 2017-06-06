namespace Perscom
{
    partial class BilletEditorForm
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
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.billetCatSelect = new System.Windows.Forms.ComboBox();
            this.zIndexBox = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.statureBox = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.earlyRetireCheckBox = new System.Windows.Forms.CheckBox();
            this.billetNameBox = new System.Windows.Forms.TextBox();
            this.maxTigBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.minTigBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.billetRankRangeSelect = new System.Windows.Forms.ComboBox();
            this.billetRangeCheckBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.billetRankSelect = new System.Windows.Forms.ComboBox();
            this.entryLevelCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.spawnGenSelect = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.spawnSpecialtySelect = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.specialtyCheckBox = new System.Windows.Forms.CheckBox();
            this.specialtySelect = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label11 = new System.Windows.Forms.Label();
            this.promotionPoolSelect = new System.Windows.Forms.ComboBox();
            this.rankPicture = new System.Windows.Forms.PictureBox();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editRequiredSpecialtiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zIndexBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTigBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTigBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox5);
            this.mainPanel.Controls.Add(this.groupBox4);
            this.mainPanel.Controls.Add(this.groupBox3);
            this.mainPanel.Controls.Add(this.bottomPanel);
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1014, 537);
            this.mainPanel.TabIndex = 8;
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.saveButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 477);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(1014, 60);
            this.bottomPanel.TabIndex = 18;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(432, 18);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(150, 30);
            this.saveButton.TabIndex = 12;
            this.saveButton.Text = "Save Changes";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.promotionPoolSelect);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.billetCatSelect);
            this.groupBox2.Controls.Add(this.zIndexBox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.statureBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.earlyRetireCheckBox);
            this.groupBox2.Controls.Add(this.billetNameBox);
            this.groupBox2.Controls.Add(this.maxTigBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.minTigBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(25, 149);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(475, 293);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Details";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Billet Catagory:";
            // 
            // billetCatSelect
            // 
            this.billetCatSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.billetCatSelect.FormattingEnabled = true;
            this.billetCatSelect.Location = new System.Drawing.Point(211, 68);
            this.billetCatSelect.Name = "billetCatSelect";
            this.billetCatSelect.Size = new System.Drawing.Size(202, 21);
            this.billetCatSelect.TabIndex = 6;
            // 
            // zIndexBox
            // 
            this.zIndexBox.Location = new System.Drawing.Point(211, 254);
            this.zIndexBox.Name = "zIndexBox";
            this.zIndexBox.Size = new System.Drawing.Size(95, 20);
            this.zIndexBox.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 256);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Z-Index:";
            // 
            // statureBox
            // 
            this.statureBox.Location = new System.Drawing.Point(211, 142);
            this.statureBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.statureBox.Name = "statureBox";
            this.statureBox.Size = new System.Drawing.Size(95, 20);
            this.statureBox.TabIndex = 7;
            this.statureBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Stature:";
            // 
            // earlyRetireCheckBox
            // 
            this.earlyRetireCheckBox.AutoSize = true;
            this.earlyRetireCheckBox.Location = new System.Drawing.Point(336, 180);
            this.earlyRetireCheckBox.Name = "earlyRetireCheckBox";
            this.earlyRetireCheckBox.Size = new System.Drawing.Size(102, 17);
            this.earlyRetireCheckBox.TabIndex = 9;
            this.earlyRetireCheckBox.Text = "Can Retire Early";
            this.earlyRetireCheckBox.UseVisualStyleBackColor = true;
            // 
            // billetNameBox
            // 
            this.billetNameBox.Location = new System.Drawing.Point(211, 33);
            this.billetNameBox.Name = "billetNameBox";
            this.billetNameBox.Size = new System.Drawing.Size(202, 20);
            this.billetNameBox.TabIndex = 5;
            // 
            // maxTigBox
            // 
            this.maxTigBox.Location = new System.Drawing.Point(211, 217);
            this.maxTigBox.Name = "maxTigBox";
            this.maxTigBox.Size = new System.Drawing.Size(95, 20);
            this.maxTigBox.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 219);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Maximum Tour Length:";
            // 
            // minTigBox
            // 
            this.minTigBox.Location = new System.Drawing.Point(211, 179);
            this.minTigBox.Name = "minTigBox";
            this.minTigBox.Size = new System.Drawing.Size(95, 20);
            this.minTigBox.TabIndex = 8;
            this.minTigBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Minimum Tour Length:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Billet Name: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.billetRankRangeSelect);
            this.groupBox1.Controls.Add(this.billetRangeCheckBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.billetRankSelect);
            this.groupBox1.Controls.Add(this.rankPicture);
            this.groupBox1.Location = new System.Drawing.Point(27, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(475, 109);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Billet Rank";
            // 
            // billetRankRangeSelect
            // 
            this.billetRankRangeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.billetRankRangeSelect.Enabled = false;
            this.billetRankRangeSelect.FormattingEnabled = true;
            this.billetRankRangeSelect.Location = new System.Drawing.Point(209, 67);
            this.billetRankRangeSelect.Name = "billetRankRangeSelect";
            this.billetRankRangeSelect.Size = new System.Drawing.Size(202, 21);
            this.billetRankRangeSelect.TabIndex = 4;
            // 
            // billetRangeCheckBox
            // 
            this.billetRangeCheckBox.AutoSize = true;
            this.billetRangeCheckBox.Location = new System.Drawing.Point(123, 69);
            this.billetRangeCheckBox.Name = "billetRangeCheckBox";
            this.billetRangeCheckBox.Size = new System.Drawing.Size(83, 17);
            this.billetRangeCheckBox.TabIndex = 3;
            this.billetRangeCheckBox.Text = "Add Range:";
            this.billetRangeCheckBox.UseVisualStyleBackColor = true;
            this.billetRangeCheckBox.CheckedChanged += new System.EventHandler(this.billetRangeCheckBox_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(120, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Billet Rank:";
            // 
            // billetRankSelect
            // 
            this.billetRankSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.billetRankSelect.FormattingEnabled = true;
            this.billetRankSelect.Location = new System.Drawing.Point(209, 31);
            this.billetRankSelect.Name = "billetRankSelect";
            this.billetRankSelect.Size = new System.Drawing.Size(202, 21);
            this.billetRankSelect.TabIndex = 2;
            this.billetRankSelect.SelectedIndexChanged += new System.EventHandler(this.billetRankSelect_SelectedIndexChanged);
            // 
            // entryLevelCheckBox
            // 
            this.entryLevelCheckBox.AutoSize = true;
            this.entryLevelCheckBox.Location = new System.Drawing.Point(35, 36);
            this.entryLevelCheckBox.Name = "entryLevelCheckBox";
            this.entryLevelCheckBox.Size = new System.Drawing.Size(90, 17);
            this.entryLevelCheckBox.TabIndex = 30;
            this.entryLevelCheckBox.Text = "Is Entry Level";
            this.entryLevelCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.spawnSpecialtySelect);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.spawnGenSelect);
            this.groupBox3.Controls.Add(this.entryLevelCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(523, 24);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(466, 157);
            this.groupBox3.TabIndex = 31;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Soldier Spawn Settings";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "Spawn Generator:";
            // 
            // spawnGenSelect
            // 
            this.spawnGenSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.spawnGenSelect.Enabled = false;
            this.spawnGenSelect.FormattingEnabled = true;
            this.spawnGenSelect.Location = new System.Drawing.Point(216, 73);
            this.spawnGenSelect.Name = "spawnGenSelect";
            this.spawnGenSelect.Size = new System.Drawing.Size(202, 21);
            this.spawnGenSelect.TabIndex = 31;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(32, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 13);
            this.label10.TabIndex = 33;
            this.label10.Text = "Spawned Soldier MOS:";
            // 
            // spawnSpecialtySelect
            // 
            this.spawnSpecialtySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.spawnSpecialtySelect.Enabled = false;
            this.spawnSpecialtySelect.FormattingEnabled = true;
            this.spawnSpecialtySelect.Location = new System.Drawing.Point(216, 112);
            this.spawnSpecialtySelect.Name = "spawnSpecialtySelect";
            this.spawnSpecialtySelect.Size = new System.Drawing.Size(202, 21);
            this.spawnSpecialtySelect.TabIndex = 34;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.specialtySelect);
            this.groupBox4.Controls.Add(this.specialtyCheckBox);
            this.groupBox4.Location = new System.Drawing.Point(523, 187);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(466, 87);
            this.groupBox4.TabIndex = 32;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Specialty Changes";
            // 
            // specialtyCheckBox
            // 
            this.specialtyCheckBox.AutoSize = true;
            this.specialtyCheckBox.Location = new System.Drawing.Point(33, 40);
            this.specialtyCheckBox.Name = "specialtyCheckBox";
            this.specialtyCheckBox.Size = new System.Drawing.Size(154, 17);
            this.specialtyCheckBox.TabIndex = 31;
            this.specialtyCheckBox.Text = "Billet Changes Specialty to:";
            this.specialtyCheckBox.UseVisualStyleBackColor = true;
            this.specialtyCheckBox.CheckedChanged += new System.EventHandler(this.specialtyCheckBox_CheckedChanged);
            // 
            // specialtySelect
            // 
            this.specialtySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.specialtySelect.Enabled = false;
            this.specialtySelect.FormattingEnabled = true;
            this.specialtySelect.Location = new System.Drawing.Point(214, 38);
            this.specialtySelect.Name = "specialtySelect";
            this.specialtySelect.Size = new System.Drawing.Size(202, 21);
            this.specialtySelect.TabIndex = 35;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.listView1);
            this.groupBox5.Location = new System.Drawing.Point(523, 280);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(465, 162);
            this.groupBox5.TabIndex = 33;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Required Specialties";
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(3, 16);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(459, 143);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Code";
            this.columnHeader1.Width = 75;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 340;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(27, 107);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Promotion Pool:";
            // 
            // promotionPoolSelect
            // 
            this.promotionPoolSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.promotionPoolSelect.FormattingEnabled = true;
            this.promotionPoolSelect.Location = new System.Drawing.Point(211, 104);
            this.promotionPoolSelect.Name = "promotionPoolSelect";
            this.promotionPoolSelect.Size = new System.Drawing.Size(202, 21);
            this.promotionPoolSelect.TabIndex = 30;
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
            this.headerPanel.Size = new System.Drawing.Size(1014, 75);
            this.headerPanel.TabIndex = 3;
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
            this.label6.Text = "Billet Manager";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editRequiredSpecialtiesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(203, 48);
            // 
            // editRequiredSpecialtiesToolStripMenuItem
            // 
            this.editRequiredSpecialtiesToolStripMenuItem.Name = "editRequiredSpecialtiesToolStripMenuItem";
            this.editRequiredSpecialtiesToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.editRequiredSpecialtiesToolStripMenuItem.Text = "Edit Required Specialties";
            this.editRequiredSpecialtiesToolStripMenuItem.Click += new System.EventHandler(this.editRequiredSpecialtiesToolStripMenuItem_Click);
            // 
            // BilletEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1014, 612);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BilletEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Billet Editor Form";
            this.mainPanel.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zIndexBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTigBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTigBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown statureBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox earlyRetireCheckBox;
        private System.Windows.Forms.TextBox billetNameBox;
        private System.Windows.Forms.NumericUpDown maxTigBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown minTigBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox billetRankSelect;
        private System.Windows.Forms.PictureBox rankPicture;
        private System.Windows.Forms.ComboBox billetRankRangeSelect;
        private System.Windows.Forms.CheckBox billetRangeCheckBox;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.NumericUpDown zIndexBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox billetCatSelect;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox spawnGenSelect;
        private System.Windows.Forms.CheckBox entryLevelCheckBox;
        private System.Windows.Forms.ComboBox spawnSpecialtySelect;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox specialtySelect;
        private System.Windows.Forms.CheckBox specialtyCheckBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox promotionPoolSelect;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editRequiredSpecialtiesToolStripMenuItem;
    }
}