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
using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Commons.Utils.Collections
{
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
