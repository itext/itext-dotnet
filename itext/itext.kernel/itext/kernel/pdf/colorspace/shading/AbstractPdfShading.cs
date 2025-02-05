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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>The PdfShading class that represents the Shading Dictionary PDF object.</summary>
    public abstract class AbstractPdfShading : PdfObjectWrapper<PdfDictionary> {
        /// <summary>
        /// Creates the
        /// <see cref="AbstractPdfShading"/>
        /// object from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// with corresponding type.
        /// </summary>
        /// <param name="shadingDictionary">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from which the
        /// <see cref="AbstractPdfShading"/>
        /// object will be created
        /// </param>
        /// <returns>
        /// Created
        /// <see cref="AbstractPdfShading"/>
        /// object
        /// </returns>
        public static iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading MakeShading(PdfDictionary shadingDictionary
            ) {
            if (!shadingDictionary.ContainsKey(PdfName.ShadingType)) {
                throw new PdfException(KernelExceptionMessageConstant.SHADING_TYPE_NOT_FOUND);
            }
            if (!shadingDictionary.ContainsKey(PdfName.ColorSpace)) {
                throw new PdfException(KernelExceptionMessageConstant.COLOR_SPACE_NOT_FOUND);
            }
            iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading shading;
            switch (shadingDictionary.GetAsNumber(PdfName.ShadingType).IntValue()) {
                case ShadingType.FUNCTION_BASED: {
                    shading = new PdfFunctionBasedShading(shadingDictionary);
                    break;
                }

                case ShadingType.AXIAL: {
                    shading = new PdfAxialShading(shadingDictionary);
                    break;
                }

                case ShadingType.RADIAL: {
                    shading = new PdfRadialShading(shadingDictionary);
                    break;
                }

                case ShadingType.FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfFreeFormGouraudShadedTriangleShading((PdfStream)shadingDictionary);
                    break;
                }

                case ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfLatticeFormGouraudShadedTriangleShading((PdfStream)shadingDictionary);
                    break;
                }

                case ShadingType.COONS_PATCH_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfCoonsPatchShading((PdfStream)shadingDictionary);
                    break;
                }

                case ShadingType.TENSOR_PRODUCT_PATCH_MESH: {
                    if (!shadingDictionary.IsStream()) {
                        throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE);
                    }
                    shading = new PdfTensorProductPatchShading((PdfStream)shadingDictionary);
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
        /// <see cref="AbstractPdfShading"/>
        /// object from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <param name="pdfObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from which the
        /// <see cref="AbstractPdfShading"/>
        /// object will be created
        /// </param>
        protected internal AbstractPdfShading(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Creates the
        /// <see cref="AbstractPdfShading"/>
        /// object from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// ,
        /// using provided type and colorspace.
        /// </summary>
        /// <param name="pdfObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from which the
        /// <see cref="AbstractPdfShading"/>
        /// object will be created
        /// </param>
        /// <param name="type">
        /// type with which this
        /// <see cref="AbstractPdfShading"/>
        /// object will be created
        /// </param>
        /// <param name="colorSpace">
        /// 
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// with which this
        /// <see cref="AbstractPdfShading"/>
        /// object will be created
        /// </param>
        protected internal AbstractPdfShading(PdfDictionary pdfObject, int type, PdfColorSpace colorSpace)
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
        /// <see cref="iText.Kernel.Pdf.PdfName.ShadingType"/>
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
        /// to set
        /// </param>
        public void SetFunction(IPdfFunction function) {
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
        /// to be set
        /// </param>
        public void SetFunction(IPdfFunction[] functions) {
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
        public sealed override void Flush() {
            base.Flush();
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
