using System;
using System.Collections.Generic;
using Perscom.Collections;

namespace Perscom.Extensions;

/// <summary>
/// Extension methods for collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> to a <see cref="DenseList{T}"/>.
    /// Uses optimized block-copies if the source is a List or another DenseList.
    /// </summary>
    public static DenseList<T> ToDenseList<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var list = new DenseList<T>();
        // AddRange already handles pre-allocation and fast memory paths
        list.AddRange(source); 
        return list;
    }

    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> to an <see cref="IdentityList{TKey, T}"/>.
    /// Requires that T implements <see cref="IKeyed{TKey}"/>.
    /// </summary>
    public static IdentityList<TKey, T> ToIdentityList<TKey, T>(this IEnumerable<T> source)
        where TKey : notnull
        where T : IKeyed<TKey>
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var list = new IdentityList<TKey, T>();
        // AddRange handles pre-allocation and duplicate key checks
        list.AddRange(source); 
        return list;
    }
}