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

namespace iText.Kernel.Exceptions {
    /// <summary>Exception class for exceptions occurred during decompressed pdf streams processing.</summary>
    public class MemoryLimitsAwareException : PdfException {
        /// <summary>Creates a new instance of MemoryLimitsAwareException.</summary>
        /// <param name="message">the detail message.</param>
        public MemoryLimitsAwareException(String message)
            : base(message) {
        }

        /// <summary>Creates a new instance of MemoryLimitsAwareException.</summary>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public MemoryLimitsAwareException(Exception cause)
            : this(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION, cause) {
        }

        /// <summary>Creates a new instance of MemoryLimitsAwareException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public MemoryLimitsAwareException(String message, Exception cause)
            : base(message, cause) {
        }
    }
}
