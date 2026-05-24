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
    public class PositionBlueprintVisualItem : SimpleListViewVisualItem
    {
        private LightVisualElement rankIconElement;
        private LightVisualElement nameElement;
        private LightVisualElement detailsElement;
        private StackLayoutPanel mainLayout;
        private StackLayoutPanel textLayout;

        private const int ShadowRadius = 3;
        private const int ShadowOffset = 2;
        private static readonly Color ShadowColor = Color.FromArgb(120, 0, 0, 0);
        private const int OutlineWidth = 1;
        private static readonly Color OutlineColor = Color.FromArgb(180, 0, 0, 0);
        private const int IconSize = 48;

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            // Horizontal layout: [Icon] [Text Column]
            this.mainLayout = new StackLayoutPanel();
            this.mainLayout.Orientation = Orientation.Horizontal;
            this.mainLayout.Margin = new Padding(2);

            // Left: rank icon element with fixed size
            this.rankIconElement = new LightVisualElement();
            this.rankIconElement.ImageLayout = ImageLayout.Zoom;
            this.rankIconElement.ImageAlignment = ContentAlignment.MiddleCenter;
            this.rankIconElement.MinSize = new Size(IconSize + 4, IconSize);
            this.rankIconElement.MaxSize = new Size(IconSize + 4, IconSize);
            this.rankIconElement.NotifyParentOnMouseInput = true;
            this.rankIconElement.ShouldHandleMouseInput = false;
            this.mainLayout.Children.Add(this.rankIconElement);

            // Right: vertical stack for name + details
            this.textLayout = new StackLayoutPanel();
            this.textLayout.Orientation = Orientation.Vertical;
            this.textLayout.Margin = new Padding(6, 2, 0, 2);

            // Position name (top line)
            this.nameElement = new LightVisualElement();
            this.nameElement.TextAlignment = ContentAlignment.MiddleLeft;
            this.nameElement.NotifyParentOnMouseInput = true;
            this.nameElement.ShouldHandleMouseInput = false;
            this.textLayout.Children.Add(this.nameElement);

            // Rank + Occupation details (below name)
            this.detailsElement = new LightVisualElement();
            this.detailsElement.TextAlignment = ContentAlignment.TopLeft;
            this.detailsElement.NotifyParentOnMouseInput = true;
            this.detailsElement.ShouldHandleMouseInput = false;
            this.textLayout.Children.Add(this.detailsElement);

            this.mainLayout.Children.Add(this.textLayout);
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

            // --- Do NOT set this.Image — that stretches across the whole item ---
            this.Image = null;
            this.Text = string.Empty;

            // --- Render SVG rank icon into the dedicated icon element ---
            if (position.TargetRank != null && !string.IsNullOrWhiteSpace(position.TargetRank.Image))
            {
                // Use pre-rendered image from Data.Image if available (set in FillPositionBlueprintsListView)
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
                            Math.Max(1, IconSize - (margin * 2))
                        );

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

            // --- Position Name (top line) ---
            this.nameElement.Text = $"<html><span style=\"font-size:11pt;font-family:Segoe UI Semibold;\">{position.Name}</span>";

            // --- Rank + Occupation (detail lines) ---
            string rankAbbr = position.TargetRank?.Abbreviation ?? "N/A";

            string occupationText = "No Occupation";
            if (position.Occupation != null)
            {
                occupationText = $"{position.Occupation.Code} - {position.Occupation.Name}";
            }

            this.detailsElement.Text =
                $"<html><span style=\"font-size:9pt;font-family:Segoe UI;\">" +
                $"Rank: <b>{rankAbbr}</b>" +
                $"<br>Occupation: <b>{occupationText}</b></span>";
        }

        protected override Type ThemeEffectiveType
        {
            get { return typeof(SimpleListViewVisualItem); }
        }
    }
}