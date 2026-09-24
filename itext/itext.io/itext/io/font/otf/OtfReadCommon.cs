/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Source;

namespace iText.IO.Font.Otf {
    /// <summary>Represents the OpenType font reading utility.</summary>
    public class OtfReadCommon {
        /// <summary>Reads the ushort array from OpenType data.</summary>
        /// <param name="rf">the raw source</param>
        /// <param name="size">the size</param>
        /// <param name="location">the location</param>
        /// <returns>the requested result</returns>
        public static int[] ReadUShortArray(RandomAccessFileOrArray rf, int size, int location) {
            int[] ret = new int[size];
            for (int k = 0; k < size; ++k) {
                int offset = rf.ReadUnsignedShort();
                ret[k] = offset == 0 ? offset : offset + location;
            }
            return ret;
        }

        /// <summary>Reads the ushort array from OpenType data</summary>
        /// <param name="rf">the raw source</param>
        /// <param name="size">the size</param>
        /// <returns>the requested result</returns>
        public static int[] ReadUShortArray(RandomAccessFileOrArray rf, int size) {
            return ReadUShortArray(rf, size, 0);
        }

        /// <summary>Reads the coverages from OpenType data.</summary>
        /// <param name="rf">the raw source</param>
        /// <param name="locations">the locations</param>
        /// <param name="coverage">the coverage</param>
        public static void ReadCoverages(RandomAccessFileOrArray rf, int[] locations, IList<ICollection<int>> coverage
            ) {
            foreach (int location in locations) {
                coverage.Add(new HashSet<int>(ReadCoverageFormat(rf, location)));
            }
        }

        /// <summary>Reads the coverage format from OpenType data.</summary>
        /// <param name="rf">the raw source</param>
        /// <param name="coverageLocation">the coverage location</param>
        /// <returns>the requested result</returns>
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

        /// <summary>Reads the GPOS value record from OpenType data.</summary>
        /// <param name="tableReader">the table reader</param>
        /// <param name="mask">the mask</param>
        /// <returns>the requested result</returns>
        public static GposValueRecord ReadGposValueRecord(OpenTypeFontTableReader tableReader, int mask) {
            GposValueRecord vr = new GposValueRecord();
            if ((mask & 0x0001) != 0) {
                vr.SetXPlacement(FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ());
            }
            if ((mask & 0x0002) != 0) {
                vr.SetYPlacement(FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ());
            }
            if ((mask & 0x0004) != 0) {
                vr.SetXAdvance(FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ());
            }
            if ((mask & 0x0008) != 0) {
                vr.SetYAdvance(FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                    ());
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

        /// <summary>Reads the GPOS anchor from OpenType data.</summary>
        /// <param name="tableReader">the table reader</param>
        /// <param name="location">the location</param>
        /// <returns>the requested result</returns>
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
                    t.SetXCoordinate(FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                        ());
                    t.SetYCoordinate(FontProgram.ConvertGlyphSpaceToTextSpace(tableReader.rf.ReadShort()) / tableReader.GetUnitsPerEm
                        ());
                    break;
                }
            }
            return t;
        }

        /// <summary>Reads the mark array from OpenType data.</summary>
        /// <param name="tableReader">the table reader</param>
        /// <param name="location">the location</param>
        /// <returns>the requested result</returns>
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
                rec.SetMarkClass(classes[k]);
                rec.SetAnchor(ReadGposAnchor(tableReader, locations[k]));
                marks.Add(rec);
            }
            return marks;
        }

        /// <summary>Reads the substitution lookup records from OpenType data.</summary>
        /// <param name="rf">the raw source</param>
        /// <param name="substCount">the subst count</param>
        /// <returns>the requested result</returns>
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

        /// <summary>Reads the positioning lookup records from OpenType data.</summary>
        /// <param name="rf">the raw source</param>
        /// <param name="recordCount">the record count</param>
        /// <returns>the requested result</returns>
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

        /// <summary>Reads the anchor array from OpenType data.</summary>
        /// <param name="tableReader">the table reader</param>
        /// <param name="locations">the locations</param>
        /// <param name="left">the left</param>
        /// <param name="right">the right</param>
        /// <returns>the requested result</returns>
        public static GposAnchor[] ReadAnchorArray(OpenTypeFontTableReader tableReader, int[] locations, int left, 
            int right) {
            GposAnchor[] anchors = new GposAnchor[right - left];
            for (int i = left; i < right; i++) {
                anchors[i - left] = ReadGposAnchor(tableReader, locations[i]);
            }
            return anchors;
        }

        /// <summary>Reads the base array from OpenType data.</summary>
        /// <param name="tableReader">the table reader</param>
        /// <param name="classCount">the class count</param>
        /// <param name="location">the location</param>
        /// <returns>the requested result</returns>
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

        /// <summary>Reads the ligature array from OpenType data.</summary>
        /// <param name="tableReader">the table reader</param>
        /// <param name="classCount">the class count</param>
        /// <param name="location">the location</param>
        /// <returns>the requested result</returns>
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
