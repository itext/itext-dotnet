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
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Renderer;

namespace iText.Layout.Properties {
    /// <summary>This class is used to store properties of the renderer that are needed to be removed/reapplied.</summary>
    /// <remarks>
    /// This class is used to store properties of the renderer that are needed to be removed/reapplied.
    /// THis is used for processing continuous container property.
    /// This behavior is used when we want to simulate a continuous appearance over multiple pages.
    /// This means that only for the first and last page the margins, paddings and borders are applied.
    /// On the first page the top properties are applied and on the last page the bottom properties are applied.
    /// </remarks>
    public sealed class ContinuousContainer {
        /// <summary>Properties needed to be removed/added for continuous container.</summary>
        private static readonly int[] PROPERTIES_NEEDED_FOR_CONTINUOUS_CONTAINER = new int[] { Property.MARGIN_BOTTOM
            , Property.BORDER_BOTTOM, Property.PADDING_BOTTOM, Property.BORDER };

        private readonly Dictionary<int, Object> properties = new Dictionary<int, Object>();

        /// <summary>
        /// Creates a new
        /// <see cref="ContinuousContainer"/>
        /// instance.
        /// </summary>
        /// <param name="renderer">the renderer that is used to get properties from.</param>
        private ContinuousContainer(IRenderer renderer) {
            foreach (int property in PROPERTIES_NEEDED_FOR_CONTINUOUS_CONTAINER) {
                properties.Put(property, renderer.GetProperty<Object>(property));
            }
        }

        /// <summary>Removes properties from the overflow renderer that are not needed for continuous container.</summary>
        /// <param name="overFlowRenderer">the renderer that is used to remove properties from.</param>
        public static void ClearPropertiesFromOverFlowRenderer(IPropertyContainer overFlowRenderer) {
            if (overFlowRenderer == null) {
                return;
            }
            if (true.Equals(overFlowRenderer.GetProperty<bool?>(Property.TREAT_AS_CONTINUOUS_CONTAINER))) {
                overFlowRenderer.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(0));
                overFlowRenderer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(0));
                overFlowRenderer.SetProperty(Property.BORDER_TOP, null);
            }
        }

        /// <summary>Sets up the needed values in the model element of the renderer.</summary>
        /// <param name="blockRenderer">the renderer that is used to set up continuous container.</param>
        public static void SetupContinuousContainerIfNeeded(AbstractRenderer blockRenderer) {
            if (true.Equals(blockRenderer.GetProperty<bool?>(Property.TREAT_AS_CONTINUOUS_CONTAINER))) {
                if (!blockRenderer.HasProperty(Property.TREAT_AS_CONTINUOUS_CONTAINER_RESULT)) {
                    iText.Layout.Properties.ContinuousContainer continuousContainer = new iText.Layout.Properties.ContinuousContainer
                        (blockRenderer);
                    blockRenderer.SetProperty(Property.TREAT_AS_CONTINUOUS_CONTAINER_RESULT, continuousContainer);
                }
                ClearPropertiesFromSplitRenderer(blockRenderer);
            }
        }

        private static void ClearPropertiesFromSplitRenderer(AbstractRenderer blockRenderer) {
            if (blockRenderer == null) {
                return;
            }
            blockRenderer.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(0));
            blockRenderer.SetProperty(Property.BORDER_BOTTOM, null);
            blockRenderer.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(0));
        }

        /// <summary>Re adds the properties that were removed from the overflow renderer.</summary>
        /// <param name="blockRenderer">the renderer that is used to reapply properties.</param>
        public void ReApplyProperties(BlockRenderer blockRenderer) {
            foreach (int property in PROPERTIES_NEEDED_FOR_CONTINUOUS_CONTAINER) {
                blockRenderer.SetProperty(property, properties.Get(property));
            }
            Border allBorders = (Border)properties.Get(Property.BORDER);
            Border bottomBorder = (Border)properties.Get(Property.BORDER_BOTTOM);
            if (allBorders != null && bottomBorder == null) {
                blockRenderer.SetProperty(Property.BORDER_BOTTOM, allBorders);
            }
        }
    }
}
