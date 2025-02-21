/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;

namespace iText.Kernel.Validation {
    /// <summary>Class that will run through all necessary checks defined in the PDF 2.0 standard.</summary>
    /// <remarks>
    /// Class that will run through all necessary checks defined in the PDF 2.0 standard. The standard that is followed is
    /// the series of ISO 32000 specifications, starting from ISO 32000-2:2020.
    /// </remarks>
    public class Pdf20Checker : IValidationChecker {
        /// <summary>
        /// Creates new
        /// <see cref="Pdf20Checker"/>
        /// instance to validate PDF document against PDF 2.0 standard.
        /// </summary>
        public Pdf20Checker() {
        }

        // Empty constructor.
        public virtual void Validate(IValidationContext validationContext) {
            switch (validationContext.GetType()) {
                case ValidationType.PDF_DOCUMENT: {
                    PdfDocumentValidationContext pdfDocContext = (PdfDocumentValidationContext)validationContext;
                    CheckCatalog(pdfDocContext.GetPdfDocument().GetCatalog());
                    break;
                }
            }
        }

        public virtual bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return true;
        }

        /// <summary>Validates document catalog dictionary against PDF 2.0 standard.</summary>
        /// <remarks>
        /// Validates document catalog dictionary against PDF 2.0 standard.
        /// <para />
        /// For now, only
        /// <c>Metadata</c>
        /// is checked.
        /// </remarks>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary to check
        /// </param>
        private void CheckCatalog(PdfCatalog catalog) {
            CheckMetadata(catalog);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks that the value of the
        /// <c>Metadata</c>
        /// key from the
        /// <c>Catalog</c>
        /// dictionary of a conforming file
        /// is a metadata stream as defined in ISO 32000-2:2020.
        /// </summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        internal virtual void CheckMetadata(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            if (!catalogDict.ContainsKey(PdfName.Metadata)) {
                return;
            }
            try {
                XMPMeta metadata = catalog.GetDocument().GetXmpMetadata();
                if (metadata == null) {
                    throw new Pdf20ConformanceException(KernelExceptionMessageConstant.INVALID_METADATA_VALUE);
                }
                PdfStream pdfStream = catalogDict.GetAsStream(PdfName.Metadata);
                PdfName type = pdfStream.GetAsName(PdfName.Type);
                PdfName subtype = pdfStream.GetAsName(PdfName.Subtype);
                if (!PdfName.Metadata.Equals(type) || !PdfName.XML.Equals(subtype)) {
                    throw new Pdf20ConformanceException(KernelExceptionMessageConstant.METADATA_STREAM_REQUIRES_METADATA_TYPE_AND_XML_SUBTYPE
                        );
                }
            }
            catch (XMPException e) {
                throw new Pdf20ConformanceException(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e);
            }
        }
//\endcond
    }
}
