/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
