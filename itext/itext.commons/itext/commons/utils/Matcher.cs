/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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

namespace iText.Commons.Utils {
    public class Matcher {

        private readonly Regex pattern;
        private readonly String input;
        private int startRegion;
        private int endRegion;
        
        private System.Text.RegularExpressions.Match matcher;

        private Matcher(Regex pattern, String input) {
            this.pattern = pattern;
            this.input = input;
            this.startRegion = 0;
            this.endRegion = input.Length;
        }

        public static Matcher Match(Regex pattern, String input) {
            return new Matcher(pattern, input);
        }
        
        public int Start() {
            CheckMatchFound();
            return startRegion + matcher.Index;
        }
        
        public int End() {
            CheckMatchFound();
            return startRegion + matcher.Index + matcher.Length;
        }
        
        public String Group() {
            return Group(0);
        }
        
        public String Group(int group) {
            CheckMatchFound();

            if (group < 0 || group >= matcher.Groups.Count) {
                throw new IndexOutOfRangeException("No group " + group);
            }

            return matcher.Groups[group].Success ? matcher.Groups[group].Value : null;
        }

        public bool Matches() {
            matcher = null;
            // Some differences between this implementation results and Java:
            // 1. If matches fails, then Java would throw exceptions on obtaining results methods (such as 
            //    start(), end(), group(), etc.), while this implementation would not. We do not expect that
            //    such difference would be important as it is not expected to call this methods in case when
            //    matches fails (i.e. the check on matches result is required).
            // 2. The matches result can differ if reluctant (lazy) or possessive quantifiers are used in regex.
            //    It is not expected to use such quantifiers with matches call.
            return Find() && Start() == startRegion && End() == endRegion;
        }
        
        public bool Find() {
            matcher = matcher == null ? 
                pattern.Match(input.Substring(startRegion, endRegion - startRegion)) : 
                matcher.NextMatch();
            return matcher.Success;
        }
        
        public bool Find(int start) {
            if (start < 0 || start > input.Length) {
                throw new IndexOutOfRangeException("Illegal start index");
            }
            startRegion = 0;
            endRegion = input.Length;
            matcher = pattern.Match(input, start);
            return matcher.Success;
        }

        public Matcher Region(int start, int end)
        {
            if ((start < 0) || (start > input.Length))
                throw new IndexOutOfRangeException("start");
            if ((end < 0) || (end > input.Length))
                throw new IndexOutOfRangeException("end");
            if (start > end)
                throw new IndexOutOfRangeException("start > end");
            startRegion = start;
            endRegion = end;
            matcher = null;
            return this;
        }

        private void CheckMatchFound() {
            if (matcher == null || !matcher.Success) {
                throw new InvalidOperationException("No match found");
            }
        }
    }
}
