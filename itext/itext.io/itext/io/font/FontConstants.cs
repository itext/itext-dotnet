/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
