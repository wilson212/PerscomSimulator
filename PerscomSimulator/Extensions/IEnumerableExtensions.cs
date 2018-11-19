using CrossLite.QueryBuilder;
using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Perscom
{
    /// <summary>
    /// My IEnumerable Extensions, mostly to filter, Group, and Order SoldierWrappers
    /// </summary>
    /// <remarks>Credits to Mitsu Furuta</remarks>
    /// <seealso cref="https://blogs.msdn.microsoft.com/mitsu/2007/12/21/playing-with-linq-grouping-groupbymany/"/>
    public static class IEnumerableExtensions
    {
        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements,
            params Func<TElement, object>[] groupSelectors)
        {

            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();

                //reduce the list recursively until zero
                var nextSelectors = groupSelectors.Skip(1).ToArray();
                return 
                    elements.GroupBy(selector).Select(
                        g => new GroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g,
                            SubGroups = g.GroupByMany(nextSelectors)
                        });
            }

            return null;
        }

        public static IEnumerable<SoldierGroupResult> GroupSoldiersBy(
            this IEnumerable<SoldierWrapper> elements, 
            List<BilletExperienceGroup> groupings)
        {
            if (groupings.Count > 0)
            {
                var selector = groupings.First();

                //reduce the list recursively until zero
                var nextSelectors = groupings.Skip(1).ToList();
                return
                    elements.GroupBy(x => x.EvaluateLookUpReverse(selector)).Select(
                        g => new SoldierGroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Soldiers = g,
                            SubGroups = g.GroupSoldiersBy(nextSelectors)
                        });
            }

            return null;
        }

        public static IEnumerable<SoldierGroupResult> GroupSoldiersBy(
            this IEnumerable<SoldierWrapper> elements,
            Func<SoldierWrapper, int> selector,
            List<BilletExperienceGroup> groupings)
        {
            if (groupings.Count() > 0)
            {
                //reduce the list recursively until zero
                var nextSelectors = groupings.ToList();
                return
                    elements.GroupBy(selector).Select(
                        g => new SoldierGroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Soldiers = g,
                            SubGroups = g.GroupSoldiersBy(nextSelectors)
                        });
            }

            return null;
        }

        public static IEnumerable<SoldierGroupResult> GroupSoldiersBy(
            this IEnumerable<SoldierWrapper> elements,
            Func<SoldierWrapper, int> selector)
        {
            return
                elements.GroupBy(selector).Select(
                    g => new SoldierGroupResult
                    {
                        Key = g.Key,
                        Count = g.Count(),
                        Soldiers = g,
                        SubGroups = null
                    });
        }

        public static IEnumerable<SoldierWrapper> GetPrimeSoldiers(
            this IEnumerable<SoldierGroupResult> elements)
        {
            foreach (var element in elements.OrderBy(x => x.Key))
            {
                // Do we even have any elements?
                if (element.Count > 0)
                {
                    if (element.SubGroups != null && element.SubGroups.Count() > 0)
                    {
                        return element.SubGroups.GetPrimeSoldiers();
                    }
                    else
                    {
                        return element.Soldiers;
                    }
                }
            }

            return new List<SoldierWrapper>();
        }

        public static IOrderedEnumerable<SoldierWrapper> OrderSoldiersBy(
            this IEnumerable<SoldierWrapper> elements,
            List<BilletExperienceSorting> sorting)
        {
            if (sorting.Count() > 0)
            {
                var selector = sorting.First();

                // reduce the list recursively until zero
                var nextSelectors = sorting.Skip(1);

                // Do initial sorting
                var oList = (selector.Direction == Sorting.Ascending)
                   ? elements.OrderBy(x => x.GetExperienceValue(selector.ExperienceId))
                   : elements.OrderByDescending(x => x.GetExperienceValue(selector.ExperienceId));

                // Apply additional sorting
                foreach (var sort in nextSelectors)
                {
                    oList = oList.ThenOrderSoldiersBy(sort);
                }

                return oList;
            }

            return elements.OrderBy(x => x);
        }

        public static IOrderedEnumerable<SoldierWrapper> OrderSoldiersBy(
            this IEnumerable<SoldierWrapper> elements,
            List<SoldierPoolSorting> sorting,
            IterationDate date)
        {
            if (sorting.Count() > 0)
            {
                var selector = sorting.First();

                // reduce the list recursively until zero
                var nextSelectors = sorting.Skip(1);

                // Do initial sorting
                IOrderedEnumerable<SoldierWrapper> oList = null;
                switch(selector.SortBy)
                {
                    default:
                    case SoldierSorting.TimeInGrade:
                        oList = (selector.Direction == Sorting.Ascending)
                            ? elements.OrderBy(x => x.GetTimeInGrade(date))
                            : elements.OrderByDescending(x => x.GetTimeInGrade(date));
                        break;
                    case SoldierSorting.TimeInService:
                        oList = (selector.Direction == Sorting.Ascending)
                            ? elements.OrderBy(x => x.GetTimeInService(date))
                            : elements.OrderByDescending(x => x.GetTimeInService(date));
                        break;
                    case SoldierSorting.TimeInPosition:
                        oList = (selector.Direction == Sorting.Ascending)
                            ? elements.OrderBy(x => x.GetTimeInBillet(date))
                            : elements.OrderByDescending(x => x.GetTimeInBillet(date));
                        break;
                    case SoldierSorting.TimeToRetirement:
                        oList = (selector.Direction == Sorting.Ascending)
                            ? elements.OrderBy(x => x.GetTimeUntilRetirement(date))
                            : elements.OrderByDescending(x => x.GetTimeUntilRetirement(date));
                        break;
                }

                // Apply additional sorting
                foreach (var sort in nextSelectors)
                {
                    oList = oList.ThenOrderSoldiersBy(sort, date);
                }

                return oList;
            }

            return elements.OrderBy(x => x);
        }

        public static IOrderedEnumerable<SoldierWrapper> ThenOrderSoldiersBy(
            this IOrderedEnumerable<SoldierWrapper> elements,
            List<BilletExperienceSorting> selectors)
        {
            var soldiers = elements;
            foreach (var ordering in selectors)
            {
                soldiers = soldiers.ThenOrderSoldiersBy(ordering);
            }

            return soldiers;
        }

        public static IOrderedEnumerable<SoldierWrapper> ThenOrderSoldiersBy(
            this IOrderedEnumerable<SoldierWrapper> elements,
            BilletExperienceSorting selector)
        {
            return (selector.Direction == Sorting.Ascending)
                    ? elements.ThenBy(x => x.GetExperienceValue(selector.ExperienceId))
                    : elements.ThenByDescending(x => x.GetExperienceValue(selector.ExperienceId));
        }

        /// <summary>
        /// This method is used to sort soldiers using the given SoldierSorting and direction
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<SoldierWrapper> ThenOrderSoldiersBy(
            this IOrderedEnumerable<SoldierWrapper> list,
            SoldierPoolSorting selector,
            IterationDate date)
        {
            switch (selector.SortBy)
            {
                default:
                    return list;
                case SoldierSorting.TimeInGrade:
                    return (selector.Direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeInGrade(date))
                        : list.ThenByDescending(x => x.GetTimeInGrade(date));
                case SoldierSorting.TimeInService:
                    return (selector.Direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeInService(date))
                        : list.ThenByDescending(x => x.GetTimeInService(date));
                case SoldierSorting.TimeInPosition:
                    return (selector.Direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeInBillet(date))
                        : list.ThenByDescending(x => x.GetTimeInBillet(date));
                case SoldierSorting.TimeToRetirement:
                    return (selector.Direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeUntilRetirement(date))
                        : list.ThenByDescending(x => x.GetTimeUntilRetirement(date));
            }
        }

        public static IOrderedEnumerable<SoldierWrapper> ThenOrderSoldiersBy(
            this IOrderedEnumerable<SoldierWrapper> elements,
            List<SoldierPoolSorting> selectors,
            IterationDate date)
        {
            var soldiers = elements;
            foreach (var ordering in selectors)
            {
                soldiers = soldiers.ThenOrderSoldiersBy(ordering, date);
            }

            return soldiers;
        }

        /// <summary>
        /// This method is used to filter soldiers using the given SoldierPoolFilters
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="logicOperator"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IEnumerable<SoldierWrapper> FilterSoldierList(
            this IEnumerable<SoldierWrapper> list,
            List<BilletExperienceFilter> filters,
            LogicOperator logicOperator)
        {
            IEnumerable<SoldierWrapper> soldiers = list;
            if (logicOperator == LogicOperator.And)
            {
                foreach (var filter in filters)
                {
                    soldiers = soldiers.FilterSoldierList(filter).ToList();
                }
            }
            else
            {
                HashSet<SoldierWrapper> people = new HashSet<SoldierWrapper>();
                foreach (var filter in filters)
                {
                    people.UnionWith(people.FilterSoldierList(filter));
                }

                soldiers = people.ToList();
            }

            return soldiers;
        }

        /// <summary>
        /// This method is used to filter soldiers using the given SoldierPoolFilter, operator and value
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filter"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IEnumerable<SoldierWrapper> FilterSoldierList(
            this IEnumerable<SoldierWrapper> list,
            BilletExperienceFilter filter)
        {
            return list.Where(x => x.MeetsExperienceRequirement(filter));
        }

        /// <summary>
        /// This method is used to filter soldiers using the given SoldierPoolFilters
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="logicOperator"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IEnumerable<SoldierWrapper> FilterSoldierList(
            this IEnumerable<SoldierWrapper> list,
            List<SoldierPoolFilter> filters,
            LogicOperator logicOperator,
            IterationDate date)
        {
            IEnumerable<SoldierWrapper> soldiers = list;
            if (logicOperator == LogicOperator.And)
            {
                foreach (var filter in filters)
                {
                    soldiers = soldiers.FilterSoldierList(filter, date).ToList();
                }
            }
            else
            {
                HashSet<SoldierWrapper> people = new HashSet<SoldierWrapper>();
                foreach (var filter in filters)
                {
                    people.UnionWith(people.FilterSoldierList(filter, date));
                }

                soldiers = people.ToList();
            }

            return soldiers;
        }

        /// <summary>
        /// This method is used to filter soldiers using the given SoldierPoolFilter, operator and value
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filter"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IEnumerable<SoldierWrapper> FilterSoldierList(
            this IEnumerable<SoldierWrapper> list,
            SoldierPoolFilter filter,
            IterationDate date)
        {
            switch (filter.FilterBy)
            {
                default:
                case SoldierFilter.TimeInPosition:
                    return list.Where(
                        x => Condition.EvaluateExpression(x.GetTimeInBillet(date), filter.Operator, filter.Value)
                    );
                case SoldierFilter.TimeInGrade:
                    return list.Where(
                        x => Condition.EvaluateExpression(x.GetTimeInGrade(date), filter.Operator, filter.Value)
                    );
                case SoldierFilter.TimeInService:
                    return list.Where(
                        x => Condition.EvaluateExpression(x.GetTimeInService(date), filter.Operator, filter.Value)
                    );
                case SoldierFilter.TimeToRetirement:
                    return list.Where(
                        x => Condition.EvaluateExpression(x.GetTimeUntilRetirement(date), filter.Operator, filter.Value)
                    );
            }
        }
    }

    public class GroupResult
    {
        public object Key
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public IEnumerable Items
        {
            get;
            set;
        }

        public IEnumerable<GroupResult> SubGroups
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Key, Count);
        }
    }

    public class SoldierGroupResult
    {
        public int Key
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public IEnumerable<SoldierWrapper> Soldiers
        {
            get;
            set;
        }

        public IEnumerable<SoldierGroupResult> SubGroups
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Key, Count);
        }
    }
}