namespace Perscom
{
    partial class SoldierFilterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoldierFilterForm));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.methodSelectBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.operatorSelectBox = new System.Windows.Forms.ComboBox();
            this.selectorSelectBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.labelHeader = new System.Windows.Forms.ShadowLabel();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.methodSelectBox);
            this.mainPanel.Controls.Add(this.label4);
            this.mainPanel.Controls.Add(this.addButton);
            this.mainPanel.Controls.Add(this.numericUpDown1);
            this.mainPanel.Controls.Add(this.label3);
            this.mainPanel.Controls.Add(this.operatorSelectBox);
            this.mainPanel.Controls.Add(this.selectorSelectBox);
            this.mainPanel.Controls.Add(this.label2);
            this.mainPanel.Controls.Add(this.label1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(434, 201);
            this.mainPanel.TabIndex = 14;
            // 
            // methodSelectBox
            // 
            this.methodSelectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.methodSelectBox.FormattingEnabled = true;
            this.methodSelectBox.Location = new System.Drawing.Point(164, 28);
            this.methodSelectBox.Name = "methodSelectBox";
            this.methodSelectBox.Size = new System.Drawing.Size(202, 21);
            this.methodSelectBox.TabIndex = 26;
            this.methodSelectBox.SelectedIndexChanged += new System.EventHandler(this.methodSelectBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Method Selector: ";
            // 
            // addButton
            // 
            this.addButton.Image = global::Perscom.Properties.Resources.plus;
            this.addButton.Location = new System.Drawing.Point(372, 69);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(24, 24);
            this.addButton.TabIndex = 24;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(164, 152);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(79, 20);
            this.numericUpDown1.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(74, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Right Value: ";
            // 
            // operatorSelectBox
            // 
            this.operatorSelectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.operatorSelectBox.FormattingEnabled = true;
            this.operatorSelectBox.Location = new System.Drawing.Point(164, 112);
            this.operatorSelectBox.Name = "operatorSelectBox";
            this.operatorSelectBox.Size = new System.Drawing.Size(202, 21);
            this.operatorSelectBox.TabIndex = 21;
            // 
            // selectorSelectBox
            // 
            this.selectorSelectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectorSelectBox.FormattingEnabled = true;
            this.selectorSelectBox.Location = new System.Drawing.Point(164, 70);
            this.selectorSelectBox.Name = "selectorSelectBox";
            this.selectorSelectBox.Size = new System.Drawing.Size(202, 21);
            this.selectorSelectBox.TabIndex = 20;
            this.selectorSelectBox.SelectedIndexChanged += new System.EventHandler(this.selectorSelectBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Operator: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Left Value Selector: ";
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.saveButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 276);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(434, 60);
            this.bottomPanel.TabIndex = 13;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(137, 15);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(150, 30);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save Changes";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.headerPanel.BackgroundImage = global::Perscom.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.labelHeader);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(434, 75);
            this.headerPanel.TabIndex = 12;
            this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.headerPanel_Paint);
            // 
            // labelHeader
            // 
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.SystemColors.Control;
            this.labelHeader.Location = new System.Drawing.Point(26, 22);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.ShadowDirection = 90;
            this.labelHeader.ShadowOpacity = 225;
            this.labelHeader.ShadowSoftness = 3F;
            this.labelHeader.Size = new System.Drawing.Size(329, 37);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Apply Soldier Filtering";
            // 
            // SoldierFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(434, 336);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoldierFilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Soldier Filter Selection";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel labelHeader;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox operatorSelectBox;
        private System.Windows.Forms.ComboBox selectorSelectBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ComboBox methodSelectBox;
        private System.Windows.Forms.Label label4;
    }
}