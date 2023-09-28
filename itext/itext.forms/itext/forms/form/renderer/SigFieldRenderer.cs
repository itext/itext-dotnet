/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form.Element;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractTextFieldRenderer"/>
    /// implementation for SigFields.
    /// </summary>
    public class SigFieldRenderer : AbstractTextFieldRenderer {
        /// <summary>Extra space at the top.</summary>
        private const float TOP_SECTION = 0.3f;

        private const float EPS = 1e-5f;

        private bool isFontSizeApproximated = false;

        /// <summary>
        /// Creates a new
        /// <see cref="SigFieldRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public SigFieldRenderer(SigField modelElement)
            : base(modelElement) {
            ApplyBackgroundImage(modelElement);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override bool IsLayoutBasedOnFlatRenderer() {
            return false;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override IRenderer CreateFlatRenderer() {
            Div div = new Div();
            String description = ((SigField)modelElement).GetDescription(true);
            SigField.RenderingMode renderingMode = ((SigField)modelElement).GetRenderingMode();
            switch (renderingMode) {
                case SigField.RenderingMode.NAME_AND_DESCRIPTION: {
                    div.Add(new Paragraph(((SigField)modelElement).GetSignedBy()).SetMargin(0).SetMultipliedLeading(0.9f)).Add
                        (new Paragraph(description).SetMargin(0).SetMultipliedLeading(0.9f));
                    break;
                }

                case SigField.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
                    ImageData signatureGraphic = ((SigField)modelElement).GetSignatureGraphic();
                    if (signatureGraphic == null) {
                        throw new InvalidOperationException("A signature image must be present when rendering mode is " + "graphic and description. Use setSignatureGraphic()"
                            );
                    }
                    div.Add(new iText.Layout.Element.Image(signatureGraphic)).Add(new Paragraph(description).SetMargin(0).SetMultipliedLeading
                        (0.9f));
                    break;
                }

                case SigField.RenderingMode.GRAPHIC: {
                    ImageData signatureGraphic_1 = ((SigField)modelElement).GetSignatureGraphic();
                    if (signatureGraphic_1 == null) {
                        throw new InvalidOperationException("A signature image must be present when rendering mode is " + "graphic. Use setSignatureGraphic()"
                            );
                    }
                    div.Add(new iText.Layout.Element.Image(signatureGraphic_1));
                    break;
                }

                default: {
                    div.Add(new Paragraph(description).SetMargin(0).SetMultipliedLeading(0.9f));
                    break;
                }
            }
            return div.CreateRendererSubTree();
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            ApproximateFontSizeToFitLayoutArea(layoutContext);
            return base.Layout(layoutContext);
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="layoutContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
            Rectangle bBox = GetOccupiedArea().GetBBox().Clone();
            ApplyPaddings(bBox, false);
            ApplyBorderBox(bBox, false);
            if (bBox.GetY() < 0) {
                bBox.SetHeight(bBox.GetY() + bBox.GetHeight());
                bBox.SetY(0);
            }
            Rectangle descriptionRect = null;
            Rectangle signatureRect = null;
            SigField.RenderingMode renderingMode = ((SigField)modelElement).GetRenderingMode();
            switch (renderingMode) {
                case SigField.RenderingMode.NAME_AND_DESCRIPTION:
                case SigField.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
                    // Split the signature field into two and add the name of the signer or an image to the one side,
                    // the description to the other side.
                    UnitValue[] paddings = GetPaddings();
                    if (bBox.GetHeight() > bBox.GetWidth()) {
                        float topPadding = paddings[0].GetValue();
                        float bottomPadding = paddings[2].GetValue();
                        signatureRect = new Rectangle(bBox.GetX(), bBox.GetY() + bBox.GetHeight() / 2 + bottomPadding / 2, bBox.GetWidth
                            (), bBox.GetHeight() / 2 - bottomPadding / 2);
                        descriptionRect = new Rectangle(bBox.GetX(), bBox.GetY(), bBox.GetWidth(), bBox.GetHeight() / 2 - topPadding
                             / 2);
                    }
                    else {
                        // origin is the bottom-left
                        float rightPadding = paddings[1].GetValue();
                        float leftPadding = paddings[3].GetValue();
                        signatureRect = new Rectangle(bBox.GetX(), bBox.GetY(), bBox.GetWidth() / 2 - rightPadding / 2, bBox.GetHeight
                            ());
                        descriptionRect = new Rectangle(bBox.GetX() + bBox.GetWidth() / 2 + leftPadding / 2, bBox.GetY(), bBox.GetWidth
                            () / 2 - leftPadding / 2, bBox.GetHeight());
                    }
                    break;
                }

                case SigField.RenderingMode.GRAPHIC: {
                    // The signature field will consist of an image only; no description will be shown.
                    signatureRect = bBox;
                    break;
                }

                default: {
                    // Default one, it just shows whatever description was defined for the signature.
                    if (RetrieveHeight() == null) {
                        // Adjust calculated occupied area height to keep the same font size.
                        float calculatedHeight = GetOccupiedArea().GetBBox().GetHeight();
                        GetOccupiedArea().GetBBox().MoveDown(calculatedHeight * TOP_SECTION).SetHeight(calculatedHeight * (1 + TOP_SECTION
                            ));
                        bBox.MoveDown(calculatedHeight * TOP_SECTION);
                    }
                    descriptionRect = bBox.SetHeight(GetOccupiedArea().GetBBox().GetHeight() * (1 - TOP_SECTION) - CalculateAdditionalHeight
                        ());
                    break;
                }
            }
            AdjustChildrenLayout(renderingMode, signatureRect, descriptionRect, layoutContext.GetArea().GetPageNumber(
                ));
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.SigFieldRenderer((SigField)modelElement);
        }

        /// <summary>Gets the default value of the form field.</summary>
        /// <returns>the default value of the form field.</returns>
        public override String GetDefaultValue() {
            // FormProperty.FORM_FIELD_VALUE is not supported for SigField element.
            return "";
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        protected internal override void ApplyAcroField(DrawContext drawContext) {
            String name = GetModelId();
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.SigFieldRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
            }
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = GetOccupiedArea().GetBBox().Clone();
            ApplyMargins(area, false);
            DeleteMargins();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            // Background is light gray by default, but can be set to null by user.
            Color backgroundColor = background == null ? null : background.GetColor();
            float fontSizeValue = fontSize.GetValue();
            if (font == null) {
                font = doc.GetDefaultFont();
            }
            // Some properties are set to the HtmlDocumentRenderer, which is root renderer for this SigFieldRenderer, but
            // in forms logic root renderer is CanvasRenderer, and these properties will have default values. So
            // we get them from renderer and set these properties to model element, which will be passed to forms logic.
            modelElement.SetProperty(Property.FONT_PROVIDER, this.GetProperty<FontProvider>(Property.FONT_PROVIDER));
            modelElement.SetProperty(Property.RENDERING_MODE, this.GetProperty<RenderingMode?>(Property.RENDERING_MODE
                ));
            PdfSignatureFormField sigField = new SignatureFormFieldBuilder(doc, name).SetWidgetRectangle(area).CreateSignature
                ();
            sigField.DisableFieldRegeneration();
            sigField.SetFont(font).SetFontSize(fontSizeValue);
            sigField.GetFirstFormAnnotation().SetBackgroundColor(backgroundColor);
            ApplyDefaultFieldProperties(sigField);
            sigField.GetFirstFormAnnotation().SetFormFieldElement((SigField)modelElement);
            sigField.EnableFieldRegeneration();
            PdfAcroForm forms = PdfFormCreator.GetAcroForm(doc, true);
            forms.AddField(sigField, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        private void AdjustChildrenLayout(SigField.RenderingMode renderingMode, Rectangle signatureRect, Rectangle
             descriptionRect, int pageNum) {
            switch (renderingMode) {
                case SigField.RenderingMode.NAME_AND_DESCRIPTION: {
                    ParagraphRenderer name = (ParagraphRenderer)flatRenderer.GetChildRenderers()[0];
                    RelayoutParagraph(name, signatureRect, pageNum);
                    ParagraphRenderer description = (ParagraphRenderer)flatRenderer.GetChildRenderers()[1];
                    RelayoutParagraph(description, descriptionRect, pageNum);
                    break;
                }

                case SigField.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
                    RelayoutImage(signatureRect, pageNum);
                    ParagraphRenderer description = (ParagraphRenderer)flatRenderer.GetChildRenderers()[1];
                    RelayoutParagraph(description, descriptionRect, pageNum);
                    break;
                }

                case SigField.RenderingMode.GRAPHIC: {
                    RelayoutImage(signatureRect, pageNum);
                    break;
                }

                default: {
                    ParagraphRenderer description = (ParagraphRenderer)flatRenderer.GetChildRenderers()[0];
                    RelayoutParagraph(description, descriptionRect, pageNum);
                    break;
                }
            }
            // Apply vertical alignment for children including floats.
            VerticalAlignment? verticalAlignment = this.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT);
            float multiplier = 0;
            if (VerticalAlignment.MIDDLE == verticalAlignment) {
                multiplier = 0.5f;
            }
            else {
                if (VerticalAlignment.BOTTOM == verticalAlignment) {
                    multiplier = 1;
                }
            }
            float lowestChildBottom = GetLowestChildBottom(flatRenderer, GetInnerAreaBBox().GetTop());
            float deltaY = lowestChildBottom - GetInnerAreaBBox().GetY();
            if (deltaY > 0) {
                flatRenderer.Move(0, -deltaY * multiplier);
            }
        }

        private void RelayoutImage(Rectangle signatureRect, int pageNum) {
            ImageRenderer image = (ImageRenderer)flatRenderer.GetChildRenderers()[0];
            Rectangle imageBBox = image.GetOccupiedArea().GetBBox();
            float imgWidth = imageBBox.GetWidth();
            if (imgWidth < EPS) {
                imgWidth = signatureRect.GetWidth();
            }
            float imgHeight = imageBBox.GetHeight();
            if (imgHeight < EPS) {
                imgHeight = signatureRect.GetHeight();
            }
            float multiplierH = signatureRect.GetWidth() / imgWidth;
            float multiplierW = signatureRect.GetHeight() / imgHeight;
            float multiplier = Math.Min(multiplierH, multiplierW);
            imgWidth *= multiplier;
            imgHeight *= multiplier;
            float x = signatureRect.GetLeft() + (signatureRect.GetWidth() - imgWidth) / 2;
            float y = signatureRect.GetBottom() + (signatureRect.GetHeight() - imgHeight) / 2;
            // We need to re-layout image since signature was divided into 2 parts and bBox was changed.
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(pageNum, new Rectangle(x, y, imgWidth, imgHeight
                )));
            image.GetModelElement().SetProperty(Property.WIDTH, UnitValue.CreatePointValue(imgWidth));
            image.GetModelElement().SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(imgHeight));
            image.Layout(layoutContext);
        }

        private void RelayoutParagraph(IRenderer renderer, Rectangle rect, int pageNum) {
            UnitValue fontSizeAsUV = this.HasOwnProperty(Property.FONT_SIZE) ? (UnitValue)this.GetOwnProperty<UnitValue
                >(Property.FONT_SIZE) : (UnitValue)modelElement.GetOwnProperty<UnitValue>(Property.FONT_SIZE);
            if (fontSizeAsUV == null || fontSizeAsUV.GetValue() < EPS || isFontSizeApproximated) {
                // Calculate font size.
                IRenderer helper = ((Paragraph)renderer.GetModelElement()).CreateRendererSubTree().SetParent(renderer.GetParent
                    ());
                this.DeleteProperty(Property.FONT_SIZE);
                LayoutContext layoutContext = new LayoutContext(new LayoutArea(pageNum, rect));
                float lFontSize = 0.1f;
                float rFontSize = 100;
                int numberOfIterations = 15;
                // 15 iterations with lFontSize = 0.1 and rFontSize = 100 should result in ~0.003 precision.
                float fontSize = CalculateFittingFontSize(helper, lFontSize, rFontSize, layoutContext, numberOfIterations);
                renderer.GetModelElement().SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(fontSize));
            }
            // Relayout the element after font size was changed or signature was split into 2 parts.
            LayoutContext layoutContext_1 = new LayoutContext(new LayoutArea(pageNum, rect));
            renderer.Layout(layoutContext_1);
        }

        private void ApplyBackgroundImage(SigField modelElement) {
            if (modelElement.GetImage() != null) {
                BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT);
                BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetPositionY
                    (BackgroundPosition.PositionY.CENTER);
                BackgroundSize size = new BackgroundSize();
                if (Math.Abs(modelElement.GetImageScale()) < EPS) {
                    size.SetBackgroundSizeToValues(UnitValue.CreatePercentValue(100), UnitValue.CreatePercentValue(100));
                }
                else {
                    float imageScale = modelElement.GetImageScale();
                    if (imageScale < 0) {
                        size.SetBackgroundSizeToContain();
                    }
                    else {
                        size.SetBackgroundSizeToValues(UnitValue.CreatePointValue(imageScale * modelElement.GetImage().GetWidth())
                            , UnitValue.CreatePointValue(imageScale * modelElement.GetImage().GetHeight()));
                    }
                }
                modelElement.SetBackgroundImage(new BackgroundImage.Builder().SetImage(new PdfImageXObject(modelElement.GetImage
                    ())).SetBackgroundSize(size).SetBackgroundRepeat(repeat).SetBackgroundPosition(position).Build());
            }
        }

        private float CalculateAdditionalHeight() {
            Rectangle dummy = new Rectangle(0, 0);
            this.ApplyMargins(dummy, true);
            this.ApplyBorderBox(dummy, true);
            this.ApplyPaddings(dummy, true);
            return dummy.GetHeight();
        }

        private void ApproximateFontSizeToFitLayoutArea(LayoutContext layoutContext) {
            if (this.HasOwnProperty(Property.FONT_SIZE) || modelElement.HasOwnProperty(Property.FONT_SIZE)) {
                return;
            }
            if (SigField.RenderingMode.GRAPHIC == ((SigField)modelElement).GetRenderingMode() || SigField.RenderingMode
                .GRAPHIC_AND_DESCRIPTION == ((SigField)modelElement).GetRenderingMode()) {
                // We can expect CLIP_ELEMENT log messages since the initial image size may be larger than the field height.
                // But image size will be adjusted during its relayout in #adjustFieldLayout.
                return;
            }
            float fontSize = ApproximateFontSize(layoutContext, 0.1f, AbstractPdfFormField.DEFAULT_FONT_SIZE);
            if (fontSize > 0) {
                isFontSizeApproximated = true;
                modelElement.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(fontSize));
            }
        }
    }
}
