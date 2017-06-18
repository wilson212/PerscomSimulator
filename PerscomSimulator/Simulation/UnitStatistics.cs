using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Database;

namespace Perscom.Simulation
{
    public class UnitStatistics
    {
        public int PositionCount { get; set; }

        public int TotalSoldiers { get; set; }

        /// <summary>
        /// Type => [Rank.Id => Count]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, int>> SoldierCountsByRank { get; set; }

        /// <summary>
        /// Type => [Rank.Grade => Count]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, int>> SoldierCountsByGrade { get; set; }

        public UnitStatistics()
        {
            SoldierCountsByRank = new Dictionary<RankType, Dictionary<int, int>>();
            SoldierCountsByGrade = new Dictionary<RankType, Dictionary<int, int>>();

            using (AppDatabase db = new AppDatabase())
            {
                var ranks = db.Ranks.ToArray();

                foreach (RankType type in Enum.GetValues(typeof(RankType)))
                {
                    SoldierCountsByRank.Add(type, new Dictionary<int, int>());
                    SoldierCountsByGrade.Add(type, new Dictionary<int, int>());

                    // Add rank grades... all of them!
                    foreach (var x in ranks.Where(x => x.Type == type))
                    {
                        SoldierCountsByRank[type].Add(x.Id, 0);
                        if (!SoldierCountsByGrade[type].ContainsKey(x.Grade))
                            SoldierCountsByGrade[type].Add(x.Grade, 0);
                    }
                }
            }
        }

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

            foreach (var pair in SoldierCountsByGrade)
            {
                foreach (var stats in pair.Value)
                {
                    other.SoldierCountsByGrade[pair.Key][stats.Key] += stats.Value;
                }
            }
        }
    }
}
