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
using iText.IO.Font;
using iText.IO.Font.Constants;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    /// <summary>A Meta Font.</summary>
    public class MetaFont : MetaObject {
        internal static readonly String[] fontNames = new String[] { "Courier", "Courier-Bold", "Courier-Oblique", 
            "Courier-BoldOblique", "Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique", "Times-Roman"
            , "Times-Bold", "Times-Italic", "Times-BoldItalic", "Symbol", "ZapfDingbats" };

        internal const int MARKER_BOLD = 1;

        internal const int MARKER_ITALIC = 2;

        internal const int MARKER_COURIER = 0;

        internal const int MARKER_HELVETICA = 4;

        internal const int MARKER_TIMES = 8;

        internal const int MARKER_SYMBOL = 12;

        internal const int DEFAULT_PITCH = 0;

        internal const int FIXED_PITCH = 1;

        internal const int VARIABLE_PITCH = 2;

        internal const int FF_DONTCARE = 0;

        internal const int FF_ROMAN = 1;

        internal const int FF_SWISS = 2;

        internal const int FF_MODERN = 3;

        internal const int FF_SCRIPT = 4;

        internal const int FF_DECORATIVE = 5;

        internal const int BOLDTHRESHOLD = 600;

        internal const int NAME_SIZE = 32;

        internal const int ETO_OPAQUE = 2;

        internal const int ETO_CLIPPED = 4;

        internal int height;

        internal float angle;

        internal int bold;

        internal int italic;

        internal bool underline;

        internal bool strikeout;

        internal int charset;

        internal int pitchAndFamily;

        internal String faceName = "arial";

        internal FontProgram font = null;

        internal FontEncoding encoding = null;

        /// <summary>Creates a MetaFont instance.</summary>
        public MetaFont()
            : base(META_FONT) {
        }

        /// <summary>Initializes the MetaFont instance.</summary>
        /// <param name="in">InputMeta containing the WMF data</param>
        public virtual void Init(InputMeta @in) {
            height = Math.Abs(@in.ReadShort());
            @in.Skip(2);
            angle = (float)(@in.ReadShort() / 1800.0 * Math.PI);
            @in.Skip(2);
            bold = (@in.ReadShort() >= BOLDTHRESHOLD ? MARKER_BOLD : 0);
            italic = (@in.ReadByte() != 0 ? MARKER_ITALIC : 0);
            underline = (@in.ReadByte() != 0);
            strikeout = (@in.ReadByte() != 0);
            charset = @in.ReadByte();
            @in.Skip(3);
            pitchAndFamily = @in.ReadByte();
            byte[] name = new byte[NAME_SIZE];
            int k;
            for (k = 0; k < NAME_SIZE; ++k) {
                int c = @in.ReadByte();
                if (c == 0) {
                    break;
                }
                name[k] = (byte)c;
            }
            try {
                faceName = iText.Commons.Utils.JavaUtil.GetStringForBytes(name, 0, k, "Cp1252");
            }
            catch (ArgumentException) {
                faceName = iText.Commons.Utils.JavaUtil.GetStringForBytes(name, 0, k);
            }
            faceName = faceName.ToLowerInvariant();
        }

        /// <summary>Returns the Font.</summary>
        /// <returns>the font</returns>
        public virtual FontProgram GetFont() {
            if (font != null) {
                return font;
            }
            FontProgram ff2 = FontProgramFactory.CreateRegisteredFont(faceName, ((italic != 0) ? FontStyles.ITALIC : 0
                ) | ((bold != 0) ? FontStyles.BOLD : 0));
            encoding = FontEncoding.CreateFontEncoding(PdfEncodings.WINANSI);
            font = ff2;
            if (font != null) {
                return font;
            }
            String fontName;
            if (faceName.Contains("courier") || faceName.Contains("terminal") || faceName.Contains("fixedsys")) {
                fontName = fontNames[MARKER_COURIER + italic + bold];
            }
            else {
                if (faceName.Contains("ms sans serif") || faceName.Contains("arial") || faceName.Contains("system")) {
                    fontName = fontNames[MARKER_HELVETICA + italic + bold];
                }
                else {
                    if (faceName.Contains("arial black")) {
                        fontName = fontNames[MARKER_HELVETICA + italic + MARKER_BOLD];
                    }
                    else {
                        if (faceName.Contains("times") || faceName.Contains("ms serif") || faceName.Contains("roman")) {
                            fontName = fontNames[MARKER_TIMES + italic + bold];
                        }
                        else {
                            if (faceName.Contains("symbol")) {
                                fontName = fontNames[MARKER_SYMBOL];
                            }
                            else {
                                int pitch = pitchAndFamily & 3;
                                int family = (pitchAndFamily >> 4) & 7;
                                switch (family) {
                                    case FF_MODERN: {
                                        fontName = fontNames[MARKER_COURIER + italic + bold];
                                        break;
                                    }

                                    case FF_ROMAN: {
                                        fontName = fontNames[MARKER_TIMES + italic + bold];
                                        break;
                                    }

                                    case FF_SWISS:
                                    case FF_SCRIPT:
                                    case FF_DECORATIVE: {
                                        fontName = fontNames[MARKER_HELVETICA + italic + bold];
                                        break;
                                    }

                                    default: {
                                        switch (pitch) {
                                            case FIXED_PITCH: {
                                                fontName = fontNames[MARKER_COURIER + italic + bold];
                                                break;
                                            }

                                            default: {
                                                fontName = fontNames[MARKER_HELVETICA + italic + bold];
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            try {
                font = FontProgramFactory.CreateFont(fontName);
                encoding = FontEncoding.CreateFontEncoding(PdfEncodings.WINANSI);
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message, e);
            }
            return font;
        }

        /// <summary>Returns the encoding used in the MetaFont.</summary>
        /// <returns>the font encoding</returns>
        public virtual FontEncoding GetEncoding() {
            return encoding;
        }

        /// <summary>Returns the angle of the MetaFont.</summary>
        /// <returns>the angle</returns>
        public virtual float GetAngle() {
            return angle;
        }

        /// <summary>Returns a boolean value indicating if the font is underlined or not.</summary>
        /// <returns>true if the font is underlined</returns>
        public virtual bool IsUnderline() {
            return underline;
        }

        /// <summary>Returns a boolean value indicating if a font has a strikeout.</summary>
        /// <returns>true if the font set strikeout</returns>
        public virtual bool IsStrikeout() {
            return strikeout;
        }

        /// <summary>Returns the font size.</summary>
        /// <param name="state">the MetaState</param>
        /// <returns>font size</returns>
        public virtual float GetFontSize(MetaState state) {
            return Math.Abs(state.TransformY(height) - state.TransformY(0)) * WmfImageHelper.wmfFontCorrection;
        }
    }
}
