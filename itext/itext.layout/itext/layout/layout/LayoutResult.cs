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
using iText.Layout.Element;
using iText.Layout.Renderer;

namespace iText.Layout.Layout {
    /// <summary>
    /// Represents the result of content
    /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
    /// </summary>
    public class LayoutResult {
        /// <summary>
        /// The status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// which indicates that the content was fully placed.
        /// </summary>
        public const int FULL = 1;

        /// <summary>
        /// The status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// which indicates that the content was placed partially.
        /// </summary>
        public const int PARTIAL = 2;

        /// <summary>
        /// The status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// which indicates that the content was not placed.
        /// </summary>
        public const int NOTHING = 3;

        /// <summary>
        /// The status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// which indicates whether the content was added or not
        /// and, if yes, was it added fully or partially.
        /// </summary>
        protected internal int status;

        /// <summary>
        /// The area occupied by the content during its
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// </summary>
        /// <remarks>
        /// The area occupied by the content during its
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// which indicates whether the content was added or not and, if yes, was it added fully or partially.
        /// </remarks>
        protected internal LayoutArea occupiedArea;

        /// <summary>
        /// The split renderer created during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// </summary>
        /// <remarks>
        /// The split renderer created during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// This renderer will be used to draw the splitted part of content.
        /// </remarks>
        protected internal IRenderer splitRenderer;

        /// <summary>
        /// The overflow renderer created during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// </summary>
        /// <remarks>
        /// The overflow renderer created during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// This renderer will be used to draw the overflowed part of content.
        /// </remarks>
        protected internal IRenderer overflowRenderer;

        /// <summary>
        /// The
        /// <see cref="iText.Layout.Element.AreaBreak"/>
        /// that will be rendered by this object.
        /// </summary>
        protected internal AreaBreak areaBreak;

        /// <summary>
        /// The first renderer to produce
        /// <see cref="NOTHING"/>
        /// during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>.
        /// </summary>
        protected internal IRenderer causeOfNothing;

        /// <summary>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// </summary>
        /// <remarks>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// The
        /// <see cref="causeOfNothing"/>
        /// will be set as null.
        /// </remarks>
        /// <param name="status">
        /// the status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// </param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the splitted part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        public LayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            )
            : this(status, occupiedArea, splitRenderer, overflowRenderer, null) {
        }

        /// <summary>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// </summary>
        /// <param name="status">
        /// the status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// </param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the splitted part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        /// <param name="cause">
        /// the first renderer to produce
        /// <see cref="NOTHING"/>
        /// </param>
        public LayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            , IRenderer cause) {
            this.status = status;
            this.occupiedArea = occupiedArea;
            this.splitRenderer = splitRenderer;
            this.overflowRenderer = overflowRenderer;
            this.causeOfNothing = cause;
        }

        /// <summary>
        /// Gets the status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>.
        /// </summary>
        /// <returns>the status</returns>
        public virtual int GetStatus() {
            return status;
        }

        /// <summary>
        /// Sets the status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>.
        /// </summary>
        /// <param name="status">
        /// the status of
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// </param>
        public virtual void SetStatus(int status) {
            this.status = status;
        }

        /// <summary>
        /// Gets the
        /// <see cref="LayoutArea">layout area</see>
        /// occupied by the content during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="LayoutArea">layout area</see>
        /// occupied by the content
        /// </returns>
        public virtual LayoutArea GetOccupiedArea() {
            return occupiedArea;
        }

        /// <summary>
        /// Gets the split
        /// <see cref="iText.Layout.Renderer.IRenderer">renderer</see>
        /// created during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Renderer.IRenderer">renderer</see>
        /// </returns>
        public virtual IRenderer GetSplitRenderer() {
            return splitRenderer;
        }

        /// <summary>
        /// Sets the split
        /// <see cref="iText.Layout.Renderer.IRenderer">renderer</see>.
        /// </summary>
        /// <param name="splitRenderer">the renderer to draw the splitted part of the content</param>
        public virtual void SetSplitRenderer(IRenderer splitRenderer) {
            this.splitRenderer = splitRenderer;
        }

        /// <summary>
        /// Gets the overflow renderer created during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Renderer.IRenderer">renderer</see>
        /// </returns>
        public virtual IRenderer GetOverflowRenderer() {
            return overflowRenderer;
        }

        /// <summary>
        /// Sets the overflow
        /// <see cref="iText.Layout.Renderer.IRenderer">renderer</see>.
        /// </summary>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        public virtual void SetOverflowRenderer(IRenderer overflowRenderer) {
            this.overflowRenderer = overflowRenderer;
        }

        /// <summary>Gets areaBreak value.</summary>
        /// <returns>the areaBreak value</returns>
        public virtual AreaBreak GetAreaBreak() {
            return areaBreak;
        }

        /// <summary>Sets areaBreak value.</summary>
        /// <param name="areaBreak">the areaBreak value</param>
        /// <returns>the areaBreak value</returns>
        public virtual iText.Layout.Layout.LayoutResult SetAreaBreak(AreaBreak areaBreak) {
            this.areaBreak = areaBreak;
            return this;
        }

        /// <summary>
        /// Gets the first renderer to produce
        /// <see cref="NOTHING"/>
        /// during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)"/>
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Renderer.IRenderer">renderer</see>
        /// </returns>
        public virtual IRenderer GetCauseOfNothing() {
            return causeOfNothing;
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            String status;
            switch (GetStatus()) {
                case FULL: {
                    status = "Full";
                    break;
                }

                case NOTHING: {
                    status = "Nothing";
                    break;
                }

                case PARTIAL: {
                    status = "Partial";
                    break;
                }

                default: {
                    status = "None";
                    break;
                }
            }
            return "LayoutResult{" + status + ", areaBreak=" + areaBreak + ", occupiedArea=" + occupiedArea + '}';
        }
    }
}
