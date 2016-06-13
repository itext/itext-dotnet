using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.IO.Util.Collections
{
    internal class EmptyList<T> : IList<T> {
        public IEnumerator<T> GetEnumerator() {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            yield break;
        }

        public void Add(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
        }

        public bool Contains(T item) {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex) {
        }

        public bool Remove(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return 0; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public int IndexOf(T item) {
            return -1;
        }

        public void Insert(int index, T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void RemoveAt(int index) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public T this[int index] {
            get { throw new IndexOutOfRangeException(); }
            set { throw new NotSupportedException("Collection is read-only."); }
        }
    }
}
