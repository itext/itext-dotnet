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

namespace iText.Commons.Exceptions {
    /// <summary>General iText exception.</summary>
    /// <remarks>
    /// General iText exception.
    /// <para />
    /// Important note, not all iText exceptions are extended from ITextException.
    /// </remarks>
    public class ITextException : Exception {
        /// <summary>Creates a new ITextException with no error message and cause.</summary>
        public ITextException()
            : base(CommonsExceptionMessageConstant.UNKNOWN_ITEXT_EXCEPTION) {
        }

        /// <summary>Creates a new ITextException.</summary>
        /// <param name="message">the detail message</param>
        public ITextException(String message)
            : base(message) {
        }

        /// <summary>Creates a new ITextException.</summary>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method)
        /// </param>
        public ITextException(Exception cause)
            : base(CommonsExceptionMessageConstant.UNKNOWN_ITEXT_EXCEPTION, cause) {
        }

        /// <summary>Creates a new ITextException.</summary>
        /// <param name="message">the detail message</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method)
        /// </param>
        public ITextException(String message, Exception cause)
            : base(message, cause) {
        }
    }
}
