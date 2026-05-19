namespace Perscom
{
    partial class RankImageSelector
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
            lblInstruction = new Telerik.WinControls.UI.RadLabel();
            rankPictureBox = new Telerik.WinControls.UI.RadPictureBox();
            lblImageName = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)lblInstruction).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rankPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lblImageName).BeginInit();
            SuspendLayout();
            // 
            // lblInstruction
            // 
            lblInstruction.AutoSize = false;
            lblInstruction.Location = new System.Drawing.Point(21, 132);
            lblInstruction.Name = "lvlInstruction";
            lblInstruction.Size = new System.Drawing.Size(108, 18);
            lblInstruction.TabIndex = 26;
            lblInstruction.Text = "Click to Add";
            lblInstruction.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rankPictureBox
            // 
            rankPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            rankPictureBox.Image = Properties.Resources.plus;
            rankPictureBox.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            rankPictureBox.ImageLayout = Telerik.WinControls.UI.RadImageLayout.Center;
            rankPictureBox.Location = new System.Drawing.Point(11, 31);
            rankPictureBox.Name = "rankPictureBox";
            rankPictureBox.Size = new System.Drawing.Size(128, 96);
            rankPictureBox.TabIndex = 25;
            rankPictureBox.ThemeName = "Fluent";
            rankPictureBox.Click += RankImageSelector_Click;
            // 
            // lblImageName
            // 
            lblImageName.AutoSize = false;
            lblImageName.Location = new System.Drawing.Point(21, 10);
            lblImageName.Name = "lblImageName";
            lblImageName.Size = new System.Drawing.Size(108, 18);
            lblImageName.TabIndex = 24;
            lblImageName.Text = "Rank 1";
            lblImageName.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RankImageSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            Controls.Add(lblInstruction);
            Controls.Add(rankPictureBox);
            Controls.Add(lblImageName);
            Name = "RankImageSelector";
            Size = new System.Drawing.Size(150, 160);
            Click += RankImageSelector_Click;
            ((System.ComponentModel.ISupportInitialize)lblInstruction).EndInit();
            ((System.ComponentModel.ISupportInitialize)rankPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)lblImageName).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Telerik.WinControls.UI.RadLabel lblInstruction;
        private Telerik.WinControls.UI.RadPictureBox rankPictureBox;
        private Telerik.WinControls.UI.RadLabel lblImageName;
    }
}
