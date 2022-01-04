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
