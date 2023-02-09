/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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

namespace iText.Forms.Logs {
    /// <summary>Class containing constants to be used in logging in forms module.</summary>
    public sealed class FormsLogMessageConstants {
        public const String ANNOTATION_IN_ACROFORM_DICTIONARY = "Annotation is noticed directly in fields array " 
            + "of AcroForm dictionary. It violates pdf specification.";

        public const String CANNOT_CREATE_FORMFIELD = "Cannot create form field from a given PDF object: {0}";

        public const String PROVIDE_FORMFIELD_NAME = "No form field name provided. Process will not be continued.";

        public const String FORM_FIELD_WAS_FLUSHED = "A form field was flushed. There's no way to create this field in the AcroForm dictionary.";

        public const String INCORRECT_PAGEROTATION = "Encounterd a page rotation that was not a multiple of 90°/ (Pi/2) when generating default appearances "
             + "for form fields";

        public const String NO_FIELDS_IN_ACROFORM = "Required AcroForm entry /Fields does not exist in the document. Empty array /Fields will be created.";

        public const String UNSUPPORTED_COLOR_IN_DA = "Unsupported color in FormField's DA";

        public const String N_ENTRY_IS_REQUIRED_FOR_APPEARANCE_DICTIONARY = "\\N entry is required to be present in an appearance dictionary.";

        private FormsLogMessageConstants() {
        }
    }
}
