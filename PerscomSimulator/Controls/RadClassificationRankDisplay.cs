using Perscom.Database;
using Perscom.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Perscom
{
    /// <summary>
    /// Displays all rank images within a RankClassification as a layered composite,
    /// with each rank slightly overlapping the previous one.
    /// </summary>
    [ToolboxItem(true)]
    public partial class RadClassificationRankDisplay : UserControl
    {
        /// <summary>
        /// The fraction (0.0–1.0) of each rank image's width that overlaps the previous one.
        /// 0.0 = no overlap (side by side), 0.5 = half overlapped, etc.
        /// </summary>
        [Category("Data")]
        [Description("Fraction of each rank image width that overlaps the previous image (0.0 to 1.0).")]
        [DefaultValue(0.35)]
        public double OverlapFraction { get; set; } = 0.35;

        /// <summary>
        /// Padding in pixels subtracted from each side of the picture box before compositing.
        /// </summary>
        [Category("Data")]
        [Description("Padding in pixels around the composite image inside the picture box.")]
        [DefaultValue(4)]
        public int ImagePadding { get; set; } = 4;

        /// <summary>
        /// The vertical offset in pixels each successive rank steps down (stair-step effect).
        /// 0 = all vertically centered on the same line.
        /// </summary>
        [Category("Data")]
        [Description("Pixels each successive rank image steps downward (stair-step effect).")]
        [DefaultValue(4)]
        public int StepDownPixels { get; set; } = 4;

        /// <summary>
        /// The radius of the drop shadow drawn behind each rank image.
        /// 0 = no shadow.
        /// </summary>
        [Category("Data")]
        [Description("Drop shadow blur radius in pixels. 0 disables the shadow.")]
        [DefaultValue(3)]
        public int ShadowRadius { get; set; } = 3;

        /// <summary>
        /// The color of the drop shadow.
        /// </summary>
        [Category("Data")]
        [Description("The color of the drop shadow behind each rank image.")]
        public Color ShadowColor { get; set; } = Color.FromArgb(120, 0, 0, 0);

        /// <summary>
        /// Horizontal and vertical offset of the drop shadow in pixels.
        /// </summary>
        [Category("Data")]
        [Description("Pixel offset of the drop shadow (X and Y).")]
        [DefaultValue(2)]
        public int ShadowOffset { get; set; } = 2;

        /// <summary>
        /// The width of the outline stroke drawn around each rank image.
        /// 0 = no outline.
        /// </summary>
        [Category("Data")]
        [Description("Width in pixels of the outline drawn around each rank image. 0 disables.")]
        [DefaultValue(1)]
        public int OutlineWidth { get; set; } = 1;

        /// <summary>
        /// The color of the outline drawn around each rank image.
        /// </summary>
        [Category("Data")]
        [Description("The color of the outline drawn around each rank image.")]
        public Color OutlineColor { get; set; } = Color.FromArgb(180, 0, 0, 0);

        /// <summary>
        /// The currently displayed RankClassification.
        /// </summary>
        private RankClassification _classification;

        /// <summary>
        /// Cached list of ranks (sorted by Precedence) for the current classification.
        /// </summary>
        private List<Rank> _ranks = new();

        public RadClassificationRankDisplay()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the classification and renders all of its rank images as a layered composite.
        /// </summary>
        /// <param name="classification">The RankClassification whose ranks to display.</param>
        /// <param name="ranks">
        /// Pre-fetched ranks for this classification, sorted by Precedence ascending.
        /// If null, the control will query them from the database.
        /// </param>
        public void SetClassification(RankClassification classification, IEnumerable<Rank> ranks = null)
        {
            _classification = classification;

            if (classification == null)
            {
                _ranks.Clear();
                var old = rankPictureBox.Image;
                rankPictureBox.Image = null;
                old?.Dispose();
                return;
            }

            if (ranks != null)
            {
                _ranks = ranks.OrderBy(r => r.Precedence).ToList();
            }
            else
            {
                _ranks = classification.Ranks.OrderBy(r => r.Precedence).ToList();
            }

            RenderComposite();
        }

        /// <summary>
        /// Builds and assigns the composite bitmap from <see cref="_ranks"/>.
        /// </summary>
        private void RenderComposite()
        {
            if (_ranks == null || _ranks.Count == 0)
            {
                var old = rankPictureBox.Image;
                rankPictureBox.Image = null;
                old?.Dispose();
                return;
            }

            // Available drawing area after padding
            int areaW = Math.Max(1, rankPictureBox.Width - (ImagePadding * 2));
            int areaH = Math.Max(1, rankPictureBox.Height - (ImagePadding * 2));
            Size area = new Size(areaW, areaH);

            // Collect SVG images (skip ranks with no image)
            var svgImages = new List<(Rank rank, RadSvgImage svg)>();
            foreach (var rank in _ranks)
            {
                if (string.IsNullOrEmpty(rank.Image)) continue;
                var svg = ImageAccessor.GetSvgImage(rank.Image);
                if (svg != null)
                    svgImages.Add((rank, svg));
            }

            if (svgImages.Count == 0)
            {
                var old = rankPictureBox.Image;
                rankPictureBox.Image = null;
                old?.Dispose();
                return;
            }

            // Single rank — still composite so it gets shadow + outline
            if (svgImages.Count == 1)
            {
                var svg = svgImages[0].svg;

                // Account for outline + shadow so they don't clip outside the bitmap
                int singleMargin = Math.Max(OutlineWidth, ShadowRadius + ShadowOffset);
                var singleArea = new Size(
                    Math.Max(1, areaW - (singleMargin * 2)),
                    Math.Max(1, areaH - (singleMargin * 2))
                );
                var scaledSize = Imager.ScaleToFit(svg.Size, singleArea);

                Bitmap singleComposite = null;
                try
                {
                    singleComposite = new Bitmap(areaW, areaH);
                    using (Graphics g = Graphics.FromImage(singleComposite))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        var bmp = svg.GetRasterImage(scaledSize);
                        if (bmp != null && bmp.Width > 0 && bmp.Height > 0)
                        {
                            int x = (areaW - scaledSize.Width) / 2;
                            int y = (areaH - scaledSize.Height) / 2;

                            if (ShadowRadius > 0)
                                Imager.DrawDropShadow(g, bmp, x, y, scaledSize.Width, scaledSize.Height, ShadowRadius, ShadowOffset, ShadowColor);

                            if (OutlineWidth > 0)
                                Imager.DrawOutline(g, bmp, x, y, scaledSize.Width, scaledSize.Height, OutlineWidth, OutlineColor);

                            g.DrawImage(bmp, x, y, scaledSize.Width, scaledSize.Height);
                        }
                    }

                    var old = rankPictureBox.Image;
                    rankPictureBox.SvgImage = null;
                    rankPictureBox.Image = singleComposite;
                    old?.Dispose();
                }
                catch
                {
                    singleComposite?.Dispose();
                }
                return;
            }

            // --- Multiple ranks: layered composite ---
            int n = svgImages.Count;
            double overlap = Math.Clamp(OverlapFraction, 0.0, 0.9);

            // Total vertical space consumed by the stair-step effect
            int totalStepDown = StepDownPixels * (n - 1);

            // Reduce available height so the stair-step fits within the area
            int slotH = Math.Max(1, areaH - totalStepDown - ImagePadding);

            // Extra space needed on the left/right for outline + shadow blur
            int leftMargin = Math.Max(OutlineWidth, ShadowRadius + ShadowOffset);
            int rightMargin = leftMargin; // symmetrical so centering looks correct
            int marginTotal = leftMargin + rightMargin;

            // Calculate slot width based on overlap fraction, minus margin space
            double effectiveSlots = 1.0 + (n - 1) * (1.0 - overlap);
            int slotWFromArea = Math.Max(1, (int)((areaW - marginTotal) / effectiveSlots));

            // Cap slot width to slot height to prevent ranks from spreading out
            // when the control is wider than needed (forces the overlap fraction)
            int slotW = Math.Min(slotWFromArea, slotH);

            Size slotSize = new Size(slotW, slotH);

            // Scale each SVG to fit within its slot
            var scaledItems = new List<(RadSvgImage svg, Size scaled)>();
            foreach (var (rank, svg) in svgImages)
            {
                var scaled = Imager.ScaleToFit(svg.Size, slotSize);
                if (scaled.Width <= 0 || scaled.Height <= 0)
                    scaled = new Size(1, 1);
                scaledItems.Add((svg, scaled));
            }

            // Use the ACTUAL maximum scaled image width to calculate the step,
            // not the slot width. This ensures visual overlap matches the configured fraction.
            int maxScaledWidth = 0;
            foreach (var (_, scaled) in scaledItems)
            {
                if (scaled.Width > maxScaledWidth)
                    maxScaledWidth = scaled.Width;
            }

            // Force the step to exactly match the overlap fraction of the actual image width
            int step = Math.Max(1, (int)(maxScaledWidth * (1.0 - overlap)));

            // Total composition width based on actual image widths and forced overlap
            int compositeW = maxScaledWidth + (n - 1) * step;

            // Center horizontally, but ensure at least leftMargin pixels of breathing room
            int startX = Math.Max(leftMargin, (areaW - compositeW) / 2);

            // Build the composite bitmap
            Bitmap composite = null;
            try
            {
                composite = new Bitmap(areaW, areaH);
                using (Graphics g = Graphics.FromImage(composite))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    int referenceH = areaH;
                    for (int i = 0; i < scaledItems.Count; i++)
                    {
                        var (svg, scaled) = scaledItems[i];
                        var bmp = svg.GetRasterImage(scaled);
                        if (bmp == null || bmp.Width <= 0 || bmp.Height <= 0) continue;

                        int x = startX + (i * step);

                        // Stair-step: each rank steps down by StepDownPixels
                        // Center the first rank vertically in the reduced slot area, then offset
                        int baseY = (slotH - scaled.Height) / 2;
                        int y = Math.Max(0, baseY + (i * StepDownPixels));

                        int sr = Imager.ScaleEffect(ShadowRadius, scaled.Height, referenceH);
                        int so = Imager.ScaleEffect(ShadowOffset, scaled.Height, referenceH);
                        int ow = Imager.ScaleEffect(OutlineWidth, scaled.Height, referenceH);

                        if (sr > 0)
                            Imager.DrawDropShadow(g, bmp, x, y, scaled.Width, scaled.Height, sr, so, ShadowColor);

                        if (ow > 0)
                            Imager.DrawOutline(g, bmp, x, y, scaled.Width, scaled.Height, ow, OutlineColor);

                        g.DrawImage(bmp, x, y, scaled.Width, scaled.Height);
                    }
                }

                var oldImage = rankPictureBox.Image;
                rankPictureBox.SvgImage = null;
                rankPictureBox.Image = composite;
                oldImage?.Dispose();
            }
            catch
            {
                composite?.Dispose();
            }
        }

        /// <summary>
        /// Re-renders the composite when the control is resized.
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Only re-render if we have data
            if (_ranks != null && _ranks.Count > 0)
                RenderComposite();
        }
    }
}
