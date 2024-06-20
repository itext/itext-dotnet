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

namespace iText.Commons.Utils.Collections {
    //\cond DO_NOT_DOCUMENT 
    internal class SingletonSet<T> : ISet<T> {
        private T element;

        public SingletonSet(T element) {
            this.element = element;
        }

        public IEnumerator<T> GetEnumerator() {
            yield return element;
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
            return other.Contains(element);
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return SetEquals(other) || !other.Any();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            bool equalFound = false;
            bool notEqualFound = false;
            foreach (T e in other) {
                bool elemEq = elementsEqual(element, e);
                equalFound = equalFound || elemEq;
                notEqualFound = notEqualFound || !elemEq;
                
                if (equalFound && notEqualFound) {
                    return true;
                }
            }
            return false;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return !other.Any();
        }

        public bool Overlaps(IEnumerable<T> other) {
            return other.Contains(element);
        }

        public bool SetEquals(IEnumerable<T> other) {
            bool first = true;
            bool equals = false;
            foreach (T e in other) {
                equals = first && elementsEqual(element, e);
                if (!first) {
                    break;
                }

                first = false;
            }

            return equals;
        }

        bool ISet<T>.Add(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Contains(T item) {
            return elementsEqual(element, item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            array[arrayIndex] = element;
        }

        public bool Remove(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return 1; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        private bool elementsEqual(T one, T another) {
            return one == null && another == null || one != null && one.Equals(another);
        }
    }
   //\endcond 
}
