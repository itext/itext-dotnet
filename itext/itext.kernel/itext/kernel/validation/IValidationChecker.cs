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
using iText.Kernel.Pdf;

namespace iText.Kernel.Validation {
    /// <summary>Used to check if a PDF document is compliant to a specific validation profile.</summary>
    public interface IValidationChecker {
        /// <summary>
        /// Validate the provided
        /// <see cref="IValidationContext"/>.
        /// </summary>
        /// <param name="validationContext">
        /// the
        /// <see cref="IValidationContext"/>
        /// to validate
        /// </param>
        void Validate(IValidationContext validationContext);

        /// <summary>
        /// Is
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// ready to flush.
        /// </summary>
        /// <param name="object">the pdf object to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the object is ready to flush,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        bool IsPdfObjectReadyToFlush(PdfObject @object);
    }
}
