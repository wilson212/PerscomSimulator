using System;
using System.Collections.Generic;
using CrossLite.QueryBuilder;
using Perscom.Collections;
using Perscom.Database;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// High-performance, zero-allocation replacement for IEnumerableExtensions.
    /// All methods operate directly on DenseList to preserve struct enumerators
    /// and avoid IEnumerable boxing on the hot simulation path.
    /// </summary>
    public static class SoldierSelectionHelper
    {
        // ─── Filtering ──────────────────────────────────────────────────

        /// <summary>
        /// Applies all filters to a DenseList of soldiers, materializing after each AND filter.
        /// Returns a new DenseList containing only soldiers that pass all filters.
        /// </summary>
        public static DenseList<SoldierWrapper> FilterSoldiers(
            DenseList<SoldierWrapper> soldiers,
            List<SelectionFilter> filters,
            LogicOperator logicOperator,
            IterationDate date)
        {
            if (filters.Count == 0)
                return soldiers;

            if (logicOperator == LogicOperator.And)
            {
                var current = soldiers;
                for (int i = 0; i < filters.Count; i++)
                {
                    var filter = filters[i];
                    var filtered = new DenseList<SoldierWrapper>(current.Count);
                    for (int j = 0; j < current.Count; j++)
                    {
                        if (current[j].EvaluateFilter(filter, date))
                            filtered.Add(current[j]);
                    }

                    current = filtered;
                }

                return current;
            }
            else // OR
            {
                var seen = new HashSet<SoldierWrapper>();
                var result = new DenseList<SoldierWrapper>();
                for (int i = 0; i < filters.Count; i++)
                {
                    var filter = filters[i];
                    for (int j = 0; j < soldiers.Count; j++)
                    {
                        var s = soldiers[j];
                        if (s.EvaluateFilter(filter, date) && seen.Add(s))
                            result.Add(s);
                    }
                }

                return result;
            }
        }

        // ─── Grouping + Prime Selection ─────────────────────────────────

        /// <summary>
        /// Recursive multi-level grouping that returns the "prime" soldiers —
        /// the soldiers in the bucket with the lowest key at each level.
        /// Replaces GroupSoldiersBy + GetPrimeSoldiers in a single pass.
        /// </summary>
        public static DenseList<SoldierWrapper> GroupAndGetPrime(
            DenseList<SoldierWrapper> soldiers,
            List<SelectionGroup> groupings,
            IterationDate date)
        {
            if (groupings.Count == 0 || soldiers.Count == 0)
                return soldiers;

            return GroupAndGetPrimeRecursive(soldiers, groupings, 0, date);
        }

        private static DenseList<SoldierWrapper> GroupAndGetPrimeRecursive(
            DenseList<SoldierWrapper> soldiers,
            List<SelectionGroup> groupings,
            int groupIndex,
            IterationDate date)
        {
            if (soldiers.Count == 0)
                return soldiers;

            // Bucket by the current grouping level
            var selector = groupings[groupIndex];
            var buckets = soldiers.GroupByDictionary(
                s => s.EvaluateLookUpReverse(selector, date)
            );

            // Find the bucket with the lowest key (prime bucket)
            int minKey = int.MaxValue;
            DenseList<SoldierWrapper>? primeBucket = null;
            foreach (var kvp in buckets)
            {
                if (kvp.Value.Count > 0 && kvp.Key < minKey)
                {
                    minKey = kvp.Key;
                    primeBucket = kvp.Value;
                }
            }

            if (primeBucket == null || primeBucket.Count == 0)
                return new DenseList<SoldierWrapper>();

            // If there are more grouping levels, recurse into the prime bucket
            int nextIndex = groupIndex + 1;
            if (nextIndex < groupings.Count)
            {
                return GroupAndGetPrimeRecursive(primeBucket, groupings, nextIndex, date);
            }

            return primeBucket;
        }

        /// <summary>
        /// Groups by a custom key selector first, then applies the SelectionGroup groupings.
        /// Used for lateral procedures that prepend a "desire" grouping level.
        /// Replaces GroupSoldiersBy(Func, IEnumerable<SelectionGroup>, date) + GetPrimeSoldiers.
        /// </summary>
        public static DenseList<SoldierWrapper> GroupByThenGetPrime(
            DenseList<SoldierWrapper> soldiers,
            Func<SoldierWrapper, int> primaryKeySelector,
            List<SelectionGroup> groupings,
            IterationDate date)
        {
            if (soldiers.Count == 0)
                return soldiers;

            // First level: bucket by the primary key
            var buckets = soldiers.GroupByDictionary(primaryKeySelector);

            // Find the prime (lowest key) bucket
            int minKey = int.MaxValue;
            DenseList<SoldierWrapper>? primeBucket = null;
            foreach (var kvp in buckets)
            {
                if (kvp.Value.Count > 0 && kvp.Key < minKey)
                {
                    minKey = kvp.Key;
                    primeBucket = kvp.Value;
                }
            }

            if (primeBucket == null || primeBucket.Count == 0)
                return new DenseList<SoldierWrapper>();

            // Then apply the remaining SelectionGroup groupings
            if (groupings.Count > 0)
            {
                return GroupAndGetPrimeRecursive(primeBucket, groupings, 0, date);
            }

            return primeBucket;
        }

        /// <summary>
        /// Groups by a single key selector and returns the prime bucket.
        /// Used when there are no SelectionGroup groupings defined.
        /// Replaces GroupSoldiersBy(Func) + GetPrimeSoldiers.
        /// </summary>
        public static DenseList<SoldierWrapper> GroupByAndGetPrime(
            DenseList<SoldierWrapper> soldiers,
            Func<SoldierWrapper, int> keySelector)
        {
            if (soldiers.Count == 0)
                return soldiers;

            var buckets = soldiers.GroupByDictionary(keySelector);

            int minKey = int.MaxValue;
            DenseList<SoldierWrapper>? primeBucket = null;
            foreach (var kvp in buckets)
            {
                if (kvp.Value.Count > 0 && kvp.Key < minKey)
                {
                    minKey = kvp.Key;
                    primeBucket = kvp.Value;
                }
            }

            return primeBucket ?? new DenseList<SoldierWrapper>();
        }

        // ─── Sorting ────────────────────────────────────────────────────

        /// <summary>
        /// Sorts a DenseList of soldiers in-place using the composite SelectionSorting chain.
        /// Replaces OrderSoldiersBy + ThenOrderSoldiersBy with a single Array.Sort pass.
        /// </summary>
        public static void SortSoldiers(
            DenseList<SoldierWrapper> soldiers,
            List<SelectionSorting> sorting,
            IterationDate date)
        {
            if (soldiers.Count <= 1 || sorting.Count == 0)
                return;

            // Build a composite comparison that evaluates all sorting levels in priority order
            soldiers.Sort((a, b) =>
            {
                for (int i = 0; i < sorting.Count; i++)
                {
                    var rule = sorting[i];
                    int valA = a.GetValue(rule.Selector, rule.SelectorId, date);
                    int valB = b.GetValue(rule.Selector, rule.SelectorId, date);

                    int cmp = valA.CompareTo(valB);
                    if (cmp != 0)
                    {
                        return rule.Direction == Sorting.Descending ? -cmp : cmp;
                    }
                }

                return 0;
            });
        }

        /// <summary>
        /// Sorts a DenseList of soldiers in-place by a single value in descending order.
        /// Used as the default sort when no SelectionSorting rules are defined.
        /// </summary>
        public static void SortSoldiersDescending(
            DenseList<SoldierWrapper> soldiers,
            Func<SoldierWrapper, double> selector)
        {
            if (soldiers.Count <= 1)
                return;

            soldiers.Sort((a, b) => selector(b).CompareTo(selector(a)));
        }
    }
}