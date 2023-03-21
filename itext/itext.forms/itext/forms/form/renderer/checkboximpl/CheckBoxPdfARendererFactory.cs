using iText.Forms.Exceptions;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Renderer;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>A factory for creating CheckBoxPdfARenderer objects.</summary>
    public class CheckBoxPdfARendererFactory : AbstractCheckBoxRendererFactory {
        private static readonly Color DEFAULT_BORDER_COLOR = ColorConstants.DARK_GRAY;

        private static readonly Color DEFAULT_BACKGROUND_COLOR = ColorConstants.WHITE;

        // 1px
        private const float DEFAULT_BORDER_WIDTH = 0.75F;

        public CheckBoxPdfARendererFactory(CheckBoxRenderer checkBoxRenderer)
            : base(checkBoxRenderer) {
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer CreateFlatRenderer() {
            SetupSize();
            Paragraph paragraph = new Paragraph().SetWidth(GetSize()).SetHeight(GetSize()).SetBorder(new SolidBorder(DEFAULT_BORDER_COLOR
                , DEFAULT_BORDER_WIDTH)).SetBackgroundColor(DEFAULT_BACKGROUND_COLOR).SetMargin(0).SetHorizontalAlignment
                (HorizontalAlignment.CENTER);
            return new CheckBoxPdfARendererFactory.FlatParagraphRenderer(this, paragraph);
        }

        /// <summary><inheritDoc/></summary>
        protected internal class FlatParagraphRenderer : ParagraphRenderer {
            public FlatParagraphRenderer(CheckBoxPdfARendererFactory _enclosing, Paragraph modelElement)
                : base(modelElement) {
                this._enclosing = _enclosing;
            }

            public override void DrawChildren(DrawContext drawContext) {
                if (!this._enclosing.ShouldDrawChildren()) {
                    return;
                }
                PdfCanvas canvas = drawContext.GetCanvas();
                Rectangle rectangle = this.GetInnerAreaBBox().Clone();
                canvas.SaveState();
                canvas.SetFillColor(this._enclosing.GetFillColor());
                CheckBoxType checkBoxType = this._enclosing.GetCheckBoxType();
                CheckBoxPdfARendererFactory.DrawIcon(checkBoxType, canvas, rectangle);
                canvas.RestoreState();
            }

            private readonly CheckBoxPdfARendererFactory _enclosing;
        }

        private static void DrawIcon(CheckBoxType type, PdfCanvas canvas1, Rectangle rectangle) {
            switch (type) {
                case CheckBoxType.CHECK: {
                    DrawingUtil.DrawPdfACheck(canvas1, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                        .GetBottom());
                    break;
                }

                case CheckBoxType.CIRCLE: {
                    DrawingUtil.DrawPdfACircle(canvas1, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                        .GetBottom());
                    break;
                }

                case CheckBoxType.CROSS: {
                    DrawingUtil.DrawPdfACross(canvas1, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                        .GetBottom());
                    break;
                }

                case CheckBoxType.DIAMOND: {
                    DrawingUtil.DrawPdfADiamond(canvas1, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                        .GetBottom());
                    break;
                }

                case CheckBoxType.SQUARE: {
                    DrawingUtil.DrawPdfASquare(canvas1, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                        .GetBottom());
                    break;
                }

                case CheckBoxType.STAR: {
                    DrawingUtil.DrawPdfAStar(canvas1, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                        .GetBottom());
                    break;
                }

                default: {
                    throw new PdfException(FormsExceptionMessageConstant.CHECKBOX_TYPE_NOT_SUPPORTED);
                }
            }
        }
    }
}
