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
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The PdfShadingMeshFlags class which extends
    /// <see cref="AbstractPdfShading"/>
    /// and
    /// <see cref="AbstractPdfShadingMesh"/>
    /// and represents shadings which are based on a mesh, with all fields from
    /// <see cref="AbstractPdfShadingMesh"/>
    /// as well as BitsPerFlag in the PDF object.
    /// </summary>
    public abstract class AbstractPdfShadingMeshWithFlags : AbstractPdfShadingMesh {
        /// <summary>Gets the number of bits used to represent the edge flag for each vertex.</summary>
        /// <remarks>
        /// Gets the number of bits used to represent the edge flag for each vertex.
        /// But only the least significant 2 bits in each flag value shall be used.
        /// The valid flag values are 0, 1, 2 or 3.
        /// </remarks>
        /// <returns>the number of bits. Can be 2, 4 or 8</returns>
        public virtual int GetBitsPerFlag() {
            return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
        }

        /// <summary>Sets the number of bits used to represent the edge flag for each vertex.</summary>
        /// <remarks>
        /// Sets the number of bits used to represent the edge flag for each vertex.
        /// But only the least significant 2 bits in each flag value shall be used.
        /// The valid flag values are 0, 1, 2 or 3.
        /// </remarks>
        /// <param name="bitsPerFlag">the number of bits to be set. Shall be 2, 4 or 8</param>
        public void SetBitsPerFlag(int bitsPerFlag) {
            GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
            SetModified();
        }

        /// <summary>Constructor for PdfShadingBlend object using a PdfDictionary.</summary>
        /// <param name="pdfObject">input PdfDictionary</param>
        protected internal AbstractPdfShadingMeshWithFlags(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Constructor for PdfShadingBlend object using a PdfDictionary, shading type and color space.</summary>
        /// <param name="pdfObject">input PdfDictionary</param>
        /// <param name="type">shading type</param>
        /// <param name="colorSpace">color space</param>
        protected internal AbstractPdfShadingMeshWithFlags(PdfDictionary pdfObject, int type, PdfColorSpace colorSpace
            )
            : base(pdfObject, type, colorSpace) {
        }
    }
}
