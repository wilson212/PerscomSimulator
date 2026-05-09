using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;

namespace Perscom
{
    /// <summary>
    /// Provides the core styling and colors used in most GUI's
    /// </summary>
    public static class FormStyling
    {
        #region Color Scheme

        public static readonly Color PANEL_COLOR_DARK = Color.FromArgb(51, 53, 53);
        public static readonly Color PANEL_COLOR_DARKER = Color.FromArgb(40, 40, 40);
        public static readonly Color PANEL_COLOR_GRAY = Color.FromArgb(240, 240, 240);

        public static readonly Color CHART_COLOR_DARK = Color.FromArgb(34, 52, 72);
        public static readonly Color CHART_COLOR_LIGHT = Color.FromArgb(50, 82, 118);

        public static readonly Color LINE_COLOR_DARK = Color.FromArgb(39, 64, 92);
        public static readonly Color LINE_COLOR_LIGHT = Color.FromArgb(100, 50, 82, 118);

        public static readonly Color BUTTON_COLOR_DARK = Color.FromArgb(45, 100, 160);
        public static readonly Color BUTTON_COLOR_LIGHT = Color.FromArgb(50, 117, 191);

        public static readonly Color AccentColor = Color.FromArgb(0, 153, 188);
        public static readonly Color AccentMouseOverColor = Color.FromArgb(0, 191, 232);
        public static readonly Color AccentPressedColor = Color.FromArgb(0, 135, 164);

        public static readonly Color DarkBlueAccentColor = Color.FromArgb(45, 100, 160);
        public static readonly Color DarkBlueAccentMouseOverColor = Color.FromArgb(65, 130, 210);
        public static readonly Color DarkBlueAccentPressedColor = Color.FromArgb(30, 60, 90);
        public static readonly Color DarkBlueAccentFocusColor = Color.FromArgb(100, 180, 255);

        public static readonly Color BlueAccentColor = Color.FromArgb(65, 130, 210);
        public static readonly Color BlueAccentPressedColor = Color.FromArgb(45, 100, 160);
        public static readonly Color BlueAccentMouseOverColor = Color.FromArgb(85, 165, 250);
        public static readonly Color BlueAccentFocusColor = Color.FromArgb(115, 195, 255);

        public static readonly Color RedAccentColor = Color.FromArgb(165, 0, 0);
        public static readonly Color RedAccentMouseOverColor = Color.FromArgb(200, 0, 0);
        public static readonly Color RedAccentPressedColor = Color.FromArgb(130, 0, 0);

        public static readonly Color GreenAccentColor = Color.FromArgb(0, 180, 45);
        public static readonly Color GreenAccentMouseOverColor = Color.FromArgb(0, 220, 45);
        public static readonly Color GreenAccentPressedColor = Color.FromArgb(0, 140, 45);

        private static readonly Pen BlackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
        private static readonly Pen GreyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

        #endregion

        public static void ApplyControlsTheme(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                // If it's a Telerik control, clear the ThemeName
                if (control is RadControl radControl)
                {
                    if (radControl.ThemeName == "Fluent")
                        radControl.ThemeName = "FluentPerscomBlue";
                }

                // Recursively check children (panels, groupboxes, tab pages, etc.)
                if (control.HasChildren)
                {
                    ApplyControlsTheme(control.Controls);
                }
            }
        }

        /// <summary>
        /// Provides Fluent theme styling to a <see cref="RadButton"/> using the specified colors
        /// </summary>
        /// <param name="button"></param>
        /// <param name="accentColor"></param>
        /// <param name="mouseOverColor"></param>
        /// <param name="pressedColor"></param>
        public static void StyleButton(RadButton button, Color accentColor, Color mouseOverColor, Color pressedColor)
        {
            // Set the font color
            button.ButtonElement.ForeColor = Color.White;

            ///
            /// Default State
            ///
            button.ButtonElement.SetThemeValueOverride(
                VisualElement.BackColorProperty,
                accentColor,
                "",
                typeof(FillPrimitive)
            );
            button.ButtonElement.SetThemeValueOverride(
                FillPrimitive.GradientStyleProperty,
                GradientStyles.Solid,
                "",
                typeof(FillPrimitive)
            );

            ///
            /// Mouse Over State
            ///
            button.ButtonElement.SetThemeValueOverride(
                VisualElement.BackColorProperty,
                mouseOverColor,
                "MouseOver",
                typeof(FillPrimitive)
            );
            button.ButtonElement.SetThemeValueOverride(
                FillPrimitive.GradientStyleProperty,
                GradientStyles.Solid,
                "MouseOver",
                typeof(FillPrimitive)
            );

            ///
            /// Pressed State
            ///
            button.ButtonElement.SetThemeValueOverride(
                VisualElement.BackColorProperty,
                pressedColor,
                "Pressed",
                typeof(FillPrimitive)
            );
            button.ButtonElement.SetThemeValueOverride(
                FillPrimitive.GradientStyleProperty,
                GradientStyles.Solid,
                "Pressed",
                typeof(FillPrimitive)
            );

            ///
            /// Is Default State
            ///
            button.ButtonElement.SetThemeValueOverride(
                VisualElement.BackColorProperty,
                accentColor,
                "IsDefault",
                typeof(FillPrimitive)
            );
            button.ButtonElement.SetThemeValueOverride(
                FillPrimitive.GradientStyleProperty,
                GradientStyles.Solid,
                "IsDefault",
                typeof(FillPrimitive)
            );
        }

        /// <summary>
        /// Provides Fluent theme styling to a <see cref="RadButton"/> using the specified colors
        /// </summary>
        /// <param name="button"></param>
        /// <param name="accentColor"></param>
        /// <param name="mouseOverColor"></param>
        /// <param name="pressedColor"></param>
        public static void StyleButton(RadButton button, Color accentColor, Color mouseOverColor, Color pressedColor, Color focusColor)
        {
            // Use base method
            StyleButton(button, accentColor, mouseOverColor, pressedColor);

            ///
            /// Focused State (Border/Glow)
            ///
            button.ButtonElement.SetThemeValueOverride(
                BorderPrimitive.ForeColorProperty,
                focusColor,
                "IsFocused",
                typeof(BorderPrimitive)
            );
            button.ButtonElement.SetThemeValueOverride(
                BorderPrimitive.GradientStyleProperty,
                GradientStyles.Solid,
                "IsFocused",
                typeof(BorderPrimitive)
            );
        }

        /// <summary>
        /// Converts a Fluent gray button to a blue color using the FluentPallete
        /// colors
        /// </summary>
        /// <param name="button"></param>
        public static void StyleButtonFluentBlue(RadButton button) 
            => StyleButton(button, AccentColor, AccentMouseOverColor, AccentPressedColor);

        /// <summary>
        /// Converts a Fluent gray button to a blue color using the FluentPallete
        /// colors
        /// </summary>
        /// <param name="button"></param>
        public static void StyleButtonDarkBlue(RadButton button)
            => StyleButton(button, DarkBlueAccentColor, DarkBlueAccentMouseOverColor, DarkBlueAccentPressedColor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        public static void StyleButtonBlue(RadButton button)
            => StyleButton(button, BlueAccentColor, BlueAccentMouseOverColor, BlueAccentPressedColor);

        /// <summary>
        /// Converts a Fluent gray button to a red color using the FluentPallete
        /// colors
        /// </summary>
        /// <param name="button"></param>
        public static void StyleButtonRed(RadButton button)
            => StyleButton(button, RedAccentColor, RedAccentMouseOverColor, RedAccentPressedColor);

        /// <summary>
        /// Converts a Fluent gray button to a green color using the FluentPallete
        /// colors
        /// </summary>
        /// <param name="button"></param>
        public static void StyleButtonGreen(RadButton button)
            => StyleButton(button, GreenAccentColor, GreenAccentMouseOverColor, GreenAccentPressedColor);

        /// <summary>
        /// Applies the dark background to a Form's header panel, as well as applying
        /// the dark underline.
        /// </summary>
        public static void StyleFormHeader(Panel headerPanel, PaintEventArgs e)
        {
            // Set background color
            headerPanel.BackColor = PANEL_COLOR_DARK;

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(GreyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(BlackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(GreyPen, point1, point2);
        }

        /// <summary>
        /// Applies the dark background to a Form's sub header panel, as well as applying
        /// the dark underline.
        /// </summary>
        public static void StyleFormSubHeader(Panel headerPanel, PaintEventArgs e)
        {
            // Set background color
            headerPanel.BackColor = PANEL_COLOR_DARKER;

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(GreyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(BlackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(GreyPen, point1, point2);
        }

        /// <summary>
        /// Applies the Gray background to a Form's footer panel.
        /// </summary>
        public static void StyleFormFooterGray(Panel bottomPanel, PaintEventArgs e)
        {
            // Set background color
            bottomPanel.BackColor = PANEL_COLOR_DARK;

            // Create pen.
            Pen blackPen = new Pen(Color.DarkGray, 1);
            Pen greyPen = new Pen(Color.LightGray, 1);

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(bottomPanel.Width, 0);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, 1);
            point2 = new Point(bottomPanel.Width, 1);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, 2);
            point2 = new Point(bottomPanel.Width, 2);
            e.Graphics.DrawLine(greyPen, point1, point2);
        }

        /// <summary>
        /// Applies the Gray background to a Form's footer panel.
        /// </summary>
        public static void StyleFormFooterDark(Panel bottomPanel, PaintEventArgs e)
        {
            // Set background color
            bottomPanel.BackColor = PANEL_COLOR_DARKER;

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(bottomPanel.Width, 0);
            e.Graphics.DrawLine(GreyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, 1);
            point2 = new Point(bottomPanel.Width, 1);
            e.Graphics.DrawLine(BlackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, 2);
            point2 = new Point(bottomPanel.Width, 2);
            e.Graphics.DrawLine(GreyPen, point1, point2);
        }

        /// <summary>
        /// Applies the Gray background to a Form's footer panel.
        /// </summary>
        public static void StyleFormFooter(RadPanel bottomPanel)
        {
            // Set background color
            bottomPanel.BackColor = PANEL_COLOR_GRAY;

            bottomPanel.PanelElement.PanelBorder.BoxStyle = BorderBoxStyle.FourBorders;
            bottomPanel.PanelElement.PanelBorder.LeftWidth = 0;
            bottomPanel.PanelElement.PanelBorder.BottomWidth = 0;
            bottomPanel.PanelElement.PanelBorder.RightWidth = 0;
        }

        /// <summary>
        /// Applies the Gray background to a Form's footer panel.
        /// </summary>
        public static void StyleFormFooterAlternative(Panel bottomPanel, PaintEventArgs e)
        {
            // Set background color
            bottomPanel.BackColor = FormStyling.CHART_COLOR_DARK;

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
