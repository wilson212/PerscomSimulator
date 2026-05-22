namespace Perscom
{
    partial class RankSelectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RankSelectForm));
            bottomPanel = new System.Windows.Forms.Panel();
            saveButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.ShadowLabel();
            rankDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)saveButton).BeginInit();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)rankDropDownList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 227);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(372, 50);
            bottomPanel.TabIndex = 19;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // saveButton
            // 
            saveButton.Location = new System.Drawing.Point(131, 15);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(110, 24);
            saveButton.TabIndex = 0;
            saveButton.Text = "Select";
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
            headerPanel.Size = new System.Drawing.Size(372, 75);
            headerPanel.TabIndex = 18;
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
            label6.Size = new System.Drawing.Size(237, 37);
            label6.TabIndex = 0;
            label6.Text = "Next Rank Selector";
            // 
            // rankDropDownList
            // 
            rankDropDownList.Location = new System.Drawing.Point(44, 163);
            rankDropDownList.Name = "rankDropDownList";
            rankDropDownList.Size = new System.Drawing.Size(282, 24);
            rankDropDownList.TabIndex = 20;
            rankDropDownList.Text = "Select Rank";
            rankDropDownList.ThemeName = "Fluent";
            // 
            // radLabel1
            // 
            radLabel1.Location = new System.Drawing.Point(44, 102);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(181, 18);
            radLabel1.TabIndex = 21;
            radLabel1.Text = "Select Next Rank for <rank name>:";
            radLabel1.ThemeName = "Fluent";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(44, 131);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(154, 18);
            radLabel2.TabIndex = 22;
            radLabel2.Text = "Leave Blank for No Next Rank";
            radLabel2.ThemeName = "Fluent";
            // 
            // RankSelectForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(372, 277);
            Controls.Add(radLabel2);
            Controls.Add(radLabel1);
            Controls.Add(rankDropDownList);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "RankSelectForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Select Next Rank";
            ThemeName = "Fluent";
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)saveButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)rankDropDownList).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton saveButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private Telerik.WinControls.UI.RadDropDownList rankDropDownList;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
    }
}
