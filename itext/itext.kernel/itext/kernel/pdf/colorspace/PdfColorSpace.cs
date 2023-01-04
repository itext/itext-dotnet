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
using iText.Commons.Utils;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Colorspace {
    /// <summary>Represents the most common properties of color spaces.</summary>
    public abstract class PdfColorSpace : PdfObjectWrapper<PdfObject> {
        public static readonly ICollection<PdfName> DIRECT_COLOR_SPACES = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.DeviceGray, PdfName.DeviceRGB, PdfName.DeviceCMYK, PdfName
            .Pattern)));

        protected internal PdfColorSpace(PdfObject pdfObject)
            : base(pdfObject) {
        }

        public abstract int GetNumberOfComponents();

        public static iText.Kernel.Pdf.Colorspace.PdfColorSpace MakeColorSpace(PdfObject pdfObject) {
            if (pdfObject.IsIndirectReference()) {
                pdfObject = ((PdfIndirectReference)pdfObject).GetRefersTo();
            }
            if (pdfObject.IsArray() && ((PdfArray)pdfObject).Size() == 1) {
                pdfObject = ((PdfArray)pdfObject).Get(0);
            }
            if (PdfName.DeviceGray.Equals(pdfObject)) {
                return new PdfDeviceCs.Gray();
            }
            else {
                if (PdfName.DeviceRGB.Equals(pdfObject)) {
                    return new PdfDeviceCs.Rgb();
                }
                else {
                    if (PdfName.DeviceCMYK.Equals(pdfObject)) {
                        return new PdfDeviceCs.Cmyk();
                    }
                    else {
                        if (PdfName.Pattern.Equals(pdfObject)) {
                            return new PdfSpecialCs.Pattern();
                        }
                        else {
                            if (pdfObject.IsArray()) {
                                PdfArray array = (PdfArray)pdfObject;
                                PdfName csType = array.GetAsName(0);
                                if (PdfName.CalGray.Equals(csType)) {
                                    return new PdfCieBasedCs.CalGray(array);
                                }
                                else {
                                    if (PdfName.CalRGB.Equals(csType)) {
                                        return new PdfCieBasedCs.CalRgb(array);
                                    }
                                    else {
                                        if (PdfName.Lab.Equals(csType)) {
                                            return new PdfCieBasedCs.Lab(array);
                                        }
                                        else {
                                            if (PdfName.ICCBased.Equals(csType)) {
                                                return new PdfCieBasedCs.IccBased(array);
                                            }
                                            else {
                                                if (PdfName.Indexed.Equals(csType)) {
                                                    return new PdfSpecialCs.Indexed(array);
                                                }
                                                else {
                                                    if (PdfName.Separation.Equals(csType)) {
                                                        return new PdfSpecialCs.Separation(array);
                                                    }
                                                    else {
                                                        if (PdfName.DeviceN.Equals(csType)) {
                                                            //TODO DEVSIX-4205 Fix colorspace creation
                                                            return array.Size() == 4 ? new PdfSpecialCs.DeviceN(array) : new PdfSpecialCs.NChannel(array);
                                                        }
                                                        else {
                                                            if (PdfName.Pattern.Equals(csType)) {
                                                                return new PdfSpecialCs.UncoloredTilingPattern(array);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
