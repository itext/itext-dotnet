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
using System.IO;
using System.Security.Cryptography;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Modes;
using iText.Commons.Bouncycastle.Security;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto {
    public class OutputStreamAesGcmEncryption : OutputStreamEncryption {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly IGCMBlockCipher cipher;

        private bool finished;

        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public const int MAC_SIZE_BITS = 128;

        public OutputStreamAesGcmEncryption(Stream @out, byte[] key, byte[] noncePart)
            : base(@out) {
            byte[] iv = new byte[12];
            byte[] randomPart = new byte[5];
            lock (rng) {
                rng.GetBytes(randomPart);
            }
            Array.Copy(randomPart, 0, iv, 0, 5);
            Array.Copy(noncePart, 0, iv, 5, 7);
            cipher = FACTORY.CreateGCMBlockCipher();
            try {
                cipher.Init(true, key, MAC_SIZE_BITS, iv);
                @out.Write(iv);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
        }

        public override void Write(byte[] b, int off, int len) {
            int outputLen = cipher.GetUpdateOutputSize(len);
            byte[] cipherBuffer = new byte[outputLen];
            try {
                cipher.ProcessBytes(b, off, len, cipherBuffer, 0);
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            if (outputLen != 0) {
                @out.Write(cipherBuffer, 0, outputLen);
            }
        }

        public override void Finish() {
            if (!finished) {
                finished = true;
                byte[] cipherBuffer = new byte[cipher.GetOutputSize(0)];
                try {
                    cipher.DoFinal(cipherBuffer, 0);
                    @out.Write(cipherBuffer, 0, cipherBuffer.Length);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
                }
                catch (AbstractInvalidCipherTextException e) {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
                }
                catch (AbstractGeneralSecurityException e) {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
                }
            }
        }
    }
}
