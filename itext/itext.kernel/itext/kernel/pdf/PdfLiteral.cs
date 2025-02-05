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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    public class PdfLiteral : PdfPrimitiveObject {
        private long position;

        public PdfLiteral(byte[] content)
            : base(true) {
            this.content = content;
        }

        public PdfLiteral(int size)
            : this(new byte[size]) {
            JavaUtil.Fill(content, (byte)32);
        }

        public PdfLiteral(String content)
            : this(PdfEncodings.ConvertToBytes(content, null)) {
        }

        private PdfLiteral()
            : this((byte[])null) {
        }

        public override byte GetObjectType() {
            return LITERAL;
        }

        public override String ToString() {
            if (content != null) {
                return iText.Commons.Utils.JavaUtil.GetStringForBytes(content, iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
            }
            else {
                return "";
            }
        }

        public virtual long GetPosition() {
            return position;
        }

        public virtual void SetPosition(long position) {
            this.position = position;
        }

        public virtual int GetBytesCount() {
            return content.Length;
        }

        protected internal override void GenerateContent() {
        }

        public override bool Equals(Object o) {
            return this == o || o != null && GetType() == o.GetType() && JavaUtil.ArraysEquals(content, ((iText.Kernel.Pdf.PdfLiteral
                )o).content);
        }

        public override int GetHashCode() {
            return content == null ? 0 : JavaUtil.ArraysHashCode(content);
        }

        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfLiteral();
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
            iText.Kernel.Pdf.PdfLiteral literal = (iText.Kernel.Pdf.PdfLiteral)from;
            this.content = literal.GetInternalContent();
        }
    }
}
