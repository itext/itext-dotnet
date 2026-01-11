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
    /// <c>ASCIIHexDecode</c>
    /// filter from the PDF specification.
    /// </summary>
    public class ASCIIHexOutputStream : FilterOutputStream, IFinishable {
        /// <summary>End Of Data marker.</summary>
        private const byte EOD = (byte)'>';

        /// <summary>
        /// Array for mapping nibble values to the corresponding lowercase
        /// hexadecimal characters.
        /// </summary>
        private static readonly byte[] CHAR_MAP = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4'
            , (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (
            byte)'e', (byte)'f' };

        /// <summary>Buffer for storing the output hex char pair.</summary>
        private readonly byte[] buffer = new byte[2];

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
        public ASCIIHexOutputStream(Stream @out)
            : base(@out) {
        }

        /// <summary><inheritDoc/></summary>
        public override void Write(int b) {
            int value = (b & 0xFF);
            // Writing via a 2-elem buffer, in case `write(byte[])` on the
            // underlying stream is more performant
            buffer[0] = CHAR_MAP[value >> 4];
            buffer[1] = CHAR_MAP[value & 0x0F];
            @out.Write(buffer);
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
            @out.Write(EOD);
            Flush();
        }
    }
}
