using System.Collections;
using System.Collections.Generic;

namespace iText.Kernel.Pdf {
    internal class PdfArrayDirectIterator : IEnumerator<PdfObject>
    {
        private IEnumerator<PdfObject> parentEnumerator;

        internal PdfArrayDirectIterator(IEnumerator<PdfObject> parentEnumerator)
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
                PdfObject obj = parentEnumerator.Current;
                if (obj.IsIndirectReference())
                {
                    obj = ((PdfIndirectReference)obj).GetRefersTo(true);
                }
                Current = obj;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            parentEnumerator.Reset();
        }

        public PdfObject Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
