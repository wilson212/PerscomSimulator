using System.Collections.Generic;

namespace Perscom
{
    public static class DictionaryExtensions
    {
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
