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
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Wrapper renderer around grid item.</summary>
    /// <remarks>Wrapper renderer around grid item. It's expected there is always exactly 1 child renderer.</remarks>
    internal class GridItemRenderer : BlockRenderer {
//\cond DO_NOT_DOCUMENT
        /// <summary>A renderer to wrap.</summary>
        internal AbstractRenderer renderer;
//\endcond

        /// <summary>Flag saying that we updated height of the renderer we wrap.</summary>
        /// <remarks>
        /// Flag saying that we updated height of the renderer we wrap.
        /// It allows to remove that property on split.
        /// </remarks>
        private bool heightSet = false;

//\cond DO_NOT_DOCUMENT
        internal GridItemRenderer()
            : base(new Div()) {
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        public override void AddChild(IRenderer renderer) {
            this.renderer = (AbstractRenderer)renderer;
            base.AddChild(renderer);
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.GridItemRenderer), this.GetType());
            return new iText.Layout.Renderer.GridItemRenderer();
        }

        /// <summary><inheritDoc/></summary>
        public override T1 GetProperty<T1>(int key) {
            // Handle only the props we are aware of
            switch (key) {
                case Property.GRID_COLUMN_START:
                case Property.GRID_COLUMN_END:
                case Property.GRID_COLUMN_SPAN:
                case Property.GRID_ROW_START:
                case Property.GRID_ROW_END:
                case Property.GRID_ROW_SPAN: {
                    T1 ownValue = this.GetOwnProperty<T1>(key);
                    if (ownValue != null) {
                        return ownValue;
                    }
                    else {
                        return renderer.GetProperty<T1>(key);
                    }
                    goto default;
                }

                default: {
                    break;
                }
            }
            return base.GetProperty<T1>(key);
        }

        /// <summary><inheritDoc/></summary>
        public override void SetProperty(int property, Object value) {
            // Handle only the props we are aware of
            switch (property) {
                case Property.HEIGHT: {
                    if (!renderer.HasProperty(property) || heightSet) {
                        renderer.SetProperty(Property.HEIGHT, value);
                        renderer.SetProperty(Property.MIN_HEIGHT, value);
                        heightSet = true;
                    }
                    break;
                }

                case Property.FILL_AVAILABLE_AREA_ON_SPLIT: {
                    renderer.SetProperty(property, value);
                    break;
                }

                case Property.COLLAPSING_MARGINS:
                case Property.GRID_COLUMN_START:
                case Property.GRID_COLUMN_END:
                case Property.GRID_COLUMN_SPAN:
                case Property.GRID_ROW_START:
                case Property.GRID_ROW_END:
                case Property.GRID_ROW_SPAN: {
                    base.SetProperty(property, value);
                    break;
                }

                default: {
                    break;
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override void UpdateHeightsOnSplit(float usedHeight, bool wasHeightClipped, AbstractRenderer splitRenderer
            , AbstractRenderer overflowRenderer, bool enlargeOccupiedAreaOnHeightWasClipped) {
            // If we set the height ourselves during layout, let's remove it while layouting on the next page
            // so that it is recalculated.
            if (heightSet) {
                // Always 1 child renderer
                overflowRenderer.childRenderers[0].DeleteOwnProperty(Property.HEIGHT);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override void AddChildRenderer(IRenderer child) {
            this.renderer = (AbstractRenderer)child;
            base.AddChildRenderer(child);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual float CalculateHeight(float initialHeight) {
            // We subtract margins/borders/paddings because we should take into account that
            // borders/paddings/margins should also fit into a cell.
            Rectangle rectangle = new Rectangle(0, 0, 0, initialHeight);
            if (AbstractRenderer.IsBorderBoxSizing(renderer)) {
                renderer.ApplyMargins(rectangle, false);
                // In BlockRenderer#layout, after applying continuous container, we call AbstractRenderer#retrieveMaxHeight,
                // which calls AbstractRenderer#retrieveHeight where in case of BoxSizing we reduce the height for top
                // padding and border. So to reduce the height for top + bottom border, padding and margin here we apply
                // both top and bottom margin, but only bottom padding and border
                UnitValue paddingBottom = renderer.GetProperty<UnitValue>(Property.PADDING_BOTTOM);
                if (paddingBottom.IsPointValue()) {
                    rectangle.DecreaseHeight(paddingBottom.GetValue());
                }
                Border borderBottom = renderer.GetBorders()[AbstractRenderer.BOTTOM_SIDE];
                if (borderBottom != null) {
                    rectangle.DecreaseHeight(borderBottom.GetWidth());
                }
            }
            else {
                renderer.ApplyMarginsBordersPaddings(rectangle, false);
            }
            return rectangle.GetHeight();
        }
//\endcond
    }
//\endcond
}
