using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perscom.Database;

namespace Perscom.Simulation
{
    public class BilletWrapper
    {
        public int Id => Billet.Id;

        /// <summary>
        /// Gets or sets the prominence level of the position. A higher value will cause
        /// this <see cref="Billet"/> to be filled with more experienced (time in grade) 
        /// soldiers, while lower values will be filled with more inexperienced soldiers.
        /// </summary>
        public int Stature => Billet.Stature;

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this <see cref="Billet"/> 
        /// before being allowed to leave. If the minimum amount is less than the remaining time 
        /// to live for the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        public int MinTourLength => Billet.MinTourLength;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this <see cref="Billet"/>
        /// before being forcefully pushed out.
        /// </summary>
        public int MaxTourLength => Billet.MaxTourLength;

        public Billet Billet { get; set; }

        public Rank Rank { get; set; }

        public Rank MaxRank { get; set; }

        public Specialty Specialty { get; set; }

        public Echelon PromotionPool { get; set; }

        public BilletSpawnSetting SpawnSetting { get; set; }

        public int[] RequiredSpecialties { get; set; }

        public bool IsEntryLevel => SpawnSetting != null;

        protected static Dictionary<int, BilletWrapper> Cache { get; set; }

        static BilletWrapper()
        {
            Cache = new Dictionary<int, BilletWrapper>();
        }

        public BilletWrapper(Billet billet)
        {
            Billet = billet;
            Rank = billet.Rank;
            MaxRank = billet.MaxRank;
            Specialty = billet.Specialties.FirstOrDefault()?.Specialty;
            SpawnSetting = billet.SpawnSettings.FirstOrDefault();
            PromotionPool = billet.PromotionPool;
            RequiredSpecialties = billet.Requirements.Select(x => x.SpecialtyId).ToArray();
        }

        public static BilletWrapper FetchCache(Billet billet)
        {
            if (!Cache.ContainsKey(billet.Id))
                Cache.Add(billet.Id, new BilletWrapper(billet));

            return Cache[billet.Id];
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
