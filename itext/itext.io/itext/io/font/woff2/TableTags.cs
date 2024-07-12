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
namespace iText.IO.Font.Woff2 {
//\cond DO_NOT_DOCUMENT
    // Font table tags
    internal class TableTags {
        // Note that the byte order is big-endian
        private static int Tag(char a, char b, char c, char d) {
            return ((a << 24) | (b << 16) | (c << 8) | d);
        }

        // Tags of popular tables.
        public const int kGlyfTableTag = 0x676c7966;

        public const int kHeadTableTag = 0x68656164;

        public const int kLocaTableTag = 0x6c6f6361;

        public const int kDsigTableTag = 0x44534947;

        public const int kCffTableTag = 0x43464620;

        public const int kHmtxTableTag = 0x686d7478;

        public const int kHheaTableTag = 0x68686561;

        public const int kMaxpTableTag = 0x6d617870;

        public static int[] kKnownTags = new int[] { Tag('c', 'm', 'a', 'p'), 
                // 0
                Tag('h', 'e', 'a', 'd'), 
                // 1
                Tag('h', 'h', 'e', 'a'), 
                // 2
                Tag('h', 'm', 't', 'x'), 
                // 3
                Tag('m', 'a', 'x', 'p'), 
                // 4
                Tag('n', 'a', 'm', 'e'), 
                // 5
                Tag('O', 'S', '/', '2'), 
                // 6
                Tag('p', 'o', 's', 't'), 
                // 7
                Tag('c', 'v', 't', ' '), 
                // 8
                Tag('f', 'p', 'g', 'm'), 
                // 9
                Tag('g', 'l', 'y', 'f'), 
                // 10
                Tag('l', 'o', 'c', 'a'), 
                // 11
                Tag('p', 'r', 'e', 'p'), 
                // 12
                Tag('C', 'F', 'F', ' '), 
                // 13
                Tag('V', 'O', 'R', 'G'), 
                // 14
                Tag('E', 'B', 'D', 'T'), 
                // 15
                Tag('E', 'B', 'L', 'C'), 
                // 16
                Tag('g', 'a', 's', 'p'), 
                // 17
                Tag('h', 'd', 'm', 'x'), 
                // 18
                Tag('k', 'e', 'r', 'n'), 
                // 19
                Tag('L', 'T', 'S', 'H'), 
                // 20
                Tag('P', 'C', 'L', 'T'), 
                // 21
                Tag('V', 'D', 'M', 'X'), 
                // 22
                Tag('v', 'h', 'e', 'a'), 
                // 23
                Tag('v', 'm', 't', 'x'), 
                // 24
                Tag('B', 'A', 'S', 'E'), 
                // 25
                Tag('G', 'D', 'E', 'F'), 
                // 26
                Tag('G', 'P', 'O', 'S'), 
                // 27
                Tag('G', 'S', 'U', 'B'), 
                // 28
                Tag('E', 'B', 'S', 'C'), 
                // 29
                Tag('J', 'S', 'T', 'F'), 
                // 30
                Tag('M', 'A', 'T', 'H'), 
                // 31
                Tag('C', 'B', 'D', 'T'), 
                // 32
                Tag('C', 'B', 'L', 'C'), 
                // 33
                Tag('C', 'O', 'L', 'R'), 
                // 34
                Tag('C', 'P', 'A', 'L'), 
                // 35
                Tag('S', 'V', 'G', ' '), 
                // 36
                Tag('s', 'b', 'i', 'x'), 
                // 37
                Tag('a', 'c', 'n', 't'), 
                // 38
                Tag('a', 'v', 'a', 'r'), 
                // 39
                Tag('b', 'd', 'a', 't'), 
                // 40
                Tag('b', 'l', 'o', 'c'), 
                // 41
                Tag('b', 's', 'l', 'n'), 
                // 42
                Tag('c', 'v', 'a', 'r'), 
                // 43
                Tag('f', 'd', 's', 'c'), 
                // 44
                Tag('f', 'e', 'a', 't'), 
                // 45
                Tag('f', 'm', 't', 'x'), 
                // 46
                Tag('f', 'v', 'a', 'r'), 
                // 47
                Tag('g', 'v', 'a', 'r'), 
                // 48
                Tag('h', 's', 't', 'y'), 
                // 49
                Tag('j', 'u', 's', 't'), 
                // 50
                Tag('l', 'c', 'a', 'r'), 
                // 51
                Tag('m', 'o', 'r', 't'), 
                // 52
                Tag('m', 'o', 'r', 'x'), 
                // 53
                Tag('o', 'p', 'b', 'd'), 
                // 54
                Tag('p', 'r', 'o', 'p'), 
                // 55
                Tag('t', 'r', 'a', 'k'), 
                // 56
                Tag('Z', 'a', 'p', 'f'), 
                // 57
                Tag('S', 'i', 'l', 'f'), 
                // 58
                Tag('G', 'l', 'a', 't'), 
                // 59
                Tag('G', 'l', 'o', 'c'), 
                // 60
                Tag('F', 'e', 'a', 't'), 
                // 61
                Tag('S', 'i', 'l', 'l') };
        // 62
    }
//\endcond
}
