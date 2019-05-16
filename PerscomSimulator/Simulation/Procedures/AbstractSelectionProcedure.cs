using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a base soldier selection procedure for billets
    /// </summary>
    public abstract class AbstractSelectionProcedure
    {
        public Billet Billet { get; protected set; }

        public List<AbstractFilter> Filters { get; protected set; }

        public List<AbstractFilter> Grouping { get; protected set; }

        public List<AbstractSort> Sorting { get; protected set; }

        public AbstractSelectionProcedure(SimDatabase db, Billet billet)
        {
            this.Billet = billet ?? throw new ArgumentNullException();

            // Designate grouping, filtering and sorting
            Filters = new List<AbstractFilter>(
                db.Query<BilletSelectionFilter>(
                    "SELECT * FROM BilletSelectionFilter WHERE BilletId=@P0 ORDER BY Precedence", 
                    billet.Id
                )
            );
            Grouping = new List<AbstractFilter>(
                db.Query<BilletSelectionGroup>(
                    "SELECT * FROM BilletSelectionGroup WHERE BilletId=@P0 ORDER BY Precedence", 
                    billet.Id
                )
            );
            Sorting = new List<AbstractSort>(
                db.Query<BilletSelectionSorting>(
                    "SELECT * FROM BilletSelectionSorting WHERE BilletId=@P0 ORDER BY Precedence", 
                    billet.Id
                )
            );
        }

        /// <summary>
        /// Gets the best candidate for the provided position based off of the selected procedure option,
        /// as well as the <see cref="Database.Billet"/>'s filtering, grouping and sorting of the <see cref="Soldier"/>'s
        /// within the <see cref="Position"/>'s promotion pool.
        /// </summary>
        /// <param name="position">The position to be filled</param>
        /// <param name="currentDate">The current <see cref="IterationDate"/> of the <see cref="Simulator"/></param>
        /// <param name="type"></param>
        /// <returns>Fetches the best candidate or null if there are no condidate's</returns>
        public abstract SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate currentDate, out SpawnSoldierType type);

        /// <summary>
        /// Gets the best candidate for the provided position based off of selected procedure option,
        /// as well as ensuring candidacy to the other position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual SoldierWrapper FindLateralCandidate(PositionWrapper position, IterationDate date)
        {
            // Ensure sanity
            if (position.Billet.Id != Billet.Id)
                throw new ArgumentException("Position billet does not match this Billet");

            // Make sure position is not empty
            if (position.IsEmpty)
                throw new Exception("Position is empty");

            // Define procedures we cannot transferring INTO
            SelectionProcedure[] illegalSelections = {
                SelectionProcedure.PromotionOnly
            };

            // Define position specific vars
            UnitWrapper topUnit = position.PromotionPoolUnit;
            RankType rType = position.Billet.Rank.Type;
            int grade = position.Billet.Rank.Grade;

            // Grab soldier list
            IEnumerable<SoldierWrapper> primeSoldiers = topUnit.SoldiersByGrade[rType][grade].Values;
            IOrderedEnumerable<SoldierWrapper> soldiers;
            IEnumerable<SoldierGroupResult> groups = null;

            // We MUST force people to move from higher stature units, otherwise the position
            // could be empty forver!
            int val = (position.Billet.Selection == SelectionProcedure.LateralOnly) ? 3 : 2;

            //
            // 1. Apply initial filter, ensuring candidacy, and any kind of desire
            //
            primeSoldiers = primeSoldiers.Where(
                x => IsCanidateForPosition(x, position, date) && GetLateralPromotionGroupId(x, position, date) <= val
            );

            // Do we have any soldiers?
            if (primeSoldiers.Count() == 0)
            {
                return null;
            }

            //
            // 2. Apply Billet grouping
            //

            // Do we have selection grouping as well?
            if (Grouping.Count > 0)
            {
                // Apply lateral desire grouping (Need, Want, Dont Want), Then By billet grouping
                groups = primeSoldiers.GroupSoldiersBy(
                    x => GetLateralPromotionGroupId(x, position, date),
                    Grouping,
                    date
                );
            }
            else
            {
                groups = primeSoldiers.GroupSoldiersBy(x => GetLateralPromotionGroupId(x, position, date));
            }

            // Get topmost group with at least one soldier in it
            primeSoldiers = groups.GetPrimeSoldiers();

            // Do we have any soldiers?
            if (primeSoldiers.Count() == 0)
                throw new Exception("Group has no prime soldiers, but there was a soldier count");

            //
            // 3. Apply soldier ordering
            //
            if (Sorting.Count > 0)
            {
                // Apply sorting
                soldiers = primeSoldiers.OrderSoldiersBy(Sorting, date);
            }
            else
            {
                // Apply default sorting
                soldiers = primeSoldiers.OrderByDescending(x => x.GetTimeInGrade(date));
            }

            // Loop through each candidate to ensure candidacy
            // to the other position for our current soldier
            foreach (var soldier in soldiers)
            {
                // Get other soldier's position!
                var lateralPosition = soldier.Position;

                // Can our candidate enter this other position?
                if (illegalSelections.Contains(lateralPosition.Billet.Selection))
                    continue;

                // If randomized procedure, check if selection pools are OK
                if (lateralPosition.Billet.Selection == SelectionProcedure.RandomizedProcedure)
                {
                    var random = (RandomizedSelectionProcedure)lateralPosition.Billet.Procedure;
                    if (!random.ProcedureWrapper.ProcedurePools.Any(x => x.RankId == soldier.Soldier.RankId))
                    {
                        // None of the RandomizedPools can accept the rank
                        // of this soldier, so we must skip!
                        continue;
                    }
                }

                // He qualifies!
                return soldier;
            }

            return null;
        }

        /// <summary>
        /// Returns a group rating for a soldier that is a candidate for a lateral promotion.
        /// A higher returned number indicates a reduced need or desire for a lateral promotion.
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position">The position we are potentially moving into</param>
        /// <param name="date">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        /// <remarks>
        /// 4 = Soldier Cant really move...
        /// 3 = Downgrade position based on stature, Soldier doesn't want to
        /// 2 = Upgrade to current position, Soldier wants to
        /// 1 = Soldier should To move up very soon (Past MaxTourLength [Waiverable] or Very Near [Not Waiverable])
        /// 0 = Soldier needs to move NOW (Not Waiverable, Will be forced to retire)
        /// </remarks>
        protected virtual int GetLateralPromotionGroupId(SoldierWrapper soldier, PositionWrapper position, IterationDate date)
        {
            //
            // CAN WE EVEN?
            //
            if (soldier.IsLockedInPosition(date))
            {
                if (!soldier.Position.Billet.Billet.CanLateralEarly)
                    return 4;
            }

            //
            // DO WE NEED TO?
            //

            // If we are getting close to our maximum tour length,
            // or we have surpassed our max tour length, return true
            if (soldier.IsNearMaxTourLength(date))
            {
                if (soldier.Position.Billet.Billet.Waiverable)
                {
                    // We'll take it just for a change of scenery
                    return (soldier.IsPastMaxTourLength(date)) ? 1 : 2;
                }
                else
                {
                    // We are nearing max tour length, and the position
                    // is NOT repeatable... shit! This is high candidacy
                    return (soldier.IsPastMaxTourLength(date)) ? 0 : 1;
                }
            }

            //
            // Do We WANT to?
            //

            // If the stature is higher, OF COURSE we want it!
            return (soldier.Position.Billet.Stature < position.Billet.Stature) ? 2 : 3;
        }

        /// <summary>
        /// This method determines if the specified soldier meets the requirements
        /// to accept the specified position
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected virtual bool IsCanidateForPosition(SoldierWrapper soldier, PositionWrapper position, IterationDate date)
        {
            // A soldier can only can move once per iteration!
            // Positions are ordered at the start of the simulation by
            // Grade and Stature anyways, so it works out
            if (soldier.Assignment.EntryIterationId == date.Id)
                return false;

            // Make sure we are not retiring THIS round!
            if (soldier.IsRetiring(date))
                return false;

            // Don't move to the same billet we already sitting in
            if (position.Billet.Id == soldier.Position.Billet.Id)
                return false;

            // Quit if this is a lateral only position
            if (position.Billet.Selection == SelectionProcedure.LateralOnly && (position.Billet.Rank.Grade != soldier.Rank.Grade))
                return false;

            // Is there a MOS requirement?
            if (position.Billet.RequiredSpecialties.Length > 0)
            {
                if (position.Billet.RequiredSpecialties.Contains(soldier.Soldier.SpecialtyId))
                {
                    // If requirements are inversed, that means the soldier 
                    // MUST NOT have the required specialty to be a canidate!
                    if (position.Billet.Billet.InverseSpecialtyRequirements)
                    {
                        return false;
                    }
                }
                else if (!position.Billet.Billet.InverseSpecialtyRequirements)
                {
                    // Position required the specialty, but this soldier
                    // does not have it!
                    return false;
                }
            }

            // Apply Billet Filtering
            foreach (var filter in Filters)
            {
                if (!soldier.EvaluateFilter(filter, date))
                    return false;
            }

            // Check if we are under ranked, and if so, check for position lock
            if (soldier.IsStandIn())
            {
                // Is this position an even higher grade than what we have?
                if (position.Billet.Rank.Grade > soldier.Position.Billet.Rank.Grade)
                {
                    return true;
                }
                else
                {
                    // Never laterally move, when locked into a position, if we are a stand in
                    return (!soldier.IsLockedInPosition(date));
                }
            }

            // Are we locked into our current billet?
            if (soldier.IsLockedInPosition(date))
            {
                // is this a promotion?
                bool isPromotion = (soldier.Rank.Grade < position.Billet.Rank.Grade);
                bool isLateral = (soldier.Rank.Grade == position.Billet.Rank.Grade);
                if (isPromotion && soldier.Position.Billet.Billet.CanBePromotedEarly)
                    return true;
                else if (isLateral && soldier.Position.Billet.Billet.CanLateralEarly)
                    return true;
                else
                    return false;
            }

            // if we are here, we meet all requirements!
            return true;
        }
    }
}
