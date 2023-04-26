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
    /// <summary>A Pen object of the WMF format.</summary>
    /// <remarks>A Pen object of the WMF format. Holds the color, style and width information of the pen.</remarks>
    public class MetaPen : MetaObject {
        public const int PS_SOLID = 0;

        public const int PS_DASH = 1;

        public const int PS_DOT = 2;

        public const int PS_DASHDOT = 3;

        public const int PS_DASHDOTDOT = 4;

        public const int PS_NULL = 5;

        public const int PS_INSIDEFRAME = 6;

        internal int style = PS_SOLID;

        internal int penWidth = 1;

        internal Color color = ColorConstants.BLACK;

        /// <summary>Creates a MetaPen object.</summary>
        public MetaPen()
            : base(META_PEN) {
        }

        /// <summary>Initializes a MetaPen object.</summary>
        /// <param name="in">the InputMeta object that holds the inputstream of the WMF image</param>
        public virtual void Init(InputMeta @in) {
            style = @in.ReadWord();
            penWidth = @in.ReadShort();
            @in.ReadWord();
            color = @in.ReadColor();
        }

        /// <summary>Get the style of the MetaPen.</summary>
        /// <returns>style of the pen</returns>
        public virtual int GetStyle() {
            return style;
        }

        /// <summary>Get the width of the MetaPen.</summary>
        /// <returns>width of the pen</returns>
        public virtual int GetPenWidth() {
            return penWidth;
        }

        /// <summary>Get the color of the MetaPen.</summary>
        /// <returns>color of the pen</returns>
        public virtual Color GetColor() {
            return color;
        }
    }
}
