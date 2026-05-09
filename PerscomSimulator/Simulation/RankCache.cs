using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perscom.Simulation
{
    /// <summary>
    /// Provides a static cache for <see cref="Rank"/> entities, allowing efficient lookup and retrieval
    /// of ranks by type, grade, and ID. The cache is populated from the database on application startup
    /// and supports queries for rank lists, grade ranges, entry-level ranks, previous and next grades,
    /// and conversion between rank type codes and <see cref="RankType"/> values.
    /// </summary>
    public static class RankCache
    {
        public static Dictionary<RankType, Dictionary<int, List<Rank>>> RanksByGrade { get; set; } = new();

        public static Dictionary<int, Rank> RanksById { get; set; } = new();

        public static Dictionary<RankType, Range<int>> RankGradeRanges { get; set; } = new();

        public static Dictionary<int, List<int>> FeederRankMap { get; private set; } = new();

        static RankCache()
        {
            using var db = new AppDatabase();
            Load(db);
        }

        /// <summary>
        /// Loads the <see cref="Rank"/> entities from the database, and cache's them
        /// </summary>
        private static void Load(BaseDatabase db)
        {
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                RanksByGrade.Add(type, new Dictionary<int, List<Rank>>());
                RankGradeRanges.Add(type, new Range<int>(1, 1));
            }

            foreach (Rank r in db.Ranks.OrderBy(x => x.Classification.PayGrade).ThenBy(x => x.Precedence))
            {
                // Extract the Classification entity, to prevent many calls from the DB
                var rankInfo = r.Classification;
                RanksById.Add(r.Id, r);


                if (RanksByGrade[rankInfo.Type].ContainsKey(rankInfo.PayGrade))
                    RanksByGrade[rankInfo.Type][rankInfo.PayGrade].Add(r);
                else
                    RanksByGrade[rankInfo.Type].Add(rankInfo.PayGrade, new List<Rank>() { r });

                if (!RankGradeRanges[rankInfo.Type].ContainsValue(rankInfo.PayGrade))
                {
                    var range = RankGradeRanges[rankInfo.Type];
                    if (range.Maximum < rankInfo.PayGrade)
                        range.Maximum = rankInfo.PayGrade;

                    if (range.Minimum > rankInfo.PayGrade)
                        range.Minimum = rankInfo.PayGrade;
                }
            }
            
            BuildFeederRankMap(db.Ranks);
        }

        /// <summary>
        /// Builds a mapping of feeder ranks, where each rank maps to a collection of its preceding ranks.
        /// This is used to quickly identify ranks that feed into a specific target rank.
        /// </summary>
        /// <param name="allRanks">A collection of all available ranks from the database.</param>
        public static void BuildFeederRankMap(IEnumerable<Rank> allRanks)
        {
            FeederRankMap = new Dictionary<int, List<int>>();

            foreach (var rank in allRanks)
            {
                if (rank.NextRankId.HasValue)
                {
                    int targetId = rank.NextRankId.Value;
                    if (!FeederRankMap.TryGetValue(targetId, out var feeders))
                    {
                        feeders = new List<int>(4);
                        FeederRankMap[targetId] = feeders;
                    }
                    feeders.Add(rank.Id);
                }
            }
        }

        /// <summary>
        /// Gets the list of PayGrade => <see cref="Rank"/> for the
        /// specified <see cref="RanksByGrade"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<int, List<Rank>> GetRankListByType(RankType type) => RanksByGrade[type];

        /// <summary>
        /// Fetches a rank by grade and type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static List<Rank> GetRanksByGrade(RankType type, int grade) => RanksByGrade[type][grade];

        /// <summary>
        /// Fetches a rank by grade and type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static Range<int> GetRankGradesByType(RankType type) => RankGradeRanges[type];

        /// <summary>
        /// Gets the entry level rank for the specified rank type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Rank> GetEntryLevelRanks(RankType type) => RanksByGrade[type].First().Value;

        /// <summary>
        /// Returns whether the specified rank and grade exists
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static bool GradeExists(RankType type, int grade) => RanksByGrade[type].ContainsKey(grade);

        /// <summary>
        /// Gets the previous rank grade
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static Rank GetPreviousGrade(RankType type, int grade)
        {
            return GetPrevousGrades(type, grade).LastOrDefault();
        }

        /// <summary>
        /// Gets a list of all previous grades to the specified rank grade
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static Rank[] GetPrevousGrades(RankType type, int grade)
        {
            var prev = new List<Rank>();

            foreach (var rank in RanksByGrade[type].OrderBy(x => x.Key))
            {
                if (rank.Key >= grade)
                    break;

                prev.AddRange(rank.Value);
            }

            return prev.ToArray();
        }

        /// <summary>
        /// Gets the <see cref="RankType"/> based on the type <see cref="char"/> code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static RankType GetRankTypeByCode(char code)
        {
            switch (char.ToLower(code))
            {
                case 'e': return RankType.Enlisted;
                case 'o': return RankType.Officer;
                default: return RankType.Warrant;
            }
        }

        public static char GetCodeByRankType(RankType type)
        {
            switch (type)
            {
                case RankType.Enlisted: return 'e';
                case RankType.Officer: return 'o';
                default: return 'w';
            }
        }

        public static List<Rank> GetNextGradeRanks(Rank rank)
        {
            int nextGrade = rank.Classification.PayGrade + 1;

            if (RanksByGrade[rank.Classification.Type].ContainsKey(nextGrade))
            {
                return RanksByGrade[rank.Classification.Type][nextGrade];
            }
            else
            {
                return new List<Rank>();
            }
        }
    }
}
