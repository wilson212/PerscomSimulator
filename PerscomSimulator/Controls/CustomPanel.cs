using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// A custom panel that allows for adjustable Borders
    /// </summary>
    [ToolboxItem(true)]
    public partial class CustomPanel : Panel
    {
        Color _borderColor = Color.Black;
        int _borderWidth = 5;

        [Category("Appearance")]
        [Description("Gets or sets the border width")]
        [DefaultValue(2)]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                Invalidate();
                PerformLayout();
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                return new Rectangle(_borderWidth, _borderWidth, Bounds.Width - _borderWidth * 2, Bounds.Height - _borderWidth * 2);
            }
        }

        [Category("Appearance")]
        [Description("Gets or Sets the Border color")]
        [DefaultValue(typeof(Color), "0x000000")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        new public BorderStyle BorderStyle
        {
            get { return _borderWidth == 0 ? BorderStyle.None : BorderStyle.FixedSingle; }
            set { }
        }

        [Category("Appearance")]
        [Description("Indicates whether the borders of this panel will be rounded")]
        [DefaultValue(false)]
        public bool BorderRounded { get; set; } = false;

        [Category("Appearance")]
        [Description("Gets or Sets the Border radius when rounded")]
        [DefaultValue(false)]
        public int BorderRoundRadius { get; set; } = 5;

        public bool IncreaseHeight(int value)
        {
            if ((this.Height + value) > this.MaximumSize.Height)
            {
                return false;
            }
            else
            {
                this.Height += value;
                return true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (this.BorderStyle == BorderStyle.FixedSingle)
            {
                using (Pen p = new Pen(_borderColor, _borderWidth))
                {
                    Rectangle r = ClientRectangle;

                    // now for the funky stuff...
                    // to get the rectangle drawn correctly, we actually need to 
                    // adjust the rectangle as .net centers the line, based on width, 
                    // on the provided rectangle.
                    r.Inflate(-Convert.ToInt32(_borderWidth / 2.0 + .5), -Convert.ToInt32(_borderWidth / 2.0 + .5));
                    if (BorderRounded)
                    {
                        // Draw rounded rectangle for borders
                        e.Graphics.DrawPath(p, RoundedRect(r, BorderRoundRadius));

                        // Also round the region of this panel
                        this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, BorderRoundRadius, BorderRoundRadius));
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(p, r);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetDisplayRectLocation(_borderWidth, _borderWidth);
        }

        protected GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            System.Drawing.Size size = new System.Drawing.Size(diameter, diameter);
            bounds.Location = new Drawing.Point(bounds.Location.X - 1, bounds.Location.Y - 1);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();

            
            return path;
        }

        /// <summary>
        /// Rounds the corners of a control
        /// </summary>
        /// <param name="nLeftRect"></param>
        /// <param name="nTopRect"></param>
        /// <param name="nRightRect"></param>
        /// <param name="nBottomRect"></param>
        /// <param name="nWidthEllipse"></param>
        /// <param name="nHeightEllipse"></param>
        /// <returns></returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );
    }
}
