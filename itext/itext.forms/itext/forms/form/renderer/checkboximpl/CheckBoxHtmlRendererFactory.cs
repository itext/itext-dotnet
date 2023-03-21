using iText.Commons.Utils;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Renderer;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>The factory class for creating a flat renderer for the checkbox in HTML rendering mode.</summary>
    public class CheckBoxHtmlRendererFactory : AbstractCheckBoxRendererFactory {
        private static readonly Color DEFAULT_BORDER_COLOR = ColorConstants.DARK_GRAY;

        private static readonly Color DEFAULT_BACKGROUND_COLOR = ColorConstants.WHITE;

        // 1px
        private const float DEFAULT_BORDER_WIDTH = 0.75F;

        // 11px
        private const float DEFAULT_SIZE = 8.25F;

        public CheckBoxHtmlRendererFactory(CheckBoxRenderer checkBoxRenderer)
            : base(checkBoxRenderer) {
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer CreateFlatRenderer() {
            //TODO DEVSIX-7426 make this method in parent class and overwrite if needed
            //TODO DEVSIX-7426 remove flag
            if (ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING) {
                SetupSize();
                Paragraph paragraph = new Paragraph().SetWidth(GetSize()).SetHeight(GetSize()).SetMargin(0).SetBorder(new 
                    SolidBorder(DEFAULT_BORDER_COLOR, DEFAULT_BORDER_WIDTH)).SetVerticalAlignment(VerticalAlignment.MIDDLE
                    ).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                return new CheckBoxHtmlRendererFactory.FlatParagraphRenderer(this, paragraph);
            }
            //TODO DEVSIX-7426 remove this
            Paragraph paragraph_1 = new Paragraph().SetWidth(DEFAULT_SIZE).SetHeight(DEFAULT_SIZE).SetBorder(new SolidBorder
                (DEFAULT_BORDER_COLOR, DEFAULT_BORDER_WIDTH)).SetBackgroundColor(DEFAULT_BACKGROUND_COLOR).SetHorizontalAlignment
                (HorizontalAlignment.CENTER);
            return new CheckBoxHtmlRendererFactory.FlatParagraphRenderer(this, paragraph_1);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override float GetDefaultSize() {
            return DEFAULT_SIZE;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override Background GetDefaultColor() {
            return new Background(DEFAULT_BACKGROUND_COLOR, 1F);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override CheckBoxType GetDefaultCheckBoxType() {
            return CheckBoxType.CHECK;
        }

        internal class FlatParagraphRenderer : ParagraphRenderer {
            public FlatParagraphRenderer(CheckBoxHtmlRendererFactory _enclosing, Paragraph modelElement)
                : base(modelElement) {
                this._enclosing = _enclosing;
            }

            public override void DrawChildren(DrawContext drawContext) {
                if (!this._enclosing.ShouldDrawChildren()) {
                    return;
                }
                PdfCanvas canvas = drawContext.GetCanvas();
                Rectangle rectangle = this.GetInnerAreaBBox();
                canvas.SaveState();
                canvas.SetFillColor(ColorConstants.BLACK);
                DrawingUtil.DrawPdfACheck(canvas, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                    .GetBottom());
                canvas.RestoreState();
            }

            private readonly CheckBoxHtmlRendererFactory _enclosing;
        }
    }
}
