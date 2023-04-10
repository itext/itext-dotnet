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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.IO.Exceptions;
using iText.IO.Font;
using iText.IO.Util;

namespace iText.IO.Image {
    public sealed class GifImageHelper {
        // max decoder pixel stack size
        internal const int MAX_STACK_SIZE = 4096;

        private class GifParameters {
            public GifParameters(GifImageData image) {
                this.image = image;
            }

            internal Stream input;

            // global color table used
            internal bool gctFlag;

            // background color index
            internal int bgIndex;

            // background color
            internal int bgColor;

            // pixel aspect ratio
            internal int pixelAspect;

            // local color table flag
            internal bool lctFlag;

            // interlace flag
            internal bool interlace;

            // local color table size
            internal int lctSize;

            // current image rectangle
            internal int ix;

            internal int iy;

            internal int iw;

            internal int ih;

            // current data block
            internal byte[] block = new byte[256];

            // block size
            internal int blockSize = 0;

            // last graphic control extension info
            // 0=no action; 1=leave in place; 2=restore to bg; 3=restore to prev
            internal int dispose = 0;

            // use transparent color
            internal bool transparency = false;

            // delay in milliseconds
            internal int delay = 0;

            // transparent color index
            internal int transIndex;

            // LZW decoder working arrays
            internal short[] prefix;

            internal byte[] suffix;

            internal byte[] pixelStack;

            internal byte[] pixels;

            internal byte[] m_out;

            internal int m_bpc;

            internal int m_gbpc;

            internal byte[] m_global_table;

            internal byte[] m_local_table;

            internal byte[] m_curr_table;

            internal int m_line_stride;

            internal byte[] fromData;

            internal Uri fromUrl;

            internal int currentFrame;

            internal GifImageData image;
        }

        /// <summary>Reads image source and fills GifImage object with parameters (frames, width, height)</summary>
        /// <param name="image">GifImage</param>
        public static void ProcessImage(GifImageData image) {
            ProcessImage(image, -1);
        }

        /// <summary>Reads image source and fills GifImage object with parameters (frames, width, height)</summary>
        /// <param name="image">GifImage</param>
        /// <param name="lastFrameNumber">the last frame of the gif image should be read</param>
        public static void ProcessImage(GifImageData image, int lastFrameNumber) {
            GifImageHelper.GifParameters gif = new GifImageHelper.GifParameters(image);
            Stream gifStream;
            try {
                if (image.GetData() == null) {
                    image.LoadData();
                }
                gifStream = new MemoryStream(image.GetData());
                Process(gifStream, gif, lastFrameNumber);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.GIF_IMAGE_EXCEPTION, e);
            }
        }

        private static void Process(Stream stream, GifImageHelper.GifParameters gif, int lastFrameNumber) {
            gif.input = stream;
            ReadHeader(gif);
            ReadContents(gif, lastFrameNumber);
            if (gif.currentFrame <= lastFrameNumber) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CANNOT_FIND_FRAME).SetMessageParams(lastFrameNumber
                    );
            }
        }

        /// <summary>Reads GIF file header information.</summary>
        private static void ReadHeader(GifImageHelper.GifParameters gif) {
            StringBuilder id = new StringBuilder("");
            for (int i = 0; i < 6; i++) {
                id.Append((char)gif.input.Read());
            }
            if (!id.ToString().StartsWith("GIF8")) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.GIF_SIGNATURE_NOT_FOUND);
            }
            ReadLSD(gif);
            if (gif.gctFlag) {
                gif.m_global_table = ReadColorTable(gif.m_gbpc, gif);
            }
        }

        /// <summary>Reads Logical Screen Descriptor</summary>
        private static void ReadLSD(GifImageHelper.GifParameters gif) {
            // logical screen size
            gif.image.SetLogicalWidth(ReadShort(gif));
            gif.image.SetLogicalHeight(ReadShort(gif));
            // packed fields
            int packed = gif.input.Read();
            // 1   : global color table flag
            gif.gctFlag = (packed & 0x80) != 0;
            gif.m_gbpc = (packed & 7) + 1;
            // background color index
            gif.bgIndex = gif.input.Read();
            // pixel aspect ratio
            gif.pixelAspect = gif.input.Read();
        }

        /// <summary>Reads next 16-bit value, LSB first</summary>
        private static int ReadShort(GifImageHelper.GifParameters gif) {
            // read 16-bit value, LSB first
            return gif.input.Read() | gif.input.Read() << 8;
        }

        /// <summary>Reads next variable length block from input.</summary>
        /// <returns>number of bytes stored in "buffer"</returns>
        private static int ReadBlock(GifImageHelper.GifParameters gif) {
            gif.blockSize = gif.input.Read();
            if (gif.blockSize <= 0) {
                return gif.blockSize = 0;
            }
            gif.blockSize = gif.input.JRead(gif.block, 0, gif.blockSize);
            return gif.blockSize;
        }

        private static byte[] ReadColorTable(int bpc, GifImageHelper.GifParameters gif) {
            int ncolors = 1 << bpc;
            int nbytes = 3 * ncolors;
            bpc = NewBpc(bpc);
            byte[] table = new byte[(1 << bpc) * 3];
            StreamUtil.ReadFully(gif.input, table, 0, nbytes);
            return table;
        }

        private static int NewBpc(int bpc) {
            switch (bpc) {
                case 1:
                case 2:
                case 4: {
                    break;
                }

                case 3: {
                    return 4;
                }

                default: {
                    return 8;
                }
            }
            return bpc;
        }

        private static void ReadContents(GifImageHelper.GifParameters gif, int lastFrameNumber) {
            // read GIF file content blocks
            bool done = false;
            gif.currentFrame = 0;
            while (!done) {
                int code = gif.input.Read();
                switch (code) {
                    case 0x2C: {
                        // image separator
                        ReadFrame(gif);
                        if (gif.currentFrame == lastFrameNumber) {
                            done = true;
                        }
                        gif.currentFrame++;
                        break;
                    }

                    case 0x21: {
                        // extension
                        code = gif.input.Read();
                        switch (code) {
                            case 0xf9: {
                                // graphics control extension
                                ReadGraphicControlExt(gif);
                                break;
                            }

                            case 0xff: {
                                // application extension
                                ReadBlock(gif);
                                // don't care
                                Skip(gif);
                                break;
                            }

                            default: {
                                // uninteresting extension
                                Skip(gif);
                                break;
                            }
                        }
                        break;
                    }

                    default: {
                        done = true;
                        break;
                    }
                }
            }
        }

        /// <summary>Reads next frame image</summary>
        private static void ReadFrame(GifImageHelper.GifParameters gif) {
            // (sub)image position & size
            gif.ix = ReadShort(gif);
            gif.iy = ReadShort(gif);
            gif.iw = ReadShort(gif);
            gif.ih = ReadShort(gif);
            int packed = gif.input.Read();
            // 1 - local color table flag
            gif.lctFlag = (packed & 0x80) != 0;
            // 2 - interlace flag
            gif.interlace = (packed & 0x40) != 0;
            // 3 - sort flag
            // 4-5 - reserved
            // 6-8 - local color table size
            gif.lctSize = 2 << (packed & 7);
            gif.m_bpc = NewBpc(gif.m_gbpc);
            if (gif.lctFlag) {
                // read table
                gif.m_curr_table = ReadColorTable((packed & 7) + 1, gif);
                gif.m_bpc = NewBpc((packed & 7) + 1);
            }
            else {
                gif.m_curr_table = gif.m_global_table;
            }
            if (gif.transparency && gif.transIndex >= gif.m_curr_table.Length / 3) {
                gif.transparency = false;
            }
            // Acrobat 5.05 doesn't like this combination
            if (gif.transparency && gif.m_bpc == 1) {
                byte[] tp = new byte[12];
                Array.Copy(gif.m_curr_table, 0, tp, 0, 6);
                gif.m_curr_table = tp;
                gif.m_bpc = 2;
            }
            // decode pixel data
            bool skipZero = DecodeImageData(gif);
            if (!skipZero) {
                Skip(gif);
            }
            try {
                Object[] colorspace = new Object[4];
                colorspace[0] = "/Indexed";
                colorspace[1] = "/DeviceRGB";
                int len = gif.m_curr_table.Length;
                colorspace[2] = len / 3 - 1;
                colorspace[3] = PdfEncodings.ConvertToString(gif.m_curr_table, null);
                IDictionary<String, Object> ad = new Dictionary<String, Object>();
                ad.Put("ColorSpace", colorspace);
                RawImageData img = new RawImageData(gif.m_out, ImageType.GIF);
                RawImageHelper.UpdateRawImageParameters(img, gif.iw, gif.ih, 1, gif.m_bpc, gif.m_out);
                RawImageHelper.UpdateImageAttributes(img, ad);
                gif.image.AddFrame(img);
                if (gif.transparency) {
                    img.SetTransparency(new int[] { gif.transIndex, gif.transIndex });
                }
            }
            catch (Exception e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.GIF_IMAGE_EXCEPTION, e);
            }
        }

        private static bool DecodeImageData(GifImageHelper.GifParameters gif) {
            int NullCode = -1;
            int npix = gif.iw * gif.ih;
            int available;
            int clear;
            int code_mask;
            int code_size;
            int end_of_information;
            int in_code;
            int old_code;
            int bits;
            int code;
            int count;
            int i;
            int datum;
            int data_size;
            int first;
            int top;
            int bi;
            bool skipZero = false;
            if (gif.prefix == null) {
                gif.prefix = new short[MAX_STACK_SIZE];
            }
            if (gif.suffix == null) {
                gif.suffix = new byte[MAX_STACK_SIZE];
            }
            if (gif.pixelStack == null) {
                gif.pixelStack = new byte[MAX_STACK_SIZE + 1];
            }
            gif.m_line_stride = (gif.iw * gif.m_bpc + 7) / 8;
            gif.m_out = new byte[gif.m_line_stride * gif.ih];
            int pass = 1;
            int inc = gif.interlace ? 8 : 1;
            int line = 0;
            int xpos = 0;
            //  Initialize GIF data stream decoder.
            data_size = gif.input.Read();
            clear = 1 << data_size;
            end_of_information = clear + 1;
            available = clear + 2;
            old_code = NullCode;
            code_size = data_size + 1;
            code_mask = (1 << code_size) - 1;
            for (code = 0; code < clear; code++) {
                gif.prefix[code] = 0;
                gif.suffix[code] = (byte)code;
            }
            //  Decode GIF pixel stream.
            datum = bits = count = first = top = bi = 0;
            for (i = 0; i < npix; ) {
                if (top == 0) {
                    if (bits < code_size) {
                        //  Load bytes until there are enough bits for a code.
                        if (count == 0) {
                            // Read a new data block.
                            count = ReadBlock(gif);
                            if (count <= 0) {
                                skipZero = true;
                                break;
                            }
                            bi = 0;
                        }
                        datum += (gif.block[bi] & 0xff) << bits;
                        bits += 8;
                        bi++;
                        count--;
                        continue;
                    }
                    //  Get the next code.
                    code = datum & code_mask;
                    datum >>= code_size;
                    bits -= code_size;
                    //  Interpret the code
                    if (code > available || code == end_of_information) {
                        break;
                    }
                    if (code == clear) {
                        //  Reset decoder.
                        code_size = data_size + 1;
                        code_mask = (1 << code_size) - 1;
                        available = clear + 2;
                        old_code = NullCode;
                        continue;
                    }
                    if (old_code == NullCode) {
                        gif.pixelStack[top++] = gif.suffix[code];
                        old_code = code;
                        first = code;
                        continue;
                    }
                    in_code = code;
                    if (code == available) {
                        gif.pixelStack[top++] = (byte)first;
                        code = old_code;
                    }
                    while (code > clear) {
                        gif.pixelStack[top++] = gif.suffix[code];
                        code = gif.prefix[code];
                    }
                    first = gif.suffix[code] & 0xff;
                    //  Add a new string to the string table,
                    if (available >= MAX_STACK_SIZE) {
                        break;
                    }
                    gif.pixelStack[top++] = (byte)first;
                    gif.prefix[available] = (short)old_code;
                    gif.suffix[available] = (byte)first;
                    available++;
                    if ((available & code_mask) == 0 && available < MAX_STACK_SIZE) {
                        code_size++;
                        code_mask += available;
                    }
                    old_code = in_code;
                }
                //  Pop a pixel off the pixel stack.
                top--;
                i++;
                SetPixel(xpos, line, gif.pixelStack[top], gif);
                ++xpos;
                if (xpos >= gif.iw) {
                    xpos = 0;
                    line += inc;
                    if (line >= gif.ih) {
                        if (gif.interlace) {
                            do {
                                pass++;
                                switch (pass) {
                                    case 2: {
                                        line = 4;
                                        break;
                                    }

                                    case 3: {
                                        line = 2;
                                        inc = 4;
                                        break;
                                    }

                                    case 4: {
                                        line = 1;
                                        inc = 2;
                                        break;
                                    }

                                    default: {
                                        // this shouldn't happen
                                        line = gif.ih - 1;
                                        inc = 0;
                                        break;
                                    }
                                }
                            }
                            while (line >= gif.ih);
                        }
                        else {
                            // this shouldn't happen
                            line = gif.ih - 1;
                            inc = 0;
                        }
                    }
                }
            }
            return skipZero;
        }

        private static void SetPixel(int x, int y, int v, GifImageHelper.GifParameters gif) {
            if (gif.m_bpc == 8) {
                int pos = x + gif.iw * y;
                gif.m_out[pos] = (byte)v;
            }
            else {
                int pos = gif.m_line_stride * y + x / (8 / gif.m_bpc);
                int vout = v << 8 - gif.m_bpc * (x % (8 / gif.m_bpc)) - gif.m_bpc;
                gif.m_out[pos] |= (byte)vout;
            }
        }

        /// <summary>Reads Graphics Control Extension values</summary>
        private static void ReadGraphicControlExt(GifImageHelper.GifParameters gif) {
            // block size
            gif.input.Read();
            // packed fields
            int packed = gif.input.Read();
            // disposal method
            gif.dispose = (packed & 0x1c) >> 2;
            if (gif.dispose == 0) {
                // elect to keep old image if discretionary
                gif.dispose = 1;
            }
            gif.transparency = (packed & 1) != 0;
            // delay in milliseconds
            gif.delay = ReadShort(gif) * 10;
            // transparent color index
            gif.transIndex = gif.input.Read();
            // block terminator
            gif.input.Read();
        }

        /// <summary>
        /// Skips variable length blocks up to and including
        /// next zero length block.
        /// </summary>
        private static void Skip(GifImageHelper.GifParameters gif) {
            do {
                ReadBlock(gif);
            }
            while (gif.blockSize > 0);
        }
    }
}
