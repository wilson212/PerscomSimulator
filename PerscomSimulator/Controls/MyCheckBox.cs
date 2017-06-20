using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perscom
{
    public partial class MyCheckBox : CheckBox
    {
        public AutoScaleMode AutoScaleMode = AutoScaleMode.Font;

        public MyCheckBox() : base()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
    }
}
