using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Perscom.Collections;

namespace Perscom.Simulation
{
    public class SoldierWrapper : IKeyed<int>, IDisposable, IEquatable<SoldierWrapper>
    {
        private const int MAX_SKILL_LEVEL = 20;

        /// <summary>
        /// Gets the unique identifier associated with the soldier.
        /// </summary>
        public int Key => Entity.Id;

        /// <summary>
        /// Gets the soldiers first and last name
        /// </summary>
        public string Name => $"{Entity.FirstName} {Entity.LastName}";

        /// <summary>
        /// Gets the <see cref="Database.Soldier"/> object
        /// </summary>
        public Soldier Entity { get; protected set; }

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
        /// <see cref="Rank.PayGrade"/> changed.
        /// </summary>
        public IterationDate LastGradeChangeDate { get; protected set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Position"/>, or null
        /// </summary>
        public PositionWrapper Position { get; protected set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Occupation"/>
        /// </summary>
        public Occupation Occupation { get; protected set; }

        /// <summary>
        /// Gets a list of billets this soldier has held
        /// </summary>
        /// <remarks>
        /// [Position.Id => PositionWrapper]
        /// </remarks>
        public Dictionary<int, PositionBlueprintWrapper> PositionsHeld { get; set; } = new Dictionary<int, PositionBlueprintWrapper>();

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
        /// Gets or sets a collection of personality traits associated with the soldier, that are temporary
        /// </summary>
        public IdentityList<int, SoldierTraitAttachment> TemporaryTraits { get; set; } = new(8);
        
        /// <summary>
        /// Gets or sets a collection of this soldier's attributes.
        /// </summary>
        public Dictionary<AttributeType, SoldierAttribute> AttributesBase { get; set; } = new();
        
        /// <summary>
        /// Gets or sets a collection of this soldier's attributes, with their current value (including modifiers)
        /// </summary>
        public Dictionary<AttributeType, int> AttributesWithModifiers { get; set; } = new();
        
        /// <summary>
        /// Cached reference to the soldier's most recent board result.
        /// Loaded from DB on construction, updated in-memory when new results are written.
        /// </summary>
        public PromotionBoardResult LatestBoardResult { get; set; }

        /// <summary>
        /// Used for the Garbage Collector
        /// </summary>
        public bool Disposed { get; private set; }
        
        /// <summary>
        /// Ring buffer storing the last 12 months of morale values.
        /// Indexed by (iterationId % 12). Written on the soldier's own thread,
        /// read from other threads via Volatile.Read for the PREVIOUS month's slot.
        /// </summary>
        private readonly int[] _moraleHistory = new int[12];

        /// <summary>
        /// Ring buffer storing the last 12 months of form ratings (scaled x10).
        /// </summary>
        private readonly int[] _formRatingHistory = new int[12];
        
        /// <summary>
        /// Ring buffer storing the last 12 months of burnout score.
        /// </summary>
        private readonly int[] _burnoutHistory = new int[12];

        #region Events

        /// <summary>
        /// Event fired after <see cref="AssignPosition(PositionWrapper, IterationDate, SimDatabase, bool)"/> is called,
        /// and there is NO rank change preformed
        /// </summary>
        public static event EventHandler<PositionChangeEventArgs> OnPositionChange;

        /// <summary>
        /// Event fired after <see cref="AssignPosition(PositionWrapper, IterationDate, SimDatabase, bool)"/> is called,
        /// and there is a rank change preformed
        /// </summary>
        public static event EventHandler<PositionAndRankChangeEventArgs> OnPositionAndRankChange;

        /// <summary>
        /// Event fired after <see cref="ExchangePositionsWith(SoldierWrapper)"/> is preformed
        /// </summary>
        public static event EventHandler<LateralPositionExchangeEventArgs> OnLateralPositionExchange;

        /// <summary>
        /// Event fired after <see cref="PromoteTo(IterationDate, Rank, SimDatabase, bool)"/> is preformed,
        /// and the Rank PayGrade was changed
        /// </summary>
        public static event EventHandler<RankChangeEventArgs> OnRankGradeChange;

        /// <summary>
        /// Event fired after <see cref="PromoteTo(IterationDate, Rank, SimDatabase, bool)"/> is preformed,
        /// and the Rank PoolSelection was changed
        /// </summary>
        public static event EventHandler<RankChangeEventArgs> OnRankTypeChange;

        /// <summary>
        /// Event occurs BEFORE the occupation changes
        /// </summary>
        public static event EventHandler<SpecialtyChangeEventArgs> OnSpecialtyChange;

        /// <summary>
        /// Event occurs AFTER the soldier is retired
        /// </summary>
        public static event EventHandler<SoldierWrapper> OnRetire;

        #endregion Events

        /// <summary>
        /// Creates a new instance of <see cref="SoldierWrapper"/>
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="date"></param>
        public SoldierWrapper(Soldier soldier, IterationDate date, SimDatabase db)
        {
            Entity = soldier;
            Rank = soldier.Rank;
            EntryServiceDate = date;
            LastPromotionDate = date;
            LastGradeChangeDate = date;
            
            // Need to load Attributes and SoldierTraits
            foreach (var attr in soldier.Attributes)
            {
                AttributesBase[attr.Attribute] = attr;
                AttributesWithModifiers[attr.Attribute] = attr.Value;
            }
    
            // Apply trait effects to in-memory Attributes and track temporary traits
            foreach (var trait in soldier.Traits)
            {
                // Apply all trait effects to the wrapper's Attributes
                foreach (var effect in trait.Trait.TraitEffects)
                {
                    if (AttributesBase.ContainsKey(effect.TargetAttribute))
                    {
                        AttributesWithModifiers[effect.TargetAttribute] = Math.Clamp(
                            AttributesBase[effect.TargetAttribute].Value + effect.Modifier, 
                            0,
                            MAX_SKILL_LEVEL
                        );
                    }
                }
        
                // Track temporary traits for later expiration
                if (trait.Duration != -1)
                    TemporaryTraits.Add(trait);
            }
            
            // Get the latest board result
            LatestBoardResult = Entity.PromotionBoardResults.OrderByDescending(r => r.IterationId).FirstOrDefault();
            
            // Load last 12 months of morale and form rating
            foreach (var record in Entity.MonthlyRecords)
            {
                _moraleHistory[record.MonthIndex] = record.Morale;
                _formRatingHistory[record.MonthIndex] = record.FormRating;
                _burnoutHistory[record.MonthIndex] = record.Burnout;
            }
        }

        /// <summary>
        /// Processes the monthly update for a soldier, including morale recalculation,
        /// flight risk assessment, and removal of expired temporary traits.
        /// </summary>
        /// <remarks>
        /// This method shall be called once per Iteration, with <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, Action{TSource})"/>
        /// </remarks>
        /// <param name="currentDate">The current date of the simulation iteration.</param>
        /// <param name="db">The database instance to use for any database operations.</param>
        public void ProcessMonthlyUpdate(IterationDate currentDate, SimDatabase db)
        {
            // Increment TIG/TIS (already tracked via iteration dates)
            
            // Remove expired traits
            for (int i = TemporaryTraits.Count - 1; i >= 0; i--)
            {
                var temp = TemporaryTraits[i];
                if (temp.AssignedOnIterationId + temp.Duration <= currentDate.Id)
                {
                    // Remove internally O(1)
                    TemporaryTraits.Remove(i);
        
                    // Remove from database
                    db.SoldierTraitAttachments.Remove(temp);
        
                    // Revert trait effects
                    foreach (var effect in temp.Trait.TraitEffects)
                    {
                        var modifier = effect.Modifier * -1;
                        AttributesWithModifiers[effect.TargetAttribute] += modifier;
                    }
                }
            }
            
            // Skill Growth (modified by Improvability)
            ApplyMonthlySkillGrowth();

            // Recalculate morale
            RecalculateMorale(currentDate);
            
            // Recalculate flight risk
            RecalculateBurnout(currentDate);
            
            // At the END of ProcessMonthlyUpdate, after morale and form rating are finalized:
            int slot = currentDate.Id % 12;
            int morale = Entity.Morale;
            int formRating = (int)(CalculateFormRating() * 10f); // 1.0-9.9 → 10-99

            Volatile.Write(ref _moraleHistory[slot], morale);
            Volatile.Write(ref _formRatingHistory[slot], formRating);
            Volatile.Write(ref _burnoutHistory[slot], Entity.Burnout);
        }

        /// <summary>
        /// Retrieves the monthly record for the current iteration month.
        /// </summary>
        /// <param name="date">The iteration date used to determine the current month.</param>
        /// <returns>A <see cref="SoldierMonthlyRecord"/> representing the soldier's record for the current month.</returns>
        public SoldierMonthlyRecord GetCurrentMonthRecord(SimDatabase db, IterationDate date)
        {
            int slot = date.Id % 12;
            var entity = db.SoldierMonthlyRecords.Create();

            entity.SoldierId = Entity.Id;
            entity.MonthIndex = slot;
            entity.IterationId = date.Id;
            entity.Morale = Volatile.Read(ref _moraleHistory[slot]);
            entity.FormRating = Volatile.Read(ref _formRatingHistory[slot]);
            entity.Burnout = Volatile.Read(ref _burnoutHistory[slot]);
            
            return entity;
        }

        /// <summary>
        /// Assigns the specified <see cref="Database.Occupation"/> to this soldier
        /// </summary>
        /// <param name="specialty"></param>
        /// <param name="currentDate"></param>
        /// <param name="db"></param>
        public void AssignOccupation(Occupation specialty, IterationDate currentDate, SimDatabase db, bool fireEvent = true)
        {
            // Fire event?
            if (fireEvent)
            {
                OnSpecialtyChange?.Invoke(this, new SpecialtyChangeEventArgs()
                {
                    To = specialty,
                    From = Occupation
                });
            }

            // Create promotion record
            db.SpecialtyAssignments.Add(new OccupationAssignment()
            {
                AssignedIteration = currentDate.Id,
                SoldierId = Entity.Id,
                OccupationId = specialty.Id
            });

            // Set spec
            Occupation = specialty;
            Entity.OccupationId = specialty.Id;
        }

        /// <summary>
        /// Calculates the form rating for the currently assigned position of the soldier.
        /// </summary>
        /// <returns>
        /// A float value between 0.1 and 9.9, representing the computed form rating, scaled based
        /// on attributes, expectations, and morale.
        /// </returns>
        public float CalculateFormRating()
        {
            return CalculateFormRating(Position);
        }

        /// <summary>
        /// Calculates the performance rating of a soldier in relation to the position they are in.
        /// </summary>
        /// <returns>
        /// A float value between 0.1 and 9.9, representing the computed form rating, scaled based
        /// on attributes, expectations, and morale.
        /// </returns>
        public float CalculateFormRating(PositionWrapper position)
        {
            float soldierRawScore = 0;
            float expectedScore = 0;

            foreach (var model in position.BlueprintWrapper.Blueprint.PerformanceModels)
            {
                // Get the expected level
                int expectedLevel = model.ExpectedLevel; 

                // Calculate the score contribution
                soldierRawScore += AttributesWithModifiers[model.Attribute] * expectedLevel;
                expectedScore += expectedLevel * expectedLevel; // Compare against the expectation
            }

            if (expectedScore == 0) return 5.0f; // Neutral rating for a position with no requirements

            float performanceRatio = soldierRawScore / expectedScore;

            // Cap the ratio to prevent absurdly high scores for being overqualified.
            const float maxRatio = 1.1f; // Cap at 110%
            performanceRatio = Math.Min(performanceRatio, maxRatio);

            const float MIN_RATING = 1.0f;
            const float MAX_RATING = 9.9f;
            // We now scale based on the capped ratio
            float baseFormRating = MIN_RATING + ((performanceRatio / maxRatio) * (MAX_RATING - MIN_RATING));

            // Apply Morale as a final multiplier
            float finalFormRating = baseFormRating * (Entity.Morale / 100.0f);

            return Math.Max(MIN_RATING, finalFormRating);
        }

        /// <summary>
        /// Retires the soldier by releaseing this instance from their <see cref="Position"/>
        /// </summary>
        public void Retire(IterationDate currentDate, SimDatabase db)
        {
            // Fire Event
            OnRetire?.Invoke(this, this);

            // Remove soldier from current position
            if (Position != null)
            {
                // Remove ourselves from the position
                RemoveFromPosition(currentDate, db);
            }

            // Set database flags
            Entity.Retired = true;
            Entity.ExitIterationId = currentDate.Id;

            // Save soldier properties
            Entity.RankId = Rank.Id;
            Entity.LastPromotionIterationId = LastPromotionDate.Id;

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
            if (Position.BlueprintWrapper.MaxTourLength > 0)
            {
                int timeLeft = Position.BlueprintWrapper.MaxTourLength - GetTimeInBillet(currentDate);
                if (timeLeft <= 0 && !Position.BlueprintWrapper.Blueprint.Waiverable)
                    return true;
            }

            // Check for max time in grade
            var classif = Rank.Classification;
            if (classif.MaxTimeInGrade > 0)
            {
                int months = currentDate.Id - LastGradeChangeDate.Id;
                if (months >= classif.MaxTimeInGrade)
                    return true;
            }

            // If we hit the end of our career...
            int careerLength = GetTimeInService(currentDate);
            if (careerLength > Entity.TargetTIS)
            {
                // Check for locked in by billet (Time In PayGrade
                if (!Position.BlueprintWrapper.Blueprint.CanRetireEarly)
                {
                    int difference = currentDate.Id - Assignment.EntryIterationId;
                    if (Position.BlueprintWrapper.MinTourLength > difference)
                        return false;
                }

                // TODO Need to start steadily increasing their Burnout rate to force retirement soon
                
                // But for now...
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
        public Promotion PromoteTo(IterationDate date, Rank newRank, SimDatabase db, bool fireEvent = true)
        {
            // Ensure we arent being promoted to the same rank we are already
            if (newRank.Id == Entity.RankId) return null;

            // Create promotion record
            var promo = new Promotion()
            {
                IterationId = date.Id,
                SoldierId = Entity.Id,
                FromRankId = Entity.RankId,
                ToRankId = newRank.Id,
                TimeInService = date.Id - Entity.EntryIterationId,
                PreviousTimeInRank = date.Id - LastPromotionDate.Id,
                TimeSinceLastGradeChange = date.Id - LastGradeChangeDate.Id
            };
            Promotions.Add(promo);

            // Remove soldier before promoting!
            RemoveSoldierRecursively(Position.ParentUnit);

            // Log grade increase
            if (newRank.PayGrade != Rank.PayGrade || newRank.Type != Rank.Type)
            {
                LastGradeChangeDate = date;
                Entity.LastGradeChangeDate = date;
            }

            // Set new rank
            var oldRank = Rank;
            Rank = newRank;
            LastPromotionDate = date;
            Entity.RankId = newRank.Id;
            Entity.LastPromotionDate = date;

            // MoveTo soldier in unit roster
            AddSoldierRecursively(Position.ParentUnit);

            // Fire even ONLY if rank type did not change!
            if (fireEvent)
            {
                if (newRank.Type == oldRank.Type)
                {
                    OnRankGradeChange?.Invoke(this, new RankChangeEventArgs()
                    {
                        Promotion = promo,
                        Soldier = this
                    });
                }
                else
                {
                    OnRankTypeChange?.Invoke(this, new RankChangeEventArgs()
                    {
                        Promotion = promo,
                        Soldier = this
                    });
                }
            }

            // Return
            return promo;
        }

        /// <summary>
        /// If the soldier is promotable by position, this method will preform the promotion
        /// </summary>
        /// <param name="date"></param>
        /// <param name="db"></param>
        /// <param name="promotion"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool DoPromotionIfEligable(IterationDate date, SimDatabase db, bool fireEvent, out Promotion promotion)
        {
            // Do we promote?
            if (IsPromotable(date, out PromotableStatus status))
            {
                if (status == PromotableStatus.Automatic || status == PromotableStatus.Position)
                {
                    // Dont log promotion data if switching rank types!
                    if (Rank.Type != Position.BlueprintWrapper.Rank.Type)
                    {
                        // Promote Soldier
                        promotion = PromoteTo(date, Position.BlueprintWrapper.Rank, db, fireEvent);
                        return true;
                    }
                    else
                    {
                        // Get the expected soldier rank/grade to promote from
                        var expectedGrade = Position.BlueprintWrapper.Rank.PayGrade - 1;

                        // Promote soldier. Do not skip rank grades!
                        if (Rank.PayGrade == expectedGrade)
                        {
                            // Billet grade is 1 level higher or of a different PoolSelection!
                            promotion = PromoteTo(date, Position.BlueprintWrapper.Rank, db, fireEvent);
                            return true;
                        }
                        else
                        {
                            // Billet grade is multiple levels higher, SO we must promote one grade at
                            // a time!
                            Rank toRank = RankCache.GetNextGradeRanks(Rank).FirstOrDefault();
                            if (toRank != null)
                            {
                                promotion = PromoteTo(date, toRank, db, fireEvent);
                                return true;
                            }
                        }
                    }
                }
                else if (status == PromotableStatus.Lateral)
                {
                    // Do not log promotion since this is lateral
                    promotion = PromoteTo(date, Position.BlueprintWrapper.Rank, db, fireEvent);
                    return true;
                }
                else if (status == PromotableStatus.Demotion)
                {
                    // Does the position demote over ranked?
                    if (Position.BlueprintWrapper.Blueprint.DemoteOverRanked)
                    {
                        promotion = PromoteTo(date, Position.BlueprintWrapper.Rank, db, fireEvent);
                        return true;
                    }
                }
            }
            
            // Not promotable
            promotion = null;
            return false;
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
            if (Rank.Type != Position.BlueprintWrapper.Rank.Type)
            {
                status = PromotableStatus.Position;
                return true;
            }

            // Always approve a lateral promotion!
            if ((Rank.PayGrade == Position.BlueprintWrapper.Rank.PayGrade) && (Rank.Id != Position.BlueprintWrapper.Rank.Id))
            {
                status = PromotableStatus.Lateral;
                return true;
            }

            // Check for demotion
            if (Rank.PayGrade > Position.BlueprintWrapper.Rank.PayGrade)
            {
                status = PromotableStatus.Demotion;
                return true;
            }

            // Check if soldier is promotable based on TIG
            int months = currentDate.Id - LastGradeChangeDate.Id;
            var rankInfo = Rank.Classification;
            bool promotable = (months >= rankInfo.PreviousTimeInGradeRequirement);

            // If the soldier is promotable, then set the status
            if (promotable)
            {
                if (rankInfo.Selection == PayGradeSelection.Automatic)
                {
                    // Automatic rank promotion
                    status = PromotableStatus.Automatic;
                    return true;
                }
                else if (Rank.PayGrade < Position.BlueprintWrapper.Rank.PayGrade)
                {
                    // Normal billet-based promotion
                    status = PromotableStatus.Position;
                    return true;
                }
                else if (Position.BlueprintWrapper.AutoPromoteInRankRange && Rank.PayGrade < Position.BlueprintWrapper.Rank.PayGrade)
                {
                    // Automatic billet-based promotion
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
        /// Assigns the provided position to this soldier, and promotes as neccessary
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="date"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        protected PositionChangeEventArgs DoAssignPosition(PositionWrapper newPosition, IterationDate date, SimDatabase db)
        {
            // Vars for event
            var oldPosition = Position;
            var oldSpecialty = Occupation;
            int tib = GetTimeInBillet(date);

            // Remove soldier from old unit, and place in new
            RemoveFromPosition(date, db);

            // Assign the soldier to the new unit, and position
            Position = newPosition;
            newPosition.AssignSoldier(this);

            // Occupation change required?
            var specialty = newPosition.BlueprintWrapper.Occupation;
            if (specialty != null && specialty.Id != Entity.OccupationId)
            {
                AssignOccupation(specialty, date, db, false);
            }

            // Add billet to list 
            if (!PositionsHeld.ContainsKey(newPosition.BlueprintWrapper.Id))
            {
                PositionsHeld.Add(newPosition.BlueprintWrapper.Id, newPosition.BlueprintWrapper);
            }

            // Create assignment
            Assignment.EntryIterationId = date.Id;
            Assignment.PositionId = newPosition.Position.Id;
            Assignment.SoldierId = Entity.Id;
            Assignment.EntryRankId = Rank.Id;

            // Did we promote?
            if (DoPromotionIfEligable(date, db, false, out Promotion promo) && promo != null)
            {
                return new PositionAndRankChangeEventArgs()
                {
                    FromPosition = oldPosition,
                    ToPosition = Position,
                    FromSpecialty = oldSpecialty,
                    ToSpecialty = Occupation,
                    Promotion = promo,
                    FromPositionTimeInBillet = tib,
                    Soldier = this
                };
            }
            else
            {
                return new PositionChangeEventArgs()
                {
                    FromPosition = oldPosition,
                    ToPosition = Position,
                    FromSpecialty = oldSpecialty,
                    ToSpecialty = Occupation,
                    FromPositionTimeInBillet = tib,
                    Soldier = this
                };
            }
        }

        /// <summary>
        /// Assigns this <see cref="Entity"/> to the specified <see cref="Position"/>
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="date"></param>
        /// <param name="db"></param>
        /// <param name="fireEvent">Indicates whether to fire the <see cref="OnPositionChange"/> event</param>
        public void AssignPosition(PositionWrapper newPosition, IterationDate date, SimDatabase db, bool fireEvent = true)
        {
            // Preform the position assignment
            PositionChangeEventArgs args = DoAssignPosition(newPosition, date, db);

            // Fire event?
            if (fireEvent)
            {
                if (args is PositionAndRankChangeEventArgs)
                {
                    OnPositionAndRankChange?.Invoke(this, args as PositionAndRankChangeEventArgs);
                }
                else
                {
                    OnPositionChange?.Invoke(this, args);
                }
            }
        }

        /// <summary>
        /// Swaps the givens soldier position with this one
        /// </summary>
        /// <param name="soldier">The <see cref="Database.Soldier"/> we are exchanging positions with</param>
        /// <param name="date">The current simulation <see cref="IterationDate"/></param>
        /// <param name="db"></param>
        /// <param name="fireEvent">Indicates whether to fire the <see cref="OnLateralPositionExchange"/> event</param>
        public void ExchangePositionsWith(SoldierWrapper soldier, IterationDate date, SimDatabase db, bool fireEvent = true)
        {
            // Do some checks
            if (Position.BlueprintWrapper.Rank.Type != soldier.Position.BlueprintWrapper.Rank.Type)
            {
                throw new ArgumentException("Lateral position exchange failed! Rank type mis-matched");
            }

            // Create vars
            var s1_pos = Position;
            var s1_tib = GetTimeInBillet(date);
            var s2_pos = soldier.Position;
            var s2_tib = soldier.GetTimeInBillet(date);

            // Remove both soldiers from thier current position
            RemoveFromPosition(date, db);
            soldier.RemoveFromPosition(date, db);

            // Assign new positions
            var s1_args = DoAssignPosition(s2_pos, date, db);
            var s2_args = soldier.DoAssignPosition(s1_pos, date, db);

            // Fire event
            if (fireEvent)
            {
                // Adjust arguments
                s1_args.FromPosition = s1_pos;
                s1_args.FromPositionTimeInBillet = s1_tib;
                s2_args.FromPosition = s2_pos;
                s2_args.FromPositionTimeInBillet = s2_tib;

                // Create event args
                var args = new LateralPositionExchangeEventArgs()
                {
                    Soldier1EventArgs = s1_args,
                    Soldier2EventArgs = s2_args
                };

                // Fire event
                OnLateralPositionExchange?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Gets the current number of months this soldier has been alive
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        public int GetTimeInService(IterationDate currentDate)
        {
            return currentDate.Id - Entity.EntryIterationId;
        }

        /// <summary>
        /// Gets the current number of months this soldier has been in this <see cref="Rank.PayGrade"/>
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
            return currentDate.Id - Assignment.EntryIterationId;
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
            return Entity.ExitIterationId.Value - currentDate.Id;
        }

        /// <summary>
        /// Gets a selection factor rating (0-100) which is used to determine who is more/less
        /// likely to get selected for a lateral position based off of many individual factors.
        /// </summary>
        /// <param name="targetPosition">The position we are evaluating the soldier for</param>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns>A score from 0 to 100</returns>
        public double GetLateralSelectionFactor(PositionWrapper targetPosition, IterationDate currentDate)
        {
            double score = 0;

            // --- Ambition vs Stature delta (30 points) ---
            // A high ambition soldier wants higher stature positions more, lower stature less
            int ambition = AttributesBase.ContainsKey(AttributeType.Ambition) ? AttributesWithModifiers[AttributeType.Ambition] : 10;
            int currentStature = Position?.BlueprintWrapper.Stature ?? 0;
            int targetStature = targetPosition.BlueprintWrapper.Stature;
            int statureDelta = targetStature - currentStature;

            // Normalize ambition to 0-1 range (assuming 0-20 attribute scale)
            double ambitionFactor = ambition / 20.0;

            if (statureDelta > 0)
            {
                // Higher stature: ambitious soldiers score higher
                score += 15.0 + (15.0 * ambitionFactor);
            }
            else if (statureDelta == 0)
            {
                // Same stature: moderate desire
                score += 15.0 * ambitionFactor;
            }
            else
            {
                // Lower stature: ambitious soldiers don't want this
                score += 15.0 * (1.0 - ambitionFactor);
            }

            // --- Estimated Form Rating in new position (30 points) ---
            // How well will they perform? (1.0 to 9.9 scale)
            float estimatedForm = CalculateFormRating(targetPosition);
            score += (estimatedForm / 9.9) * 30.0;

            // --- Time in current position (20 points) ---
            // Are they stale? Score caps based on tour length
            int timeInBillet = GetTimeInBillet(currentDate);
            int maxTour = Position?.BlueprintWrapper.MaxTourLength ?? 36;
            if (maxTour <= 0) maxTour = 36;
            double billetRatio = Math.Min((double)timeInBillet / maxTour, 1.0);
            score += billetRatio * 20.0;

            // --- Time until retirement (10 points) ---
            // If close to retirement, less desire to move
            int timeToRetire = GetTimeUntilRetirement(currentDate);
            if (timeToRetire > 24)
            {
                score += 10.0;
            }
            else if (timeToRetire > 0)
            {
                score += (timeToRetire / 24.0) * 10.0;
            }

            // --- Time in Grade / seniority (10 points) ---
            int timeInGrade = GetTimeInGrade(currentDate);
            double tigScore = Math.Min(timeInGrade / 36.0, 1.0) * 10.0;
            score += tigScore;

            return Math.Max(0, Math.Min(100, score));
        }

        /// <summary>
        /// Determines whether the soldier will attempt to apply for a special assignment
        /// based on various factors such as ambition, ego, pride, retirement horizon,
        /// and human variance.
        /// </summary>
        /// <param name="openPosition">The position being evaluated for eligibility and suitability.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the soldier decides to attempt the special assignment;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool WillAttemptSpecialAssignment(PositionWrapper openPosition)
        {
            /*
            // ---------------------------------------------------------
            // 1. HARD VETTING (Legally Cannot)
            // ---------------------------------------------------------
            if (soldier.MonthsUntilRetirement <= openPosition.RequiredSchoolingMonths) 
                return false;

            // This represents their % chance of applying (0 to 100)
            double chanceToApply = 0;

            // ---------------------------------------------------------
            // 2. BASE DRIVE: AMBITION (Max 45 points)
            // ---------------------------------------------------------
            // Ambition is the engine. 20 Ambition = 45% base chance to apply.
            chanceToApply += (AttributesBase[AttributeType.Ambition] / 20.0) * 45.0;

            // ---------------------------------------------------------
            // 3. THE "DELUSION" FACTOR: FORM vs EGO (Max 30 points)
            // ---------------------------------------------------------
            // Get their actual form/skill for this job (0 to 20 scale)
            double actualForm = CalculateFormRating(openPosition); 
            
            // Ego acts as a percentage of how much of their "missing" skill they ignore.
            // If Form is 5.0, they are missing 4.9 points. 
            // With 100 Ego, they add 100% of that 4.9 back. They think they are a 9.9!
            double egoMultiplier = Entity.Ego / 100.0;
            double perceivedForm = actualForm + ((9.9 - actualForm) * egoMultiplier);
        
            // Convert that 0.0-9.9 perceived form into our 30-point bucket
            chanceToApply += (perceivedForm / 9.9) * 30.0;

            // ---------------------------------------------------------
            // 4. THE "PRIDE" FACTOR: PAYGRADE CHANGES
            // ---------------------------------------------------------
            int gradeDiff = openPosition.BlueprintWrapper.Rank.PayGrade - Entity.Rank.PayGrade; // Adjust based on how your Rank IDs map to PayGrades
            if (gradeDiff < 0)
            {
                // IT IS A DEMOTION! (e.g., E-7 to E-5 means gradeDiff is -2)
                int gradesLost = Math.Abs(gradeDiff);
                
                // Ego multiplies the penalty. 
                // 20 Ego dropping 2 grades = -100 points (They will NEVER do it).
                // 5 Ego dropping 2 grades = -30 points (They might still do it if Ambition is maxed!)
                double pridePenalty = gradesLost * (5 + (Entity.Ego * 2.5)); 
                chanceToApply -= pridePenalty; 
            }
            else if (gradeDiff > 0)
            {
                // It's a promotion! Give a flat bonus based on how many grades they skip.
                chanceToApply += (gradeDiff * 10.0);
            }

            // ---------------------------------------------------------
            // 5. RETIREMENT HORIZON (Max 25 points)
            // ---------------------------------------------------------
            int activeMonthsAfterSchool = soldier.MonthsUntilRetirement - openPosition.RequiredSchoolingMonths;
            double timeRatio = Math.Min(1.0, activeMonthsAfterSchool / 48.0); // 4 years post-school is ideal
            
            chanceToApply += (timeRatio * 25.0);

            // ---------------------------------------------------------
            // 6. THE HUMAN VARIANCE ROLL
            // ---------------------------------------------------------
            // Clamp the final percentage between 1% and 99% so there is ALWAYS a tiny chance
            // for a miracle or a completely irrational human decision.
            chanceToApply = Math.Clamp(chanceToApply, 1.0, 99.0);
            
            int diceRoll = _random.Next(1, 101); // Roll 1 to 100
            
            // If the dice roll is under their chance percentage, they apply!
            return diceRoll <= chanceToApply; 
            */
            return false;
        }
        
        /// <summary>
        /// Determines whether the soldier will attempt to submit a packet for a grade increase
        /// (forward promotion) based on ambition, form rating, time until retirement,
        /// and time in grade seniority.
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <param name="maxPayGrade">The maximum pay grade achievable for this soldier's rank type.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the soldier decides to attempt the grade increase;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool WillAttemptGradeIncrease(IterationDate currentDate, int maxPayGrade, int boardsPerYear)
        {
            int currentGrade = Rank.Classification.PayGrade;
            int ambition = AttributesBase.ContainsKey(AttributeType.Ambition)
                ? AttributesWithModifiers[AttributeType.Ambition]
                : 10;

            // ---------------------------------------------------------
            // 0. LOW AMBITION GATE
            // ---------------------------------------------------------
            // Soldiers with very low ambition (< 5) almost never volunteer.
            // Give them a flat 2% chance per month — just enough for rare outliers.
            if (ambition < 5)
            {
                return Random.Shared.Next(1, 101) <= 2;
            }

            // ---------------------------------------------------------
            // 0b. GRADE-SCALED AMBITION THRESHOLD
            // ---------------------------------------------------------
            // Higher grades require more ambition. People naturally fear
            // the responsibility that comes with senior positions.
            // E-3 needs ambition >= 5, E-6 needs >= 8, E-8 needs >= 10, etc.
            int ambitionThreshold = 5 + (currentGrade / 2);
            if (ambition < ambitionThreshold)
            {
                // They don't have the drive for THIS level. Tiny chance only.
                return Random.Shared.Next(1, 101) <= 3;
            }

            double points = 0;

            // ---------------------------------------------------------
            // 1. BASE DRIVE: AMBITION (Max 40 points)
            // ---------------------------------------------------------
            int naturalFactor = maxPayGrade - currentGrade;
            points += Math.Min(naturalFactor + (ambition * 2), 40);

            // ---------------------------------------------------------
            // 2. FORM RATING: WEIGHTED SKILLS (Max 10 points)
            // ---------------------------------------------------------
            float formRating = CalculateFormRating();
            points += (formRating / 9.9) * 10.0;

            // ---------------------------------------------------------
            // 3. TIME UNTIL RETIREMENT (Max 20 points)
            // ---------------------------------------------------------
            int timeToRetire = GetTimeUntilRetirement(currentDate);
            if (timeToRetire > 48)
            {
                points += 20.0;
            }
            else if (timeToRetire > 0)
            {
                points += (timeToRetire / 48.0) * 20.0;
            }

            // ---------------------------------------------------------
            // 4. TIME IN GRADE / SENIORITY (Max 30 points)
            // ---------------------------------------------------------
            int timeInGrade = GetTimeInGrade(currentDate);
            double ambitionMultiplier = 1.0 + (ambition / 20.0);
            double tigScore = Math.Min((timeInGrade * ambitionMultiplier) / 48.0, 1.0) * 30.0;
            points += tigScore;
            
            // ---------------------------------------------------------
            // 5. MORALE PENALTY (Max -20 points)
            // ---------------------------------------------------------
            // Soldiers who aren't having fun don't seek more responsibility.
            if (Entity.Morale < 50)
            {
                double moralePenalty = ((50 - Entity.Morale) / 50.0) * 20.0;
                points -= moralePenalty;
            }

            // ---------------------------------------------------------
            // 6. BURNOUT PENALTY (Max -15 points)
            // ---------------------------------------------------------
            // Burned-out soldiers are focused on survival, not advancement.
            if (Entity.Burnout > 30)
            {
                double burnoutPenalty = ((Entity.Burnout - 30) / 70.0) * 15.0;
                points -= burnoutPenalty;
            }
            
            // ---------------------------------------------------------
            // 7. POWER CURVE SCALING
            // ---------------------------------------------------------
            // Cube the normalized score to widen the gap between strong
            // and weak candidates, then scale to target range.
            double normalized = Math.Clamp(points / 100.0, 0.0, 1.0);
            double curved = Math.Pow(normalized, 3.0) * 32.5;
            points = curved;
            
            // ---------------------------------------------------------
            // 7b. FREQUENCY SCALING
            // ---------------------------------------------------------
            // The power curve above is calibrated for monthly boards (12/year).
            // Scale down proportionally for less frequent boards.
            double frequencyScale = boardsPerYear / 12.0;
            points *= frequencyScale;
            
            // ---------------------------------------------------------
            // 8. FINAL ROLL
            // ---------------------------------------------------------
            double chances = Math.Clamp(points, 1.0, 99.0);
            int diceRoll = Random.Shared.Next(1, 101);
            return diceRoll <= chances;
        }
        
        /// <summary>
        /// Attempts to attend a promotion board for the soldier's target rank
        /// </summary>
        /// <param name="date">The current iteration date</param>
        /// <param name="db">The simulation database</param>
        /// <returns>True if successfully attended a board, false otherwise</returns>
        public bool AttemptPromotionBoard(IterationDate date, SimDatabase db)
        {
            // Check if soldier wants to attempt promotion
            var payGrade = RankCache.RanksById[Entity.TargetRankId.Value].PayGrade;
            if (!WillAttemptGradeIncrease(date, payGrade, 4)) // Assuming 4 boards per year
                return false;

            // Find the appropriate board using the position's promotion echelon
            var board = FindApplicablePromotionBoard();
            if (board == null)
                return false;

            // Check if already evaluated this iteration
            if (LatestBoardResult?.IterationId == date.Id)
                return false;

            // Evaluate the candidate
            board.EvaluateCandidate(this, date, db);
            return true;
        }

        /// <summary>
        /// Finds the promotion board applicable to this soldier based on their
        /// position's PromotionEchelon and target rank
        /// </summary>
        /// <returns>The applicable promotion board wrapper, or null if none found</returns>
        private PromotionBoardWrapper FindApplicablePromotionBoard()
        {
            if (Position == null || Position.ParentUnit == null)
                return null;

            // Get the echelon that handles boards for this position
            var boardEchelon = Position.BlueprintWrapper.PromotionPool; // This is the Echelon entity
            
            // Walk up the unit hierarchy to find the unit at the correct echelon level
            var currentUnit = Position.ParentUnit;
            while (currentUnit != null)
            {
                if (currentUnit.Echelon.HierarchyLevel >= boardEchelon.HierarchyLevel)
                {
                    // Found the right echelon level, now find the board
                    return Simulator.Current.FindPromotionBoard(Entity.TargetRankId.Value, Entity.OccupationId);
                }

                currentUnit = currentUnit.Parent;
            }

            // Fallback
            return null;
        }

        /// <summary>
        /// Indicates whether this soldier is a stand in for their current <see cref="Database.Position"/>.
        /// To be considered a stand in, the soldier must be a lower <see cref="Rank.PayGrade"/> than the
        /// <see cref="Rank.PayGrade"/>
        /// </summary>
        /// <returns></returns>
        public bool IsStandIn()
        {
            return (Rank.Classification.PayGrade < Position.BlueprintWrapper.Rank.Classification.PayGrade);
        }

        /// <summary>
        /// Determines whether this soldier is nearing his <see cref="PositionBlueprint.MaxTourLength"/>
        /// </summary>
        /// <param name="currentDate">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        public bool IsNearMaxTourLength(IterationDate currentDate)
        {
            if (Position == null) return true;

            if (Position.BlueprintWrapper.MaxTourLength > 0)
            {

                int timeLeft = Position.BlueprintWrapper.MaxTourLength - GetTimeInBillet(currentDate);
                int timeToRetire = (Entity.EntryIterationId + Entity.TargetTIS) - currentDate.Id;

                // If we are going to retire before we hit max tour length, just return false
                if (timeToRetire < timeLeft)
                    return false;

                // Get the median difference between max and min tour lengths
                double len = Position.BlueprintWrapper.MaxTourLength - Position.BlueprintWrapper.MinTourLength;
                int medium = (int)Math.Ceiling(len / 2);
                return (timeLeft <= medium);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether this soldier is at or past their Max Tour Length
        /// </summary>
        /// <param name="currentDate">the current simulation <see cref="IterationDate"/></param>
        /// <returns></returns>
        public bool IsPastMaxTourLength(IterationDate currentDate)
        {
            if (Position == null) return true;

            if (Position.BlueprintWrapper.MaxTourLength > 0)
            {

                int timeLeft = Position.BlueprintWrapper.MaxTourLength - GetTimeInBillet(currentDate);
                return (timeLeft < 1);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether this <see cref="Entity"/> is currently locked in by
        /// <see cref="PositionBlueprint.MinTourLength"/>
        /// </summary>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public bool IsLockedInPosition(IterationDate currentDate)
        {
            if (Position == null) return false;

            int timePassed = currentDate.Id - Assignment.EntryIterationId;
            return Position.BlueprintWrapper.MinTourLength > timePassed;
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
        public void GiveExperience(PositionBlueprintExperience item)
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
        /// Recalculates the morale of the soldier based on various factors such as form rating, time in grade, and attributes like ambition.
        /// Updates the soldier's morale property within the valid range of 0 to 100.
        /// </summary>
        /// <param name="currentDate">The current simulation date used to factor time-based morale adjustments.</param>
        private void RecalculateMorale(IterationDate currentDate)
        {
            double morale = 50.0; // baseline

            // === Factor 1: Form Rating (how well they fit their position) ===
            // FormRating is 1.0-9.9, baseline is 5.0
            morale += (CalculateFormRating() - 5.0) * 3.0; // Range: -12 to +14.7

            // === Factor 2: TIG stagnation (long TIG + high ambition = morale drop) ===
            int tig = GetTimeInGrade(currentDate);
            double ambitionFactor = AttributesWithModifiers[AttributeType.Ambition] / 20.0; // 0.0 to 1.0
            morale -= (tig / 12.0) * ambitionFactor * 8.0; // -8 per year at max ambition

            // === Factor 3: Leadership quality (THREAD-SAFE) ===
            var leader = Position?.SupervisorPosition?.Holder;
            if (leader != null && leader != this)
            {
                // Read LAST month's frozen slot — never being written to concurrently
                int lastSlot = (currentDate.Id - 1) % 12;
                if (lastSlot < 0) lastSlot += 12; // handle month 0 edge case
    
                float leaderFormRating = Volatile.Read(ref leader._formRatingHistory[lastSlot]) / 10f;
    
                // Innate personality attributes are static — safe to read directly
                double leaderLeadership = leader.AttributesWithModifiers.GetValueOrDefault(AttributeType.Leadership, 10);
                double leaderAgreeableness = leader.AttributesWithModifiers.GetValueOrDefault(AttributeType.Agreeableness, 10);

                double leaderQuality = (leaderFormRating * 0.5) + ((leaderLeadership + leaderAgreeableness) / 40.0 * 5.0);
                morale += (leaderQuality - 5.0) * 2.0;
            }

            // === Factor 4: Squad cohesion (THREAD-SAFE) ===
            var peers = Position?.ParentUnit?.GetAssignedSoldiers();
            if (peers != null)
            {
                int lastSlot = (currentDate.Id - 1) % 12;
                if (lastSlot < 0) lastSlot += 12;
    
                int peerCount = 0;
                double peerMoraleSum = 0;
                foreach (var peer in peers)
                {
                    if (peer == this) continue;
                    peerMoraleSum += Volatile.Read(ref peer._moraleHistory[lastSlot]);
                    peerCount++;
                }

                if (peerCount > 0)
                {
                    var avgPeerMorale = peerMoraleSum / peerCount;
                    // Extraversion determines how much peers affect you
                    var extraversionFactor = AttributesWithModifiers.GetValueOrDefault(AttributeType.Extraversion, 10) / 20.0;
                    morale += (avgPeerMorale - 50.0) / 50.0 * 5.0 * extraversionFactor; // Range: -5 to +5
                }
            }

            // === Factor 5: Recent board results ===
            if (LatestBoardResult != null)
            {
                int monthsAgo = currentDate.Id - LatestBoardResult.IterationId;
                if (monthsAgo < 6)
                {
                    double recency = 1.0 - (monthsAgo / 6.0);
                    if (LatestBoardResult.Passed)
                        morale += 10.0 * recency;
                    else
                        morale -= 15.0 * recency * ambitionFactor;
                }
            }

            // === Factor 6: Conscientiousness as a stabilizer ===
            // High conscientiousness soldiers are more resilient to morale swings
            double conscientiousness = AttributesWithModifiers.GetValueOrDefault(AttributeType.Conscientiousness, 10) / 20.0;
            // Pull morale toward 50 based on conscientiousness (dampening effect)
            morale = morale + (50.0 - morale) * conscientiousness * 0.15;

            Entity.Morale = Math.Clamp((int)morale, 0, 100);
        }

        /// <summary>
        /// Recalculates the burnout score for the soldier based on various factors
        /// such as time in service (TIS), morale, and other attributes.
        /// </summary>
        /// <param name="currentDate">The current simulation date used for calculations.</param>
        private void RecalculateBurnout(IterationDate currentDate)
        {
            double risk = 0;
            int tis = GetTimeInService(currentDate);

            // Factor: Proximity to TargetTIS (bell-curve intended lifespan)
            double tisRatio = (double)tis / Entity.TargetTIS;
            if (tisRatio > 0.8) risk += (tisRatio - 0.8) * 200; // Ramps up sharply

            // Factor: Low morale
            if (Entity.Morale < 40) risk += (40 - Entity.Morale) * 1.5;

            // Factor: Financial strain (subscription cost vs rank stipend)
            // Factor: Innate traits (e.g., "Content" trait reduces risk)

            Entity.Burnout = Math.Clamp((int)risk, 0, 100);
        }

        /// <summary>
        /// Evalutates the <see cref="AbstractFilter"/>, returning a reversed
        /// integer value of the result, which is used for ordering Groups
        /// </summary>
        /// <param name="selector">the <see cref="AbstractFilter"/> to evaluate</param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        /// <returns>0 if true, 1 if false</returns>
        public int EvaluateLookUpReverse(SelectionFilter selector, IterationDate date)
        {
            return (EvaluateFilter(selector, date)) ? 0 : 1;
        }
        
        public int EvaluateLookUpReverse(SelectionGroup selector, IterationDate date)
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
                case PositionFunction.PositionBlueprintId:
                    return Position.BlueprintWrapper.Id;
                case PositionFunction.PositionStature:
                    return Position.Position.Blueprint.Stature;
                case PositionFunction.IsNormalAssignment:
                    return (Position.BlueprintWrapper.Blueprint.Flag == PositionFlag.NormalAssignment) ? 1 : 0;
                case PositionFunction.IsCommandPosition:
                    return (Position.BlueprintWrapper.Blueprint.Flag == PositionFlag.CommandPosition) ? 1 : 0;
                case PositionFunction.IsSpecialAssignment:
                    return (Position.BlueprintWrapper.Blueprint.Flag == PositionFlag.SpecialAssignment) ? 1 : 0;
                case PositionFunction.IsStaffPosition:
                    return (Position.BlueprintWrapper.Blueprint.Flag == PositionFlag.StaffPosition) ? 1 : 0;
            }
        }

        /// <summary>
        /// Evaluates a filter against the current soldier using specific criteria based on the provided selector and date.
        /// </summary>
        /// <param name="filter">The filter containing the criteria to evaluate.</param>
        /// <param name="date">The iteration date used for date-specific evaluations.</param>
        /// <returns>True if the soldier meets the criteria specified in the filter; otherwise, false.</returns>
        public bool EvaluateFilter(SelectionFilter filter, IterationDate date)
            => EvaluateClause(filter.Selector, filter.SelectorId, filter.Operator, filter.RightValue, date);

        public bool EvaluateFilter(SelectionGroup group, IterationDate date)
            => EvaluateClause(group.Selector, group.SelectorId, group.Operator, group.RightValue, date);

        private bool EvaluateClause(ClauseLeftSelector selector, int selectorId, ComparisonOperator op, int rightValue, IterationDate date)
        {
            int leftValue = selector switch
            {
                ClauseLeftSelector.SoldierExperience => GetExperienceValue(selectorId),
                ClauseLeftSelector.SoldierPosition   => GetPositionValue((PositionFunction)selectorId, date),
                ClauseLeftSelector.SoldierValue      => GetSoldierValue((SoldierFunction)selectorId, date),
                _ => 0
            };
            return Condition.EvaluateExpression(leftValue, op, rightValue);
        }

        /// <summary>
        /// Saves the current soldier data to the database.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="date">the current simulation <see cref="IterationDate"/></param>
        public void Save(SimDatabase database, IterationDate date)
        {
            database.Soldiers.Update(Entity);

            // Save current position
            if (Position != null && !Entity.Retired)
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
                    Soldier = Entity,
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
                SoldierId = Entity.Id,
                PositionId = Position.Position.Id,
                EntryIterationId = Assignment.EntryIterationId,
                EntryRankId = Assignment.EntryRankId,
                ExitIterationId = date.Id,
                ExitRankId = Rank.Id,
                LastGradeChangeIterationId = LastGradeChangeDate.Id
            });
        }
        
        /// <summary>
        /// Gets the total XP required to level up from the current level to the next.
        /// Curve: Lvl 1->2 (100), Lvl 2->3 (400), Lvl 3->4 (900)...
        /// </summary>
        public static float GetRequiredXpForNextLevel(int currentLevel, float careerLengthMultiplier)
        {
            if (currentLevel >= MAX_SKILL_LEVEL) return 0f;
            
            float baseXp = currentLevel * currentLevel * 100f;
            return baseXp * careerLengthMultiplier;
        }

        /// <summary>
        /// Helper for the UI to easily draw the progress bar fill amount (0-100%).
        /// </summary>
        public static float GetSkillProgressPercentage(int currentLevel, float currentXp, float careerLengthMultiplier)
        {
            if (currentLevel >= MAX_SKILL_LEVEL) return 100f;
            
            float requiredXp = GetRequiredXpForNextLevel(currentLevel, careerLengthMultiplier);
            return (currentXp / requiredXp) * 100f;
        }

        /// <summary>
        /// Applies the monthly skill growth adjustments for the soldier.
        /// </summary>
        /// <param name="careerLengthMultiplier">A multiplier that adjusts the rate of skill growth based on the length of the soldier's career. Default is 1.0.</param>
        public void ApplyMonthlySkillGrowth(float careerLengthMultiplier = 1.0f)
        {
            // 1. Calculate BASE VOLUME
            float monthlyXpPool = 100f + (AttributesWithModifiers[AttributeType.Improvability] * 35f);

            // 2. THE INTELLECT CAPACITY CAP
            float sumInnate = AttributesWithModifiers[AttributeType.Agreeableness]
                + AttributesWithModifiers[AttributeType.Ambition]
                + AttributesWithModifiers[AttributeType.Conscientiousness]
                + AttributesWithModifiers[AttributeType.Extraversion]
                + AttributesWithModifiers[AttributeType.Mindfullness]
                + AttributesWithModifiers[AttributeType.Courage];

            float sumBasic = AttributesWithModifiers[AttributeType.Leadership]
                + AttributesWithModifiers[AttributeType.Marksmanship]
                + AttributesWithModifiers[AttributeType.Fitness]
                + AttributesWithModifiers[AttributeType.Teamwork]
                + AttributesWithModifiers[AttributeType.Composure]
                + AttributesWithModifiers[AttributeType.Discipline];

            float overallAverage = (sumInnate + sumBasic) / 12f;

            if (overallAverage > AttributesWithModifiers[AttributeType.Intellect])
            {
                float overage = overallAverage - AttributesWithModifiers[AttributeType.Intellect];
                float capacityPenalty = Math.Max(0.1f, 1.0f - (overage * 0.25f));
                monthlyXpPool *= capacityPenalty;
            }

            // 3. Calculate DISTRIBUTION WEIGHTS for Basic Skills
            float leadershipWeight = AttributesWithModifiers[AttributeType.Ambition]
                + (AttributesWithModifiers[AttributeType.Extraversion] * 0.5f);
            float marksmanshipWeight = AttributesWithModifiers[AttributeType.Mindfullness]
                + (AttributesWithModifiers[AttributeType.Intellect] * 0.5f);
            float fitnessWeight = AttributesWithModifiers[AttributeType.Courage]
                + (AttributesWithModifiers[AttributeType.Conscientiousness] * 0.5f);
            float teamworkWeight = AttributesWithModifiers[AttributeType.Agreeableness]
                + (AttributesWithModifiers[AttributeType.Extraversion] * 0.5f);
            float composureWeight = AttributesWithModifiers[AttributeType.Mindfullness]
                + AttributesWithModifiers[AttributeType.Courage];
            float disciplineWeight = AttributesWithModifiers[AttributeType.Conscientiousness] * 1.5f;

            float totalWeight = leadershipWeight + marksmanshipWeight + fitnessWeight
                + teamworkWeight + composureWeight + disciplineWeight;

            // 4. GET JOB PERFORMANCE MODEL
            var jobModel = Position?.BlueprintWrapper?.PerformanceModels ?? new Dictionary<AttributeType, int>();

            // 5. APPLY BASIC SKILL GROWTH
            GrowBasicSkill(AttributeType.Leadership, (leadershipWeight / totalWeight) * monthlyXpPool, jobModel, careerLengthMultiplier);
            GrowBasicSkill(AttributeType.Marksmanship, (marksmanshipWeight / totalWeight) * monthlyXpPool, jobModel, careerLengthMultiplier);
            GrowBasicSkill(AttributeType.Fitness, (fitnessWeight / totalWeight) * monthlyXpPool, jobModel, careerLengthMultiplier);
            GrowBasicSkill(AttributeType.Teamwork, (teamworkWeight / totalWeight) * monthlyXpPool, jobModel, careerLengthMultiplier);
            GrowBasicSkill(AttributeType.Composure, (composureWeight / totalWeight) * monthlyXpPool, jobModel, careerLengthMultiplier);
            GrowBasicSkill(AttributeType.Discipline, (disciplineWeight / totalWeight) * monthlyXpPool, jobModel, careerLengthMultiplier);

            // 6. APPLY INNATE SKILL GROWTH (Maturity Drift)
            GrowInnateSkill(AttributeType.Agreeableness, careerLengthMultiplier);
            GrowInnateSkill(AttributeType.Conscientiousness, careerLengthMultiplier);
            GrowInnateSkill(AttributeType.Extraversion, careerLengthMultiplier);
            GrowInnateSkill(AttributeType.Mindfullness, careerLengthMultiplier);
            GrowInnateSkill(AttributeType.Courage, careerLengthMultiplier);
        }

        /// <summary>
        /// Increases the basic skill level of the soldier for the specified attribute type based on the provided experience and career factors.
        /// </summary>
        /// <param name="type">The attribute type for which the skill level will be increased.</param>
        /// <param name="baseXp">The base experience points to be applied.</param>
        /// <param name="jobModel">A dictionary mapping attribute types to their expected levels in the job model.</param>
        /// <param name="careerLengthMultiplier">A multiplier representing the soldier's career length factor impacting skill growth.</param>
        private void GrowBasicSkill(AttributeType type, float baseXp, Dictionary<AttributeType, int> jobModel,
            float careerLengthMultiplier)
        {
            var attr = AttributesBase[type];
            if (attr.Value >= MAX_SKILL_LEVEL) return;

            float challengeModifier;

            if (jobModel.TryGetValue(type, out int expectedLevel))
            {
                // Skill IS in the performance model — the position actively trains this skill.
                // Base bonus just for being in the model: 1.0×
                // Additional boost when the model demands MORE than the soldier has (room to grow).
                // Diminishing returns when the soldier exceeds the model (already mastered it).
        
                float delta = expectedLevel - attr.Value;
        
                if (delta > 0)
                {
                    // Soldier is BELOW the position's expectation — strong growth pressure
                    // Range: 1.2× to 2.0× (capped)
                    challengeModifier = 1.2f + Math.Min(0.8f, delta * 0.15f);
                }
                else if (delta == 0)
                {
                    // Soldier MATCHES the position — maintenance-level boost
                    // Still better than not being in the model at all
                    challengeModifier = 1.0f;
                }
                else
                {
                    // Soldier EXCEEDS the position's expectation — diminishing returns
                    // Range: 0.3× to 0.9×
                    challengeModifier = Math.Max(0.3f, 1.0f - (Math.Abs(delta) * 0.15f));
                }
            }
            else
            {
                // Skill is NOT in the performance model — minimal passive gains only
                challengeModifier = 0.15f;
            }

            float finalXp = baseXp * challengeModifier;

            int skillLevel = attr.Value;
            float currentXp = attr.TotalExperience;
            ProcessLevelUp(ref skillLevel, ref currentXp, finalXp, careerLengthMultiplier);
            attr.Value = skillLevel;
            attr.TotalExperience = (int)currentXp;

            AttributesWithModifiers[type] = skillLevel;
        }

        /// <summary>
        /// Adjusts the innate skill level of a specific attribute type based on experience and career length multiplier.
        /// </summary>
        /// <param name="type">The type of attribute to modify.</param>
        /// <param name="careerLengthMultiplier">A multiplier based on the length of the soldier's career.</param>
        private void GrowInnateSkill(AttributeType type, float careerLengthMultiplier)
        {
            var attr = AttributesBase[type];
            if (attr.Value >= MAX_SKILL_LEVEL) return;

            // Improvability drives innate growth: 20 = ~1 lvl/5yr, 12 = ~1 lvl/10yr, 5 = basically never
            float improvability = AttributesWithModifiers[AttributeType.Improvability];
            float finalXp = attr.Value * attr.Value * 0.07f * improvability;

            int skillLevel = attr.Value;
            float currentXp = attr.TotalExperience;
            ProcessLevelUp(ref skillLevel, ref currentXp, finalXp, careerLengthMultiplier);
            attr.Value = skillLevel;
            attr.TotalExperience = (int)currentXp;

            AttributesWithModifiers[type] = skillLevel;
        }

        /// <summary>
        /// Processes the leveling up of a skill by updating the skill level and experience points based on the experience gained and a career length multiplier.
        /// </summary>
        /// <param name="skillLevel">A reference to the current skill level of the soldier.</param>
        /// <param name="currentXp">A reference to the current total experience points of the soldier in the skill.</param>
        /// <param name="xpToAdd">The amount of experience points to add to the current total.</param>
        /// <param name="careerLengthMultiplier">A multiplier that adjusts the required experience for leveling up based on the career length.</param>
        private void ProcessLevelUp(ref int skillLevel, ref float currentXp, float xpToAdd,
            float careerLengthMultiplier)
        {
            currentXp += xpToAdd;

            // Get the target required to beat the current level
            float xpRequired = GetRequiredXpForNextLevel(skillLevel, careerLengthMultiplier);

            // Using while loop for massive XP dumps (in case they gain 2 levels at once)
            while (skillLevel < MAX_SKILL_LEVEL && currentXp >= xpRequired)
            {
                skillLevel++;
                
                // Recalculate the requirement for the NEW level we just hit
                xpRequired = GetRequiredXpForNextLevel(skillLevel, careerLengthMultiplier);
            }

            // Hard clamp at 20
            if (skillLevel >= MAX_SKILL_LEVEL)
            {
                skillLevel = MAX_SKILL_LEVEL;
            }
        }

        public static bool operator ==(SoldierWrapper a, SoldierWrapper b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(SoldierWrapper a, SoldierWrapper b)
        {
            return !(a == b);
        }

        public bool Equals(SoldierWrapper other)
        {
            if (other == null || other.Entity == null) return false;
            return (Entity.Id == other.Entity.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SoldierWrapper);
        }

        public override int GetHashCode() => Entity?.Id.GetHashCode() ?? 0;

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
                PositionsHeld.Clear();
                PositionsHeld = null;

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

        /// <summary>
        /// Spawns a new instance of <see cref="SoldierWrapper"/> by creating a soldier entity with the specified attributes and adding it to the database.
        /// </summary>
        /// <param name="persona">The persona template defining the soldier's characteristics.</param>
        /// <param name="rank">The rank assigned to the soldier.</param>
        /// <param name="date">The iteration date marking the soldier's entry into service.</param>
        /// <param name="occupation">The occupation assigned to the soldier.</param>
        /// <param name="db">The simulation database where the soldier will be stored.</param>
        /// <returns>A newly created <see cref="SoldierWrapper"/> instance.</returns>
        public static SoldierWrapper Spawn(Persona persona, Rank rank, IterationDate date, Occupation occupation, SimDatabase db)
        {
            // Create a new soldier entity
            var soldier = db.Soldiers.Create();
            soldier.Rank = rank;
            soldier.Persona = persona;
            soldier.Occupation = occupation;
            soldier.EntryServiceDate = date;
            soldier.LastGradeChangeDate = date;
            soldier.LastPromotionDate = date;
            soldier.Morale = 100;
            soldier.Ego = Random.Shared.Next(1, 101);
            
            // Create target random but skewed TIS
            var careerLength = persona.CareerLength;
            soldier.TargetTIS = MathUtils.GenerateSkewedInt(
                careerLength.MinimumMonths,
                careerLength.AverageMonths,
                careerLength.MaximumMonths,
                careerLength.SkewLeft,
                careerLength.SkewRight
            );
            
            // Assign Sex
            soldier.IsMale = Random.Shared.Next(1, 101) >= persona.FemaleGenderRatio;
            
            // Generate Age. Age will increase on their anniversary Entry Date
            soldier.Age = MathUtils.GenerateSkewedInt(
                persona.MinAge,
                persona.AverageAge,
                persona.MaxAge,
                persona.SkewAgeLeft,
                persona.SkewAgeRight
            );
            
            // Assign Race
            var race = RandomNameGenerator.GetRandomRace();
            
            // Assign Name based on gender and race
            soldier.FirstName = RandomNameGenerator.GetFirstName(soldier.IsMale, race);
            soldier.LastName = RandomNameGenerator.GetLastName(race);
            soldier.Race = race;
            
            // Add Soldier
            db.Soldiers.Add(soldier);

            // Assign 0..MaxTraits random traits
            int traitCount = MathUtils.GenerateSkewedInt(
                persona.MinTraits,
                persona.AverageTraits,
                persona.MaxTraits,
                persona.SkewTraitsLeft,
                persona.SkewTraitsRight
            );

            // Assign traits
            if (traitCount > 0 && persona.Traits.Count > 0)
            {
                var pool = new ProbabilityGenerator<PersonaTrait>(persona.Traits);
                var lockedGroups = new HashSet<int>();

                for (int i = 0; i < traitCount && pool.ItemCount > 0; i++)
                {
                    var pick = pool.SpawnUnique();

                    // Check exclusion group conflict
                    int? group = pick.Trait.ExclusionGroup;
                    if (group.HasValue && !lockedGroups.Add(group.Value))
                    {
                        // Conflicting group — skip, don't re-add to pool
                        i--;
                        continue;
                    }

                    var attachment = db.SoldierTraitAttachments.Create();
                    attachment.SoldierId = soldier.Id;
                    attachment.TraitId = pick.TraitId;
                    attachment.AssignedOn = date;
                    attachment.Duration = pick.Duration;
                    db.SoldierTraitAttachments.Add(attachment);
                }
            }
            
            // Spawn Soldier Attributes. NOTE: The attributes in the database are base values
            // and never modified by traits or effects in the database.
            foreach (var attr in persona.Attributes)
            {
                var soldierAttr = db.SoldierAttributes.Create();
                soldierAttr.SoldierId = soldier.Id;
                soldierAttr.Attribute = attr.AttributeId;
                soldierAttr.Value = Random.Shared.Next(attr.MinSpawnValue, attr.MaxSpawnValue + 1);
                
                // Calculate cumulative XP to have reached this level, plus random progress toward next
                int level = soldierAttr.Value;
                int cumulativeXp = 0;
                for (int i = 1; i < level; i++)
                    cumulativeXp += i * i * 100;

                int xpForNextLevel = level * level * 100;
                int randomProgress = Random.Shared.Next(0, xpForNextLevel);
                
                soldierAttr.TotalExperience = cumulativeXp + randomProgress;
                db.SoldierAttributes.Add(soldierAttr);
            }
            
            var sw = new SoldierWrapper(soldier, date, db);
            
            // Assign Occupation
            sw.AssignOccupation(occupation, date, db, false);

            return sw;
        }
    }
}
