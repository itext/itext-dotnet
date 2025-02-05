/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Layout.Element;
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
        private const float DEFAULT_COMB_PADDING = 0;

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
            if (String.IsNullOrEmpty(defaultValue)) {
                defaultValue = "\u00a0";
            }
            IRenderer flatRenderer;
            if (IsComb()) {
                SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(DEFAULT_COMB_PADDING));
                SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(DEFAULT_COMB_PADDING));
                int maxLen = GetMaxLen();
                int numberOfCharacters = Math.Min(maxLen, defaultValue.Length);
                int start;
                TextAlignment? textAlignment = this.GetProperty<TextAlignment?>(Property.TEXT_ALIGNMENT, TextAlignment.LEFT
                    );
                switch (textAlignment) {
                    case TextAlignment.RIGHT: {
                        start = (maxLen - numberOfCharacters);
                        break;
                    }

                    case TextAlignment.CENTER: {
                        start = (maxLen - numberOfCharacters) / 2;
                        break;
                    }

                    default: {
                        start = 0;
                        break;
                    }
                }
                Paragraph paragraph = new Paragraph();
                for (int i = 0; i < start; i++) {
                    paragraph.Add(GetSubParagraph("", maxLen));
                }
                for (int i = 0; i < numberOfCharacters; i++) {
                    paragraph.Add(GetSubParagraph(defaultValue.JSubstring(i, i + 1), maxLen));
                }
                for (int i = start + numberOfCharacters; i < maxLen; i++) {
                    paragraph.Add(GetSubParagraph("", maxLen));
                }
                flatRenderer = paragraph.SetMargin(0).CreateRendererSubTree();
            }
            else {
                Text text = new Text(defaultValue);
                FormFieldValueNonTrimmingTextRenderer nextRenderer = new FormFieldValueNonTrimmingTextRenderer(text);
                text.SetNextRenderer(nextRenderer);
                flatRenderer = new Paragraph(text).SetMargin(0).CreateRendererSubTree();
            }
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
            PdfTextFormField inputField = new TextFormFieldBuilder(doc, name).SetWidgetRectangle(area).SetFont(font).SetConformance
                (GetConformance(doc)).CreateText();
            inputField.DisableFieldRegeneration();
            inputField.SetValue(value);
            inputField.SetFontSize(fontSizeValue);
            if (password) {
                inputField.SetPassword(true);
            }
            else {
                inputField.SetDefaultValue(new PdfString(value));
            }
            if (IsComb()) {
                inputField.SetComb(true);
                inputField.SetMaxLen(GetMaxLen());
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

        private static Paragraph GetSubParagraph(String value, int maxLen) {
            Text text = new Text(value);
            FormFieldValueNonTrimmingTextRenderer nextRenderer = new FormFieldValueNonTrimmingTextRenderer(text);
            text.SetNextRenderer(nextRenderer);
            return new Paragraph(text).SetTextAlignment(TextAlignment.CENTER).SetWidth(UnitValue.CreatePercentValue((float
                )100 / maxLen)).SetHeight(UnitValue.CreatePercentValue(100)).SetMargin(0);
        }

        /// <summary>Checks if the input field is a comb field.</summary>
        /// <returns>true, if the input field is a comb field</returns>
        private bool IsComb() {
            return (bool)this.GetProperty<bool?>(FormProperty.TEXT_FIELD_COMB_FLAG, false);
        }

        /// <summary>Gets the maximum length of the field's text, in characters.</summary>
        /// <returns>the current maximum text length</returns>
        private int GetMaxLen() {
            return (int)this.GetProperty<int?>(FormProperty.TEXT_FIELD_MAX_LEN, 0);
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
