// Copyright 2013 Google Inc. All Rights Reserved.
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
    // Helper for rounding
    internal class Round {
        // Round a value up to the nearest multiple of 4. Don't round the value in the
        // case that rounding up overflows.
        public static int Round4(int value) {
            if (int.MaxValue - value < 3) {
                return value;
            }
            return (value + 3) & ~3;
        }

        public static long Round4(long value) {
            if (long.MaxValue - value < 3) {
                return value;
            }
            return (value + 3) & ~3;
        }
    }
//\endcond
}
