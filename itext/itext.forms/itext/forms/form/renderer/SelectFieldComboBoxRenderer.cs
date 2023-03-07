/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
