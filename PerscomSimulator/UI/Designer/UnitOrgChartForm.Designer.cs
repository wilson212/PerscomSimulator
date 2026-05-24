namespace Perscom
{
    partial class UnitOrgChartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnitOrgChartForm));
            radDiagram = new Telerik.WinControls.UI.RadDiagram();
            bottomPanel = new System.Windows.Forms.Panel();
            exportButton = new Telerik.WinControls.UI.RadButton();
            closeButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new System.Windows.Forms.Panel();
            labelHeader = new System.Windows.Forms.ShadowLabel();
            ((System.ComponentModel.ISupportInitialize)radDiagram).BeginInit();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)exportButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)closeButton).BeginInit();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // radDiagram
            // 
            radDiagram.Dock = System.Windows.Forms.DockStyle.Fill;
            radDiagram.Location = new System.Drawing.Point(0, 0);
            radDiagram.Name = "radDiagram";
            radDiagram.Size = new System.Drawing.Size(1432, 967);
            radDiagram.TabIndex = 55;
            radDiagram.Text = "radDiagram1";
            radDiagram.ThemeName = "Fluent";
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(exportButton);
            bottomPanel.Controls.Add(closeButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 907);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(1432, 60);
            bottomPanel.TabIndex = 2;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // exportButton
            // 
            exportButton.Location = new System.Drawing.Point(41, 18);
            exportButton.Name = "exportButton";
            exportButton.Size = new System.Drawing.Size(150, 30);
            exportButton.TabIndex = 26;
            exportButton.Text = "Export to Image";
            exportButton.ThemeName = "Fluent";
            exportButton.Click += ExportButton_Click;
            // 
            // closeButton
            // 
            closeButton.Location = new System.Drawing.Point(1241, 18);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(150, 30);
            closeButton.TabIndex = 25;
            closeButton.Text = "Close";
            closeButton.ThemeName = "Fluent";
            closeButton.Click += CloseButton_Click;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(labelHeader);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(1432, 75);
            headerPanel.TabIndex = 1;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // labelHeader
            // 
            labelHeader.BackColor = System.Drawing.Color.Transparent;
            labelHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            labelHeader.ForeColor = System.Drawing.SystemColors.Control;
            labelHeader.Location = new System.Drawing.Point(26, 22);
            labelHeader.Name = "labelHeader";
            labelHeader.ShadowDirection = 60;
            labelHeader.ShadowOpacity = 180;
            labelHeader.ShadowSoftness = 3F;
            labelHeader.Size = new System.Drawing.Size(653, 37);
            labelHeader.TabIndex = 0;
            labelHeader.Text = "Organization Chart For - ";
            // 
            // UnitOrgChartForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(1432, 967);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            Controls.Add(radDiagram);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "UnitOrgChartForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "UnitOrgChartForm";
            ThemeName = "Fluent";
            Load += UnitOrgChartForm_Load;
            ((System.ComponentModel.ISupportInitialize)radDiagram).EndInit();
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)exportButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)closeButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadDiagram radDiagram;
        private System.Windows.Forms.Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton closeButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel labelHeader;
        private Telerik.WinControls.UI.RadButton exportButton;
    }
}
