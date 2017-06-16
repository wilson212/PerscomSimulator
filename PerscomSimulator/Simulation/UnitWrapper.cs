using System;
using System.Collections.Generic;
using Perscom.Database;

namespace Perscom.Simulation
{
    public class UnitWrapper
    {
        /// <summary>
        /// Gets or Sets the name of this Unit instance
        /// </summary>
        public string Name => Unit.Name;

        /// <summary>
        /// The parent unit of this unit instance
        /// </summary>
        public Unit Unit { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Echelon"/> level of this unit
        /// </summary>
        public Echelon Echelon { get; set; }

        /// <summary>
        /// The parent unit of this unit instance
        /// </summary>
        public UnitWrapper Parent { get; set; }

        /// <summary>
        /// Gets the topmost unit in which this unit's billets
        /// can pull soldiers from
        /// </summary>
        public UnitWrapper PromotionPoolUnit { get; set; }

        /// <summary>
        /// Gets or Sets a list of all soldier positions in this Unit (excluding sub units).
        /// </summary>
        public List<PositionWrapper> Positions { get; set; }

        /// <summary>
        /// A list of all <see cref="Unit"/>s that fall under this one
        /// </summary>
        public List<UnitWrapper> Subunits { get; set; }

        /// <summary>
        /// RankType => [Grade => [SoldierID => SoldierWrapper]]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, Dictionary<int, SoldierWrapper>>> SoldiersByGrade { get; set; }

        /// <summary>
        /// Creates a new instance of UnitWrapper
        /// </summary>
        public UnitWrapper(Unit unit, UnitTemplateWrapper template, UnitWrapper parent)
        {
            Unit = unit;
            Parent = parent;
            Echelon = template.Echelon;
            var stats = UnitBuilder.GetUnitStatistics(template.Template);

            SoldiersByGrade = new Dictionary<RankType, Dictionary<int, Dictionary<int, SoldierWrapper>>>();
            Positions = new List<PositionWrapper>(stats.PositionCount + 1);
            Subunits = new List<UnitWrapper>();

            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                int count = stats.SoldierCountsByGrade[type].Count;
                SoldiersByGrade.Add(type, new Dictionary<int, Dictionary<int, SoldierWrapper>>(count));

                foreach (var grade in stats.SoldierCountsByGrade[type])
                {
                    int size = grade.Value == 0 ? 4 : grade.Value;
                    SoldiersByGrade[type].Add(grade.Key, new Dictionary<int, SoldierWrapper>(size));
                }
            }

            // Get our soldier promotion pool
            Echelon promotionP = template.PromotionPool;
            if (parent == null || promotionP.HierarchyLevel == template.Echelon.HierarchyLevel)
            {
                PromotionPoolUnit = this;
            }
            else if (promotionP.HierarchyLevel == 99)
            {
                PromotionPoolUnit = parent.PromotionPoolUnit;
            }
            else
            {
                // Loop through each parent unit, and find our promotion pool
                UnitWrapper parentUnit = Parent;
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
        /// Returns a List of all positions in this unit, and it's sub units.
        /// </summary>
        /// <returns></returns>
        public List<PositionWrapper> GetAllPositions()
        {
            var val = new List<PositionWrapper>(Positions);
            foreach (UnitWrapper sub in Subunits)
            {
                val.AddRange(sub.GetAllPositions());
            }

            return val;
        }

        /// <summary>
        /// Adds the <see cref="Soldier"/> to the unit roster.
        /// </summary>
        /// <param name="soldier"></param>
        public void AddSoldier(SoldierWrapper soldier)
        {
            if (soldier != null)
            {
                var rank = soldier.Rank;
                SoldiersByGrade[rank.Type][rank.Grade].Add(soldier.Soldier.Id, soldier);

                if (Parent != null)
                    Parent.AddSoldier(soldier);
            }
        }

        /// <summary>
        /// Removes the <see cref="Soldier"/> to the unit roster.
        /// </summary>
        /// <param name="soldier"></param>
        public void RemoveSoldier(SoldierWrapper soldier)
        {
            if (soldier == null) return;

            if (!SoldierExistsInUnit(soldier))
            {
                // We have a major problem!
                throw new Exception("Soldier not found in unit");
            }

            var rank = soldier.Rank;
            SoldiersByGrade[rank.Type][rank.Grade].Remove(soldier.Soldier.Id);

            // Check if soldier exists
            if (SoldierExistsInUnit(soldier))
            {
                // We have a major problem!
                throw new Exception("Soldier is not being removed properly from UnitWrapper!");
            }


            if (Parent != null)
                Parent.RemoveSoldier(soldier);
        }

        protected bool SoldierExistsInUnit(SoldierWrapper soldier)
        {
            var rank = soldier.Rank;
            return SoldiersByGrade[rank.Type][rank.Grade].ContainsKey(soldier.Soldier.Id);
        }

        public override string ToString()
        {
            return (Parent == null) ? Name : String.Concat(Name, ", ", Parent.ToString());
        }
    }
}
