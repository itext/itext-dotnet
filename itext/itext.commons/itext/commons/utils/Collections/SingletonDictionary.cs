using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Commons.Utils.Collections
{
    public class SingletonDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IDictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        public SingletonDictionary(TKey key, TValue value)
        {
            _dict.Add(key, value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _dict).GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Dictionary is read-only.");
        }

        public void Clear()
        {
            throw new NotSupportedException("Dictionary is read-only.");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Dictionary is read-only.");
        }

        public int Count => _dict.Count;

        public bool IsReadOnly => true;

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("Dictionary is read-only.");
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException("Dictionary is read-only.");
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _dict[key];
            set => throw new NotSupportedException("Dictionary is read-only.");
        }

        public ICollection<TKey> Keys => _dict.Keys;

        public ICollection<TValue> Values => _dict.Values;
    }
}