/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

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

namespace iText.Svg.Exceptions {
    /// <summary>Class that holds the logging and exception messages.</summary>
    public sealed class SvgLogMessageConstant {
        private SvgLogMessageConstant() {
        }

        public const String ATTRIBUTES_NULL = "The attributes of this element are null.";

        public const String COORDINATE_VALUE_ABSENT = "The coordinate value is empty or null.";

        public const String COULDNOTINSTANTIATE = "Could not instantiate Renderer for tag {0}";

        public const String DRAW_NO_DRAW = "Can't draw a NoDrawOperationSvgNodeRenderer.";

        public const String ERROR_CLOSING_CSS_STREAM = "An error occured when trying to close the InputStream of the default CSS.";

        public const String ERROR_INITIALIZING_DEFAULT_CSS = "Error loading the default CSS. Initializing an empty style sheet.";

        public const String FLOAT_PARSING_NAN = "The passed value is not a number.";

        public const String FONT_NOT_FOUND = "The font wasn't found.";

        public const String INODEROOTISNULL = "Input root value is null";

        public const String INVALID_TRANSFORM_DECLARATION = "Transformation declaration is not formed correctly.";

        public const String LOOP = "Loop detected";

        public const String NAMED_OBJECT_NAME_NULL_OR_EMPTY = "The name of the named object can't be null or empty.";

        public const String NAMED_OBJECT_NULL = "A named object can't be null.";

        public const String NOROOT = "No root found";

        public const String PARAMETER_CANNOT_BE_NULL = "Parameters for this method cannot be null.";

        public const String ROOT_SVG_NO_BBOX = "The root svg tag needs to have a bounding box defined.";

        public const String POINTS_ATTRIBUTE_INVALID_LIST = "Points attribute {0} on polyline tag does not contain a valid set of points";

        public const String TAGPARAMETERNULL = "Tag parameter must not be null";

        public const String TRANSFORM_EMPTY = "The transformation value is empty.";

        public const String TRANSFORM_INCORRECT_NUMBER_OF_VALUES = "Transformation doesn't contain the right number of values.";

        public const String TRANSFORM_INCORRECT_VALUE_TYPE = "The transformation value is not a number.";

        public const String TRANSFORM_NULL = "The transformation value is null.";

        public const String UNMAPPEDTAG = "Could not find implementation for tag {0}";

        public const String UNKNOWN_TRANSFORMATION_TYPE = "Unsupported type of transformation.";

        public const String UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI = "Unable to retrieve stream with given base URI ({0}) and source path ({1})";

        /// <summary>Message in case the font provider doesn't know about any fonts.</summary>
        public const String FONT_PROVIDER_CONTAINS_ZERO_FONTS = "Font Provider contains zero fonts. At least one font shall be present";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_FONT.</summary>
        public const String UNABLE_TO_RETRIEVE_FONT = "Unable to retrieve font:\n {0}";
    }
}
