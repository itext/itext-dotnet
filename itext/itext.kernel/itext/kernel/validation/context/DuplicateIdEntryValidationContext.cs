/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class for duplicate ID entry in structure element tree validation.</summary>
    public class DuplicateIdEntryValidationContext : IValidationContext {
        private readonly PdfString id;

        /// <summary>
        /// Instantiates a new
        /// <see cref="DuplicateIdEntryValidationContext"/>
        /// based on ID string.
        /// </summary>
        /// <param name="id">the ID of the entry</param>
        public DuplicateIdEntryValidationContext(PdfString id) {
            this.id = id;
        }

        /// <summary>Gets the ID of the entry.</summary>
        /// <returns>the ID</returns>
        public virtual PdfString GetId() {
            return id;
        }

        public virtual ValidationType GetType() {
            return ValidationType.DUPLICATE_ID_ENTRY;
        }
    }
}
