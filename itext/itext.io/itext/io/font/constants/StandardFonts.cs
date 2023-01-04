/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
