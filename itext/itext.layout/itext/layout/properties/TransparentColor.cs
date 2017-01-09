using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;

namespace iText.Layout.Properties {
    public class TransparentColor {
        private Color color;

        private float opacity;

        public TransparentColor(Color color) {
            this.color = color;
            this.opacity = 1f;
        }

        public TransparentColor(Color color, float opacity) {
            this.color = color;
            this.opacity = opacity;
        }

        public virtual Color GetColor() {
            return color;
        }

        public virtual float GetOpacity() {
            return opacity;
        }

        public virtual void ApplyFillTransparency(PdfCanvas canvas) {
            ApplyTransparency(canvas, false);
        }

        public virtual void ApplyStrokeTransparency(PdfCanvas canvas) {
            ApplyTransparency(canvas, true);
        }

        private void ApplyTransparency(PdfCanvas canvas, bool isStroke) {
            if (IsTransparent()) {
                PdfExtGState extGState = new PdfExtGState();
                if (isStroke) {
                    extGState.SetStrokeOpacity(opacity);
                }
                else {
                    extGState.SetFillOpacity(opacity);
                }
                canvas.SetExtGState(extGState);
            }
        }

        private bool IsTransparent() {
            return opacity < 1f;
        }
    }
}
