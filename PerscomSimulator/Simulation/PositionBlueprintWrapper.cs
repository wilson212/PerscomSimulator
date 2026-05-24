using System;
using System.Collections.Generic;
using Perscom.Database;
using Perscom.Simulation.Procedures;
using System.Linq;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents a wrapper for a PositionBlueprint, providing additional functionality
    /// and integrating simulation-specific behaviors.
    /// </summary>
    public class PositionBlueprintWrapper
    {
        /// <summary>
        /// Gets the unique identifier for this position.
        /// </summary>
        public int Id => Blueprint.Id;

        /// <summary>
        /// Gets or sets the prominence level of the position. A higher value will cause
        /// this <see cref="Blueprint"/> to be filled with more experienced (time in grade) 
        /// soldiers, while lower values will be filled with more inexperienced soldiers.
        /// </summary>
        public int Stature => Blueprint.Stature;

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this <see cref="Blueprint"/> 
        /// before being allowed to leave. If the minimum amount is less than the remaining time 
        /// to live for the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        public int MinTourLength => Blueprint.MinTourLength;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this <see cref="Blueprint"/>
        /// before being forcefully pushed out.
        /// </summary>
        public int MaxTourLength => Blueprint.MaxTourLength;

        /// <summary>
        /// Represents the detailed blueprint configuration for a specific position. This property
        /// encapsulates all relevant rules, constraints, and settings related to the position,
        /// including rank, occupation, promotion parameters, and assignment policies. The
        /// <see cref="Blueprint"/> is pivotal in defining how this position behaves within the
        /// overall simulation or database structure.
        /// </summary>
        public PositionBlueprint Blueprint { get; private set; }

        /// <summary>
        /// Gets the designated rank associated with this position.
        /// This determines the rank level required for soldiers to occupy
        /// or be considered for this position during selection or promotion procedures.
        /// </summary>
        public Rank Rank { get; private set; }

        /// <summary>
        /// Gets the occupation associated with this position. An incoming soldier will have his occupation set to this value upon assignment.
        /// </summary>
        public Occupation Occupation { get; private set; }

        /// <summary>
        /// Gets or sets the promotion eligibility hierarchy for positions associated
        /// with this wrapper. It defines the organizational level at which promotions
        /// are managed for personnel in this position.
        /// </summary>
        public Echelon PromotionPool { get; private set; }

        /// <summary>
        /// Gets or sets the list of occupations required for soldiers to occupy this position.
        /// </summary>
        public int[] RequiredOccupations { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this position creates new soldiers
        /// </summary>
        public bool CreatesNewSoldiers => FillProcedure == SelectionProcedure.CreateNewSoldier;

        /// <summary>
        /// Gets the selection procedure that determines how positions within the
        /// <see cref="PositionBlueprint"/> will be filled. This property is based on the
        /// <see cref="PositionBlueprint.SelectionMethod"/> and influences whether the position
        /// demands new soldier creation, lateral transfers, promotions, or a combination of these.
        /// </summary>
        public SelectionProcedure FillProcedure => Blueprint.SelectionMethod;

        /// <summary>
        /// Gets a value indicating whether the soldier holding this <see cref="PositionBlueprint"/> will be automatically
        /// promoted in <see cref="Database.Rank.PayGrade"/> if their current grade is lower than the
        /// <see cref="Rank.Classification.PayGrade"/>.
        /// </summary>
        public bool AutoPromoteInRankRange => Blueprint.AutoPromoteInRankRange;

        /// <summary>
        /// Gets a value indicating whether the soldier holding this <see cref="PositionBlueprint"/> will be automatically
        /// demoted in <see cref="Database.Rank.PayGrade"/> if their current grade is higher than the position's
        /// <see cref="Rank.Classification.PayGrade"/>.
        /// </summary>
        public bool DemoteOverRanked => Blueprint.DemoteOverRanked;

        /// <summary>
        /// Gets or sets a dictionary of performance models, where the key represents an attribute type and the value represents
        /// the associated performance modifier. This dictionary is used to calculate the expected performance level of a soldier
        /// based on their attributes.
        /// </summary>
        public Dictionary<AttributeType, int> PerformanceModels { get; set; }
        
        /// <summary>
        /// Gets the soldier selection procedure
        /// </summary>
        public AbstractSelectionProcedure Procedure { get; protected set; }

        /// <summary>
        /// Represents a wrapper for a PositionBlueprint, providing additional functionality
        /// and integrating simulation-specific behaviors.
        /// </summary>
        public PositionBlueprintWrapper(PositionBlueprint blueprint, SimDatabase db)
        {
            Blueprint = blueprint;
            Rank = blueprint.TargetRank;
            Occupation = blueprint.Occupation;
            PromotionPool = blueprint.PromotionPool;
            RequiredOccupations = blueprint.OccupationRequirements.Select(x => x.OccupationId).ToArray();
            
            // Cache performance models
            PerformanceModels = new Dictionary<AttributeType, int>();
            foreach (var model in blueprint.PerformanceModels)
            {
                PerformanceModels.Add(model.Attribute, model.ExpectedLevel);
            }

            switch (FillProcedure)
            {
                case SelectionProcedure.CreateNewSoldier:
                    Procedure = new SoldierEntryProcedure(db, blueprint);
                    break;
                case SelectionProcedure.LateralOnly:
                    Procedure = new LateralOnlyProcedure(db, blueprint);
                    break;
                case SelectionProcedure.PromotionOnly:
                    Procedure = new PromotionOnlyProcedure(db, blueprint);
                    break;
                case SelectionProcedure.PromotionOrLateral:
                    Procedure = new PromotionOrLateralProcedure(db, blueprint);
                    break;
                case SelectionProcedure.EvaluationBoard:
                    Procedure = new CustomSelectionProcedure(db, blueprint);
                    break;
                default:
                    var name = Enum.GetName(typeof(SelectionProcedure), FillProcedure);
                    throw new ArgumentException($@"Invalid selection procedure: {name}", nameof(FillProcedure));
            }
        }
    }
}
