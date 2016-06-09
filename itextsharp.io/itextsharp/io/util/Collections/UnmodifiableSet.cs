using System;
using System.Collections;
using System.Collections.Generic;

namespace iTextSharp.IO.Util.Collections
{
    internal class UnmodifiableSet<T> : ISet<T> {
        private ISet<T> _set;

        public UnmodifiableSet(ISet<T> set) {
            this._set = set;
        }

        public IEnumerator<T> GetEnumerator() {
            return _set.GetEnumerator();
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
            return _set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return _set.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return _set.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return _set.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other) {
            return _set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other) {
            return _set.SetEquals(other);
        }

        bool ISet<T>.Add(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public void Clear() {
            throw new NotSupportedException("Collection is read-only.");
        }

        public bool Contains(T item) {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _set.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            throw new NotSupportedException("Collection is read-only.");
        }

        public int Count {
            get { return _set.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }
    }
}
