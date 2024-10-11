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
using iText.Commons.Bouncycastle.Crypto.Modes;
using iText.Commons.Bouncycastle.Security;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto {
    /// <summary>Creates an Advanced Encryption Standard-Galois/Counter Mode (AES-GCM) Cipher.</summary>
    public class AESGCMCipher {
        public const int MAC_SIZE_BITS = 128;

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly IGCMBlockCipher cipher;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="AESGCMCipher"/>.
        /// </summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        /// <param name="iv">initialization vector to be used in cipher</param>
        public AESGCMCipher(bool forEncryption, byte[] key, byte[] iv) {
            try {
                cipher = BOUNCY_CASTLE_FACTORY.CreateGCMBlockCipher();
                cipher.Init(forEncryption, key, MAC_SIZE_BITS, iv);
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.ERROR_WHILE_INITIALIZING_AES_CIPHER, e);
            }
        }

        /// <summary>
        /// Continues a multiple-part encryption or decryption operation
        /// (depending on how this cipher was initialized), processing another data
        /// part.
        /// </summary>
        /// <remarks>
        /// Continues a multiple-part encryption or decryption operation
        /// (depending on how this cipher was initialized), processing another data
        /// part.
        /// <para />
        /// The first
        /// <paramref name="len"/>
        /// bytes in the
        /// <paramref name="b"/>
        /// input buffer, starting at
        /// <paramref name="off"/>
        /// offset inclusive,
        /// are processed, and the result is stored in a new buffer.
        /// </remarks>
        /// <param name="b">the input buffer</param>
        /// <param name="off">
        /// the offset in
        /// <paramref name="b"/>
        /// where the input starts
        /// </param>
        /// <param name="len">the input length</param>
        /// <returns>the new buffer with the result</returns>
        public virtual byte[] Update(byte[] b, int off, int len) {
            byte[] cipherBuffer = new byte[cipher.GetUpdateOutputSize(len)];
            try {
                cipher.ProcessBytes(b, off, len, cipherBuffer, 0);
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            return cipherBuffer;
        }

        /// <summary>
        /// Finishes a multiple-part encryption or decryption operation, depending on how this cipher was initialized
        /// and resets underlying cipher object to the state it was in when previously
        /// initialized via a call to init.
        /// </summary>
        /// <returns>final bytes array</returns>
        public virtual byte[] DoFinal() {
            byte[] cipherBuffer = new byte[cipher.GetOutputSize(0)];
            try {
                cipher.DoFinal(cipherBuffer, 0);
                return cipherBuffer;
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            catch (ArgumentException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
        }
    }
}
