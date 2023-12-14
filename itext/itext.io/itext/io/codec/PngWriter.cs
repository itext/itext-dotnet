/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Source;

namespace iText.IO.Codec {
    /// <summary>Writes a PNG image.</summary>
    public class PngWriter {
        private static readonly byte[] PNG_SIGNTURE = new byte[] { (byte)137, 80, 78, 71, 13, 10, 26, 10 };

        private static readonly byte[] IHDR = ByteUtils.GetIsoBytes("IHDR");

        private static readonly byte[] PLTE = ByteUtils.GetIsoBytes("PLTE");

        private static readonly byte[] IDAT = ByteUtils.GetIsoBytes("IDAT");

        private static readonly byte[] IEND = ByteUtils.GetIsoBytes("IEND");

        private static readonly byte[] iCCP = ByteUtils.GetIsoBytes("iCCP");

        private static int[] crc_table;

        private Stream outp;

        public PngWriter(Stream outp) {
            this.outp = outp;
            outp.Write(PNG_SIGNTURE);
        }

        public virtual void WriteHeader(int width, int height, int bitDepth, int colorType) {
            MemoryStream ms = new MemoryStream();
            OutputInt(width, ms);
            OutputInt(height, ms);
            ms.Write(bitDepth);
            ms.Write(colorType);
            ms.Write(0);
            ms.Write(0);
            ms.Write(0);
            WriteChunk(IHDR, ms.ToArray());
        }

        public virtual void WriteEnd() {
            WriteChunk(IEND, new byte[0]);
        }

        public virtual void WriteData(byte[] data, int stride) {
            MemoryStream stream = new MemoryStream();
            DeflaterOutputStream zip = new DeflaterOutputStream(stream);
            int k;
            for (k = 0; k < data.Length - stride; k += stride) {
                zip.Write(0);
                zip.Write(data, k, stride);
            }
            int remaining = data.Length - k;
            if (remaining > 0) {
                zip.Write(0);
                zip.Write(data, k, remaining);
            }
            zip.Dispose();
            WriteChunk(IDAT, stream.ToArray());
        }

        public virtual void WritePalette(byte[] data) {
            WriteChunk(PLTE, data);
        }

        public virtual void WriteIccProfile(byte[] data) {
            MemoryStream stream = new MemoryStream();
            stream.Write((byte)'I');
            stream.Write((byte)'C');
            stream.Write((byte)'C');
            stream.Write(0);
            stream.Write(0);
            DeflaterOutputStream zip = new DeflaterOutputStream(stream);
            zip.Write(data);
            zip.Dispose();
            WriteChunk(iCCP, stream.ToArray());
        }

        private static void Make_crc_table() {
            if (crc_table != null) {
                return;
            }
            int[] crc2 = new int[256];
            for (int n = 0; n < 256; n++) {
                int c = n;
                for (int k = 0; k < 8; k++) {
                    if ((c & 1) != 0) {
                        c = (int)(unchecked((int)(0xedb88320)) ^ ((int)(((uint)c) >> 1)));
                    }
                    else {
                        c = (int)(((uint)c) >> 1);
                    }
                }
                crc2[n] = c;
            }
            crc_table = crc2;
        }

        private static int Update_crc(int crc, byte[] buf, int offset, int len) {
            int c = crc;
            if (crc_table == null) {
                Make_crc_table();
            }
            for (int n = 0; n < len; n++) {
                c = crc_table[(c ^ buf[n + offset]) & 0xff] ^ ((int)(((uint)c) >> 8));
            }
            return c;
        }

        private static int Crc(byte[] buf, int offset, int len) {
            return ~Update_crc(-1, buf, offset, len);
        }

        private static int Crc(byte[] buf) {
            return ~Update_crc(-1, buf, 0, buf.Length);
        }

        public virtual void OutputInt(int n) {
            OutputInt(n, outp);
        }

        public static void OutputInt(int n, Stream s) {
            s.Write((byte)(n >> 24));
            s.Write((byte)(n >> 16));
            s.Write((byte)(n >> 8));
            s.Write((byte)n);
        }

        public virtual void WriteChunk(byte[] chunkType, byte[] data) {
            OutputInt(data.Length);
            outp.Write(chunkType, 0, 4);
            outp.Write(data);
            int c = Update_crc(-1, chunkType, 0, chunkType.Length);
            c = ~Update_crc(c, data, 0, data.Length);
            OutputInt(c);
        }
    }
}
