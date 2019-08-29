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

namespace iText.Layout.Hyphenation {
    /// <summary>
    /// This interface is used to connect the XML pattern file parser to
    /// the hyphenation tree.
    /// </summary>
    /// <remarks>
    /// This interface is used to connect the XML pattern file parser to
    /// the hyphenation tree.
    /// <para />
    /// This work was authored by Carlos Villegas (cav@uniscope.co.jp).
    /// </remarks>
    public interface IPatternConsumer {
        /// <summary>Add a character class.</summary>
        /// <remarks>
        /// Add a character class.
        /// A character class defines characters that are considered
        /// equivalent for the purpose of hyphenation (e.g. "aA"). It
        /// usually means to ignore case.
        /// </remarks>
        /// <param name="chargroup">character group</param>
        void AddClass(String chargroup);

        /// <summary>Add a hyphenation exception.</summary>
        /// <remarks>
        /// Add a hyphenation exception. An exception replaces the
        /// result obtained by the algorithm for cases for which this
        /// fails or the user wants to provide his own hyphenation.
        /// A hyphenatedword is a vector of alternating String's and
        /// <see cref="Hyphen">Hyphen</see>
        /// instances
        /// </remarks>
        /// <param name="word">word to add as an exception</param>
        /// <param name="hyphenatedword">pre-hyphenated word</param>
        void AddException(String word, IList hyphenatedword);

        /// <summary>Add hyphenation patterns.</summary>
        /// <param name="pattern">the pattern</param>
        /// <param name="values">
        /// interletter values expressed as a string of
        /// digit characters.
        /// </param>
        void AddPattern(String pattern, String values);
    }
}
