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
