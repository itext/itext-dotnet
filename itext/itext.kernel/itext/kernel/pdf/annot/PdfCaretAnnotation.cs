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
    public class PdfCaretAnnotation : PdfMarkupAnnotation {
        public PdfCaretAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfCaretAnnotation"/>
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
        protected internal PdfCaretAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Caret;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfCaretAnnotation SetSymbol(PdfString symbol) {
            return (iText.Kernel.Pdf.Annot.PdfCaretAnnotation)Put(PdfName.Sy, symbol);
        }

        public virtual PdfString GetSymbol() {
            return GetPdfObject().GetAsString(PdfName.Sy);
        }

        /// <summary>
        /// A set of four numbers describing the numerical differences between two rectangles:
        /// the Rect entry of the annotation and the actual boundaries of the underlying caret.
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
        /// the Rect entry of the annotation and the actual boundaries of the underlying caret.
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
        /// <see cref="PdfCaretAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCaretAnnotation SetRectangleDifferences(PdfArray rect) {
            return (iText.Kernel.Pdf.Annot.PdfCaretAnnotation)Put(PdfName.RD, rect);
        }
    }
}
