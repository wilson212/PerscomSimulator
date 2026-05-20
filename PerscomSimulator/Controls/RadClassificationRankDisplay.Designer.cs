namespace Perscom
{
    partial class RadClassificationRankDisplay
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            rankPictureBox = new Telerik.WinControls.UI.RadPictureBox();
            ((System.ComponentModel.ISupportInitialize)rankPictureBox).BeginInit();
            SuspendLayout();
            // 
            // rankPictureBox
            // 
            rankPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            rankPictureBox.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            rankPictureBox.ImageLayout = Telerik.WinControls.UI.RadImageLayout.Center;
            rankPictureBox.Location = new System.Drawing.Point(0, 0);
            rankPictureBox.Name = "rankPictureBox";
            rankPictureBox.Size = new System.Drawing.Size(150, 150);
            rankPictureBox.TabIndex = 0;
            // 
            // RadClassificationRankDisplay
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            Controls.Add(rankPictureBox);
            Name = "RadClassificationRankDisplay";
            ((System.ComponentModel.ISupportInitialize)rankPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Telerik.WinControls.UI.RadPictureBox rankPictureBox;
    }
}
