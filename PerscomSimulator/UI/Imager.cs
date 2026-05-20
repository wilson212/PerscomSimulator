using System;
using System.Drawing;

namespace Perscom.UI
{
    /// <summary>
    /// Provides utility methods for image manipulation, including scaling,
    /// drawing shadows, and adding outlines to images.
    /// </summary>
    internal static class Imager
    {
        /// <summary>
        /// Calculates a size that fits within the target size while maintaining aspect ratio.
        /// </summary>
        /// <param name="source">The original image size.</param>
        /// <param name="target">The available area to fit within.</param>
        /// <param name="padding">Padding in pixels to subtract from each side of the target area.</param>
        public static Size ScaleToFit(Size source, Size target, int padding = 0)
        {
            // Shrink the target by padding on all sides
            int effectiveWidth = target.Width - (padding * 2);
            int effectiveHeight = target.Height - (padding * 2);

            if (source.Width == 0 || source.Height == 0 || effectiveWidth <= 0 || effectiveHeight <= 0)
                return target;

            float sourceRatio = (float)source.Width / source.Height;
            float targetRatio = (float)effectiveWidth / effectiveHeight;

            int width, height;

            if (sourceRatio > targetRatio)
            {
                // Source is wider, fit to width
                width = effectiveWidth;
                height = (int)(effectiveWidth / sourceRatio);
            }
            else
            {
                // Source is taller, fit to height
                height = effectiveHeight;
                width = (int)(effectiveHeight * sourceRatio);
            }

            return new Size(width, height);
        }

        /// <summary>
        /// Draws a drop shadow behind the given bitmap by rendering it offset and blurred
        /// using multiple semi-transparent passes.
        /// </summary>
        public static void DrawDropShadow(Graphics g, Bitmap source, int x, int y, int w, int h, int shadowRadius, int shadowOffset, Color shadowColor)
        {
            int radius = shadowRadius;
            int ox = shadowOffset;
            int oy = shadowOffset;

            // Create a shadow bitmap: solid shadow color masked by the source alpha
            using var shadow = new Bitmap(w, h);
            using (Graphics sg = Graphics.FromImage(shadow))
            {
                sg.Clear(Color.Transparent);

                // Draw the source image to get its alpha channel
                sg.DrawImage(source, 0, 0, w, h);
            }

            // Replace all visible pixels with the shadow color while preserving alpha
            for (int py = 0; py < shadow.Height; py++)
            {
                for (int px = 0; px < shadow.Width; px++)
                {
                    Color pixel = shadow.GetPixel(px, py);
                    if (pixel.A > 0)
                    {
                        // Scale shadow alpha by the pixel's original alpha
                        int alpha = (int)((pixel.A / 255.0) * shadowColor.A);
                        shadow.SetPixel(px, py, Color.FromArgb(
                            alpha, shadowColor.R, shadowColor.G, shadowColor.B));
                    }
                }
            }

            // Draw the shadow multiple times at slight offsets to simulate blur
            if (radius <= 1)
            {
                // Simple offset shadow, no blur
                g.DrawImage(shadow, x + ox, y + oy, w, h);
            }
            else
            {
                // Multi-pass blur: draw the shadow at offsets within the radius
                // with reduced opacity per pass
                int passes = 0;
                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        if (dx * dx + dy * dy <= radius * radius)
                            passes++;
                    }
                }

                float alphaScale = 1.0f / Math.Max(1, passes / 2);

                using var attrs = new System.Drawing.Imaging.ImageAttributes();
                float[][] matrixItems = {
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 0, 0, alphaScale, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(matrixItems);
                attrs.SetColorMatrix(colorMatrix);

                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        if (dx * dx + dy * dy > radius * radius) continue;

                        var destRect = new Rectangle(x + ox + dx, y + oy + dy, w, h);
                        g.DrawImage(shadow, destRect, 0, 0, w, h, GraphicsUnit.Pixel, attrs);
                    }
                }
            }
        }
        
        /// <summary>
        /// Draws an outline around the non-transparent pixels of the source bitmap
        /// by creating a dilated silhouette that expands the opaque region outward.
        /// </summary>
        public static void DrawOutline(Graphics g, Bitmap source, int x, int y, int w, int h, int width, Color color)
        {
            if (width <= 0) return;

            // Build a dilated silhouette: for each pixel, if ANY pixel within 'width' radius
            // in the source is opaque, this pixel becomes the outline color.
            using var silhouette = new Bitmap(w + (width * 2), h + (width * 2));

            // First, draw the source into a temp bitmap to read its alpha
            using var sourceCopy = new Bitmap(w, h);
            using (Graphics sg = Graphics.FromImage(sourceCopy))
            {
                sg.Clear(Color.Transparent);
                sg.DrawImage(source, 0, 0, w, h);
            }

            // Dilate: for each pixel in the expanded silhouette, check if any source pixel
            // within 'width' distance is opaque
            for (int py = 0; py < silhouette.Height; py++)
            {
                for (int px = 0; px < silhouette.Width; px++)
                {
                    // Map back to source coordinates (offset by width)
                    int srcX = px - width;
                    int srcY = py - width;

                    // Check if any source pixel within radius 'width' is opaque
                    int maxAlpha = 0;
                    for (int dy = -width; dy <= width; dy++)
                    {
                        for (int dx = -width; dx <= width; dx++)
                        {
                            int sx = srcX + dx;
                            int sy = srcY + dy;
                            if (sx < 0 || sx >= w || sy < 0 || sy >= h) continue;

                            Color sp = sourceCopy.GetPixel(sx, sy);
                            if (sp.A > maxAlpha)
                                maxAlpha = sp.A;
                        }
                    }

                    if (maxAlpha > 0)
                    {
                        int alpha = (int)((maxAlpha / 255.0) * color.A);
                        silhouette.SetPixel(px, py, Color.FromArgb(alpha, color.R, color.G, color.B));
                    }
                }
            }

            // Draw the dilated silhouette behind the image (offset so it's centered on the image)
            g.DrawImage(silhouette, x - width, y - width, silhouette.Width, silhouette.Height);
        }
        
        /// <summary>
        /// Scales an effect size (shadow radius, offset, outline width) proportionally
        /// to the rendered image height relative to a reference height.
        /// Returns at least 1 if the input is > 0, to avoid completely losing the effect.
        /// </summary>
        public static int ScaleEffect(int baseValue, int renderedHeight, int referenceHeight)
        {
            if (baseValue <= 0 || referenceHeight <= 0) return baseValue;
            double scale = (double)renderedHeight / referenceHeight;
            int scaled = (int)Math.Round(baseValue * scale);
            return Math.Max(1, scaled); // never drop below 1px if the effect is enabled
        }
    }
}
