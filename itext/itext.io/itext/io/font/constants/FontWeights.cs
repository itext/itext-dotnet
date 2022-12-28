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

namespace iText.IO.Font.Constants {
    public sealed class FontWeights {
        private FontWeights() {
        }

        // Font weight Thin
        public const int THIN = 100;

        // Font weight Extra-light (Ultra-light)
        public const int EXTRA_LIGHT = 200;

        // Font weight Light
        public const int LIGHT = 300;

        // Font weight Normal
        public const int NORMAL = 400;

        // Font weight Medium
        public const int MEDIUM = 500;

        // Font weight Semi-bold
        public const int SEMI_BOLD = 600;

        // Font weight Bold
        public const int BOLD = 700;

        // Font weight Extra-bold (Ultra-bold)
        public const int EXTRA_BOLD = 800;

        // Font weight Black (Heavy)
        public const int BLACK = 900;

        public static int FromType1FontWeight(String weight) {
            int fontWeight = NORMAL;
            switch (weight.ToLowerInvariant()) {
                case "ultralight": {
                    fontWeight = THIN;
                    break;
                }

                case "thin":
                case "extralight": {
                    fontWeight = EXTRA_LIGHT;
                    break;
                }

                case "light": {
                    fontWeight = LIGHT;
                    break;
                }

                case "book":
                case "regular":
                case "normal": {
                    fontWeight = NORMAL;
                    break;
                }

                case "medium": {
                    fontWeight = MEDIUM;
                    break;
                }

                case "demibold":
                case "semibold": {
                    fontWeight = SEMI_BOLD;
                    break;
                }

                case "bold": {
                    fontWeight = BOLD;
                    break;
                }

                case "extrabold":
                case "ultrabold": {
                    fontWeight = EXTRA_BOLD;
                    break;
                }

                case "heavy":
                case "black":
                case "ultra":
                case "ultrablack": {
                    fontWeight = BLACK;
                    break;
                }

                case "fat":
                case "extrablack": {
                    fontWeight = BLACK;
                    break;
                }
            }
            return fontWeight;
        }

        public static int NormalizeFontWeight(int fontWeight) {
            fontWeight = (fontWeight / 100) * 100;
            if (fontWeight < iText.IO.Font.Constants.FontWeights.THIN) {
                return iText.IO.Font.Constants.FontWeights.THIN;
            }
            if (fontWeight > iText.IO.Font.Constants.FontWeights.BLACK) {
                return iText.IO.Font.Constants.FontWeights.BLACK;
            }
            return fontWeight;
        }
    }
}
