namespace Perscom
{
    partial class OccupationBoardListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OccupationBoardListForm));
            bottomPanel = new System.Windows.Forms.Panel();
            closeButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new System.Windows.Forms.Panel();
            headerLabel = new System.Windows.Forms.ShadowLabel();
            radGroupBox2 = new Telerik.WinControls.UI.RadGroupBox();
            promoBoardsGridView = new Telerik.WinControls.UI.RadGridView();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            radLabel3 = new Telerik.WinControls.UI.RadLabel();
            boardContextMenu = new Telerik.WinControls.UI.RadContextMenu(components);
            addMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            classificationMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            rankMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radMenuSeparatorItem1 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            deleteMenuItem = new Telerik.WinControls.UI.RadMenuItem();
            radContextMenuManager1 = new Telerik.WinControls.UI.RadContextMenuManager();
            bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)closeButton).BeginInit();
            headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radGroupBox2).BeginInit();
            radGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)promoBoardsGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)promoBoardsGridView.MasterTemplate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(closeButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 627);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(725, 50);
            bottomPanel.TabIndex = 25;
            bottomPanel.Paint += bottomPanel_Paint;
            // 
            // closeButton
            // 
            closeButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            closeButton.Location = new System.Drawing.Point(287, 11);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(150, 28);
            closeButton.TabIndex = 91;
            closeButton.Text = "Close";
            closeButton.ThemeName = "Fluent";
            closeButton.Click += CloseButton_Click;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(headerLabel);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            headerPanel.Location = new System.Drawing.Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new System.Drawing.Size(725, 75);
            headerPanel.TabIndex = 24;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // headerLabel
            // 
            headerLabel.BackColor = System.Drawing.Color.Transparent;
            headerLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            headerLabel.ForeColor = System.Drawing.SystemColors.Control;
            headerLabel.Location = new System.Drawing.Point(26, 22);
            headerLabel.Name = "headerLabel";
            headerLabel.ShadowDirection = 60;
            headerLabel.ShadowOpacity = 180;
            headerLabel.ShadowSoftness = 3F;
            headerLabel.Size = new System.Drawing.Size(717, 37);
            headerLabel.TabIndex = 0;
            headerLabel.Text = "Occupation Promotion Board Override List";
            // 
            // radGroupBox2
            // 
            radGroupBox2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            radGroupBox2.Controls.Add(promoBoardsGridView);
            radGroupBox2.HeaderAlignment = Telerik.WinControls.UI.HeaderAlignment.Center;
            radGroupBox2.HeaderMargin = new System.Windows.Forms.Padding(3);
            radGroupBox2.HeaderText = "Occupation Promotion Boards";
            radGroupBox2.Location = new System.Drawing.Point(12, 142);
            radGroupBox2.Name = "radGroupBox2";
            radGroupBox2.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            radGroupBox2.Size = new System.Drawing.Size(700, 436);
            radGroupBox2.TabIndex = 30;
            radGroupBox2.Text = "Occupation Promotion Boards";
            radGroupBox2.ThemeName = "Fluent";
            // 
            // promoBoardsGridView
            // 
            promoBoardsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            promoBoardsGridView.Location = new System.Drawing.Point(0, 22);
            // 
            // 
            // 
            promoBoardsGridView.MasterTemplate.AllowAddNewRow = false;
            promoBoardsGridView.MasterTemplate.AllowCellContextMenu = false;
            promoBoardsGridView.MasterTemplate.AllowColumnChooser = false;
            promoBoardsGridView.MasterTemplate.AllowColumnHeaderContextMenu = false;
            promoBoardsGridView.MasterTemplate.AllowColumnReorder = false;
            promoBoardsGridView.MasterTemplate.AllowColumnResize = false;
            promoBoardsGridView.MasterTemplate.AllowDragToGroup = false;
            promoBoardsGridView.MasterTemplate.AllowEditRow = false;
            promoBoardsGridView.MasterTemplate.AllowRowHeaderContextMenu = false;
            promoBoardsGridView.MasterTemplate.AllowRowResize = false;
            gridViewTextBoxColumn1.HeaderText = "Board Name";
            gridViewTextBoxColumn1.Name = "column1";
            gridViewTextBoxColumn1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn1.Width = 230;
            gridViewTextBoxColumn2.HeaderText = "Scope";
            gridViewTextBoxColumn2.Name = "column2";
            gridViewTextBoxColumn2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn2.Width = 180;
            gridViewTextBoxColumn3.HeaderText = "Type";
            gridViewTextBoxColumn3.Name = "column3";
            gridViewTextBoxColumn3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn3.Width = 120;
            gridViewTextBoxColumn4.HeaderText = "Pass Threshold";
            gridViewTextBoxColumn4.Name = "column4";
            gridViewTextBoxColumn4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn4.Width = 140;
            promoBoardsGridView.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] { gridViewTextBoxColumn1, gridViewTextBoxColumn2, gridViewTextBoxColumn3, gridViewTextBoxColumn4 });
            promoBoardsGridView.MasterTemplate.EnableAlternatingRowColor = true;
            promoBoardsGridView.MasterTemplate.HorizontalScrollState = Telerik.WinControls.UI.ScrollState.AlwaysHide;
            promoBoardsGridView.MasterTemplate.ViewDefinition = tableViewDefinition1;
            promoBoardsGridView.Name = "promoBoardsGridView";
            radContextMenuManager1.SetRadContextMenu(promoBoardsGridView, boardContextMenu);
            promoBoardsGridView.ReadOnly = true;
            promoBoardsGridView.Size = new System.Drawing.Size(700, 414);
            promoBoardsGridView.TabIndex = 10;
            promoBoardsGridView.ThemeName = "Fluent";
            promoBoardsGridView.CellFormatting += GridView_CellFormatting;
            promoBoardsGridView.DoubleClick += PromoBoardsGridView_DoubleClick;
            // 
            // positionNameLabel
            // 
            radLabel1.Location = new System.Drawing.Point(12, 584);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new System.Drawing.Size(547, 18);
            radLabel1.TabIndex = 31;
            radLabel1.Text = "* Right Click to Add or Delete a Promotion Board. Double click on a row to open the Promotion Board Editor";
            // 
            // radLabel2
            // 
            radLabel2.Location = new System.Drawing.Point(88, 85);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new System.Drawing.Size(549, 18);
            radLabel2.TabIndex = 32;
            radLabel2.Text = "This form contains a list of Promtion Board's that are specific to this Occupational Specialty, that override the";
            // 
            // radLabel3
            // 
            radLabel3.Location = new System.Drawing.Point(164, 109);
            radLabel3.Name = "radLabel3";
            radLabel3.Size = new System.Drawing.Size(396, 18);
            radLabel3.TabIndex = 33;
            radLabel3.Text = "Promotion Board's defined for each Rank and Classification in the Rank Editor.";
            // 
            // boardContextMenu
            // 
            boardContextMenu.Items.AddRange(new Telerik.WinControls.RadItem[] { addMenuItem, radMenuSeparatorItem1, deleteMenuItem });
            boardContextMenu.ThemeName = "Fluent";
            // 
            // addMenuItem
            // 
            addMenuItem.Items.AddRange(new Telerik.WinControls.RadItem[] { classificationMenuItem, rankMenuItem });
            addMenuItem.Name = "addMenuItem";
            addMenuItem.Text = "Add New Board";
            // 
            // classificationMenuItem
            // 
            classificationMenuItem.Name = "classificationMenuItem";
            classificationMenuItem.Text = "Classification Override";
            // 
            // rankMenuItem
            // 
            rankMenuItem.Name = "rankMenuItem";
            rankMenuItem.Text = "Rank Override";
            // 
            // radMenuSeparatorItem1
            // 
            radMenuSeparatorItem1.Name = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.Text = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deleteMenuItem
            // 
            deleteMenuItem.Name = "deleteMenuItem";
            deleteMenuItem.Text = "Delete Board";
            // 
            // OccupationBoardListForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            ClientSize = new System.Drawing.Size(725, 677);
            Controls.Add(radLabel3);
            Controls.Add(radLabel2);
            Controls.Add(radLabel1);
            Controls.Add(radGroupBox2);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OccupationBoardListForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Occupation Board List";
            ThemeName = "Fluent";
            bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)closeButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radGroupBox2).EndInit();
            radGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)promoBoardsGridView.MasterTemplate).EndInit();
            ((System.ComponentModel.ISupportInitialize)promoBoardsGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)radLabel3).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton closeButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel headerLabel;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox2;
        private Telerik.WinControls.UI.RadGridView promoBoardsGridView;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadContextMenu boardContextMenu;
        private Telerik.WinControls.UI.RadMenuItem addMenuItem;
        private Telerik.WinControls.UI.RadMenuItem classificationMenuItem;
        private Telerik.WinControls.UI.RadMenuItem rankMenuItem;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem1;
        private Telerik.WinControls.UI.RadMenuItem deleteMenuItem;
        private Telerik.WinControls.UI.RadContextMenuManager radContextMenuManager1;
    }
}
