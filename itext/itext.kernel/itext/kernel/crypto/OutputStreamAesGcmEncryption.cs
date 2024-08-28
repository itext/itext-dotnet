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
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto {
    /// <summary>
    /// An output stream accepts output bytes and sends them to underlying
    /// <see cref="OutputStreamEncryption"/>
    /// instance.
    /// </summary>
    public class OutputStreamAesGcmEncryption : OutputStreamEncryption {
        private readonly AESGCMCipher cipher;

        private bool finished;

        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="OutputStreamAesGcmEncryption"/>.
        /// </summary>
        /// <param name="out">
        /// the
        /// <see cref="System.IO.Stream"/>
        /// instance to be used as the destination for the encrypted content
        /// </param>
        /// <param name="key">the byte array containing the key for encryption</param>
        /// <param name="noncePart">a 7 byte nonce</param>
        public OutputStreamAesGcmEncryption(Stream @out, byte[] key, byte[] noncePart)
            : base(@out) {
            byte[] iv = new byte[12];
            byte[] randomPart = new byte[5];
            lock (rng) {
                rng.GetBytes(randomPart);
            }
            Array.Copy(randomPart, 0, iv, 0, 5);
            Array.Copy(noncePart, 0, iv, 5, 7);
            cipher = new AESGCMCipher(true, key, iv);
            try {
                @out.Write(iv);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
        }

        /// <summary>
        /// Writes
        /// <paramref name="len"/>
        /// bytes from the specified byte array
        /// starting at offset
        /// <paramref name="off"/>
        /// to this output stream.
        /// </summary>
        /// <remarks>
        /// Writes
        /// <paramref name="len"/>
        /// bytes from the specified byte array
        /// starting at offset
        /// <paramref name="off"/>
        /// to this output stream.
        /// The general contract for
        /// <c>write(b, off, len)</c>
        /// is that
        /// some bytes in the array
        /// <paramref name="b"/>
        /// are written to the
        /// output stream in order; element
        /// <c>b[off]</c>
        /// is the first
        /// byte written and
        /// <c>b[off+len-1]</c>
        /// is the last byte written
        /// by this operation.
        /// <para />
        /// The
        /// <c>write</c>
        /// method of
        /// <c>OutputStream</c>
        /// calls
        /// the write method of one argument on each of the bytes to be
        /// written out. Subclasses are encouraged to override this method and
        /// provide a more efficient implementation.
        /// <para />
        /// If
        /// <paramref name="off"/>
        /// is negative, or
        /// <paramref name="len"/>
        /// is negative, or
        /// <c>off+len</c>
        /// is greater than the length of the array
        /// <paramref name="b"/>
        /// , then an <tt>IndexOutOfBoundsException</tt> is thrown.
        /// </remarks>
        /// <param name="b">the data</param>
        /// <param name="off">the start offset in the data</param>
        /// <param name="len">the number of bytes to write</param>
        public override void Write(byte[] b, int off, int len) {
            byte[] cipherBuffer = cipher.Update(b, off, len);
            if (cipherBuffer.Length != 0) {
                @out.Write(cipherBuffer, 0, cipherBuffer.Length);
            }
        }

        /// <summary>Finishes and dispose all resources used for writing in encrypted stream.</summary>
        /// <remarks>
        /// Finishes and dispose all resources used for writing in encrypted stream.
        /// Input data that may have been buffered during a previous update operation is processed,
        /// with padding (if requested) being applied and authentication tag is appended.
        /// </remarks>
        public override void Finish() {
            if (!finished) {
                finished = true;
                byte[] cipherBuffer = cipher.DoFinal();
                try {
                    @out.Write(cipherBuffer, 0, cipherBuffer.Length);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
                }
            }
        }
    }
}
