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
using iText.Kernel.Exceptions;

namespace iText.Signatures.Validation {
    /// <summary>
    /// In some cases we need to propagate the exception without @{link SafeCalling} mechanism converting it to
    /// report items.
    /// </summary>
    /// <remarks>
    /// In some cases we need to propagate the exception without @{link SafeCalling} mechanism converting it to
    /// report items.
    /// This exception is used to indicate that something actually went wrong and not only the validation report is Invalid,
    /// but an underlying process might be affected.
    /// </remarks>
    public class SafeCallingAvoidantException : PdfException {
        // Needed to be able to modify class, which implements serializable.
        // Otherwise, japi-cmp won't allow any modifications.
        private const long serialVersionUID = -660602896592130700L;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SafeCallingAvoidantException"/>
        /// with the specified detail message.
        /// </summary>
        /// <param name="message">the detail message</param>
        public SafeCallingAvoidantException(String message)
            : base(message) {
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SafeCallingAvoidantException"/>
        /// with the specified underlying cause.
        /// </summary>
        /// <param name="cause">
        /// 
        /// <see cref="System.Exception"/>
        /// which caused the exception
        /// </param>
        public SafeCallingAvoidantException(Exception cause)
            : base(cause) {
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SafeCallingAvoidantException"/>
        /// with the specified detail message
        /// </summary>
        /// <param name="message">the detail message.</param>
        /// <param name="obj">an object for more details.</param>
        public SafeCallingAvoidantException(String message, Object obj)
            : this(message) {
            this.@object = obj;
        }
    }
}
