using Perscom.Controls;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Summary description for ShadowLabel.
    /// </summary>
    /// <seealso cref="https://blogs.msdn.microsoft.com/cjacks/2006/05/26/creating-text-labels-with-a-drop-shadow-effect-in-windows-forms/"/>
    [ToolboxItem(true)]
    [Designer(typeof(ShadowLabelDesigner))]
    public class ShadowLabel : Label
    {
        private Color color;
        private int direction;
        private float softness;
        private int opacity;
        private int shadowDepth;

        public ShadowLabel() : base() {
            color = Color.Black;
            direction = 315;
            softness = 2f;
            opacity = 100;
            shadowDepth = 4;
        }

        [Category("Appearance")]
        [Description("Gets or sets the color of the shadow")]
        [DefaultValue(typeof(Color), "0x000000")]
        public Color ShadowColor
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the degree of opacity of the shadow")]
        [DefaultValue(100)]
        public int ShadowOpacity
        {
            get
            {
                return opacity;
            }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("Opacity", @"Opacity must be between 0 and 255");
                }
                opacity = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets how soft the shadow is")]
        [DefaultValue(2f)]
        public float ShadowSoftness
        {
            get
            {
                return softness;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Softness", @"Softness must be greater than 0");
                }
                softness = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the angle the shadow is cast")]
        [DefaultValue(315)]
        public int ShadowDirection
        {
            get
            {
                return direction;
            }
            set
            {
                if (value < 0 || value > 360)
                {
                    throw new ArgumentOutOfRangeException("Direction", @"Direction must be between 0 and 360");
                }
                direction = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the distance between the plane " +
          "of the object casting the shadow and the shadow plane")]
        [DefaultValue(4)]
        public int ShadowDepth
        {
            get
            {
                return shadowDepth;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("ShadowDepth", @"ShadowDepth must be greater than 0");
                }
                shadowDepth = value;
                Invalidate();
            }
        }
        
        protected override Padding DefaultPadding => new Padding((int)(shadowDepth + softness));
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            // Calculate the content rectangle (respects Padding)
            Rectangle contentRect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            // Build StringFormat for measuring
            using var sf = new StringFormat();
            sf.FormatFlags |= StringFormatFlags.NoWrap;
            sf.Trimming = StringTrimming.EllipsisCharacter;

            // Measure text to manually compute position
            SizeF textSize = e.Graphics.MeasureString(Text, Font, contentRect.Width, sf);

            // Compute vertical position from TextAlign
            float textY;
            switch (TextAlign)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    textY = contentRect.Bottom - textSize.Height;
                    break;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    textY = contentRect.Y + (contentRect.Height - textSize.Height) / 2f;
                    break;
                default: // Top*
                    textY = contentRect.Y;
                    break;
            }

            // Compute horizontal position from TextAlign
            float textX;
            switch (TextAlign)
            {
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    textX = contentRect.Right - textSize.Width;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    textX = contentRect.X + (contentRect.Width - textSize.Width) / 2f;
                    break;
                default: // *Left
                    textX = contentRect.X;
                    break;
            }

            // Shadow offset
            double angle = Math.PI * direction / 180.0;
            float offsetX = (float)(shadowDepth * Math.Cos(angle));
            float offsetY = (float)(shadowDepth * Math.Sin(angle));

            // Draw shadow passes
            int passes = Math.Max(1, (int)softness);
            int alphaPerPass = Math.Max(1, opacity / (passes * passes));

            using (var shadowBrush = new SolidBrush(Color.FromArgb(alphaPerPass, color)))
            {
                for (int x = -passes; x <= passes; x++)
                {
                    for (int y = -passes; y <= passes; y++)
                    {
                        e.Graphics.DrawString(Text, Font, shadowBrush,
                            textX + offsetX + x, textY + offsetY + y);
                    }
                }
            }

            // Draw foreground text
            using (var foreBrush = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(Text, Font, foreBrush, textX, textY);
            }
        }
        
        /*

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            double angle = Math.PI * direction / 180.0;
            float offsetX = (float)(shadowDepth * Math.Cos(angle));
            float offsetY = (float)(shadowDepth * Math.Sin(angle));

            // Draw multiple shadow passes at slight offsets for a soft blur effect
            int passes = Math.Max(1, (int)softness);
            int alphaPerPass = Math.Max(1, opacity / (passes * passes));

            using (var shadowBrush = new SolidBrush(Color.FromArgb(alphaPerPass, color)))
            {
                for (int x = -passes; x <= passes; x++)
                {
                    for (int y = -passes; y <= passes; y++)
                    {
                        e.Graphics.DrawString(
                            Text, Font, shadowBrush,
                            offsetX + x, offsetY + y,
                            StringFormat.GenericTypographic
                        );
                    }
                }
            }

            // Draw foreground text
            using (var foreBrush = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(Text, Font, foreBrush, 0, 0, StringFormat.GenericTypographic);
            }
        }
        */
    }
}