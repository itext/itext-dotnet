/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections.Generic;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Navigation {
    public class PdfStructureDestination : PdfDestination {
        public PdfStructureDestination(PdfArray structureDestination)
            : base(structureDestination) {
        }

        private PdfStructureDestination()
            : base(new PdfArray()) {
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateXYZ(PdfStructElem elem, float left
            , float top, float zoom) {
            return Create(elem, PdfName.XYZ, left, float.NaN, float.NaN, top, zoom);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFit(PdfStructElem elem) {
            return Create(elem, PdfName.Fit, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitH(PdfStructElem elem, float top
            ) {
            return Create(elem, PdfName.FitH, float.NaN, float.NaN, float.NaN, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitV(PdfStructElem elem, float left
            ) {
            return Create(elem, PdfName.FitV, left, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitR(PdfStructElem elem, float left
            , float bottom, float right, float top) {
            return Create(elem, PdfName.FitR, left, bottom, right, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitB(PdfStructElem elem) {
            return Create(elem, PdfName.FitB, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitBH(PdfStructElem elem, float top
            ) {
            return Create(elem, PdfName.FitBH, float.NaN, float.NaN, float.NaN, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitBV(PdfStructElem elem, float left
            ) {
            return Create(elem, PdfName.FitBH, left, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        private static iText.Kernel.Pdf.Navigation.PdfStructureDestination Create(PdfStructElem elem, PdfName type
            , float left, float bottom, float right, float top, float zoom) {
            return new iText.Kernel.Pdf.Navigation.PdfStructureDestination().Add(elem).Add(type).Add(left).Add(bottom)
                .Add(right).Add(top).Add(zoom);
        }

        public override PdfObject GetDestinationPage(IPdfNameTreeAccess names) {
            PdfObject firstObj = ((PdfArray)GetPdfObject()).Get(0);
            if (firstObj.IsDictionary()) {
                PdfStructElem structElem = new PdfStructElem((PdfDictionary)firstObj);
                while (true) {
                    IList<IStructureNode> kids = structElem.GetKids();
                    IStructureNode firstKid = kids.Count > 0 ? kids[0] : null;
                    if (firstKid is PdfMcr) {
                        return ((PdfMcr)firstKid).GetPageObject();
                    }
                    else {
                        if (firstKid is PdfStructElem) {
                            structElem = (PdfStructElem)firstKid;
                        }
                        else {
                            break;
                        }
                    }
                }
            }
            return null;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }

        private iText.Kernel.Pdf.Navigation.PdfStructureDestination Add(float value) {
            if (!float.IsNaN(value)) {
                ((PdfArray)GetPdfObject()).Add(new PdfNumber(value));
            }
            return this;
        }

        private iText.Kernel.Pdf.Navigation.PdfStructureDestination Add(PdfStructElem elem) {
            if (elem.GetPdfObject().GetIndirectReference() == null) {
                throw new PdfException(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_IN_STRUCTURE_DESTINATION_SHALL_BE_AN_INDIRECT_OBJECT
                    );
            }
            ((PdfArray)GetPdfObject()).Add(elem.GetPdfObject());
            return this;
        }

        private iText.Kernel.Pdf.Navigation.PdfStructureDestination Add(PdfName type) {
            ((PdfArray)GetPdfObject()).Add(type);
            return this;
        }
    }
}
