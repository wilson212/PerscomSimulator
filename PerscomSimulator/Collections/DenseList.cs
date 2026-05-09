#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Perscom.Collections
{
    /// <summary>
    /// A high-performance dense array list with cache-friendly iteration, O(1) swap-and-pop removal,
    /// zero-allocation LINQ shadows, parallel iteration, and Span access.
    /// Works like a <see cref="List{T}"/> but optimized for high-churn simulation workloads. Does NOT
    /// keep item order once an item is removed.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <author>Steven Wilson</author>
    public sealed class DenseList<T> : ICollection<T>
    {
        private T[] _items;
        private int _count;
        private int _version;

        private static readonly Dictionary<nint, IComparer<T>> _comparerCache = new();

        /// <summary>
        /// Gets the number of elements currently stored in the collection.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Provides direct access to the underlying storage array (including unused buffer slots).
        /// </summary>
        public T[] RawArray => _items;

        /// <summary>
        /// Gets a Span over the active (dense) portion of the array.
        /// </summary>
        public Span<T> AsSpan() => _items.AsSpan(0, _count);

        /// <summary>
        /// Specifies the minimum number of elements by which the collection's capacity
        /// should increase when resizing is required.
        /// </summary>
        public int MinimumResizeCapacity
        {
            get => _minimumResizeCapacity;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), @"MinimumResizeCapacity must be greater than 0.");
                _minimumResizeCapacity = value;
            }
        }

        private int _minimumResizeCapacity = 256;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)_count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if ((uint)index >= (uint)_count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _items[index] = value;
                _version++;
            }
        }

        /// <summary>
        /// Creates a new DenseList with the specified initial capacity.
        /// </summary>
        public DenseList(int initialCapacity = 0)
        {
            _items = initialCapacity == 0 ? Array.Empty<T>() : new T[initialCapacity];
            _count = 0;
            _version = 0;
        }

        /// <summary>
        /// Adds an item to the end of the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (_count >= _items.Length) Grow();
            _items[_count] = item;
            _count++;
            _version++;
        }

        /// <summary>
        /// Adds all elements from the specified collection to the end of the list.
        /// Optimized with fast-paths for <see cref="DenseList{T}"/>, <see cref="ICollection{T}"/>,
        /// and a fallback for general <see cref="IEnumerable{T}"/>.
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            // Fast path 1: Another DenseList<T>
            if (items is DenseList<T> other)
            {
                int addCount = other.Count;
                if (addCount == 0) return;
                EnsureCapacity(_count + addCount);
                
                // Access the DenseList as a Span to copy memory directly
                other.AsSpan().CopyTo(_items.AsSpan(_count));
                _count += addCount;
                _version++;
                return;
            }

            // Fast path 2: Standard .NET List<T>
            if (items is List<T> list)
            {
                int addCount = list.Count;
                if (addCount == 0) return;
                EnsureCapacity(_count + addCount);
                
                // Access the list as a Span to copy memory directly
                CollectionsMarshal.AsSpan(list).CopyTo(_items.AsSpan(_count));
                _count += addCount;
                _version++;
                return;
            }

            // Fast path 3: Any collection with a known count (Arrays, HashSets, etc.)
            if (items is ICollection<T> collection)
            {
                int addCount = collection.Count;
                if (addCount == 0) return;
                EnsureCapacity(_count + addCount);
                collection.CopyTo(_items, _count);
                _count += addCount;
                _version++;
                return;
            }

            // Fallback: Generic IEnumerable (e.g., LINQ results)
            // We try to "peek" the count to pre-allocate memory once
            if (Enumerable.TryGetNonEnumeratedCount(items, out int count))
            {
                EnsureCapacity(_count + count);
            }

            foreach (T item in items)
            {
                if (_count >= _items.Length) Grow();
                _items[_count++] = item;
            }
            _version++;
        }

        /// <summary>
        /// Expands the internal storage capacity of the list to accommodate additional items.
        /// </summary>
        /// <remarks>
        /// This method is triggered when the internal array's capacity is exceeded during operations like adding elements.
        /// It increases the array size by a growth factor, ensuring sufficient space for new elements while minimizing allocations.
        /// The growth factor is determined by either the minimum resize capacity or a fraction of the current array size, whichever is larger.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow()
        {
            int currentLength = _items.Length;
            int growth = currentLength == 0 ? _minimumResizeCapacity : Math.Max(_minimumResizeCapacity, currentLength / 4);
            int newSize = currentLength + growth;
            Array.Resize(ref _items, newSize);
        }

        /// <summary>
        /// Removes the element at the specified index using swap-and-pop. O(1).
        /// WARNING: This breaks sort order.
        /// </summary>
        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_count)
                throw new ArgumentOutOfRangeException(nameof(index));

            int lastIndex = _count - 1;

            if (index < lastIndex)
            {
                _items[index] = _items[lastIndex];
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items[lastIndex] = default!;
            }

            _count--;
            _version++;
        }

        /// <summary>
        /// Removes the first occurrence of the specified item using swap-and-pop. O(N) scan, O(1) removal.
        /// WARNING: This breaks sort order.
        /// </summary>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes all elements matching the predicate using a reverse swap-and-pop pass. O(N).
        /// WARNING: This breaks sort order.
        /// </summary>
        public int RemoveAll(Predicate<T> match)
        {
            int removedCount = 0;

            for (int i = _count - 1; i >= 0; i--)
            {
                if (match(_items[i]))
                {
                    int lastIndex = _count - 1;

                    if (i < lastIndex)
                    {
                        _items[i] = _items[lastIndex];
                    }

                    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                    {
                        _items[lastIndex] = default!;
                    }

                    _count--;
                    removedCount++;
                }
            }

            if (removedCount > 0) _version++;
            return removedCount;
        }

        /// <summary>
        /// Removes and returns the item at the END of the dense array. O(1).
        /// Does NOT trigger a swap, preserving sorted order.
        /// </summary>
        public bool TryPop(out T value)
        {
            if (_count == 0)
            {
                value = default!;
                return false;
            }

            int lastIndex = _count - 1;
            value = _items[lastIndex];

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items[lastIndex] = default!;
            }

            _count--;
            _version++;
            return true;
        }

        /// <summary>
        /// Efficiently drains the collection from end-to-start (LIFO), yielding items one by one.
        /// The collection is immediately cleared for reuse.
        /// </summary>
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// Consume directly in a foreach loop using 'var'. Do NOT cast to IEnumerable<T>.
        /// </remarks>
        public DrainEnumerable Drain()
        {
            int drainCount = _count;
            _count = 0;
            _version++;
            return new DrainEnumerable(this, drainCount);
        }

        /// <summary>
        /// Returns the index of the first occurrence of the item, or -1 if not found.
        /// </summary>
        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < _count; i++)
            {
                if (comparer.Equals(_items[i], item)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Determines whether the collection contains the specified item.
        /// </summary>
        public bool Contains(T item) => IndexOf(item) >= 0;

        /// <summary>
        /// Sorts the collection in-place.
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            if (_count <= 1) return;
            Array.Sort(_items, 0, _count, comparer);
            _version++;
        }

        /// <summary>
        /// Sorts the collection using a Comparison delegate (e.g. lambdas).
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            nint key = comparison.Method.MethodHandle.GetFunctionPointer();
            if (!_comparerCache.TryGetValue(key, out var comparer))
            {
                comparer = Comparer<T>.Create(comparison);
                _comparerCache[key] = comparer;
            }
            Sort(comparer);
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements.
        /// </summary>
        public void TrimExcess()
        {
            if (_count < _items.Length)
            {
                Array.Resize(ref _items, _count);
            }
        }

        /// <summary>
        /// Ensures the internal storage has at least the specified capacity.
        /// </summary>
        public void EnsureCapacity(int capacity)
        {
            if (capacity > _items.Length)
            {
                Array.Resize(ref _items, capacity);
            }
        }

        /// <summary>
        /// Copies elements to the specified array.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (_count > 0)
            {
                // Only clear the array if it actually contains references
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    Array.Clear(_items, 0, _count);
                }
                
                _count = 0;
                _version++;
            }
        }

        /// <summary>
        /// Executes a parallel for loop over the dense part of the collection.
        /// </summary>
        public void ParallelEach(Action<T> body)
        {
            T[] items = _items;
            int count = _count;
            var capturedState = (items, body);

            Parallel.For(0, count, () => capturedState, static (i, _, s) =>
                {
                    s.body(s.items[i]);
                    return s;
                },
                static _ => { }
            );
        }

        /// <summary>
        /// Executes a parallel for loop with a custom state to avoid closure allocations.
        /// </summary>
        public void ParallelEach<TState>(TState state, Action<T, TState> body)
        {
            T[] items = _items;
            int count = _count;
            var capturedState = (items, body, state);

            Parallel.For(0, count, () => capturedState, static (i, _, s) =>
                {
                    s.body(s.items[i], s.state);
                    return s;
                },
                static _ => { }
            );
        }

        // ─── Zero-Allocation LINQ Shadows ───────────────────────────────
        
        /// <summary>
        /// Projects each element of the current DenseList to a new form using the specified selector function.
        /// </summary>
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// Consume directly in a foreach loop using 'var'. Do NOT cast to <see cref="IEnumerable{TResult}"/>.
        /// </remarks>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TResult">The type of the value returned by the selector function and the resulting enumerable.</typeparam>
        /// <returns>An enumerable containing the results of applying the selector function to each element of the DenseList.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="selector"/> is null.</exception>
        public SelectEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return new SelectEnumerable<TResult>(this, selector);
        }
        
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// Consume directly in a foreach loop using 'var'. Do NOT cast to IEnumerable<T>.
        /// </remarks>
        public WhereEnumerable Where(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereEnumerable(this, predicate);
        }

        /// <summary>
        /// Determines whether the DenseList contains any elements.
        /// </summary>
        /// <returns>True if the DenseList contains one or more elements; otherwise, false.</returns>
        public bool Any() => _count > 0;

        /// <summary>
        /// Determines whether at least one element in the collection matches the specified condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if any elements in the collection satisfy the condition; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="predicate"/> is null.</exception>
        public bool Any(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            ReadOnlySpan<T> span = _items.AsSpan(0, _count);
            for (int i = 0; i < span.Length; i++)
            {
                if (predicate(span[i])) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the first element in the collection that satisfies the given predicate, or the default value if no such element exists.
        /// </summary>
        /// <param name="predicate">A function that defines the condition the element must satisfy.</param>
        /// <returns>The first element that satisfies the specified condition, or the default value if no element is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="predicate"/> is null.</exception>
        public T? FirstOrDefault(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            ReadOnlySpan<T> span = _items.AsSpan(0, _count);
            for (int i = 0; i < span.Length; i++)
            {
                if (predicate(span[i])) return span[i];
            }
            return default;
        }

        /// <summary>
        /// Counts the number of elements in the collection that satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The number of elements that match the condition defined by the predicate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the predicate is null.</exception>
        public int CountWhere(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int count = 0;
            ReadOnlySpan<T> span = _items.AsSpan(0, _count);
            for (int i = 0; i < span.Length; i++)
            {
                if (predicate(span[i])) count++;
            }
            return count;
        }
        /// <summary>
        /// Calculates the sum of the values obtained by applying the specified selector function
        /// to each element in the collection.
        /// </summary>
        /// <param name="selector">A function that projects each element of the collection into an integer value to be summed.</param>
        /// <returns>The sum of the projected integer values.</returns>
        public int Sum(Func<T, int> selector)
        {
            int sum = 0;

            // By creating a Span once, the JIT can often optimize away bounds checks
            ReadOnlySpan<T> span = _items.AsSpan(0, _count); 
            for (int i = 0; i < span.Length; i++)
            {
                // This is significantly faster than _items[i]
                sum += selector(span[i]); 
            }
            return sum;
        }

        /// <summary>
        /// Calculates the sum of the elements in the collection based on a specified projection.
        /// </summary>
        /// <param name="selector">A function to project each element of the collection into a double value.</param>
        /// <returns>The sum of the projected values.</returns>
        public double Sum(Func<T, double> selector)
        {
            double sum = 0;

            // By creating a Span once, the JIT can often optimize away bounds checks
            ReadOnlySpan<T> span = _items.AsSpan(0, _count); 
            for (int i = 0; i < span.Length; i++)
            {
                // This is significantly faster than _items[i]
                sum += selector(span[i]); 
            }
            return sum;
        }

        /// <summary>
        /// Computes the average of the values in the collection that are obtained
        /// by invoking the specified selector function.
        /// </summary>
        /// <param name="selector">A function to extract a double value from each element.</param>
        /// <returns>The average of the selected values.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
        public double Average(Func<T, double> selector)
        {
            if (_count == 0) throw new InvalidOperationException("Sequence contains no elements.");
            return Sum(selector) / _count;
        }

        /// <summary>
        /// Returns the element with the minimum value as determined by the specified selector function.
        /// </summary>
        /// <param name="selector">A function to extract the numeric value to evaluate for each element.</param>
        /// <returns>The element with the minimum value, or <c>null</c> if the collection is empty.</returns>
        public T? MinBy(Func<T, double> selector)
        {
            if (_count == 0) return default;

            ReadOnlySpan<T> span = _items.AsSpan(0, _count);
            T min = span[0];
            double minVal = selector(min);
    
            for (int i = 1; i < span.Length; i++)
            {
                double val = selector(span[i]);
                if (val < minVal) 
                { 
                    min = span[i]; 
                    minVal = val; 
                }
            }
            return min;
        }

        /// <summary>
        /// Finds the maximum element in the collection based on a specified criterion.
        /// </summary>
        /// <param name="selector">A function that maps each element to a comparable value for comparison.</param>
        /// <returns>The element with the maximum value based on the specified criterion, or default if the collection is empty.</returns>
        public T? MaxBy(Func<T, double> selector)
        {
            // Check if the list is empty before proceeding
            if (_count == 0) return default;
    
            // Create the span once to trigger JIT optimization
            ReadOnlySpan<T> span = _items.AsSpan(0, _count);
    
            T max = span[0];
            double maxVal = selector(max);
    
            // Starting the loop at index 1
            for (int i = 1; i < span.Length; i++)
            {
                double val = selector(span[i]);
                if (val > maxVal) 
                { 
                    max = span[i]; 
                    maxVal = val; 
                }
            }
    
            return max;
        }

        /// <summary>
        /// Determines whether all elements in the collection satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// True if all elements in the collection satisfy the predicate or the collection is empty;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="predicate"/> argument is null.
        /// </exception>
        public bool All(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            ReadOnlySpan<T> span = _items.AsSpan(0, _count);
            for (int i = 0; i < span.Length; i++)
            {
                if (!predicate(span[i])) return false;
            }
            return true;
        }
        
        /// <summary>
        /// Groups elements into a Dictionary where each bucket is a DenseList<T>.
        /// High-performance eager execution using .NET 8 memory referencing.
        /// </summary>
        public Dictionary<TGroupKey, DenseList<T>> GroupByDictionary<TGroupKey>(Func<T, TGroupKey> keySelector) 
            where TGroupKey : notnull
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            // Heuristic: start with a capacity related to the parent list size
            var dict = new Dictionary<TGroupKey, DenseList<T>>(_count / 8); 
    
            for (int i = 0; i < _count; i++)
            {
                T item = _items[i];
                TGroupKey key = keySelector(item);

                // .NET 8 Optimization: Single hash lookup to get a ref to the bucket
                ref DenseList<T>? bucketRef = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out bool exists);
        
                if (!exists)
                {
                    // Initialize with a small capacity to save memory on many small groups
                    bucketRef = new DenseList<T>(8); 
                }
        
                bucketRef!.Add(item);
            }

            return dict;
        }

        /// <summary>
        /// Creates a new list containing all the elements in the dense list.
        /// </summary>
        /// <returns>A new <see cref="List{T}"/> containing the elements of the dense list.</returns>
        public List<T> ToList()
        {
            var list = new List<T>(_count);
            for (int i = 0; i < _count; i++) list.Add(_items[i]);
            return list;
        }

        /// <summary>
        /// Creates and returns a new array containing all the elements of the DenseList.
        /// </summary>
        /// <returns>An array of type T containing the elements of the DenseList, in the same order as they appear in the DenseList.</returns>
        public T[] ToArray()
        {
            var array = new T[_count];
            Array.Copy(_items, array, _count);
            return array;
        }

        // ─── Enumerators ────────────────────────────────────────────────

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator(DenseList<T> list) : IEnumerator<T>
        {
            private int _index = -1;
            private readonly int _version = list._version;

            public T Current => list._items[_index];
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                int next = _index + 1;
                if (next < list._count)
                {
                    _index = next;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                _index = -1;
            }

            public void Dispose() { }
        }

        // ─── Where ─────────────────────────────────────────────────────

        public readonly struct WhereEnumerable(DenseList<T> list, Func<T, bool> predicate) : IEnumerable<T>
        {
            public WhereEnumerator GetEnumerator() => new WhereEnumerator(list, predicate);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public WhereSelectEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
            {
                if (selector == null) throw new ArgumentNullException(nameof(selector));
                return new WhereSelectEnumerable<TResult>(list, predicate, selector);
            }

            public T? FirstOrDefault()
            {
                foreach (var item in this) return item;
                return default;
            }

            public int Count()
            {
                int count = 0;
                foreach (var item in this) count++;
                return count;
            }

            public bool Any()
            {
                foreach (var item in this) return true;
                return false;
            }
            
            /// <summary>
            /// Materializes the filtered results into a new DenseList.
            /// Required when the result needs to be grouped or sorted downstream.
            /// </summary>
            public DenseList<T> ToDenseList()
            {
                var result = new DenseList<T>();
                using var enumerator = GetEnumerator(); // struct, no boxing
                while (enumerator.MoveNext())
                {
                    result.Add(enumerator.Current);
                }
                return result;
            }
        }

        public struct WhereEnumerator(DenseList<T> list, Func<T, bool> predicate) : IEnumerator<T>
        {
            private int _index = -1;
            private readonly int _version = list._version;

            public T Current => list._items[_index];
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                while (++_index < list._count)
                {
                    if (predicate(list._items[_index])) return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                _index = -1;
            }

            public void Dispose() { }
        }

        // ─── Where + Select (fused) ────────────────────────────────────

        public readonly struct WhereSelectEnumerable<TResult>(
            DenseList<T> list,
            Func<T, bool> predicate,
            Func<T, TResult> selector) : IEnumerable<TResult>
        {
            public WhereSelectEnumerator<TResult> GetEnumerator() => new WhereSelectEnumerator<TResult>(list, predicate, selector);
            IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public TResult? FirstOrDefault()
            {
                var enumerator = GetEnumerator();
                return enumerator.MoveNext() ? enumerator.Current : default;
            }

            public TResult? FirstOrDefault(Func<TResult, bool> predicate)
            {
                foreach (var item in this)
                    if (predicate(item)) return item;
                return default;
            }

            public bool Any()
            {
                var enumerator = GetEnumerator();
                return enumerator.MoveNext();
            }

            public int Count()
            {
                int count = 0;
                foreach (var item in this) count++;
                return count;
            }
        }

        public struct WhereSelectEnumerator<TResult>(
            DenseList<T> list,
            Func<T, bool> predicate,
            Func<T, TResult> selector) : IEnumerator<TResult>
        {
            private int _index = -1;
            private readonly int _version = list._version;

            public TResult Current => selector(list._items[_index]);
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                while (++_index < list._count)
                {
                    if (predicate(list._items[_index])) return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                _index = -1;
            }

            public void Dispose() { }
        }

        // ─── Select ────────────────────────────────────────────────────

        public readonly struct SelectEnumerable<TResult>(DenseList<T> list, Func<T, TResult> selector) : IEnumerable<TResult>
        {
            public SelectEnumerator<TResult> GetEnumerator() => new SelectEnumerator<TResult>(list, selector);
            IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public TResult? FirstOrDefault()
            {
                var enumerator = GetEnumerator();
                return enumerator.MoveNext() ? enumerator.Current : default;
            }

            public TResult? FirstOrDefault(Func<TResult, bool> predicate)
            {
                foreach (var item in this)
                    if (predicate(item)) return item;
                return default;
            }

            public bool Any() => list._count > 0;
            public int Count() => list._count;

            /// <summary>
            /// Converts the collection to a List containing the projected elements
            /// based on the specified selector function.
            /// </summary>
            /// <returns>A List containing the projected elements.</returns>
            public List<TResult> ToList()
            {
                var result = new List<TResult>(list._count);
                for (int i = 0; i < list._count; i++)
                {
                    result.Add(selector(list._items[i]));
                }
                return result;
            }
        }

        public struct SelectEnumerator<TResult>(DenseList<T> list, Func<T, TResult> selector) : IEnumerator<TResult>
        {
            private int _index = -1;
            private readonly int _version = list._version;

            public TResult Current => selector(list._items[_index]);
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                int next = _index + 1;
                if (next < list._count)
                {
                    _index = next;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                _index = -1;
            }

            public void Dispose() { }
        }

        // ─── Drain ─────────────────────────────────────────────────────

        public readonly struct DrainEnumerable(DenseList<T> list, int drainCount) : IEnumerable<T>
        {
            public DrainEnumerator GetEnumerator() => new DrainEnumerator(list, drainCount);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct DrainEnumerator(DenseList<T> list, int drainCount) : IEnumerator<T>
        {
            private int _index = drainCount;
            private readonly int _version = list._version;
            private T _current = default!;

            public T Current => _current;
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                if (_index > 0)
                {
                    _index--;
                    _current = list._items[_index];

                    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                    {
                        list._items[_index] = default!;
                    }
                    return true;
                }
                return false;
            }

            public void Reset() => throw new NotSupportedException("Drain enumerator cannot be reset.");
            public void Dispose() { }
        }
    }
}