using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Kernel.Pdf {
    internal class PdfDictionaryEntrySet : AbstractSet<KeyValuePair<PdfName, PdfObject>> {
        private readonly ICollection<KeyValuePair<PdfName, PdfObject>> set;

        internal PdfDictionaryEntrySet(ICollection<KeyValuePair<PdfName, PdfObject>> set) {
            this.set = set;
        }

        public override IEnumerator<KeyValuePair<PdfName, PdfObject>> Iterator() {
            return new PdfDictionaryEntrySet.DirectIterator(this);
        }

        public override int Count {
            get {
                return set.Count;
            }
        }

        public override void Clear() {
            set.Clear();
        }

        private class DirectIterator : IEnumerator<KeyValuePair<PdfName, PdfObject>> {
            internal IEnumerator<KeyValuePair<PdfName, PdfObject>> parentIterator = this._enclosing.set.Iterator();

            public override bool HasNext() {
                return this.parentIterator.HasNext();
            }

            public override KeyValuePair<PdfName, PdfObject> Next() {
                return new PdfDictionaryEntrySet.DirectEntry(this, this.parentIterator.Next());
            }

            public override void Remove() {
                this.parentIterator.Remove();
            }

            internal DirectIterator(PdfDictionaryEntrySet _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly PdfDictionaryEntrySet _enclosing;
        }

        private class DirectEntry : KeyValuePair<PdfName, PdfObject> {
            internal KeyValuePair<PdfName, PdfObject> entry;

            public DirectEntry(PdfDictionaryEntrySet _enclosing, KeyValuePair<PdfName, PdfObject> entry) {
                this._enclosing = _enclosing;
                this.entry = entry;
            }

            public virtual PdfName Key {
                get {
                    return this.entry.Key;
                }
            }

            public virtual PdfObject Value {
                get {
                    PdfObject obj = this.entry.Value;
                    if (obj.IsIndirectReference()) {
                        obj = ((PdfIndirectReference)obj).GetRefersTo(true);
                    }
                    return obj;
                }
            }

            public virtual PdfObject SetValue(PdfObject value) {
                return this.entry.SetValue(value);
            }

            public override bool Equals(Object o) {
                if (!(o is DictionaryEntry)) {
                    return false;
                }
                DictionaryEntry e = (DictionaryEntry)o;
                Object k1 = this.Key;
                Object k2 = e.Key;
                if (k1 != null && k1.Equals(k2)) {
                    Object v1 = this.Value;
                    Object v2 = e.Value;
                    if (v1 != null && v1.Equals(v2)) {
                        return true;
                    }
                }
                return false;
            }

            public override int GetHashCode() {
                return Objects.HashCode(this.Key) ^ Objects.HashCode(this.Value);
            }

            private readonly PdfDictionaryEntrySet _enclosing;
        }
    }
}
