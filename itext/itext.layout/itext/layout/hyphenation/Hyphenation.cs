/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Text;

namespace iText.Layout.Hyphenation {
    /// <summary>This class represents a hyphenated word.</summary>
    /// <remarks>
    /// This class represents a hyphenated word.
    /// <para />
    /// This work was authored by Carlos Villegas (cav@uniscope.co.jp).
    /// </remarks>
    public class Hyphenation {
        private int[] hyphenPoints;

        private String word;

        /// <summary>number of hyphenation points in word</summary>
        private int len;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// rawWord as made of alternating strings and
        /// <see cref="Hyphen">Hyphen</see>
        /// instances
        /// </summary>
        internal Hyphenation(String word, int[] points) {
            this.word = word;
            hyphenPoints = points;
            len = points.Length;
        }
//\endcond

        /// <returns>the number of hyphenation points in the word</returns>
        public virtual int Length() {
            return len;
        }

        /// <param name="index">an index position</param>
        /// <returns>the pre-break text, not including the hyphen character</returns>
        public virtual String GetPreHyphenText(int index) {
            return word.JSubstring(0, hyphenPoints[index]);
        }

        /// <param name="index">an index position</param>
        /// <returns>the post-break text</returns>
        public virtual String GetPostHyphenText(int index) {
            return word.Substring(hyphenPoints[index]);
        }

        /// <returns>the hyphenation points</returns>
        public virtual int[] GetHyphenationPoints() {
            return hyphenPoints;
        }

        /// <summary>
        /// <inheritDoc/>
        /// 
        /// </summary>
        public override String ToString() {
            StringBuilder str = new StringBuilder();
            int start = 0;
            for (int i = 0; i < len; i++) {
                str.Append(word.JSubstring(start, hyphenPoints[i]) + "-");
                start = hyphenPoints[i];
            }
            str.Append(word.Substring(start));
            return str.ToString();
        }
    }
}
