using System;
using System.Collections.Generic;

namespace iText.Kernel.Pdf {
    internal class PdfDictionaryValues : AbstractCollection<PdfObject> {
        private readonly ICollection<PdfObject> collection;

        internal PdfDictionaryValues(ICollection<PdfObject> collection) {
            this.collection = collection;
        }

        public override bool Add(PdfObject @object) {
            return collection.Add(@object);
        }

        public override bool Contains(Object o) {
            if (collection.Contains(o)) {
                return true;
            }
            if (o != null) {
                if (((PdfObject)o).GetIndirectReference() != null && collection.Contains(((PdfObject)o).GetIndirectReference
                    ())) {
                    return true;
                }
                else {
                    if (((PdfObject)o).IsIndirectReference() && collection.Contains(((PdfIndirectReference)o).GetRefersTo())) {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Remove(Object o) {
            if (collection.Remove(o)) {
                return true;
            }
            if (o != null) {
                if (((PdfObject)o).GetIndirectReference() != null && collection.Remove(((PdfObject)o).GetIndirectReference
                    ())) {
                    return true;
                }
                else {
                    if (((PdfObject)o).IsIndirectReference() && collection.Remove(((PdfIndirectReference)o).GetRefersTo())) {
                        return true;
                    }
                }
            }
            return false;
        }

        public override int Count {
            get {
                return collection.Count;
            }
        }

        public override void Clear() {
            collection.Clear();
        }

        public override IEnumerator<PdfObject> Iterator() {
            return new PdfDictionaryValues.DirectIterator(this);
        }

        private class DirectIterator : IEnumerator<PdfObject> {
            internal IEnumerator<PdfObject> parentIterator = this._enclosing.collection.Iterator();

            public override bool HasNext() {
                return this.parentIterator.HasNext();
            }

            public override PdfObject Next() {
                PdfObject obj = this.parentIterator.Next();
                if (obj.IsIndirectReference()) {
                    obj = ((PdfIndirectReference)obj).GetRefersTo(true);
                }
                return obj;
            }

            public override void Remove() {
                this.parentIterator.Remove();
            }

            internal DirectIterator(PdfDictionaryValues _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly PdfDictionaryValues _enclosing;
        }
    }
}
