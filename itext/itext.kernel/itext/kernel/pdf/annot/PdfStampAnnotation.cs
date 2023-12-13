/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    public class PdfStampAnnotation : PdfMarkupAnnotation {
        public PdfStampAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfStampAnnotation"/>
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
        protected internal PdfStampAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Stamp;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfStampAnnotation SetStampName(PdfName name) {
            return (iText.Kernel.Pdf.Annot.PdfStampAnnotation)Put(PdfName.Name, name);
        }

        public virtual PdfName GetStampName() {
            return GetPdfObject().GetAsName(PdfName.Name);
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
        /// <item><description>Approved
        /// </description></item>
        /// <item><description>Experimental
        /// </description></item>
        /// <item><description>NotApproved
        /// </description></item>
        /// <item><description>AsIs
        /// </description></item>
        /// <item><description>Expired
        /// </description></item>
        /// <item><description>NotForPublicRelease
        /// </description></item>
        /// <item><description>Confidential
        /// </description></item>
        /// <item><description>Final
        /// </description></item>
        /// <item><description>Sold
        /// </description></item>
        /// <item><description>Departmental
        /// </description></item>
        /// <item><description>ForComment
        /// </description></item>
        /// <item><description>TopSecret
        /// </description></item>
        /// <item><description>Draft
        /// </description></item>
        /// <item><description>ForPublicRelease.
        /// </description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfStampAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfStampAnnotation SetIconName(PdfName name) {
            return (iText.Kernel.Pdf.Annot.PdfStampAnnotation)Put(PdfName.Name, name);
        }
    }
}
