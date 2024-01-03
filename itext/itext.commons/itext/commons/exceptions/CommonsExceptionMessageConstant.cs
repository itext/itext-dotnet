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

namespace iText.Commons.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class CommonsExceptionMessageConstant {
        /// <summary>Message warns about overriding of the identifier of identifiable element.</summary>
        /// <remarks>
        /// Message warns about overriding of the identifier of identifiable element. List of params:
        /// <list type="bullet">
        /// <item><description>0th is an original element identifier;
        /// </description></item>
        /// <item><description>1st is a new element identifier;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String ELEMENT_ALREADY_HAS_IDENTIFIER = "Element already has sequence id: {0}, new id {1} " +
             "will be ignored";

        public const String FILE_SHOULD_EXIST = "File should exist.";

        public const String FILE_NAME_ALREADY_EXIST = "File name: {0}, already exists.";

        public const String FILE_NAME_CAN_NOT_BE_NULL = "File name can not be null.";

        public const String FILE_NAME_SHOULD_BE_UNIQUE = "File name should be unique.";

        public const String INVALID_USAGE_CONFIGURATION_FORBIDDEN = "Invalid usage of placeholder \"{0}\": any " +
             "configuration is forbidden";

        public const String INVALID_USAGE_FORMAT_REQUIRED = "Invalid usage of placeholder \"{0}\": format is " + "required";

        public const String JSON_OBJECT_CAN_NOT_BE_NULL = "Passed json object can not be null";

        public const String NO_EVENTS_WERE_REGISTERED_FOR_THE_DOCUMENT = "No events were registered for the " + "document!";

        public const String PATTERN_CONTAINS_OPEN_QUOTATION = "Pattern contains open quotation!";

        public const String PATTERN_CONTAINS_UNEXPECTED_CHARACTER = "Pattern contains unexpected character {0}";

        public const String PATTERN_CONTAINS_UNEXPECTED_COMPONENT = "Pattern contains unexpected component {0}";

        public const String PRODUCT_NAME_CAN_NOT_BE_NULL = "Product name can not be null.";

        public const String STREAM_CAN_NOT_BE_NULL = "Passed stream can not be null";

        public const String UNKNOWN_ITEXT_EXCEPTION = "Unknown ITextException.";

        public const String ZIP_ENTRY_NOT_FOUND = "Zip entry not found for name: {0}";

        private CommonsExceptionMessageConstant() {
        }
        // Empty constructor.
    }
}
