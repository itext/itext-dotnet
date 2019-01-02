/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
Authors: iText Software.

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
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements curveTo(L) attribute of SVG's path element</summary>
    public class CurveTo : AbstractPathShape {
        private String[][] coordinates;

        public CurveTo()
            : this(false) {
        }

        public CurveTo(bool relative) {
            // Original coordinates from path instruction, according to the (x1 y1 x2 y2 x y)+ spec
            this.relative = relative;
        }

        public override void Draw(PdfCanvas canvas) {
            for (int i = 0; i < coordinates.Length; i++) {
                float x1 = CssUtils.ParseAbsoluteLength(coordinates[i][0]);
                float y1 = CssUtils.ParseAbsoluteLength(coordinates[i][1]);
                float x2 = CssUtils.ParseAbsoluteLength(coordinates[i][2]);
                float y2 = CssUtils.ParseAbsoluteLength(coordinates[i][3]);
                float x = CssUtils.ParseAbsoluteLength(coordinates[i][4]);
                float y = CssUtils.ParseAbsoluteLength(coordinates[i][5]);
                canvas.CurveTo(x1, y1, x2, y2, x, y);
            }
        }

        public override void SetCoordinates(String[] coordinates, Point startPoint) {
            if (coordinates.Length == 0 || coordinates.Length % 6 != 0) {
                throw new ArgumentException(MessageFormatUtil.Format(SvgExceptionMessageConstant.CURVE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0
                    , JavaUtil.ArraysToString(coordinates)));
            }
            this.coordinates = new String[coordinates.Length / 6][];
            double[] initialPoint = new double[] { startPoint.GetX(), startPoint.GetY() };
            for (int i = 0; i < coordinates.Length; i += 6) {
                String[] curCoordinates = new String[] { coordinates[i], coordinates[i + 1], coordinates[i + 2], coordinates
                    [i + 3], coordinates[i + 4], coordinates[i + 5] };
                if (IsRelative()) {
                    curCoordinates = SvgCoordinateUtils.MakeRelativeOperatorCoordinatesAbsolute(curCoordinates, initialPoint);
                    initialPoint[0] = (float)CssUtils.ParseFloat(curCoordinates[4]);
                    initialPoint[1] = (float)CssUtils.ParseFloat(curCoordinates[5]);
                }
                this.coordinates[i / 6] = curCoordinates;
            }
        }

        /// <summary>
        /// Returns coordinates of the last control point (the one closer to the ending point)
        /// in the series of Bezier curves (possibly, one curve), in SVG space coordinates
        /// </summary>
        /// <returns>coordinates of the last control points in SVG space coordinates</returns>
        public virtual Point GetLastControlPoint() {
            return CreatePoint(coordinates[coordinates.Length - 1][2], coordinates[coordinates.Length - 1][3]);
        }

        public override Point GetEndingPoint() {
            return CreatePoint(coordinates[coordinates.Length - 1][4], coordinates[coordinates.Length - 1][5]);
        }
    }
}
