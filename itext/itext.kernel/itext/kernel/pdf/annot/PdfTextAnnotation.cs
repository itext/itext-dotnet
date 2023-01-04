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

namespace iText.Kernel.Pdf.Annot {
    public class PdfTextAnnotation : PdfMarkupAnnotation {
        public PdfTextAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfTextAnnotation"/>
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
        protected internal PdfTextAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Text;
        }

        public virtual PdfString GetState() {
            return GetPdfObject().GetAsString(PdfName.State);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfTextAnnotation SetState(PdfString state) {
            return (iText.Kernel.Pdf.Annot.PdfTextAnnotation)Put(PdfName.State, state);
        }

        public virtual PdfString GetStateModel() {
            return GetPdfObject().GetAsString(PdfName.StateModel);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfTextAnnotation SetStateModel(PdfString stateModel) {
            return (iText.Kernel.Pdf.Annot.PdfTextAnnotation)Put(PdfName.StateModel, stateModel);
        }

        /// <summary>A flag specifying whether the annotation shall initially be displayed open.</summary>
        /// <remarks>
        /// A flag specifying whether the annotation shall initially be displayed open.
        /// This flag has affect to not all kinds of annotations.
        /// </remarks>
        /// <returns>true if annotation is initially open, false - if closed.</returns>
        public virtual bool GetOpen() {
            return PdfBoolean.TRUE.Equals(GetPdfObject().GetAsBoolean(PdfName.Open));
        }

        /// <summary>Sets a flag specifying whether the annotation shall initially be displayed open.</summary>
        /// <remarks>
        /// Sets a flag specifying whether the annotation shall initially be displayed open.
        /// This flag has affect to not all kinds of annotations.
        /// </remarks>
        /// <param name="open">true if annotation shall initially be open, false - if closed.</param>
        /// <returns>
        /// this
        /// <see cref="PdfTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTextAnnotation SetOpen(bool open) {
            return (iText.Kernel.Pdf.Annot.PdfTextAnnotation)Put(PdfName.Open, PdfBoolean.ValueOf(open));
        }

        /// <summary>The name of an icon that is used in displaying the annotation.</summary>
        /// <remarks>
        /// The name of an icon that is used in displaying the annotation.
        /// Possible values are described in
        /// <see cref="SetIconName(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that specifies the icon for displaying annotation, or null if icon name is not specified.
        /// </returns>
        public virtual PdfName GetIconName() {
            return GetPdfObject().GetAsName(PdfName.Name);
        }

        /// <summary>The name of an icon that is used in displaying the annotation.</summary>
        /// <param name="name">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that specifies the icon for displaying annotation. Possible values are:
        /// <list type="bullet">
        /// <item><description>Comment
        /// </description></item>
        /// <item><description>Key
        /// </description></item>
        /// <item><description>Note
        /// </description></item>
        /// <item><description>Help
        /// </description></item>
        /// <item><description>NewParagraph
        /// </description></item>
        /// <item><description>Paragraph
        /// </description></item>
        /// <item><description>Insert
        /// </description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfTextAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTextAnnotation SetIconName(PdfName name) {
            return (iText.Kernel.Pdf.Annot.PdfTextAnnotation)Put(PdfName.Name, name);
        }
    }
}
