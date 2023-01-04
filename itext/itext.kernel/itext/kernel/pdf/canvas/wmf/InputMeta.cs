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
