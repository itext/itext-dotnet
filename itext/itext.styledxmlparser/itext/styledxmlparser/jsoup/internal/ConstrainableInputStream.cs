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
using System.IO;
using System.Net.Sockets;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Internal {
    /// <summary>
    /// A jsoup internal class (so don't use it as there is no contract API) that enables constraints on an Input Stream,
    /// namely a maximum read size, and the ability to Thread.interrupt() the read.
    /// </summary>
    public sealed class ConstrainableInputStream : Stream {
        private Stream input;
        
        private const int DefaultSize = 1024 * 32;

        private readonly bool capped;

        private readonly int maxSize;

        private long startTime;

        private long timeout = 0;

        // optional max time of request
        private int remaining;

        private ConstrainableInputStream(Stream input, int bufferSize, int maxSize)
        {
            Validate.IsTrue(maxSize >= 0);
            this.input = new BufferedStream(input, bufferSize);
            this.maxSize = maxSize;
            remaining = maxSize;
            capped = maxSize != 0;
            startTime = System.DateTime.Now.Ticks;
        }

        /// <summary>If this InputStream is not already a ConstrainableInputStream, let it be one.</summary>
        /// <param name="input">the input stream to (maybe) wrap</param>
        /// <param name="bufferSize">the buffer size to use when reading</param>
        /// <param name="maxSize">the maximum size to allow to be read. 0 == infinite.</param>
        /// <returns>a constrainable input stream</returns>
        public static ConstrainableInputStream Wrap(Stream input, int bufferSize, int maxSize)
        {
            return input is ConstrainableInputStream
                ? (ConstrainableInputStream) input
                : new ConstrainableInputStream(input, bufferSize, maxSize);
        }

        public override int Read(byte[] b, int off, int len) {
            if (capped && remaining <= 0) {
                return -1;
            }
            
            if (Expired()) {
                throw new SocketException();
            }
            if (capped && len > remaining) {
                len = remaining;
            }
            // don't read more than desired, even if available
            try {
                int read = input.Read(b, off, len);
                remaining -= read;
                return read;
            }
            catch (SocketException) {
                return 0;
            }
        }

        /// <summary>Reads this inputstream to a ByteBuffer.</summary>
        /// <remarks>
        /// Reads this inputstream to a ByteBuffer. The supplied max may be less than the inputstream's max, to support
        /// reading just the first bytes.
        /// </remarks>
        public ByteBuffer ReadToByteBuffer(int max) {
            Validate.IsTrue(max >= 0, "maxSize must be 0 (unlimited) or larger");
            bool localCapped = max > 0;
            // still possibly capped input total stream
            int bufferSize = localCapped && max < DefaultSize ? max : DefaultSize;
            byte[] readBuffer = new byte[bufferSize];
            MemoryStream outStream = new MemoryStream(bufferSize);
            int read;
            int remaining = max;
            while (true) {
                read = input.Read(readBuffer);
                if (read == -1) {
                    break;
                }
                if (localCapped) {
                    // this local byteBuffer cap may be smaller than the overall maxSize (like when reading first bytes)
                    if (read >= remaining) {
                        outStream.Write(readBuffer, 0, remaining);
                        break;
                    }
                    remaining -= read;
                }
                outStream.Write(readBuffer, 0, read);
            }
            return ByteBuffer.Wrap(outStream.ToArray());
        }

        public iText.StyledXmlParser.Jsoup.Internal.ConstrainableInputStream Timeout(long startTimeNanos, long timeoutMillis
            ) {
            this.startTime = startTimeNanos;
            this.timeout = timeoutMillis * 1000000;
            return this;
        }

        private bool Expired() {
            if (timeout == 0) {
                return false;
            }
            long now = System.DateTime.Now.Ticks;
            long dur = now - startTime;
            return (dur > timeout);
        }

        public override void Flush()
        {
            input.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return input.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            input.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            input.Write(buffer, offset, count);
        }

        public override bool CanRead => input.CanRead;

        public override bool CanSeek => input.CanSeek;

        public override bool CanWrite => input.CanWrite;

        public override long Length => input.Length;

        public override long Position
        {
            get => input.Position;
            set
            {
                remaining = (int) (maxSize - value);
                input.Position = value;
            }
        }
    }
}
