/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;

namespace iText.IO.Codec.Brotli.Dec {
//\cond DO_NOT_DOCUMENT
    /// <summary>Common context lookup table for all context modes.</summary>
    internal sealed class Context {
//\cond DO_NOT_DOCUMENT
        internal static readonly int[] LOOKUP = new int[2048];
//\endcond

        private const String UTF_MAP = "         !!  !                  \"#$##%#$&'##(#)#+++++++++" + "+((&*'##,---,---,-----,-----,-----&#'###.///.///./////./////./////&#'# ";

        private const String UTF_RLE = "A/*  ':  & : $  \u0081 @";

        private static void UnpackLookupTable(int[] lookup, String utfMap, String utfRle) {
            // LSB6, MSB6, SIGNED
            for (int i = 0; i < 256; ++i) {
                lookup[i] = i & 0x3F;
                lookup[512 + i] = i >> 2;
                lookup[1792 + i] = 2 + (i >> 6);
            }
            // UTF8
            for (int i = 0; i < 128; ++i) {
                lookup[1024 + i] = 4 * ((int)utfMap[i] - 32);
            }
            for (int i = 0; i < 64; ++i) {
                lookup[1152 + i] = i & 1;
                lookup[1216 + i] = 2 + (i & 1);
            }
            int offset = 1280;
            for (int k = 0; k < 19; ++k) {
                int value = k & 3;
                int rep = (int)utfRle[k] - 32;
                for (int i = 0; i < rep; ++i) {
                    lookup[offset++] = value;
                }
            }
            // SIGNED
            for (int i = 0; i < 16; ++i) {
                lookup[1792 + i] = 1;
                lookup[2032 + i] = 6;
            }
            lookup[1792] = 0;
            lookup[2047] = 7;
            for (int i = 0; i < 256; ++i) {
                lookup[1536 + i] = lookup[1792 + i] << 3;
            }
        }

        static Context() {
            UnpackLookupTable(LOOKUP, UTF_MAP, UTF_RLE);
        }
    }
//\endcond
}
