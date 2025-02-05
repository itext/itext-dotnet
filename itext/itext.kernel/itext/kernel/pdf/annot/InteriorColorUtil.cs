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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
//\cond DO_NOT_DOCUMENT
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
//\endcond
}
