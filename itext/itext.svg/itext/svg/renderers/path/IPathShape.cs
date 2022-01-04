/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers.Path {
    /// <summary>Interface for IPathShape, which draws the Path-data's d element instructions.</summary>
    public interface IPathShape {
        /// <summary>Draws this instruction to a canvas object.</summary>
        /// <param name="canvas">to which this instruction is drawn</param>
        void Draw(PdfCanvas canvas);

        /// <summary>
        /// This method sets the coordinates for the path painting operator and does internal
        /// preprocessing, if necessary
        /// </summary>
        /// <param name="inputCoordinates">an array containing point values for path coordinates</param>
        /// <param name="startPoint">
        /// the ending point of the previous operator, or, in broader terms,
        /// the point that the coordinates should be absolutized against, for relative operators
        /// </param>
        void SetCoordinates(String[] inputCoordinates, Point startPoint);

        /// <summary>
        /// Gets the ending point on the canvas after the path shape has been drawn
        /// via the
        /// <see cref="Draw(iText.Kernel.Pdf.Canvas.PdfCanvas)"/>
        /// method, in SVG space coordinates.
        /// </summary>
        /// <returns>
        /// The
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// representing the final point in the drawn path.
        /// If the point does not exist or does not change
        /// <see langword="null"/>
        /// may be returned.
        /// </returns>
        Point GetEndingPoint();

        /// <summary>Returns true when this shape is a relative operator.</summary>
        /// <remarks>Returns true when this shape is a relative operator. False if it is an absolute operator.</remarks>
        /// <returns>true if relative, false if absolute</returns>
        bool IsRelative();

        /// <summary>Get bounding rectangle of the current path shape.</summary>
        /// <param name="lastPoint">start point for this shape</param>
        /// <returns>calculated rectangle</returns>
        Rectangle GetPathShapeRectangle(Point lastPoint);
    }
}
