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
using iText.Forms.Exceptions;
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Forms.Logs;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>A field that represents a control for selecting one of the provided options.</summary>
    public class ComboBoxField : AbstractSelectField {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Element.ComboBoxField
            ));

        private String selectedExportValue;

        /// <summary>Creates a new select field box.</summary>
        /// <param name="id">the id</param>
        public ComboBoxField(String id)
            : base(id) {
        }

        /// <summary>Gets the export value of the selected option.</summary>
        /// <returns>the export value of the selected option. This may be null if no value has been selected.</returns>
        public virtual String GetSelectedExportValue() {
            return selectedExportValue;
        }

        /// <summary>Selects an option by its index.</summary>
        /// <remarks>Selects an option by its index. The index is zero-based.</remarks>
        /// <param name="index">the index of the option to select.</param>
        /// <returns>
        /// this
        /// <see cref="ComboBoxField"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.ComboBoxField SetSelected(int index) {
            if (index < 0 || index >= this.GetOptions().Count) {
                String message = MessageFormatUtil.Format(FormsExceptionMessageConstant.INDEX_OUT_OF_BOUNDS, index, this.GetOptions
                    ().Count);
                throw new IndexOutOfRangeException(message);
            }
            SetSelected(this.GetOptions()[index]);
            return this;
        }

        /// <summary>Selects an option by its export value.</summary>
        /// <param name="value">the export value of the option to select.</param>
        /// <returns>
        /// this
        /// <see cref="ComboBoxField"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.ComboBoxField SetSelected(String value) {
            ClearSelected();
            selectedExportValue = value;
            bool found = false;
            foreach (SelectFieldItem option in this.GetOptions()) {
                if (option.GetExportValue().Equals(value)) {
                    if (!found) {
                        option.GetElement().SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                        found = true;
                    }
                    else {
                        LOGGER.LogWarning(FormsLogMessageConstants.DUPLICATE_EXPORT_VALUE);
                    }
                }
            }
            return this;
        }

        /// <summary>Selects an option by its value.</summary>
        /// <remarks>
        /// Selects an option by its value. This will use the export value of the
        /// option to match it to existing options.
        /// </remarks>
        /// <param name="item">the option to select.</param>
        /// <returns>
        /// this
        /// <see cref="ComboBoxField"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.ComboBoxField SetSelected(SelectFieldItem item) {
            if (item == null) {
                return this;
            }
            SetSelected(item.GetExportValue());
            return this;
        }

        /// <summary>Add an option to the element.</summary>
        /// <param name="option">
        /// a
        /// <see cref="SelectFieldItem"/>.
        /// </param>
        public override void AddOption(SelectFieldItem option) {
            bool found = false;
            foreach (SelectFieldItem item in this.GetOptions()) {
                if (item.GetExportValue().Equals(option.GetExportValue())) {
                    found = true;
                    break;
                }
            }
            if (found) {
                LOGGER.LogWarning(FormsLogMessageConstants.DUPLICATE_EXPORT_VALUE);
            }
            base.AddOption(option);
        }

        /// <summary>Gets the selected option.</summary>
        /// <returns>the selected option. This may be null if no option has been selected.</returns>
        public virtual SelectFieldItem GetSelectedOption() {
            if (selectedExportValue == null) {
                return null;
            }
            foreach (SelectFieldItem option in this.GetOptions()) {
                if (option.GetExportValue().Equals(selectedExportValue)) {
                    return option;
                }
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new FormDefaultAccessibilityProperties(FormDefaultAccessibilityProperties.FORM_FIELD_LIST_BOX
                    );
            }
            return tagProperties;
        }

        protected override IRenderer MakeNewRenderer() {
            return new SelectFieldComboBoxRenderer(this);
        }

        private void ClearSelected() {
            this.selectedExportValue = null;
            foreach (SelectFieldItem option in this.GetOptions()) {
                option.GetElement().DeleteOwnProperty(FormProperty.FORM_FIELD_SELECTED);
            }
        }
    }
}
