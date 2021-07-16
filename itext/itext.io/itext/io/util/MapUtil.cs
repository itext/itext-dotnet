using System;
using System.Collections.Generic;

namespace iText.IO.Util {
    /// <summary>Utility class for work with collections.</summary>
    /// <remarks>Utility class for work with collections. Not for public use.</remarks>
    public sealed class MapUtil {
        private MapUtil() {
        }

        /// <summary>
        /// Checks if two
        /// <see cref="System.Collections.IDictionary{K, V}">maps</see>
        /// are equal: the are of the same types and has equal number of stored
        /// entries and both has the same set of keys ans each key is associated with an appropriate
        /// value.
        /// </summary>
        /// <param name="m1">is the first map</param>
        /// <param name="m2">is the second map</param>
        /// <typeparam name="K">is a type of keys</typeparam>
        /// <typeparam name="V">is a type of values</typeparam>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if maps are equal and
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool Equals<K, V>(IDictionary<K, V> m1, IDictionary<K, V> m2) {
            if (m1 == m2) {
                return true;
            }
            if (m1 == null || m2 == null) {
                return false;
            }
            if (!m1.GetType().Equals(m2.GetType())) {
                return false;
            }
            if (m1.Count != m2.Count) {
                return false;
            }
            foreach (KeyValuePair<K, V> entry in m1) {
                V obj1 = entry.Value;
                V obj2 = m2.Get(entry.Key);
                if (!m2.ContainsKey(entry.Key) || !Object.Equals(obj1, obj2)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Merges data from source Map into destination Map using provided function if key exists in both Maps.
        ///     </summary>
        /// <remarks>
        /// Merges data from source Map into destination Map using provided function if key exists in both Maps.
        /// If key doesn't exist in destination Map in will be putted directly.
        /// </remarks>
        /// <param name="destination">Map to which data will be merged.</param>
        /// <param name="source">Map from which data will be taken.</param>
        /// <param name="valuesMerger">function which will be used to merge Maps values.</param>
        /// <typeparam name="K">is a type of keys</typeparam>
        /// <typeparam name="V">is a type of values</typeparam>
        public static void Merge<K, V>(IDictionary<K, V> destination, IDictionary<K, V> source, Func<V, V, V> valuesMerger
            ) {
            if (destination == source) {
                return;
            }
            foreach (KeyValuePair<K, V> entry in source) {
                V value = destination.Get(entry.Key);
                if (value == null) {
                    destination.Put(entry.Key, entry.Value);
                }
                else {
                    destination.Put(entry.Key, valuesMerger(value, entry.Value));
                }
            }
        }

        /// <summary>
        /// Calculates the hash code of the
        /// <see cref="System.Collections.IDictionary{K, V}">map</see>.
        /// </summary>
        /// <param name="m1">is the map</param>
        /// <typeparam name="K">is a type of keys</typeparam>
        /// <typeparam name="V">is a type of values</typeparam>
        /// <returns>
        /// the hash code of the
        /// <see cref="System.Collections.IDictionary{K, V}">map</see>.
        /// </returns>
        public static int GetHashCode<K, V>(IDictionary<K, V> m1) {
            if (null == m1) {
                return 0;
            }
            int hash = 0;
            foreach (KeyValuePair<K, V> entry in m1) {
                K key = entry.Key;
                V value = entry.Value;
                hash = 31 * hash + (key == null ? 0 : key.GetHashCode());
                hash = 31 * hash + (value == null ? 0 : value.GetHashCode());
            }
            return hash;
        }

        /// <summary>Puts value to map if the value is not null.</summary>
        /// <param name="map">the map in which value can be pushed</param>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        /// <typeparam name="K">is a type of key</typeparam>
        /// <typeparam name="V">is a type of value</typeparam>
        public static void PutIfNotNull<K, V>(IDictionary<K, V> map, K key, V value) {
            if (value != null) {
                map.Put(key, value);
            }
        }
    }
}
