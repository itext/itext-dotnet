using System;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    public class AbstractRenderInfo : IEventData {
        protected internal CanvasGraphicsState gs;

        private bool graphicsStateIsPreserved;

        public AbstractRenderInfo(CanvasGraphicsState gs) {
            this.gs = gs;
        }

        public virtual CanvasGraphicsState GetGraphicsState() {
            CheckGraphicsState();
            return graphicsStateIsPreserved ? gs : new CanvasGraphicsState(gs);
        }

        public virtual bool IsGraphicsStatePreserved() {
            return graphicsStateIsPreserved;
        }

        public virtual void PreserveGraphicsState() {
            CheckGraphicsState();
            this.graphicsStateIsPreserved = true;
            gs = new CanvasGraphicsState(gs);
        }

        public virtual void ReleaseGraphicsState() {
            if (!graphicsStateIsPreserved) {
                gs = null;
            }
        }

        // check if graphics state was released
        protected internal virtual void CheckGraphicsState() {
            if (null == gs) {
                throw new InvalidOperationException(iText.IO.LogMessageConstant.GRAPHICS_STATE_WAS_DELETED);
            }
        }
    }
}
