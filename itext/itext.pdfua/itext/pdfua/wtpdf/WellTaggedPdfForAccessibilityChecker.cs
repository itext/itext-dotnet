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
using iText.Kernel.Pdf;
using iText.Kernel.Utils.Checkers;
using iText.Kernel.XMP;
using iText.Pdfua.Checkers;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Wtpdf {
    /// <summary>Checker that validates a PDF document against the Well Tagged PDF for Accessibility standard.</summary>
    public class WellTaggedPdfForAccessibilityChecker : PdfUA2Checker {
        /// <summary>
        /// Creates a new
        /// <see cref="WellTaggedPdfForAccessibilityChecker"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="WellTaggedPdfForAccessibilityChecker"/>
        /// instance.
        /// <para />
        /// The created checker will validate the provided document against the
        /// WTPDF for Accessibility standard.
        /// </remarks>
        /// <param name="pdfDocument">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to validate; must not be
        /// <see langword="null"/>
        /// </param>
        public WellTaggedPdfForAccessibilityChecker(PdfDocument pdfDocument)
            : base(pdfDocument) {
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
        /// a metadata stream as defined in ISO 32000-2:2020.
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
        protected internal override void CheckMetadata(PdfCatalog catalog) {
            PdfCheckersUtil.CheckMetadata(catalog.GetPdfObject(), PdfConformance.WELL_TAGGED_PDF_FOR_ACCESSIBILITY, (msg
                ) => new PdfUAConformanceException(msg));
            try {
                XMPMeta metadata = catalog.GetDocument().GetXmpMetadata();
                if (metadata.GetProperty(XMPConst.NS_DC, XMPConst.TITLE) == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY);
                }
            }
            catch (XMPException e) {
                throw new PdfUAConformanceException(e.Message, e);
            }
        }
    }
}
