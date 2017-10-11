using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    internal class BorderStyleUtil {
        private BorderStyleUtil() {
        }

        /// <summary>Setter for the border style.</summary>
        /// <remarks>
        /// Setter for the border style. Possible values are
        /// <ul>
        /// <li>
        /// <see cref="PdfAnnotation.STYLE_SOLID"/>
        /// - A solid rectangle surrounding the annotation.</li>
        /// <li>
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// - A dashed rectangle surrounding the annotation.</li>
        /// <li>
        /// <see cref="PdfAnnotation.STYLE_BEVELED"/>
        /// - A simulated embossed rectangle that appears to be raised above the surface of the page.</li>
        /// <li>
        /// <see cref="PdfAnnotation.STYLE_INSET"/>
        /// - A simulated engraved rectangle that appears to be recessed below the surface of the page.</li>
        /// <li>
        /// <see cref="PdfAnnotation.STYLE_UNDERLINE"/>
        /// - A single line along the bottom of the annotation rectangle.</li>
        /// </ul>
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
        /// <see cref="SetStyle(iText.Kernel.Pdf.PdfDictionary, iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// See ISO-320001 8.4.3.6, “Line Dash Pattern” for the format in which dash pattern shall be specified.
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
