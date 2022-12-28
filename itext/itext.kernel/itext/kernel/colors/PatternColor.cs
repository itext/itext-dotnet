/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Colors {
    public class PatternColor : Color {
        private PdfPattern pattern;

        // The underlying color for uncolored patterns. Will be null for colored ones.
        private Color underlyingColor;

        public PatternColor(PdfPattern coloredPattern)
            : base(new PdfSpecialCs.Pattern(), null) {
            this.pattern = coloredPattern;
        }

        public PatternColor(PdfPattern.Tiling uncoloredPattern, Color color)
            : this(uncoloredPattern, color.GetColorSpace(), color.GetColorValue()) {
        }

        public PatternColor(PdfPattern.Tiling uncoloredPattern, PdfColorSpace underlyingCS, float[] colorValue)
            : this(uncoloredPattern, new PdfSpecialCs.UncoloredTilingPattern(EnsureNotPatternCs(underlyingCS)), colorValue
                ) {
        }

        public PatternColor(PdfPattern.Tiling uncoloredPattern, PdfSpecialCs.UncoloredTilingPattern uncoloredTilingCS
            , float[] colorValue)
            : base(uncoloredTilingCS, colorValue) {
            this.pattern = uncoloredPattern;
            this.underlyingColor = MakeColor(uncoloredTilingCS.GetUnderlyingColorSpace(), colorValue);
        }

        public virtual PdfPattern GetPattern() {
            return pattern;
        }

        public override void SetColorValue(float[] value) {
            base.SetColorValue(value);
            underlyingColor.SetColorValue(value);
        }

        public override bool Equals(Object o) {
            if (!base.Equals(o)) {
                return false;
            }
            iText.Kernel.Colors.PatternColor color = (iText.Kernel.Colors.PatternColor)o;
            return pattern.Equals(color.pattern) && (underlyingColor != null ? underlyingColor.Equals(color.underlyingColor
                ) : color.underlyingColor == null);
        }

        private static PdfColorSpace EnsureNotPatternCs(PdfColorSpace underlyingCS) {
            if (underlyingCS is PdfSpecialCs.Pattern) {
                throw new ArgumentException("underlyingCS");
            }
            return underlyingCS;
        }
    }
}
