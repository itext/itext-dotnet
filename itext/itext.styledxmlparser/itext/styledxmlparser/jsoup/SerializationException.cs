/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.StyledXmlParser.Jsoup {
    /// <summary>A SerializationException is raised whenever serialization of a DOM element fails.</summary>
    /// <remarks>
    /// A SerializationException is raised whenever serialization of a DOM element fails. This exception usually wraps an
    /// <see cref="System.IO.IOException"/>
    /// that may be thrown due to an inaccessible output stream.
    /// </remarks>
    public sealed class SerializationException : Exception {
        /// <summary>Creates and initializes a new serialization exception with no error message and cause.</summary>
        public SerializationException()
            : base() {
        }

        /// <summary>Creates and initializes a new serialization exception with the given error message and no cause.</summary>
        /// <param name="message">the error message of the new serialization exception (may be <c>null</c>).</param>
        public SerializationException(String message)
            : base(message) {
        }

        /// <summary>
        /// Creates and initializes a new serialization exception with the specified cause and an error message of
        /// <c>(cause==null ? null : cause.toString())</c> (which typically contains the class and error message of
        /// <c>cause</c>).
        /// </summary>
        /// <param name="cause">the cause of the new serialization exception (may be <c>null</c>).</param>
        public SerializationException(Exception cause)
            : base(cause == null ? "Exception with null cause" : cause.Message, cause) {
        }

        /// <summary>Creates and initializes a new serialization exception with the given error message and cause.</summary>
        /// <param name="message">the error message of the new serialization exception.</param>
        /// <param name="cause">the cause of the new serialization exception.</param>
        public SerializationException(String message, Exception cause)
            : base(message, cause) {
        }
    }
}
