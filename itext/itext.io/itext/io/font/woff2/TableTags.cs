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
    internal class TableTags {
        // Font table tags
        // Note that the byte order is big-endian
        private static int Tag(char a, char b, char c, char d) {
            return ((a << 24) | (b << 16) | (c << 8) | d);
        }

        public const int kGlyfTableTag = 0x676c7966;

        public const int kHeadTableTag = 0x68656164;

        public const int kLocaTableTag = 0x6c6f6361;

        public const int kDsigTableTag = 0x44534947;

        public const int kCffTableTag = 0x43464620;

        public const int kHmtxTableTag = 0x686d7478;

        public const int kHheaTableTag = 0x68686561;

        public const int kMaxpTableTag = 0x6d617870;

        public static int[] kKnownTags = new int[] { Tag('c', 'm', 'a', 'p'), Tag('h', 'e', 'a', 'd'), Tag('h', 'h'
            , 'e', 'a'), Tag('h', 'm', 't', 'x'), Tag('m', 'a', 'x', 'p'), Tag('n', 'a', 'm', 'e'), Tag('O', 'S', 
            '/', '2'), Tag('p', 'o', 's', 't'), Tag('c', 'v', 't', ' '), Tag('f', 'p', 'g', 'm'), Tag('g', 'l', 'y'
            , 'f'), Tag('l', 'o', 'c', 'a'), Tag('p', 'r', 'e', 'p'), Tag('C', 'F', 'F', ' '), Tag('V', 'O', 'R', 
            'G'), Tag('E', 'B', 'D', 'T'), Tag('E', 'B', 'L', 'C'), Tag('g', 'a', 's', 'p'), Tag('h', 'd', 'm', 'x'
            ), Tag('k', 'e', 'r', 'n'), Tag('L', 'T', 'S', 'H'), Tag('P', 'C', 'L', 'T'), Tag('V', 'D', 'M', 'X'), 
            Tag('v', 'h', 'e', 'a'), Tag('v', 'm', 't', 'x'), Tag('B', 'A', 'S', 'E'), Tag('G', 'D', 'E', 'F'), Tag
            ('G', 'P', 'O', 'S'), Tag('G', 'S', 'U', 'B'), Tag('E', 'B', 'S', 'C'), Tag('J', 'S', 'T', 'F'), Tag('M'
            , 'A', 'T', 'H'), Tag('C', 'B', 'D', 'T'), Tag('C', 'B', 'L', 'C'), Tag('C', 'O', 'L', 'R'), Tag('C', 
            'P', 'A', 'L'), Tag('S', 'V', 'G', ' '), Tag('s', 'b', 'i', 'x'), Tag('a', 'c', 'n', 't'), Tag('a', 'v'
            , 'a', 'r'), Tag('b', 'd', 'a', 't'), Tag('b', 'l', 'o', 'c'), Tag('b', 's', 'l', 'n'), Tag('c', 'v', 
            'a', 'r'), Tag('f', 'd', 's', 'c'), Tag('f', 'e', 'a', 't'), Tag('f', 'm', 't', 'x'), Tag('f', 'v', 'a'
            , 'r'), Tag('g', 'v', 'a', 'r'), Tag('h', 's', 't', 'y'), Tag('j', 'u', 's', 't'), Tag('l', 'c', 'a', 
            'r'), Tag('m', 'o', 'r', 't'), Tag('m', 'o', 'r', 'x'), Tag('o', 'p', 'b', 'd'), Tag('p', 'r', 'o', 'p'
            ), Tag('t', 'r', 'a', 'k'), Tag('Z', 'a', 'p', 'f'), Tag('S', 'i', 'l', 'f'), Tag('G', 'l', 'a', 't'), 
            Tag('G', 'l', 'o', 'c'), Tag('F', 'e', 'a', 't'), Tag('S', 'i', 'l', 'l') };
        // Tags of popular tables.
        // 0
        // 1
        // 2
        // 3
        // 4
        // 5
        // 6
        // 7
        // 8
        // 9
        // 10
        // 11
        // 12
        // 13
        // 14
        // 15
        // 16
        // 17
        // 18
        // 19
        // 20
        // 21
        // 22
        // 23
        // 24
        // 25
        // 26
        // 27
        // 28
        // 29
        // 30
        // 31
        // 32
        // 33
        // 34
        // 35
        // 36
        // 37
        // 38
        // 39
        // 40
        // 41
        // 42
        // 43
        // 44
        // 45
        // 46
        // 47
        // 48
        // 49
        // 50
        // 51
        // 52
        // 53
        // 54
        // 55
        // 56
        // 57
        // 58
        // 59
        // 60
        // 61
        // 62
    }
}
