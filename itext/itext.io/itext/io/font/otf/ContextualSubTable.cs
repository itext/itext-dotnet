/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System.Collections.Generic;

namespace iText.IO.Font.Otf {
    public abstract class ContextualSubTable {
        protected internal OpenTypeFontTableReader openReader;

        protected internal int lookupFlag;

        protected internal ContextualSubTable(OpenTypeFontTableReader openReader, int lookupFlag) {
            this.openReader = openReader;
            this.lookupFlag = lookupFlag;
        }

        /// <summary>Gets a most preferable context rule that matches the line at current position.</summary>
        /// <remarks>
        /// Gets a most preferable context rule that matches the line at current position. If no matching context rule is found,
        /// it returns null.
        /// <br /><br />
        /// NOTE: if matching context rule is found, the <code>GlyphLine.start</code> and <code>GlyphLine.end</code> will be
        /// changed in such way, that they will point at start and end of the matching context glyph sequence inside the glyph line.
        /// </remarks>
        /// <param name="line">a line, which is to be checked if it matches some context.</param>
        /// <returns>matching context rule or null, if none was found.</returns>
        public virtual ContextualSubstRule GetMatchingContextRule(GlyphLine line) {
            if (line.idx >= line.end) {
                return null;
            }
            Glyph g = line.Get(line.idx);
            IList<ContextualSubstRule> rules = GetSetOfRulesForStartGlyph(g.GetCode());
            foreach (ContextualSubstRule rule in rules) {
                int lastGlyphIndex = CheckIfContextMatch(line, rule);
                if (lastGlyphIndex != -1) {
                    line.start = line.idx;
                    line.end = lastGlyphIndex + 1;
                    return rule;
                }
            }
            return null;
        }

        /// <summary>Gets a set of rules, which start with given glyph id.</summary>
        /// <param name="startId">id of the first glyph in the sequence.</param>
        protected internal abstract IList<ContextualSubstRule> GetSetOfRulesForStartGlyph(int startId);

        /// <summary>Checks if given glyph line at the given position matches given rule.</summary>
        /// <returns>
        /// either index which corresponds to the last glyph of the matching context inside the glyph line if context matches,
        /// or -1 if context doesn't match.
        /// </returns>
        protected internal virtual int CheckIfContextMatch(GlyphLine line, ContextualSubstRule rule) {
            int j;
            OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
            gidx.line = line;
            gidx.idx = line.idx;
            //Note, that starting index shall be 1
            for (j = 1; j < rule.GetContextLength(); ++j) {
                gidx.NextGlyph(openReader, lookupFlag);
                if (gidx.glyph == null || !rule.IsGlyphMatchesInput(gidx.glyph.GetCode(), j)) {
                    break;
                }
            }
            bool isMatch = j == rule.GetContextLength();
            if (isMatch) {
                return gidx.idx;
            }
            else {
                return -1;
            }
        }
    }
}
