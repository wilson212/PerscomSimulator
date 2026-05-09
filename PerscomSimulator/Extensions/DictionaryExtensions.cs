using System.Collections.Generic;

namespace Perscom
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Retrieves the value associated with the specified key from the dictionary.
        /// If the key does not exist, the default value for the value type is returned.
        /// </summary>
        /// <param name="dic">The dictionary to search in.</param>
        /// <param name="key">The key whose associated value is to be returned.</param>
        /// <typeparam name="K">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="V">The type of the values in the dictionary.</typeparam>
        /// <returns>The value associated with the specified key, or the default value for the value type if the key is not found.</returns>
        public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dic, K key)
        {
            V ret;
            bool found = dic.TryGetValue(key, out ret);
            if (found)
            {
                return ret;
            }
            return default(V);
        }
    }
}
