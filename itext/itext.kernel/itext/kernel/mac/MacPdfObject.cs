/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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

namespace iText.Kernel.Mac {
//\cond DO_NOT_DOCUMENT
    internal class MacPdfObject : PdfObjectWrapper<PdfDictionary> {
//\cond DO_NOT_DOCUMENT
        internal MacPdfObject(int macContainerSize)
            : base(new PdfDictionary()) {
            PdfLiteral macPlaceholder = new PdfLiteral(macContainerSize);
            PdfLiteral byteRangePlaceholder = new PdfLiteral(80);
            GetPdfObject().Put(new PdfName("MACLocation"), new PdfName("Standalone"));
            GetPdfObject().Put(new PdfName("MAC"), macPlaceholder);
            GetPdfObject().Put(PdfName.ByteRange, byteRangePlaceholder);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual long[] ComputeByteRange(long totalLength) {
            PdfLiteral macPlaceholder = GetMacPlaceholder();
            long macStart = macPlaceholder.GetPosition();
            long macLength = macPlaceholder.GetBytesCount();
            long macEnd = macStart + macLength;
            return new long[] { 0, macStart, macEnd, totalLength - macEnd };
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual long GetByteRangePosition() {
            return GetByteRangePlaceholder().GetPosition();
        }
//\endcond

        private PdfLiteral GetMacPlaceholder() {
            PdfObject mac = GetPdfObject().Get(new PdfName("MAC"));
            return (PdfLiteral)mac;
        }

        private PdfLiteral GetByteRangePlaceholder() {
            PdfObject br = GetPdfObject().Get(PdfName.ByteRange);
            return (PdfLiteral)br;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
//\endcond
}
