using Perscom.Database;
using Perscom.Simulation.Procedures;
using System.Linq;

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

        public int[] RequiredSpecialties { get; set; }

        public bool CreatesNewSoldiers => Selection == SelectionProcedure.CreateNewSoldier;

        public SelectionProcedure Selection => Billet.Selection;

        public bool AutoPromoteInRankRange => Billet.AutoPromoteInRankRange;

        public bool DemoteOverRanked => Billet.DemoteOverRanked;

        /// <summary>
        /// Gets the soldier selection procedure
        /// </summary>
        public AbstractSelectionProcedure Procedure { get; protected set; }

        public BilletWrapper(Billet billet, SimDatabase db)
        {
            Billet = billet;
            Rank = billet.Rank;
            MaxRank = billet.MaxRank;
            Specialty = billet.Specialties.FirstOrDefault()?.Specialty;
            PromotionPool = billet.PromotionPool;
            RequiredSpecialties = billet.Requirements.Select(x => x.SpecialtyId).ToArray();

            switch (Selection)
            {
                case SelectionProcedure.CreateNewSoldier:
                    Procedure = new SoldierEntryProcedure(db, billet);
                    break;
                case SelectionProcedure.LateralOnly:
                    Procedure = new LateralOnlyProcedure(db, billet);
                    break;
                case SelectionProcedure.PromotionOnly:
                    Procedure = new PromotionOnlyProcedure(db, billet);
                    break;
                case SelectionProcedure.PromotionOrLateral:
                    Procedure = new PromotionOrLateralProcedure(db, billet);
                    break;
                case SelectionProcedure.OrderedProcedure:
                    Procedure = new OrderedSelectionProcedure(db, billet);
                    break;
                case SelectionProcedure.RandomizedProcedure:
                    Procedure = new RandomizedSelectionProcedure(db, billet);
                    break;
            }
        }
    }
}
