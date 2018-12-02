namespace Perscom
{
    public static class Condition
    {
        public static bool EvaluateExpression(int value1, ComparisonOperator @operator, int value2)
        {
            switch (@operator)
            {
                default:
                case ComparisonOperator.Equals:
                    return (value1 == value2);
                case ComparisonOperator.GreaterThan:
                    return (value1 > value2);
                case ComparisonOperator.GreaterThanOrEqualTo:
                    return (value1 >= value2);
                case ComparisonOperator.LessThan:
                    return (value1 < value2);
                case ComparisonOperator.LessThanOrEqualTo:
                    return (value1 <= value2);
                case ComparisonOperator.NotEqualTo:
                    return (value1 != value2);
            }
        }
    }
}
