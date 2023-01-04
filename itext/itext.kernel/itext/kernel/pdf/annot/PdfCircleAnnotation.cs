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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    public class PdfCircleAnnotation : PdfMarkupAnnotation {
        public PdfCircleAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfCircleAnnotation"/>
        /// instance based on
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// instance, that represents existing annotation object in the document.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing annotation object
        /// </param>
        /// <seealso cref="PdfAnnotation.MakeAnnotation(iText.Kernel.Pdf.PdfObject)"/>
        protected internal PdfCircleAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Circle;
        }

        /// <summary>The dictionaries for some annotation types (such as free text and polygon annotations) can include the BS entry.
        ///     </summary>
        /// <remarks>
        /// The dictionaries for some annotation types (such as free text and polygon annotations) can include the BS entry.
        /// That entry specifies a border style dictionary that has more settings than the array specified for the Border
        /// entry (see
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ). If an annotation dictionary includes the BS entry, then the Border
        /// entry is ignored. If annotation includes AP (see
        /// <see cref="PdfAnnotation.GetAppearanceDictionary()"/>
        /// ) it takes
        /// precedence over the BS entry. For more info on BS entry see ISO-320001, Table 166.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which is a border style dictionary or null if it is not specified.
        /// </returns>
        public virtual PdfDictionary GetBorderStyle() {
            return GetPdfObject().GetAsDictionary(PdfName.BS);
        }

        /// <summary>
        /// Sets border style dictionary that has more settings than the array specified for the Border entry (
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ).
        /// </summary>
        /// <remarks>
        /// Sets border style dictionary that has more settings than the array specified for the Border entry (
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ).
        /// See ISO-320001, Table 166 and
        /// <see cref="GetBorderStyle()"/>
        /// for more info.
        /// </remarks>
        /// <param name="borderStyle">
        /// a border style dictionary specifying the line width and dash pattern that shall be used
        /// in drawing the annotation’s border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfCircleAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCircleAnnotation SetBorderStyle(PdfDictionary borderStyle) {
            return (iText.Kernel.Pdf.Annot.PdfCircleAnnotation)Put(PdfName.BS, borderStyle);
        }

        /// <summary>Setter for the annotation's preset border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset border style. Possible values are
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_SOLID"/>
        /// - A solid rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// - A dashed rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_BEVELED"/>
        /// - A simulated embossed rectangle that appears to be raised above the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_INSET"/>
        /// - A simulated engraved rectangle that appears to be recessed below the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_UNDERLINE"/>
        /// - A single line along the bottom of the annotation rectangle.
        /// </description></item>
        /// </list>
        /// See also ISO-320001, Table 166.
        /// </remarks>
        /// <param name="style">The new value for the annotation's border style.</param>
        /// <returns>
        /// this
        /// <see cref="PdfCircleAnnotation"/>
        /// instance.
        /// </returns>
        /// <seealso cref="GetBorderStyle()"/>
        public virtual iText.Kernel.Pdf.Annot.PdfCircleAnnotation SetBorderStyle(PdfName style) {
            return SetBorderStyle(BorderStyleUtil.SetStyle(GetBorderStyle(), style));
        }

        /// <summary>Setter for the annotation's preset dashed border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset dashed border style. This property has affect only if
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// style was used for the annotation border style (see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>.
        /// See ISO-320001 8.4.3.6, "Line Dash Pattern" for the format in which dash pattern shall be specified.
        /// </remarks>
        /// <param name="dashPattern">
        /// a dash array defining a pattern of dashes and gaps that
        /// shall be used in drawing a dashed border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfCircleAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCircleAnnotation SetDashPattern(PdfArray dashPattern) {
            return SetBorderStyle(BorderStyleUtil.SetDashPattern(GetBorderStyle(), dashPattern));
        }

        /// <summary>
        /// A set of four numbers describing the numerical differences between two rectangles:
        /// the Rect entry of the annotation and the actual boundaries of the underlying circle.
        /// </summary>
        /// <returns>
        /// null if not specified, otherwise a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// with four numbers which correspond to the
        /// differences in default user space between the left, top, right, and bottom coordinates of Rect and those
        /// of the inner rectangle, respectively.
        /// </returns>
        public virtual PdfArray GetRectangleDifferences() {
            return GetPdfObject().GetAsArray(PdfName.RD);
        }

        /// <summary>
        /// A set of four numbers describing the numerical differences between two rectangles:
        /// the Rect entry of the annotation and the actual boundaries of the underlying circle.
        /// </summary>
        /// <param name="rect">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// with four numbers which correspond to the differences in default user space between
        /// the left, top, right, and bottom coordinates of Rect and those of the inner rectangle, respectively.
        /// Each value shall be greater than or equal to 0. The sum of the top and bottom differences shall be
        /// less than the height of Rect, and the sum of the left and right differences shall be less than
        /// the width of Rect.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfCircleAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCircleAnnotation SetRectangleDifferences(PdfArray rect) {
            return (iText.Kernel.Pdf.Annot.PdfCircleAnnotation)Put(PdfName.RD, rect);
        }

        /// <summary>Gets a border effect dictionary that specifies an effect that shall be applied to the border of the annotations.
        ///     </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// , which is a border effect dictionary (see ISO-320001, Table 167).
        /// </returns>
        public virtual PdfDictionary GetBorderEffect() {
            return GetPdfObject().GetAsDictionary(PdfName.BE);
        }

        /// <summary>Sets a border effect dictionary that specifies an effect that shall be applied to the border of the annotations.
        ///     </summary>
        /// <param name="borderEffect">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which contents shall be specified in accordance to ISO-320001, Table 167.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfCircleAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCircleAnnotation SetBorderEffect(PdfDictionary borderEffect) {
            return (iText.Kernel.Pdf.Annot.PdfCircleAnnotation)Put(PdfName.BE, borderEffect);
        }

        /// <summary>The interior color which is used to fill the annotation's ellipse.</summary>
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
        public virtual Color GetInteriorColor() {
            return InteriorColorUtil.ParseInteriorColor(GetPdfObject().GetAsArray(PdfName.IC));
        }

        /// <summary>
        /// An array of numbers in the range 0.0 to 1.0 specifying the interior color
        /// which is used to fill the annotation's ellipse.
        /// </summary>
        /// <param name="interiorColor">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers in the range 0.0 to 1.0. The number of array elements determines
        /// the colour space in which the colour is defined: 0 - No colour, transparent; 1 - DeviceGray,
        /// 3 - DeviceRGB, 4 - DeviceCMYK. For the
        /// <see cref="PdfRedactAnnotation"/>
        /// number of elements shall be
        /// equal to 3 (which defines DeviceRGB colour space).
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfCircleAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCircleAnnotation SetInteriorColor(PdfArray interiorColor) {
            return (iText.Kernel.Pdf.Annot.PdfCircleAnnotation)Put(PdfName.IC, interiorColor);
        }

        /// <summary>
        /// An array of numbers in the range 0.0 to 1.0 specifying the interior color
        /// which is used to fill the annotation's ellipse.
        /// </summary>
        /// <param name="interiorColor">an array of floats in the range 0.0 to 1.0.</param>
        /// <returns>
        /// this
        /// <see cref="PdfCircleAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCircleAnnotation SetInteriorColor(float[] interiorColor) {
            return SetInteriorColor(new PdfArray(interiorColor));
        }
    }
}
