﻿using CrossLite.QueryBuilder;
using Perscom.Database;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Perscom
{
    public partial class SoldierSortingForm : Form
    {
        private AbstractSort Selected { get; set; }

        public SoldierSortingForm(AbstractSort item)
        {
            // Create controls
            InitializeComponent();

            // Fill selected variable
            Selected = item ?? throw new ArgumentException("Item cannot be null", "item");

            // Set value first
            sortingDirectionBox.SelectedIndex = (int)Selected.Direction;

            // Finally add methods
            foreach (var val in Enum.GetValues(typeof(ClauseLeftSelector)).Cast<ClauseLeftSelector>())
            {
                methodSelectBox.Items.Add(val);

                // Is this what we are editing?
                if (Selected.Selector == val)
                {
                    methodSelectBox.SelectedIndex = methodSelectBox.Items.Count - 1;
                }
            }

            // Fill select box
            FillSelectBox(false);
            addButton.Enabled = (Selected.SelectorId == (int)ClauseLeftSelector.SoldierExperience);
        }

        private void FillSelectBox(bool methodChanged)
        {
            // Clear old items first!
            selectorSelectBox.Items.Clear();

            // Grab selection mode
            var selector = (ClauseLeftSelector)methodSelectBox.SelectedIndex;
            switch (selector)
            {
                case ClauseLeftSelector.SoldierValue:
                    // Add methods
                    foreach (var val in Enum.GetValues(typeof(SoldierFunction)).Cast<SoldierFunction>())
                    {
                        selectorSelectBox.Items.Add(val);

                        // Is this what we are editing?
                        if (!methodChanged && Selected.SelectorId == (int)val)
                        {
                            selectorSelectBox.SelectedIndex = selectorSelectBox.Items.Count - 1;
                        }
                    }
                    break;

                case ClauseLeftSelector.SoldierPosition:
                    // Add methods
                    foreach (var val in Enum.GetValues(typeof(PositionFunction)).Cast<PositionFunction>())
                    {
                        selectorSelectBox.Items.Add(val);

                        // Is this what we are editing?
                        if (!methodChanged && Selected.SelectorId == (int)val)
                        {
                            selectorSelectBox.SelectedIndex = selectorSelectBox.Items.Count - 1;
                        }
                    }
                    break;

                case ClauseLeftSelector.SoldierExperience:
                    // Add items
                    using (AppDatabase db = new AppDatabase())
                    {
                        foreach (var exp in db.Experience.OrderBy(x => x.Name))
                        {
                            selectorSelectBox.Items.Add(exp);

                            // Is this what we are editing?
                            if (!methodChanged && Selected.SelectorId == exp.Id)
                            {
                                selectorSelectBox.SelectedIndex = selectorSelectBox.Items.Count - 1;
                            }
                        }
                    }
                    break;
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void methodSelectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillSelectBox(true);
            addButton.Enabled = (methodSelectBox.SelectedIndex == 2);
        }

        private void selectorSelectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addButton.Enabled = (selectorSelectBox.SelectedIndex == (int)ClauseLeftSelector.SoldierExperience);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using (ExperienceForm form = new ExperienceForm())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    FillSelectBox(false);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Ensure we have an item selected!
            if (selectorSelectBox.SelectedIndex == -1)
            {
                ShowErrorMessage("A left selector item has not been selected!");
                return;
            }

            // Grab selection mode
            var selector = (ClauseLeftSelector)methodSelectBox.SelectedIndex;
            switch (selector)
            {
                case ClauseLeftSelector.SoldierValue:
                    var item1 = (SoldierFunction)selectorSelectBox.SelectedItem;
                    Selected.Selector = ClauseLeftSelector.SoldierValue;
                    Selected.SelectorId = (int)item1;
                    break;

                case ClauseLeftSelector.SoldierPosition:
                    var item2 = (SoldierFunction)selectorSelectBox.SelectedItem;
                    Selected.Selector = ClauseLeftSelector.SoldierPosition;
                    Selected.SelectorId = (int)item2;
                    break;

                case ClauseLeftSelector.SoldierExperience:
                    var item3 = selectorSelectBox.SelectedItem as Experience;
                    Selected.Selector = ClauseLeftSelector.SoldierExperience;
                    Selected.SelectorId = item3.Id;
                    break;
            }

            // Set value
            Selected.Direction = (Sorting)sortingDirectionBox.SelectedIndex;

            // Close dialog and Signal OK
            this.DialogResult = DialogResult.OK;
            this.Close();
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
    }
}
