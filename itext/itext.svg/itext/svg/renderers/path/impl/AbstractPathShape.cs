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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Renderers.Path;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>This class handles common behaviour in IPathShape implementations</summary>
    public abstract class AbstractPathShape : IPathShape {
        /// <summary>The properties of this shape.</summary>
        protected internal IDictionary<String, String> properties;

        /// <summary>Whether this is a relative operator or not.</summary>
        protected internal bool relative;

        protected internal readonly IOperatorConverter copier;

        // Original coordinates from path instruction, according to the (x1 y1 x2 y2 x y)+ spec
        protected internal String[] coordinates;

        public AbstractPathShape()
            : this(false) {
        }

        public AbstractPathShape(bool relative)
            : this(relative, new DefaultOperatorConverter()) {
        }

        public AbstractPathShape(bool relative, IOperatorConverter copier) {
            this.relative = relative;
            this.copier = copier;
        }

        public virtual bool IsRelative() {
            return this.relative;
        }

        protected internal virtual Point CreatePoint(String coordX, String coordY) {
            return new Point((double)CssDimensionParsingUtils.ParseDouble(coordX), (double)CssDimensionParsingUtils.ParseDouble
                (coordY));
        }

        public virtual Point GetEndingPoint() {
            return CreatePoint(coordinates[coordinates.Length - 2], coordinates[coordinates.Length - 1]);
        }

        /// <summary>Get bounding rectangle of the current path shape.</summary>
        /// <param name="lastPoint">start point for this shape</param>
        /// <returns>calculated rectangle</returns>
        public virtual Rectangle GetPathShapeRectangle(Point lastPoint) {
            return new Rectangle((float)CssUtils.ConvertPxToPts(GetEndingPoint().GetX()), (float)CssUtils.ConvertPxToPts
                (GetEndingPoint().GetY()), 0, 0);
        }

        public abstract void Draw(PdfCanvas arg1);

        public abstract void SetCoordinates(String[] arg1, Point arg2);
    }
}
