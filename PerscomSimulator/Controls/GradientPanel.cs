using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public class GradientPanel : Panel
    {
        [Category("Appearance")]
        [Description("Gets or sets the gradient transition color")]
        [DefaultValue(typeof(Color), "Black")]
        public Color GradientColor { get; set; } = Color.Black;

        [Category("Appearance")]
        [Description("Gets or sets the gradient angle")]
        [DefaultValue(90F)]
        public float GradientAngle { get; set; } = 90F;

        public GradientPanel()
        {
            this.ResizeRedraw = true;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                this.BackColor,
                GradientColor,
                GradientAngle))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();
            base.OnScroll(se);
        }
    }
}
