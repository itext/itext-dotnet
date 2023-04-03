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
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Form.Renderer.Checkboximpl;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
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
        // 1px
        public const float DEFAULT_BORDER_WIDTH = 0.75F;

        private static readonly Color DEFAULT_BORDER_COLOR = ColorConstants.DARK_GRAY;

        private static readonly Color DEFAULT_BACKGROUND_COLOR = ColorConstants.WHITE;

        // 11px
        private const float DEFAULT_SIZE = 8.25F;

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

        /// <summary>Gets the rendering mode of the checkbox.</summary>
        /// <returns>the rendering mode of the checkbox</returns>
        public virtual RenderingMode? GetRenderingMode() {
            RenderingMode? renderingMode = this.GetProperty<RenderingMode?>(Property.RENDERING_MODE);
            if (renderingMode != null) {
                return renderingMode;
            }
            return RenderingMode.DEFAULT_LAYOUT_MODE;
        }

        /// <summary>Returns whether or not the checkbox is in PDF/A mode.</summary>
        /// <returns>true if the checkbox is in PDF/A mode, false otherwise</returns>
        public virtual bool IsPdfA() {
            return this.GetProperty<PdfAConformanceLevel>(FormProperty.FORM_CONFORMANCE_LEVEL) != null;
        }

        /// <summary>Gets the checkBoxType.</summary>
        /// <returns>the checkBoxType</returns>
        public virtual CheckBoxType GetCheckBoxType() {
            if (this.HasProperty(FormProperty.FORM_CHECKBOX_TYPE)) {
                return (CheckBoxType)this.GetProperty<CheckBoxType?>(FormProperty.FORM_CHECKBOX_TYPE);
            }
            return CheckBoxType.CROSS;
        }

        /// <summary>creates a ICheckBoxRenderingStrategy based on the current settings.</summary>
        /// <returns>the ICheckBoxRenderingStrategy</returns>
        public virtual ICheckBoxRenderingStrategy CreateCheckBoxRenderStrategy() {
            // html rendering is PDFA compliant this means we don't have to check if its PDFA.
            ICheckBoxRenderingStrategy renderingStrategy;
            if (GetRenderingMode() == RenderingMode.HTML_MODE) {
                renderingStrategy = new HtmlCheckBoxRenderingStrategy();
            }
            else {
                if (GetRenderingMode() == RenderingMode.DEFAULT_LAYOUT_MODE && IsPdfA()) {
                    renderingStrategy = new PdfACheckBoxRenderingStrategy();
                }
                else {
                    renderingStrategy = new PdfCheckBoxRenderingStrategy();
                }
            }
            return renderingStrategy;
        }

        public override void DrawBackground(DrawContext drawContext) {
            if (!ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING) {
                base.DrawBackground(drawContext);
            }
        }

        // draw background in child
        public override void DrawBorder(DrawContext drawContext) {
            if (!ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING) {
                base.DrawBorder(drawContext);
            }
        }

        //draw border in child
        protected override Rectangle ApplyBorderBox(Rectangle rect, Border[] borders, bool reverse) {
            if (!ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING) {
                return base.ApplyBorderBox(rect, borders, reverse);
            }
            // Do not apply borders here, they will be applied in flat renderer
            return rect;
        }

        /// <summary>Defines whether the box is checked or not.</summary>
        /// <returns>the default value of the checkbox field</returns>
        public virtual bool IsBoxChecked() {
            return true.Equals(this.GetProperty<bool?>(FormProperty.FORM_FIELD_CHECKED));
        }

        /* (non-Javadoc)
        * @see com.itextpdf.html2pdf.attach.impl.layout.form.renderer.AbstractFormFieldRenderer#adjustFieldLayout()
        */
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
            this.SetProperty(Property.BACKGROUND, null);
        }

        /// <summary>Creates a flat renderer for the checkbox.</summary>
        /// <returns>an IRenderer object for the flat renderer</returns>
        protected internal override IRenderer CreateFlatRenderer() {
            if (!ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING) {
                Paragraph paragraph = new Paragraph().SetWidth(DEFAULT_SIZE).SetHeight(DEFAULT_SIZE).SetBorder(new SolidBorder
                    (DEFAULT_BORDER_COLOR, DEFAULT_BORDER_WIDTH)).SetBackgroundColor(DEFAULT_BACKGROUND_COLOR).SetHorizontalAlignment
                    (HorizontalAlignment.CENTER);
                return new CheckBoxRenderer.FlatParagraphRenderer(this, paragraph);
            }
            UnitValue heightUV = GetPropertyAsUnitValue(Property.HEIGHT);
            UnitValue widthUV = GetPropertyAsUnitValue(Property.WIDTH);
            float height = null == heightUV ? DEFAULT_SIZE : heightUV.GetValue();
            float width = null == widthUV ? DEFAULT_SIZE : widthUV.GetValue();
            Paragraph paragraph_1 = new Paragraph().SetWidth(width).SetHeight(height).SetMargin(0).SetVerticalAlignment
                (VerticalAlignment.MIDDLE).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            paragraph_1.SetProperty(Property.BOX_SIZING, this.GetProperty<BoxSizingPropertyValue?>(Property.BOX_SIZING
                ));
            modelElement.SetProperty(Property.RENDERING_MODE, this.GetProperty<RenderingMode?>(Property.RENDERING_MODE
                ));
            paragraph_1.SetBorder(this.GetProperty<Border>(Property.BORDER));
            paragraph_1.SetProperty(Property.BACKGROUND, this.GetProperty<Background>(Property.BACKGROUND));
            //In html 2 pdf rendering the boxes height width ratio is always 1:1
            // with chrome taking the max of the two as the size of the box
            if (GetRenderingMode() == RenderingMode.HTML_MODE) {
                paragraph_1.SetWidth(Math.Max(width, height));
                paragraph_1.SetHeight(Math.Max(width, height));
            }
            return new CheckBoxRenderer.FlatParagraphRenderer(this, paragraph_1);
        }

        protected internal override void ApplyAcroField(DrawContext drawContext) {
            String name = GetModelId();
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = flatRenderer.GetOccupiedArea().GetBBox().Clone();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            CheckBoxFormFieldBuilder builder = new CheckBoxFormFieldBuilder(doc, name).SetWidgetRectangle(area).SetConformanceLevel
                (this.GetProperty<PdfAConformanceLevel>(FormProperty.FORM_CONFORMANCE_LEVEL));
            if (this.HasProperty(FormProperty.FORM_CHECKBOX_TYPE)) {
                builder.SetCheckType((CheckBoxType)this.GetProperty<CheckBoxType?>(FormProperty.FORM_CHECKBOX_TYPE));
            }
            PdfButtonFormField checkBox = builder.CreateCheckBox();
            if (ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING) {
                Border border = this.GetProperty<Border>(Property.BORDER);
                if (border != null) {
                    checkBox.GetFirstFormAnnotation().SetBorderColor(border.GetColor());
                    checkBox.GetFirstFormAnnotation().SetBorderWidth(border.GetWidth());
                }
                Background background = this.modelElement.GetProperty<Background>(Property.BACKGROUND);
                if (background != null) {
                    checkBox.GetFirstFormAnnotation().SetBackgroundColor(background.GetColor());
                }
            }
            checkBox.SetValue(PdfFormAnnotation.ON_STATE_VALUE);
            if (!IsBoxChecked()) {
                checkBox.SetValue(PdfFormAnnotation.OFF_STATE_VALUE);
            }
            PdfAcroForm.GetAcroForm(doc, true).AddField(checkBox, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        protected internal override bool IsLayoutBasedOnFlatRenderer() {
            return false;
        }

        /// <summary>A flat renderer for the checkbox.</summary>
        protected internal class FlatParagraphRenderer : ParagraphRenderer {
            /// <summary>
            /// Creates a new
            /// <see cref="FlatParagraphRenderer"/>
            /// instance.
            /// </summary>
            /// <param name="modelElement">the model element</param>
            public FlatParagraphRenderer(CheckBoxRenderer _enclosing, Paragraph modelElement)
                : base(modelElement) {
                this._enclosing = _enclosing;
            }

            public override void DrawChildren(DrawContext drawContext) {
                Rectangle rectangle = this.GetInnerAreaBBox().Clone();
                this._enclosing.CreateCheckBoxRenderStrategy().DrawCheckBoxContent(drawContext, this._enclosing, rectangle
                    );
            }

            private readonly CheckBoxRenderer _enclosing;
        }
    }
}
