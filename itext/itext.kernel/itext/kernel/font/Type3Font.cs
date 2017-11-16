/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.IO.Font;
using iText.IO.Font.Otf;

namespace iText.Kernel.Font {
    public class Type3Font : FontProgram {
        private readonly IDictionary<int, Type3Glyph> type3Glyphs = new Dictionary<int, Type3Glyph>();

        private bool colorized = false;

        private int flags = 0;

        public Type3Font(bool colorized) {
            this.colorized = colorized;
            this.fontNames = new FontNames();
            GetFontMetrics().SetBbox(0, 0, 0, 0);
        }

        public virtual Type3Glyph GetType3Glyph(int unicode) {
            return type3Glyphs.Get(unicode);
        }

        public override int GetPdfFontFlags() {
            return flags;
        }

        public override bool IsFontSpecific() {
            return false;
        }

        public virtual bool IsColorized() {
            return colorized;
        }

        public override int GetKerning(Glyph glyph1, Glyph glyph2) {
            return 0;
        }

        public virtual int GetGlyphsCount() {
            return type3Glyphs.Count;
        }

        /// <summary>Sets the PostScript name of the font.</summary>
        /// <param name="fontName">the PostScript name of the font, shall not be null or empty.</param>
        protected internal override void SetFontName(String fontName) {
            //This dummy override allows PdfType3Font to set font name.
            base.SetFontName(fontName);
        }

        /// <summary>Sets a preferred font family name.</summary>
        /// <param name="fontFamily">a preferred font family name.</param>
        protected internal override void SetFontFamily(String fontFamily) {
            //This dummy override allows PdfType3Font to set font family.
            base.SetFontFamily(fontFamily);
        }

        /// <summary>Sets font weight.</summary>
        /// <param name="fontWeight">
        /// integer form 100 to 900. See
        /// <see cref="iText.IO.Font.Constants.FontWeights"/>
        /// .
        /// </param>
        protected internal override void SetFontWeight(int fontWeight) {
            //This dummy override allows PdfType3Font to set font weight.
            base.SetFontWeight(fontWeight);
        }

        /// <summary>Sets font width in css notation (font-stretch property)</summary>
        /// <param name="fontWidth">
        /// 
        /// <see cref="iText.IO.Font.Constants.FontStretches"/>
        /// .
        /// </param>
        protected internal override void SetFontStretch(String fontWidth) {
            //This dummy override allows PdfType3Font to set font stretch.
            base.SetFontStretch(fontWidth);
        }

        /// <summary>Sets the PostScript italic angel.</summary>
        /// <remarks>
        /// Sets the PostScript italic angel.
        /// <br/>
        /// Italic angle in counter-clockwise degrees from the vertical. Zero for upright text, negative for text that leans to the right (forward).
        /// </remarks>
        /// <param name="italicAngle">in counter-clockwise degrees from the vertical</param>
        protected internal override void SetItalicAngle(int italicAngle) {
            //This dummy override allows PdfType3Font to set the PostScript italicAngel.
            base.SetItalicAngle(italicAngle);
        }

        /// <summary>Sets Font descriptor flags.</summary>
        /// <seealso cref="iText.IO.Font.Constants.FontDescriptorFlags"/>
        /// <param name="flags"/>
        internal virtual void SetPdfFontFlags(int flags) {
            this.flags = flags;
        }

        internal virtual void AddGlyph(int code, int unicode, int width, int[] bbox, Type3Glyph type3Glyph) {
            Glyph glyph = new Glyph(code, width, unicode, bbox);
            codeToGlyph.Put(code, glyph);
            unicodeToGlyph.Put(unicode, glyph);
            type3Glyphs.Put(unicode, type3Glyph);
        }
    }
}
