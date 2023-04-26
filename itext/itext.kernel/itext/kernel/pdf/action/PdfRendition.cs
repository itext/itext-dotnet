/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;

namespace iText.Kernel.Pdf.Action {
    /// <summary>This a wrapper around a rendition dictionary.</summary>
    /// <remarks>This a wrapper around a rendition dictionary. See ISO 32000-1 sections 13.2.3.2, 13.2.3.3.</remarks>
    public class PdfRendition : PdfObjectWrapper<PdfDictionary> {
        /// <summary>
        /// Creates a new wrapper around an existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// </summary>
        /// <param name="pdfObject">a rendition object to create a wrapper for</param>
        public PdfRendition(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Creates a new wrapper around a newly created media rendition dictionary object.</summary>
        /// <param name="file">a text string specifying the name of the file to display</param>
        /// <param name="fs">a file specification that specifies the actual media data</param>
        /// <param name="mimeType">an ASCII string identifying the type of data</param>
        public PdfRendition(String file, PdfFileSpec fs, String mimeType)
            : this(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.S, PdfName.MR);
            GetPdfObject().Put(PdfName.N, new PdfString(MessageFormatUtil.Format("Rendition for {0}", file)));
            GetPdfObject().Put(PdfName.C, new PdfMediaClipData(file, fs, mimeType).GetPdfObject());
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
