using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation
{
    public interface ISpawnable : ICloneable
    {
        int OneInThousandProbability { get; }
    }
}
