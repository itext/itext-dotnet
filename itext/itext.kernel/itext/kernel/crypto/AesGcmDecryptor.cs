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

namespace iText.Kernel.Crypto {
    /// <summary>Class for decrypting aes-gcm encrypted bytes.</summary>
    public class AesGcmDecryptor : IDecryptor {
        private AESGCMCipher cipher;

        private readonly byte[] key;

        private bool initiated;

        private readonly byte[] iv = new byte[12];

        private int ivptr;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="AesGcmDecryptor"/>.
        /// </summary>
        /// <param name="key">the byte array containing the key for decryption</param>
        /// <param name="off">offset of the key in the byte array</param>
        /// <param name="len">the length of the key in the byte array</param>
        public AesGcmDecryptor(byte[] key, int off, int len) {
            this.key = new byte[len];
            Array.Copy(key, off, this.key, 0, len);
        }

        /// <summary>
        /// Continues a multiple-part decryption operation, processing another data part and initializing aes-gcm cipher if
        /// this method called for the first time.
        /// </summary>
        /// <param name="b">the input buffer</param>
        /// <param name="off">the offset in input where the input starts</param>
        /// <param name="len">the input length</param>
        /// <returns>decrypted bytes array</returns>
        public virtual byte[] Update(byte[] b, int off, int len) {
            if (!initiated) {
                int left = Math.Min(iv.Length - ivptr, len);
                Array.Copy(b, off, iv, ivptr, left);
                off += left;
                len -= left;
                ivptr += left;
                if (ivptr == iv.Length) {
                    cipher = new AESGCMCipher(false, key, iv);
                    initiated = true;
                }
                if (len == 0) {
                    return null;
                }
            }
            return cipher.Update(b, off, len);
        }

        /// <summary>Finishes a multiple-part decryption operation.</summary>
        /// <returns>input data that may have been buffered during a previous update operation</returns>
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
