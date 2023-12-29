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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Source;

namespace iText.IO.Font {
    /// <summary>Subsets a True Type font by removing the unneeded glyphs from the font.</summary>
    internal class TrueTypeFontSubset {
        // If it's a regular font subset, we should not add `name` and `post`,
        // because information in these tables maybe irrelevant for a subset.
        private static readonly String[] TABLE_NAMES_SUBSET = new String[] { "cvt ", "fpgm", "glyf", "head", "hhea"
            , "hmtx", "loca", "maxp", "prep", "cmap", "OS/2" };

        // In case ttc file with subset = false (#directoryOffset > 0) `name` and `post` shall be included,
        // because it's actually a full font.
        private static readonly String[] TABLE_NAMES = new String[] { "cvt ", "fpgm", "glyf", "head", "hhea", "hmtx"
            , "loca", "maxp", "prep", "cmap", "OS/2", "name", "post" };

        private static readonly int[] entrySelectors = new int[] { 0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 
            4, 4, 4, 4, 4 };

        private const int TABLE_CHECKSUM = 0;

        private const int TABLE_OFFSET = 1;

        private const int TABLE_LENGTH = 2;

        private const int HEAD_LOCA_FORMAT_OFFSET = 51;

        private const int ARG_1_AND_2_ARE_WORDS = 1;

        private const int WE_HAVE_A_SCALE = 8;

        private const int MORE_COMPONENTS = 32;

        private const int WE_HAVE_AN_X_AND_Y_SCALE = 64;

        private const int WE_HAVE_A_TWO_BY_TWO = 128;

        /// <summary>Contains the location of the several tables.</summary>
        /// <remarks>
        /// Contains the location of the several tables. The key is the name of
        /// the table and the value is an
        /// <c>int[3]</c>
        /// where position 0
        /// is the checksum, position 1 is the offset from the start of the file
        /// and position 2 is the length of the table.
        /// </remarks>
        private IDictionary<String, int[]> tableDirectory;

        /// <summary>The file in use.</summary>
        protected internal RandomAccessFileOrArray rf;

        /// <summary>The file name.</summary>
        private String fileName;

        private bool locaShortTable;

        private int[] locaTable;

        private ICollection<int> glyphsUsed;

        private IList<int> glyphsInList;

        private int tableGlyphOffset;

        private int[] newLocaTable;

        private byte[] newLocaTableOut;

        private byte[] newGlyfTable;

        private int glyfTableRealSize;

        private int locaTableRealSize;

        private byte[] outFont;

        private int fontPtr;

        private int directoryOffset;

        private readonly String[] tableNames;

        /// <summary>Creates a new TrueTypeFontSubSet</summary>
        /// <param name="directoryOffset">The offset from the start of the file to the table directory</param>
        /// <param name="fileName">the file name of the font</param>
        /// <param name="glyphsUsed">the glyphs used</param>
        internal TrueTypeFontSubset(String fileName, RandomAccessFileOrArray rf, ICollection<int> glyphsUsed, int 
            directoryOffset, bool subset) {
            this.fileName = fileName;
            this.rf = rf;
            this.glyphsUsed = new HashSet<int>(glyphsUsed);
            this.directoryOffset = directoryOffset;
            // subset = false is possible with directoryOffset > 0, i.e. ttc font without subset.
            if (subset) {
                tableNames = TABLE_NAMES_SUBSET;
            }
            else {
                tableNames = TABLE_NAMES;
            }
            glyphsInList = new List<int>(glyphsUsed);
        }

        /// <summary>Does the actual work of subsetting the font.</summary>
        /// <returns>the subset font</returns>
        internal virtual byte[] Process() {
            try {
                CreateTableDirectory();
                ReadLoca();
                FlatGlyphs();
                CreateNewGlyphTables();
                LocaToBytes();
                AssembleFont();
                return outFont;
            }
            finally {
                try {
                    rf.Close();
                }
                catch (Exception) {
                }
            }
        }

        private void AssembleFont() {
            int[] tableLocation;
            int fullFontSize = 0;
            int tablesUsed = 2;
            foreach (String name in tableNames) {
                if (name.Equals("glyf") || name.Equals("loca")) {
                    continue;
                }
                tableLocation = tableDirectory.Get(name);
                if (tableLocation == null) {
                    continue;
                }
                tablesUsed++;
                fullFontSize += tableLocation[TABLE_LENGTH] + 3 & ~3;
            }
            fullFontSize += newLocaTableOut.Length;
            fullFontSize += newGlyfTable.Length;
            int reference = 16 * tablesUsed + 12;
            fullFontSize += reference;
            outFont = new byte[fullFontSize];
            fontPtr = 0;
            WriteFontInt(0x00010000);
            WriteFontShort(tablesUsed);
            int selector = entrySelectors[tablesUsed];
            WriteFontShort((1 << selector) * 16);
            WriteFontShort(selector);
            WriteFontShort((tablesUsed - (1 << selector)) * 16);
            foreach (String name in tableNames) {
                int len;
                tableLocation = tableDirectory.Get(name);
                if (tableLocation == null) {
                    continue;
                }
                WriteFontString(name);
                switch (name) {
                    case "glyf": {
                        WriteFontInt(CalculateChecksum(newGlyfTable));
                        len = glyfTableRealSize;
                        break;
                    }

                    case "loca": {
                        WriteFontInt(CalculateChecksum(newLocaTableOut));
                        len = locaTableRealSize;
                        break;
                    }

                    default: {
                        WriteFontInt(tableLocation[TABLE_CHECKSUM]);
                        len = tableLocation[TABLE_LENGTH];
                        break;
                    }
                }
                WriteFontInt(reference);
                WriteFontInt(len);
                reference += len + 3 & ~3;
            }
            foreach (String name in tableNames) {
                tableLocation = tableDirectory.Get(name);
                if (tableLocation == null) {
                    continue;
                }
                switch (name) {
                    case "glyf": {
                        Array.Copy(newGlyfTable, 0, outFont, fontPtr, newGlyfTable.Length);
                        fontPtr += newGlyfTable.Length;
                        newGlyfTable = null;
                        break;
                    }

                    case "loca": {
                        Array.Copy(newLocaTableOut, 0, outFont, fontPtr, newLocaTableOut.Length);
                        fontPtr += newLocaTableOut.Length;
                        newLocaTableOut = null;
                        break;
                    }

                    default: {
                        rf.Seek(tableLocation[TABLE_OFFSET]);
                        rf.ReadFully(outFont, fontPtr, tableLocation[TABLE_LENGTH]);
                        fontPtr += tableLocation[TABLE_LENGTH] + 3 & ~3;
                        break;
                    }
                }
            }
        }

        private void CreateTableDirectory() {
            tableDirectory = new Dictionary<String, int[]>();
            rf.Seek(directoryOffset);
            int id = rf.ReadInt();
            if (id != 0x00010000) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.NOT_AT_TRUE_TYPE_FILE).SetMessageParams
                    (fileName);
            }
            int num_tables = rf.ReadUnsignedShort();
            rf.SkipBytes(6);
            for (int k = 0; k < num_tables; ++k) {
                String tag = ReadStandardString(4);
                int[] tableLocation = new int[3];
                tableLocation[TABLE_CHECKSUM] = rf.ReadInt();
                tableLocation[TABLE_OFFSET] = rf.ReadInt();
                tableLocation[TABLE_LENGTH] = rf.ReadInt();
                tableDirectory.Put(tag, tableLocation);
            }
        }

        private void ReadLoca() {
            int[] tableLocation = tableDirectory.Get("head");
            if (tableLocation == null) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                    ("head", fileName);
            }
            rf.Seek(tableLocation[TABLE_OFFSET] + HEAD_LOCA_FORMAT_OFFSET);
            locaShortTable = rf.ReadUnsignedShort() == 0;
            tableLocation = tableDirectory.Get("loca");
            if (tableLocation == null) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                    ("loca", fileName);
            }
            rf.Seek(tableLocation[TABLE_OFFSET]);
            if (locaShortTable) {
                int entries = tableLocation[TABLE_LENGTH] / 2;
                locaTable = new int[entries];
                for (int k = 0; k < entries; ++k) {
                    locaTable[k] = rf.ReadUnsignedShort() * 2;
                }
            }
            else {
                int entries = tableLocation[TABLE_LENGTH] / 4;
                locaTable = new int[entries];
                for (int k = 0; k < entries; ++k) {
                    locaTable[k] = rf.ReadInt();
                }
            }
        }

        private void CreateNewGlyphTables() {
            newLocaTable = new int[locaTable.Length];
            int[] activeGlyphs = new int[glyphsInList.Count];
            for (int k = 0; k < activeGlyphs.Length; ++k) {
                activeGlyphs[k] = (int)glyphsInList[k];
            }
            JavaUtil.Sort(activeGlyphs);
            int glyfSize = 0;
            foreach (int glyph in activeGlyphs) {
                glyfSize += locaTable[glyph + 1] - locaTable[glyph];
            }
            glyfTableRealSize = glyfSize;
            glyfSize = glyfSize + 3 & ~3;
            newGlyfTable = new byte[glyfSize];
            int glyfPtr = 0;
            int listGlyf = 0;
            for (int k = 0; k < newLocaTable.Length; ++k) {
                newLocaTable[k] = glyfPtr;
                if (listGlyf < activeGlyphs.Length && activeGlyphs[listGlyf] == k) {
                    ++listGlyf;
                    newLocaTable[k] = glyfPtr;
                    int start = locaTable[k];
                    int len = locaTable[k + 1] - start;
                    if (len > 0) {
                        rf.Seek(tableGlyphOffset + start);
                        rf.ReadFully(newGlyfTable, glyfPtr, len);
                        glyfPtr += len;
                    }
                }
            }
        }

        private void LocaToBytes() {
            if (locaShortTable) {
                locaTableRealSize = newLocaTable.Length * 2;
            }
            else {
                locaTableRealSize = newLocaTable.Length * 4;
            }
            newLocaTableOut = new byte[locaTableRealSize + 3 & ~3];
            outFont = newLocaTableOut;
            fontPtr = 0;
            foreach (int location in newLocaTable) {
                if (locaShortTable) {
                    WriteFontShort(location / 2);
                }
                else {
                    WriteFontInt(location);
                }
            }
        }

        private void FlatGlyphs() {
            int[] tableLocation = tableDirectory.Get("glyf");
            if (tableLocation == null) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                    ("glyf", fileName);
            }
            int glyph0 = 0;
            if (!glyphsUsed.Contains(glyph0)) {
                glyphsUsed.Add(glyph0);
                glyphsInList.Add(glyph0);
            }
            tableGlyphOffset = tableLocation[TABLE_OFFSET];
            // Do not replace with foreach. ConcurrentModificationException will arise.
            // noinspection ForLoopReplaceableByForEach
            for (int i = 0; i < glyphsInList.Count; i++) {
                CheckGlyphComposite((int)glyphsInList[i]);
            }
        }

        private void CheckGlyphComposite(int glyph) {
            int start = locaTable[glyph];
            // no contour
            if (start == locaTable[glyph + 1]) {
                return;
            }
            rf.Seek(tableGlyphOffset + start);
            int numContours = rf.ReadShort();
            if (numContours >= 0) {
                return;
            }
            rf.SkipBytes(8);
            for (; ; ) {
                int flags = rf.ReadUnsignedShort();
                int cGlyph = rf.ReadUnsignedShort();
                if (!glyphsUsed.Contains(cGlyph)) {
                    glyphsUsed.Add(cGlyph);
                    glyphsInList.Add(cGlyph);
                }
                if ((flags & MORE_COMPONENTS) == 0) {
                    return;
                }
                int skip;
                if ((flags & ARG_1_AND_2_ARE_WORDS) != 0) {
                    skip = 4;
                }
                else {
                    skip = 2;
                }
                if ((flags & WE_HAVE_A_SCALE) != 0) {
                    skip += 2;
                }
                else {
                    if ((flags & WE_HAVE_AN_X_AND_Y_SCALE) != 0) {
                        skip += 4;
                    }
                }
                if ((flags & WE_HAVE_A_TWO_BY_TWO) != 0) {
                    skip += 8;
                }
                rf.SkipBytes(skip);
            }
        }

        /// <summary>
        /// Reads a
        /// <c>String</c>
        /// from the font file as bytes using the Cp1252 encoding.
        /// </summary>
        /// <param name="length">the length of bytes to read</param>
        /// <returns>
        /// the
        /// <c>String</c>
        /// read
        /// </returns>
        private String ReadStandardString(int length) {
            byte[] buf = new byte[length];
            rf.ReadFully(buf);
            try {
                return iText.Commons.Utils.JavaUtil.GetStringForBytes(buf, PdfEncodings.WINANSI);
            }
            catch (Exception e) {
                throw new iText.IO.Exceptions.IOException("TrueType font", e);
            }
        }

        private void WriteFontShort(int n) {
            outFont[fontPtr++] = (byte)(n >> 8);
            outFont[fontPtr++] = (byte)n;
        }

        private void WriteFontInt(int n) {
            outFont[fontPtr++] = (byte)(n >> 24);
            outFont[fontPtr++] = (byte)(n >> 16);
            outFont[fontPtr++] = (byte)(n >> 8);
            outFont[fontPtr++] = (byte)n;
        }

        private void WriteFontString(String s) {
            byte[] b = PdfEncodings.ConvertToBytes(s, PdfEncodings.WINANSI);
            Array.Copy(b, 0, outFont, fontPtr, b.Length);
            fontPtr += b.Length;
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
    }
}
