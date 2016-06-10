using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iTextSharp.IO.Util.Collections
{
    internal class EmptySet<T> : ISet<T> {
        public IEnumerator<T> GetEnumerator() {
            yield break;
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
            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return !other.Any();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return false;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return true;
        }

        public bool Overlaps(IEnumerable<T> other) {
            return false;
        }

        public bool SetEquals(IEnumerable<T> other) {
            return !other.Any();
        }

        bool ISet<T>.Add(T item) {
            throw new InvalidOperationException();
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
    }
}
