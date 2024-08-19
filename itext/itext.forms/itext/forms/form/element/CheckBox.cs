/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Forms.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
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
            SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            SetChecked(false);
        }

        /// <summary>Sets the checked state of the checkbox.</summary>
        /// <param name="checked">the checked state to set</param>
        /// <returns>this checkbox instance</returns>
        public virtual iText.Forms.Form.Element.CheckBox SetChecked(bool @checked) {
            SetProperty(FormProperty.FORM_FIELD_CHECKED, @checked);
            return this;
        }

        /// <summary>Sets the conformance level for the checkbox.</summary>
        /// <param name="conformanceLevel">The PDF/A conformance level to set.</param>
        /// <returns>this checkbox instance</returns>
        public virtual iText.Forms.Form.Element.CheckBox SetPdfConformanceLevel(IConformanceLevel conformanceLevel
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
        public override iText.Forms.Form.Element.CheckBox SetSize(float size) {
            if (size <= 0) {
                LOGGER.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.INVALID_VALUE_FALLBACK_TO_DEFAULT, "size"
                    , size));
                return this;
            }
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(size));
            SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(size));
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new FormDefaultAccessibilityProperties(FormDefaultAccessibilityProperties.FORM_FIELD_CHECK
                    );
            }
            if (tagProperties is FormDefaultAccessibilityProperties) {
                ((FormDefaultAccessibilityProperties)tagProperties).UpdateCheckedValue(this);
            }
            return tagProperties;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.element.AbstractElement#makeNewRenderer()
        */
        protected override IRenderer MakeNewRenderer() {
            return new CheckBoxRenderer(this);
        }
    }
}
