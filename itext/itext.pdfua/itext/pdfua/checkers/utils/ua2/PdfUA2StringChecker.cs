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
using System.Collections.Generic;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs UA-2 checks related to PdfString objects.</summary>
    public sealed class PdfUA2StringChecker {
        private PdfUA2StringChecker() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>Checks PdfString object to be UA-2 compatible.</summary>
        /// <param name="string">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// to be checked
        /// </param>
        public static void CheckPdfString(PdfString @string) {
            // Only perform this check if PdfString is text string (intended to be human-readable).
            if (PdfEncodings.PDF_DOC_ENCODING.Equals(@string.GetEncoding()) ||
                PdfEncodings.UTF8.Equals(@string.GetEncoding()) ||
                PdfEncodings.UNICODE_BIG.Equals(@string.GetEncoding())) {
                if (StringContainsPua(@string.GetValue())) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.TEXT_STRING_USES_UNICODE_PUA);
                }
            }
        }

        internal static bool StringContainsPua(string @string) {
            if (@string != null) {
                IList<int> characterCodePoints = getCharacterCodePoints(@string);
                for (int i = 0; i < characterCodePoints.Count; ++i) {
                    int code = characterCodePoints[i];
                    bool isPrivateArea = code >= 0xE000 && code <= 0xF8FF;
                    bool isSupplementaryPrivateAreaA = code >= 0xF0000 && code <= 0xFFFFD;
                    bool isSupplementaryPrivateAreaB = code >= 0x100000 && code <= 0x10FFFD;
                    if (isPrivateArea || isSupplementaryPrivateAreaA || isSupplementaryPrivateAreaB) {
                        return true;
                    }
                }
            }
            return false;
        }

        private static IList<int> getCharacterCodePoints(string s) {
            IList<int> codePoints = new List<int>();
            for (int i = 0; i < s.Length; i += char.IsSurrogatePair(s, i) ? 2 : 1) {
                int codepoint = char.ConvertToUtf32(s, i);
                codePoints.Add(codepoint);
            }
            return codePoints;
        }
    }
}