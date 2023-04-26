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
using System;
using System.Text;
using iText.IO.Font;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// A
    /// <c>PdfString</c>
    /// -class is the PDF-equivalent of a
    /// JAVA-
    /// <c>String</c>
    /// -object.
    /// </summary>
    /// <remarks>
    /// A
    /// <c>PdfString</c>
    /// -class is the PDF-equivalent of a
    /// JAVA-
    /// <c>String</c>
    /// -object.
    /// <para />
    /// A string is a sequence of characters delimited by parenthesis.
    /// If a string is too long to be conveniently placed on a single line, it may
    /// be split across multiple lines by using the backslash character (\) at the
    /// end of a line to indicate that the string continues on the following line.
    /// Within a string, the backslash character is used as an escape to specify
    /// unbalanced parenthesis, non-printing ASCII characters, and the backslash
    /// character itself. Use of the \<i>ddd</i> escape sequence is the preferred
    /// way to represent characters outside the printable ASCII character set.<br />
    /// This object is described in the 'Portable Document Format Reference Manual
    /// version 1.7' section 3.2.3 (page 53-56).
    /// </remarks>
    /// <seealso cref="PdfObject"/>
    public class PdfString : PdfPrimitiveObject {
        protected internal String value;

        protected internal String encoding;

        protected internal bool hexWriting = false;

        private int decryptInfoNum;

        private int decryptInfoGen;

        // if it's not null: content shall contain encrypted data; value shall be null
        private PdfEncryption decryption;

        public PdfString(String value, String encoding)
            : base() {
            System.Diagnostics.Debug.Assert(value != null);
            this.value = value;
            this.encoding = encoding;
        }

        public PdfString(String value)
            : this(value, null) {
        }

        public PdfString(byte[] content)
            : base() {
            if (content != null && content.Length > 0) {
                StringBuilder str = new StringBuilder(content.Length);
                foreach (byte b in content) {
                    str.Append((char)(b & 0xff));
                }
                this.value = str.ToString();
            }
            else {
                this.value = "";
            }
        }

        /// <summary>Only PdfReader can use this method!</summary>
        /// <param name="content">
        /// byte content the
        /// <see cref="PdfString"/>
        /// will be created from
        /// </param>
        /// <param name="hexWriting">boolean indicating if hex writing will be used</param>
        protected internal PdfString(byte[] content, bool hexWriting)
            : base(content) {
            this.hexWriting = hexWriting;
        }

        private PdfString()
            : base() {
        }

        public override byte GetObjectType() {
            return STRING;
        }

        public virtual bool IsHexWriting() {
            return hexWriting;
        }

        public virtual iText.Kernel.Pdf.PdfString SetHexWriting(bool hexWriting) {
            if (value == null) {
                GenerateValue();
            }
            content = null;
            this.hexWriting = hexWriting;
            return this;
        }

        public virtual String GetValue() {
            if (value == null) {
                GenerateValue();
            }
            return value;
        }

        /// <summary>Gets the encoding of this string.</summary>
        /// <returns>
        /// the name of the encoding specifying the byte representation of current
        /// <see cref="PdfString"/>
        /// value
        /// </returns>
        public virtual String GetEncoding() {
            return encoding;
        }

        /// <summary>
        /// Returns the Unicode
        /// <c>String</c>
        /// value of this
        /// <c>PdfString</c>
        /// -object.
        /// </summary>
        /// <returns>
        /// Unicode string value created by current
        /// <see cref="PdfString"/>
        /// object
        /// </returns>
        public virtual String ToUnicodeString() {
            if (encoding != null && encoding.Length != 0) {
                return GetValue();
            }
            if (content == null) {
                GenerateContent();
            }
            byte[] b = DecodeContent();
            if (b.Length >= 2 && b[0] == (byte)0xFE && b[1] == (byte)0xFF) {
                return PdfEncodings.ConvertToString(b, PdfEncodings.UNICODE_BIG);
            }
            else {
                if (b.Length >= 3 && b[0] == (byte)0xEF && b[1] == (byte)0xBB && b[2] == (byte)0xBF) {
                    return PdfEncodings.ConvertToString(b, PdfEncodings.UTF8);
                }
                else {
                    return PdfEncodings.ConvertToString(b, PdfEncodings.PDF_DOC_ENCODING);
                }
            }
        }

        /// <summary>Gets bytes of String-value considering encoding.</summary>
        /// <returns>byte array</returns>
        public virtual byte[] GetValueBytes() {
            // Analog of com.itextpdf.text.pdf.PdfString.getBytes() method in iText5.
            if (value == null) {
                GenerateValue();
            }
            if (encoding != null && PdfEncodings.UNICODE_BIG.Equals(encoding) && PdfEncodings.IsPdfDocEncoding(value)) {
                return PdfEncodings.ConvertToBytes(value, PdfEncodings.PDF_DOC_ENCODING);
            }
            else {
                return PdfEncodings.ConvertToBytes(value, encoding);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Pdf.PdfString that = (iText.Kernel.Pdf.PdfString)o;
            String v1 = GetValue();
            String v2 = that.GetValue();
            if (v1 != null && v1.Equals(v2)) {
                String e1 = GetEncoding();
                String e2 = that.GetEncoding();
                if ((e1 == null && e2 == null) || (e1 != null && e1.Equals(e2))) {
                    return true;
                }
            }
            return false;
        }

        public override String ToString() {
            if (value == null) {
                return iText.Commons.Utils.JavaUtil.GetStringForBytes(DecodeContent(), iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
            }
            else {
                return GetValue();
            }
        }

        public override int GetHashCode() {
            String v = GetValue();
            String e = GetEncoding();
            int result = v != null ? v.GetHashCode() : 0;
            return 31 * result + (e != null ? e.GetHashCode() : 0);
        }

        /// <summary>Marks this string object as not encrypted in the encrypted document.</summary>
        /// <remarks>
        /// Marks this string object as not encrypted in the encrypted document.
        /// <para />
        /// If it's marked so, it will be considered as already in plaintext and decryption will not be performed for it.
        /// In order to have effect, this method shall be called before
        /// <see cref="GetValue()"/>
        /// and
        /// <see cref="GetValueBytes()"/>
        /// methods.
        /// <para />
        /// NOTE: this method is only needed in a very specific cases of encrypted documents. E.g. digital signature dictionary
        /// /Contents entry shall not be encrypted. Also this method isn't meaningful in non-encrypted documents.
        /// </remarks>
        public virtual void MarkAsUnencryptedObject() {
            SetState(PdfObject.UNENCRYPTED);
        }

        internal virtual void SetDecryption(int decryptInfoNum, int decryptInfoGen, PdfEncryption decryption) {
            this.decryptInfoNum = decryptInfoNum;
            this.decryptInfoGen = decryptInfoGen;
            this.decryption = decryption;
        }

        protected internal virtual void GenerateValue() {
            System.Diagnostics.Debug.Assert(content != null, "No byte[] content to generate value");
            value = PdfEncodings.ConvertToString(DecodeContent(), null);
            if (decryption != null) {
                decryption = null;
                content = null;
            }
        }

        protected internal override void GenerateContent() {
            content = EncodeBytes(GetValueBytes());
        }

        /// <summary>
        /// Encrypt content of
        /// <c>value</c>
        /// and set as content.
        /// </summary>
        /// <remarks>
        /// Encrypt content of
        /// <c>value</c>
        /// and set as content.
        /// <c>generateContent()</c>
        /// won't be called.
        /// </remarks>
        /// <param name="encrypt">
        /// 
        /// <see cref="PdfEncryption"/>
        /// instance
        /// </param>
        /// <returns>true if value was encrypted, otherwise false.</returns>
        protected internal virtual bool Encrypt(PdfEncryption encrypt) {
            if (CheckState(PdfObject.UNENCRYPTED)) {
                return false;
            }
            if (encrypt != decryption) {
                if (decryption != null) {
                    GenerateValue();
                }
                if (encrypt != null && !encrypt.IsEmbeddedFilesOnly()) {
                    byte[] b = encrypt.EncryptByteArray(GetValueBytes());
                    content = EncodeBytes(b);
                    return true;
                }
            }
            return false;
        }

        protected internal virtual byte[] DecodeContent() {
            byte[] decodedBytes = PdfTokenizer.DecodeStringContent(content, hexWriting);
            if (decryption != null && !CheckState(PdfObject.UNENCRYPTED)) {
                decryption.SetHashKeyForNextObject(decryptInfoNum, decryptInfoGen);
                decodedBytes = decryption.DecryptByteArray(decodedBytes);
            }
            return decodedBytes;
        }

        /// <summary>Escape special symbols or convert to hexadecimal string.</summary>
        /// <remarks>
        /// Escape special symbols or convert to hexadecimal string.
        /// This method don't change either
        /// <c>value</c>
        /// or
        /// <c>content</c>
        /// ot the
        /// <c>PdfString</c>.
        /// </remarks>
        /// <param name="bytes">byte array to manipulate with.</param>
        /// <returns>Hexadecimal string or string with escaped symbols in byte array view.</returns>
        protected internal virtual byte[] EncodeBytes(byte[] bytes) {
            if (hexWriting) {
                ByteBuffer buf = new ByteBuffer(bytes.Length * 2);
                foreach (byte b in bytes) {
                    buf.AppendHex(b);
                }
                return buf.GetInternalBuffer();
            }
            else {
                ByteBuffer buf = StreamUtil.CreateBufferedEscapedString(bytes);
                return buf.ToByteArray(1, buf.Size() - 2);
            }
        }

        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfString();
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
            iText.Kernel.Pdf.PdfString @string = (iText.Kernel.Pdf.PdfString)from;
            value = @string.value;
            hexWriting = @string.hexWriting;
            decryption = @string.decryption;
            decryptInfoNum = @string.decryptInfoNum;
            decryptInfoGen = @string.decryptInfoGen;
            encoding = @string.encoding;
        }
    }
}
