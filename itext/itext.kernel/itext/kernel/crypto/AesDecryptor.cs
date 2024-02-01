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
using System;

namespace iText.Kernel.Crypto {
    public class AesDecryptor : IDecryptor {
        private AESCipher cipher;

        private byte[] key;

        private bool initiated;

        private byte[] iv = new byte[16];

        private int ivptr;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="AesDecryptor"/>
        /// </summary>
        /// <param name="key">the byte array containing the key for decryption</param>
        /// <param name="off">offset of the key in the byte array</param>
        /// <param name="len">the length of the key in the byte array</param>
        public AesDecryptor(byte[] key, int off, int len) {
            this.key = new byte[len];
            Array.Copy(key, off, this.key, 0, len);
        }

        public virtual byte[] Update(byte[] b, int off, int len) {
            if (initiated) {
                return cipher.Update(b, off, len);
            }
            else {
                int left = Math.Min(iv.Length - ivptr, len);
                Array.Copy(b, off, iv, ivptr, left);
                off += left;
                len -= left;
                ivptr += left;
                if (ivptr == iv.Length) {
                    cipher = new AESCipher(false, key, iv);
                    initiated = true;
                    if (len > 0) {
                        return cipher.Update(b, off, len);
                    }
                }
                return null;
            }
        }

        public virtual byte[] Finish() {
            if (cipher != null) {
                return cipher.DoFinal();
            }
            else {
                return null;
            }
        }
    }
}
