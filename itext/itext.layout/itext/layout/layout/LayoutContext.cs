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
