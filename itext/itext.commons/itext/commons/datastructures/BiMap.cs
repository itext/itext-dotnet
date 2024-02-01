/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;

namespace iText.Commons.Datastructures {
    /// <summary>A simple bi-directional map.</summary>
    /// <typeparam name="K">the type of the first key</typeparam>
    /// <typeparam name="V">the type of the second key</typeparam>
    public sealed class BiMap<K, V> {
        private readonly IDictionary<K, V> map = new Dictionary<K, V>();

        private readonly IDictionary<V, K> inverseMap = new Dictionary<V, K>();

        /// <summary>
        /// Creates a new
        /// <see cref="BiMap{K, V}"/>
        /// instance.
        /// </summary>
        public BiMap() {
        }

        // empty constructor
        /// <summary>Puts the entry into the map.</summary>
        /// <remarks>
        /// Puts the entry into the map.
        /// If the key already exists, the value will be overwritten.
        /// If the value already exists, the key will be overwritten.
        /// If both key and value already exist, the entry will be overwritten.
        /// If neither key nor value already exist, the entry will be added.
        /// </remarks>
        /// <param name="k">the key</param>
        /// <param name="v">the value</param>
        public void Put(K k, V v) {
            map.Put(k, v);
            inverseMap.Put(v, k);
        }

        /// <summary>Gets the value by key.</summary>
        /// <param name="value">the key</param>
        /// <returns>the value</returns>
        public V GetByKey(K value) {
            return map.Get(value);
        }

        /// <summary>Gets the key by value.</summary>
        /// <param name="key">the value</param>
        /// <returns>the key</returns>
        public K GetByValue(V key) {
            return inverseMap.Get(key);
        }

        /// <summary>Removes the entry by key.</summary>
        /// <param name="k">the key</param>
        public void RemoveByKey(K k) {
            V v = map.JRemove(k);
            if (v != null) {
                inverseMap.JRemove(v);
            }
        }

        /// <summary>Removes the entry by value.</summary>
        /// <param name="v">the value</param>
        public void RemoveByValue(V v) {
            K k = inverseMap.JRemove(v);
            if (k != null) {
                map.JRemove(k);
            }
        }

        /// <summary>Gets the size of the map.</summary>
        /// <returns>the size of the map</returns>
        public int Size() {
            return map.Count;
        }

        /// <summary>removes all entries from the map.</summary>
        public void Clear() {
            map.Clear();
            inverseMap.Clear();
        }

        /// <summary>Checks if the map is empty.</summary>
        /// <returns>true, if the map is empty</returns>
        public bool IsEmpty() {
            return map.IsEmpty();
        }

        /// <summary>Checks if the map contains the key.</summary>
        /// <param name="k">the key</param>
        /// <returns>true, if the map contains the key</returns>
        public bool ContainsKey(K k) {
            return map.ContainsKey(k);
        }

        /// <summary>Checks if the map contains the value.</summary>
        /// <param name="v">the value</param>
        /// <returns>true, if the map contains the value</returns>
        public bool ContainsValue(V v) {
            return inverseMap.ContainsKey(v);
        }
    }
}
