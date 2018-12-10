using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrossLite.QueryBuilder;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public partial class BilletEditorForm : Form
    {
        protected UnitTemplate Template { get; set; }

        protected Billet Billet { get; set; }

        protected List<Specialty> Requirements { get; set; } = new List<Specialty>();

        protected List<BilletExperience> ExperienceGiven { get; set; } = new List<BilletExperience>();

        protected List<BilletSelectionGroup> SelectionGrouping { get; set; } = new List<BilletSelectionGroup>();

        protected List<BilletSelectionSorting> SelectionSorting { get; set; } = new List<BilletSelectionSorting>();

        protected List<BilletSelectionFilter> SelectionFilters { get; set; } = new List<BilletSelectionFilter>();

        protected Dictionary<int, Experience> Experience { get; set; }

        protected bool ExperienceChanged { get; set; }

        protected bool GroupingChanged { get; set; }

        protected bool SortingChanged { get; set; }

        protected bool FiltersChanged { get; set; }

        public BilletEditorForm(UnitTemplate template, Billet billet)
        {
            // Setup form controls
            InitializeComponent();
            var symbol = new string(Encoding.Default.GetChars(new byte[] { 134 }));
            groupBox9.Text += String.Concat(" ", symbol);
            specialLabel.Text = String.Concat(symbol, " ", specialLabel.Text);
            inverseCheckBox.BackColor = Color.Transparent;

            // Set internal properties
            Template = template;
            Billet = billet;

            // Fill Ranks
            FillRanksAndCatagories();

            // Set form fields if this is an existing billet
            if (billet.Id > 0)
            {
                // Update header text
                labelHeader.Text = "Editing Billet #" + billet.Id;

                // Update fields
                billetNameBox.Text = billet.Name;
                statureBox.Value = billet.Stature;
                maxTigBox.Value = billet.MaxTourLength;
                minTigBox.Value = billet.MinTourLength;
                earlyRetireCheckBox.Checked = billet.CanRetireEarly;
                earlyPromotionCheckBox.Checked = billet.CanBePromotedEarly;
                earlyLatteralCheckBox.Checked = billet.CanLateralEarly;
                inverseCheckBox.Checked = billet.InverseRequirements;
                repeatCheckBox.Checked = billet.Waiverable;
                zIndexBox.Value = billet.ZIndex;
                soldierSpawnSelect.SelectedIndex = (int)billet.Selection;
                demoteCheckBox.Checked = (billet.Selection == BilletSelection.RandomSoldierGenerator && billet.DemoteOverRanked);
                orRadioButton.Checked = (billet.ExperienceLogic == LogicOperator.Or);

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

                // Add generator settings
                Requirements.AddRange(billet.Requirements.Select(x => x.Specialty));
                FillSpecialtyListView();

                // Spawn Settings
                var spawn = billet.SpawnSettings.FirstOrDefault();
                if (spawn != null)
                {
                    // Set index
                    index = spawnGenSelect.Items.IndexOf(spawn.Generator);
                    if (index >= 0)
                    {
                        spawnGenSelect.SelectedIndex = index;
                    }
                }

                // Specialty
                var spec = billet.Specialties.FirstOrDefault();
                if (spec != null)
                {
                    specialtyCheckBox.Checked = true;

                    // Set index
                    index = specialtySelect.Items.IndexOf(spec.Specialty);
                    if (index >= 0)
                    {
                        specialtySelect.SelectedIndex = index;
                    }
                }

                // Experience
                ExperienceGiven.AddRange(billet.Experience);
                SelectionFilters.AddRange(billet.Filters.OrderBy(x => x.Precedence));
                SelectionSorting.AddRange(billet.Sorting.OrderBy(x => x.Precedence));
                SelectionGrouping.AddRange(billet.Grouping.OrderBy(x => x.Precedence));
                FillExperienceListView();
                FillSortingListView();
                FillGroupingListView();
                FillFiltersListView();
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

        private void FillGroupingListView()
        {
            // Reset listview
            groupingListView.Items.Clear();

            // Prepare update
            groupingListView.BeginUpdate();

            // Do we even have items to show?
            if (SelectionGrouping.Count > 0)
            {
                // Order them
                int i = 0;

                // Add each item
                foreach (var thing in SelectionGrouping)
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

        private void FillExperienceListView()
        {
            // Reset listview
            experienceListView.Items.Clear();

            // Prepare update
            experienceListView.BeginUpdate();

            // Do we even have items to show?
            if (ExperienceGiven.Count > 0)
            {
                // Order them
                int i = 0;
                var ordered = ExperienceGiven.OrderByDescending(x => x.Rate).ThenBy(x => x.ExperienceId);

                // Add each item
                foreach (var thing in ordered)
                {
                    // Check for new items added!
                    CheckExperience(thing.ExperienceId);

                    // Add item to list
                    var text = Experience[thing.ExperienceId].ToString();
                    var item = new ListViewItem(text);
                    item.SubItems.Add($"{thing.Rate.ToString()}x");
                    item.Tag = thing;
                    experienceListView.Items.Add(item);
                    i++;
                }
            }

            // End update
            experienceListView.EndUpdate();
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

                // Next, Get promotion pool
                var list = db.Echelons.OrderByDescending(x => x.HierarchyLevel).ToArray();
                promotionPoolSelect.Items.AddRange(list);
                promotionPoolSelect.SelectedIndex = 0;

                // Next, get spawn instruction
                // Finally, Get soldier generators
                foreach (BilletSelection gen in Enum.GetValues(typeof(BilletSelection)))
                {
                    soldierSpawnSelect.Items.Add(gen);
                }

                // Set default index
                soldierSpawnSelect.SelectedIndex = 0;

                // Finally, Get soldier generators
                foreach (var gen in db.SoldierGenerators)
                {
                    spawnGenSelect.Items.Add(gen);
                }

                if (spawnGenSelect.Items.Count > 0)
                    spawnGenSelect.SelectedIndex = 0;

                // Experience
                Experience = db.Experience.ToDictionary(x => x.Id, y => y);
            }

            // Fill Enum selects
            foreach (var val in Enum.GetValues(typeof(BilletFlag)).Cast<BilletFlag>())
            {
                billetFlagSelect.Items.Add(val);

                // Is this what we are editing?
                if (Billet.Flag == val)
                {
                    billetFlagSelect.SelectedIndex = billetFlagSelect.Items.Count - 1;
                }
            }
        }

        private bool CreatesNewSoldiers()
        {
            var selected = (BilletSelection)soldierSpawnSelect.SelectedIndex;
            if (selected == BilletSelection.RandomSoldierGenerator)
            {
                // Fetch spawn Generator
                SoldierGenerator gen = (SoldierGenerator)spawnGenSelect.SelectedItem;
                return gen.CreatesNewSoldiers;
            }

            return false;
        }

        private bool UsesCustomSpawnGenerator()
        {
            var selected = (BilletSelection)soldierSpawnSelect.SelectedIndex;
            return (selected == BilletSelection.RandomSoldierGenerator);
        }

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
                }

                // Set default indexies
                if (specialtySelect.Items.Count > 0)
                {
                    specialtySelect.SelectedIndex = 0;
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
            }
        }

        private void billetRangeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            billetRankRangeSelect.Enabled = billetRangeCheckBox.Checked;
        }

        private void specialtyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            specialtySelect.Enabled = specialtyCheckBox.Checked;

            // Billet must change MOS if its an entry level billet
            if (!specialtyCheckBox.Checked && CreatesNewSoldiers())
                specialtyCheckBox.Checked = true;
        }

        private void soldierSpawnSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UsesCustomSpawnGenerator())
            {
                // Billet must change MOS if its an entry level billet
                if (!specialtyCheckBox.Checked && CreatesNewSoldiers())
                    specialtyCheckBox.Checked = true;

                spawnGenSelect.Enabled = true;
                demoteCheckBox.Enabled = true;
                demoteCheckBox.Checked = Billet.DemoteOverRanked;
            }
            else
            {
                spawnGenSelect.Enabled = false;
                demoteCheckBox.Checked = false;
                demoteCheckBox.Enabled = false;
            }
        }

        private void spawnGenSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UsesCustomSpawnGenerator())
            {
                // Billet must change MOS if its an entry level billet
                if (!specialtyCheckBox.Checked && CreatesNewSoldiers())
                    specialtyCheckBox.Checked = true;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool createsNewSoldiers = CreatesNewSoldiers();

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
            else if (createsNewSoldiers && spawnGenSelect.SelectedIndex < 0)
            {
                ShowErrorMessage("No soldier spawn generator was selected!");
                return;
            }
            else if (maxTigBox.Value > 0 && minTigBox.Value > maxTigBox.Value)
            {
                ShowErrorMessage("Minimum tour length is greater than the Maximum!");
                return;
            }
            else if (String.IsNullOrWhiteSpace(billetNameBox.Text))
            {
                ShowErrorMessage("Invalid billet name entered!");
                return;
            }

            // Disable button
            saveButton.Enabled = false;

            // Save
            Billet.Name = billetNameBox.Text;
            Billet.RankId = ((Rank)billetRankSelect.SelectedItem).Id;
            Billet.MaxRankId = ((Rank)billetRankRangeSelect.SelectedItem).Id;
            Billet.MaxTourLength = (int)maxTigBox.Value;
            Billet.MinTourLength = (int)minTigBox.Value;
            Billet.Stature = (int)statureBox.Value;
            Billet.CanRetireEarly = earlyRetireCheckBox.Checked;
            Billet.CanBePromotedEarly = earlyPromotionCheckBox.Checked;
            Billet.CanLateralEarly = earlyLatteralCheckBox.Checked;
            Billet.Waiverable = repeatCheckBox.Checked;
            Billet.InverseRequirements = inverseCheckBox.Checked;
            Billet.UnitTypeId = Template.Id;
            Billet.BilletCatagoryId = ((BilletCatagory)billetCatSelect.SelectedItem).Id;
            Billet.PromotionPoolId = ((Echelon)promotionPoolSelect.SelectedItem).Id;
            Billet.Flag = (BilletFlag)billetFlagSelect.SelectedItem;
            Billet.ZIndex = (int)zIndexBox.Value;
            Billet.Selection = (BilletSelection)soldierSpawnSelect.SelectedIndex;
            Billet.DemoteOverRanked = demoteCheckBox.Checked;
            Billet.ExperienceLogic = (andRadioButton.Checked) ? LogicOperator.And : LogicOperator.Or;

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
                        // Extract Specialty ID
                        var specialty = ((Specialty)specialtySelect.SelectedItem);

                        // Fill billet specialty values
                        var current = Billet.Specialties.FirstOrDefault();
                        if (current != null)
                        {
                            if (current.SpecialtyId != specialty.Id)
                            {
                                current.SpecialtyId = specialty.Id;
                                db.BilletSpecialties.Update(current);
                            }
                        }
                        else
                        {
                            db.BilletSpecialties.Add(new BilletSpecialty(Billet, specialty));
                        }
                    }
                    else
                    {
                        db.BilletSpecialties.RemoveRange(Billet.Specialties);
                    }

                    // Apply Spawn Settings
                    if (UsesCustomSpawnGenerator())
                    {
                        // Fill spawn settings values
                        var current = Billet.SpawnSettings?.FirstOrDefault() ?? new BilletSpawnSetting();
                        current.Billet = Billet;
                        current.GeneratorId = ((SoldierGenerator)spawnGenSelect.SelectedItem).Id;
                        current.SpecialtyId = ((Specialty)specialtySelect.SelectedItem).Id;

                        // Add or Update record
                        db.BilletSpawnSettings.AddOrUpdate(current);
                    }
                    else
                    {
                        db.BilletSpawnSettings.RemoveRange(Billet.SpawnSettings);
                    }

                    // ***************************************************************************
                    // Apply required specialties
                    // ***************************************************************************
                    DeleteQueryBuilder query;
                    IEnumerable<int> currentItems = Billet.Requirements.Select(x => x.SpecialtyId);
                    IEnumerable<int> selectedItems = Requirements.Select(x => x.Id).ToList();

                    // Remove
                    foreach (int id in currentItems.Except(selectedItems))
                    {
                        query = new DeleteQueryBuilder(db);
                        query.From(nameof(BilletSpecialtyRequirement))
                            .Where("BilletId", Comparison.Equals, Billet.Id)
                            .And("SpecialtyId", Comparison.Equals, id);
                        query.Execute();
                    }

                    // Add
                    foreach (int id in selectedItems.Except(currentItems))
                    {
                        var spec = new BilletSpecialtyRequirement();
                        spec.BilletId = Billet.Id;
                        spec.SpecialtyId = id;
                        db.BilletSpecialtyRequirements.Add(spec);
                    }

                    // ***************************************************************************
                    // Apply Experience Given
                    // ***************************************************************************

                    if (ExperienceChanged)
                    {
                        // Remove All
                        query = new DeleteQueryBuilder(db);
                        query.From(nameof(BilletExperience))
                            .Where("BilletId", Comparison.Equals, Billet.Id);
                        query.Execute();

                        // Re-add what we have
                        foreach (var item in ExperienceGiven)
                        {
                            item.Billet = Billet;
                            db.BilletExperience.Add(item);
                        }
                    }

                    // ***************************************************************************
                    // Apply Experience Required
                    // ***************************************************************************

                    int i = 1;
                    if (FiltersChanged)
                    {
                        // Remove All
                        query = new DeleteQueryBuilder(db);
                        query.From(nameof(BilletSelectionFilter))
                            .Where("BilletId", Comparison.Equals, Billet.Id);
                        query.Execute();

                        // Re-add what we have
                        foreach (var item in SelectionFilters)
                        {
                            item.Billet = Billet;
                            item.Precedence = i;
                            db.BilletSelectionFilters.Add(item);
                            i++;
                        }
                    }

                    // ***************************************************************************
                    // Apply Experience Grouping
                    // ***************************************************************************

                    if (GroupingChanged)
                    {
                        // Remove All
                        query = new DeleteQueryBuilder(db);
                        query.From(nameof(BilletSelectionGroup))
                            .Where("BilletId", Comparison.Equals, Billet.Id);
                        query.Execute();

                        // Re-add what we have
                        i = 1;
                        foreach (var item in SelectionGrouping)
                        {
                            item.Billet = Billet;
                            item.Precedence = i;
                            db.BilletSelectionGroups.Add(item);
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
                        query.From(nameof(BilletSelectionSorting))
                            .Where("BilletId", Comparison.Equals, Billet.Id);
                        query.Execute();

                        // Re-add what we have
                        i = 1;
                        foreach (var item in SelectionSorting)
                        {
                            item.Billet = Billet;
                            item.Precedence = i;
                            db.BilletSelectionSorting.Add(item);
                            i++;
                        }
                    }

                    // Save
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ExceptionHandler.ShowException(ex);
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

        private void repeatCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void andRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Re-apply text
            int i = 0;
            var op = (andRadioButton.Checked) ? "And: " : "Or: ";
            foreach (ListViewItem item in filterListView.Items)
            {
                item.Text = (i > 0) ? op : "Where:";
                i++;
            }
        }

        #region Edit Required Specialties

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //bool ebl = listView1.SelectedItems.Count > 0;
            //removeSpecialtyToolStripMenuItem.Enabled = ebl;

            //ebl = listView1.Items.Count > 0;
            //removeAllSpecialtiesToolStripMenuItem.Enabled = ebl;
        }

        private void editRequiredSpecialtiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (BilletSpecialtiesForm form = new BilletSpecialtiesForm(Requirements))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Update listView
                    FillSpecialtyListView();
                }
            }
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

        #endregion Edit Required Specialties

        #region Add Experience Menu

        private void givesExpMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Ensure we have an item selected!
            var items = experienceListView.SelectedItems;
            removeItemToolStripMenuItem.Enabled = (items.Count != 0);
            editItemToolStripMenuItem.Enabled = (items.Count != 0);
        }

        private void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var exp = new BilletExperience() { Billet = Billet };
            using (ExperienceSelectForm form = new ExperienceSelectForm(exp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Check for duplicate
                    if (ExperienceGiven.Any(x => x.IsDuplicateOf(exp)))
                    {
                        ShowErrorMessage("Duplicate item entered. This Billet already offers this experience!");
                        return;
                    }

                    // Add item!
                    ExperienceGiven.Add(exp);

                    // Finally, Re-fill list!
                    FillExperienceListView();

                    // Flag change
                    ExperienceChanged = true;
                }
            }
        }

        /// <summary>
        /// Add experience given menu item click
        /// </summary>
        private void editItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure we have an item selected!
            if (experienceListView.SelectedItems.Count == 0)
                return;

            // Get selected item
            var selected = experienceListView.SelectedItems[0].Tag as BilletExperience;
            if (selected == null)
                return;

            // open editor!
            using (ExperienceSelectForm form = new ExperienceSelectForm(selected))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Finally, Re-fill list!
                    FillExperienceListView();

                    // Flag change
                    ExperienceChanged = true;
                }
            }
        }

        /// <summary>
        /// Remove experience given menu item click
        /// </summary>
        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure we have an item selected!
            if (experienceListView.SelectedItems.Count == 0)
                return;

            // Get selected item
            var selected = experienceListView.SelectedItems[0].Tag as BilletExperience;
            if (selected == null)
                return;

            // Remove item
            int index = ExperienceGiven.FindIndex(x => x.IsDuplicateOf(selected));
            if (index >= 0)
            {
                ExperienceGiven.RemoveAt(index);

                // Refill list view
                FillExperienceListView();

                // Flag change
                ExperienceChanged = true;
            }
        }

        #endregion #region Add Experience Menu

        #region #region Edit Experience Required Menu

        private void requiredExpMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Ensure we have an item selected!
            var items = filterListView.SelectedItems;
            editItemToolStripMenuItem1.Enabled = (items.Count != 0);
            removeItemToolStripMenuItem1.Enabled = (items.Count != 0);
        }

        private void addRequiredExperienceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var filter = new BilletSelectionFilter() { Billet = Billet };

            using (var form = new SoldierFilterForm(filter, false))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    if (SelectionFilters.FindIndex(x => x.IsDuplicateOf(filter)) > -1)
                    {
                        ShowErrorMessage("No duplicates allowed. Filter option already exists!");
                        return;
                    }

                    // Add item
                    SelectionFilters.Add(filter);

                    // Finally, Re-fill list!
                    FillFiltersListView();

                    // Flag change
                    FiltersChanged = true;
                }
            }
        }

        /// <summary>
        /// Edit experience required menu item click
        /// </summary>
        private void editRequiredExperienceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure we have an item selected!
            if (filterListView.SelectedItems.Count == 0)
                return;

            // Get selected item
            var selected = filterListView.SelectedItems[0].Tag as BilletSelectionFilter;
            if (selected == null)
                return;

            // open editor!
            using (SoldierFilterForm form = new SoldierFilterForm(selected, false))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Finally, Re-fill list!
                    FillFiltersListView();

                    // Flag change
                    FiltersChanged = true;
                }
            }
        }

        /// <summary>
        /// Remove experience Required menu item click
        /// </summary>
        private void removeItemToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Make sure we have an item selected!
            if (filterListView.SelectedItems.Count == 0)
                return;

            // Get selected item
            var selected = filterListView.SelectedItems[0].Tag as BilletSelectionFilter;
            if (selected == null)
                return;

            // Remove item
            int index = SelectionFilters.FindIndex(x => x.IsDuplicateOf(selected));
            if (index >= 0)
            {
                SelectionFilters.RemoveAt(index);

                // Refill list view
                FillFiltersListView();

                // Flag change
                FiltersChanged = true;
            }
        }

        #endregion Edit Experience Required Menu

        #region Edit Experience Sorting Menu

        /// <summary>
        /// Edit experience sorting menu item click
        /// </summary>
        private void addSortingItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sort = new BilletSelectionSorting() { Billet = Billet };

            using (var form = new SoldierSortingForm(sort))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    if (SelectionSorting.FindIndex(x => x.IsDuplicateOf(sort)) > -1)
                    {
                        ShowErrorMessage("No duplicates allowed. Sorting option already exists!");
                        return;
                    }

                    // Add item
                    SelectionSorting.Add(sort);

                    // Finally, Re-fill list!
                    FillSortingListView();

                    // Flag change
                    SortingChanged = true;
                }
            }
        }

        private void editSortingItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure we have an item selected!
            if (sortingListView.SelectedItems.Count == 0)
                return;

            // Get selected item
            var selected = sortingListView.SelectedItems[0].Tag as BilletSelectionSorting;
            if (selected == null)
                return;

            // open editor!
            using (var form = new SoldierSortingForm(selected))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Finally, Re-fill list!
                    FillSortingListView();

                    // Flag change
                    SortingChanged = true;
                }
            }
        }

        /// <summary>
        /// Edit experience sorting menu item click
        /// </summary>
        private void removeItemToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Get selected item
            ListViewItem item = sortingListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            if (item == null) return;

            // Get item index
            var sortOption = (BilletSelectionSorting)item.Tag;
            int index = SelectionSorting.FindIndex(x => x.IsDuplicateOf(sortOption));
            if (index < 0) return;

            // Remove item from list
            SelectionSorting.RemoveAt(index);
            sortingListView.Items.Remove(item);

            // Flag change
            SortingChanged = true;

            // Re-apply text
            int i = 0;
            foreach (ListViewItem item2 in sortingListView.Items)
            {
                item2.Text = (i > 0) ? "Then By:" : "Order By:";
                i++;
            }
        }

        #endregion Edit Experience Sorting Menu

        #region Edit Experience Grouping Menu

        private void groupContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Ensure we have an item selected!
            var items = groupingListView.SelectedItems;
            editGroupItemToolStripMenuItem.Enabled = (items.Count != 0);
            removeItemToolStripMenuItem3.Enabled = (items.Count != 0);
        }

        private void addGroupingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filter = new BilletSelectionGroup() { Billet = Billet };

            using (var form = new SoldierFilterForm(filter, true))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    if (SelectionGrouping.FindIndex(x => x.IsDuplicateOf(filter)) > -1)
                    {
                        ShowErrorMessage("No duplicates allowed. Grouping option already exists!");
                        return;
                    }

                    // Add item
                    SelectionGrouping.Add(filter);

                    // Finally, Re-fill list!
                    FillGroupingListView();

                    // Flag change
                    GroupingChanged = true;
                }
            }
        }

        private void editGroupItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure we have an item selected!
            if (groupingListView.SelectedItems.Count == 0)
                return;

            // Get selected item
            var selected = groupingListView.SelectedItems[0].Tag as BilletSelectionGroup;
            if (selected == null)
                return;

            // open editor!
            using (var form = new SoldierFilterForm(selected, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Finally, Re-fill list!
                    FillGroupingListView();

                    // Flag change
                    GroupingChanged = true;
                }
            }
        }

        private void removeItemToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Make sure we have an item selected!
            if (groupingListView.SelectedItems.Count == 0)
                return;

            // Get selected item
            var selected = groupingListView.SelectedItems[0].Tag as BilletSelectionGroup;
            if (selected == null)
                return;

            // Remove item
            int index = SelectionGrouping.FindIndex(x => x.IsDuplicateOf(selected));
            if (index >= 0)
            {
                SelectionGrouping.RemoveAt(index);

                // Refill list view
                FillGroupingListView();

                // Flag change
                GroupingChanged = true;
            }
        }

        #endregion Edit Experience Grouping Menu

        #region Column Width Change Events

        private void sortingListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = sortingListView.Columns[e.ColumnIndex].Width;
        }

        private void filterListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = filterListView.Columns[e.ColumnIndex].Width;
        }

        private void groupingListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = groupingListView.Columns[e.ColumnIndex].Width;
        }

        #endregion Column Width Change Events

        #region Drag and Drop Events

        private void sortingListView_DragDrop(object sender, DragEventArgs e)
        {
            // Re-apply text
            int i = 0;
            foreach (ListViewItem item in sortingListView.Items)
            {
                item.Text = (i > 0) ? "Then By:" : "Order By:";
                i++;
            }
        }

        private void groupingListView_DragDrop(object sender, DragEventArgs e)
        {
            // Re-apply text
            int i = 0;
            foreach (ListViewItem item in groupingListView.Items)
            {
                item.Text = (i > 0) ? "Then By:" : "Group By:";
                i++;
            }
        }

        private void sortingListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
            {
                var items = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));
                var tag = items[0].Tag as BilletSelectionSorting;
                if (tag == null)
                {

                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void groupingListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
            {
                var items = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));
                var tag = items[0].Tag as BilletSelectionGroup;
                if (tag == null)
                {
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void sortingListView_DragOver(object sender, DragEventArgs e)
        {
            //sortingListView_DragEnter(sender, e);
        }

        private void groupingListView_DragOver(object sender, DragEventArgs e)
        {
            //groupingListView_DragEnter(sender, e);
        }

        #endregion

        #region Double Mouse Click Events

        private void filterListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editRequiredExperienceToolStripMenuItem_Click(sender, e);
        }

        private void sortingListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editSortingItemToolStripMenuItem_Click(sender, e);
        }

        private void groupingListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editGroupItemToolStripMenuItem_Click(sender, e);
        }

        private void experienceListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editItemToolStripMenuItem_Click(sender, e);
        }

        #endregion

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
    }
}
