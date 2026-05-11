using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Perscom
{
    public partial class SelectionProcedureForm : Telerik.WinControls.UI.RadForm
    {
        public SelectionProcedureForm()
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);
        }
    }
}
