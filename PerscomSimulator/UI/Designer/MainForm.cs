using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class MainForm : RadForm
    {
        public MainForm()
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            // Button styling
            //FormStyling.StyleButtonFluentBlue(btnDesignUnit);
            FormStyling.StyleButtonRed(btnDelete);
            FormStyling.StyleButtonFluentBlue(btnRunSim);
            FormStyling.StyleButtonDarkBlue(btnViewResult);
            FormStyling.StyleButtonDarkBlue(btnVerify);
            FormStyling.StyleButtonRed(btnClear);
            FormStyling.StyleButtonFluentBlue(btnNew);

            // Register for events (We cannot do this from the designer)
            AddPersonaMenuItem.Click += AddPersonaMenuItem_Click;
        }

        private void AddPersonaMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new PersonaEditorForm())
            {
                frm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Adds the darker border line color between the header panel and the contents
        /// panel
        /// </summary>
        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormSubHeader(panel3, e);
        }

        /// <summary>
        /// Adds the darker border line color between the footer panel and the contents
        /// panel
        /// </summary>
        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooterDark(bottomPanel, e);
        }

        #region Tile Element Click (overview page)

        private void FactionTileElement_Click(object sender, EventArgs e)
        {
            using (FactionEditorForm frm = new FactionEditorForm())
            {
                frm.ShowDialog(this);
            }
        }

        private void PersonaTileElement_Click(object sender, EventArgs e)
        {
            using (PersonaEditorForm frm = new PersonaEditorForm())
            {
                frm.ShowDialog(this);
            }
        }

        private void TraitsTileElement_Click(object sender, EventArgs e)
        {

        }

        private void ExperienceTileElement_Click(object sender, EventArgs e)
        {

        }

        #endregion Tile Element Click (overview page)

        #region Context Menus Opening

        private void CareerContextMenu_DropDownOpening(object sender, CancelEventArgs e)
        {
            var itemSelected = CareerLengthGridView.CurrentRow != null;
            EditCareerMenuItem.Enabled = itemSelected;
            DeleteCareerMenuItem.Enabled = itemSelected;
        }

        private void SoldierContextMenu_DropDownOpening(object sender, CancelEventArgs e)
        {
            var itemSelected = SoldierGridView.CurrentRow != null;
            EditSoldierMenuItem.Enabled = itemSelected;
            DeleteSoldierMenuItem.Enabled = itemSelected;
        }

        #endregion Context Menus Opening
    }
}
