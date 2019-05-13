using CrossLite.QueryBuilder;
using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Perscom
{
    public partial class RandomizedPoolForm : Form
    {
        protected RandomizedPool Selected { get; set; }

        protected Dictionary<int, Experience> Experience { get; set; }

        protected List<RandomizedPoolSorting> SelectionSorting { get; set; } = new List<RandomizedPoolSorting>();

        protected List<RandomizedPoolFilter> SelectionFilters { get; set; } = new List<RandomizedPoolFilter>();

        private CareerGenerator SelectedNewCareer { get; set; }

        private List<CareerGenerator> CareerGens { get; set; }

        private Dictionary<int, Rank> Ranks { get; set; }

        public RandomizedPoolForm(RandomizedPool setting)
        {
            // Setup form controls
            InitializeComponent();

            // Save settings
            Selected = setting;
            Ranks = new Dictionary<int, Rank>();

            // Fill Ranks
            using (AppDatabase db = new AppDatabase())
            {
                // Fill Ranks
                var ranks = db.Ranks.OrderBy(x => x.Type).ThenBy(x => x.Grade);
                foreach (var rank in ranks)
                {
                    rankSelect.Items.Add(rank);
                    Ranks.Add(rank.Id, rank);
                }

                if (ranks.Count() > 0)
                    rankSelect.SelectedIndex = 0;

                // Fill Experience
                Experience = db.Experience.ToDictionary(x => x.Id, y => y);

                // Fill career generators
                CareerGens = new List<CareerGenerator>(db.CareerGenerators);

                // Fill drop down
                foreach (var length in CareerGens)
                    careerGeneratorBox.Items.Add(length);

                // Select Career Generator Default
                if (CareerGens.Count > 0)
                    careerGeneratorBox.SelectedIndex = 0;

                // Set filter logic
                if (setting.FilterLogic == LogicOperator.And)
                {
                    //orRadioButton.Checked = false;
                    andRadioButton.Checked = true;
                }
                else
                {
                    //andRadioButton.Checked = false;
                    orRadioButton.Checked = true;
                }

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
                IEnumerable<RandomizedPoolSorting> sorting = null;
                if (setting.TemporarySoldierSorting != null && setting.TemporarySoldierSorting.Count > 0)
                {
                    sorting = setting.TemporarySoldierSorting;
                }
                else if (setting.SoldierSorting != null)
                {
                    sorting = setting.SoldierSorting;
                }

                // Add items if we have them
                if (sorting != null && sorting.Count() > 0)
                {
                    // Order them
                    var ordered = sorting.OrderBy(x => x.Precedence);
                    foreach (var thing in ordered)
                    {
                        SelectionSorting.Add(thing);
                    }
                }

                // Add filtering options
                IEnumerable<RandomizedPoolFilter> filtering = null;
                if (setting.TemporarySoldierFiltering != null && setting.TemporarySoldierFiltering.Count > 0)
                {
                    filtering = setting.TemporarySoldierFiltering;
                }
                else if (setting.SoldierFiltering != null)
                {
                    filtering = setting.SoldierFiltering;
                }

                // Add items if we have them
                if (filtering != null && filtering.Count() > 0)
                {
                    // Order them
                    var ordered = filtering.OrderBy(x => x.Precedence);
                    foreach (var thing in ordered)
                    {
                        SelectionFilters.Add(thing);
                    }
                }
            }

            // Fill Filters and Sorting!
            FillFiltersListView();
            FillSortingListView();

            // Set form values for existing settings
            if (setting.RankId != 0)
            {
                // Grab Rank. This can be null if this pool is not saved yet!
                Rank rank = Ranks[setting.RankId];

                // Get rank index
                var index = rankSelect.Items.IndexOf(rank);
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
                sortCheckBox.Checked = setting.OrdersBeforeBilletOrdering;
            }
        }

        protected string GetNameFrom(ClauseLeftSelector selector, int id)
        {
            switch (selector)
            {
                default:
                case ClauseLeftSelector.SoldierValue:
                    return $"Soldier.{((SoldierFunction)id)}";
                case ClauseLeftSelector.SoldierPosition:
                    return $"Position.{((PositionFunction)id)}";
                case ClauseLeftSelector.SoldierExperience:
                    // Check for new experience items added!
                    CheckExperience(id);
                    return $"Experience.{Experience[id]}";
            }
        }

        private void CheckExperience(int experienceId)
        {
            if (!Experience.ContainsKey(experienceId))
            {
                using (AppDatabase db = new AppDatabase())
                {
                    Experience = db.Experience.ToDictionary(x => x.Id, y => y);
                }
            }
        }

        private void FillFiltersListView()
        {
            // Reset listview
            filterListView.Items.Clear();

            // Prepare update
            filterListView.BeginUpdate();

            // Do we even have items to show?
            if (SelectionFilters.Count > 0)
            {
                // Order them
                int i = 0;

                // Add each item
                foreach (var thing in SelectionFilters)
                {
                    var req = (andRadioButton.Checked) ? "And: " : "Or: ";
                    var text = (i > 0) ? req : "Where:";
                    var item = new ListViewItem(text);
                    item.SubItems.Add(GetNameFrom(thing.Selector, thing.SelectorId));
                    item.SubItems.Add(thing.Operator.ToString());
                    item.SubItems.Add(thing.RightValue.ToString());
                    item.Tag = thing;
                    filterListView.Items.Add(item);
                    i++;
                }
            }

            // End update
            filterListView.EndUpdate();
        }

        private void FillSortingListView()
        {
            // Reset listview
            sortingListView.Items.Clear();

            // Prepare update
            sortingListView.BeginUpdate();

            // Do we even have items to show?
            if (SelectionSorting.Count > 0)
            {
                // Order them
                int i = 0;

                // Add each item
                foreach (var thing in SelectionSorting)
                {
                    // Check for new experience items added!
                    if (thing.Selector == ClauseLeftSelector.SoldierExperience)
                        CheckExperience(thing.SelectorId);

                    // Add item to list
                    var text = (i > 0) ? "Then By:" : "Order By:";
                    var item = new ListViewItem(text);
                    item.SubItems.Add(GetNameFrom(thing.Selector, thing.SelectorId));
                    item.SubItems.Add(thing.Direction.ToString());
                    item.Tag = thing;
                    sortingListView.Items.Add(item);
                    i++;
                }
            }

            // End update
            sortingListView.EndUpdate();
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
            Selected.FilterLogic = (andRadioButton.Checked) ? LogicOperator.And : LogicOperator.Or;
            Selected.OrdersBeforeBilletOrdering = sortCheckBox.Checked;

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
            if (sortingListView.Items.Count > 0)
            {
                SelectionSorting = new List<RandomizedPoolSorting>();

                int i = 1;
                foreach (ListViewItem item in sortingListView.Items)
                {
                    var newItem = (RandomizedPoolSorting)item.Tag;
                    newItem.Precedence = i++;
                    SelectionSorting.Add(newItem);
                }

                Selected.TemporarySoldierSorting = SelectionSorting;
            }

            // Re-apply filtering
            if (filterListView.Items.Count > 0)
            {
                SelectionFilters = new List<RandomizedPoolFilter>();

                int i = 1;
                foreach (ListViewItem item in filterListView.Items)
                {
                    var newItem = (RandomizedPoolFilter)item.Tag;
                    newItem.Precedence = i++;
                    SelectionFilters.Add(newItem);
                }

                Selected.TemporarySoldierFiltering = SelectionFilters;
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
            var sort = new RandomizedPoolSorting();
            using (var form = new SoldierSortingForm(sort))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Check for duplicates
                    if (SelectionSorting.FindIndex(x => x.IsDuplicateOf(sort)) > -1)
                    {
                        ShowErrorMessage("No duplicates allowed. Sorting option already exists!");
                        return;
                    }

                    // Add item and redraw
                    SelectionSorting.Add(sort);
                    FillSortingListView();
                }
            }
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get selected item
            ListViewItem item = sortingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            // Get item index
            var sortOption = (RandomizedPoolSorting)item.Tag;
            int index = SelectionSorting.FindIndex(x => x.IsDuplicateOf(sortOption));
            if (index < 0) return;

            // Remove item from list
            SelectionSorting.RemoveAt(index);
            FillSortingListView();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // Get selected item
            ListViewItem item = sortingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            removeItemToolStripMenuItem.Enabled = (item != null);
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            // Re-apply text
            int i = 0;
            foreach (ListViewItem item in sortingListView.Items)
            {
                item.Text = (i > 0) ? "Then By:" : "Order By:";
                i++;
            }
        }

        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = sortingListView.Columns[e.ColumnIndex].Width;
        }

        private void filterListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = filterListView.Columns[e.ColumnIndex].Width;
        }

        private void addNewFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new filter
            var filter = new RandomizedPoolFilter();

            using (var form = new SoldierFilterForm(filter, false))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Check for duplicates
                    if (SelectionFilters.FindIndex(x => x.IsDuplicateOf(filter)) > -1)
                    {
                        ShowErrorMessage("No duplicates allowed. Filter option already exists!");
                        return;
                    }

                    // Add item and re-draw
                    SelectionFilters.Add(filter);
                    FillFiltersListView();
                }
            }
        }

        private void removeFilterItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get selected item
            ListViewItem item = filterListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            // Get item index
            var option = (RandomizedPoolFilter)item.Tag;
            int index = SelectionFilters.FindIndex(x => x.IsDuplicateOf(option));
            if (index < 0) return;

            // Remove item from list
            SelectionFilters.RemoveAt(index);

            // Redraw filterListView
            FillFiltersListView();
        }

        private void filterListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Get selected item
            ListViewItem item = filterListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            AbstractFilter filter = (RandomizedPoolFilter)item.Tag;
            using (var form = new SoldierFilterForm(filter, false))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Add item and re-draw
                    FillFiltersListView();
                }
            }
        }

        private void sortingListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Get selected item
            ListViewItem item = sortingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            AbstractSort filter = (RandomizedPoolSorting)item.Tag;
            using (var form = new SoldierSortingForm(filter))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Add item and re-draw
                    FillSortingListView();
                }
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            // Get selected item
            ListViewItem item = filterListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            removeFilterItemToolStripMenuItem.Enabled = (item != null);
        }

        private void listView2_DragDrop(object sender, DragEventArgs e)
        {
            // Re-apply text
            int i = 0;
            var op = (andRadioButton.Checked) ? "And: " : "Or: ";
            foreach (ListViewItem item in filterListView.Items)
            {
                item.Text = (i > 0) ? op : "Where:"; ;
                i++;
            }
        }

        private void andRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Re-apply text
            int i = 0;
            var op = (andRadioButton.Checked) ? "And: " : "Or: ";
            foreach (ListViewItem item in filterListView.Items)
            {
                item.Text = (i > 0) ? op : "Where:"; ;
                i++;
            }
        }
    }
}
