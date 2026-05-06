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
using System.Text;

namespace iText.Commons.Utils {
    /// <summary>This class is used to generate placeholder text for the examples and tests.</summary>
    /// <remarks>
    /// This class is used to generate placeholder text for the examples and tests.
    /// It is not used anywhere in the actual code.
    /// This provides a better way than using Lorem Ipsum text as it is more readable.
    /// </remarks>
    public sealed class PlaceHolderTextUtil {
//\cond DO_NOT_DOCUMENT
        internal const String TEMPLATE = "The Portable Document Format (PDF)\n" + "The Portable Document Format, universally known as PDF, was developed by Adobe Systems in the early "
             + "1990s as a solution to a pervasive problem in digital communication: how to share documents that look "
             + "identical regardless of the device, operating system, or software used to view them. Introduced by "
             + "Adobe co-founder John Warnock through his \"Camelot\" project, the format was officially launched in "
             + "1993. Before PDF, sending a formatted document across different computers was a gamble — fonts would "
             + "change, layouts would break, and carefully designed pages would arrive looking nothing like the " 
            + "original. PDF solved this by embedding everything a document needs — fonts, images, vector graphics, "
             + "and layout instructions — into a single, self-contained file.\n" + "At its technical core, a PDF file is a sophisticated container built around a page description "
             + "language derived from PostScript. Each PDF encodes content as a series of objects: text streams, image"
             + " data, font definitions, and geometric paths, all referenced through a cross-reference table that "
             + "allows PDF readers to jump directly to any page without reading the entire file. Modern PDF versions "
             + "support an impressive array of features beyond static text and images, including interactive form "
             + "fields, digital signatures, embedded multimedia, 3D models, layers, and JavaScript-based interactivity"
             + ". This richness makes PDF far more than a simple image of a page — it is a fully structured, " + "programmable document format.\n"
             + "One of PDF's most significant milestones came in 2008, when Adobe released the PDF specification as an"
             + " open standard, handing it over to the International Organization for Standardization. It became ISO "
             + "32000-1, freeing the format from proprietary control and enabling any developer or organization to "
             + "implement PDF support without licensing restrictions. This openness accelerated the proliferation of "
             + "PDF tools across every platform and industry. Alongside the base standard, specialized subsets emerged"
             + " to serve specific needs: PDF/A for long-term archival, PDF/X for professional print production, PDF/E"
             + " for engineering documents, and PDF/UA for universal accessibility — ensuring that the format could "
             + "meet the stringent requirements of governments, courts, publishers, and healthcare systems alike.\n"
             + "PDF has become the backbone of document exchange in virtually every professional field. Legal " + 
            "contracts, academic research papers, government forms, financial reports, and instruction manuals are "
             + "routinely distributed as PDFs because of the format's reliability and near-universal support. Every "
             + "major operating system — Windows, macOS, Linux, iOS, and Android — can open PDFs natively or with "
             + "minimal additional software. The format's ability to lock down content through password protection, "
             + "permission controls, and digital signatures has also made it a trusted medium for sensitive " + "information. Courts accept PDF filings, tax authorities issue PDF forms, and publishers distribute "
             + "entire books in the format, a testament to its extraordinary versatility.\n" + "Despite being over three decades old, PDF continues to evolve. The latest standard, PDF 2.0 (ISO "
             + "32000-2), introduced improvements in encryption, digital signatures, and support for modern color "
             + "spaces and rich media. Tools powered by artificial intelligence can now extract structured data from "
             + "PDFs, recognize text in scanned documents through optical character recognition (OCR), and " + "automatically tag documents for accessibility. Cloud-based services have made it easier than ever to "
             + "create, edit, merge, split, and annotate PDFs from any device. Far from being a legacy format on its "
             + "way out, PDF remains one of the most important and enduring standards in the history of digital " 
            + "computing — a quiet but indispensable pillar of how the world shares information.";
//\endcond

        private PlaceHolderTextUtil() {
        }

        // Empty constructor
        /// <summary>Gets the placeholder text.</summary>
        /// <param name="by">if you want to get the placeholder text by words or by characters</param>
        /// <param name="amount">the number of words or characters</param>
        /// <returns>the placeholder text</returns>
        public static String GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy by, int amount) {
            if (by == PlaceHolderTextUtil.PlaceHolderTextBy.WORDS) {
                return GetPlaceHolderTextByWords(amount);
            }
            else {
                return GetPlaceHolderTextByCharacters(amount);
            }
        }

        private static String GetPlaceHolderTextByWords(int amount) {
            String[] words = iText.Commons.Utils.StringUtil.Split(TEMPLATE, " ");
            int approximateWordLength = 5;
            int heuristic = amount * approximateWordLength;
            StringBuilder sb = new StringBuilder(heuristic);
            for (int i = 0; i < amount; i++) {
                sb.Append(words[i % words.Length]);
                if (i + 1 == amount) {
                    break;
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }

        private static String GetPlaceHolderTextByCharacters(int amount) {
            StringBuilder sb = new StringBuilder(amount);
            for (int i = 0; i < amount; i++) {
                sb.Append(TEMPLATE[i % TEMPLATE.Length]);
            }
            return sb.ToString();
        }

        /// <summary>The enum Place holder text by.</summary>
        /// <remarks>
        /// The enum Place holder text by.
        /// This enum is used to get the placeholder text by words or by characters
        /// </remarks>
        public enum PlaceHolderTextBy {
            WORDS,
            CHARACTERS
        }
    }
}
