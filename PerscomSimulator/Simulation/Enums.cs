namespace Perscom
{
    public enum SoldierSorting
    {
        TimeInService,
        TimeInGrade,
        TimeInBillet,
        TimeToRetirement
    }

    public enum SoldierFilter
    {
        TimeInService,
        TimeInGrade,
        TimeInBillet,
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
        HasValue,
        ByValue
    }
}
