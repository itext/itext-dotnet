/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
namespace iText.Kernel.Pdf {
//\cond DO_NOT_DOCUMENT
    internal class PdfPages : PdfObjectWrapper<PdfDictionary> {
        private int from;

        private PdfNumber count;

        private readonly PdfArray kids;

        private readonly iText.Kernel.Pdf.PdfPages parent;

//\cond DO_NOT_DOCUMENT
        internal PdfPages(int from, PdfDocument pdfDocument, iText.Kernel.Pdf.PdfPages parent)
            : base(new PdfDictionary()) {
            if (pdfDocument.GetWriter() != null) {
                GetPdfObject().MakeIndirect(pdfDocument);
            }
            SetForbidRelease();
            this.from = from;
            this.count = new PdfNumber(0);
            this.kids = new PdfArray();
            this.parent = parent;
            GetPdfObject().Put(PdfName.Type, PdfName.Pages);
            GetPdfObject().Put(PdfName.Kids, this.kids);
            GetPdfObject().Put(PdfName.Count, this.count);
            if (parent != null) {
                GetPdfObject().Put(PdfName.Parent, this.parent.GetPdfObject());
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal PdfPages(int from, PdfDocument pdfDocument)
            : this(from, pdfDocument, null) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal PdfPages(int from, int maxCount, PdfDictionary pdfObject, iText.Kernel.Pdf.PdfPages parent)
            : base(pdfObject) {
            SetForbidRelease();
            this.from = from;
            this.count = pdfObject.GetAsNumber(PdfName.Count);
            this.parent = parent;
            if (this.count == null) {
                this.count = new PdfNumber(1);
                pdfObject.Put(PdfName.Count, this.count);
            }
            else {
                if (maxCount < this.count.IntValue()) {
                    this.count.SetValue(maxCount);
                }
            }
            this.kids = pdfObject.GetAsArray(PdfName.Kids);
            pdfObject.Put(PdfName.Type, PdfName.Pages);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddPage(PdfDictionary page) {
            kids.Add(page);
            IncrementCount();
            page.Put(PdfName.Parent, GetPdfObject());
            page.SetModified();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool AddPage(int index, PdfPage pdfPage) {
            if (index < from || index > from + GetCount()) {
                return false;
            }
            kids.Add(index - from, pdfPage.GetPdfObject());
            pdfPage.GetPdfObject().Put(PdfName.Parent, GetPdfObject());
            pdfPage.SetModified();
            IncrementCount();
            return true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool RemovePage(int pageNum) {
            if (pageNum < from || pageNum >= from + GetCount()) {
                return false;
            }
            DecrementCount();
            kids.Remove(pageNum - from);
            return true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddPages(iText.Kernel.Pdf.PdfPages pdfPages) {
            kids.Add(pdfPages.GetPdfObject());
            count.SetValue(count.IntValue() + pdfPages.GetCount());
            pdfPages.GetPdfObject().Put(PdfName.Parent, GetPdfObject());
            pdfPages.SetModified();
            SetModified();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // remove empty PdfPage.
        internal virtual void RemoveFromParent() {
            if (parent != null) {
                System.Diagnostics.Debug.Assert(GetCount() == 0);
                parent.kids.Remove(GetPdfObject().GetIndirectReference());
                if (parent.GetCount() == 0) {
                    parent.RemoveFromParent();
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetFrom() {
            return from;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetCount() {
            return count.IntValue();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void CorrectFrom(int correction) {
            from += correction;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PdfArray GetKids() {
            return GetPdfObject().GetAsArray(PdfName.Kids);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.Kernel.Pdf.PdfPages GetParent() {
            return parent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void IncrementCount() {
            count.Increment();
            SetModified();
            if (parent != null) {
                parent.IncrementCount();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void DecrementCount() {
            count.Decrement();
            SetModified();
            if (parent != null) {
                parent.DecrementCount();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int CompareTo(int index) {
            if (index < from) {
                return 1;
            }
            if (index >= from + GetCount()) {
                return -1;
            }
            return 0;
        }
//\endcond

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
//\endcond
}
