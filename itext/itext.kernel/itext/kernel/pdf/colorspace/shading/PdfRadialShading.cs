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
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// and
    /// <see cref="AbstractPdfShadingBlend"/>
    /// classes
    /// and is in charge of Shading Dictionary with radial type, that defines a colour blend that varies between two circles.
    /// </summary>
    /// <remarks>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// and
    /// <see cref="AbstractPdfShadingBlend"/>
    /// classes
    /// and is in charge of Shading Dictionary with radial type, that defines a colour blend that varies between two circles.
    /// <para />
    /// This type of shading shall not be used with an Indexed colour space
    /// </remarks>
    public class PdfRadialShading : AbstractPdfShadingBlend {
        /// <summary>
        /// Creates the new instance of the class from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <param name="pdfDictionary">
        /// from which this
        /// <see cref="PdfRadialShading"/>
        /// will be created
        /// </param>
        public PdfRadialShading(PdfDictionary pdfDictionary)
            : base(pdfDictionary) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The Indexed color space isn't excepted
        /// </param>
        /// <param name="x0">the X coordinate of starting circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="y0">the Y coordinate of starting circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="r0">
        /// the radius of starting circle's centre, should be greater or equal to 0.
        /// If 0 then starting circle is treated as point.
        /// If both radii are 0, nothing shall be painted
        /// </param>
        /// <param name="color0">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the start circle
        /// </param>
        /// <param name="x1">the X coordinate of ending circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="y1">the Y coordinate of ending circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="r1">
        /// the radius of ending circle's centre, should be greater or equal to 0.
        /// If 0 then ending circle is treated as point.
        /// If both radii are 0, nothing shall be painted
        /// </param>
        /// <param name="color1">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the end circle
        /// </param>
        public PdfRadialShading(PdfColorSpace cs, float x0, float y0, float r0, float[] color0, float x1, float y1
            , float r1, float[] color1)
            : base(new PdfDictionary(), ShadingType.RADIAL, cs) {
            SetCoords(x0, y0, r0, x1, y1, r1);
            IPdfFunction func = new PdfType2Function(new float[] { 0, 1 }, null, color0, color1, 1);
            SetFunction(func);
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The Indexed color space isn't excepted
        /// </param>
        /// <param name="x0">the X coordinate of starting circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="y0">the Y coordinate of starting circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="r0">
        /// the radius of starting circle's centre, should be greater or equal to 0.
        /// If 0 then starting circle is treated as point.
        /// If both radii are 0, nothing shall be painted
        /// </param>
        /// <param name="color0">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the start circle
        /// </param>
        /// <param name="x1">the X coordinate of ending circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="y1">the Y coordinate of ending circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="r1">
        /// the radius of ending circle's centre, should be greater or equal to 0.
        /// If 0 then ending circle is treated as point.
        /// If both radii are 0, nothing shall be painted
        /// </param>
        /// <param name="color1">
        /// the
        /// <c>float[]</c>
        /// that represents the color in the end circle
        /// </param>
        /// <param name="extend">
        /// the array of two
        /// <c>boolean</c>
        /// that specified whether to extend the shading
        /// beyond the starting and ending points of the axis, respectively
        /// </param>
        public PdfRadialShading(PdfColorSpace cs, float x0, float y0, float r0, float[] color0, float x1, float y1
            , float r1, float[] color1, bool[] extend)
            : this(cs, x0, y0, r0, color0, x1, y1, r1, color1) {
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
        /// The Indexed color space isn't excepted
        /// </param>
        /// <param name="coords">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of of six numbers [x0 y0 r0 x1 y1 r1],
        /// specifying the centres and radii of the starting and ending circles,
        /// expressed in the shading’s target coordinate space.
        /// The radii r0 and r1 shall both be greater than or equal to 0.
        /// If one radius is 0, the corresponding circle shall be treated as a point;
        /// if both are 0, nothing shall be painted
        /// </param>
        /// <param name="function">
        /// the
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object, that is used to calculate color transitions
        /// </param>
        public PdfRadialShading(PdfColorSpace cs, PdfArray coords, IPdfFunction function)
            : base(new PdfDictionary(), ShadingType.RADIAL, cs) {
            SetCoords(coords);
            SetFunction(function);
        }

        /// <summary>Sets the coords object.</summary>
        /// <param name="x0">the X coordinate of starting circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="y0">the Y coordinate of starting circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="r0">
        /// the radius of starting circle's centre, should be greater or equal to 0.
        /// If 0 then starting circle is treated as point.
        /// If both radii are 0, nothing shall be painted
        /// </param>
        /// <param name="x1">the X coordinate of ending circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="y1">the Y coordinate of ending circle's centre, expressed in in the shading’s target coordinate space
        ///     </param>
        /// <param name="r1">
        /// the radius of ending circle's centre, should be greater or equal to 0.
        /// If 0 then ending circle is treated as point.
        /// If both radii are 0, nothing shall be painted
        /// </param>
        public void SetCoords(float x0, float y0, float r0, float x1, float y1, float r1) {
            SetCoords(new PdfArray(new float[] { x0, y0, r0, x1, y1, r1 }));
        }
    }
}
