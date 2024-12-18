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
using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Svg.Renderers.Path;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>This class handles common behaviour in IPathShape implementations</summary>
    public abstract class AbstractPathShape : IPathShape {
        private PathSvgNodeRenderer parent;

        /// <summary>The properties of this shape.</summary>
        protected internal IDictionary<String, String> properties;

        /// <summary>Whether this is a relative operator or not.</summary>
        protected internal bool relative;

        protected internal readonly IOperatorConverter copier;

        // Original coordinates from path instruction, according to the (x1 y1 x2 y2 x y)+ spec
        protected internal String[] coordinates;

        protected internal SvgDrawContext context;

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

        public virtual void Draw(PdfCanvas canvas) {
            Draw();
        }

        /// <summary>Draws this instruction to a canvas object.</summary>
        public abstract void Draw();

        /// <summary>Set parent path for this shape.</summary>
        /// <param name="parent">
        /// 
        /// <see cref="iText.Svg.Renderers.Impl.PathSvgNodeRenderer"/>
        /// instance
        /// </param>
        public virtual void SetParent(PathSvgNodeRenderer parent) {
            this.parent = parent;
        }

        /// <summary>Set svg draw context for this shape.</summary>
        /// <param name="context">
        /// 
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// instance.
        /// </param>
        public virtual void SetContext(SvgDrawContext context) {
            this.context = context;
        }

        /// <summary>Parse x axis length value.</summary>
        /// <param name="length">
        /// 
        /// <see cref="System.String"/>
        /// length for parsing
        /// </param>
        /// <returns>absolute length in points</returns>
        protected internal virtual float ParseHorizontalLength(String length) {
            return SvgCssUtils.ParseAbsoluteHorizontalLength(parent, length, 0.0F, context);
        }

        /// <summary>Parse y axis length value.</summary>
        /// <param name="length">
        /// 
        /// <see cref="System.String"/>
        /// length for parsing
        /// </param>
        /// <returns>absolute length in points</returns>
        protected internal virtual float ParseVerticalLength(String length) {
            return SvgCssUtils.ParseAbsoluteVerticalLength(parent, length, 0.0F, context);
        }

        public abstract void SetCoordinates(String[] arg1, Point arg2);
    }
}
