namespace Perscom
{
    partial class SoldierSortingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoldierSortingForm));
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.methodSelectBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.selectorSelectBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sortingDirectionBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.labelHeader = new System.Windows.Forms.ShadowLabel();
            this.bottomPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bottomPanel.Controls.Add(this.saveButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 166);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(434, 60);
            this.bottomPanel.TabIndex = 18;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(142, 15);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(150, 30);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save Changes";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.methodSelectBox);
            this.mainPanel.Controls.Add(this.label4);
            this.mainPanel.Controls.Add(this.addButton);
            this.mainPanel.Controls.Add(this.selectorSelectBox);
            this.mainPanel.Controls.Add(this.label1);
            this.mainPanel.Controls.Add(this.sortingDirectionBox);
            this.mainPanel.Controls.Add(this.label3);
            this.mainPanel.Controls.Add(this.bottomPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(434, 226);
            this.mainPanel.TabIndex = 16;
            // 
            // methodSelectBox
            // 
            this.methodSelectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.methodSelectBox.FormattingEnabled = true;
            this.methodSelectBox.Location = new System.Drawing.Point(164, 32);
            this.methodSelectBox.Name = "methodSelectBox";
            this.methodSelectBox.Size = new System.Drawing.Size(202, 21);
            this.methodSelectBox.TabIndex = 32;
            this.methodSelectBox.SelectedIndexChanged += new System.EventHandler(this.methodSelectBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Method Selector: ";
            // 
            // addButton
            // 
            this.addButton.Image = global::Perscom.Properties.Resources.plus;
            this.addButton.Location = new System.Drawing.Point(372, 73);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(24, 24);
            this.addButton.TabIndex = 30;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // selectorSelectBox
            // 
            this.selectorSelectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectorSelectBox.FormattingEnabled = true;
            this.selectorSelectBox.Location = new System.Drawing.Point(164, 74);
            this.selectorSelectBox.Name = "selectorSelectBox";
            this.selectorSelectBox.Size = new System.Drawing.Size(202, 21);
            this.selectorSelectBox.TabIndex = 29;
            this.selectorSelectBox.SelectedIndexChanged += new System.EventHandler(this.selectorSelectBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Left Value Selector: ";
            // 
            // sortingDirectionBox
            // 
            this.sortingDirectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortingDirectionBox.FormattingEnabled = true;
            this.sortingDirectionBox.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.sortingDirectionBox.Location = new System.Drawing.Point(166, 116);
            this.sortingDirectionBox.Name = "sortingDirectionBox";
            this.sortingDirectionBox.Size = new System.Drawing.Size(202, 21);
            this.sortingDirectionBox.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Sorting Direction:";
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
            this.headerPanel.TabIndex = 15;
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
            this.labelHeader.Size = new System.Drawing.Size(295, 37);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Apply Soldier Sorting";
            // 
            // SoldierSortingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(434, 301);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoldierSortingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Soldier Sorting Selection";
            this.bottomPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel labelHeader;
        private System.Windows.Forms.ComboBox sortingDirectionBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox methodSelectBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ComboBox selectorSelectBox;
        private System.Windows.Forms.Label label1;
    }
}