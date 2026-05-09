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
    /// A high-performance dense array collection where <typeparamref name="T"/> self-identifies its key
    /// via <see cref="IKeyed{TKey}"/>. Combines the simple <c>Add(item)</c> API of a List with
    /// O(1) key-based lookup and swap-and-pop removal of a KeyedList.
    /// </summary>
    /// <typeparam name="TKey">The unique identifier type (usually int or Guid).</typeparam>
    /// <typeparam name="T">The element type, which must implement <see cref="IKeyed{TKey}"/>.</typeparam>
    /// <author>Steven Wilson</author>
    public sealed class IdentityList<TKey, T> : ICollection<T>
        where TKey : notnull
        where T : IKeyed<TKey>
    {
        private T[] _items;
        private readonly Dictionary<TKey, int> _keyToIndex;
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
        /// Gets the value associated with the specified key.
        /// </summary>
        public T this[TKey key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, key);
                if (!Unsafe.IsNullRef(ref indexRef)) return _items[indexRef];
                throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public T AtIndex(int index)
        {
            if ((uint)index >= (uint)_count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _items[index];
        }

        /// <summary>
        /// Creates a new IndexedList with the specified initial capacity.
        /// </summary>
        public IdentityList(int initialCapacity = 0)
        {
            _items = initialCapacity == 0 ? Array.Empty<T>() : new T[initialCapacity];
            _keyToIndex = new Dictionary<TKey, int>(initialCapacity);
            _count = 0;
            _version = 0;
        }

        /// <summary>
        /// Adds an item to the collection. The key is extracted automatically via <see cref="IKeyed{TKey}.Key"/>.
        /// Throws if the key already exists.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            TKey key = item.Key;

            ref int indexRef = ref CollectionsMarshal.GetValueRefOrAddDefault(_keyToIndex, key, out bool exists);

            if (exists)
                throw new ArgumentException($"An item with the same key has already been added: '{key}'");

            if (_count >= _items.Length) Grow();

            int newIndex = _count;
            _items[newIndex] = item;
            indexRef = newIndex;
            _count++;
            _version++;
        }

        /// <summary>
        /// Adds the item if its key does not exist; updates (replaces) the item if it does.
        /// </summary>
        public void AddOrUpdate(T item)
        {
            TKey key = item.Key;

            ref int indexRef = ref CollectionsMarshal.GetValueRefOrAddDefault(_keyToIndex, key, out bool exists);

            if (exists)
            {
                _items[indexRef] = item;
                _version++;
                return;
            }

            if (_count >= _items.Length) Grow();

            int newIndex = _count;
            _items[newIndex] = item;
            indexRef = newIndex;
            _count++;
            _version++;
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// Each item in the provided sequence will be individually added, ensuring key uniqueness.
        /// </summary>
        /// <param name="items">The enumerable collection of items to be added to the list.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="items"/> parameter is null.
        /// </exception>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            // Pre-allocate memory once to avoid multiple Grow() calls
            if (Enumerable.TryGetNonEnumeratedCount(items, out int count))
            {
                EnsureCapacity(_count + count);
            }

            foreach (var item in items)
            {
                Add(item); // Still call Add to ensure key uniqueness
            }
        }

        /// <summary>
        /// Increases the capacity of the backing array to accommodate additional elements.
        /// </summary>
        /// <remarks>
        /// This method dynamically resizes the internal array to ensure sufficient space for new items,
        /// using a growth strategy based on the current size and a configurable minimum resize capacity.
        /// The associated dictionary used for key-to-index mapping also adjusts its capacity to match the updated size.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow()
        {
            int currentLength = _items.Length;
            int growth = currentLength == 0 ? _minimumResizeCapacity : Math.Max(_minimumResizeCapacity, currentLength / 4);
            int newSize = currentLength + growth;
            Array.Resize(ref _items, newSize);
            _keyToIndex.EnsureCapacity(newSize);
        }

        /// <summary>
        /// Removes an item by key using swap-and-pop. O(1).
        /// WARNING: This breaks sort order.
        /// </summary>
        public bool Remove(TKey key)
        {
            if (!_keyToIndex.Remove(key, out int indexToRemove))
                return false;

            int lastIndex = _count - 1;

            if (indexToRemove < lastIndex)
            {
                T lastItem = _items[lastIndex];
                _items[indexToRemove] = lastItem;

                ref int movedRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, lastItem.Key);
                movedRef = indexToRemove;
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items[lastIndex] = default!;
            }

            _count--;
            _version++;
            return true;
        }

        /// <summary>
        /// Removes an item directly. Extracts the key via the interface. O(1).
        /// </summary>
        public bool Remove(T item) => Remove(item.Key);

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
                    TKey keyToRemove = _items[i].Key;
                    int lastIndex = _count - 1;

                    if (i < lastIndex)
                    {
                        T lastItem = _items[lastIndex];
                        _items[i] = lastItem;

                        ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, lastItem.Key);
                        if (!Unsafe.IsNullRef(ref indexRef))
                        {
                            indexRef = i;
                        }
                    }

                    _keyToIndex.Remove(keyToRemove);

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
            _keyToIndex.Remove(value.Key);

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
        /// Consume directly in a foreach loop using 'var'. Do NOT cast to IEnumerable&lt;T&gt;.
        /// </remarks>
        public DrainEnumerable Drain()
        {
            int drainCount = _count;
            _count = 0;
            _keyToIndex.Clear();
            _version++;
            return new DrainEnumerable(this, drainCount);
        }

        /// <summary>
        /// O(1) key-based lookup.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(TKey key)
        {
            ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, key);
            if (!Unsafe.IsNullRef(ref indexRef)) return _items[indexRef];
            throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out T value)
        {
            ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, key);
            if (!Unsafe.IsNullRef(ref indexRef))
            {
                value = _items[indexRef];
                return true;
            }
            value = default!;
            return false;
        }

        /// <summary>
        /// Returns the value for the key, or a default if not found.
        /// </summary>
        public T GetOrDefault(TKey key, T defaultValue)
        {
            ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, key);
            if (!Unsafe.IsNullRef(ref indexRef)) return _items[indexRef];
            return defaultValue;
        }

        /// <summary>
        /// O(1) key existence check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key) => _keyToIndex.ContainsKey(key);

        /// <summary>
        /// Sorts the collection in-place and rebuilds the O(1) index map.
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            if (_count <= 1) return;
            Array.Sort(_items, 0, _count, comparer);

            // Rebuild index map from the items' own keys — no parallel key array needed.
            for (int i = 0; i < _count; i++)
            {
                ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, _items[i].Key);
                indexRef = i;
            }
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
                _keyToIndex.TrimExcess();
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
                _keyToIndex.EnsureCapacity(capacity);
            }
        }

        /// <summary>
        /// Determines whether the specified item exists in the collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.</param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public bool Contains(T item) => _keyToIndex.ContainsKey(item.Key);

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
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    Array.Clear(_items, 0, _count);
                }
                
                _keyToIndex.Clear();
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
        
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// Consume directly in a foreach loop using 'var'. Do NOT cast to IEnumerable&lt;TResult&gt;.
        /// </remarks>
        public SelectEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return new SelectEnumerable<TResult>(this, selector);
        }
        
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// Consume directly in a foreach loop using 'var'. Do NOT cast to IEnumerable&lt;T&gt;.
        /// </remarks>
        public WhereEnumerable Where(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereEnumerable(this, predicate);
        }

        /// <summary>
        /// Determines whether the collection contains any items.
        /// </summary>
        /// <returns>
        /// true if the collection contains one or more items; otherwise, false.
        /// </returns>
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
        /// Groups the elements of the collection by a specified key selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the key for grouping.</typeparam>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>
        /// A <see cref="KeyedList{TGroupKey, List{T}}"/> containing grouped elements, where the key is of
        /// type <typeparamref name="TGroupKey"/> and the value is a list of elements of type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="keySelector"/> is null.</exception>
        public KeyedList<TGroupKey, List<T>> GroupBy<TGroupKey>(Func<T, TGroupKey> keySelector)
            where TGroupKey : notnull
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            var result = new KeyedList<TGroupKey, List<T>>();
            for (int i = 0; i < _count; i++)
            {
                T item = _items[i];
                TGroupKey gk = keySelector(item);
                if (!result.TryGetValue(gk, out var list))
                {
                    list = new List<T>();
                    result.Add(gk, list);
                }
                list.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Groups the elements of the list into a dictionary based on a specified key selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the key returned by the key selector function.</typeparam>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>A dictionary where each key is associated with a list of elements that share the same key.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="keySelector"/> is null.</exception>
        public Dictionary<TGroupKey, List<T>> GroupByDictionary<TGroupKey>(Func<T, TGroupKey> keySelector)
            where TGroupKey : notnull
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            var dict = new Dictionary<TGroupKey, List<T>>();
            for (int i = 0; i < _count; i++)
            {
                T item = _items[i];
                TGroupKey gk = keySelector(item);
                ref List<T>? listRef = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, gk, out bool exists);
                if (!exists) listRef = new List<T>();
                listRef!.Add(item);
            }
            return dict;
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
        /// Converts the collection to a new list containing all elements in the collection.
        /// </summary>
        /// <returns>A List containing all elements of the collection.</returns>
        public List<T> ToList()
        {
            var list = new List<T>(_count);
            for (int i = 0; i < _count; i++) list.Add(_items[i]);
            return list;
        }

        /// <summary>
        /// Creates and returns a new array containing all elements currently in the collection.
        /// </summary>
        /// <returns>An array of type T containing all elements in the collection.</returns>
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

        public struct Enumerator(IdentityList<TKey, T> list) : IEnumerator<T>
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
                if (next < list._count) { _index = next; return true; }
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

        public readonly struct WhereEnumerable(IdentityList<TKey, T> list, Func<T, bool> predicate) : IEnumerable<T>
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
        }

        public struct WhereEnumerator(IdentityList<TKey, T> list, Func<T, bool> predicate) : IEnumerator<T>
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
            IdentityList<TKey, T> list,
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
            IdentityList<TKey, T> list,
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

        public readonly struct SelectEnumerable<TResult>(IdentityList<TKey, T> list, Func<T, TResult> selector) : IEnumerable<TResult>
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
        }

        public struct SelectEnumerator<TResult>(IdentityList<TKey, T> list, Func<T, TResult> selector) : IEnumerator<TResult>
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
                if (next < list._count) { _index = next; return true; }
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

        public readonly struct DrainEnumerable(IdentityList<TKey, T> list, int drainCount) : IEnumerable<T>
        {
            public DrainEnumerator GetEnumerator() => new DrainEnumerator(list, drainCount);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct DrainEnumerator(IdentityList<TKey, T> list, int drainCount) : IEnumerator<T>
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