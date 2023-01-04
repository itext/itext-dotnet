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
using System.Collections.Generic;
using System.Text;
using iText.IO.Source;

namespace iText.IO.Font {
    public class CFFFont {
        internal static readonly String[] operatorNames = new String[] { "version", "Notice", "FullName", "FamilyName"
            , "Weight", "FontBBox", "BlueValues", "OtherBlues", "FamilyBlues", "FamilyOtherBlues", "StdHW", "StdVW"
            , "UNKNOWN_12", "UniqueID", "XUID", "charset", "Encoding", "CharStrings", "Private", "Subrs", "defaultWidthX"
            , "nominalWidthX", "UNKNOWN_22", "UNKNOWN_23", "UNKNOWN_24", "UNKNOWN_25", "UNKNOWN_26", "UNKNOWN_27", 
            "UNKNOWN_28", "UNKNOWN_29", "UNKNOWN_30", "UNKNOWN_31", "Copyright", "isFixedPitch", "ItalicAngle", "UnderlinePosition"
            , "UnderlineThickness", "PaintType", "CharstringType", "FontMatrix", "StrokeWidth", "BlueScale", "BlueShift"
            , "BlueFuzz", "StemSnapH", "StemSnapV", "ForceBold", "UNKNOWN_12_15", "UNKNOWN_12_16", "LanguageGroup"
            , "ExpansionFactor", "initialRandomSeed", "SyntheticBase", "PostScript", "BaseFontName", "BaseFontBlend"
            , "UNKNOWN_12_24", "UNKNOWN_12_25", "UNKNOWN_12_26", "UNKNOWN_12_27", "UNKNOWN_12_28", "UNKNOWN_12_29"
            , "ROS", "CIDFontVersion", "CIDFontRevision", "CIDFontType", "CIDCount", "UIDBase", "FDArray", "FDSelect"
            , "FontName" };

        internal static readonly String[] standardStrings = new String[] { 
                // Automatically generated from Appendix A of the CFF specification; do
                
                // not edit. Size should be 391.
                ".notdef", "space", "exclam", "quotedbl", "numbersign", "dollar", "percent", "ampersand", "quoteright", "parenleft"
            , "parenright", "asterisk", "plus", "comma", "hyphen", "period", "slash", "zero", "one", "two", "three"
            , "four", "five", "six", "seven", "eight", "nine", "colon", "semicolon", "less", "equal", "greater", "question"
            , "at", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", 
            "T", "U", "V", "W", "X", "Y", "Z", "bracketleft", "backslash", "bracketright", "asciicircum", "underscore"
            , "quoteleft", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r"
            , "s", "t", "u", "v", "w", "x", "y", "z", "braceleft", "bar", "braceright", "asciitilde", "exclamdown"
            , "cent", "sterling", "fraction", "yen", "florin", "section", "currency", "quotesingle", "quotedblleft"
            , "guillemotleft", "guilsinglleft", "guilsinglright", "fi", "fl", "endash", "dagger", "daggerdbl", "periodcentered"
            , "paragraph", "bullet", "quotesinglbase", "quotedblbase", "quotedblright", "guillemotright", "ellipsis"
            , "perthousand", "questiondown", "grave", "acute", "circumflex", "tilde", "macron", "breve", "dotaccent"
            , "dieresis", "ring", "cedilla", "hungarumlaut", "ogonek", "caron", "emdash", "AE", "ordfeminine", "Lslash"
            , "Oslash", "OE", "ordmasculine", "ae", "dotlessi", "lslash", "oslash", "oe", "germandbls", "onesuperior"
            , "logicalnot", "mu", "trademark", "Eth", "onehalf", "plusminus", "Thorn", "onequarter", "divide", "brokenbar"
            , "degree", "thorn", "threequarters", "twosuperior", "registered", "minus", "eth", "multiply", "threesuperior"
            , "copyright", "Aacute", "Acircumflex", "Adieresis", "Agrave", "Aring", "Atilde", "Ccedilla", "Eacute"
            , "Ecircumflex", "Edieresis", "Egrave", "Iacute", "Icircumflex", "Idieresis", "Igrave", "Ntilde", "Oacute"
            , "Ocircumflex", "Odieresis", "Ograve", "Otilde", "Scaron", "Uacute", "Ucircumflex", "Udieresis", "Ugrave"
            , "Yacute", "Ydieresis", "Zcaron", "aacute", "acircumflex", "adieresis", "agrave", "aring", "atilde", 
            "ccedilla", "eacute", "ecircumflex", "edieresis", "egrave", "iacute", "icircumflex", "idieresis", "igrave"
            , "ntilde", "oacute", "ocircumflex", "odieresis", "ograve", "otilde", "scaron", "uacute", "ucircumflex"
            , "udieresis", "ugrave", "yacute", "ydieresis", "zcaron", "exclamsmall", "Hungarumlautsmall", "dollaroldstyle"
            , "dollarsuperior", "ampersandsmall", "Acutesmall", "parenleftsuperior", "parenrightsuperior", "twodotenleader"
            , "onedotenleader", "zerooldstyle", "oneoldstyle", "twooldstyle", "threeoldstyle", "fouroldstyle", "fiveoldstyle"
            , "sixoldstyle", "sevenoldstyle", "eightoldstyle", "nineoldstyle", "commasuperior", "threequartersemdash"
            , "periodsuperior", "questionsmall", "asuperior", "bsuperior", "centsuperior", "dsuperior", "esuperior"
            , "isuperior", "lsuperior", "msuperior", "nsuperior", "osuperior", "rsuperior", "ssuperior", "tsuperior"
            , "ff", "ffi", "ffl", "parenleftinferior", "parenrightinferior", "Circumflexsmall", "hyphensuperior", 
            "Gravesmall", "Asmall", "Bsmall", "Csmall", "Dsmall", "Esmall", "Fsmall", "Gsmall", "Hsmall", "Ismall"
            , "Jsmall", "Ksmall", "Lsmall", "Msmall", "Nsmall", "Osmall", "Psmall", "Qsmall", "Rsmall", "Ssmall", 
            "Tsmall", "Usmall", "Vsmall", "Wsmall", "Xsmall", "Ysmall", "Zsmall", "colonmonetary", "onefitted", "rupiah"
            , "Tildesmall", "exclamdownsmall", "centoldstyle", "Lslashsmall", "Scaronsmall", "Zcaronsmall", "Dieresissmall"
            , "Brevesmall", "Caronsmall", "Dotaccentsmall", "Macronsmall", "figuredash", "hypheninferior", "Ogoneksmall"
            , "Ringsmall", "Cedillasmall", "questiondownsmall", "oneeighth", "threeeighths", "fiveeighths", "seveneighths"
            , "onethird", "twothirds", "zerosuperior", "foursuperior", "fivesuperior", "sixsuperior", "sevensuperior"
            , "eightsuperior", "ninesuperior", "zeroinferior", "oneinferior", "twoinferior", "threeinferior", "fourinferior"
            , "fiveinferior", "sixinferior", "seveninferior", "eightinferior", "nineinferior", "centinferior", "dollarinferior"
            , "periodinferior", "commainferior", "Agravesmall", "Aacutesmall", "Acircumflexsmall", "Atildesmall", 
            "Adieresissmall", "Aringsmall", "AEsmall", "Ccedillasmall", "Egravesmall", "Eacutesmall", "Ecircumflexsmall"
            , "Edieresissmall", "Igravesmall", "Iacutesmall", "Icircumflexsmall", "Idieresissmall", "Ethsmall", "Ntildesmall"
            , "Ogravesmall", "Oacutesmall", "Ocircumflexsmall", "Otildesmall", "Odieresissmall", "OEsmall", "Oslashsmall"
            , "Ugravesmall", "Uacutesmall", "Ucircumflexsmall", "Udieresissmall", "Yacutesmall", "Thornsmall", "Ydieresissmall"
            , "001.000", "001.001", "001.002", "001.003", "Black", "Bold", "Book", "Light", "Medium", "Regular", "Roman"
            , "Semibold" };

        //private String[] strings;
        public virtual String GetString(char sid) {
            if (sid < standardStrings.Length) {
                return standardStrings[sid];
            }
            if (sid >= standardStrings.Length + stringOffsets.Length - 1) {
                return null;
            }
            int j = sid - standardStrings.Length;
            //java.lang.System.err.println("going for "+j);
            int p = GetPosition();
            Seek(stringOffsets[j]);
            StringBuilder s = new StringBuilder();
            for (int k = stringOffsets[j]; k < stringOffsets[j + 1]; k++) {
                s.Append(GetCard8());
            }
            Seek(p);
            return s.ToString();
        }

        internal virtual char GetCard8() {
            try {
                byte i = buf.ReadByte();
                return (char)(i & 0xff);
            }
            catch (Exception e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IoException, e);
            }
        }

        internal virtual char GetCard16() {
            try {
                return buf.ReadChar();
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IoException, e);
            }
        }

        internal virtual int GetOffset(int offSize) {
            int offset = 0;
            for (int i = 0; i < offSize; i++) {
                offset *= 256;
                offset += GetCard8();
            }
            return offset;
        }

        internal virtual void Seek(int offset) {
            buf.Seek(offset);
        }

        internal virtual short GetShort() {
            try {
                return buf.ReadShort();
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IoException, e);
            }
        }

        internal virtual int GetInt() {
            try {
                return buf.ReadInt();
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IoException, e);
            }
        }

        internal virtual int GetPosition() {
            return (int)buf.GetPosition();
        }

        internal int nextIndexOffset;

        // read the offsets in the next index
        // data structure, convert to global
        // offsets, and return them.
        // Sets the nextIndexOffset.
        internal virtual int[] GetIndex(int nextIndexOffset) {
            int count;
            int indexOffSize;
            Seek(nextIndexOffset);
            count = GetCard16();
            int[] offsets = new int[count + 1];
            if (count == 0) {
                offsets[0] = -1;
                // TODO death store to local var .. should this be this.nextIndexOffset ?
                nextIndexOffset += 2;
                return offsets;
            }
            indexOffSize = GetCard8();
            for (int j = 0; j <= count; j++) {
                //nextIndexOffset = ofset to relative segment
                offsets[j] = nextIndexOffset + 
                                //2-> count in the index header. 1->offset size in index header
                                2 + 1 + 
                                //offset array size * offset size
                                (count + 1) * indexOffSize - 
                                //???zero <-> one base
                                1 + 
                                // read object offset relative to object array base
                                GetOffset(indexOffSize);
            }
            //nextIndexOffset = offsets[count];
            return offsets;
        }

        protected internal String key;

        protected internal Object[] args = new Object[48];

        protected internal int arg_count = 0;

        protected internal virtual void GetDictItem() {
            for (int i = 0; i < arg_count; i++) {
                args[i] = null;
            }
            arg_count = 0;
            key = null;
            bool gotKey = false;
            while (!gotKey) {
                char b0 = GetCard8();
                if (b0 == 29) {
                    int item = GetInt();
                    args[arg_count] = item;
                    arg_count++;
                    //System.err.println(item+" ");
                    continue;
                }
                if (b0 == 28) {
                    short item = GetShort();
                    args[arg_count] = (int)item;
                    arg_count++;
                    //System.err.println(item+" ");
                    continue;
                }
                if (b0 >= 32 && b0 <= 246) {
                    args[arg_count] = b0 - 139;
                    arg_count++;
                    //System.err.println(item+" ");
                    continue;
                }
                if (b0 >= 247 && b0 <= 250) {
                    char b1 = GetCard8();
                    short item = (short)((b0 - 247) * 256 + b1 + 108);
                    args[arg_count] = (int)item;
                    arg_count++;
                    //System.err.println(item+" ");
                    continue;
                }
                if (b0 >= 251 && b0 <= 254) {
                    char b1 = GetCard8();
                    short item = (short)(-(b0 - 251) * 256 - b1 - 108);
                    args[arg_count] = (int)item;
                    arg_count++;
                    //System.err.println(item+" ");
                    continue;
                }
                if (b0 == 30) {
                    StringBuilder item = new StringBuilder("");
                    bool done = false;
                    char buffer = (char)0;
                    byte avail = 0;
                    int nibble = 0;
                    while (!done) {
                        // get a nibble
                        if (avail == 0) {
                            buffer = GetCard8();
                            avail = 2;
                        }
                        if (avail == 1) {
                            nibble = buffer / 16;
                            avail--;
                        }
                        if (avail == 2) {
                            nibble = buffer % 16;
                            avail--;
                        }
                        switch (nibble) {
                            case 0xa: {
                                item.Append(".");
                                break;
                            }

                            case 0xb: {
                                item.Append("E");
                                break;
                            }

                            case 0xc: {
                                item.Append("E-");
                                break;
                            }

                            case 0xe: {
                                item.Append("-");
                                break;
                            }

                            case 0xf: {
                                done = true;
                                break;
                            }

                            default: {
                                if (nibble >= 0 && nibble <= 9) {
                                    item.Append(nibble);
                                }
                                else {
                                    item.Append("<NIBBLE ERROR: ").Append(nibble).Append('>');
                                    done = true;
                                }
                                break;
                            }
                        }
                    }
                    args[arg_count] = item.ToString();
                    arg_count++;
                    //System.err.println(" real=["+item+"]");
                    continue;
                }
                if (b0 <= 21) {
                    gotKey = true;
                    if (b0 != 12) {
                        key = operatorNames[b0];
                    }
                    else {
                        key = operatorNames[32 + GetCard8()];
                    }
                    //for (int i=0; i<arg_count; i++)
                    //  System.err.print(args[i].toString()+" ");
                    //System.err.println(key+" ;");
                    continue;
                }
            }
        }

        /// <summary>List items for the linked list that builds the new CID font.</summary>
        protected internal abstract class Item {
            protected internal int myOffset = -1;

            /// <summary>Remember the current offset and increment by item's size in bytes.</summary>
            /// <param name="currentOffset">increment offset by item's size</param>
            public virtual void Increment(int[] currentOffset) {
                myOffset = currentOffset[0];
            }

            /// <summary>Emit the byte stream for this item.</summary>
            /// <param name="buffer">byte array</param>
            public virtual void Emit(byte[] buffer) {
            }

            /// <summary>Fix up cross references to this item (applies only to markers).</summary>
            public virtual void Xref() {
            }
        }

        protected internal abstract class OffsetItem : CFFFont.Item {
            public int value;

            /// <summary>Set the value of an offset item that was initially unknown.</summary>
            /// <remarks>
            /// Set the value of an offset item that was initially unknown.
            /// It will be fixed up latex by a call to xref on some marker.
            /// </remarks>
            /// <param name="offset">offset to set</param>
            public virtual void Set(int offset) {
                this.value = offset;
            }
        }

        /// <summary>A range item.</summary>
        protected internal sealed class RangeItem : CFFFont.Item {
            public int offset;

            public int length;

            private RandomAccessFileOrArray buf;

            public RangeItem(RandomAccessFileOrArray buf, int offset, int length) {
                this.offset = offset;
                this.length = length;
                this.buf = buf;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += length;
            }

            public override void Emit(byte[] buffer) {
                //System.err.println("range emit offset "+offset+" size="+length);
                try {
                    buf.Seek(offset);
                    for (int i = myOffset; i < myOffset + length; i++) {
                        buffer[i] = buf.ReadByte();
                    }
                }
                catch (System.IO.IOException e) {
                    throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IoException, e);
                }
            }
            //System.err.println("finished range emit");
        }

        /// <summary>An index-offset item for the list.</summary>
        /// <remarks>
        /// An index-offset item for the list.
        /// The size denotes the required size in the CFF. A positive
        /// value means that we need a specific size in bytes (for offset arrays)
        /// and a negative value means that this is a dict item that uses a
        /// variable-size representation.
        /// </remarks>
        protected internal sealed class IndexOffsetItem : CFFFont.OffsetItem {
            public readonly int size;

            public IndexOffsetItem(int size, int value) {
                this.size = size;
                this.value = value;
            }

            public IndexOffsetItem(int size) {
                this.size = size;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += size;
            }

            public override void Emit(byte[] buffer) {
                if (size >= 1 && size <= 4) {
                    for (int i = 0; i < size; i++) {
                        buffer[myOffset + i] = (byte)((int)(((uint)value) >> ((size - 1 - i) << 3)) & 0xFF);
                    }
                }
            }
        }

        protected internal sealed class IndexBaseItem : CFFFont.Item {
            public IndexBaseItem() {
            }
        }

        protected internal sealed class IndexMarkerItem : CFFFont.Item {
            private CFFFont.OffsetItem offItem;

            private CFFFont.IndexBaseItem indexBase;

            public IndexMarkerItem(CFFFont.OffsetItem offItem, CFFFont.IndexBaseItem indexBase) {
                this.offItem = offItem;
                this.indexBase = indexBase;
            }

            public override void Xref() {
                //System.err.println("index marker item, base="+indexBase.myOffset+" my="+this.myOffset);
                offItem.Set(this.myOffset - indexBase.myOffset + 1);
            }
        }

        protected internal sealed class SubrMarkerItem : CFFFont.Item {
            private CFFFont.OffsetItem offItem;

            private CFFFont.IndexBaseItem indexBase;

            public SubrMarkerItem(CFFFont.OffsetItem offItem, CFFFont.IndexBaseItem indexBase) {
                this.offItem = offItem;
                this.indexBase = indexBase;
            }

            public override void Xref() {
                //System.err.println("index marker item, base="+indexBase.myOffset+" my="+this.myOffset);
                offItem.Set(this.myOffset - indexBase.myOffset);
            }
        }

        /// <summary>an unknown offset in a dictionary for the list.</summary>
        /// <remarks>
        /// an unknown offset in a dictionary for the list.
        /// We will fix up the offset later; for now, assume it's large.
        /// </remarks>
        protected internal sealed class DictOffsetItem : CFFFont.OffsetItem {
            public readonly int size;

            public DictOffsetItem() {
                this.size = 5;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += size;
            }

            // this is incomplete!
            public override void Emit(byte[] buffer) {
                if (size == 5) {
                    buffer[myOffset] = 29;
                    buffer[myOffset + 1] = (byte)((int)(((uint)value) >> 24) & 0xff);
                    buffer[myOffset + 2] = (byte)((int)(((uint)value) >> 16) & 0xff);
                    buffer[myOffset + 3] = (byte)((int)(((uint)value) >> 8) & 0xff);
                    buffer[myOffset + 4] = (byte)((int)(((uint)value) >> 0) & 0xff);
                }
            }
        }

        /// <summary>Card24 item.</summary>
        protected internal sealed class UInt24Item : CFFFont.Item {
            public int value;

            public UInt24Item(int value) {
                this.value = value;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 3;
            }

            // this is incomplete!
            public override void Emit(byte[] buffer) {
                buffer[myOffset + 0] = (byte)((int)(((uint)value) >> 16) & 0xff);
                buffer[myOffset + 1] = (byte)((int)(((uint)value) >> 8) & 0xff);
                buffer[myOffset + 2] = (byte)((int)(((uint)value) >> 0) & 0xff);
            }
        }

        /// <summary>Card32 item.</summary>
        protected internal sealed class UInt32Item : CFFFont.Item {
            public int value;

            public UInt32Item(int value) {
                this.value = value;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 4;
            }

            // this is incomplete!
            public override void Emit(byte[] buffer) {
                buffer[myOffset + 0] = (byte)((int)(((uint)value) >> 24) & 0xff);
                buffer[myOffset + 1] = (byte)((int)(((uint)value) >> 16) & 0xff);
                buffer[myOffset + 2] = (byte)((int)(((uint)value) >> 8) & 0xff);
                buffer[myOffset + 3] = (byte)((int)(((uint)value) >> 0) & 0xff);
            }
        }

        /// <summary>A SID or Card16 item.</summary>
        protected internal sealed class UInt16Item : CFFFont.Item {
            public char value;

            public UInt16Item(char value) {
                this.value = value;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 2;
            }

            // this is incomplete!
            public override void Emit(byte[] buffer) {
                //            Simplify from: there is no sense in >>> for unsigned char.
                //            buffer[myOffset+0] = (byte) (value >>> 8 & 0xff);
                //            buffer[myOffset+1] = (byte) (value >>> 0 & 0xff);
                buffer[myOffset + 0] = (byte)(value >> 8 & 0xff);
                buffer[myOffset + 1] = (byte)(value >> 0 & 0xff);
            }
        }

        /// <summary>A Card8 item.</summary>
        protected internal sealed class UInt8Item : CFFFont.Item {
            public char value;

            public UInt8Item(char value) {
                this.value = value;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 1;
            }

            // this is incomplete!
            public override void Emit(byte[] buffer) {
                //buffer[myOffset+0] = (byte) (value >>> 0 & 0xff);
                buffer[myOffset + 0] = (byte)(value & 0xff);
            }
        }

        protected internal sealed class StringItem : CFFFont.Item {
            public String s;

            public StringItem(String s) {
                this.s = s;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += s.Length;
            }

            public override void Emit(byte[] buffer) {
                for (int i = 0; i < s.Length; i++) {
                    buffer[myOffset + i] = (byte)(s[i] & 0xff);
                }
            }
        }

        /// <summary>A dictionary number on the list.</summary>
        /// <remarks>
        /// A dictionary number on the list.
        /// This implementation is inefficient: it doesn't use the variable-length
        /// representation.
        /// </remarks>
        protected internal sealed class DictNumberItem : CFFFont.Item {
            public readonly int value;

            public int size = 5;

            public DictNumberItem(int value) {
                this.value = value;
            }

            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += size;
            }

            // this is incomplete!
            public override void Emit(byte[] buffer) {
                if (size == 5) {
                    buffer[myOffset] = 29;
                    buffer[myOffset + 1] = (byte)((int)(((uint)value) >> 24) & 0xff);
                    buffer[myOffset + 2] = (byte)((int)(((uint)value) >> 16) & 0xff);
                    buffer[myOffset + 3] = (byte)((int)(((uint)value) >> 8) & 0xff);
                    buffer[myOffset + 4] = (byte)((int)(((uint)value) >> 0) & 0xff);
                }
            }
        }

        /// <summary>An offset-marker item for the list.</summary>
        /// <remarks>
        /// An offset-marker item for the list.
        /// It is used to mark an offset and to set the offset list item.
        /// </remarks>
        protected internal sealed class MarkerItem : CFFFont.Item {
            internal CFFFont.OffsetItem p;

            public MarkerItem(CFFFont.OffsetItem pointerToMarker) {
                p = pointerToMarker;
            }

            public override void Xref() {
                p.Set(this.myOffset);
            }
        }

        /// <summary>a utility that creates a range item for an entire index</summary>
        /// <param name="indexOffset">where the index is</param>
        /// <returns>a range item representing the entire index</returns>
        protected internal virtual CFFFont.RangeItem GetEntireIndexRange(int indexOffset) {
            Seek(indexOffset);
            int count = GetCard16();
            if (count == 0) {
                return new CFFFont.RangeItem(buf, indexOffset, 2);
            }
            else {
                int indexOffSize = GetCard8();
                Seek(indexOffset + 2 + 1 + count * indexOffSize);
                int size = GetOffset(indexOffSize) - 1;
                return new CFFFont.RangeItem(buf, indexOffset, 2 + 1 + (count + 1) * indexOffSize + size);
            }
        }

        /// <summary>get a single CID font.</summary>
        /// <remarks>
        /// get a single CID font. The PDF architecture (1.4)
        /// supports 16-bit strings only with CID CFF fonts, not
        /// in Type-1 CFF fonts, so we convert the font to CID if
        /// it is in the Type-1 format.
        /// Two other tasks that we need to do are to select
        /// only a single font from the CFF package (this again is
        /// a PDF restriction) and to subset the CharStrings glyph
        /// description.
        /// </remarks>
        /// <param name="fontName">name of the font</param>
        /// <returns>byte array represents the CID font</returns>
        public virtual byte[] GetCID(String fontName) {
            //throws java.io.FileNotFoundException
            int j;
            for (j = 0; j < fonts.Length; j++) {
                if (fontName.Equals(fonts[j].name)) {
                    break;
                }
            }
            if (j == fonts.Length) {
                return null;
            }
            LinkedList<CFFFont.Item> l = new LinkedList<CFFFont.Item>();
            // copy the header
            Seek(0);
            int major = GetCard8();
            int minor = GetCard8();
            int hdrSize = GetCard8();
            int offSize = GetCard8();
            nextIndexOffset = hdrSize;
            l.AddLast(new CFFFont.RangeItem(buf, 0, hdrSize));
            int nglyphs = -1;
            int nstrings = -1;
            if (!fonts[j].isCID) {
                // count the glyphs
                Seek(fonts[j].charstringsOffset);
                nglyphs = GetCard16();
                Seek(stringIndexOffset);
                nstrings = GetCard16() + standardStrings.Length;
            }
            //System.err.println("number of glyphs = "+nglyphs);
            // create a name index
            // count
            l.AddLast(new CFFFont.UInt16Item((char)1));
            // offSize
            l.AddLast(new CFFFont.UInt8Item((char)1));
            // first offset
            l.AddLast(new CFFFont.UInt8Item((char)1));
            l.AddLast(new CFFFont.UInt8Item((char)(1 + fonts[j].name.Length)));
            l.AddLast(new CFFFont.StringItem(fonts[j].name));
            // create the topdict Index
            // count
            l.AddLast(new CFFFont.UInt16Item((char)1));
            // offSize
            l.AddLast(new CFFFont.UInt8Item((char)2));
            // first offset
            l.AddLast(new CFFFont.UInt16Item((char)1));
            CFFFont.OffsetItem topdictIndex1Ref = new CFFFont.IndexOffsetItem(2);
            l.AddLast(topdictIndex1Ref);
            CFFFont.IndexBaseItem topdictBase = new CFFFont.IndexBaseItem();
            l.AddLast(topdictBase);
            /*
            int maxTopdictLen = (topdictOffsets[j+1]-topdictOffsets[j])
            + 9*2 // at most 9 new keys
            + 8*5 // 8 new integer arguments
            + 3*2;// 3 new SID arguments
            */
            //int    topdictNext = 0;
            //byte[] topdict = new byte[maxTopdictLen];
            CFFFont.OffsetItem charsetRef = new CFFFont.DictOffsetItem();
            CFFFont.OffsetItem charstringsRef = new CFFFont.DictOffsetItem();
            CFFFont.OffsetItem fdarrayRef = new CFFFont.DictOffsetItem();
            CFFFont.OffsetItem fdselectRef = new CFFFont.DictOffsetItem();
            if (!fonts[j].isCID) {
                // create a ROS key
                l.AddLast(new CFFFont.DictNumberItem(nstrings));
                l.AddLast(new CFFFont.DictNumberItem(nstrings + 1));
                l.AddLast(new CFFFont.DictNumberItem(0));
                l.AddLast(new CFFFont.UInt8Item((char)12));
                l.AddLast(new CFFFont.UInt8Item((char)30));
                // create a CIDCount key
                l.AddLast(new CFFFont.DictNumberItem(nglyphs));
                l.AddLast(new CFFFont.UInt8Item((char)12));
                l.AddLast(new CFFFont.UInt8Item((char)34));
            }
            // What about UIDBase (12,35)? Don't know what is it.
            // I don't think we need FontName; the font I looked at didn't have it.
            // create an FDArray key
            l.AddLast(fdarrayRef);
            l.AddLast(new CFFFont.UInt8Item((char)12));
            l.AddLast(new CFFFont.UInt8Item((char)36));
            // create an FDSelect key
            l.AddLast(fdselectRef);
            l.AddLast(new CFFFont.UInt8Item((char)12));
            l.AddLast(new CFFFont.UInt8Item((char)37));
            // create an charset key
            l.AddLast(charsetRef);
            l.AddLast(new CFFFont.UInt8Item((char)15));
            // create a CharStrings key
            l.AddLast(charstringsRef);
            l.AddLast(new CFFFont.UInt8Item((char)17));
            Seek(topdictOffsets[j]);
            while (GetPosition() < topdictOffsets[j + 1]) {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                if ("Encoding".Equals(key) || "Private".Equals(key) || "FDSelect".Equals(key) || "FDArray".Equals(key) || 
                    "charset".Equals(key) || "CharStrings".Equals(key)) {
                }
                else {
                    // just drop them
                    l.AddLast(new CFFFont.RangeItem(buf, p1, p2 - p1));
                }
            }
            l.AddLast(new CFFFont.IndexMarkerItem(topdictIndex1Ref, topdictBase));
            // Copy the string index and append new strings.
            // We need 3 more strings: Registry, Ordering, and a FontName for one FD.
            // The total length is at most "Adobe"+"Identity"+63 = 76
            if (fonts[j].isCID) {
                l.AddLast(GetEntireIndexRange(stringIndexOffset));
            }
            else {
                String fdFontName = fonts[j].name + "-OneRange";
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
                l.AddLast(new CFFFont.UInt16Item((char)(stringOffsets.Length - 1 + 3)));
                // offSize
                l.AddLast(new CFFFont.UInt8Item((char)stringsIndexOffSize));
                foreach (int stringOffset in stringOffsets) {
                    l.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, stringOffset - stringsBaseOffset));
                }
                int currentStringsOffset = stringOffsets[stringOffsets.Length - 1] - stringsBaseOffset;
                // l.addLast(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
                currentStringsOffset += "Adobe".Length;
                l.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
                currentStringsOffset += "Identity".Length;
                l.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
                currentStringsOffset += fdFontName.Length;
                l.AddLast(new CFFFont.IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
                l.AddLast(new CFFFont.RangeItem(buf, stringOffsets[0], origStringsLen));
                l.AddLast(new CFFFont.StringItem(extraStrings));
            }
            // copy the global subroutine index
            l.AddLast(GetEntireIndexRange(gsubrIndexOffset));
            // deal with fdarray, fdselect, and the font descriptors
            if (fonts[j].isCID) {
            }
            else {
                // copy the FDArray, FDSelect, charset
                // create FDSelect
                l.AddLast(new CFFFont.MarkerItem(fdselectRef));
                // format identifier
                l.AddLast(new CFFFont.UInt8Item((char)3));
                // nRanges
                l.AddLast(new CFFFont.UInt16Item((char)1));
                // Range[0].firstGlyph
                l.AddLast(new CFFFont.UInt16Item((char)0));
                // Range[0].fd
                l.AddLast(new CFFFont.UInt8Item((char)0));
                // sentinel
                l.AddLast(new CFFFont.UInt16Item((char)nglyphs));
                // recreate a new charset
                // This format is suitable only for fonts without subsetting
                l.AddLast(new CFFFont.MarkerItem(charsetRef));
                // format identifier
                l.AddLast(new CFFFont.UInt8Item((char)2));
                // first glyph in range (ignore .notdef)
                l.AddLast(new CFFFont.UInt16Item((char)1));
                // nLeft
                l.AddLast(new CFFFont.UInt16Item((char)(nglyphs - 1)));
                // now all are covered, the data structure is complete.
                // create a font dict index (fdarray)
                l.AddLast(new CFFFont.MarkerItem(fdarrayRef));
                l.AddLast(new CFFFont.UInt16Item((char)1));
                // offSize
                l.AddLast(new CFFFont.UInt8Item((char)1));
                // first offset
                l.AddLast(new CFFFont.UInt8Item((char)1));
                CFFFont.OffsetItem privateIndex1Ref = new CFFFont.IndexOffsetItem(1);
                l.AddLast(privateIndex1Ref);
                CFFFont.IndexBaseItem privateBase = new CFFFont.IndexBaseItem();
                l.AddLast(privateBase);
                // looking at the PS that acrobat generates from a PDF with
                // a CFF opentype font embedded with an identity-H encoding,
                // it seems that it does not need a FontName.
                //l.addLast(new DictNumberItem((standardStrings.length+(stringOffsets.length-1)+2)));
                //l.addLast(new UInt8Item((char)12));
                //l.addLast(new UInt8Item((char)38)); // FontName
                l.AddLast(new CFFFont.DictNumberItem(fonts[j].privateLength));
                CFFFont.OffsetItem privateRef = new CFFFont.DictOffsetItem();
                l.AddLast(privateRef);
                // Private
                l.AddLast(new CFFFont.UInt8Item((char)18));
                l.AddLast(new CFFFont.IndexMarkerItem(privateIndex1Ref, privateBase));
                // copy the private index & local subroutines
                l.AddLast(new CFFFont.MarkerItem(privateRef));
                // copy the private dict and the local subroutines.
                // the length of the private dict seems to NOT include
                // the local subroutines.
                l.AddLast(new CFFFont.RangeItem(buf, fonts[j].privateOffset, fonts[j].privateLength));
                if (fonts[j].privateSubrs >= 0) {
                    //System.err.println("has subrs="+fonts[j].privateSubrs+" ,len="+fonts[j].privateLength);
                    l.AddLast(GetEntireIndexRange(fonts[j].privateSubrs));
                }
            }
            // copy the charstring index
            l.AddLast(new CFFFont.MarkerItem(charstringsRef));
            l.AddLast(GetEntireIndexRange(fonts[j].charstringsOffset));
            // now create the new CFF font
            int[] currentOffset = new int[1];
            currentOffset[0] = 0;
            foreach (CFFFont.Item item in l) {
                item.Increment(currentOffset);
            }
            foreach (CFFFont.Item item in l) {
                item.Xref();
            }
            int size = currentOffset[0];
            byte[] b = new byte[size];
            foreach (CFFFont.Item item in l) {
                item.Emit(b);
            }
            return b;
        }

        public virtual bool IsCID() {
            return IsCID(GetNames()[0]);
        }

        public virtual bool IsCID(String fontName) {
            int j;
            for (j = 0; j < fonts.Length; j++) {
                if (fontName.Equals(fonts[j].name)) {
                    return fonts[j].isCID;
                }
            }
            return false;
        }

        public virtual bool Exists(String fontName) {
            int j;
            for (j = 0; j < fonts.Length; j++) {
                if (fontName.Equals(fonts[j].name)) {
                    return true;
                }
            }
            return false;
        }

        public virtual String[] GetNames() {
            String[] names = new String[fonts.Length];
            for (int i = 0; i < fonts.Length; i++) {
                names[i] = fonts[i].name;
            }
            return names;
        }

        /// <summary>A random Access File or an array</summary>
        protected internal RandomAccessFileOrArray buf;

        private int offSize;

        protected internal int nameIndexOffset;

        protected internal int topdictIndexOffset;

        protected internal int stringIndexOffset;

        protected internal int gsubrIndexOffset;

        protected internal int[] nameOffsets;

        protected internal int[] topdictOffsets;

        protected internal int[] stringOffsets;

        protected internal int[] gsubrOffsets;

        protected internal sealed class Font {
            public String name;

            public String fullName;

            public bool isCID = false;

            // only if not CID
            public int privateOffset = -1;

            // only if not CID
            public int privateLength = -1;

            public int privateSubrs = -1;

            public int charstringsOffset = -1;

            public int encodingOffset = -1;

            public int charsetOffset = -1;

            // only if CID
            public int fdarrayOffset = -1;

            // only if CID
            public int fdselectOffset = -1;

            public int[] fdprivateOffsets;

            public int[] fdprivateLengths;

            public int[] fdprivateSubrs;

            // Added by Oren & Ygal
            public int nglyphs;

            public int nstrings;

            public int CharsetLength;

            public int[] charstringsOffsets;

            public int[] charset;

            public int[] FDSelect;

            public int FDSelectLength;

            public int FDSelectFormat;

            public int CharstringType = 2;

            public int FDArrayCount;

            public int FDArrayOffsize;

            public int[] FDArrayOffsets;

            public int[] PrivateSubrsOffset;

            public int[][] PrivateSubrsOffsetsArray;

            public int[] SubrsOffsets;

            public int[] gidToCid;

            internal Font(CFFFont _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly CFFFont _enclosing;
        }

        // Changed from private to protected by Ygal&Oren
        protected internal CFFFont.Font[] fonts;

        internal RandomAccessSourceFactory rasFactory = new RandomAccessSourceFactory();

        public CFFFont(byte[] cff) {
            //System.err.println("CFF: nStdString = "+standardStrings.length);
            buf = new RandomAccessFileOrArray(rasFactory.CreateSource(cff));
            Seek(0);
            int major;
            int minor;
            major = GetCard8();
            minor = GetCard8();
            //System.err.println("CFF Major-Minor = "+major+"-"+minor);
            int hdrSize = GetCard8();
            offSize = GetCard8();
            //System.err.println("offSize = "+offSize);
            //int count, indexOffSize, indexOffset, nextOffset;
            nameIndexOffset = hdrSize;
            nameOffsets = GetIndex(nameIndexOffset);
            topdictIndexOffset = nameOffsets[nameOffsets.Length - 1];
            topdictOffsets = GetIndex(topdictIndexOffset);
            stringIndexOffset = topdictOffsets[topdictOffsets.Length - 1];
            stringOffsets = GetIndex(stringIndexOffset);
            gsubrIndexOffset = stringOffsets[stringOffsets.Length - 1];
            gsubrOffsets = GetIndex(gsubrIndexOffset);
            fonts = new CFFFont.Font[nameOffsets.Length - 1];
            // now get the name index
            /*
            names             = new String[nfonts];
            privateOffset     = new int[nfonts];
            charsetOffset     = new int[nfonts];
            encodingOffset    = new int[nfonts];
            charstringsOffset = new int[nfonts];
            fdarrayOffset     = new int[nfonts];
            fdselectOffset    = new int[nfonts];
            */
            for (int j = 0; j < nameOffsets.Length - 1; j++) {
                fonts[j] = new CFFFont.Font(this);
                Seek(nameOffsets[j]);
                fonts[j].name = "";
                for (int k = nameOffsets[j]; k < nameOffsets[j + 1]; k++) {
                    fonts[j].name += GetCard8();
                }
            }
            //System.err.println("name["+j+"]=<"+fonts[j].name+">");
            // string index
            //strings = new String[stringOffsets.length-1];
            /*
            System.err.println("std strings = "+standardStrings.length);
            System.err.println("fnt strings = "+(stringOffsets.length-1));
            for (char j=0; j<standardStrings.length+(stringOffsets.length-1); j++) {
            //seek(stringOffsets[j]);
            //strings[j] = "";
            //for (int k=stringOffsets[j]; k<stringOffsets[j+1]; k++) {
            //	strings[j] += (char)getCard8();
            //}
            System.err.println("j="+(int)j+" <? "+(standardStrings.length+(stringOffsets.length-1)));
            System.err.println("strings["+(int)j+"]=<"+getString(j)+">");
            }
            */
            // top dict
            for (int j = 0; j < topdictOffsets.Length - 1; j++) {
                Seek(topdictOffsets[j]);
                while (GetPosition() < topdictOffsets[j + 1]) {
                    GetDictItem();
                    if (key == "FullName") {
                        //System.err.println("getting fullname sid = "+((Integer)args[0]).intValue());
                        fonts[j].fullName = GetString((char)((int?)args[0]).Value);
                    }
                    else {
                        //System.err.println("got it");
                        if (key == "ROS") {
                            fonts[j].isCID = true;
                        }
                        else {
                            if (key == "Private") {
                                fonts[j].privateLength = (int)((int?)args[0]).Value;
                                fonts[j].privateOffset = (int)((int?)args[1]).Value;
                            }
                            else {
                                if (key == "charset") {
                                    fonts[j].charsetOffset = (int)((int?)args[0]).Value;
                                }
                                else {
                                    //                else if (key=="Encoding"){
                                    //                    int encOffset = ((Integer)args[0]).intValue();
                                    //                    if (encOffset > 0) {
                                    //                        fonts[j].encodingOffset = encOffset;
                                    //                        ReadEncoding(fonts[j].encodingOffset);
                                    //                    }
                                    //                }
                                    if (key == "CharStrings") {
                                        fonts[j].charstringsOffset = (int)((int?)args[0]).Value;
                                        //System.err.println("charstrings "+fonts[j].charstringsOffset);
                                        // Added by Oren & Ygal
                                        int p = GetPosition();
                                        fonts[j].charstringsOffsets = GetIndex(fonts[j].charstringsOffset);
                                        Seek(p);
                                    }
                                    else {
                                        if (key == "FDArray") {
                                            fonts[j].fdarrayOffset = (int)((int?)args[0]).Value;
                                        }
                                        else {
                                            if (key == "FDSelect") {
                                                fonts[j].fdselectOffset = (int)((int?)args[0]).Value;
                                            }
                                            else {
                                                if (key == "CharstringType") {
                                                    fonts[j].CharstringType = (int)((int?)args[0]).Value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // private dict
                if (fonts[j].privateOffset >= 0) {
                    //System.err.println("PRIVATE::");
                    Seek(fonts[j].privateOffset);
                    while (GetPosition() < fonts[j].privateOffset + fonts[j].privateLength) {
                        GetDictItem();
                        if (key == "Subrs") {
                            //Add the private offset to the lsubrs since the offset is
                            // relative to the beginning of the PrivateDict
                            fonts[j].privateSubrs = (int)((int?)args[0]).Value + fonts[j].privateOffset;
                        }
                    }
                }
                // fdarray index
                if (fonts[j].fdarrayOffset >= 0) {
                    int[] fdarrayOffsets = GetIndex(fonts[j].fdarrayOffset);
                    fonts[j].fdprivateOffsets = new int[fdarrayOffsets.Length - 1];
                    fonts[j].fdprivateLengths = new int[fdarrayOffsets.Length - 1];
                    //System.err.println("FD Font::");
                    for (int k = 0; k < fdarrayOffsets.Length - 1; k++) {
                        Seek(fdarrayOffsets[k]);
                        while (GetPosition() < fdarrayOffsets[k + 1]) {
                            GetDictItem();
                            if (key == "Private") {
                                fonts[j].fdprivateLengths[k] = (int)((int?)args[0]).Value;
                                fonts[j].fdprivateOffsets[k] = (int)((int?)args[1]).Value;
                            }
                        }
                    }
                }
            }
        }

        //System.err.println("CFF: done");
        // ADDED BY Oren & Ygal
        internal virtual void ReadEncoding(int nextIndexOffset) {
            int format;
            Seek(nextIndexOffset);
            format = GetCard8();
        }
    }
}
