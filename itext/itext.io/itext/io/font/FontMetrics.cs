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
namespace iText.IO.Font {
    /// <summary>Holds the typographic metrics parsed from a font program.</summary>
    /// <remarks>
    /// Holds the typographic metrics parsed from a font program.
    /// <para />
    /// Unless stated otherwise, dimensional values are normalized to the PDF glyph space of 1000 units
    /// per em when they are assigned by the font readers.
    /// </remarks>
    public class FontMetrics {
        /// <summary>
        /// Multiplier that converts values expressed in the source font's units per em to normalized
        /// 1000-unit glyph space.
        /// </summary>
        /// <remarks>
        /// Multiplier that converts values expressed in the source font's units per em to normalized
        /// 1000-unit glyph space. Font readers update it after reading
        /// <c>head.unitsPerEm</c>.
        /// </remarks>
        protected internal float normalizationCoef = 1f;

        // head.unitsPerEm
        private int unitsPerEm = FontProgram.UNITS_NORMALIZATION;

        // maxp.numGlyphs
        private int numOfGlyphs;

        // hmtx
        private int[] glyphWidths;

        // os_2.sTypoAscender * normalization
        private int typoAscender = 800;

        // os_2.sTypoDescender * normalization
        private int typoDescender = -200;

        // os_2.sCapHeight * normalization
        private int capHeight = 700;

        // os_2.sxHeight * normalization
        private int xHeight = 0;

        // post.italicAngle
        private float italicAngle = 0;

        // llx: head.xMin * normalization; lly: head.yMin * normalization
        // urx: head.xMax * normalization; ury: head.yMax * normalization
        private int[] bbox = new int[] { -50, -200, 1000, 900 };

        // hhea.Ascender * normalization
        private int ascender;

        // hhea.Descender * normalization
        private int descender;

        // hhea.LineGap * normaliztion (leading)
        private int lineGap;

        // os_2.winAscender * normalization
        private int winAscender;

        // os_2.winDescender * normalization
        private int winDescender;

        // hhea.advanceWidthMax * normalization
        private int advanceWidthMax;

        // (post.underlinePosition - post.underlineThickness / 2) * normalization
        private int underlinePosition = -100;

        // post.underlineThickness * normalization
        private int underlineThickness = 50;

        // os_2.yStrikeoutPosition * normalization
        private int strikeoutPosition;

        // os_2.yStrikeoutSize * normalization
        private int strikeoutSize;

        // os_2.ySubscriptYSize * normalization
        private int subscriptSize;

        // -os_2.ySubscriptYOffset * normalization
        private int subscriptOffset;

        // os_2.ySuperscriptYSize * normalization
        private int superscriptSize;

        // os_2.ySuperscriptYOffset * normalization
        private int superscriptOffset;

        // in type1/cff it is stdVW
        private int stemV = 80;

        // in type1/cff it is stdHW
        private int stemH = 0;

        // post.isFixedPitch (monospaced)
        private bool isFixedPitch;

        /// <summary>Gets the units per em value declared by the source font.</summary>
        /// <returns>source font design units in one em</returns>
        public virtual int GetUnitsPerEm() {
            return unitsPerEm;
        }

        /// <summary>Gets the number of glyphs declared by the font.</summary>
        /// <returns>glyph count</returns>
        public virtual int GetNumberOfGlyphs() {
            return numOfGlyphs;
        }

        /// <summary>Gets the glyph width array.</summary>
        /// <returns>glyph widths indexed by glyph ID</returns>
        public virtual int[] GetGlyphWidths() {
            return glyphWidths;
        }

        /// <summary>Gets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to ascender.</summary>
        /// <remarks>
        /// Gets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to ascender.
        /// <para />
        /// Typo vertical metrics are the primary source for iText ascender/descender calculations.
        /// </remarks>
        /// <returns>typo ascender value in normalized 1000-units</returns>
        public virtual int GetTypoAscender() {
            return typoAscender;
        }

        /// <summary>Gets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to descender.</summary>
        /// <remarks>
        /// Gets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to descender.
        /// <para />
        /// Typo vertical metrics are the primary source for iText ascender/descender calculations.
        /// </remarks>
        /// <returns>typo descender value in normalized 1000-units</returns>
        public virtual int GetTypoDescender() {
            return typoDescender;
        }

        /// <summary>Gets the capital letters height.</summary>
        /// <remarks>
        /// Gets the capital letters height.
        /// <para />
        /// This property defines the vertical coordinate of the top of flat capital letters,
        /// measured from the baseline.
        /// </remarks>
        /// <returns>cap height in 1000-units</returns>
        public virtual int GetCapHeight() {
            return capHeight;
        }

        /// <summary>Gets the height of lowercase flat characters above the baseline.</summary>
        /// <returns>x-height in normalized glyph units, or zero when not present</returns>
        public virtual int GetXHeight() {
            return xHeight;
        }

        /// <summary>Gets the PostScript italic angle.</summary>
        /// <returns>counterclockwise degrees from vertical; negative values lean right</returns>
        public virtual float GetItalicAngle() {
            return italicAngle;
        }

        /// <summary>Gets the font bounding box.</summary>
        /// <returns>array containing lower-left x/y and upper-right x/y in normalized glyph units</returns>
        public virtual int[] GetBbox() {
            return bbox;
        }

        /// <summary>Replaces the normalized font bounding box.</summary>
        /// <param name="llx">lower-left x coordinate</param>
        /// <param name="lly">lower-left y coordinate</param>
        /// <param name="urx">upper-right x coordinate</param>
        /// <param name="ury">upper-right y coordinate</param>
        public virtual void SetBbox(int llx, int lly, int urx, int ury) {
            bbox[0] = llx;
            bbox[1] = lly;
            bbox[2] = urx;
            bbox[3] = ury;
        }

        /// <summary>Gets the horizontal ascender.</summary>
        /// <returns>ascender in normalized glyph units</returns>
        public virtual int GetAscender() {
            return ascender;
        }

        /// <summary>Gets the horizontal descender.</summary>
        /// <returns>descender in normalized glyph units</returns>
        public virtual int GetDescender() {
            return descender;
        }

        /// <summary>Gets the line spacing.</summary>
        /// <returns>horizontal line gap in normalized glyph units</returns>
        public virtual int GetLineGap() {
            return lineGap;
        }

        /// <summary>Gets the Windows ascender metric.</summary>
        /// <returns>
        /// OS/2
        /// <c>usWinAscent</c>
        /// in normalized glyph units
        /// </returns>
        public virtual int GetWinAscender() {
            return winAscender;
        }

        /// <summary>Gets the Windows descender metric.</summary>
        /// <returns>
        /// OS/2
        /// <c>usWinDescent</c>
        /// in normalized glyph units
        /// </returns>
        public virtual int GetWinDescender() {
            return winDescender;
        }

        /// <summary>Gets the largest horizontal advance width in the font.</summary>
        /// <returns>maximum advance width in normalized glyph units</returns>
        public virtual int GetAdvanceWidthMax() {
            return advanceWidthMax;
        }

        /// <summary>Gets the underline center position relative to the baseline.</summary>
        /// <returns>normalized underline position, adjusted from the stored edge by half its thickness</returns>
        public virtual int GetUnderlinePosition() {
            return underlinePosition - underlineThickness / 2;
        }

        /// <summary>Gets the recommended underline thickness.</summary>
        /// <returns>stored thickness value; readers currently supply it in source font units</returns>
        public virtual int GetUnderlineThickness() {
            return underlineThickness;
        }

        /// <summary>Gets the strikeout position relative to the baseline.</summary>
        /// <returns>OS/2 strikeout position in normalized glyph units</returns>
        public virtual int GetStrikeoutPosition() {
            return strikeoutPosition;
        }

        /// <summary>Gets the strikeout thickness.</summary>
        /// <returns>OS/2 strikeout size in normalized glyph units</returns>
        public virtual int GetStrikeoutSize() {
            return strikeoutSize;
        }

        /// <summary>Gets the recommended vertical size for subscripts.</summary>
        /// <returns>subscript y-size in normalized glyph units</returns>
        public virtual int GetSubscriptSize() {
            return subscriptSize;
        }

        /// <summary>Gets the subscript baseline offset.</summary>
        /// <returns>subscript y-offset in normalized glyph units</returns>
        public virtual int GetSubscriptOffset() {
            return subscriptOffset;
        }

        /// <summary>Gets the recommended vertical size for superscripts.</summary>
        /// <returns>superscript y-size in source font units</returns>
        public virtual int GetSuperscriptSize() {
            return superscriptSize;
        }

        /// <summary>Gets the superscript baseline offset.</summary>
        /// <returns>superscript y-offset in normalized glyph units</returns>
        public virtual int GetSuperscriptOffset() {
            return superscriptOffset;
        }

        /// <summary>Gets the dominant vertical stem width.</summary>
        /// <returns>
        /// Type 1/CFF
        /// <c>StdVW</c>
        /// value, or the reader's fallback, in glyph units
        /// </returns>
        public virtual int GetStemV() {
            return stemV;
        }

        /// <summary>Gets the dominant horizontal stem width.</summary>
        /// <returns>
        /// Type 1/CFF
        /// <c>StdHW</c>
        /// value, or the reader's fallback, in glyph units
        /// </returns>
        public virtual int GetStemH() {
            return stemH;
        }

        /// <summary>Checks whether all glyphs use a common width.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when the font declares fixed pitch
        /// </returns>
        public virtual bool IsFixedPitch() {
            return isFixedPitch;
        }

        /// <summary>Sets source units per em.</summary>
        /// <remarks>
        /// Sets source units per em.
        /// <para />
        /// It recalculates the normalization multiplier used by subsequent setters.
        /// </remarks>
        /// <param name="unitsPerEm">positive number of design units in an em</param>
        protected internal virtual void SetUnitsPerEm(int unitsPerEm) {
            this.unitsPerEm = unitsPerEm;
            normalizationCoef = (float)FontProgram.UNITS_NORMALIZATION / unitsPerEm;
        }

        /// <summary>Converts and stores a font bounding box in source units.</summary>
        /// <param name="llx">lower-left x coordinate in source units</param>
        /// <param name="lly">lower-left y coordinate in source units</param>
        /// <param name="urx">upper-right x coordinate in source units</param>
        /// <param name="ury">upper-right y coordinate in source units</param>
        protected internal virtual void UpdateBbox(float llx, float lly, float urx, float ury) {
            bbox[0] = (int)(llx * normalizationCoef);
            bbox[1] = (int)(lly * normalizationCoef);
            bbox[2] = (int)(urx * normalizationCoef);
            bbox[3] = (int)(ury * normalizationCoef);
        }

        /// <summary>Sets the glyph count declared by the font.</summary>
        /// <param name="numOfGlyphs">number of glyphs</param>
        protected internal virtual void SetNumberOfGlyphs(int numOfGlyphs) {
            this.numOfGlyphs = numOfGlyphs;
        }

        /// <summary>Stores the glyph width table.</summary>
        /// <param name="glyphWidths">width array indexed by glyph ID</param>
        protected internal virtual void SetGlyphWidths(int[] glyphWidths) {
            this.glyphWidths = glyphWidths;
        }

        /// <summary>Sets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to ascender.</summary>
        /// <remarks>
        /// Sets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to ascender.
        /// <para />
        /// Typo vertical metrics are the primary source for iText ascender/descender calculations.
        /// </remarks>
        /// <param name="typoAscender">typo ascender value in normalized 1000-units</param>
        protected internal virtual void SetTypoAscender(int typoAscender) {
            this.typoAscender = (int)(typoAscender * normalizationCoef);
        }

        /// <summary>Sets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to descender.</summary>
        /// <remarks>
        /// Sets typo (a.k.a. sTypo or OS/2) vertical metric corresponding to descender.
        /// <para />
        /// Typo vertical metrics are the primary source for iText ascender/descender calculations.
        /// </remarks>
        /// <param name="typoDescender">typo descender value in normalized 1000-units</param>
        protected internal virtual void SetTypoDescender(int typoDescender) {
            this.typoDescender = (int)(typoDescender * normalizationCoef);
        }

        /// <summary>Sets the capital letters height.</summary>
        /// <remarks>
        /// Sets the capital letters height.
        /// <para />
        /// This property defines the vertical coordinate of the top of flat capital letters,
        /// measured from the baseline.
        /// </remarks>
        /// <param name="capHeight">cap height in 1000-units</param>
        protected internal virtual void SetCapHeight(int capHeight) {
            this.capHeight = (int)(capHeight * normalizationCoef);
        }

        /// <summary>Sets the source font x-height.</summary>
        /// <param name="xHeight">
        /// OS/2
        /// <c>sxHeight</c>
        /// in source units
        /// </param>
        protected internal virtual void SetXHeight(int xHeight) {
            this.xHeight = (int)(xHeight * normalizationCoef);
        }

        /// <summary>Sets the PostScript italic angle without unit normalization.</summary>
        /// <param name="italicAngle">counterclockwise degrees from vertical</param>
        protected internal virtual void SetItalicAngle(float italicAngle) {
            this.italicAngle = italicAngle;
        }

        /// <summary>Sets the source horizontal ascender.</summary>
        /// <param name="ascender">
        /// 
        /// <c>hhea.Ascender</c>
        /// in source units
        /// </param>
        protected internal virtual void SetAscender(int ascender) {
            this.ascender = (int)(ascender * normalizationCoef);
        }

        /// <summary>Sets the source horizontal descender.</summary>
        /// <param name="descender">
        /// 
        /// <c>hhea.Descender</c>
        /// in source units
        /// </param>
        protected internal virtual void SetDescender(int descender) {
            this.descender = (int)(descender * normalizationCoef);
        }

        /// <summary>Sets the line space.</summary>
        /// <param name="lineGap">
        /// 
        /// <c>hhea.LineGap</c>
        /// in source units
        /// </param>
        protected internal virtual void SetLineGap(int lineGap) {
            this.lineGap = (int)(lineGap * normalizationCoef);
        }

        /// <summary>Sets the source Windows ascender metric.</summary>
        /// <param name="winAscender">
        /// OS/2
        /// <c>usWinAscent</c>
        /// in source units
        /// </param>
        protected internal virtual void SetWinAscender(int winAscender) {
            this.winAscender = (int)(winAscender * normalizationCoef);
        }

        /// <summary>Sets the source Windows descender metric.</summary>
        /// <param name="winDescender">
        /// OS/2
        /// <c>usWinDescent</c>
        /// in source units
        /// </param>
        protected internal virtual void SetWinDescender(int winDescender) {
            this.winDescender = (int)(winDescender * normalizationCoef);
        }

        /// <summary>Sets the largest horizontal advance width declared by the source font.</summary>
        /// <param name="advanceWidthMax">
        /// 
        /// <c>hhea.advanceWidthMax</c>
        /// in source units
        /// </param>
        protected internal virtual void SetAdvanceWidthMax(int advanceWidthMax) {
            this.advanceWidthMax = (int)(advanceWidthMax * normalizationCoef);
        }

        /// <summary>Sets the source underline position.</summary>
        /// <param name="underlinePosition">
        /// 
        /// <c>post.underlinePosition</c>
        /// in source units
        /// </param>
        protected internal virtual void SetUnderlinePosition(int underlinePosition) {
            this.underlinePosition = (int)(underlinePosition * normalizationCoef);
        }

        /// <summary>Sets the underline thickness.</summary>
        /// <param name="underlineThickness">
        /// 
        /// <c>post.underlineThickness</c>
        /// ; stored without normalization
        /// </param>
        protected internal virtual void SetUnderlineThickness(int underlineThickness) {
            this.underlineThickness = underlineThickness;
        }

        /// <summary>Sets the source strikeout position.</summary>
        /// <param name="strikeoutPosition">
        /// OS/2
        /// <c>yStrikeoutPosition</c>
        /// in source units
        /// </param>
        protected internal virtual void SetStrikeoutPosition(int strikeoutPosition) {
            this.strikeoutPosition = (int)(strikeoutPosition * normalizationCoef);
        }

        /// <summary>Sets the source strikeout thickness.</summary>
        /// <param name="strikeoutSize">
        /// OS/2
        /// <c>yStrikeoutSize</c>
        /// in source units
        /// </param>
        protected internal virtual void SetStrikeoutSize(int strikeoutSize) {
            this.strikeoutSize = (int)(strikeoutSize * normalizationCoef);
        }

        /// <summary>Sets the source subscript vertical size.</summary>
        /// <param name="subscriptSize">
        /// OS/2
        /// <c>ySubscriptYSize</c>
        /// in source units
        /// </param>
        protected internal virtual void SetSubscriptSize(int subscriptSize) {
            this.subscriptSize = (int)(subscriptSize * normalizationCoef);
        }

        /// <summary>Sets the source subscript vertical offset.</summary>
        /// <param name="subscriptOffset">
        /// OS/2
        /// <c>ySubscriptYOffset</c>
        /// in source units
        /// </param>
        protected internal virtual void SetSubscriptOffset(int subscriptOffset) {
            this.subscriptOffset = (int)(subscriptOffset * normalizationCoef);
        }

        /// <summary>Sets the superscript vertical size.</summary>
        /// <param name="superscriptSize">
        /// OS/2
        /// <c>ySuperscriptYSize</c>
        /// ; stored without normalization
        /// </param>
        protected internal virtual void SetSuperscriptSize(int superscriptSize) {
            this.superscriptSize = superscriptSize;
        }

        /// <summary>Sets the source superscript vertical offset.</summary>
        /// <param name="superscriptOffset">
        /// OS/2
        /// <c>ySuperscriptYOffset</c>
        /// in source units
        /// </param>
        protected internal virtual void SetSuperscriptOffset(int superscriptOffset) {
            this.superscriptOffset = (int)(superscriptOffset * normalizationCoef);
        }

        /// <summary>Sets the dominant vertical stem width.</summary>
        /// <param name="stemV">
        /// Type 1/CFF
        /// <c>StdVW</c>
        /// value in glyph units
        /// </param>
        public virtual void SetStemV(int stemV) {
            this.stemV = stemV;
        }

        /// <summary>Sets the dominant horizontal stem width.</summary>
        /// <param name="stemH">
        /// Type 1/CFF
        /// <c>StdHW</c>
        /// value in glyph units
        /// </param>
        protected internal virtual void SetStemH(int stemH) {
            this.stemH = stemH;
        }

        /// <summary>Sets whether the font declares a common width for all glyphs.</summary>
        /// <param name="isFixedPitch">
        /// 
        /// <see langword="true"/>
        /// for fixed pitch fonts
        /// </param>
        protected internal virtual void SetIsFixedPitch(bool isFixedPitch) {
            this.isFixedPitch = isFixedPitch;
        }
    }
}
