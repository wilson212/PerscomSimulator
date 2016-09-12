using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation
{
    public class RetirementInfo : PromotionInfo
    {
        public int TotalRank { get; set; } = 0;

        public decimal AverageRank
        {
            get
            {
                return Math.Round(TotalRank / (decimal)TotalPersonel, 2);
            }
        }
    }
}
