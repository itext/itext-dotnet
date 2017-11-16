using System;
using System.Collections.Generic;

namespace iText.IO.Font.Constants {
    public sealed class StandardFonts {
        private StandardFonts() {
        }

        private static readonly ICollection<String> BUILTIN_FONTS = new HashSet<String>();

        static StandardFonts() {
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.COURIER);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.COURIER_BOLD);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.COURIER_BOLDOBLIQUE);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.COURIER_OBLIQUE);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLDOBLIQUE);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA_OBLIQUE);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.SYMBOL);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.TIMES_BOLD);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.TIMES_BOLDITALIC);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.TIMES_ITALIC);
            BUILTIN_FONTS.Add(iText.IO.Font.Constants.StandardFonts.ZAPFDINGBATS);
        }

        public static bool Contains(String fontName) {
            return BUILTIN_FONTS.Contains(fontName);
        }

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String COURIER = "Courier";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String COURIER_BOLD = "Courier-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String COURIER_OBLIQUE = "Courier-Oblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String COURIER_BOLDOBLIQUE = "Courier-BoldOblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String HELVETICA = "Helvetica";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String HELVETICA_BOLD = "Helvetica-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String HELVETICA_OBLIQUE = "Helvetica-Oblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String HELVETICA_BOLDOBLIQUE = "Helvetica-BoldOblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String SYMBOL = "Symbol";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String TIMES_ROMAN = "Times-Roman";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String TIMES_BOLD = "Times-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String TIMES_ITALIC = "Times-Italic";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String TIMES_BOLDITALIC = "Times-BoldItalic";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const String ZAPFDINGBATS = "ZapfDingbats";

        public const String TIMES = "Times";
    }
}
