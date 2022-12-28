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
using iText.Kernel.Pdf.Action;

namespace iText.Kernel.Pdf.Annot {
    public class PdfWidgetAnnotation : PdfAnnotation {
        public const int HIDDEN = 1;

        public const int VISIBLE_BUT_DOES_NOT_PRINT = 2;

        public const int HIDDEN_BUT_PRINTABLE = 3;

        public const int VISIBLE = 4;

        public PdfWidgetAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfWidgetAnnotation"/>
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
        protected internal PdfWidgetAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Widget;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetParent(PdfObject parent) {
            return (iText.Kernel.Pdf.Annot.PdfWidgetAnnotation)Put(PdfName.Parent, parent);
        }

        /// <summary>Setter for the annotation's highlighting mode.</summary>
        /// <remarks>
        /// Setter for the annotation's highlighting mode. Possible values are
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="PdfAnnotation.HIGHLIGHT_NONE"/>
        /// - No highlighting.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.HIGHLIGHT_INVERT"/>
        /// - Invert the contents of the annotation rectangle.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.HIGHLIGHT_OUTLINE"/>
        /// - Invert the annotation's border.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.HIGHLIGHT_PUSH"/>
        /// - Display the annotation?s down appearance, if any.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.HIGHLIGHT_TOGGLE"/>
        /// - Same as P.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="mode">The new value for the annotation's highlighting mode.</param>
        /// <returns>The widget annotation which this method was called on.</returns>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetHighlightMode(PdfName mode) {
            return (iText.Kernel.Pdf.Annot.PdfWidgetAnnotation)Put(PdfName.H, mode);
        }

        /// <summary>Getter for the annotation's highlighting mode.</summary>
        /// <returns>Current value of the annotation's highlighting mode.</returns>
        public virtual PdfName GetHighlightMode() {
            return GetPdfObject().GetAsName(PdfName.H);
        }

        /// <summary>Remove widget annotation from AcroForm hierarchy.</summary>
        public virtual void ReleaseFormFieldFromWidgetAnnotation() {
            PdfDictionary annotationDictionary = GetPdfObject();
            PdfDictionary parent = annotationDictionary.GetAsDictionary(PdfName.Parent);
            if (parent != null) {
                PdfArray kids = parent.GetAsArray(PdfName.Kids);
                kids.Remove(annotationDictionary);
                if (kids.IsEmpty()) {
                    parent.Remove(PdfName.Kids);
                }
            }
        }

        /// <summary>
        /// Set the visibility flags of the Widget annotation
        /// Options are: HIDDEN, HIDDEN_BUT_PRINTABLE, VISIBLE, VISIBLE_BUT_DOES_NOT_PRINT
        /// </summary>
        /// <param name="visibility">visibility option</param>
        /// <returns>the edited widget annotation</returns>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetVisibility(int visibility) {
            switch (visibility) {
                case HIDDEN: {
                    GetPdfObject().Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT | PdfAnnotation.HIDDEN));
                    break;
                }

                case VISIBLE_BUT_DOES_NOT_PRINT: {
                    break;
                }

                case HIDDEN_BUT_PRINTABLE: {
                    GetPdfObject().Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT | PdfAnnotation.NO_VIEW));
                    break;
                }

                case VISIBLE:
                default: {
                    GetPdfObject().Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT));
                    break;
                }
            }
            return this;
        }

        /// <summary>
        /// An
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to perform, such as launching an application, playing a sound,
        /// changing an annotation’s appearance state etc, when the annotation is activated.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which defines the characteristics and behaviour of an action.
        /// </returns>
        public virtual PdfDictionary GetAction() {
            return GetPdfObject().GetAsDictionary(PdfName.A);
        }

        /// <summary>
        /// Sets a
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed when the annotation is activated.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to set to this annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfWidgetAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetAction(PdfAction action) {
            return (iText.Kernel.Pdf.Annot.PdfWidgetAnnotation)Put(PdfName.A, action.GetPdfObject());
        }

        /// <summary>An additional actions dictionary that extends the set of events that can trigger the execution of an action.
        ///     </summary>
        /// <remarks>
        /// An additional actions dictionary that extends the set of events that can trigger the execution of an action.
        /// See ISO-320001 12.6.3 Trigger Events.
        /// </remarks>
        /// <returns>
        /// an additional actions
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </returns>
        /// <seealso cref="GetAction()"/>
        public virtual PdfDictionary GetAdditionalAction() {
            return GetPdfObject().GetAsDictionary(PdfName.AA);
        }

        /// <summary>
        /// Sets an additional
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed in response to
        /// the specific trigger event defined by
        /// <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        /// Sets an additional
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed in response to
        /// the specific trigger event defined by
        /// <paramref name="key"/>
        /// . See ISO-320001 12.6.3, "Trigger Events".
        /// </remarks>
        /// <param name="key">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that denotes a type of the additional action to set.
        /// </param>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to set as additional to this annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfWidgetAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetAdditionalAction(PdfName key, PdfAction action
            ) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        /// <summary>
        /// An appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream.
        /// </summary>
        /// <remarks>
        /// An appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream. See ISO-320001, Table 189.
        /// </remarks>
        /// <returns>an appearance characteristics dictionary or null if it isn't specified.</returns>
        public virtual PdfDictionary GetAppearanceCharacteristics() {
            return GetPdfObject().GetAsDictionary(PdfName.MK);
        }

        /// <summary>
        /// Sets an appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream.
        /// </summary>
        /// <remarks>
        /// Sets an appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream. See ISO-320001, Table 189.
        /// </remarks>
        /// <param name="characteristics">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// with additional information for appearance stream.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfWidgetAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetAppearanceCharacteristics(PdfDictionary characteristics
            ) {
            return (iText.Kernel.Pdf.Annot.PdfWidgetAnnotation)Put(PdfName.MK, characteristics);
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
        /// <see cref="PdfWidgetAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetBorderStyle(PdfDictionary borderStyle) {
            return (iText.Kernel.Pdf.Annot.PdfWidgetAnnotation)Put(PdfName.BS, borderStyle);
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
        /// <see cref="PdfWidgetAnnotation"/>
        /// instance.
        /// </returns>
        /// <seealso cref="GetBorderStyle()"/>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetBorderStyle(PdfName style) {
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
        /// <see cref="PdfWidgetAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfWidgetAnnotation SetDashPattern(PdfArray dashPattern) {
            return SetBorderStyle(BorderStyleUtil.SetDashPattern(GetBorderStyle(), dashPattern));
        }
    }
}
