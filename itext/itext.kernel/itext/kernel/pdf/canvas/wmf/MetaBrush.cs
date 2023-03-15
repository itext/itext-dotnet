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
using iText.Kernel.Colors;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    /// <summary>
    /// A Brush bject that holds information about the style, the hatch and the color of
    /// the brush.
    /// </summary>
    public class MetaBrush : MetaObject {
        public const int BS_SOLID = 0;

        public const int BS_NULL = 1;

        public const int BS_HATCHED = 2;

        public const int BS_PATTERN = 3;

        public const int BS_DIBPATTERN = 5;

        public const int HS_HORIZONTAL = 0;

        public const int HS_VERTICAL = 1;

        public const int HS_FDIAGONAL = 2;

        public const int HS_BDIAGONAL = 3;

        public const int HS_CROSS = 4;

        public const int HS_DIAGCROSS = 5;

        internal int style = BS_SOLID;

        internal int hatch;

        internal Color color = ColorConstants.WHITE;

        /// <summary>Creates a MetaBrush object.</summary>
        public MetaBrush()
            : base(META_BRUSH) {
        }

        /// <summary>Initializes this MetaBrush object.</summary>
        /// <param name="in">the InputMeta</param>
        public virtual void Init(InputMeta @in) {
            style = @in.ReadWord();
            color = @in.ReadColor();
            hatch = @in.ReadWord();
        }

        /// <summary>Get the style of the MetaBrush.</summary>
        /// <returns>style of the brush</returns>
        public virtual int GetStyle() {
            return style;
        }

        /// <summary>Get the hatch pattern of the MetaBrush</summary>
        /// <returns>hatch of the brush</returns>
        public virtual int GetHatch() {
            return hatch;
        }

        /// <summary>Get the color of the MetaBrush.</summary>
        /// <returns>color of the brush</returns>
        public virtual Color GetColor() {
            return color;
        }
    }
}
