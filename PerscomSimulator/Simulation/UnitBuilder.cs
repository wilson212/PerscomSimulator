using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    /// <summary>
    /// An object used to 3 dimensionalize <see cref="UnitTemplate"/>s into
    /// an array of <see cref="Unit"/> and <see cref="Position"/> objects
    /// </summary>
    public static class UnitBuilder
    {
        private static Dictionary<int, UnitStatistics> UnitStats { get; set; }

        private static Dictionary<int, UnitTemplateWrapper> UnitTemplates { get; set; }

        static UnitBuilder()
        {
            UnitStats = new Dictionary<int, UnitStatistics>();
            UnitTemplates = new Dictionary<int, UnitTemplateWrapper>();
        }

        /// <summary>
        /// Converts a 2 dimensional <see cref="UnitTemplate"/> tree into a
        /// 3 dimensional tree of <see cref="Unit"/>s
        /// </summary>
        /// <param name="db">The <see cref="SimDatabase"/> context to run the simulation on</param>
        /// <param name="template">The template to convert into a 3D unit tree</param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static UnitWrapper BuildUnit(
            SimDatabase db, 
            UnitTemplate template, 
            IProgress<TaskProgressUpdate> progress,
            CancellationToken token)
        {
            var wrapper = UnitTemplateWrapper.FetchCache(template);
            return BuildUnit(db, wrapper, progress, token, null, 1);
        }

        private static UnitWrapper BuildUnit(
            SimDatabase db, 
            UnitTemplateWrapper template, 
            IProgress<TaskProgressUpdate> progress,
            CancellationToken token,
            UnitWrapper parent, 
            int unitIndex)
        {
            // Stop if cancel was requested
            token.ThrowIfCancellationRequested();

            // Format unit name
            string unitName = FormatUnitName(template.Template.UnitNameFormat, unitIndex);

            // Update TaskForm progress
            TaskProgressUpdate update = new TaskProgressUpdate();
            update.MessageText = $"Creating unit \"{unitName}\"";
            progress.Report(update);

            // Convert this template to a Unit
            Unit unit = new Unit();
            unit.Name = unitName;
            unit.UnitCode = FormatUnitName(template.Template.UnitCodeFormat, unitIndex);
            unit.UnitTemplateId = template.Template.Id;
            db.Units.Add(unit);

            // Convert to wrapper
            var unitWrap = new UnitWrapper(unit, template, parent);

            // Get a list of billets, and convert those to positions
            foreach (Billet billet in template.Billets)
            {
                Position pos = new Position();
                pos.BilletId = billet.Id;
                pos.UnitId = unit.Id;
                pos.Name = billet.Name;

                db.Positions.Add(pos);

                // Convert to wrapper
                var wrapper = new PositionWrapper(pos, billet, unitWrap);
                unitWrap.Positions.Add(wrapper);
            }

            // Get a list of sub templates, and convert to real Units
            int i = 0;
            foreach (UnitTemplateWrapper attachment in template.SubUnits)
            {
                // Skip parent units
                UnitWrapper wrapper = BuildUnit(db, attachment, progress, token, unitWrap, ++i);
                unitWrap.Subunits.Add(wrapper);

                // Create attachment
                var entry = new UnitAttachment()
                {
                    ParentId = unitWrap.Unit.Id,
                    ChildId = wrapper.Unit.Id
                };
                db.UnitAttachments.Add(entry);
            }

            return unitWrap;
        }

        /// <summary>
        /// Fetches a <see cref="UnitStatistics"/> of the specified unit template.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static UnitStatistics GetUnitStatistics(UnitTemplate template)
        {
            if (!UnitStats.ContainsKey(template.Id))
            {
                // Get our sub counts
                GetSoldierCounts(template, null);
            }

            return UnitStats[template.Id];
        }

        private static void GetSoldierCounts(UnitTemplate template, UnitStatistics parent)
        {
            if (UnitStats.ContainsKey(template.Id))
            {
                UnitStats[template.Id].AddTo(parent);
                return;
            }

            // Add it
            var stats = new UnitStatistics();
            UnitStats.Add(template.Id, stats);

            // Get a list of billets, and convert those to positions
            var billets = template.Billets.ToList();

            // Get a list of sub templates, and convert to real Units
            var subUnits = template.UnitTemplateAttachments.ToList();

            foreach (Billet billet in billets)
            {
                Rank rank = billet.Rank;
                stats.SoldierCountsByRank[rank.Type][rank.Id] += 1;
                stats.SoldierCountsByGrade[rank.Type][rank.Grade] += 1;
                stats.TotalSoldiers++;
                stats.PositionCount++;
            }

            foreach (var attachment in subUnits)
            {
                // Only process child templates
                if (attachment.ChildId == template.Id)
                    continue;

                UnitTemplate t = attachment.Child;
                for (int i = 0; i < attachment.Count; i++)
                    GetSoldierCounts(t, stats);
            }

            // Finally, add our stats to our parent
            if (parent != null)
                stats.AddTo(parent);
        }

        private static string FormatUnitName(string name, int unitNumber)
        {
            if (name.Contains('%'))
            {
                return name
                    .Replace("%n", unitNumber.ToTitleCase())
                    .Replace("%i", unitNumber.ToString())
                    .Replace("%c", unitNumber.ToCharString(true));
            }

            return name;
        }
    }
}
