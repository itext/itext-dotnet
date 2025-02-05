/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System;
using iText.IO.Source;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>Representation of the null object in the PDF specification.</summary>
    public class PdfNull : PdfPrimitiveObject {
        public static readonly iText.Kernel.Pdf.PdfNull PDF_NULL = new iText.Kernel.Pdf.PdfNull(true);

        private static readonly byte[] NullContent = ByteUtils.GetIsoBytes("null");

        /// <summary>Creates a PdfNull instance.</summary>
        public PdfNull()
            : base() {
        }

        private PdfNull(bool directOnly)
            : base(directOnly) {
        }

        public override byte GetObjectType() {
            return NULL;
        }

        public override String ToString() {
            return "null";
        }

        protected internal override void GenerateContent() {
            content = NullContent;
        }

        //Here we create new object, because if we use static object it can cause unpredictable behavior during copy objects
        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfNull();
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
        }

        public override bool Equals(Object obj) {
            return this == obj || obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode() {
            return 0;
        }
    }
}
