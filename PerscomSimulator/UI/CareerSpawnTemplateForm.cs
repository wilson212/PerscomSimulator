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
    public partial class CareerSpawnTemplateForm : Form
    {
        public CareerSpawnTemplateForm()
        {
            // Setup form controls
            InitializeComponent();

            // Set chart colors
            chart1.Series[0].BorderColor = FormStyling.LINE_COLOR_DARK;
            chart1.Series[0].Color = FormStyling.LINE_COLOR_LIGHT;
        }

        #region Panel Border Painting

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        #endregion

        private void spawnGenSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
