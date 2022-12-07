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
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    public abstract class PdfFont : PdfObjectWrapper<PdfDictionary> {
        /// <summary>The upper bound value for char code.</summary>
        /// <remarks>
        /// The upper bound value for char code. As for simple fonts char codes are a single byte values,
        /// it may vary from 0 to 255.
        /// </remarks>
        public const int SIMPLE_FONT_MAX_CHAR_CODE_VALUE = 255;

        protected internal FontProgram fontProgram;

        protected internal static readonly byte[] EMPTY_BYTES = new byte[0];

        protected internal IDictionary<int, Glyph> notdefGlyphs = new Dictionary<int, Glyph>();

        /// <summary>false, if the font comes from PdfDocument.</summary>
        protected internal bool newFont = true;

        /// <summary>true if the font is to be embedded in the PDF.</summary>
        protected internal bool embedded = false;

        /// <summary>Indicates if all the glyphs and widths for that particular encoding should be included in the document.
        ///     </summary>
        protected internal bool subset = true;

        protected internal IList<int[]> subsetRanges;

        protected internal PdfFont(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            GetPdfObject().Put(PdfName.Type, PdfName.Font);
        }

        protected internal PdfFont()
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.Type, PdfName.Font);
        }

        /// <summary>Get glyph by unicode</summary>
        /// <param name="unicode">a unicode code point</param>
        /// <returns>
        /// 
        /// <see cref="iText.IO.Font.Otf.Glyph"/>
        /// if it exists or .NOTDEF if supported, otherwise
        /// <see langword="null"/>.
        /// </returns>
        public abstract Glyph GetGlyph(int unicode);

        /// <summary>Check whether font contains glyph with specified unicode.</summary>
        /// <param name="unicode">a unicode code point</param>
        /// <returns>
        /// true if font contains glyph, represented with the unicode code point,
        /// otherwise false.
        /// </returns>
        public virtual bool ContainsGlyph(int unicode) {
            Glyph glyph = GetGlyph(unicode);
            if (glyph != null) {
                if (GetFontProgram() != null && GetFontProgram().IsFontSpecific()) {
                    //if current is symbolic, zero code is valid value
                    return glyph.GetCode() > -1;
                }
                else {
                    return glyph.GetCode() > 0;
                }
            }
            else {
                return false;
            }
        }

        public abstract GlyphLine CreateGlyphLine(String content);

        /// <summary>Append all supported glyphs and return number of processed chars.</summary>
        /// <remarks>
        /// Append all supported glyphs and return number of processed chars.
        /// Composite font supports surrogate pairs.
        /// </remarks>
        /// <param name="text">String to convert to glyphs.</param>
        /// <param name="from">from index of the text.</param>
        /// <param name="to">to index of the text.</param>
        /// <param name="glyphs">array for a new glyphs, shall not be null.</param>
        /// <returns>number of processed chars from text.</returns>
        public abstract int AppendGlyphs(String text, int from, int to, IList<Glyph> glyphs);

        /// <summary>Append any single glyph, even notdef.</summary>
        /// <remarks>
        /// Append any single glyph, even notdef.
        /// Returns number of processed chars: 2 in case surrogate pair, otherwise 1.
        /// </remarks>
        /// <param name="text">String to convert to glyphs.</param>
        /// <param name="from">from index of the text.</param>
        /// <param name="glyphs">array for a new glyph, shall not be null.</param>
        /// <returns>number of processed chars: 2 in case surrogate pair, otherwise 1</returns>
        public abstract int AppendAnyGlyph(String text, int from, IList<Glyph> glyphs);

        /// <summary>Converts the text into bytes to be placed in the document.</summary>
        /// <remarks>
        /// Converts the text into bytes to be placed in the document.
        /// The conversion is done according to the font and the encoding and the characters
        /// used are stored.
        /// </remarks>
        /// <param name="text">the text to convert</param>
        /// <returns>the conversion</returns>
        public abstract byte[] ConvertToBytes(String text);

        public abstract byte[] ConvertToBytes(GlyphLine glyphLine);

        public abstract String Decode(PdfString content);

        /// <summary>
        /// Decodes sequence of character codes (e.g. from content stream) into a
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// </summary>
        /// <param name="characterCodes">
        /// the string which is interpreted as a sequence of character codes. Note, that
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// acts as a storage for char code values specific to given font, therefore
        /// individual character codes must not be interpreted as code units of the UTF-16 encoding
        /// </param>
        /// <returns>
        /// the
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// containing the glyphs encoded by the passed string
        /// </returns>
        public abstract GlyphLine DecodeIntoGlyphLine(PdfString characterCodes);

        /// <summary>
        /// Decodes sequence of character codes (e.g. from content stream) to sequence of glyphs
        /// and appends them to the passed list.
        /// </summary>
        /// <param name="list">the list to the end of which decoded glyphs are to be added</param>
        /// <param name="characterCodes">
        /// the string which is interpreted as a sequence of character codes. Note, that
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// acts as a storage for char code values specific to given font, therefore
        /// individual character codes must not be interpreted as code units of the UTF-16 encoding
        /// </param>
        /// <returns>true if all codes where successfully decoded, false otherwise</returns>
        public virtual bool AppendDecodedCodesToGlyphsList(IList<Glyph> list, PdfString characterCodes) {
            return false;
        }

        public abstract float GetContentWidth(PdfString content);

        public abstract byte[] ConvertToBytes(Glyph glyph);

        public abstract void WriteText(GlyphLine text, int from, int to, PdfOutputStream stream);

        public abstract void WriteText(String text, PdfOutputStream stream);

        /// <summary>Returns the width of a certain character of this font in 1000 normalized units.</summary>
        /// <param name="unicode">a certain character.</param>
        /// <returns>a width in Text Space.</returns>
        public virtual int GetWidth(int unicode) {
            Glyph glyph = GetGlyph(unicode);
            return glyph != null ? glyph.GetWidth() : 0;
        }

        /// <summary>Returns the width of a certain character of this font in points.</summary>
        /// <param name="unicode">a certain character.</param>
        /// <param name="fontSize">the font size.</param>
        /// <returns>a width in points.</returns>
        public virtual float GetWidth(int unicode, float fontSize) {
            return FontProgram.ConvertTextSpaceToGlyphSpace(GetWidth(unicode) * fontSize);
        }

        /// <summary>Returns the width of a string of this font in 1000 normalized units.</summary>
        /// <param name="text">a string content.</param>
        /// <returns>a width of string in Text Space.</returns>
        public virtual int GetWidth(String text) {
            int total = 0;
            for (int i = 0; i < text.Length; i++) {
                int ch;
                if (iText.IO.Util.TextUtil.IsSurrogatePair(text, i)) {
                    ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, i);
                    i++;
                }
                else {
                    ch = text[i];
                }
                Glyph glyph = GetGlyph(ch);
                if (glyph != null) {
                    total += glyph.GetWidth();
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the width of a
        /// <c>String</c>
        /// in points.
        /// </summary>
        /// <param name="text">
        /// the
        /// <c>String</c>
        /// to get the width of
        /// </param>
        /// <param name="fontSize">the font size</param>
        /// <returns>the width in points</returns>
        public virtual float GetWidth(String text, float fontSize) {
            return FontProgram.ConvertTextSpaceToGlyphSpace(GetWidth(text) * fontSize);
        }

        /// <summary>
        /// Gets the descent of a
        /// <c>String</c>
        /// in points.
        /// </summary>
        /// <remarks>
        /// Gets the descent of a
        /// <c>String</c>
        /// in points. The descent will always be
        /// less than or equal to zero even if all the characters have an higher descent.
        /// </remarks>
        /// <param name="text">
        /// the
        /// <c>String</c>
        /// to get the descent of
        /// </param>
        /// <param name="fontSize">the font size</param>
        /// <returns>the descent in points</returns>
        public virtual int GetDescent(String text, float fontSize) {
            int min = 0;
            for (int k = 0; k < text.Length; ++k) {
                int ch;
                if (iText.IO.Util.TextUtil.IsSurrogatePair(text, k)) {
                    ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, k);
                    k++;
                }
                else {
                    ch = text[k];
                }
                Glyph glyph = GetGlyph(ch);
                if (glyph != null) {
                    int[] bbox = glyph.GetBbox();
                    if (bbox != null && bbox[1] < min) {
                        min = bbox[1];
                    }
                    else {
                        if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoDescender() < min) {
                            min = GetFontProgram().GetFontMetrics().GetTypoDescender();
                        }
                    }
                }
            }
            return (int)FontProgram.ConvertTextSpaceToGlyphSpace(min * fontSize);
        }

        /// <summary>Gets the descent of a char code in points.</summary>
        /// <remarks>
        /// Gets the descent of a char code in points. The descent will always be
        /// less than or equal to zero even if all the characters have an higher descent.
        /// </remarks>
        /// <param name="unicode">the char code to get the descent of</param>
        /// <param name="fontSize">the font size</param>
        /// <returns>the descent in points</returns>
        public virtual int GetDescent(int unicode, float fontSize) {
            int min = 0;
            Glyph glyph = GetGlyph(unicode);
            if (glyph == null) {
                return 0;
            }
            int[] bbox = glyph.GetBbox();
            if (bbox != null && bbox[1] < min) {
                min = bbox[1];
            }
            else {
                if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoDescender() < min) {
                    min = GetFontProgram().GetFontMetrics().GetTypoDescender();
                }
            }
            return (int)FontProgram.ConvertTextSpaceToGlyphSpace(min * fontSize);
        }

        /// <summary>
        /// Gets the ascent of a
        /// <c>String</c>
        /// in points.
        /// </summary>
        /// <remarks>
        /// Gets the ascent of a
        /// <c>String</c>
        /// in points. The ascent will always be
        /// greater than or equal to zero even if all the characters have a lower ascent.
        /// </remarks>
        /// <param name="text">
        /// the
        /// <c>String</c>
        /// to get the ascent of
        /// </param>
        /// <param name="fontSize">the font size</param>
        /// <returns>the ascent in points</returns>
        public virtual int GetAscent(String text, float fontSize) {
            int max = 0;
            for (int k = 0; k < text.Length; ++k) {
                int ch;
                if (iText.IO.Util.TextUtil.IsSurrogatePair(text, k)) {
                    ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, k);
                    k++;
                }
                else {
                    ch = text[k];
                }
                Glyph glyph = GetGlyph(ch);
                if (glyph != null) {
                    int[] bbox = glyph.GetBbox();
                    if (bbox != null && bbox[3] > max) {
                        max = bbox[3];
                    }
                    else {
                        if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoAscender() > max) {
                            max = GetFontProgram().GetFontMetrics().GetTypoAscender();
                        }
                    }
                }
            }
            return (int)FontProgram.ConvertTextSpaceToGlyphSpace(max * fontSize);
        }

        /// <summary>Gets the ascent of a char code in normalized 1000 units.</summary>
        /// <remarks>
        /// Gets the ascent of a char code in normalized 1000 units. The ascent will always be
        /// greater than or equal to zero even if all the characters have a lower ascent.
        /// </remarks>
        /// <param name="unicode">the char code to get the ascent of</param>
        /// <param name="fontSize">the font size</param>
        /// <returns>the ascent in points</returns>
        public virtual int GetAscent(int unicode, float fontSize) {
            int max = 0;
            Glyph glyph = GetGlyph(unicode);
            if (glyph == null) {
                return 0;
            }
            int[] bbox = glyph.GetBbox();
            if (bbox != null && bbox[3] > max) {
                max = bbox[3];
            }
            else {
                if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoAscender() > max) {
                    max = GetFontProgram().GetFontMetrics().GetTypoAscender();
                }
            }
            return (int)FontProgram.ConvertTextSpaceToGlyphSpace(max * fontSize);
        }

        public virtual FontProgram GetFontProgram() {
            return fontProgram;
        }

        public virtual bool IsEmbedded() {
            return embedded;
        }

        /// <summary>
        /// Indicates if all the glyphs and widths for that particular
        /// encoding should be included in the document.
        /// </summary>
        /// <returns><c>false</c> to include all the glyphs and widths.</returns>
        public virtual bool IsSubset() {
            return subset;
        }

        /// <summary>
        /// Indicates if all the glyphs and widths for that particular
        /// encoding should be included in the document.
        /// </summary>
        /// <remarks>
        /// Indicates if all the glyphs and widths for that particular
        /// encoding should be included in the document. When set to
        /// <see langword="true"/>
        /// only the glyphs used will be included in the font. When set to
        /// <see langword="false"/>
        /// the full font will be included and all subset ranges will be removed.
        /// </remarks>
        /// <param name="subset">new value of property subset</param>
        /// <seealso cref="AddSubsetRange(int[])"/>
        public virtual void SetSubset(bool subset) {
            this.subset = subset;
        }

        /// <summary>Adds a character range when subsetting.</summary>
        /// <remarks>
        /// Adds a character range when subsetting. The range is an <c>int</c> array
        /// where the first element is the start range inclusive and the second element is the
        /// end range inclusive. Several ranges are allowed in the same array.
        /// Note, #setSubset(true) will be called implicitly
        /// therefore this range is an addition to the used glyphs.
        /// </remarks>
        /// <param name="range">the character range</param>
        public virtual void AddSubsetRange(int[] range) {
            if (subsetRanges == null) {
                subsetRanges = new List<int[]>();
            }
            subsetRanges.Add(range);
            SetSubset(true);
        }

        public virtual IList<String> SplitString(String text, float fontSize, float maxWidth) {
            IList<String> resultString = new List<String>();
            int lastWhiteSpace = 0;
            int startPos = 0;
            float tokenLength = 0;
            for (int i = 0; i < text.Length; i++) {
                char ch = text[i];
                if (iText.IO.Util.TextUtil.IsWhiteSpace(ch)) {
                    lastWhiteSpace = i;
                }
                float currentCharWidth = GetWidth(ch, fontSize);
                if (tokenLength + currentCharWidth >= maxWidth || ch == '\n') {
                    if (startPos < lastWhiteSpace) {
                        resultString.Add(text.JSubstring(startPos, lastWhiteSpace));
                        startPos = lastWhiteSpace + 1;
                        tokenLength = 0;
                        i = lastWhiteSpace;
                    }
                    else {
                        if (startPos != i) {
                            resultString.Add(text.JSubstring(startPos, i));
                            startPos = i;
                            tokenLength = currentCharWidth;
                        }
                        else {
                            resultString.Add(text.JSubstring(startPos, startPos + 1));
                            startPos = i + 1;
                            tokenLength = 0;
                        }
                    }
                }
                else {
                    tokenLength += currentCharWidth;
                }
            }
            resultString.Add(text.Substring(startPos));
            return resultString;
        }

        /// <summary>
        /// Checks whether the
        /// <see cref="PdfFont"/>
        /// was built with corresponding fontProgram and encoding or CMAP.
        /// </summary>
        /// <remarks>
        /// Checks whether the
        /// <see cref="PdfFont"/>
        /// was built with corresponding fontProgram and encoding or CMAP.
        /// Default value is false unless overridden.
        /// </remarks>
        /// <param name="fontProgram">a font name or path to a font program</param>
        /// <param name="encoding">an encoding or CMAP</param>
        /// <returns>true, if the PdfFont was built with the fontProgram and encoding. Otherwise false.</returns>
        /// <seealso cref="iText.Kernel.Pdf.PdfDocument.FindFont(System.String, System.String)"/>
        /// <seealso cref="iText.IO.Font.FontProgram.IsBuiltWith(System.String)"/>
        /// <seealso cref="iText.IO.Font.FontEncoding.IsBuiltWith(System.String)"/>
        /// <seealso cref="iText.IO.Font.CMapEncoding.IsBuiltWith(System.String)"/>
        public virtual bool IsBuiltWith(String fontProgram, String encoding) {
            return false;
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        protected internal abstract PdfDictionary GetFontDescriptor(String fontName);

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>Adds a unique subset prefix to be added to the font name when the font is embedded and subsetted.
        ///     </summary>
        /// <param name="fontName">the original font name.</param>
        /// <param name="isSubset">denotes whether font in question is subsetted, i.e. only used symbols are kept in it.
        ///     </param>
        /// <param name="isEmbedded">denotes whether font in question is embedded into the PDF document.</param>
        /// <returns>
        /// the font name prefixed with subset if isSubset and isEmbedded are true,
        /// otherwise original font name is returned intact.
        /// </returns>
        protected internal static String UpdateSubsetPrefix(String fontName, bool isSubset, bool isEmbedded) {
            if (isSubset && isEmbedded) {
                return FontUtil.AddRandomSubsetPrefixForFontName(fontName);
            }
            return fontName;
        }

        /// <summary>
        /// Create
        /// <c>PdfStream</c>
        /// based on
        /// <paramref name="fontStreamBytes"/>.
        /// </summary>
        /// <param name="fontStreamBytes">original font data, must be not null.</param>
        /// <param name="fontStreamLengths">
        /// array to generate
        /// <c>Length*</c>
        /// keys, must be not null.
        /// </param>
        /// <returns>
        /// the PdfStream containing the font or
        /// <see langword="null"/>
        /// , if there is an error reading the font.
        /// </returns>
        protected internal virtual PdfStream GetPdfFontStream(byte[] fontStreamBytes, int[] fontStreamLengths) {
            if (fontStreamBytes == null || fontStreamLengths == null) {
                throw new PdfException(KernelExceptionMessageConstant.FONT_EMBEDDING_ISSUE);
            }
            PdfStream fontStream = new PdfStream(fontStreamBytes);
            MakeObjectIndirect(fontStream);
            for (int k = 0; k < fontStreamLengths.Length; ++k) {
                fontStream.Put(new PdfName("Length" + (k + 1)), new PdfNumber(fontStreamLengths[k]));
            }
            return fontStream;
        }

        /// <summary>Helper method for making an object indirect, if the object already is indirect.</summary>
        /// <remarks>
        /// Helper method for making an object indirect, if the object already is indirect.
        /// Useful for FontDescriptor and FontFile to make possible immediate flushing.
        /// If there is no PdfDocument, mark the object as
        /// <c>MUST_BE_INDIRECT</c>.
        /// </remarks>
        /// <param name="obj">an object to make indirect.</param>
        /// <returns>
        /// if current object isn't indirect, returns
        /// <see langword="false"/>
        /// , otherwise
        /// <c>tree</c>
        /// </returns>
        internal virtual bool MakeObjectIndirect(PdfObject obj) {
            if (GetPdfObject().GetIndirectReference() != null) {
                obj.MakeIndirect(GetPdfObject().GetIndirectReference().GetDocument());
                return true;
            }
            else {
                MarkObjectAsIndirect(obj);
                return false;
            }
        }

        public override String ToString() {
            return "PdfFont{" + "fontProgram=" + fontProgram + '}';
        }
    }
}
