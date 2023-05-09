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

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.TextAreaRenderer((TextArea)GetModelElement());
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            UnitValue fontSize = GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (fontSize != null && fontSize.GetValue() < EPS) {
                ApproximateFontSizeToFitMultiLine(layoutContext);
            }
            return base.Layout(layoutContext);
        }

        /// <summary><inheritDoc/></summary>
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
                    float fontSizeValue = fontSize.GetValue();
                    if (fontSizeValue < EPS) {
                        fontSizeValue = AbstractPdfFormField.DEFAULT_FONT_SIZE;
                    }
                    int cols = GetCols();
                    return (T1)(Object)UnitValue.CreatePointValue(UpdateHtmlColsSizeBasedWidth(fontSizeValue * (cols * 0.5f + 
                        2) + 2));
                }
                return width;
            }
            return base.GetProperty<T1>(key);
        }

        /// <summary><inheritDoc/></summary>
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
                if (!HasOwnOrModelProperty(FormProperty.FORM_FIELD_ROWS)) {
                    SetProperty(FormProperty.FORM_FIELD_ROWS, flatLines.Count);
                }
                CropContentLines(flatLines, flatBBox);
            }
            flatBBox.SetWidth((float)RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth()));
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IRenderer CreateFlatRenderer() {
            return CreateParagraphRenderer(GetDefaultValue());
        }

        /// <summary><inheritDoc/></summary>
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
            Rectangle area = GetOccupiedArea().GetBBox().Clone();
            ApplyMargins(area, false);
            DeleteMargins();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            float fontSizeValue = fontSize.GetValue();
            PdfString defaultValue = new PdfString(GetDefaultValue());
            // Default html2pdf text area appearance differs from the default one for form fields.
            // That's why we got rid of several properties we set by default during TextArea instance creation.
            modelElement.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            PdfFormField inputField = new TextFormFieldBuilder(doc, name).SetWidgetRectangle(area).CreateMultilineText
                ().SetValue(value);
            inputField.SetFont(font).SetFontSize(fontSizeValue);
            inputField.SetDefaultValue(defaultValue);
            ApplyDefaultFieldProperties(inputField);
            inputField.GetFirstFormAnnotation().SetFormFieldElement((TextArea)modelElement);
            PdfFormCreator.GetAcroForm(doc, true).AddField(inputField, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        /// <summary><inheritDoc/></summary>
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

        internal override IRenderer CreateParagraphRenderer(String defaultValue) {
            if (String.IsNullOrEmpty(defaultValue) && null != ((TextArea)modelElement).GetPlaceholder() && !((TextArea
                )modelElement).GetPlaceholder().IsEmpty()) {
                return ((TextArea)modelElement).GetPlaceholder().CreateRendererSubTree();
            }
            IRenderer flatRenderer = base.CreateParagraphRenderer(defaultValue);
            flatRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
            return flatRenderer;
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

        private void ApproximateFontSizeToFitMultiLine(LayoutContext layoutContext) {
            IRenderer flatRenderer = CreateFlatRenderer();
            flatRenderer.SetParent(this);
            TextArea modelElement = (TextArea)this.GetModelElement();
            float lFontSize = AbstractPdfFormField.MIN_FONT_SIZE;
            float rFontSize = AbstractPdfFormField.DEFAULT_FONT_SIZE;
            flatRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(AbstractPdfFormField.DEFAULT_FONT_SIZE
                ));
            float? areaWidth = RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth());
            float? areaHeight = RetrieveHeight();
            LayoutContext newLayoutContext;
            if (areaWidth == null || areaHeight == null) {
                modelElement.SetFontSize(AbstractPdfFormField.DEFAULT_FONT_SIZE);
                return;
            }
            newLayoutContext = new LayoutContext(new LayoutArea(1, new Rectangle((float)areaWidth, (float)areaHeight))
                );
            if (flatRenderer.Layout(newLayoutContext).GetStatus() == LayoutResult.FULL) {
                lFontSize = AbstractPdfFormField.DEFAULT_FONT_SIZE;
            }
            else {
                int numberOfIterations = 6;
                for (int i = 0; i < numberOfIterations; i++) {
                    float mFontSize = (lFontSize + rFontSize) / 2;
                    flatRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(mFontSize));
                    LayoutResult result = flatRenderer.Layout(newLayoutContext);
                    if (result.GetStatus() == LayoutResult.FULL) {
                        lFontSize = mFontSize;
                    }
                    else {
                        rFontSize = mFontSize;
                    }
                }
            }
            modelElement.SetFontSize(lFontSize);
        }
    }
}
