using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    /// <summary>
    /// An object used to 3 dimensionalize <see cref="UnitBlueprint"/>s into
    /// an array of <see cref="Unit"/> and <see cref="Position"/> objects
    /// </summary>
    public static class UnitBuilder
    {
        private static Dictionary<int, UnitStatistics> UnitStats { get; set; }

        static UnitBuilder()
        {
            UnitStats = new Dictionary<int, UnitStatistics>();
        }

        public static void ClearUnitStats()
        {
            UnitStats.Clear();
        }

        public static void ClearCache()
        {
            UnitStats.Clear();
        }

        /// <summary>
        /// Converts a 2 dimensional <see cref="UnitBlueprint"/> tree into a
        /// 3 dimensional tree of <see cref="Unit"/>s
        /// </summary>
        /// <param name="db">The <see cref="SimDatabase"/> context to run the simulation on</param>
        /// <param name="template">The template to convert into a 3D unit tree</param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static UnitWrapper BuildUnit(
            SimDatabase db, 
            UnitBlueprint template, 
            IProgress<TaskProgressUpdate> progress,
            CancellationToken token)
        {
            var wrapper = UnitTemplateWrapper.FetchCache(template);
            return BuildUnit(db, wrapper, progress, token, null, 1, 1);
        }

        private static UnitWrapper BuildUnit(
            SimDatabase db, 
            UnitTemplateWrapper template, 
            IProgress<TaskProgressUpdate> progress,
            CancellationToken token,
            UnitWrapper parent, 
            int unitIndex,
            int typeIndex)
        {
            // Stop if cancel was requested
            token.ThrowIfCancellationRequested();

            // Format unit name
            var unitName = FormatUnitName(template.Blueprint.UnitNameFormat, unitIndex, typeIndex);

            // Update TaskForm progress
            progress.Report(new TaskProgressUpdate
            {
                MessageText = $"Creating unit \"{unitName}\""
            });

            // Convert this template to a Unit
            Unit unit = new Unit();
            unit.Name = unitName;
            unit.UnitCode = FormatUnitName(template.Blueprint.UnitCodeFormat, unitIndex, typeIndex);
            unit.UnitBlueprintId = template.Blueprint.Id;
            unit.ParentUnitId = parent?.Unit.Id;
            db.Units.Add(unit);

            // Convert to wrapper
            var unitWrap = new UnitWrapper(unit, template, parent);

            // Get a list of blueprints and convert those to positions
            foreach (var billet in template.PositionBlueprints)
            {
                Position pos = new Position();
                pos.BlueprintId = billet.Id;
                pos.UnitId = unit.Id;
                pos.Name = billet.Name;

                db.Positions.Add(pos);

                // Convert to wrapper
                var wrapper = new PositionWrapper(pos, billet, unitWrap, db);
                unitWrap.Positions.Add(wrapper);
            }

            // Get a list of sub templates, and convert to real Units
            int i = 0;
            var unitTypeCounter = new Dictionary<int, int>();
            foreach (var attachment in template.SubUnits)
            {
                // Add template type if not existing
                if (!unitTypeCounter.ContainsKey(attachment.Blueprint.Id))
                {
                    unitTypeCounter.Add(attachment.Blueprint.Id, 1);
                }

                // Skip parent units
                int t = unitTypeCounter[attachment.Blueprint.Id];
                UnitWrapper wrapper = BuildUnit(db, attachment, progress, token, unitWrap, ++i, t);
                unitWrap.Subunits.Add(wrapper);

                // Increment
                unitTypeCounter[attachment.Blueprint.Id] += 1;
            }
            
            // Assign supervisors
            foreach (var pos in unitWrap.Positions)
            {
                int? supId = pos.BlueprintWrapper.Blueprint.SupervisorPositionBlueprintId;
                if (!supId.HasValue) continue;

                // Search current unit first, then walk up the chain
                PositionWrapper found = null;
                UnitWrapper searchUnit = unitWrap;
    
                while (searchUnit != null && found == null)
                {
                    foreach (var candidate in searchUnit.Positions)
                    {
                        if (candidate.BlueprintWrapper.Blueprint.Id == supId.Value)
                        {
                            found = candidate;
                            break;
                        }
                    }
                    searchUnit = searchUnit.Parent;
                }

                pos.SupervisorPosition = found;
            }

            return unitWrap;
        }

        /// <summary>
        /// Fetches a <see cref="UnitStatistics"/> of the specified unit template.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static UnitStatistics GetUnitStatistics(UnitBlueprint template)
        {
            if (!UnitStats.ContainsKey(template.Id))
            {
                // Get our sub counts
                GetSoldierCounts(template, null);
            }

            return UnitStats[template.Id];
        }

        private static void GetSoldierCounts(UnitBlueprint template, UnitStatistics parent)
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
            var billets = template.PositionBlueprints.ToList();

            // Get a list of sub templates, and convert to real Units
            var subUnits = template.Attachments.ToList();

            foreach (PositionBlueprint billet in billets)
            {
                Rank rank = billet.TargetRank;
                stats.SoldierCountsByRank[rank.Type][rank.Id] += 1;
                stats.SoldierCountsByGrade[(rank.Type, rank.PayGrade)] += 1;
                stats.TotalSoldiers++;
                stats.PositionCount++;
            }

            foreach (var attachment in subUnits)
            {
                // Only process child templates
                if (attachment.ChildId == template.Id)
                    continue;

                UnitBlueprint t = attachment.Child;
                for (int i = 0; i < attachment.Count; i++)
                    GetSoldierCounts(t, stats);
            }

            // Finally, add our stats to our parent
            if (parent != null)
                stats.AddTo(parent);
        }

        /// <summary>
        /// Formats the unit name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="unitNumber"></param>
        /// <param name="typeNumber"></param>
        /// <returns></returns>
        private static string FormatUnitName(string name, int unitNumber, int typeNumber)
        {
            if (name.Contains('%'))
            {
                return Regex.Replace(name, "%(?<Code>[nic])", match =>
                {
                    switch (match.Groups["Code"].Value)
                    {
                        default:
                        case "n": return unitNumber.ToTitleCase();
                        case "i": return unitNumber.ToString();
                        case "c": return unitNumber.ToCharString(true);
                        case "N": return typeNumber.ToTitleCase();
                        case "I": return typeNumber.ToString();
                        case "C": return typeNumber.ToCharString(true);
                    }
                }, RegexOptions.IgnoreCase);
            }

            return name;
        }
    }
}
