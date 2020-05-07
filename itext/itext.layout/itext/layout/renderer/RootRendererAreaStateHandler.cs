using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout.Layout;

namespace iText.Layout.Renderer {
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
                rootRenderer.currentPageNumber = storedPreviousArea.GetPageNumber();
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
                rootRenderer.currentPageNumber = storedNextArea.GetPageNumber();
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
}
