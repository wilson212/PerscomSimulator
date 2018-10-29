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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.maxInput = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.minInput = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.probInput = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.mainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(464, 302);
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
            this.groupBox2.Location = new System.Drawing.Point(16, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(433, 264);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Details";
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
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(338, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(12, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "/";
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(338, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "months";
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
            this.maxInput.Enter += new System.EventHandler(this.maxInput_Enter);
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
            this.minInput.Enter += new System.EventHandler(this.minInput_Enter);
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
            this.probInput.Enter += new System.EventHandler(this.probInput_Enter);
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
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.label6);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(464, 75);
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
            // CareerLengthEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(464, 377);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "CareerLengthEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Career Length Editor";
            this.mainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.probInput)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
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
    }
}