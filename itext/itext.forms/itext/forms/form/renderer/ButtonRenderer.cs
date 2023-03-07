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
