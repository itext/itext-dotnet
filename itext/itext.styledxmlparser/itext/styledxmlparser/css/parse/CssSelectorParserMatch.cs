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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Internal class not for public use.</summary>
    /// <remarks>Internal class not for public use. Its API may change.</remarks>
    internal class CssSelectorParserMatch {
        private bool success;

        private Matcher matcher;

        private String source;

        /// <summary>Construct a new CssSelectorParserMatch</summary>
        /// <param name="source">the text being matched</param>
        /// <param name="pattern">the pattern against which the text is being matched</param>
        public CssSelectorParserMatch(String source, Regex pattern) {
            this.source = source;
            this.matcher = Matcher.Match(pattern, source);
            Next();
        }

        /// <summary>Get the index at which the last match started</summary>
        public virtual int GetIndex() {
            return matcher.Start();
        }

        /// <summary>Get the text of the last match</summary>
        public virtual String GetValue() {
            return matcher.Group(0);
        }

        /// <summary>Get the source text being matched</summary>
        public virtual String GetSource() {
            return source;
        }

        /// <summary>Return whether or not the match was successful</summary>
        public virtual bool Success() {
            return success;
        }

        /// <summary>Attempt to match the pattern against the next piece of the source text</summary>
        public virtual void Next() {
            success = matcher.Find();
        }

        /// <summary>Get the index at which the next match of the pattern takes place</summary>
        /// <param name="startIndex">the index at which to start matching the pattern</param>
        public virtual void Next(int startIndex) {
            success = matcher.Find(startIndex);
        }
    }
}
