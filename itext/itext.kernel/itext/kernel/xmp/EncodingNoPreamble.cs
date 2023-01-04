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

using System.Text;

namespace iText.Kernel.XMP {

    /// <summary>
    /// A wrapper for an Encoding to suppress the preamble.
    /// </summary>
    public class EncodingNoPreamble : Encoding {

        private Encoding encoding;
        private static byte[] emptyPreamble = new byte[0];

        public EncodingNoPreamble(Encoding encoding) {
            this.encoding = encoding;
        }
    
        public override int GetByteCount(char[] chars, int index, int count) {
            return encoding.GetByteCount(chars, index, count);
        }
    
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) {
            return encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }
    
        public override int GetCharCount(byte[] bytes, int index, int count) {
            return encoding.GetCharCount(bytes, index, count);
        }
    
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) {
            return encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }
    
        public override int GetMaxByteCount(int charCount) {
            return encoding.GetMaxByteCount(charCount);
        }
    
        public override int GetMaxCharCount(int byteCount) {
            return encoding.GetMaxCharCount(byteCount);
        }

        public override string BodyName {
            get {
                return encoding.BodyName;
            }
        }

        public override string HeaderName {
            get {
                return encoding.HeaderName;
            }
        }
    
        public override bool IsBrowserDisplay {
            get {
                return encoding.IsBrowserDisplay;
            }
        }
    
        public override bool IsBrowserSave {
            get {
                return encoding.IsBrowserSave;
            }
        }
    
        public override bool IsMailNewsDisplay {
            get {
                return encoding.IsMailNewsDisplay;
            }
        }
    
        public override bool IsMailNewsSave {
            get {
                return encoding.IsMailNewsSave;
            }
        }

        public override int WindowsCodePage {
            get {
                return encoding.WindowsCodePage;
            }
        }
    

        public override int CodePage {
            get {
                return encoding.CodePage;
            }
        }
    
        public override string EncodingName {
            get {
                return encoding.EncodingName;
            }
        }
    

        public override string WebName {
            get {
                return encoding.WebName;
            }
        }
    
        
        public override Decoder GetDecoder() {
            return encoding.GetDecoder ();
        }
    
        public override Encoder GetEncoder() {
            return encoding.GetEncoder ();
        }
    
        public override byte[] GetPreamble() {
            return emptyPreamble;
        }
    }
}
