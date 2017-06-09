using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Layout.Font {
    public class FontSetCollection : ICollection<FontInfo> {
        private ICollection<FontInfo> primary;
        private ICollection<FontInfo> temporary;

        public FontSetCollection(ICollection<FontInfo> primary, ICollection<FontInfo> temporary) {
            this.primary = primary;
            this.temporary = temporary;
        }

        public IEnumerator<FontInfo> GetEnumerator() {
            return new FontSetCollectionEnumerator(primary, temporary);
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
            get { return primary.Count + (temporary != null ? temporary.Count : 0); }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        private class FontSetCollectionEnumerator : IEnumerator<FontInfo> {
            private IEnumerator<FontInfo> primary;
            private IEnumerator<FontInfo> temporary;
            private IEnumerator<FontInfo> currentIterator;
            private bool isPrimary = true;


            public FontSetCollectionEnumerator(ICollection<FontInfo> primary, ICollection<FontInfo> temporary) {
                this.primary = primary.GetEnumerator();
                this.temporary = temporary != null ? temporary.GetEnumerator() : null;
                this.currentIterator = this.primary;
            }

            public void Dispose() {
                primary.Dispose();
                if (temporary != null) temporary.Dispose();
            }

            public bool MoveNext() {
                if (currentIterator.MoveNext()) {
                    return true;
                }
                if (isPrimary && temporary != null) {
                    isPrimary = false;
                    currentIterator = temporary;
                    return currentIterator.MoveNext();
                }
                return false;
            }

            public void Reset() {
                primary.Reset();
                if (temporary != null) temporary.Reset();
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