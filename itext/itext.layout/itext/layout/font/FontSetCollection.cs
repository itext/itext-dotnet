/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
