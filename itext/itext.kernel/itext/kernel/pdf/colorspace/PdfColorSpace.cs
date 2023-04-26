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
