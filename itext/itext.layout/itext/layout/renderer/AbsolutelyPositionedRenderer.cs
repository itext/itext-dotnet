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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Wrapper for absolutely positioned elements which lack coordinates in a certain axis.</summary>
    /// <remarks>
    /// Wrapper for absolutely positioned elements which lack coordinates in a certain axis.
    /// This wrapper performs fake static layout for such elements to determine it's coordinates.
    /// </remarks>
    internal class AbsolutelyPositionedRenderer : IRenderer {
        private readonly IRenderer wrappedRenderer;

        private readonly bool verticalCoordinateMissing;

        private readonly bool horizontalCoordinateMissing;

        private readonly DivRenderer dummyRenderer = new DivRenderer(new Div().SetWidth(0).SetHeight(0));

        public AbsolutelyPositionedRenderer(IRenderer wrappedRenderer, bool verticalCoordinateMissing, bool horizontalCoordinateMissing
            ) {
            this.wrappedRenderer = wrappedRenderer;
            this.wrappedRenderer.SetProperty(Property.POSITIONED_ELEMENT_WRAPPED, new Object());
            this.verticalCoordinateMissing = verticalCoordinateMissing;
            this.horizontalCoordinateMissing = horizontalCoordinateMissing;
        }

        public virtual LayoutResult Layout(LayoutContext layoutContext) {
            LayoutContext copiedContext = CopyContext(layoutContext);
            Object positioning = wrappedRenderer.GetOwnProperty<int?>(Property.POSITION);
            wrappedRenderer.SetProperty(Property.POSITION, LayoutPosition.STATIC);
            wrappedRenderer.SetProperty(Property.FLOAT, FloatPropertyValue.NONE);
            LayoutResult result = wrappedRenderer.Layout(copiedContext);
            if (result.GetStatus() == LayoutResult.NOTHING) {
                wrappedRenderer.SetProperty(Property.FORCED_PLACEMENT, true);
                result = wrappedRenderer.Layout(copiedContext);
                wrappedRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
            }
            if (positioning == null) {
                wrappedRenderer.DeleteOwnProperty(Property.POSITION);
            }
            else {
                wrappedRenderer.SetProperty(Property.POSITION, positioning);
            }
            if (verticalCoordinateMissing) {
                wrappedRenderer.SetProperty(Property.TOP_CALCULATED, result.GetOccupiedArea().GetBBox().GetTop());
            }
            if (horizontalCoordinateMissing) {
                wrappedRenderer.SetProperty(Property.LEFT_CALCULATED, result.GetOccupiedArea().GetBBox().GetLeft());
            }
            if (wrappedRenderer is AbstractRenderer) {
                ((AbstractRenderer)wrappedRenderer).occupiedArea = null;
            }
            return dummyRenderer.Layout(copiedContext);
        }

        public virtual IRenderer GetWrappedRenderer() {
            return wrappedRenderer;
        }

        public virtual IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.AbsolutelyPositionedRenderer(wrappedRenderer.GetNextRenderer(), verticalCoordinateMissing
                , horizontalCoordinateMissing);
        }

        public virtual void Draw(DrawContext drawContext) {
        }

        // We never need to draw wrapper renderer.
        public virtual LayoutArea GetOccupiedArea() {
            return dummyRenderer.GetOccupiedArea();
        }

        public virtual void Move(float dx, float dy) {
        }

        // We don't need to move wrapper renderer.
        public virtual bool HasProperty(int property) {
            return wrappedRenderer.HasProperty(property);
        }

        public virtual bool HasOwnProperty(int property) {
            return wrappedRenderer.HasOwnProperty(property);
        }

        public virtual T1 GetProperty<T1>(int property) {
            if (Property.POSITION == property) {
                // This absolutely positioned renderer wrapper is never supposed to be treated as absolutely positioned.
                // The whole idea of this wrapper is to calculate it's potential static coordinates.
                return (T1)(Object)LayoutPosition.STATIC;
            }
            return wrappedRenderer.GetProperty<T1>(property);
        }

        public virtual T1 GetProperty<T1>(int property, T1 defaultValue) {
            if (Property.POSITION == property) {
                // This absolutely positioned renderer wrapper is never supposed to be treated as absolutely positioned.
                // The whole idea of this wrapper is to calculate it's potential static coordinates.
                return (T1)(Object)LayoutPosition.STATIC;
            }
            return wrappedRenderer.GetProperty<T1>(property, defaultValue);
        }

        public virtual T1 GetOwnProperty<T1>(int property) {
            return wrappedRenderer.GetOwnProperty<T1>(property);
        }

        public virtual T1 GetDefaultProperty<T1>(int property) {
            return wrappedRenderer.GetDefaultProperty<T1>(property);
        }

        public virtual void SetProperty(int property, Object value) {
            wrappedRenderer.SetProperty(property, value);
        }

        public virtual void DeleteOwnProperty(int property) {
            wrappedRenderer.DeleteOwnProperty(property);
        }

        public virtual void AddChild(IRenderer renderer) {
            wrappedRenderer.AddChild(renderer);
        }

        public virtual IRenderer SetParent(IRenderer parent) {
            wrappedRenderer.SetParent(parent);
            return this;
        }

        public virtual IRenderer GetParent() {
            return wrappedRenderer.GetParent();
        }

        public virtual IPropertyContainer GetModelElement() {
            return wrappedRenderer.GetModelElement();
        }

        public virtual IList<IRenderer> GetChildRenderers() {
            return wrappedRenderer.GetChildRenderers();
        }

        public virtual bool IsFlushed() {
            return wrappedRenderer.IsFlushed();
        }

        private static LayoutContext CopyContext(LayoutContext originalContext) {
            MarginsCollapseInfo copiedMarginsCollapseInfo = null;
            if (originalContext.GetMarginsCollapseInfo() != null) {
                copiedMarginsCollapseInfo = MarginsCollapseInfo.CreateDeepCopy(originalContext.GetMarginsCollapseInfo());
            }
            List<Rectangle> attemptFloatRectsList = new List<Rectangle>(originalContext.GetFloatRendererAreas());
            return new LayoutContext(originalContext.GetArea().Clone(), copiedMarginsCollapseInfo, attemptFloatRectsList
                , originalContext.IsClippedHeight());
        }
    }
//\endcond
}
