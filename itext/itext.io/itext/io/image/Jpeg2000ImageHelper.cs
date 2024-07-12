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
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Exceptions;
using iText.IO.Util;

namespace iText.IO.Image {
//\cond DO_NOT_DOCUMENT
    internal sealed class Jpeg2000ImageHelper {
        private class Jpeg2000Box {
//\cond DO_NOT_DOCUMENT
            internal int length;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int type;
//\endcond
        }

        private class ZeroBoxSizeException : System.IO.IOException {
//\cond DO_NOT_DOCUMENT
            internal ZeroBoxSizeException(String s)
                : base(s) {
            }
//\endcond
        }

        private const int JPIP_JPIP = 0x6a706970;

        private const int JP2_JP = 0x6a502020;

        private const int JP2_IHDR = 0x69686472;

        private const int JP2_FTYP = 0x66747970;

        private const int JP2_JP2H = 0x6a703268;

        private const int JP2_COLR = 0x636f6c72;

        private const int JP2_JP2C = 0x6a703263;

        private const int JP2_URL = 0x75726c20;

        private const int JP2_DBTL = 0x6474626c;

        private const int JP2_BPCC = 0x62706363;

        private const int JP2_JP2 = 0x6a703220;

        private const int JPX_JPXB = 0x6a707862;

        public static void ProcessImage(ImageData image) {
            if (image.GetOriginalType() != ImageType.JPEG2000) {
                throw new ArgumentException("JPEG2000 image expected");
            }
            ProcessParameters((Jpeg2000ImageData)image);
            image.SetFilter("JPXDecode");
        }

        /// <summary>This method checks if the image is a valid JPEG and processes some parameters.</summary>
        private static void ProcessParameters(Jpeg2000ImageData jp2) {
            jp2.parameters = new Jpeg2000ImageData.Parameters();
            try {
                if (jp2.GetData() == null) {
                    jp2.LoadData();
                }
                Stream jpeg2000Stream = new MemoryStream(jp2.GetData());
                Jpeg2000ImageHelper.Jpeg2000Box box = new Jpeg2000ImageHelper.Jpeg2000Box();
                box.length = Cio_read(4, jpeg2000Stream);
                if (box.length == 0x0000000c) {
                    jp2.parameters.isJp2 = true;
                    box.type = Cio_read(4, jpeg2000Stream);
                    if (JP2_JP != box.type) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.EXPECTED_JP_MARKER);
                    }
                    if (0x0d0a870a != Cio_read(4, jpeg2000Stream)) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.ERROR_WITH_JP_MARKER);
                    }
                    Jp2_read_boxhdr(box, jpeg2000Stream);
                    if (JP2_FTYP != box.type) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.EXPECTED_FTYP_MARKER);
                    }
                    StreamUtil.Skip(jpeg2000Stream, 8);
                    for (int i = 4; i < box.length / 4; ++i) {
                        if (Cio_read(4, jpeg2000Stream) == JPX_JPXB) {
                            jp2.parameters.isJpxBaseline = true;
                        }
                    }
                    Jp2_read_boxhdr(box, jpeg2000Stream);
                    do {
                        if (JP2_JP2H != box.type) {
                            if (box.type == JP2_JP2C) {
                                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.EXPECTED_JP2H_MARKER);
                            }
                            StreamUtil.Skip(jpeg2000Stream, box.length - 8);
                            Jp2_read_boxhdr(box, jpeg2000Stream);
                        }
                    }
                    while (JP2_JP2H != box.type);
                    Jp2_read_boxhdr(box, jpeg2000Stream);
                    if (JP2_IHDR != box.type) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.EXPECTED_IHDR_MARKER);
                    }
                    jp2.SetHeight(Cio_read(4, jpeg2000Stream));
                    jp2.SetWidth(Cio_read(4, jpeg2000Stream));
                    jp2.parameters.numOfComps = Cio_read(2, jpeg2000Stream);
                    jp2.SetBpc(Cio_read(1, jpeg2000Stream));
                    StreamUtil.Skip(jpeg2000Stream, 3);
                    Jp2_read_boxhdr(box, jpeg2000Stream);
                    if (box.type == JP2_BPCC) {
                        jp2.parameters.bpcBoxData = new byte[box.length - 8];
                        jpeg2000Stream.JRead(jp2.parameters.bpcBoxData, 0, box.length - 8);
                    }
                    else {
                        if (box.type == JP2_COLR) {
                            do {
                                if (jp2.parameters.colorSpecBoxes == null) {
                                    jp2.parameters.colorSpecBoxes = new List<Jpeg2000ImageData.ColorSpecBox>();
                                }
                                jp2.parameters.colorSpecBoxes.Add(Jp2_read_colr(box, jpeg2000Stream));
                                try {
                                    Jp2_read_boxhdr(box, jpeg2000Stream);
                                }
                                catch (Jpeg2000ImageHelper.ZeroBoxSizeException) {
                                }
                            }
                            //Probably we have reached the contiguous codestream box which is the last in jpeg2000 and has no length.
                            while (JP2_COLR == box.type);
                        }
                    }
                }
                else {
                    if (box.length == unchecked((int)(0xff4fff51))) {
                        StreamUtil.Skip(jpeg2000Stream, 4);
                        int x1 = Cio_read(4, jpeg2000Stream);
                        int y1 = Cio_read(4, jpeg2000Stream);
                        int x0 = Cio_read(4, jpeg2000Stream);
                        int y0 = Cio_read(4, jpeg2000Stream);
                        StreamUtil.Skip(jpeg2000Stream, 16);
                        jp2.SetColorEncodingComponentsNumber(Cio_read(2, jpeg2000Stream));
                        jp2.SetBpc(8);
                        jp2.SetHeight(y1 - y0);
                        jp2.SetWidth(x1 - x0);
                    }
                    else {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_JPEG2000_FILE);
                    }
                }
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.JPEG2000_IMAGE_EXCEPTION, e);
            }
        }

        private static Jpeg2000ImageData.ColorSpecBox Jp2_read_colr(Jpeg2000ImageHelper.Jpeg2000Box box, Stream jpeg2000Stream
            ) {
            int readBytes = 8;
            Jpeg2000ImageData.ColorSpecBox colorSpecBox = new Jpeg2000ImageData.ColorSpecBox();
            for (int i = 0; i < 3; i++) {
                colorSpecBox.Add(Cio_read(1, jpeg2000Stream));
                readBytes++;
            }
            if (colorSpecBox.GetMeth() == 1) {
                colorSpecBox.Add(Cio_read(4, jpeg2000Stream));
                readBytes += 4;
            }
            else {
                colorSpecBox.Add(0);
            }
            if (box.length - readBytes > 0) {
                byte[] colorProfile = new byte[box.length - readBytes];
                jpeg2000Stream.JRead(colorProfile, 0, box.length - readBytes);
                colorSpecBox.SetColorProfile(colorProfile);
            }
            return colorSpecBox;
        }

        private static void Jp2_read_boxhdr(Jpeg2000ImageHelper.Jpeg2000Box box, Stream jpeg2000Stream) {
            box.length = Cio_read(4, jpeg2000Stream);
            box.type = Cio_read(4, jpeg2000Stream);
            if (box.length == 1) {
                if (Cio_read(4, jpeg2000Stream) != 0) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CANNOT_HANDLE_BOX_SIZES_HIGHER_THAN_2_32
                        );
                }
                box.length = Cio_read(4, jpeg2000Stream);
                if (box.length == 0) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.UNSUPPORTED_BOX_SIZE_EQ_EQ_0);
                }
            }
            else {
                if (box.length == 0) {
                    throw new Jpeg2000ImageHelper.ZeroBoxSizeException("Unsupported box size == 0");
                }
            }
        }

        private static int Cio_read(int n, Stream jpeg2000Stream) {
            int v = 0;
            for (int i = n - 1; i >= 0; i--) {
                v += jpeg2000Stream.Read() << (i << 3);
            }
            return v;
        }
    }
//\endcond
}
