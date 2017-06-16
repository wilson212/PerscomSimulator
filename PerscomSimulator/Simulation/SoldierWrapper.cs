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
        /// 
        /// </summary>
        public Soldier Soldier { get; protected set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Rank"/>
        /// </summary>
        public Rank Rank { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Assignment Assignment { get; protected set; } = new Assignment();

        /// <summary>
        /// 
        /// </summary>
        public IterationDate EntryServiceDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IterationDate LastPromotionDate { get; set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Position"/>, or null
        /// </summary>
        public PositionWrapper Position { get; protected set; }

        public Dictionary<int, BilletWrapper> BilletsHeld { get; set; } = new Dictionary<int, BilletWrapper>();

        public bool Disposed { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SoldierWrapper"/>
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="rank"></param>
        public SoldierWrapper(Soldier soldier, Rank rank, IterationDate date)
        {
            Soldier = soldier;
            soldier.Rank = rank;
            Rank = rank;
            EntryServiceDate = date;
            LastPromotionDate = date;
        }

        /// <summary>
        /// Retires the soldier by releaseing this instance from their <see cref="Position"/>
        /// </summary>
        public void Retire(IterationDate currentDate, SimDatabase db)
        {
            if (Position != null)
            {
                // Move current Assingment to Past
                LogAssignment(currentDate, db);

                // Remove soldier from old unit, and place in new
                Position.AssignSoldier(null);
                Position = null;

                Soldier.Retired = true;
                Soldier.ExitIterationId = currentDate.Id;
                
                // Save soldier properties
                Soldier.RankId = Rank.Id;
                Soldier.LastPromotionIterationId = LastPromotionDate.Id;
            }
        }

        // <summary>
        /// Returns whether this soldier's has lifespan has expired
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <returns></returns>
        public bool IsRetiring(IterationDate currentDate)
        {
            bool rcp = false;
            if (Rank.MaxTimeInGrade > 0)
            {
                int months = currentDate.Date.MonthDifference(LastPromotionDate.Date);
                rcp = months >= Rank.MaxTimeInGrade;
            }

            return (currentDate.Id >= Soldier.ExitIterationId || rcp);
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
            db.Promotions.Add(new Promotion()
            {
                IterationId = date.Id,
                SoldierId = Soldier.Id,
                FromRankId = Soldier.RankId,
                ToRankId = newRank.Id,
                TimeInService = date.Id - Soldier.EntryIterationId,
                PreviousTimeInGrade = date.Id - LastPromotionDate.Id
            });

            // Remove soldier before promoting!
            RemoveSoldierRecursively(Position.ParentUnit);

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
            // Check if soldier is promotable based on TIG
            int months = currentDate.Id - LastPromotionDate.Id;
            bool promotable = (months >= Rank.PromotableAt);

            // If the soldier is promotable, then set the status
            if (promotable)
            {
                if (Rank.AutoPromote)
                {
                    status = PromotableStatus.Automatic;
                    return true;
                }
                else if (Rank.Type != Position.Billet.Rank.Type)
                {
                    status = PromotableStatus.Position;
                    return true;
                }
                else if (Rank.Grade < Position.Billet.Rank.Grade)
                {
                    status = PromotableStatus.Position;
                    return true;
                }
                else if ((Rank.Grade == Position.Billet.Rank.Grade) && (Rank.Id != Position.Billet.Rank.Id))
                {
                    status = PromotableStatus.Lateral;
                    return true;
                }
                else
                {
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
        /// Assigns this <see cref="Soldier"/> to the specified <see cref="Position"/>
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="date"></param>
        /// <param name="db"></param>
        public void AssignPosition(PositionWrapper newPosition, IterationDate date, SimDatabase db)
        {
            // Remove soldier from old unit, and place in new
            if (Position != null)
            {
                Position.AssignSoldier(null);

                // Store past assignment
                LogAssignment(date, db);
            }

            // Assign the soldier to the new unit, and position
            newPosition.AssignSoldier(this);
            Position = newPosition;

            // Specialty change required?
            var specialty = newPosition.Billet.Specialty;
            if (specialty != null)
                Soldier.Specialty = specialty;

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

        public int GetTimeInService(IterationDate currentDate)
        {
            return currentDate.Id - Soldier.EntryIterationId;
        }

        protected void RemoveSoldierRecursively(UnitWrapper unit)
        {
            // Remove soldier from old unit
            unit.RemoveSoldier(this);
        }

        protected void AddSoldierRecursively(UnitWrapper unit)
        {
            unit.AddSoldier(this);
        }

        /// <summary>
        /// Saves the current soldier data to the database.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="date"></param>
        public void Save(SimDatabase database, IterationDate date)
        {
            database.Soldiers.Update(Soldier);

            if (Position != null && !Soldier.Retired)
            {
                database.Assignments.Add(Assignment);
            }
        }

        /// <summary>
        /// Moves the current soldier assignment into a past assignment
        /// </summary>
        /// <param name="date"></param>
        private void LogAssignment(IterationDate date, SimDatabase db)
        {
            // Store past assignment
            db.PastAssignments.Add(new PastAssignment()
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
