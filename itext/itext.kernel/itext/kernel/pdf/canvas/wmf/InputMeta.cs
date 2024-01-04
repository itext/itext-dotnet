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
using System.IO;
using iText.IO.Util;
using iText.Kernel.Colors;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    /// <summary>Helper class to read nt, short, words, etc. from an InputStream.</summary>
    public class InputMeta {
        internal Stream @in;

        internal int length;

        /// <summary>Creates an InputMeta object.</summary>
        /// <param name="in">InputStream containing the WMF data</param>
        public InputMeta(Stream @in) {
            this.@in = @in;
        }

        /// <summary>Read the next word from the InputStream.</summary>
        /// <returns>the next word or 0 if the end of the stream has been reached</returns>
        public virtual int ReadWord() {
            length += 2;
            int k1 = @in.Read();
            if (k1 < 0) {
                return 0;
            }
            return (k1 + (@in.Read() << 8)) & 0xffff;
        }

        /// <summary>Read the next short from the InputStream.</summary>
        /// <returns>the next short value</returns>
        public virtual int ReadShort() {
            int k = ReadWord();
            if (k > 0x7fff) {
                k -= 0x10000;
            }
            return k;
        }

        /// <summary>Read the next int from the InputStream.</summary>
        /// <returns>the next int</returns>
        public virtual int ReadInt() {
            length += 4;
            int k1 = @in.Read();
            if (k1 < 0) {
                return 0;
            }
            int k2 = @in.Read() << 8;
            int k3 = @in.Read() << 16;
            return k1 + k2 + k3 + (@in.Read() << 24);
        }

        /// <summary>Read the next byte from the InputStream.</summary>
        /// <returns>the next byte</returns>
        public virtual int ReadByte() {
            ++length;
            return @in.Read() & 0xff;
        }

        /// <summary>Skips "len" amount of bytes from the InputStream.</summary>
        /// <remarks>Skips "len" amount of bytes from the InputStream. If len is &lt; 0, nothing is skipped.</remarks>
        /// <param name="len">amount of bytes needed to skip</param>
        public virtual void Skip(int len) {
            length += len;
            StreamUtil.Skip(@in, len);
        }

        /// <summary>Get the amount of bytes read and/or skipped from the InputStream.</summary>
        /// <returns>number of bytes read</returns>
        public virtual int GetLength() {
            return length;
        }

        /// <summary>
        /// Read the next
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// from the InputStream.
        /// </summary>
        /// <remarks>
        /// Read the next
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// from the InputStream. This reads 4 bytes.
        /// </remarks>
        /// <returns>the next Color</returns>
        public virtual Color ReadColor() {
            int red = ReadByte();
            int green = ReadByte();
            int blue = ReadByte();
            ReadByte();
            return new DeviceRgb(red, green, blue);
        }
    }
}
