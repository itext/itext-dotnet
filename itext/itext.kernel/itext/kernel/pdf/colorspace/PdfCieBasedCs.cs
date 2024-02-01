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
using System.Collections.Generic;
using System.IO;
using iText.IO.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Colorspace {
    public abstract class PdfCieBasedCs : PdfColorSpace {
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

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        protected internal PdfCieBasedCs(PdfArray pdfObject)
            : base(pdfObject) {
        }

        public class CalGray : PdfCieBasedCs {
            public CalGray(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public CalGray(float[] whitePoint)
                : this(GetInitialPdfArray()) {
                if (whitePoint == null || whitePoint.Length != 3) {
                    throw new PdfException(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, this);
                }
                PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
                d.Put(PdfName.WhitePoint, new PdfArray(whitePoint));
            }

            public CalGray(float[] whitePoint, float[] blackPoint, float gamma)
                : this(whitePoint) {
                PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
                if (blackPoint != null) {
                    d.Put(PdfName.BlackPoint, new PdfArray(blackPoint));
                }
                if (gamma != float.NaN) {
                    d.Put(PdfName.Gamma, new PdfNumber(gamma));
                }
            }

            public override int GetNumberOfComponents() {
                return 1;
            }

            private static PdfArray GetInitialPdfArray() {
                List<PdfObject> tempArray = new List<PdfObject>(2);
                tempArray.Add(PdfName.CalGray);
                tempArray.Add(new PdfDictionary());
                return new PdfArray(tempArray);
            }
        }

        public class CalRgb : PdfCieBasedCs {
            public CalRgb(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public CalRgb(float[] whitePoint)
                : this(GetInitialPdfArray()) {
                if (whitePoint == null || whitePoint.Length != 3) {
                    throw new PdfException(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, this);
                }
                PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
                d.Put(PdfName.WhitePoint, new PdfArray(whitePoint));
            }

            public CalRgb(float[] whitePoint, float[] blackPoint, float[] gamma, float[] matrix)
                : this(whitePoint) {
                PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
                if (blackPoint != null) {
                    d.Put(PdfName.BlackPoint, new PdfArray(blackPoint));
                }
                if (gamma != null) {
                    d.Put(PdfName.Gamma, new PdfArray(gamma));
                }
                if (matrix != null) {
                    d.Put(PdfName.Matrix, new PdfArray(matrix));
                }
            }

            public override int GetNumberOfComponents() {
                return 3;
            }

            private static PdfArray GetInitialPdfArray() {
                List<PdfObject> tempArray = new List<PdfObject>(2);
                tempArray.Add(PdfName.CalRGB);
                tempArray.Add(new PdfDictionary());
                return new PdfArray(tempArray);
            }
        }

        public class Lab : PdfCieBasedCs {
            public Lab(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public Lab(float[] whitePoint)
                : this(GetInitialPdfArray()) {
                if (whitePoint == null || whitePoint.Length != 3) {
                    throw new PdfException(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, this);
                }
                PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
                d.Put(PdfName.WhitePoint, new PdfArray(whitePoint));
            }

            public Lab(float[] whitePoint, float[] blackPoint, float[] range)
                : this(whitePoint) {
                PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
                if (blackPoint != null) {
                    d.Put(PdfName.BlackPoint, new PdfArray(blackPoint));
                }
                if (range != null) {
                    d.Put(PdfName.Range, new PdfArray(range));
                }
            }

            public override int GetNumberOfComponents() {
                return 3;
            }

            private static PdfArray GetInitialPdfArray() {
                List<PdfObject> tempArray = new List<PdfObject>(2);
                tempArray.Add(PdfName.Lab);
                tempArray.Add(new PdfDictionary());
                return new PdfArray(tempArray);
            }
        }

        public class IccBased : PdfCieBasedCs {
            public IccBased(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public IccBased(Stream iccStream)
                // TODO DEVSIX-4217 add parsing of the Range
                : this(GetInitialPdfArray(iccStream, null)) {
            }

            public IccBased(Stream iccStream, float[] range)
                : this(GetInitialPdfArray(iccStream, range)) {
            }

            public override int GetNumberOfComponents() {
                return (int)((PdfArray)GetPdfObject()).GetAsStream(1).GetAsInt(PdfName.N);
            }

            public static PdfStream GetIccProfileStream(Stream iccStream) {
                IccProfile iccProfile = IccProfile.GetInstance(iccStream);
                return GetIccProfileStream(iccProfile);
            }

            public static PdfStream GetIccProfileStream(Stream iccStream, float[] range) {
                IccProfile iccProfile = IccProfile.GetInstance(iccStream);
                return GetIccProfileStream(iccProfile, range);
            }

            public static PdfStream GetIccProfileStream(IccProfile iccProfile) {
                PdfStream stream = new PdfStream(iccProfile.GetData());
                stream.Put(PdfName.N, new PdfNumber(iccProfile.GetNumComponents()));
                switch (iccProfile.GetNumComponents()) {
                    case 1: {
                        stream.Put(PdfName.Alternate, PdfName.DeviceGray);
                        break;
                    }

                    case 3: {
                        stream.Put(PdfName.Alternate, PdfName.DeviceRGB);
                        break;
                    }

                    case 4: {
                        stream.Put(PdfName.Alternate, PdfName.DeviceCMYK);
                        break;
                    }

                    default: {
                        break;
                    }
                }
                return stream;
            }

            public static PdfStream GetIccProfileStream(IccProfile iccProfile, float[] range) {
                PdfStream stream = GetIccProfileStream(iccProfile);
                stream.Put(PdfName.Range, new PdfArray(range));
                return stream;
            }

            private static PdfArray GetInitialPdfArray(Stream iccStream, float[] range) {
                List<PdfObject> tempArray = new List<PdfObject>(2);
                tempArray.Add(PdfName.ICCBased);
                tempArray.Add(range == null ? GetIccProfileStream(iccStream) : GetIccProfileStream(iccStream, range));
                return new PdfArray(tempArray);
            }
        }
    }
}
