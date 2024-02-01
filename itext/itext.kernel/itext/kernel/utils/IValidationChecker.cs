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
using System;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils {
    /// <summary>Used to check if a PDF document is compliant to a specific validation profile.</summary>
    public interface IValidationChecker {
        /// <summary>
        /// Validate the provided
        /// <see cref="ValidationContext"/>.
        /// </summary>
        /// <remarks>
        /// Validate the provided
        /// <see cref="ValidationContext"/>.
        /// <para />
        /// This method is called by the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.Close()"/>
        /// to check for additional conformance requirements.
        /// </remarks>
        /// <param name="validationContext">
        /// the
        /// <see cref="ValidationContext"/>
        /// to validate
        /// </param>
        void ValidateDocument(ValidationContext validationContext);

        /// <summary>Check the provided object for conformance.</summary>
        /// <remarks>
        /// Check the provided object for conformance.
        /// <para />
        /// This method is called by the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.CheckIsoConformance(System.Object, iText.Kernel.Pdf.IsoKey, iText.Kernel.Pdf.PdfResources, iText.Kernel.Pdf.PdfStream, System.Object)
        ///     "/>
        /// to check for additional conformance requirements.
        /// </remarks>
        /// <param name="obj">the object to check</param>
        /// <param name="key">
        /// the
        /// <see cref="iText.Kernel.Pdf.IsoKey"/>
        /// of the object
        /// </param>
        /// <param name="resources">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfResources"/>
        /// of the object
        /// </param>
        /// <param name="contentStream">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// of the object
        /// </param>
        /// <param name="extra">additional information</param>
        void ValidateObject(Object obj, IsoKey key, PdfResources resources, PdfStream contentStream, Object extra);
    }
}
