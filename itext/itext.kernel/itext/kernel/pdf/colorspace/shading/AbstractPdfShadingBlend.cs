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
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The PdfShadingBlend class which extends
    /// <see cref="AbstractPdfShading"/>
    /// and represents shadings which are
    /// based on a blend, with Coords, Domain and Extend fields in the PDF object.
    /// </summary>
    public abstract class AbstractPdfShadingBlend : AbstractPdfShading {
        /// <summary>
        /// Gets the coords
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// object.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// coords object
        /// </returns>
        public virtual PdfArray GetCoords() {
            return GetPdfObject().GetAsArray(PdfName.Coords);
        }

        /// <summary>
        /// Sets the Coords object with the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// ,
        /// that specified the starting and the endings coordinates of thew axis,
        /// expressed in the shading's target coordinate space.
        /// </summary>
        /// <param name="coords">
        /// the Chords
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// to be set
        /// </param>
        public void SetCoords(PdfArray coords) {
            GetPdfObject().Put(PdfName.Coords, coords);
            SetModified();
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two
        /// <c>float</c>
        /// [t0, t1] that represent the limiting values of a parametric
        /// variable t, that becomes an input of color function(s).
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of Domain object ([0.0 1.0] by default)
        /// </returns>
        public virtual PdfArray GetDomain() {
            PdfArray domain = GetPdfObject().GetAsArray(PdfName.Domain);
            if (domain == null) {
                domain = new PdfArray(new float[] { 0, 1 });
                SetDomain(domain);
            }
            return domain;
        }

        /// <summary>
        /// Sets the Domain with the array of two
        /// <c>float</c>
        /// [t0, t1] that represent the limiting values
        /// of a parametric variable t, that becomes an input of color function(s).
        /// </summary>
        /// <param name="t0">first limit of variable t</param>
        /// <param name="t1">second limit of variable t</param>
        public void SetDomain(float t0, float t1) {
            SetDomain(new PdfArray(new float[] { t0, t1 }));
        }

        /// <summary>
        /// Sets the Domain with the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two
        /// <c>float</c>
        /// [t0, t1] that represent the limiting values
        /// of a parametric variable t, that becomes an input of color function(s).
        /// </summary>
        /// <param name="domain">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// that represents domain
        /// </param>
        public void SetDomain(PdfArray domain) {
            GetPdfObject().Put(PdfName.Domain, domain);
            SetModified();
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two
        /// <c>boolean</c>
        /// that specified whether to extend the shading
        /// beyond the starting and ending points of the axis, respectively.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of Extended object ([false false] by default)
        /// </returns>
        public virtual PdfArray GetExtend() {
            PdfArray extend = GetPdfObject().GetAsArray(PdfName.Extend);
            if (extend == null) {
                extend = new PdfArray(new bool[] { false, false });
                SetExtend(extend);
            }
            return extend;
        }

        /// <summary>
        /// Sets the Extend object with the two
        /// <c>boolean</c>
        /// value.
        /// </summary>
        /// <param name="extendStart">if true will extend shading beyond the starting point of Coords</param>
        /// <param name="extendEnd">if true will extend shading beyond the ending point of Coords</param>
        public void SetExtend(bool extendStart, bool extendEnd) {
            SetExtend(new PdfArray(new bool[] { extendStart, extendEnd }));
        }

        /// <summary>
        /// Sets the Extend object with the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two
        /// <c>boolean</c>.
        /// </summary>
        /// <remarks>
        /// Sets the Extend object with the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two
        /// <c>boolean</c>.
        /// If first is true shading will extend beyond the starting point of Coords.
        /// If second is true shading will extend beyond the ending point of Coords.
        /// </remarks>
        /// <param name="extend">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// representing Extend object
        /// </param>
        public void SetExtend(PdfArray extend) {
            GetPdfObject().Put(PdfName.Extend, extend);
            SetModified();
        }

        /// <summary>Constructor for PdfShadingBlend object using a PdfDictionary.</summary>
        /// <param name="pdfObject">input PdfDictionary</param>
        protected internal AbstractPdfShadingBlend(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Constructor for PdfShadingBlend object using PdfDictionary, shading type and colorspace value.</summary>
        /// <param name="pdfObject">input PdfDictionary</param>
        /// <param name="shadingType">shading type</param>
        /// <param name="cs">color space</param>
        protected internal AbstractPdfShadingBlend(PdfDictionary pdfObject, int shadingType, PdfColorSpace cs)
            : base(pdfObject, shadingType, cs) {
        }
    }
}
