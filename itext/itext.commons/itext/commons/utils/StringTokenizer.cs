/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Commons.Utils {

    // a replacement for the StringTokenizer java class
    public class StringTokenizer {

        private int pos;
        private String str;
        private int len;
        private String delim;
        private bool retDelims;

        public StringTokenizer(String str) : this(str, " \t\n\r\f", false) {
        }

        public StringTokenizer(String str, String delim) : this(str, delim, false) {
        }

        public StringTokenizer(String str, String delim, bool retDelims) {
            len = str.Length;
            this.str = str;
            this.delim = delim;
            this.retDelims = retDelims;
            this.pos = 0;
        }

        public virtual bool HasMoreTokens() {
            if (! retDelims) {
                while (pos < len && delim.IndexOf(str[pos]) >= 0)
                    pos++;
            }
            return pos < len;
        }

        public virtual String NextToken(String delim) {
            this.delim = delim;
            return NextToken();
        }

        public virtual String NextToken() {
            if (pos < len && delim.IndexOf(str[pos]) >= 0) {
                if (retDelims)
                    return str.Substring(pos++, 1);
                while (++pos < len && delim.IndexOf(str[pos]) >= 0);
            }
            if (pos < len) {
                int start = pos;
                while (++pos < len && delim.IndexOf(str[pos]) < 0);

                return str.Substring(start, pos - start);
            }
            throw new IndexOutOfRangeException();
        }

        public virtual int CountTokens() {
            int count = 0;
            int delimiterCount = 0;
            bool tokenFound = false;
            int tmpPos = pos;

            while (tmpPos < len) {
                if (delim.IndexOf(str[tmpPos++]) >= 0) {
                    if (tokenFound) {
                        count++;
                        tokenFound = false;
                    }
                    delimiterCount++;
                }
                else {
                    tokenFound = true;
                    while (tmpPos < len
                        && delim.IndexOf(str[tmpPos]) < 0)
                        ++tmpPos;
                }
            }
            if (tokenFound)
                count++;
            return retDelims ? count + delimiterCount : count;
        }
    }
}
