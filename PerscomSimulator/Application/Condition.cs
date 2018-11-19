using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom
{
    public static class Condition
    {
        public static bool EvaluateExpression(int value1, ConditionOperator @operator, int value2)
        {
            switch (@operator)
            {
                default:
                case ConditionOperator.Equals:
                    return (value1 == value2);
                case ConditionOperator.GreaterThan:
                    return (value1 > value2);
                case ConditionOperator.GreaterThanOrEqualTo:
                    return (value1 >= value2);
                case ConditionOperator.LessThan:
                    return (value1 < value2);
                case ConditionOperator.LessThanOrEqualTo:
                    return (value1 <= value2);
                case ConditionOperator.NotEqualTo:
                    return (value1 != value2);
            }
        }
    }
}
