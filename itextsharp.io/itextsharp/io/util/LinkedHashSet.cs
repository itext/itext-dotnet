using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace io.itextsharp.io.util {
    public class LinkedHashSet<TKey> : ISet<TKey> {
        private readonly IDictionary<TKey, LinkedListNode<TKey>> _map = new Dictionary<TKey, LinkedListNode<TKey>>();
        private readonly LinkedList<TKey> _list = new LinkedList<TKey>();

        public IEnumerator<TKey> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(TKey item) {
            ((ISet<TKey>) this).Add(item);
        }

        public void UnionWith(IEnumerable<TKey> other) {
            foreach (TKey item in other) {
                Add(item);
            }
        }

        public void IntersectWith(IEnumerable<TKey> other) {
            foreach (TKey item in other) {
                if (!Contains(item)) {
                    Remove(item);
                }
            }
        }

        public void ExceptWith(IEnumerable<TKey> other) {
            foreach (TKey item in other) {
                Remove(item);
            }
        }

        public void SymmetricExceptWith(IEnumerable<TKey> other) {
            foreach (TKey item in other) {
                if (Contains(item)) {
                    Remove(item);
                }
            }
        }

        public bool IsSubsetOf(IEnumerable<TKey> other) {
            return this.All(other.Contains);
        }

        public bool IsSupersetOf(IEnumerable<TKey> other) {
            return other.All(this.Contains);
        }

        public bool IsProperSupersetOf(IEnumerable<TKey> other) {
            int cnt = 0;
            foreach (TKey item in other) {
                if (!Contains(item)) {
                    return false;
                }
                cnt++;
            }
            return cnt == 0 || Count > cnt;
        }

        public bool IsProperSubsetOf(IEnumerable<TKey> other) {
            int otherCount = 0;
            foreach (TKey item in other) {
                otherCount++;
                if (otherCount > Count) {
                    break;
                }
            }
            return otherCount > Count && this.All(other.Contains);
        }

        public bool Overlaps(IEnumerable<TKey> other) {
            return other.Any(Contains);
        }

        public bool SetEquals(IEnumerable<TKey> other) {
            int otherCount = 0;
            foreach (TKey item in other) {
                if (!Contains(item)) {
                    return false;
                }
                otherCount++;
            }
            return Count == otherCount;
        }

        bool ISet<TKey>.Add(TKey item) {
            if (!_map.ContainsKey(item)) {
                LinkedListNode<TKey> node = _list.AddLast(item);
                _map.Add(item, node);
                return true;
            }
            return false;
        }

        public void Clear() {
            _map.Clear();
            _list.Clear();
        }

        public bool Contains(TKey item) {
            return _map.ContainsKey(item);
        }

        public void CopyTo(TKey[] array, int arrayIndex) {
            foreach (TKey item in _list) {
                array.SetValue(item, arrayIndex++);
            }
        }

        public bool Remove(TKey item) {
            LinkedListNode<TKey> node;
            if (_map.TryGetValue(item, out node)) {
                _map.Remove(item);
                _list.Remove(node);
                return true;
            }
            return false;
        }

        public int Count => _map.Count;

        public bool IsReadOnly => _map.IsReadOnly;
    }
}
