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
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
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

        protected internal override void ApplyAcroField(DrawContext drawContext) {
        }

        // TODO DEVSIX-1901
        protected internal override IRenderer CreateFlatRenderer() {
            return CreateFlatRenderer(false);
        }

        private IRenderer CreateFlatRenderer(bool addAllOptionsToChildren) {
            AbstractSelectField selectField = (AbstractSelectField)modelElement;
            IList<IBlockElement> options = selectField.GetOptions();
            Div pseudoContainer = new Div();
            foreach (IBlockElement option in options) {
                pseudoContainer.Add(option);
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

        private static Paragraph CreateComboBoxOptionFlatElement() {
            return CreateComboBoxOptionFlatElement(null, false);
        }

        private static Paragraph CreateComboBoxOptionFlatElement(String label, bool simulateOptGroupMargin) {
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
            // These constants are defined according to values in default.css.
            // At least in Chrome paddings of options in comboboxes cannot be altered through css styles.
            float leftRightPaddingVal = 2 * 0.75f;
            float bottomPaddingVal = 0.75f;
            float topPaddingVal = 0;
            paragraph.SetPaddings(topPaddingVal, leftRightPaddingVal, bottomPaddingVal, leftRightPaddingVal);
            return paragraph;
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
    }
}
