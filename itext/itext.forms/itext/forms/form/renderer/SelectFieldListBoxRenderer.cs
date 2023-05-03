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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="SelectFieldListBoxRenderer"/>
    /// implementation for select field renderer.
    /// </summary>
    public class SelectFieldListBoxRenderer : AbstractSelectFieldRenderer {
        /// <summary>
        /// Creates a new
        /// <see cref="SelectFieldListBoxRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public SelectFieldListBoxRenderer(AbstractSelectField modelElement)
            : base(modelElement) {
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.SelectFieldListBoxRenderer((AbstractSelectField)modelElement);
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            LayoutResult layoutResult = base.Layout(layoutContext);
            // options container is the only kid of the select field renderer by design
            IRenderer optionsContainer = childRenderers.Count == 1 ? childRenderers[0] : null;
            if (!IsFlatten() || layoutResult.GetStatus() != LayoutResult.FULL || optionsContainer == null || optionsContainer
                .GetOccupiedArea() == null) {
                return layoutResult;
            }
            if (IsOverflowProperty(OverflowPropertyValue.HIDDEN, this, Property.OVERFLOW_Y)) {
                IList<IRenderer> selectedOptions = GetSelectedOptions(this);
                IRenderer firstSelectedOption;
                if (!selectedOptions.IsEmpty() && (firstSelectedOption = selectedOptions[0]).GetOccupiedArea() != null) {
                    Rectangle borderAreaBBox = GetBorderAreaBBox();
                    Rectangle optionBBox = firstSelectedOption.GetOccupiedArea().GetBBox().Clone();
                    if (firstSelectedOption is AbstractRenderer) {
                        ((AbstractRenderer)firstSelectedOption).ApplyMargins(optionBBox, false);
                    }
                    if (optionBBox.GetHeight() < borderAreaBBox.GetHeight()) {
                        float selectedBottom = optionBBox.GetBottom();
                        float borderAreaBBoxBottom = borderAreaBBox.GetBottom();
                        if (selectedBottom < borderAreaBBoxBottom) {
                            optionsContainer.Move(0, borderAreaBBoxBottom - selectedBottom);
                        }
                    }
                    else {
                        optionsContainer.Move(0, borderAreaBBox.GetTop() - optionBBox.GetTop());
                    }
                }
            }
            return layoutResult;
        }

        protected override bool AllowLastYLineRecursiveExtraction() {
            return false;
        }

        protected internal override IRenderer CreateFlatRenderer() {
            AbstractSelectField selectField = (AbstractSelectField)modelElement;
            IList<IBlockElement> options = selectField.GetOptions();
            Div optionsContainer = new Div();
            foreach (IBlockElement option in options) {
                optionsContainer.Add(option);
            }
            String lang = GetLang();
            if (lang != null) {
                AccessibilityProperties properties = optionsContainer.GetAccessibilityProperties();
                if (properties.GetLanguage() == null) {
                    properties.SetLanguage(lang);
                }
            }
            IRenderer rendererSubTree;
            if (optionsContainer.GetChildren().IsEmpty()) {
                Paragraph pStub = new Paragraph("\u00A0").SetMargin(0);
                pStub.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                pStub.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                // applying this property for the sake of finding this element as option
                pStub.SetProperty(FormProperty.FORM_FIELD_SELECTED, false);
                optionsContainer.Add(pStub);
                rendererSubTree = optionsContainer.CreateRendererSubTree();
            }
            else {
                rendererSubTree = optionsContainer.CreateRendererSubTree();
                IList<IRenderer> selectedOptions = GetSelectedOptions(rendererSubTree);
                foreach (IRenderer selectedOption in selectedOptions) {
                    ApplySelectedStyle(selectedOption);
                }
            }
            return rendererSubTree;
        }

        protected internal override float GetFinalSelectFieldHeight(float availableHeight, float actualHeight, bool
             isClippedHeight) {
            float? height = RetrieveHeight();
            float calculatedHeight;
            if (height == null) {
                calculatedHeight = GetCalculatedHeight(this);
                float? maxHeight = RetrieveMaxHeight();
                if (maxHeight != null && maxHeight < calculatedHeight) {
                    calculatedHeight = (float)maxHeight;
                }
                float? minHeight = RetrieveMinHeight();
                if (minHeight != null && minHeight > calculatedHeight) {
                    calculatedHeight = (float)minHeight;
                }
            }
            else {
                calculatedHeight = height.Value;
            }
            return base.GetFinalSelectFieldHeight(availableHeight, calculatedHeight, isClippedHeight);
        }

        protected internal override void ApplyAcroField(DrawContext drawContext) {
            // Retrieve font properties
            Object retrievedFont = this.GetProperty<Object>(Property.FONT);
            PdfFont font = retrievedFont is PdfFont ? (PdfFont)retrievedFont : null;
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.SelectFieldListBoxRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
            }
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = this.GetOccupiedArea().GetBBox().Clone();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            // Some properties are set to the HtmlDocumentRenderer, which is root renderer for this ButtonRenderer, but
            // in forms logic root renderer is CanvasRenderer, and these properties will have default values. So
            // we get them from renderer and set these properties to model element, which will be passed to forms logic.
            modelElement.SetProperty(Property.FONT_PROVIDER, this.GetProperty<FontProvider>(Property.FONT_PROVIDER));
            modelElement.SetProperty(Property.RENDERING_MODE, this.GetProperty<RenderingMode?>(Property.RENDERING_MODE
                ));
            ListBoxField lbModelElement = (ListBoxField)modelElement;
            IList<String> selectedOptions = lbModelElement.GetSelectedStrings();
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(doc, GetModelId()).SetWidgetRectangle(area);
            SetupBuilderValues(builder, lbModelElement);
            PdfChoiceFormField choiceField = builder.CreateList();
            if (font != null) {
                choiceField.SetFont(font);
            }
            choiceField.SetFontSize(fontSize.GetValue());
            choiceField.SetMultiSelect(IsMultiple());
            choiceField.SetListSelected(selectedOptions.ToArray(new String[selectedOptions.Count]));
            TransparentColor color = GetPropertyAsTransparentColor(Property.FONT_COLOR);
            if (color != null) {
                choiceField.SetColor(color.GetColor());
            }
            choiceField.SetJustification(this.GetProperty<TextAlignment?>(Property.TEXT_ALIGNMENT));
            AbstractFormFieldRenderer.ApplyBorderProperty(this, choiceField.GetFirstFormAnnotation());
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            if (background != null) {
                choiceField.GetFirstFormAnnotation().SetBackgroundColor(background.GetColor());
            }
            choiceField.GetFirstFormAnnotation().SetFormFieldElement(lbModelElement);
            PdfAcroForm.GetAcroForm(doc, true).AddField(choiceField, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        private float GetCalculatedHeight(IRenderer flatRenderer) {
            int? sizeProp = this.GetProperty<int?>(FormProperty.FORM_FIELD_SIZE);
            int size;
            if (sizeProp == null || sizeProp <= 0) {
                // Ensure height will not be negative or zero.
                // There is no particular reason for setting specifically 4.
                size = 4;
            }
            else {
                size = (int)sizeProp;
            }
            float maxOptionActualHeight = GetMaxOptionActualHeight(flatRenderer);
            if (maxOptionActualHeight == float.Epsilon) {
                UnitValue fontSize = flatRenderer.GetProperty<UnitValue>(Property.FONT_SIZE);
                if (fontSize != null && fontSize.IsPointValue()) {
                    // according to default styles for options (min-height: 1.2em)
                    maxOptionActualHeight = fontSize.GetValue() * 1.2f;
                }
                else {
                    maxOptionActualHeight = 0;
                }
            }
            return size * maxOptionActualHeight;
        }

        private float GetMaxOptionActualHeight(IRenderer flatRenderer) {
            float maxActualHeight = float.Epsilon;
            foreach (IRenderer child in flatRenderer.GetChildRenderers()) {
                if (IsOptionRenderer(child)) {
                    float childHeight;
                    if (child is AbstractRenderer) {
                        AbstractRenderer abstractChild = (AbstractRenderer)child;
                        childHeight = abstractChild.ApplyMargins(abstractChild.GetOccupiedAreaBBox(), false).GetHeight();
                    }
                    else {
                        childHeight = child.GetOccupiedArea().GetBBox().GetHeight();
                    }
                    if (childHeight > maxActualHeight) {
                        maxActualHeight = childHeight;
                    }
                }
                else {
                    float maxNestedHeight = GetMaxOptionActualHeight(child);
                    if (maxNestedHeight > maxActualHeight) {
                        maxActualHeight = maxNestedHeight;
                    }
                }
            }
            return maxActualHeight;
        }

        private IList<IRenderer> GetSelectedOptions(IRenderer rendererSubTree) {
            IList<IRenderer> selectedOptions = new List<IRenderer>();
            IList<IRenderer> optionsWhichMarkedSelected = GetOptionsMarkedSelected(rendererSubTree);
            if (!optionsWhichMarkedSelected.IsEmpty()) {
                if (IsMultiple()) {
                    selectedOptions.AddAll(optionsWhichMarkedSelected);
                }
                else {
                    selectedOptions.Add(optionsWhichMarkedSelected[optionsWhichMarkedSelected.Count - 1]);
                }
            }
            return selectedOptions;
        }

        private bool IsMultiple() {
            bool? propertyAsBoolean = GetPropertyAsBoolean(FormProperty.FORM_FIELD_MULTIPLE);
            return propertyAsBoolean != null && (bool)propertyAsBoolean;
        }

        private void ApplySelectedStyle(IRenderer selectedOption) {
            selectedOption.SetProperty(Property.BACKGROUND, new Background(new DeviceRgb(0, 120, 215)));
            SetFontColorRecursively(selectedOption);
        }

        /// <summary>
        /// The `select` tag has default color css property,
        /// therefore it makes sense to explicitly override this property to all children,
        /// otherwise it will be not applied due to the css resolving mechanism.
        /// </summary>
        private void SetFontColorRecursively(IRenderer selectedOption) {
            selectedOption.SetProperty(Property.FONT_COLOR, new TransparentColor(ColorConstants.WHITE));
            foreach (IRenderer renderer in selectedOption.GetChildRenderers()) {
                SetFontColorRecursively(renderer);
            }
        }
    }
}
