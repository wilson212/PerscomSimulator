namespace Perscom
{
    partial class AdvisorChatForm
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
            headerPanel = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.ShadowLabel();
            chatWindow = new Telerik.WinControls.UI.RadChat();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chatWindow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(label6);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(626, 75);
            headerPanel.TabIndex = 23;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // label6
            // 
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.SystemColors.Control;
            label6.Location = new System.Drawing.Point(12, 19);
            label6.Name = "label6";
            label6.ShadowDirection = 90;
            label6.ShadowOpacity = 225;
            label6.ShadowSoftness = 3F;
            label6.Size = new System.Drawing.Size(466, 37);
            label6.TabIndex = 2;
            label6.Text = "Virtual Assistant";
            // 
            // chatWindow
            // 
            chatWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            chatWindow.Location = new System.Drawing.Point(0, 75);
            chatWindow.Name = "chatWindow";
            chatWindow.Size = new System.Drawing.Size(626, 732);
            chatWindow.TabIndex = 25;
            chatWindow.Text = "radChat1";
            chatWindow.ThemeName = "Fluent";
            chatWindow.TimeSeparatorInterval = System.TimeSpan.Parse("1.00:00:00");
            chatWindow.SendMessage += chatWindow_SendMessage;
            // 
            // AdvisorChatForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(626, 807);
            Controls.Add(chatWindow);
            Controls.Add(headerPanel);
            Name = "AdvisorChatForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Virtual Assistant";
            ThemeName = "Fluent";
            Load += AdvisorChatForm_Load;
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chatWindow).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel label6;
        private Telerik.WinControls.UI.RadChat chatWindow;
    }
}
