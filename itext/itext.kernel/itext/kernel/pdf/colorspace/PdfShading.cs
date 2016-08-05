/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Colorspace {
    /// <summary>The abstract PdfShading class that represents the Shading Dictionary PDF object.</summary>
    public abstract class PdfShading : PdfObjectWrapper<PdfDictionary> {
        /// <summary>
        /// constants of shading type
        /// <seealso>ISO-320001 Table 78</seealso>
        /// 
        /// </summary>
        private class ShadingType {
            /// <summary>The int value of function-based shading type</summary>
            public const int FUNCTION_BASED = 1;

            /// <summary>The int value of axial shading type</summary>
            public const int AXIAL = 2;

            /// <summary>The int value of radial shading type</summary>
            public const int RADIAL = 3;

            /// <summary>The int value of free-form Gouraud-shaded triangle mesh shading type</summary>
            public const int FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH = 4;

            /// <summary>The int value of lattice-form Gouraud-shaded triangle mesh shading type</summary>
            public const int LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH = 5;

            /// <summary>The int value of coons patch meshes shading type</summary>
            public const int COONS_PATCH_MESH = 6;

            /// <summary>The int value of tensor-product patch meshes shading type</summary>
            public const int TENSOR_PRODUCT_PATCH_MESH = 7;
        }

        /// <summary>
        /// Creates the
        /// <see cref="PdfShading"/>
        /// object from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// with corresponding type.
        /// </summary>
        /// <param name="shadingDictionary">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from which the
        /// <see cref="PdfShading"/>
        /// object will be created.
        /// </param>
        /// <returns>
        /// Created
        /// <see cref="PdfShading"/>
        /// object.
        /// </returns>
        public static PdfShading MakeShading(PdfDictionary shadingDictionary) {
            if (!shadingDictionary.ContainsKey(PdfName.ShadingType)) {
                throw new PdfException(PdfException.UnexpectedShadingType);
            }
            PdfShading shading;
            switch (shadingDictionary.GetAsNumber(PdfName.ShadingType).IntValue()) {
                case PdfShading.ShadingType.FUNCTION_BASED: {
                    shading = new PdfShading.FunctionBased(shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.AXIAL: {
                    shading = new PdfShading.Axial(shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.RADIAL: {
                    shading = new PdfShading.Radial(shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(PdfException.UnexpectedShadingType);
                    }
                    shading = new PdfShading.FreeFormGouraudShadedTriangleMesh((PdfStream)shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(PdfException.UnexpectedShadingType);
                    }
                    shading = new PdfShading.LatticeFormGouraudShadedTriangleMesh((PdfStream)shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.COONS_PATCH_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(PdfException.UnexpectedShadingType);
                    }
                    shading = new PdfShading.CoonsPatchMesh((PdfStream)shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.TENSOR_PRODUCT_PATCH_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(PdfException.UnexpectedShadingType);
                    }
                    shading = new PdfShading.TensorProductPatchMesh((PdfStream)shadingDictionary);
                    break;
                }

                default: {
                    throw new PdfException(PdfException.UnexpectedShadingType);
                }
            }
            return shading;
        }

        protected internal PdfShading(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        protected internal PdfShading(PdfDictionary pdfObject, int type, PdfObject colorSpace)
            : base(pdfObject) {
            GetPdfObject().Put(PdfName.ShadingType, new PdfNumber(type));
            GetPdfObject().Put(PdfName.ColorSpace, colorSpace);
        }

        /// <summary>Gets the shading type.</summary>
        /// <returns>
        /// int value of
        /// <see cref="ShadingType"/>
        /// .
        /// </returns>
        public virtual int GetShadingType() {
            return (int)GetPdfObject().GetAsInt(PdfName.ShadingType);
        }

        /// <summary>Gets the color space in which colour values shall be expressed.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// Color space
        /// </returns>
        public virtual PdfObject GetColorSpace() {
            return GetPdfObject().Get(PdfName.ColorSpace);
        }

        /// <summary>
        /// Gets the function PdfObject that represents color transitions
        /// across the shading geometry.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// Function
        /// </returns>
        public virtual PdfObject GetFunction() {
            return GetPdfObject().Get(PdfName.Function);
        }

        /// <summary>
        /// Sets the function that represents color transitions
        /// across the shading geometry as one object.
        /// </summary>
        /// <param name="function">
        /// The
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// to set.
        /// </param>
        public virtual void SetFunction(PdfFunction function) {
            GetPdfObject().Put(PdfName.Function, function.GetPdfObject());
            SetModified();
        }

        /// <summary>
        /// Sets the function object that represents color transitions
        /// across the shading geometry as an array of functions.
        /// </summary>
        /// <param name="functions">
        /// The array of
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// to be set.
        /// </param>
        public virtual void SetFunction(PdfFunction[] functions) {
            PdfArray arr = new PdfArray();
            foreach (PdfFunction func in functions) {
                arr.Add(func.GetPdfObject());
            }
            GetPdfObject().Put(PdfName.Function, arr);
            SetModified();
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </summary>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with function-based type,
        /// that defines color at every point in the domain by a specified mathematical function.
        /// </summary>
        public class FunctionBased : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
            /// object.
            /// </summary>
            /// <param name="pdfDictionary">
            /// 
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
            /// from which the instance is created.
            /// </param>
            [System.ObsoleteAttribute(@"Intended only for private use. You should use PdfShading.MakeShading(iText.Kernel.Pdf.PdfDictionary) instead."
                )]
            public FunctionBased(PdfDictionary pdfDictionary)
                : base(pdfDictionary) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="colorSpace">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// </param>
            /// <param name="function">
            /// the
            /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
            /// , that is used to calculate color transitions.
            /// </param>
            public FunctionBased(PdfColorSpace colorSpace, PdfFunction function)
                : this(colorSpace.GetPdfObject(), function) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="colorSpace">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfObject"/>
            /// , that represents color space in which colour values shall be expressed.
            /// </param>
            /// <param name="function">
            /// the
            /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
            /// , that is used to calculate color transitions.
            /// </param>
            public FunctionBased(PdfObject colorSpace, PdfFunction function)
                : base(new PdfDictionary(), PdfShading.ShadingType.FUNCTION_BASED, colorSpace) {
                SetFunction(function);
            }

            /// <summary>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// domain rectangle object that establishes an internal coordinate space
            /// for the shading that is independent of the target coordinate space in which it shall be painted.
            /// </summary>
            /// <returns>
            /// 
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// domain rectangle.
            /// </returns>
            public virtual PdfArray GetDomain() {
                return GetPdfObject().GetAsArray(PdfName.Domain);
            }

            /// <summary>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// domain rectangle object that establishes an internal coordinate space
            /// for the shading that is independent of the target coordinate space in which it shall be painted.
            /// </summary>
            /// <param name="xmin">the Xmin coordinate of rectangle.</param>
            /// <param name="xmax">the Xmax coordinate of rectangle.</param>
            /// <param name="ymin">the Ymin coordinate of rectangle.</param>
            /// <param name="ymax">the Ymax coordinate of rectangle.</param>
            public virtual void SetDomain(float xmin, float xmax, float ymin, float ymax) {
                GetPdfObject().Put(PdfName.Domain, new PdfArray(new float[] { xmin, xmax, ymin, ymax }));
                SetModified();
            }

            /// <summary>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// domain rectangle object that establishes an internal coordinate space
            /// for the shading that is independent of the target coordinate space in which it shall be painted.
            /// </summary>
            /// <param name="domain">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// domain rectangle object to be set.
            /// </param>
            public virtual void SetDomain(PdfArray domain) {
                GetPdfObject().Put(PdfName.Domain, domain);
                SetModified();
            }

            /// <summary>
            /// Gets the array of floats that represents the transformation matrix that maps the domain rectangle
            /// into a corresponding figure in the target coordinate space.
            /// </summary>
            /// <returns>
            /// the
            /// <c>float[]</c>
            /// of transformation matrix (identical matrix by default).
            /// </returns>
            public virtual float[] GetMatrix() {
                PdfArray matrix = GetPdfObject().GetAsArray(PdfName.Matrix);
                if (matrix == null) {
                    return new float[] { 1, 0, 0, 1, 0, 0 };
                }
                float[] result = new float[6];
                for (int i = 0; i < 6; i++) {
                    result[i] = matrix.GetAsNumber(i).FloatValue();
                }
                return result;
            }

            /// <summary>
            /// Sets the array of floats that represents the transformation matrix that maps the domain rectangle
            /// into a corresponding figure in the target coordinate space.
            /// </summary>
            /// <param name="matrix">
            /// the
            /// <c>float[]</c>
            /// of transformation matrix to be set.
            /// </param>
            public virtual void SetMatrix(float[] matrix) {
                SetMatrix(new PdfArray(matrix));
            }

            /// <summary>
            /// Sets the array of floats that represents the transformation matrix that maps the domain rectangle
            /// into a corresponding figure in the target coordinate space.
            /// </summary>
            /// <param name="matrix">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// transformation matrix object to be set.
            /// </param>
            public virtual void SetMatrix(PdfArray matrix) {
                GetPdfObject().Put(PdfName.Matrix, matrix);
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with axial type,
        /// that define a colour blend that varies along a linear axis between two endpoints
        /// and extends indefinitely perpendicular to that axis.
        /// </summary>
        public class Axial : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
            /// object.
            /// </summary>
            /// <param name="pdfDictionary">
            /// 
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
            /// from which the instance is created.
            /// </param>
            [System.ObsoleteAttribute(@"Intended only for private use. You should use PdfShading.MakeShading(iText.Kernel.Pdf.PdfDictionary) instead."
                )]
            public Axial(PdfDictionary pdfDictionary)
                : base(pdfDictionary) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="x0">the start coordinate of X axis expressed in the shading's target coordinate space.</param>
            /// <param name="y0">the start coordinate of Y axis expressed in the shading's target coordinate space.</param>
            /// <param name="color0">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the start point.
            /// </param>
            /// <param name="x1">the end coordinate of X axis expressed in the shading's target coordinate space.</param>
            /// <param name="y1">the end coordinate of Y axis expressed in the shading's target coordinate space.</param>
            /// <param name="color1">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the end point.
            /// </param>
            public Axial(PdfColorSpace cs, float x0, float y0, float[] color0, float x1, float y1, float[] color1)
                : base(new PdfDictionary(), PdfShading.ShadingType.AXIAL, cs.GetPdfObject()) {
                if (cs is PdfSpecialCs.Pattern) {
                    throw new ArgumentException("colorSpace");
                }
                SetCoords(x0, y0, x1, y1);
                PdfFunction func = new PdfFunction.Type2(new PdfArray(new float[] { 0, 1 }), null, new PdfArray(color0), new 
                    PdfArray(color1), new PdfNumber(1));
                SetFunction(func);
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="x0">the start coordinate of X axis expressed in the shading's target coordinate space.</param>
            /// <param name="y0">the start coordinate of Y axis expressed in the shading's target coordinate space.</param>
            /// <param name="color0">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the start point.
            /// </param>
            /// <param name="x1">the end coordinate of X axis expressed in the shading's target coordinate space.</param>
            /// <param name="y1">the end coordinate of Y axis expressed in the shading's target coordinate space.</param>
            /// <param name="color1">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the end point.
            /// </param>
            /// <param name="extend">
            /// the array of two booleans that specified whether to extend the shading
            /// beyond the starting and ending points of the axis, respectively.
            /// </param>
            public Axial(PdfColorSpace cs, float x0, float y0, float[] color0, float x1, float y1, float[] color1, bool
                [] extend)
                : this(cs, x0, y0, color0, x1, y1, color1) {
                if (extend == null || extend.Length != 2) {
                    throw new ArgumentException("extend");
                }
                SetExtend(extend[0], extend[1]);
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="coords">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of four number four numbers [x0 y0 x1 y1] that specified the starting
            /// and the endings coordinates of thew axis, expressed in the shading's target coordinate space.
            /// </param>
            /// <param name="function">
            /// the
            /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
            /// object, that is used to calculate color transitions.
            /// </param>
            public Axial(PdfColorSpace cs, PdfArray coords, PdfFunction function)
                : base(new PdfDictionary(), PdfShading.ShadingType.AXIAL, cs.GetPdfObject()) {
                SetCoords(coords);
                SetFunction(function);
            }

            /// <summary>
            /// Gets the Coords object - a
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of four numbers [x0 y0 x1 y1] that specified the starting
            /// and the endings coordinates of thew axis, expressed in the shading's target coordinate space.
            /// </summary>
            /// <returns>
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Coords object.
            /// </returns>
            public virtual PdfArray GetCoords() {
                return GetPdfObject().GetAsArray(PdfName.Coords);
            }

            /// <summary>Sets the Choords object with the four params expressed in the shading's target coordinate space.</summary>
            /// <param name="x0">the start coordinate of X axis to be set.</param>
            /// <param name="y0">the start coordinate of Y axis to be set.</param>
            /// <param name="x1">the end coordinate of X axis to be set.</param>
            /// <param name="y1">the end coordinate of Y axis to be set.</param>
            public virtual void SetCoords(float x0, float y0, float x1, float y1) {
                SetCoords(new PdfArray(new float[] { x0, y0, x1, y1 }));
            }

            /// <summary>
            /// Sets the Choords object with the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of four numbers [x0 y0 x1 y1],
            /// that specified the starting and the endings coordinates of thew axis,
            /// expressed in the shading's target coordinate space.
            /// </summary>
            /// <param name="coords">
            /// the Chords
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// to be set.
            /// </param>
            public virtual void SetCoords(PdfArray coords) {
                GetPdfObject().Put(PdfName.Coords, coords);
                SetModified();
            }

            /// <summary>
            /// Gets the array of two
            /// <c>float</c>
            /// [t0, t1] that represent the limiting values of a parametric
            /// variable t, that becomes an input of color function(s).
            /// </summary>
            /// <returns>
            /// 
            /// <c>float[]</c>
            /// of Domain object ([0.0 1.0] by default)
            /// </returns>
            public virtual float[] GetDomain() {
                PdfArray domain = GetPdfObject().GetAsArray(PdfName.Domain);
                if (domain == null) {
                    return new float[] { 0, 1 };
                }
                return new float[] { domain.GetAsNumber(0).FloatValue(), domain.GetAsNumber(1).FloatValue() };
            }

            /// <summary>
            /// Sets the Domain with the array of two
            /// <c>float</c>
            /// [t0, t1] that represent the limiting values
            /// of a parametric variable t, that becomes an input of color function(s).
            /// </summary>
            /// <param name="t0">first limit of variable t</param>
            /// <param name="t1">second limit of variable t</param>
            public virtual void SetDomain(float t0, float t1) {
                GetPdfObject().Put(PdfName.Domain, new PdfArray(new float[] { t0, t1 }));
                SetModified();
            }

            /// <summary>
            /// Gets the array of two
            /// <c>boolean</c>
            /// that specified whether to extend the shading
            /// beyond the starting and ending points of the axis, respectively.
            /// </summary>
            /// <returns>
            /// 
            /// <c>boolean[]</c>
            /// of Extended object ([false false] by default)
            /// </returns>
            public virtual bool[] GetExtend() {
                PdfArray extend = GetPdfObject().GetAsArray(PdfName.Extend);
                if (extend == null) {
                    return new bool[] { false, false };
                }
                return new bool[] { extend.GetAsBoolean(0).GetValue(), extend.GetAsBoolean(1).GetValue() };
            }

            /// <summary>
            /// Sets the Extend object with the two
            /// <c>boolean</c>
            /// value.
            /// </summary>
            /// <param name="extendStart">if true will extend shading beyond the starting point of Coords</param>
            /// <param name="extendEnd">if true will extend shading beyond the ending point of Coords</param>
            public virtual void SetExtend(bool extendStart, bool extendEnd) {
                GetPdfObject().Put(PdfName.Extend, new PdfArray(new bool[] { extendStart, extendEnd }));
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with radial type,
        /// that define a colour blend that varies between two circles.
        /// This type of shading shall not be used with an Indexed colour space
        /// </summary>
        public class Radial : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
            /// object.
            /// </summary>
            /// <param name="pdfDictionary">
            /// -
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
            /// from which the instance is created.
            /// </param>
            [System.ObsoleteAttribute(@"Intended only for private use. You should use PdfShading.MakeShading(iText.Kernel.Pdf.PdfDictionary) instead."
                )]
            public Radial(PdfDictionary pdfDictionary)
                : base(pdfDictionary) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The Indexed color space isn't excepted.
            /// </param>
            /// <param name="x0">the X coordinate of starting circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="y0">the Y coordinate of starting circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="r0">
            /// the radius of starting circle's centre, should be greater or equal to 0.
            /// If 0 then starting circle is treated as point.
            /// If both radii are 0, nothing shall be painted.
            /// </param>
            /// <param name="color0">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the start circle.
            /// </param>
            /// <param name="x1">the X coordinate of ending circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="y1">the Y coordinate of ending circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="r1">
            /// the radius of ending circle's centre, should be greater or equal to 0.
            /// If 0 then ending circle is treated as point.
            /// If both radii are 0, nothing shall be painted.
            /// </param>
            /// <param name="color1">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the end circle.
            /// </param>
            public Radial(PdfColorSpace cs, float x0, float y0, float r0, float[] color0, float x1, float y1, float r1
                , float[] color1)
                : base(new PdfDictionary(), PdfShading.ShadingType.RADIAL, cs.GetPdfObject()) {
                SetCoords(x0, y0, r0, x1, y1, r1);
                PdfFunction func = new PdfFunction.Type2(new PdfArray(new float[] { 0, 1 }), null, new PdfArray(color0), new 
                    PdfArray(color1), new PdfNumber(1));
                SetFunction(func);
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The Indexed color space isn't excepted.
            /// </param>
            /// <param name="x0">the X coordinate of starting circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="y0">the Y coordinate of starting circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="r0">
            /// the radius of starting circle's centre, should be greater or equal to 0.
            /// If 0 then starting circle is treated as point.
            /// If both radii are 0, nothing shall be painted.
            /// </param>
            /// <param name="color0">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the start circle.
            /// </param>
            /// <param name="x1">the X coordinate of ending circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="y1">the Y coordinate of ending circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="r1">
            /// the radius of ending circle's centre, should be greater or equal to 0.
            /// If 0 then ending circle is treated as point.
            /// If both radii are 0, nothing shall be painted.
            /// </param>
            /// <param name="color1">
            /// the
            /// <c>float[]</c>
            /// that represents the color in the end circle.
            /// </param>
            /// <param name="extend">
            /// the array of two
            /// <c>boolean</c>
            /// that specified whether to extend the shading
            /// beyond the starting and ending points of the axis, respectively.
            /// </param>
            public Radial(PdfColorSpace cs, float x0, float y0, float r0, float[] color0, float x1, float y1, float r1
                , float[] color1, bool[] extend)
                : this(cs, x0, y0, r0, color0, x1, y1, r1, color1) {
                if (extend != null) {
                    SetExtend(extend[0], extend[1]);
                }
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The Indexed color space isn't excepted.
            /// </param>
            /// <param name="coords">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of of six numbers [x0 y0 r0 x1 y1 r1],
            /// specifying the centres and radii of the starting and ending circles,
            /// expressed in the shading’s target coordinate space.
            /// The radii r0 and r1 shall both be greater than or equal to 0.
            /// If one radius is 0, the corresponding circle shall be treated as a point;
            /// if both are 0, nothing shall be painted.
            /// </param>
            /// <param name="function">
            /// the
            /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
            /// object, that is used to calculate color transitions.
            /// </param>
            public Radial(PdfColorSpace cs, PdfArray coords, PdfFunction function)
                : base(new PdfDictionary(), PdfShading.ShadingType.RADIAL, cs.GetPdfObject()) {
                SetCoords(coords);
                SetFunction(function);
            }

            /// <summary>
            /// Gets the coords
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// object - an array of six numbers [x0 y0 r0 x1 y1 r1],
            /// specifying the centres and radii of the starting and ending circles,
            /// expressed in the shading’s target coordinate space.
            /// The radii r0 and r1 shall both be greater than or equal to 0.
            /// If one radius is 0, the corresponding circle shall be treated as a point;
            /// if both are 0, nothing shall be painted.
            /// </summary>
            /// <returns>
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// coords object.
            /// </returns>
            public virtual PdfArray GetCoords() {
                return GetPdfObject().GetAsArray(PdfName.Coords);
            }

            /// <summary>Sets the coords object.</summary>
            /// <param name="x0">the X coordinate of starting circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="y0">the Y coordinate of starting circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="r0">
            /// the radius of starting circle's centre, should be greater or equal to 0.
            /// If 0 then starting circle is treated as point.
            /// If both radii are 0, nothing shall be painted.
            /// </param>
            /// <param name="x1">the X coordinate of ending circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="y1">the Y coordinate of ending circle's centre, expressed in in the shading’s target coordinate space.
            ///     </param>
            /// <param name="r1">
            /// the radius of ending circle's centre, should be greater or equal to 0.
            /// If 0 then ending circle is treated as point.
            /// If both radii are 0, nothing shall be painted.
            /// </param>
            public virtual void SetCoords(float x0, float y0, float r0, float x1, float y1, float r1) {
                SetCoords(new PdfArray(new float[] { x0, y0, r0, x1, y1, r1 }));
            }

            /// <summary>
            /// Sets the coords
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// object - an array of six numbers [x0 y0 r0 x1 y1 r1],
            /// specifying the centres and radii of the starting and ending circles,
            /// expressed in the shading’s target coordinate space.
            /// The radii r0 and r1 shall both be greater than or equal to 0.
            /// If one radius is 0, the corresponding circle shall be treated as a point;
            /// if both are 0, nothing shall be painted.
            /// </summary>
            /// <param name="coords">
            /// -
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// choords object to be set.
            /// </param>
            public virtual void SetCoords(PdfArray coords) {
                GetPdfObject().Put(PdfName.Coords, coords);
                SetModified();
            }

            /// <summary>
            /// Gets the array of two
            /// <c>float</c>
            /// [t0, t1] that represent the limiting values of a parametric
            /// variable t, that becomes an input of color function(s).
            /// </summary>
            /// <returns>
            /// 
            /// <c>float[]</c>
            /// of Domain object ([0.0 1.0] by default)
            /// </returns>
            public virtual float[] GetDomain() {
                PdfArray domain = GetPdfObject().GetAsArray(PdfName.Domain);
                if (domain == null) {
                    return new float[] { 0, 1 };
                }
                return new float[] { domain.GetAsNumber(0).FloatValue(), domain.GetAsNumber(1).FloatValue() };
            }

            /// <summary>
            /// Sets the Domain with the array of two
            /// <c>float</c>
            /// [t0, t1] that represent the limiting values
            /// of a parametric variable t, that becomes an input of color function(s).
            /// </summary>
            /// <param name="t0">first limit of variable t</param>
            /// <param name="t1">second limit of variable t</param>
            public virtual void SetDomain(float t0, float t1) {
                GetPdfObject().Put(PdfName.Domain, new PdfArray(new float[] { t0, t1 }));
                SetModified();
            }

            /// <summary>
            /// Gets the array of two
            /// <c>boolean</c>
            /// that specified whether to extend the shading
            /// beyond the starting and ending circles of the axis, respectively.
            /// </summary>
            /// <returns>
            /// 
            /// <c>boolean[]</c>
            /// of Extended object ([false false] by default)
            /// </returns>
            public virtual bool[] GetExtend() {
                PdfArray extend = GetPdfObject().GetAsArray(PdfName.Extend);
                if (extend == null) {
                    return new bool[] { false, false };
                }
                return new bool[] { extend.GetAsBoolean(0).GetValue(), extend.GetAsBoolean(1).GetValue() };
            }

            /// <summary>
            /// Sets the Extend object with the two
            /// <c>boolean</c>
            /// value.
            /// </summary>
            /// <param name="extendStart">if true will extend shading beyond the starting circle of Coords.</param>
            /// <param name="extendEnd">if true will extend shading beyond the ending circle of Coords.</param>
            public virtual void SetExtend(bool extendStart, bool extendEnd) {
                GetPdfObject().Put(PdfName.Extend, new PdfArray(new bool[] { extendStart, extendEnd }));
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// free-form Gouraud-shaded triangle mesh type.
        /// The area to be shaded is defined by a path composed entirely of triangles.
        /// The colour at each vertex of the triangles is specified,
        /// and a technique known as Gouraud interpolation is used to colour the interiors.
        /// </summary>
        public class FreeFormGouraudShadedTriangleMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// </summary>
            /// <param name="pdfStream">
            /// -
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// from which the instance is created.
            /// </param>
            [System.ObsoleteAttribute(@"Intended only for private use. You should use PdfShading.MakeShading(iText.Kernel.Pdf.PdfDictionary) instead."
                )]
            public FreeFormGouraudShadedTriangleMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            public FreeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int
                 bitsPerFlag, float[] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
            }

            public FreeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int
                 bitsPerFlag, PdfArray decode)
                : base(new PdfStream(), PdfShading.ShadingType.FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH, cs.GetPdfObject()) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetBitsPerFlag(bitsPerFlag);
                SetDecode(decode);
            }

            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            public virtual int GetBitsPerFlag() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
            }

            public virtual void SetBitsPerFlag(int bitsPerFlag) {
                GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
                SetModified();
            }

            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            public virtual void SetDecode(float[] decode) {
                GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
            }

            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
            }
        }

        public class LatticeFormGouraudShadedTriangleMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// </summary>
            /// <param name="pdfStream">
            /// -
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// from which the instance is created.
            /// </param>
            [System.ObsoleteAttribute(@"Intended only for private use. You should use PdfShading.MakeShading(iText.Kernel.Pdf.PdfDictionary) instead."
                )]
            public LatticeFormGouraudShadedTriangleMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            public LatticeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, 
                int verticesPerRow, float[] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, verticesPerRow, new PdfArray(decode)) {
            }

            public LatticeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, 
                int verticesPerRow, PdfArray decode)
                : base(new PdfStream(), PdfShading.ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH, cs.GetPdfObject(
                    )) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetVerticesPerRow(verticesPerRow);
                SetDecode(decode);
            }

            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            public virtual int GetVerticesPerRow() {
                return (int)GetPdfObject().GetAsInt(PdfName.VerticesPerRow);
            }

            public virtual void SetVerticesPerRow(int verticesPerRow) {
                GetPdfObject().Put(PdfName.VerticesPerRow, new PdfNumber(verticesPerRow));
                SetModified();
            }

            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            public virtual void SetDecode(float[] decode) {
                GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
            }

            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
            }
        }

        public class CoonsPatchMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// </summary>
            /// <param name="pdfStream">
            /// -
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// from which the instance is created.
            /// </param>
            [System.ObsoleteAttribute(@"Intended only for private use. You should use PdfShading.MakeShading(iText.Kernel.Pdf.PdfDictionary) instead."
                )]
            public CoonsPatchMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            public CoonsPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag, float
                [] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
            }

            public CoonsPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag, PdfArray
                 decode)
                : base(new PdfStream(), PdfShading.ShadingType.COONS_PATCH_MESH, cs.GetPdfObject()) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetBitsPerFlag(bitsPerFlag);
                SetDecode(decode);
            }

            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            public virtual int GetBitsPerFlag() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
            }

            public virtual void SetBitsPerFlag(int bitsPerFlag) {
                GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
                SetModified();
            }

            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            public virtual void SetDecode(float[] decode) {
                GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
            }

            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
            }
        }

        public class TensorProductPatchMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// </summary>
            /// <param name="pdfStream">
            /// -
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// from which the instance is created.
            /// </param>
            [System.ObsoleteAttribute(@"Intended only for private use. You should use PdfShading.MakeShading(iText.Kernel.Pdf.PdfDictionary) instead."
                )]
            public TensorProductPatchMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            public TensorProductPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag
                , float[] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
            }

            public TensorProductPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag
                , PdfArray decode)
                : base(new PdfStream(), PdfShading.ShadingType.TENSOR_PRODUCT_PATCH_MESH, cs.GetPdfObject()) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetBitsPerFlag(bitsPerFlag);
                SetDecode(decode);
            }

            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            public virtual int GetBitsPerFlag() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
            }

            public virtual void SetBitsPerFlag(int bitsPerFlag) {
                GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
                SetModified();
            }

            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            public virtual void SetDecode(float[] decode) {
                GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
            }

            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
            }
        }
    }
}
