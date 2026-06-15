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
using System.Collections.Generic;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    // TODO DEVSIX-10004: Remove after the change
    /// <summary>The class stores logic for removing renderers from a renderer tree.</summary>
    internal sealed class RendererRemovalUtil {
        private RendererRemovalUtil() {
        }

//\cond DO_NOT_DOCUMENT
        // do nothing
        /// <summary>Removes all SectionBreak and AreaBreak instances from the renderer tree.</summary>
        /// <param name="renderer">
        /// 
        /// <see cref="IRenderer"/>
        /// from which the renderers will be removed
        /// </param>
        /// <returns><c>boolean</c> value indicating whether a removal occurred.</returns>
        internal static bool RemoveAreaBreakAndSectionBreakDescendants(IRenderer renderer) {
            bool rendererRemoved = false;
            IList<IRenderer> descendants = new List<IRenderer>();
            descendants.Add(renderer);
            while (!descendants.IsEmpty()) {
                IRenderer descendant = descendants.JRemoveAt(descendants.Count - 1);
                if (descendant == null) {
                    continue;
                }
                if (descendant is TableRenderer) {
                    TableRenderer tableRenderer = (TableRenderer)descendant;
                    descendants.Add(tableRenderer.headerRenderer);
                    descendants.Add(tableRenderer.footerRenderer);
                }
                IList<IRenderer> descendantChildRenderers = descendant.GetChildRenderers();
                if (descendantChildRenderers == null) {
                    continue;
                }
                bool childOfDescendantRemoved = descendantChildRenderers.RemoveIf((childOfDescendant) => childOfDescendant
                     is AreaBreakRenderer || childOfDescendant is SectionBreakRenderer);
                if (childOfDescendantRemoved) {
                    rendererRemoved = true;
                }
                descendants.AddAll(descendantChildRenderers);
            }
            return rendererRemoved;
        }
//\endcond
    }
//\endcond
}
