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
