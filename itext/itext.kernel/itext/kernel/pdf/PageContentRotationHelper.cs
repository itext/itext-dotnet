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
namespace iText.Kernel.Pdf {
    /// <summary>
    /// Helper class to specify or check whether inverse matrix is already applied to the page content stream in case
    /// page rotation is applied and
    /// <see cref="PdfPage.SetIgnorePageRotationForContent(bool)"/>
    /// is set to
    /// <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// Helper class to specify or check whether inverse matrix is already applied to the page content stream in case
    /// page rotation is applied and
    /// <see cref="PdfPage.SetIgnorePageRotationForContent(bool)"/>
    /// is set to
    /// <see langword="true"/>.
    /// <para />
    /// Page rotation inverse matrix rotates content into the opposite direction from page rotation direction
    /// in order to give the impression of the not rotated text. It should be applied only once for the page.
    /// </remarks>
    public sealed class PageContentRotationHelper {
        /// <summary>
        /// Checks if page rotation inverse matrix (which rotates content into the opposite direction from the page rotation
        /// direction in order to give the impression of the not rotated text) is already applied to the page content stream.
        /// </summary>
        /// <remarks>
        /// Checks if page rotation inverse matrix (which rotates content into the opposite direction from the page rotation
        /// direction in order to give the impression of the not rotated text) is already applied to the page content stream.
        /// See
        /// <see cref="PdfPage.SetIgnorePageRotationForContent(bool)"/>.
        /// </remarks>
        /// <param name="page">
        /// 
        /// <see cref="PdfPage"/>
        /// to check applied content rotation for
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if inverse matrix is already applied,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsPageRotationInverseMatrixWritten(PdfPage page) {
            return page.IsPageRotationInverseMatrixWritten();
        }

        /// <summary>
        /// Specify that inverse matrix (which rotates content into the opposite direction from the page rotation
        /// direction in order to give the impression of the not rotated text) is applied to the page content stream.
        /// </summary>
        /// <remarks>
        /// Specify that inverse matrix (which rotates content into the opposite direction from the page rotation
        /// direction in order to give the impression of the not rotated text) is applied to the page content stream.
        /// See
        /// <see cref="PdfPage.SetIgnorePageRotationForContent(bool)"/>.
        /// </remarks>
        /// <param name="page">
        /// 
        /// <see cref="PdfPage"/>
        /// for which to specify that content rotation is applied
        /// </param>
        public static void SetPageRotationInverseMatrixWritten(PdfPage page) {
            page.SetPageRotationInverseMatrixWritten();
        }

        private PageContentRotationHelper() {
        }
        // Private constructor will prevent the instantiation of this class directly.
    }
}
