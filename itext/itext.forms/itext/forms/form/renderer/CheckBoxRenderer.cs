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
