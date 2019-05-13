using CrossLite.QueryBuilder;
using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Perscom
{
    public partial class OrderedPoolForm : Form
    {
        protected OrderedPool SelectedPool { get; set; }

        private CareerGenerator SelectedNewCareer { get; set; }

        private List<CareerGenerator> CareerGens { get; set; }

        private Dictionary<int, Rank> Ranks { get; set; }

        protected Dictionary<int, Experience> Experience { get; set; }

        protected List<Specialty> Requirements { get; set; } = new List<Specialty>();

        protected List<OrderedPoolSorting> SelectionSorting { get; set; } = new List<OrderedPoolSorting>();

        protected List<OrderedPoolFilter> SelectionFilters { get; set; } = new List<OrderedPoolFilter>();

        protected List<OrderedPoolGroup> SelectionGroups { get; set; } = new List<OrderedPoolGroup>();

        protected bool RequirementsChanged { get; set; }

        protected bool GroupingChanged { get; set; }

        protected bool SortingChanged { get; set; }

        protected bool FiltersChanged { get; set; }

        public OrderedPoolForm(OrderedPool pool)
        {
            // Setup form controls
            InitializeComponent();

            // Save settings
            SelectedPool = pool;
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
                if (pool.FilterLogic == LogicOperator.And)
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
                if (pool.NewCareerLength)
                {
                    var item = pool.CareerGenerator;
                    if (item != null && item != default(CareerGenerator))
                    {
                        SelectedNewCareer = item;
                        newCareerCheckBox.Checked = true;
                        careerGeneratorBox.SelectedIndex = CareerGens.FindIndex(x => x.Id == item.Id);
                    }
                }

                // Add filtering options
                if (pool.SoldierFilters != null)
                {
                    // Order them
                    var ordered = pool.SoldierFilters.OrderBy(x => x.Precedence);
                    foreach (var thing in ordered)
                    {
                        SelectionFilters.Add(thing);
                    }
                }

                // Add grouping options
                if (pool.SoldierGroups != null)
                {
                    // Order them
                    var ordered = pool.SoldierGroups.OrderBy(x => x.Precedence);
                    foreach (var thing in ordered)
                    {
                        SelectionGroups.Add(thing);
                    }
                }

                // Add sorting options
                if (pool.SoldierSorting != null)
                {
                    // Order them
                    var ordered = pool.SoldierSorting.OrderBy(x => x.Precedence);
                    foreach (var thing in ordered)
                    {
                        SelectionSorting.Add(thing);
                    }
                }

                // Add required specialties
                if (pool.Requirements != null)
                {
                    Requirements.AddRange(pool.Requirements.Select(x => x.Specialty));
                }
            }

            // Fill Filters and Sorting!
            FillFiltersListView();
            FillGroupsListView();
            FillSortingListView();
            FillSpecialtyListView();

            // Set form values for existing settings
            if (pool.RankId != 0)
            {
                // Grab Rank. This can be null if this pool is not saved yet!
                Rank rank = Ranks[pool.RankId];

                // Get rank index
                var index = rankSelect.Items.IndexOf(rank);
                if (index >= 0)
                {
                    rankSelect.SelectedIndex = index;
                }

                // Set probability
                existTrackBar.Value = pool.Probability;

                // Misc Settings
                useRankGradeCheckBox.Checked = pool.UseRankGrade;
                promotableCheckBox.Checked = pool.MustBePromotable;
                lockedCheckBox.Checked = pool.NotLockedInBillet;
                randomCheckBox.Checked = pool.SelectRandom;
                desireCheckBox.Checked = pool.GroupByDesire;
                inverseCheckBox.Checked = pool.InverseSpecialtyRequirements;
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
                    // Check for new experience items added!
                    if (thing.Selector == ClauseLeftSelector.SoldierExperience)
                        CheckExperience(thing.SelectorId);

                    // Add item to list
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

        private void FillGroupsListView()
        {
            // Reset listview
            groupingListView.Items.Clear();

            // Prepare update
            groupingListView.BeginUpdate();

            // Do we even have items to show?
            if (SelectionGroups.Count > 0)
            {
                // Order them
                int i = 0;

                // Add each item
                foreach (var thing in SelectionGroups)
                {
                    // Check for new experience items added!
                    if (thing.Selector == ClauseLeftSelector.SoldierExperience)
                        CheckExperience(thing.SelectorId);

                    // Add item to list
                    var text = (i > 0) ? "Then By:" : "Group By:";
                    var item = new ListViewItem(text);
                    item.SubItems.Add(GetNameFrom(thing.Selector, thing.SelectorId));
                    item.SubItems.Add(thing.Operator.ToString());
                    item.SubItems.Add(thing.RightValue.ToString());
                    item.Tag = thing;
                    groupingListView.Items.Add(item);
                    i++;
                }
            }

            // End update
            groupingListView.EndUpdate();
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

        private void FillSpecialtyListView()
        {
            // Prepare update
            listView1.BeginUpdate();

            // Reset listview
            listView1.Items.Clear();

            // Create ListView items
            foreach (Specialty spec in Requirements.OrderBy(x => x.Code))
            {
                ListViewItem item = new ListViewItem(spec.Code);
                item.SubItems.Add(spec.Name);
                item.Tag = spec;
                listView1.Items.Add(item);
            }

            // End update
            listView1.EndUpdate();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            SelectedPool.RankId = ((Rank)rankSelect.SelectedItem).Id;
            SelectedPool.Probability = existTrackBar.Value;
            SelectedPool.UseRankGrade = useRankGradeCheckBox.Checked;
            SelectedPool.NewCareerLength = newCareerCheckBox.Checked;
            SelectedPool.MustBePromotable = promotableCheckBox.Checked;
            SelectedPool.NotLockedInBillet = lockedCheckBox.Checked;
            SelectedPool.FilterLogic = (andRadioButton.Checked) ? LogicOperator.And : LogicOperator.Or;
            SelectedPool.GroupByDesire = desireCheckBox.Checked;
            SelectedPool.SelectRandom = randomCheckBox.Checked;
            SelectedPool.InverseSpecialtyRequirements = inverseCheckBox.Checked;

            // Save Settings
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    // Insert or Update record
                    if (SelectedPool.Id == 0)
                        db.OrderedPools.Add(SelectedPool);
                    else
                        db.OrderedPools.Update(SelectedPool);

                    // ***************************************************************************
                    // Apply required specialties
                    // ***************************************************************************
                    DeleteQueryBuilder query;
                    IEnumerable<int> currentItems = SelectedPool.Requirements.Select(x => x.SpecialtyId);
                    IEnumerable<int> selectedItems = Requirements.Select(x => x.Id).ToList();

                    if (RequirementsChanged)
                    {
                        // Remove
                        foreach (int id in currentItems.Except(selectedItems))
                        {
                            query = new DeleteQueryBuilder(db);
                            query.From(nameof(OrderedPoolSpecialty))
                                .Where("OrderedPoolId", Comparison.Equals, SelectedPool.Id)
                                .And("SpecialtyId", Comparison.Equals, id);
                            query.Execute();
                        }

                        // Add
                        foreach (int id in selectedItems.Except(currentItems))
                        {
                            var spec = new OrderedPoolSpecialty();
                            spec.OrderedPool = SelectedPool;
                            spec.SpecialtyId = id;
                            db.OrderedPoolSpecialties.Add(spec);
                        }
                    }

                    // ***************************************************************************
                    // Apply Filters
                    // ***************************************************************************

                    int i = 0;
                    if (FiltersChanged)
                    {
                        // Remove All
                        query = new DeleteQueryBuilder(db);
                        query.From(nameof(OrderedPoolFilter)).Where("OrderedPoolId", Comparison.Equals, SelectedPool.Id);
                        query.Execute();

                        // Re-add what we have
                        foreach (var item in SelectionFilters)
                        {
                            item.OrderedPool = SelectedPool;
                            item.Precedence = i;
                            db.OrderedPoolFilters.Add(item);
                            i++;
                        }
                    }

                    // ***************************************************************************
                    // Apply Grouping
                    // ***************************************************************************

                    if (GroupingChanged)
                    {
                        // Remove All
                        query = new DeleteQueryBuilder(db);
                        query.From(nameof(BilletSelectionGroup)).Where("OrderedPoolId", Comparison.Equals, SelectedPool.Id);
                        query.Execute();

                        // Re-add what we have
                        i = 0;
                        foreach (var item in SelectionGroups)
                        {
                            item.OrderedPool = SelectedPool;
                            item.Precedence = i;
                            db.OrderedPoolGroups.Add(item);
                            i++;
                        }
                    }

                    // ***************************************************************************
                    // Apply Experience Sorting
                    // ***************************************************************************

                    if (SortingChanged)
                    {
                        // Remove All
                        query = new DeleteQueryBuilder(db);
                        query.From(nameof(BilletSelectionSorting)).Where("OrderedPoolId", Comparison.Equals, SelectedPool.Id);
                        query.Execute();

                        // Re-add what we have
                        i = 0;
                        foreach (var item in SelectionSorting)
                        {
                            item.OrderedPool = SelectedPool;
                            item.Precedence = i;
                            db.OrderedPoolSorting.Add(item);
                            i++;
                        }
                    }

                    // Refresh object properties
                    db.OrderedPools.Refresh(SelectedPool);

                    // Save
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ExceptionHandler.ShowException(ex);
                }
            }

            // Close form
            this.DialogResult = DialogResult.OK;
            this.Close();
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

        private void newCareerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            careerGeneratorBox.Enabled = newCareerCheckBox.Checked;
        }

        private void existTrackBar_ValueChanged(object sender, EventArgs e)
        {
            existProbLabel.Text = $"{existTrackBar.Value}%";
        }

        #region Form Styling

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

        #region Context Menu Events

        private void sortingListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Get selected item
            ListViewItem item = sortingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            AbstractSort filter = (OrderedPoolSorting)item.Tag;
            using (var form = new SoldierSortingForm(filter))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Add item and re-draw
                    FillSortingListView();

                    // Flag changes made
                    SortingChanged = true;
                }
            }
        }

        private void addNewSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sort = new OrderedPoolSorting();
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

                    // Flag changes made
                    SortingChanged = true;

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
            var sortOption = (OrderedPoolSorting)item.Tag;
            int index = SelectionSorting.FindIndex(x => x.IsDuplicateOf(sortOption));
            if (index < 0) return;

            // Flag changes made
            SortingChanged = true;

            // Remove item from list
            SelectionSorting.RemoveAt(index);
            FillSortingListView();
        }

        private void filterListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Get selected item
            ListViewItem item = filterListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            AbstractFilter filter = (OrderedPoolFilter)item.Tag;
            using (var form = new SoldierFilterForm(filter, false))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Add item and re-draw
                    FillFiltersListView();

                    // Flag changes made
                    FiltersChanged = true;
                }
            }
        }

        private void addNewFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new filter
            var filter = new OrderedPoolFilter();

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

                    // Flag changes made
                    FiltersChanged = true;

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
            var option = (OrderedPoolFilter)item.Tag;
            int index = SelectionFilters.FindIndex(x => x.IsDuplicateOf(option));
            if (index < 0) return;

            // Flag changes made
            FiltersChanged = true;

            // Remove item from list
            SelectionFilters.RemoveAt(index);

            // Redraw filterListView
            FillFiltersListView();
        }

        private void groupingListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Get selected item
            ListViewItem item = groupingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            AbstractFilter filter = (OrderedPoolGroup)item.Tag;
            using (var form = new SoldierFilterForm(filter, false))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Add item and re-draw
                    FillGroupsListView();

                    // Flag changes made
                    GroupingChanged = true;
                }
            }
        }

        private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new filter
            var filter = new OrderedPoolGroup();

            using (var form = new SoldierFilterForm(filter, false))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // Check for duplicates
                    if (SelectionGroups.FindIndex(x => x.IsDuplicateOf(filter)) > -1)
                    {
                        ShowErrorMessage("No duplicates allowed. Group option already exists!");
                        return;
                    }

                    // Flag changes made
                    GroupingChanged = true;

                    // Add item and re-draw
                    SelectionGroups.Add(filter);
                    FillFiltersListView();
                }
            }
        }

        private void removeGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get selected item
            ListViewItem item = groupingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            // Get item index
            var option = (OrderedPoolGroup)item.Tag;
            int index = SelectionGroups.FindIndex(x => x.IsDuplicateOf(option));
            if (index < 0) return;

            // Flag changes made
            GroupingChanged = true;

            // Remove item from list
            SelectionGroups.RemoveAt(index);

            // Redraw filterListView
            FillGroupsListView();
        }

        private void contextMenuStrip1_Opening_1(object sender, CancelEventArgs e)
        {
            // Get selected item
            if (sortingListView.Enabled)
            {
                ListViewItem item = sortingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
                removeItemToolStripMenuItem.Enabled = (item != null);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            // Get selected item
            ListViewItem item = filterListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            removeFilterItemToolStripMenuItem.Enabled = (item != null);
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            // Get selected item
            ListViewItem item = groupingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            removeGroupToolStripMenuItem.Enabled = (item != null);
        }

        private void editRequiredSpecialtiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (RequiredSpecialtiesForm form = new RequiredSpecialtiesForm(Requirements))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Flag change
                    RequirementsChanged = true;

                    // Update listView
                    FillSpecialtyListView();
                }
            }
        }

        #endregion Context Menu Events

        private void filterListView_DragDrop(object sender, DragEventArgs e)
        {
            // Clear stuff
            SelectionFilters.Clear();

            // Re-apply text
            int i = 0;
            var op = (andRadioButton.Checked) ? "And: " : "Or: ";
            foreach (ListViewItem item in filterListView.Items)
            {
                // Re-add item
                var thing = (OrderedPoolFilter)item.Tag;
                SelectionFilters.Add(thing);

                // Update display text
                item.Text = (i > 0) ? op : "Where:"; ;
                i++;
            }

            // Flag a change
            FiltersChanged = true;
        }

        private void groupingListView_DragDrop(object sender, DragEventArgs e)
        {
            // Clear stuff
            SelectionGroups.Clear();

            // Re-apply text
            int i = 0;
            foreach (ListViewItem item in groupingListView.Items)
            {
                // Re-add item
                var thing = (OrderedPoolGroup)item.Tag;
                SelectionGroups.Add(thing);

                // Update display text
                item.Text = (i > 0) ? "Then By:" : "Group By:";
                i++;
            }

            // Flag a change
            GroupingChanged = true;
        }

        private void sortingListView_DragDrop(object sender, DragEventArgs e)
        {
            // Clear stuff
            SelectionSorting.Clear();

            // Re-apply text
            int i = 0;
            foreach (ListViewItem item in sortingListView.Items)
            {
                // Re-add item
                var thing = (OrderedPoolSorting)item.Tag;
                SelectionSorting.Add(thing);

                // Update display text
                item.Text = (i > 0) ? "Then By:" : "Order By:";
                i++;
            }

            // Flag a change
            SortingChanged = true;
        }

        private void randomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = randomCheckBox.Checked;
            sortingListView.Enabled = !enabled;
        }
    }
}