using System.Drawing;
using System.Windows.Forms;

namespace Perscom
{
    /// <summary>
    /// Provides the core styling and colors used in most GUI's
    /// </summary>
    public static class FormStyling
    {
        #region Color Scheme

        public static readonly Color THEME_COLOR_DARK = Color.FromArgb(51, 53, 53);
        public static readonly Color THEME_COLOR_GRAY = Color.FromArgb(225, 225, 225);
        public static readonly Color CHART_COLOR_DARK = Color.FromArgb(34, 52, 72);
        public static readonly Color CHART_COLOR_LIGHT = Color.FromArgb(50, 82, 118);
        public static readonly Color LINE_COLOR_DARK = Color.FromArgb(39, 64, 92);
        public static readonly Color LINE_COLOR_LIGHT = Color.FromArgb(100, 50, 82, 118);

        #endregion

        /// <summary>
        /// Applies the dark background to a Form's header panel, as well as applying
        /// the dark underline.
        /// </summary>
        /// <param name="headerPanel"></param>
        /// <param name="e"></param>
        public static void StyleFormHeader(Panel headerPanel, PaintEventArgs e)
        {
            // Set background color
            headerPanel.BackColor = THEME_COLOR_DARK;

            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
            Pen greyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(greyPen, point1, point2);
        }

        /// <summary>
        /// Applies the Gray background to a Form's footer panel.
        /// </summary>
        /// <param name="bottomPanel"></param>
        /// <param name="e"></param>
        public static void StyleFormFooter(Panel bottomPanel, PaintEventArgs e)
        {
            // Set background color
            bottomPanel.BackColor = THEME_COLOR_GRAY;

            // Create pen.
            Pen blackPen = new Pen(Color.Gray, 1);

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(bottomPanel.Width, 0);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
        }

        /// <summary>
        /// Applies the dark gray line to the right of a side Panel
        /// </summary>
        /// <param name="sidePanel"></param>
        /// <param name="e"></param>
        public static void StyleFormSidePanel(Panel sidePanel, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Gray, 1);

            // Create points that define line.
            Point point1 = new Point(sidePanel.Width - 1, 0);
            Point point2 = new Point(sidePanel.Width - 1, sidePanel.Height);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
        }
    }
}
