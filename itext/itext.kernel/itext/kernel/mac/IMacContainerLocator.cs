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

namespace iText.Kernel.Mac {
    /// <summary>
    /// Strategy interface, which is responsible for
    /// <see cref="AbstractMacIntegrityProtector"/>
    /// container location.
    /// </summary>
    /// <remarks>
    /// Strategy interface, which is responsible for
    /// <see cref="AbstractMacIntegrityProtector"/>
    /// container location.
    /// Expected to be used in
    /// <see cref="iText.Commons.Utils.DIContainer"/>.
    /// </remarks>
    public interface IMacContainerLocator {
        /// <summary>
        /// Locates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// container.
        /// </summary>
        /// <param name="macIntegrityProtector">
        /// 
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// container to be located
        /// </param>
        void LocateMacContainer(AbstractMacIntegrityProtector macIntegrityProtector);

        /// <summary>
        /// Creates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// from explicitly provided MAC properties.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which MAC container shall be created
        /// </param>
        /// <param name="macProperties">
        /// 
        /// <see cref="MacProperties"/>
        /// to be used for MAC container creation
        /// </param>
        /// <returns>
        /// 
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// which specific implementation depends on interface implementation.
        /// </returns>
        AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, MacProperties macProperties
            );

        /// <summary>
        /// Creates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// from already existing AuthCode dictionary.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which MAC container shall be created
        /// </param>
        /// <param name="authDictionary">
        /// AuthCode
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which contains MAC related information
        /// </param>
        /// <returns>
        /// 
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// which specific implementation depends on interface implementation.
        /// </returns>
        AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, PdfDictionary authDictionary
            );

        /// <summary>Handles MAC validation error.</summary>
        /// <param name="exception">
        /// 
        /// <see cref="MacValidationException"/>
        /// to handle.
        /// </param>
        void HandleMacValidationError(MacValidationException exception);
    }
}
