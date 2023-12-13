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
using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout.Margincollapse;

namespace iText.Layout.Layout {
    /// <summary>
    /// Represents the context for content
    /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
    /// </summary>
    public class LayoutContext {
        /// <summary>
        /// The
        /// <see cref="LayoutArea"/>
        /// for the content to be placed on.
        /// </summary>
        protected internal LayoutArea area;

        /// <summary>The info about margins collapsing.</summary>
        protected internal MarginsCollapseInfo marginsCollapseInfo;

        /// <summary>
        /// The list of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// objects.
        /// </summary>
        protected internal IList<Rectangle> floatRendererAreas = new List<Rectangle>();

        /// <summary>Indicates whether the height is clipped or not.</summary>
        protected internal bool clippedHeight = false;

        /// <summary>Creates the layout context.</summary>
        /// <param name="area">for the content to be placed on</param>
        public LayoutContext(LayoutArea area) {
            this.area = area;
        }

        /// <summary>Creates the layout context.</summary>
        /// <param name="area">for the content to be placed on</param>
        /// <param name="marginsCollapseInfo">the info about margins collapsing</param>
        public LayoutContext(LayoutArea area, MarginsCollapseInfo marginsCollapseInfo) {
            this.area = area;
            this.marginsCollapseInfo = marginsCollapseInfo;
        }

        /// <summary>Creates the layout context.</summary>
        /// <param name="area">for the content to be placed on</param>
        /// <param name="marginsCollapseInfo">the info about margins collapsing</param>
        /// <param name="floatedRendererAreas">
        /// list of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// objects
        /// </param>
        public LayoutContext(LayoutArea area, MarginsCollapseInfo marginsCollapseInfo, IList<Rectangle> floatedRendererAreas
            )
            : this(area, marginsCollapseInfo) {
            if (floatedRendererAreas != null) {
                this.floatRendererAreas = floatedRendererAreas;
            }
        }

        /// <summary>Creates the layout context.</summary>
        /// <param name="area">for the content to be placed on</param>
        /// <param name="clippedHeight">indicates whether the height is clipped or not</param>
        public LayoutContext(LayoutArea area, bool clippedHeight)
            : this(area) {
            this.clippedHeight = clippedHeight;
        }

        /// <summary>Creates the layout context.</summary>
        /// <param name="area">for the content to be placed on</param>
        /// <param name="marginsCollapseInfo">the info about margins collapsing</param>
        /// <param name="floatedRendererAreas">
        /// list of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// objects
        /// </param>
        /// <param name="clippedHeight">indicates whether the height is clipped or not</param>
        public LayoutContext(LayoutArea area, MarginsCollapseInfo marginsCollapseInfo, IList<Rectangle> floatedRendererAreas
            , bool clippedHeight)
            : this(area, marginsCollapseInfo) {
            if (floatedRendererAreas != null) {
                this.floatRendererAreas = floatedRendererAreas;
            }
            this.clippedHeight = clippedHeight;
        }

        /// <summary>
        /// Gets the
        /// <see cref="LayoutArea">area</see>
        /// the content to be placed on.
        /// </summary>
        /// <returns>the area for content layouting.</returns>
        public virtual LayoutArea GetArea() {
            return area;
        }

        /// <summary>Gets info about margins collapsing.</summary>
        /// <returns>the info about margins collapsing</returns>
        public virtual MarginsCollapseInfo GetMarginsCollapseInfo() {
            return marginsCollapseInfo;
        }

        /// <summary>
        /// Gets list of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// objects.
        /// </summary>
        /// <returns>
        /// list of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// objects
        /// </returns>
        public virtual IList<Rectangle> GetFloatRendererAreas() {
            return floatRendererAreas;
        }

        /// <summary>Indicates whether the layout area's height is clipped or not.</summary>
        /// <returns>whether the layout area's height is clipped or not.</returns>
        public virtual bool IsClippedHeight() {
            return clippedHeight;
        }

        /// <summary>Defines whether the layout area's height is clipped or not.</summary>
        /// <param name="clippedHeight">indicates whether the height is clipped or not.</param>
        public virtual void SetClippedHeight(bool clippedHeight) {
            this.clippedHeight = clippedHeight;
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return area.ToString();
        }
    }
}
