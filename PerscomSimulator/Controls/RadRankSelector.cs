using Perscom.Database;
using Perscom.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Perscom
{
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

            if (rank.NextRankId.HasValue && rank.NextRank != null)
            {
                // Branching: still need manual compositing for two images
                rankPictureBox.SvgImage = null; // clear SVG so .Image takes over

                var oldImage = rankPictureBox.Image;

                Bitmap composite = new Bitmap(96, 64);
                using (Graphics g = Graphics.FromImage(composite))
                {
                    g.Clear(Color.Transparent);

                    RadSvgImage nextSvg = ImageAccessor.GetSvgImage(rank.NextRank.Image);
                    if (nextSvg != null)
                    {
                        using (Bitmap nextBmp = nextSvg.GetRasterImage(new Size(32, 32)))
                            g.DrawImage(nextBmp, 64, 16);
                    }

                    RadSvgImage currentSvg = ImageAccessor.GetSvgImage(rank.Image);
                    if (currentSvg != null)
                    {
                        using (Bitmap currentBmp = currentSvg.GetRasterImage(new Size(64, 64)))
                            g.DrawImage(currentBmp, 0, 0);
                    }
                }

                rankPictureBox.Image = composite;
                oldImage?.Dispose();
            }
            else
            {
                // Non-branching: let Telerik handle SVG natively — zero bitmap allocation
                rankPictureBox.Image = null; // clear raster so .SvgImage takes over
                rankPictureBox.SvgImage = ImageAccessor.GetSvgImage(rank.Image);
            }
            
            rankPictureBox.Invalidate();
        }

        private void OpenRankSelector(object sender, EventArgs e)
        {
            OnClick?.Invoke(this, e);
        }
    }
}
