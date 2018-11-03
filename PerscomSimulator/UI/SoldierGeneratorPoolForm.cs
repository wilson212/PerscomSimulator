using CrossLite.QueryBuilder;
using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Perscom
{
    public partial class SoldierGeneratorPoolForm : Form
    {
        protected SoldierGeneratorPool Selected { get; set; }

        protected List<SoldierPoolSorting> SortingOptions { get; set; } = new List<SoldierPoolSorting>();

        private CareerGenerator SelectedNewCareer { get; set; }

        private List<CareerGenerator> CareerGens { get; set; }

        public SoldierGeneratorPoolForm(SoldierGeneratorPool setting)
        {
            // Setup form controls
            InitializeComponent();

            // Save settings
            Selected = setting;

            // Fill Ranks
            using (AppDatabase db = new AppDatabase())
            {
                // Fill Ranks
                var ranks = db.Ranks.OrderBy(x => x.Type).ThenBy(x => x.Grade);
                foreach (var rank in ranks)
                    rankSelect.Items.Add(rank);

                if (ranks.Count() > 0)
                    rankSelect.SelectedIndex = 0;

                // Fill career generators
                CareerGens = new List<CareerGenerator>(db.CareerGenerators);

                // Fill drop down
                foreach (var length in CareerGens)
                    careerGeneratorBox.Items.Add(length);

                if (CareerGens.Count > 0)
                    careerGeneratorBox.SelectedIndex = 0;

                // Get selected career adjustment
                if (setting.NewCareerLength || setting.TemporaryCareer != null)
                {
                    var item = setting.CareerGenerator;
                    if (item != null && item != default(CareerGenerator))
                    {
                        SelectedNewCareer = item;
                        newCareerCheckBox.Checked = true;
                        careerGeneratorBox.SelectedIndex = CareerGens.FindIndex(x => x.Id == item.Id);
                    }
                    else if (setting.TemporaryCareer != null)
                    {
                        SelectedNewCareer = setting.TemporaryCareer;
                        newCareerCheckBox.Checked = true;
                        careerGeneratorBox.SelectedIndex = CareerGens.FindIndex(x => x.Id == setting.TemporaryCareer.Id);
                    }
                }

                // Add sorting options
                IEnumerable<SoldierPoolSorting> items = null;
                if (setting.TemporarySoldierSorting != null && setting.TemporarySoldierSorting.Count > 0)
                {
                    items = setting.TemporarySoldierSorting;
                }
                else if (setting.SoldierSorting != null)
                {
                    items = setting.SoldierSorting;
                }

                // Add items if we have them
                if (items != null && items.Count() > 0)
                {
                    // Order them
                    int i = 0;
                    var ordered = items.OrderBy(x => x.Precedence);
                    foreach (var thing in ordered)
                    {
                        SortingOptions.Add(thing);

                        var text = (i > 0) ? "Then By:" : "Order By:";
                        var item = new ListViewItem(text);
                        item.SubItems.Add(thing.SortBy.ToString());
                        item.SubItems.Add(thing.Direction.ToString());
                        item.Tag = thing;
                        listView1.Items.Add(item);
                        i++;
                    }
                }
            }

            // Set form values for existing settings
            if (setting.RankId != 0)
            {
                // Get rank index
                var index = rankSelect.Items.IndexOf(setting.Rank);
                if (index >= 0)
                {
                    rankSelect.SelectedIndex = index;
                }

                // Set probability
                existTrackBar.Value = setting.Probability;

                // Misc Settings
                useRankGradeCheckBox.Checked = setting.UseRankGrade;
                promotableCheckBox.Checked = setting.MustBePromotable;
                lockedCheckBox.Checked = setting.NotLockedInBillet;
            }
        }

        /// <summary>
        /// Allows toggeling the Enabled status for the input fields
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleFields(bool enabled)
        {
            rankSelect.Enabled = enabled;

            existTrackBar.Enabled = enabled;
            newCareerCheckBox.Enabled = enabled;
            promotableCheckBox.Enabled = enabled;
            lockedCheckBox.Enabled = enabled;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Ensure fields are selected!
            if (rankSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No rank was selected!");
                return;
            }
            else if (existTrackBar.Value == 0)
            {
                ShowErrorMessage("Probability must be greater than zero!");
                return;
            }

            // Add new Setting
            Selected.RankId = ((Rank)rankSelect.SelectedItem).Id;
            Selected.Probability = existTrackBar.Value;
            Selected.UseRankGrade = useRankGradeCheckBox.Checked;
            Selected.NewCareerLength = newCareerCheckBox.Checked;
            Selected.MustBePromotable = promotableCheckBox.Checked;
            Selected.NotLockedInBillet = lockedCheckBox.Checked;

            if (newCareerCheckBox.Checked)
            {
                var item = careerGeneratorBox.SelectedItem as CareerGenerator;
                Selected.TemporaryCareer = item;
            }
            else
            {
                Selected.TemporaryCareer = null;
            }

            // Re-apply sorting
            if (listView1.Items.Count > 0)
            {
                SortingOptions = new List<SoldierPoolSorting>();

                int i = 1;
                foreach (ListViewItem item in listView1.Items)
                {
                    var newItem = (SoldierPoolSorting)item.Tag;
                    newItem.Precedence = i++;
                    SortingOptions.Add(newItem);
                }

                Selected.TemporarySoldierSorting = SortingOptions;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void existTrackBar_ValueChanged(object sender, EventArgs e)
        {
            existProbLabel.Text = $"{existTrackBar.Value}%";
        }

        private void rankSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rankSelect.SelectedIndex < 0) return;

            // Get selected rank
            Rank rank = (Rank)rankSelect.SelectedItem;

            // Set new image if it exists
            var image = ImageAccessor.GetImage(Path.Combine("large", rank.Image));
            rankPicture.Image = image;
        }

        #region Panel Border Painting

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion

        private void newCareerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            careerGeneratorBox.Enabled = newCareerCheckBox.Checked;
        }

        private void addNewSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new SoldierSortingForm(SoldierSorting.TimeInService, Sorting.Ascending))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Ensure we aren't a duplicate!
                    var sort = new SoldierPoolSorting()
                    {
                        SoldierPool = Selected,
                        Direction = form.Direction,
                        SortBy = form.SortBy
                    };

                    if (SortingOptions.FindIndex(x => x.IsDuplicateOf(sort)) > -1)
                    {
                        ShowErrorMessage("No duplicates allowed. Sorting option already exists!");
                        return;
                    }

                    SortingOptions.Add(sort);

                    // Add item
                    var text = (listView1.Items.Count > 0) ? "Then By:" : "Order By:";
                    var item = new ListViewItem(text);
                    item.SubItems.Add(form.SortBy.ToString());
                    item.SubItems.Add(form.Direction.ToString());
                    item.Tag = sort;
                    listView1.Items.Add(item);
                }
            }
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get selected item
            ListViewItem item = listView1.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            // Get item index
            var sortOption = (SoldierPoolSorting)item.Tag;
            int index = SortingOptions.FindIndex(x => x.IsDuplicateOf(sortOption));
            if (index < 0) return;

            // Remove item from list
            SortingOptions.RemoveAt(index);
            listView1.Items.Remove(item);

            // Re-apply text
            int i = 0;
            foreach (ListViewItem item2 in listView1.Items)
            {
                item2.Text = (i > 0) ? "Then By:" : "Order By:";
                i++;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // Get selected item
            ListViewItem item = listView1.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            removeItemToolStripMenuItem.Enabled = (item != null);
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            // Re-apply text
            int i = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                item.Text = (i > 0) ? "Then By:" : "Order By:";
                i++;
            }
        }

        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listView1.Columns[e.ColumnIndex].Width;
        }
    }
}
