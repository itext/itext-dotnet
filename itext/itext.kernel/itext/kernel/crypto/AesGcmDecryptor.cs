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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Modes;
using iText.Commons.Bouncycastle.Security;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto {
    public class AesGcmDecryptor : IDecryptor {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private IGCMBlockCipher cipher;

        private readonly byte[] key;

        private bool initiated;

        private readonly byte[] iv = new byte[12];

        private int ivptr;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="AesDecryptor"/>
        /// </summary>
        /// <param name="key">the byte array containing the key for decryption</param>
        /// <param name="off">offset of the key in the byte array</param>
        /// <param name="len">the length of the key in the byte array</param>
        public AesGcmDecryptor(byte[] key, int off, int len) {
            this.key = new byte[len];
            Array.Copy(key, off, this.key, 0, len);
        }

        public virtual byte[] Update(byte[] b, int off, int len) {
            if (!initiated) {
                int left = Math.Min(iv.Length - ivptr, len);
                Array.Copy(b, off, iv, ivptr, left);
                off += left;
                len -= left;
                ivptr += left;
                if (ivptr == iv.Length) {
                    cipher = FACTORY.CreateGCMBlockCipher();
                    try {
                        cipher.Init(false, key, OutputStreamAesGcmEncryption.MAC_SIZE_BITS, iv);
                    }
                    catch (AbstractGeneralSecurityException e) {
                        throw new PdfException(e);
                    }
                    initiated = true;
                }
                if (len == 0) {
                    return null;
                }
            }
            byte[] plainText = new byte[cipher.GetUpdateOutputSize(len)];
            try {
                cipher.ProcessBytes(b, off, len, plainText, 0);
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(e);
            }
            return plainText;
        }

        public virtual byte[] Finish() {
            if (cipher != null) {
                byte[] plainText = new byte[cipher.GetOutputSize(0)];
                try {
                    cipher.DoFinal(plainText, 0);
                }
                catch (AbstractInvalidCipherTextException e) {
                    throw new PdfException(e);
                }
                catch (AbstractGeneralSecurityException e) {
                    throw new PdfException(e);
                }
                return plainText;
            }
            else {
                return null;
            }
        }
    }
}
