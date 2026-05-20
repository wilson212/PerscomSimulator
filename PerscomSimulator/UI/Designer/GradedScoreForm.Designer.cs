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
    public partial class GradedScoreForm
    {
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(GradedScoreForm));
            expLvlSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel3 = new Telerik.WinControls.UI.RadLabel();
            scoreSpinEditor = new Telerik.WinControls.UI.RadSpinEditor();
            radLabel2 = new Telerik.WinControls.UI.RadLabel();
            radLabel1 = new Telerik.WinControls.UI.RadLabel();
            methodDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            bottomPanel = new Panel();
            saveButton = new Telerik.WinControls.UI.RadButton();
            headerPanel = new Panel();
            label6 = new ShadowLabel();
            radLabel4 = new Telerik.WinControls.UI.RadLabel();
            valueDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            radLabel5 = new Telerik.WinControls.UI.RadLabel();
            operatorDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            ((ISupportInitialize)expLvlSpinEditor).BeginInit();
            ((ISupportInitialize)radLabel3).BeginInit();
            ((ISupportInitialize)scoreSpinEditor).BeginInit();
            ((ISupportInitialize)radLabel2).BeginInit();
            ((ISupportInitialize)radLabel1).BeginInit();
            ((ISupportInitialize)methodDropDownList).BeginInit();
            bottomPanel.SuspendLayout();
            ((ISupportInitialize)saveButton).BeginInit();
            headerPanel.SuspendLayout();
            ((ISupportInitialize)radLabel4).BeginInit();
            ((ISupportInitialize)valueDropDownList).BeginInit();
            ((ISupportInitialize)radLabel5).BeginInit();
            ((ISupportInitialize)operatorDropDownList).BeginInit();
            ((ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // expLvlSpinEditor
            // 
            expLvlSpinEditor.Location = new Point(157, 267);
            expLvlSpinEditor.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            expLvlSpinEditor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            expLvlSpinEditor.Name = "expLvlSpinEditor";
            expLvlSpinEditor.NullableValue = new decimal(new int[] { 10, 0, 0, 0 });
            expLvlSpinEditor.Size = new Size(98, 24);
            expLvlSpinEditor.TabIndex = 31;
            expLvlSpinEditor.ThemeName = "Fluent";
            expLvlSpinEditor.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // radLabel3
            // 
            radLabel3.Location = new Point(66, 269);
            radLabel3.Name = "radLabel3";
            radLabel3.Size = new Size(85, 18);
            radLabel3.TabIndex = 30;
            radLabel3.Text = "Required Value:";
            radLabel3.ThemeName = "Fluent";
            // 
            // scoreSpinEditor
            // 
            scoreSpinEditor.Location = new Point(157, 316);
            scoreSpinEditor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            scoreSpinEditor.Name = "scoreSpinEditor";
            scoreSpinEditor.NullableValue = new decimal(new int[] { 1, 0, 0, 0 });
            scoreSpinEditor.Size = new Size(98, 24);
            scoreSpinEditor.TabIndex = 29;
            scoreSpinEditor.ThemeName = "Fluent";
            scoreSpinEditor.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // radLabel2
            // 
            radLabel2.Location = new Point(81, 318);
            radLabel2.Name = "radLabel2";
            radLabel2.Size = new Size(70, 18);
            radLabel2.TabIndex = 28;
            radLabel2.Text = "Points Value:";
            radLabel2.ThemeName = "Fluent";
            // 
            // radLabel1
            // 
            radLabel1.Location = new Point(59, 122);
            radLabel1.Name = "radLabel1";
            radLabel1.Size = new Size(92, 18);
            radLabel1.TabIndex = 27;
            radLabel1.Text = "Method Selector:";
            radLabel1.ThemeName = "Fluent";
            // 
            // methodDropDownList
            // 
            methodDropDownList.Location = new Point(157, 120);
            methodDropDownList.Name = "methodDropDownList";
            methodDropDownList.Size = new Size(238, 24);
            methodDropDownList.TabIndex = 26;
            methodDropDownList.Text = "Select Method";
            methodDropDownList.ThemeName = "Fluent";
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = SystemColors.ControlLight;
            bottomPanel.BackgroundImage = Properties.Resources.mainPattern;
            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 389);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(454, 50);
            bottomPanel.TabIndex = 25;
            // 
            // saveButton
            // 
            saveButton.DisplayStyle = DisplayStyle.Text;
            saveButton.Location = new Point(152, 11);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(150, 28);
            saveButton.TabIndex = 91;
            saveButton.Text = "Save";
            saveButton.ThemeName = "Fluent";
            // 
            // headerPanel
            // 
            headerPanel.BackColor = SystemColors.ControlDarkDark;
            headerPanel.BackgroundImage = Properties.Resources.mainPattern;
            headerPanel.Controls.Add(label6);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(454, 75);
            headerPanel.TabIndex = 24;
            // 
            // label6
            // 
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.Control;
            label6.Location = new Point(26, 22);
            label6.Name = "label6";
            label6.ShadowDirection = 60;
            label6.ShadowOpacity = 180;
            label6.ShadowSoftness = 3F;
            label6.Size = new Size(411, 37);
            label6.TabIndex = 0;
            label6.Text = "Additional Score Selection";
            // 
            // radLabel4
            // 
            radLabel4.Location = new Point(71, 171);
            radLabel4.Name = "radLabel4";
            radLabel4.Size = new Size(80, 18);
            radLabel4.TabIndex = 33;
            radLabel4.Text = "Value Selector:";
            radLabel4.ThemeName = "Fluent";
            // 
            // valueDropDownList
            // 
            valueDropDownList.Location = new Point(157, 169);
            valueDropDownList.Name = "valueDropDownList";
            valueDropDownList.Size = new Size(238, 24);
            valueDropDownList.TabIndex = 32;
            valueDropDownList.Text = "Select Value";
            valueDropDownList.ThemeName = "Fluent";
            // 
            // radLabel5
            // 
            radLabel5.Location = new Point(97, 220);
            radLabel5.Name = "radLabel5";
            radLabel5.Size = new Size(54, 18);
            radLabel5.TabIndex = 35;
            radLabel5.Text = "Operator:";
            radLabel5.ThemeName = "Fluent";
            // 
            // operatorDropDownList
            // 
            operatorDropDownList.Location = new Point(157, 218);
            operatorDropDownList.Name = "operatorDropDownList";
            operatorDropDownList.Size = new Size(238, 24);
            operatorDropDownList.TabIndex = 34;
            operatorDropDownList.Text = "Select Operator";
            operatorDropDownList.ThemeName = "Fluent";
            // 
            // GradedScoreForm
            // 
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(454, 439);
            Controls.Add(radLabel5);
            Controls.Add(operatorDropDownList);
            Controls.Add(radLabel4);
            Controls.Add(valueDropDownList);
            Controls.Add(expLvlSpinEditor);
            Controls.Add(radLabel3);
            Controls.Add(scoreSpinEditor);
            Controls.Add(radLabel2);
            Controls.Add(radLabel1);
            Controls.Add(methodDropDownList);
            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GradedScoreForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Additional Score";
            ThemeName = "Fluent";
            ((ISupportInitialize)expLvlSpinEditor).EndInit();
            ((ISupportInitialize)radLabel3).EndInit();
            ((ISupportInitialize)scoreSpinEditor).EndInit();
            ((ISupportInitialize)radLabel2).EndInit();
            ((ISupportInitialize)radLabel1).EndInit();
            ((ISupportInitialize)methodDropDownList).EndInit();
            bottomPanel.ResumeLayout(false);
            ((ISupportInitialize)saveButton).EndInit();
            headerPanel.ResumeLayout(false);
            ((ISupportInitialize)radLabel4).EndInit();
            ((ISupportInitialize)valueDropDownList).EndInit();
            ((ISupportInitialize)radLabel5).EndInit();
            ((ISupportInitialize)operatorDropDownList).EndInit();
            ((ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        private Telerik.WinControls.UI.RadSpinEditor expLvlSpinEditor;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadSpinEditor scoreSpinEditor;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadDropDownList methodDropDownList;
        private Panel bottomPanel;
        private Telerik.WinControls.UI.RadButton saveButton;
        private Panel headerPanel;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadDropDownList valueDropDownList;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadDropDownList operatorDropDownList;
        private ShadowLabel label6;
    }
}