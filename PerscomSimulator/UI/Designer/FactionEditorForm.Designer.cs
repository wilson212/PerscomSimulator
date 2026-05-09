namespace Perscom
{
    partial class FactionEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FactionEditorForm));
            fluentTheme1 = new Telerik.WinControls.Themes.FluentTheme();
            label6 = new System.Windows.Forms.ShadowLabel();
            headerPanel = new System.Windows.Forms.Panel();
            bottomPanel = new System.Windows.Forms.Panel();
            CloseButton = new Telerik.WinControls.UI.RadButton();
            radTreeView1 = new Telerik.WinControls.UI.RadTreeView();
            radLabel4 = new Telerik.WinControls.UI.RadLabel();
            radPanel2 = new Telerik.WinControls.UI.RadPanel();
            commandBarRowElement1 = new Telerik.WinControls.UI.CommandBarRowElement();
            commandBarStripElement1 = new Telerik.WinControls.UI.CommandBarStripElement();
            commandBarButton1 = new Telerik.WinControls.UI.CommandBarButton();
            commandBarButton2 = new Telerik.WinControls.UI.CommandBarButton();
            commandBarButton3 = new Telerik.WinControls.UI.CommandBarButton();
            radPanel3 = new Telerik.WinControls.UI.RadPanel();
            radPanorama1 = new Telerik.WinControls.UI.RadPanorama();
            tileGroupElement1 = new Telerik.WinControls.UI.TileGroupElement();
            RankTileElement = new Telerik.WinControls.UI.RadTileElement();
            TraitsTileElement = new Telerik.WinControls.UI.RadTileElement();
            OccupationTileElement = new Telerik.WinControls.UI.RadTileElement();
            ExperienceTileElement = new Telerik.WinControls.UI.RadTileElement();
            radPanel1 = new Telerik.WinControls.UI.RadPanel();
            radPanel4 = new Telerik.WinControls.UI.RadPanel();
            radButton1 = new Telerik.WinControls.UI.RadButton();
            radButton2 = new Telerik.WinControls.UI.RadButton();
            radCommandBar1 = new Telerik.WinControls.UI.RadCommandBar();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radTextBox1 = new Telerik.WinControls.UI.RadTextBox();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            radPictureBox1 = new Telerik.WinControls.UI.RadPictureBox();
            headerPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CloseButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).BeginInit();
            radTreeView1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radLabel4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radPanel2).BeginInit();
            radPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radPanel3).BeginInit();
            radPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radPanorama1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radPanel1).BeginInit();
            radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radPanel4).BeginInit();
            radPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radButton1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radButton2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radCommandBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radTextBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radPictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // label6
            // 
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.SystemColors.Control;
            label6.Location = new System.Drawing.Point(26, 22);
            label6.Name = "label6";
            label6.ShadowDirection = 90;
            label6.ShadowOpacity = 225;
            label6.ShadowSoftness = 3F;
            label6.Size = new System.Drawing.Size(717, 37);
            label6.TabIndex = 0;
            label6.Text = "Faction Editor";
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(label6);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(1244, 75);
            headerPanel.TabIndex = 22;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.Controls.Add(CloseButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 836);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(1244, 50);
            bottomPanel.TabIndex = 23;
            // 
            // CloseButton
            // 
            CloseButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            CloseButton.Location = new System.Drawing.Point(547, 11);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new System.Drawing.Size(150, 28);
            CloseButton.TabIndex = 91;
            CloseButton.Text = "Close";
            CloseButton.ThemeName = "Fluent";
            // 
            // radTreeView1
            // 
            radTreeView1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            radTreeView1.Controls.Add(radLabel4);
            radTreeView1.Dock = System.Windows.Forms.DockStyle.Left;
            radTreeView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            radTreeView1.ForeColor = System.Drawing.Color.Black;
            radTreeView1.ItemHeight = 28;
            radTreeView1.LineColor = System.Drawing.Color.FromArgb(204, 204, 204);
            radTreeView1.LineStyle = Telerik.WinControls.UI.TreeLineStyle.Solid;
            radTreeView1.Location = new System.Drawing.Point(0, 0);
            radTreeView1.Name = "radTreeView1";
            radTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            radTreeView1.Size = new System.Drawing.Size(220, 730);
            radTreeView1.TabIndex = 16;
            radTreeView1.ThemeName = "Fluent";
            // 
            // radLabel4
            // 
            radLabel4.Location = new System.Drawing.Point(26, 693);
            radLabel4.Name = "radLabel4";
            radLabel4.Size = new System.Drawing.Size(171, 18);
            radLabel4.TabIndex = 17;
            radLabel4.Text = "Right click to open context menu";
            // 
            // radPanel2
            // 
            radPanel2.Controls.Add(radTreeView1);
            radPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            radPanel2.Location = new System.Drawing.Point(0, 106);
            radPanel2.Name = "radPanel2";
            radPanel2.Size = new System.Drawing.Size(226, 730);
            radPanel2.TabIndex = 24;
            radPanel2.ThemeName = "Fluent";
            // 
            // commandBarRowElement1
            // 
            commandBarRowElement1.MinSize = new System.Drawing.Size(25, 25);
            commandBarRowElement1.Name = "commandBarRowElement1";
            commandBarRowElement1.Strips.AddRange(new Telerik.WinControls.UI.CommandBarStripElement[] { commandBarStripElement1 });
            // 
            // commandBarStripElement1
            // 
            commandBarStripElement1.DisplayName = "commandBarStripElement1";
            commandBarStripElement1.Items.AddRange(new Telerik.WinControls.UI.RadCommandBarBaseItem[] { commandBarButton1, commandBarButton2, commandBarButton3 });
            commandBarStripElement1.Name = "commandBarStripElement1";
            // 
            // commandBarButton1
            // 
            commandBarButton1.DisplayName = "commandBarButton1";
            commandBarButton1.DrawText = true;
            commandBarButton1.Image = Properties.Resources.pencil;
            commandBarButton1.Name = "commandBarButton1";
            commandBarButton1.Text = "Save Changes";
            commandBarButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // commandBarButton2
            // 
            commandBarButton2.DisplayName = "commandBarButton2";
            commandBarButton2.DrawText = true;
            commandBarButton2.Image = Properties.Resources.plus;
            commandBarButton2.ImageAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            commandBarButton2.Name = "commandBarButton2";
            commandBarButton2.Text = "Add New Faction";
            commandBarButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // commandBarButton3
            // 
            commandBarButton3.DisplayName = "delFaction";
            commandBarButton3.DrawText = true;
            commandBarButton3.Image = (System.Drawing.Image)resources.GetObject("commandBarButton3.Image");
            commandBarButton3.Name = "commandBarButton3";
            commandBarButton3.Text = "Delete Faction";
            commandBarButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // radPanel3
            // 
            radPanel3.Controls.Add(radPictureBox1);
            radPanel3.Location = new System.Drawing.Point(830, 371);
            radPanel3.Name = "radPanel3";
            radPanel3.Size = new System.Drawing.Size(400, 345);
            radPanel3.TabIndex = 27;
            radPanel3.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            radPanel3.ThemeName = "Fluent";
            // 
            // radPanorama1
            // 
            radPanorama1.BackColor = System.Drawing.Color.White;
            radPanorama1.Groups.AddRange(new Telerik.WinControls.RadItem[] { tileGroupElement1 });
            radPanorama1.Location = new System.Drawing.Point(7, 12);
            radPanorama1.Name = "radPanorama1";
            radPanorama1.RowsCount = 3;
            radPanorama1.ShowGroups = true;
            radPanorama1.Size = new System.Drawing.Size(964, 198);
            radPanorama1.TabIndex = 28;
            radPanorama1.ThemeName = "Fluent";
            // 
            // tileGroupElement1
            // 
            tileGroupElement1.AutoSize = false;
            tileGroupElement1.BackColor2 = System.Drawing.Color.White;
            tileGroupElement1.Bounds = new System.Drawing.Rectangle(0, 0, 940, 200);
            tileGroupElement1.CellSize = new System.Drawing.Size(235, 160);
            tileGroupElement1.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            tileGroupElement1.Items.AddRange(new Telerik.WinControls.RadItem[] { RankTileElement, TraitsTileElement, OccupationTileElement, ExperienceTileElement });
            tileGroupElement1.Margin = new System.Windows.Forms.Padding(10, 2, 10, 10);
            tileGroupElement1.Name = "tileGroupElement1";
            tileGroupElement1.StretchHorizontally = false;
            tileGroupElement1.StretchVertically = false;
            tileGroupElement1.Text = "Faction Specific Structure";
            tileGroupElement1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            tileGroupElement1.UseCompatibleTextRendering = false;
            // 
            // RankTileElement
            // 
            RankTileElement.BackgroundImage = Properties.Resources.tile_1;
            RankTileElement.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            RankTileElement.Name = "RankTileElement";
            RankTileElement.Text = "Manage Ranks";
            RankTileElement.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            RankTileElement.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            RankTileElement.UseCompatibleTextRendering = false;
            RankTileElement.Click += RankTileElement_Click;
            // 
            // TraitsTileElement
            // 
            TraitsTileElement.BackgroundImage = Properties.Resources.tile_2;
            TraitsTileElement.Column = 2;
            TraitsTileElement.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            TraitsTileElement.Name = "TraitsTileElement";
            TraitsTileElement.Text = "Edit Roles";
            TraitsTileElement.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            TraitsTileElement.UseCompatibleTextRendering = false;
            // 
            // OccupationTileElement
            // 
            OccupationTileElement.BackgroundImage = Properties.Resources.tile_2;
            OccupationTileElement.Column = 1;
            OccupationTileElement.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            OccupationTileElement.Name = "OccupationTileElement";
            OccupationTileElement.Text = "Unit Designer";
            OccupationTileElement.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            OccupationTileElement.UseCompatibleTextRendering = false;
            // 
            // ExperienceTileElement
            // 
            ExperienceTileElement.BackgroundImage = Properties.Resources.tile_3;
            ExperienceTileElement.Column = 3;
            ExperienceTileElement.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            ExperienceTileElement.Name = "ExperienceTileElement";
            ExperienceTileElement.Text = "Evaluation Boards";
            ExperienceTileElement.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            ExperienceTileElement.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            ExperienceTileElement.UseCompatibleTextRendering = false;
            // 
            // radPanel1
            // 
            radPanel1.Controls.Add(radPanorama1);
            radPanel1.Location = new System.Drawing.Point(252, 118);
            radPanel1.Name = "radPanel1";
            radPanel1.Size = new System.Drawing.Size(978, 232);
            radPanel1.TabIndex = 29;
            radPanel1.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            radPanel1.ThemeName = "Fluent";
            // 
            // radPanel4
            // 
            radPanel4.Controls.Add(radLabel2);
            radPanel4.Controls.Add(radTextBox1);
            radPanel4.Controls.Add(radLabel1);
            radPanel4.Location = new System.Drawing.Point(252, 371);
            radPanel4.Name = "radPanel4";
            radPanel4.Size = new System.Drawing.Size(560, 345);
            radPanel4.TabIndex = 30;
            radPanel4.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            radPanel4.ThemeName = "Fluent";
            // 
            // radButton1
            // 
            radButton1.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            radButton1.Location = new System.Drawing.Point(571, 763);
            radButton1.Name = "radButton1";
            radButton1.Size = new System.Drawing.Size(150, 28);
            radButton1.TabIndex = 92;
            radButton1.Text = "Cancel";
            radButton1.ThemeName = "Fluent";
            // 
            // radButton2
            // 
            radButton2.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            radButton2.Location = new System.Drawing.Point(748, 763);
            radButton2.Name = "radButton2";
            radButton2.Size = new System.Drawing.Size(150, 28);
            radButton2.TabIndex = 93;
            radButton2.Text = "Apply Changes";
            radButton2.ThemeName = "Fluent";
            // 
            // radCommandBar1
            // 
            radCommandBar1.Dock = System.Windows.Forms.DockStyle.Top;
            radCommandBar1.Location = new System.Drawing.Point(0, 75);
            radCommandBar1.Name = "radCommandBar1";
            radCommandBar1.Rows.AddRange(new Telerik.WinControls.UI.CommandBarRowElement[] { commandBarRowElement1 });
            radCommandBar1.Size = new System.Drawing.Size(1244, 31);
            radCommandBar1.TabIndex = 25;
            radCommandBar1.ThemeName = "Fluent";
            // 
            // radLabel1
            // 
            radLabel1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            radLabel1.ForeColor = System.Drawing.Color.Black;
            radLabel1.Location = new System.Drawing.Point(21, 15);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(96, 23);
            radLabel1.TabIndex = 0;
            radLabel1.Text = "Basic Details";
            radLabel1.ThemeName = "Fluent";
            // 
            // radTextBox1
            // 
            radTextBox1.Location = new System.Drawing.Point(101, 75);
            radTextBox1.Name = "radTextBox1";
            radTextBox1.Size = new System.Drawing.Size(210, 24);
            radTextBox1.TabIndex = 1;
            radTextBox1.ThemeName = "Fluent";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(21, 77);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(78, 18);
            radLabel2.TabIndex = 2;
            radLabel2.Text = "Faction Name:";
            // 
            // radPictureBox1
            // 
            radPictureBox1.Location = new System.Drawing.Point(48, 29);
            radPictureBox1.Name = "radPictureBox1";
            radPictureBox1.Size = new System.Drawing.Size(306, 137);
            radPictureBox1.TabIndex = 0;
            // 
            // FactionEditorForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1244, 886);
            Controls.Add(radButton2);
            Controls.Add(radButton1);
            Controls.Add(radPanel4);
            Controls.Add(radPanel1);
            Controls.Add(radPanel3);
            Controls.Add(radPanel2);
            Controls.Add(radCommandBar1);
            Controls.Add(headerPanel);
            Controls.Add(bottomPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FactionEditorForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "FactionEditorForm";
            ThemeName = "Fluent";
            headerPanel.ResumeLayout(false);
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)CloseButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)radTreeView1).EndInit();
            radTreeView1.ResumeLayout(false);
            radTreeView1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)radLabel4).EndInit();
            ((System.ComponentModel.ISupportInitialize)radPanel2).EndInit();
            radPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radPanel3).EndInit();
            radPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radPanorama1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radPanel1).EndInit();
            radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radPanel4).EndInit();
            radPanel4.ResumeLayout(false);
            radPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)radButton1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radButton2).EndInit();
            ((System.ComponentModel.ISupportInitialize)radCommandBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radTextBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)radPictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Telerik.WinControls.Themes.FluentTheme fluentTheme1;
        private System.Windows.Forms.ShadowLabel label6;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton CloseButton;
        private Telerik.WinControls.UI.RadTreeView radTreeView1;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private Telerik.WinControls.UI.RadCommandBar radCommandBar1;
        private Telerik.WinControls.UI.CommandBarRowElement commandBarRowElement1;
        private Telerik.WinControls.UI.CommandBarStripElement commandBarStripElement1;
        private Telerik.WinControls.UI.CommandBarButton commandBarButton1;
        private Telerik.WinControls.UI.CommandBarButton commandBarButton2;
        private Telerik.WinControls.UI.CommandBarButton commandBarButton3;
        private Telerik.WinControls.UI.RadPanel radPanel3;
        private Telerik.WinControls.UI.RadPanorama radPanorama1;
        private Telerik.WinControls.UI.TileGroupElement tileGroupElement1;
        private Telerik.WinControls.UI.RadTileElement RankTileElement;
        private Telerik.WinControls.UI.RadTileElement TraitsTileElement;
        private Telerik.WinControls.UI.RadTileElement OccupationTileElement;
        private Telerik.WinControls.UI.RadTileElement ExperienceTileElement;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadPanel radPanel4;
        private Telerik.WinControls.UI.RadButton radButton1;
        private Telerik.WinControls.UI.RadButton radButton2;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadTextBox radTextBox1;
        private Telerik.WinControls.UI.RadPictureBox radPictureBox1;
    }
}
