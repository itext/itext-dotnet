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
namespace iText.Kernel.Pdf {
    internal class PdfPages : PdfObjectWrapper<PdfDictionary> {
        private int from;

        private PdfNumber count;

        private readonly PdfArray kids;

        private readonly iText.Kernel.Pdf.PdfPages parent;

        public PdfPages(int from, PdfDocument pdfDocument, iText.Kernel.Pdf.PdfPages parent)
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

        public PdfPages(int from, PdfDocument pdfDocument)
            : this(from, pdfDocument, null) {
        }

        public PdfPages(int from, int maxCount, PdfDictionary pdfObject, iText.Kernel.Pdf.PdfPages parent)
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

        public virtual void AddPage(PdfDictionary page) {
            kids.Add(page);
            IncrementCount();
            page.Put(PdfName.Parent, GetPdfObject());
            page.SetModified();
        }

        public virtual bool AddPage(int index, PdfPage pdfPage) {
            if (index < from || index > from + GetCount()) {
                return false;
            }
            kids.Add(index - from, pdfPage.GetPdfObject());
            pdfPage.GetPdfObject().Put(PdfName.Parent, GetPdfObject());
            pdfPage.SetModified();
            IncrementCount();
            return true;
        }

        public virtual bool RemovePage(int pageNum) {
            if (pageNum < from || pageNum >= from + GetCount()) {
                return false;
            }
            DecrementCount();
            kids.Remove(pageNum - from);
            return true;
        }

        public virtual void AddPages(iText.Kernel.Pdf.PdfPages pdfPages) {
            kids.Add(pdfPages.GetPdfObject());
            count.SetValue(count.IntValue() + pdfPages.GetCount());
            pdfPages.GetPdfObject().Put(PdfName.Parent, GetPdfObject());
            pdfPages.SetModified();
            SetModified();
        }

        // remove empty PdfPage.
        public virtual void RemoveFromParent() {
            if (parent != null) {
                System.Diagnostics.Debug.Assert(GetCount() == 0);
                parent.kids.Remove(GetPdfObject().GetIndirectReference());
                if (parent.GetCount() == 0) {
                    parent.RemoveFromParent();
                }
            }
        }

        public virtual int GetFrom() {
            return from;
        }

        public virtual int GetCount() {
            return count.IntValue();
        }

        public virtual void CorrectFrom(int correction) {
            from += correction;
        }

        public virtual PdfArray GetKids() {
            return GetPdfObject().GetAsArray(PdfName.Kids);
        }

        public virtual iText.Kernel.Pdf.PdfPages GetParent() {
            return parent;
        }

        public virtual void IncrementCount() {
            count.Increment();
            SetModified();
            if (parent != null) {
                parent.IncrementCount();
            }
        }

        public virtual void DecrementCount() {
            count.Decrement();
            SetModified();
            if (parent != null) {
                parent.DecrementCount();
            }
        }

        public virtual int CompareTo(int index) {
            if (index < from) {
                return 1;
            }
            if (index >= from + GetCount()) {
                return -1;
            }
            return 0;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
