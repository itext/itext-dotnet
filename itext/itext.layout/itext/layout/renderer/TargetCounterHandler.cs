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
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Handler to handle target-counter logic.</summary>
    public class TargetCounterHandler {
        /// <summary>Pages for all renderers with id.</summary>
        private IDictionary<String, int?> renderersPages = new Dictionary<String, int?>();

        private IDictionary<String, int?> previousRenderersPages = new Dictionary<String, int?>();

        /// <summary>
        /// Creates a copy of the given
        /// <see cref="TargetCounterHandler"/>
        /// instance.
        /// </summary>
        /// <param name="targetCounterHandler">
        /// 
        /// <see cref="TargetCounterHandler"/>
        /// instance to be copied
        /// </param>
        public TargetCounterHandler(iText.Layout.Renderer.TargetCounterHandler targetCounterHandler) {
            this.renderersPages = targetCounterHandler.renderersPages;
            this.previousRenderersPages = targetCounterHandler.previousRenderersPages;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TargetCounterHandler"/>
        /// instance.
        /// </summary>
        public TargetCounterHandler() {
        }

        /// <summary>Adds renderer's page to the root renderer map.</summary>
        /// <param name="renderer">renderer from which page and root renderer will be taken.</param>
        public static void AddPageByID(IRenderer renderer) {
            String id = renderer.GetProperty<String>(Property.ID);
            if (id != null) {
                iText.Layout.Renderer.TargetCounterHandler targetCounterHandler = GetTargetCounterHandler(renderer);
                if (targetCounterHandler != null && renderer.GetOccupiedArea() != null) {
                    int currentPageNumber = renderer.GetOccupiedArea().GetPageNumber();
                    targetCounterHandler.renderersPages.Put(id, currentPageNumber);
                }
            }
        }

        /// <summary>Gets page from renderer using given id.</summary>
        /// <param name="renderer">renderer from which root renderer will be taken</param>
        /// <param name="id">key to the renderersPages Map</param>
        /// <returns>page on which renderer was layouted</returns>
        public static int? GetPageByID(IRenderer renderer, String id) {
            iText.Layout.Renderer.TargetCounterHandler targetCounterHandler = GetTargetCounterHandler(renderer);
            return targetCounterHandler == null ? null : targetCounterHandler.previousRenderersPages.Get(id);
        }

        /// <summary>Indicates if page value was defined for this id.</summary>
        /// <param name="renderer">renderer from which root renderer will be taken</param>
        /// <param name="id">target id</param>
        /// <returns>true if value is defined for this id, false otherwise</returns>
        public static bool IsValueDefinedForThisId(IRenderer renderer, String id) {
            iText.Layout.Renderer.TargetCounterHandler targetCounterHandler = GetTargetCounterHandler(renderer);
            return targetCounterHandler != null && targetCounterHandler.renderersPages.ContainsKey(id);
        }

        /// <summary>Indicates if relayout is required.</summary>
        /// <returns>true if relayout is required, false otherwise</returns>
        public virtual bool IsRelayoutRequired() {
            foreach (KeyValuePair<String, int?> rendererPage in renderersPages) {
                if (!rendererPage.Value.Equals(previousRenderersPages.Get(rendererPage.Key))) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Prepares handler to relayout.</summary>
        public virtual void PrepareHandlerToRelayout() {
            previousRenderersPages = new Dictionary<String, int?>(renderersPages);
        }

        private static iText.Layout.Renderer.TargetCounterHandler GetTargetCounterHandler(IRenderer renderer) {
            IRenderer rootRenderer = renderer;
            while (rootRenderer.GetParent() != null) {
                rootRenderer = rootRenderer.GetParent();
            }
            if (rootRenderer is DocumentRenderer) {
                return ((DocumentRenderer)rootRenderer).GetTargetCounterHandler();
            }
            return null;
        }
    }
}
