/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.Commons.Utils.Collections
{
    //\cond DO_NOT_DOCUMENT 
    internal class SingletonList<T> : IList<T>, IList {
        private readonly T element;

        public SingletonList(T obj) {
            this.element = obj;
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

        public int Add(object value)
        {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Contains(object value)
        {
            return (element == null && value == null || element != null && element.Equals(value));
        }
        
        public bool Contains(T item) {
            return Contains((object)item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            array[arrayIndex] = element;
        }
        
        public void CopyTo(Array array, int index)
        {
            array.SetValue(element, index);
        }

        public bool Remove(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }
        
        public void Remove(object value)
        {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return 1; }
        }

        public object SyncRoot
        {
            get { return this; }
        }
        public bool IsSynchronized
        {
            get { return true; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            return IndexOf((object) item);
        }
        
        public int IndexOf(object value)
        {
            return Contains(value) ? 0 : -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Insert(int index, T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void RemoveAt(int index) {
            throw new NotSupportedException("Collection is read-only.");
        }

        object IList.this[int index]
        {
            get
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }

                return element;
            }
            set { throw new NotSupportedException("Collection is read-only."); }
        }

        public T this[int index] {
            get {
                if (index != 0) {
                    throw new IndexOutOfRangeException();
                }
                return element;
            }
            set { throw new NotSupportedException("Collection is read-only."); }
        }
    }
   //\endcond 
}
