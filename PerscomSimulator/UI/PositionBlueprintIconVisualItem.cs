using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Perscom.Database;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI;

namespace Perscom.UI
{
    public class PositionBlueprintIconVisualItem : IconListViewVisualItem
    {
        private LightVisualElement rankIconElement;
        private LightVisualElement nameElement;
        private StackLayoutPanel mainLayout;

        private const int ShadowRadius = 3;
        private const int ShadowOffset = 2;
        private static readonly Color ShadowColor = Color.FromArgb(120, 0, 0, 0);
        private const int OutlineWidth = 1;
        private static readonly Color OutlineColor = Color.FromArgb(180, 0, 0, 0);
        private const int IconSize = 80;

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            // Vertical layout: [Rank Icon] then [Name]
            this.mainLayout = new StackLayoutPanel();
            this.mainLayout.Orientation = Orientation.Vertical;
            this.mainLayout.Margin = new Padding(4);

            // Top: rank icon, centered
            this.rankIconElement = new LightVisualElement();
            this.rankIconElement.ImageLayout = ImageLayout.Zoom;
            this.rankIconElement.ImageAlignment = ContentAlignment.MiddleCenter;
            this.rankIconElement.MinSize = new Size(IconSize, IconSize);
            this.rankIconElement.MaxSize = new Size(130, IconSize);
            this.rankIconElement.Alignment = ContentAlignment.TopCenter;
            this.rankIconElement.NotifyParentOnMouseInput = true;
            this.rankIconElement.ShouldHandleMouseInput = false;
            this.mainLayout.Children.Add(this.rankIconElement);

            // Bottom: position name below the icon
            this.nameElement = new LightVisualElement();
            this.nameElement.TextAlignment = ContentAlignment.TopCenter;
            this.nameElement.TextWrap = true;
            this.nameElement.AutoEllipsis = true;
            this.nameElement.NotifyParentOnMouseInput = true;
            this.nameElement.ShouldHandleMouseInput = false;
            this.mainLayout.Children.Add(this.nameElement);

            this.Children.Add(this.mainLayout);
        }

        protected override void SynchronizeProperties()
        {
            base.SynchronizeProperties();

            if (this.Data == null || this.Data.Tag == null)
                return;

            var position = this.Data.Tag as PositionBlueprint;
            if (position == null)
                return;

            // CRITICAL: Clear BOTH base class image and text so they don't render
            this.Image = null;
            this.Text = string.Empty;

            // --- Rank icon ---
            if (position.TargetRank != null && !string.IsNullOrWhiteSpace(position.TargetRank.Image))
            {
                if (this.Data.Image != null)
                {
                    this.rankIconElement.Image = this.Data.Image;
                }
                else
                {
                    var svgImage = ImageAccessor.GetSvgImage(position.TargetRank.Image);
                    if (svgImage != null)
                    {
                        int margin = Math.Max(OutlineWidth, ShadowRadius + ShadowOffset);
                        var paddedSize = new Size(
                            Math.Max(1, IconSize - (margin * 2)),
                            Math.Max(1, IconSize - (margin * 2)));

                        var scaledSize = Imager.ScaleToFit(svgImage.Size, paddedSize);
                        if (scaledSize.Width <= 0 || scaledSize.Height <= 0)
                            scaledSize = new Size(1, 1);

                        var bmp = svgImage.GetRasterImage(scaledSize);
                        if (bmp != null && bmp.Width > 0 && bmp.Height > 0)
                        {
                            var composite = new Bitmap(IconSize, IconSize);
                            using (Graphics g = Graphics.FromImage(composite))
                            {
                                g.Clear(Color.Transparent);
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                int x = (IconSize - scaledSize.Width) / 2;
                                int y = (IconSize - scaledSize.Height) / 2;

                                if (ShadowRadius > 0 && ShadowOffset > 0)
                                    Imager.DrawDropShadow(g, bmp, x, y, scaledSize.Width, scaledSize.Height, ShadowRadius, ShadowOffset, ShadowColor);

                                if (OutlineWidth > 0)
                                    Imager.DrawOutline(g, bmp, x, y, scaledSize.Width, scaledSize.Height, OutlineWidth, OutlineColor);

                                g.DrawImage(bmp, x, y, scaledSize.Width, scaledSize.Height);
                            }

                            this.rankIconElement.Image = composite;
                        }
                        else
                        {
                            this.rankIconElement.Image = null;
                        }
                    }
                    else
                    {
                        this.rankIconElement.Image = null;
                    }
                }
            }
            else
            {
                this.rankIconElement.Image = null;
            }

            // Position name below the icon — rendered by our custom nameElement, NOT by base class
            this.nameElement.Text =
                $"<html><span style=\"font-size:9pt;font-family:Segoe UI Semibold;\">{position.Name}</span>";
        }

        protected override Type ThemeEffectiveType
        {
            get { return typeof(IconListViewVisualItem); }
        }
    }
}