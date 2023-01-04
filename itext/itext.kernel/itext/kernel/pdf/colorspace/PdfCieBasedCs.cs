/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
