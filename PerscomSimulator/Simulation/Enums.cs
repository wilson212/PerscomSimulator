namespace Perscom
{
    public enum SoldierSorting
    {
        TimeInService,
        TimeInGrade,
        TimeInPosition,
        TimeToRetirement
    }

    public enum SoldierFilter
    {
        TimeInService,
        TimeInGrade,
        TimeInPosition,
        TimeToRetirement
    }

    public enum ConditionOperator
    {
        Equals,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }

    public enum SortMode
    {
        OrderByValue,
        GroupByValue,
    }
}
