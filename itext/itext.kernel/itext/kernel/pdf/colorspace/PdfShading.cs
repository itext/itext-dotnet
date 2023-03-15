/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Colorspace {
    /// <summary>The abstract PdfShading class that represents the Shading Dictionary PDF object.</summary>
    public abstract class PdfShading : PdfObjectWrapper<PdfDictionary> {
        /// <summary>constants of shading type (see ISO-320001 Table 78)</summary>
        internal sealed class ShadingType {
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

            private ShadingType() {
            }
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
                throw new PdfException(KernelExceptionMessageConstant.SHADING_TYPE_NOT_FOUND);
            }
            if (!shadingDictionary.ContainsKey(PdfName.ColorSpace)) {
                throw new PdfException(KernelExceptionMessageConstant.COLOR_SPACE_NOT_FOUND);
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
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfShading.FreeFormGouraudShadedTriangleMesh((PdfStream)shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfShading.LatticeFormGouraudShadedTriangleMesh((PdfStream)shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.COONS_PATCH_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfShading.CoonsPatchMesh((PdfStream)shadingDictionary);
                    break;
                }

                case PdfShading.ShadingType.TENSOR_PRODUCT_PATCH_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfShading.TensorProductPatchMesh((PdfStream)shadingDictionary);
                    break;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                }
            }
            return shading;
        }

        /// <summary>
        /// Creates the
        /// <see cref="PdfShading"/>
        /// object from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <param name="pdfObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from which the
        /// <see cref="PdfShading"/>
        /// object will be created.
        /// </param>
        protected internal PdfShading(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Creates the
        /// <see cref="PdfShading"/>
        /// object from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// ,
        /// using provided type and colorspace.
        /// </summary>
        /// <param name="pdfObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from which the
        /// <see cref="PdfShading"/>
        /// object will be created.
        /// </param>
        /// <param name="type">
        /// type with which this
        /// <see cref="PdfShading"/>
        /// object will be created.
        /// </param>
        /// <param name="colorSpace">
        /// 
        /// <see cref="PdfColorSpace"/>
        /// with which this
        /// <see cref="PdfShading"/>
        /// object will be created.
        /// </param>
        protected internal PdfShading(PdfDictionary pdfObject, int type, PdfColorSpace colorSpace)
            : base(pdfObject) {
            GetPdfObject().Put(PdfName.ShadingType, new PdfNumber(type));
            if (colorSpace is PdfSpecialCs.Pattern) {
                throw new ArgumentException("colorSpace");
            }
            GetPdfObject().Put(PdfName.ColorSpace, colorSpace.GetPdfObject());
        }

        /// <summary>Gets the shading type.</summary>
        /// <returns>
        /// int value of
        /// <see cref="iText.Kernel.Pdf.PdfName.ShadingType"/>.
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
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// to set.
        /// </param>
        public virtual void SetFunction(IPdfFunction function) {
            GetPdfObject().Put(PdfName.Function, function.GetAsPdfObject());
            SetModified();
        }

        /// <summary>
        /// Sets the function object that represents color transitions
        /// across the shading geometry as an array of functions.
        /// </summary>
        /// <param name="functions">
        /// The array of
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// to be set.
        /// </param>
        public virtual void SetFunction(IPdfFunction[] functions) {
            PdfArray arr = new PdfArray();
            foreach (IPdfFunction func in functions) {
                arr.Add(func.GetAsPdfObject());
            }
            GetPdfObject().Put(PdfName.Function, arr);
            SetModified();
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        /// <summary><inheritDoc/></summary>
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
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
            /// </summary>
            /// <param name="pdfDictionary">
            /// from which this
            /// <see cref="FunctionBased"/>
            /// will be created
            /// </param>
            protected internal FunctionBased(PdfDictionary pdfDictionary)
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
            /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
            /// , that is used to calculate color transitions.
            /// </param>
            public FunctionBased(PdfColorSpace colorSpace, IPdfFunction function)
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
            /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
            /// , that is used to calculate color transitions.
            /// </param>
            public FunctionBased(PdfObject colorSpace, IPdfFunction function)
                : base(new PdfDictionary(), PdfShading.ShadingType.FUNCTION_BASED, PdfColorSpace.MakeColorSpace(colorSpace
                    )) {
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
                SetDomain(new PdfArray(new float[] { xmin, xmax, ymin, ymax }));
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
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of floats that represents the transformation matrix that maps the domain rectangle
            /// into a corresponding figure in the target coordinate space.
            /// </summary>
            /// <returns>
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of transformation matrix (identical matrix by default).
            /// </returns>
            public virtual PdfArray GetMatrix() {
                PdfArray matrix = GetPdfObject().GetAsArray(PdfName.Matrix);
                if (matrix == null) {
                    matrix = new PdfArray(new float[] { 1, 0, 0, 1, 0, 0 });
                    SetMatrix(matrix);
                }
                return matrix;
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
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
            /// </summary>
            /// <param name="pdfDictionary">
            /// from which this
            /// <see cref="Axial"/>
            /// will be created
            /// </param>
            protected internal Axial(PdfDictionary pdfDictionary)
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
                : base(new PdfDictionary(), PdfShading.ShadingType.AXIAL, cs) {
                SetCoords(x0, y0, x1, y1);
                IPdfFunction func = new PdfType2Function(new float[] { 0, 1 }, null, color0, color1, 1);
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
            /// of four numbers [x0 y0 x1 y1] that specified the starting
            /// and the endings coordinates of thew axis, expressed in the shading's target coordinate space.
            /// </param>
            /// <param name="function">
            /// the
            /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
            /// object, that is used to calculate color transitions.
            /// </param>
            public Axial(PdfColorSpace cs, PdfArray coords, IPdfFunction function)
                : this(cs, coords, null, function) {
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
            /// of four numbers [x0 y0 x1 y1] that specified
            /// the starting and the endings coordinates of thew axis, expressed
            /// in the shading's target coordinate space.
            /// </param>
            /// <param name="domain">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of two numbers [t0 t1] specifying the limiting values
            /// of a parametric variable t which is considered to vary linearly between
            /// these two values and becomes the input argument to the colour function.
            /// </param>
            /// <param name="function">
            /// the
            /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
            /// object, that is used to calculate color transitions.
            /// </param>
            public Axial(PdfColorSpace cs, PdfArray coords, PdfArray domain, IPdfFunction function)
                : base(new PdfDictionary(), PdfShading.ShadingType.AXIAL, cs) {
                SetCoords(coords);
                if (domain != null) {
                    SetDomain(domain);
                }
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
            public virtual void SetDomain(float t0, float t1) {
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
            public virtual void SetDomain(PdfArray domain) {
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
            public virtual void SetExtend(bool extendStart, bool extendEnd) {
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
            public virtual void SetExtend(PdfArray extend) {
                GetPdfObject().Put(PdfName.Extend, extend);
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with radial type,
        /// that define a colour blend that varies between two circles.
        /// </summary>
        /// <remarks>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with radial type,
        /// that define a colour blend that varies between two circles.
        /// This type of shading shall not be used with an Indexed colour space
        /// </remarks>
        public class Radial : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
            /// </summary>
            /// <param name="pdfDictionary">
            /// from which this
            /// <see cref="Radial"/>
            /// will be created
            /// </param>
            protected internal Radial(PdfDictionary pdfDictionary)
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
                : base(new PdfDictionary(), PdfShading.ShadingType.RADIAL, cs) {
                SetCoords(x0, y0, r0, x1, y1, r1);
                IPdfFunction func = new PdfType2Function(new float[] { 0, 1 }, null, color0, color1, 1);
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
            /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
            /// object, that is used to calculate color transitions.
            /// </param>
            public Radial(PdfColorSpace cs, PdfArray coords, IPdfFunction function)
                : base(new PdfDictionary(), PdfShading.ShadingType.RADIAL, cs) {
                SetCoords(coords);
                SetFunction(function);
            }

            /// <summary>
            /// Gets the coords
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// object - an array of six numbers [x0 y0 r0 x1 y1 r1],
            /// specifying the centres and radii of the starting and ending circles,
            /// expressed in the shading’s target coordinate space.
            /// </summary>
            /// <remarks>
            /// Gets the coords
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// object - an array of six numbers [x0 y0 r0 x1 y1 r1],
            /// specifying the centres and radii of the starting and ending circles,
            /// expressed in the shading’s target coordinate space.
            /// The radii r0 and r1 shall both be greater than or equal to 0.
            /// If one radius is 0, the corresponding circle shall be treated as a point;
            /// if both are 0, nothing shall be painted.
            /// </remarks>
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
            /// </summary>
            /// <remarks>
            /// Sets the coords
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// object - an array of six numbers [x0 y0 r0 x1 y1 r1],
            /// specifying the centres and radii of the starting and ending circles,
            /// expressed in the shading’s target coordinate space.
            /// The radii r0 and r1 shall both be greater than or equal to 0.
            /// If one radius is 0, the corresponding circle shall be treated as a point;
            /// if both are 0, nothing shall be painted.
            /// </remarks>
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
            public virtual void SetDomain(float t0, float t1) {
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
            public virtual void SetDomain(PdfArray domain) {
                GetPdfObject().Put(PdfName.Domain, domain);
                SetModified();
            }

            /// <summary>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of two
            /// <c>boolean</c>
            /// that specified whether to extend the shading
            /// beyond the starting and ending circles of the axis, respectively.
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
            /// <param name="extendStart">if true will extend shading beyond the starting circle of Coords.</param>
            /// <param name="extendEnd">if true will extend shading beyond the ending circle of Coords.</param>
            public virtual void SetExtend(bool extendStart, bool extendEnd) {
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
            /// If first is true shading will extend beyond the starting circle of Coords.
            /// If second is true shading will extend beyond the ending circle of Coords.
            /// </remarks>
            /// <param name="extend">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// representing Extend object
            /// </param>
            public virtual void SetExtend(PdfArray extend) {
                GetPdfObject().Put(PdfName.Extend, extend);
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// free-form Gouraud-shaded triangle mesh type.
        /// </summary>
        /// <remarks>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// free-form Gouraud-shaded triangle mesh type.
        /// The area to be shaded is defined by a path composed entirely of triangles.
        /// The colour at each vertex of the triangles is specified,
        /// and a technique known as Gouraud interpolation is used to colour the interiors.
        /// The object shall be represented as stream containing a sequence of vertex data.
        /// Each vertex is specified by the following values, in the order shown:
        /// f x y c1 ... cn where:
        /// f -  the vertex's edge flag, that determines the vertex is connected to other vertices of the triangle mesh.
        /// For full description, see ISO-320001 Paragraph 8.7.4.5.5
        /// x, y - vertex's horizontal and vertical coordinates, expressed in the shading's target coordinate space.
        /// c1...cn - vertex's colour components.
        /// If the shading dictionary includes a Function entry, only a single parametric value, t,
        /// shall be specified for each vertex in place of the colour components c1...cn.
        /// </remarks>
        public class FreeFormGouraudShadedTriangleMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
            /// </summary>
            /// <param name="pdfStream">
            /// from which this
            /// <see cref="FreeFormGouraudShadedTriangleMesh"/>
            /// will be created
            /// </param>
            protected internal FreeFormGouraudShadedTriangleMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="bitsPerFlag">
            /// the number of bits used to represent the edge flag for each vertex.
            /// The value of BitsPerFlag shall be 2, 4, or 8,
            /// but only the least significant 2 bits in each flag value shall be used.
            /// The value for the edge flag shall be 0, 1, or 2.
            /// </param>
            /// <param name="decode">
            /// the
            /// <c>int[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public FreeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int
                 bitsPerFlag, float[] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="bitsPerFlag">
            /// the number of bits used to represent the edge flag for each vertex.
            /// The value of BitsPerFlag shall be 2, 4, or 8,
            /// but only the least significant 2 bits in each flag value shall be used.
            /// The value for the edge flag shall be 0, 1, or 2.
            /// </param>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public FreeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int
                 bitsPerFlag, PdfArray decode)
                : base(new PdfStream(), PdfShading.ShadingType.FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH, cs) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetBitsPerFlag(bitsPerFlag);
                SetDecode(decode);
            }

            /// <summary>Gets the number of bits used to represent each vertex coordinate.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, 16, 24, or 32.</returns>
            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            /// <summary>Sets the number of bits used to represent each vertex coordinate.</summary>
            /// <param name="bitsPerCoordinate">the number of bits to be set. Shall be 1, 2, 4, 8, 12, 16, 24, or 32.</param>
            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            /// <summary>Gets the number of bits used to represent each colour component.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, or 16.</returns>
            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            /// <summary>Sets the number of bits used to represent each colour component.</summary>
            /// <param name="bitsPerComponent">the number of bits to be set. Shall be 1, 2, 4, 8, 12, or 16.</param>
            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            /// <summary>Gets the number of bits used to represent the edge flag for each vertex.</summary>
            /// <remarks>
            /// Gets the number of bits used to represent the edge flag for each vertex.
            /// But only the least significant 2 bits in each flag value shall be used.
            /// The valid flag values are 0, 1 or 2.
            /// </remarks>
            /// <returns>the number of bits. Can be 2, 4 or 8.</returns>
            public virtual int GetBitsPerFlag() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
            }

            /// <summary>Sets the number of bits used to represent the edge flag for each vertex.</summary>
            /// <remarks>
            /// Sets the number of bits used to represent the edge flag for each vertex.
            /// But only the least significant 2 bits in each flag value shall be used.
            /// The valid flag values are 0, 1 or 2.
            /// </remarks>
            /// <param name="bitsPerFlag">the number of bits to be set. Shall be 2, 4 or 8.</param>
            public virtual void SetBitsPerFlag(int bitsPerFlag) {
                GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
                SetModified();
            }

            /// <summary>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <returns>
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object.
            /// </returns>
            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            /// <summary>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <c>float[]</c>
            /// of Decode object to set.
            /// </param>
            public virtual void SetDecode(float[] decode) {
                SetDecode(new PdfArray(decode));
            }

            /// <summary>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object to set.
            /// </param>
            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// lattice-form Gouraud-shaded triangle mesh type.
        /// </summary>
        /// <remarks>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// lattice-form Gouraud-shaded triangle mesh type.
        /// This type is similar to
        /// <see cref="FreeFormGouraudShadedTriangleMesh"/>
        /// but instead of using free-form geometry,
        /// the vertices are arranged in a pseudorectangular lattice,
        /// which is topologically equivalent to a rectangular grid.
        /// The vertices are organized into rows, which need not be geometrically linear.
        /// The verticals data in stream is similar to
        /// <see cref="FreeFormGouraudShadedTriangleMesh"/>
        /// ,
        /// except there is no edge flag.
        /// </remarks>
        public class LatticeFormGouraudShadedTriangleMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
            /// </summary>
            /// <param name="pdfStream">
            /// from which this
            /// <see cref="LatticeFormGouraudShadedTriangleMesh"/>
            /// will be created
            /// </param>
            protected internal LatticeFormGouraudShadedTriangleMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="verticesPerRow">
            /// the number of vertices in each row of the lattice (shall be &gt; 1).
            /// The number of rows need not be specified.
            /// </param>
            /// <param name="decode">
            /// the
            /// <c>int[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public LatticeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, 
                int verticesPerRow, float[] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, verticesPerRow, new PdfArray(decode)) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="verticesPerRow">
            /// the number of vertices in each row of the lattice (shall be &gt; 1).
            /// The number of rows need not be specified.
            /// </param>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public LatticeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, 
                int verticesPerRow, PdfArray decode)
                : base(new PdfStream(), PdfShading.ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH, cs) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetVerticesPerRow(verticesPerRow);
                SetDecode(decode);
            }

            /// <summary>Gets the number of bits used to represent each vertex coordinate.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, 16, 24, or 32.</returns>
            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            /// <summary>Sets the number of bits used to represent each vertex coordinate.</summary>
            /// <param name="bitsPerCoordinate">the number of bits to be set. Shall be 1, 2, 4, 8, 12, 16, 24, or 32.</param>
            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            /// <summary>Gets the number of bits used to represent each colour component.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, or 16.</returns>
            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            /// <summary>Sets the number of bits used to represent each colour component.</summary>
            /// <param name="bitsPerComponent">the number of bits to be set. Shall be 1, 2, 4, 8, 12, or 16.</param>
            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            /// <summary>Gets the number of vertices in each row of the lattice.</summary>
            /// <returns>the number of vertices. Can only be greater than 1.</returns>
            public virtual int GetVerticesPerRow() {
                return (int)GetPdfObject().GetAsInt(PdfName.VerticesPerRow);
            }

            /// <summary>Sets the number of vertices in each row of the lattice.</summary>
            /// <remarks>
            /// Sets the number of vertices in each row of the lattice.
            /// The number of rows need not be specified.
            /// </remarks>
            /// <param name="verticesPerRow">the number of vertices to be set. Shall be greater than 1.</param>
            public virtual void SetVerticesPerRow(int verticesPerRow) {
                GetPdfObject().Put(PdfName.VerticesPerRow, new PdfNumber(verticesPerRow));
                SetModified();
            }

            /// <summary>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <returns>
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object.
            /// </returns>
            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            /// <summary>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <c>float[]</c>
            /// of Decode object to set.
            /// </param>
            public virtual void SetDecode(float[] decode) {
                SetDecode(new PdfArray(decode));
            }

            /// <summary>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object to set.
            /// </param>
            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// Coons Patch mesh type.
        /// </summary>
        /// <remarks>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// Coons Patch mesh type.
        /// This type of shading is constructed from one or more colour patches, each bounded by four cubic Bézier curves.
        /// Degenerate Bézier curves are allowed and are useful for certain graphical effects.
        /// At least one complete patch shall be specified.
        /// The shape of patch is defined by 12 control points.
        /// Colours are specified for each corner of the unit square,
        /// and bilinear interpolation is used to fill in colours over the entire unit square.
        /// Coordinates are mapped from the unit square into a four-sided patch whose sides are not necessarily linear.
        /// The mapping is continuous: the corners of the unit square map to corners of the patch
        /// and the sides of the unit square map to sides of the patch.
        /// For the format of data stream, that defines patches (see ISO-320001 Table 85).
        /// If the shading dictionary contains a Function entry, the colour data for each corner of a patch
        /// shall be specified by a single parametric value t rather than by n separate colour components c1...cn.
        /// </remarks>
        public class CoonsPatchMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
            /// </summary>
            /// <param name="pdfStream">
            /// from which this
            /// <see cref="CoonsPatchMesh"/>
            /// will be created
            /// </param>
            protected internal CoonsPatchMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="bitsPerFlag">
            /// the number of bits used to represent the edge flag for each vertex.
            /// The value of BitsPerFlag shall be 2, 4, or 8,
            /// but only the least significant 2 bits in each flag value shall be used.
            /// The value for the edge flag shall be 0, 1, 2 or 3.
            /// </param>
            /// <param name="decode">
            /// the
            /// <c>int[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public CoonsPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag, float
                [] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="bitsPerFlag">
            /// the number of bits used to represent the edge flag for each vertex.
            /// The value of BitsPerFlag shall be 2, 4, or 8,
            /// but only the least significant 2 bits in each flag value shall be used.
            /// The value for the edge flag shall be 0, 1, 2 or 3.
            /// </param>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public CoonsPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag, PdfArray
                 decode)
                : base(new PdfStream(), PdfShading.ShadingType.COONS_PATCH_MESH, cs) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetBitsPerFlag(bitsPerFlag);
                SetDecode(decode);
            }

            /// <summary>Gets the number of bits used to represent each vertex coordinate.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, 16, 24, or 32.</returns>
            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            /// <summary>Sets the number of bits used to represent each vertex coordinate.</summary>
            /// <param name="bitsPerCoordinate">the number of bits to be set. Shall be 1, 2, 4, 8, 12, 16, 24, or 32.</param>
            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            /// <summary>Gets the number of bits used to represent each colour component.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, or 16.</returns>
            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            /// <summary>Sets the number of bits used to represent each colour component.</summary>
            /// <param name="bitsPerComponent">the number of bits to be set. Shall be 1, 2, 4, 8, 12, or 16.</param>
            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            /// <summary>Gets the number of bits used to represent the edge flag for each vertex.</summary>
            /// <remarks>
            /// Gets the number of bits used to represent the edge flag for each vertex.
            /// But only the least significant 2 bits in each flag value shall be used.
            /// The valid flag values are 0, 1, 2 or 3.
            /// </remarks>
            /// <returns>the number of bits. Can be 2, 4 or 8.</returns>
            public virtual int GetBitsPerFlag() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
            }

            /// <summary>Sets the number of bits used to represent the edge flag for each vertex.</summary>
            /// <remarks>
            /// Sets the number of bits used to represent the edge flag for each vertex.
            /// But only the least significant 2 bits in each flag value shall be used.
            /// The valid flag values are 0, 1, 2 or 3.
            /// </remarks>
            /// <param name="bitsPerFlag">the number of bits to be set. Shall be 2, 4 or 8.</param>
            public virtual void SetBitsPerFlag(int bitsPerFlag) {
                GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
                SetModified();
            }

            /// <summary>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <returns>
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object.
            /// </returns>
            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            /// <summary>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <c>float[]</c>
            /// of Decode object to set.
            /// </param>
            public virtual void SetDecode(float[] decode) {
                SetDecode(new PdfArray(decode));
            }

            /// <summary>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object to set.
            /// </param>
            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
                SetModified();
            }
        }

        /// <summary>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// Tensor-Product Patch mesh type.
        /// </summary>
        /// <remarks>
        /// The class that extends
        /// <see cref="PdfShading"/>
        /// class and is in charge of Shading Dictionary with
        /// Tensor-Product Patch mesh type.
        /// This type of shading is identical to
        /// <see cref="CoonsPatchMesh"/>
        /// , except that it's based on a
        /// bicubic tensor-product patch defined by 16 control points.
        /// For the format of data stream, that defines patches, see ISO-320001 Table 86.
        /// </remarks>
        public class TensorProductPatchMesh : PdfShading {
            /// <summary>
            /// Creates the new instance of the class from the existing
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
            /// </summary>
            /// <param name="pdfStream">
            /// from which this
            /// <see cref="TensorProductPatchMesh"/>
            /// will be created
            /// </param>
            protected internal TensorProductPatchMesh(PdfStream pdfStream)
                : base(pdfStream) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="bitsPerFlag">
            /// the number of bits used to represent the edge flag for each vertex.
            /// The value of BitsPerFlag shall be 2, 4, or 8,
            /// but only the least significant 2 bits in each flag value shall be used.
            /// The value for the edge flag shall be 0, 1, 2 or 3.
            /// </param>
            /// <param name="decode">
            /// the
            /// <c>int[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public TensorProductPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag
                , float[] decode)
                : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
            }

            /// <summary>Creates the new instance of the class.</summary>
            /// <param name="cs">
            /// the
            /// <see cref="PdfColorSpace"/>
            /// object in which colour values shall be expressed.
            /// The special Pattern space isn't excepted.
            /// </param>
            /// <param name="bitsPerCoordinate">
            /// the number of bits used to represent each vertex coordinate.
            /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32.
            /// </param>
            /// <param name="bitsPerComponent">
            /// the number of bits used to represent each colour component.
            /// The value shall be 1, 2, 4, 8, 12, or 16.
            /// </param>
            /// <param name="bitsPerFlag">
            /// the number of bits used to represent the edge flag for each vertex.
            /// The value of BitsPerFlag shall be 2, 4, or 8,
            /// but only the least significant 2 bits in each flag value shall be used.
            /// The value for the edge flag shall be 0, 1, 2 or 3.
            /// </param>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </param>
            public TensorProductPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag
                , PdfArray decode)
                : base(new PdfStream(), PdfShading.ShadingType.TENSOR_PRODUCT_PATCH_MESH, cs) {
                SetBitsPerCoordinate(bitsPerCoordinate);
                SetBitsPerComponent(bitsPerComponent);
                SetBitsPerFlag(bitsPerFlag);
                SetDecode(decode);
            }

            /// <summary>Gets the number of bits used to represent each vertex coordinate.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, 16, 24, or 32.</returns>
            public virtual int GetBitsPerCoordinate() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
            }

            /// <summary>Sets the number of bits used to represent each vertex coordinate.</summary>
            /// <param name="bitsPerCoordinate">the number of bits to be set. Shall be 1, 2, 4, 8, 12, 16, 24, or 32.</param>
            public virtual void SetBitsPerCoordinate(int bitsPerCoordinate) {
                GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
                SetModified();
            }

            /// <summary>Gets the number of bits used to represent each colour component.</summary>
            /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, or 16.</returns>
            public virtual int GetBitsPerComponent() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
            }

            /// <summary>Sets the number of bits used to represent each colour component.</summary>
            /// <param name="bitsPerComponent">the number of bits to be set. Shall be 1, 2, 4, 8, 12, or 16.</param>
            public virtual void SetBitsPerComponent(int bitsPerComponent) {
                GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
                SetModified();
            }

            /// <summary>Gets the number of bits used to represent the edge flag for each vertex.</summary>
            /// <remarks>
            /// Gets the number of bits used to represent the edge flag for each vertex.
            /// But only the least significant 2 bits in each flag value shall be used.
            /// The valid flag values are 0, 1, 2 or 3.
            /// </remarks>
            /// <returns>the number of bits. Can be 2, 4 or 8.</returns>
            public virtual int GetBitsPerFlag() {
                return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
            }

            /// <summary>Sets the number of bits used to represent the edge flag for each vertex.</summary>
            /// <remarks>
            /// Sets the number of bits used to represent the edge flag for each vertex.
            /// But only the least significant 2 bits in each flag value shall be used.
            /// The valid flag values are 0, 1, 2 or 3.
            /// </remarks>
            /// <param name="bitsPerFlag">the number of bits to be set. Shall be 2, 4 or 8.</param>
            public virtual void SetBitsPerFlag(int bitsPerFlag) {
                GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
                SetModified();
            }

            /// <summary>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Gets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <returns>
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object.
            /// </returns>
            public virtual PdfArray GetDecode() {
                return GetPdfObject().GetAsArray(PdfName.Decode);
            }

            /// <summary>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <c>float[]</c>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <c>float[]</c>
            /// of Decode object to set.
            /// </param>
            public virtual void SetDecode(float[] decode) {
                SetDecode(new PdfArray(decode));
            }

            /// <summary>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values.
            /// </summary>
            /// <remarks>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// of numbers specifying how to map vertex coordinates and colour components
            /// into the appropriate ranges of values. The ranges shall be specified as follows:
            /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
            /// Only one pair of color values shall be specified if a Function entry is present.
            /// </remarks>
            /// <param name="decode">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfArray"/>
            /// Decode object to set.
            /// </param>
            public virtual void SetDecode(PdfArray decode) {
                GetPdfObject().Put(PdfName.Decode, decode);
                SetModified();
            }
        }
    }
}
