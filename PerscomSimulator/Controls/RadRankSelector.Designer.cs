namespace Perscom
{
    partial class RadRankSelector
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
            lblRankTitle = new Telerik.WinControls.UI.RadLabel();
            lblRankName = new Telerik.WinControls.UI.RadLabel();
            rankPictureBox = new Telerik.WinControls.UI.RadPictureBox();
            ((System.ComponentModel.ISupportInitialize)lblRankTitle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lblRankName).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rankPictureBox).BeginInit();
            SuspendLayout();
            // 
            // lblRankTitle
            // 
            lblRankTitle.AutoSize = false;
            lblRankTitle.Location = new System.Drawing.Point(0, 10);
            lblRankTitle.Name = "lblRankTitle";
            lblRankTitle.Size = new System.Drawing.Size(108, 18);
            lblRankTitle.TabIndex = 0;
            lblRankTitle.Text = "Rank 1";
            lblRankTitle.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRankName
            // 
            lblRankName.AutoSize = false;
            lblRankName.Location = new System.Drawing.Point(0, 98);
            lblRankName.Name = "lblRankName";
            lblRankName.Size = new System.Drawing.Size(108, 18);
            lblRankName.TabIndex = 23;
            lblRankName.Text = "Click to Add";
            lblRankName.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rankPictureBox
            // 
            rankPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            rankPictureBox.Image = Properties.Resources.plus;
            rankPictureBox.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            rankPictureBox.ImageLayout = Telerik.WinControls.UI.RadImageLayout.Center;
            rankPictureBox.Location = new System.Drawing.Point(6, 31);
            rankPictureBox.Name = "rankPictureBox";
            rankPictureBox.Size = new System.Drawing.Size(96, 64);
            rankPictureBox.TabIndex = 22;
            rankPictureBox.Click += OpenRankSelector;
            // 
            // RadRankSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            Controls.Add(lblRankName);
            Controls.Add(rankPictureBox);
            Controls.Add(lblRankTitle);
            Name = "RadRankSelector";
            Size = new System.Drawing.Size(108, 128);
            ((System.ComponentModel.ISupportInitialize)lblRankTitle).EndInit();
            ((System.ComponentModel.ISupportInitialize)lblRankName).EndInit();
            ((System.ComponentModel.ISupportInitialize)rankPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Telerik.WinControls.UI.RadLabel lblRankTitle;
        private Telerik.WinControls.UI.RadLabel lblRankName;
        private Telerik.WinControls.UI.RadPictureBox rankPictureBox;
    }
}
