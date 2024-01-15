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
