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
using System;
using iText.Commons.Exceptions;

namespace iText.StyledXmlParser.Exceptions {
    /// <summary>Runtime exception that gets thrown if something goes wrong in the HTML to PDF conversion.</summary>
    public class StyledXMLParserException : ITextException {
        /// <summary>The Constant INVALID_GRADIENT_VALUE.</summary>
        public const String INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST = "Invalid gradient function arguments list: {0}";

        /// <summary>The Constant INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING.</summary>
        public const String INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING = "Invalid direction string: {0}";

        /// <summary>The Constant INVALID_GRADIENT_COLOR_STOP_VALUE.</summary>
        public const String INVALID_GRADIENT_COLOR_STOP_VALUE = "Invalid color stop value: {0}";

        /// <summary>The Constant NAN.</summary>
        public const String NAN = "The passed value (@{0}) is not a number";

        /// <summary>Message in case the font provider doesn't know about any fonts.</summary>
        public const String FontProviderContainsZeroFonts = "Font Provider contains zero fonts. At least one font shall be present";

        /// <summary>The Constant UnsupportedEncodingException.</summary>
        public const String UnsupportedEncodingException = "Unsupported encoding exception.";

        /// <summary>
        /// Creates a new
        /// <see cref="StyledXMLParserException"/>
        /// instance.
        /// </summary>
        /// <param name="message">the message</param>
        public StyledXMLParserException(String message)
            : base(message) {
        }
    }
}
