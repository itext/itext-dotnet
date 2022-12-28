/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
