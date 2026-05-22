namespace Perscom
{
    partial class GradedAttributeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GradedAttributeForm));
            bottomPanel = new System.Windows.Forms.Panel();
            saveButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.ShadowLabel();
            attrDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            scoreSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            expLvlSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel3 = new Telerik.WinControls.UI.RadLabel();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)saveButton).BeginInit();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)attrDropDownList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)scoreSpinEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)expLvlSpinEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 261);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(390, 50);
            bottomPanel.TabIndex = 17;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // saveButton
            // 
            saveButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            saveButton.Location = new System.Drawing.Point(120, 11);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(150, 28);
            saveButton.TabIndex = 91;
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
            headerPanel.Size = new System.Drawing.Size(390, 75);
            headerPanel.TabIndex = 16;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // headerLabel
            // 
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.SystemColors.Control;
            label6.Location = new System.Drawing.Point(26, 22);
            label6.Name = "label6";
            label6.ShadowDirection = 60;
            label6.ShadowOpacity = 180;
            label6.ShadowSoftness = 3F;
            label6.Size = new System.Drawing.Size(411, 37);
            label6.TabIndex = 0;
            label6.Text = "Attribute Weight Selection";
            // 
            // attrDropDownList
            // 
            attrDropDownList.Location = new System.Drawing.Point(120, 126);
            attrDropDownList.Name = "attrDropDownList";
            attrDropDownList.Size = new System.Drawing.Size(238, 24);
            attrDropDownList.TabIndex = 18;
            attrDropDownList.Text = "Select Attribute";
            attrDropDownList.ThemeName = "Fluent";
            // 
            // radLabel1
            // 
            radLabel1.Location = new System.Drawing.Point(52, 128);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(53, 18);
            radLabel1.TabIndex = 19;
            radLabel1.Text = "Attribute:";
            radLabel1.ThemeName = "Fluent";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(41, 211);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(64, 18);
            radLabel2.TabIndex = 20;
            radLabel2.Text = "Max Points:";
            radLabel2.ThemeName = "Fluent";
            // 
            // scoreSpinEditor
            // 
            scoreSpinEditor.Location = new System.Drawing.Point(120, 210);
            scoreSpinEditor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            scoreSpinEditor.Name = "scoreSpinEditor";
            scoreSpinEditor.NullableValue = new decimal(new int[] { 1, 0, 0, 0 });
            scoreSpinEditor.Size = new System.Drawing.Size(98, 24);
            scoreSpinEditor.TabIndex = 21;
            scoreSpinEditor.ThemeName = "Fluent";
            scoreSpinEditor.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // expLvlSpinEditor
            // 
            expLvlSpinEditor.Location = new System.Drawing.Point(120, 165);
            expLvlSpinEditor.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            expLvlSpinEditor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            expLvlSpinEditor.Name = "expLvlSpinEditor";
            expLvlSpinEditor.NullableValue = new decimal(new int[] { 10, 0, 0, 0 });
            expLvlSpinEditor.Size = new System.Drawing.Size(98, 24);
            expLvlSpinEditor.TabIndex = 23;
            expLvlSpinEditor.ThemeName = "Fluent";
            expLvlSpinEditor.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // radLabel3
            // 
            radLabel3.Location = new System.Drawing.Point(23, 168);
            radLabel3.Name = "radLabel3";
            radLabel3.Size = new System.Drawing.Size(82, 18);
            radLabel3.TabIndex = 22;
            radLabel3.Text = "Expected Level:";
            radLabel3.ThemeName = "Fluent";
            // 
            // GradedAttributeForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(390, 311);
            Controls.Add(expLvlSpinEditor);
            Controls.Add(radLabel3);
            Controls.Add(scoreSpinEditor);
            Controls.Add(radLabel2);
            Controls.Add(radLabel1);
            Controls.Add(attrDropDownList);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GradedAttributeForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Graded Attribute";
            ThemeName = "Fluent";
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)saveButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)attrDropDownList).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)scoreSpinEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)expLvlSpinEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton saveButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private Telerik.WinControls.UI.RadDropDownList attrDropDownList;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadSpinEditor scoreSpinEditor;
        private Telerik.WinControls.UI.RadSpinEditor expLvlSpinEditor;
        private Telerik.WinControls.UI.RadLabel radLabel3;
    }
}
