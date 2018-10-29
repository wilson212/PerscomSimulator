using System;
using System.Drawing;
using System.Windows.Forms;
using Perscom.Database;

namespace Perscom
{
    public partial class SoldierEditForm : Form
    {
        protected Soldier Soldier { get; set; }

        /// <summary>
        /// SoldierEditForm constructor
        /// </summary>
        /// <param name="soldier"></param>
        public SoldierEditForm(Soldier soldier)
        {
            InitializeComponent();

            this.Soldier = soldier;
            textBox1.Text = soldier.FirstName;
            textBox2.Text = soldier.LastName;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Soldier.FirstName = textBox1.Text.Trim();
            Soldier.LastName = textBox2.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
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
    }
}
