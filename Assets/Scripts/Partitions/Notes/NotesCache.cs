using System.Collections.Generic;
using UnityEngine;

namespace Practear.Partitions
{
    /// <summary>
    /// A runtime cache to keep loaded notes and avoid reloading them everytime.
    /// </summary>
    static public class NotesCache
    {

        #region Instance variables

        /// <summary>
        /// The cache (key is path to resource, value is audio clip).
        /// </summary>
        static private Dictionary<string, AudioClip> s_Cache = new Dictionary<string, AudioClip>();

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Does the cache contain a value ?
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>true if this value has been cached, false otherwise.</returns>
        static public bool Contains(string key)
        {
            return s_Cache.ContainsKey(key);
        }

        /// <summary>
        /// Add an audio clip to the cache.
        /// </summary>
        /// <param name="key">The key (path to resource)</param>
        /// <param name="value">The audio clip value.</param>
        static public void AddOrUpdate(string key, AudioClip value)
        {
            if (s_Cache.ContainsKey(key))
                s_Cache[key] = value;
            else
                s_Cache.Add(key, value);
        }

        /// <summary>
        /// Fetch a cached value.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The cached audio clip for this key.</returns>
        static public AudioClip GetCachedValue(string key)
        {
            if (!Contains(key))
                return null;

            return s_Cache[key];
        }

        /// <summary>
        /// Clear the cache
        /// </summary>
        static public void Clear()
        {
            s_Cache.Clear();
        }

        #endregion // Methods

    }
}

