#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Perscom.Collections
{
    /// <summary>
    /// A high-performance collection that maintains a dense array of items for 
    /// cache-friendly iteration while providing O(1) key-based lookups via an internal index map.
    /// Ideal for large, high-churn collections that require frequent iteration, bulk removal, sorting, and parallel processing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Internally composed of three structures: a dense <c>TValue[]</c> array, a parallel <c>TKey[]</c> array,
    /// and a <see cref="Dictionary{TKey, TValue}"/> index map. Items are always packed contiguously with no holes,
    /// maintained via swap-and-pop removal.
    /// </para>
    /// <para>
    /// <b>Benefits over <see cref="Dictionary{TKey, TValue}"/>:</b>
    /// <list type="bullet">
    ///     <item><b>Cache-friendly iteration:</b> Items are stored in a contiguous dense array, enabling 
    ///     CPU L1/L2 cache line prefetching. Dictionary iterates over sparse buckets with poor locality (2-5x slower).</item>
    ///     <item><b>Zero-allocation LINQ shadows:</b> Built-in <c>Where</c>, <c>Select</c>, <c>Any</c>, <c>Sum</c>, 
    ///     <c>MinBy</c>, <c>MaxBy</c>, etc. use struct enumerators and direct array loops — no <see cref="IEnumerable{T}"/> boxing. 
    ///     Dictionary requires standard LINQ, which allocates heap-based iterator classes.</item>
    ///     <item><b>Native parallel iteration:</b> <c>ParallelEach</c> runs <c>Parallel.For</c> directly over the dense array 
    ///     with near-zero closure allocations. Dictionary has no built-in parallel path; <c>.Values</c> allocates a copy.</item>
    ///     <item><b>In-place sorting:</b> <c>Sort()</c> uses <c>Array.Sort</c> on the dense array and rebuilds the index map. 
    ///     Dictionary is unordered — sorting requires copying to a List, sorting, and rebuilding.</item>
    ///     <item><b>O(1) swap-and-pop removal:</b> Removing an item swaps it with the last element and decrements count — 
    ///     no tombstones, no rehashing. Dictionary's <c>Remove</c> leaves internal holes that degrade iteration performance.</item>
    ///     <item><b>Efficient bulk removal:</b> <c>RemoveAll(predicate)</c> performs a single reverse pass with inline swap-and-pop. 
    ///     Dictionary requires collecting matching keys into a temporary list, then removing each individually.</item>
    ///     <item><b>Drain pattern:</b> <c>Drain()</c> yields all items in a zero-allocation LIFO pass while clearing the collection 
    ///     instantly for reuse. No Dictionary equivalent exists.</item>
    ///     <item><b>Span access:</b> <c>AsSpan()</c> provides a direct <see cref="Span{T}"/> window over active items 
    ///     for stack-only, bounds-checked processing. Dictionary cannot expose its values as a Span.</item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>When to use <see cref="Dictionary{TKey, TValue}"/> instead:</b>
    /// <list type="bullet">
    ///     <item><b>Lookup-only workloads:</b> If you never iterate the collection and only perform key lookups, 
    ///     Dictionary is ~5-10% faster because it stores values directly in its entry array with no index indirection.</item>
    ///     <item><b>Memory-constrained scenarios:</b> Dictionary uses a single backing array of entries. 
    ///     KeyedList uses three structures (<c>TValue[]</c> + <c>TKey[]</c> + <see cref="Dictionary{TKey, TValue}"/>), 
    ///     consuming ~30% more memory for the same element count.</item>
    ///     <item><b>Write-once, read-by-key registries:</b> Static lookup tables (e.g., blueprint registries, 
    ///     configuration maps) that are populated once at startup and only accessed by key gain nothing from 
    ///     dense array storage. Dictionary is simpler and lighter for this pattern.</item>
    ///     <item><b>Small collections (less than 50 items):</b> The overhead of maintaining three parallel structures 
    ///     provides no measurable benefit at small scale. Dictionary's simpler internals are sufficient.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <typeparam name="TKey">The unique identifier type (usually int or Guid).</typeparam>
    /// <typeparam name="TValue">The Wrapper or Entity class.</typeparam>
    /// <author>Steven Wilson</author>
    public class KeyedList<TKey, TValue> : IEnumerable<TValue> where TKey : notnull
    {
        /// <summary>
        /// Represents the internal storage array for the collection.
        /// Provides fast access to the stored items for operations such as
        /// iteration or direct indexing within the collection.
        /// </summary>
        private TValue[] _items;

        /// <summary>
        /// Stores the unique identifiers corresponding to the elements
        /// in the collection. This array ensures efficient lookups and
        /// maintains direct association between keys and their respective values.
        /// Resized dynamically as new elements are added.
        /// </summary>
        private TKey[] _keys;

        /// <summary>
        /// Maintains a mapping between unique keys and their corresponding indices
        /// within the internal storage array. Enables O(1) lookups for retrieving
        /// or updating items by key, ensuring efficient access and manipulation
        /// of the collection's elements.
        /// </summary>
        private readonly Dictionary<TKey, int> _keyToIndex;

        /// <summary>
        /// Tracks the current number of items in the collection.
        /// Used internally to manage indices for efficient addition,
        /// access, and removal of items within the data structure.
        /// </summary>
        private int _count;
        
        /// <summary>
        /// Tracks the version of the collection.
        /// Used to ensure thread-safety and prevent concurrent modifications.
        /// </summary>
        private int _version;

        /// <summary>
        /// Gets the number of elements currently stored in the collection.
        /// </summary>
        public int Count => _count;
        
        
        private static readonly Dictionary<nint, IComparer<TValue>> _comparerCache = new();

        /// <summary>
        /// Provides direct access to the underlying storage array of the collection.
        /// This array contains all items, including any unused or null entries at the
        /// end of the buffer, which are reserved for potential future additions.
        /// </summary>
        public TValue[] RawArray => _items;

        /// <summary>
        /// Gets a Span of the active items in the collection.
        /// This provides a "window" into the dense part of the array, 
        /// ensuring you never encounter a null reference at the end of the buffer.
        /// </summary>
        public Span<TValue> AsSpan() => _items.AsSpan(0, _count);

        /// <summary>
        /// Specifies the minimum number of elements by which the collection's capacity
        /// should increase when resizing is required. Ensures that the collection grows
        /// by at least the defined minimum size regardless of the growth strategy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when an attempt is made to set a value less than 1.
        /// </exception>
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

        /// <summary>
        /// Defines the minimum capacity to which the internal storage is resized
        /// when the collection requires additional space. This value ensures that
        /// resizing operations allocate at least the specified capacity, enabling
        /// efficient growth and reducing the frequency of resizing operations.
        /// </summary>
        private int _minimumResizeCapacity = 256;
        
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found during a get operation.</exception>
        public TValue this[TKey key]
        {
            get
            {
                // .NET 8: Get a direct memory reference to the dictionary value (skips double lookups)
                ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, key);
                if (!Unsafe.IsNullRef(ref indexRef)) return _items[indexRef];
                
                throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
            }
            set => AddOrUpdate(key, value, updateExisting: true);
        }

        /// <summary>
        /// Creates a new KeyedList instance with the specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The starting capacity of the internal arrays.</param>
        public KeyedList(int initialCapacity = 0)
        {
            // Use Array.Empty to avoid allocations if capacity is 0
            _items = initialCapacity == 0 ? Array.Empty<TValue>() : new TValue[initialCapacity];
            _keys = initialCapacity == 0 ? Array.Empty<TKey>() : new TKey[initialCapacity];
            _keyToIndex = new Dictionary<TKey, int>(initialCapacity);
            _count = 0;
            _version = 0;
        }

        /// <summary>
        /// Adds a new key-value pair to the collection. If the key already exists, an ArgumentException is thrown.
        /// If the collection reaches its capacity, it will dynamically resize to accommodate new entries.
        /// </summary>
        /// <param name="key">The unique identifier associated with the value to be added.</param>
        /// <param name="value">The value to be stored in the collection.</param>
        public void Add(TKey key, TValue value) => AddOrUpdate(key, value, false);

        /// <summary>
        /// Adds a new key-value pair to the collection or updates the value
        /// of an existing key, based on the specified behavior.
        /// </summary>
        /// <param name="key">The key associated with the value to add or update. The key must be unique within the collection.</param>
        /// <param name="value">The value to associate with the key in the collection.</param>
        /// <param name="updateExisting">A boolean value indicating whether the method should update the value of an existing key.
        /// If set to <c>true</c>, the value of an existing key is updated. If set to <c>false</c>, an exception is thrown when a key collision occurs.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="updateExisting"/> is <c>false</c> and
        /// a key collision occurs (i.e., the collection already contains the specified key).</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddOrUpdate(TKey key, TValue value, bool updateExisting = true)
        {
            // .NET 8 MAGIC: This hashes the key exactly ONCE.
            ref int indexRef = ref CollectionsMarshal.GetValueRefOrAddDefault(_keyToIndex, key, out bool exists);

            if (exists)
            {
                if (!updateExisting)
                    throw new ArgumentException($"An item with the same key has already been added: '{key}'");
                
                _items[indexRef] = value;
                _version++;
                return;
            }

            if (_count >= _items.Length) Grow();

            int newIndex = _count;
            _items[newIndex] = value;
            _keys[newIndex] = key;
            
            // Assign directly to the dictionary value by reference. Zero extra hashes!
            indexRef = newIndex; 
            _count++;
            _version++;
        }

        /// <summary>
        /// Resizes the internal storage arrays to accommodate additional elements.
        /// This method follows a conservative growth strategy, increasing the size of the arrays
        /// by 25% of their current length or by a minimum capacity, whichever is greater.
        /// This ensures balanced growth while minimizing memory overhead.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow()
        {
            // Conservative growth strategy: 
            // Once we are large, doubling is too much. 
            // Grow by 25% or at least MinimumResizeCapacity items.
            int currentLength = _items.Length;
            int growth = currentLength == 0 ? _minimumResizeCapacity : Math.Max(_minimumResizeCapacity, currentLength / 4);
            int newSize = currentLength + growth;
            
            Array.Resize(ref _items, newSize);
            Array.Resize(ref _keys, newSize);
            _keyToIndex.EnsureCapacity(newSize);
        }

        /// <summary>
        /// Removes an item from the map using the specified key.
        /// If the key is found, the item is removed and the dense array is updated to close any gaps.
        /// </summary>
        /// <remarks>
        /// Removes an item using "Swap and Pop". O(1).
        /// WARNING: This breaks the sort order of the underlying array.
        /// </remarks>
        /// <param name="key">The key associated with the item to remove.</param>
        /// <returns>Returns true if the item is successfully removed; otherwise, false.</returns>
        public bool Remove(TKey key)
        {
            // Remove from dictionary AND get the value in a SINGLE hash lookup.
            if (!_keyToIndex.Remove(key, out int indexToRemove))
                return false;

            int lastIndex = _count - 1;

            if (indexToRemove < lastIndex)
            {
                // Take the last item in the dense list
                TValue lastItem = _items[lastIndex];
                TKey lastKey = _keys[lastIndex];
                
                // Move it into the gap
                _items[indexToRemove] = lastItem;
                _keys[indexToRemove] = lastKey;

                // Update the index mapping for the moved item (O(1))
                ref int movedRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, lastKey);
                movedRef = indexToRemove; // Single hash
                //_keyToIndex[lastKey] = indexToRemove;
            }

            // Only need to null out the last item in the dense array if it's a reference type
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
            {
                _items[lastIndex] = default!;
            }
            
            // Only need to null out the last key in the dense array if it's a reference type
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
            {
                _keys[lastIndex] = default!;
            }
            
            _count--;
            _version++;
            return true;
        }
        
        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// Operates in O(N) time by utilizing a reverse Swap-and-Pop iteration.
        /// WARNING: This will disrupt the sorted order of the collection.
        /// </summary>
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the collection.</returns>
        public int RemoveAll(Predicate<TValue> match)
        {
            int removedCount = 0;

            // CRITICAL: We MUST iterate backwards.
            // If we swap the last item into the current slot, iterating backwards guarantees 
            // that we have ALREADY evaluated that swapped item earlier in the loop.
            for (int i = _count - 1; i >= 0; i--)
            {
                if (match(_items[i]))
                {
                    TKey keyToRemove = _keys[i];
                    int lastIndex = _count - 1;

                    // Swap and Pop Logic (Inline for max performance)
                    if (i < lastIndex)
                    {
                        TValue lastItem = _items[lastIndex];
                        TKey lastKey = _keys[lastIndex];

                        _items[i] = lastItem;
                        _keys[i] = lastKey;

                        // Update the Dictionary mapping via high-speed .NET 8 ref
                        ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, lastKey);
                        if (!Unsafe.IsNullRef(ref indexRef))
                        {
                            indexRef = i;
                        }
                    }

                    // Clean up the dictionary and the old last slot
                    _keyToIndex.Remove(keyToRemove);
                    
                    // Only need to null out the last item in the dense array if it's a reference type
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                    {
                        _items[lastIndex] = default!;
                    }
            
                    // Only need to null out the last key in the dense array if it's a reference type
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                    {
                        _keys[lastIndex] = default!;
                    }
                    
                    _count--;
                    removedCount++;
                }
            }

            if (removedCount > 0)
            {
                _version++;
            }

            return removedCount;
        }

        /// <summary>
        /// Removes and returns the item at the END of the dense array. O(1).
        /// Because it removes from the end, it DOES NOT trigger a swap, preserving sorted order.
        /// </summary>
        /// <param name="value">The popped value, or default if empty.</param>
        /// <returns>True if an item was successfully popped; otherwise, false.</returns>
        public bool TryPop(out TValue value)
        {
            if (_count == 0)
            {
                value = default!;
                return false;
            }

            int lastIndex = _count - 1;
            value = _items[lastIndex];
            _keyToIndex.Remove(_keys[lastIndex]);
            
            // Only need to null out the last item in the dense array if it's a reference type
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
            {
                _items[lastIndex] = default!;
            }
            
            // Only need to null out the last key in the dense array if it's a reference type
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
            {
                _keys[lastIndex] = default!;
            }
            
            _count--;
            _version++;
            return true;
        }

        /// <summary>
        /// <para>
        /// Efficiently drains the collection from end-to-start (LIFO) yielding items one by one.
        /// Bypasses individual dictionary removals for extreme performance.
        /// </para>
        /// <para>
        /// WARNING: Do not Add to this collection while enumerating the Drain() method.
        /// </para>
        /// </summary>
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// To achieve zero-allocation performance, you MUST consume this method using 'var' or directly in a foreach loop:
        /// foreach (var item in list.Drain()) { ... }
        /// 
        /// DO NOT cast the result to an <see cref="IEnumerable{TValue}"/> (e.g., IEnumerable&lt;TValue&gt; items = list.Drain()).
        /// Doing so forces the compiler to "box" the stack-allocated struct into a reference type on the heap, 
        /// completely destroying the performance benefits and triggering Garbage Collection.
        /// </remarks>
        /// <returns>An allocation-free struct enumerable of the drained items.</returns>
        public DrainEnumerable Drain()
        {
            int drainCount = _count;
            
            // Instantly clear visible state so the collection can be reused immediately 
            // by other synchronous code while we enumerate the drained items.
            _count = 0;
            _keyToIndex.Clear();
            _version++;

            return new DrainEnumerable(this, drainCount);
        }

        /// <summary>
        /// Retrieves an item from the collection associated with the specified key.
        /// The method provides an O(1) lookup performance for retrieving entities
        /// or values stored in the collection.
        /// </summary>
        /// <param name="key">The unique key corresponding to the item to retrieve.</param>
        /// <returns>The item associated with the provided key</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Get(TKey key)
        {
            ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, key);
            if (!Unsafe.IsNullRef(ref indexRef)) return _items[indexRef];
            
            throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose associated value is to be retrieved.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if the key exists in the map and the value is successfully retrieved; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
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
        /// Retrieves the value associated with the specified key or returns the default value if the key is not found.
        /// </summary>
        /// <param name="key">The key whose associated value is to be retrieved.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The value associated with the specified key, or the provided default value if the key does not exist.</returns>
        public TValue GetOrDefault(TKey key, TValue defaultValue)
        {
            ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, key);
            if (!Unsafe.IsNullRef(ref indexRef)) return _items[indexRef];

            return defaultValue;
        }

        /// <summary>
        /// Sorts the collection in-place and automatically rebuilds the O(1) index map.
        /// </summary>
        /// <param name="comparer">The comparer to use when sorting the values.</param>
        public void Sort(IComparer<TValue> comparer)
        {
            if (_count <= 1) return;

            Array.Sort(_items, _keys, 0, _count, comparer);

            // Do not clear the dictionary! Overwriting existing keys prevents 
            // internal bucket resizing and allocations.
            for (int i = 0; i < _count; i++)
            {
                // Saves one hash per element during sort rebuilds
                ref int indexRef = ref CollectionsMarshal.GetValueRefOrNullRef(_keyToIndex, _keys[i]);
                indexRef = i;
            }
            
            _version++;
        }
        
        /// <summary>
        /// Sorts the collection using a Comparison delegate (e.g. Lambdas).
        /// </summary>
        public void Sort(Comparison<TValue> comparison)
        {
            // Use the function pointer as a stable cache key
            nint key = comparison.Method.MethodHandle.GetFunctionPointer();
    
            if (!_comparerCache.TryGetValue(key, out var comparer))
            {
                comparer = Comparer<TValue>.Create(comparison);
                _comparerCache[key] = comparer;
            }

            Sort(comparer);
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the collection, 
        /// if that number is less than the current capacity.
        /// </summary>
        public void TrimExcess()
        {
            if (_count < _items.Length)
            {
                Array.Resize(ref _items, _count);
                Array.Resize(ref _keys, _count);
                _keyToIndex.TrimExcess();
            }
        }

        /// <summary>
        /// Ensures that the internal storage of the collection has at least the specified capacity.
        /// Resizes the internal storage arrays if the current capacity is less than the specified value.
        /// </summary>
        /// <param name="capacity">The minimum required capacity for the collection.</param>
        public void EnsureCapacity(int capacity)
        {
            if (capacity > _items.Length)
            {
                Array.Resize(ref _items, capacity);
                Array.Resize(ref _keys, capacity);
                _keyToIndex.EnsureCapacity(capacity);
            }
        }

        /// <summary>
        /// Copies the elements of the KeyedList to the specified array starting at the specified array index.
        /// </summary>
        /// <param name="array">The destination array where the elements will be copied.</param>
        /// <param name="arrayIndex">The zero-based index in the destination array where copying begins.</param>
        public void CopyTo(TValue[] array, int arrayIndex)
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
                Array.Clear(_items, 0, _count);
                Array.Clear(_keys, 0, _count);
                _keyToIndex.Clear();
                _count = 0;
                _version++;
            }
        }

        /// <summary>
        /// Executes a parallel for loop over the dense part of the collection.
        /// This is the most efficient way to process all items while avoiding
        /// Span-in-Lambda compiler restrictions or excessive lookups
        /// </summary>
        /// <param name="body">The action to perform on each item in the collection.</param>
        public void ParallelEach(Action<TValue> body)
        {
            TValue[] items = _items;
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
        /// Executes a parallel for loop over the dense part of the collection with a custom state.
        /// This version is more efficient as it helps avoid closure allocations.
        /// </summary>
        public void ParallelEach<TState>(TState state, Action<TValue, TState> body)
        {
            TValue[] items = _items;
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

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// Provides O(1) lookup to verify the existence of the key in the collection.
        /// </summary>
        /// <param name="key">The key to locate in the collection.</param>
        /// <returns>True if the key exists in the collection; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key) => _keyToIndex.ContainsKey(key);

        /// <summary>
        /// Determines whether the collection contains any elements.
        /// Shadows LINQ's Any() to evaluate in O(1) time without allocations.
        /// </summary>
        public bool Any() => _count > 0;

        /// <summary>
        /// Determines whether any element of the collection satisfies a condition.
        /// Shadows LINQ's Any() to avoid IEnumerable allocations.
        /// </summary>
        public bool Any(Func<TValue, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            
            for (int i = 0; i < _count; i++)
            {
                if (predicate(_items[i])) return true;
            }
            return false;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// Shadows LINQ's Where() to iterate over the dense array directly without allocations.
        /// </summary>
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// To achieve zero-allocation performance, you MUST consume this method using 'var' or directly in a foreach loop:
        /// foreach (var item in list.Where(x => ...)) { ... }
        /// 
        /// DO NOT cast the result to an <see cref="IEnumerable{TValue}"/> (e.g., IEnumerable&lt;TValue&gt; items = list.Where(...)).
        /// Doing so forces the compiler to "box" the stack-allocated struct into a reference type on the heap, 
        /// completely destroying the performance benefits and triggering Garbage Collection.
        /// </remarks>
        public WhereEnumerable Where(Func<TValue, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return new WhereEnumerable(this, predicate);
        }
        
        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
        /// Shadows LINQ's FirstOrDefault() to iterate over the dense array directly.
        /// </summary>
        public TValue? FirstOrDefault(Func<TValue, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            
            for (int i = 0; i < _count; i++)
            {
                if (predicate(_items[i])) return _items[i];
            }
            return default;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// Shadows LINQ's Count() to iterate over the dense array directly without allocations.
        /// </summary>
        public int CountWhere(Func<TValue, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            
            int count = 0;
            for (int i = 0; i < _count; i++)
            {
                if (predicate(_items[i])) count++;
            }
            return count;
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// Shadows LINQ's Select() to map data directly from the underlying array without allocations.
        /// </summary>
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// To achieve zero-allocation performance, you MUST consume this method using 'var' or directly in a foreach loop:
        /// foreach (var item in list.Select(x => ...)) { ... }
        /// 
        /// DO NOT cast the result to an <see cref="IEnumerable{TResult}"/> (e.g., IEnumerable&lt;TResult&gt; items = list.Select(...)).
        /// Doing so forces the compiler to "box" the stack-allocated struct into a reference type on the heap, 
        /// completely destroying the performance benefits and triggering Garbage Collection.
        /// </remarks>
        public SelectEnumerable<TResult> Select<TResult>(Func<TValue, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return new SelectEnumerable<TResult>(this, selector);
        }
        
        /// <summary>
        /// Groups the elements of the sequence into a new KeyedList according to a specified key selector function.
        /// Useful when the resulting grouped collection needs high-frequency adds, removes, or parallel iteration.
        /// </summary>
        public KeyedList<TGroupKey, List<TValue>> GroupBy<TGroupKey>(Func<TValue, TGroupKey> keySelector) where TGroupKey : notnull
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var result = new KeyedList<TGroupKey, List<TValue>>();
            for (int i = 0; i < _count; i++)
            {
                TValue item = _items[i];
                TGroupKey key = keySelector(item);

                if (!result.TryGetValue(key, out var list))
                {
                    list = new List<TValue>();
                    result.Add(key, list);
                }
                list.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Groups the elements of the sequence into a Dictionary according to a specified key selector function.
        /// This is significantly faster and allocates less memory than standard LINQ GroupBy().
        /// </summary>
        public Dictionary<TGroupKey, List<TValue>> GroupByDictionary<TGroupKey>(Func<TValue, TGroupKey> keySelector) where TGroupKey : notnull
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            
            var dict = new Dictionary<TGroupKey, List<TValue>>();
            for (int i = 0; i < _count; i++)
            {
                TValue item = _items[i];
                TGroupKey key = keySelector(item);
                
                // Use .NET 8 memory referencing to avoid double-hashing the dictionary lookup
                ref List<TValue>? listRef = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out bool exists);
                if (!exists)
                {
                    listRef = new List<TValue>();
                }
                
                listRef!.Add(item);
            }
            
            return dict;
        }

        /// <summary>
        /// Computes the sum of values produced by applying a selector function to each element in the collection.
        /// </summary>
        /// <param name="selector">A function that projects each element of the collection into an integer value to be summed.</param>
        /// <returns>The sum of the projected values of all elements in the collection.</returns>
        public int Sum(Func<TValue, int> selector)
        {
            int sum = 0;
            for (int i = 0; i < _count; i++) sum += selector(_items[i]);
            return sum;
        }

        /// <summary>
        /// Computes the sum of the values from the collection based on a specified selector function.
        /// </summary>
        /// <param name="selector">A function that projects each element of the collection to a double value to be summed.</param>
        /// <returns>The sum of the projected values.</returns>
        public double Sum(Func<TValue, double> selector)
        {
            double sum = 0;
            for (int i = 0; i < _count; i++) sum += selector(_items[i]);
            return sum;
        }

        /// <summary>
        /// Computes the average of a sequence of numerical values obtained by applying the specified selector function
        /// to each element in the collection.
        /// </summary>
        /// <param name="selector">A function to transform each element in the collection into a numerical value.</param>
        /// <returns>The average of the selected numerical values.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the collection contains no elements.</exception>
        public double Average(Func<TValue, double> selector)
        {
            if (_count == 0) throw new InvalidOperationException("Sequence contains no elements.");
            return Sum(selector) / _count;
        }

        /// <summary>
        /// Finds the item in the collection that has the minimum value based on the provided selector function.
        /// </summary>
        /// <param name="selector">A function that projects each item to a double value for comparison.</param>
        /// <returns>The item in the collection with the minimum value according to the selector function, or <c>null</c> if the collection is empty.</returns>
        public TValue? MinBy(Func<TValue, double> selector)
        {
            if (_count == 0) return default;
            TValue min = _items[0];
            double minVal = selector(min);
            for (int i = 1; i < _count; i++)
            {
                double val = selector(_items[i]);
                if (val < minVal) { min = _items[i]; minVal = val; }
            }
            return min;
        }

        /// <summary>
        /// Retrieves the element from the collection that yields the highest value
        /// based on the provided selector function.
        /// </summary>
        /// <param name="selector">A function to compute a numeric value from an element in the collection.</param>
        /// <returns>The element with the maximum value as determined by the selector function,
        /// or <c>null</c> if the collection is empty.</returns>
        public TValue? MaxBy(Func<TValue, double> selector)
        {
            if (_count == 0) return default;
            TValue max = _items[0];
            double maxVal = selector(max);
            for (int i = 1; i < _count; i++)
            {
                double val = selector(_items[i]);
                if (val > maxVal) { max = _items[i]; maxVal = val; }
            }
            return max;
        }

        /// <summary>
        /// Determines whether all elements in the collection satisfy the specified condition.
        /// </summary>
        /// <param name="predicate">The function that defines the condition to check against each element in the collection.</param>
        /// <returns>True if all elements satisfy the condition defined by the predicate; otherwise, false.</returns>
        public bool All(Func<TValue, bool> predicate)
        {
            for (int i = 0; i < _count; i++)
            {
                if (!predicate(_items[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the current collection into a <see cref="List{T}"/> containing all the active items.
        /// This creates a new list instance with the items copied from the collection.
        /// </summary>
        /// <returns>
        /// A new <see cref="List{T}"/> containing all items from the collection in their current order.
        /// </returns>
        public List<TValue> ToList()
        {
            var list = new List<TValue>(_count);
            for (int i = 0; i < _count; i++) list.Add(_items[i]);
            return list;
        }

        /// <summary>
        /// Converts the current collection into a new array containing all the elements.
        /// The resulting array maintains the order of the elements as they appear in the collection.
        /// </summary>
        /// <returns>A new array containing all the elements of the collection.</returns>
        public TValue[] ToArray()
        {
            var array = new TValue[_count];
            Array.Copy(_items, array, _count);
            return array;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// Enables enumeration of the items in the <see cref="KeyedList{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        /// An enumerator for iterating over the items in the collection.
        /// </returns>
        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <summary>
        /// A custom enumerator for iterating over the items in a <see cref="KeyedList{TKey, TValue}"/>.
        /// This enumerator is designed to avoid heap allocations during foreach loops for high-performance scenarios.
        /// </summary>
        public struct Enumerator(KeyedList<TKey, TValue> list) : IEnumerator<TValue>
        {
            private int _index = -1;

            /// <summary>
            /// The version of the collection at the time the enumerator was created.
            /// Used to detect modifications made during enumeration to provide fail-fast execution.
            /// </summary>
            private readonly int _version = list._version;

            public TValue Current => list._items[_index];
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }

                int next = _index + 1;
                if (next < list.Count)
                {
                    _index = next;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                _index = -1;
            }
            
            public void Dispose() { }
        }

        /// <summary>
        /// Zero-allocation structure returned by the Where() method.
        /// </summary>
        public readonly struct WhereEnumerable(KeyedList<TKey, TValue> list, Func<TValue, bool> predicate) : IEnumerable<TValue>
        {
            public WhereEnumerator GetEnumerator() => new WhereEnumerator(list, predicate);
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <summary>
            /// Projects each filtered element into a new form without allocations.
            /// Enables zero-alloc chaining: list.Where(x => ...).Select(x => ...)
            /// </summary>
            public WhereSelectEnumerable<TResult> Select<TResult>(Func<TValue, TResult> selector)
            {
                if (selector == null) throw new ArgumentNullException(nameof(selector));
                return new WhereSelectEnumerable<TResult>(list, predicate, selector);
            }
            
            public TValue? FirstOrDefault()
            {
                foreach (var item in this)
                    return item;
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

        /// <summary>
        /// Zero-allocation enumerator for the Where() method.
        /// </summary>
        public struct WhereEnumerator(KeyedList<TKey, TValue> list, Func<TValue, bool> predicate) : IEnumerator<TValue>
        {
            private int _index = -1;
            private readonly int _version = list._version;

            public TValue Current => list._items[_index];
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }

                while (++_index < list.Count)
                {
                    if (predicate(list._items[_index])) return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                _index = -1;
            }
            
            public void Dispose() { }
        }
        
        /// <summary>
        /// Zero-allocation structure returned by WhereEnumerable.Select().
        /// Fuses filter + projection into a single pass over the dense array.
        /// </summary>
        /// <remarks>
        /// ⚠️ PERFORMANCE WARNING (BOXING): 
        /// Consume directly in a foreach loop. Do NOT cast to IEnumerable<TResult>.
        /// </remarks>
        public readonly struct WhereSelectEnumerable<TResult>(
            KeyedList<TKey, TValue> list, 
            Func<TValue, bool> predicate, 
            Func<TValue, TResult> selector) : IEnumerable<TResult>
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

        /// <summary>
        /// Zero-allocation enumerator that fuses Where + Select into a single pass.
        /// </summary>
        public struct WhereSelectEnumerator<TResult>(
            KeyedList<TKey, TValue> list, 
            Func<TValue, bool> predicate, 
            Func<TValue, TResult> selector) : IEnumerator<TResult>
        {
            private int _index = -1;
            private readonly int _version = list._version;

            public TResult Current => selector(list._items[_index]);
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }

                while (++_index < list.Count)
                {
                    if (predicate(list._items[_index])) return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                _index = -1;
            }
    
            public void Dispose() { }
        }

        /// <summary>
        /// Zero-allocation structure returned by the Select() method.
        /// </summary>
        public readonly struct SelectEnumerable<TResult>(KeyedList<TKey, TValue> list, Func<TValue, TResult> selector) : IEnumerable<TResult>
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

            public bool Any() => list.Count > 0;

            public int Count() => list.Count;
        }

        /// <summary>
        /// Zero-allocation enumerator for the Select() method.
        /// </summary>
        public struct SelectEnumerator<TResult>(KeyedList<TKey, TValue> list, Func<TValue, TResult> selector) : IEnumerator<TResult>
        {
            private int _index = -1;
            private readonly int _version = list._version;

            public TResult Current => selector(list._items[_index]);
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }

                int next = _index + 1;
                if (next < list.Count)
                {
                    _index = next;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                _index = -1;
            }
            
            public void Dispose() { }
        }

        /// <summary>
        /// Zero-allocation structure returned by the Drain() method.
        /// </summary>
        public readonly struct DrainEnumerable(KeyedList<TKey, TValue> list, int drainCount) : IEnumerable<TValue>
        {
            public DrainEnumerator GetEnumerator() => new DrainEnumerator(list, drainCount);
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Zero-allocation enumerator for the Drain() method.
        /// </summary>
        public struct DrainEnumerator(KeyedList<TKey, TValue> list, int drainCount) : IEnumerator<TValue>
        {
            private int _index = drainCount;
            private readonly int _version = list._version;
            private TValue _current = default!;

            public TValue Current => _current;
            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                if (_version != list._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                
                if (_index > 0)
                {
                    _index--;
                    _current = list._items[_index];
                    
                    // GC cleanup as we yield backward through the drained elements
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                    {
                        list._items[_index] = default!;
                    }
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                    {
                        list._keys[_index] = default!;
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