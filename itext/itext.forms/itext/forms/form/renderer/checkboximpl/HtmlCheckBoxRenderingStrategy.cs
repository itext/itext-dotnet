using iText.Forms.Form.Renderer;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>This class is used to draw a checkBox icon in HTML mode.</summary>
    public sealed class HtmlCheckBoxRenderingStrategy : ICheckBoxRenderingStrategy {
        public HtmlCheckBoxRenderingStrategy() {
        }

        // empty constructor
        /// <summary><inheritDoc/></summary>
        public void DrawCheckBoxContent(DrawContext drawContext, CheckBoxRenderer checkBoxRenderer, Rectangle rectangle
            ) {
            if (!checkBoxRenderer.IsBoxChecked()) {
                return;
            }
            PdfCanvas canvas = drawContext.GetCanvas();
            canvas.SaveState();
            canvas.SetFillColor(ColorConstants.BLACK);
            DrawingUtil.DrawPdfACheck(canvas, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                .GetBottom());
            canvas.RestoreState();
        }
    }
}
