using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Perscom.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class RankImageSelector : UserControl
    {
        /// <summary>
        /// Gets or sets the name displayed at the bottom of the control image in the RadRankSelector.
        /// </summary>
        /// <remarks>
        /// This property allows customization of the rank name text that appears in the control.
        /// The default value for this property is "Click to Add".
        /// </remarks>
        [Category("Data")]
        [Description("Gets or sets the text at the top of the control image.")]
        [DefaultValue("Rank Image")]
        public string RankText
        {
            get { return lblImageName.Text; }
            set { lblImageName.Text = value; }
        }
        
        /// <summary>
        /// Gets or sets the padding (in pixels) applied around the rank image inside the picture box.
        /// </summary>
        [Category("Layout")]
        [Description("Gets or sets the padding in pixels applied around the rank image.")]
        [DefaultValue(0)]
        public int ImagePadding { get; set; } = 0;

        /// <summary>
        /// Gets or sets the name displayed at the bottom of the control image in the RadRankSelector.
        /// </summary>
        /// <remarks>
        /// This property allows customization of the rank name text that appears in the control.
        /// The default value for this property is "Click to Add".
        /// </remarks>
        [Category("Data")]
        [Description("Gets or sets the instruction text at the bottom of the control image.")]
        [DefaultValue("Click to Change")]
        public string InstructionText
        {
            get { return lblInstruction.Text; }
            set { lblInstruction.Text = value; }
        }
        
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
        /// Gets the relative file path of the currently selected image in the RankImageSelector control.
        /// </summary>
        /// <remarks>
        /// This property stores the path of the selected image relative to the root image directory.
        /// It is updated when an image is selected through the file dialog in the control. Changing
        /// the path programmatically does not trigger the image change event.
        /// </remarks>
        public string SelectedImagePath { get; protected set; }

        /// <summary>
        /// Occurs when the selected image is changed in the RankImageSelector control.
        /// </summary>
        /// <remarks>
        /// This event is triggered after a user selects a new image in the file dialog.
        /// It allows subscribers to respond to changes in the selected image, such as updating
        /// the UI or performing additional processing related to the new image.
        /// </remarks>
        public event EventHandler OnImageChanged;

        /// <summary>
        /// Represents a reference to the parent form that owns or interacts with the RankImageSelector control.
        /// </summary>
        /// <remarks>
        /// This field is used to provide parent form context, enabling operations such as displaying modal dialogs
        /// within the scope of the owning form. It ensures that the user interface interactions are tied to the appropriate
        /// parent form, improving integration and user experience.
        /// </remarks>
        private RadForm _parentForm;

        public RankImageSelector() : this(null, null)
        {
            
        }

        /// <summary>
        /// Represents a user control for selecting and displaying a rank image in the application.
        /// This control allows users to select an SVG image and provides properties to customize
        /// the accompanying text displayed at the top and bottom of the control.
        /// </summary>
        /// <remarks>
        /// The control triggers an event when the image is changed and maintains the path of the
        /// selected image.
        /// </remarks>
        public RankImageSelector(RadForm parentForm, string filePath = null)
        {
            InitializeComponent();
            SelectedImagePath = filePath;
            _parentForm = parentForm;
        }

        /// <summary>
        /// Handles the click event for the rank image selector. This method opens a file dialog
        /// to allow the user to select an SVG image for the rank image. If a valid image is selected,
        /// the image is processed and displayed, and the relative path is updated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data associated with the click action.</param>
        /// <exception cref="Exception">Thrown if an error occurs during image processing.</exception>
        private void RankImageSelector_Click(object sender, EventArgs e)
        {
            // Build the initial directory path for rank images
            string imagesPath = System.IO.Path.Combine(Program.RootPath, "Images", "ranks");

            // Ensure the directory exists before opening the dialog
            if (!System.IO.Directory.Exists(imagesPath))
            {
                System.IO.Directory.CreateDirectory(imagesPath);
            }

            using var dialog = new RadOpenFileDialog
            {
                Filter = "SVG Images (*.svg)|*.svg",
                InitialDirectory = imagesPath,
                RestoreDirectory = true,
                MultiSelect = false
            };

            dialog.OpenFileDialogForm.ThemeName = Program.ThemeName;
            dialog.OpenFileDialogForm.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(_parentForm) != DialogResult.OK)
                return;

            // Calculate the relative path from the Images folder
            // ImageAccessor expects paths relative to Program.RootPath/Images/
            string imagesRoot = System.IO.Path.Combine(Program.RootPath, "Images");
            string selectedFullPath = dialog.FileName;

            string relativePath;
            if (!selectedFullPath.StartsWith(imagesRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Path must be inside the Images folder");
            }

            // Strip the Images root + separator to get the relative path
            relativePath = selectedFullPath.Substring(imagesRoot.Length).TrimStart(
                System.IO.Path.DirectorySeparatorChar,
                System.IO.Path.AltDirectorySeparatorChar
            );

            // Set image
            var svgImage = ImageAccessor.GetSvgImage(selectedFullPath);
            SetImage(svgImage);

            // Fire after the image has been selected
            SelectedImagePath = relativePath;
            OnImageChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Sets the image to be displayed in the rank image selector.
        /// </summary>
        /// <param name="svgImage">The SVG image to set. Pass null to reset to the default image.</param>
        public void SetImage(RadSvgImage svgImage)
        {
            if (svgImage != null)
            {
                var oldImage = rankPictureBox.Image;
                rankPictureBox.SvgImage = null;

                // Account for outline + shadow so they don't clip
                int margin = Math.Max(OutlineWidth, ShadowRadius + ShadowOffset);

                // Reduce the available area by the padding AND the effect margin on each side
                var paddedSize = new Size(
                    Math.Max(1, rankPictureBox.Width - (ImagePadding * 2) - (margin * 2)),
                    Math.Max(1, rankPictureBox.Height - (ImagePadding * 2) - (margin * 2))
                );

                var sizeRatio = Imager.ScaleToFit(svgImage.Size, paddedSize);
                if (sizeRatio.Width <= 0 || sizeRatio.Height <= 0)
                    sizeRatio = new Size(1, 1);

                int areaW = rankPictureBox.Width - (ImagePadding * 2);
                int areaH = rankPictureBox.Height - (ImagePadding * 2);

                Bitmap composite = null;
                try
                {
                    composite = new Bitmap(Math.Max(1, areaW), Math.Max(1, areaH));
                    using (Graphics g = Graphics.FromImage(composite))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        var bmp = svgImage.GetRasterImage(sizeRatio);
                        if (bmp != null && bmp.Width > 0 && bmp.Height > 0)
                        {
                            int x = (areaW - sizeRatio.Width) / 2;
                            int y = (areaH - sizeRatio.Height) / 2;

                            if (ShadowRadius > 0 && ShadowOffset > 0)
                            {
                                Imager.DrawDropShadow(g, bmp, x, y, sizeRatio.Width, sizeRatio.Height,
                                    ShadowRadius, ShadowOffset, ShadowColor);
                            }

                            if (OutlineWidth > 0)
                            {
                                Imager.DrawOutline(g, bmp, x, y, sizeRatio.Width, sizeRatio.Height,
                                    OutlineWidth, OutlineColor);
                            }

                            g.DrawImage(bmp, x, y, sizeRatio.Width, sizeRatio.Height);
                        }
                    }

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
                rankPictureBox.Image = Properties.Resources.plus;
                oldImage?.Dispose();
            }
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
            lblImageName.Size = new Size(contentWidth, labelHeight);
            lblImageName.Location = new Point(margin, margin);

            // Bottom label: fixed height, horizontally centered, pinned to bottom
            lblInstruction.Size = new Size(contentWidth, labelHeight);
            lblInstruction.Location = new Point(margin, Height - margin - labelHeight);

            // PictureBox: fills the space between the two labels
            int picTop = lblImageName.Bottom + gap;
            int picBottom = lblInstruction.Top - gap;
            int picHeight = Math.Max(1, picBottom - picTop);

            rankPictureBox.Location = new Point(margin, picTop);
            rankPictureBox.Size = new Size(contentWidth, picHeight);
        }
    }
}
