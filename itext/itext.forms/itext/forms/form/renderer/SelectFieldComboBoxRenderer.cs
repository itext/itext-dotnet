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
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="SelectFieldComboBoxRenderer"/>
    /// implementation for select field renderer.
    /// </summary>
    public class SelectFieldComboBoxRenderer : AbstractSelectFieldRenderer {
        private readonly IRenderer minMaxWidthRenderer;

        /// <summary>
        /// Creates a new
        /// <see cref="SelectFieldComboBoxRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public SelectFieldComboBoxRenderer(AbstractSelectField modelElement)
            : base(modelElement) {
            SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.MIDDLE);
            SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.HIDDEN);
            SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            minMaxWidthRenderer = CreateFlatRenderer(true);
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.SelectFieldComboBoxRenderer((AbstractSelectField)modelElement);
        }

        public override MinMaxWidth GetMinMaxWidth() {
            IList<IRenderer> realChildRenderers = childRenderers;
            childRenderers = new List<IRenderer>();
            childRenderers.Add(minMaxWidthRenderer);
            MinMaxWidth minMaxWidth = base.GetMinMaxWidth();
            childRenderers = realChildRenderers;
            return minMaxWidth;
        }

        protected override bool AllowLastYLineRecursiveExtraction() {
            return true;
        }

        protected internal override IRenderer CreateFlatRenderer() {
            return CreateFlatRenderer(false);
        }

        protected internal override void ApplyAcroField(DrawContext drawContext) {
            ComboBoxField comboBoxFieldModelElement = (ComboBoxField)this.modelElement;
            String name = GetModelId();
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = GetOccupiedAreaBBox();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            PdfFont font = GetResolvedFont(doc);
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(doc, name).SetWidgetRectangle(area).SetFont(font
                ).SetConformanceLevel(GetConformanceLevel(doc));
            modelElement.SetProperty(Property.FONT_PROVIDER, this.GetProperty<FontProvider>(Property.FONT_PROVIDER));
            modelElement.SetProperty(Property.RENDERING_MODE, this.GetProperty<RenderingMode?>(Property.RENDERING_MODE
                ));
            SetupBuilderValues(builder, comboBoxFieldModelElement);
            PdfChoiceFormField comboBoxField = builder.CreateComboBox();
            comboBoxField.DisableFieldRegeneration();
            Background background = this.modelElement.GetProperty<Background>(Property.BACKGROUND);
            if (background != null) {
                comboBoxField.GetFirstFormAnnotation().SetBackgroundColor(background.GetColor());
            }
            BorderStyleUtil.ApplyBorderProperty(this, comboBoxField.GetFirstFormAnnotation());
            UnitValue fontSize = GetFontSize();
            if (fontSize != null) {
                comboBoxField.SetFontSize(fontSize.GetValue());
            }
            SelectFieldItem selectedLabel = comboBoxFieldModelElement.GetSelectedOption();
            if (selectedLabel != null) {
                comboBoxField.SetValue(selectedLabel.GetDisplayValue());
            }
            else {
                String exportValue = comboBoxFieldModelElement.GetSelectedExportValue();
                if (exportValue == null) {
                    RenderingMode? renderingMode = comboBoxFieldModelElement.GetProperty<RenderingMode?>(Property.RENDERING_MODE
                        );
                    if (RenderingMode.HTML_MODE == renderingMode && comboBoxFieldModelElement.HasOptions()) {
                        comboBoxFieldModelElement.SetSelected(0);
                        comboBoxField.SetValue(comboBoxFieldModelElement.GetSelectedExportValue());
                    }
                }
                else {
                    comboBoxField.SetValue(comboBoxFieldModelElement.GetSelectedExportValue());
                }
            }
            comboBoxField.GetFirstFormAnnotation().SetFormFieldElement(comboBoxFieldModelElement);
            comboBoxField.EnableFieldRegeneration();
            PdfFormCreator.GetAcroForm(doc, true).AddField(comboBoxField, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        private UnitValue GetFontSize() {
            if (!this.HasProperty(Property.FONT_SIZE)) {
                return null;
            }
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.SelectFieldComboBoxRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
            }
            return fontSize;
        }

        private IRenderer CreateFlatRenderer(bool addAllOptionsToChildren) {
            AbstractSelectField selectField = (AbstractSelectField)modelElement;
            IList<SelectFieldItem> options = selectField.GetItems();
            Div pseudoContainer = new Div();
            foreach (SelectFieldItem option in options) {
                pseudoContainer.Add(option.GetElement());
            }
            IList<Paragraph> allOptions;
            IRenderer pseudoRendererSubTree = pseudoContainer.CreateRendererSubTree();
            if (addAllOptionsToChildren) {
                allOptions = GetAllOptionsFlatElements(pseudoRendererSubTree);
            }
            else {
                allOptions = GetSingleSelectedOptionFlatRenderer(pseudoRendererSubTree);
            }
            if (allOptions.IsEmpty()) {
                allOptions.Add(CreateComboBoxOptionFlatElement());
            }
            pseudoContainer.GetChildren().Clear();
            foreach (Paragraph option in allOptions) {
                pseudoContainer.Add(option);
            }
            String lang = GetLang();
            if (lang != null) {
                AccessibilityProperties properties = pseudoContainer.GetAccessibilityProperties();
                if (properties.GetLanguage() == null) {
                    properties.SetLanguage(lang);
                }
            }
            IRenderer rendererSubTree = pseudoContainer.CreateRendererSubTree();
            return rendererSubTree;
        }

        private IList<Paragraph> GetSingleSelectedOptionFlatRenderer(IRenderer optionsSubTree) {
            IList<Paragraph> selectedOptionFlatRendererList = new List<Paragraph>();
            IList<IRenderer> selectedOptions = GetOptionsMarkedSelected(optionsSubTree);
            IRenderer selectedOption;
            if (selectedOptions.IsEmpty()) {
                selectedOption = GetFirstOption(optionsSubTree);
            }
            else {
                selectedOption = selectedOptions[selectedOptions.Count - 1];
            }
            if (selectedOption != null) {
                String label = selectedOption.GetProperty<String>(FormProperty.FORM_FIELD_LABEL);
                Paragraph p = CreateComboBoxOptionFlatElement(label, false);
                ProcessLangAttribute(p, selectedOption);
                selectedOptionFlatRendererList.Add(p);
            }
            else {
                ComboBoxField modelElement = (ComboBoxField)GetModelElement();
                SelectFieldItem selectedOptionItem = modelElement.GetSelectedOption();
                String label = modelElement.GetSelectedExportValue();
                if (selectedOptionItem != null) {
                    label = selectedOptionItem.GetDisplayValue();
                }
                if (label != null) {
                    Paragraph p = CreateComboBoxOptionFlatElement(label, false);
                    p.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                    ProcessLangAttribute(p, p.GetRenderer());
                    selectedOptionFlatRendererList.Add(p);
                }
            }
            return selectedOptionFlatRendererList;
        }

        private IRenderer GetFirstOption(IRenderer renderer) {
            IRenderer firstOption = null;
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                if (IsOptionRenderer(child)) {
                    firstOption = child;
                    break;
                }
                firstOption = GetFirstOption(child);
                if (firstOption != null) {
                    break;
                }
            }
            return firstOption;
        }

        private IList<Paragraph> GetAllOptionsFlatElements(IRenderer renderer) {
            return GetAllOptionsFlatElements(renderer, false);
        }

        private IList<Paragraph> GetAllOptionsFlatElements(IRenderer renderer, bool isInOptGroup) {
            IList<Paragraph> options = new List<Paragraph>();
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                if (IsOptionRenderer(child)) {
                    String label = child.GetProperty<String>(FormProperty.FORM_FIELD_LABEL);
                    options.Add(CreateComboBoxOptionFlatElement(label, isInOptGroup));
                }
                else {
                    options.AddAll(GetAllOptionsFlatElements(child, isInOptGroup || IsOptGroupRenderer(child)));
                }
            }
            return options;
        }

        private void ProcessLangAttribute(Paragraph optionFlatElement, IRenderer originalOptionRenderer) {
            IPropertyContainer propertyContainer = originalOptionRenderer.GetModelElement();
            if (propertyContainer is IAccessibleElement) {
                String lang = ((IAccessibleElement)propertyContainer).GetAccessibilityProperties().GetLanguage();
                AccessibilityProperties properties = ((IAccessibleElement)optionFlatElement).GetAccessibilityProperties();
                if (properties.GetLanguage() == null) {
                    properties.SetLanguage(lang);
                }
            }
        }

        private Paragraph CreateComboBoxOptionFlatElement() {
            return CreateComboBoxOptionFlatElement(null, false);
        }

        private Paragraph CreateComboBoxOptionFlatElement(String label, bool simulateOptGroupMargin) {
            Paragraph paragraph = new Paragraph().SetMargin(0);
            if (simulateOptGroupMargin) {
                paragraph.Add("\u200d    ");
            }
            if (label == null || String.IsNullOrEmpty(label)) {
                label = "\u00A0";
            }
            paragraph.Add(label);
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            paragraph.SetFontColor(modelElement.GetProperty<TransparentColor>(Property.FONT_COLOR));
            UnitValue fontSize = modelElement.GetProperty<UnitValue>(Property.FONT_SIZE);
            if (fontSize != null) {
                paragraph.SetFontSize(fontSize.GetValue());
            }
            PdfFont font = GetResolvedFont(null);
            if (font != null) {
                paragraph.SetFont(font);
            }
            float paddingTop = 0f;
            float paddingBottom = 0.75f;
            float paddingLeft = 1.5f;
            float paddingRight = 1.5f;
            if (!IsFlatten()) {
                float extraPaddingChrome = 10f;
                paddingRight += extraPaddingChrome;
            }
            paragraph.SetPaddings(paddingTop, paddingRight, paddingBottom, paddingLeft);
            return paragraph;
        }
    }
}
