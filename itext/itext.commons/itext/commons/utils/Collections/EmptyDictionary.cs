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
using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Commons.Utils.Collections {
    //\cond DO_NOT_DOCUMENT
    internal class EmptyDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return 0; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool ContainsKey(TKey key) {
            return false;
        }

        public void Add(TKey key, TValue value) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Remove(TKey key) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool TryGetValue(TKey key, out TValue value) {
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key] {
            get { throw new IndexOutOfRangeException(); }
            set { throw new NotSupportedException("Collection is read-only."); }
        }

        public ICollection<TKey> Keys {
            get { return new EmptySet<TKey>(); }
        }

        public ICollection<TValue> Values {
            get { return new EmptySet<TValue>(); }
        }
    }
    //\endcond
}
