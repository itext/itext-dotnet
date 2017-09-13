using System;
using System.Collections.Generic;
using iText.Kernel;
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

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateXYZ(PdfStructElement elem, float left
            , float top, float zoom) {
            return Create(elem, PdfName.XYZ, left, float.NaN, float.NaN, top, zoom);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFit(PdfStructElement elem) {
            return Create(elem, PdfName.Fit, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitH(PdfStructElement elem, float 
            top) {
            return Create(elem, PdfName.FitH, float.NaN, float.NaN, float.NaN, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitV(PdfStructElement elem, float 
            left) {
            return Create(elem, PdfName.FitV, left, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitR(PdfStructElement elem, float 
            left, float bottom, float right, float top) {
            return Create(elem, PdfName.FitR, left, bottom, right, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitB(PdfStructElement elem) {
            return Create(elem, PdfName.FitB, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitBH(PdfStructElement elem, float
             top) {
            return Create(elem, PdfName.FitBH, float.NaN, float.NaN, float.NaN, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfStructureDestination CreateFitBV(PdfStructElement elem, float
             left) {
            return Create(elem, PdfName.FitBH, left, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        private static iText.Kernel.Pdf.Navigation.PdfStructureDestination Create(PdfStructElement elem, PdfName type
            , float left, float bottom, float right, float top, float zoom) {
            return new iText.Kernel.Pdf.Navigation.PdfStructureDestination().Add(elem).Add(type).Add(left).Add(bottom)
                .Add(right).Add(top).Add(zoom);
        }

        public override PdfObject GetDestinationPage(IDictionary<String, PdfObject> names) {
            PdfObject firstObj = ((PdfArray)GetPdfObject()).Get(0);
            if (firstObj.IsDictionary()) {
                PdfStructElement structElem = new PdfStructElement((PdfDictionary)firstObj);
                while (true) {
                    IList<IPdfStructElem> kids = structElem.GetKids();
                    IPdfStructElem firstKid = kids.Count > 0 ? kids[0] : null;
                    if (firstKid is PdfMcr) {
                        return ((PdfMcr)firstKid).GetPageObject();
                    }
                    else {
                        if (firstKid is PdfStructElement) {
                            structElem = (PdfStructElement)firstKid;
                        }
                        else {
                            break;
                        }
                    }
                }
            }
            return null;
        }

        public override PdfDestination ReplaceNamedDestination(IDictionary<Object, PdfObject> names) {
            return this;
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

        private iText.Kernel.Pdf.Navigation.PdfStructureDestination Add(PdfStructElement elem) {
            if (elem.GetPdfObject().GetIndirectReference() == null) {
                throw new PdfException(PdfException.StructureElementInStructureDestinationShallBeAnIndirectObject);
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
