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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    internal class BorderStyleUtil {
        private BorderStyleUtil() {
        }

        /// <summary>Setter for the border style.</summary>
        /// <remarks>
        /// Setter for the border style. Possible values are
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_SOLID"/>
        /// - A solid rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// - A dashed rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_BEVELED"/>
        /// - A simulated embossed rectangle that appears to be raised above the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_INSET"/>
        /// - A simulated engraved rectangle that appears to be recessed below the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_UNDERLINE"/>
        /// - A single line along the bottom of the annotation rectangle.
        /// </description></item>
        /// </list>
        /// See also ISO-320001, Table 166.
        /// </remarks>
        /// <param name="bs">original border style dictionary.</param>
        /// <param name="style">The new value for the annotation's border style.</param>
        /// <returns>Updated border style dictionary entry.</returns>
        public static PdfDictionary SetStyle(PdfDictionary bs, PdfName style) {
            if (null == bs) {
                bs = new PdfDictionary();
            }
            bs.Put(PdfName.S, style);
            return bs;
        }

        /// <summary>Setter for the dashed border style.</summary>
        /// <remarks>
        /// Setter for the dashed border style. This property has affect only if
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// style was used for border style (see
        /// <see cref="SetStyle(iText.Kernel.Pdf.PdfDictionary, iText.Kernel.Pdf.PdfName)"/>.
        /// See ISO-320001 8.4.3.6, "Line Dash Pattern" for the format in which dash pattern shall be specified.
        /// </remarks>
        /// <param name="bs">original border style dictionary.</param>
        /// <param name="dashPattern">
        /// a dash array defining a pattern of dashes and gaps that
        /// shall be used in drawing a dashed border.
        /// </param>
        /// <returns>Updated border style dictionary entry.</returns>
        public static PdfDictionary SetDashPattern(PdfDictionary bs, PdfArray dashPattern) {
            if (null == bs) {
                bs = new PdfDictionary();
            }
            bs.Put(PdfName.D, dashPattern);
            return bs;
        }
    }
}
