using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using NodaTime;
using Perscom.Simulation;

namespace Perscom
{
    public partial class SoldierConfigForm : Form
    {
        protected Dictionary<RankType, List<SoldierSetting>> Settings { get; set; }


        public SoldierConfigForm()
        {
            // Setup form controls
            InitializeComponent();
            headerPanel.BackColor = MainForm.THEME_COLOR_DARK;
            bottomPanel.BackColor = MainForm.THEME_COLOR_GRAY;
            treeView1.Dock = DockStyle.Left;
            treeView1.Width = sidePanel.Width - (2 + sidePanel.Padding.Left);

            // Load the probabilities
            LoadSoldierSettings();

            foreach (RankType type in Enum.GetValues(typeof(RankType)))
                rankTypeBox.Items.Add(type);

            // Set default indexies
            rankTypeBox.SelectedIndex = 0;

            // Unselect node
            treeView1.SelectedNode = null;
        }

        private void ReloadTree()
        {
            // Prepare treeview
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            RankType type = (RankType)rankTypeBox.SelectedItem;
            var list = Settings[type];
            int totalProb = 0;

            // Add soldier settings
            int i = 0;
            foreach (SoldierSetting setting in list.OrderBy(x => x.TimeToLive.Maximum))
            {
                TreeNode node = new TreeNode();
                node.Tag = i++;
                node.Text = setting.ToString();
                treeView1.Nodes.Add(node);
                totalProb += setting.Probability;
            }

            // End update
            treeView1.ExpandAll();
            treeView1.EndUpdate();

            // Update label
            totalProbLabel.Text = totalProb.ToString();
        }

        private void LoadSoldierSettings()
        {
            // Ensure the unit exists
            string filePath = Path.Combine(Program.RootPath, "Config", "Soldiers.xml");
            if (!File.Exists(filePath))
                throw new Exception($"Soldiers.xml file is missing!");

            // Load the document
            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var root = document.DocumentElement;

            // Create lists
            Settings = new Dictionary<RankType, List<SoldierSetting>>();

            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                Settings.Add(type, new List<SoldierSetting>());
                string name = Enum.GetName(typeof(RankType), type).ToLower();
                XmlNodeList items = root.SelectNodes($"{name}/soldier");
                foreach (XmlElement element in items)
                {
                    int prob = Int32.Parse(element.Attributes["probability"].Value);
                    int min = Int32.Parse(element.Attributes["minTime"].Value);
                    int max = Int32.Parse(element.Attributes["maxTime"].Value);
                    Settings[type].Add(new SoldierSetting(min, max, prob));
                }
            }
        }

        private void rankTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeView1.SelectedNode = null;
            ReloadTree();
        }

        private void createRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (createRadio.Checked)
            {
                addButton.Text = "Add Soldier";
                addButton.Enabled = true;
                treeView1.SelectedNode = null;
                totalProbLabel.Focus();
                probInput.Enabled = true;
                minInput.Enabled = true;
                maxInput.Enabled = true;
            }
            else
            {
                addButton.Text = "Apply Change";
                addButton.Enabled = false;
                probInput.Enabled = false;
                minInput.Enabled = false;
                maxInput.Enabled = false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Set input values
            TreeNode node = treeView1.SelectedNode;
            if (node == null) return;

            // Grab setting
            int index = (int)node.Tag;
            RankType type = (RankType)rankTypeBox.SelectedItem;
            var list = Settings[type];
            SoldierSetting setting = list[index];

            // Set input values
            probInput.Value = setting.Probability;
            minInput.Value = setting.TimeToLive.Minimum;
            maxInput.Value = setting.TimeToLive.Maximum;

            // Enable inputs
            probInput.Enabled = true;
            minInput.Enabled = true;
            maxInput.Enabled = true;
        }

        private void numericInput_ValueChanged(object sender, EventArgs e)
        {
            if (!createRadio.Checked)
            {
                addButton.Enabled = true;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // Load list
            RankType type = (RankType)rankTypeBox.SelectedItem;
            var list = Settings[type];

            if (createRadio.Checked)
            {
                // Add the new soldiersetting
                int prob = (int)probInput.Value;
                int min = (int)minInput.Value;
                int max = (int)maxInput.Value;
                SoldierSetting setting = new SoldierSetting(min, max, prob);

                // Check for overlaps
                foreach (SoldierSetting s in list)
                {
                    if (s.TimeToLive.IsInsideRange(setting.TimeToLive))
                    {
                        MessageBox.Show(
                            "Soldier range overlaps another soldiers range. Unable to add item.", 
                            "Failed", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }

                // Add the item and reload the treeview
                list.Add(setting);
                ReloadTree();

                treeView1.SelectedNode = null;
                totalProbLabel.Focus();
            }
            else
            {
                TreeNode node = treeView1.SelectedNode;
                if (node == null) return;

                // Grab setting
                int index = (int)node.Tag;
                var newRange = new Range<int>((int)minInput.Value, (int)maxInput.Value);

                // Check for overlaps
                int i = 0;
                foreach (SoldierSetting s in list)
                {
                    if (i == index) continue;

                    if (!s.TimeToLive.Equals(newRange) && s.TimeToLive.IsInsideRange(newRange))
                    {
                        MessageBox.Show(
                            "Soldier range overlaps another soldiers range. Unable to add item.",
                            "Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }

                // Set new settings
                SoldierSetting setting = list[index];

                // Set input values
                setting.Probability = (int)probInput.Value;
                setting.TimeToLive.Minimum = newRange.Minimum;
                setting.TimeToLive.Maximum = newRange.Maximum;

                node.Text = setting.ToString();
                ReloadTree();
            }
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            LoadSoldierSettings();
            ReloadTree();
            if (createRadio.Checked)
            {
                treeView1.SelectedNode = null;
                totalProbLabel.Focus();
            }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Grab node
                TreeNode node = treeView1.SelectedNode;
                if (node == null) return;

                // Grab setting
                int index = (int)node.Tag;
                RankType type = (RankType)rankTypeBox.SelectedItem;
                var list = Settings[type];

                // Remove item from list
                list.RemoveAt(index);

                // Redraw tree
                ReloadTree();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Create XML Settings
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.Indent = true;
            Settings.IndentChars = "  ";
            Settings.NewLineChars = Environment.NewLine;
            Settings.NewLineHandling = NewLineHandling.Replace;

            string path = Path.Combine(Program.RootPath, "Config", "Soldiers.xml");

            // Write XML data
            using (XmlWriter xml = XmlWriter.Create(path, Settings))
            {
                // Player Element
                xml.WriteStartDocument();
                xml.WriteStartElement("Probability");

                foreach (var type in this.Settings)
                {
                    // Manifest
                    string name = Enum.GetName(typeof(RankType), type.Key).ToLower();
                    xml.WriteStartElement(name);
                    foreach (SoldierSetting setting in type.Value)
                    {
                        xml.WriteStartElement("soldier");
                        xml.WriteAttributeString("minTime", setting.TimeToLive.Minimum.ToString());
                        xml.WriteAttributeString("maxTime", setting.TimeToLive.Maximum.ToString());
                        xml.WriteAttributeString("probability", setting.Probability.ToString());
                        xml.WriteEndElement();
                    }
                    xml.WriteEndElement(); // Enlisted
                }

                // Close Tags and File
                xml.WriteEndElement();  // Close Probability
                xml.WriteEndDocument(); // End and Save file
            }

            MessageBox.Show("Successfully saved the settings in the Soldiers.xml", 
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region Panel Border Painting

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Gray, 1);

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(bottomPanel.Width, 0);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
            base.OnPaint(e);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Gray, 1);

            // Create points that define line.
            Point point1 = new Point(sidePanel.Width-1, 0);
            Point point2 = new Point(sidePanel.Width-1, sidePanel.Height);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
            base.OnPaint(e);
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
            Pen greyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(greyPen, point1, point2);

            base.OnPaint(e);
        }

        #endregion

        protected class SoldierSetting
        {
            public int Probability { get; set; }

            public Range<int> TimeToLive { get; set; }

            public SoldierSetting(int min, int max, int prob)
            {
                Probability = prob;
                TimeToLive = new Range<int>(min, max);
            }

            public override string ToString()
            {
                // Get years
                int period = TimeToLive.Maximum - TimeToLive.Minimum;
                LocalDate start = new LocalDate(2000, 1, 1);
                LocalDate end = start.PlusMonths(TimeToLive.Minimum);
                Period startPeriod = Period.Between(start, end, PeriodUnits.Years | PeriodUnits.Months);
                end = start.PlusMonths(TimeToLive.Maximum);
                Period endPeriod = Period.Between(start, end, PeriodUnits.Years | PeriodUnits.Months);

                StringBuilder message = new StringBuilder($"{Probability} :: ");
                if (startPeriod.Years > 0)
                    message.Append($"{startPeriod.Years} yrs ");
                
                if (startPeriod.Months > 0)
                    message.Append($"{startPeriod.Months}m ");

                message.Append(" through ");

                if (endPeriod.Years > 0)
                    message.Append($"{endPeriod.Years} yrs");

                if (endPeriod.Months > 0)
                    message.Append($" {endPeriod.Months}m");


                return message.ToString();
            }
        }
    }
}
