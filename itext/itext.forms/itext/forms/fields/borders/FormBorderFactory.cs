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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout.Borders;

namespace iText.Forms.Fields.Borders {
    /// <summary>
    /// A factory for creating
    /// <see cref="AbstractFormBorder"/>
    /// implementations.
    /// </summary>
    public sealed class FormBorderFactory {
        private FormBorderFactory() {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns
        /// <see cref="iText.Layout.Borders.Border"/>
        /// for specific borderStyle.
        /// </summary>
        /// <param name="borderStyle">border style dictionary. ISO 32000-1 12.5.4</param>
        /// <param name="borderWidth">width of the border</param>
        /// <param name="borderColor">color of the border</param>
        /// <param name="backgroundColor">element background color. This param used for drawing beveled border type</param>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Borders.Border"/>
        /// implementation or
        /// <see langword="null"/>
        /// </returns>
        public static Border GetBorder(PdfDictionary borderStyle, float borderWidth, Color borderColor, Color backgroundColor
            ) {
            if (borderStyle == null || borderStyle.GetAsName(PdfName.S) == null || borderColor == null || borderWidth 
                <= 0) {
                return null;
            }
            Border resultBorder;
            PdfName borderType = borderStyle.GetAsName(PdfName.S);
            if (PdfName.U.Equals(borderType)) {
                resultBorder = new UnderlineBorder(borderColor, borderWidth);
            }
            else {
                if (PdfName.S.Equals(borderType)) {
                    resultBorder = new SolidBorder(borderColor, borderWidth);
                }
                else {
                    if (PdfName.D.Equals(borderType)) {
                        PdfArray dashArray = borderStyle.GetAsArray(PdfName.D);
                        float unitsOn = FixedDashedBorder.DEFAULT_UNITS_VALUE;
                        if (dashArray != null && dashArray.Size() > 0 && dashArray.GetAsNumber(0) != null) {
                            unitsOn = dashArray.GetAsNumber(0).IntValue();
                        }
                        float unitsOff = unitsOn;
                        if (dashArray != null && dashArray.Size() > 1 && dashArray.GetAsNumber(1) != null) {
                            unitsOff = dashArray.GetAsNumber(1).IntValue();
                        }
                        resultBorder = new FixedDashedBorder(borderColor, borderWidth, unitsOn, unitsOff, 0);
                    }
                    else {
                        if (PdfName.I.Equals(borderType)) {
                            resultBorder = new InsetBorder(borderColor, borderWidth);
                        }
                        else {
                            if (PdfName.B.Equals(borderType)) {
                                resultBorder = new BeveledBorder(borderColor, borderWidth, backgroundColor);
                            }
                            else {
                                resultBorder = null;
                            }
                        }
                    }
                }
            }
            return resultBorder;
        }
    }
}
