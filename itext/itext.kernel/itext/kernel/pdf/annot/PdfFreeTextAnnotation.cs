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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot.DA;

namespace iText.Kernel.Pdf.Annot {
    public class PdfFreeTextAnnotation : PdfMarkupAnnotation {
        /// <summary>Text justification options.</summary>
        public const int LEFT_JUSTIFIED = 0;

        public const int CENTERED = 1;

        public const int RIGHT_JUSTIFIED = 2;

        /// <summary>Creates new instance</summary>
        /// <param name="rect">- rectangle that specifies annotation position and bounds on page</param>
        /// <param name="contents">- the displayed text</param>
        public PdfFreeTextAnnotation(Rectangle rect, PdfString contents)
            : base(rect) {
            SetContents(contents);
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfFreeTextAnnotation"/>
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
        protected internal PdfFreeTextAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.FreeText;
        }

        public virtual PdfString GetDefaultStyleString() {
            return GetPdfObject().GetAsString(PdfName.DS);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetDefaultStyleString(PdfString defaultStyleString
            ) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.DS, defaultStyleString);
        }

        /// <summary>The default appearance string that shall be used in formatting the text.</summary>
        /// <remarks>The default appearance string that shall be used in formatting the text. See ISO-32001 12.7.3.3, "Variable Text".
        ///     </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// that specifies the default appearance, or null if default appereance is not specified.
        /// </returns>
        public virtual PdfString GetDefaultAppearance() {
            return GetPdfObject().GetAsString(PdfName.DA);
        }

        /// <summary>The default appearance string that shall be used in formatting the text.</summary>
        /// <remarks>The default appearance string that shall be used in formatting the text. See ISO-32001 12.7.3.3, "Variable Text".
        ///     </remarks>
        /// <param name="appearanceString">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// that specifies the default appearance.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfFreeTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetDefaultAppearance(PdfString appearanceString
            ) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.DA, appearanceString);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetDefaultAppearance(AnnotationDefaultAppearance
             da) {
            return SetDefaultAppearance(da.ToPdfString());
        }

        public virtual PdfArray GetCalloutLine() {
            return GetPdfObject().GetAsArray(PdfName.CL);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetCalloutLine(float[] calloutLine) {
            return SetCalloutLine(new PdfArray(calloutLine));
        }

        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetCalloutLine(PdfArray calloutLine) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.CL, calloutLine);
        }

        public virtual PdfName GetLineEndingStyle() {
            return GetPdfObject().GetAsName(PdfName.LE);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetLineEndingStyle(PdfName lineEndingStyle) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.LE, lineEndingStyle);
        }

        /// <summary>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified.
        /// </summary>
        /// <remarks>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified. Default value: 0 (left-justified).
        /// </remarks>
        /// <returns>a code specifying the form of quadding (justification), returns the default value if not explicitly specified.
        ///     </returns>
        public virtual int GetJustification() {
            PdfNumber q = GetPdfObject().GetAsNumber(PdfName.Q);
            return q == null ? 0 : q.IntValue();
        }

        /// <summary>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified.
        /// </summary>
        /// <remarks>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified. Default value: 0 (left-justified).
        /// </remarks>
        /// <param name="justification">a code specifying the form of quadding (justification).</param>
        /// <returns>
        /// this
        /// <see cref="PdfFreeTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetJustification(int justification) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.Q, new PdfNumber(justification));
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
        /// <see cref="PdfFreeTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetBorderStyle(PdfDictionary borderStyle) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.BS, borderStyle);
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
        /// <see cref="PdfFreeTextAnnotation"/>
        /// instance.
        /// </returns>
        /// <seealso cref="GetBorderStyle()"/>
        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetBorderStyle(PdfName style) {
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
        /// <see cref="PdfFreeTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetDashPattern(PdfArray dashPattern) {
            return SetBorderStyle(BorderStyleUtil.SetDashPattern(GetBorderStyle(), dashPattern));
        }

        /// <summary>
        /// A set of four numbers describing the numerical differences between two rectangles:
        /// the Rect entry of the annotation and the inner rectangle where the annotation's text should be displayed
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
        /// the Rect entry of the annotation and the inner rectangle where the annotation's text should be displayed
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
        /// <see cref="PdfFreeTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetRectangleDifferences(PdfArray rect) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.RD, rect);
        }

        /// <summary>A border effect dictionary that specifies an effect that shall be applied to the border of the annotations.
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
        /// <see cref="PdfFreeTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation SetBorderEffect(PdfDictionary borderEffect) {
            return (iText.Kernel.Pdf.Annot.PdfFreeTextAnnotation)Put(PdfName.BE, borderEffect);
        }
    }
}
