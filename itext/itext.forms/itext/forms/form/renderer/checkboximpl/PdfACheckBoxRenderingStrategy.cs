using iText.Forms.Exceptions;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>This class is used to draw a checkBox icon in PDF/A mode.</summary>
    public sealed class PdfACheckBoxRenderingStrategy : ICheckBoxRenderingStrategy {
        /// <summary>
        /// Creates a new
        /// <see cref="PdfACheckBoxRenderingStrategy"/>
        /// instance.
        /// </summary>
        public PdfACheckBoxRenderingStrategy() {
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
            canvas.SetFillColor(ColorConstants.RED);
            CheckBoxType checkBoxType = (CheckBoxType)checkBoxRenderer.GetProperty<CheckBoxType?>(FormProperty.FORM_CHECKBOX_TYPE
                , CheckBoxType.CROSS);
            DrawIcon(checkBoxType, canvas, rectangle);
            canvas.RestoreState();
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
