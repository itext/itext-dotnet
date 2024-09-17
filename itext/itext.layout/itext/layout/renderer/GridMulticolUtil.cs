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
using iText.Layout.Borders;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>The class stores common logic for multicol and grid layout.</summary>
    internal sealed class GridMulticolUtil {
        private GridMulticolUtil() {
        }

//\cond DO_NOT_DOCUMENT
        // do nothing
        /// <summary>Creates a split renderer.</summary>
        /// <param name="children">children of the split renderer</param>
        /// <param name="renderer">parent renderer</param>
        /// <returns>
        /// a new
        /// <see cref="AbstractRenderer"/>
        /// instance
        /// </returns>
        internal static AbstractRenderer CreateSplitRenderer(IList<IRenderer> children, AbstractRenderer renderer) {
            AbstractRenderer splitRenderer = (AbstractRenderer)renderer.GetNextRenderer();
            splitRenderer.parent = renderer.parent;
            splitRenderer.modelElement = renderer.modelElement;
            splitRenderer.occupiedArea = renderer.occupiedArea;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.SetChildRenderers(children);
            splitRenderer.AddAllProperties(renderer.GetOwnProperties());
            ContinuousContainer.SetupContinuousContainerIfNeeded(splitRenderer);
            return splitRenderer;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float UpdateOccupiedWidth(float initialWidth, AbstractRenderer renderer) {
            float result = initialWidth;
            result += SafelyRetrieveFloatProperty(Property.PADDING_LEFT, renderer);
            result += SafelyRetrieveFloatProperty(Property.PADDING_RIGHT, renderer);
            result += SafelyRetrieveFloatProperty(Property.MARGIN_LEFT, renderer);
            result += SafelyRetrieveFloatProperty(Property.MARGIN_RIGHT, renderer);
            result += SafelyRetrieveFloatProperty(Property.BORDER_LEFT, renderer);
            result += SafelyRetrieveFloatProperty(Property.BORDER_RIGHT, renderer);
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float UpdateOccupiedHeight(float initialHeight, bool isFull, AbstractRenderer renderer) {
            float result = initialHeight;
            if (isFull) {
                result += SafelyRetrieveFloatProperty(Property.PADDING_BOTTOM, renderer);
                result += SafelyRetrieveFloatProperty(Property.MARGIN_BOTTOM, renderer);
                result += SafelyRetrieveFloatProperty(Property.BORDER_BOTTOM, renderer);
            }
            result += SafelyRetrieveFloatProperty(Property.PADDING_TOP, renderer);
            result += SafelyRetrieveFloatProperty(Property.MARGIN_TOP, renderer);
            result += SafelyRetrieveFloatProperty(Property.BORDER_TOP, renderer);
            return result;
        }
//\endcond

        private static float SafelyRetrieveFloatProperty(int property, AbstractRenderer renderer) {
            Object value = renderer.GetProperty<Object>(property);
            if (value is UnitValue) {
                return ((UnitValue)value).GetValue();
            }
            if (value is Border) {
                return ((Border)value).GetWidth();
            }
            return 0F;
        }
    }
//\endcond
}
