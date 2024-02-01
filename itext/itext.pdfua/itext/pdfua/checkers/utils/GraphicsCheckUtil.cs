/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Class that provides methods for checking PDF/UA compliance of graphics elements.</summary>
    public sealed class GraphicsCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="GraphicsCheckUtil"/>
        /// instance.
        /// </summary>
        private GraphicsCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if image has alternative description or actual text.</summary>
        /// <param name="image">image to check</param>
        public static void CheckLayoutImage(Image image) {
            if (image.GetAccessibilityProperties() == null) {
                throw new InvalidOperationException();
            }
            if (!StandardRoles.FIGURE.Equals(image.GetAccessibilityProperties().GetRole())) {
                // image is not a figure tag, so we don't need to check it
                return;
            }
            AccessibilityProperties props = image.GetAccessibilityProperties();
            bool hasSomeValue = HasAtleastOneValidValue(props.GetAlternateDescription(), props.GetActualText());
            if (!hasSomeValue) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT);
            }
        }

        /// <summary>Checks if figure tag has alternative description or actual text.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Tagutils.ITagTreeIteratorHandler"/>
        /// handler implementation that checks if figure tag has alternative
        /// description or actual text
        /// </returns>
        public static ITagTreeIteratorHandler CreateFigureTagHandler() {
            return new _ITagTreeIteratorHandler_79();
        }

        private sealed class _ITagTreeIteratorHandler_79 : ITagTreeIteratorHandler {
            public _ITagTreeIteratorHandler_79() {
            }

            public void NextElement(IStructureNode elem) {
                if (!PdfName.Figure.Equals(elem.GetRole())) {
                    return;
                }
                // we only need to check struct elems, not MCR numbers as they don't contain any useful info
                if (!(elem is PdfStructElem)) {
                    return;
                }
                PdfStructElem structElem = ((PdfStructElem)elem);
                PdfDictionary pdfObject = structElem.GetPdfObject();
                if (!iText.Pdfua.Checkers.Utils.GraphicsCheckUtil.HasAtleastOneValidValue(pdfObject.GetAsString(PdfName.Alt
                    ), pdfObject.GetAsString(PdfName.ActualText))) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT);
                }
            }
        }

        private static bool HasAtleastOneValidValue(Object altText, Object actualText) {
            String altTextValue = null;
            if (altText is PdfString) {
                altTextValue = ((PdfString)altText).GetValue();
            }
            String actualTextValue = null;
            if (actualText is PdfString) {
                actualTextValue = ((PdfString)actualText).GetValue();
            }
            if (altText is String) {
                altTextValue = (String)altText;
            }
            if (actualText is String) {
                actualTextValue = (String)actualText;
            }
            // PDF spec is not super clear, but it seems actualText can be an empty string
            return !(altTextValue == null || String.IsNullOrEmpty(altTextValue)) || actualTextValue != null;
        }
    }
}
