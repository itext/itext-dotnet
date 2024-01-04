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

namespace iText.Kernel.Crypto {
    public class OutputStreamStandardEncryption : OutputStreamEncryption {
        protected internal ARCFOUREncryption arcfour;

        /// <summary>Creates a new instance of OutputStreamStandardEncryption</summary>
        /// <param name="out">
        /// the
        /// <see cref="System.IO.Stream"/>
        /// to which data will be written
        /// </param>
        /// <param name="key">data to be written</param>
        /// <param name="off">the start offset in data</param>
        /// <param name="len">number of bytes to write</param>
        public OutputStreamStandardEncryption(Stream @out, byte[] key, int off, int len)
            : base(@out) {
            arcfour = new ARCFOUREncryption();
            arcfour.PrepareARCFOURKey(key, off, len);
        }

        public OutputStreamStandardEncryption(Stream @out, byte[] key)
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
            byte[] b2 = new byte[Math.Min(len, 4192)];
            while (len > 0) {
                int sz = Math.Min(len, b2.Length);
                arcfour.EncryptARCFOUR(b, off, sz, b2, 0);
                @out.Write(b2, 0, sz);
                len -= sz;
                off += sz;
            }
        }

        public override void Finish() {
        }
    }
}
