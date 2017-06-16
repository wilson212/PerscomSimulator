using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom.Database
{
    public static class RankCache
    {
        public static Dictionary<RankType, Dictionary<int, List<Rank>>> RanksByGrade { get; set; }

        public static Dictionary<int, Rank> RanksById { get; set; }

        public static Dictionary<RankType, Range<int>> RankGradeRanges { get; set; }

        static RankCache()
        {
            RankGradeRanges = new Dictionary<RankType, Range<int>>();
            RanksByGrade = new Dictionary<RankType, Dictionary<int, List<Rank>>>();
            RanksById = new Dictionary<int, Rank>();

            using (AppDatabase db = new AppDatabase())
                Load(db);
        }

        /// <summary>
        /// Loads the <see cref="Rank"/> entities from the database, and cache's them
        /// </summary>
        public static void Load(BaseDatabase db)
        {
            RankGradeRanges.Clear();
            RanksByGrade.Clear();
            RanksById.Clear();

            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                RanksByGrade.Add(type, new Dictionary<int, List<Rank>>());
                RankGradeRanges.Add(type, new Range<int>(1, 1));
            }

            foreach (Rank r in db.Ranks.OrderBy(x => x.Grade))
            {
                RanksById.Add(r.Id, r);
                if (RanksByGrade[r.Type].ContainsKey(r.Grade))
                    RanksByGrade[r.Type][r.Grade].Add(r);
                else
                    RanksByGrade[r.Type].Add(r.Grade, new List<Rank>() { r });

                if (!RankGradeRanges[r.Type].ContainsValue(r.Grade))
                {
                    var range = RankGradeRanges[r.Type];
                    if (range.Maximum < r.Grade)
                        range.Maximum = r.Grade;

                    if (range.Minimum > r.Grade)
                        range.Minimum = r.Grade;
                }
            }
        }

        /// <summary>
        /// Gets the list of Grade => <see cref="Rank"/> for the
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
            switch (Char.ToLower(code))
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
            int nextGrade = rank.Grade + 1;

            if (RanksByGrade[rank.Type].ContainsKey(nextGrade))
            {
                return RanksByGrade[rank.Type][nextGrade];
            }
            else
            {
                return new List<Rank>();
            }
        }
    }
}
