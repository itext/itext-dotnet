using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.IO.Util.Collections
{
    internal class UnmodifiableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        private IDictionary<TKey, TValue> _dict;

        public UnmodifiableDictionary(IDictionary<TKey, TValue> backingDict) {
            _dict = backingDict;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return _dict.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool ContainsKey(TKey key) {
            return _dict.ContainsKey(key);
        }

        public void Add(TKey key, TValue value) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Remove(TKey key) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _dict.TryGetValue(key, out value);
        }

        public TValue this[TKey key] {
            get { return _dict[key]; }
            set { throw new NotSupportedException("Collection is read-only."); }
        }

        public ICollection<TKey> Keys {
            get { return _dict.Keys; }
        }

        public ICollection<TValue> Values {
            get { return _dict.Values; }
        }
    }
}
