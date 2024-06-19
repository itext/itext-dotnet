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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout.Layout;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal class RootRendererAreaStateHandler {
        private RootLayoutArea storedPreviousArea;

        private RootLayoutArea storedNextArea;

        private IList<Rectangle> storedPreviousFloatRenderAreas = null;

        private IList<Rectangle> storedNextFloatRenderAreas = null;

        public virtual bool AttemptGoBackToStoredPreviousStateAndStoreNextState(RootRenderer rootRenderer) {
            bool result = false;
            if (storedPreviousArea != null) {
                storedNextArea = rootRenderer.currentArea;
                rootRenderer.currentArea = storedPreviousArea;
                storedNextFloatRenderAreas = new List<Rectangle>(rootRenderer.floatRendererAreas);
                rootRenderer.floatRendererAreas = storedPreviousFloatRenderAreas;
                storedPreviousFloatRenderAreas = null;
                storedPreviousArea = null;
                result = true;
            }
            return result;
        }

        public virtual bool AttemptGoForwardToStoredNextState(RootRenderer rootRenderer) {
            if (storedNextArea != null) {
                rootRenderer.currentArea = storedNextArea;
                rootRenderer.floatRendererAreas = storedNextFloatRenderAreas;
                storedNextArea = null;
                storedNextFloatRenderAreas = null;
                return true;
            }
            else {
                return false;
            }
        }

        public virtual RootRendererAreaStateHandler StorePreviousState(RootRenderer rootRenderer) {
            storedPreviousArea = rootRenderer.currentArea;
            storedPreviousFloatRenderAreas = new List<Rectangle>(rootRenderer.floatRendererAreas);
            return this;
        }
    }
//\endcond
}
