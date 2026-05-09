using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class FactionEditorForm : RadForm
    {
        public FactionEditorForm()
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

        private void RankTileElement_Click(object sender, EventArgs e)
        {
            using (RankGradeEditorForm form = new RankGradeEditorForm())
            {
                form.ShowDialog(this);
            }
        }
    }
}
