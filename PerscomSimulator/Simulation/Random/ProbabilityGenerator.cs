using System;
using System.Collections.Generic;
using System.Linq;

namespace Perscom.Simulation
{
    /// <summary>
    /// A generic class that provides functionality for managing and generating items
    /// based on their associated probabilities.
    /// </summary>
    /// <remarks>P( TEntity item ) = item.Probability / CumulativeProbability</remarks>
    /// <typeparam name="T">
    /// The type of items to be managed. Must implement the <see cref="IProbable"/> interface.
    /// </typeparam>
    public sealed class ProbabilityGenerator<T> where T : IProbable
    {
        /// <summary>
        /// Internal list of items that can be generated using the <see cref="Spawn()"/> 
        /// or <see cref="TrySpawn(out T)"/> methods
        /// </summary>
        private List<ProbableItem<T>> Items;

        /// <summary>
        /// Gets the number of items stored in this <see cref="ProbabilityGenerator{T}"/>
        /// </summary>
        public int ItemCount => Items.Count;

        /// <summary>
        /// The Cumulative Probability of all the spawnable objects
        /// </summary>
        public int CumulativeProbability { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilityGenerator{T}"/>
        /// </summary>
        public ProbabilityGenerator()
        {
            Items = new List<ProbableItem<T>>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilityGenerator{T}"/>, and adds
        /// the <paramref name="objects"/> to the internal pool
        /// </summary>
        /// <param name="objects"></param>
        public ProbabilityGenerator(IEnumerable<T> objects)
        {
            Items = new List<ProbableItem<T>>();

            AddRange(objects);
        }

        /// <summary>
        /// Adds the item to the item pool
        /// </summary>
        /// <param name="obj"></param>
        public void Add(T obj)
        {
            var spawnable = new ProbableItem<T>(this, obj, CumulativeProbability);
            CumulativeProbability = spawnable.MaxThreshold;
            Items.Add(spawnable);
        }

        /// <summary>
        /// Adds a range of items to the item pool
        /// </summary>
        /// <param name="objects"></param>
        public void AddRange(IEnumerable<T> objects)
        {
            // Dont allow a null pass
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            foreach (var o in objects)
            {
                var spawnable = new ProbableItem<T>(this, o, CumulativeProbability);
                CumulativeProbability = spawnable.MaxThreshold;
                Items.Add(spawnable);
            }
        }

        /// <summary>
        /// Gets all items from this <see cref="ProbabilityGenerator{T}"/>
        /// </summary>
        /// <returns></returns>
        public ProbableItem<T>[] GetItemPool()
        {
            return Items.ToArray();
        }

        /// <summary>
        /// Gets all items from this <see cref="ProbabilityGenerator{T}"/>
        /// </summary>
        /// <returns></returns>
        public T[] GetItems()
        {
            return Items.Select(x => x.Item).ToArray();
        }

        /// <summary>
        /// Clears all items in this <see cref="ProbabilityGenerator{T}"/>
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Filters the items within this <see cref="ProbabilityGenerator{T}"/> based on the predicate
        /// </summary>
        /// <param name="predicate">The condition to apply</param>
        /// <returns>A new <see cref="ProbabilityGenerator{T}"/> containing only the items that satisfies the condition</returns>
        public ProbabilityGenerator<T> Where(Func<ProbableItem<T>, bool> predicate)
        {
            var items = Items.Where(predicate).Select(x => x.Item).ToArray();
            return (items == null) ? new ProbabilityGenerator<T>() : new ProbabilityGenerator<T>(items);
        }

        /// <summary>
        /// Determines whether any item within this <see cref="ProbabilityGenerator{T}"/> satisfies the condition
        /// </summary>
        /// <param name="predicate">The condition to apply</param>
        /// <returns>A bool indicating whether any item satisfies the condition</returns>
        public bool Any(Func<ProbableItem<T>, bool> predicate)
        {
            return Items.Any(predicate);
        }

        /// <summary>
        /// Returns an instance of <typeparamref name="T"/> based off of the 
        /// RNG probability of that instance.
        /// </summary>
        /// <returns></returns>
        public T Spawn()
        {
            // Ensure we have at least 1 object to spawn
            if (Items.Count == 0)
                throw new Exception("There are no spawnable entities");

            // If we have just 1 item, return that
            if (Items.Count == 1)
                return Items.First().Item;

            // Generate the next random number
            var i = Random.Shared.Next(1, CumulativeProbability + 1);
            return (from s in Items where s.ContainsThreshold(i) select s.Item).First();
        }

        /// <summary>
        /// Returns an instance of <typeparamref name="T"/> based off of the 
        /// RNG probability of that instance.
        /// </summary>
        /// <returns></returns>
        public bool TrySpawn(out T retVal)
        {
            // Set to default
            retVal = default;

            // Ensure we have at least 1 object to spawn
            if (Items.Count == 0)
            {
                return false;
            }
            else if (Items.Count == 1)
            {
                // If we have just 1 item, return that
                retVal = Items.First().Item;
                return true;
            }

            // Generate the next random number
            try
            {
                var i = Random.Shared.Next(1, CumulativeProbability + 1);
                retVal = (from s in Items where s.ContainsThreshold(i) select s.Item).First();
                return true;
            }
            catch (Exception)
            {
                //Log.Exception(e);
                return false;
            }
        }
        
        /// <summary>
        /// Returns an instance of <typeparamref name="T"/> based on RNG probability,
        /// then removes it from the pool so it cannot be selected again.
        /// </summary>
        public T SpawnUnique()
        {
            if (Items.Count == 0)
                throw new Exception("There are no spawnable entities");

            T result;

            if (Items.Count == 1)
            {
                result = Items[0].Item;
                Items.Clear();
                CumulativeProbability = 0;
                return result;
            }

            var i = Random.Shared.Next(1, CumulativeProbability + 1);
            var match = Items.First(s => s.ContainsThreshold(i));
            result = match.Item;

            Items.Remove(match);

            // Rebuild thresholds since removing an item shifts all ranges
            Rebuild();

            return result;
        }

        /// <summary>
        /// Attempts to spawn a unique item. Returns false if the pool is empty.
        /// </summary>
        public bool TrySpawnUnique(out T retVal)
        {
            retVal = default;

            if (Items.Count == 0)
                return false;

            retVal = SpawnUnique();
            return true;
        }

        /// <summary>
        /// Rebuilds the internal item pool
        /// </summary>
        internal void Rebuild()
        {
            T[] items = GetItems();

            Items.Clear();
            CumulativeProbability = 0;

            AddRange(items);
        }
    }
}
