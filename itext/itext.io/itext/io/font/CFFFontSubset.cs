/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.IO.Util;

namespace iText.IO.Font {
    /// <summary>This Class subsets a CFF Type Font.</summary>
    /// <remarks>
    /// This Class subsets a CFF Type Font. The subset is preformed for CID fonts and NON CID fonts.
    /// The Charstring is subsetted for both types. For CID fonts only the FDArray which are used are embedded.
    /// The Lsubroutines of the FDArrays used are subsetted as well. The Subroutine subset supports both Type1 and Type2
    /// formatting although only tested on Type2 Format.
    /// For Non CID the Lsubroutines are subsetted. On both types the Gsubroutines is subsetted.
    /// A font which was not of CID type is transformed into CID as a part of the subset process.
    /// The CID synthetic creation was written by Sivan Toledo (sivan@math.tau.ac.il)
    /// </remarks>
    public class CFFFontSubset : CFFFont {
//\cond DO_NOT_DOCUMENT
        /// <summary>The Strings in this array represent Type1/Type2 operator names</summary>
        internal static readonly String[] SubrsFunctions = new String[] { "RESERVED_0", "hstem", "RESERVED_2", "vstem"
            , "vmoveto", "rlineto", "hlineto", "vlineto", "rrcurveto", "RESERVED_9", "callsubr", "return", "escape"
            , "RESERVED_13", "endchar", "RESERVED_15", "RESERVED_16", "RESERVED_17", "hstemhm", "hintmask", "cntrmask"
            , "rmoveto", "hmoveto", "vstemhm", "rcurveline", "rlinecurve", "vvcurveto", "hhcurveto", "shortint", "callgsubr"
            , "vhcurveto", "hvcurveto" };
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The Strings in this array represent Type1/Type2 escape operator names</summary>
        internal static readonly String[] SubrsEscapeFuncs = new String[] { "RESERVED_0", "RESERVED_1", "RESERVED_2"
            , "and", "or", "not", "RESERVED_6", "RESERVED_7", "RESERVED_8", "abs", "add", "sub", "div", "RESERVED_13"
            , "neg", "eq", "RESERVED_16", "RESERVED_17", "drop", "RESERVED_19", "put", "get", "ifelse", "random", 
            "mul", "RESERVED_25", "sqrt", "dup", "exch", "index", "roll", "RESERVED_31", "RESERVED_32", "RESERVED_33"
            , "hflex", "flex", "hflex1", "flex1", "RESERVED_REST" };
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Operator codes for unused  CharStrings and unused local and global Subrs</summary>
        internal const byte ENDCHAR_OP = 14;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const byte RETURN_OP = 11;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// A Map containing the glyphs used in the text after being converted
        /// to glyph number by the CMap
        /// </summary>
        internal ICollection<int> GlyphsUsed;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The GlyphsUsed keys as an list</summary>
        internal IList<int> glyphsInList;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>A Set for keeping the FDArrays being used by the font</summary>
        internal ICollection<int> FDArrayUsed = new HashSet<int>();
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>A Maps array for keeping the subroutines used in each FontDict</summary>
        internal GenericArray<ICollection<int>> hSubrsUsed;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The SubroutinesUsed Maps as lists</summary>
        internal GenericArray<IList<int>> lSubrsUsed;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>A Map for keeping the Global subroutines used in the font</summary>
        internal ICollection<int> hGSubrsUsed = new HashSet<int>();
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The Global SubroutinesUsed Maps as lists</summary>
        internal IList<int> lGSubrsUsed = new List<int>();
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>A Map for keeping the subroutines used in a non-cid font</summary>
        internal ICollection<int> hSubrsUsedNonCID = new HashSet<int>();
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The SubroutinesUsed Map as list</summary>
        internal IList<int> lSubrsUsedNonCID = new List<int>();
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>An array of the new Indexes for the local Subr.</summary>
        /// <remarks>An array of the new Indexes for the local Subr. One index for each FontDict</remarks>
        internal byte[][] NewLSubrsIndex;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The new subroutines index for a non-cid font</summary>
        internal byte[] NewSubrsIndexNonCID;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The new global subroutines index of the font</summary>
        internal byte[] NewGSubrsIndex;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The new CharString of the font</summary>
        internal byte[] NewCharStringsIndex;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The bias for the global subroutines</summary>
        internal int GBias = 0;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The linked list for generating the new font stream</summary>
        internal LinkedList<CFFFont.Item> OutputList;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Number of arguments to the stem operators in a subroutine calculated recursively</summary>
        internal int NumOfHints = 0;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>C'tor for CFFFontSubset</summary>
        /// <param name="cff">- The font file</param>
        internal CFFFontSubset(byte[] cff)
            : this(cff, JavaCollectionsUtil.EmptySet<int>(), true) {
        }
//\endcond

        public CFFFontSubset(byte[] cff, ICollection<int> GlyphsUsed)
            : this(cff, GlyphsUsed, false) {
        }

//\cond DO_NOT_DOCUMENT
        internal CFFFontSubset(byte[] cff, ICollection<int> GlyphsUsed, bool isCidParsingRequired)
            : base(
                        // Use CFFFont c'tor in order to parse the font file.
                        cff) {
            this.GlyphsUsed = GlyphsUsed;
            //Put the glyphs into a list
            glyphsInList = new List<int>(GlyphsUsed);
            for (int i = 0; i < fonts.Length; ++i) {
                // Read the number of glyphs in the font
                Seek(fonts[i].GetCharstringsOffset());
                fonts[i].SetNglyphs(GetCard16());
                // Jump to the count field of the String Index
                Seek(stringIndexOffset);
                fonts[i].SetNstrings(GetCard16() + standardStrings.Length);
                // For each font save the offset array of the charstring
                fonts[i].SetCharstringsOffsets(GetIndex(fonts[i].GetCharstringsOffset()));
                if (isCidParsingRequired) {
                    InitGlyphIdToCharacterIdArray(i, fonts[i].GetNglyphs(), fonts[i].GetCharsetOffset());
                }
                // Process the FDSelect if exist
                if (fonts[i].GetFdselectOffset() >= 0) {
                    // Process the FDSelect
                    ReadFDSelect(i);
                    // Build the FDArrayUsed Map
                    BuildFDArrayUsed(i);
                }
                if (fonts[i].IsCID()) {
                    // Build the FD Array used  Map
                    ReadFDArray(i);
                }
                // compute the charset length
                fonts[i].SetCharsetLength(CountCharset(fonts[i].GetCharsetOffset(), fonts[i].GetNglyphs()));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Calculates the length of the charset according to its format</summary>
        /// <param name="Offset">The Charset Offset</param>
        /// <param name="NumofGlyphs">Number of glyphs in the font</param>
        /// <returns>the length of the Charset</returns>
        internal virtual int CountCharset(int Offset, int NumofGlyphs) {
            int format;
            int Length = 0;
            Seek(Offset);
            // Read the format
            format = GetCard8();
            // Calc according to format
            switch (format) {
                case 0: {
                    Length = 1 + 2 * NumofGlyphs;
                    break;
                }

                case 1: {
                    Length = 1 + 3 * CountRange(NumofGlyphs, 1);
                    break;
                }

                case 2: {
                    Length = 1 + 4 * CountRange(NumofGlyphs, 2);
                    break;
                }

                default: {
                    break;
                }
            }
            return Length;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Function calculates the number of ranges in the Charset</summary>
        /// <param name="NumofGlyphs">The number of glyphs in the font</param>
        /// <param name="Type">The format of the Charset</param>
        /// <returns>The number of ranges in the Charset data structure</returns>
        internal virtual int CountRange(int NumofGlyphs, int Type) {
            int num = 0;
            char Sid;
            int i = 1;
            int nLeft;
            while (i < NumofGlyphs) {
                num++;
                Sid = GetCard16();
                if (Type == 1) {
                    nLeft = GetCard8();
                }
                else {
                    nLeft = GetCard16();
                }
                i += nLeft + 1;
            }
            return num;
        }
//\endcond

        /// <summary>Read the FDSelect of the font and compute the array and its length</summary>
        /// <param name="Font">The index of the font being processed</param>
        protected internal virtual void ReadFDSelect(int Font) {
            // Restore the number of glyphs
            int numOfGlyphs = fonts[Font].GetNglyphs();
            int[] FDSelect = new int[numOfGlyphs];
            // Go to the beginning of the FDSelect
            Seek(fonts[Font].GetFdselectOffset());
            // Read the FDSelect's format
            fonts[Font].SetFDSelectFormat(GetCard8());
            switch (fonts[Font].GetFDSelectFormat()) {
                // Format==0 means each glyph has an entry that indicated
                // its FD.
                case 0: {
                    for (int i = 0; i < numOfGlyphs; i++) {
                        FDSelect[i] = GetCard8();
                    }
                    // The FDSelect's Length is one for each glyph + the format
                    // for later use
                    fonts[Font].SetFDSelectLength(fonts[Font].GetNglyphs() + 1);
                    break;
                }

                case 3: {
                    // Format==3 means the ranges version
                    // The number of ranges
                    int nRanges = GetCard16();
                    int l = 0;
                    // Read the first in the first range
                    int first = GetCard16();
                    for (int i = 0; i < nRanges; i++) {
                        // Read the FD index
                        int fd = GetCard8();
                        // Read the first of the next range
                        int last = GetCard16();
                        // Calc the steps and write to the array
                        int steps = last - first;
                        for (int k = 0; k < steps; k++) {
                            FDSelect[l] = fd;
                            l++;
                        }
                        // The last from this iteration is the first of the next
                        first = last;
                    }
                    // Store the length for later use
                    fonts[Font].SetFDSelectLength(1 + 2 + nRanges * 3 + 2);
                    break;
                }

                default: {
                    break;
                }
            }
            // Save the FDSelect of the font
            fonts[Font].SetFDSelect(FDSelect);
        }

        /// <summary>Function reads the FDSelect and builds the FDArrayUsed Map According to the glyphs used</summary>
        /// <param name="Font">the Number of font being processed</param>
        protected internal virtual void BuildFDArrayUsed(int Font) {
            int[] fdSelect = fonts[Font].GetFDSelect();
            // For each glyph used
            foreach (int? glyphsInList1 in glyphsInList) {
                // Pop the glyphs index
                int glyph = (int)glyphsInList1;
                // Pop the glyph's FD
                int FD = fdSelect[glyph];
                // Put the FD index into the FDArrayUsed Map
                FDArrayUsed.Add(FD);
            }
        }

        /// <summary>Read the FDArray count, offsize and Offset array</summary>
        /// <param name="Font">the Number of font being processed</param>
        protected internal virtual void ReadFDArray(int Font) {
            Seek(fonts[Font].GetFdarrayOffset());
            fonts[Font].SetFDArrayCount(GetCard16());
            fonts[Font].SetFDArrayOffsize(GetCard8());
            // Since we will change values inside the FDArray objects
            // We increase its offsize to prevent errors
            if (fonts[Font].GetFDArrayOffsize() < 4) {
                fonts[Font].SetFDArrayOffsize(fonts[Font].GetFDArrayOffsize() + 1);
            }
            fonts[Font].SetFDArrayOffsets(GetIndex(fonts[Font].GetFdarrayOffset()));
        }

        /// <summary>
        /// The Process function extracts one font out of the CFF file and returns a
        /// subset version of the original.
        /// </summary>
        /// <param name="fontName">- The name of the font to be taken out of the CFF</param>
        /// <returns>The new font stream</returns>
        public virtual byte[] Process(String fontName) {
            try {
                // Find the Font that we will be dealing with
                int j;
                for (j = 0; j < fonts.Length; j++) {
                    if (fontName.Equals(fonts[j].GetName())) {
                        break;
                    }
                }
                if (j == fonts.Length) {
                    return null;
                }
                // Calc the bias for the global subrs
                if (gsubrIndexOffset >= 0) {
                    GBias = CalcBias(gsubrIndexOffset, j);
                }
                // Prepare the new CharStrings Index
                BuildNewCharString(j);
                // Prepare the new Global and Local Subrs Indices
                BuildNewLGSubrs(j);
                // Build the new file
                return BuildNewFile(j);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IO_EXCEPTION, e);
            }
            finally {
                try {
                    buf.Close();
                }
                catch (Exception) {
                }
            }
        }

        // empty on purpose
        /// <summary>
        /// The Process function extracts one font out of the CFF file and returns a
        /// subset version of the original with the first name.
        /// </summary>
        /// <returns>The new font stream</returns>
        public virtual byte[] Process() {
            return Process(GetNames()[0]);
        }

        /// <summary>
        /// Function calcs bias according to the CharString type and the count
        /// of the subrs
        /// </summary>
        /// <param name="Offset">The offset to the relevant subrs index</param>
        /// <param name="Font">the font</param>
        /// <returns>The calculated Bias</returns>
        protected internal virtual int CalcBias(int Offset, int Font) {
            Seek(Offset);
            int nSubrs = GetCard16();
            // If type==1 -> bias=0
            if (fonts[Font].GetCharstringType() == 1) {
                return 0;
            }
            else {
                // else calc according to the count
                if (nSubrs < 1240) {
                    return 107;
                }
                else {
                    if (nSubrs < 33900) {
                        return 1131;
                    }
                    else {
                        return 32768;
                    }
                }
            }
        }

        /// <summary>Function uses BuildNewIndex to create the new index of the subset charstrings.</summary>
        /// <param name="FontIndex">the font</param>
        protected internal virtual void BuildNewCharString(int FontIndex) {
            NewCharStringsIndex = BuildNewIndex(fonts[FontIndex].GetCharstringsOffsets(), GlyphsUsed, ENDCHAR_OP);
        }

        /// <summary>Function builds the new local and global subsrs indices.</summary>
        /// <remarks>
        /// Function builds the new local and global subsrs indices. IF CID then All of
        /// the FD Array lsubrs will be subsetted.
        /// </remarks>
        /// <param name="Font">the font</param>
        protected internal virtual void BuildNewLGSubrs(int Font) {
            // If the font is CID then the lsubrs are divided into FontDicts.
            // for each FD array the lsubrs will be subsetted.
            if (fonts[Font].IsCID()) {
                // Init the Map-array and the list-array to hold the subrs used
                // in each private dict.
                hSubrsUsed = new GenericArray<ICollection<int>>(fonts[Font].GetFdprivateOffsets().Length);
                lSubrsUsed = new GenericArray<IList<int>>(fonts[Font].GetFdprivateOffsets().Length);
                // A [][] which will store the byte array for each new FD Array lsubs index
                NewLSubrsIndex = new byte[fonts[Font].GetFdprivateOffsets().Length][];
                // An array to hold the offset for each Lsubr index
                fonts[Font].SetPrivateSubrsOffset(new int[fonts[Font].GetFdprivateOffsets().Length]);
                // A [][] which will store the offset array for each lsubr index
                fonts[Font].SetPrivateSubrsOffsetsArray(new int[fonts[Font].GetFdprivateOffsets().Length][]);
                // Put the FDarrayUsed into a list
                IList<int> FDInList = new List<int>(FDArrayUsed);
                // For each FD array which is used subset the lsubr
                for (int j = 0; j < FDInList.Count; j++) {
                    // The FDArray index,  Map, List to work on
                    int FD = (int)FDInList[j];
                    hSubrsUsed.Set(FD, new HashSet<int>());
                    lSubrsUsed.Set(FD, new List<int>());
                    //Reads the private dicts looking for the subr operator and
                    // store both the offset for the index and its offset array
                    BuildFDSubrsOffsets(Font, FD);
                    // Verify that FDPrivate has a LSubrs index
                    if (fonts[Font].GetPrivateSubrsOffset()[FD] >= 0) {
                        //Scans the Charstring data storing the used Local and Global subroutines
                        // by the glyphs. Scans the Subrs recursively.
                        BuildSubrUsed(Font, FD, fonts[Font].GetPrivateSubrsOffset()[FD], fonts[Font].GetPrivateSubrsOffsetsArray()
                            [FD], hSubrsUsed.Get(FD), lSubrsUsed.Get(FD));
                        // Builds the New Local Subrs index
                        NewLSubrsIndex[FD] = BuildNewIndex(fonts[Font].GetPrivateSubrsOffsetsArray()[FD], hSubrsUsed.Get(FD), RETURN_OP
                            );
                    }
                }
            }
            else {
                // If the font is not CID && the Private Subr exists then subset:
                if (fonts[Font].GetPrivateSubrs() >= 0) {
                    // Build the subrs offsets;
                    fonts[Font].SetSubrsOffsets(GetIndex(fonts[Font].GetPrivateSubrs()));
                    //Scans the Charstring data storing the used Local and Global subroutines
                    // by the glyphs. Scans the Subrs recursively.
                    BuildSubrUsed(Font, -1, fonts[Font].GetPrivateSubrs(), fonts[Font].GetSubrsOffsets(), hSubrsUsedNonCID, lSubrsUsedNonCID
                        );
                }
            }
            // For all fonts subset the Global Subroutines
            // Scan the Global Subr Map recursively on the Gsubrs
            BuildGSubrsUsed(Font);
            if (fonts[Font].GetPrivateSubrs() >= 0) {
                // Builds the New Local Subrs index
                NewSubrsIndexNonCID = BuildNewIndex(fonts[Font].GetSubrsOffsets(), hSubrsUsedNonCID, RETURN_OP);
            }
            //Builds the New Global Subrs index
            // NOTE We copy all global subroutines to index here.
            // In some fonts (see NotoSansCJKjp-Bold.otf, Version 1.004;PS 1.004;hotconv 1.0.82;makeotf.lib2.5.63406)
            // global subroutines are not derived from local ones. Previously in such cases iText didn't build global subroutines
            // and, if one had set subset as true, produced pdf-document with incorrect cff table.
            // However the code isn't optimised. One can parse all used glyphs and copy not all global subroutines, but only needed.
            NewGSubrsIndex = BuildNewIndexAndCopyAllGSubrs(gsubrOffsets, RETURN_OP);
        }

        /// <summary>
        /// The function finds for the FD array processed the local subr offset and its
        /// offset array.
        /// </summary>
        /// <param name="Font">the font</param>
        /// <param name="FD">The FDARRAY processed</param>
        protected internal virtual void BuildFDSubrsOffsets(int Font, int FD) {
            // Initiate to -1 to indicate lsubr operator present
            int[] privateSubrsOffset = fonts[Font].GetPrivateSubrsOffset();
            privateSubrsOffset[FD] = -1;
            fonts[Font].SetPrivateSubrsOffset(privateSubrsOffset);
            // Goto beginning of objects
            Seek(fonts[Font].GetFdprivateOffsets()[FD]);
            // While in the same object:
            while (GetPosition() < fonts[Font].GetFdprivateOffsets()[FD] + fonts[Font].GetFdprivateLengths()[FD]) {
                GetDictItem();
                // If the dictItem is the "Subrs" then find and store offset,
                if ("Subrs".Equals(key)) {
                    privateSubrsOffset = fonts[Font].GetPrivateSubrsOffset();
                    privateSubrsOffset[FD] = (int)((int?)args[0]) + fonts[Font].GetFdprivateOffsets()[FD];
                    fonts[Font].SetPrivateSubrsOffset(privateSubrsOffset);
                }
            }
            //Read the lsubr index if the lsubr was found
            if (fonts[Font].GetPrivateSubrsOffset()[FD] >= 0) {
                int[][] privateSubrsOffsetsArray = fonts[Font].GetPrivateSubrsOffsetsArray();
                privateSubrsOffsetsArray[FD] = GetIndex(fonts[Font].GetPrivateSubrsOffset()[FD]);
                fonts[Font].SetPrivateSubrsOffsetsArray(privateSubrsOffsetsArray);
            }
        }

        /// <summary>Function uses ReadAsubr on the glyph used to build the LSubr and Gsubr Map.</summary>
        /// <remarks>
        /// Function uses ReadAsubr on the glyph used to build the LSubr and Gsubr Map.
        /// The Map (of the lsubr only) is then scanned recursively for Lsubr and Gsubrs
        /// calls.
        /// </remarks>
        /// <param name="Font">the font</param>
        /// <param name="FD">FD array processed. 0 indicates function was called by non CID font</param>
        /// <param name="SubrOffset">the offset to the subr index to calc the bias</param>
        /// <param name="SubrsOffsets">the offset array of the subr index</param>
        /// <param name="hSubr">Map of the subrs used</param>
        /// <param name="lSubr">list of the subrs used</param>
        protected internal virtual void BuildSubrUsed(int Font, int FD, int SubrOffset, int[] SubrsOffsets, ICollection
            <int> hSubr, IList<int> lSubr) {
            // Calc the Bias for the subr index
            int LBias = CalcBias(SubrOffset, Font);
            // For each glyph used find its GID, start & end pos
            foreach (int? usedGlyph in glyphsInList) {
                int glyph = (int)usedGlyph;
                int start = fonts[Font].GetCharstringsOffsets()[glyph];
                int end = fonts[Font].GetCharstringsOffsets()[glyph + 1];
                // IF CID:
                if (FD >= 0) {
                    EmptyStack();
                    NumOfHints = 0;
                    // Using FDSELECT find the FD Array the glyph belongs to.
                    int glyphFD = fonts[Font].GetFDSelect()[glyph];
                    // If the Glyph is part of the FD being processed
                    if (glyphFD == FD) {
                        // Find the Subrs called by the glyph and insert to hash:
                        ReadASubr(start, end, GBias, LBias, hSubr, lSubr, SubrsOffsets);
                    }
                }
                else {
                    // If the font is not CID
                    //Find the Subrs called by the glyph and insert to hash:
                    ReadASubr(start, end, GBias, LBias, hSubr, lSubr, SubrsOffsets);
                }
            }
            // For all Lsubrs used, check recursively for Lsubr & Gsubr used
            for (int i = 0; i < lSubr.Count; i++) {
                // Pop the subr value from the hash
                int Subr = (int)lSubr[i];
                // Ensure the Lsubr call is valid
                if (Subr < SubrsOffsets.Length - 1 && Subr >= 0) {
                    // Read and process the subr
                    int Start = SubrsOffsets[Subr];
                    int End = SubrsOffsets[Subr + 1];
                    ReadASubr(Start, End, GBias, LBias, hSubr, lSubr, SubrsOffsets);
                }
            }
        }

        /// <summary>
        /// Function scans the Glsubr used list to find recursive calls
        /// to Gsubrs and adds to Map and list
        /// </summary>
        /// <param name="Font">the font</param>
        protected internal virtual void BuildGSubrsUsed(int Font) {
            int LBias = 0;
            int SizeOfNonCIDSubrsUsed = 0;
            if (fonts[Font].GetPrivateSubrs() >= 0) {
                LBias = CalcBias(fonts[Font].GetPrivateSubrs(), Font);
                SizeOfNonCIDSubrsUsed = lSubrsUsedNonCID.Count;
            }
            // For each global subr used
            for (int i = 0; i < lGSubrsUsed.Count; i++) {
                //Pop the value + check valid
                int Subr = (int)lGSubrsUsed[i];
                if (Subr < gsubrOffsets.Length - 1 && Subr >= 0) {
                    // Read the subr and process
                    int Start = gsubrOffsets[Subr];
                    int End = gsubrOffsets[Subr + 1];
                    if (fonts[Font].IsCID()) {
                        ReadASubr(Start, End, GBias, 0, hGSubrsUsed, lGSubrsUsed, null);
                    }
                    else {
                        ReadASubr(Start, End, GBias, LBias, hSubrsUsedNonCID, lSubrsUsedNonCID, fonts[Font].GetSubrsOffsets());
                        if (SizeOfNonCIDSubrsUsed < lSubrsUsedNonCID.Count) {
                            for (int j = SizeOfNonCIDSubrsUsed; j < lSubrsUsedNonCID.Count; j++) {
                                //Pop the value + check valid
                                int lSubr = (int)lSubrsUsedNonCID[j];
                                if (lSubr < fonts[Font].GetSubrsOffsets().Length - 1 && lSubr >= 0) {
                                    // Read the subr and process
                                    int lStart = fonts[Font].GetSubrsOffsets()[lSubr];
                                    int lEnd = fonts[Font].GetSubrsOffsets()[lSubr + 1];
                                    ReadASubr(lStart, lEnd, GBias, LBias, hSubrsUsedNonCID, lSubrsUsedNonCID, fonts[Font].GetSubrsOffsets());
                                }
                            }
                            SizeOfNonCIDSubrsUsed = lSubrsUsedNonCID.Count;
                        }
                    }
                }
            }
        }

        /// <summary>The function reads a subrs (glyph info) between begin and end.</summary>
        /// <remarks>
        /// The function reads a subrs (glyph info) between begin and end.
        /// Adds calls to a Lsubr to the hSubr and lSubrs.
        /// Adds calls to a Gsubr to the hGSubr and lGSubrs.
        /// </remarks>
        /// <param name="begin">the start point of the subr</param>
        /// <param name="end">the end point of the subr</param>
        /// <param name="GBias">the bias of the Global Subrs</param>
        /// <param name="LBias">the bias of the Local Subrs</param>
        /// <param name="hSubr">the subroutines used as set</param>
        /// <param name="lSubr">the subroutines used as list</param>
        /// <param name="LSubrsOffsets">the offsets array of the subroutines</param>
        protected internal virtual void ReadASubr(int begin, int end, int GBias, int LBias, ICollection<int> hSubr
            , IList<int> lSubr, int[] LSubrsOffsets) {
            // Clear the stack for the subrs
            EmptyStack();
            NumOfHints = 0;
            // Goto beginning of the subr
            Seek(begin);
            while (GetPosition() < end) {
                // Read the next command
                ReadCommand();
                int pos = GetPosition();
                Object TopElement = null;
                if (arg_count > 0) {
                    TopElement = args[arg_count - 1];
                }
                int NumOfArgs = arg_count;
                // Check the modification needed on the Argument Stack according to key;
                HandelStack();
                if (null != key) {
                    // a call to a Lsubr
                    switch (key) {
                        // a call to a Gsubr
                        case "callsubr": {
                            // Verify that arguments are passed
                            if (NumOfArgs > 0) {
                                // Calc the index of the Subrs
                                int Subr = (int)((int?)TopElement) + LBias;
                                // If the subr isn't in the Map -> Put in
                                if (!hSubr.Contains(Subr)) {
                                    hSubr.Add(Subr);
                                    lSubr.Add(Subr);
                                }
                                CalcHints(LSubrsOffsets[Subr], LSubrsOffsets[Subr + 1], LBias, GBias, LSubrsOffsets);
                                Seek(pos);
                            }
                            break;
                        }

                        // A call to "stem"
                        case "callgsubr": {
                            // Verify that arguments are passed
                            if (NumOfArgs > 0) {
                                // Calc the index of the Subrs
                                int Subr = (int)((int?)TopElement) + GBias;
                                // If the subr isn't in the Map -> Put in
                                if (!hGSubrsUsed.Contains(Subr)) {
                                    hGSubrsUsed.Add(Subr);
                                    lGSubrsUsed.Add(Subr);
                                }
                                CalcHints(gsubrOffsets[Subr], gsubrOffsets[Subr + 1], LBias, GBias, LSubrsOffsets);
                                Seek(pos);
                            }
                            break;
                        }

                        case "hstem":
                        case "vstem":
                        case "hstemhm":
                        case "vstemhm": {
                            // Increment the NumOfHints by the number couples of of arguments
                            NumOfHints += NumOfArgs / 2;
                            break;
                        }

                        case "hintmask":
                        case "cntrmask": {
                            // if stack is not empty the reason is vstem implicit definition
                            // See Adobe Technical Note #5177, page 25, hintmask usage example.
                            NumOfHints += NumOfArgs / 2;
                            // Compute the size of the mask
                            int SizeOfMask = NumOfHints / 8;
                            if (NumOfHints % 8 != 0 || SizeOfMask == 0) {
                                SizeOfMask++;
                            }
                            // Continue the pointer in SizeOfMask steps
                            for (int i = 0; i < SizeOfMask; i++) {
                                GetCard8();
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function Checks how the current operator effects the run time stack after being run
        /// An operator may increase or decrease the stack size
        /// </summary>
        protected internal virtual void HandelStack() {
            // Find out what the operator does to the stack
            int StackHandel = StackOpp();
            if (StackHandel < 2) {
                // The operators that enlarge the stack by one
                if (StackHandel == 1) {
                    PushStack();
                }
                else {
                    // The operators that pop the stack
                    // Abs value for the for loop
                    StackHandel *= -1;
                    for (int i = 0; i < StackHandel; i++) {
                        PopStack();
                    }
                }
            }
            else {
                // All other flush the stack
                EmptyStack();
            }
        }

        /// <summary>Function checks the key and return the change to the stack after the operator</summary>
        /// <returns>The change in the stack. 2-&gt; flush the stack</returns>
        protected internal virtual int StackOpp() {
            switch (key) {
                case "ifelse": {
                    return -3;
                }

                case "roll":
                case "put": {
                    return -2;
                }

                case "callsubr":
                case "callgsubr":
                case "add":
                case "sub":
                case "div":
                case "mul":
                case "drop":
                case "and":
                case "or":
                case "eq": {
                    return -1;
                }

                case "abs":
                case "neg":
                case "sqrt":
                case "exch":
                case "index":
                case "get":
                case "not":
                case "return": {
                    return 0;
                }

                case "random":
                case "dup": {
                    return 1;
                }
            }
            return 2;
        }

        /// <summary>Empty the Type2 Stack</summary>
        protected internal virtual void EmptyStack() {
            // Null the arguments
            for (int i = 0; i < arg_count; i++) {
                args[i] = null;
            }
            arg_count = 0;
        }

        /// <summary>Pop one element from the stack</summary>
        protected internal virtual void PopStack() {
            if (arg_count > 0) {
                args[arg_count - 1] = null;
                arg_count--;
            }
        }

        /// <summary>Add an item to the stack</summary>
        protected internal virtual void PushStack() {
            arg_count++;
        }

        /// <summary>The function reads the next command after the file pointer is set</summary>
        protected internal virtual void ReadCommand() {
            key = null;
            bool gotKey = false;
            // Until a key is found
            while (!gotKey) {
                // Read the first Char
                char b0 = GetCard8();
                // decode according to the type1/type2 format
                if (b0 == 28) {
                    // the two next bytes represent a short int;
                    int first = GetCard8();
                    int second = GetCard8();
                    args[arg_count] = first << 8 | second;
                    arg_count++;
                    continue;
                }
                // The byte read is the byte;
                if (b0 >= 32 && b0 <= 246) {
                    args[arg_count] = b0 - 139;
                    arg_count++;
                    continue;
                }
                // The byte read and the next byte constitute a short int
                if (b0 >= 247 && b0 <= 250) {
                    int w = GetCard8();
                    args[arg_count] = (b0 - 247) * 256 + w + 108;
                    arg_count++;
                    continue;
                }
                // Same as above except negative
                if (b0 >= 251 && b0 <= 254) {
                    int w = GetCard8();
                    args[arg_count] = -(b0 - 251) * 256 - w - 108;
                    arg_count++;
                    continue;
                }
                // The next for bytes represent a double.
                if (b0 == 255) {
                    int first = GetCard8();
                    int second = GetCard8();
                    int third = GetCard8();
                    int fourth = GetCard8();
                    args[arg_count] = first << 24 | second << 16 | third << 8 | fourth;
                    arg_count++;
                    continue;
                }
                // An operator was found.. Set Key.
                if (b0 <= 31 && b0 != 28) {
                    gotKey = true;
                    // 12 is an escape command therefore the next byte is a part
                    // of this command
                    if (b0 == 12) {
                        int b1 = GetCard8();
                        if (b1 > SubrsEscapeFuncs.Length - 1) {
                            b1 = SubrsEscapeFuncs.Length - 1;
                        }
                        key = SubrsEscapeFuncs[b1];
                    }
                    else {
                        key = SubrsFunctions[b0];
                    }
                    continue;
                }
            }
        }

        /// <summary>The function reads the subroutine and returns the number of the hint in it.</summary>
        /// <remarks>
        /// The function reads the subroutine and returns the number of the hint in it.
        /// If a call to another subroutine is found the function calls recursively.
        /// </remarks>
        /// <param name="begin">the start point of the subr</param>
        /// <param name="end">the end point of the subr</param>
        /// <param name="LBias">the bias of the Local Subrs</param>
        /// <param name="GBias">the bias of the Global Subrs</param>
        /// <param name="LSubrsOffsets">The Offsets array of the subroutines</param>
        /// <returns>The number of hints in the subroutine read.</returns>
        protected internal virtual int CalcHints(int begin, int end, int LBias, int GBias, int[] LSubrsOffsets) {
            // Goto beginning of the subr
            Seek(begin);
            while (GetPosition() < end) {
                // Read the next command
                ReadCommand();
                int pos = GetPosition();
                Object TopElement = null;
                if (arg_count > 0) {
                    TopElement = args[arg_count - 1];
                }
                int NumOfArgs = arg_count;
                //Check the modification needed on the Argument Stack according to key;
                HandelStack();
                // a call to a Lsubr
                switch (key) {
                    // a call to a Gsubr
                    case "callsubr": {
                        if (NumOfArgs > 0) {
                            System.Diagnostics.Debug.Assert(TopElement is int?);
                            int Subr = (int)((int?)TopElement) + LBias;
                            CalcHints(LSubrsOffsets[Subr], LSubrsOffsets[Subr + 1], LBias, GBias, LSubrsOffsets);
                            Seek(pos);
                        }
                        break;
                    }

                    // A call to "stem"
                    case "callgsubr": {
                        if (NumOfArgs > 0) {
                            System.Diagnostics.Debug.Assert(TopElement is int?);
                            int Subr = (int)((int?)TopElement) + GBias;
                            CalcHints(gsubrOffsets[Subr], gsubrOffsets[Subr + 1], LBias, GBias, LSubrsOffsets);
                            Seek(pos);
                        }
                        break;
                    }

                    case "hstem":
                    case "vstem":
                    case "hstemhm":
                    case "vstemhm": {
                        // Increment the NumOfHints by the number couples of of arguments
                        NumOfHints += NumOfArgs / 2;
                        break;
                    }

                    case "hintmask":
                    case "cntrmask": {
                        // Compute the size of the mask
                        int SizeOfMask = NumOfHints / 8;
                        if (NumOfHints % 8 != 0 || SizeOfMask == 0) {
                            SizeOfMask++;
                        }
                        // Continue the pointer in SizeOfMask steps
                        for (int i = 0; i < SizeOfMask; i++) {
                            GetCard8();
                        }
                        break;
                    }
                }
            }
            return NumOfHints;
        }

        /// <summary>Function builds the new offset array, object array and assembles the index.</summary>
        /// <remarks>
        /// Function builds the new offset array, object array and assembles the index.
        /// used for creating the glyph and subrs subsetted index
        /// </remarks>
        /// <param name="Offsets">the offset array of the original index</param>
        /// <param name="Used">the Map of the used objects</param>
        /// <param name="OperatorForUnusedEntries">the operator inserted into the data stream for unused entries</param>
        /// <returns>the new index subset version</returns>
        protected internal virtual byte[] BuildNewIndex(int[] Offsets, ICollection<int> Used, byte OperatorForUnusedEntries
            ) {
            int unusedCount = 0;
            int Offset = 0;
            int[] NewOffsets = new int[Offsets.Length];
            // Build the Offsets Array for the Subset
            for (int i = 0; i < Offsets.Length; ++i) {
                NewOffsets[i] = Offset;
                // If the object in the offset is also present in the used
                // Map then increment the offset var by its size
                if (Used.Contains(i)) {
                    Offset += Offsets[i + 1] - Offsets[i];
                }
                else {
                    // Else the same offset is kept in i+1.
                    unusedCount++;
                }
            }
            // Offset var determines the size of the object array
            byte[] NewObjects = new byte[Offset + unusedCount];
            // Build the new Object array
            int unusedOffset = 0;
            for (int i = 0; i < Offsets.Length - 1; ++i) {
                int start = NewOffsets[i];
                int end = NewOffsets[i + 1];
                NewOffsets[i] = start + unusedOffset;
                // If start != End then the Object is used
                // So, we will copy the object data from the font file
                if (start != end) {
                    // All offsets are Global Offsets relative to the beginning of the font file.
                    // Jump the file pointer to the start address to read from.
                    buf.Seek(Offsets[i]);
                    // Read from the buffer and write into the array at start.
                    buf.ReadFully(NewObjects, start + unusedOffset, end - start);
                }
                else {
                    NewObjects[start + unusedOffset] = OperatorForUnusedEntries;
                    unusedOffset++;
                }
            }
            NewOffsets[Offsets.Length - 1] += unusedOffset;
            // Use AssembleIndex to build the index from the offset & object arrays
            return AssembleIndex(NewOffsets, NewObjects);
        }

        /// <summary>Function builds the new offset array, object array and assembles the index.</summary>
        /// <remarks>
        /// Function builds the new offset array, object array and assembles the index.
        /// used for creating the glyph and subrs subsetted index
        /// </remarks>
        /// <param name="Offsets">the offset array of the original index</param>
        /// <param name="OperatorForUnusedEntries">the operator inserted into the data stream for unused entries</param>
        /// <returns>the new index subset version</returns>
        protected internal virtual byte[] BuildNewIndexAndCopyAllGSubrs(int[] Offsets, byte OperatorForUnusedEntries
            ) {
            int unusedCount = 0;
            int Offset = 0;
            int[] NewOffsets = new int[Offsets.Length];
            // Build the Offsets Array for the Subset
            for (int i = 0; i < Offsets.Length - 1; ++i) {
                NewOffsets[i] = Offset;
                Offset += Offsets[i + 1] - Offsets[i];
            }
            // Else the same offset is kept in i+1.
            NewOffsets[Offsets.Length - 1] = Offset;
            unusedCount++;
            // Offset var determines the size of the object array
            byte[] NewObjects = new byte[Offset + unusedCount];
            // Build the new Object array
            int unusedOffset = 0;
            for (int i = 0; i < Offsets.Length - 1; ++i) {
                int start = NewOffsets[i];
                int end = NewOffsets[i + 1];
                NewOffsets[i] = start + unusedOffset;
                // If start != End then the Object is used
                // So, we will copy the object data from the font file
                if (start != end) {
                    // All offsets are Global Offsets relative to the beginning of the font file.
                    // Jump the file pointer to the start address to read from.
                    buf.Seek(Offsets[i]);
                    // Read from the buffer and write into the array at start.
                    buf.ReadFully(NewObjects, start + unusedOffset, end - start);
                }
                else {
                    NewObjects[start + unusedOffset] = OperatorForUnusedEntries;
                    unusedOffset++;
                }
            }
            NewOffsets[Offsets.Length - 1] += unusedOffset;
            // Use AssembleIndex to build the index from the offset & object arrays
            return AssembleIndex(NewOffsets, NewObjects);
        }

        /// <summary>
        /// Function creates the new index, inserting the count,offsetsize,offset array
        /// and object array.
        /// </summary>
        /// <param name="NewOffsets">the subsetted offset array</param>
        /// <param name="NewObjects">the subsetted object array</param>
        /// <returns>the new index created</returns>
        protected internal virtual byte[] AssembleIndex(int[] NewOffsets, byte[] NewObjects) {
            // Calc the index' count field
            char Count = (char)(NewOffsets.Length - 1);
            // Calc the size of the object array
            int Size = NewOffsets[NewOffsets.Length - 1];
            // Calc the Offsize
            byte Offsize;
            // Previously the condition wasn't strict. However while writing offsets iText adds 1 to them.
            // That can cause overflow (f.e., offset 0xffff will result in 0x0000).
            if (Size < 0xff) {
                Offsize = 1;
            }
            else {
                if (Size < 0xffff) {
                    Offsize = 2;
                }
                else {
                    if (Size < 0xffffff) {
                        Offsize = 3;
                    }
                    else {
                        Offsize = 4;
                    }
                }
            }
            // The byte array for the new index. The size is calc by
            // Count=2, Offsize=1, OffsetArray = Offsize*(Count+1), The object array
            byte[] NewIndex = new byte[2 + 1 + Offsize * (Count + 1) + NewObjects.Length];
            // The counter for writing
            int Place = 0;
            // Write the count field
            // There is no sense in >>> for char
            // NewIndex[Place++] = (byte) (Count >>> 8 & 0xff);
            NewIndex[Place++] = (byte)(Count >> 8 & 0xff);
            NewIndex[Place++] = (byte)(Count & 0xff);
            // Write the offsize field
            NewIndex[Place++] = Offsize;
            // Write the offset array according to the offsize
            foreach (int newOffset in NewOffsets) {
                // The value to be written
                int Num = newOffset - NewOffsets[0] + 1;
                // Write in bytes according to the offsize
                for (int i = Offsize; i > 0; i--) {
                    NewIndex[Place++] = (byte)((int)(((uint)Num) >> ((i - 1) << 3)) & 0xff);
                }
            }
            // Write the new object array one by one
            foreach (byte newObject in NewObjects) {
                NewIndex[Place++] = newObject;
            }
            // Return the new index
            return NewIndex;
        }

        /// <summary>The function builds the new output stream according to the subset process</summary>
        /// <param name="Font">the font</param>
        /// <returns>the subsetted font stream</returns>
        protected internal virtual byte[] BuildNewFile(int Font) {
            // Prepare linked list for new font components
            OutputList = new LinkedList<CFFFont.Item>();
            // copy the header of the font
            CopyHeader();
            // create a name index
            BuildIndexHeader(1, 1, 1);
            OutputList.AddLast(new CFFFont.UInt8Item((char)(1 + fonts[Font].GetName().Length)));
            OutputList.AddLast(new CFFFont.StringItem(fonts[Font].GetName()));
            // create the topdict Index
            BuildIndexHeader(1, 2, 1);
            CFFFont.OffsetItem topdictIndex1Ref = new CFFFont.IndexOffsetItem(2);
            OutputList.AddLast(topdictIndex1Ref);
            CFFFont.IndexBaseItem topdictBase = new CFFFont.IndexBaseItem();
            OutputList.AddLast(topdictBase);
            // Initialize the Dict Items for later use
            CFFFont.OffsetItem charsetRef = new CFFFont.DictOffsetItem();
            CFFFont.OffsetItem charstringsRef = new CFFFont.DictOffsetItem();
            CFFFont.OffsetItem fdarrayRef = new CFFFont.DictOffsetItem();
            CFFFont.OffsetItem fdselectRef = new CFFFont.DictOffsetItem();
            CFFFont.OffsetItem privateRef = new CFFFont.DictOffsetItem();
            // If the font is not CID create the following keys
            if (!fonts[Font].IsCID()) {
                // create a ROS key
                OutputList.AddLast(new CFFFont.DictNumberItem(fonts[Font].GetNstrings()));
                OutputList.AddLast(new CFFFont.DictNumberItem(fonts[Font].GetNstrings() + 1));
                OutputList.AddLast(new CFFFont.DictNumberItem(0));
                OutputList.AddLast(new CFFFont.UInt8Item((char)12));
                OutputList.AddLast(new CFFFont.UInt8Item((char)30));
                // create a CIDCount key
                OutputList.AddLast(new CFFFont.DictNumberItem(fonts[Font].GetNglyphs()));
                OutputList.AddLast(new CFFFont.UInt8Item((char)12));
                OutputList.AddLast(new CFFFont.UInt8Item((char)34));
            }
            // Sivan's comments
            // What about UIDBase (12,35)? Don't know what is it.
            // I don't think we need FontName; the font I looked at didn't have it.
            // Go to the TopDict of the font being processed
            Seek(topdictOffsets[Font]);
            // Run until the end of the TopDict
            while (GetPosition() < topdictOffsets[Font + 1]) {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                // The encoding key is disregarded since CID has no encoding
                if ("Encoding".Equals(key) || 
                                // These keys will be added manually by the process.
                                "Private".Equals(key) || "FDSelect".Equals(key) || "FDArray".Equals(key) || "charset".Equals(key) || "CharStrings"
                    .Equals(key)) {
                }
                else {
                    //OtherWise copy key "as is" to the output list
                    OutputList.AddLast(new CFFFont.RangeItem(buf, p1, p2 - p1));
                }
            }
            // Create the FDArray, FDSelect, Charset and CharStrings Keys
            CreateKeys(fdarrayRef, fdselectRef, charsetRef, charstringsRef);
            // Mark the end of the top dict area
            OutputList.AddLast(new CFFFont.IndexMarkerItem(topdictIndex1Ref, topdictBase));
            // Copy the string index
            if (fonts[Font].IsCID()) {
                OutputList.AddLast(GetEntireIndexRange(stringIndexOffset));
            }
            else {
                // If the font is not CID we need to append new strings.
                // We need 3 more strings: Registry, Ordering, and a FontName for one FD.
                // The total length is at most "Adobe"+"Identity"+63 = 76
                CreateNewStringIndex(Font);
            }
            // copy the new subsetted global subroutine index
            OutputList.AddLast(new CFFFont.RangeItem(new RandomAccessFileOrArray(rasFactory.CreateSource(NewGSubrsIndex
                )), 0, NewGSubrsIndex.Length));
            // deal with fdarray, fdselect, and the font descriptors
            // If the font is CID:
            if (fonts[Font].IsCID()) {
                // copy the FDArray, FDSelect, charset
                // Copy FDSelect
                // Mark the beginning
                OutputList.AddLast(new CFFFont.MarkerItem(fdselectRef));
                // If an FDSelect exists copy it
                if (fonts[Font].GetFdselectOffset() >= 0) {
                    OutputList.AddLast(new CFFFont.RangeItem(buf, fonts[Font].GetFdselectOffset(), fonts[Font].GetFDSelectLength
                        ()));
                }
                else {
                    // Else create a new one
                    CreateFDSelect(fdselectRef, fonts[Font].GetNglyphs());
                }
                // Copy the Charset
                // Mark the beginning and copy entirely
                OutputList.AddLast(new CFFFont.MarkerItem(charsetRef));
                OutputList.AddLast(new CFFFont.RangeItem(buf, fonts[Font].GetCharsetOffset(), fonts[Font].GetCharsetLength
                    ()));
                // Copy the FDArray
                // If an FDArray exists
                if (fonts[Font].GetFdarrayOffset() >= 0) {
                    // Mark the beginning
                    OutputList.AddLast(new CFFFont.MarkerItem(fdarrayRef));
                    // Build a new FDArray with its private dicts and their LSubrs
                    Reconstruct(Font);
                }
                else {
                    // Else create a new one
                    CreateFDArray(fdarrayRef, privateRef, Font);
                }
            }
            else {
                // If the font is not CID
                // create FDSelect
                CreateFDSelect(fdselectRef, fonts[Font].GetNglyphs());
                // recreate a new charset
                CreateCharset(charsetRef, fonts[Font].GetNglyphs());
                // create a font dict index (fdarray)
                CreateFDArray(fdarrayRef, privateRef, Font);
            }
            // if a private dict exists insert its subsetted version
            if (fonts[Font].GetPrivateOffset() >= 0) {
                // Mark the beginning of the private dict
                CFFFont.IndexBaseItem PrivateBase = new CFFFont.IndexBaseItem();
                OutputList.AddLast(PrivateBase);
                OutputList.AddLast(new CFFFont.MarkerItem(privateRef));
                CFFFont.OffsetItem Subr = new CFFFont.DictOffsetItem();
                // Build and copy the new private dict
                CreateNonCIDPrivate(Font, Subr);
                // Copy the new LSubrs index
                CreateNonCIDSubrs(Font, PrivateBase, Subr);
            }
            // copy the charstring index
            OutputList.AddLast(new CFFFont.MarkerItem(charstringsRef));
            // Add the subsetted charstring
            OutputList.AddLast(new CFFFont.RangeItem(new RandomAccessFileOrArray(rasFactory.CreateSource(NewCharStringsIndex
                )), 0, NewCharStringsIndex.Length));
            // now create the new CFF font
            int[] currentOffset = new int[1];
            currentOffset[0] = 0;
            // Count and save the offset for each item
            foreach (CFFFont.Item item in OutputList) {
                item.Increment(currentOffset);
            }
            // Compute the Xref for each of the offset items
            foreach (CFFFont.Item item in OutputList) {
                item.Xref();
            }
            int size = currentOffset[0];
            byte[] b = new byte[size];
            // Emit all the items into the new byte array
            foreach (CFFFont.Item item in OutputList) {
                item.Emit(b);
            }
            // Return the new stream
            return b;
        }

        /// <summary>Function Copies the header from the original fileto the output list</summary>
        protected internal virtual void CopyHeader() {
            Seek(0);
            int major = GetCard8();
            int minor = GetCard8();
            int hdrSize = GetCard8();
            int offSize = GetCard8();
            OutputList.AddLast(new CFFFont.RangeItem(buf, 0, hdrSize));
        }

        /// <summary>Function Build the header of an index</summary>
        /// <param name="Count">the count field of the index</param>
        /// <param name="Offsize">the offsize field of the index</param>
        /// <param name="First">the first offset of the index</param>
        protected internal virtual void BuildIndexHeader(int Count, int Offsize, int First) {
            // Add the count field
            OutputList.AddLast(new CFFFont.UInt16Item((char)Count));
            // Add the offsize field
            OutputList.AddLast(new CFFFont.UInt8Item((char)Offsize));
            // Add the first offset according to the offsize
            switch (Offsize) {
                case 1: {
                    // first offset
                    OutputList.AddLast(new CFFFont.UInt8Item((char)First));
                    break;
                }

                case 2: {
                    // first offset
                    OutputList.AddLast(new CFFFont.UInt16Item((char)First));
                    break;
                }

                case 3: {
                    // first offset
                    OutputList.AddLast(new CFFFont.UInt24Item((char)First));
                    break;
                }

                case 4: {
                    // first offset
                    OutputList.AddLast(new CFFFont.UInt32Item((char)First));
                    break;
                }

                default: {
                    break;
                }
            }
        }

        /// <summary>Function adds the keys into the TopDict</summary>
        /// <param name="fdarrayRef">OffsetItem for the FDArray</param>
        /// <param name="fdselectRef">OffsetItem for the FDSelect</param>
        /// <param name="charsetRef">OffsetItem for the CharSet</param>
        /// <param name="charstringsRef">OffsetItem for the CharString</param>
        protected internal virtual void CreateKeys(CFFFont.OffsetItem fdarrayRef, CFFFont.OffsetItem fdselectRef, 
            CFFFont.OffsetItem charsetRef, CFFFont.OffsetItem charstringsRef) {
            // create an FDArray key
            OutputList.AddLast(fdarrayRef);
            OutputList.AddLast(new CFFFont.UInt8Item((char)12));
            OutputList.AddLast(new CFFFont.UInt8Item((char)36));
            // create an FDSelect key
            OutputList.AddLast(fdselectRef);
            OutputList.AddLast(new CFFFont.UInt8Item((char)12));
            OutputList.AddLast(new CFFFont.UInt8Item((char)37));
            // create an charset key
            OutputList.AddLast(charsetRef);
            OutputList.AddLast(new CFFFont.UInt8Item((char)15));
            // create a CharStrings key
            OutputList.AddLast(charstringsRef);
            OutputList.AddLast(new CFFFont.UInt8Item((char)17));
        }

        /// <summary>
        /// Function takes the original string item and adds the new strings
        /// to accommodate the CID rules
        /// </summary>
        /// <param name="Font">the font</param>
        protected internal virtual void CreateNewStringIndex(int Font) {
            String fdFontName = fonts[Font].GetName() + "-OneRange";
            if (fdFontName.Length > 127) {
                fdFontName = fdFontName.JSubstring(0, 127);
            }
            String extraStrings = "Adobe" + "Identity" + fdFontName;
            int origStringsLen = stringOffsets[stringOffsets.Length - 1] - stringOffsets[0];
            int stringsBaseOffset = stringOffsets[0] - 1;
            byte stringsIndexOffSize;
            if (origStringsLen + extraStrings.Length <= 0xff) {
                stringsIndexOffSize = 1;
            }
            else {
                if (origStringsLen + extraStrings.Length <= 0xffff) {
                    stringsIndexOffSize = 2;
                }
                else {
                    if (origStringsLen + extraStrings.Length <= 0xffffff) {
                        stringsIndexOffSize = 3;
                    }
                    else {
                        stringsIndexOffSize = 4;
                    }
                }
            }
            // count
            OutputList.AddLast(new CFFFont.UInt16Item((char)(stringOffsets.Length - 1 + 3)));
            // offSize
            OutputList.AddLast(new CFFFont.UInt8Item((char)stringsIndexOffSize));
            foreach (int stringOffset in stringOffsets) {
                OutputList.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, stringOffset - stringsBaseOffset));
            }
            int currentStringsOffset = stringOffsets[stringOffsets.Length - 1] - stringsBaseOffset;
            // l.addLast(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
            currentStringsOffset += "Adobe".Length;
            OutputList.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
            currentStringsOffset += "Identity".Length;
            OutputList.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
            currentStringsOffset += fdFontName.Length;
            OutputList.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
            OutputList.AddLast(new CFFFont.RangeItem(buf, stringOffsets[0], origStringsLen));
            OutputList.AddLast(new CFFFont.StringItem(extraStrings));
        }

        /// <summary>Function creates new FDSelect for non-CID fonts.</summary>
        /// <remarks>
        /// Function creates new FDSelect for non-CID fonts.
        /// The FDSelect built uses a single range for all glyphs
        /// </remarks>
        /// <param name="fdselectRef">OffsetItem for the FDSelect</param>
        /// <param name="nglyphs">the number of glyphs in the font</param>
        protected internal virtual void CreateFDSelect(CFFFont.OffsetItem fdselectRef, int nglyphs) {
            OutputList.AddLast(new CFFFont.MarkerItem(fdselectRef));
            // format identifier
            OutputList.AddLast(new CFFFont.UInt8Item((char)3));
            // nRanges
            OutputList.AddLast(new CFFFont.UInt16Item((char)1));
            // Range[0].firstGlyph
            OutputList.AddLast(new CFFFont.UInt16Item((char)0));
            // Range[0].fd
            OutputList.AddLast(new CFFFont.UInt8Item((char)0));
            // sentinel
            OutputList.AddLast(new CFFFont.UInt16Item((char)nglyphs));
        }

        /// <summary>Function creates new CharSet for non-CID fonts.</summary>
        /// <remarks>
        /// Function creates new CharSet for non-CID fonts.
        /// The CharSet built uses a single range for all glyphs
        /// </remarks>
        /// <param name="charsetRef">OffsetItem for the CharSet</param>
        /// <param name="nglyphs">the number of glyphs in the font</param>
        protected internal virtual void CreateCharset(CFFFont.OffsetItem charsetRef, int nglyphs) {
            OutputList.AddLast(new CFFFont.MarkerItem(charsetRef));
            // format identifier
            OutputList.AddLast(new CFFFont.UInt8Item((char)2));
            // first glyph in range (ignore .notdef)
            OutputList.AddLast(new CFFFont.UInt16Item((char)1));
            // nLeft
            /*
            Maintenance note: Here's the rationale for subtracting 2:
            - The .notdef glyph is included in the nglyphs count, but
            we excluded it by starting our range at 1 => decrement once.
            - The CFF specification mandates that the nLeft field _exclude_
            the first glyph => decrement once more.
            
            This line used to say "nglyphs - 1" for the better part of two decades,
            so many PDFs out there contain wrong charset extents.
            */
            OutputList.AddLast(new CFFFont.UInt16Item((char)(nglyphs - 2)));
        }

        /// <summary>Function creates new FDArray for non-CID fonts.</summary>
        /// <remarks>
        /// Function creates new FDArray for non-CID fonts.
        /// The FDArray built has only the "Private" operator that points to the font's
        /// original private dict
        /// </remarks>
        /// <param name="fdarrayRef">OffsetItem for the FDArray</param>
        /// <param name="privateRef">OffsetItem for the Private Dict</param>
        /// <param name="Font">the font</param>
        protected internal virtual void CreateFDArray(CFFFont.OffsetItem fdarrayRef, CFFFont.OffsetItem privateRef
            , int Font) {
            OutputList.AddLast(new CFFFont.MarkerItem(fdarrayRef));
            // Build the header (count=offsize=first=1)
            BuildIndexHeader(1, 1, 1);
            // Mark
            CFFFont.OffsetItem privateIndex1Ref = new CFFFont.IndexOffsetItem(1);
            OutputList.AddLast(privateIndex1Ref);
            CFFFont.IndexBaseItem privateBase = new CFFFont.IndexBaseItem();
            // Insert the private operands and operator
            OutputList.AddLast(privateBase);
            // Calc the new size of the private after subsetting
            // Origianl size
            int newSize = fonts[Font].GetPrivateLength();
            // Calc the original size of the Subr offset in the private
            int orgSubrsOffsetSize = CalcSubrOffsetSize(fonts[Font].GetPrivateOffset(), fonts[Font].GetPrivateLength()
                );
            // Increase the ptivate's size
            if (orgSubrsOffsetSize != 0) {
                newSize += 5 - orgSubrsOffsetSize;
            }
            OutputList.AddLast(new CFFFont.DictNumberItem(newSize));
            OutputList.AddLast(privateRef);
            // Private
            OutputList.AddLast(new CFFFont.UInt8Item((char)18));
            OutputList.AddLast(new CFFFont.IndexMarkerItem(privateIndex1Ref, privateBase));
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Function reconstructs the FDArray, PrivateDict and LSubr for CID fonts</summary>
        /// <param name="Font">the font</param>
        internal virtual void Reconstruct(int Font) {
            // Init for later use
            CFFFont.OffsetItem[] fdPrivate = new CFFFont.DictOffsetItem[fonts[Font].GetFDArrayOffsets().Length - 1];
            CFFFont.IndexBaseItem[] fdPrivateBase = new CFFFont.IndexBaseItem[fonts[Font].GetFdprivateOffsets().Length
                ];
            CFFFont.OffsetItem[] fdSubrs = new CFFFont.DictOffsetItem[fonts[Font].GetFdprivateOffsets().Length];
            // Reconstruct each type
            ReconstructFDArray(Font, fdPrivate);
            ReconstructPrivateDict(Font, fdPrivate, fdPrivateBase, fdSubrs);
            ReconstructPrivateSubrs(Font, fdPrivateBase, fdSubrs);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Function subsets the FDArray and builds the new one with new offsets</summary>
        /// <param name="Font">The font</param>
        /// <param name="fdPrivate">OffsetItem Array (one for each FDArray)</param>
        internal virtual void ReconstructFDArray(int Font, CFFFont.OffsetItem[] fdPrivate) {
            // Build the header of the index
            BuildIndexHeader(fonts[Font].GetFDArrayCount(), fonts[Font].GetFDArrayOffsize(), 1);
            // For each offset create an Offset Item
            CFFFont.OffsetItem[] fdOffsets = new CFFFont.IndexOffsetItem[fonts[Font].GetFDArrayOffsets().Length - 1];
            for (int i = 0; i < fonts[Font].GetFDArrayOffsets().Length - 1; i++) {
                fdOffsets[i] = new CFFFont.IndexOffsetItem(fonts[Font].GetFDArrayOffsize());
                OutputList.AddLast(fdOffsets[i]);
            }
            // Declare beginning of the object array
            CFFFont.IndexBaseItem fdArrayBase = new CFFFont.IndexBaseItem();
            OutputList.AddLast(fdArrayBase);
            // For each object check if that FD is used.
            // if is used build a new one by changing the private object
            // Else do nothing
            // At the end of each object mark its ending (Even if wasn't written)
            for (int k = 0; k < fonts[Font].GetFDArrayOffsets().Length - 1; k++) {
                //			if (FDArrayUsed.contains(Integer.valueOf(k)))
                //			{
                // Goto beginning of objects
                Seek(fonts[Font].GetFDArrayOffsets()[k]);
                while (GetPosition() < fonts[Font].GetFDArrayOffsets()[k + 1]) {
                    int p1 = GetPosition();
                    GetDictItem();
                    int p2 = GetPosition();
                    // If the dictItem is the "Private" then compute and copy length,
                    // use marker for offset and write operator number
                    if ("Private".Equals(key)) {
                        // Save the original length of the private dict
                        int newSize = (int)((int?)args[0]);
                        // Save the size of the offset to the subrs in that private
                        int orgSubrsOffsetSize = CalcSubrOffsetSize(fonts[Font].GetFdprivateOffsets()[k], fonts[Font].GetFdprivateLengths
                            ()[k]);
                        // Increase the private's length accordingly
                        if (orgSubrsOffsetSize != 0) {
                            newSize += 5 - orgSubrsOffsetSize;
                        }
                        // Insert the new size, OffsetItem and operator key number
                        OutputList.AddLast(new CFFFont.DictNumberItem(newSize));
                        fdPrivate[k] = new CFFFont.DictOffsetItem();
                        OutputList.AddLast(fdPrivate[k]);
                        // Private
                        OutputList.AddLast(new CFFFont.UInt8Item((char)18));
                        // Go back to place
                        Seek(p2);
                    }
                    else {
                        // Else copy the entire range
                        // other than private
                        OutputList.AddLast(new CFFFont.RangeItem(buf, p1, p2 - p1));
                    }
                }
                //			}
                // Mark the ending of the object (even if wasn't written)
                OutputList.AddLast(new CFFFont.IndexMarkerItem(fdOffsets[k], fdArrayBase));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Function Adds the new private dicts (only for the FDs used) to the list</summary>
        /// <param name="Font">the font</param>
        /// <param name="fdPrivate">OffsetItem array one element for each private</param>
        /// <param name="fdPrivateBase">IndexBaseItem array one element for each private</param>
        /// <param name="fdSubrs">OffsetItem array one element for each private</param>
        internal virtual void ReconstructPrivateDict(int Font, CFFFont.OffsetItem[] fdPrivate, CFFFont.IndexBaseItem
            [] fdPrivateBase, CFFFont.OffsetItem[] fdSubrs) {
            // For each fdarray private dict check if that FD is used.
            // if is used build a new one by changing the subrs offset
            // Else do nothing
            for (int i = 0; i < fonts[Font].GetFdprivateOffsets().Length; i++) {
                //			if (FDArrayUsed.contains(Integer.valueOf(i)))
                //			{
                // Mark beginning
                OutputList.AddLast(new CFFFont.MarkerItem(fdPrivate[i]));
                fdPrivateBase[i] = new CFFFont.IndexBaseItem();
                OutputList.AddLast(fdPrivateBase[i]);
                // Goto beginning of objects
                Seek(fonts[Font].GetFdprivateOffsets()[i]);
                while (GetPosition() < fonts[Font].GetFdprivateOffsets()[i] + fonts[Font].GetFdprivateLengths()[i]) {
                    int p1 = GetPosition();
                    GetDictItem();
                    int p2 = GetPosition();
                    // If the dictItem is the "Subrs" then,
                    // use marker for offset and write operator number
                    if ("Subrs".Equals(key)) {
                        fdSubrs[i] = new CFFFont.DictOffsetItem();
                        OutputList.AddLast(fdSubrs[i]);
                        // Subrs
                        OutputList.AddLast(new CFFFont.UInt8Item((char)19));
                    }
                    else {
                        // Else copy the entire range
                        OutputList.AddLast(new CFFFont.RangeItem(buf, p1, p2 - p1));
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        //			}
        /// <summary>Function Adds the new LSubrs dicts (only for the FDs used) to the list</summary>
        /// <param name="Font">The index of the font</param>
        /// <param name="fdPrivateBase">The IndexBaseItem array for the linked list</param>
        /// <param name="fdSubrs">OffsetItem array for the linked list</param>
        internal virtual void ReconstructPrivateSubrs(int Font, CFFFont.IndexBaseItem[] fdPrivateBase, CFFFont.OffsetItem
            [] fdSubrs) {
            // For each private dict
            for (int i = 0; i < fonts[Font].GetFdprivateLengths().Length; i++) {
                // If that private dict's Subrs are used insert the new LSubrs
                // computed earlier
                if (fdSubrs[i] != null && fonts[Font].GetPrivateSubrsOffset()[i] >= 0) {
                    OutputList.AddLast(new CFFFont.SubrMarkerItem(fdSubrs[i], fdPrivateBase[i]));
                    if (NewLSubrsIndex[i] != null) {
                        OutputList.AddLast(new CFFFont.RangeItem(new RandomAccessFileOrArray(rasFactory.CreateSource(NewLSubrsIndex
                            [i])), 0, NewLSubrsIndex[i].Length));
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Calculates how many byte it took to write the offset for the subrs in a specific
        /// private dict.
        /// </summary>
        /// <param name="Offset">The Offset for the private dict</param>
        /// <param name="Size">The size of the private dict</param>
        /// <returns>The size of the offset of the subrs in the private dict</returns>
        internal virtual int CalcSubrOffsetSize(int Offset, int Size) {
            // Set the size to 0
            int OffsetSize = 0;
            // Go to the beginning of the private dict
            Seek(Offset);
            // Go until the end of the private dict
            while (GetPosition() < Offset + Size) {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                // When reached to the subrs offset
                if ("Subrs".Equals(key)) {
                    // The Offsize (minus the subrs key)
                    OffsetSize = p2 - p1 - 1;
                }
            }
            // All other keys are ignored
            // return the size
            return OffsetSize;
        }
//\endcond

        /// <summary>Function computes the size of an index</summary>
        /// <param name="indexOffset">The offset for the computed index</param>
        /// <returns>The size of the index</returns>
        protected internal virtual int CountEntireIndexRange(int indexOffset) {
            // Go to the beginning of the index
            Seek(indexOffset);
            // Read the count field
            int count = GetCard16();
            // If count==0 -> size=2
            if (count == 0) {
                return 2;
            }
            else {
                // Read the offsize field
                int indexOffSize = GetCard8();
                // Go to the last element of the offset array
                Seek(indexOffset + 2 + 1 + count * indexOffSize);
                // The size of the object array is the value of the last element-1
                int size = GetOffset(indexOffSize) - 1;
                // Return the size of the entire index
                return 2 + 1 + (count + 1) * indexOffSize + size;
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// The function creates a private dict for a font that was not CID
        /// All the keys are copied as is except for the subrs key
        /// </summary>
        /// <param name="Font">the font</param>
        /// <param name="Subr">The OffsetItem for the subrs of the private</param>
        internal virtual void CreateNonCIDPrivate(int Font, CFFFont.OffsetItem Subr) {
            // Go to the beginning of the private dict and read until the end
            Seek(fonts[Font].GetPrivateOffset());
            while (GetPosition() < fonts[Font].GetPrivateOffset() + fonts[Font].GetPrivateLength()) {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                // If the dictItem is the "Subrs" then,
                // use marker for offset and write operator number
                if ("Subrs".Equals(key)) {
                    OutputList.AddLast(Subr);
                    // Subrs
                    OutputList.AddLast(new CFFFont.UInt8Item((char)19));
                }
                else {
                    // Else copy the entire range
                    OutputList.AddLast(new CFFFont.RangeItem(buf, p1, p2 - p1));
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// the function marks the beginning of the subrs index and adds the subsetted subrs
        /// index to the output list.
        /// </summary>
        /// <param name="Font">the font</param>
        /// <param name="PrivateBase">IndexBaseItem for the private that's referencing to the subrs</param>
        /// <param name="Subrs">OffsetItem for the subrs</param>
        internal virtual void CreateNonCIDSubrs(int Font, CFFFont.IndexBaseItem PrivateBase, CFFFont.OffsetItem Subrs
            ) {
            // Mark the beginning of the Subrs index
            OutputList.AddLast(new CFFFont.SubrMarkerItem(Subrs, PrivateBase));
            // Put the subsetted new subrs index
            if (NewSubrsIndexNonCID != null) {
                OutputList.AddLast(new CFFFont.RangeItem(new RandomAccessFileOrArray(rasFactory.CreateSource(NewSubrsIndexNonCID
                    )), 0, NewSubrsIndexNonCID.Length));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Returns the CID to which specified GID is mapped.</summary>
        /// <param name="gid">glyph identifier</param>
        /// <returns>CID value</returns>
        internal virtual int GetCidForGlyphId(int gid) {
            return GetCidForGlyphId(0, gid);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Returns the CID to which specified GID is mapped.</summary>
        /// <param name="fontIndex">index of font for which cid-gid mapping is to be identified</param>
        /// <param name="gid">glyph identifier</param>
        /// <returns>CID value</returns>
        internal virtual int GetCidForGlyphId(int fontIndex, int gid) {
            if (fonts[fontIndex].GetGidToCid() == null) {
                return gid;
            }
            // gidToCid mapping starts with value corresponding to gid == 1, becuase .notdef is omitted
            int index = gid - 1;
            return index >= 0 && index < fonts[fontIndex].GetGidToCid().Length ? fonts[fontIndex].GetGidToCid()[index]
                 : gid;
        }
//\endcond

        /// <summary>Creates glyph-to-character id array.</summary>
        /// <param name="fontIndex">index of font for which charsets data is to be parsed</param>
        /// <param name="numOfGlyphs">number of glyphs in the font</param>
        /// <param name="offset">the offset to charsets data</param>
        private void InitGlyphIdToCharacterIdArray(int fontIndex, int numOfGlyphs, int offset) {
            // Seek charset offset
            Seek(offset);
            // Read the format
            int format = GetCard8();
            // .notdef is omitted, therefore remaining number of elements is one less than overall number
            int numOfElements = numOfGlyphs - 1;
            fonts[fontIndex].SetGidToCid(new int[numOfElements]);
            switch (format) {
                case 0: {
                    for (int i = 0; i < numOfElements; i++) {
                        int cid = GetCard16();
                        int[] gidToCid = fonts[fontIndex].GetGidToCid();
                        gidToCid[i] = cid;
                        fonts[fontIndex].SetGidToCid(gidToCid);
                    }
                    break;
                }

                case 1:
                case 2: {
                    int start = 0;
                    while (start < numOfElements) {
                        int first = GetCard16();
                        int nLeft = format == 1 ? GetCard8() : GetCard16();
                        for (int i = 0; i <= nLeft && start < numOfElements; i++) {
                            int[] gidToCid = fonts[fontIndex].GetGidToCid();
                            gidToCid[start++] = first + i;
                            fonts[fontIndex].SetGidToCid(gidToCid);
                        }
                    }
                    break;
                }

                default: {
                    break;
                }
            }
        }
    }
}
