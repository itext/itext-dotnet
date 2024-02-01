/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>
    /// Represents a
    /// <see cref="Border3D"/>
    /// with an outset effect being applied.
    /// </summary>
    public class OutsetBorder : Border3D {
        /// <summary>Creates an OutsetBorder instance with the specified width.</summary>
        /// <remarks>Creates an OutsetBorder instance with the specified width. The color is set to the predefined gray.
        ///     </remarks>
        /// <param name="width">width of the border</param>
        public OutsetBorder(float width)
            : base(width) {
        }

        /// <summary>
        /// Creates an OutsetBorder instance with the specified width and the
        /// <see cref="iText.Kernel.Colors.DeviceRgb">rgb color</see>.
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="color">
        /// the
        /// <see cref="iText.Kernel.Colors.DeviceRgb">rgb color</see>
        /// of the border
        /// </param>
        public OutsetBorder(DeviceRgb color, float width)
            : base(color, width) {
        }

        /// <summary>
        /// Creates an OutsetBorder instance with the specified width and the
        /// <see cref="iText.Kernel.Colors.DeviceCmyk">cmyk color</see>.
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="color">
        /// the
        /// <see cref="iText.Kernel.Colors.DeviceCmyk">cmyk color</see>
        /// of the border
        /// </param>
        public OutsetBorder(DeviceCmyk color, float width)
            : base(color, width) {
        }

        /// <summary>
        /// Creates an OutsetBorder instance with the specified width and the
        /// <see cref="iText.Kernel.Colors.DeviceGray">gray color</see>.
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="color">
        /// the
        /// <see cref="iText.Kernel.Colors.DeviceGray">gray color</see>
        /// of the border
        /// </param>
        public OutsetBorder(DeviceGray color, float width)
            : base(color, width) {
        }

        /// <summary>Creates an OutsetBorder instance with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">opacity of the border</param>
        public OutsetBorder(DeviceRgb color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary>Creates an OutsetBorder instance with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">opacity of the border</param>
        public OutsetBorder(DeviceCmyk color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary>Creates an OutsetBorder instance with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">opacity of the border</param>
        public OutsetBorder(DeviceGray color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return Border._3D_OUTSET;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void SetInnerHalfColor(PdfCanvas canvas, Border.Side side) {
            switch (side) {
                case Border.Side.TOP:
                case Border.Side.LEFT: {
                    canvas.SetFillColor(GetColor());
                    break;
                }

                case Border.Side.BOTTOM:
                case Border.Side.RIGHT: {
                    canvas.SetFillColor(GetDarkerColor());
                    break;
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void SetOuterHalfColor(PdfCanvas canvas, Border.Side side) {
            switch (side) {
                case Border.Side.TOP:
                case Border.Side.LEFT: {
                    canvas.SetFillColor(GetColor());
                    break;
                }

                case Border.Side.BOTTOM:
                case Border.Side.RIGHT: {
                    canvas.SetFillColor(GetDarkerColor());
                    break;
                }
            }
        }
    }
}
