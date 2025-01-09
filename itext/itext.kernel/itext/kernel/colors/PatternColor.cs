/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
    /// <summary>Representation of a Pattern Color.</summary>
    public class PatternColor : Color {
        private PdfPattern pattern;

        // The underlying color for uncolored patterns. Will be null for colored ones.
        private Color underlyingColor;

        /// <summary>Creates a pattern color using the given color pattern object.</summary>
        /// <param name="coloredPattern">Color space that uses pattern objects</param>
        public PatternColor(PdfPattern coloredPattern)
            : base(new PdfSpecialCs.Pattern(), null) {
            this.pattern = coloredPattern;
        }

        /// <summary>Creates a pattern color using the given uncolored pattern object and color.</summary>
        /// <param name="uncoloredPattern">Tiling pattern object of the color space</param>
        /// <param name="color">Color object</param>
        public PatternColor(PdfPattern.Tiling uncoloredPattern, Color color)
            : this(uncoloredPattern, color.GetColorSpace(), color.GetColorValue()) {
        }

        /// <summary>Creates a pattern color using the given uncolored pattern object, an underlying color space and color values.
        ///     </summary>
        /// <param name="uncoloredPattern">Tiling pattern object of the color space</param>
        /// <param name="underlyingCS">Underlying color space object</param>
        /// <param name="colorValue">Color values</param>
        public PatternColor(PdfPattern.Tiling uncoloredPattern, PdfColorSpace underlyingCS, float[] colorValue)
            : this(uncoloredPattern, new PdfSpecialCs.UncoloredTilingPattern(EnsureNotPatternCs(underlyingCS)), colorValue
                ) {
        }

        /// <summary>Creates a pattern color using the given uncolored pattern object, uncolored tiling pattern and color values.
        ///     </summary>
        /// <param name="uncoloredPattern">Tiling pattern object of the color space</param>
        /// <param name="uncoloredTilingCS">Tiling pattern color space</param>
        /// <param name="colorValue">Color values</param>
        public PatternColor(PdfPattern.Tiling uncoloredPattern, PdfSpecialCs.UncoloredTilingPattern uncoloredTilingCS
            , float[] colorValue)
            : base(uncoloredTilingCS, colorValue) {
            this.pattern = uncoloredPattern;
            this.underlyingColor = MakeColor(uncoloredTilingCS.GetUnderlyingColorSpace(), colorValue);
        }

        /// <summary>Returns the pattern of the color space.</summary>
        /// <returns>PdfPattern object</returns>
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
