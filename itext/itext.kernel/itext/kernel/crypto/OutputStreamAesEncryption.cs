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
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto {
    public class OutputStreamAesEncryption : OutputStreamEncryption {
        protected internal AESCipher cipher;

        private bool finished;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="OutputStreamAesEncryption"/>
        /// </summary>
        /// <param name="out">
        /// the
        /// <see cref="System.IO.Stream"/>
        /// instance to be used as the destination for the encrypted content
        /// </param>
        /// <param name="key">the byte array containing the key for encryption</param>
        /// <param name="off">offset of the key in the byte array</param>
        /// <param name="len">the length of the key in the byte array</param>
        public OutputStreamAesEncryption(Stream @out, byte[] key, int off, int len)
            : base(@out) {
            byte[] iv = IVGenerator.GetIV();
            byte[] nkey = new byte[len];
            Array.Copy(key, off, nkey, 0, len);
            cipher = new AESCipher(true, nkey, iv);
            try {
                Write(iv);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="OutputStreamAesEncryption"/>
        /// </summary>
        /// <param name="out">
        /// the
        /// <see cref="System.IO.Stream"/>
        /// instance to be used as the destination for the encrypted content
        /// </param>
        /// <param name="key">the byte array which is the key for encryption</param>
        public OutputStreamAesEncryption(Stream @out, byte[] key)
            : this(@out, key, 0, key.Length) {
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
        /// some of the bytes in the array
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
        /// <paramref name="b"/>
        /// is
        /// <see langword="null"/>
        /// , a
        /// <c>NullPointerException</c>
        /// is thrown.
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
        /// <param name="b">the data.</param>
        /// <param name="off">the start offset in the data.</param>
        /// <param name="len">the number of bytes to write.</param>
        public override void Write(byte[] b, int off, int len) {
            byte[] b2 = cipher.Update(b, off, len);
            if (b2 == null || b2.Length == 0) {
                return;
            }
            @out.Write(b2, 0, b2.Length);
        }

        public override void Finish() {
            if (!finished) {
                finished = true;
                byte[] b = cipher.DoFinal();
                try {
                    @out.Write(b, 0, b.Length);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
                }
            }
        }
    }
}
