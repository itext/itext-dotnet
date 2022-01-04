/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

namespace iText.StyledXmlParser.Logs {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class StyledXmlParserLogMessageConstant {
        /// <summary>The Constant SHORTHAND_PROPERTY_CANNOT_BE_EMPTY.</summary>
        public const String SHORTHAND_PROPERTY_CANNOT_BE_EMPTY = "{0} shorthand property cannot be empty.";

        /// <summary>The Constant DEFAULT_VALUE_OF_CSS_PROPERTY_UNKNOWN.</summary>
        public const String DEFAULT_VALUE_OF_CSS_PROPERTY_UNKNOWN = "Default value of the css property \"{0}\" is unknown.";

        /// <summary>The Constant ERROR_ADDING_CHILD_NODE.</summary>
        public const String ERROR_ADDING_CHILD_NODE = "Error adding child node.";

        /// <summary>The Constant ERROR_PARSING_COULD_NOT_MAP_NODE.</summary>
        public const String ERROR_PARSING_COULD_NOT_MAP_NODE = "Could not map node type: {0}";

        /// <summary>The Constant ERROR_PARSING_CSS_SELECTOR.</summary>
        public const String ERROR_PARSING_CSS_SELECTOR = "Error while parsing css selector: {0}";

        /// <summary>The Constant ONLY_THE_LAST_BACKGROUND_CAN_INCLUDE_BACKGROUND_COLOR.</summary>
        public const String ONLY_THE_LAST_BACKGROUND_CAN_INCLUDE_BACKGROUND_COLOR = "Only the last background can include a background color.";

        /// <summary>The Constant UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED.</summary>
        public const String UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED = "Unknown absolute metric length parsed \"{0}\".";

        public const String UNKNOWN_METRIC_ANGLE_PARSED = "Unknown metric angle parsed: \"{0}\".";

        /// <summary>The Constant UNKNOWN__PROPERTY.</summary>
        public const String UNKNOWN_PROPERTY = "Unknown {0} property: \"{1}\".";

        public const String URL_IS_EMPTY_IN_CSS_EXPRESSION = "url function is empty in expression:{0}";

        public const String URL_IS_NOT_CLOSED_IN_CSS_EXPRESSION = "url function is not properly closed in expression:{0}";

        /// <summary>The Constant QUOTES_PROPERTY_INVALID.</summary>
        public const String QUOTES_PROPERTY_INVALID = "Quote property \"{0}\" is invalid. It should contain even number of <string> values.";

        /// <summary>The Constant QUOTE_IS_NOT_CLOSED_IN_CSS_EXPRESSION.</summary>
        public const String QUOTE_IS_NOT_CLOSED_IN_CSS_EXPRESSION = "The quote is not closed in css expression: {0}";

        /// <summary>The Constant INVALID_CSS_PROPERTY_DECLARATION.</summary>
        public const String INVALID_CSS_PROPERTY_DECLARATION = "Invalid css property declaration: {0}";

        /// <summary>The Constant INCORRECT_CHARACTER_SEQUENCE.</summary>
        public const String INCORRECT_CHARACTER_SEQUENCE = "Incorrect character sequence.";

        public const String INCORRECT_RESOLUTION_UNIT_VALUE = "Resolution value unit should be either dpi, dppx or dpcm!";

        /// <summary>The Constant RULE_IS_NOT_SUPPORTED.</summary>
        public const String RULE_IS_NOT_SUPPORTED = "The rule @{0} is unsupported. All selectors in this rule will be ignored.";

        /// <summary>The Constant RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT.</summary>
        public const String RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT = "Resource with given URL ({0}) was filtered out.";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI.</summary>
        public const String UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI = "Unable to retrieve image with data URI {0}";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_RESOURCE_WITH_GIVEN_RESOURCE_SIZE_BYTE_LIMIT.</summary>
        public const String UNABLE_TO_RETRIEVE_RESOURCE_WITH_GIVEN_RESOURCE_SIZE_BYTE_LIMIT = "Unable to retrieve resource with given URL ({0}) and resource size byte limit ({1}).";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI.</summary>
        public const String UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI = "Unable to retrieve image with given base URI ({0}) and image source path ({1})";

        public const String UNABLE_TO_RESOLVE_IMAGE_URL = "Unable to resolve image path with given base URI ({0}) and image source path ({1})";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI.</summary>
        public const String UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI = "Unable to retrieve stream with given base URI ({0}) and source path ({1})";

        public const String UNABLE_TO_PROCESS_EXTERNAL_CSS_FILE = "Unable to process external css file";

        public const String UNABLE_TO_RETRIEVE_FONT = "Unable to retrieve font:\n {0}";

        public const String UNSUPPORTED_PSEUDO_CSS_SELECTOR = "Unsupported pseudo css selector: {0}";

        /// <summary>The Constant WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES.</summary>
        public const String WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES = "Was not able to define one of the background CSS shorthand properties: {0}";

        /// <summary>The Constant ERROR_RESOLVING_PARENT_STYLES.</summary>
        public const String ERROR_RESOLVING_PARENT_STYLES = "Element parent styles are not resolved. Styles for current element might be incorrect.";

        /// <summary>Instantiates a new log message constant.</summary>
        private StyledXmlParserLogMessageConstant() {
        }
        //Private constructor will prevent the instantiation of this class directly
    }
}
