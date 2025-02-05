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
using System.Collections.Generic;
using System.IO;

namespace iText.IO.Codec {
    /// <summary>Exports images as TIFF.</summary>
    public class TiffWriter {
        private SortedDictionary<int, TiffWriter.FieldBase> ifd = new SortedDictionary<int, TiffWriter.FieldBase>(
            );

        public virtual void AddField(TiffWriter.FieldBase field) {
            ifd.Put(Convert.ToInt32(field.GetTag()), field);
        }

        public virtual int GetIfdSize() {
            return 6 + ifd.Count * 12;
        }

        public virtual void WriteFile(Stream stream) {
            stream.Write(0x4d);
            stream.Write(0x4d);
            stream.Write(0);
            stream.Write(42);
            WriteLong(8, stream);
            WriteShort(ifd.Count, stream);
            int offset = 8 + GetIfdSize();
            foreach (TiffWriter.FieldBase field in ifd.Values) {
                int size = field.GetValueSize();
                if (size > 4) {
                    field.SetOffset(offset);
                    offset += size;
                }
                field.WriteField(stream);
            }
            WriteLong(0, stream);
            foreach (TiffWriter.FieldBase field in ifd.Values) {
                field.WriteValue(stream);
            }
        }

        /// <summary>Inner class class containing information about a field.</summary>
        public abstract class FieldBase {
            private int tag;

            private int fieldType;

            private int count;

            protected internal byte[] data;

            private int offset;

            protected internal FieldBase(int tag, int fieldType, int count) {
                this.tag = tag;
                this.fieldType = fieldType;
                this.count = count;
            }

            public virtual int GetValueSize() {
                return (int)((data.Length + 1) & unchecked((int)(0xfffffffe)));
            }

            public virtual int GetTag() {
                return tag;
            }

            public virtual void SetOffset(int offset) {
                this.offset = offset;
            }

            public virtual void WriteField(Stream stream) {
                WriteShort(tag, stream);
                WriteShort(fieldType, stream);
                WriteLong(count, stream);
                if (data.Length <= 4) {
                    stream.Write(data);
                    for (int k = data.Length; k < 4; ++k) {
                        stream.Write(0);
                    }
                }
                else {
                    WriteLong(offset, stream);
                }
            }

            public virtual void WriteValue(Stream stream) {
                if (data.Length <= 4) {
                    return;
                }
                stream.Write(data);
                if ((data.Length & 1) == 1) {
                    stream.Write(0);
                }
            }
        }

        /// <summary>Inner class containing info about a field.</summary>
        public class FieldShort : TiffWriter.FieldBase {
            public FieldShort(int tag, int value)
                : base(tag, 3, 1) {
                data = new byte[2];
                data[0] = (byte)(value >> 8);
                data[1] = (byte)value;
            }

            public FieldShort(int tag, int[] values)
                : base(tag, 3, values.Length) {
                data = new byte[values.Length * 2];
                int ptr = 0;
                foreach (int value in values) {
                    data[ptr++] = (byte)(value >> 8);
                    data[ptr++] = (byte)value;
                }
            }
        }

        /// <summary>Inner class containing info about a field.</summary>
        public class FieldLong : TiffWriter.FieldBase {
            public FieldLong(int tag, int value)
                : base(tag, 4, 1) {
                data = new byte[4];
                data[0] = (byte)(value >> 24);
                data[1] = (byte)(value >> 16);
                data[2] = (byte)(value >> 8);
                data[3] = (byte)value;
            }

            public FieldLong(int tag, int[] values)
                : base(tag, 4, values.Length) {
                data = new byte[values.Length * 4];
                int ptr = 0;
                foreach (int value in values) {
                    data[ptr++] = (byte)(value >> 24);
                    data[ptr++] = (byte)(value >> 16);
                    data[ptr++] = (byte)(value >> 8);
                    data[ptr++] = (byte)value;
                }
            }
        }

        /// <summary>Inner class containing info about a field.</summary>
        public class FieldRational : TiffWriter.FieldBase {
            public FieldRational(int tag, int[] value)
                : this(tag, new int[][] { value }) {
            }

            public FieldRational(int tag, int[][] values)
                : base(tag, 5, values.Length) {
                data = new byte[values.Length * 8];
                int ptr = 0;
                foreach (int[] value in values) {
                    data[ptr++] = (byte)(value[0] >> 24);
                    data[ptr++] = (byte)(value[0] >> 16);
                    data[ptr++] = (byte)(value[0] >> 8);
                    data[ptr++] = (byte)value[0];
                    data[ptr++] = (byte)(value[1] >> 24);
                    data[ptr++] = (byte)(value[1] >> 16);
                    data[ptr++] = (byte)(value[1] >> 8);
                    data[ptr++] = (byte)value[1];
                }
            }
        }

        /// <summary>Inner class containing info about a field.</summary>
        public class FieldByte : TiffWriter.FieldBase {
            public FieldByte(int tag, byte[] values)
                : base(tag, 1, values.Length) {
                data = values;
            }
        }

        /// <summary>Inner class containing info about a field.</summary>
        public class FieldUndefined : TiffWriter.FieldBase {
            public FieldUndefined(int tag, byte[] values)
                : base(tag, 7, values.Length) {
                data = values;
            }
        }

        /// <summary>Inner class containing info about a field.</summary>
        public class FieldImage : TiffWriter.FieldBase {
            public FieldImage(byte[] values)
                : base(TIFFConstants.TIFFTAG_STRIPOFFSETS, 4, 1) {
                data = values;
            }
        }

        /// <summary>Inner class containing info about an ASCII field.</summary>
        public class FieldAscii : TiffWriter.FieldBase {
            public FieldAscii(int tag, String values)
                : base(tag, 2, values.GetBytes(System.Text.Encoding.ASCII).Length + 1) {
                byte[] b = values.GetBytes(System.Text.Encoding.ASCII);
                data = new byte[b.Length + 1];
                Array.Copy(b, 0, data, 0, b.Length);
            }
        }

        public static void WriteShort(int v, Stream stream) {
            stream.Write((v >> 8) & 0xff);
            stream.Write(v & 0xff);
        }

        public static void WriteLong(int v, Stream stream) {
            stream.Write((v >> 24) & 0xff);
            stream.Write((v >> 16) & 0xff);
            stream.Write((v >> 8) & 0xff);
            stream.Write(v & 0xff);
        }

        public static void CompressLZW(Stream stream, int predictor, byte[] b, int height, int samplesPerPixel, int
             stride) {
            LZWCompressor lzwCompressor = new LZWCompressor(stream, 8, true);
            bool usePredictor = predictor == TIFFConstants.PREDICTOR_HORIZONTAL_DIFFERENCING;
            if (!usePredictor) {
                lzwCompressor.Compress(b, 0, b.Length);
            }
            else {
                int off = 0;
                byte[] rowBuf = new byte[stride];
                for (int i = 0; i < height; i++) {
                    Array.Copy(b, off, rowBuf, 0, stride);
                    for (int j = stride - 1; j >= samplesPerPixel; j--) {
                        rowBuf[j] -= rowBuf[j - samplesPerPixel];
                    }
                    lzwCompressor.Compress(rowBuf, 0, stride);
                    off += stride;
                }
            }
            lzwCompressor.Flush();
        }
    }
}
