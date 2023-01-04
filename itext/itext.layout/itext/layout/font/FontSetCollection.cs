/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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

namespace iText.Layout.Font {
    public class FontSetCollection : ICollection<FontInfo> {
        private ICollection<FontInfo> primary;
        private ICollection<FontInfo> additional;

        public FontSetCollection(ICollection<FontInfo> primary, ICollection<FontInfo> additional) {
            this.primary = primary;
            this.additional = additional;
        }

        public IEnumerator<FontInfo> GetEnumerator() {
            return new FontSetCollectionEnumerator(primary, additional);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(FontInfo item) {
            throw new NotSupportedException();
        }

        public void Clear() {
            throw new NotSupportedException();
        }

        public bool Contains(FontInfo item) {
            foreach (FontInfo fontInfo in this) {
                if (fontInfo.Equals(item)) return true;
            }
            return false;
        }

        public void CopyTo(FontInfo[] array, int arrayIndex) {
            foreach (FontInfo fontInfo in this) {
                array[arrayIndex++] = fontInfo;
            }
        }

        public bool Remove(FontInfo item) {
            throw new NotSupportedException();
        }

        public int Count {
            get { return primary.Count + (additional != null ? additional.Count : 0); }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        private class FontSetCollectionEnumerator : IEnumerator<FontInfo> {
            private IEnumerator<FontInfo> primary;
            private IEnumerator<FontInfo> additional;
            private IEnumerator<FontInfo> currentIterator;
            private bool isPrimary = true;


            public FontSetCollectionEnumerator(ICollection<FontInfo> primary, ICollection<FontInfo> temporary) {
                this.primary = primary.GetEnumerator();
                this.additional = temporary != null ? temporary.GetEnumerator() : null;
                this.currentIterator = this.primary;
            }

            public void Dispose() {
                primary.Dispose();
                if (additional != null) additional.Dispose();
            }

            public bool MoveNext() {
                if (currentIterator.MoveNext()) {
                    return true;
                }
                if (isPrimary && additional != null) {
                    isPrimary = false;
                    currentIterator = additional;
                    return currentIterator.MoveNext();
                }
                return false;
            }

            public void Reset() {
                primary.Reset();
                if (additional != null) additional.Reset();
                currentIterator = primary;
                isPrimary = true;
            }

            public FontInfo Current {
                get { return currentIterator.Current; }
            }

            object IEnumerator.Current {
                get { return Current; }
            }
        }
    }
}
