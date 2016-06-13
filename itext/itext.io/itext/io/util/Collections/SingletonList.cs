using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.IO.Util.Collections
{
    internal class SingletonList<T> : IList<T> {
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

        public void Clear() {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Contains(T item) {
            return (element == null && item == null || element != null && element.Equals(item));
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

        public int IndexOf(T item) {
            if (Contains(item)) {
                return 0;
            }
            return -1;
        }

        public void Insert(int index, T item) {
            throw new InvalidOperationException();
        }

        public void RemoveAt(int index) {
            throw new NotSupportedException("Collection is read-only.");
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
}
