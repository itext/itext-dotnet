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
