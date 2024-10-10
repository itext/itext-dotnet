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
namespace iText.Kernel.Pdf {
    /// <summary>
    /// The class is helper which used inside
    /// <see cref="PdfDocument"/>
    /// to properly configure PDF document's info dictionary.
    /// </summary>
    public class DocumentInfoHelper {
        /// <summary>If document info dictionary should be added to the trailer.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if should be added, otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool ShouldAddDocumentInfoToTrailer() {
            return true;
        }

        /// <summary>
        /// Adjusts document info before it's flushing and adding to the trailer
        /// if required, see
        /// <see cref="ShouldAddDocumentInfoToTrailer()"/>.
        /// </summary>
        /// <param name="documentInfo">
        /// the
        /// <see cref="PdfDocumentInfo"/>
        /// instance to adjust
        /// </param>
        public virtual void AdjustDocumentInfo(PdfDocumentInfo documentInfo) {
        }
        // do nothing
    }
}
