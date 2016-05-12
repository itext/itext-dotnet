using System;
using System.Collections;
using System.Collections.Generic;

namespace itextsharp.io.util.Collections {
    internal class EmptyDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return 0; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool ContainsKey(TKey key) {
            return false;
        }

        public void Add(TKey key, TValue value) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Remove(TKey key) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool TryGetValue(TKey key, out TValue value) {
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key] {
            get { throw new IndexOutOfRangeException(); }
            set { throw new NotSupportedException("Collection is read-only."); }
        }

        public ICollection<TKey> Keys {
            get { return new EmptySet<TKey>(); }
        }

        public ICollection<TValue> Values {
            get { return new EmptySet<TValue>(); }
        }
    }
}
