/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
namespace iText.Forms.Form {
    /// <summary>Set of constants that will be used as keys to get and set properties.</summary>
    public sealed class FormProperty {
        /// <summary>The Constant PROPERTY_START.</summary>
        private const int PROPERTY_START = (1 << 21);

        /// <summary>The Constant FORM_FIELD_FLATTEN for form related properties.</summary>
        public const int FORM_FIELD_FLATTEN = PROPERTY_START + 1;

        /// <summary>The Constant FORM_FIELD_SIZE.</summary>
        public const int FORM_FIELD_SIZE = PROPERTY_START + 2;

        /// <summary>The Constant FORM_FIELD_VALUE.</summary>
        public const int FORM_FIELD_VALUE = PROPERTY_START + 3;

        /// <summary>The Constant FORM_FIELD_PASSWORD_FLAG.</summary>
        public const int FORM_FIELD_PASSWORD_FLAG = PROPERTY_START + 4;

        /// <summary>The Constant FORM_FIELD_COLS.</summary>
        public const int FORM_FIELD_COLS = PROPERTY_START + 5;

        /// <summary>The Constant FORM_FIELD_ROWS.</summary>
        public const int FORM_FIELD_ROWS = PROPERTY_START + 6;

        /// <summary>The Constant FORM_FIELD_CHECKED.</summary>
        public const int FORM_FIELD_CHECKED = PROPERTY_START + 7;

        /// <summary>The Constant FORM_FIELD_MULTIPLE.</summary>
        public const int FORM_FIELD_MULTIPLE = PROPERTY_START + 8;

        /// <summary>The Constant FORM_FIELD_SELECTED.</summary>
        public const int FORM_FIELD_SELECTED = PROPERTY_START + 9;

        /// <summary>The Constant FORM_FIELD_LABEL.</summary>
        public const int FORM_FIELD_LABEL = PROPERTY_START + 10;

        /// <summary>The Constant FORM_FIELD_RADIO_GROUP_NAME.</summary>
        public const int FORM_FIELD_RADIO_GROUP_NAME = PROPERTY_START + 12;

        /// <summary>The Constant FORM_FIELD_RADIO_BORDER_CIRCLE.</summary>
        public const int FORM_FIELD_RADIO_BORDER_CIRCLE = PROPERTY_START + 13;

        /// <summary>The Constant FORM_CHECKBOX_TYPE.</summary>
        public const int FORM_CHECKBOX_TYPE = PROPERTY_START + 14;

        /// <summary>The Constant FORM_CONFORMANCE_LEVEL.</summary>
        public const int FORM_CONFORMANCE_LEVEL = PROPERTY_START + 15;

        /// <summary>The Constant LIST_BOX_TOP_INDEX representing the index of the first visible option in a scrollable list.
        ///     </summary>
        public const int LIST_BOX_TOP_INDEX = PROPERTY_START + 16;

        /// <summary>
        /// The Constant TEXT_FIELD_COMB_FLAG representing
        /// <c>Comb</c>
        /// flag for the text field.
        /// </summary>
        public const int TEXT_FIELD_COMB_FLAG = PROPERTY_START + 17;

        /// <summary>The Constant TEXT_FIELD_MAX_LEN representing the maximum length of the field's text, in characters.
        ///     </summary>
        public const int TEXT_FIELD_MAX_LEN = PROPERTY_START + 18;

        private FormProperty() {
        }
        // Empty constructor.
    }
}
