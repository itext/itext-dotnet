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
using iText.Signatures.Validation;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This exception is thrown when there is invalid data in the country-specific Lotl (List of Trusted Lists).
    ///     </summary>
    /// <remarks>
    /// This exception is thrown when there is invalid data in the country-specific Lotl (List of Trusted Lists).
    /// It extends
    /// <see cref="iText.Signatures.Validation.SafeCallingAvoidantException"/>
    /// to indicate that the issue is severe enough to avoid safe calling.
    /// </remarks>
    public class InvalidLotlDataException : SafeCallingAvoidantException {
        /// <summary>Constructs a new InvalidCountryLotlDataException with the specified detail message.</summary>
        /// <param name="message">the detail message</param>
        public InvalidLotlDataException(String message)
            : base(message) {
        }

        /// <summary>
        /// Constructs a new InvalidCountryLotlDataException with the specified detail message and an object for more
        /// details.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="obj">an object providing additional context or details about the exception</param>
        public InvalidLotlDataException(String message, Object obj)
            : base(message, obj) {
        }
    }
}
