using CrossLite.QueryBuilder;
using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections;
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
    public partial class PositionHistoryForm : Form
    {
        /// <summary>
        /// Sets the number of records to display on the Soldiers Data Grid View
        /// </summary>
        const int DATA_GRID_PAGE_SIZE = 50;

        /// <summary>
        /// Contains the active <see cref="SimDatabase"/> instance
        /// </summary>
        protected SimDatabase Database { get; set; }

        /// <summary>
        /// Gets the current <see cref="Position.Id"/>
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// The current simulated <see cref="IterationDate"/>
        /// </summary>
        public IterationDate CurrentDate { get; set; }

        /// <summary>
        /// Cached query builder for populating the list of past soldiers
        /// </summary>
        protected SelectQueryBuilder Query { get; set; }

        public PositionHistoryForm(SimDatabase db, int positionId, IterationDate date)
        {
            // Set internals
            Database = db;
            PositionId = positionId;
            CurrentDate = date;

            // Create form controls
            InitializeComponent();

            // Set header text
            var position = db.Query<Position>("SELECT * FROM Position WHERE Id=@P0", positionId).First();
            var unit = position.Unit;
            var unitNameBuilder = new StringBuilder();
            while (unit != null)
            {
                unitNameBuilder.Append(unit.Name);
                unit = unit.Attachments.Where(x => x.ChildId == unit.Id).Select(x => x.ParentUnit).FirstOrDefault();
                if (unit != null)
                    unitNameBuilder.Append(", ");
            }
            unitLabel.Text = unitNameBuilder.ToString();
            labelHeader.Text = position.Name;

            // Cache query
            Query = new SelectQueryBuilder(Database);
            Query.From(nameof(PastAssignment))
                .Where("PositionId").Equals(PositionId)
                .SelectAll()
                .OrderBy("EntryIterationId", Sorting.Descending)
                .Limit = DATA_GRID_PAGE_SIZE;

            // Assign DataSource
            bindingSource1.DataSource = new PageOffsetList(db, positionId);
            bindingSource1.MoveFirst();
        }

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

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            // Always clear the current rows!
            dataGridView1.Rows.Clear();

            // Grab soldiers by setting our offset and executing the query
            Query.Offset = (int)bindingSource1.Current;
            var assignments = Query.ExecuteQuery<PastAssignment>();

            // Loop through each soldier who held this position
            foreach (PastAssignment assignment in assignments)
            {
                Soldier s = assignment.Soldier;
                IterationDate exitDate = assignment.ExitDate;

                // Create soldier wrapper (additional methods)
                SoldierFormWrapper soldier = new SoldierFormWrapper(s, exitDate, assignment);

                // Add soldier row
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Tag = new PositionResult { Soldier = soldier, PastAssignment = assignment };
                row.SetValues(new object[]
                {
                    soldier.CurrentRankIcon ?? new Bitmap(1, 1),
                    soldier.Name,
                    assignment.EntryDate.Date.ToShortDateString(),
                    soldier.EntryRankIcon ?? new Bitmap(1, 1),
                    assignment.ExitDate.Date.ToShortDateString(),
                    soldier.ExitRankIcon ?? new Bitmap(1, 1),
                    assignment.ExitDate.MonthsDifference(assignment.EntryDate),
                    Math.Round((double)soldier.TimeInService / 12, 2).ToString(),
                    soldier.TimeInGrade,
                });
                dataGridView1.Rows.Add(row);
            }
        }

        private async void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var row = dataGridView1.SelectedRows[0];
            var result = (PositionResult)row.Tag;
            var soldier = result.Soldier;

            // Show Task Form!
            TaskForm.Show(this, "Loading", "Loading soldier statistics... Please Wait", false);

            // New thread
            var form = await Task.Run(() => new SoldierViewForm(soldier, CurrentDate.Date));
            using (form)
            {
                TaskForm.CloseForm();

                // Show soldier dialog
                form.ShowDialog(this);
            }
        }

        /// <summary>
        /// An internal class used to specify the total records in a <see cref="BindingNavigator"/>
        /// </summary>
        protected class PageOffsetList : IListSource
        {
            public bool ContainsListCollection { get; protected set; }

            /// <summary>
            /// Gets the total number of records
            /// </summary>
            public int TotalRecords { get; set; }

            public int PositionId { get; protected set; }

            /// <summary>
            /// Creates a new instance of PageOffsetList
            /// </summary>
            /// <param name="soldiers"></param>
            public PageOffsetList(SimDatabase db, int positionId)
            {
                SelectQueryBuilder builder = new SelectQueryBuilder(db);
                TotalRecords = builder.From(nameof(PastAssignment))
                    .Where("PositionId").Equals(positionId)
                    .SelectCount()
                    .ExecuteScalar<int>();
            }

            public IList GetList()
            {
                // Return a list of page offsets based on "totalRecords" and "pageSize"
                var pageOffsets = new List<int>();
                for (int offset = 0; offset < TotalRecords; offset += DATA_GRID_PAGE_SIZE)
                    pageOffsets.Add(offset);

                return pageOffsets;
            }
        }

        protected class PositionResult
        {
            public SoldierFormWrapper Soldier { get; set; }

            public PastAssignment PastAssignment { get; set; }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
