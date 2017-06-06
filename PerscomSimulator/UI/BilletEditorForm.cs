using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Perscom.Database;

namespace Perscom
{
    public partial class BilletEditorForm : Form
    {
        protected UnitTemplate Template { get; set; }

        protected Billet Billet { get; set; }

        public BilletEditorForm(UnitTemplate template, Billet billet)
        {
            // Setup form controls
            InitializeComponent();
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            bottomPanel.BackColor = MainForm.THEME_COLOR_GRAY;

            // Set internal properties
            Template = template;
            Billet = billet;

            // Fill Ranks
            FillRanksAndCatagories();

            // Set form fields if this is an existing billet
            if (billet.Id > 0)
            {
                billetNameBox.Text = billet.Name;
                maxTigBox.Value = billet.MaxTourLength;
                minTigBox.Value = billet.MinTourLength;
                earlyRetireCheckBox.Checked = billet.CanRetireEarly;
                entryLevelCheckBox.Checked = billet.EntryLevel;
                zIndexBox.Value = billet.ZIndex;

                // Get rank index
                var index = billetRankSelect.Items.IndexOf(billet.Rank);
                if (index >= 0)
                {
                    billetRankSelect.SelectedIndex = index;

                    // Check for rank range
                    if (billet.Rank.Grade < billet.MaxRank.Grade)
                    {
                        index = billetRankRangeSelect.Items.IndexOf(billet.MaxRank);
                        if (index >= 0)
                        {
                            billetRangeCheckBox.Checked = true;
                            billetRankRangeSelect.SelectedIndex = index;
                        }
                    }
                }

                // Get catagory index
                index = billetCatSelect.Items.IndexOf(billet.Catagory);
                if (index >= 0)
                {
                    billetCatSelect.SelectedIndex = index;
                }

                // Set index
                index = promotionPoolSelect.Items.IndexOf(billet.PromotionPool);
                if (index >= 0)
                {
                    promotionPoolSelect.SelectedIndex = index;
                }
            }
        }

        private void FillRanksAndCatagories()
        {
            // Fill rank types box
            using (AppDatabase db = new AppDatabase())
            {
                // Add ranks to combo select box
                var ranks = db.Ranks.OrderBy(x => x.Type).ThenBy(x => x.Grade);
                foreach (var rank in ranks)
                {
                    billetRankSelect.Items.Add(rank);
                }

                if (billetRankSelect.Items.Count > 0)
                    billetRankSelect.SelectedIndex = 0;

                // Next, add catagories
                var catagories = db.BilletCatagories.OrderByDescending(x => x.ZIndex);
                foreach (var cat in catagories)
                {
                    billetCatSelect.Items.Add(cat);
                }

                if (billetCatSelect.Items.Count > 0)
                    billetCatSelect.SelectedIndex = 0;

                // Finally, Get promotion pool
                var list = db.Echelons.OrderByDescending(x => x.HierarchyLevel).ToArray();
                promotionPoolSelect.Items.AddRange(list);
                promotionPoolSelect.SelectedIndex = 0;
            }
        }

        #region Panel Border Painting

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
            Pen greyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(greyPen, point1, point2);

            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Gray, 1);

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(bottomPanel.Width, 0);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
            base.OnPaint(e);
        }

        #endregion

        private void billetRankSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (billetRankSelect.SelectedIndex < 0) return;

            // Get selected rank
            Rank rank = (Rank)billetRankSelect.SelectedItem;

            // Set new image if it exists
            var image = ImageAccessor.GetImage(Path.Combine("large", rank.Image));
            rankPicture.Image = image;

            // Clear old items
            billetRankRangeSelect.Items.Clear();
            specialtySelect.Items.Clear();
            spawnSpecialtySelect.Items.Clear();

            // Fill rank types box
            using (AppDatabase db = new AppDatabase())
            {
                // Grab ranks of the same type, and equal or greater grade
                var ranks = db.Ranks.Where(x => x.Type == rank.Type && x.Grade >= rank.Grade).OrderBy(x => x.Grade).ToArray();

                // Add files in this directory
                foreach (var r in ranks)
                {
                    billetRankRangeSelect.Items.Add(r);
                }

                if (billetRankRangeSelect.Items.Count > 0)
                    billetRankRangeSelect.SelectedIndex = 0;


                // Add specialty combo boxes based on rank type
                foreach (var spec in db.Specialties.Where(x => x.Type == rank.Type).OrderBy(x => x.Code))
                {
                    specialtySelect.Items.Add(spec);
                    spawnSpecialtySelect.Items.Add(spec);
                }

                // Set default indexies
                if (specialtySelect.Items.Count > 0)
                {
                    specialtySelect.SelectedIndex = 0;
                    spawnSpecialtySelect.SelectedIndex = 0;
                }
            }

            // TODO
            // Set values of existing billets
            if (Billet.Id > 0)
            {
                //
                // Set specialty MOS
                //
                BilletSpecialty special = Billet.Specialties.FirstOrDefault();
                if (special != null)
                {
                    specialtyCheckBox.Checked = true;
                    var index = specialtySelect.Items.IndexOf(special);
                    if (index > 0)
                        specialtySelect.SelectedIndex = index;
                }

                //
                // Filter and Adjust specialty requirements
                //

                //
                // Set spawn MOS?
                //
            }
        }

        private void billetRangeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            billetRankRangeSelect.Enabled = billetRangeCheckBox.Checked;
        }

        private void specialtyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            specialtySelect.Enabled = specialtyCheckBox.Checked;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Check for validation errors
            if (billetRankSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No billet rank was selected!");
                return;
            }
            else if (billetCatSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No billet catagory was selected!");
                return;
            }
            else if (String.IsNullOrWhiteSpace(billetNameBox.Text))
            {
                ShowErrorMessage("Invalid billet name entered!");
                return;
            }

            // Save
            Billet.Name = billetNameBox.Text;
            Billet.RankId = ((Rank)billetRankSelect.SelectedItem).Id;
            Billet.MaxRankId = ((Rank)billetRankRangeSelect.SelectedItem).Id;
            Billet.MaxTourLength = (int)maxTigBox.Value;
            Billet.MinTourLength = (int)minTigBox.Value;
            Billet.Stature = (int)statureBox.Value;
            Billet.CanRetireEarly = earlyRetireCheckBox.Checked;
            Billet.EntryLevel = entryLevelCheckBox.Checked;
            Billet.UnitTypeId = Template.Id;
            Billet.BilletCatagoryId = ((BilletCatagory)billetCatSelect.SelectedItem).Id;
            Billet.PromotionPoolId = ((Echelon)promotionPoolSelect.SelectedItem).Id;
            Billet.ZIndex = (int)zIndexBox.Value;

            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    // Insert or Update record
                    if (Billet.Id == 0)
                        db.Billets.Add(Billet);
                    else
                        db.Billets.Update(Billet);

                    // Apply specialty change
                    if (specialtyCheckBox.Checked)
                    {

                    }

                    // Apply required specialties
                    if (listView1.Items.Count > 0)
                    {

                    }
                }
                catch
                {
                    trans.Rollback();
                }
            }

            // Close this form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //bool ebl = listView1.SelectedItems.Count > 0;
            //removeSpecialtyToolStripMenuItem.Enabled = ebl;

            //ebl = listView1.Items.Count > 0;
            //removeAllSpecialtiesToolStripMenuItem.Enabled = ebl;
        }

        private void editRequiredSpecialtiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
