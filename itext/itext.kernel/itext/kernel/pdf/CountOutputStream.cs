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
using System.IO;

namespace iText.Kernel.Pdf {
    /// <summary>An <c>OutputStream</c> that counts the written bytes.</summary>
    /// <remarks>
    /// An <c>OutputStream</c> that counts the written bytes.
    /// You should not use same instance of this class in different threads as far as it's not thread safe.
    /// </remarks>
    public class CountOutputStream : Stream {
        private readonly Stream outputStream;

        private long amountOfWrittenBytes = 0;

        /// <summary>Creates an instance of output stream which counts written bytes.</summary>
        /// <param name="outputStream">
        /// inner
        /// <see cref="System.IO.Stream"/>
        /// </param>
        public CountOutputStream(Stream outputStream)
            : base() {
            this.outputStream = outputStream;
        }

        /// <summary><inheritDoc/></summary>
        public void Write(byte[] b) {
            outputStream.Write(b);
            amountOfWrittenBytes += b.Length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("You can't set position for CountOutputStream");
        }

        public override void SetLength(long value)
        {
            outputStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("You can't read from CountOutputStream");
        }

        /// <summary><inheritDoc/></summary>
        public override void Write(byte[] b, int off, int len) {
            outputStream.Write(b, off, len);
            amountOfWrittenBytes += len;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return outputStream.Length; }
        }

        public override long Position
        {
            get { return outputStream.Position; }
            set
            {
                throw new NotSupportedException("You can't set position for CountOutputStream");
            }
        }

        /// <summary><inheritDoc/></summary>
        public void Write(int b) {
            outputStream.Write(b);
            ++amountOfWrittenBytes;
        }

        /// <summary><inheritDoc/></summary>
        public override void Flush() {
            outputStream.Flush();
        }

        /// <summary><inheritDoc/></summary>
        public override void Close() {
            outputStream.Close();
        }

        /// <summary>Gets amount of bytes written to the inner output stream.</summary>
        /// <returns>amount of bytes</returns>
        public virtual long GetAmountOfWrittenBytes() {
            return amountOfWrittenBytes;
        }
    }
}
