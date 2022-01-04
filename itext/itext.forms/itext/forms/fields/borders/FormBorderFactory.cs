/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
