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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.firstOrderDirection = new System.Windows.Forms.ComboBox();
            this.firstOrderBox = new System.Windows.Forms.ComboBox();
            this.existProbLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.existTrackBar = new System.Windows.Forms.TrackBar();
            this.careerGeneratorBox = new System.Windows.Forms.ComboBox();
            this.lockedCheckBox = new System.Windows.Forms.CheckBox();
            this.firstOrderCheckBox = new System.Windows.Forms.CheckBox();
            this.promotableCheckBox = new System.Windows.Forms.CheckBox();
            this.newCareerCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rankPicture = new System.Windows.Forms.PictureBox();
            this.rankSelect = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.ShadowLabel();
            this.thenOrderDirection = new System.Windows.Forms.ComboBox();
            this.thenOrderBox = new System.Windows.Forms.ComboBox();
            this.thenOrderCheckBox = new System.Windows.Forms.CheckBox();
            this.mainPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.existTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.groupBox3);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 75);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(512, 467);
            this.mainPanel.TabIndex = 11;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.thenOrderDirection);
            this.groupBox3.Controls.Add(this.thenOrderBox);
            this.groupBox3.Controls.Add(this.thenOrderCheckBox);
            this.groupBox3.Controls.Add(this.firstOrderDirection);
            this.groupBox3.Controls.Add(this.firstOrderBox);
            this.groupBox3.Controls.Add(this.existProbLabel);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.existTrackBar);
            this.groupBox3.Controls.Add(this.careerGeneratorBox);
            this.groupBox3.Controls.Add(this.lockedCheckBox);
            this.groupBox3.Controls.Add(this.firstOrderCheckBox);
            this.groupBox3.Controls.Add(this.promotableCheckBox);
            this.groupBox3.Controls.Add(this.newCareerCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(17, 141);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(475, 320);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // firstOrderDirection
            // 
            this.firstOrderDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.firstOrderDirection.Enabled = false;
            this.firstOrderDirection.FormattingEnabled = true;
            this.firstOrderDirection.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.firstOrderDirection.Location = new System.Drawing.Point(349, 169);
            this.firstOrderDirection.Name = "firstOrderDirection";
            this.firstOrderDirection.Size = new System.Drawing.Size(90, 21);
            this.firstOrderDirection.TabIndex = 48;
            // 
            // firstOrderBox
            // 
            this.firstOrderBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.firstOrderBox.Enabled = false;
            this.firstOrderBox.FormattingEnabled = true;
            this.firstOrderBox.Location = new System.Drawing.Point(172, 169);
            this.firstOrderBox.Name = "firstOrderBox";
            this.firstOrderBox.Size = new System.Drawing.Size(160, 21);
            this.firstOrderBox.TabIndex = 47;
            this.firstOrderBox.SelectedIndexChanged += new System.EventHandler(this.firstOrderBox_SelectedIndexChanged);
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
            this.lockedCheckBox.Location = new System.Drawing.Point(35, 276);
            this.lockedCheckBox.Name = "lockedCheckBox";
            this.lockedCheckBox.Size = new System.Drawing.Size(177, 17);
            this.lockedCheckBox.TabIndex = 42;
            this.lockedCheckBox.Text = "Must Not Be Locked In To Billet";
            this.lockedCheckBox.UseVisualStyleBackColor = true;
            // 
            // firstOrderCheckBox
            // 
            this.firstOrderCheckBox.AutoSize = true;
            this.firstOrderCheckBox.Location = new System.Drawing.Point(35, 169);
            this.firstOrderCheckBox.Name = "firstOrderCheckBox";
            this.firstOrderCheckBox.Size = new System.Drawing.Size(92, 17);
            this.firstOrderCheckBox.TabIndex = 41;
            this.firstOrderCheckBox.Text = "First Order By:";
            this.firstOrderCheckBox.UseVisualStyleBackColor = true;
            this.firstOrderCheckBox.CheckedChanged += new System.EventHandler(this.firstOrderCheckBox_CheckedChanged);
            // 
            // promotableCheckBox
            // 
            this.promotableCheckBox.AutoSize = true;
            this.promotableCheckBox.Location = new System.Drawing.Point(35, 253);
            this.promotableCheckBox.Name = "promotableCheckBox";
            this.promotableCheckBox.Size = new System.Drawing.Size(121, 17);
            this.promotableCheckBox.TabIndex = 40;
            this.promotableCheckBox.Text = "Must Be Promotable";
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
            // rankPicture
            // 
            this.rankPicture.Location = new System.Drawing.Point(20, 27);
            this.rankPicture.Name = "rankPicture";
            this.rankPicture.Size = new System.Drawing.Size(64, 64);
            this.rankPicture.TabIndex = 14;
            this.rankPicture.TabStop = false;
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
            this.bottomPanel.Location = new System.Drawing.Point(0, 542);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(512, 60);
            this.bottomPanel.TabIndex = 10;
            this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(179, 18);
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
            this.headerPanel.Controls.Add(this.label6);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(512, 75);
            this.headerPanel.TabIndex = 9;
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
            this.label6.Size = new System.Drawing.Size(466, 37);
            this.label6.TabIndex = 0;
            this.label6.Text = "From Existing Soldier Pool";
            // 
            // thenOrderDirection
            // 
            this.thenOrderDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.thenOrderDirection.Enabled = false;
            this.thenOrderDirection.FormattingEnabled = true;
            this.thenOrderDirection.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.thenOrderDirection.Location = new System.Drawing.Point(349, 209);
            this.thenOrderDirection.Name = "thenOrderDirection";
            this.thenOrderDirection.Size = new System.Drawing.Size(90, 21);
            this.thenOrderDirection.TabIndex = 51;
            // 
            // thenOrderBox
            // 
            this.thenOrderBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.thenOrderBox.Enabled = false;
            this.thenOrderBox.FormattingEnabled = true;
            this.thenOrderBox.Location = new System.Drawing.Point(172, 209);
            this.thenOrderBox.Name = "thenOrderBox";
            this.thenOrderBox.Size = new System.Drawing.Size(160, 21);
            this.thenOrderBox.TabIndex = 50;
            // 
            // thenOrderCheckBox
            // 
            this.thenOrderCheckBox.AutoSize = true;
            this.thenOrderCheckBox.Location = new System.Drawing.Point(35, 209);
            this.thenOrderCheckBox.Name = "thenOrderCheckBox";
            this.thenOrderCheckBox.Size = new System.Drawing.Size(98, 17);
            this.thenOrderCheckBox.TabIndex = 49;
            this.thenOrderCheckBox.Text = "Then Order By:";
            this.thenOrderCheckBox.UseVisualStyleBackColor = true;
            this.thenOrderCheckBox.CheckedChanged += new System.EventHandler(this.thenOrderCheckBox_CheckedChanged);
            // 
            // SoldierGeneratorPoolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(512, 602);
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
            ((System.ComponentModel.ISupportInitialize)(this.rankPicture)).EndInit();
            this.bottomPanel.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox firstOrderCheckBox;
        private System.Windows.Forms.CheckBox promotableCheckBox;
        private System.Windows.Forms.CheckBox newCareerCheckBox;
        private System.Windows.Forms.ComboBox rankSelect;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label existProbLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar existTrackBar;
        private System.Windows.Forms.ComboBox firstOrderDirection;
        private System.Windows.Forms.ComboBox firstOrderBox;
        private System.Windows.Forms.ComboBox thenOrderDirection;
        private System.Windows.Forms.ComboBox thenOrderBox;
        private System.Windows.Forms.CheckBox thenOrderCheckBox;
    }
}