/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;

namespace iText.Kernel.Geom {
    /// <summary>Represents a line.</summary>
    public class Line : IShape {
        private readonly Point p1;

        private readonly Point p2;

        /// <summary>Constructs a new zero-length line starting at zero.</summary>
        public Line()
            : this(0, 0, 0, 0) {
        }

        /// <summary>Constructs a new line based on the given coordinates.</summary>
        /// <param name="x1">x-coordinate of start point of this Line</param>
        /// <param name="y1">y-coordinate of start point of this Line</param>
        /// <param name="x2">x-coordinate of end point of this Line</param>
        /// <param name="y2">y-coordinate of end point of this Line</param>
        public Line(float x1, float y1, float x2, float y2) {
            p1 = new Point(x1, y1);
            p2 = new Point(x2, y2);
        }

        /// <summary>Constructs a new line based on the given coordinates.</summary>
        /// <param name="p1">start point of this Line</param>
        /// <param name="p2">end point of this Line</param>
        public Line(Point p1, Point p2)
            : this((float)p1.GetX(), (float)p1.GetY(), (float)p2.GetX(), (float)p2.GetY()) {
        }

        public virtual IList<Point> GetBasePoints() {
            IList<Point> basePoints = new List<Point>(2);
            basePoints.Add(p1);
            basePoints.Add(p2);
            return basePoints;
        }
    }
}
