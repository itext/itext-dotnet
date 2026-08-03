/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Logs;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Renderer for the
    /// <see cref="iText.Layout.Element.AreaBreak"/>
    /// layout element.
    /// </summary>
    /// <remarks>
    /// Renderer for the
    /// <see cref="iText.Layout.Element.AreaBreak"/>
    /// layout element. Will terminate the
    /// current content area and initialize a new one.
    /// </remarks>
    public class AreaBreakRenderer : AbstractBreakRenderer {
        protected internal AreaBreak areaBreak;

        protected internal LayoutArea occupiedArea;

        private static readonly LazyLogger LOGGER = new LazyLogger(typeof(iText.Layout.Renderer.AreaBreakRenderer)
            );

        /// <summary>Creates an AreaBreakRenderer.</summary>
        /// <param name="areaBreak">
        /// the
        /// <see cref="iText.Layout.Element.AreaBreak"/>
        /// that will be rendered by this object
        /// </param>
        public AreaBreakRenderer(AreaBreak areaBreak) {
            this.areaBreak = areaBreak;
        }

        /// <summary>
        /// Logs a warning about unexpected use of
        /// <see cref="AreaBreakRenderer"/>
        /// if not ignored,
        /// because instances of this class are only used for terminating the current content area.
        /// </summary>
        /// <param name="renderer">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void AddChild(IRenderer renderer) {
            if (this.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS) == null) {
                LOGGER.Warn(() => LayoutLogMessageConstant.AREA_BREAK_UNEXPECTED);
            }
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            if (true.Equals(this.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS))) {
                if (occupiedArea == null) {
                    LOGGER.Warn(() => LayoutLogMessageConstant.AREA_BREAK_IGNORED);
                }
                Rectangle layoutContextAreaBbox = layoutContext.GetArea().GetBBox();
                Rectangle occupiedAreaBbox = new Rectangle(layoutContextAreaBbox.GetLeft(), layoutContextAreaBbox.GetTop()
                    , 0, 0);
                occupiedArea = new LayoutArea(layoutContext.GetArea().GetPageNumber(), occupiedAreaBbox);
                return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null, this);
            }
            return new LayoutResult(LayoutResult.NOTHING, null, null, null, this).SetAreaBreak(areaBreak);
        }

        /// <summary>
        /// Logs a warning about unexpected use of
        /// <see cref="AreaBreakRenderer"/>
        /// if not ignored,
        /// because instances of this class are only used for terminating the current content area.
        /// </summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void Draw(DrawContext drawContext) {
            if (this.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS) == null) {
                LOGGER.Warn(() => LayoutLogMessageConstant.AREA_BREAK_UNEXPECTED);
            }
        }

        /// <summary>
        /// Throws an UnsupportedOperationException if not ignored, because instances of this
        /// class are only used for terminating the current content area.
        /// </summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override LayoutArea GetOccupiedArea() {
            if (true.Equals(this.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS))) {
                return occupiedArea;
            }
            throw new NotSupportedException();
        }

        public override IPropertyContainer GetModelElement() {
            return null;
        }

        /// <summary>
        /// Logs a warning about unexpected use of
        /// <see cref="AreaBreakRenderer"/>
        /// if not ignored,
        /// because instances of this class are only used for terminating the current content area.
        /// </summary>
        /// <param name="dx">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="dy">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void Move(float dx, float dy) {
            if (this.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS) == null) {
                LOGGER.Warn(() => LayoutLogMessageConstant.AREA_BREAK_UNEXPECTED);
            }
        }

        public override IRenderer GetNextRenderer() {
            return null;
        }
    }
}
