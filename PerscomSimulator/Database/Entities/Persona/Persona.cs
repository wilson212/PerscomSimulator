using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// A Persona is a blueprint for a soldier that can be used to generate
    /// </summary>
    [Table]
    public class Persona : EntityBase, IProbable
    {
        #region Columns

        /// <summary>
        /// The IsUnique Persona ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this Persona
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the chances of this template spawning from
        /// a <see cref="ProbabilityGenerator{T}"/>
        /// </summary>
        [Column, Required]
        public virtual int Probability { get; set; }

        /// <summary>
        /// Gets or sets the chances of this template spawning as
        /// a Male or Female soldier
        /// </summary>
        /// <remarks>0 equates to 100% male, 100 equates to 100% female</remarks>
        [Column, Required]
        public virtual int FemaleGenderRatio { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CareerLength.Id"/> this entity
        /// uses to spawn a TIS target
        /// </summary>
        [Column, Required]
        public virtual int CareerLengthId { get; set; }
        
        /// <summary>
        /// Gets or sets the minimum age (Years) a soldier will be when spawned
        /// </summary>
        [Column, Required]
        public virtual int MinAge { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum age (Years) a soldier will be when spawned
        /// </summary>
        [Column, Required]
        public virtual int MaxAge { get; set; } = 0;
        
        /// <summary>
        /// Gets or sets the average age (Years) a soldier will be when spawned
        /// </summary>
        [Column, Required]
        public virtual int AverageAge { get; set; } = 0;
        
        /// <summary>
        /// Gets or sets the skew right of this career length.
        /// </summary>
        [Column, Required]
        public virtual double SkewAgeRight { get; set; }
        
        /// <summary>
        /// Gets or sets the skew left of this career length.
        /// </summary>
        [Column, Required]
        public virtual double SkewAgeLeft { get; set; }

        /// <summary>
        /// The minimum number of traits that a Persona can possess.
        /// </summary>
        [Column, Required]
        public virtual int MinTraits { get; set; }

        /// <summary>
        /// The maximum number of traits that a Persona can possess.
        /// </summary>
        [Column, Required]
        public virtual int MaxTraits { get; set; }

        /// <summary>
        /// The average number of traits that a Persona can possess.
        /// </summary>
        [Column, Required]
        public virtual int AverageTraits { get; set; }

        /// <summary>
        /// Represents the left skewness factor for the distribution of traits assigned to a Persona.
        /// This property indicates the extent to which the traits distribution is skewed toward the left side of the scale,
        /// influencing the likelihood of traits being assigned at the lower end of their range.
        /// </summary>
        [Column, Required]
        public virtual double SkewTraitsLeft { get; set; }

        /// <summary>
        /// Represents the skewness factor applied to the probability distribution
        /// for the right-hand portion of traits associated with a Persona.
        /// </summary>
        [Column, Required]
        public virtual double SkewTraitsRight { get; set; }

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets the <see cref="Database.CareerLength"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(CareerLengthId))]
        [References(nameof(Database.CareerLength.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade )]
        public virtual CareerLength CareerLength { get; set; }

        #endregion

        #region Child Navigation Properties
        
        /// <summary>
        /// Gets a list of <see cref="PersonaAttribute"/> entities that reference this Persona
        /// </summary>
        public virtual EntitySet<PersonaAttribute> Attributes { get; set; }
        
        /// <summary>
        /// Gets a list of <see cref="PersonaTrait"/> entities that reference this Persona
        /// </summary>
        public virtual EntitySet<PersonaTrait> Traits { get; set; }

        #endregion
    }
}
