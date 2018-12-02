using System;
using System.Collections.Generic;
using Perscom.Database;

namespace Perscom.Simulation
{
    public class SoldierWrapper : IDisposable, IEquatable<SoldierWrapper>
    {
        /// <summary>
        /// Gets the soldiers first and last name
        /// </summary>
        public string Name => $"{Soldier.FirstName} {Soldier.LastName}";

        /// <summary>
        /// Gets the <see cref="Database.Soldier"/> object
        /// </summary>
        public Soldier Soldier { get; protected set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Rank"/>
        /// </summary>
        public Rank Rank { get; set; }

        /// <summary>
        /// Gets the current soldier Assignment, if any
        /// </summary>
        public Assignment Assignment { get; protected set; } = new Assignment();

        /// <summary>
        /// Gets the entry Iteration Date this soldier was created in
        /// </summary>
        public IterationDate EntryServiceDate { get; set; }

        /// <summary>
        /// Gets the <see cref="IterationDate"/> from when this soldier's last 
        /// <see cref="Database.Rank"/> change occured
        /// </summary>
        public IterationDate LastPromotionDate { get; protected set; }

        /// <summary>
        /// Gets the <see cref="IterationDate"/> of when this soldier had their 
        /// <see cref="Rank.Grade"/> changed.
        /// </summary>
        public IterationDate LastGradeChangeDate { get; protected set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Position"/>, or null
        /// </summary>
        public PositionWrapper Position { get; protected set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Specialty"/>
        /// </summary>
        public Specialty Specialty => Soldier.Specialty;

        /// <summary>
        /// Gets a list of billets this soldier has held
        /// </summary>
        /// <remarks>
        /// [Billet.Id => BilletWrapper]
        /// </remarks>
        public Dictionary<int, BilletWrapper> BilletsHeld { get; set; } = new Dictionary<int, BilletWrapper>();

        /// <summary>
        /// Contains a list of soldier promotions
        /// </summary>
        public List<Promotion> Promotions { get; set; } = new List<Promotion>();

        /// <summary>
        /// Contains a list of soldier promotions
        /// </summary>
        public List<PastAssignment> PastAssignments { get; set; } = new List<PastAssignment>();

        /// <summary>
        /// Gets a list of ExperienceId's and the value this soldier has accumulated
        /// </summary>
        public Dictionary<int, int> Experience { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Used for the Garbage Collector
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SoldierWrapper"/>
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="rank"></param>
        public SoldierWrapper(Soldier soldier, Rank rank, IterationDate date, SimDatabase db)
        {
            Soldier = soldier;
            soldier.RankId = rank.Id;
            Rank = rank;
            EntryServiceDate = date;
            LastPromotionDate = date;
            LastGradeChangeDate = date;

            AssignSpecialty(soldier.SpecialtyId, date, db);
        }

        /// <summary>
        /// Assigns the specified <see cref="Database.Specialty"/> to this soldier
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="currentDate"></param>
        /// <param name="db"></param>
        public void AssignSpecialty(int specId, IterationDate currentDate, SimDatabase db)
        {
            // Create promotion record
            db.SpecialtyAssignments.Add(new SpecialtyAssignment()
            {
                AssignedIteration = currentDate.Id,
                SoldierId = Soldier.Id,
                SpecialtyId = specId
            });

            // Set spec
            Soldier.SpecialtyId = specId;
        }

        /// <summary>
        /// Retires the soldier by releaseing this instance from their <see cref="Position"/>
        /// </summary>
        public void Retire(IterationDate currentDate, SimDatabase db)
        {
            if (Position != null)
            {
                // Remove ourselves from the position
                RemoveFromPosition(currentDate, db);
            }

            // Set database flags
            Soldier.Retired = true;
            Soldier.ExitIterationId = currentDate.Id;

            // Save soldier properties
            Soldier.RankId = Rank.Id;
            Soldier.LastPromotionIterationId = LastPromotionDate.Id;

            // Clear assignment
            Assignment = null;
        }

        // <summary>
        /// Returns whether this soldier's has lifespan has expired
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <returns></returns>
        public bool IsRetiring(IterationDate currentDate)
        {
            /* Forced out by billet? 
             * 
             * The simlator will always try to perform Lateral promotions for a position
             * before checking this method. If we are at or past our max tour length,
             * than there really is NO options for this soldier, so we must retire!
             * 
             * The lateral priority of this soldier has been rising over time in the
             * "TryPerformLateralMovement" method, and either positions just have
             * not been opening up, or other soldiers have had higher priority
             */
            if (Position.Billet.MaxTourLength > 0)
            {
                int timeLeft = Position.Billet.MaxTourLength - GetTimeInBillet(currentDate);
                if (timeLeft <= 0 && !Position.Billet.Billet.Waiverable)
                    return true;
            }

            // Check for max time in grade
            if (Rank.MaxTimeInGrade > 0)
            {
                int months = currentDate.Id - LastGradeChangeDate.Id;
                if (months >= Rank.MaxTimeInGrade)
                    return true;
            }

            // If we hit the end of our career...
            if (currentDate.Id >= Soldier.ExitIterationId)
            {
                // Check for locked in by billet (Time In Grade
                if (!Position.Billet.Billet.CanRetireEarly)
                {
                    int difference = currentDate.Id - Assignment.AssignedIteration;
                    if (Position.Billet.MinTourLength > difference)
                        return false;
                }

                return true;
            }

            // Nope, we good
            return false;
        }

        /// <summary>
        /// Promotes the soldier to the specified <see cref="Database.Rank"/>
        /// </summary>
        /// <param name="date"></param>
        /// <param name="newRank"></param>
        public void PromoteTo(IterationDate date, Rank newRank, SimDatabase db)
        {
            // Ensure we arent being promoted to the same rank we are already
            if (newRank.Id == Soldier.RankId) return;

            // Create promotion record
            Promotions.Add(new Promotion()
            {
                IterationId = date.Id,
                SoldierId = Soldier.Id,
                FromRankId = Soldier.RankId,
                ToRankId = newRank.Id,
                TimeInService = date.Id - Soldier.EntryIterationId,
                PreviousTimeInRank = date.Id - LastPromotionDate.Id,
                TimeSinceLastGradeChange = date.Id - LastGradeChangeDate.Id
            });

            // Remove soldier before promoting!
            RemoveSoldierRecursively(Position.ParentUnit);

            // Log grade increase
            if (newRank.Grade > Rank.Grade || newRank.Type != Rank.Type)
            {
                LastGradeChangeDate = date;
                Soldier.LastGradeChangeDate = date;
            }

            // Set new rank
            Rank = newRank;
            LastPromotionDate = date;
            Soldier.RankId = newRank.Id;
            Soldier.LastPromotionDate = date;

            // Move soldier in unit roster
            AddSoldierRecursively(Position.ParentUnit);
        }

        /// <summary>
        /// Returns whether the soldier is currently promotable
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <returns></returns>
        public bool IsPromotable(IterationDate currentDate)
        {
            PromotableStatus type;
            return IsPromotable(currentDate, out type);
        }

        /// <summary>
        /// Returns whether the soldier is currently promotable
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <returns></returns>
        public bool IsPromotable(IterationDate currentDate, out PromotableStatus status)
        {
            // Always approve a promotion to a different rank type,
            // no matter if they are promotable or not!
            if (Rank.Type != Position.Billet.Rank.Type)
            {
                status = PromotableStatus.Position;
                return true;
            }

            // Always approve a lateral promotion!
            if ((Rank.Grade == Position.Billet.Rank.Grade) && (Rank.Id != Position.Billet.Rank.Id))
            {
                status = PromotableStatus.Lateral;
                return true;
            }

            // Check for demotion
            if (Rank.Grade > Position.Billet.MaxRank.Grade)
            {
                status = PromotableStatus.Demotion;
                return true;
            }

            // Check if soldier is promotable based on TIG
            int months = currentDate.Id - LastGradeChangeDate.Id;
            bool promotable = (months >= Rank.PromotableAt);

            // If the soldier is promotable, then set the status
            if (promotable)
            {
                if (Rank.AutoPromote)
                {
                    // Automatic rank promotion
                    status = PromotableStatus.Automatic;
                    return true;
                }
                else if (Rank.Grade < Position.Billet.Rank.Grade)
                {
                    // Normal billet based promotion
                    status = PromotableStatus.Position;
                    return true;
                }
                else if (Position.Billet.AutoPromoteInRankRange && Rank.Grade < Position.Billet.MaxRank.Grade)
                {
                    // Automatic billet based promotion
                    status = PromotableStatus.Position;
                    return true;
                }
                else
                {
                    // Normally promotable
                    status = PromotableStatus.Normal;
                    return true;
                }
            }
            else
            {
                status = PromotableStatus.None;
                return false;
            }
        }

        /// <summary>
        /// Logs the current assignment in the database as a Past Assignment
        /// and completely removes this soldier from the unit roster.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="db"></param>
        public void RemoveFromPosition(IterationDate date, SimDatabase db)
        {
            // Remove soldier from old unit, and place in new
            if (Position != null)
            {
                // Store past assignment
                LogAssignment(date, db);

                // Clear
                Position.AssignSoldier(null);
                Position = null;
            }
        }

        /// <summary>
        /// Assigns this <see cref="Soldier"/> to the specified <see cref="Position"/>
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="date"></param>
        /// <param name="db"></param>
        public void AssignPosition(PositionWrapper newPosition, IterationDate date, SimDatabase db)
        {
            // Remove soldier from old unit, and place in new
            RemoveFromPosition(date, db);

            // Assign the soldier to the new unit, and position
            Position = newPosition;
            newPosition.AssignSoldier(this);

            // Specialty change required?
            var specialty = newPosition.Billet.Specialty;
            if (specialty != null && specialty.Id != Soldier.SpecialtyId)
            {
                AssignSpecialty(specialty.Id, date, db);
            }

            // Add billet to list
            if (!BilletsHeld.ContainsKey(newPosition.Billet.Id))
            {
                BilletsHeld.Add(newPosition.Billet.Id, newPosition.Billet);
            }

            // Create assignment
            Assignment.AssignedIteration = date.Id;
            Assignment.PositionId = newPosition.Position.Id;
            Assignment.SoldierId = Soldier.Id;
        }

        /// <summary>
        /// Gets the current number of months this soldier has been alive
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetTimeInService(IterationDate currentDate)
        {
            return currentDate.Id - Soldier.EntryIterationId;
        }

        /// <summary>
        /// Gets the current number of months this soldier has been in this <see cref="Rank.Grade"/>
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetTimeInGrade(IterationDate currentDate)
        {
            return currentDate.Id - LastGradeChangeDate.Id;
        }

        /// <summary>
        /// Gets the current number of months this soldier has been in this <see cref="Database.Position"/>
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetTimeInBillet(IterationDate currentDate)
        {
            return currentDate.Id - Assignment.AssignedIteration;
        }

        /// <summary>
        /// Gets the current number of months this soldier has before he will retire naturally
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetTimeUntilRetirement(IterationDate currentDate)
        {
            // Reversed, since ExitIterationId will always be greater or equal to
            // the current iteration date
            return Soldier.ExitIterationId - currentDate.Id;
        }

        /// <summary>
        /// Indicates whether this soldier is a stand in for their current <see cref="Database.Position"/>.
        /// To be considered a stand in, the soldier must be a lower <see cref="Rank.Grade"/> than the
        /// <see cref="Rank.Grade"/>
        /// </summary>
        /// <returns></returns>
        public bool IsStandIn()
        {
            return (Rank.Grade < Position.Billet.Rank.Grade);
        }

        /// <summary>
        /// Determines whether this soldier is nearing his <see cref="Billet.MaxTourLength"/>
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        public bool IsNearMaxTourLength(IterationDate currentDate)
        {
            if (Position == null) return true;

            if (Position.Billet.MaxTourLength > 0)
            {

                int timeLeft = Position.Billet.MaxTourLength - GetTimeInBillet(currentDate);
                int timeToRetire = Soldier.ExitIterationId - currentDate.Id;

                // If we are going to retire before we hit max tour length, just return false
                if (timeToRetire < timeLeft)
                    return false;

                // Get the median difference between max and min tour lengths
                double len = Position.Billet.MaxTourLength - Position.Billet.MinTourLength;
                int medium = (int)Math.Ceiling(len / 2);
                return (timeLeft <= medium);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether this soldier is at or past thier Max Tour Length
        /// </summary>
        /// <param name="currentDate">the current simulation <see cref="IterationDate"/></param>
        /// <returns></returns>
        public bool IsPastMaxTourLength(IterationDate currentDate)
        {
            if (Position == null) return true;

            if (Position.Billet.MaxTourLength > 0)
            {

                int timeLeft = Position.Billet.MaxTourLength - GetTimeInBillet(currentDate);
                return (timeLeft < 1);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether this <see cref="Soldier"/> is currently locked in by
        /// <see cref="Billet.MinTourLength"/>
        /// </summary>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public bool IsLockedInPosition(IterationDate currentDate)
        {
            if (Position == null) return false;

            int timePassed = currentDate.Id - Assignment.AssignedIteration;
            return Position.Billet.MinTourLength > timePassed;
        }

        /// <summary>
        /// Removes this soldier recursivly from the specified unit
        /// and all of it's parent units
        /// </summary>
        /// <param name="unit"></param>
        protected void RemoveSoldierRecursively(UnitWrapper unit)
        {
            // Remove soldier from old unit
            unit.RemoveSoldier(this);
        }

        /// <summary>
        /// Adds this soldier recursivly from the specified unit
        /// and all of it's parent units
        /// </summary>
        /// <param name="unit"></param>
        protected void AddSoldierRecursively(UnitWrapper unit)
        {
            unit.AddSoldier(this);
        }

        /// <summary>
        /// Gives the soldier the indicated experience
        /// </summary>
        /// <param name="item"></param>
        public void GiveExperience(BilletExperience item)
        {
            // Ensure we have a key
            if (!Experience.ContainsKey(item.ExperienceId))
                Experience.Add(item.ExperienceId, 0);

            // Add at the given rate
            Experience[item.ExperienceId] += item.Rate;
        }

        /// <summary>
        /// Returns the current experience value this soldier has
        /// </summary>
        /// <param name="experienceId"></param>
        /// <returns></returns>
        public int GetExperienceValue(int experienceId)
        {
            Experience.TryGetValue(experienceId, out int val);
            return val;
        }

        /// <summary>
        /// Evalutates the <see cref="AbstractFilter"/>, returning a reversed
        /// integer value of the result, which is used for ordering Groups
        /// </summary>
        /// <param name="selector">the <see cref="AbstractFilter"/> to evaluate</param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        /// <returns>0 if true, 1 if false</returns>
        public int EvaluateLookUpReverse(AbstractFilter selector, IterationDate date)
        {
            return (EvaluateFilter(selector, date)) ? 0 : 1;
        }

        /// <summary>
        /// Gets a value from this <see cref="Database.Soldier"/>
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="selectorId"></param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetValue(ClauseLeftSelector selector, int selectorId, IterationDate date)
        {
            switch (selector)
            {
                default:
                    throw new ArgumentOutOfRangeException("selector");
                case ClauseLeftSelector.SoldierExperience:
                    return GetExperienceValue(selectorId);
                case ClauseLeftSelector.SoldierPosition:
                    return GetPositionValue((PositionFunction)selectorId, date);
                case ClauseLeftSelector.SoldierValue:
                    return GetSoldierValue((SoldierFunction)selectorId, date);
            }
        }

        /// <summary>
        /// Gets a soldier value from this <see cref="Database.Soldier"/>
        /// </summary>
        /// <param name="function"></param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetSoldierValue(SoldierFunction function, IterationDate date)
        {
            switch (function)
            {
                default:
                case SoldierFunction.TimeInService:
                    return GetTimeInService(date);
                case SoldierFunction.TimeInGrade:
                    return GetTimeInGrade(date);
                case SoldierFunction.TimeInPosition:
                    return GetTimeInBillet(date);
                case SoldierFunction.TimeInRank:
                    return date.MonthsDifference(LastPromotionDate);
                case SoldierFunction.TimeToRetirement:
                    return GetTimeUntilRetirement(date);
            }
        }

        /// <summary>
        /// Gets a positional value from this <see cref="Database.Soldier"/>
        /// </summary>
        /// <param name="function"></param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetPositionValue(PositionFunction function, IterationDate date)
        {
            switch (function)
            {
                default:
                case PositionFunction.BilletId:
                    return Position.Billet.Id;
                case PositionFunction.BilletStature:
                    return Position.Position.Billet.Stature;
                case PositionFunction.IsNormalAssignment:
                    return (Position.Billet.Billet.Flag == BilletFlag.NormalAssignment) ? 1 : 0;
                case PositionFunction.IsCommandPosition:
                    return (Position.Billet.Billet.Flag == BilletFlag.CommandPosition) ? 1 : 0;
                case PositionFunction.IsSpecialAssignment:
                    return (Position.Billet.Billet.Flag == BilletFlag.SpecialAssignment) ? 1 : 0;
                case PositionFunction.IsStaffPosition:
                    return (Position.Billet.Billet.Flag == BilletFlag.StaffPosition) ? 1 : 0;
            }
        }

        /// <summary>
        /// Evaluates the provided <see cref="AbstractFilter"/> against this soldier
        /// and returns the result
        /// </summary>
        /// <param name="filter">the <see cref="AbstractFilter"/> to evaluate</param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        /// <returns></returns>
        public bool EvaluateFilter(AbstractFilter filter, IterationDate date)
        {
            int leftValue = 0;
            switch (filter.Selector)
            {
                case ClauseLeftSelector.SoldierExperience:
                    leftValue = GetExperienceValue(filter.SelectorId);
                    break;
                case ClauseLeftSelector.SoldierPosition:
                    leftValue = GetPositionValue((PositionFunction)filter.SelectorId, date);
                    break;
                case ClauseLeftSelector.SoldierValue:
                    leftValue = GetSoldierValue((SoldierFunction)filter.SelectorId, date);
                    break;
            }

            return Condition.EvaluateExpression(leftValue, filter.Operator, filter.RightValue);
        }

        /// <summary>
        /// Saves the current soldier data to the database.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        public void Save(SimDatabase database, IterationDate date)
        {
            database.Soldiers.Update(Soldier);

            // Save current position
            if (Position != null && !Soldier.Retired)
            {
                database.Assignments.Add(Assignment);
            }

            // Save promotions
            foreach (var item in Promotions)
            {
                database.Promotions.Add(item);
            }

            // Save past assignments
            foreach (var item in PastAssignments)
            {
                database.PastAssignments.Add(item);
            }

            // Save experiences
            foreach (var exp in Experience)
            {
                var item = new SoldierExperience()
                {
                    Soldier = Soldier,
                    ExperienceId = exp.Key,
                    Value = exp.Value
                };
                database.SoldierExperience.Add(item);
            }
        }

        /// <summary>
        /// Moves the current soldier assignment into a past assignment
        /// </summary>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        private void LogAssignment(IterationDate date, SimDatabase db)
        {
            // Store past assignment
            PastAssignments.Add(new PastAssignment()
            {
                PositionId = Position.Position.Id,
                SoldierId = Soldier.Id,
                RemovedIteration = date.Id,
                StartIteration = Assignment.AssignedIteration
            });
        }

        public bool Equals(SoldierWrapper other)
        {
            if (other == null || other.Soldier == null) return false;
            return (Soldier.Id == other.Soldier.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SoldierWrapper);
        }

        public override int GetHashCode() => Soldier?.Id ?? 0;

        /// <summary>
        /// Converts this object into the soldiers name
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        public void Dispose() => Dispose(true);

        protected void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                BilletsHeld.Clear();
                BilletsHeld = null;

                Promotions.Clear();
                Promotions = null;

                PastAssignments.Clear();
                PastAssignments = null;

                GC.SuppressFinalize(this);
                Disposed = true;
            }
        }

        ~SoldierWrapper()
        {
            Dispose(false);
        }
    }
}
