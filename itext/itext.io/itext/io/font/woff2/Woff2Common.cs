// Copyright 2014 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// This is part of java port of project hosted at https://github.com/google/woff2
using System;

namespace iText.IO.Font.Woff2 {
//\cond DO_NOT_DOCUMENT
    // Common definition for WOFF2 encoding/decoding
    // Helpers common across multiple parts of woff2
    internal class Woff2Common {
        public const int kWoff2Signature = 0x774f4632;

        // "wOF2"
        // Leave the first byte open to store flag_byte
        public const int kWoff2FlagsTransform = 1 << 8;

        // TrueType Collection ID string: 'ttcf'
        public const int kTtcFontFlavor = 0x74746366;

        public const int kSfntHeaderSize = 12;

        public const int kSfntEntrySize = 16;

        public class Point {
            public int x;

            public int y;

            public bool on_curve;

            public Point(int x, int y, bool on_curve) {
                this.x = x;
                this.y = y;
                this.on_curve = on_curve;
            }
        }

        public class Table : IComparable<Woff2Common.Table> {
            public int tag;

            public int flags;

            public int src_offset;

            public int src_length;

            public int transform_length;

            public int dst_offset;

            public int dst_length;

            public virtual int CompareTo(Woff2Common.Table o) {
                return JavaUnsignedUtil.CompareAsUnsigned(tag, o.tag);
            }
        }

        // Size of the collection header. 0 if version indicates this isn't a
        // collection. Ref http://www.microsoft.com/typography/otspec/otff.htm,
        // True Type Collections
        public static int CollectionHeaderSize(int header_version, int num_fonts) {
            int size = 0;
            if (header_version == 0x00020000) {
                size += 12;
            }
            // ulDsig{Tag,Length,Offset}
            if (header_version == 0x00010000 || header_version == 0x00020000) {
                size += 12 + 
                                // TTCTag, Version, numFonts
                                4 * num_fonts;
            }
            // OffsetTable[numFonts]
            return size;
        }

        // Compute checksum over size bytes of buf
        public static int ComputeULongSum(byte[] buf, int offset, int size) {
            int checksum = 0;
            int aligned_size = (size & ~3);
            for (int i = 0; i < aligned_size; i += 4) {
                checksum += (JavaUnsignedUtil.AsU8(buf[offset + i]) << 24) | (JavaUnsignedUtil.AsU8(buf[offset + i + 1]) <<
                     16) | (JavaUnsignedUtil.AsU8(buf[offset + i + 2]) << 8) | JavaUnsignedUtil.AsU8(buf[offset + i + 3]);
            }
            // treat size not aligned on 4 as if it were padded to 4 with 0's
            if (size != aligned_size) {
                int v = 0;
                for (int i = aligned_size; i < size; ++i) {
                    v |= JavaUnsignedUtil.AsU8(buf[offset + i]) << (24 - 8 * (i & 3));
                }
                checksum += v;
            }
            return checksum;
        }
    }
//\endcond
}
