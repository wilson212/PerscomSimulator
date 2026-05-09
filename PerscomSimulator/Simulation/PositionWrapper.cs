using System;
using System.Text;
using System.Threading;
using Perscom.Database;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents a Billit, as part of a unit, that a <see cref="Soldier"/>
    /// will occupy while active.
    /// </summary>
    public class PositionWrapper : IEquatable<PositionWrapper>
    {
        /// <summary>
        /// Gets or Sets the name of this position
        /// </summary>
        public string Name => Position.Name;

        /// <summary>
        /// 
        /// </summary>
        public Position Position { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public PositionBlueprintWrapper BlueprintWrapper { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="UniWrapper"/> That this position
        /// belongs to.
        /// </summary>
        public UnitWrapper ParentUnit { get; set; }

        /// <summary>
        /// Gets the topmost unit in which this billet can pull soldiers from
        /// to fill.
        /// </summary>
        public UnitWrapper PromotionPoolUnit { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Soldier"/> who occupies this position currently
        /// </summary>
        public SoldierWrapper Holder { get; protected set; }

        /// <summary>
        /// Indicates whether this position is filled or empty
        /// </summary>
        public bool IsEmpty => Holder == null;

        /// <summary>
        /// Indicates whether the position is marked as queued for vacancy processing.
        /// A value of 0 represents not queued, and a value of 1 represents queued.
        /// </summary>
        private int _isVacantQueued;
        
        public PositionWrapper SupervisorPosition { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PositionWrapper"/>
        /// </summary>
        /// <param name="position">The position this instance is wrapping around</param>
        /// <param name="blueprint">The billet template for this position</param>
        /// <param name="parent">The <see cref="UnitWrapper"/> this position is attached to</param>
        public PositionWrapper(Position position, PositionBlueprint blueprint, UnitWrapper parent, SimDatabase db)
        {
            // Set properties
            Position = position ?? throw new ArgumentNullException("position");
            ParentUnit = parent ?? throw new ArgumentNullException("parent");
            BlueprintWrapper = SimulationCache.FetchBillet(blueprint, db);

            // Get our soldier promotion pool
            Echelon promotionP = BlueprintWrapper.PromotionPool;
            if (parent.Parent == null || promotionP.HierarchyLevel == 99)
            {
                PromotionPoolUnit = parent.PromotionPoolUnit;
            }
            else
            {
                // Loop through each parent unit, and find our promotion pool
                UnitWrapper parentUnit = parent;
                while (parentUnit != null)
                {
                    if (parentUnit.Echelon.HierarchyLevel >= promotionP.HierarchyLevel || parentUnit.Parent == null)
                    {
                        PromotionPoolUnit = parentUnit;
                        break;
                    }

                    // Raise the parent unit up one level
                    parentUnit = parentUnit.Parent;
                }
            }
        }

        /// <summary>
        /// Assigns the specified <see cref="Soldier"/> to this position
        /// </summary>
        /// <param name="soldier"></param>
        public void AssignSoldier(SoldierWrapper soldier)
        {
            // Mark spot vacant?
            if (soldier is null)
            {
                Holder = null;
                return;
            }
            
            // Remove this soldier from the old position
            ParentUnit.RemoveSoldier(Holder);

            // Set a new position holder to this local soldier
            ParentUnit.AddSoldier(soldier);
            Holder = soldier;
        }
        
        /// <summary>
        /// Attempts to mark this position as vacant, and returns true if successful.
        /// </summary>
        /// <returns></returns>
        public bool TryMarkVacantQueued()
        {
            return Interlocked.CompareExchange(ref _isVacantQueued, 1, 0) == 0;
        }

        /// <summary>
        /// Resets the "vacant queued" state of this position to indicate that it is no longer marked as queued for vacancy processing.
        /// </summary>
        public void ClearVacantQueued()
        {
            Volatile.Write(ref _isVacantQueued, 0);
        }
        
        #region Operator Overloads
        
        public static bool operator ==(PositionWrapper a, PositionWrapper b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }
        
        public static bool operator !=(PositionWrapper a, PositionWrapper b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as PositionWrapper);
        }

        public bool Equals(PositionWrapper other)
        {
            if (other is null) return false;
            return (Position.Id == other.Position.Id);
        }
        
        #endregion

        public override string ToString()
        {
            StringBuilder b = new StringBuilder(Name);
            if (Holder != null)
            {
                b.Append($" ({Holder})");
            }
            else
            {
                b.Append(" (empty)");
            }
            if (ParentUnit != null)
            {
                b.Append($", {ParentUnit}");
            }
            return b.ToString();
        }
    }
}
