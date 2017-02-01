/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.IO.Log;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;

namespace iText.Kernel.Pdf.Annot {
    public class PdfLinkAnnotation : PdfAnnotation {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.Annot.PdfLinkAnnotation
            ));

        /// <summary>Highlight modes.</summary>
        public static readonly PdfName None = PdfName.N;

        public static readonly PdfName Invert = PdfName.I;

        public static readonly PdfName Outline = PdfName.O;

        public static readonly PdfName Push = PdfName.P;

        public PdfLinkAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public PdfLinkAnnotation(Rectangle rect)
            : base(rect) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Link;
        }

        public virtual PdfObject GetDestinationObject() {
            return GetPdfObject().Get(PdfName.Dest);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetDestination(PdfObject destination) {
            if (GetPdfObject().ContainsKey(PdfName.A)) {
                GetPdfObject().Remove(PdfName.A);
                logger.Warn(iText.IO.LogMessageConstant.DESTINATION_NOT_PERMITTED_WHEN_ACTION_IS_SET);
            }
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.Dest, destination);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetDestination(PdfDestination destination) {
            return SetDestination(destination.GetPdfObject());
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation RemoveDestination() {
            GetPdfObject().Remove(PdfName.Dest);
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetAction(PdfDictionary action) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.A, action);
        }

        public override PdfAnnotation SetAction(PdfAction action) {
            if (GetDestinationObject() != null) {
                RemoveDestination();
                logger.Warn(iText.IO.LogMessageConstant.ACTION_WAS_SET_TO_LINK_ANNOTATION_WITH_DESTINATION);
            }
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.A, action.GetPdfObject());
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation RemoveAction() {
            GetPdfObject().Remove(PdfName.A);
            return this;
        }

        public virtual PdfName GetHighlightMode() {
            return GetPdfObject().GetAsName(PdfName.H);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetHighlightMode(PdfName hlMode) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.H, hlMode);
        }

        public virtual PdfDictionary GetUriActionObject() {
            return GetPdfObject().GetAsDictionary(PdfName.PA);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetUriAction(PdfDictionary action) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.PA, action);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetUriAction(PdfAction action) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.PA, action.GetPdfObject());
        }
    }
}
