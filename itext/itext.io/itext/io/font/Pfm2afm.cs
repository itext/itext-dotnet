/*
*
* This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
* Authors: Bruno Lowagie, Paulo Soares, et al.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License version 3
* as published by the Free Software Foundation with the addition of the
* following permission added to Section 15 as permitted in Section 7(a):
* FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
* ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
* OF THIRD PARTY RIGHTS
*
* This program is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
* or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU Affero General Public License for more details.
* You should have received a copy of the GNU Affero General Public License
* along with this program; if not, see http://www.gnu.org/licenses or write to
* the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
* Boston, MA, 02110-1301 USA, or download the license from the following URL:
* http://itextpdf.com/terms-of-use/
*
* The interactive user interfaces in modified source and object code versions
* of this program must display Appropriate Legal Notices, as required under
* Section 5 of the GNU Affero General Public License.
*
* In accordance with Section 7(b) of the GNU Affero General Public License,
* a covered work must retain the producer line in every PDF that is created
* or manipulated using iText.
*
* You can be released from the requirements of the license by purchasing
* a commercial license. Buying such a license is mandatory as soon as you
* develop commercial activities involving the iText software without
* disclosing the source code of your own applications.
* These activities include: offering paid services to customers as an ASP,
* serving PDFs on the fly in a web application, shipping iText with a closed
* source product.
*
* For more information, please contact iText Software Corp. at this
* address: sales@itextpdf.com
*/
using System;
using System.IO;
using System.Text;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font {
    /// <summary>Converts a PFM file into an AFM file.</summary>
    public sealed class Pfm2afm {
        private RandomAccessFileOrArray input;

        private StreamWriter output;

        /// <summary>Creates a new instance of Pfm2afm</summary>
        /// <exception cref="System.IO.IOException"/>
        private Pfm2afm(RandomAccessFileOrArray input, Stream output) {
            this.input = input;
            this.output = FileUtil.CreatePrintWriter(output, "ISO-8859-1");
        }

        /// <summary>Converts a PFM file into an AFM file.</summary>
        /// <param name="input">the PFM file</param>
        /// <param name="output">the AFM file</param>
        /// <exception cref="System.IO.IOException">on error</exception>
        public static void Convert(RandomAccessFileOrArray input, Stream output) {
            iText.IO.Font.Pfm2afm p = new iText.IO.Font.Pfm2afm(input, output);
            p.Openpfm();
            p.Putheader();
            p.Putchartab();
            p.Putkerntab();
            p.Puttrailer();
            p.output.Flush();
        }

        /// <exception cref="System.IO.IOException"/>
        private String ReadString(int n) {
            byte[] b = new byte[n];
            input.ReadFully(b);
            int k;
            for (k = 0; k < b.Length; ++k) {
                if (b[k] == 0) {
                    break;
                }
            }
            return iText.IO.Util.JavaUtil.GetStringForBytes(b, 0, k, "ISO-8859-1");
        }

        /// <exception cref="System.IO.IOException"/>
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

        /// <exception cref="System.IO.IOException"/>
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
            if (h_len != input.Length() || extlen != 30 || fontname < 75 || fontname > 512) {
                throw new System.IO.IOException("not.a.valid.pfm.file");
            }
            input.Seek(psext + 14);
            capheight = input.ReadShortLE();
            xheight = input.ReadShortLE();
            ascender = input.ReadShortLE();
            descender = input.ReadShortLE();
        }

        /// <exception cref="System.IO.IOException"/>
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
            if ((kind & 1) == 0 || avgwidth == maxwidth) {
                /* Flag for mono */
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

        /// <exception cref="System.IO.IOException"/>
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

        /// <exception cref="System.IO.IOException"/>
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

        private String copyright;

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

        private short pixwidth;

        private short pixheight;

        private byte kind;

        private short avgwidth;

        private short maxwidth;

        private int firstchar;

        private int lastchar;

        private byte defchar;

        private byte brkchar;

        private short widthby;

        private int device;

        private int face;

        private int bits;

        private int bitoff;

        private short extlen;

        private int psext;

        private int chartab;

        private int res1;

        private int kernpairs;

        private int res2;

        private int fontname;

        private short capheight;

        private short xheight;

        private short ascender;

        private short descender;

        private bool isMono;

        /// <summary>Translate table from 1004 to psstd.</summary>
        /// <remarks>
        /// Translate table from 1004 to psstd.  1004 is an extension of the
        /// Windows translate table used in PM.
        /// </remarks>
        private int[] Win2PSStd = new int[] { 0, 0, 0, 0, 197, 198, 199, 0, 202, 0, 205, 206, 207, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 33, 34, 35, 36, 37, 38, 169, 40, 41, 42, 43, 44, 45, 46, 
            47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72
            , 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 193, 97, 
            98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 
            119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 0, 184, 166, 185, 188, 178, 179, 195, 189, 0, 172, 234
            , 0, 0, 0, 0, 96, 0, 170, 186, 183, 177, 208, 196, 0, 0, 173, 250, 0, 0, 0, 0, 161, 162, 163, 168, 165
            , 0, 167, 200, 0, 227, 171, 0, 0, 0, 197, 0, 0, 0, 0, 194, 0, 182, 180, 203, 0, 235, 187, 0, 0, 0, 191
            , 0, 0, 0, 0, 0, 0, 225, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 233, 0, 0, 0, 0, 0, 0, 251
            , 0, 0, 0, 0, 0, 0, 241, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 249, 0, 0, 0, 0, 0, 0, 0 };

        /// <summary>Windows character names.</summary>
        /// <remarks>
        /// Windows character names.  Give a name to the used locations
        /// for when the all flag is specified.
        /// </remarks>
        private String[] WinChars = new String[] { "W00", "W01", "W02", "W03", "macron", "breve", "dotaccent", "W07"
            , "ring", "W09", "W0a", "W0b", "W0c", "W0d", "W0e", "W0f", "hungarumlaut", "ogonek", "caron", "W13", "W14"
            , "W15", "W16", "W17", "W18", "W19", "W1a", "W1b", "W1c", "W1d", "W1e", "W1f", "space", "exclam", "quotedbl"
            , "numbersign", "dollar", "percent", "ampersand", "quotesingle", "parenleft", "parenright", "asterisk"
            , "plus", "comma", "hyphen", "period", "slash", "zero", "one", "two", "three", "four", "five", "six", 
            "seven", "eight", "nine", "colon", "semicolon", "less", "equal", "greater", "question", "at", "A", "B"
            , "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", 
            "W", "X", "Y", "Z", "bracketleft", "backslash", "bracketright", "asciicircum", "underscore", "grave", 
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u"
            , "v", "w", "x", "y", "z", "braceleft", "bar", "braceright", "asciitilde", "W7f", "euro", "W81", "quotesinglbase"
            , "florin", "quotedblbase", "ellipsis", "dagger", "daggerdbl", "circumflex", "perthousand", "Scaron", 
            "guilsinglleft", "OE", "W8d", "Zcaron", "W8f", "W90", "quoteleft", "quoteright", "quotedblleft", "quotedblright"
            , "bullet", "endash", "emdash", "tilde", "trademark", "scaron", "guilsinglright", "oe", "W9d", "zcaron"
            , "Ydieresis", "reqspace", "exclamdown", "cent", "sterling", "currency", "yen", "brokenbar", "section"
            , "dieresis", "copyright", "ordfeminine", "guillemotleft", "logicalnot", "syllable", "registered", "macron"
            , "degree", "plusminus", "twosuperior", "threesuperior", "acute", "mu", "paragraph", "periodcentered", 
            "cedilla", "onesuperior", "ordmasculine", "guillemotright", "onequarter", "onehalf", "threequarters", 
            "questiondown", "Agrave", "Aacute", "Acircumflex", "Atilde", "Adieresis", "Aring", "AE", "Ccedilla", "Egrave"
            , "Eacute", "Ecircumflex", "Edieresis", "Igrave", "Iacute", "Icircumflex", "Idieresis", "Eth", "Ntilde"
            , "Ograve", "Oacute", "Ocircumflex", "Otilde", "Odieresis", "multiply", "Oslash", "Ugrave", "Uacute", 
            "Ucircumflex", "Udieresis", "Yacute", "Thorn", "germandbls", "agrave", "aacute", "acircumflex", "atilde"
            , "adieresis", "aring", "ae", "ccedilla", "egrave", "eacute", "ecircumflex", "edieresis", "igrave", "iacute"
            , "icircumflex", "idieresis", "eth", "ntilde", "ograve", "oacute", "ocircumflex", "otilde", "odieresis"
            , "divide", "oslash", "ugrave", "uacute", "ucircumflex", "udieresis", "yacute", "thorn", "ydieresis" };
        /* Total length of .pfm file */
        /* Copyright string [60]*/
        /* 0=windows, otherwise nomap */
        /* Width for mono fonts */
        /* Lower bit off in mono */
        /* Mono if avg=max width */
        /* Use to compute bounding box */
        /* First char in table */
        /* Last char in table */
        /* Face name */
        /* PostScript extension */
        /* Character width tables */
        /* Kerning pairs */
        /* Font name */
        /*
        *  Some metrics from the PostScript extension
        */
        /* Cap height */
        /* X height */
        /* Ascender */
        /* Descender (positive) */
        // 00
        // 10
        // 20
        // 30
        // 40
        // 50
        // 60
        // 70
        // 80
        // 90
        // A0
        // B0
        // C0
        // D0
        // E0
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
        /*   00    */
        /*   01    */
        /*   02    */
        /*   03    */
        /*   04    */
        /*   05    */
        /*   06    */
        /*   07    */
        /*   08    */
        /*   09    */
        /*   0a    */
        /*   0b    */
        /*   0c    */
        /*   0d    */
        /*   0e    */
        /*   0f    */
        /*   10    */
        /*   11    */
        /*   12    */
        /*   13    */
        /*   14    */
        /*   15    */
        /*   16    */
        /*   17    */
        /*   18    */
        /*   19    */
        /*   1a    */
        /*   1b    */
        /*   1c    */
        /*   1d    */
        /*   1e    */
        /*   1f    */
        /*   20    */
        /*   21    */
        /*   22    */
        /*   23    */
        /*   24    */
        /*   25    */
        /*   26    */
        /*   27    */
        /*   28    */
        /*   29    */
        /*   2A    */
        /*   2B    */
        /*   2C    */
        /*   2D    */
        /*   2E    */
        /*   2F    */
        /*   30    */
        /*   31    */
        /*   32    */
        /*   33    */
        /*   34    */
        /*   35    */
        /*   36    */
        /*   37    */
        /*   38    */
        /*   39    */
        /*   3A    */
        /*   3B    */
        /*   3C    */
        /*   3D    */
        /*   3E    */
        /*   3F    */
        /*   40    */
        /*   41    */
        /*   42    */
        /*   43    */
        /*   44    */
        /*   45    */
        /*   46    */
        /*   47    */
        /*   48    */
        /*   49    */
        /*   4A    */
        /*   4B    */
        /*   4C    */
        /*   4D    */
        /*   4E    */
        /*   4F    */
        /*   50    */
        /*   51    */
        /*   52    */
        /*   53    */
        /*   54    */
        /*   55    */
        /*   56    */
        /*   57    */
        /*   58    */
        /*   59    */
        /*   5A    */
        /*   5B    */
        /*   5C    */
        /*   5D    */
        /*   5E    */
        /*   5F    */
        /*   60    */
        /*   61    */
        /*   62    */
        /*   63    */
        /*   64    */
        /*   65    */
        /*   66    */
        /*   67    */
        /*   68    */
        /*   69    */
        /*   6A    */
        /*   6B    */
        /*   6C    */
        /*   6D    */
        /*   6E    */
        /*   6F    */
        /*   70    */
        /*   71    */
        /*   72    */
        /*   73    */
        /*   74    */
        /*   75    */
        /*   76    */
        /*   77    */
        /*   78    */
        /*   79    */
        /*   7A    */
        /*   7B    */
        /*   7C    */
        /*   7D    */
        /*   7E    */
        /*   7F    */
        /*   80    */
        /*   81    */
        /*   82    */
        /*   83    */
        /*   84    */
        /*   85    */
        /*   86    */
        /*   87    */
        /*   88    */
        /*   89    */
        /*   8A    */
        /*   8B    */
        /*   8C    */
        /*   8D    */
        /*   8E    */
        /*   8F    */
        /*   90    */
        /*   91    */
        /*   92    */
        /*   93    */
        /*   94    */
        /*   95    */
        /*   96    */
        /*   97    */
        /*   98    */
        /*   99    */
        /*   9A    */
        /*   9B    */
        /*   9C    */
        /*   9D    */
        /*   9E    */
        /*   9F    */
        /*   A0    */
        /*   A1    */
        /*   A2    */
        /*   A3    */
        /*   A4    */
        /*   A5    */
        /*   A6    */
        /*   A7    */
        /*   A8    */
        /*   A9    */
        /*   AA    */
        /*   AB    */
        /*   AC    */
        /*   AD    */
        /*   AE    */
        /*   AF    */
        /*   B0    */
        /*   B1    */
        /*   B2    */
        /*   B3    */
        /*   B4    */
        /*   B5    */
        /*   B6    */
        /*   B7    */
        /*   B8    */
        /*   B9    */
        /*   BA    */
        /*   BB    */
        /*   BC    */
        /*   BD    */
        /*   BE    */
        /*   BF    */
        /*   C0    */
        /*   C1    */
        /*   C2    */
        /*   C3    */
        /*   C4    */
        /*   C5    */
        /*   C6    */
        /*   C7    */
        /*   C8    */
        /*   C9    */
        /*   CA    */
        /*   CB    */
        /*   CC    */
        /*   CD    */
        /*   CE    */
        /*   CF    */
        /*   D0    */
        /*   D1    */
        /*   D2    */
        /*   D3    */
        /*   D4    */
        /*   D5    */
        /*   D6    */
        /*   D7    */
        /*   D8    */
        /*   D9    */
        /*   DA    */
        /*   DB    */
        /*   DC    */
        /*   DD    */
        /*   DE    */
        /*   DF    */
        /*   E0    */
        /*   E1    */
        /*   E2    */
        /*   E3    */
        /*   E4    */
        /*   E5    */
        /*   E6    */
        /*   E7    */
        /*   E8    */
        /*   E9    */
        /*   EA    */
        /*   EB    */
        /*   EC    */
        /*   ED    */
        /*   EE    */
        /*   EF    */
        /*   F0    */
        /*   F1    */
        /*   F2    */
        /*   F3    */
        /*   F4    */
        /*   F5    */
        /*   F6    */
        /*   F7    */
        /*   F8    */
        /*   F9    */
        /*   FA    */
        /*   FB    */
        /*   FC    */
        /*   FD    */
        /*   FE    */
        /*   FF    */
    }
}
