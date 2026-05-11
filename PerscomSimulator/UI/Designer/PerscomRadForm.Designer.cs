namespace Perscom
{
    partial class PerscomRadForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerscomRadForm));
            fluentTheme1 = new Telerik.WinControls.Themes.FluentTheme();
            bottomPanel = new System.Windows.Forms.Panel();
            headerPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 486);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(647, 50);
            bottomPanel.TabIndex = 22;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(647, 75);
            headerPanel.TabIndex = 21;
            // 
            // PerscomRadForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(647, 536);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "PerscomRadForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Perscom Base Form";
            ThemeName = "Fluent";
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.Themes.FluentTheme fluentTheme1;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel headerPanel;
    }
}
