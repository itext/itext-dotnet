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
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Form.Renderer.Checkboximpl;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
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

        /// <summary>Creates a flat renderer for the checkbox.</summary>
        /// <returns>an IRenderer object for the flat renderer</returns>
        protected internal override IRenderer CreateFlatRenderer() {
            return CreateCheckBoxRenderFactory().CreateFlatRenderer();
        }

        /// <summary>Creates a CheckBoxRenderFactory for the checkbox based on the different rendering modes and PDFA mode.
        ///     </summary>
        /// <returns>a CheckBoxRenderFactory object for the checkbox</returns>
        public virtual AbstractCheckBoxRendererFactory CreateCheckBoxRenderFactory() {
            // html rendering is pdfa compliant so we dont have to check if its pdfa
            if (GetRenderingMode() == RenderingMode.HTML_MODE) {
                return new CheckBoxHtmlRendererFactory(this);
            }
            if (GetRenderingMode() == RenderingMode.DEFAULT_LAYOUT_MODE && IsPdfA()) {
                return new CheckBoxPdfARendererFactory(this);
            }
            return new CheckBoxPdfRendererFactory(this);
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
            return true.Equals(this.GetProperty<bool?>(FormProperty.FORM_FIELD_CHECKED));
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
            checkBox.SetValue(IsBoxChecked() ? PdfFormAnnotation.ON_STATE_VALUE : PdfFormAnnotation.OFF_STATE_VALUE, true
                );
            PdfAcroForm.GetAcroForm(doc, true).AddField(checkBox, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        protected internal override bool IsLayoutBasedOnFlatRenderer() {
            return false;
        }
    }
}
