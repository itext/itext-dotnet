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
    internal class UnmodifiableSet<T> : ISet<T> {
        private ISet<T> _set;

        public UnmodifiableSet(ISet<T> set) {
            this._set = set;
        }

        public IEnumerator<T> GetEnumerator() {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void UnionWith(IEnumerable<T> other) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void IntersectWith(IEnumerable<T> other) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void ExceptWith(IEnumerable<T> other) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void SymmetricExceptWith(IEnumerable<T> other) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            return _set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return _set.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return _set.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return _set.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other) {
            return _set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other) {
            return _set.SetEquals(other);
        }

        bool ISet<T>.Add(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Contains(T item) {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _set.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return _set.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }
    }
}
