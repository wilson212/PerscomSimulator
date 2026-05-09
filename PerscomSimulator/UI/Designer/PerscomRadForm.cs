using System.Windows.Forms;

namespace Perscom
{
    public partial class PerscomRadForm : Telerik.WinControls.UI.RadForm
    {
        public PerscomRadForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds the darker border line color between the header panel and the contents
        /// panel
        /// </summary>
        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooterAlternative(bottomPanel, e);
        }
    }
}
