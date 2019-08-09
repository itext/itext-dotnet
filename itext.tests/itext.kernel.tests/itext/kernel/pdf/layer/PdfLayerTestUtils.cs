using System;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Layer {
    internal class PdfLayerTestUtils {
        public static void AddTextInsideLayer(IPdfOCG layer, PdfCanvas canvas, String text, float x, float y) {
            canvas.BeginLayer(layer).BeginText().MoveText(x, y).ShowText(text).EndText().EndLayer();
        }
    }
}
