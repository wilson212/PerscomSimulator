using System.Drawing;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom.UI
{
    /// <summary>
    /// Provides utility methods for simplifying operations related to the RadGridView
    /// control, such as adjusting column widths and managing selection or focus state.
    /// </summary>
    internal static class RadGridViewHelper
    {
        /// <summary>
        /// Adjusts the width of a specified column in a <paramref name="grid"/> to account
        /// for the visibility of the vertical scrollbar, ensuring proper column sizing.
        /// </summary>
        /// <param name="grid">The RadGridView instance containing the column to be adjusted.</param>
        /// <param name="targetColumnName">The name of the column to adjust.</param>
        /// <param name="defaultColumnWidth">The default width to apply to the column if the scrollbar is not visible.</param>
        public static void AdjustColumnForScrollBar(RadGridView grid, string targetColumnName, int defaultColumnWidth)
        {
            if (grid.Columns.Contains(targetColumnName))
            {
                // Get the actual physical width Telerik is currently rendering for the scrollbar
                int scrollBarWidth = grid.TableElement.VScrollBar.ControlBoundingRectangle.Width;

                // If the width is greater than 0, it is actively visible on screen
                bool isVScrollVisible = scrollBarWidth > 0;

                if (isVScrollVisible)
                {
                    // Subtract the exact Telerik theme scrollbar width
                    grid.Columns[targetColumnName].Width = defaultColumnWidth - scrollBarWidth;
                }
                else
                {
                    grid.Columns[targetColumnName].Width = defaultColumnWidth;
                }
            }
        }

        /// <summary>
        /// Clears the selected row on the grid view when an external control is selected
        /// </summary>
        public static void ClearFocusAndSelection(RadGridView gridView)
        {
            // Ends any in-progress editing
            gridView.EndEdit();
            
            // Clears all highlighted selections
            gridView.ClearSelection();

            // Removes the internal "active" pointer so the theme completely lets go of the row
            gridView.CurrentRow = null;
        }

        /// <summary>
        /// Prevents the selected cell border and "pop" out
        /// </summary>
        public static void RemoveSelectedCellBorderAndPop(CellFormattingEventArgs e)
        {
            if (e.CellElement.IsCurrent)
            {
                // 1. Keep the border enabled so the layout engine doesn't recalculate the size
                e.CellElement.DrawBorder = true;

                // 2. Make the focus border invisible
                e.CellElement.BorderColor = Color.Transparent;

                // 3. Force the style to match standard cells so it doesn't try to draw a 3D focus box
                e.CellElement.BorderBoxStyle = BorderBoxStyle.SingleBorder;
                e.CellElement.BorderGradientStyle = GradientStyles.Solid;
            }
            else
            {
                // Restore default theme behavior for non-current cells
                e.CellElement.ResetValue(LightVisualElement.DrawBorderProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BorderColorProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BorderBoxStyleProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BorderGradientStyleProperty, ValueResetFlags.Local);
            }
        }
    }
}
