namespace Perscom
{
    partial class SpawnEditorForm
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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.rankTypeSelect = new System.Windows.Forms.ComboBox();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.totalProbLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.maxInput = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.minInput = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.probInput = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.sidePanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(240, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(489, 417);
            this.mainPanel.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.maxInput);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.minInput);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.probInput);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.applyButton);
            this.groupBox2.Location = new System.Drawing.Point(29, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(433, 264);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Details";
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(141, 204);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(150, 30);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.totalProbLabel);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.rankTypeSelect);
            this.groupBox1.Location = new System.Drawing.Point(31, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(431, 106);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spawn Type";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(70, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Rank Type:";
            // 
            // rankTypeSelect
            // 
            this.rankTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankTypeSelect.FormattingEnabled = true;
            this.rankTypeSelect.Location = new System.Drawing.Point(159, 65);
            this.rankTypeSelect.Name = "rankTypeSelect";
            this.rankTypeSelect.Size = new System.Drawing.Size(202, 21);
            this.rankTypeSelect.TabIndex = 1;
            this.rankTypeSelect.SelectedIndexChanged += new System.EventHandler(this.rankTypeSelect_SelectedIndexChanged);
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
            this.sidePanel.Size = new System.Drawing.Size(240, 417);
            this.sidePanel.TabIndex = 9;
            this.sidePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sidePanel_Paint);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(129, 344);
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
            this.newButton.Location = new System.Drawing.Point(31, 344);
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
            this.headerPanel.Size = new System.Drawing.Size(729, 75);
            this.headerPanel.TabIndex = 8;
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
            this.label6.Text = "Soldier Career Length Probability Editor";
            // 
            // totalProbLabel
            // 
            this.totalProbLabel.ForeColor = System.Drawing.Color.Red;
            this.totalProbLabel.Location = new System.Drawing.Point(256, 32);
            this.totalProbLabel.Name = "totalProbLabel";
            this.totalProbLabel.Size = new System.Drawing.Size(32, 13);
            this.totalProbLabel.TabIndex = 29;
            this.totalProbLabel.Text = "0";
            this.totalProbLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(103, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Current Cumulative Probability:";
            // 
            // maxInput
            // 
            this.maxInput.Location = new System.Drawing.Point(237, 138);
            this.maxInput.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.maxInput.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.maxInput.Name = "maxInput";
            this.maxInput.Size = new System.Drawing.Size(95, 20);
            this.maxInput.TabIndex = 4;
            this.maxInput.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Maximum Career Length:";
            // 
            // minInput
            // 
            this.minInput.Location = new System.Drawing.Point(237, 100);
            this.minInput.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.minInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.minInput.Name = "minInput";
            this.minInput.Size = new System.Drawing.Size(95, 20);
            this.minInput.TabIndex = 3;
            this.minInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Minimum Career Length:";
            // 
            // probInput
            // 
            this.probInput.Location = new System.Drawing.Point(237, 60);
            this.probInput.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.probInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.probInput.Name = "probInput";
            this.probInput.Size = new System.Drawing.Size(95, 20);
            this.probInput.TabIndex = 2;
            this.probInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Career Length Probability:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(338, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "months";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(338, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "months";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(338, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(12, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "/";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(347, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "1000";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(288, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "/ 1000";
            // 
            // SpawnEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(729, 492);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SpawnEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Career Length Editor";
            this.mainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.sidePanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.maxInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox rankTypeSelect;
        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.Label totalProbLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown maxInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown minInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown probInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
    }
}