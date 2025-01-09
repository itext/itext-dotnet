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
using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// and
    /// <see cref="AbstractPdfShadingBlend"/>
    /// classes
    /// and is in charge of Shading Dictionary with axial type, that define a colour blend that varies along
    /// a linear axis between two endpoints and extends indefinitely perpendicular to that axis.
    /// </summary>
    public class PdfAxialShading : AbstractPdfShadingBlend {
        /// <summary>
        /// Creates the new instance of the class from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <param name="pdfDictionary">
        /// from which this
        /// <see cref="PdfAxialShading"/>
        /// will be created
        /// </param>
        public PdfAxialShading(PdfDictionary pdfDictionary)
            : base(pdfDictionary) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="x0">the start coordinate of X axis expressed in the shading's target coordinate space</param>
        /// <param name="y0">the start coordinate of Y axis expressed in the shading's target coordinate space</param>
        /// <param name="color0">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the start point
        /// </param>
        /// <param name="x1">the end coordinate of X axis expressed in the shading's target coordinate space</param>
        /// <param name="y1">the end coordinate of Y axis expressed in the shading's target coordinate space</param>
        /// <param name="color1">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the end point
        /// </param>
        public PdfAxialShading(PdfColorSpace cs, float x0, float y0, float[] color0, float x1, float y1, float[] color1
            )
            : base(new PdfDictionary(), ShadingType.AXIAL, cs) {
            SetCoords(x0, y0, x1, y1);
            IPdfFunction func = new PdfType2Function(new float[] { 0, 1 }, null, color0, color1, 1);
            SetFunction(func);
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="x0">the start coordinate of X axis expressed in the shading's target coordinate space</param>
        /// <param name="y0">the start coordinate of Y axis expressed in the shading's target coordinate space</param>
        /// <param name="color0">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the start point
        /// </param>
        /// <param name="x1">the end coordinate of X axis expressed in the shading's target coordinate space</param>
        /// <param name="y1">the end coordinate of Y axis expressed in the shading's target coordinate space</param>
        /// <param name="color1">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the end point
        /// </param>
        /// <param name="extend">
        /// the array of two booleans that specified whether to extend the shading
        /// beyond the starting and ending points of the axis, respectively
        /// </param>
        public PdfAxialShading(PdfColorSpace cs, float x0, float y0, float[] color0, float x1, float y1, float[] color1
            , bool[] extend)
            : this(cs, x0, y0, color0, x1, y1, color1) {
            if (extend == null || extend.Length != 2) {
                throw new ArgumentException("extend");
            }
            SetExtend(extend[0], extend[1]);
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="coords">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of four numbers [x0 y0 x1 y1] that specified the starting
        /// and the endings coordinates of thew axis, expressed in the shading's target coordinate space
        /// </param>
        /// <param name="function">
        /// the
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object, that is used to calculate color transitions
        /// </param>
        public PdfAxialShading(PdfColorSpace cs, PdfArray coords, IPdfFunction function)
            : this(cs, coords, null, function) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="coords">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of four numbers [x0 y0 x1 y1] that specified
        /// the starting and the endings coordinates of thew axis, expressed
        /// in the shading's target coordinate space
        /// </param>
        /// <param name="domain">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two numbers [t0 t1] specifying the limiting values
        /// of a parametric variable t which is considered to vary linearly between
        /// these two values and becomes the input argument to the colour function
        /// </param>
        /// <param name="function">
        /// the
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object, that is used to calculate color transitions
        /// </param>
        public PdfAxialShading(PdfColorSpace cs, PdfArray coords, PdfArray domain, IPdfFunction function)
            : base(new PdfDictionary(), ShadingType.AXIAL, cs) {
            SetCoords(coords);
            if (domain != null) {
                SetDomain(domain);
            }
            SetFunction(function);
        }

        /// <summary>Sets the Choords object with the four params expressed in the shading's target coordinate space.</summary>
        /// <param name="x0">the start coordinate of X axis to be set</param>
        /// <param name="y0">the start coordinate of Y axis to be set</param>
        /// <param name="x1">the end coordinate of X axis to be set</param>
        /// <param name="y1">the end coordinate of Y axis to be set</param>
        public void SetCoords(float x0, float y0, float x1, float y1) {
            SetCoords(new PdfArray(new float[] { x0, y0, x1, y1 }));
        }
    }
}
