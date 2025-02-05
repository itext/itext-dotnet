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
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Resolver.Resource {
    //\cond DO_NOT_DOCUMENT 
    /// <summary>
    /// Implementation of the
    /// <see cref="System.IO.Stream"/>
    /// abstract class, which is used to restrict
    /// reading bytes from input stream i.e. if more bytes are read than the readingByteLimit,
    /// an
    /// <see cref="ReadingByteLimitException"/>
    /// exception will be thrown.
    ///
    /// Note that the readingByteLimit is not taken into account in the <see cref="Seek"/>,
    /// <see cref="set_Position"/> methods.
    /// </summary>
    internal class LimitedInputStream : Stream {
        private bool isLimitViolated;

        private long readingByteLimit;

        private Stream inputStream;

        /// <summary>
        /// Creates a new
        /// <see cref="LimitedInputStream"/>
        /// instance.
        /// </summary>
        /// <param name="inputStream">the input stream, the reading of bytes from which will be limited</param>
        /// <param name="readingByteLimit">the reading byte limit, must not be less than zero</param>
        public LimitedInputStream(Stream inputStream, long readingByteLimit) {
            if (readingByteLimit < 0) {
                throw new ArgumentException(StyledXmlParserExceptionMessage.READING_BYTE_LIMIT_MUST_NOT_BE_LESS_ZERO);
            }
            this.isLimitViolated = false;
            this.inputStream = inputStream;
            this.readingByteLimit = readingByteLimit;
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (isLimitViolated) {
                throw new ReadingByteLimitException();
            }
            
            if (count > readingByteLimit) {
                if (readingByteLimit == 0) {
                    // Still need to test if end of stream is reached, so setting 1 byte to read 
                    count = 1;
                } else {
                    // Safe to cast to int, because count is int and greater
                    count = (int) readingByteLimit;
                }
            }

            int bytesRead = inputStream.Read(buffer, offset, count);
            readingByteLimit -= bytesRead;

            // If end of stream is met at the moment when readingByteLimit == 0 
            // we will not throw an exception, because readingByteLimit would not change 
            if (readingByteLimit < 0) {
                isLimitViolated = true;
                throw new ReadingByteLimitException();
            }
            
            return bytesRead;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                inputStream.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void Flush() {
            inputStream.Flush();
        }
        
        public override long Seek(long offset, SeekOrigin origin) {
            return inputStream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            inputStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            inputStream.Write(buffer, offset, count);
        }

        public override bool CanRead {
            get {
                return inputStream.CanRead;
            }
        }

        public override bool CanSeek {
            get {
                return inputStream.CanSeek;
            }
        }

        public override bool CanWrite {
            get {
                return inputStream.CanWrite;
            }
        }

        public override long Length {
            get {
                return inputStream.Length;
            }
        }

        public override long Position {
            get {
                return inputStream.Position;
            }
            set {
                inputStream.Position = value;
            }
        }
    }
   //\endcond 
}
