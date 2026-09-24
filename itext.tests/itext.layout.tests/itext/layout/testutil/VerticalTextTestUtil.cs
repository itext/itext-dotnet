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
using System;
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils.Collections;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace iText.Layout.Testutil {
    public sealed class VerticalTextTestUtil {
        private VerticalTextTestUtil() {
        }

        /// <summary>Extracts all text content from page 1 of the given PDF file.</summary>
        /// <param name="outFileName">path to the PDF file to extract text from</param>
        /// <returns>
        /// the extracted text, as returned by
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy"/>
        /// </returns>
        public static String ExtractPageText(String outFileName) {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(outFileName))) {
                return PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new LocationTextExtractionStrategy());
            }
        }

        /// <summary>Counts occurrences of each non-whitespace character in the given text.</summary>
        /// <param name="text">the text to count characters in</param>
        /// <returns>a map of character to occurrence count, excluding whitespace characters</returns>
        public static IDictionary<char, int?> CountNonWhitespaceChars(String text) {
            IDictionary<char, int?> counts = new Dictionary<char, int?>();
            for (int i = 0; i < text.Length; i++) {
                char c = text[i];
                if (!iText.IO.Util.TextUtil.IsWhiteSpace(c)) {
                    int? currentCount = counts.Get(c);
                    if (currentCount == null) {
                        counts.Put(c, 1);
                    }
                    else {
                        counts.Put(c, currentCount + 1);
                    }
                }
            }
            return counts;
        }

        /// <summary>
        /// Checks whether every non-whitespace character in
        /// <paramref name="expected"/>
        /// occurs in
        /// <paramref name="extractedCounts"/>
        /// at least as many times as it occurs in
        /// <paramref name="expected"/>.
        /// </summary>
        /// <remarks>
        /// Checks whether every non-whitespace character in
        /// <paramref name="expected"/>
        /// occurs in
        /// <paramref name="extractedCounts"/>
        /// at least as many times as it occurs in
        /// <paramref name="expected"/>
        /// . Whitespace and character order are ignored,
        /// so this is a multiset containment check rather than a substring or exact-equality check.
        /// </remarks>
        /// <param name="extractedCounts">
        /// character occurrence counts of the extracted page text,
        /// as produced by
        /// <see cref="CountNonWhitespaceChars(System.String)"/>
        /// </param>
        /// <param name="expected">the text whose characters are expected to be present</param>
        /// <returns>
        /// true if all non-whitespace characters of
        /// <paramref name="expected"/>
        /// are present with sufficient count
        /// </returns>
        public static bool ContainsAllCharacters(IDictionary<char, int?> extractedCounts, String expected) {
            foreach (KeyValuePair<char, int?> entry in CountNonWhitespaceChars(expected)) {
                if (extractedCounts.GetOrDefault(entry.Key, 0) < entry.Value) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Extracts text from page 1 of the given PDF file and counts occurrences of each non-whitespace
        /// character in it.
        /// </summary>
        /// <remarks>
        /// Extracts text from page 1 of the given PDF file and counts occurrences of each non-whitespace
        /// character in it. Equivalent to
        /// <c>countNonWhitespaceChars(extractPageText(outFileName))</c>.
        /// </remarks>
        /// <param name="outFileName">path to the PDF file to extract text from</param>
        /// <returns>a map of character to occurrence count, excluding whitespace characters</returns>
        public static IDictionary<char, int?> ExtractPageCharacterCounts(String outFileName) {
            return CountNonWhitespaceChars(ExtractPageText(outFileName));
        }

        /// <summary>
        /// Checks whether every non-whitespace character in
        /// <paramref name="expected"/>
        /// occurs in
        /// <paramref name="extractedCounts"/>
        /// at least
        /// <paramref name="multiplier"/>
        /// times as often as it occurs in
        /// <paramref name="expected"/>.
        /// </summary>
        /// <remarks>
        /// Checks whether every non-whitespace character in
        /// <paramref name="expected"/>
        /// occurs in
        /// <paramref name="extractedCounts"/>
        /// at least
        /// <paramref name="multiplier"/>
        /// times as often as it occurs in
        /// <paramref name="expected"/>
        /// . Used to verify text
        /// appears a specific number of times (e.g. once per font in a side-by-side comparison), while
        /// remaining robust to the same reading-order caveats as
        /// <see cref="ContainsAllCharacters(System.Collections.Generic.IDictionary{K, V}, System.String)"/>.
        /// </remarks>
        /// <param name="extractedCounts">
        /// character occurrence counts of the extracted page text,
        /// as produced by
        /// <see cref="CountNonWhitespaceChars(System.String)"/>
        /// </param>
        /// <param name="expected">the text whose characters are expected to be present</param>
        /// <param name="multiplier">
        /// how many times each character of
        /// <paramref name="expected"/>
        /// is expected to occur
        /// </param>
        /// <returns>
        /// true if all non-whitespace characters of
        /// <paramref name="expected"/>
        /// are present with sufficient count
        /// </returns>
        public static bool ContainsAllCharacters(IDictionary<char, int?> extractedCounts, String expected, int multiplier
            ) {
            foreach (KeyValuePair<char, int?> entry in CountNonWhitespaceChars(expected)) {
                if (extractedCounts.GetOrDefault(entry.Key, 0) < entry.Value * multiplier) {
                    return false;
                }
            }
            return true;
        }
    }
}
