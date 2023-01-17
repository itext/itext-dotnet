/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Commons.Utils.Collections
{
    internal class UnmodifiableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        private IDictionary<TKey, TValue> _dict;

        public UnmodifiableDictionary(IDictionary<TKey, TValue> backingDict) {
            _dict = backingDict;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return _dict.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool ContainsKey(TKey key) {
            return _dict.ContainsKey(key);
        }

        public void Add(TKey key, TValue value) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Remove(TKey key) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _dict.TryGetValue(key, out value);
        }

        public TValue this[TKey key] {
            get { return _dict[key]; }
            set { throw new NotSupportedException("Collection is read-only."); }
        }

        public ICollection<TKey> Keys {
            get { return _dict.Keys; }
        }

        public ICollection<TValue> Values {
            get { return _dict.Values; }
        }
    }
}
