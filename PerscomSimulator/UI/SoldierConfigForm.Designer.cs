namespace Perscom
{
    partial class SoldierConfigForm
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.reloadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.editRadio = new System.Windows.Forms.RadioButton();
            this.createRadio = new System.Windows.Forms.RadioButton();
            this.totalProbLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.maxInput = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.minInput = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.probInput = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.rankTypeBox = new System.Windows.Forms.ComboBox();
            this.headerPanel.SuspendLayout();
            this.sidePanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).BeginInit();
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
            this.headerPanel.Size = new System.Drawing.Size(684, 75);
            this.headerPanel.TabIndex = 1;
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
            this.label6.Text = "Soldier Spawn Settings";
            // 
            // sidePanel
            // 
            this.sidePanel.Controls.Add(this.treeView1);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 75);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Padding = new System.Windows.Forms.Padding(5, 10, 0, 0);
            this.sidePanel.Size = new System.Drawing.Size(240, 377);
            this.sidePanel.TabIndex = 3;
            this.sidePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Location = new System.Drawing.Point(10, 7);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(214, 314);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.reloadButton);
            this.bottomPanel.Controls.Add(this.saveButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 452);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(684, 60);
            this.bottomPanel.TabIndex = 4;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // reloadButton
            // 
            this.reloadButton.Location = new System.Drawing.Point(31, 18);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(150, 30);
            this.reloadButton.TabIndex = 1;
            this.reloadButton.Text = "Reload Settings";
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(511, 18);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(150, 30);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save Changes";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Controls.Add(this.totalProbLabel);
            this.mainPanel.Controls.Add(this.label4);
            this.mainPanel.Controls.Add(this.maxInput);
            this.mainPanel.Controls.Add(this.label3);
            this.mainPanel.Controls.Add(this.minInput);
            this.mainPanel.Controls.Add(this.label2);
            this.mainPanel.Controls.Add(this.addButton);
            this.mainPanel.Controls.Add(this.probInput);
            this.mainPanel.Controls.Add(this.label1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(240, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(444, 377);
            this.mainPanel.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rankTypeBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(72, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 50);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Soldier Type:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.editRadio);
            this.groupBox1.Controls.Add(this.createRadio);
            this.groupBox1.Location = new System.Drawing.Point(72, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 50);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Current Mode:";
            // 
            // editRadio
            // 
            this.editRadio.AutoSize = true;
            this.editRadio.Location = new System.Drawing.Point(225, 17);
            this.editRadio.Name = "editRadio";
            this.editRadio.Size = new System.Drawing.Size(43, 17);
            this.editRadio.TabIndex = 16;
            this.editRadio.Text = "Edit";
            this.editRadio.UseVisualStyleBackColor = true;
            // 
            // createRadio
            // 
            this.createRadio.AutoSize = true;
            this.createRadio.Checked = true;
            this.createRadio.Location = new System.Drawing.Point(141, 17);
            this.createRadio.Name = "createRadio";
            this.createRadio.Size = new System.Drawing.Size(56, 17);
            this.createRadio.TabIndex = 15;
            this.createRadio.TabStop = true;
            this.createRadio.Text = "Create";
            this.createRadio.UseVisualStyleBackColor = true;
            this.createRadio.CheckedChanged += new System.EventHandler(this.createRadio_CheckedChanged);
            // 
            // totalProbLabel
            // 
            this.totalProbLabel.AutoSize = true;
            this.totalProbLabel.Location = new System.Drawing.Point(293, 28);
            this.totalProbLabel.Name = "totalProbLabel";
            this.totalProbLabel.Size = new System.Drawing.Size(13, 13);
            this.totalProbLabel.TabIndex = 8;
            this.totalProbLabel.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(138, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Cumulative Spawn Probability:";
            // 
            // maxInput
            // 
            this.maxInput.Location = new System.Drawing.Point(267, 272);
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
            this.maxInput.TabIndex = 6;
            this.maxInput.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.maxInput.ValueChanged += new System.EventHandler(this.numericInput_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(83, 274);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Maximum Time to Live:";
            // 
            // minInput
            // 
            this.minInput.Location = new System.Drawing.Point(267, 234);
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
            this.minInput.TabIndex = 4;
            this.minInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.minInput.ValueChanged += new System.EventHandler(this.numericInput_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(83, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Minimum Time to Live:";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(147, 325);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(150, 30);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add Soldier";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // probInput
            // 
            this.probInput.Location = new System.Drawing.Point(267, 194);
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
            this.probInput.TabIndex = 1;
            this.probInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.probInput.ValueChanged += new System.EventHandler(this.numericInput_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Soldier Spawn Probability:";
            // 
            // rankTypeBox
            // 
            this.rankTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankTypeBox.FormattingEnabled = true;
            this.rankTypeBox.Location = new System.Drawing.Point(100, 15);
            this.rankTypeBox.Name = "rankTypeBox";
            this.rankTypeBox.Size = new System.Drawing.Size(174, 21);
            this.rankTypeBox.TabIndex = 15;
            this.rankTypeBox.SelectedIndexChanged += new System.EventHandler(this.rankTypeBox_SelectedIndexChanged);
            // 
            // SoldierConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(684, 512);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoldierConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Soldier Config";
            this.headerPanel.ResumeLayout(false);
            this.sidePanel.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label totalProbLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.NumericUpDown maxInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown minInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.NumericUpDown probInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton editRadio;
        private System.Windows.Forms.RadioButton createRadio;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.ComboBox rankTypeBox;
    }
}