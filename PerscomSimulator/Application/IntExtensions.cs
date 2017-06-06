using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom
{
    public static class IntExtensions
    {
        public static uint RotateLeft(this uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}
