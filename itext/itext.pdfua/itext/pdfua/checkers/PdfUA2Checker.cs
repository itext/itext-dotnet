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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils.Checkers;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;
using iText.Layout.Validation.Context;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Ua2;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers {
    /// <summary>
    /// The class defines the requirements of the PDF/UA-2 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// The class defines the requirements of the PDF/UA-2 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// <para />
    /// The specification implemented by this class is ISO 14289-2.
    /// </remarks>
    public class PdfUA2Checker : PdfUAChecker {
        private readonly PdfDocument pdfDocument;

        private readonly PdfUAValidationContext context;

        private readonly PdfUA2HeadingsChecker headingsChecker;

        /// <summary>
        /// Creates
        /// <see cref="PdfUA2Checker"/>
        /// instance with PDF document which will be validated against PDF/UA-2 standard.
        /// </summary>
        /// <param name="pdfDocument">the document to validate</param>
        public PdfUA2Checker(PdfDocument pdfDocument)
            : base() {
            this.pdfDocument = pdfDocument;
            this.context = new PdfUAValidationContext(this.pdfDocument);
            this.headingsChecker = new PdfUA2HeadingsChecker(this.context);
        }

        public override void Validate(IValidationContext context) {
            switch (context.GetType()) {
                case ValidationType.PDF_DOCUMENT: {
                    PdfDocumentValidationContext pdfDocContext = (PdfDocumentValidationContext)context;
                    CheckCatalog(pdfDocContext.GetPdfDocument().GetCatalog());
                    CheckStructureTreeRoot(pdfDocContext.GetPdfDocument().GetStructTreeRoot());
                    break;
                }

                case ValidationType.LAYOUT: {
                    LayoutValidationContext layoutContext = (LayoutValidationContext)context;
                    headingsChecker.CheckLayoutElement(layoutContext.GetRenderer());
                    break;
                }
            }
        }

        public override bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return true;
        }

        /// <summary>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file contains the
        /// <c>Metadata</c>
        /// key whose value is
        /// a metadata stream as defined in ISO 32000-2:2020.
        /// </summary>
        /// <remarks>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file contains the
        /// <c>Metadata</c>
        /// key whose value is
        /// a metadata stream as defined in ISO 32000-2:2020. Also checks that the value of
        /// <c>pdfuaid:part</c>
        /// is 2 for
        /// conforming PDF files and validates required
        /// <c>pdfuaid:rev</c>
        /// value.
        /// <para />
        /// Checks that the
        /// <c>Metadata</c>
        /// stream as specified in ISO 32000-2:2020, 14.3 in the document catalog dictionary
        /// includes a
        /// <c>dc: title</c>
        /// entry reflecting the title of the document.
        /// </remarks>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        protected internal virtual void CheckMetadata(PdfCatalog catalog) {
            PdfCheckersUtil.CheckMetadata(catalog.GetPdfObject(), PdfConformance.PDF_UA_2, EXCEPTION_SUPPLIER);
            try {
                XMPMeta metadata = catalog.GetDocument().GetXmpMetadata();
                if (metadata.GetProperty(XMPConst.NS_DC, XMPConst.TITLE) == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY);
                }
            }
            catch (XMPException e) {
                throw new PdfUAConformanceException(e.Message);
            }
        }

        /// <summary>Validates document catalog dictionary against PDF/UA-2 standard.</summary>
        /// <remarks>
        /// Validates document catalog dictionary against PDF/UA-2 standard.
        /// <para />
        /// For now, only
        /// <c>Metadata</c>
        /// and
        /// <c>ViewerPreferences</c>
        /// are checked.
        /// </remarks>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary to check
        /// </param>
        private void CheckCatalog(PdfCatalog catalog) {
            CheckLang(catalog);
            CheckMetadata(catalog);
            CheckViewerPreferences(catalog);
        }

        /// <summary>Validates structure tree root dictionary against PDF/UA-2 standard.</summary>
        /// <remarks>
        /// Validates structure tree root dictionary against PDF/UA-2 standard.
        /// <para />
        /// For now, only headings check is performed.
        /// </remarks>
        /// <param name="structTreeRoot">
        /// 
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructTreeRoot"/>
        /// structure tree root dictionary to check
        /// </param>
        private void CheckStructureTreeRoot(PdfStructTreeRoot structTreeRoot) {
            TagTreeIterator tagTreeIterator = new TagTreeIterator(structTreeRoot);
            tagTreeIterator.AddHandler(new PdfUA2HeadingsChecker.PdfUA2HeadingHandler(context));
            tagTreeIterator.Traverse();
        }
    }
}
