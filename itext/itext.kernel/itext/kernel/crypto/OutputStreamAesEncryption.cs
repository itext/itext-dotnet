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
