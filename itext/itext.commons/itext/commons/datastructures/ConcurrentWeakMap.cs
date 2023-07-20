/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace iText.Commons.Datastructures {
    /// <summary>Concurrent weak hash map implementation.</summary>
    /// <typeparam name="K">type of the keys</typeparam>
    /// <typeparam name="V">type of the values</typeparam>
    public class ConcurrentWeakMap<K, V> : IDictionary<K, V>
        where K : class
        where V : class {
        
        private static readonly string UNSUPPORTED_OPERATION = "This operation is not supported.";
        
        private readonly ConditionalWeakTable<K, V> map = new ConditionalWeakTable<K, V>();

        /// <summary><inheritDoc/></summary>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public void Add(KeyValuePair<K, V> item) {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public void Clear() {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool Contains(KeyValuePair<K, V> item) {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool Remove(KeyValuePair<K, V> item) {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public int Count => throw new NotSupportedException(UNSUPPORTED_OPERATION);

        /// <summary><inheritDoc/></summary>
        public bool IsReadOnly => throw new NotSupportedException(UNSUPPORTED_OPERATION);

        /// <summary><inheritDoc/></summary>
        public bool ContainsKey(K key) {
            return map.ContainsKey(key);
        }

        /// <summary><inheritDoc/></summary>
        public void Add(K key, V value) {
            map.Add(key, value);
        }

        /// <summary><inheritDoc/></summary>
        public bool Remove(K key) {
            return map.Remove(key);
        }

        /// <summary><inheritDoc/></summary>
        public bool TryGetValue(K key, out V value) {
            return map.TryGetValue(key, out value);
        }

        /// <summary><inheritDoc/></summary>
        public V this[K key] {
            get => map.GetOrCreateValue(key);
            set => map.Add(key, value);
        }

        /// <summary><inheritDoc/></summary>
        public ICollection<K> Keys => throw new NotSupportedException(UNSUPPORTED_OPERATION);

        /// <summary><inheritDoc/></summary>
        public ICollection<V> Values => throw new NotSupportedException(UNSUPPORTED_OPERATION);
    }
}
