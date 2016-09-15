using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.Kernel.Pdf {

    internal class PdfDictionaryValues : ICollection<PdfObject> {
        private readonly ICollection<PdfObject> collection;

        internal PdfDictionaryValues(ICollection<PdfObject> collection) {
            this.collection = collection;
        }

        public void Add(PdfObject item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(PdfObject item)
        {
            if (collection.Contains(item))
                return true;
            if (item == null)
                return false;
            foreach (PdfObject pdfObject in this)
            {
                if (PdfObject.EqualContent(item, pdfObject))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(PdfObject[] array, int arrayIndex)
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
            foreach (PdfObject item in this)
            {
                if (--count < 0)
                    break;
                array[index++] = item;
            }
        }

        public bool Remove(PdfObject item)
        {
            //Actually we will get exception:
            // System.NotSupportedException : Mutating a value collection derived from a dictionary is not allowed.
            return collection.Remove(item);
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return collection.IsReadOnly; }
        }


        public IEnumerator<PdfObject> GetEnumerator()
        {
            return new DirectEnumerator(collection.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DirectEnumerator : IEnumerator<PdfObject>
        {
            private IEnumerator<PdfObject> parentEnumerator;

            public DirectEnumerator(IEnumerator<PdfObject> parentEnumerator)
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
                    if (obj != null && obj.IsIndirectReference())
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
}
