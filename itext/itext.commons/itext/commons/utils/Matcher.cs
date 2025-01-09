/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
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
