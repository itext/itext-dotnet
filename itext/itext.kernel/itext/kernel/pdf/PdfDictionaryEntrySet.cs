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

namespace iText.Kernel.Pdf {
    internal class PdfDictionaryEntrySet : ICollection<KeyValuePair<PdfName, PdfObject>>
    {
        private readonly ICollection<KeyValuePair<PdfName, PdfObject>> collection;

        internal PdfDictionaryEntrySet(ICollection<KeyValuePair<PdfName, PdfObject>> collection) {
            this.collection = collection;
        }

        public IEnumerator<KeyValuePair<PdfName, PdfObject>> GetEnumerator()
        {
            return new DirectEnumerator(collection.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<PdfName, PdfObject> item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(KeyValuePair<PdfName, PdfObject> item)
        {
            if (collection.Contains(item))
                return true;
            if (item.Value == null)
                return false;
            foreach (KeyValuePair<PdfName, PdfObject> entry in this)
                if (item.Key.Equals(entry.Key) && item.Value.Equals(entry.Value))
                    return true;
            return false;
        }

        public void CopyTo(KeyValuePair<PdfName, PdfObject>[] array, int arrayIndex)
        {
            int count = collection.Count;
            if (count == 0)
                return;
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "arrayIndex mut not be negtive");
            if (arrayIndex >= array.Length || count > array.Length - arrayIndex)
                throw new ArgumentException("arrayIndex", "array too small");

            int index = arrayIndex;
            foreach (KeyValuePair<PdfName, PdfObject> item in this)
            {
                if (--count < 0)
                    break;
                array[index++] = item;
            }
        }

        public bool Remove(KeyValuePair<PdfName, PdfObject> item)
        {
            if (collection.Remove(item))
                return true;
            if (item.Value == null)
                return false;
            KeyValuePair<PdfName, PdfObject> toDelete = new KeyValuePair<PdfName, PdfObject>(null, null);
            foreach (KeyValuePair<PdfName, PdfObject> entry in collection)
                if (item.Key.Equals(entry.Key) && PdfObject.EqualContent(item.Value, entry.Value))
                {
                    toDelete = entry;
                    break;
                }

            return toDelete.Key != null && collection.Remove(toDelete);
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return collection.IsReadOnly; }
        }

        private class DirectEnumerator : IEnumerator<KeyValuePair<PdfName, PdfObject>>
        {
            private IEnumerator<KeyValuePair<PdfName, PdfObject>> parentEnumerator;

            public DirectEnumerator(IEnumerator<KeyValuePair<PdfName, PdfObject>> parentEnumerator)
            {
                this.parentEnumerator = parentEnumerator;
            }

            public void Dispose()
            {
                parentEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (parentEnumerator.MoveNext())
                {
                    KeyValuePair<PdfName, PdfObject> current = parentEnumerator.Current;
                    if (current.Value != null && current.Value.IsIndirectReference())
                    {
                        current = new KeyValuePair<PdfName, PdfObject>(current.Key, ((PdfIndirectReference)current.Value).GetRefersTo(true));
                    }
                    Current = current;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                parentEnumerator.Reset();
            }

            public KeyValuePair<PdfName, PdfObject> Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
