/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.IO.Source {
    /// <summary>
    /// An output stream that encodes data according to the
    /// <c>ASCII85Decode</c>
    /// filter from the PDF specification.
    /// </summary>
    public class ASCII85OutputStream : FilterOutputStream, IFinishable {
        private const int BASE = 85;

        /// <summary>Offset to the first base-85 output char.</summary>
        private const int OFFSET = 33;

        /// <summary>Size of the encoding block.</summary>
        /// <remarks>
        /// Size of the encoding block. After this amount of bytes data is converted
        /// and flush to the backing stream.
        /// </remarks>
        private const int INPUT_LENGTH = 4;

        /// <summary>Amount of bytes produced from a block of input bytes.</summary>
        private const int OUTPUT_LENGTH = 5;

        /// <summary>Marker written, when all input bytes are zero.</summary>
        /// <remarks>
        /// Marker written, when all input bytes are zero. Not used for partial
        /// blocks.
        /// </remarks>
        private const byte ALL_ZEROS_MARKER = (byte)'z';

        /// <summary>End Of Data marker.</summary>
        private static readonly byte[] EOD = new byte[] { (byte)'~', (byte)'>' };

        /// <summary>Encoding block buffer.</summary>
        /// <remarks>Encoding block buffer. Reused for encoding output, when flushing.</remarks>
        private readonly byte[] buffer = new byte[OUTPUT_LENGTH];

        /// <summary>Bitwise OR of all bytes within the encoding block.</summary>
        /// <remarks>
        /// Bitwise OR of all bytes within the encoding block. Used to quickly
        /// check, whether the encoding block contains only zeros.
        /// </remarks>
        private int inputOr = 0;

        /// <summary>Input bytes cursor within the buffer.</summary>
        private int inputCursor = 0;

        /// <summary>
        /// Flag for detecting, whether
        /// <see cref="Finish()"/>
        /// has been called.
        /// </summary>
        private bool finished = false;

        /// <summary>
        /// Creates a new
        /// <c>ASCIIHexDecode</c>
        /// encoding stream.
        /// </summary>
        /// <param name="out">the output stream to write encoded data to</param>
        public ASCII85OutputStream(Stream @out)
            : base(@out) {
        }

        /// <summary><inheritDoc/></summary>
        public override void Write(int b) {
            int value = b & 0xFF;
            buffer[inputCursor] = (byte)value;
            inputOr |= value;
            ++inputCursor;
            WriteBufferIfFull();
        }

        /// <summary><inheritDoc/></summary>
        public override void Close() {
            Finish();
            base.Close();
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Finish() {
            if (finished) {
                return;
            }
            finished = true;
            // Writing the remainder
            if (inputCursor > 0) {
                if (inputOr == 0) {
                    // If all zeros, output is just n + 1 exclamation points
                    JavaUtil.Fill(buffer, 0, inputCursor + 1, (byte)'!');
                }
                else {
                    JavaUtil.Fill(buffer, inputCursor, INPUT_LENGTH, (byte)0);
                    ConvertBuffer();
                }
                @out.Write(buffer, 0, inputCursor + 1);
                ResetBuffer();
            }
            @out.Write(EOD);
            Flush();
        }

        private void WriteBufferIfFull() {
            if (inputCursor < INPUT_LENGTH) {
                return;
            }
            if (inputOr == 0) {
                // Special case, if all zeros
                @out.Write(ALL_ZEROS_MARKER);
            }
            else {
                ConvertBuffer();
                @out.Write(buffer);
            }
            ResetBuffer();
        }

        private void ResetBuffer() {
            inputOr = 0;
            inputCursor = 0;
        }

        private void ConvertBuffer() {
            long num = ((buffer[0] & 0xFFL) << 24) | ((buffer[1] & 0xFFL) << 16) | ((buffer[2] & 0xFFL) << 8) | (buffer
                [3] & 0xFFL);
            for (int i = OUTPUT_LENGTH - 1; i >= 0; --i) {
                buffer[i] = (byte)(OFFSET + (num % BASE));
                num /= BASE;
            }
        }
    }
}
