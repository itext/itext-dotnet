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
using System.IO;
using iText.IO.Source;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    internal class PdfObjectStream : PdfStream {
        /// <summary>Max number of objects in object stream.</summary>
        public const int MAX_OBJ_STREAM_SIZE = 200;

        /// <summary>Current object stream size (number of objects inside).</summary>
        protected internal PdfNumber size = new PdfNumber(0);

        /// <summary>Stream containing object indices, a heading part of object stream.</summary>
        protected internal PdfOutputStream indexStream;

        public PdfObjectStream(PdfDocument doc)
            : this(doc, new ByteArrayOutputStream()) {
            indexStream = new PdfOutputStream(new ByteArrayOutputStream());
        }

        /// <summary>This constructor is for reusing ByteArrayOutputStreams of indexStream and outputStream.</summary>
        /// <remarks>
        /// This constructor is for reusing ByteArrayOutputStreams of indexStream and outputStream.
        /// NOTE Only for internal use in PdfWriter!
        /// </remarks>
        /// <param name="prev">previous PdfObjectStream.</param>
        internal PdfObjectStream(iText.Kernel.Pdf.PdfObjectStream prev)
            : this(prev.GetIndirectReference().GetDocument(), prev.GetOutputStream().GetOutputStream()) {
            indexStream = new PdfOutputStream(prev.indexStream.GetOutputStream());
            ((ByteArrayOutputStream)outputStream.GetOutputStream()).JReset();
            ((ByteArrayOutputStream)indexStream.GetOutputStream()).JReset();
            prev.ReleaseContent(true);
        }

        private PdfObjectStream(PdfDocument doc, Stream outputStream)
            : base(outputStream) {
            //avoid reuse existed references, create new, opposite to get next reference
            MakeIndirect(doc, doc.GetXref().CreateNewIndirectReference(doc));
            GetOutputStream().document = doc;
            Put(PdfName.Type, PdfName.ObjStm);
            Put(PdfName.N, size);
            Put(PdfName.First, new PdfNumber(0));
        }

        /// <summary>Adds object to the object stream.</summary>
        /// <param name="object">object to add.</param>
        public virtual void AddObject(PdfObject @object) {
            if (size.IntValue() == MAX_OBJ_STREAM_SIZE) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_OBJECT_STREAM_REACH_MAX_SIZE);
            }
            PdfOutputStream outputStream = GetOutputStream();
            indexStream.WriteInteger(@object.GetIndirectReference().GetObjNumber()).WriteSpace().WriteLong(outputStream
                .GetCurrentPos()).WriteSpace();
            outputStream.Write(@object);
            @object.GetIndirectReference().SetObjStreamNumber(GetIndirectReference().GetObjNumber());
            @object.GetIndirectReference().SetIndex(size.IntValue());
            outputStream.WriteSpace();
            size.Increment();
            GetAsNumber(PdfName.First).SetValue(indexStream.GetCurrentPos());
        }

        /// <summary>Gets object stream size (number of objects inside).</summary>
        /// <returns>object stream size.</returns>
        public virtual int GetSize() {
            return size.IntValue();
        }

        public virtual PdfOutputStream GetIndexStream() {
            return indexStream;
        }

        protected internal override void ReleaseContent() {
            ReleaseContent(false);
        }

        private void ReleaseContent(bool close) {
            if (close) {
                outputStream = null;
                indexStream = null;
                base.ReleaseContent();
            }
        }
    }
}
