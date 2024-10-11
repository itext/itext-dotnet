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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Commons.Utils;

namespace iText.Layout.Hyphenation {
    /// <summary>
    /// This tree structure stores the hyphenation patterns in an efficient
    /// way for fast lookup.
    /// </summary>
    /// <remarks>
    /// This tree structure stores the hyphenation patterns in an efficient
    /// way for fast lookup. It provides the provides the method to
    /// hyphenate a word.
    /// <para />
    /// This work was authored by Carlos Villegas (cav@uniscope.co.jp).
    /// </remarks>
    public class HyphenationTree : TernaryTree, IPatternConsumer {
        /// <summary>value space: stores the interletter values</summary>
        protected internal ByteVector vspace;

        /// <summary>This map stores hyphenation exceptions</summary>
        protected internal IDictionary<String, IList> stoplist;

        /// <summary>This map stores the character classes</summary>
        protected internal TernaryTree classmap;

        /// <summary>Temporary map to store interletter values on pattern loading.</summary>
        private TernaryTree ivalues;

        /// <summary>Default constructor.</summary>
        public HyphenationTree() {
            stoplist = new Dictionary<String, IList>(23);
            classmap = new TernaryTree();
            vspace = new ByteVector();
            // this reserves index 0, which we don't use
            vspace.Alloc(1);
        }

        /// <summary>
        /// Packs the values by storing them in 4 bits, two values into a byte
        /// Values range is from 0 to 9.
        /// </summary>
        /// <remarks>
        /// Packs the values by storing them in 4 bits, two values into a byte
        /// Values range is from 0 to 9. We use zero as terminator,
        /// so we'll add 1 to the value.
        /// </remarks>
        /// <param name="values">
        /// a string of digits from '0' to '9' representing the
        /// interletter values.
        /// </param>
        /// <returns>
        /// the index into the vspace array where the packed values
        /// are stored.
        /// </returns>
        protected internal virtual int PackValues(String values) {
            int i;
            int n = values.Length;
            int m = (n & 1) == 1 ? (n >> 1) + 2 : (n >> 1) + 1;
            int offset = vspace.Alloc(m);
            byte[] va = vspace.GetArray();
            for (i = 0; i < n; i++) {
                int j = i >> 1;
                byte v = (byte)((values[i] - '0' + 1) & 0x0f);
                if ((i & 1) == 1) {
                    va[j + offset] = (byte)(va[j + offset] | v);
                }
                else {
                    // big endian
                    va[j + offset] = (byte)(v << 4);
                }
            }
            // terminator
            va[m - 1 + offset] = 0;
            return offset;
        }

        /// <summary>Unpack values.</summary>
        /// <param name="k">an integer</param>
        /// <returns>a string</returns>
        protected internal virtual String UnpackValues(int k) {
            StringBuilder buf = new StringBuilder();
            byte v = vspace.Get(k++);
            while (v != 0) {
                char c = (char)((v >> 4) - 1 + '0');
                buf.Append(c);
                c = (char)(v & 0x0f);
                if (c == 0) {
                    break;
                }
                c = (char)(c - 1 + '0');
                buf.Append(c);
                v = vspace.Get(k++);
            }
            return buf.ToString();
        }

        /// <summary>Read hyphenation patterns from an XML file.</summary>
        /// <param name="filename">the filename</param>
        public virtual void LoadPatterns(String filename) {
            LoadPatterns(FileUtil.GetInputStreamForFile(filename), filename);
        }

        /// <summary>Read hyphenation patterns from an XML file.</summary>
        /// <param name="stream">the InputSource for the file</param>
        /// <param name="name">unique key representing country-language combination</param>
        public virtual void LoadPatterns(Stream stream, String name) {
            PatternParser pp = new PatternParser(this);
            ivalues = new TernaryTree();
            pp.Parse(stream, name);
            // patterns/values should be now in the tree
            // let's optimize a bit
            TrimToSize();
            vspace.TrimToSize();
            classmap.TrimToSize();
            // get rid of the auxiliary map
            ivalues = null;
        }

        /// <summary>Find pattern.</summary>
        /// <param name="pat">a pattern</param>
        /// <returns>a string</returns>
        public virtual String FindPattern(String pat) {
            int k = base.Find(pat);
            if (k >= 0) {
                return UnpackValues(k);
            }
            return "";
        }

        /// <summary>
        /// String compare, returns 0 if equal or
        /// t is a substring of s.
        /// </summary>
        /// <param name="s">first character array</param>
        /// <param name="si">starting index into first array</param>
        /// <param name="t">second character array</param>
        /// <param name="ti">starting index into second array</param>
        /// <returns>an integer</returns>
        protected internal virtual int Hstrcmp(char[] s, int si, char[] t, int ti) {
            for (; s[si] == t[ti]; si++, ti++) {
                if (s[si] == 0) {
                    return 0;
                }
            }
            if (t[ti] == 0) {
                return 0;
            }
            return s[si] - t[ti];
        }

        /// <summary>Get values.</summary>
        /// <param name="k">an integer</param>
        /// <returns>a byte array</returns>
        protected internal virtual byte[] GetValues(int k) {
            StringBuilder buf = new StringBuilder();
            byte v = vspace.Get(k++);
            while (v != 0) {
                char c = (char)((v >> 4) - 1);
                buf.Append(c);
                c = (char)(v & 0x0f);
                if (c == 0) {
                    break;
                }
                c = (char)(c - 1);
                buf.Append(c);
                v = vspace.Get(k++);
            }
            byte[] res = new byte[buf.Length];
            for (int i = 0; i < res.Length; i++) {
                res[i] = (byte)buf[i];
            }
            return res;
        }

        /// <summary>
        /// Search for all possible partial matches of word starting
        /// at index an update interletter values.
        /// </summary>
        /// <remarks>
        /// Search for all possible partial matches of word starting
        /// at index an update interletter values. In other words, it
        /// does something like:
        /// <para />
        /// <c>
        /// for(i=0; i&lt;patterns.length; i++) {
        /// if ( word.substring(index).startsWidth(patterns[i]) )
        /// update_interletter_values(patterns[i]);
        /// }
        /// </c>
        /// <para />
        /// But it is done in an efficient way since the patterns are
        /// stored in a ternary tree. In fact, this is the whole purpose
        /// of having the tree: doing this search without having to test
        /// every single pattern. The number of patterns for languages
        /// such as English range from 4000 to 10000. Thus, doing thousands
        /// of string comparisons for each word to hyphenate would be
        /// really slow without the tree. The tradeoff is memory, but
        /// using a ternary tree instead of a trie, almost halves the
        /// the memory used by Lout or TeX. It's also faster than using
        /// a hash table
        /// </remarks>
        /// <param name="word">null terminated word to match</param>
        /// <param name="index">start index from word</param>
        /// <param name="il">interletter values array to update</param>
        protected internal virtual void SearchPatterns(char[] word, int index, byte[] il) {
            byte[] values;
            int i = index;
            char p;
            char q;
            char sp = word[i];
            p = root;
            while (p > 0 && p < sc.Length) {
                if (sc[p] == 0xFFFF) {
                    if (Hstrcmp(word, i, kv.GetArray(), lo[p]) == 0) {
                        // data pointer is in eq[]
                        values = GetValues(eq[p]);
                        int j = index;
                        for (int k = 0; k < values.Length; k++) {
                            if (j < il.Length && values[k] > il[j]) {
                                il[j] = values[k];
                            }
                            j++;
                        }
                    }
                    return;
                }
                int d = sp - sc[p];
                if (d == 0) {
                    if (sp == 0) {
                        break;
                    }
                    sp = word[++i];
                    p = eq[p];
                    q = p;
                    // look for a pattern ending at this position by searching for
                    // the null char ( splitchar == 0 )
                    while (q > 0 && q < sc.Length) {
                        // stop at compressed branch
                        if (sc[q] == 0xFFFF) {
                            break;
                        }
                        if (sc[q] == 0) {
                            values = GetValues(eq[q]);
                            int j = index;
                            for (int k = 0; k < values.Length; k++) {
                                if (j < il.Length && values[k] > il[j]) {
                                    il[j] = values[k];
                                }
                                j++;
                            }
                            break;
                        }
                        else {
                            q = lo[q];
                        }
                    }
                }
                else {
                    p = d < 0 ? lo[p] : hi[p];
                }
            }
        }

        /// <summary>Hyphenate word and return a Hyphenation object.</summary>
        /// <param name="word">the word to be hyphenated</param>
        /// <param name="remainCharCount">
        /// Minimum number of characters allowed
        /// before the hyphenation point.
        /// </param>
        /// <param name="pushCharCount">
        /// Minimum number of characters allowed after
        /// the hyphenation point.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="Hyphenation">Hyphenation</see>
        /// object representing
        /// the hyphenated word or null if word is not hyphenated.
        /// </returns>
        public virtual iText.Layout.Hyphenation.Hyphenation Hyphenate(String word, int remainCharCount, int pushCharCount
            ) {
            char[] w = word.ToCharArray();
            if (IsMultiPartWord(w, w.Length)) {
                IList<char[]> words = SplitOnNonCharacters(w);
                return new iText.Layout.Hyphenation.Hyphenation(new String(w), GetHyphPointsForWords(words, remainCharCount
                    , pushCharCount));
            }
            else {
                return Hyphenate(w, 0, w.Length, remainCharCount, pushCharCount);
            }
        }

        private bool IsMultiPartWord(char[] w, int len) {
            int wordParts = 0;
            for (int i = 0; i < len; i++) {
                char[] c = new char[2];
                c[0] = w[i];
                int nc = classmap.Find(c, 0);
                if (nc > 0) {
                    if (wordParts > 1) {
                        return true;
                    }
                    wordParts = 1;
                }
                else {
                    if (wordParts == 1) {
                        wordParts++;
                    }
                }
            }
            return false;
        }

        private IList<char[]> SplitOnNonCharacters(char[] word) {
            IList<int> breakPoints = GetNonLetterBreaks(word);
            if (breakPoints.Count == 0) {
                return JavaCollectionsUtil.EmptyList<char[]>();
            }
            IList<char[]> words = new List<char[]>();
            for (int ibreak = 0; ibreak < breakPoints.Count; ibreak++) {
                char[] newWord = GetWordFromCharArray(word, ((ibreak == 0) ? 0 : breakPoints[ibreak - 1]), breakPoints[ibreak
                    ]);
                words.Add(newWord);
            }
            if (word.Length - breakPoints[breakPoints.Count - 1] - 1 > 1) {
                char[] newWord = GetWordFromCharArray(word, breakPoints[breakPoints.Count - 1], word.Length);
                words.Add(newWord);
            }
            return words;
        }

        private IList<int> GetNonLetterBreaks(char[] word) {
            char[] c = new char[2];
            IList<int> breakPoints = new List<int>();
            bool foundLetter = false;
            for (int i = 0; i < word.Length; i++) {
                c[0] = word[i];
                if (classmap.Find(c, 0) < 0) {
                    if (foundLetter) {
                        breakPoints.Add(i);
                    }
                }
                else {
                    foundLetter = true;
                }
            }
            return breakPoints;
        }

        private char[] GetWordFromCharArray(char[] word, int startIndex, int endIndex) {
            char[] newWord = new char[endIndex - ((startIndex == 0) ? startIndex : startIndex + 1)];
            int iChar = 0;
            for (int i = (startIndex == 0) ? 0 : startIndex + 1; i < endIndex; i++) {
                newWord[iChar++] = word[i];
            }
            return newWord;
        }

        private int[] GetHyphPointsForWords(IList<char[]> nonLetterWords, int remainCharCount, int pushCharCount) {
            int[] breaks = new int[0];
            for (int iNonLetterWord = 0; iNonLetterWord < nonLetterWords.Count; iNonLetterWord++) {
                char[] nonLetterWord = nonLetterWords[iNonLetterWord];
                iText.Layout.Hyphenation.Hyphenation curHyph = Hyphenate(nonLetterWord, 0, nonLetterWord.Length, (iNonLetterWord
                     == 0) ? remainCharCount : 1, (iNonLetterWord == nonLetterWords.Count - 1) ? pushCharCount : 1);
                if (curHyph == null) {
                    continue;
                }
                int[] combined = new int[breaks.Length + curHyph.GetHyphenationPoints().Length];
                int[] hyphPoints = curHyph.GetHyphenationPoints();
                int foreWordsSize = CalcForeWordsSize(nonLetterWords, iNonLetterWord);
                for (int i = 0; i < hyphPoints.Length; i++) {
                    hyphPoints[i] += foreWordsSize;
                }
                Array.Copy(breaks, 0, combined, 0, breaks.Length);
                Array.Copy(hyphPoints, 0, combined, breaks.Length, hyphPoints.Length);
                breaks = combined;
            }
            return breaks;
        }

        private int CalcForeWordsSize(IList<char[]> nonLetterWords, int iNonLetterWord) {
            int result = 0;
            for (int i = 0; i < iNonLetterWord; i++) {
                result += nonLetterWords[i].Length + 1;
            }
            return result;
        }

        /// <summary>Hyphenate word and return an array of hyphenation points.</summary>
        /// <param name="w">char array that contains the word</param>
        /// <param name="offset">Offset to first character in word</param>
        /// <param name="len">Length of word</param>
        /// <param name="remainCharCount">
        /// Minimum number of characters allowed
        /// before the hyphenation point.
        /// </param>
        /// <param name="pushCharCount">
        /// Minimum number of characters allowed after
        /// the hyphenation point.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="Hyphenation">Hyphenation</see>
        /// object representing
        /// the hyphenated word or null if word is not hyphenated.
        /// </returns>
        public virtual iText.Layout.Hyphenation.Hyphenation Hyphenate(char[] w, int offset, int len, int remainCharCount
            , int pushCharCount) {
            int i;
            char[] word = new char[len + 3];
            // normalize word
            char[] c = new char[2];
            int iIgnoreAtBeginning = 0;
            int iLength = len;
            bool bEndOfLetters = false;
            for (i = 1; i <= len; i++) {
                c[0] = w[offset + i - 1];
                int nc = classmap.Find(c, 0);
                // found a non-letter character ...
                if (nc < 0) {
                    if (i == (1 + iIgnoreAtBeginning)) {
                        // ... before any letter character
                        iIgnoreAtBeginning++;
                    }
                    else {
                        // ... after a letter character
                        bEndOfLetters = true;
                    }
                    iLength--;
                }
                else {
                    if (!bEndOfLetters) {
                        word[i - iIgnoreAtBeginning] = (char)nc;
                    }
                    else {
                        return null;
                    }
                }
            }
            len = iLength;
            if (len < (remainCharCount + pushCharCount)) {
                // word is too short to be hyphenated
                return null;
            }
            int[] result = new int[len + 1];
            int k = 0;
            // check exception list first
            String sw = new String(word, 1, len);
            if (stoplist.ContainsKey(sw)) {
                // assume only simple hyphens (Hyphen.pre="-", Hyphen.post = Hyphen.no = null)
                ArrayList hw = (ArrayList)stoplist.Get(sw);
                int j = 0;
                for (i = 0; i < hw.Count; i++) {
                    Object o = hw[i];
                    // j = index(sw) = letterindex(word)?
                    // result[k] = corresponding index(w)
                    if (o is String) {
                        j += ((String)o).Length;
                        if (j >= remainCharCount && j < (len - pushCharCount)) {
                            result[k++] = j + iIgnoreAtBeginning;
                        }
                    }
                }
            }
            else {
                // use algorithm to get hyphenation points
                // word start marker
                word[0] = '.';
                // word end marker
                word[len + 1] = '.';
                // null terminated
                word[len + 2] = (char)0;
                // initialized to zero
                byte[] il = new byte[len + 3];
                for (i = 0; i < len + 1; i++) {
                    SearchPatterns(word, i, il);
                }
                // hyphenation points are located where interletter value is odd
                // i is letterindex(word),
                // i + 1 is index(word),
                // result[k] = corresponding index(w)
                for (i = 0; i < len; i++) {
                    if (((il[i + 1] & 1) == 1) && i >= remainCharCount && i <= (len - pushCharCount)) {
                        result[k++] = i;
                    }
                }
            }
            if (k > 0) {
                // trim result array
                int[] res = new int[k];
                Array.Copy(result, 0, res, 0, k);
                return new iText.Layout.Hyphenation.Hyphenation(new String(w, iIgnoreAtBeginning, len), res);
            }
            else {
                return null;
            }
        }

        /// <summary>Add a character class to the tree.</summary>
        /// <remarks>
        /// Add a character class to the tree. It is used by
        /// <see cref="PatternParser">PatternParser</see>
        /// as callback to
        /// add character classes. Character classes define the
        /// valid word characters for hyphenation. If a word contains
        /// a character not defined in any of the classes, it is not hyphenated.
        /// It also defines a way to normalize the characters in order
        /// to compare them with the stored patterns. Usually pattern
        /// files use only lower case characters, in this case a class
        /// for letter 'a', for example, should be defined as "aA", the first
        /// character being the normalization char.
        /// </remarks>
        /// <param name="chargroup">a character class (group)</param>
        public virtual void AddClass(String chargroup) {
            if (chargroup.Length > 0) {
                char equivChar = chargroup[0];
                char[] key = new char[2];
                key[1] = (char)0;
                for (int i = 0; i < chargroup.Length; i++) {
                    key[0] = chargroup[i];
                    classmap.Insert(key, 0, equivChar);
                }
            }
        }

        /// <summary>Add an exception to the tree.</summary>
        /// <remarks>
        /// Add an exception to the tree. It is used by
        /// <see cref="PatternParser">PatternParser</see>
        /// class as callback to
        /// store the hyphenation exceptions.
        /// </remarks>
        /// <param name="word">normalized word</param>
        /// <param name="hyphenatedword">
        /// a vector of alternating strings and
        /// <see cref="Hyphen">hyphen</see>
        /// objects.
        /// </param>
        public virtual void AddException(String word, IList hyphenatedword) {
            stoplist.Put(word, hyphenatedword);
        }

        /// <summary>Add a pattern to the tree.</summary>
        /// <remarks>
        /// Add a pattern to the tree. Mainly, to be used by
        /// <see cref="PatternParser">PatternParser</see>
        /// class as callback to
        /// add a pattern to the tree.
        /// </remarks>
        /// <param name="pattern">the hyphenation pattern</param>
        /// <param name="ivalue">
        /// interletter weight values indicating the
        /// desirability and priority of hyphenating at a given point
        /// within the pattern. It should contain only digit characters.
        /// (i.e. '0' to '9').
        /// </param>
        public virtual void AddPattern(String pattern, String ivalue) {
            int k = ivalues.Find(ivalue);
            if (k <= 0) {
                k = PackValues(ivalue);
                ivalues.Insert(ivalue, (char)k);
            }
            Insert(pattern, (char)k);
        }
    }
}
