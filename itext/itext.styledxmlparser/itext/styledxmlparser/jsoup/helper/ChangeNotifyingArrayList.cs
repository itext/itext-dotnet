/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
    Authors: iText Software.

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

namespace iText.StyledXmlParser.Jsoup.Helper {
    /// <summary>Implementation of ArrayList that watches out for changes to the contents.</summary>
    public abstract class ChangeNotifyingArrayList<T> : IList<T> {
        
        private List<T> innerList;
        
        public ChangeNotifyingArrayList(int initialCapacity) {
            innerList = new List<T>(initialCapacity);
        }

        public abstract void OnContentsChanged();

        public void Add(T item)
        {
            OnContentsChanged();
            innerList.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            OnContentsChanged();
            innerList.AddRange(collection);
        }

        public void Clear()
        {
            OnContentsChanged();
            innerList.Clear();
        }

        public bool Contains(T item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            OnContentsChanged();
            return innerList.Remove(item);
        }

        public int Count => innerList.Count;

        public bool IsReadOnly => false;

        public int RemoveAll(Predicate<T> match)
        {
            OnContentsChanged();
            return innerList.RemoveAll(match);
        }

        public void RemoveRange(int index, int count)
        {
            OnContentsChanged();
            innerList.RemoveRange(index, count);
        }

        public int IndexOf(T item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            OnContentsChanged();
            innerList.Insert(index, item);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            OnContentsChanged();
            innerList.InsertRange(index, collection);
        }

        public void RemoveAt(int index)
        {
            OnContentsChanged();
            innerList.RemoveAt(index);
        }

        public T this[int index]
        {
            get => innerList[index];

            set
            {
                OnContentsChanged();
                innerList[index] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) innerList).GetEnumerator();
        }
    }
}
