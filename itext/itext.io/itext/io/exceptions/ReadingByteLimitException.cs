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
using iText.Commons.Exceptions;

namespace iText.IO.Exceptions {
    /// <summary>Thrown when the limit on the number of bytes read was violated.</summary>
    public class ReadingByteLimitException : ITextException {
        /// <summary>
        /// Creates a new
        /// <see cref="ReadingByteLimitException"/>
        /// instance.
        /// </summary>
        public ReadingByteLimitException() {
        }
        // Empty constructor
    }
}
