using Perscom.Database;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Perscom
{
    public partial class ExperienceNameChangeForm : Form
    {
        public Experience Selected { get; set; }

        public ExperienceNameChangeForm(Experience item)
        {
            InitializeComponent();
            Selected = item;

            // Set initial value
            textBox1.Text = item.Name;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;

            // Create the regular expression to filter name
            string pattern = "s/[^A-Za-z0-9 ]+//g";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            name = regex.Replace(name, "");

            // Ensure name length
            if (name.Length < 3)
            {
                ShowErrorMessage("Experience item name must be at least 3 characters in length!");
                return;
            }

            // Save values
            Selected.Name = name;

            // Close form
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

        private void ExperienceNameChangeForm_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.Select(0, textBox1.Text.Length);
        }
    }
}
