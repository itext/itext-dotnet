/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    internal class InteriorColorUtil {
        private InteriorColorUtil() {
        }

        /// <summary>The interior color which is used to fill areas specific for different types of annotation.</summary>
        /// <remarks>
        /// The interior color which is used to fill areas specific for different types of annotation. For
        /// <see cref="PdfLineAnnotation"/>
        /// and polyline annotation (
        /// <see cref="PdfPolyGeomAnnotation"/>
        /// - the annotation's line endings, for
        /// <see cref="PdfSquareAnnotation"/>
        /// and
        /// <see cref="PdfCircleAnnotation"/>
        /// - the annotation's rectangle or ellipse, for
        /// <see cref="PdfRedactAnnotation"/>
        /// - the redacted
        /// region after the affected content has been removed.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of either
        /// <see cref="iText.Kernel.Colors.DeviceGray"/>
        /// ,
        /// <see cref="iText.Kernel.Colors.DeviceRgb"/>
        /// or
        /// <see cref="iText.Kernel.Colors.DeviceCmyk"/>
        /// type which defines
        /// interior color of the annotation, or null if interior color is not specified.
        /// </returns>
        public static Color ParseInteriorColor(PdfArray color) {
            if (color == null) {
                return null;
            }
            switch (color.Size()) {
                case 1: {
                    return new DeviceGray(color.GetAsNumber(0).FloatValue());
                }

                case 3: {
                    return new DeviceRgb(color.GetAsNumber(0).FloatValue(), color.GetAsNumber(1).FloatValue(), color.GetAsNumber
                        (2).FloatValue());
                }

                case 4: {
                    return new DeviceCmyk(color.GetAsNumber(0).FloatValue(), color.GetAsNumber(1).FloatValue(), color.GetAsNumber
                        (2).FloatValue(), color.GetAsNumber(3).FloatValue());
                }

                default: {
                    return null;
                }
            }
        }
    }
}
