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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Logs;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractTextFieldRenderer"/>
    /// implementation for text area fields.
    /// </summary>
    public class TextAreaRenderer : AbstractTextFieldRenderer {
        /// <summary>
        /// Creates a new
        /// <see cref="TextAreaRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public TextAreaRenderer(TextArea modelElement)
            : base(modelElement) {
        }

        /// <summary>Gets the number of columns.</summary>
        /// <returns>the cols value of the text area field</returns>
        public virtual int GetCols() {
            int? cols = this.GetPropertyAsInteger(FormProperty.FORM_FIELD_COLS);
            if (cols != null && cols.Value > 0) {
                return (int)cols;
            }
            return (int)modelElement.GetDefaultProperty<int>(FormProperty.FORM_FIELD_COLS);
        }

        /// <summary>Gets the number of rows.</summary>
        /// <returns>the rows value of the text area field</returns>
        public virtual int GetRows() {
            int? rows = this.GetPropertyAsInteger(FormProperty.FORM_FIELD_ROWS);
            if (rows != null && rows.Value > 0) {
                return (int)rows;
            }
            return (int)modelElement.GetDefaultProperty<int>(FormProperty.FORM_FIELD_ROWS);
        }

        protected override float? GetLastYLineRecursively() {
            if (occupiedArea != null && occupiedArea.GetBBox() != null) {
                return occupiedArea.GetBBox().GetBottom();
            }
            return null;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.renderer.IRenderer#getNextRenderer()
        */
        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.TextAreaRenderer((TextArea)GetModelElement());
        }

        /* (non-Javadoc)
        * @see com.itextpdf.html2pdf.attach.impl.layout.form.renderer.AbstractFormFieldRenderer#adjustFieldLayout()
        */
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
            IList<LineRenderer> flatLines = ((ParagraphRenderer)flatRenderer).GetLines();
            UpdatePdfFont((ParagraphRenderer)flatRenderer);
            Rectangle flatBBox = flatRenderer.GetOccupiedArea().GetBBox();
            if (flatLines.IsEmpty() || font == null) {
                ITextLogManager.GetLogger(GetType()).LogError(MessageFormatUtil.Format(FormsLogMessageConstants.ERROR_WHILE_LAYOUT_OF_FORM_FIELD_WITH_TYPE
                    , "text area"));
                SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flatBBox.SetHeight(0);
            }
            else {
                CropContentLines(flatLines, flatBBox);
            }
            flatBBox.SetWidth((float)RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth()));
        }

        /* (non-Javadoc)
        * @see AbstractFormFieldRenderer#createFlatRenderer()
        */
        protected internal override IRenderer CreateFlatRenderer() {
            return CreateParagraphRenderer(GetDefaultValue());
        }

        internal override IRenderer CreateParagraphRenderer(String defaultValue) {
            if (String.IsNullOrEmpty(defaultValue) && null != ((TextArea)modelElement).GetPlaceholder() && !((TextArea
                )modelElement).GetPlaceholder().IsEmpty()) {
                return ((TextArea)modelElement).GetPlaceholder().CreateRendererSubTree();
            }
            return base.CreateParagraphRenderer(defaultValue);
        }

        /* (non-Javadoc)
        * @see AbstractFormFieldRenderer#applyAcroField(com.itextpdf.layout.renderer.DrawContext)
        */
        protected internal override void ApplyAcroField(DrawContext drawContext) {
            font.SetSubset(false);
            String value = GetDefaultValue();
            String name = GetModelId();
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.TextAreaRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
            }
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = flatRenderer.GetOccupiedArea().GetBBox().Clone();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            float fontSizeValue = fontSize.GetValue();
            PdfString defaultValue = new PdfString(GetDefaultValue());
            PdfFormField inputField = new TextFormFieldBuilder(doc, name).SetWidgetRectangle(area).CreateText().SetValue
                (value);
            inputField.SetFont(font).SetFontSize(fontSizeValue);
            inputField.SetFieldFlag(PdfFormField.FF_MULTILINE, true);
            inputField.SetDefaultValue(defaultValue);
            ApplyDefaultFieldProperties(inputField);
            PdfAcroForm.GetAcroForm(doc, true).AddField(inputField, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        public override T1 GetProperty<T1>(int key) {
            if (key == Property.WIDTH) {
                T1 width = base.GetProperty<T1>(Property.WIDTH);
                if (width == null) {
                    UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
                    if (!fontSize.IsPointValue()) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.TextAreaRenderer));
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                            , Property.FONT_SIZE));
                    }
                    int cols = GetCols();
                    return (T1)(Object)UnitValue.CreatePointValue(UpdateHtmlColsSizeBasedWidth(fontSize.GetValue() * (cols * 0.5f
                         + 2) + 2));
                }
                return width;
            }
            return base.GetProperty<T1>(key);
        }

        protected override bool SetMinMaxWidthBasedOnFixedWidth(MinMaxWidth minMaxWidth) {
            if (!HasAbsoluteUnitValue(Property.WIDTH)) {
                UnitValue width = this.GetProperty<UnitValue>(Property.WIDTH);
                bool restoreWidth = HasOwnProperty(Property.WIDTH);
                SetProperty(Property.WIDTH, null);
                bool result = base.SetMinMaxWidthBasedOnFixedWidth(minMaxWidth);
                if (restoreWidth) {
                    SetProperty(Property.WIDTH, width);
                }
                else {
                    DeleteOwnProperty(Property.WIDTH);
                }
                return result;
            }
            return base.SetMinMaxWidthBasedOnFixedWidth(minMaxWidth);
        }

        private void CropContentLines(IList<LineRenderer> lines, Rectangle bBox) {
            float? height = RetrieveHeight();
            float? minHeight = RetrieveMinHeight();
            float? maxHeight = RetrieveMaxHeight();
            int rowsAttribute = GetRows();
            float rowsHeight = GetHeightRowsBased(lines, bBox, rowsAttribute);
            if (height != null && (float)height > 0) {
                AdjustNumberOfContentLines(lines, bBox, (float)height);
            }
            else {
                if (minHeight != null && (float)minHeight > rowsHeight) {
                    AdjustNumberOfContentLines(lines, bBox, (float)minHeight);
                }
                else {
                    if (maxHeight != null && (float)maxHeight > 0 && (float)maxHeight < rowsHeight) {
                        AdjustNumberOfContentLines(lines, bBox, (float)maxHeight);
                    }
                    else {
                        AdjustNumberOfContentLines(lines, bBox, rowsAttribute);
                    }
                }
            }
        }
    }
}
