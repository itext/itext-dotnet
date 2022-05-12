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

namespace iText.Commons.Logs {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class CommonsLogMessageConstant {
        /// <summary>Message notifies that an invalid statistics name was received, because there is no aggregator for it.
        ///     </summary>
        /// <remarks>
        /// Message notifies that an invalid statistics name was received, because there is no aggregator for it.
        /// <list type="bullet">
        /// <item><description>0th is a statistics name which is invalid;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String INVALID_STATISTICS_NAME = "Statistics name {0} is invalid. Cannot find corresponding statistics aggregator.";

        /// <summary>Message notifies that files archiving operation failed.</summary>
        /// <remarks>
        /// Message notifies that files archiving operation failed.
        /// <list type="bullet">
        /// <item><description>0th is a message of thrown exception;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String LOCAL_FILE_COMPRESSION_FAILED = "Cannot archive files into zip. " + "Exception message: {0}.";

        /// <summary>Message notifies that some exception has been thrown during json deserialization from object.</summary>
        /// <remarks>
        /// Message notifies that some exception has been thrown during json deserialization from object.
        /// List of params:
        /// <list type="bullet">
        /// <item><description>0th is a class name of thrown exception;
        /// </description></item>
        /// <item><description>1st is a message of thrown exception;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String UNABLE_TO_DESERIALIZE_JSON = "Unable to deserialize json. Exception {0} was thrown with the message: {1}.";

        /// <summary>Message notifies that some exception has been thrown during json serialization to object.</summary>
        /// <remarks>
        /// Message notifies that some exception has been thrown during json serialization to object.
        /// List of params:
        /// <list type="bullet">
        /// <item><description>0th is a class name of thrown exception;
        /// </description></item>
        /// <item><description>1st is a message of thrown exception;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String UNABLE_TO_SERIALIZE_OBJECT = "Unable to serialize object. Exception {0} was thrown with the message: {1}.";

        /// <summary>
        /// Message notifies that unknown placeholder was ignored during parsing of the producer line
        /// format.
        /// </summary>
        /// <remarks>
        /// Message notifies that unknown placeholder was ignored during parsing of the producer line
        /// format. List of params:
        /// <list type="bullet">
        /// <item><description>0th is a name of ignored placeholder;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String UNKNOWN_PLACEHOLDER_WAS_IGNORED = "Unknown placeholder {0} was ignored";

        /// <summary>Message warns that some event is at confirmation stage but it is not known.</summary>
        /// <remarks>
        /// Message warns that some event is at confirmation stage but it is not known. Probably some processing has failed.
        /// List of params:
        /// <list type="bullet">
        /// <item><description>0th is a name of product for which event was reported;
        /// </description></item>
        /// <item><description>1st is an event type;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String UNREPORTED_EVENT = "Event for the product {0} with type {1} attempted to be confirmed but it had not been reported yet. "
             + "Probably appropriate process fail";

        private CommonsLogMessageConstant() {
        }
        // Private constructor will prevent the instantiation of this class directly.
    }
}
