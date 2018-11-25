namespace Perscom
{
    partial class CareerLengthEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CareerLengthEditorForm));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.labelMaxLength = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rangeInput = new System.Windows.Forms.NumericUpDown();
            this.minInput = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.probInput = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.applyButton = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rangeInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.labelMaxLength);
            this.mainPanel.Controls.Add(this.label8);
            this.mainPanel.Controls.Add(this.label3);
            this.mainPanel.Controls.Add(this.label7);
            this.mainPanel.Controls.Add(this.label10);
            this.mainPanel.Controls.Add(this.label9);
            this.mainPanel.Controls.Add(this.label5);
            this.mainPanel.Controls.Add(this.label4);
            this.mainPanel.Controls.Add(this.rangeInput);
            this.mainPanel.Controls.Add(this.minInput);
            this.mainPanel.Controls.Add(this.label2);
            this.mainPanel.Controls.Add(this.probInput);
            this.mainPanel.Controls.Add(this.label1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(434, 255);
            this.mainPanel.TabIndex = 10;
            // 
            // labelMaxLength
            // 
            this.labelMaxLength.AutoSize = true;
            this.labelMaxLength.Location = new System.Drawing.Point(224, 143);
            this.labelMaxLength.Name = "labelMaxLength";
            this.labelMaxLength.Size = new System.Drawing.Size(50, 13);
            this.labelMaxLength.TabIndex = 37;
            this.labelMaxLength.Text = "0 months";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(72, 143);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(122, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Maxmum Career Length:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(82, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Career Length Range:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(212, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 34;
            this.label7.Text = "+";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(337, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 33;
            this.label10.Text = "1000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(328, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(12, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "/";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(328, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "months";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(328, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "months";
            // 
            // rangeInput
            // 
            this.rangeInput.Increment = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.rangeInput.Location = new System.Drawing.Point(227, 103);
            this.rangeInput.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.rangeInput.Name = "rangeInput";
            this.rangeInput.Size = new System.Drawing.Size(95, 20);
            this.rangeInput.TabIndex = 27;
            this.rangeInput.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.rangeInput.ValueChanged += new System.EventHandler(this.maxInput_ValueChanged);
            this.rangeInput.Enter += new System.EventHandler(this.rangeInput_Enter);
            // 
            // minInput
            // 
            this.minInput.Location = new System.Drawing.Point(227, 65);
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
            this.minInput.TabIndex = 26;
            this.minInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.minInput.Enter += new System.EventHandler(this.minInput_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Minimum Career Length:";
            // 
            // probInput
            // 
            this.probInput.Location = new System.Drawing.Point(227, 27);
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
            this.probInput.TabIndex = 25;
            this.probInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.probInput.Enter += new System.EventHandler(this.probInput_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Career Length Probability:";
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.label6);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(434, 75);
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
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.applyButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 270);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(434, 60);
            this.bottomPanel.TabIndex = 14;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // applyButton
            // 
            this.applyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applyButton.Location = new System.Drawing.Point(142, 18);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(150, 30);
            this.applyButton.TabIndex = 0;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // CareerLengthEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(434, 330);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CareerLengthEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Career Length Editor";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rangeInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label labelMaxLength;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown rangeInput;
        private System.Windows.Forms.NumericUpDown minInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown probInput;
        private System.Windows.Forms.Label label1;
    }
}