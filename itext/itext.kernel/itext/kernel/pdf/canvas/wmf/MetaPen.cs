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
