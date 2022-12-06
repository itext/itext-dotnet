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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Source;

namespace iText.IO.Font.Otf {
    public class OtfReadCommon {
        public static int[] ReadUShortArray(RandomAccessFileOrArray rf, int size, int location) {
            int[] ret = new int[size];
            for (int k = 0; k < size; ++k) {
                int offset = rf.ReadUnsignedShort();
                ret[k] = offset == 0 ? offset : offset + location;
            }
            return ret;
        }

        public static int[] ReadUShortArray(RandomAccessFileOrArray rf, int size) {
            return ReadUShortArray(rf, size, 0);
        }

        public static void ReadCoverages(RandomAccessFileOrArray rf, int[] locations, IList<ICollection<int>> coverage
            ) {
            foreach (int location in locations) {
                coverage.Add(new HashSet<int>(ReadCoverageFormat(rf, location)));
            }
        }

        public static IList<int> ReadCoverageFormat(RandomAccessFileOrArray rf, int coverageLocation) {
            rf.Seek(coverageLocation);
            int coverageFormat = rf.ReadShort();
            IList<int> glyphIds;
            if (coverageFormat == 1) {
                int glyphCount = rf.ReadUnsignedShort();
                glyphIds = new List<int>(glyphCount);
                for (int i = 0; i < glyphCount; i++) {
                    int coverageGlyphId = rf.ReadUnsignedShort();
                    glyphIds.Add(coverageGlyphId);
                }
            }
            else {
                if (coverageFormat == 2) {
                    int rangeCount = rf.ReadUnsignedShort();
                    glyphIds = new List<int>();
                    for (int i = 0; i < rangeCount; i++) {
                        ReadRangeRecord(rf, glyphIds);
                    }
                }
                else {
                    throw new NotSupportedException(MessageFormatUtil.Format("Invalid coverage format: {0}", coverageFormat));
                }
            }
            return JavaCollectionsUtil.UnmodifiableList(glyphIds);
        }

        private static void ReadRangeRecord(RandomAccessFileOrArray rf, IList<int> glyphIds) {
            int startGlyphId = rf.ReadUnsignedShort();
            int endGlyphId = rf.ReadUnsignedShort();
            int startCoverageIndex = rf.ReadShort();
            for (int glyphId = startGlyphId; glyphId <= endGlyphId; glyphId++) {
                glyphIds.Add(glyphId);
            }
        }

        public static GposValueRecord ReadGposValueRecord(OpenTypeFontTableReader tableReader, int mask) {
            GposValueRecord vr = new GposValueRecord();
            if ((mask & 0x0001) != 0) {
                vr.XPlacement = FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ();
            }
            if ((mask & 0x0002) != 0) {
                vr.YPlacement = FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ();
            }
            if ((mask & 0x0004) != 0) {
                vr.XAdvance = FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ();
            }
            if ((mask & 0x0008) != 0) {
                vr.YAdvance = FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ();
            }
            if ((mask & 0x0010) != 0) {
                tableReader.rf.Skip(2);
            }
            if ((mask & 0x0020) != 0) {
                tableReader.rf.Skip(2);
            }
            if ((mask & 0x0040) != 0) {
                tableReader.rf.Skip(2);
            }
            if ((mask & 0x0080) != 0) {
                tableReader.rf.Skip(2);
            }
            return vr;
        }

        public static GposAnchor ReadGposAnchor(OpenTypeFontTableReader tableReader, int location) {
            if (location == 0) {
                return null;
            }
            tableReader.rf.Seek(location);
            int format = tableReader.rf.ReadUnsignedShort();
            GposAnchor t = null;
            switch (format) {
                default: {
                    t = new GposAnchor();
                    t.XCoordinate = FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                        ();
                    t.YCoordinate = FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                        ();
                    break;
                }
            }
            return t;
        }

        public static IList<OtfMarkRecord> ReadMarkArray(OpenTypeFontTableReader tableReader, int location) {
            tableReader.rf.Seek(location);
            int markCount = tableReader.rf.ReadUnsignedShort();
            int[] classes = new int[markCount];
            int[] locations = new int[markCount];
            for (int k = 0; k < markCount; ++k) {
                classes[k] = tableReader.rf.ReadUnsignedShort();
                int offset = tableReader.rf.ReadUnsignedShort();
                locations[k] = location + offset;
            }
            IList<OtfMarkRecord> marks = new List<OtfMarkRecord>();
            for (int k = 0; k < markCount; ++k) {
                OtfMarkRecord rec = new OtfMarkRecord();
                rec.markClass = classes[k];
                rec.anchor = ReadGposAnchor(tableReader, locations[k]);
                marks.Add(rec);
            }
            return marks;
        }

        public static SubstLookupRecord[] ReadSubstLookupRecords(RandomAccessFileOrArray rf, int substCount) {
            SubstLookupRecord[] substLookUpRecords = new SubstLookupRecord[substCount];
            for (int i = 0; i < substCount; ++i) {
                SubstLookupRecord slr = new SubstLookupRecord();
                slr.sequenceIndex = rf.ReadUnsignedShort();
                slr.lookupListIndex = rf.ReadUnsignedShort();
                substLookUpRecords[i] = slr;
            }
            return substLookUpRecords;
        }

        public static PosLookupRecord[] ReadPosLookupRecords(RandomAccessFileOrArray rf, int recordCount) {
            PosLookupRecord[] posLookUpRecords = new PosLookupRecord[recordCount];
            for (int i = 0; i < recordCount; ++i) {
                PosLookupRecord lookupRecord = new PosLookupRecord();
                lookupRecord.sequenceIndex = rf.ReadUnsignedShort();
                lookupRecord.lookupListIndex = rf.ReadUnsignedShort();
                posLookUpRecords[i] = lookupRecord;
            }
            return posLookUpRecords;
        }

        public static GposAnchor[] ReadAnchorArray(OpenTypeFontTableReader tableReader, int[] locations, int left, 
            int right) {
            GposAnchor[] anchors = new GposAnchor[right - left];
            for (int i = left; i < right; i++) {
                anchors[i - left] = ReadGposAnchor(tableReader, locations[i]);
            }
            return anchors;
        }

        public static IList<GposAnchor[]> ReadBaseArray(OpenTypeFontTableReader tableReader, int classCount, int location
            ) {
            IList<GposAnchor[]> baseArray = new List<GposAnchor[]>();
            tableReader.rf.Seek(location);
            int baseCount = tableReader.rf.ReadUnsignedShort();
            int[] anchorLocations = ReadUShortArray(tableReader.rf, baseCount * classCount, location);
            int idx = 0;
            for (int k = 0; k < baseCount; ++k) {
                baseArray.Add(ReadAnchorArray(tableReader, anchorLocations, idx, idx + classCount));
                idx += classCount;
            }
            return baseArray;
        }

        public static IList<IList<GposAnchor[]>> ReadLigatureArray(OpenTypeFontTableReader tableReader, int classCount
            , int location) {
            IList<IList<GposAnchor[]>> ligatureArray = new List<IList<GposAnchor[]>>();
            tableReader.rf.Seek(location);
            int ligatureCount = tableReader.rf.ReadUnsignedShort();
            int[] ligatureAttachLocations = ReadUShortArray(tableReader.rf, ligatureCount, location);
            for (int liga = 0; liga < ligatureCount; ++liga) {
                int ligatureAttachLocation = ligatureAttachLocations[liga];
                IList<GposAnchor[]> ligatureAttach = new List<GposAnchor[]>();
                tableReader.rf.Seek(ligatureAttachLocation);
                int componentCount = tableReader.rf.ReadUnsignedShort();
                int[] componentRecordsLocation = ReadUShortArray(tableReader.rf, classCount * componentCount, ligatureAttachLocation
                    );
                int idx = 0;
                for (int k = 0; k < componentCount; ++k) {
                    ligatureAttach.Add(ReadAnchorArray(tableReader, componentRecordsLocation, idx, idx + classCount));
                    idx += classCount;
                }
                ligatureArray.Add(ligatureAttach);
            }
            return ligatureArray;
        }
    }
}
