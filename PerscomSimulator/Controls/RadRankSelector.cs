using Perscom.Database;
using Perscom.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
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

            if (ShowNextRank && rank.NextRankId.HasValue && rank.NextRank != null)
            {
                // Branching: still need manual compositing for two images
                rankPictureBox.SvgImage = null; // clear SVG so .Image takes over

                RadSvgImage currentSvg = ImageAccessor.GetSvgImage(rank.Image);
                RadSvgImage nextSvg = ImageAccessor.GetSvgImage(rank.NextRank.Image);

                if (currentSvg != null && nextSvg != null)
                {
                    int arrowSize = 16;
                    int minGap = 2;
                    int maxGap = 6;
                    int gap = minGap; // Start with minimum gap for sizing calculation
                    
                    var paddedTarget = new Size(
                        Math.Max(1, _targetSize.Width - (ImagePadding * 2)),
                        Math.Max(1, _targetSize.Height - (ImagePadding * 2))
                    );

                    // Calculate available width for BOTH rank images combined
                    int reservedWidth = arrowSize + (gap * 2); // arrow + gaps on each side
                    int availableForRanks = paddedTarget.Width - reservedWidth; // e.g. 96 - 20 = 76
                    int perRankMaxWidth = availableForRanks / 2; // e.g. 38px each

                    // Target size accounts for total composition width
                    Size rankTargetSize = new Size(perRankMaxWidth, paddedTarget.Height);

                    // Calculate scaled sizes maintaining aspect ratios
                    Size currentScaledSize = ScaleToFit(currentSvg.Size, rankTargetSize);
                    Size nextScaledSize = ScaleToFit(nextSvg.Size, rankTargetSize);

                    // Validate sizes before proceeding
                    if (currentScaledSize.Width <= 0 || currentScaledSize.Height <= 0 ||
                        nextScaledSize.Width <= 0 || nextScaledSize.Height <= 0)
                    {
                        var oldImage = rankPictureBox.Image;
                        rankPictureBox.SvgImage = null;
                        rankPictureBox.Image = currentSvg.GetRasterImage(new Size(64, 64));
                        oldImage?.Dispose();
                        return;
                    }

                    // Total width of the composition
                    int compositionWidth = currentScaledSize.Width + gap + arrowSize + gap + nextScaledSize.Width;
                    int startX = Math.Max(0, (paddedTarget.Width - compositionWidth) / 2);

                    // Create composite bitmap
                    Bitmap composite = null;
                    Bitmap currentBmp = null;
                    Bitmap nextBmp = null;

                    try
                    {
                        composite = new Bitmap(paddedTarget.Width, paddedTarget.Height);
                        using (Graphics g = Graphics.FromImage(composite))
                        {
                            g.Clear(Color.Transparent);
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            // Rasterize images
                            currentBmp = currentSvg.GetRasterImage(currentScaledSize);
                            nextBmp = nextSvg.GetRasterImage(nextScaledSize);

                            // Draw current rank (left side)
                            if (currentBmp != null && currentBmp.Width > 0 && currentBmp.Height > 0)
                            {
                                int currentY = Math.Max(0, (paddedTarget.Height - currentScaledSize.Height) / 2);
                                g.DrawImage(currentBmp, startX, currentY, currentScaledSize.Width, currentScaledSize.Height);
                            }

                            // Draw arrow in the center between the two ranks
                            int arrowX = startX + currentScaledSize.Width + gap;
                            int arrowY = (paddedTarget.Height - 8) / 2; // Vertically centered
                            g.DrawImage(Resources.double_arrow_right, arrowX, arrowY, arrowSize, arrowSize);

                            // Draw next rank (right side, full size)
                            if (nextBmp != null && nextBmp.Width > 0 && nextBmp.Height > 0)
                            {
                                int nextX = arrowX + arrowSize + gap;
                                int nextY = Math.Max(0, (paddedTarget.Height - nextScaledSize.Height) / 2);
                                g.DrawImage(nextBmp, nextX, nextY, nextScaledSize.Width, nextScaledSize.Height);
                            }
                        }

                        // Store old image reference
                        var oldImage = rankPictureBox.Image;
                        
                        // Set new image
                        rankPictureBox.Image = composite;
                        oldImage?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        // Clean up on error
                        composite?.Dispose();
                        System.Diagnostics.Debug.WriteLine($"Error creating composite rank image: {ex.Message}");
                        
                        // Fallback: just show the current rank
                        var oldImage = rankPictureBox.Image;
                        rankPictureBox.Image = currentSvg?.GetRasterImage(new Size(64, 64));
                        oldImage?.Dispose();
                    }
                }
            }
            else
            {
                // Scale down SVG to fit the picture box while maintaining aspect ratio
                var svgImage = ImageAccessor.GetSvgImage(rank.Image);
                if (svgImage != null)
                {
                    // Calculate the scaled size maintaining aspect ratio
                    var paddedSize = new Size(
                        Math.Max(1, _targetSize.Width - (ImagePadding * 2)),
                        Math.Max(1, _targetSize.Height - (ImagePadding * 2))
                    );
                    Size scaledSize = ScaleToFit(svgImage.Size, paddedSize);

                    // Rasterize at the scaled size
                    var oldImage = rankPictureBox.Image;
                    rankPictureBox.SvgImage = null;
                    rankPictureBox.Image = svgImage.GetRasterImage(scaledSize);
                    oldImage?.Dispose();
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

        /// <summary>
        /// Calculates a size that fits within the target size while maintaining aspect ratio.
        /// </summary>
        private Size ScaleToFit(Size source, Size target)
        {
            if (source.Width == 0 || source.Height == 0 || target.Width == 0 || target.Height == 0)
                return target;

            float sourceRatio = (float)source.Width / source.Height;
            float targetRatio = (float)target.Width / target.Height;

            int width, height;

            if (sourceRatio > targetRatio)
            {
                // Source is wider, fit to width
                width = target.Width;
                height = (int)(target.Width / sourceRatio);
            }
            else
            {
                // Source is taller, fit to height
                height = target.Height;
                width = (int)(target.Height * sourceRatio);
            }

            return new Size(width, height);
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
