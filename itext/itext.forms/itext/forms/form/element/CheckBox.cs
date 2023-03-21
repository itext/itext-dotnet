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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Forms.Logs;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Extension of the
    /// <see cref="FormField{T}"/>
    /// class representing a checkbox so that
    /// a
    /// <see cref="iText.Forms.Form.Renderer.CheckBoxRenderer"/>
    /// is used instead of the default renderer for fields.
    /// </summary>
    public class CheckBox : FormField<iText.Forms.Form.Element.CheckBox> {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Element.CheckBox
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="CheckBox"/>
        /// instance.
        /// </summary>
        /// <param name="id">the id</param>
        public CheckBox(String id)
            : base(id) {
        }

        /// <summary>Sets the checked state of the checkbox.</summary>
        /// <param name="checked">the checked state to set</param>
        /// <returns>this checkbox instance</returns>
        public virtual iText.Forms.Form.Element.CheckBox SetChecked(bool @checked) {
            SetProperty(FormProperty.FORM_FIELD_CHECKED, @checked);
            return this;
        }

        /// <summary>Sets the rendering mode for the checkbox.</summary>
        /// <param name="renderingMode">the rendering mode to set</param>
        /// <returns>this checkbox instance</returns>
        public virtual iText.Forms.Form.Element.CheckBox SetRenderingMode(RenderingMode? renderingMode) {
            if (renderingMode == null) {
                LOGGER.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.INVALID_VALUE_FALLBACK_TO_DEFAULT, "renderingMode"
                    , null));
                return this;
            }
            SetProperty(Property.RENDERING_MODE, renderingMode);
            return this;
        }

        /// <summary>Sets the PDF/A conformance level for the checkbox.</summary>
        /// <param name="conformanceLevel">the PDF/A conformance level to set</param>
        /// <returns>this checkbox instance</returns>
        public virtual iText.Forms.Form.Element.CheckBox SetPdfAConformanceLevel(PdfAConformanceLevel conformanceLevel
            ) {
            SetProperty(FormProperty.FORM_CONFORMANCE_LEVEL, conformanceLevel);
            return this;
        }

        /// <summary>Sets the icon of the checkbox.</summary>
        /// <param name="checkBoxType">the type of the checkbox to set</param>
        /// <returns>this checkbox instance</returns>
        public virtual iText.Forms.Form.Element.CheckBox SetCheckBoxType(CheckBoxType checkBoxType) {
            if (checkBoxType == null) {
                LOGGER.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.INVALID_VALUE_FALLBACK_TO_DEFAULT, "checkBoxType"
                    , null));
                return this;
            }
            SetProperty(FormProperty.FORM_CHECKBOX_TYPE, checkBoxType);
            return this;
        }

        /// <summary>Sets the size of the checkbox.</summary>
        /// <param name="size">the size of the checkbox to set, in points</param>
        /// <returns>this checkbox instance</returns>
        public virtual iText.Forms.Form.Element.CheckBox SetSize(float size) {
            if (size <= 0) {
                LOGGER.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.INVALID_VALUE_FALLBACK_TO_DEFAULT, "size"
                    , size));
                return this;
            }
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(size));
            SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(size));
            return this;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.element.AbstractElement#makeNewRenderer()
        */
        protected override IRenderer MakeNewRenderer() {
            return new CheckBoxRenderer(this);
        }
    }
}
