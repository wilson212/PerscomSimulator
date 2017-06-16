using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom
{
    static class ListExtensions
    {
        /// <summary>
        /// Creates a new list by Cloning all items in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// Disposes all items in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public static void DisposeItems<T>(this IEnumerable<T> source) where T : IDisposable
        {
            foreach (var item in source)
            {
                item.Dispose();
            }
        }


        /// <summary>
        /// Forces garbage colleciton on a list and all of its items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lista"></param>
        public static void LiberarLista<T>(this List<T> lista)
        {
            lista.Clear();
            int identificador = GC.GetGeneration(lista);
            GC.Collect(identificador, GCCollectionMode.Forced);
        }
    }
}
