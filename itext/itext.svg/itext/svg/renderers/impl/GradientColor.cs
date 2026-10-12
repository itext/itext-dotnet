/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Svg.Renderers.Impl {
//\cond DO_NOT_DOCUMENT
    /// <summary>Represents a gradient color with an optional opacity mask color.</summary>
    /// <remarks>
    /// Represents a gradient color with an optional opacity mask color.
    /// It's a container for 2 colors and uses inheritance only to be "compatible" with
    /// <see cref="iText.Kernel.Colors.Color"/>
    /// class.
    /// </remarks>
    internal sealed class GradientColor : Color {
        private const String UNSUPPORTED_OPERATION = "This class is a container for 2 colors, do not use the methods from a parent class";

        private readonly Color color;

        private readonly Color opacityColor;

//\cond DO_NOT_DOCUMENT
        internal GradientColor(Color color, Color opacityColor)
            : base(color.GetColorSpace(), color.GetColorValue()) {
            this.color = color;
            this.opacityColor = opacityColor;
        }
//\endcond

        public override int GetNumberOfComponents() {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        public override PdfColorSpace GetColorSpace() {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        public override float[] GetColorValue() {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

        public override void SetColorValue(float[] value) {
            throw new NotSupportedException(UNSUPPORTED_OPERATION);
        }

//\cond DO_NOT_DOCUMENT
        internal Color GetColor() {
            return color;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Color GetOpacityColor() {
            return opacityColor;
        }
//\endcond

        public override int GetHashCode() {
            return 31 * color.GetHashCode() + opacityColor.GetHashCode();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Svg.Renderers.Impl.GradientColor that = (iText.Svg.Renderers.Impl.GradientColor)o;
            return color.Equals(that.color) && opacityColor.Equals(that.opacityColor);
        }
    }
//\endcond
}
