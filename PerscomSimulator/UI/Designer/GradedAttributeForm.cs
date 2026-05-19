using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class GradedAttributeForm : RadForm
    {
        /// <summary>
        /// The selected attribute type after the user clicks Save
        /// </summary>
        public AttributeType SelectedAttribute { get; private set; }

        /// <summary>
        /// The max points value after the user clicks Save
        /// </summary>
        public int MaxPoints { get; private set; }

        private HashSet<AttributeType> _excludedAttributes = new();

        /// <summary>
        /// Creates a new GradedAttributeForm in Add mode, excluding attributes already on the board
        /// </summary>
        public GradedAttributeForm(IEnumerable<AttributeType> excludedAttributes) : this()
        {
            _excludedAttributes = new HashSet<AttributeType>(excludedAttributes);
            PopulateAttributeDropDown(); // re-populate with exclusions applied
            if (attrDropDownList.Items.Count > 0)
                attrDropDownList.SelectedIndex = 0;
        }

        /// <summary>
        /// Creates a new GradedAttributeForm in Edit mode, excluding other attributes already on the board
        /// </summary>
        public GradedAttributeForm(AttributeType attribute, int weight, IEnumerable<AttributeType> excludedAttributes)
            : this(excludedAttributes)
        {
            // Pre-select the current attribute
            foreach (RadListDataItem item in attrDropDownList.Items)
            {
                if (item.Tag is AttributeType at && at == attribute)
                {
                    attrDropDownList.SelectedItem = item;
                    break;
                }
            }
            scoreSpinEditor.Value = weight;
        }

        /// <summary>
        /// Creates a new GradedAttributeForm in Add mode
        /// </summary>
        public GradedAttributeForm()
        {
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            PopulateAttributeDropDown();
            attrDropDownList.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates the attribute dropdown with all AttributeType enum values
        /// </summary>
        private void PopulateAttributeDropDown()
        {
            attrDropDownList.Items.Clear();
            attrDropDownList.DropDownStyle = RadDropDownStyle.DropDownList;

            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
            {
                if (_excludedAttributes.Contains(attr))
                    continue;

                attrDropDownList.Items.Add(new RadListDataItem
                {
                    Tag = attr,
                    Text = Enum.GetName(typeof(AttributeType), attr)
                });
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Validate selection
            if (attrDropDownList.SelectedItem == null || attrDropDownList.SelectedItem.Tag is not AttributeType)
            {
                MessageBox.Show("Please select an attribute.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedAttribute = (AttributeType)attrDropDownList.SelectedItem.Tag;
            MaxPoints = (int)scoreSpinEditor.Value;

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
            FormStyling.StyleFormFooterGray(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion
    }
}