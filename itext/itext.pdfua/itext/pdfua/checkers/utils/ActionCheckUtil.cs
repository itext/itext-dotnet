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
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Class that provides methods for checking PDF/UA compliance of actions.</summary>
    public class ActionCheckUtil {
        private ActionCheckUtil() {
        }

        // Empty constructor.
        /// <summary>Check PDF/UA compliance of an action</summary>
        /// <param name="action">action to check</param>
        public static void CheckAction(PdfDictionary action) {
            if (action == null) {
                return;
            }
            PdfName s = action.GetAsName(PdfName.S);
            PdfDictionary rendition = action.GetAsDictionary(PdfName.R);
            if (PdfName.Rendition.Equals(s) && rendition != null) {
                CheckRenditionMedia(rendition.GetAsDictionary(PdfName.BE) != null ? rendition.GetAsDictionary(PdfName.BE).
                    GetAsDictionary(PdfName.C) : null);
                CheckRenditionMedia(rendition.GetAsDictionary(PdfName.MH) != null ? rendition.GetAsDictionary(PdfName.MH).
                    GetAsDictionary(PdfName.C) : null);
                CheckRenditionMedia(rendition.GetAsDictionary(PdfName.C));
            }
        }

        private static void CheckRenditionMedia(PdfDictionary mediaClipDict) {
            if (mediaClipDict != null && (mediaClipDict.Get(PdfName.CT) == null || mediaClipDict.Get(PdfName.Alt) == null
                )) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP
                    );
            }
        }
    }
}
