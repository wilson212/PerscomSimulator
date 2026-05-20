using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    /// <summary>
    /// Represents a form used to configure the Advisor component.
    /// </summary>
    public partial class AdvisorConfigForm : RadForm
    {
        /// <summary>
        /// A list of available models for the Gemini API.
        /// </summary>
        public List<string> Models { get; }

        /// <summary>
        /// The API key used for authenticating with the Gemini API.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// The selected model from the dropdown list.
        /// </summary>
        public string SelectedModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvisorConfigForm"/> class.
        /// </summary>
        public AdvisorConfigForm()
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            Models = [
                "gemini-flash-latest",
                "gemini-pro-latest",
                "gemini-2.5-flash",
                "gemini-3-flash-preview",
                "gemini-3.1-pro-preview"
            ];

            foreach (string model in Models)
            {
                modelDropDownList.Items.Add(model);
            }
            modelDropDownList.SelectedIndex = 0;
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooterDarker(bottomPanel, e);
            base.OnPaint(e);
        }

        /// <summary>
        /// Handles the click event of the saveButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            ApiKey = keyTextBox.Text?.Trim();
            SelectedModel = modelDropDownList.SelectedItem?.Text;

            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                RadMessageBox.Show("Please enter an API key.", "Validation", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
