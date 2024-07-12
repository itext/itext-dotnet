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
using System.Linq;

namespace iText.Commons.Utils.Collections
{
    //\cond DO_NOT_DOCUMENT 
    internal class EmptySet<T> : ISet<T> {
        public IEnumerator<T> GetEnumerator() {
            yield break;
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
            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return !other.Any();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return false;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return true;
        }

        public bool Overlaps(IEnumerable<T> other) {
            return false;
        }

        public bool SetEquals(IEnumerable<T> other) {
            return !other.Any();
        }

        bool ISet<T>.Add(T item) {
            throw new InvalidOperationException();
        }

        public void Clear() {
        }

        public bool Contains(T item) {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex) {
        }

        public bool Remove(T item) {
            // According to .NET this method should throw NotSupportedException, if IsReadOnly is true.
            // However this collection is generally intended to be used in context of autoportable Java code, 
            // and in Java analogous method simply returns false.
            return false;
        }

        public int Count {
            get { return 0; }
        }

        public bool IsReadOnly {
            get { return true; }
        }
    }
   //\endcond 
}
