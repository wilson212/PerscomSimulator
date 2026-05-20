using Perscom.Database;
using Perscom.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Perscom.UI;
using Telerik.WinControls;

namespace Perscom
{
    /// <summary>
    /// Represents a custom user control for selecting and displaying a rank.
    /// </summary>
    [ToolboxItem(true)]
    public partial class RadRankSelector : UserControl
    {
        /// <summary>
        /// Gets or sets the title displayed at the top of the control image in the RadRankSelector.
        /// </summary>
        /// <remarks>
        /// This property allows customization of the rank title text that appears in the control.
        /// The default value for this property is "Rank 1".
        /// </remarks>
        [Category("Data")]
        [Description("Gets or sets the title at the top of the control image.")]
        [DefaultValue("Rank 1")]
        public string RankTitle
        {
            get { return lblRankTitle.Text; }
            set { lblRankTitle.Text = value; }
        }

        /// <summary>
        /// Gets or sets the name displayed at the bottom of the control image in the RadRankSelector.
        /// </summary>
        /// <remarks>
        /// This property allows customization of the rank name text that appears in the control.
        /// The default value for this property is "Click to Add".
        /// </remarks>
        [Category("Data")]
        [Description("Gets or sets the name at the bottom of the control image.")]
        [DefaultValue("Click to Add")]
        public string RankName
        {
            get { return lblRankName.Text; }
            set { lblRankName.Text = value; }
        }
        
        /// <summary>
        /// Gets or sets whether the composite next-rank image should be drawn.
        /// When false, only the current rank's image is shown even if NextRankId is set.
        /// </summary>
        [Category("Behavior")]
        [Description("When true, draws the composite current→next rank image if NextRankId is set.")]
        [DefaultValue(true)]
        public bool ShowNextRank { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the padding (in pixels) applied around the rank image inside the picture box.
        /// </summary>
        [Category("Layout")]
        [Description("Gets or sets the padding in pixels applied around the rank image.")]
        [DefaultValue(0)]
        public int ImagePadding { get; set; } = 0;
        
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
        /// Gets or sets the rank object associated with the control.
        /// </summary>
        /// <remarks>
        /// This property allows managing the <see cref="Perscom.Database.Rank"/> instance
        /// that is linked to the control. It enables retrieval or assignment of rank data
        /// for display or operational purposes within the RadRankSelector component.
        /// </remarks>
        public Rank Rank
        {
            get => _rank;
            set => SetRank(value);
        }

        private Rank _rank;

        private Size _targetSize = new Size(96, 64);

        /// <summary>
        /// 
        /// </summary>
        public new event EventHandler OnClick;

        public RadRankSelector()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the rank and updates the associated UI elements to reflect the selected rank.
        /// </summary>
        /// <param name="rank">The rank to set. If null, the control is reset to its default state.</param>
        public void SetRank(Rank rank)
        {
            if (rank == null)
            {
                _rank = null;
                rankPictureBox.SvgImage = null;
                rankPictureBox.Image = Resources.plus;
                RankName = "Click to Add";
                return;
            }

            _rank = rank;
            RankName = rank.Name;
            int referenceH = Math.Max(1, _targetSize.Height - (ImagePadding * 2));

            // Calculate the effect margin needed for shadow/outline so they don't clip
            int effectMargin = 0;
            if (ShadowRadius > 0 && ShadowOffset > 0)
                effectMargin = Math.Max(effectMargin, ShadowRadius + ShadowOffset);
            if (OutlineWidth > 0)
                effectMargin = Math.Max(effectMargin, OutlineWidth);

            if (ShowNextRank && rank.NextRankId.HasValue && rank.NextRank != null)
            {
                // --- Composite two-rank path ---
                rankPictureBox.SvgImage = null;

                RadSvgImage currentSvg = ImageAccessor.GetSvgImage(rank.Image);
                RadSvgImage nextSvg = ImageAccessor.GetSvgImage(rank.NextRank.Image);

                if (currentSvg != null && nextSvg != null)
                {
                    int arrowSize = 16;
                    int minGap = 4; // minimum 4px on each side of the arrow

                    // Full area for the bitmap
                    int fullW = Math.Max(1, _targetSize.Width - (ImagePadding * 2));
                    int fullH = Math.Max(1, _targetSize.Height - (ImagePadding * 2));

                    // Padded target for ScaleToFit (shrunk by effect margin)
                    var paddedTarget = new Size(
                        Math.Max(1, fullW - (effectMargin * 2)),
                        Math.Max(1, fullH - (effectMargin * 2))
                    );

                    // Calculate available width for BOTH rank images combined
                    int reservedWidth = arrowSize + (minGap * 2);
                    int availableForRanks = paddedTarget.Width - reservedWidth;
                    int perRankMaxWidth = availableForRanks / 2;

                    Size rankTargetSize = new Size(perRankMaxWidth, paddedTarget.Height);

                    Size currentScaledSize = Imager.ScaleToFit(currentSvg.Size, rankTargetSize);
                    Size nextScaledSize = Imager.ScaleToFit(nextSvg.Size, rankTargetSize);

                    if (currentScaledSize.Width <= 0 || currentScaledSize.Height <= 0 ||
                        nextScaledSize.Width <= 0 || nextScaledSize.Height <= 0)
                    {
                        var oldImage = rankPictureBox.Image;
                        rankPictureBox.SvgImage = null;
                        var fallbackSize = Imager.ScaleToFit(currentSvg.Size, paddedTarget);
                        rankPictureBox.Image = currentSvg.GetRasterImage(fallbackSize);
                        oldImage?.Dispose();
                        return;
                    }

                    // Total minimum composition width
                    int minCompositionWidth = currentScaledSize.Width + (minGap * 2) + arrowSize + nextScaledSize.Width;

                    // Distribute any extra space equally to both gaps
                    int extraSpace = Math.Max(0, fullW - minCompositionWidth);
                    int gap = minGap + (extraSpace / 2);

                    // Recalculate actual composition width with the final gap
                    int compositionWidth = currentScaledSize.Width + gap + arrowSize + gap + nextScaledSize.Width;
                    int startX = Math.Max(0, (fullW - compositionWidth) / 2);

                    Bitmap composite = null;
                    Bitmap currentBmp = null;
                    Bitmap nextBmp = null;

                    try
                    {
                        composite = new Bitmap(fullW, fullH);
                        using (Graphics g = Graphics.FromImage(composite))
                        {
                            g.Clear(Color.Transparent);
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            currentBmp = currentSvg.GetRasterImage(currentScaledSize);
                            nextBmp = nextSvg.GetRasterImage(nextScaledSize);

                            // Draw current rank (left side)
                            if (currentBmp != null && currentBmp.Width > 0 && currentBmp.Height > 0)
                            {
                                int currentY = Math.Max(0, (fullH - currentScaledSize.Height) / 2);

                                // Scale effects to match the smaller image size
                                int scaledShadowRadius = Imager.ScaleEffect(ShadowRadius, currentScaledSize.Height, referenceH);
                                int scaledShadowOffset = Imager.ScaleEffect(ShadowOffset, currentScaledSize.Height, referenceH);
                                int scaledOutlineWidth = Imager.ScaleEffect(OutlineWidth, currentScaledSize.Height, referenceH);

                                if (scaledShadowRadius > 0)
                                    Imager.DrawDropShadow(g, currentBmp, startX, currentY,
                                        currentScaledSize.Width, currentScaledSize.Height,
                                        scaledShadowRadius, scaledShadowOffset, ShadowColor);

                                if (scaledOutlineWidth > 0)
                                    Imager.DrawOutline(g, currentBmp, startX, currentY,
                                        currentScaledSize.Width, currentScaledSize.Height,
                                        scaledOutlineWidth, OutlineColor);

                                g.DrawImage(currentBmp, startX, currentY, currentScaledSize.Width, currentScaledSize.Height);
                            }

                            // Draw arrow in the center between the two ranks
                            int arrowX = startX + currentScaledSize.Width + gap;
                            int arrowY = ((fullH - arrowSize) / 2) + 1; // +1 nudge to visually center
                            g.DrawImage(Resources.double_arrow_right, arrowX, arrowY, arrowSize, arrowSize);

                            // Draw next rank (right side)
                            if (nextBmp != null && nextBmp.Width > 0 && nextBmp.Height > 0)
                            {
                                int nextX = arrowX + arrowSize + gap;
                                int nextY = Math.Max(0, (fullH - nextScaledSize.Height) / 2);

                                int scaledShadowRadiusNext = Imager.ScaleEffect(ShadowRadius, nextScaledSize.Height, referenceH);
                                int scaledShadowOffsetNext = Imager.ScaleEffect(ShadowOffset, nextScaledSize.Height, referenceH);
                                int scaledOutlineWidthNext = Imager.ScaleEffect(OutlineWidth, nextScaledSize.Height, referenceH);

                                if (scaledShadowRadiusNext > 0)
                                    Imager.DrawDropShadow(g, nextBmp, nextX, nextY,
                                        nextScaledSize.Width, nextScaledSize.Height,
                                        scaledShadowRadiusNext, scaledShadowOffsetNext, ShadowColor);

                                if (scaledOutlineWidthNext > 0)
                                    Imager.DrawOutline(g, nextBmp, nextX, nextY,
                                        nextScaledSize.Width, nextScaledSize.Height,
                                        scaledOutlineWidthNext, OutlineColor);

                                g.DrawImage(nextBmp, nextX, nextY, nextScaledSize.Width, nextScaledSize.Height);
                            }
                        }

                        var oldImage = rankPictureBox.Image;
                        rankPictureBox.Image = composite;
                        oldImage?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        composite?.Dispose();
                        System.Diagnostics.Debug.WriteLine($"Error creating composite rank image: {ex.Message}");

                        var oldImage = rankPictureBox.Image;
                        var fallbackPadded = new Size(
                            Math.Max(1, _targetSize.Width - (ImagePadding * 2)),
                            Math.Max(1, _targetSize.Height - (ImagePadding * 2))
                        );
                        var fallbackSize = Imager.ScaleToFit(currentSvg.Size, fallbackPadded);
                        rankPictureBox.Image = currentSvg?.GetRasterImage(fallbackSize);
                        oldImage?.Dispose();
                    }
                }
            }
            else
            {
                // --- Single rank path ---
                var svgImage = ImageAccessor.GetSvgImage(rank.Image);
                if (svgImage != null)
                {
                    int fullW = Math.Max(1, _targetSize.Width - (ImagePadding * 2));
                    int fullH = Math.Max(1, _targetSize.Height - (ImagePadding * 2));

                    // Shrink the scale target by the effect margin so shadow/outline don't clip
                    var paddedSize = new Size(
                        Math.Max(1, fullW - (effectMargin * 2)),
                        Math.Max(1, fullH - (effectMargin * 2))
                    );
                    Size scaledSize = Imager.ScaleToFit(svgImage.Size, paddedSize);
                    if (scaledSize.Width <= 0 || scaledSize.Height <= 0)
                        scaledSize = new Size(1, 1);

                    Bitmap composite = null;
                    try
                    {
                        composite = new Bitmap(fullW, fullH);
                        using (Graphics g = Graphics.FromImage(composite))
                        {
                            g.Clear(Color.Transparent);
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            var bmp = svgImage.GetRasterImage(scaledSize);
                            if (bmp != null && bmp.Width > 0 && bmp.Height > 0)
                            {
                                int x = (fullW - scaledSize.Width) / 2;
                                int y = (fullH - scaledSize.Height) / 2;

                                if (ShadowRadius > 0)
                                    Imager.DrawDropShadow(g, bmp, x, y, scaledSize.Width, scaledSize.Height,
                                        ShadowRadius, ShadowOffset, ShadowColor);

                                if (OutlineWidth > 0)
                                    Imager.DrawOutline(g, bmp, x, y, scaledSize.Width, scaledSize.Height,
                                        OutlineWidth, OutlineColor);

                                g.DrawImage(bmp, x, y, scaledSize.Width, scaledSize.Height);
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
                else
                {
                    var oldImage = rankPictureBox.Image;
                    rankPictureBox.Image = null;
                    oldImage?.Dispose();
                }
            }

            rankPictureBox.Invalidate();
        }
        

        private void OpenRankSelector(object sender, EventArgs e)
        {
            OnClick?.Invoke(this, e);
        }
        
        /// <summary>
        /// Repositions and resizes child controls when the UserControl is resized,
        /// keeping labels at fixed height and centered, while the picture box fills
        /// the remaining vertical space.
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            const int margin = 10;       // horizontal margin from control edges
            const int labelHeight = 18;  // fixed label height
            const int gap = 3;           // vertical gap between elements

            int contentWidth = Width - (margin * 2);
            if (contentWidth < 1) contentWidth = 1;

            // Top label: fixed height, horizontally centered
            lblRankTitle.Size = new Size(contentWidth, labelHeight);
            lblRankTitle.Location = new Point(margin, margin);

            // Bottom label: fixed height, horizontally centered, pinned to bottom
            lblRankName.Size = new Size(contentWidth, labelHeight);
            lblRankName.Location = new Point(margin, Height - margin - labelHeight);

            // PictureBox: fills the space between the two labels
            int picTop = lblRankTitle.Bottom + gap;
            int picBottom = lblRankName.Top - gap;
            int picHeight = Math.Max(1, picBottom - picTop);

            rankPictureBox.Location = new Point(margin, picTop);
            rankPictureBox.Size = new Size(contentWidth, picHeight);

            // Update the internal target size so SetRank uses the new dimensions
            _targetSize = rankPictureBox.Size;
        }
    }
}
