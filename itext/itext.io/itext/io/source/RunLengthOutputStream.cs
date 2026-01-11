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
    /// <c>RunLengthDecode</c>
    /// filter from the PDF specification.
    /// </summary>
    public class RunLengthOutputStream : FilterOutputStream, IFinishable {
        /// <summary>Maximum length of a run.</summary>
        /// <remarks>Maximum length of a run. Applies to both "unique" and repeating ones.</remarks>
        private const int MAX_LENGTH = 128;

        /// <summary>End Of Data marker.</summary>
        private const byte EOD = (byte)128;

        /// <summary>Buffer for storing the pending run.</summary>
        private readonly byte[] buffer = new byte[MAX_LENGTH];

        /// <summary>Value, that repeats in a repeating run.</summary>
        /// <remarks>
        /// Value, that repeats in a repeating run. Set to
        /// <c>-1</c>
        /// , when the
        /// pending run is a "unique" one.
        /// </remarks>
        private int repeatValue = -1;

        /// <summary>Current length of the pending run.</summary>
        private int currentLength = 0;

        /// <summary>
        /// Flag for detecting, whether
        /// <see cref="Finish()"/>
        /// has been called.
        /// </summary>
        private bool finished = false;

        /// <summary>
        /// Creates a new
        /// <c>RunLengthDecode</c>
        /// encoding stream.
        /// </summary>
        /// <param name="out">the output stream to write encoded data to</param>
        public RunLengthOutputStream(Stream @out)
            : base(@out) {
        }

        /// <summary><inheritDoc/></summary>
        public override void Write(int b) {
            int value = b & 0xFF;
            // Case for continuing a repeating run
            if (value == repeatValue) {
                ++currentLength;
                if (currentLength == MAX_LENGTH) {
                    WritePending();
                }
                return;
            }
            /*
            * If there was a repeating run, but we got a different value, then we
            * need to write the current repeating run we had and start a new
            * "unique" run.
            */
            if (repeatValue != -1) {
                WritePending();
                buffer[currentLength] = (byte)value;
                ++currentLength;
                return;
            }
            /*
            * As soon as we detect a sequence of 3 or more bytes, which are the
            * same, we need to switch to a repeating run. For this we will write
            * the values before the repeated one as a "unique" run and start a
            * new repeating run at length 3.
            *
            * Technically speaking we can switch to a repeating run at 2 bytes,
            * but in the vast majority of cases this will make the compression
            * ratio worse.
            */
            if (currentLength >= 2 && buffer[currentLength - 1] == (byte)value && buffer[currentLength - 2] == (byte)value
                ) {
                currentLength -= 2;
                WritePending();
                repeatValue = value;
                currentLength = 3;
                return;
            }
            // Just continuing (or starting) a "unique" run
            buffer[currentLength] = (byte)value;
            ++currentLength;
            if (currentLength == MAX_LENGTH) {
                WritePending();
            }
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
            WritePending();
            @out.Write(EOD);
            Flush();
        }

        private void WritePending() {
            if (currentLength <= 0) {
                return;
            }
            if (repeatValue < 0) {
                // Writing "unique" run
                @out.Write(currentLength - 1);
                @out.Write(buffer, 0, currentLength);
            }
            else {
                // Writing repeating run
                @out.Write(257 - currentLength);
                @out.Write(repeatValue);
            }
            ResetPending();
        }

        private void ResetPending() {
            repeatValue = -1;
            currentLength = 0;
        }
    }
}
