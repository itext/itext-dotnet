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

namespace iText.IO.Font {
    /// <summary>
    /// Font constants for
    /// <see cref="FontProgramFactory"/>
    /// and PdfFontFactory.
    /// </summary>
    [System.ObsoleteAttribute(@"Use constants from com.itextpdf.io.font.constants.")]
    public class FontConstants {
        //-Font styles------------------------------------------------------------------------------------------------------
        /// <summary>Undefined font style.</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.FontStyles.UNDEFINED instead.")]
        public const int UNDEFINED = -1;

        /// <summary>Normal font style.</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.FontStyles.NORMAL instead.")]
        public const int NORMAL = 0;

        /// <summary>Bold font style.</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.FontStyles.BOLD instead.")]
        public const int BOLD = 1;

        /// <summary>Italic font style.</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.FontStyles.ITALIC instead.")]
        public const int ITALIC = 2;

        /// <summary>Bold-Italic font style.</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.FontStyles.BOLDITALIC instead.")]
        public const int BOLDITALIC = BOLD | ITALIC;

        //-Default fonts----------------------------------------------------------------------------------------------------
        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.COURIER instead.")]
        public const String COURIER = "Courier";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.COURIER_BOLD instead.")]
        public const String COURIER_BOLD = "Courier-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.COURIER_OBLIQUE instead.")]
        public const String COURIER_OBLIQUE = "Courier-Oblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.COURIER_BOLDOBLIQUE instead.")]
        public const String COURIER_BOLDOBLIQUE = "Courier-BoldOblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.HELVETICA instead.")]
        public const String HELVETICA = "Helvetica";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD instead.")]
        public const String HELVETICA_BOLD = "Helvetica-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.HELVETICA_OBLIQUE instead.")]
        public const String HELVETICA_OBLIQUE = "Helvetica-Oblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLDOBLIQUE instead.")]
        public const String HELVETICA_BOLDOBLIQUE = "Helvetica-BoldOblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.SYMBOL instead.")]
        public const String SYMBOL = "Symbol";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN instead.")]
        public const String TIMES_ROMAN = "Times-Roman";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.TIMES_BOLD instead.")]
        public const String TIMES_BOLD = "Times-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.TIMES_ITALIC instead.")]
        public const String TIMES_ITALIC = "Times-Italic";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.TIMES_BOLDITALIC instead.")]
        public const String TIMES_BOLDITALIC = "Times-BoldItalic";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        [System.ObsoleteAttribute(@"use iText.IO.Font.Constants.StandardFonts.ZAPFDINGBATS instead.")]
        public const String ZAPFDINGBATS = "ZapfDingbats";
    }
}
