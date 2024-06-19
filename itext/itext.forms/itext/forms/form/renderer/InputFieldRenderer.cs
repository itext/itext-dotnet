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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Logs;
using iText.Forms.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractOneLineTextFieldRenderer"/>
    /// implementation for input fields.
    /// </summary>
    public class InputFieldRenderer : AbstractOneLineTextFieldRenderer {
        /// <summary>
        /// Creates a new
        /// <see cref="InputFieldRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public InputFieldRenderer(InputField modelElement)
            : base(modelElement) {
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.InputFieldRenderer((InputField)modelElement);
        }

        /// <summary>Gets the size of the input field.</summary>
        /// <returns>the input field size</returns>
        public virtual int GetSize() {
            int? size = this.GetPropertyAsInteger(FormProperty.FORM_FIELD_SIZE);
            return size == null ? (int)modelElement.GetDefaultProperty<int>(FormProperty.FORM_FIELD_SIZE) : (int)size;
        }

        /// <summary>Checks if the input field is a password field.</summary>
        /// <returns>true, if the input field is a password field</returns>
        public virtual bool IsPassword() {
            bool? password = GetPropertyAsBoolean(FormProperty.FORM_FIELD_PASSWORD_FLAG);
            return password == null ? (bool)modelElement.GetDefaultProperty<bool>(FormProperty.FORM_FIELD_PASSWORD_FLAG
                ) : (bool)password;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override IRenderer CreateParagraphRenderer(String defaultValue) {
            if (String.IsNullOrEmpty(defaultValue) && null != ((InputField)modelElement).GetPlaceholder() && !((InputField
                )modelElement).GetPlaceholder().IsEmpty()) {
                return ((InputField)modelElement).GetPlaceholder().CreateRendererSubTree();
            }
            IRenderer flatRenderer = base.CreateParagraphRenderer(defaultValue);
            flatRenderer.SetProperty(Property.NO_SOFT_WRAP_INLINE, true);
            return flatRenderer;
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
            IList<LineRenderer> flatLines = ((ParagraphRenderer)flatRenderer).GetLines();
            Rectangle flatBBox = flatRenderer.GetOccupiedArea().GetBBox();
            UpdatePdfFont((ParagraphRenderer)flatRenderer);
            if (flatLines.IsEmpty() || font == null) {
                ITextLogManager.GetLogger(GetType()).LogError(MessageFormatUtil.Format(FormsLogMessageConstants.ERROR_WHILE_LAYOUT_OF_FORM_FIELD_WITH_TYPE
                    , "text input"));
                SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flatBBox.SetY(flatBBox.GetTop()).SetHeight(0);
            }
            else {
                CropContentLines(flatLines, flatBBox);
            }
            flatBBox.SetWidth((float)RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth()));
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IRenderer CreateFlatRenderer() {
            String defaultValue = GetDefaultValue();
            bool flatten = IsFlatten();
            bool password = IsPassword();
            if (flatten && password) {
                defaultValue = ObfuscatePassword(defaultValue);
            }
            return CreateParagraphRenderer(defaultValue);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void ApplyAcroField(DrawContext drawContext) {
            font.SetSubset(false);
            bool password = IsPassword();
            String value = password ? "" : GetDefaultValue();
            String name = GetModelId();
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.InputFieldRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
            }
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = this.GetOccupiedArea().GetBBox().Clone();
            ApplyMargins(area, false);
            IDictionary<int, Object> properties = FormFieldRendererUtil.RemoveProperties(this.modelElement);
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            float fontSizeValue = fontSize.GetValue();
            // Some properties are set to the HtmlDocumentRenderer, which is root renderer for this ButtonRenderer, but
            // in forms logic root renderer is CanvasRenderer, and these properties will have default values. So
            // we get them from renderer and set these properties to model element, which will be passed to forms logic.
            modelElement.SetProperty(Property.FONT_PROVIDER, this.GetProperty<FontProvider>(Property.FONT_PROVIDER));
            modelElement.SetProperty(Property.RENDERING_MODE, this.GetProperty<RenderingMode?>(Property.RENDERING_MODE
                ));
            // Default html2pdf input field appearance differs from the default one for form fields.
            // That's why we got rid of several properties we set by default during InputField instance creation.
            modelElement.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            PdfFormField inputField = new TextFormFieldBuilder(doc, name).SetWidgetRectangle(area).SetFont(font).SetGenericConformanceLevel
                (GetGenericConformanceLevel(doc)).CreateText();
            inputField.DisableFieldRegeneration();
            inputField.SetValue(value);
            inputField.SetFontSize(fontSizeValue);
            if (password) {
                inputField.SetFieldFlag(PdfFormField.FF_PASSWORD, true);
            }
            else {
                inputField.SetDefaultValue(new PdfString(value));
            }
            int rotation = ((InputField)modelElement).GetRotation();
            if (rotation != 0) {
                inputField.GetFirstFormAnnotation().SetRotation(rotation);
            }
            ApplyDefaultFieldProperties(inputField);
            ApplyAccessibilityProperties(inputField, doc);
            inputField.GetFirstFormAnnotation().SetFormFieldElement((InputField)modelElement);
            inputField.EnableFieldRegeneration();
            PdfFormCreator.GetAcroForm(doc, true).AddField(inputField, page);
            FormFieldRendererUtil.ReapplyProperties(modelElement, properties);
        }

        /// <summary><inheritDoc/></summary>
        public override T1 GetProperty<T1>(int key) {
            if (key == Property.WIDTH) {
                T1 width = base.GetProperty<T1>(Property.WIDTH);
                if (width == null) {
                    UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
                    if (!fontSize.IsPointValue()) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.InputFieldRenderer));
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                            , Property.FONT_SIZE));
                    }
                    int size = GetSize();
                    return (T1)(Object)UnitValue.CreatePointValue(UpdateHtmlColsSizeBasedWidth(fontSize.GetValue() * (size * 0.5f
                         + 2) + 2));
                }
                return width;
            }
            return base.GetProperty<T1>(key);
        }

        /// <summary><inheritDoc/></summary>
        protected override bool SetMinMaxWidthBasedOnFixedWidth(MinMaxWidth minMaxWidth) {
            bool result = false;
            if (HasRelativeUnitValue(Property.WIDTH)) {
                UnitValue widthUV = this.GetProperty<UnitValue>(Property.WIDTH);
                bool restoreWidth = HasOwnProperty(Property.WIDTH);
                SetProperty(Property.WIDTH, null);
                float? width = RetrieveWidth(0);
                if (width != null) {
                    // the field can be shrinked if necessary so only max width is set here
                    minMaxWidth.SetChildrenMaxWidth((float)width);
                    result = true;
                }
                if (restoreWidth) {
                    SetProperty(Property.WIDTH, widthUV);
                }
                else {
                    DeleteOwnProperty(Property.WIDTH);
                }
            }
            else {
                result = base.SetMinMaxWidthBasedOnFixedWidth(minMaxWidth);
            }
            return result;
        }

        /// <summary>Obfuscates the content of a password input field.</summary>
        /// <param name="text">the password</param>
        /// <returns>a string consisting of '*' characters.</returns>
        private String ObfuscatePassword(String text) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; ++i) {
                builder.Append('*');
            }
            return builder.ToString();
        }
    }
}
