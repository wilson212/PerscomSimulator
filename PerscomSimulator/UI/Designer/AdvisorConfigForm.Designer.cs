namespace Perscom
{
    partial class AdvisorConfigForm
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
            bottomPanel = new System.Windows.Forms.Panel();
            saveButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.ShadowLabel();
            modelDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            keyTextBox = new Telerik.WinControls.UI.RadTextBox();
            radLabel3 = new Telerik.WinControls.UI.RadLabel();
            radLabel4 = new Telerik.WinControls.UI.RadLabel();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)saveButton).BeginInit();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)modelDropDownList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)keyTextBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 269);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(429, 50);
            bottomPanel.TabIndex = 24;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // saveButton
            // 
            saveButton.Location = new System.Drawing.Point(159, 13);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(110, 24);
            saveButton.TabIndex = 0;
            saveButton.Text = "Save";
            saveButton.ThemeName = "Fluent";
            saveButton.Click += saveButton_Click;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(label6);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(429, 75);
            headerPanel.TabIndex = 23;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // headerLabel
            // 
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.SystemColors.Control;
            label6.Location = new System.Drawing.Point(12, 20);
            label6.Name = "label6";
            label6.ShadowDirection = 60;
            label6.ShadowOpacity = 180;
            label6.ShadowSoftness = 3F;
            label6.Size = new System.Drawing.Size(346, 37);
            label6.TabIndex = 3;
            label6.Text = "Virtual Assistant Configuration";
            // 
            // modelDropDownList
            // 
            modelDropDownList.Location = new System.Drawing.Point(122, 166);
            modelDropDownList.Name = "modelDropDownList";
            modelDropDownList.Size = new System.Drawing.Size(288, 24);
            modelDropDownList.TabIndex = 25;
            modelDropDownList.ThemeName = "Fluent";
            // 
            // radLabel1
            // 
            radLabel1.Location = new System.Drawing.Point(23, 168);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(82, 18);
            radLabel1.TabIndex = 26;
            radLabel1.Text = "Gemeni Model:";
            radLabel1.ThemeName = "Fluent";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(23, 216);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(88, 18);
            radLabel2.TabIndex = 27;
            radLabel2.Text = "Gemeni API Key:";
            radLabel2.ThemeName = "Fluent";
            // 
            // keyTextBox
            // 
            keyTextBox.Location = new System.Drawing.Point(122, 213);
            keyTextBox.Name = "keyTextBox";
            keyTextBox.Size = new System.Drawing.Size(288, 24);
            keyTextBox.TabIndex = 28;
            keyTextBox.ThemeName = "Fluent";
            // 
            // radLabel3
            // 
            radLabel3.Location = new System.Drawing.Point(26, 98);
            radLabel3.Name = "radLabel3";
            radLabel3.Size = new System.Drawing.Size(376, 18);
            radLabel3.TabIndex = 29;
            radLabel3.Text = "Please enter your Google API model and API Key. These can be aquired at:";
            // 
            // radLabel4
            // 
            radLabel4.Location = new System.Drawing.Point(26, 122);
            radLabel4.Name = "radLabel4";
            radLabel4.Size = new System.Drawing.Size(214, 18);
            radLabel4.TabIndex = 30;
            radLabel4.Text = "https://aistudio.google.com/app/api-keys";
            radLabel4.ThemeName = "Fluent";
            // 
            // AdvisorConfigForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(429, 319);
            Controls.Add(radLabel4);
            Controls.Add(radLabel3);
            Controls.Add(keyTextBox);
            Controls.Add(radLabel2);
            Controls.Add(radLabel1);
            Controls.Add(modelDropDownList);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            MaximizeBox = false;
            Name = "AdvisorConfigForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Advisor Configuration";
            ThemeName = "Fluent";
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)saveButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)modelDropDownList).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)keyTextBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel4).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel headerPanel;
        private Telerik.WinControls.UI.RadDropDownList modelDropDownList;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadTextBox keyTextBox;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadButton saveButton;
        private System.Windows.Forms.ShadowLabel label6;
    }
}
