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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    /// <summary>
    /// This is a super class for the annotations which are defined as markup annotations
    /// because they are used primarily to mark up PDF documents.
    /// </summary>
    /// <remarks>
    /// This is a super class for the annotations which are defined as markup annotations
    /// because they are used primarily to mark up PDF documents. These annotations have
    /// text that appears as part of the annotation and may be displayed in other ways
    /// by a conforming reader, such as in a Comments pane.
    /// See also ISO-320001 12.5.6.2 "Markup Annotations".
    /// </remarks>
    public abstract class PdfMarkupAnnotation : PdfAnnotation {
        protected internal PdfAnnotation inReplyTo = null;

        protected internal PdfPopupAnnotation popup = null;

        protected internal PdfMarkupAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfMarkupAnnotation"/>
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
        protected internal PdfMarkupAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// The text label that will be displayed in the title bar of the annotation's pop-up window
        /// when open and active.
        /// </summary>
        /// <remarks>
        /// The text label that will be displayed in the title bar of the annotation's pop-up window
        /// when open and active. This entry shall identify the user who added the annotation.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value is an annotation text label content
        /// or null if text is not specified.
        /// </returns>
        public virtual PdfString GetText() {
            return GetPdfObject().GetAsString(PdfName.T);
        }

        /// <summary>
        /// Sets the text label that will be displayed in the title bar of the annotation's pop-up window
        /// when open and active.
        /// </summary>
        /// <remarks>
        /// Sets the text label that will be displayed in the title bar of the annotation's pop-up window
        /// when open and active. This entry shall identify the user who added the annotation.
        /// </remarks>
        /// <param name="text">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value is an annotation text label content.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetText(PdfString text) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.T, text);
        }

        /// <summary>The constant opacity value that will be used in painting the annotation.</summary>
        /// <remarks>
        /// The constant opacity value that will be used in painting the annotation.
        /// This value is applied to all visible elements of the annotation in its closed state
        /// (including its background and border) but not to the pop-up window that appears when
        /// the annotation is opened. Default value: 1.0.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfNumber"/>
        /// which value is in range between 0 and 1, which specifies the
        /// level of opacity. This method returns null if opacity is not specified; in this case default
        /// value is used, which is 1.
        /// </returns>
        public virtual PdfNumber GetOpacity() {
            return GetPdfObject().GetAsNumber(PdfName.CA);
        }

        /// <summary>Sets the constant opacity value that will be used in painting the annotation.</summary>
        /// <param name="ca">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfNumber"/>
        /// which value is in range between 0 and 1, which specifies the
        /// level of opacity.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        /// <seealso cref="GetOpacity()"/>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetOpacity(PdfNumber ca) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.CA, ca);
        }

        /// <summary>
        /// A rich text string (see ISO-320001 12.7.3.4, "Rich Text Strings") that
        /// shall be displayed in the pop-up window when the annotation is opened.
        /// </summary>
        /// <returns>
        /// text string or text stream that specifies rich text or null if
        /// rich text is not specified.
        /// </returns>
        public virtual PdfObject GetRichText() {
            return GetPdfObject().Get(PdfName.RC);
        }

        /// <summary>
        /// Sets a rich text string (see ISO-320001 12.7.3.4, "Rich Text Strings") that
        /// shall be displayed in the pop-up window when the annotation is opened.
        /// </summary>
        /// <param name="richText">text string or text stream that specifies rich text.</param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetRichText(PdfObject richText) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.RC, richText);
        }

        /// <summary>The date and time when the annotation was created.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value should be in the date format specified in (ISO-320001 7.9.4, "Dates").
        /// </returns>
        public virtual PdfString GetCreationDate() {
            return GetPdfObject().GetAsString(PdfName.CreationDate);
        }

        /// <summary>Sets the date and time when the annotation was created.</summary>
        /// <param name="creationDate">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value should be in the date format
        /// specified in (ISO-320001 7.9.4, "Dates").
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetCreationDate(PdfString creationDate) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.CreationDate, creationDate);
        }

        /// <summary>
        /// An annotation object that this annotation is "in reply to."
        /// Both annotations shall be on the same page of the document.
        /// </summary>
        /// <remarks>
        /// An annotation object that this annotation is "in reply to."
        /// Both annotations shall be on the same page of the document.
        /// The relationship between the two annotations shall be specified by the RT entry
        /// (see
        /// <see cref="GetReplyType()"/>
        /// ).
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents an annotation that this annotation is "in reply to."
        /// </returns>
        public virtual PdfDictionary GetInReplyToObject() {
            return GetPdfObject().GetAsDictionary(PdfName.IRT);
        }

        /// <summary>
        /// An annotation that this annotation is "in reply to."
        /// Both annotations shall be on the same page of the document.
        /// </summary>
        /// <remarks>
        /// An annotation that this annotation is "in reply to."
        /// Both annotations shall be on the same page of the document.
        /// The relationship between the two annotations shall be specified by the RT entry
        /// (see
        /// <see cref="GetReplyType()"/>
        /// ).
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="PdfAnnotation"/>
        /// that this annotation is "in reply to."
        /// </returns>
        public virtual PdfAnnotation GetInReplyTo() {
            if (inReplyTo == null) {
                inReplyTo = MakeAnnotation(GetInReplyToObject());
            }
            return inReplyTo;
        }

        /// <summary>
        /// Sets an annotation that this annotation is "in reply to."
        /// Both annotations shall be on the same page of the document.
        /// </summary>
        /// <remarks>
        /// Sets an annotation that this annotation is "in reply to."
        /// Both annotations shall be on the same page of the document.
        /// The relationship between the two annotations shall be specified by the RT entry
        /// (see
        /// <see cref="GetReplyType()"/>
        /// ).
        /// </remarks>
        /// <param name="inReplyTo">
        /// a
        /// <see cref="PdfAnnotation"/>
        /// that this annotation is "in reply to."
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetInReplyTo(PdfAnnotation inReplyTo) {
            this.inReplyTo = inReplyTo;
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.IRT, inReplyTo.GetPdfObject());
        }

        /// <summary>Sets a pop-up annotation for entering or editing the text associated with this annotation.</summary>
        /// <remarks>
        /// Sets a pop-up annotation for entering or editing the text associated with this annotation.
        /// Pop-up annotation defines an associated with this annotation pop-up window that may contain text.
        /// The Contents (see
        /// <see cref="PdfAnnotation.SetContents(iText.Kernel.Pdf.PdfString)"/>
        /// ) entry of the annotation that has
        /// an associated popup specifies the text that shall be displayed when the pop-up window is opened.
        /// </remarks>
        /// <param name="popup">
        /// an
        /// <see cref="PdfPopupAnnotation"/>
        /// that will be associated with this annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetPopup(PdfPopupAnnotation popup) {
            this.popup = popup;
            popup.SetParent(this);
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.Popup, popup.GetPdfObject());
        }

        /// <summary>An associated pop-up annotation object.</summary>
        /// <remarks>
        /// An associated pop-up annotation object. See
        /// <see cref="GetPopup()"/>
        /// for more info.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents an associated pop-up annotation,
        /// or null if popup annotation is not specified.
        /// </returns>
        public virtual PdfDictionary GetPopupObject() {
            return GetPdfObject().GetAsDictionary(PdfName.Popup);
        }

        /// <summary>An associated pop-up annotation for entering or editing the text associated with this annotation.
        ///     </summary>
        /// <remarks>
        /// An associated pop-up annotation for entering or editing the text associated with this annotation.
        /// Pop-up annotation defines an associated with this annotation pop-up window that may contain text.
        /// The Contents (see
        /// <see cref="PdfAnnotation.GetContents()"/>
        /// ) entry of the annotation that has
        /// an associated popup specifies the text that shall be displayed when the pop-up window is opened.
        /// </remarks>
        /// <returns>
        /// an
        /// <see cref="PdfPopupAnnotation"/>
        /// that is associated with this annotation, or null if there is none.
        /// </returns>
        public virtual PdfPopupAnnotation GetPopup() {
            if (popup == null) {
                PdfDictionary popupObject = GetPopupObject();
                if (popupObject != null) {
                    PdfAnnotation annotation = MakeAnnotation(popupObject);
                    if (!(annotation is PdfPopupAnnotation)) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Annot.PdfMarkupAnnotation));
                        logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.POPUP_ENTRY_IS_NOT_POPUP_ANNOTATION);
                        return null;
                    }
                    popup = (PdfPopupAnnotation)annotation;
                }
            }
            return popup;
        }

        /// <summary>Text representing a short description of the subject being addressed by the annotation.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value is a annotation subject.
        /// </returns>
        public virtual PdfString GetSubject() {
            return GetPdfObject().GetAsString(PdfName.Subj);
        }

        /// <summary>Sets the text representing a short description of the subject being addressed by the annotation.</summary>
        /// <param name="subject">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value is a annotation subject.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetSubject(PdfString subject) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.Subj, subject);
        }

        /// <summary>
        /// A name specifying the relationship (the "reply type") between this annotation and one specified by IRT entry
        /// (see
        /// <see cref="GetInReplyTo()"/>
        /// ).
        /// </summary>
        /// <remarks>
        /// A name specifying the relationship (the "reply type") between this annotation and one specified by IRT entry
        /// (see
        /// <see cref="GetInReplyTo()"/>
        /// ). Valid values are:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.R"/>
        /// - The annotation shall be considered a reply to the annotation specified by IRT.
        /// Conforming readers shall not display replies to an annotation individually but together in the form of
        /// threaded comments.
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Group"/>
        /// - The annotation shall be grouped with the annotation specified by IRT.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// specifying relationship with the specified by the IRT entry; or null if reply
        /// type is not specified, in this case the default value is
        /// <see cref="iText.Kernel.Pdf.PdfName.R"/>.
        /// </returns>
        public virtual PdfName GetReplyType() {
            return GetPdfObject().GetAsName(PdfName.RT);
        }

        /// <summary>
        /// Sets the relationship (the "reply type") between this annotation and one specified by IRT entry
        /// (see
        /// <see cref="SetInReplyTo(PdfAnnotation)"/>
        /// ).
        /// </summary>
        /// <remarks>
        /// Sets the relationship (the "reply type") between this annotation and one specified by IRT entry
        /// (see
        /// <see cref="SetInReplyTo(PdfAnnotation)"/>
        /// ). For valid values see
        /// <see cref="GetInReplyTo()"/>.
        /// </remarks>
        /// <param name="replyType">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// specifying relationship with the specified by the IRT entry.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetReplyType(PdfName replyType) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.RT, replyType);
        }

        /// <summary>A name describing the intent of the markup annotation.</summary>
        /// <remarks>
        /// A name describing the intent of the markup annotation.
        /// See
        /// <see cref="SetIntent(iText.Kernel.Pdf.PdfName)"/>
        /// for more info.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// describing the intent of the markup annotation, or null if not specified.
        /// </returns>
        public virtual PdfName GetIntent() {
            return GetPdfObject().GetAsName(PdfName.IT);
        }

        /// <summary>Sets a name describing the intent of the markup annotation.</summary>
        /// <remarks>
        /// Sets a name describing the intent of the markup annotation.
        /// Intents allow conforming readers to distinguish between different uses and behaviors
        /// of a single markup annotation type. If this entry is not present or its value is the same as the annotation type,
        /// the annotation shall have no explicit intent and should behave in a generic manner in a conforming reader.
        /// <para />
        /// See ISO-320001, free text annotations (Table 174), line annotations (Table 175), polygon annotations (Table 178),
        /// and polyline annotations (Table 178) for the specific intent values for those types.
        /// </remarks>
        /// <param name="intent">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// describing the intent of the markup annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetIntent(PdfName intent) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.IT, intent);
        }

        /// <summary>An external data dictionary specifying data that shall be associated with the annotation.</summary>
        /// <remarks>
        /// An external data dictionary specifying data that shall be associated with the annotation.
        /// This dictionary contains the following entries:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Type"/>
        /// - (optional) If present, shall be
        /// <see cref="iText.Kernel.Pdf.PdfName.ExData"/>.
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Subtype"/>
        /// - (required) a name specifying the type of data that the markup annotation
        /// shall be associated with. The only defined value is
        /// <see cref="iText.Kernel.Pdf.PdfName.Markup3D"/>
        /// . Table 298 (ISO-320001)
        /// lists the values that correspond to a subtype of Markup3D (See also
        /// <see cref="Pdf3DAnnotation"/>
        /// ).
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// An external data
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// , or null if not specified.
        /// </returns>
        public virtual PdfDictionary GetExternalData() {
            return GetPdfObject().GetAsDictionary(PdfName.ExData);
        }

        /// <summary>Sets an external data dictionary specifying data that shall be associated with the annotation.</summary>
        /// <remarks>
        /// Sets an external data dictionary specifying data that shall be associated with the annotation.
        /// This dictionary should contain the following entries:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Type"/>
        /// - (optional) If present, shall be
        /// <see cref="iText.Kernel.Pdf.PdfName.ExData"/>.
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Subtype"/>
        /// - (required) a name specifying the type of data that the markup annotation
        /// shall be associated with. The only defined value is
        /// <see cref="iText.Kernel.Pdf.PdfName.Markup3D"/>
        /// . Table 298 (ISO-320001)
        /// lists the values that correspond to a subtype of Markup3D (See also
        /// <see cref="Pdf3DAnnotation"/>
        /// ).
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="exData">the external data dictionary</param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetExternalData(PdfDictionary exData) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.ExData, exData);
        }
    }
}
