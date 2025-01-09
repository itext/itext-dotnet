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
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Source;

namespace iText.IO.Font {
    /// <summary>Converts a PFM file into an AFM file.</summary>
    public sealed class Pfm2afm {
        private RandomAccessFileOrArray input;

        private FormattingStreamWriter output;

        /// <summary>Creates a new instance of Pfm2afm</summary>
        private Pfm2afm(RandomAccessFileOrArray input, Stream output) {
            this.input = input;
            this.output = (FormattingStreamWriter)FileUtil.CreatePrintWriter(output, "ISO-8859-1");
        }

        /// <summary>Converts a PFM file into an AFM file.</summary>
        /// <param name="input">the PFM file</param>
        /// <param name="output">the AFM file</param>
        public static void Convert(RandomAccessFileOrArray input, Stream output) {
            iText.IO.Font.Pfm2afm p = new iText.IO.Font.Pfm2afm(input, output);
            p.Openpfm();
            p.Putheader();
            p.Putchartab();
            p.Putkerntab();
            p.Puttrailer();
            p.output.Flush();
        }

        private String ReadString(int n) {
            byte[] b = new byte[n];
            input.ReadFully(b);
            int k;
            for (k = 0; k < b.Length; ++k) {
                if (b[k] == 0) {
                    break;
                }
            }
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(b, 0, k, "ISO-8859-1");
        }

        private String ReadString() {
            StringBuilder buf = new StringBuilder();
            while (true) {
                int c = input.Read();
                if (c <= 0) {
                    break;
                }
                buf.Append((char)c);
            }
            return buf.ToString();
        }

        private void Outval(int n) {
            output.Write(' ');
            output.Write(n);
        }

        /*
        *  Output a character entry
        */
        private void Outchar(int code, int width, String name) {
            output.Write("C ");
            Outval(code);
            output.Write(" ; WX ");
            Outval(width);
            if (name != null) {
                output.Write(" ; N ");
                output.Write(name);
            }
            output.Write(" ;\n");
        }

        private void Openpfm() {
            input.Seek(0);
            vers = input.ReadShortLE();
            h_len = input.ReadIntLE();
            copyright = ReadString(60);
            type = input.ReadShortLE();
            points = input.ReadShortLE();
            verres = input.ReadShortLE();
            horres = input.ReadShortLE();
            ascent = input.ReadShortLE();
            intleading = input.ReadShortLE();
            extleading = input.ReadShortLE();
            italic = (byte)input.Read();
            uline = (byte)input.Read();
            overs = (byte)input.Read();
            weight = input.ReadShortLE();
            charset = (byte)input.Read();
            pixwidth = input.ReadShortLE();
            pixheight = input.ReadShortLE();
            kind = (byte)input.Read();
            avgwidth = input.ReadShortLE();
            maxwidth = input.ReadShortLE();
            firstchar = input.Read();
            lastchar = input.Read();
            defchar = (byte)input.Read();
            brkchar = (byte)input.Read();
            widthby = input.ReadShortLE();
            device = input.ReadIntLE();
            face = input.ReadIntLE();
            bits = input.ReadIntLE();
            bitoff = input.ReadIntLE();
            extlen = input.ReadShortLE();
            psext = input.ReadIntLE();
            chartab = input.ReadIntLE();
            res1 = input.ReadIntLE();
            kernpairs = input.ReadIntLE();
            res2 = input.ReadIntLE();
            fontname = input.ReadIntLE();
            //Those checks come from an old C implementation of pfm2afm, when reading the specs
            //Nothing indicates the fontnameOffset should be max 512
            if (h_len != input.Length() || extlen != 30 || fontname < 75 || fontname > 1024) {
                throw new System.IO.IOException("not.a.valid.pfm.file");
            }
            input.Seek(psext + 14);
            capheight = input.ReadShortLE();
            xheight = input.ReadShortLE();
            ascender = input.ReadShortLE();
            descender = input.ReadShortLE();
        }

        private void Putheader() {
            output.Write("StartFontMetrics 2.0\n");
            if (copyright.Length > 0) {
                output.Write("Comment " + copyright + '\n');
            }
            output.Write("FontName ");
            input.Seek(fontname);
            String fname = ReadString();
            output.Write(fname);
            output.Write("\nEncodingScheme ");
            if (charset != 0) {
                output.Write("FontSpecific\n");
            }
            else {
                output.Write("AdobeStandardEncoding\n");
            }
            /*
            * The .pfm is missing full name, so construct from font name by
            * changing the hyphen to a space.  This actually works in a lot
            * of cases.
            */
            output.Write("FullName " + fname.Replace('-', ' '));
            if (face != 0) {
                input.Seek(face);
                output.Write("\nFamilyName " + ReadString());
            }
            output.Write("\nWeight ");
            if (weight > 475 || fname.ToLowerInvariant().Contains("bold")) {
                output.Write("Bold");
            }
            else {
                if ((weight < 325 && weight != 0) || fname.ToLowerInvariant().Contains("light")) {
                    output.Write("Light");
                }
                else {
                    if (fname.ToLowerInvariant().Contains("black")) {
                        output.Write("Black");
                    }
                    else {
                        output.Write("Medium");
                    }
                }
            }
            output.Write("\nItalicAngle ");
            if (italic != 0 || fname.ToLowerInvariant().Contains("italic")) {
                output.Write("-12.00");
            }
            else {
                /* this is a typical value; something else may work better for a
                specific font */
                output.Write("0");
            }
            /*
            *  The mono flag in the pfm actually indicates whether there is a
            *  table of font widths, not if they are all the same.
            */
            output.Write("\nIsFixedPitch ");
            if ((kind & 1) == 0 || /* Flag for mono */ avgwidth == maxwidth) {
                /* Avg width = max width */
                output.Write("true");
                isMono = true;
            }
            else {
                output.Write("false");
                isMono = false;
            }
            /*
            * The font bounding box is lost, but try to reconstruct it.
            * Much of this is just guess work.  The bounding box is required in
            * the .afm, but is not used by the PM font installer.
            */
            output.Write("\nFontBBox");
            if (isMono) {
                Outval(-20);
            }
            else {
                /* Just guess at left bounds */
                Outval(-100);
            }
            Outval(-(descender + 5));
            /* Descender is given as positive value */
            Outval(maxwidth + 10);
            Outval(ascent + 5);
            /*
            * Give other metrics that were kept
            */
            output.Write("\nCapHeight");
            Outval(capheight);
            output.Write("\nXHeight");
            Outval(xheight);
            output.Write("\nDescender");
            Outval(-descender);
            output.Write("\nAscender");
            Outval(ascender);
            output.Write('\n');
        }

        private void Putchartab() {
            int count = lastchar - firstchar + 1;
            int[] ctabs = new int[count];
            input.Seek(chartab);
            for (int k = 0; k < count; ++k) {
                ctabs[k] = input.ReadUnsignedShortLE();
            }
            int[] back = new int[256];
            if (charset == 0) {
                for (int i = firstchar; i <= lastchar; ++i) {
                    if (Win2PSStd[i] != 0) {
                        back[Win2PSStd[i]] = i;
                    }
                }
            }
            /* Put out the header */
            output.Write("StartCharMetrics");
            Outval(count);
            output.Write('\n');
            /* Put out all encoded chars */
            if (charset != 0) {
                /*
                * If the charset is not the Windows standard, just put out
                * unnamed entries.
                */
                for (int i = firstchar; i <= lastchar; i++) {
                    if (ctabs[i - firstchar] != 0) {
                        Outchar(i, ctabs[i - firstchar], null);
                    }
                }
            }
            else {
                for (int i = 0; i < 256; i++) {
                    int j = back[i];
                    if (j != 0) {
                        Outchar(i, ctabs[j - firstchar], WinChars[j]);
                        ctabs[j - firstchar] = 0;
                    }
                }
                /* Put out all non-encoded chars */
                for (int i = firstchar; i <= lastchar; i++) {
                    if (ctabs[i - firstchar] != 0) {
                        Outchar(-1, ctabs[i - firstchar], WinChars[i]);
                    }
                }
            }
            /* Put out the trailer */
            output.Write("EndCharMetrics\n");
        }

        private void Putkerntab() {
            if (kernpairs == 0) {
                return;
            }
            input.Seek(kernpairs);
            int count = input.ReadUnsignedShortLE();
            int nzero = 0;
            int[] kerns = new int[count * 3];
            for (int k = 0; k < kerns.Length; ) {
                kerns[k++] = input.Read();
                kerns[k++] = input.Read();
                if ((kerns[k++] = input.ReadShortLE()) != 0) {
                    ++nzero;
                }
            }
            if (nzero == 0) {
                return;
            }
            output.Write("StartKernData\nStartKernPairs");
            Outval(nzero);
            output.Write('\n');
            for (int k = 0; k < kerns.Length; k += 3) {
                if (kerns[k + 2] != 0) {
                    output.Write("KPX ");
                    output.Write(WinChars[kerns[k]]);
                    output.Write(' ');
                    output.Write(WinChars[kerns[k + 1]]);
                    Outval(kerns[k + 2]);
                    output.Write('\n');
                }
            }
            /* Put out trailer */
            output.Write("EndKernPairs\nEndKernData\n");
        }

        private void Puttrailer() {
            output.Write("EndFontMetrics\n");
        }

        private short vers;

        private int h_len;

        /* Total length of .pfm file */
        private String copyright;

        /* Copyright string [60]*/
        private short type;

        private short points;

        private short verres;

        private short horres;

        private short ascent;

        private short intleading;

        private short extleading;

        private byte italic;

        private byte uline;

        private byte overs;

        private short weight;

        private byte charset;

        /* 0=windows, otherwise nomap */
        private short pixwidth;

        /* Width for mono fonts */
        private short pixheight;

        private byte kind;

        /* Lower bit off in mono */
        private short avgwidth;

        /* Mono if avg=max width */
        private short maxwidth;

        /* Use to compute bounding box */
        private int firstchar;

        /* First char in table */
        private int lastchar;

        /* Last char in table */
        private byte defchar;

        private byte brkchar;

        private short widthby;

        private int device;

        private int face;

        /* Face name */
        private int bits;

        private int bitoff;

        private short extlen;

        private int psext;

        /* PostScript extension */
        private int chartab;

        /* Character width tables */
        private int res1;

        private int kernpairs;

        /* Kerning pairs */
        private int res2;

        private int fontname;

        /* Font name */
        /*
        *  Some metrics from the PostScript extension
        */
        private short capheight;

        /* Cap height */
        private short xheight;

        /* X height */
        private short ascender;

        /* Ascender */
        private short descender;

        /* Descender (positive) */
        private bool isMono;

        /// <summary>Translate table from 1004 to psstd.</summary>
        /// <remarks>
        /// Translate table from 1004 to psstd.  1004 is an extension of the
        /// Windows translate table used in PM.
        /// </remarks>
        private int[] Win2PSStd = new int[] { 0, 0, 0, 0, 197, 198, 199, 0, 202, 0, 205, 206, 207, 0, 0, 0, 
                // 00
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                // 10
                32, 33, 34, 35, 36, 37, 38, 169, 40, 41, 42, 43, 44, 45, 46, 47, 
                // 20
                48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 
                // 30
                64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 
                // 40
                80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 
                // 50
                193, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 
                // 60
                112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 
                // 70
                128, 0, 184, 166, 185, 188, 178, 179, 195, 189, 0, 172, 234, 0, 0, 0, 
                // 80
                0, 96, 0, 170, 186, 183, 177, 208, 196, 0, 0, 173, 250, 0, 0, 0, 
                // 90
                0, 161, 162, 163, 168, 165, 0, 167, 200, 0, 227, 171, 0, 0, 0, 197, 
                // A0
                0, 0, 0, 0, 194, 0, 182, 180, 203, 0, 235, 187, 0, 0, 0, 191, 
                // B0
                0, 0, 0, 0, 0, 0, 225, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                // C0
                0, 0, 0, 0, 0, 0, 0, 0, 233, 0, 0, 0, 0, 0, 0, 251, 
                // D0
                0, 0, 0, 0, 0, 0, 241, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                // E0
                0, 0, 0, 0, 0, 0, 0, 0, 249, 0, 0, 0, 0, 0, 0, 0 };

        // F0
        //    /**
        //     *  Character class.  This is a minor attempt to overcome the problem that
        //     *  in the pfm file, all unused characters are given the width of space.
        //     *  Note that this array isn't used in iText.
        //     */
        //    private int[] WinClass = {
        //        0, 0, 0, 0, 2, 2, 2, 0, 2, 0, 2, 2, 2, 0, 0, 0,   /* 00 */
        //        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   /* 10 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 20 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 30 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 40 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 50 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 60 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2,   /* 70 */
        //        0, 0, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0,   /* 80 */
        //        0, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2,   /* 90 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* a0 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* b0 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* c0 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* d0 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* e0 */
        //        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1   /* f0 */
        //    };
        /// <summary>Windows character names.</summary>
        /// <remarks>
        /// Windows character names.  Give a name to the used locations
        /// for when the all flag is specified.
        /// </remarks>
        private String[] WinChars = new String[] { "W00", /*   00    */ "W01", /*   01    */ "W02", /*   02    */ 
            "W03", /*   03    */ "macron", /*   04    */ "breve", /*   05    */ "dotaccent", /*   06    */ "W07", 
            /*   07    */ "ring", /*   08    */ "W09", /*   09    */ "W0a", /*   0a    */ "W0b", /*   0b    */ "W0c"
            , /*   0c    */ "W0d", /*   0d    */ "W0e", /*   0e    */ "W0f", /*   0f    */ "hungarumlaut", /*   10    */ 
            "ogonek", /*   11    */ "caron", /*   12    */ "W13", /*   13    */ "W14", /*   14    */ "W15", /*   15    */ 
            "W16", /*   16    */ "W17", /*   17    */ "W18", /*   18    */ "W19", /*   19    */ "W1a", /*   1a    */ 
            "W1b", /*   1b    */ "W1c", /*   1c    */ "W1d", /*   1d    */ "W1e", /*   1e    */ "W1f", /*   1f    */ 
            "space", /*   20    */ "exclam", /*   21    */ "quotedbl", /*   22    */ "numbersign", /*   23    */ "dollar"
            , /*   24    */ "percent", /*   25    */ "ampersand", /*   26    */ "quotesingle", /*   27    */ "parenleft"
            , /*   28    */ "parenright", /*   29    */ "asterisk", /*   2A    */ "plus", /*   2B    */ "comma", /*   2C    */ 
            "hyphen", /*   2D    */ "period", /*   2E    */ "slash", /*   2F    */ "zero", /*   30    */ "one", /*   31    */ 
            "two", /*   32    */ "three", /*   33    */ "four", /*   34    */ "five", /*   35    */ "six", /*   36    */ 
            "seven", /*   37    */ "eight", /*   38    */ "nine", /*   39    */ "colon", /*   3A    */ "semicolon"
            , /*   3B    */ "less", /*   3C    */ "equal", /*   3D    */ "greater", /*   3E    */ "question", /*   3F    */ 
            "at", /*   40    */ "A", /*   41    */ "B", /*   42    */ "C", /*   43    */ "D", /*   44    */ "E", /*   45    */ 
            "F", /*   46    */ "G", /*   47    */ "H", /*   48    */ "I", /*   49    */ "J", /*   4A    */ "K", /*   4B    */ 
            "L", /*   4C    */ "M", /*   4D    */ "N", /*   4E    */ "O", /*   4F    */ "P", /*   50    */ "Q", /*   51    */ 
            "R", /*   52    */ "S", /*   53    */ "T", /*   54    */ "U", /*   55    */ "V", /*   56    */ "W", /*   57    */ 
            "X", /*   58    */ "Y", /*   59    */ "Z", /*   5A    */ "bracketleft", /*   5B    */ "backslash", /*   5C    */ 
            "bracketright", /*   5D    */ "asciicircum", /*   5E    */ "underscore", /*   5F    */ "grave", /*   60    */ 
            "a", /*   61    */ "b", /*   62    */ "c", /*   63    */ "d", /*   64    */ "e", /*   65    */ "f", /*   66    */ 
            "g", /*   67    */ "h", /*   68    */ "i", /*   69    */ "j", /*   6A    */ "k", /*   6B    */ "l", /*   6C    */ 
            "m", /*   6D    */ "n", /*   6E    */ "o", /*   6F    */ "p", /*   70    */ "q", /*   71    */ "r", /*   72    */ 
            "s", /*   73    */ "t", /*   74    */ "u", /*   75    */ "v", /*   76    */ "w", /*   77    */ "x", /*   78    */ 
            "y", /*   79    */ "z", /*   7A    */ "braceleft", /*   7B    */ "bar", /*   7C    */ "braceright", /*   7D    */ 
            "asciitilde", /*   7E    */ "W7f", /*   7F    */ "euro", /*   80    */ "W81", /*   81    */ "quotesinglbase"
            , /*   82    */ "florin", /*   83    */ "quotedblbase", /*   84    */ "ellipsis", /*   85    */ "dagger"
            , /*   86    */ "daggerdbl", /*   87    */ "circumflex", /*   88    */ "perthousand", /*   89    */ "Scaron"
            , /*   8A    */ "guilsinglleft", /*   8B    */ "OE", /*   8C    */ "W8d", /*   8D    */ "Zcaron", /*   8E    */ 
            "W8f", /*   8F    */ "W90", /*   90    */ "quoteleft", /*   91    */ "quoteright", /*   92    */ "quotedblleft"
            , /*   93    */ "quotedblright", /*   94    */ "bullet", /*   95    */ "endash", /*   96    */ "emdash"
            , /*   97    */ "tilde", /*   98    */ "trademark", /*   99    */ "scaron", /*   9A    */ "guilsinglright"
            , /*   9B    */ "oe", /*   9C    */ "W9d", /*   9D    */ "zcaron", /*   9E    */ "Ydieresis", /*   9F    */ 
            "reqspace", /*   A0    */ "exclamdown", /*   A1    */ "cent", /*   A2    */ "sterling", /*   A3    */ 
            "currency", /*   A4    */ "yen", /*   A5    */ "brokenbar", /*   A6    */ "section", /*   A7    */ "dieresis"
            , /*   A8    */ "copyright", /*   A9    */ "ordfeminine", /*   AA    */ "guillemotleft", /*   AB    */ 
            "logicalnot", /*   AC    */ "syllable", /*   AD    */ "registered", /*   AE    */ "macron", /*   AF    */ 
            "degree", /*   B0    */ "plusminus", /*   B1    */ "twosuperior", /*   B2    */ "threesuperior", /*   B3    */ 
            "acute", /*   B4    */ "mu", /*   B5    */ "paragraph", /*   B6    */ "periodcentered", /*   B7    */ 
            "cedilla", /*   B8    */ "onesuperior", /*   B9    */ "ordmasculine", /*   BA    */ "guillemotright", 
            /*   BB    */ "onequarter", /*   BC    */ "onehalf", /*   BD    */ "threequarters", /*   BE    */ "questiondown"
            , /*   BF    */ "Agrave", /*   C0    */ "Aacute", /*   C1    */ "Acircumflex", /*   C2    */ "Atilde", 
            /*   C3    */ "Adieresis", /*   C4    */ "Aring", /*   C5    */ "AE", /*   C6    */ "Ccedilla", /*   C7    */ 
            "Egrave", /*   C8    */ "Eacute", /*   C9    */ "Ecircumflex", /*   CA    */ "Edieresis", /*   CB    */ 
            "Igrave", /*   CC    */ "Iacute", /*   CD    */ "Icircumflex", /*   CE    */ "Idieresis", /*   CF    */ 
            "Eth", /*   D0    */ "Ntilde", /*   D1    */ "Ograve", /*   D2    */ "Oacute", /*   D3    */ "Ocircumflex"
            , /*   D4    */ "Otilde", /*   D5    */ "Odieresis", /*   D6    */ "multiply", /*   D7    */ "Oslash", 
            /*   D8    */ "Ugrave", /*   D9    */ "Uacute", /*   DA    */ "Ucircumflex", /*   DB    */ "Udieresis"
            , /*   DC    */ "Yacute", /*   DD    */ "Thorn", /*   DE    */ "germandbls", /*   DF    */ "agrave", /*   E0    */ 
            "aacute", /*   E1    */ "acircumflex", /*   E2    */ "atilde", /*   E3    */ "adieresis", /*   E4    */ 
            "aring", /*   E5    */ "ae", /*   E6    */ "ccedilla", /*   E7    */ "egrave", /*   E8    */ "eacute", 
            /*   E9    */ "ecircumflex", /*   EA    */ "edieresis", /*   EB    */ "igrave", /*   EC    */ "iacute"
            , /*   ED    */ "icircumflex", /*   EE    */ "idieresis", /*   EF    */ "eth", /*   F0    */ "ntilde", 
            /*   F1    */ "ograve", /*   F2    */ "oacute", /*   F3    */ "ocircumflex", /*   F4    */ "otilde", /*   F5    */ 
            "odieresis", /*   F6    */ "divide", /*   F7    */ "oslash", /*   F8    */ "ugrave", /*   F9    */ "uacute"
            , /*   FA    */ "ucircumflex", /*   FB    */ "udieresis", /*   FC    */ "yacute", /*   FD    */ "thorn"
            , /*   FE    */ "ydieresis" };
        /*   FF    */
    }
}
