/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Commons.Utils.Collections
{
	//\cond DO_NOT_DOCUMENT	
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
   //\endcond 
}
