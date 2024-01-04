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
using iText.Commons.Utils;
using iText.Forms.Exceptions;
using iText.Forms.Form;
using iText.Kernel.Exceptions;
using iText.Layout.Element;

namespace iText.Forms.Form.Element {
    /// <summary>A field that represents a control for selecting one of the provided options.</summary>
    /// <remarks>
    /// A field that represents a control for selecting one of the provided options.
    /// It is used in the
    /// <see cref="ComboBoxField"/>
    /// class.
    /// </remarks>
    public class SelectFieldItem {
        /// <summary>The export value of the item.</summary>
        /// <remarks>
        /// The export value of the item.
        /// this is the value of the form which will be submitted. If the display value is not set, the export value will be
        /// used as display value.
        /// </remarks>
        private readonly String exportValue;

        /// <summary>The display value of the item.</summary>
        /// <remarks>
        /// The display value of the item.
        /// This is the value which will be displayed in the dropdown.
        /// </remarks>
        private readonly String displayValue;

        /// <summary>The option element of the item.</summary>
        /// <remarks>
        /// The option element of the item.
        /// This is the element which will be displayed in the dropdown.
        /// It allows for customization
        /// </remarks>
        private readonly IBlockElement optionElement;

        /// <summary>
        /// Create a new
        /// <see cref="SelectFieldItem"/>.
        /// </summary>
        /// <param name="exportValue">the export value of the item.</param>
        /// <param name="displayValue">the display value of the item.</param>
        public SelectFieldItem(String exportValue, String displayValue)
            : this(exportValue, displayValue, new Paragraph(displayValue).SetMargin(0).SetMultipliedLeading(1)) {
        }

        /// <summary>
        /// Create a new
        /// <see cref="SelectFieldItem"/>.
        /// </summary>
        /// <param name="value">the export value of the item.</param>
        public SelectFieldItem(String value)
            : this(value, null, new Paragraph(value).SetMargin(0).SetMultipliedLeading(1)) {
        }

        /// <summary>
        /// Create a new
        /// <see cref="SelectFieldItem"/>.
        /// </summary>
        /// <param name="value">the export value of the item.</param>
        /// <param name="optionElement">the option element of the item.</param>
        public SelectFieldItem(String value, IBlockElement optionElement)
            : this(value, null, optionElement) {
        }

        /// <summary>
        /// Create a new
        /// <see cref="SelectFieldItem"/>.
        /// </summary>
        /// <param name="exportValue">the export value of the item.</param>
        /// <param name="displayValue">the display value of the item.</param>
        /// <param name="optionElement">the option element of the item.</param>
        public SelectFieldItem(String exportValue, String displayValue, IBlockElement optionElement) {
            if (exportValue == null) {
                throw new PdfException(MessageFormatUtil.Format(FormsExceptionMessageConstant.VALUE_SHALL_NOT_BE_NULL, "exportValue"
                    ));
            }
            this.exportValue = exportValue;
            this.displayValue = displayValue;
            if (optionElement == null) {
                throw new PdfException(FormsExceptionMessageConstant.OPTION_ELEMENT_SHALL_NOT_BE_NULL);
            }
            this.optionElement = optionElement;
            SetLabel();
        }

        /// <summary>Get the export value of the item.</summary>
        /// <returns>export value.</returns>
        public virtual String GetExportValue() {
            return exportValue;
        }

        /// <summary>Get the display value of the item.</summary>
        /// <remarks>
        /// Get the display value of the item.
        /// If the display value is not set, the export value will be used as display value.
        /// </remarks>
        /// <returns>display value.</returns>
        public virtual String GetDisplayValue() {
            if (displayValue != null) {
                return displayValue;
            }
            return exportValue;
        }

        /// <summary>Get the option element of the item.</summary>
        /// <remarks>
        /// Get the option element of the item.
        /// <para />
        /// This is the element which will be displayed in the dropdown.
        /// It allows for customization.
        /// </remarks>
        /// <returns>option element.</returns>
        public virtual IBlockElement GetElement() {
            return optionElement;
        }

        /// <summary>Check if the item has a display value.</summary>
        /// <remarks>Check if the item has a display value. and export value.</remarks>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the item has both export and display values,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool HasExportAndDisplayValues() {
            return exportValue != null && displayValue != null;
        }

        private void SetLabel() {
            optionElement.SetProperty(FormProperty.FORM_FIELD_LABEL, GetDisplayValue());
        }
    }
}
