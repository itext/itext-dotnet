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
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class for sign type validation context.</summary>
    public class SignTypeValidationContext : IValidationContext {
        private readonly bool isCAdES;

        /// <summary>
        /// Instantiates a new
        /// <see cref="SignTypeValidationContext"/>
        /// based on whether sign is CAdeS or not.
        /// </summary>
        /// <param name="isCAdES">whether sign is CAdeS or not</param>
        public SignTypeValidationContext(bool isCAdES) {
            this.isCAdES = isCAdES;
        }

        /// <summary>Whether sign is CAdeS or not.</summary>
        /// <returns>whether sign is CAdeS or not</returns>
        public virtual bool IsCAdES() {
            return isCAdES;
        }

        public virtual ValidationType GetType() {
            return ValidationType.SIGNATURE_TYPE;
        }
    }
}
