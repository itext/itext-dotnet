/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.IO.Font.Otf {
    /// <summary>LookupType 1: Single Substitution Subtable</summary>
    /// <author>psoares</author>
    public class GsubLookupType1 : OpenTableLookup {
        private IntHashtable substMap;

        public GsubLookupType1(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            substMap = new IntHashtable();
            ReadSubTables();
        }

        public override bool TransformOne(GlyphLine line) {
            if (line.idx >= line.end) {
                return false;
            }
            Glyph g = line.Get(line.idx);
            bool changed = false;
            if (!openReader.IsSkip(g.GetCode(), lookupFlag)) {
                int substCode = substMap.Get(g.GetCode());
                // there is no need to substitute a symbol with itself
                if (substCode != 0 && substCode != g.GetCode()) {
                    line.SubstituteOneToOne(openReader, substCode);
                    changed = true;
                }
            }
            line.idx++;
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            int substFormat = openReader.rf.ReadShort();
            if (substFormat == 1) {
                int coverage = openReader.rf.ReadUnsignedShort();
                int deltaGlyphID = openReader.rf.ReadShort();
                IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverage);
                foreach (int coverageGlyphId in coverageGlyphIds) {
                    int substituteGlyphId = coverageGlyphId + deltaGlyphID;
                    substMap.Put(coverageGlyphId, substituteGlyphId);
                }
            }
            else {
                if (substFormat == 2) {
                    int coverage = openReader.rf.ReadUnsignedShort();
                    int glyphCount = openReader.rf.ReadUnsignedShort();
                    int[] substitute = new int[glyphCount];
                    for (int k = 0; k < glyphCount; ++k) {
                        substitute[k] = openReader.rf.ReadUnsignedShort();
                    }
                    IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverage);
                    for (int k = 0; k < glyphCount; ++k) {
                        substMap.Put(coverageGlyphIds[k], substitute[k]);
                    }
                }
                else {
                    throw new ArgumentException("Bad substFormat: " + substFormat);
                }
            }
        }

        public override bool HasSubstitution(int index) {
            return substMap.ContainsKey(index);
        }
    }
}
