using System;
using Perscom.Simulation;

namespace Perscom
{
    public class Promotion
    {
        public DateTime Date { get; set; }

        public Rank FromRank { get; set; }

        public Rank ToRank { get; set; }

        public int PreviousTimeInGrade { get; set; }

        public int TimeInService { get; set; }

        public override string ToString()
        {
            return Date.ToShortDateString();
        }
    }
}