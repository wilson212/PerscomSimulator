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
            FormStyling.StyleButtonRed(deleteDbSourceButton);
            FormStyling.StyleButtonFluentBlue(runSimButton);
            FormStyling.StyleButtonDarkBlue(loadSimButton);
            FormStyling.StyleButtonDarkBlue(verifyDbSourceButton);
            FormStyling.StyleButtonRed(clearDbSourceButton);
            FormStyling.StyleButtonFluentBlue(newDbSourceButton);

            // Register for events (We cannot do this from the designer)
            AddPersonaMenuItem.Click += AddPersonaMenuItem_Click;

            // Subscribe to the dropdown's selection change
            dbSourceDropDownList.SelectedIndexChanged += DbSourceDropDownList_SelectedIndexChanged;

            // Disable tiles and sim buttons until a DB source is selected
            //SetDatabaseDependentControlsEnabled(false);
        }

        private void SetDatabaseDependentControlsEnabled(bool enabled)
        {
            // Tile elements
            FactionTileElement.Enabled = enabled;
            PersonaTileElement.Enabled = enabled;
            TraitsTileElement.Enabled = enabled;
            CareerTileElement.Enabled = enabled;

            // Simulation buttons
            runSimButton.Enabled = enabled;
            loadSimButton.Enabled = enabled;
            verifyDbSourceButton.Enabled = enabled;
            clearDbSourceButton.Enabled = enabled;

            // Menu Items
            //PageViewPage1.Enabled = enabled;
            PageViewPage2.Enabled = enabled;
            PageViewPage3.Enabled = enabled;
            PageViewPage5.Enabled = enabled;
        }

        private void DbSourceDropDownList_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            bool hasSelection = dbSourceDropDownList.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(dbSourceDropDownList.Text);
            SetDatabaseDependentControlsEnabled(hasSelection);
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
            FormStyling.StyleFormFooterDarker(bottomPanel, e);
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
