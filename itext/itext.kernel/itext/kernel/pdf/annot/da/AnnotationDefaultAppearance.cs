/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System;
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot.DA {
    /// <summary>Helper class for setting annotation default appearance.</summary>
    /// <remarks>
    /// Helper class for setting annotation default appearance. The class provides setters for
    /// font color, font size and font itself.
    /// <para />
    /// Note that only standard font names that do not require font resources are supported.
    /// <para />
    /// Note that it is possible to create annotation with custom font name in DA, but this require
    /// manual resource modifications (you have to put font in DR of AcroForm and use
    /// its resource name in DA) and only Acrobat supports that workflow.
    /// </remarks>
    public class AnnotationDefaultAppearance {
        private static readonly IDictionary<StandardAnnotationFont, String> stdAnnotFontNames = new Dictionary<StandardAnnotationFont
            , String>();

        private static readonly IDictionary<ExtendedAnnotationFont, String> extAnnotFontNames = new Dictionary<ExtendedAnnotationFont
            , String>();

        static AnnotationDefaultAppearance() {
            stdAnnotFontNames.Put(StandardAnnotationFont.CourierBoldOblique, "/" + StandardFonts.COURIER_BOLDOBLIQUE);
            stdAnnotFontNames.Put(StandardAnnotationFont.CourierBold, "/" + StandardFonts.COURIER_BOLD);
            stdAnnotFontNames.Put(StandardAnnotationFont.CourierOblique, "/" + StandardFonts.COURIER_OBLIQUE);
            stdAnnotFontNames.Put(StandardAnnotationFont.Courier, "/" + StandardFonts.COURIER);
            stdAnnotFontNames.Put(StandardAnnotationFont.HelveticaBoldOblique, "/" + StandardFonts.HELVETICA_BOLDOBLIQUE
                );
            stdAnnotFontNames.Put(StandardAnnotationFont.HelveticaBold, "/" + StandardFonts.HELVETICA_BOLD);
            stdAnnotFontNames.Put(StandardAnnotationFont.HelveticaOblique, "/" + StandardFonts.COURIER_OBLIQUE);
            stdAnnotFontNames.Put(StandardAnnotationFont.Helvetica, "/" + StandardFonts.HELVETICA);
            stdAnnotFontNames.Put(StandardAnnotationFont.Symbol, "/" + StandardFonts.SYMBOL);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesBoldItalic, "/" + StandardFonts.TIMES_BOLDITALIC);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesBold, "/" + StandardFonts.TIMES_BOLD);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesItalic, "/" + StandardFonts.TIMES_ITALIC);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesRoman, "/" + StandardFonts.TIMES_ROMAN);
            stdAnnotFontNames.Put(StandardAnnotationFont.ZapfDingbats, "/" + StandardFonts.ZAPFDINGBATS);
            extAnnotFontNames.Put(ExtendedAnnotationFont.HYSMyeongJoMedium, "/HySm");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HYGoThicMedium, "/HyGo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HeiseiKakuGoW5, "/KaGo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HeiseiMinW3, "/KaMi");
            extAnnotFontNames.Put(ExtendedAnnotationFont.MHeiMedium, "/MHei");
            extAnnotFontNames.Put(ExtendedAnnotationFont.MSungLight, "/MSun");
            extAnnotFontNames.Put(ExtendedAnnotationFont.STSongLight, "/STSo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.MSungStdLight, "/MSun");
            extAnnotFontNames.Put(ExtendedAnnotationFont.STSongStdLight, "/STSo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HYSMyeongJoStdMedium, "/HySm");
            extAnnotFontNames.Put(ExtendedAnnotationFont.KozMinProRegular, "/KaMi");
        }

        private String colorOperand = "0 g";

        private String rawFontName = "/Helv";

        private float fontSize = 0;

        /// <summary>
        /// Creates the default instance of
        /// <see cref="AnnotationDefaultAppearance"/>.
        /// </summary>
        /// <remarks>
        /// Creates the default instance of
        /// <see cref="AnnotationDefaultAppearance"/>.
        /// <para />
        /// The default font is
        /// <see cref="StandardAnnotationFont.Helvetica"/>
        /// . The default font size is 12.
        /// </remarks>
        public AnnotationDefaultAppearance() {
            SetFont(StandardAnnotationFont.Helvetica);
            SetFontSize(12);
        }

        /// <summary>
        /// Sets the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's default font.
        /// </summary>
        /// <param name="font">
        /// one of
        /// <see cref="StandardAnnotationFont">standard annotation fonts</see>
        /// to be set as
        /// the default one for this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetFont(StandardAnnotationFont font) {
            SetRawFontName(stdAnnotFontNames.Get(font));
            return this;
        }

        /// <summary>
        /// Sets the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's default font.
        /// </summary>
        /// <param name="font">
        /// one of
        /// <see cref="ExtendedAnnotationFont">extended annotation fonts</see>
        /// to be set as
        /// the default one for this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetFont(ExtendedAnnotationFont font) {
            SetRawFontName(extAnnotFontNames.Get(font));
            return this;
        }

        /// <summary>
        /// Sets the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's default font size.
        /// </summary>
        /// <param name="fontSize">
        /// font size to be set as the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's default font size
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetFontSize(float fontSize) {
            this.fontSize = fontSize;
            return this;
        }

        /// <summary>
        /// Sets the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's default font color.
        /// </summary>
        /// <param name="rgbColor">
        /// 
        /// <see cref="iText.Kernel.Colors.DeviceRgb"/>
        /// to be set as the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's
        /// default font color
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetColor(DeviceRgb rgbColor) {
            SetColorOperand(rgbColor.GetColorValue(), "rg");
            return this;
        }

        /// <summary>
        /// Sets the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's default font color.
        /// </summary>
        /// <param name="cmykColor">
        /// 
        /// <see cref="iText.Kernel.Colors.DeviceCmyk"/>
        /// to be set as the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's
        /// default font color
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetColor(DeviceCmyk cmykColor) {
            SetColorOperand(cmykColor.GetColorValue(), "k");
            return this;
        }

        /// <summary>
        /// Sets the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's default font color.
        /// </summary>
        /// <param name="grayColor">
        /// 
        /// <see cref="iText.Kernel.Colors.DeviceGray"/>
        /// to be set as the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's
        /// default font color
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetColor(DeviceGray grayColor) {
            SetColorOperand(grayColor.GetColorValue(), "g");
            return this;
        }

        /// <summary>
        /// Gets the
        /// <see cref="AnnotationDefaultAppearance"/>
        /// 's representation as
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// representation of this
        /// <see cref="AnnotationDefaultAppearance"/>
        /// </returns>
        public virtual PdfString ToPdfString() {
            return new PdfString(MessageFormatUtil.Format("{0} {1} Tf {2}", rawFontName, fontSize, colorOperand));
        }

        private void SetColorOperand(float[] colorValues, String operand) {
            StringBuilder builder = new StringBuilder();
            foreach (float value in colorValues) {
                builder.Append(MessageFormatUtil.Format("{0} ", value));
            }
            builder.Append(operand);
            this.colorOperand = builder.ToString();
        }

        private void SetRawFontName(String rawFontName) {
            if (rawFontName == null) {
                throw new ArgumentException("Passed raw font name can not be null");
            }
            this.rawFontName = rawFontName;
        }
    }
}
