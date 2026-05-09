namespace Perscom.Collections
{
    /// <summary>
    /// Implemented by any type that can provide its own unique key for indexed collection storage.
    /// </summary>
    /// <typeparam name="TKey">The key type (usually int or Guid).</typeparam>
    public interface IKeyed<out TKey>
    {
        /// <summary>
        /// The unique key that identifies this item within an <see cref="IdentityList{TKey,T}"/>.
        /// Must remain stable for the lifetime of the item in the collection.
        /// </summary>
        TKey Key { get; }
    }
}