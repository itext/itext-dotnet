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
using iText.Kernel.Color;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    public abstract class PdfMarkupAnnotation : PdfAnnotation {
        protected internal PdfAnnotation inReplyTo = null;

        protected internal PdfPopupAnnotation popup = null;

        protected internal PdfMarkupAnnotation(Rectangle rect)
            : base(rect) {
        }

        protected internal PdfMarkupAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public virtual PdfString GetText() {
            return GetPdfObject().GetAsString(PdfName.T);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetText(PdfString text) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.T, text);
        }

        public virtual PdfNumber GetOpacity() {
            return GetPdfObject().GetAsNumber(PdfName.CA);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetOpacity(PdfNumber ca) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.CA, ca);
        }

        public virtual PdfObject GetRichText() {
            return GetPdfObject().GetAsDictionary(PdfName.RC);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetRichText(PdfObject richText) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.RC, richText);
        }

        public virtual PdfString GetCreationDate() {
            return GetPdfObject().GetAsString(PdfName.CreationDate);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetCreationDate(PdfString creationDate) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.CreationDate, creationDate);
        }

        public virtual PdfDictionary GetInReplyToObject() {
            return GetPdfObject().GetAsDictionary(PdfName.IRT);
        }

        public virtual PdfAnnotation GetInReplyTo() {
            return inReplyTo;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetInReplyTo(PdfAnnotation inReplyTo) {
            this.inReplyTo = inReplyTo;
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.IRT, inReplyTo.GetPdfObject());
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetPopup(PdfPopupAnnotation popup) {
            this.popup = popup;
            popup.Put(PdfName.Parent, GetPdfObject());
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.Popup, popup.GetPdfObject());
        }

        public virtual PdfDictionary GetPopupObject() {
            return GetPdfObject().GetAsDictionary(PdfName.Popup);
        }

        public virtual PdfPopupAnnotation GetPopup() {
            return popup;
        }

        public virtual PdfString GetSubject() {
            return GetPdfObject().GetAsString(PdfName.Subj);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetSubject(PdfString subject) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.Subj, subject);
        }

        public virtual PdfName GetReplyType() {
            return GetPdfObject().GetAsName(PdfName.RT);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetReplyType(PdfName replyType) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.RT, replyType);
        }

        public virtual PdfName GetIntent() {
            return GetPdfObject().GetAsName(PdfName.IT);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetIntent(PdfName intent) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.IT, intent);
        }

        public virtual PdfDictionary GetExternalData() {
            return GetPdfObject().GetAsDictionary(PdfName.ExData);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetExternalData(PdfName exData) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.ExData, exData);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetRectangleDifferences(PdfArray rect) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.RD, rect);
        }

        public virtual PdfArray GetRectangleDifferences() {
            return GetPdfObject().GetAsArray(PdfName.RD);
        }

        public virtual PdfDictionary GetBorderEffect() {
            return GetPdfObject().GetAsDictionary(PdfName.BE);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetBorderEffect(PdfDictionary borderEffect) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.BE, borderEffect);
        }

        public virtual iText.Kernel.Color.Color GetInteriorColor() {
            PdfArray color = GetPdfObject().GetAsArray(PdfName.IC);
            if (color == null) {
                return null;
            }
            switch (color.Size()) {
                case 1: {
                    return new DeviceGray(color.GetAsNumber(0).FloatValue());
                }

                case 3: {
                    return new DeviceRgb(color.GetAsNumber(0).FloatValue(), color.GetAsNumber(1).FloatValue(), color.GetAsNumber
                        (2).FloatValue());
                }

                case 4: {
                    return new DeviceCmyk(color.GetAsNumber(0).FloatValue(), color.GetAsNumber(1).FloatValue(), color.GetAsNumber
                        (2).FloatValue(), color.GetAsNumber(3).FloatValue());
                }

                default: {
                    return null;
                }
            }
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetInteriorColor(PdfArray interiorColor) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.IC, interiorColor);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetInteriorColor(float[] interiorColor) {
            return SetInteriorColor(new PdfArray(interiorColor));
        }

        public virtual PdfName GetIconName() {
            return GetPdfObject().GetAsName(PdfName.Name);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetIconName(PdfName name) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.Name, name);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetDefaultAppearance(PdfString appearanceString) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.DA, appearanceString);
        }

        public virtual PdfString GetDefaultAppearance() {
            return GetPdfObject().GetAsString(PdfName.DA);
        }

        public virtual int GetJustification() {
            PdfNumber q = GetPdfObject().GetAsNumber(PdfName.Q);
            return q == null ? 0 : q.IntValue();
        }

        public virtual iText.Kernel.Pdf.Annot.PdfMarkupAnnotation SetJustification(int justification) {
            return (iText.Kernel.Pdf.Annot.PdfMarkupAnnotation)Put(PdfName.Q, new PdfNumber(justification));
        }
    }
}
