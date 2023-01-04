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
    public class PdfScreenAnnotation : PdfAnnotation {
        public PdfScreenAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfScreenAnnotation"/>
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
        protected internal PdfScreenAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Screen;
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
        /// <see cref="PdfScreenAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfScreenAnnotation SetAction(PdfAction action) {
            return (iText.Kernel.Pdf.Annot.PdfScreenAnnotation)Put(PdfName.A, action.GetPdfObject());
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
        /// <see cref="PdfScreenAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfScreenAnnotation SetAdditionalAction(PdfName key, PdfAction action
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
        /// <see cref="PdfScreenAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfScreenAnnotation SetAppearanceCharacteristics(PdfDictionary characteristics
            ) {
            return (iText.Kernel.Pdf.Annot.PdfScreenAnnotation)Put(PdfName.MK, characteristics);
        }
    }
}
