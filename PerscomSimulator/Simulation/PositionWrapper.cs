using System;
using System.Linq;
using System.Text;
using Perscom.Database;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents a Billit, as part of a unit, that a <see cref="Soldier"/>
    /// will occupy while active.
    /// </summary>
    public class PositionWrapper
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
        public BilletWrapper Billet { get; protected set; }


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
        /// Creates a new instance of <see cref="PositionWrapper"/>
        /// </summary>
        /// <param name="position">The position this instance is wrapping around</param>
        /// <param name="billet">The billet template for this position</param>
        /// <param name="parent">The <see cref="UnitWrapper"/> this position is attached to</param>
        public PositionWrapper(Position position, Billet billet, UnitWrapper parent)
        {
            // Set properties
            Position = position;
            Billet = BilletWrapper.FetchCache(billet);
            ParentUnit = parent;

            if (parent.Echelon.Id == 3 )
            {

            }

            // Get our soldier promotion pool
            Echelon promotionP = Billet.PromotionPool;
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
                    if (parentUnit.Echelon.HierarchyLevel >= promotionP.HierarchyLevel)
                    {
                        PromotionPoolUnit = parentUnit;
                        break;
                    }
                    else if (parentUnit.Parent == null)
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
            ParentUnit.RemoveSoldier(Holder);

            Holder = soldier;

            ParentUnit.AddSoldier(soldier);
        }

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
