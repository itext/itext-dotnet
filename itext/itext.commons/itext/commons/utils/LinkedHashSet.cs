/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iText.Commons.Utils {
    public class LinkedHashSet<TKey> : ISet<TKey> {
        private readonly IDictionary<TKey, LinkedListNode<TKey>> _map = new Dictionary<TKey, LinkedListNode<TKey>>();
        private readonly LinkedList<TKey> _list = new LinkedList<TKey>();

        public LinkedHashSet() {
        } 

        public LinkedHashSet(ICollection<TKey> c) {
            this.AddAll(c);
        } 

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

        public int Count
        {
            get { return _map.Count; }
        }

        public bool IsReadOnly
        {
            get { return _map.IsReadOnly; }
        }
    }
}
