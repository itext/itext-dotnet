/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
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
    public class SignatureAppearanceRenderer : AbstractTextFieldRenderer {
        /// <summary>Extra space at the top.</summary>
        private const float TOP_SECTION = 0.3f;

        private const float EPS = 1e-5f;

        private readonly SignatureAppearanceRenderer.RenderingMode renderingMode;

        private bool isFontSizeApproximated = false;

        /// <summary>
        /// Creates a new
        /// <see cref="SignatureAppearanceRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public SignatureAppearanceRenderer(SignatureFieldAppearance modelElement)
            : base(modelElement) {
            renderingMode = RetrieveRenderingMode();
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
            foreach (IElement element in ((SignatureFieldAppearance)modelElement).GetContentElements()) {
                if (element is Image) {
                    div.Add((Image)element);
                }
                else {
                    div.Add((IBlockElement)element);
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
            switch (renderingMode) {
                case SignatureAppearanceRenderer.RenderingMode.NAME_AND_DESCRIPTION:
                case SignatureAppearanceRenderer.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
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

                case SignatureAppearanceRenderer.RenderingMode.GRAPHIC: {
                    // The signature field will consist of an image only; no description will be shown.
                    signatureRect = bBox;
                    break;
                }

                case SignatureAppearanceRenderer.RenderingMode.DESCRIPTION: {
                    // Default one, it just shows whatever description was defined for the signature.
                    float additionalHeight = CalculateAdditionalHeight();
                    if (RetrieveHeight() == null) {
                        // Adjust calculated occupied area height to keep the same font size.
                        float calculatedHeight = GetOccupiedArea().GetBBox().GetHeight();
                        // (calcHeight + addHeight + topSect) * (1 - TOP_SECTION) - addHeight = calcHeight, =>
                        float topSection = (calculatedHeight + additionalHeight) * TOP_SECTION / (1 - TOP_SECTION);
                        GetOccupiedArea().GetBBox().MoveDown(topSection + additionalHeight).SetHeight(calculatedHeight + topSection
                             + additionalHeight);
                        bBox.MoveDown(bBox.GetBottom() - GetOccupiedArea().GetBBox().GetBottom() - additionalHeight / 2);
                    }
                    descriptionRect = bBox.SetHeight(GetOccupiedArea().GetBBox().GetHeight() * (1 - TOP_SECTION) - additionalHeight
                        );
                    break;
                }

                default: {
                    return;
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
            return new iText.Forms.Form.Renderer.SignatureAppearanceRenderer((SignatureFieldAppearance)modelElement);
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
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.SignatureAppearanceRenderer));
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
            modelElement.SetProperty(Property.RENDERING_MODE, this.GetProperty<SignatureAppearanceRenderer.RenderingMode?
                >(Property.RENDERING_MODE));
            PdfSignatureFormField sigField = new SignatureFormFieldBuilder(doc, name).SetWidgetRectangle(area).SetConformanceLevel
                (GetConformanceLevel(doc)).SetFont(font).CreateSignature();
            sigField.DisableFieldRegeneration();
            sigField.SetFontSize(fontSizeValue);
            sigField.GetFirstFormAnnotation().SetBackgroundColor(backgroundColor);
            ApplyDefaultFieldProperties(sigField);
            sigField.GetFirstFormAnnotation().SetFormFieldElement((SignatureFieldAppearance)modelElement);
            sigField.EnableFieldRegeneration();
            PdfAcroForm forms = PdfFormCreator.GetAcroForm(doc, true);
            forms.AddField(sigField, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        private void AdjustChildrenLayout(SignatureAppearanceRenderer.RenderingMode renderingMode, Rectangle signatureRect
            , Rectangle descriptionRect, int pageNum) {
            switch (renderingMode) {
                case SignatureAppearanceRenderer.RenderingMode.NAME_AND_DESCRIPTION: {
                    ParagraphRenderer name = (ParagraphRenderer)flatRenderer.GetChildRenderers()[0];
                    RelayoutParagraph(name, signatureRect, pageNum);
                    ParagraphRenderer description = (ParagraphRenderer)flatRenderer.GetChildRenderers()[1];
                    RelayoutParagraph(description, descriptionRect, pageNum);
                    break;
                }

                case SignatureAppearanceRenderer.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
                    RelayoutImage(signatureRect, pageNum);
                    ParagraphRenderer description = (ParagraphRenderer)flatRenderer.GetChildRenderers()[1];
                    RelayoutParagraph(description, descriptionRect, pageNum);
                    break;
                }

                case SignatureAppearanceRenderer.RenderingMode.GRAPHIC: {
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
            if (SignatureAppearanceRenderer.RenderingMode.GRAPHIC == renderingMode || SignatureAppearanceRenderer.RenderingMode
                .GRAPHIC_AND_DESCRIPTION == renderingMode || SignatureAppearanceRenderer.RenderingMode.CUSTOM == renderingMode
                ) {
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

        private SignatureAppearanceRenderer.RenderingMode RetrieveRenderingMode() {
            IList<IElement> contentElements = ((SignatureFieldAppearance)modelElement).GetContentElements();
            if (contentElements.Count == 2 && contentElements[1] is Paragraph) {
                if (contentElements[0] is Paragraph) {
                    return SignatureAppearanceRenderer.RenderingMode.NAME_AND_DESCRIPTION;
                }
                if (contentElements[0] is Image) {
                    return SignatureAppearanceRenderer.RenderingMode.GRAPHIC_AND_DESCRIPTION;
                }
            }
            if (contentElements.Count == 1) {
                if (contentElements[0] is Paragraph) {
                    return SignatureAppearanceRenderer.RenderingMode.DESCRIPTION;
                }
                if (contentElements[0] is Image) {
                    return SignatureAppearanceRenderer.RenderingMode.GRAPHIC;
                }
            }
            return SignatureAppearanceRenderer.RenderingMode.CUSTOM;
        }

        /// <summary>Signature rendering modes.</summary>
        private enum RenderingMode {
            /// <summary>The rendering mode is just the description.</summary>
            DESCRIPTION,
            /// <summary>The rendering mode is the name of the signer and the description.</summary>
            NAME_AND_DESCRIPTION,
            /// <summary>The rendering mode is an image and the description.</summary>
            GRAPHIC_AND_DESCRIPTION,
            /// <summary>The rendering mode is just an image.</summary>
            GRAPHIC,
            /// <summary>The rendering mode is div.</summary>
            CUSTOM
        }
    }
}
