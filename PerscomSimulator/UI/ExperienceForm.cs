using CrossLite.QueryBuilder;
using Perscom.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perscom
{
    public partial class ExperienceForm : Form
    {
        private List<Experience> Items { get; set; }

        public ExperienceForm()
        {
            InitializeComponent();

            // Load items
            using (AppDatabase db = new AppDatabase())
            {
                Items = db.Experience.ToList();
            }

            FillListView();
        }

        private void FillListView()
        {
            // Prepare update
            listView1.BeginUpdate();

            // Reset listview
            listView1.Items.Clear();

            // Create ListView items
            foreach (Experience spec in Items.OrderBy(x => x.Name))
            {
                ListViewItem item = new ListViewItem(spec.Name);
                item.Tag = spec;
                listView1.Items.Add(item);
            }

            // End update
            listView1.EndUpdate();
        }

        #region Panel Border Painting

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooter(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion

        private void addButton_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;

            // Create the regular expression to filter name
            string pattern = "s/[^A-Za-z0-9 ]+//g";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            name = regex.Replace(name, "");

            // Ensure name length
            if (name.Length < 3)
            {
                ShowErrorMessage("Experience item name must be at least 3 characters in length!");
                return;
            }

            // Ensure this trait name does not already exist
            if (Items.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                ShowErrorMessage("Experience item with the same name already exists!");
                return;
            }

            // Create new item!
            var item = new Experience() { Name = name };

            // Add item
            Items.Add(item);

            // Fill list view!
            FillListView();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    var newItems = listView1.Items.Cast<ListViewItem>().Select(x => x.Tag as Experience).ToList();
                    var oldItems = db.Experience.ToList();

                    // Remove
                    foreach (var item in oldItems.Except(newItems))
                    {
                        DeleteQueryBuilder query = new DeleteQueryBuilder(db);
                        query.From(nameof(Experience))
                            .Where("Id", Comparison.Equals, item.Id);
                        query.Execute();
                    }

                    // Add
                    foreach (var item in newItems.Except(oldItems))
                    {
                        db.Experience.Add(item);
                    }

                    // Update
                    foreach (var item in newItems.Intersect(oldItems))
                    {
                        db.Experience.Update(item);
                    }

                    // Save changes
                    trans.Commit();

                    // Close form
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ExceptionHandler.ShowException(ex);
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editItemNameToolStripMenuItem_Click(sender, e);
        }

        private void editItemNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;

            // Grab UnitTemplate from selected item tag
            var exp = (Experience)listView1.SelectedItems[0].Tag;

            // Open editor
            using (var form = new ExperienceNameChangeForm(exp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Redraw the sub units listView
                    FillListView();
                }
            }
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure we have a selected item
            if (listView1.SelectedItems.Count == 0)
                return;

            // Grab UnitTemplate from selected item tag
            var exp = (Experience)listView1.SelectedItems[0].Tag;

            // Remove the unit tempalate from the list
            Items.Remove(exp);

            // Now redraw the sub units listView
            FillListView();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // Ensure we have an item selected!
            var items = listView1.SelectedItems;
            removeItemToolStripMenuItem.Enabled = (items.Count != 0);
            editItemNameToolStripMenuItem.Enabled = (items.Count != 0);
        }
    }
}
