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
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractOneLineTextFieldRenderer"/>
    /// implementation for buttons with kids.
    /// </summary>
    public class ButtonRenderer : BlockRenderer {
        private const float DEFAULT_FONT_SIZE = 12f;

        public ButtonRenderer(Button modelElement)
            : base(modelElement) {
        }

        public override void Draw(DrawContext drawContext) {
            base.Draw(drawContext);
            if (!IsFlatten()) {
                String value = GetDefaultValue();
                String name = GetModelId();
                UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
                if (!fontSize.IsPointValue()) {
                    fontSize = UnitValue.CreatePointValue(DEFAULT_FONT_SIZE);
                }
                PdfDocument doc = drawContext.GetDocument();
                Rectangle area = GetOccupiedArea().GetBBox().Clone();
                ApplyMargins(area, false);
                PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
                TransparentColor transparentColor = GetPropertyAsTransparentColor(Property.FONT_COLOR);
                Color color = transparentColor == null ? null : transparentColor.GetColor();
                float fontSizeValue = fontSize.GetValue();
                PdfFont font = doc.GetDefaultFont();
                PdfButtonFormField button = new PushButtonFormFieldBuilder(doc, name).SetWidgetRectangle(area).SetCaption(
                    value).CreatePushButton();
                button.SetFont(font).SetFontSize(fontSizeValue);
                button.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_NONE);
                button.GetFirstFormAnnotation().SetBorderWidth(0);
                button.GetFirstFormAnnotation().SetBackgroundColor(null);
                if (color != null) {
                    button.SetColor(color);
                }
                PdfAcroForm forms = PdfAcroForm.GetAcroForm(doc, true);
                //Add fields only if it isn't already added. This can happen on split.
                if (forms.GetField(name) == null) {
                    forms.AddField(button, page);
                }
                if (doc.IsTagged()) {
                    TagTreePointer formParentPointer = doc.GetTagStructureContext().GetAutoTaggingPointer();
                    IList<String> kidsRoles = formParentPointer.GetKidsRoles();
                    int lastFormIndex = kidsRoles.LastIndexOf(StandardRoles.FORM);
                    TagTreePointer formPointer = formParentPointer.MoveToKid(lastFormIndex);
                    String lang = this.GetProperty<String>(FormProperty.FORM_ACCESSIBILITY_LANGUAGE);
                    if (lang != null) {
                        formPointer.GetProperties().SetLanguage(lang);
                    }
                    formParentPointer.MoveToParent();
                }
            }
        }

        protected override float? GetLastYLineRecursively() {
            return base.GetFirstYLineRecursively();
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.ButtonRenderer((Button)modelElement);
        }

        //NOTE: Duplicates methods from AbstractFormFieldRenderer should be changed in next major version
        /// <summary>Gets the model id.</summary>
        /// <returns>the model id</returns>
        protected internal virtual String GetModelId() {
            return ((IFormField)GetModelElement()).GetId();
        }

        /// <summary>Checks if form fields need to be flattened.</summary>
        /// <returns>true, if fields need to be flattened</returns>
        public virtual bool IsFlatten() {
            bool? flatten = GetPropertyAsBoolean(FormProperty.FORM_FIELD_FLATTEN);
            return flatten == null ? (bool)modelElement.GetDefaultProperty<bool>(FormProperty.FORM_FIELD_FLATTEN) : (bool
                )flatten;
        }

        /// <summary>Gets the default value of the form field.</summary>
        /// <returns>the default value of the form field</returns>
        public virtual String GetDefaultValue() {
            String defaultValue = this.GetProperty<String>(FormProperty.FORM_FIELD_VALUE);
            return defaultValue == null ? modelElement.GetDefaultProperty<String>(FormProperty.FORM_FIELD_VALUE) : defaultValue;
        }
    }
}
