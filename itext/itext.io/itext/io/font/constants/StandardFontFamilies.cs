using System;

namespace iText.IO.Font.Constants {
    /// <summary>
    /// Class containing families for
    /// <see cref="StandardFonts"/>
    /// .
    /// This class was made for
    /// <see cref="iText.IO.Font.FontRegisterProvider"/>
    /// .
    /// </summary>
    public sealed class StandardFontFamilies {
        private StandardFontFamilies() {
        }

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.COURIER"/>
        /// ,
        /// <see cref="StandardFonts.COURIER_BOLD"/>
        /// ,
        /// <see cref="StandardFonts.COURIER_OBLIQUE"/>
        /// and
        /// <see cref="StandardFonts.COURIER_BOLDOBLIQUE"/>
        /// .
        /// </summary>
        public const String COURIER = "Courier";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.HELVETICA"/>
        /// ,
        /// <see cref="StandardFonts.HELVETICA_BOLD"/>
        /// ,
        /// <see cref="StandardFonts.HELVETICA_OBLIQUE"/>
        /// and
        /// <see cref="StandardFonts.HELVETICA_BOLDOBLIQUE"/>
        /// .
        /// </summary>
        public const String HELVETICA = "Helvetica";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.SYMBOL"/>
        /// .
        /// </summary>
        public const String SYMBOL = "Symbol";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.ZAPFDINGBATS"/>
        /// .
        /// </summary>
        public const String ZAPFDINGBATS = "ZapfDingbats";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.TIMES_ROMAN"/>
        /// ,
        /// <see cref="StandardFonts.TIMES_BOLD"/>
        /// ,
        /// <see cref="StandardFonts.TIMES_ITALIC"/>
        /// and
        /// <see cref="StandardFonts.TIMES_BOLDITALIC"/>
        /// .
        /// </summary>
        public const String TIMES = "Times";
    }
}
