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
            IEnumerable<AbstractFilter> groupings,
            IterationDate date)
        {
            if (groupings.Count() > 0)
            {
                var selector = groupings.First();

                //reduce the list recursively until zero
                var nextSelectors = groupings.Skip(1).ToList();
                return
                    elements.GroupBy(x => x.EvaluateLookUpReverse(selector, date)).Select(
                        g => new SoldierGroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Soldiers = g,
                            SubGroups = g.GroupSoldiersBy(nextSelectors, date)
                        });
            }

            return null;
        }

        public static IEnumerable<SoldierGroupResult> GroupSoldiersBy(
            this IEnumerable<SoldierWrapper> elements,
            Func<SoldierWrapper, int> selector,
            IEnumerable<BilletSelectionGroup> groupings,
            IterationDate date)
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
                            SubGroups = g.GroupSoldiersBy(nextSelectors, date)
                        });
            }

            return GroupSoldiersBy(elements, selector);
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
            IEnumerable<AbstractSort> sorting,
            IterationDate date)
        {
            if (sorting.Count() > 0)
            {
                var selector = sorting.First();

                // reduce the list recursively until zero
                var nextSelectors = sorting.Skip(1);

                // Do initial sorting
                var oList = (selector.Direction == Sorting.Ascending)
                   ? elements.OrderBy(x => x.GetValue(selector.Selector, selector.SelectorId, date))
                   : elements.OrderByDescending(x => x.GetValue(selector.Selector, selector.SelectorId, date));

                return oList.ThenOrderSoldiersBy(nextSelectors, date);
            }

            return elements.OrderBy(x => x);
        }

        public static IOrderedEnumerable<SoldierWrapper> ThenOrderSoldiersBy(
            this IOrderedEnumerable<SoldierWrapper> elements,
            IEnumerable<AbstractSort> selectors,
            IterationDate date)
        {
            var soldiers = elements;
            foreach (var ordering in selectors)
            {
                soldiers = soldiers.ThenOrderSoldiersBy(ordering, date);
            }

            return soldiers;
        }

        public static IOrderedEnumerable<SoldierWrapper> ThenOrderSoldiersBy(
            this IOrderedEnumerable<SoldierWrapper> elements,
            AbstractSort selector,
            IterationDate date)
        {
            return (selector.Direction == Sorting.Ascending)
                    ? elements.ThenBy(x => x.GetValue(selector.Selector, selector.SelectorId, date))
                    : elements.ThenByDescending(x => x.GetValue(selector.Selector, selector.SelectorId, date));
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
            IEnumerable<AbstractFilter> filters,
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
            AbstractFilter filter,
            IterationDate date)
        {
            return list.Where(x => x.EvaluateFilter(filter, date));
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