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
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    public class PdfIndirectReference : PdfObject, IComparable<iText.Kernel.Pdf.PdfIndirectReference> {
        private const int LENGTH_OF_INDIRECTS_CHAIN = 31;

        /// <summary>Object number.</summary>
        protected internal readonly int objNr;

        /// <summary>Object generation.</summary>
        protected internal int genNr;

        /// <summary>PdfObject that current PdfIndirectReference instance refers to.</summary>
        protected internal PdfObject refersTo = null;

        /// <summary>Indirect reference number of object stream containing refersTo object.</summary>
        /// <remarks>
        /// Indirect reference number of object stream containing refersTo object.
        /// If refersTo is not placed into object stream - objectStreamNumber = 0.
        /// </remarks>
        protected internal int objectStreamNumber = 0;

        /// <summary>
        /// Offset in a document of the
        /// <c>refersTo</c>
        /// object.
        /// </summary>
        /// <remarks>
        /// Offset in a document of the
        /// <c>refersTo</c>
        /// object.
        /// If the object placed into object stream then it is an object index inside object stream.
        /// </remarks>
        protected internal long offsetOrIndex = 0;

        /// <summary>PdfDocument object belongs to.</summary>
        /// <remarks>PdfDocument object belongs to. For direct objects it is null.</remarks>
        protected internal PdfDocument pdfDocument = null;

        protected internal PdfIndirectReference(PdfDocument doc, int objNr)
            : this(doc, objNr, 0) {
        }

        protected internal PdfIndirectReference(PdfDocument doc, int objNr, int genNr)
            : base() {
            this.pdfDocument = doc;
            this.objNr = objNr;
            this.genNr = genNr;
        }

        protected internal PdfIndirectReference(PdfDocument doc, int objNr, int genNr, long offset)
            : base() {
            this.pdfDocument = doc;
            this.objNr = objNr;
            this.genNr = genNr;
            this.offsetOrIndex = offset;
            System.Diagnostics.Debug.Assert(offset >= 0);
        }

        public virtual int GetObjNumber() {
            return objNr;
        }

        public virtual int GetGenNumber() {
            return genNr;
        }

        public virtual PdfObject GetRefersTo() {
            return GetRefersTo(true);
        }

        /// <summary>Gets direct object and try to resolve indirects chain.</summary>
        /// <remarks>
        /// Gets direct object and try to resolve indirects chain.
        /// <para />
        /// Note: If chain of references has length of more than 32,
        /// this method return 31st reference in chain.
        /// </remarks>
        /// <param name="recursively">
        /// 
        /// <see langword="true"/>
        /// to resolve indirects chain
        /// </param>
        /// <returns>
        /// the
        /// <see cref="PdfObject"/>
        /// result of indirect reference resolving
        /// </returns>
        public virtual PdfObject GetRefersTo(bool recursively) {
            if (!recursively) {
                if (refersTo == null && !CheckState(FLUSHED) && !CheckState(MODIFIED) && !CheckState(FREE) && GetReader() 
                    != null) {
                    refersTo = GetReader().ReadObject(this);
                }
                return refersTo;
            }
            else {
                PdfObject currentRefersTo = GetRefersTo(false);
                for (int i = 0; i < LENGTH_OF_INDIRECTS_CHAIN; i++) {
                    if (currentRefersTo is iText.Kernel.Pdf.PdfIndirectReference) {
                        currentRefersTo = ((iText.Kernel.Pdf.PdfIndirectReference)currentRefersTo).GetRefersTo(false);
                    }
                    else {
                        break;
                    }
                }
                return currentRefersTo;
            }
        }

        protected internal virtual void SetRefersTo(PdfObject refersTo) {
            this.refersTo = refersTo;
        }

        public virtual int GetObjStreamNumber() {
            return objectStreamNumber;
        }

        /// <summary>Gets refersTo object offset in a document.</summary>
        /// <returns>object offset in a document. If refersTo object is in object stream then -1.</returns>
        public virtual long GetOffset() {
            return objectStreamNumber == 0 ? offsetOrIndex : -1;
        }

        /// <summary>Gets refersTo object index in the object stream.</summary>
        /// <returns>object index in a document. If refersTo object is not in object stream then -1.</returns>
        public virtual int GetIndex() {
            return objectStreamNumber == 0 ? -1 : (int)offsetOrIndex;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Pdf.PdfIndirectReference that = (iText.Kernel.Pdf.PdfIndirectReference)o;
            bool documentsEquals = pdfDocument == that.pdfDocument;
            if (!documentsEquals) {
                documentsEquals = pdfDocument != null && that.pdfDocument != null && pdfDocument.GetDocumentId() == that.pdfDocument
                    .GetDocumentId();
            }
            return objNr == that.objNr && genNr == that.genNr && documentsEquals;
        }

        public override int GetHashCode() {
            int result = objNr;
            result = 31 * result + genNr;
            if (pdfDocument != null) {
                result = 31 * result + (int)pdfDocument.GetDocumentId();
            }
            return result;
        }

        public virtual int CompareTo(iText.Kernel.Pdf.PdfIndirectReference o) {
            if (objNr == o.objNr) {
                if (genNr == o.genNr) {
                    return ComparePdfDocumentLinks(o);
                }
                return (genNr > o.genNr) ? 1 : -1;
            }
            return (objNr > o.objNr) ? 1 : -1;
        }

        public override byte GetObjectType() {
            return INDIRECT_REFERENCE;
        }

        public virtual PdfDocument GetDocument() {
            return pdfDocument;
        }

        /// <summary>Marks indirect reference as free in the document.</summary>
        /// <remarks>
        /// Marks indirect reference as free in the document. This doesn't "remove" indirect objects from the document,
        /// it only ensures that corresponding xref entry is free and indirect object referred by this reference is no longer
        /// linked to it. Actual object still might be written to the resultant document (and would get a new corresponding
        /// indirect reference in this case) if it is still contained in some other object.
        /// <para />
        /// This method will not give any result if the corresponding indirect object or another object
        /// that contains a reference to this object is already flushed.
        /// <para />
        /// Note: in some cases, removing a link of indirect object to it's indirect reference while
        /// leaving the actual object in the document structure might lead to errors, because some objects are expected
        /// to always have such explicit link (e.g. Catalog object, page objects, etc).
        /// </remarks>
        public virtual void SetFree() {
            GetDocument().GetXref().FreeReference(this);
        }

        /// <summary>
        /// Checks if this
        /// <see cref="PdfIndirectReference"/>
        /// instance corresponds to free indirect reference.
        /// </summary>
        /// <remarks>
        /// Checks if this
        /// <see cref="PdfIndirectReference"/>
        /// instance corresponds to free indirect reference.
        /// Indirect reference might be in a free state either because it was read as such from the opened existing
        /// PDF document or because it was set free via
        /// <see cref="SetFree()"/>
        /// method.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this
        /// <see cref="PdfIndirectReference"/>
        /// is free,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IsFree() {
            return CheckState(FREE);
        }

        public override String ToString() {
            StringBuilder states = new StringBuilder(" ");
            if (CheckState(FREE)) {
                states.Append("Free; ");
            }
            if (CheckState(MODIFIED)) {
                states.Append("Modified; ");
            }
            if (CheckState(MUST_BE_FLUSHED)) {
                states.Append("MustBeFlushed; ");
            }
            if (CheckState(READING)) {
                states.Append("Reading; ");
            }
            if (CheckState(FLUSHED)) {
                states.Append("Flushed; ");
            }
            if (CheckState(ORIGINAL_OBJECT_STREAM)) {
                states.Append("OriginalObjectStream; ");
            }
            if (CheckState(FORBID_RELEASE)) {
                states.Append("ForbidRelease; ");
            }
            if (CheckState(READ_ONLY)) {
                states.Append("ReadOnly; ");
            }
            return MessageFormatUtil.Format("{0} {1} R{2}", JavaUtil.IntegerToString(GetObjNumber()), JavaUtil.IntegerToString
                (GetGenNumber()), states.JSubstring(0, states.Length - 1));
        }

        /// <summary>Gets a PdfWriter associated with the document object belongs to.</summary>
        /// <returns>PdfWriter.</returns>
        protected internal virtual PdfWriter GetWriter() {
            if (GetDocument() != null) {
                return GetDocument().GetWriter();
            }
            return null;
        }

        /// <summary>Gets a PdfReader associated with the document object belongs to.</summary>
        /// <returns>PdfReader.</returns>
        protected internal virtual PdfReader GetReader() {
            if (GetDocument() != null) {
                return GetDocument().GetReader();
            }
            return null;
        }

        protected internal override PdfObject NewInstance() {
            return PdfNull.PDF_NULL;
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document) {
        }

        /// <summary>Sets special states of current object.</summary>
        /// <param name="state">special flag of current object</param>
        protected internal override PdfObject SetState(short state) {
            return base.SetState(state);
        }

        internal virtual void SetObjStreamNumber(int objectStreamNumber) {
            this.objectStreamNumber = objectStreamNumber;
        }

        internal virtual void SetIndex(long index) {
            this.offsetOrIndex = index;
        }

        internal virtual void SetOffset(long offset) {
            this.offsetOrIndex = offset;
            this.objectStreamNumber = 0;
        }

        internal virtual void FixOffset(long offset) {
            if (!IsFree()) {
                this.offsetOrIndex = offset;
            }
        }

        private int ComparePdfDocumentLinks(iText.Kernel.Pdf.PdfIndirectReference toCompare) {
            if (pdfDocument == toCompare.pdfDocument) {
                return 0;
            }
            else {
                if (pdfDocument == null) {
                    return -1;
                }
                else {
                    if (toCompare.pdfDocument == null) {
                        return 1;
                    }
                    else {
                        long thisDocumentId = pdfDocument.GetDocumentId();
                        long documentIdToCompare = toCompare.pdfDocument.GetDocumentId();
                        if (thisDocumentId == documentIdToCompare) {
                            return 0;
                        }
                        return (thisDocumentId > documentIdToCompare) ? 1 : -1;
                    }
                }
            }
        }
    }
}
