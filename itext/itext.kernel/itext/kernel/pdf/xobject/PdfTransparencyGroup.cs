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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Xobject {
    public class PdfTransparencyGroup : PdfObjectWrapper<PdfDictionary> {
        public PdfTransparencyGroup()
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.S, PdfName.Transparency);
        }

        /// <summary>Determining the initial backdrop against which its stack is composited.</summary>
        /// <param name="isolated">
        /// defines whether the
        /// <see cref="iText.Kernel.Pdf.PdfName.I"/>
        /// flag will be set or removed
        /// </param>
        public virtual void SetIsolated(bool isolated) {
            if (isolated) {
                GetPdfObject().Put(PdfName.I, PdfBoolean.TRUE);
            }
            else {
                GetPdfObject().Remove(PdfName.I);
            }
        }

        /// <summary>Determining whether the objects within the stack are composited with one another or only with the group's backdrop.
        ///     </summary>
        /// <param name="knockout">
        /// defines whether the
        /// <see cref="iText.Kernel.Pdf.PdfName.K"/>
        /// flag will be set or removed
        /// </param>
        public virtual void SetKnockout(bool knockout) {
            if (knockout) {
                GetPdfObject().Put(PdfName.K, PdfBoolean.TRUE);
            }
            else {
                GetPdfObject().Remove(PdfName.K);
            }
        }

        public virtual void SetColorSpace(PdfName colorSpace) {
            GetPdfObject().Put(PdfName.CS, colorSpace);
        }

        public virtual void SetColorSpace(PdfArray colorSpace) {
            GetPdfObject().Put(PdfName.CS, colorSpace);
        }

        public virtual iText.Kernel.Pdf.Xobject.PdfTransparencyGroup Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
