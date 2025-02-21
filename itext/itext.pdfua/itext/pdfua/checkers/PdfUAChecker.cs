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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Logs;

namespace iText.Pdfua.Checkers {
    /// <summary>An abstract class that will run through all necessary checks defined in the different PDF/UA standards.
    ///     </summary>
    /// <remarks>
    /// An abstract class that will run through all necessary checks defined in the different PDF/UA standards. A number of
    /// common checks are executed in this class, while standard-dependent specifications are implemented in the available
    /// subclasses. The standard that is followed is the series of ISO 14289 specifications, currently generations 1 and 2.
    /// <para />
    /// While it is possible to subclass this method and implement its abstract methods in client code, this is not
    /// encouraged and will have little effect. It is not possible to plug custom implementations into iText, because
    /// iText should always refuse to create non-compliant PDF/UA, which would be possible with client code implementations.
    /// Any future generations of the PDF/UA standard and its derivatives will get their own implementation in the iText -
    /// pdfua project.
    /// </remarks>
    public abstract class PdfUAChecker : IValidationChecker {
        private bool warnedOnPageFlush = false;

        /// <summary>
        /// Creates new
        /// <see cref="PdfUAChecker"/>
        /// instance.
        /// </summary>
        protected internal PdfUAChecker() {
        }

        // Empty constructor.
        /// <summary>Logs a warn on page flushing that page flushing is disabled in PDF/UA mode.</summary>
        public virtual void WarnOnPageFlush() {
            if (!warnedOnPageFlush) {
                ITextLogManager.GetLogger(typeof(iText.Pdfua.Checkers.PdfUAChecker)).LogWarning(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED
                    );
                warnedOnPageFlush = true;
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks that the
        /// <c>ViewerPreferences</c>
        /// dictionary of the document catalog dictionary is present and contains
        /// at least the
        /// <c>DisplayDocTitle</c>
        /// key with a value of
        /// <see langword="true"/>
        /// , as defined in
        /// ISO 32000-1:2008, 12.2, Table 150 or ISO 32000-2:2020, Table 147.
        /// </summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        internal virtual void CheckViewerPreferences(PdfCatalog catalog) {
            PdfDictionary viewerPreferences = catalog.GetPdfObject().GetAsDictionary(PdfName.ViewerPreferences);
            if (viewerPreferences == null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MISSING_VIEWER_PREFERENCES);
            }
            PdfObject displayDocTitle = viewerPreferences.Get(PdfName.DisplayDocTitle);
            if (!(displayDocTitle is PdfBoolean)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MISSING_VIEWER_PREFERENCES);
            }
            if (PdfBoolean.FALSE.Equals(displayDocTitle)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.VIEWER_PREFERENCES_IS_FALSE);
            }
        }
//\endcond

        public abstract bool IsPdfObjectReadyToFlush(PdfObject arg1);

        public abstract void Validate(IValidationContext arg1);
    }
}
