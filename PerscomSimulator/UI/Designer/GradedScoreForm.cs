using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Perscom.Database;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class GradedScoreForm : RadForm
    {
        /// <summary>
        /// The selected method (ClauseLeftSelector) after the user clicks Save
        /// </summary>
        public ClauseLeftSelector SelectedMethod { get; private set; }

        /// <summary>
        /// The selected value function (SoldierFunction) after the user clicks Save
        /// </summary>
        public SoldierFunction SelectedValue { get; private set; }

        /// <summary>
        /// The selected comparison operator after the user clicks Save
        /// </summary>
        public ComparisonOperator SelectedOperator { get; private set; }

        /// <summary>
        /// The required value after the user clicks Save
        /// </summary>
        public int RequiredValue { get; private set; }

        /// <summary>
        /// The points value after the user clicks Save
        /// </summary>
        public int PointsValue { get; private set; }

        /// <summary>
        /// Creates a new GradedScoreForm in Add mode
        /// </summary>
        public GradedScoreForm()
        {
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            // Populate Method Operator with ClauseLeftSelector
            PopulateDropDown(methodDropDownList, typeof(ClauseLeftSelector));

            // Populate Value Selection with SoldierFunction
            PopulateDropDown(valueDropDownList, typeof(SoldierFunction));

            // Populate Operator with ComparisonOperator
            PopulateDropDown(operatorDropDownList, typeof(ComparisonOperator));

            // Wire events
            saveButton.Click += saveButton_Click;
            headerPanel.Paint += headerPanel_Paint;
            bottomPanel.Paint += bottomPanel_Paint;
        }

        /// <summary>
        /// Creates a new GradedScoreForm in Edit mode with pre-populated values
        /// </summary>
        public GradedScoreForm(ClauseLeftSelector method, SoldierFunction value,
            ComparisonOperator op, int requiredValue, int points) : this()
        {
            // Pre-select method
            foreach (RadListDataItem item in methodDropDownList.Items)
            {
                if (item.Tag is ClauseLeftSelector cls && cls == method)
                {
                    methodDropDownList.SelectedItem = item;
                    break;
                }
            }

            // Pre-select value
            foreach (RadListDataItem item in valueDropDownList.Items)
            {
                if (item.Tag is SoldierFunction sf && sf == value)
                {
                    valueDropDownList.SelectedItem = item;
                    break;
                }
            }

            // Pre-select operator
            foreach (RadListDataItem item in operatorDropDownList.Items)
            {
                if (item.Tag is ComparisonOperator co && co == op)
                {
                    operatorDropDownList.SelectedItem = item;
                    break;
                }
            }

            expLvlSpinEditor.Value = requiredValue;
            scoreSpinEditor.Value = points;
        }

        /// <summary>
        /// Populates a RadDropDownList with all values from the given enum type
        /// </summary>
        private void PopulateDropDown(RadDropDownList dropDown, Type enumType)
        {
            dropDown.Items.Clear();
            dropDown.DropDownStyle = RadDropDownStyle.DropDownList;

            foreach (var value in Enum.GetValues(enumType))
            {
                dropDown.Items.Add(new RadListDataItem
                {
                    Tag = value,
                    Text = Enum.GetName(enumType, value)
                });
            }

            if (dropDown.Items.Count > 0)
                dropDown.SelectedIndex = 0;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // ValidateAndAlertUserOnFail method selection
            if (methodDropDownList.SelectedItem == null || methodDropDownList.SelectedItem.Tag is not ClauseLeftSelector)
            {
                MessageBox.Show("Please select a method.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ValidateAndAlertUserOnFail value selection
            if (valueDropDownList.SelectedItem == null || valueDropDownList.SelectedItem.Tag is not SoldierFunction)
            {
                MessageBox.Show("Please select a value.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ValidateAndAlertUserOnFail operator selection
            if (operatorDropDownList.SelectedItem == null || operatorDropDownList.SelectedItem.Tag is not ComparisonOperator)
            {
                MessageBox.Show("Please select an operator.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedMethod = (ClauseLeftSelector)methodDropDownList.SelectedItem.Tag;
            SelectedValue = (SoldierFunction)valueDropDownList.SelectedItem.Tag;
            SelectedOperator = (ComparisonOperator)operatorDropDownList.SelectedItem.Tag;
            RequiredValue = (int)expLvlSpinEditor.Value;
            PointsValue = (int)scoreSpinEditor.Value;

            DialogResult = DialogResult.OK;
            Close();
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
    }
}
