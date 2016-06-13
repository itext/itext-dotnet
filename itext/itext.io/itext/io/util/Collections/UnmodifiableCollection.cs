using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.IO.Util.Collections
{
    internal class UnmodifiableCollection<T> : ICollection<T> {
        private ICollection<T> _collection;

        public UnmodifiableCollection(ICollection<T> collection) {
            this._collection = collection;
        }

        public IEnumerator<T> GetEnumerator() {
            return _collection.GetEnumerator();
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
            return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return _collection.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }
    }
}
