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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Source;

namespace iText.IO.Font {
//\cond DO_NOT_DOCUMENT
    /// <summary>The abstract class which provides a functionality to modify TrueType fonts with returning raw data of modified font.
    ///     </summary>
    internal abstract class AbstractTrueTypeFontModifier {
        // If it's a regular font subset, we should not add `name` and `post`,
        // because information in these tables maybe irrelevant for a subset.
        private static readonly String[] TABLE_NAMES_SUBSET = new String[] { "cvt ", "fpgm", "glyf", "head", "hhea"
            , "hmtx", "loca", "maxp", "prep", "cmap", "OS/2" };

        // In case ttc (true type collection) file with subset = false (#directoryOffset > 0)
        // `name` and `post` shall be included, because it's actually a full font.
        private static readonly String[] TABLE_NAMES = new String[] { "cvt ", "fpgm", "glyf", "head", "hhea", "hmtx"
            , "loca", "maxp", "prep", "cmap", "OS/2", "name", "post" };

        private static readonly int[] ENTRY_SELECTORS = new int[] { 0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3
            , 4, 4, 4, 4, 4 };

        private const int TABLE_CHECKSUM = 0;

        private const int TABLE_OFFSET = 1;

        private const int TABLE_LENGTH = 2;

        private const int HEAD_LOCA_FORMAT_OFFSET = 51;

        /// <summary>Contains the location of the several tables.</summary>
        /// <remarks>
        /// Contains the location of the several tables. The key is the name of the table and the
        /// value is an
        /// <c>int[3]</c>
        /// where position 0 is the checksum, position 1 is the offset
        /// from the start of the file and position 2 is the length of the table.
        /// <para />
        /// The length of the table can be any. But when writing data, it should be aligned to 4 bytes. Say, if table length
        /// is 54 we should write 2 extra zeroes at the end, and it should be taken into account for calculating the start
        /// offset of the next table (but not the length of the current table).
        /// </remarks>
        protected internal IDictionary<String, int[]> tableDirectory;

        /// <summary>Contains glyph data from `glyf` table.</summary>
        /// <remarks>Contains glyph data from `glyf` table. The key is the GID (glyph ID) and the value is a raw data of glyph.
        ///     </remarks>
        protected internal IDictionary<int, byte[]> glyphDataMap;

        /// <summary>Contains font tables which have been changed during the modification.</summary>
        /// <remarks>
        /// Contains font tables which have been changed during the modification. The key is the name of table which has
        /// been changed and the value is raw data of the modified table.
        /// </remarks>
        protected internal readonly IDictionary<String, byte[]> modifiedTables = new Dictionary<String, byte[]>();

        /// <summary>Font raw data on which new modified font will be built.</summary>
        protected internal RandomAccessFileOrArray raf;

        /// <summary>
        /// Table directory offset which corresponds to font's raw data from
        /// <see cref="raf"/>.
        /// </summary>
        protected internal int directoryOffset;

        /// <summary>the name of font which will be modified</summary>
        protected internal readonly String fontName;

        protected internal IDictionary<int, byte[]> horizontalMetricMap;

        protected internal int numberOfHMetrics;

        private AbstractTrueTypeFontModifier.FontRawData outFont;

        private readonly String[] tableNames;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Instantiates a new
        /// <see cref="AbstractTrueTypeFontModifier"/>
        /// instance.
        /// </summary>
        /// <param name="fontName">the name of font which will be modified</param>
        /// <param name="subsetTables">
        /// whether subset tables (remove `name` and `post` tables) or not. It's used in case of ttc
        /// (true type collection) font where single "full" font is needed. Despite the value of that
        /// flag, only used glyphs will be left in the font
        /// </param>
        internal AbstractTrueTypeFontModifier(String fontName, bool subsetTables) {
            // subset = false is possible with directoryOffset > 0, i.e. ttc font without subset.
            if (subsetTables) {
                tableNames = TABLE_NAMES_SUBSET;
            }
            else {
                tableNames = TABLE_NAMES;
            }
            this.fontName = fontName;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual Tuple2<int, byte[]> Process() {
            try {
                CreateTableDirectory();
                int numberOfGlyphs = MergeTables();
                AssembleFont();
                return new Tuple2<int, byte[]>(numberOfGlyphs, outFont.GetData());
            }
            finally {
                try {
                    raf.Close();
                }
                catch (Exception) {
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract int MergeTables();
//\endcond

        protected internal virtual int CreateModifiedTables() {
            int[] activeGlyphs = new int[glyphDataMap.Count];
            int i = 0;
            int glyfSize = 0;
            foreach (KeyValuePair<int, byte[]> entry in glyphDataMap) {
                activeGlyphs[i++] = entry.Key;
                glyfSize += entry.Value.Length;
            }
            JavaUtil.Sort(activeGlyphs);
            int maxGlyphId = activeGlyphs[activeGlyphs.Length - 1];
            // Update loca and glyf tables
            // If the biggest used GID is X, size of loca should be X + 1 (array index starts from 0),
            // plus one extra entry to get size of X element (loca[X + 1] - loca[X]), it's why 2 added
            int locaSize = maxGlyphId + 2;
            bool isLocaShortTable = IsLocaShortTable();
            int newLocaTableSize = isLocaShortTable ? locaSize * 2 : locaSize * 4;
            byte[] newLoca = new byte[newLocaTableSize];
            byte[] newGlyf = new byte[glyfSize];
            int glyfPtr = 0;
            int listGlyf = 0;
            for (int k = 0; k < locaSize; ++k) {
                WriteToLoca(newLoca, k, glyfPtr, isLocaShortTable);
                if (listGlyf < activeGlyphs.Length && activeGlyphs[listGlyf] == k) {
                    ++listGlyf;
                    byte[] glyphData = glyphDataMap.Get(k);
                    Array.Copy(glyphData, 0, newGlyf, glyfPtr, glyphData.Length);
                    glyfPtr += glyphData.Length;
                }
            }
            modifiedTables.Put("glyf", newGlyf);
            modifiedTables.Put("loca", newLoca);
            // Update maxp table
            int[] tableLocation = tableDirectory.Get("maxp");
            raf.Seek(tableLocation[TABLE_OFFSET]);
            byte[] maxp = new byte[tableLocation[TABLE_LENGTH]];
            raf.Read(maxp);
            WriteShortToTable(maxp, 2, maxGlyphId + 1);
            modifiedTables.Put("maxp", maxp);
            // Merging vertical fonts aren't supported yet, it's why vmtx and vhea tables ignored
            // Update hhea table
            // numberOfHMetrics can't be increased because there are no advanced width data. So only
            // numberOfHMetrics decreasing is allowed. See createNewHorizontalMetricsTable method for more info.
            if (numberOfHMetrics > maxGlyphId + 1) {
                int[] hheaTableLocation = tableDirectory.Get("hhea");
                raf.Seek(hheaTableLocation[TABLE_OFFSET]);
                byte[] hhea = new byte[hheaTableLocation[TABLE_LENGTH]];
                raf.Read(hhea);
                WriteShortToTable(hhea, 17, maxGlyphId + 1);
                modifiedTables.Put("hhea", hhea);
            }
            // Update hmtx table
            byte[] newHtmx = CreateNewHorizontalMetricsTable(maxGlyphId);
            modifiedTables.Put("hmtx", newHtmx);
            return maxGlyphId + 1;
        }

        /// <summary>Creates new hmtx table.</summary>
        /// <remarks>
        /// Creates new hmtx table.
        /// <para />
        /// hmtx is a "hybrid" table, it has maxp.numGlyphs indices, for (indices &lt;= hhea.numberOfHMetrics) 4 bytes (advanced
        /// width + left side bearing), for (hhea.numberOfHMetrics &lt; indices &lt;= maxp.numGlyphs) 2 bytes (left side bearing).
        /// </remarks>
        /// <param name="maxGlyphId">the max glyph id</param>
        /// <returns>raw data of new hmtx table</returns>
        private byte[] CreateNewHorizontalMetricsTable(int maxGlyphId) {
            int[] tableLocation = tableDirectory.Get("hmtx");
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            raf.Seek(tableLocation[TABLE_OFFSET]);
            for (int k = 0; k < numberOfHMetrics; ++k) {
                if (k > maxGlyphId) {
                    break;
                }
                if (horizontalMetricMap.ContainsKey(k)) {
                    raf.SkipBytes(4);
                    baos.Write(horizontalMetricMap.Get(k));
                }
                else {
                    baos.Write(raf.ReadByte());
                    baos.Write(raf.ReadByte());
                    baos.Write(raf.ReadByte());
                    baos.Write(raf.ReadByte());
                }
            }
            // The 2nd part of hmtx. Only left side bearings
            for (int k = numberOfHMetrics; k <= maxGlyphId; ++k) {
                if (horizontalMetricMap.ContainsKey(k)) {
                    baos.Write(horizontalMetricMap.Get(k));
                }
                else {
                    baos.Write(new byte[] { 0, 0 });
                }
            }
            return baos.ToArray();
        }

        private void CreateTableDirectory() {
            tableDirectory = new Dictionary<String, int[]>();
            raf.Seek(directoryOffset);
            int id = raf.ReadInt();
            if (id != 0x00010000) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.NOT_AT_TRUE_TYPE_FILE).SetMessageParams
                    (fontName);
            }
            int numTables = raf.ReadUnsignedShort();
            raf.SkipBytes(6);
            for (int k = 0; k < numTables; ++k) {
                String tag = ReadTag();
                int[] tableLocation = new int[3];
                tableLocation[TABLE_CHECKSUM] = raf.ReadInt();
                tableLocation[TABLE_OFFSET] = raf.ReadInt();
                tableLocation[TABLE_LENGTH] = raf.ReadInt();
                tableDirectory.Put(tag, tableLocation);
            }
        }

        private bool IsLocaShortTable() {
            int[] tableLocation = tableDirectory.Get("head");
            if (tableLocation == null) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                    ("head", fontName);
            }
            raf.Seek(tableLocation[TABLE_OFFSET] + HEAD_LOCA_FORMAT_OFFSET);
            return raf.ReadUnsignedShort() == 0;
        }

        private void AssembleFont() {
            int[] tableLocation;
            // Calculate size of the out font
            int fullFontSize = 0;
            int tablesUsed = modifiedTables.Count;
            foreach (String name in tableNames) {
                if (modifiedTables.ContainsKey(name)) {
                    continue;
                }
                tableLocation = tableDirectory.Get(name);
                if (tableLocation == null) {
                    continue;
                }
                tablesUsed++;
                // + 3 & ~3 here and further is to align to 4 bytes
                fullFontSize += tableLocation[TABLE_LENGTH] + 3 & ~3;
            }
            foreach (byte[] table in modifiedTables.Values) {
                fullFontSize += table.Length + 3 & ~3;
            }
            int reference = 16 * tablesUsed + 12;
            fullFontSize += reference;
            outFont = new AbstractTrueTypeFontModifier.FontRawData(fullFontSize);
            // Write font headers + tables directory
            outFont.WriteFontInt(0x00010000);
            outFont.WriteFontShort(tablesUsed);
            int selector = ENTRY_SELECTORS[tablesUsed];
            outFont.WriteFontShort((1 << selector) * 16);
            outFont.WriteFontShort(selector);
            outFont.WriteFontShort((tablesUsed - (1 << selector)) * 16);
            foreach (String name in tableNames) {
                int len;
                tableLocation = tableDirectory.Get(name);
                if (tableLocation == null) {
                    continue;
                }
                outFont.WriteFontString(name);
                if (modifiedTables.ContainsKey(name)) {
                    byte[] table = modifiedTables.Get(name);
                    outFont.WriteFontInt(CalculateChecksum(table));
                    len = table.Length;
                }
                else {
                    outFont.WriteFontInt(tableLocation[TABLE_CHECKSUM]);
                    len = tableLocation[TABLE_LENGTH];
                }
                outFont.WriteFontInt(reference);
                outFont.WriteFontInt(len);
                reference += len + 3 & ~3;
            }
            // Write tables data
            foreach (String name in tableNames) {
                tableLocation = tableDirectory.Get(name);
                if (tableLocation == null) {
                    continue;
                }
                if (modifiedTables.ContainsKey(name)) {
                    byte[] table = modifiedTables.Get(name);
                    outFont.WriteFontTable(table);
                }
                else {
                    raf.Seek(tableLocation[TABLE_OFFSET]);
                    outFont.WriteFontTable(raf, tableLocation[TABLE_LENGTH]);
                }
            }
        }

        private String ReadTag() {
            byte[] buf = new byte[4];
            raf.ReadFully(buf);
            try {
                return iText.Commons.Utils.JavaUtil.GetStringForBytes(buf, PdfEncodings.WINANSI);
            }
            catch (Exception e) {
                throw new iText.IO.Exceptions.IOException("TrueType font", e);
            }
        }

        private static void WriteToLoca(byte[] loca, int index, int location, bool isLocaShortTable) {
            if (isLocaShortTable) {
                index *= 2;
                location /= 2;
                loca[index] = (byte)(location >> 8);
                loca[index + 1] = (byte)location;
            }
            else {
                index *= 4;
                loca[index] = (byte)(location >> 24);
                loca[index + 1] = (byte)(location >> 16);
                loca[index + 2] = (byte)(location >> 8);
                loca[index + 3] = (byte)location;
            }
        }

        private static void WriteShortToTable(byte[] table, int index, int data) {
            // 2 bytes per field
            index *= 2;
            table[index] = (byte)(data >> 8);
            table[index + 1] = (byte)data;
        }

        private int CalculateChecksum(byte[] b) {
            int len = b.Length / 4;
            int v0 = 0;
            int v1 = 0;
            int v2 = 0;
            int v3 = 0;
            int ptr = 0;
            for (int k = 0; k < len; ++k) {
                v3 += b[ptr++] & 0xff;
                v2 += b[ptr++] & 0xff;
                v1 += b[ptr++] & 0xff;
                v0 += b[ptr++] & 0xff;
            }
            return v0 + (v1 << 8) + (v2 << 16) + (v3 << 24);
        }

        private class FontRawData {
            private readonly byte[] data;

            private int ptr;

//\cond DO_NOT_DOCUMENT
            internal FontRawData(int size) {
                this.data = new byte[size];
                this.ptr = 0;
            }
//\endcond

            public virtual byte[] GetData() {
                return data;
            }

//\cond DO_NOT_DOCUMENT
            internal virtual void WriteFontTable(RandomAccessFileOrArray raf, int tableLength) {
                raf.ReadFully(data, ptr, tableLength);
                ptr += tableLength + 3 & ~3;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual void WriteFontTable(byte[] tableData) {
                Array.Copy(tableData, 0, data, ptr, tableData.Length);
                ptr += tableData.Length + 3 & ~3;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual void WriteFontShort(int n) {
                data[ptr++] = (byte)(n >> 8);
                data[ptr++] = (byte)n;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual void WriteFontInt(int n) {
                data[ptr++] = (byte)(n >> 24);
                data[ptr++] = (byte)(n >> 16);
                data[ptr++] = (byte)(n >> 8);
                data[ptr++] = (byte)n;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual void WriteFontString(String s) {
                byte[] b = PdfEncodings.ConvertToBytes(s, PdfEncodings.WINANSI);
                Array.Copy(b, 0, data, ptr, b.Length);
                ptr += b.Length;
            }
//\endcond
        }
    }
//\endcond
}
