using System;
using System.Collections;
using System.Collections.Generic;

namespace itextsharp.io.util.Collections {
    internal class UnmodifiableList : IList {
        private IList _list;

        public UnmodifiableList(IList list) {
			this._list = list;
        }

		public bool IsFixedSize {
			get { return true; }
		}

		public bool IsReadOnly {
			get { return true; }
		}

		public object this[int index] {
			get { return _list[index]; }
			set { throw new NotSupportedException("Collection is read-only."); }
		}

		public int Add(object value) {
			throw new NotSupportedException("Collection is read-only.");
		}

		public void Clear() {
			throw new NotSupportedException("Collection is read-only.");
		}

		public bool Contains(object value) {
			return _list.Contains(value);
		}

		public int IndexOf(object value) {
			return _list.IndexOf(value);
		}

		public void Insert(int index, object value) {
			throw new NotSupportedException("Collection is read-only.");
		}

		public void Remove(object value) {
			throw new NotSupportedException("Collection is read-only.");
		}

		public void RemoveAt(int index) {
			throw new NotSupportedException("Collection is read-only.");
		}
			
		public int Count {
			get { return _list.Count; }
		}

		public bool IsSynchronized {
			get { return _list.IsSynchronized; }
		}

		public object SyncRoot {
			get { return _list.SyncRoot; }
		}
			
		public void CopyTo(Array array, int index) {
			_list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator() {
			return _list.GetEnumerator();
		}
    }
}
