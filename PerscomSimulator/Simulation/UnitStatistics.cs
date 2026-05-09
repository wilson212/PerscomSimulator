using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Database;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents statistical data for a unit, including the count of soldiers,
    /// their ranks, and pay grades.
    /// </summary>
    public class UnitStatistics
    {
        /// <summary>
        /// Represents the total number of positions associated with the unit, including
        /// positions defined by the unit's blueprint and its attachments.
        /// </summary>
        public int PositionCount { get; set; }

        /// <summary>
        /// Represents the total number of soldiers currently assigned to a unit,
        /// including soldiers across all ranks and pay grades.
        /// </summary>
        public int TotalSoldiers { get; set; }

        /// <summary>
        /// PoolSelection => [Rank.Id => Count]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, int>> SoldierCountsByRank { get; set; }

        /// <summary>
        /// (RankType, PayGrade) => Count
        /// </summary>
        public Dictionary<(RankType, int), int> SoldierCountsByGrade { get; set; }

        /// <summary>
        /// Represents statistical data and calculations related to unit composition,
        /// including counts of positions and soldiers, as well as detailed breakdowns
        /// based on rank types and grades.
        /// </summary>
        public UnitStatistics()
        {
            SoldierCountsByRank = new Dictionary<RankType, Dictionary<int, int>>();
            SoldierCountsByGrade = new Dictionary<(RankType, int), int>();

            using (AppDatabase db = new AppDatabase())
            {
                var ranks = db.Ranks.ToArray();

                foreach (RankType type in Enum.GetValues(typeof(RankType)))
                {
                    SoldierCountsByRank.Add(type, new Dictionary<int, int>());

                    // Add rank grades... all of them!
                    foreach (var x in ranks.Where(x => x.Type == type))
                    {
                        SoldierCountsByRank[type].Add(x.Id, 0);

                        var gradeKey = (type, x.PayGrade);
                        if (!SoldierCountsByGrade.ContainsKey(gradeKey))
                            SoldierCountsByGrade.Add(gradeKey, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the values from the current <see cref="UnitStatistics"/> instance
        /// to the specified target <see cref="UnitStatistics"/> instance,
        /// updating soldier counts, ranks, and grades in the target.
        /// </summary>
        /// <param name="other">
        /// The target <see cref="UnitStatistics"/> instance to which values
        /// from the current instance will be added.
        /// </param>
        public void AddTo(UnitStatistics other)
        {
            foreach (var pair in SoldierCountsByRank)
            {
                foreach (var stats in pair.Value)
                {
                    other.SoldierCountsByRank[pair.Key][stats.Key] += stats.Value;
                    other.TotalSoldiers += stats.Value;
                }
            }

            foreach (var kvp in SoldierCountsByGrade)
            {
                other.SoldierCountsByGrade[kvp.Key] += kvp.Value;
            }
        }
    }
}
