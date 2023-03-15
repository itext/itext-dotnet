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
