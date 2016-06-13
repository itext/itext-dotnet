/*
* Copyright 2003-2012 by Paulo Soares.
*
* This code was originally released in 2001 by SUN (see class
* com.sun.media.imageioimpl.plugins.tiff.TIFFFaxDecompressor.java)
* using the BSD license in a specific wording. In a mail dating from
* January 23, 2008, Brian Burkhalter (@sun.com) gave us permission
* to use the code under the following version of the BSD license:
*
* Copyright (c) 2005 Sun Microsystems, Inc. All  Rights Reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions
* are met:
*
* - Redistribution of source code must retain the above copyright
*   notice, this  list of conditions and the following disclaimer.
*
* - Redistribution in binary form must reproduce the above copyright
*   notice, this list of conditions and the following disclaimer in
*   the documentation and/or other materials provided with the
*   distribution.
*
* Neither the name of Sun Microsystems, Inc. or the names of
* contributors may be used to endorse or promote products derived
* from this software without specific prior written permission.
*
* This software is provided "AS IS," without a warranty of any
* kind. ALL EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND
* WARRANTIES, INCLUDING ANY IMPLIED WARRANTY OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE OR NON-INFRINGEMENT, ARE HEREBY
* EXCLUDED. SUN MIDROSYSTEMS, INC. ("SUN") AND ITS LICENSORS SHALL
* NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF
* USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
* DERIVATIVES. IN NO EVENT WILL SUN OR ITS LICENSORS BE LIABLE FOR
* ANY LOST REVENUE, PROFIT OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL,
* CONSEQUENTIAL, INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND
* REGARDLESS OF THE THEORY OF LIABILITY, ARISING OUT OF THE USE OF OR
* INABILITY TO USE THIS SOFTWARE, EVEN IF SUN HAS BEEN ADVISED OF THE
* POSSIBILITY OF SUCH DAMAGES.
*
* You acknowledge that this software is not designed or intended for
* use in the design, construction, operation or maintenance of any
* nuclear facility.
*/
namespace iTextSharp.IO.Codec {
    /// <summary>Class that can decode TIFF files.</summary>
    public class TIFFFaxDecoder {
        private int bitPointer;

        private int bytePointer;

        private byte[] data;

        private int w;

        private int h;

        private int fillOrder;

        private int changingElemSize = 0;

        private int[] prevChangingElems;

        private int[] currChangingElems;

        private int lastChangingElement = 0;

        private int compression = 2;

        private int uncompressedMode = 0;

        private int fillBits = 0;

        private int oneD;

        private bool recoverFromImageError;

        internal static int[] table1 = new int[] { 0x00, 0x01, 0x03, 0x07, 0x0f, 0x1f, 0x3f, 0x7f, 0xff };

        internal static int[] table2 = new int[] { 0x00, 0x80, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc, 0xfe, 0xff };

        public static byte[] flipTable = new byte[] { (byte)0x00, (byte)0x80, (byte)0x40, (byte)0xc0, (byte)0x20, 
            (byte)0xa0, (byte)0x60, (byte)0xe0, (byte)0x10, (byte)0x90, (byte)0x50, (byte)0xd0, (byte)0x30, (byte)
            0xb0, (byte)0x70, (byte)0xf0, (byte)0x08, (byte)0x88, (byte)0x48, (byte)0xc8, (byte)0x28, (byte)0xa8, 
            (byte)0x68, (byte)0xe8, (byte)0x18, (byte)0x98, (byte)0x58, (byte)0xd8, (byte)0x38, (byte)0xb8, (byte)
            0x78, (byte)0xf8, (byte)0x04, (byte)0x84, (byte)0x44, (byte)0xc4, (byte)0x24, (byte)0xa4, (byte)0x64, 
            (byte)0xe4, (byte)0x14, (byte)0x94, (byte)0x54, (byte)0xd4, (byte)0x34, (byte)0xb4, (byte)0x74, (byte)
            0xf4, (byte)0x0c, (byte)0x8c, (byte)0x4c, (byte)0xcc, (byte)0x2c, (byte)0xac, (byte)0x6c, (byte)0xec, 
            (byte)0x1c, (byte)0x9c, (byte)0x5c, (byte)0xdc, (byte)0x3c, (byte)0xbc, (byte)0x7c, (byte)0xfc, (byte)
            0x02, (byte)0x82, (byte)0x42, (byte)0xc2, (byte)0x22, (byte)0xa2, (byte)0x62, (byte)0xe2, (byte)0x12, 
            (byte)0x92, (byte)0x52, (byte)0xd2, (byte)0x32, (byte)0xb2, (byte)0x72, (byte)0xf2, (byte)0x0a, (byte)
            0x8a, (byte)0x4a, (byte)0xca, (byte)0x2a, (byte)0xaa, (byte)0x6a, (byte)0xea, (byte)0x1a, (byte)0x9a, 
            (byte)0x5a, (byte)0xda, (byte)0x3a, (byte)0xba, (byte)0x7a, (byte)0xfa, (byte)0x06, (byte)0x86, (byte)
            0x46, (byte)0xc6, (byte)0x26, (byte)0xa6, (byte)0x66, (byte)0xe6, (byte)0x16, (byte)0x96, (byte)0x56, 
            (byte)0xd6, (byte)0x36, (byte)0xb6, (byte)0x76, (byte)0xf6, (byte)0x0e, (byte)0x8e, (byte)0x4e, (byte)
            0xce, (byte)0x2e, (byte)0xae, (byte)0x6e, (byte)0xee, (byte)0x1e, (byte)0x9e, (byte)0x5e, (byte)0xde, 
            (byte)0x3e, (byte)0xbe, (byte)0x7e, (byte)0xfe, (byte)0x01, (byte)0x81, (byte)0x41, (byte)0xc1, (byte)
            0x21, (byte)0xa1, (byte)0x61, (byte)0xe1, (byte)0x11, (byte)0x91, (byte)0x51, (byte)0xd1, (byte)0x31, 
            (byte)0xb1, (byte)0x71, (byte)0xf1, (byte)0x09, (byte)0x89, (byte)0x49, (byte)0xc9, (byte)0x29, (byte)
            0xa9, (byte)0x69, (byte)0xe9, (byte)0x19, (byte)0x99, (byte)0x59, (byte)0xd9, (byte)0x39, (byte)0xb9, 
            (byte)0x79, (byte)0xf9, (byte)0x05, (byte)0x85, (byte)0x45, (byte)0xc5, (byte)0x25, (byte)0xa5, (byte)
            0x65, (byte)0xe5, (byte)0x15, (byte)0x95, (byte)0x55, (byte)0xd5, (byte)0x35, (byte)0xb5, (byte)0x75, 
            (byte)0xf5, (byte)0x0d, (byte)0x8d, (byte)0x4d, (byte)0xcd, (byte)0x2d, (byte)0xad, (byte)0x6d, (byte)
            0xed, (byte)0x1d, (byte)0x9d, (byte)0x5d, (byte)0xdd, (byte)0x3d, (byte)0xbd, (byte)0x7d, (byte)0xfd, 
            (byte)0x03, (byte)0x83, (byte)0x43, (byte)0xc3, (byte)0x23, (byte)0xa3, (byte)0x63, (byte)0xe3, (byte)
            0x13, (byte)0x93, (byte)0x53, (byte)0xd3, (byte)0x33, (byte)0xb3, (byte)0x73, (byte)0xf3, (byte)0x0b, 
            (byte)0x8b, (byte)0x4b, (byte)0xcb, (byte)0x2b, (byte)0xab, (byte)0x6b, (byte)0xeb, (byte)0x1b, (byte)
            0x9b, (byte)0x5b, (byte)0xdb, (byte)0x3b, (byte)0xbb, (byte)0x7b, (byte)0xfb, (byte)0x07, (byte)0x87, 
            (byte)0x47, (byte)0xc7, (byte)0x27, (byte)0xa7, (byte)0x67, (byte)0xe7, (byte)0x17, (byte)0x97, (byte)
            0x57, (byte)0xd7, (byte)0x37, (byte)0xb7, (byte)0x77, (byte)0xf7, (byte)0x0f, (byte)0x8f, (byte)0x4f, 
            (byte)0xcf, (byte)0x2f, (byte)0xaf, (byte)0x6f, (byte)0xef, (byte)0x1f, (byte)0x9f, (byte)0x5f, (byte)
            0xdf, (byte)0x3f, (byte)0xbf, (byte)0x7f, (byte)0xff };

        internal static short[] white = new short[] { 6430, 6400, 6400, 6400, 3225, 3225, 3225, 3225, 944, 944, 944
            , 944, 976, 976, 976, 976, 1456, 1456, 1456, 1456, 1488, 1488, 1488, 1488, 718, 718, 718, 718, 718, 718
            , 718, 718, 750, 750, 750, 750, 750, 750, 750, 750, 1520, 1520, 1520, 1520, 1552, 1552, 1552, 1552, 428
            , 428, 428, 428, 428, 428, 428, 428, 428, 428, 428, 428, 428, 428, 428, 428, 654, 654, 654, 654, 654, 
            654, 654, 654, 1072, 1072, 1072, 1072, 1104, 1104, 1104, 1104, 1136, 1136, 1136, 1136, 1168, 1168, 1168
            , 1168, 1200, 1200, 1200, 1200, 1232, 1232, 1232, 1232, 622, 622, 622, 622, 622, 622, 622, 622, 1008, 
            1008, 1008, 1008, 1040, 1040, 1040, 1040, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 
            44, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 1712, 1712, 1712, 
            1712, 1744, 1744, 1744, 1744, 846, 846, 846, 846, 846, 846, 846, 846, 1264, 1264, 1264, 1264, 1296, 1296
            , 1296, 1296, 1328, 1328, 1328, 1328, 1360, 1360, 1360, 1360, 1392, 1392, 1392, 1392, 1424, 1424, 1424
            , 1424, 686, 686, 686, 686, 686, 686, 686, 686, 910, 910, 910, 910, 910, 910, 910, 910, 1968, 1968, 1968
            , 1968, 2000, 2000, 2000, 2000, 2032, 2032, 2032, 2032, 16, 16, 16, 16, 10257, 10257, 10257, 10257, 12305
            , 12305, 12305, 12305, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 
            330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 362, 362, 362, 362, 362
            , 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 
            362, 362, 362, 362, 362, 362, 362, 878, 878, 878, 878, 878, 878, 878, 878, 1904, 1904, 1904, 1904, 1936
            , 1936, 1936, 1936, -18413, -18413, -16365, -16365, -14317, -14317, -10221, -10221, 590, 590, 590, 590
            , 590, 590, 590, 590, 782, 782, 782, 782, 782, 782, 782, 782, 1584, 1584, 1584, 1584, 1616, 1616, 1616
            , 1616, 1648, 1648, 1648, 1648, 1680, 1680, 1680, 1680, 814, 814, 814, 814, 814, 814, 814, 814, 1776, 
            1776, 1776, 1776, 1808, 1808, 1808, 1808, 1840, 1840, 1840, 1840, 1872, 1872, 1872, 1872, 6157, 6157, 
            6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157, -12275, -12275, -12275
            , -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275
            , 14353, 14353, 14353, 14353, 16401, 16401, 16401, 16401, 22547, 22547, 24595, 24595, 20497, 20497, 20497
            , 20497, 18449, 18449, 18449, 18449, 26643, 26643, 28691, 28691, 30739, 30739, -32749, -32749, -30701, 
            -30701, -28653, -28653, -26605, -26605, -24557, -24557, -22509, -22509, -20461, -20461, 8207, 8207, 8207
            , 8207, 8207, 8207, 8207, 8207, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72
            , 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 
            72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 104, 104, 104, 104
            , 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 
            104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104
            , 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 4107, 
            4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 
            4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107, 266, 266, 266, 266
            , 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 
            266, 266, 266, 266, 266, 266, 266, 266, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298
            , 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 298, 524, 
            524, 524, 524, 524, 524, 524, 524, 524, 524, 524, 524, 524, 524, 524, 524, 556, 556, 556, 556, 556, 556
            , 556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 
            136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136
            , 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 
            136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 168, 168, 168, 168, 168, 168, 168, 168
            , 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 
            168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168
            , 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 460, 460, 460, 460, 460, 
            460, 460, 460, 460, 460, 460, 460, 460, 460, 460, 460, 492, 492, 492, 492, 492, 492, 492, 492, 492, 492
            , 492, 492, 492, 492, 492, 492, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059
            , 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059
            , 2059, 2059, 2059, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200
            , 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 
            200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200
            , 200, 200, 200, 200, 200, 200, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 
            232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232
            , 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 
            232, 232, 232, 232, 232, 232, 232, 232, 232 };

        public static short[] additionalMakeup = new short[] { 28679, 28679, 31752, -32759, -31735, -30711, -29687
            , -28663, 29703, 29703, 30727, 30727, -27639, -26615, -25591, -24567 };

        internal static short[] initBlack = new short[] { 3226, 6412, 200, 168, 38, 38, 134, 134, 100, 100, 100, 100
            , 68, 68, 68, 68 };

        internal static short[] twoBitBlack = new short[] { 292, 260, 226, 226 };

        internal static short[] black = new short[] { 62, 62, 30, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225
            , 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225
            , 3225, 3225, 3225, 3225, 588, 588, 588, 588, 588, 588, 588, 588, 1680, 1680, 20499, 22547, 24595, 26643
            , 1776, 1776, 1808, 1808, -24557, -22509, -20461, -18413, 1904, 1904, 1936, 1936, -16365, -14317, 782, 
            782, 782, 782, 814, 814, 814, 814, -12269, -10221, 10257, 10257, 12305, 12305, 14353, 14353, 16403, 18451
            , 1712, 1712, 1744, 1744, 28691, 30739, -32749, -30701, -28653, -26605, 2061, 2061, 2061, 2061, 2061, 
            2061, 2061, 2061, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 
            424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 424, 750, 750, 750, 750, 1616, 1616
            , 1648, 1648, 1424, 1424, 1456, 1456, 1488, 1488, 1520, 1520, 1840, 1840, 1872, 1872, 1968, 1968, 8209
            , 8209, 524, 524, 524, 524, 524, 524, 524, 524, 556, 556, 556, 556, 556, 556, 556, 556, 1552, 1552, 1584
            , 1584, 2000, 2000, 2032, 2032, 976, 976, 1008, 1008, 1040, 1040, 1072, 1072, 1296, 1296, 1328, 1328, 
            718, 718, 718, 718, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456
            , 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 456, 326, 326, 326, 326, 326, 
            326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326
            , 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 
            326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 326, 358, 358, 358
            , 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 
            358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358
            , 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 
            490, 490, 490, 490, 490, 490, 490, 490, 490, 490, 490, 490, 490, 490, 490, 490, 4113, 4113, 6161, 6161
            , 848, 848, 880, 880, 912, 912, 944, 944, 622, 622, 622, 622, 654, 654, 654, 654, 1104, 1104, 1136, 1136
            , 1168, 1168, 1200, 1200, 1232, 1232, 1264, 1264, 686, 686, 686, 686, 1360, 1360, 1392, 1392, 12, 12, 
            12, 12, 12, 12, 12, 12, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390
            , 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 
            390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390, 390
            , 390, 390, 390, 390, 390, 390, 390 };

        internal static byte[] twoDCodes = new byte[] { 80, 88, 23, 71, 30, 30, 62, 62, 4, 4, 4, 4, 4, 4, 4, 4, 11
            , 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 
            35, 35, 35, 35, 35, 35, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 41, 41, 41, 41
            , 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 
            41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41
            , 41, 41, 41, 41, 41, 41, 41, 41, 41 };

        /// <param name="fillOrder">The fill order of the compressed data bytes.</param>
        /// <param name="w"/>
        /// <param name="h"/>
        public TIFFFaxDecoder(int fillOrder, int w, int h) {
            // Data structures needed to store changing elements for the previous
            // and the current scanline
            // Element at which to start search in getNextChangingElement
            // Variables set by T4Options
            // should iText try to recover from images it can't read?
            // 0 bits are left in first byte - SHOULD NOT HAPPEN
            // 1 bits are left in first byte
            // 2 bits are left in first byte
            // 3 bits are left in first byte
            // 4 bits are left in first byte
            // 5 bits are left in first byte
            // 6 bits are left in first byte
            // 7 bits are left in first byte
            // 8 bits are left in first byte
            // 0
            // 1
            // 2
            // 3
            // 4
            // 5
            // 6
            // 7
            // 8
            // Table to be used when fillOrder = 2, for flipping bytes.
            // The main 10 bit white runs lookup table
            // 0 - 7
            // 8 - 15
            // 16 - 23
            // 24 - 31
            // 32 - 39
            // 40 - 47
            // 48 - 55
            // 56 - 63
            // 64 - 71
            // 72 - 79
            // 80 - 87
            // 88 - 95
            // 96 - 103
            // 104 - 111
            // 112 - 119
            // 120 - 127
            // 128 - 135
            // 136 - 143
            // 144 - 151
            // 152 - 159
            // 160 - 167
            // 168 - 175
            // 176 - 183
            // 184 - 191
            // 192 - 199
            // 200 - 207
            // 208 - 215
            // 216 - 223
            // 224 - 231
            // 232 - 239
            // 240 - 247
            // 248 - 255
            // 256 - 263
            // 264 - 271
            // 272 - 279
            // 280 - 287
            // 288 - 295
            // 296 - 303
            // 304 - 311
            // 312 - 319
            // 320 - 327
            // 328 - 335
            // 336 - 343
            // 344 - 351
            // 352 - 359
            // 360 - 367
            // 368 - 375
            // 376 - 383
            // 384 - 391
            // 392 - 399
            // 400 - 407
            // 408 - 415
            // 416 - 423
            // 424 - 431
            // 432 - 439
            // 440 - 447
            // 448 - 455
            // 456 - 463
            // 464 - 471
            // 472 - 479
            // 480 - 487
            // 488 - 495
            // 496 - 503
            // 504 - 511
            // 512 - 519
            // 520 - 527
            // 528 - 535
            // 536 - 543
            // 544 - 551
            // 552 - 559
            // 560 - 567
            // 568 - 575
            // 576 - 583
            // 584 - 591
            // 592 - 599
            // 600 - 607
            // 608 - 615
            // 616 - 623
            // 624 - 631
            // 632 - 639
            // 640 - 647
            // 648 - 655
            // 656 - 663
            // 664 - 671
            // 672 - 679
            // 680 - 687
            // 688 - 695
            // 696 - 703
            // 704 - 711
            // 712 - 719
            // 720 - 727
            // 728 - 735
            // 736 - 743
            // 744 - 751
            // 752 - 759
            // 760 - 767
            // 768 - 775
            // 776 - 783
            // 784 - 791
            // 792 - 799
            // 800 - 807
            // 808 - 815
            // 816 - 823
            // 824 - 831
            // 832 - 839
            // 840 - 847
            // 848 - 855
            // 856 - 863
            // 864 - 871
            // 872 - 879
            // 880 - 887
            // 888 - 895
            // 896 - 903
            // 904 - 911
            // 912 - 919
            // 920 - 927
            // 928 - 935
            // 936 - 943
            // 944 - 951
            // 952 - 959
            // 960 - 967
            // 968 - 975
            // 976 - 983
            // 984 - 991
            // 992 - 999
            // 1000 - 1007
            // 1008 - 1015
            // 1016 - 1023
            // Additional make up codes for both White and Black runs
            //    static short[] additionalMakeup = {
            //        28679,  28679,  31752,  (short)32777,
            //        (short)33801,  (short)34825,  (short)35849,  (short)36873,
            //        (short)29703,  (short)29703,  (short)30727,  (short)30727,
            //        (short)37897,  (short)38921,  (short)39945,  (short)40969
            //    };
            //replace with constants without overload
            // Initial black run look up table, uses the first 4 bits of a code
            // 0 - 7
            // 8 - 15
            //
            // 0 - 3
            // Main black run table, using the last 9 bits of possible 13 bit code
            // 0 - 7
            // 8 - 15
            // 16 - 23
            // 24 - 31
            // 32 - 39
            // 40 - 47
            // 48 - 55
            // 56 - 63
            // 64 - 71
            // 72 - 79
            // 80 - 87
            // 88 - 95
            // 96 - 103
            // 104 - 111
            // 112 - 119
            // 120 - 127
            // 128 - 135
            // 136 - 143
            // 144 - 151
            // 152 - 159
            // 160 - 167
            // 168 - 175
            // 176 - 183
            // 184 - 191
            // 192 - 199
            // 200 - 207
            // 208 - 215
            // 216 - 223
            // 224 - 231
            // 232 - 239
            // 240 - 247
            // 248 - 255
            // 256 - 263
            // 264 - 271
            // 272 - 279
            // 280 - 287
            // 288 - 295
            // 296 - 303
            // 304 - 311
            // 312 - 319
            // 320 - 327
            // 328 - 335
            // 336 - 343
            // 344 - 351
            // 352 - 359
            // 360 - 367
            // 368 - 375
            // 376 - 383
            // 384 - 391
            // 392 - 399
            // 400 - 407
            // 408 - 415
            // 416 - 423
            // 424 - 431
            // 432 - 439
            // 440 - 447
            // 448 - 455
            // 456 - 463
            // 464 - 471
            // 472 - 479
            // 480 - 487
            // 488 - 495
            // 496 - 503
            // 504 - 511
            // 0 - 7
            // 8 - 15
            // 16 - 23
            // 24 - 31
            // 32 - 39
            // 40 - 47
            // 48 - 55
            // 56 - 63
            // 64 - 71
            // 72 - 79
            // 80 - 87
            // 88 - 95
            // 96 - 103
            // 104 - 111
            // 112 - 119
            // 120 - 127
            this.fillOrder = fillOrder;
            this.w = w;
            this.h = h;
            this.bitPointer = 0;
            this.bytePointer = 0;
            this.prevChangingElems = new int[2 * w];
            this.currChangingElems = new int[2 * w];
        }

        /// <summary>Reverses the bits in the array</summary>
        /// <param name="b">the bits to reverse</param>
        public static void ReverseBits(byte[] b) {
            for (int k = 0; k < b.Length; ++k) {
                b[k] = flipTable[b[k] & 0xff];
            }
        }

        // One-dimensional decoding methods
        public virtual void Decode1D(byte[] buffer, byte[] compData, int startX, int height) {
            this.data = compData;
            int lineOffset = 0;
            int scanlineStride = (w + 7) / 8;
            bitPointer = 0;
            bytePointer = 0;
            for (int i = 0; i < height; i++) {
                DecodeNextScanline(buffer, lineOffset, startX);
                lineOffset += scanlineStride;
            }
        }

        public virtual void DecodeNextScanline(byte[] buffer, int lineOffset, int bitOffset) {
            int bits;
            int code;
            int isT;
            int current;
            int entry;
            int twoBits;
            bool isWhite = true;
            // Initialize starting of the changing elements array
            changingElemSize = 0;
            // While scanline not complete
            while (bitOffset < w) {
                while (isWhite) {
                    // White run
                    current = NextNBits(10);
                    entry = white[current];
                    // Get the 3 fields from the entry
                    isT = entry & 0x0001;
                    bits = ((int)(((uint)entry) >> 1)) & 0x0f;
                    if (bits == 12) {
                        // Additional Make up code
                        // Get the next 2 bits
                        twoBits = NextLesserThan8Bits(2);
                        // Consolidate the 2 new bits and last 2 bits into 4 bits
                        current = ((current << 2) & 0x000c) | twoBits;
                        entry = additionalMakeup[current];
                        bits = ((int)(((uint)entry) >> 1)) & 0x07;
                        // 3 bits 0000 0111
                        code = ((int)(((uint)entry) >> 4)) & 0x0fff;
                        // 12 bits
                        bitOffset += code;
                        // Skip white run
                        UpdatePointer(4 - bits);
                    }
                    else {
                        if (bits == 0) {
                            // ERROR
                            throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.InvalidCodeEncountered);
                        }
                        else {
                            if (bits == 15) {
                                // EOL
                                throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.EolCodeWordEncounteredInWhiteRun);
                            }
                            else {
                                // 11 bits - 0000 0111 1111 1111 = 0x07ff
                                code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                                bitOffset += code;
                                UpdatePointer(10 - bits);
                                if (isT == 0) {
                                    isWhite = false;
                                    currChangingElems[changingElemSize++] = bitOffset;
                                }
                            }
                        }
                    }
                }
                // Check whether this run completed one width, if so
                // advance to next byte boundary for compression = 2.
                if (bitOffset == w) {
                    if (compression == 2) {
                        AdvancePointer();
                    }
                    break;
                }
                while (!isWhite) {
                    // Black run
                    current = NextLesserThan8Bits(4);
                    entry = initBlack[current];
                    // Get the 3 fields from the entry
                    bits = ((int)(((uint)entry) >> 1)) & 0x000f;
                    code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                    if (code == 100) {
                        current = NextNBits(9);
                        entry = black[current];
                        // Get the 3 fields from the entry
                        isT = entry & 0x0001;
                        bits = ((int)(((uint)entry) >> 1)) & 0x000f;
                        code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                        if (bits == 12) {
                            // Additional makeup codes
                            UpdatePointer(5);
                            current = NextLesserThan8Bits(4);
                            entry = additionalMakeup[current];
                            bits = ((int)(((uint)entry) >> 1)) & 0x07;
                            // 3 bits 0000 0111
                            code = ((int)(((uint)entry) >> 4)) & 0x0fff;
                            // 12 bits
                            SetToBlack(buffer, lineOffset, bitOffset, code);
                            bitOffset += code;
                            UpdatePointer(4 - bits);
                        }
                        else {
                            if (bits == 15) {
                                // EOL code
                                throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.EolCodeWordEncounteredInWhiteRun);
                            }
                            else {
                                SetToBlack(buffer, lineOffset, bitOffset, code);
                                bitOffset += code;
                                UpdatePointer(9 - bits);
                                if (isT == 0) {
                                    isWhite = true;
                                    currChangingElems[changingElemSize++] = bitOffset;
                                }
                            }
                        }
                    }
                    else {
                        if (code == 200) {
                            // Is a Terminating code
                            current = NextLesserThan8Bits(2);
                            entry = twoBitBlack[current];
                            code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                            bits = ((int)(((uint)entry) >> 1)) & 0x0f;
                            SetToBlack(buffer, lineOffset, bitOffset, code);
                            bitOffset += code;
                            UpdatePointer(2 - bits);
                            isWhite = true;
                            currChangingElems[changingElemSize++] = bitOffset;
                        }
                        else {
                            // Is a Terminating code
                            SetToBlack(buffer, lineOffset, bitOffset, code);
                            bitOffset += code;
                            UpdatePointer(4 - bits);
                            isWhite = true;
                            currChangingElems[changingElemSize++] = bitOffset;
                        }
                    }
                }
                // Check whether this run completed one width
                if (bitOffset == w) {
                    if (compression == 2) {
                        AdvancePointer();
                    }
                    break;
                }
            }
            currChangingElems[changingElemSize++] = bitOffset;
        }

        // Two-dimensional decoding methods
        public virtual void Decode2D(byte[] buffer, byte[] compData, int startX, int height, long tiffT4Options) {
            this.data = compData;
            compression = 3;
            bitPointer = 0;
            bytePointer = 0;
            int scanlineStride = (w + 7) / 8;
            int a0;
            int a1;
            int b1;
            int b2;
            int[] b = new int[2];
            int entry;
            int code;
            int bits;
            bool isWhite;
            int currIndex;
            int[] temp;
            // fillBits - dealt with this in readEOL
            // 1D/2D encoding - dealt with this in readEOL
            // uncompressedMode - haven't dealt with this yet.
            oneD = (int)(tiffT4Options & 0x01);
            uncompressedMode = (int)((tiffT4Options & 0x02) >> 1);
            fillBits = (int)((tiffT4Options & 0x04) >> 2);
            // The data must start with an EOL code
            if (ReadEOL(true) != 1) {
                throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.FirstScanlineMustBe1dEncoded);
            }
            int lineOffset = 0;
            int bitOffset;
            // Then the 1D encoded scanline data will occur, changing elements
            // array gets set.
            DecodeNextScanline(buffer, lineOffset, startX);
            lineOffset += scanlineStride;
            for (int lines = 1; lines < height; lines++) {
                // Every line must begin with an EOL followed by a bit which
                // indicates whether the following scanline is 1D or 2D encoded.
                if (ReadEOL(false) == 0) {
                    // 2D encoded scanline follows
                    // Initialize previous scanlines changing elements, and
                    // initialize current scanline's changing elements array
                    temp = prevChangingElems;
                    prevChangingElems = currChangingElems;
                    currChangingElems = temp;
                    currIndex = 0;
                    // a0 has to be set just before the start of this scanline.
                    a0 = -1;
                    isWhite = true;
                    bitOffset = startX;
                    lastChangingElement = 0;
                    while (bitOffset < w) {
                        // Get the next changing element
                        GetNextChangingElement(a0, isWhite, b);
                        b1 = b[0];
                        b2 = b[1];
                        // Get the next seven bits
                        entry = NextLesserThan8Bits(7);
                        // Run these through the 2DCodes table
                        entry = twoDCodes[entry] & 0xff;
                        // Get the code and the number of bits used up
                        code = (int)(((uint)(entry & 0x78)) >> 3);
                        bits = entry & 0x07;
                        if (code == 0) {
                            if (!isWhite) {
                                SetToBlack(buffer, lineOffset, bitOffset, b2 - bitOffset);
                            }
                            bitOffset = a0 = b2;
                            // Set pointer to consume the correct number of bits.
                            UpdatePointer(7 - bits);
                        }
                        else {
                            if (code == 1) {
                                // Horizontal
                                UpdatePointer(7 - bits);
                                // identify the next 2 codes.
                                int number;
                                if (isWhite) {
                                    number = DecodeWhiteCodeWord();
                                    bitOffset += number;
                                    currChangingElems[currIndex++] = bitOffset;
                                    number = DecodeBlackCodeWord();
                                    SetToBlack(buffer, lineOffset, bitOffset, number);
                                    bitOffset += number;
                                    currChangingElems[currIndex++] = bitOffset;
                                }
                                else {
                                    number = DecodeBlackCodeWord();
                                    SetToBlack(buffer, lineOffset, bitOffset, number);
                                    bitOffset += number;
                                    currChangingElems[currIndex++] = bitOffset;
                                    number = DecodeWhiteCodeWord();
                                    bitOffset += number;
                                    currChangingElems[currIndex++] = bitOffset;
                                }
                                a0 = bitOffset;
                            }
                            else {
                                if (code <= 8) {
                                    // Vertical
                                    a1 = b1 + (code - 5);
                                    currChangingElems[currIndex++] = a1;
                                    // We write the current color till a1 - 1 pos,
                                    // since a1 is where the next color starts
                                    if (!isWhite) {
                                        SetToBlack(buffer, lineOffset, bitOffset, a1 - bitOffset);
                                    }
                                    bitOffset = a0 = a1;
                                    isWhite = !isWhite;
                                    UpdatePointer(7 - bits);
                                }
                                else {
                                    throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.InvalidCodeEncounteredWhileDecoding2dGroup3CompressedData
                                        );
                                }
                            }
                        }
                    }
                    // Add the changing element beyond the current scanline for the
                    // other color too
                    currChangingElems[currIndex++] = bitOffset;
                    changingElemSize = currIndex;
                }
                else {
                    // 1D encoded scanline follows
                    DecodeNextScanline(buffer, lineOffset, startX);
                }
                lineOffset += scanlineStride;
            }
        }

        public virtual void DecodeT6(byte[] buffer, byte[] compData, int startX, int height, long tiffT6Options) {
            this.data = compData;
            compression = 4;
            bitPointer = 0;
            bytePointer = 0;
            int scanlineStride = (w + 7) / 8;
            int a0;
            int a1;
            int b1;
            int b2;
            int entry;
            int code;
            int bits;
            bool isWhite;
            int currIndex;
            int[] temp;
            // Return values from getNextChangingElement
            int[] b = new int[2];
            // uncompressedMode - have written some code for this, but this
            // has not been tested due to lack of test images using this optional
            uncompressedMode = (int)((tiffT6Options & 0x02) >> 1);
            // Local cached reference
            int[] cce = currChangingElems;
            // Assume invisible preceding row of all white pixels and insert
            // both black and white changing elements beyond the end of this
            // imaginary scanline.
            changingElemSize = 0;
            cce[changingElemSize++] = w;
            cce[changingElemSize++] = w;
            int lineOffset = 0;
            int bitOffset;
            for (int lines = 0; lines < height; lines++) {
                // a0 has to be set just before the start of the scanline.
                a0 = -1;
                isWhite = true;
                // Assign the changing elements of the previous scanline to
                // prevChangingElems and start putting this new scanline's
                // changing elements into the currChangingElems.
                temp = prevChangingElems;
                prevChangingElems = currChangingElems;
                cce = currChangingElems = temp;
                currIndex = 0;
                // Start decoding the scanline at startX in the raster
                bitOffset = startX;
                // Reset search start position for getNextChangingElement
                lastChangingElement = 0;
                // Till one whole scanline is decoded
                while (bitOffset < w && bytePointer < data.Length) {
                    // Get the next changing element
                    GetNextChangingElement(a0, isWhite, b);
                    b1 = b[0];
                    b2 = b[1];
                    // Get the next seven bits
                    entry = NextLesserThan8Bits(7);
                    // Run these through the 2DCodes table
                    entry = twoDCodes[entry] & 0xff;
                    // Get the code and the number of bits used up
                    code = (int)(((uint)(entry & 0x78)) >> 3);
                    bits = entry & 0x07;
                    if (code == 0) {
                        // Pass
                        // We always assume WhiteIsZero format for fax.
                        if (!isWhite) {
                            SetToBlack(buffer, lineOffset, bitOffset, b2 - bitOffset);
                        }
                        bitOffset = a0 = b2;
                        // Set pointer to only consume the correct number of bits.
                        UpdatePointer(7 - bits);
                    }
                    else {
                        if (code == 1) {
                            // Horizontal
                            // Set pointer to only consume the correct number of bits.
                            UpdatePointer(7 - bits);
                            // identify the next 2 alternating color codes.
                            int number;
                            if (isWhite) {
                                // Following are white and black runs
                                number = DecodeWhiteCodeWord();
                                bitOffset += number;
                                cce[currIndex++] = bitOffset;
                                number = DecodeBlackCodeWord();
                                SetToBlack(buffer, lineOffset, bitOffset, number);
                                bitOffset += number;
                                cce[currIndex++] = bitOffset;
                            }
                            else {
                                // First a black run and then a white run follows
                                number = DecodeBlackCodeWord();
                                SetToBlack(buffer, lineOffset, bitOffset, number);
                                bitOffset += number;
                                cce[currIndex++] = bitOffset;
                                number = DecodeWhiteCodeWord();
                                bitOffset += number;
                                cce[currIndex++] = bitOffset;
                            }
                            a0 = bitOffset;
                        }
                        else {
                            if (code <= 8) {
                                // Vertical
                                a1 = b1 + (code - 5);
                                cce[currIndex++] = a1;
                                // We write the current color till a1 - 1 pos,
                                // since a1 is where the next color starts
                                if (!isWhite) {
                                    SetToBlack(buffer, lineOffset, bitOffset, a1 - bitOffset);
                                }
                                bitOffset = a0 = a1;
                                isWhite = !isWhite;
                                UpdatePointer(7 - bits);
                            }
                            else {
                                if (code == 11) {
                                    if (NextLesserThan8Bits(3) != 7) {
                                        throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.InvalidCodeEncounteredWhileDecoding2dGroup4CompressedData
                                            );
                                    }
                                    int zeros = 0;
                                    bool exit = false;
                                    while (!exit) {
                                        while (NextLesserThan8Bits(1) != 1) {
                                            zeros++;
                                        }
                                        if (zeros > 5) {
                                            // Exit code
                                            // Zeros before exit code
                                            zeros = zeros - 6;
                                            if (!isWhite && (zeros > 0)) {
                                                cce[currIndex++] = bitOffset;
                                            }
                                            // Zeros before the exit code
                                            bitOffset += zeros;
                                            if (zeros > 0) {
                                                // Some zeros have been written
                                                isWhite = true;
                                            }
                                            // Read in the bit which specifies the color of
                                            // the following run
                                            if (NextLesserThan8Bits(1) == 0) {
                                                if (!isWhite) {
                                                    cce[currIndex++] = bitOffset;
                                                }
                                                isWhite = true;
                                            }
                                            else {
                                                if (isWhite) {
                                                    cce[currIndex++] = bitOffset;
                                                }
                                                isWhite = false;
                                            }
                                            exit = true;
                                        }
                                        if (zeros == 5) {
                                            if (!isWhite) {
                                                cce[currIndex++] = bitOffset;
                                            }
                                            bitOffset += zeros;
                                            // Last thing written was white
                                            isWhite = true;
                                        }
                                        else {
                                            bitOffset += zeros;
                                            cce[currIndex++] = bitOffset;
                                            SetToBlack(buffer, lineOffset, bitOffset, 1);
                                            ++bitOffset;
                                            // Last thing written was black
                                            isWhite = false;
                                        }
                                    }
                                }
                                else {
                                    //micah_tessler@yahoo.com
                                    //Microsoft TIFF renderers seem to treat unknown codes as line-breaks
                                    //That is, they give up on the current line and move on to the next one
                                    //set bitOffset to w to move on to the next scan line.
                                    bitOffset = w;
                                    UpdatePointer(7 - bits);
                                }
                            }
                        }
                    }
escape_continue: ;
                }
escape_break: ;
                // end loop
                // Add the changing element beyond the current scanline for the
                // other color too
                //make sure that the index does not exceed the bounds of the array
                if (currIndex < cce.Length) {
                    cce[currIndex++] = bitOffset;
                }
                // Number of changing elements in this scanline.
                changingElemSize = currIndex;
                lineOffset += scanlineStride;
            }
        }

        private void SetToBlack(byte[] buffer, int lineOffset, int bitOffset, int numBits) {
            int bitNum = 8 * lineOffset + bitOffset;
            int lastBit = bitNum + numBits;
            int byteNum = bitNum >> 3;
            // Handle bits in first byte
            int shift = bitNum & 0x7;
            if (shift > 0) {
                int maskVal = 1 << (7 - shift);
                byte val = buffer[byteNum];
                while (maskVal > 0 && bitNum < lastBit) {
                    val |= (byte)maskVal;
                    maskVal >>= 1;
                    ++bitNum;
                }
                buffer[byteNum] = val;
            }
            // Fill in 8 bits at a time
            byteNum = bitNum >> 3;
            while (bitNum < lastBit - 7) {
                buffer[byteNum++] = (byte)255;
                bitNum += 8;
            }
            // Fill in remaining bits
            while (bitNum < lastBit) {
                byteNum = bitNum >> 3;
                if (recoverFromImageError && !(byteNum < buffer.Length)) {
                }
                else {
                    // do nothing
                    buffer[byteNum] |= (byte)(1 << (7 - (bitNum & 0x7)));
                }
                ++bitNum;
            }
        }

        // Returns run length
        private int DecodeWhiteCodeWord() {
            int current;
            int entry;
            int bits;
            int isT;
            int twoBits;
            int code = -1;
            int runLength = 0;
            bool isWhite = true;
            while (isWhite) {
                current = NextNBits(10);
                entry = white[current];
                // Get the 3 fields from the entry
                isT = entry & 0x0001;
                bits = ((int)(((uint)entry) >> 1)) & 0x0f;
                if (bits == 12) {
                    // Additional Make up code
                    // Get the next 2 bits
                    twoBits = NextLesserThan8Bits(2);
                    // Consolidate the 2 new bits and last 2 bits into 4 bits
                    current = ((current << 2) & 0x000c) | twoBits;
                    entry = additionalMakeup[current];
                    bits = ((int)(((uint)entry) >> 1)) & 0x07;
                    // 3 bits 0000 0111
                    code = ((int)(((uint)entry) >> 4)) & 0x0fff;
                    // 12 bits
                    runLength += code;
                    UpdatePointer(4 - bits);
                }
                else {
                    if (bits == 0) {
                        // ERROR
                        throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.InvalidCodeEncountered);
                    }
                    else {
                        if (bits == 15) {
                            // EOL
                            if (runLength == 0) {
                                isWhite = false;
                            }
                            else {
                                throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.EolCodeWordEncounteredInWhiteRun);
                            }
                        }
                        else {
                            // 11 bits - 0000 0111 1111 1111 = 0x07ff
                            code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                            runLength += code;
                            UpdatePointer(10 - bits);
                            if (isT == 0) {
                                isWhite = false;
                            }
                        }
                    }
                }
            }
            return runLength;
        }

        // Returns run length
        private int DecodeBlackCodeWord() {
            int current;
            int entry;
            int bits;
            int isT;
            int code = -1;
            int runLength = 0;
            bool isWhite = false;
            while (!isWhite) {
                current = NextLesserThan8Bits(4);
                entry = initBlack[current];
                // Get the 3 fields from the entry
                isT = entry & 0x0001;
                bits = ((int)(((uint)entry) >> 1)) & 0x000f;
                code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                if (code == 100) {
                    current = NextNBits(9);
                    entry = black[current];
                    // Get the 3 fields from the entry
                    isT = entry & 0x0001;
                    bits = ((int)(((uint)entry) >> 1)) & 0x000f;
                    code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                    if (bits == 12) {
                        // Additional makeup codes
                        UpdatePointer(5);
                        current = NextLesserThan8Bits(4);
                        entry = additionalMakeup[current];
                        bits = ((int)(((uint)entry) >> 1)) & 0x07;
                        // 3 bits 0000 0111
                        code = ((int)(((uint)entry) >> 4)) & 0x0fff;
                        // 12 bits
                        runLength += code;
                        UpdatePointer(4 - bits);
                    }
                    else {
                        if (bits == 15) {
                            // EOL code
                            throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.EolCodeWordEncounteredInBlackRun);
                        }
                        else {
                            runLength += code;
                            UpdatePointer(9 - bits);
                            if (isT == 0) {
                                isWhite = true;
                            }
                        }
                    }
                }
                else {
                    if (code == 200) {
                        // Is a Terminating code
                        current = NextLesserThan8Bits(2);
                        entry = twoBitBlack[current];
                        code = ((int)(((uint)entry) >> 5)) & 0x07ff;
                        runLength += code;
                        bits = ((int)(((uint)entry) >> 1)) & 0x0f;
                        UpdatePointer(2 - bits);
                        isWhite = true;
                    }
                    else {
                        // Is a Terminating code
                        runLength += code;
                        UpdatePointer(4 - bits);
                        isWhite = true;
                    }
                }
            }
            return runLength;
        }

        private int ReadEOL(bool isFirstEOL) {
            if (fillBits == 0) {
                int next12Bits = NextNBits(12);
                if (isFirstEOL && next12Bits == 0) {
                    // Might have the case of EOL padding being used even
                    // though it was not flagged in the T4Options field.
                    // This was observed to be the case in TIFFs produced
                    // by a well known vendor who shall remain nameless.
                    if (NextNBits(4) == 1) {
                        // EOL must be padded: reset the fillBits flag.
                        fillBits = 1;
                        return 1;
                    }
                }
                if (next12Bits != 1) {
                    throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.ScanlineMustBeginWithEolCodeWord);
                }
            }
            else {
                if (fillBits == 1) {
                    // First EOL code word xxxx 0000 0000 0001 will occur
                    // As many fill bits will be present as required to make
                    // the EOL code of 12 bits end on a byte boundary.
                    int bitsLeft = 8 - bitPointer;
                    if (NextNBits(bitsLeft) != 0) {
                        throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.AllFillBitsPrecedingEolCodeMustBe0);
                    }
                    // If the number of bitsLeft is less than 8, then to have a 12
                    // bit EOL sequence, two more bytes are certainly going to be
                    // required. The first of them has to be all zeros, so ensure
                    // that.
                    if (bitsLeft < 4) {
                        if (NextNBits(8) != 0) {
                            throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.AllFillBitsPrecedingEolCodeMustBe0);
                        }
                    }
                    // There might be a random number of fill bytes with 0s, so
                    // loop till the EOL of 0000 0001 is found, as long as all
                    // the bytes preceding it are 0's.
                    int n;
                    while ((n = NextNBits(8)) != 1) {
                        // If not all zeros
                        if (n != 0) {
                            throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.AllFillBitsPrecedingEolCodeMustBe0);
                        }
                    }
                }
            }
            // If one dimensional encoding mode, then always return 1
            if (oneD == 0) {
                return 1;
            }
            else {
                // Otherwise for 2D encoding mode,
                // The next one bit signifies 1D/2D encoding of next line.
                return NextLesserThan8Bits(1);
            }
        }

        private void GetNextChangingElement(int a0, bool isWhite, int[] ret) {
            // Local copies of instance variables
            int[] pce = this.prevChangingElems;
            int ces = this.changingElemSize;
            // If the previous match was at an odd element, we still
            // have to search the preceeding element.
            // int start = lastChangingElement & ~0x1;
            int start = lastChangingElement > 0 ? lastChangingElement - 1 : 0;
            if (isWhite) {
                start &= ~0x1;
            }
            else {
                // Search even numbered elements
                start |= 0x1;
            }
            // Search odd numbered elements
            int i = start;
            for (; i < ces; i += 2) {
                int temp = pce[i];
                if (temp > a0) {
                    lastChangingElement = i;
                    ret[0] = temp;
                    break;
                }
            }
            if (i + 1 < ces) {
                ret[1] = pce[i + 1];
            }
        }

        private int NextNBits(int bitsToGet) {
            byte b;
            byte next;
            byte next2next;
            int l = data.Length - 1;
            int bp = this.bytePointer;
            if (fillOrder == 1) {
                b = data[bp];
                if (bp == l) {
                    next = 0x00;
                    next2next = 0x00;
                }
                else {
                    if ((bp + 1) == l) {
                        next = data[bp + 1];
                        next2next = 0x00;
                    }
                    else {
                        next = data[bp + 1];
                        next2next = data[bp + 2];
                    }
                }
            }
            else {
                if (fillOrder == 2) {
                    b = flipTable[data[bp] & 0xff];
                    if (bp == l) {
                        next = 0x00;
                        next2next = 0x00;
                    }
                    else {
                        if ((bp + 1) == l) {
                            next = flipTable[data[bp + 1] & 0xff];
                            next2next = 0x00;
                        }
                        else {
                            next = flipTable[data[bp + 1] & 0xff];
                            next2next = flipTable[data[bp + 2] & 0xff];
                        }
                    }
                }
                else {
                    throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.TiffFillOrderTagMustBeEither1Or2);
                }
            }
            int bitsLeft = 8 - bitPointer;
            int bitsFromNextByte = bitsToGet - bitsLeft;
            int bitsFromNext2NextByte = 0;
            if (bitsFromNextByte > 8) {
                bitsFromNext2NextByte = bitsFromNextByte - 8;
                bitsFromNextByte = 8;
            }
            bytePointer++;
            int i1 = (b & table1[bitsLeft]) << (bitsToGet - bitsLeft);
            int i2 = (int)(((uint)(next & table2[bitsFromNextByte])) >> (8 - bitsFromNextByte));
            int i3;
            if (bitsFromNext2NextByte != 0) {
                i2 <<= bitsFromNext2NextByte;
                i3 = (int)(((uint)(next2next & table2[bitsFromNext2NextByte])) >> (8 - bitsFromNext2NextByte));
                i2 |= i3;
                bytePointer++;
                bitPointer = bitsFromNext2NextByte;
            }
            else {
                if (bitsFromNextByte == 8) {
                    bitPointer = 0;
                    bytePointer++;
                }
                else {
                    bitPointer = bitsFromNextByte;
                }
            }
            return i1 | i2;
        }

        private int NextLesserThan8Bits(int bitsToGet) {
            byte b = 0;
            byte next = 0;
            int l = data.Length - 1;
            int bp = this.bytePointer;
            if (fillOrder == 1) {
                b = data[bp];
                if (bp == l) {
                    next = 0x00;
                }
                else {
                    next = data[bp + 1];
                }
            }
            else {
                if (fillOrder == 2) {
                    if (recoverFromImageError && !(bp < data.Length)) {
                    }
                    else {
                        // do nothing
                        b = flipTable[data[bp] & 0xff];
                        if (bp == l) {
                            next = 0x00;
                        }
                        else {
                            next = flipTable[data[bp + 1] & 0xff];
                        }
                    }
                }
                else {
                    throw new iTextSharp.IO.IOException(iTextSharp.IO.IOException.TiffFillOrderTagMustBeEither1Or2);
                }
            }
            int bitsLeft = 8 - bitPointer;
            int bitsFromNextByte = bitsToGet - bitsLeft;
            int shift = bitsLeft - bitsToGet;
            int i1;
            int i2;
            if (shift >= 0) {
                i1 = (int)(((uint)(b & table1[bitsLeft])) >> shift);
                bitPointer += bitsToGet;
                if (bitPointer == 8) {
                    bitPointer = 0;
                    bytePointer++;
                }
            }
            else {
                i1 = (b & table1[bitsLeft]) << (-shift);
                i2 = (int)(((uint)(next & table2[bitsFromNextByte])) >> (8 - bitsFromNextByte));
                i1 |= i2;
                bytePointer++;
                bitPointer = bitsFromNextByte;
            }
            return i1;
        }

        // Move pointer backwards by given amount of bits
        private void UpdatePointer(int bitsToMoveBack) {
            int i = bitPointer - bitsToMoveBack;
            if (i < 0) {
                bytePointer--;
                bitPointer = 8 + i;
            }
            else {
                bitPointer = i;
            }
        }

        // Move to the next byte boundary
        private bool AdvancePointer() {
            if (bitPointer != 0) {
                bytePointer++;
                bitPointer = 0;
            }
            return true;
        }

        public virtual void SetRecoverFromImageError(bool recoverFromImageError) {
            this.recoverFromImageError = recoverFromImageError;
        }
    }
}
