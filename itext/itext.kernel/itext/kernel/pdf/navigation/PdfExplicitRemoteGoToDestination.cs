using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Navigation {
    /// <summary>
    /// This class shall be used for creation of destinations, associated Remote Go-To and Embedded Go-To actions only,
    /// i.e.
    /// </summary>
    /// <remarks>
    /// This class shall be used for creation of destinations, associated Remote Go-To and Embedded Go-To actions only,
    /// i.e. the destination point is in another PDF.
    /// If you need to create a destination, associated with an object inside current PDF, you should use
    /// <see cref="PdfExplicitDestination"/>
    /// class instead.
    /// </remarks>
    public class PdfExplicitRemoteGoToDestination : PdfDestination {
        public PdfExplicitRemoteGoToDestination()
            : this(new PdfArray()) {
        }

        public PdfExplicitRemoteGoToDestination(PdfArray pdfObject)
            : base(pdfObject) {
        }

        public override PdfObject GetDestinationPage(IDictionary<String, PdfObject> names) {
            return ((PdfArray)GetPdfObject()).Get(0);
        }

        public override PdfDestination ReplaceNamedDestination(IDictionary<Object, PdfObject> names) {
            return this;
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateXYZ(int pageNum, float left
            , float top, float zoom) {
            return Create(pageNum, PdfName.XYZ, left, float.NaN, float.NaN, top, zoom);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateFit(int pageNum) {
            return Create(pageNum, PdfName.Fit, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateFitH(int pageNum, float top
            ) {
            return Create(pageNum, PdfName.FitH, float.NaN, float.NaN, float.NaN, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateFitV(int pageNum, float left
            ) {
            return Create(pageNum, PdfName.FitV, left, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateFitR(int pageNum, float left
            , float bottom, float right, float top) {
            return Create(pageNum, PdfName.FitR, left, bottom, right, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateFitB(int pageNum) {
            return Create(pageNum, PdfName.FitB, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateFitBH(int pageNum, float 
            top) {
            return Create(pageNum, PdfName.FitBH, float.NaN, float.NaN, float.NaN, top, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination CreateFitBV(int pageNum, float 
            left) {
            return Create(pageNum, PdfName.FitBH, left, float.NaN, float.NaN, float.NaN, float.NaN);
        }

        public static iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination Create(int pageNum, PdfName type
            , float left, float bottom, float right, float top, float zoom) {
            return new iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination().Add(--pageNum).Add(type).Add(left
                ).Add(bottom).Add(right).Add(top).Add(zoom);
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }

        private iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination Add(float value) {
            if (!float.IsNaN(value)) {
                ((PdfArray)GetPdfObject()).Add(new PdfNumber(value));
            }
            return this;
        }

        private iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination Add(int value) {
            ((PdfArray)GetPdfObject()).Add(new PdfNumber(value));
            return this;
        }

        private iText.Kernel.Pdf.Navigation.PdfExplicitRemoteGoToDestination Add(PdfName type) {
            ((PdfArray)GetPdfObject()).Add(type);
            return this;
        }
    }
}
