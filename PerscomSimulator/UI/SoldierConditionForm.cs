using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perscom
{
    public partial class SoldierConditionForm : Form
    {
        public SoldierFilter Filter { get; set; }

        public ConditionOperator Operator { get; set; }

        public int Value { get; set; }

        public SoldierConditionForm(SoldierFilter filter, ConditionOperator @operator, int value = 0)
        {
            InitializeComponent();

            // Add filter types
            foreach (var val in Enum.GetValues(typeof(SoldierFilter)))
            {
                filterTypeSelect.Items.Add(val);
            }

            // Add operator types
            foreach (var val in Enum.GetValues(typeof(ConditionOperator)))
            {
                operatorSelectBox.Items.Add(val);
            }

            // Set indexies
            filterTypeSelect.SelectedIndex = (int)filter;
            operatorSelectBox.SelectedIndex = (int)@operator;
            numericUpDown1.SetValueInRange(value);
        }

        #region Panel Border Painting

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
            base.OnPaint(e);
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        #endregion

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Filter = (SoldierFilter)Enum.Parse(typeof(SoldierFilter), filterTypeSelect.SelectedItem.ToString());
            this.Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), operatorSelectBox.SelectedItem.ToString());
            this.Value = (int)numericUpDown1.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
