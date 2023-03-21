using System;
using System.Collections.Generic;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Renderer;
using iText.Forms.Logs;
using iText.Forms.Util;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>A factory for creating CheckBoxPdfRenderer objects.</summary>
    public class CheckBoxPdfRendererFactory : AbstractCheckBoxRendererFactory {
        private static readonly Dictionary<CheckBoxType, String> CHECKBOX_TYPE_ZAPFDINGBATS_CODE = new Dictionary<
            CheckBoxType, String>();

        static CheckBoxPdfRendererFactory() {
            CHECKBOX_TYPE_ZAPFDINGBATS_CODE.Put(CheckBoxType.CHECK, "4");
            CHECKBOX_TYPE_ZAPFDINGBATS_CODE.Put(CheckBoxType.CIRCLE, "l");
            CHECKBOX_TYPE_ZAPFDINGBATS_CODE.Put(CheckBoxType.CROSS, "8");
            CHECKBOX_TYPE_ZAPFDINGBATS_CODE.Put(CheckBoxType.DIAMOND, "u");
            CHECKBOX_TYPE_ZAPFDINGBATS_CODE.Put(CheckBoxType.SQUARE, "n");
            CHECKBOX_TYPE_ZAPFDINGBATS_CODE.Put(CheckBoxType.STAR, "H");
        }

        public CheckBoxPdfRendererFactory(CheckBoxRenderer checkBoxRenderer)
            : base(checkBoxRenderer) {
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer CreateFlatRenderer() {
            SetupSize();
            Paragraph paragraph = new Paragraph().SetWidth(GetSize()).SetHeight(GetSize()).SetMargin(0).SetVerticalAlignment
                (VerticalAlignment.MIDDLE).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            return new CheckBoxPdfRendererFactory.FlatParagraphRenderer(this, paragraph);
        }

        protected internal class FlatParagraphRenderer : ParagraphRenderer {
            public FlatParagraphRenderer(CheckBoxPdfRendererFactory _enclosing, Paragraph modelElement)
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
                canvas.SetFillColor(this._enclosing.GetFillColor());
                // matrix transformation to draw the checkbox in the right place
                // because we come here with relative and not absolute coordinates
                canvas.ConcatMatrix(1, 0, 0, 1, rectangle.GetLeft(), rectangle.GetBottom());
                float fontSize = this.GetPropertyAsUnitValue(Property.HEIGHT).GetValue();
                CheckBoxType checkBoxType = this._enclosing.GetCheckBoxType();
                if (checkBoxType == CheckBoxType.CROSS) {
                    DrawingUtil.DrawCross(canvas, rectangle.GetWidth(), rectangle.GetHeight(), 1);
                    canvas.RestoreState();
                    return;
                }
                PdfFont fontContainingSymbols;
                try {
                    fontContainingSymbols = PdfFontFactory.CreateFont(StandardFonts.ZAPFDINGBATS);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                if (fontSize <= 0) {
                    throw new PdfException(FormsLogMessageConstants.CHECKBOX_FONT_SIZE_IS_NOT_POSITIVE);
                }
                String text = CheckBoxPdfRendererFactory.CHECKBOX_TYPE_ZAPFDINGBATS_CODE.Get(checkBoxType);
                canvas.BeginText().SetFontAndSize(fontContainingSymbols, fontSize).ResetFillColorRgb().SetTextMatrix((rectangle
                    .GetWidth() - fontContainingSymbols.GetWidth(text, fontSize)) / 2, (rectangle.GetHeight() - fontContainingSymbols
                    .GetAscent(text, fontSize)) / 2).ShowText(text).EndText();
                canvas.RestoreState();
            }

            private readonly CheckBoxPdfRendererFactory _enclosing;
        }
    }
}
