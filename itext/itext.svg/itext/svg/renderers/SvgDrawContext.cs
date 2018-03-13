using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers {
    /// <summary>
    /// The SvgDrawContext keeps a stack of
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
    /// instances, which
    /// represent all levels of XObjects that are added to the root canvas.
    /// </summary>
    public class SvgDrawContext {
        private readonly Stack<PdfCanvas> canvases = new Stack<PdfCanvas>();

        /// <summary>Retrieves the current top of the stack, without modifying the stack.</summary>
        /// <returns>the current canvas that can be used for drawing operations.</returns>
        public virtual PdfCanvas GetCurrentCanvas() {
            return canvases.Peek();
        }

        /// <summary>
        /// Retrieves the current top of the stack, thereby taking the current item
        /// off the stack.
        /// </summary>
        /// <returns>the current canvas that can be used for drawing operations.</returns>
        public virtual PdfCanvas PopCanvas() {
            return canvases.Pop();
        }

        /// <summary>
        /// Adds a
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to the stack (by definition its top), for use in
        /// drawing operations.
        /// </summary>
        /// <param name="canvas">the new top of the stack</param>
        public virtual void PushCanvas(PdfCanvas canvas) {
            canvases.Push(canvas);
        }

        /// <summary>
        /// Get the current size of the stack, signifying the nesting level of the
        /// XObjects.
        /// </summary>
        /// <returns>the current size of the stack.</returns>
        public virtual int Size() {
            return canvases.Count;
        }
    }
}
