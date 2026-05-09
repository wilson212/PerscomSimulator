using Perscom.Database;
using System;
using System.Collections.Generic;

namespace Perscom.Application
{
    public class FormRatingCalculator
    {
        // --- Main Calculation Method ---
        public float CalculateFormRating(Soldier soldier, PositionBlueprint position)
        {
            // STEP 1: Calculate the base rating based on the position's "Performance Model".
            // This single calculation handles all user-weighted attributes, skills, experience, etc.
            float baseFormRating = CalculateBaseRating(soldier, position);

            // STEP 2: Get the hidden, dynamic modifiers.
            float settlingInModifier = GetSettlingInModifier(soldier);
            float moraleModifier = soldier.Morale / 100.0f; // Assumes Morale is a 0-100 scale.

            // STEP 3: Apply the modifiers to the base rating.
            float finalFormRating = baseFormRating * settlingInModifier * moraleModifier;

            // STEP 4: Ensure the final rating is never below the minimum.
            const float MIN_RATING = 1.0f;
            return (float)Math.Round(Math.Max(MIN_RATING, finalFormRating), 1);
        }

        // --- Helper for Step 1 ---
        private float CalculateBaseRating(Soldier soldier, PositionBlueprint position)
        {
            float soldierRawScore = 0;
            float expectedScore = 0;

            /** This loop processes every factor the user has added to the position's Performance Model.
            foreach (var factor in position.PerformanceModel)
            {
                string attributeName = factor.Key;
                int expectedLevel = factor.Value;

                // This switch statement gets the correct value from the soldier object.
                int soldierValue = GetSoldierValueForAttribute(soldier, attributeName);

                soldierRawScore += soldierValue * expectedLevel;
                expectedScore += expectedLevel * expectedLevel;
            }
            */

            if (expectedScore == 0) return 5.0f; // Neutral rating for a position with no requirements.

            float performanceRatio = soldierRawScore / expectedScore;

            // Cap the ratio at 110% to give a small bonus for being overqualified.
            const float maxRatio = 1.1f;
            performanceRatio = Math.Min(performanceRatio, maxRatio);

            // Scale the result to the 1.0 - 9.9 range.
            const float MIN_RATING = 1.0f;
            const float MAX_RATING = 9.9f;
            return MIN_RATING + ((performanceRatio / maxRatio) * (MAX_RATING - MIN_RATING));
        }

        // --- Helper for Step 2 ---
        private float GetSettlingInModifier(Soldier soldier)
        {
            // This calculates the temporary effect of adaptability when a soldier is new to a job.

            // 1. Convert the 0-20 Adaptability score to a 0.75x to 1.25x modifier.
            // An average score of 10 results in a neutral 1.0x modifier.
            const float MAX_ATTRIBUTE_VALUE = 20.0f;
            float baseModifier = 0.75f + (soldier.Adaptability / MAX_ATTRIBUTE_VALUE) * 0.5f;

            // 2. Determine how "settled in" the soldier is (0.0 to 1.0).
            // Assumes it takes 24 months to fully settle in.
            const float MONTHS_TO_SETTLE = 24.0f;
            float settlingInFactor = Math.Min(1.0f, soldier.TimeInPositionInMonths / MONTHS_TO_SETTLE);

            // 3. Fade the modifier back to neutral (1.0x) as the soldier settles in.
            float finalModifier = baseModifier + ((1.0f - baseModifier) * settlingInFactor);

            return finalModifier;
        }

        // --- Data Retrieval Helper ---
        private int GetSoldierValueForAttribute(Soldier soldier, string attributeName)
        {
            // This method retrieves the correct value from the soldier object.
            // It needs access to the soldier's attributes, TIS, TIG, etc.
            switch (attributeName)
            {
                // Skills & Personality
                case "Leadership": return soldier.Attributes.Leadership;
                case "Courage": return soldier.Attributes.Courage;
                // ... add all other skills and personality traits here

                // Innate/Static
                case "Intelligence": return soldier.Intelligence;
                case "Adaptability": return soldier.Adaptability;

                // Experience
                case "Time in Service": return soldier.TimeInServiceInYears;
                case "Time in Grade": return soldier.TimeInGradeInYears;

                default: return 0;
            }
        }
    }

    // You would need placeholder classes for Soldier and PositionBlueprint like so:
    public class Soldier
    {
        public SoldierAttributes Attributes { get; set; } = new SoldierAttributes();
        public int Intelligence { get; set; }
        public int Adaptability { get; set; }
        public int TimeInServiceInYears { get; set; }
        public int TimeInGradeInYears { get; set; }
        public int TimeInPositionInMonths { get; set; }
        public float Morale { get; set; }
    }

    public class SoldierAttributes
    {
        public int Leadership { get; set; }
        public int Courage { get; set; }
        // ... other attributes
    }
}
