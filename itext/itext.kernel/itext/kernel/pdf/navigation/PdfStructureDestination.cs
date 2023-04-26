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
