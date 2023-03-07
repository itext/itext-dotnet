/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractOneLineTextFieldRenderer"/>
    /// implementation for checkboxes.
    /// </summary>
    public class CheckBoxRenderer : AbstractFormFieldRenderer {
        private static readonly Color DEFAULT_BORDER_COLOR = ColorConstants.DARK_GRAY;

        private static readonly Color DEFAULT_BACKGROUND_COLOR = ColorConstants.WHITE;

        private const float DEFAULT_BORDER_WIDTH = 0.75f;

        // 1px
        private const float DEFAULT_SIZE = 8.25f;

        // 11px
        /// <summary>
        /// Creates a new
        /// <see cref="CheckBoxRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public CheckBoxRenderer(CheckBox modelElement)
            : base(modelElement) {
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.renderer.IRenderer#getNextRenderer()
        */
        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.CheckBoxRenderer((CheckBox)modelElement);
        }

        protected internal override IRenderer CreateFlatRenderer() {
            Paragraph paragraph = new Paragraph().SetWidth(DEFAULT_SIZE).SetHeight(DEFAULT_SIZE).SetBorder(new SolidBorder
                (DEFAULT_BORDER_COLOR, DEFAULT_BORDER_WIDTH)).SetBackgroundColor(DEFAULT_BACKGROUND_COLOR).SetHorizontalAlignment
                (HorizontalAlignment.CENTER);
            return new CheckBoxRenderer.FlatParagraphRenderer(this, paragraph);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.html2pdf.attach.impl.layout.form.renderer.AbstractFormFieldRenderer#adjustFieldLayout()
        */
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
            this.SetProperty(Property.BACKGROUND, null);
        }

        /// <summary>Defines whether the box is checked or not.</summary>
        /// <returns>the default value of the checkbox field</returns>
        public virtual bool IsBoxChecked() {
            return null != this.GetProperty<Object>(FormProperty.FORM_FIELD_CHECKED);
        }

        protected internal override void ApplyAcroField(DrawContext drawContext) {
            String name = GetModelId();
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = flatRenderer.GetOccupiedArea().GetBBox().Clone();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(doc, name).SetWidgetRectangle(area).CreateCheckBox
                ();
            checkBox.SetValue(IsBoxChecked() ? "Yes" : "Off", true);
            PdfAcroForm.GetAcroForm(doc, true).AddField(checkBox, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        protected internal override bool IsLayoutBasedOnFlatRenderer() {
            return false;
        }

        private class FlatParagraphRenderer : ParagraphRenderer {
            public FlatParagraphRenderer(CheckBoxRenderer _enclosing, Paragraph modelElement)
                : base(modelElement) {
                this._enclosing = _enclosing;
            }

            public override void DrawChildren(DrawContext drawContext) {
                if (this._enclosing.IsBoxChecked()) {
                    PdfCanvas canvas = drawContext.GetCanvas();
                    Rectangle rectangle = this.GetInnerAreaBBox();
                    canvas.SaveState();
                    canvas.SetFillColor(ColorConstants.BLACK);
                    DrawingUtil.DrawPdfACheck(canvas, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                        .GetBottom());
                    canvas.RestoreState();
                }
            }

            private readonly CheckBoxRenderer _enclosing;
        }
    }
}
