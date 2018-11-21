namespace Perscom
{
    partial class RankEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RankEditorForm));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.precedenceBox = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.rankAbbrBox = new System.Windows.Forms.TextBox();
            this.promotableBox = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.rankGradeBox = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.autoPromoteCheckBox = new System.Windows.Forms.CheckBox();
            this.rankNameBox = new System.Windows.Forms.TextBox();
            this.maxTigBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.minTigBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rankTypeSelect = new System.Windows.Forms.ComboBox();
            this.imageSelect = new System.Windows.Forms.ComboBox();
            this.rankPicture = new System.Windows.Forms.PictureBox();
            this.headerPanel.SuspendLayout();
            this.sidePanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.precedenceBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.promotableBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rankGradeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTigBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTigBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).BeginInit();
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
            this.headerPanel.Size = new System.Drawing.Size(797, 75);
            this.headerPanel.TabIndex = 2;
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
            this.label6.Text = "Rank Settings";
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
            this.sidePanel.Size = new System.Drawing.Size(240, 436);
            this.sidePanel.TabIndex = 6;
            this.sidePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sidePanel_Paint);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(132, 366);
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
            this.newButton.Location = new System.Drawing.Point(34, 366);
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
            this.treeView1.Size = new System.Drawing.Size(214, 314);
            this.treeView1.TabIndex = 0;
            this.treeView1.TabStop = false;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(240, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(557, 436);
            this.mainPanel.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.precedenceBox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.rankAbbrBox);
            this.groupBox2.Controls.Add(this.promotableBox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.rankGradeBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.autoPromoteCheckBox);
            this.groupBox2.Controls.Add(this.rankNameBox);
            this.groupBox2.Controls.Add(this.maxTigBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.minTigBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.applyButton);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(29, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(497, 303);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Details";
            // 
            // precedenceBox
            // 
            this.precedenceBox.Location = new System.Drawing.Point(211, 96);
            this.precedenceBox.Name = "precedenceBox";
            this.precedenceBox.Size = new System.Drawing.Size(95, 20);
            this.precedenceBox.TabIndex = 28;
            this.precedenceBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(27, 98);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "Rank Precedence:";
            // 
            // rankAbbrBox
            // 
            this.rankAbbrBox.Location = new System.Drawing.Point(413, 25);
            this.rankAbbrBox.Name = "rankAbbrBox";
            this.rankAbbrBox.Size = new System.Drawing.Size(55, 20);
            this.rankAbbrBox.TabIndex = 4;
            // 
            // promotableBox
            // 
            this.promotableBox.Location = new System.Drawing.Point(211, 204);
            this.promotableBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.promotableBox.Name = "promotableBox";
            this.promotableBox.Size = new System.Drawing.Size(95, 20);
            this.promotableBox.TabIndex = 8;
            this.promotableBox.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Promotable Time In Grade:";
            // 
            // rankGradeBox
            // 
            this.rankGradeBox.Location = new System.Drawing.Point(211, 60);
            this.rankGradeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rankGradeBox.Name = "rankGradeBox";
            this.rankGradeBox.Size = new System.Drawing.Size(95, 20);
            this.rankGradeBox.TabIndex = 5;
            this.rankGradeBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Rank Grade:";
            // 
            // autoPromoteCheckBox
            // 
            this.autoPromoteCheckBox.AutoSize = true;
            this.autoPromoteCheckBox.Location = new System.Drawing.Point(338, 205);
            this.autoPromoteCheckBox.Name = "autoPromoteCheckBox";
            this.autoPromoteCheckBox.Size = new System.Drawing.Size(130, 17);
            this.autoPromoteCheckBox.TabIndex = 9;
            this.autoPromoteCheckBox.Text = "Automatically Promote";
            this.autoPromoteCheckBox.UseVisualStyleBackColor = true;
            // 
            // rankNameBox
            // 
            this.rankNameBox.Location = new System.Drawing.Point(211, 25);
            this.rankNameBox.Name = "rankNameBox";
            this.rankNameBox.Size = new System.Drawing.Size(196, 20);
            this.rankNameBox.TabIndex = 3;
            // 
            // maxTigBox
            // 
            this.maxTigBox.Location = new System.Drawing.Point(211, 168);
            this.maxTigBox.Name = "maxTigBox";
            this.maxTigBox.Size = new System.Drawing.Size(95, 20);
            this.maxTigBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Maximum Time In Grade:";
            // 
            // minTigBox
            // 
            this.minTigBox.Location = new System.Drawing.Point(211, 132);
            this.minTigBox.Name = "minTigBox";
            this.minTigBox.Size = new System.Drawing.Size(95, 20);
            this.minTigBox.TabIndex = 6;
            this.minTigBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Minimum Time In Grade:";
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(173, 250);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(150, 30);
            this.applyButton.TabIndex = 10;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Rank Name / Abbreviation: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.rankTypeSelect);
            this.groupBox1.Controls.Add(this.imageSelect);
            this.groupBox1.Controls.Add(this.rankPicture);
            this.groupBox1.Location = new System.Drawing.Point(31, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(495, 91);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(120, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Rank Image:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(120, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Rank Type:";
            // 
            // rankTypeSelect
            // 
            this.rankTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankTypeSelect.FormattingEnabled = true;
            this.rankTypeSelect.Location = new System.Drawing.Point(209, 21);
            this.rankTypeSelect.Name = "rankTypeSelect";
            this.rankTypeSelect.Size = new System.Drawing.Size(200, 21);
            this.rankTypeSelect.TabIndex = 1;
            this.rankTypeSelect.SelectedIndexChanged += new System.EventHandler(this.rankType_SelectedIndexChanged);
            // 
            // imageSelect
            // 
            this.imageSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageSelect.FormattingEnabled = true;
            this.imageSelect.Location = new System.Drawing.Point(209, 59);
            this.imageSelect.Name = "imageSelect";
            this.imageSelect.Size = new System.Drawing.Size(202, 21);
            this.imageSelect.TabIndex = 2;
            this.imageSelect.SelectedIndexChanged += new System.EventHandler(this.imageSelect_SelectedIndexChanged);
            // 
            // rankPicture
            // 
            this.rankPicture.Location = new System.Drawing.Point(20, 18);
            this.rankPicture.Name = "rankPicture";
            this.rankPicture.Size = new System.Drawing.Size(64, 64);
            this.rankPicture.TabIndex = 14;
            this.rankPicture.TabStop = false;
            // 
            // RankEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(797, 511);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RankEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RankEditor";
            this.headerPanel.ResumeLayout(false);
            this.sidePanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.precedenceBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.promotableBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rankGradeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTigBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTigBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox imageSelect;
        private System.Windows.Forms.PictureBox rankPicture;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox rankNameBox;
        private System.Windows.Forms.NumericUpDown maxTigBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown minTigBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox autoPromoteCheckBox;
        private System.Windows.Forms.NumericUpDown rankGradeBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown promotableBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox rankAbbrBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.ComboBox rankTypeSelect;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown precedenceBox;
        private System.Windows.Forms.Label label9;
    }
}