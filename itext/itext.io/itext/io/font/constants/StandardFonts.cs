/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.IO.Font.Constants {
    public sealed class StandardFonts {
        private StandardFonts() {
        }

        private static readonly ICollection<String> BUILTIN_FONTS;

        static StandardFonts() {
            // HashSet is required in order to autoport correctly in .Net
            HashSet<String> tempSet = new HashSet<String>();
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.COURIER);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.COURIER_BOLD);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.COURIER_BOLDOBLIQUE);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.COURIER_OBLIQUE);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLDOBLIQUE);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.HELVETICA_OBLIQUE);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.SYMBOL);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.TIMES_BOLD);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.TIMES_BOLDITALIC);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.TIMES_ITALIC);
            tempSet.Add(iText.IO.Font.Constants.StandardFonts.ZAPFDINGBATS);
            BUILTIN_FONTS = JavaCollectionsUtil.UnmodifiableSet(tempSet);
        }

        public static bool IsStandardFont(String fontName) {
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
    }
}
