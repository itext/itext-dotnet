/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
