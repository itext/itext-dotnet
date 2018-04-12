/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.IO.Font;
using iText.IO.Util;

namespace iText.IO.Image {
    public sealed class GifImageHelper {
        internal const int MAX_STACK_SIZE = 4096;

        private class GifParameters {
            public GifParameters(GifImageData image) {
                // max decoder pixel stack size
                this.image = image;
            }

            internal Stream input;

            internal bool gctFlag;

            internal int bgIndex;

            internal int bgColor;

            internal int pixelAspect;

            internal bool lctFlag;

            internal bool interlace;

            internal int lctSize;

            internal int ix;

            internal int iy;

            internal int iw;

            internal int ih;

            internal byte[] block = new byte[256];

            internal int blockSize = 0;

            internal int dispose = 0;

            internal bool transparency = false;

            internal int delay = 0;

            internal int transIndex;

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
            // global color table used
            // background color index
            // background color
            // pixel aspect ratio
            // local color table flag
            // interlace flag
            // local color table size
            // current image rectangle
            // current data block
            // block size
            // last graphic control extension info
            // 0=no action; 1=leave in place; 2=restore to bg; 3=restore to prev
            // use transparent color
            // delay in milliseconds
            // transparent color index
            // LZW decoder working arrays
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
                throw new iText.IO.IOException(iText.IO.IOException.GifImageException, e);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static void Process(Stream stream, GifImageHelper.GifParameters gif, int lastFrameNumber) {
            gif.input = stream;
            ReadHeader(gif);
            ReadContents(gif, lastFrameNumber);
            if (gif.currentFrame <= lastFrameNumber) {
                throw new iText.IO.IOException(iText.IO.IOException.CannotFind1Frame).SetMessageParams(lastFrameNumber);
            }
        }

        /// <summary>Reads GIF file header information.</summary>
        /// <exception cref="System.IO.IOException"/>
        private static void ReadHeader(GifImageHelper.GifParameters gif) {
            StringBuilder id = new StringBuilder("");
            for (int i = 0; i < 6; i++) {
                id.Append((char)gif.input.Read());
            }
            if (!id.ToString().StartsWith("GIF8")) {
                throw new iText.IO.IOException(iText.IO.IOException.GifSignatureNotFound);
            }
            ReadLSD(gif);
            if (gif.gctFlag) {
                gif.m_global_table = ReadColorTable(gif.m_gbpc, gif);
            }
        }

        /// <summary>Reads Logical Screen Descriptor</summary>
        /// <exception cref="System.IO.IOException"/>
        private static void ReadLSD(GifImageHelper.GifParameters gif) {
            // logical screen size
            gif.image.SetLogicalWidth(ReadShort(gif));
            gif.image.SetLogicalHeight(ReadShort(gif));
            // packed fields
            int packed = gif.input.Read();
            gif.gctFlag = (packed & 0x80) != 0;
            // 1   : global color table flag
            gif.m_gbpc = (packed & 7) + 1;
            gif.bgIndex = gif.input.Read();
            // background color index
            gif.pixelAspect = gif.input.Read();
        }

        // pixel aspect ratio
        /// <summary>Reads next 16-bit value, LSB first</summary>
        /// <exception cref="System.IO.IOException"/>
        private static int ReadShort(GifImageHelper.GifParameters gif) {
            // read 16-bit value, LSB first
            return gif.input.Read() | gif.input.Read() << 8;
        }

        /// <summary>Reads next variable length block from input.</summary>
        /// <returns>number of bytes stored in "buffer"</returns>
        /// <exception cref="System.IO.IOException"/>
        private static int ReadBlock(GifImageHelper.GifParameters gif) {
            gif.blockSize = gif.input.Read();
            if (gif.blockSize <= 0) {
                return gif.blockSize = 0;
            }
            gif.blockSize = gif.input.JRead(gif.block, 0, gif.blockSize);
            return gif.blockSize;
        }

        /// <exception cref="System.IO.IOException"/>
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

        /// <exception cref="System.IO.IOException"/>
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
                                Skip(gif);
                                // don't care
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
        /// <exception cref="System.IO.IOException"/>
        private static void ReadFrame(GifImageHelper.GifParameters gif) {
            gif.ix = ReadShort(gif);
            // (sub)image position & size
            gif.iy = ReadShort(gif);
            gif.iw = ReadShort(gif);
            gif.ih = ReadShort(gif);
            int packed = gif.input.Read();
            gif.lctFlag = (packed & 0x80) != 0;
            // 1 - local color table flag
            gif.interlace = (packed & 0x40) != 0;
            // 2 - interlace flag
            // 3 - sort flag
            // 4-5 - reserved
            gif.lctSize = 2 << (packed & 7);
            // 6-8 - local color table size
            gif.m_bpc = NewBpc(gif.m_gbpc);
            if (gif.lctFlag) {
                gif.m_curr_table = ReadColorTable((packed & 7) + 1, gif);
                // read table
                gif.m_bpc = NewBpc((packed & 7) + 1);
            }
            else {
                gif.m_curr_table = gif.m_global_table;
            }
            if (gif.transparency && gif.transIndex >= gif.m_curr_table.Length / 3) {
                gif.transparency = false;
            }
            if (gif.transparency && gif.m_bpc == 1) {
                // Acrobat 5.05 doesn't like this combination
                byte[] tp = new byte[12];
                Array.Copy(gif.m_curr_table, 0, tp, 0, 6);
                gif.m_curr_table = tp;
                gif.m_bpc = 2;
            }
            bool skipZero = DecodeImageData(gif);
            // decode pixel data
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
                throw new iText.IO.IOException(iText.IO.IOException.GifImageException, e);
            }
        }

        /// <exception cref="System.IO.IOException"/>
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
                            line = gif.ih - 1;
                            // this shouldn't happen
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
        /// <exception cref="System.IO.IOException"/>
        private static void ReadGraphicControlExt(GifImageHelper.GifParameters gif) {
            gif.input.Read();
            // block size
            int packed = gif.input.Read();
            // packed fields
            gif.dispose = (packed & 0x1c) >> 2;
            // disposal method
            if (gif.dispose == 0) {
                gif.dispose = 1;
            }
            // elect to keep old image if discretionary
            gif.transparency = (packed & 1) != 0;
            gif.delay = ReadShort(gif) * 10;
            // delay in milliseconds
            gif.transIndex = gif.input.Read();
            // transparent color index
            gif.input.Read();
        }

        // block terminator
        /// <summary>
        /// Skips variable length blocks up to and including
        /// next zero length block.
        /// </summary>
        /// <exception cref="System.IO.IOException"/>
        private static void Skip(GifImageHelper.GifParameters gif) {
            do {
                ReadBlock(gif);
            }
            while (gif.blockSize > 0);
        }
    }
}
