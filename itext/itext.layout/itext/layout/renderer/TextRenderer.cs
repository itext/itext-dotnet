/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Hyphenation;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Splitting;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Text"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Text"/>
    /// object. It will draw the glyphs of the textual content on the
    /// <see cref="DrawContext"/>.
    /// </remarks>
    public class TextRenderer : AbstractRenderer, ILeafElementRenderer {
        protected internal const float TEXT_SPACE_COEFF = FontProgram.UNITS_NORMALIZATION;

        internal const float TYPO_ASCENDER_SCALE_COEFF = 1.2f;

        private const float ITALIC_ANGLE = 0.21256f;

        private const float BOLD_SIMULATION_STROKE_COEFF = 1 / 30f;

        protected internal float yLineOffset;

        // font should be stored only during converting original string to GlyphLine, however now it's not true
        private PdfFont font;

        protected internal GlyphLine text;

        protected internal GlyphLine line;

        protected internal String strToBeConverted;

        protected internal bool otfFeaturesApplied = false;

        protected internal float tabAnchorCharacterPosition = -1;

        protected internal IList<int[]> reversedRanges;

        protected internal GlyphLine savedWordBreakAtLineEnding;

        /// <summary>Creates a TextRenderer from its corresponding layout object.</summary>
        /// <param name="textElement">
        /// the
        /// <see cref="iText.Layout.Element.Text"/>
        /// which this object should manage
        /// </param>
        public TextRenderer(Text textElement)
            : this(textElement, textElement.GetText()) {
        }

        /// <summary>
        /// Creates a TextRenderer from its corresponding layout object, with a custom
        /// text to replace the contents of the
        /// <see cref="iText.Layout.Element.Text"/>.
        /// </summary>
        /// <param name="textElement">
        /// the
        /// <see cref="iText.Layout.Element.Text"/>
        /// which this object should manage
        /// </param>
        /// <param name="text">the replacement text</param>
        public TextRenderer(Text textElement, String text)
            : base(textElement) {
            this.strToBeConverted = text;
        }

        protected internal TextRenderer(iText.Layout.Renderer.TextRenderer other)
            : base(other) {
            this.text = other.text;
            this.line = other.line;
            this.font = other.font;
            this.yLineOffset = other.yLineOffset;
            this.strToBeConverted = other.strToBeConverted;
            this.otfFeaturesApplied = other.otfFeaturesApplied;
            this.tabAnchorCharacterPosition = other.tabAnchorCharacterPosition;
            this.reversedRanges = other.reversedRanges;
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            UpdateFontAndText();
            if (null != text) {
                // if text != null => font != null
                text = ReplaceSpecialWhitespaceGlyphs(text, font);
            }
            LayoutArea area = layoutContext.GetArea();
            Rectangle layoutBox = area.GetBBox().Clone();
            bool noSoftWrap = true.Equals(this.parent.GetOwnProperty<bool?>(Property.NO_SOFT_WRAP_INLINE));
            OverflowPropertyValue? overflowX = this.parent.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            FloatPropertyValue? floatPropertyValue = this.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                FloatingHelper.AdjustFloatedBlockLayoutBox(this, layoutBox, null, floatRendererAreas, floatPropertyValue, 
                    overflowX);
            }
            UnitValue[] margins = GetMargins();
            ApplyMargins(layoutBox, margins, false);
            Border[] borders = GetBorders();
            ApplyBorderBox(layoutBox, borders, false);
            UnitValue[] paddings = GetPaddings();
            ApplyPaddings(layoutBox, paddings, false);
            MinMaxWidth countedMinMaxWidth = new MinMaxWidth(area.GetBBox().GetWidth() - layoutBox.GetWidth());
            AbstractWidthHandler widthHandler;
            if (noSoftWrap) {
                widthHandler = new SumSumWidthHandler(countedMinMaxWidth);
            }
            else {
                widthHandler = new MaxSumWidthHandler(countedMinMaxWidth);
            }
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), 0, 0));
            bool anythingPlaced = false;
            int currentTextPos = text.start;
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Property
                    .FONT_SIZE));
            }
            float textRise = (float)this.GetPropertyAsFloat(Property.TEXT_RISE);
            float? characterSpacing = this.GetPropertyAsFloat(Property.CHARACTER_SPACING);
            float? wordSpacing = this.GetPropertyAsFloat(Property.WORD_SPACING);
            float hScale = (float)this.GetProperty(Property.HORIZONTAL_SCALING, (float?)1f);
            ISplitCharacters splitCharacters = this.GetProperty<ISplitCharacters>(Property.SPLIT_CHARACTERS);
            float italicSkewAddition = true.Equals(GetPropertyAsBoolean(Property.ITALIC_SIMULATION)) ? ITALIC_ANGLE * 
                fontSize.GetValue() : 0;
            float boldSimulationAddition = true.Equals(GetPropertyAsBoolean(Property.BOLD_SIMULATION)) ? BOLD_SIMULATION_STROKE_COEFF
                 * fontSize.GetValue() : 0;
            line = new GlyphLine(text);
            line.start = line.end = -1;
            float ascender = 0;
            float descender = 0;
            float currentLineAscender = 0;
            float currentLineDescender = 0;
            float currentLineHeight = 0;
            int initialLineTextPos = currentTextPos;
            float currentLineWidth = 0;
            int previousCharPos = -1;
            RenderingMode? mode = this.GetProperty<RenderingMode?>(Property.RENDERING_MODE);
            float[] ascenderDescender = CalculateAscenderDescender(font, mode);
            ascender = ascenderDescender[0];
            descender = ascenderDescender[1];
            if (RenderingMode.HTML_MODE.Equals(mode)) {
                currentLineAscender = ascenderDescender[0];
                currentLineDescender = ascenderDescender[1];
                currentLineHeight = (currentLineAscender - currentLineDescender) * fontSize.GetValue() / TEXT_SPACE_COEFF 
                    + textRise;
            }
            savedWordBreakAtLineEnding = null;
            Glyph wordBreakGlyphAtLineEnding = null;
            char? tabAnchorCharacter = this.GetProperty<char?>(Property.TAB_ANCHOR);
            TextLayoutResult result = null;
            OverflowPropertyValue? overflowY = !layoutContext.IsClippedHeight() ? OverflowPropertyValue.FIT : this.parent
                .GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_Y);
            // true in situations like "\nHello World" or "Hello\nWorld"
            bool isSplitForcedByNewLine = false;
            // needed in situation like "\nHello World" or " Hello World", when split occurs on first character, but we want to leave it on previous line
            bool forcePartialSplitOnFirstChar = false;
            // true in situations like "Hello\nWorld"
            bool ignoreNewLineSymbol = false;
            // true when \r\n are found
            bool crlf = false;
            HyphenationConfig hyphenationConfig = this.GetProperty<HyphenationConfig>(Property.HYPHENATION);
            // For example, if a first character is a RTL mark (U+200F), and the second is a newline, we need to break anyway
            int firstPrintPos = currentTextPos;
            while (firstPrintPos < text.end && NoPrint(text.Get(firstPrintPos))) {
                firstPrintPos++;
            }
            while (currentTextPos < text.end) {
                if (NoPrint(text.Get(currentTextPos))) {
                    if (line.start == -1) {
                        line.start = currentTextPos;
                    }
                    line.end = Math.Max(line.end, currentTextPos + 1);
                    currentTextPos++;
                    continue;
                }
                int nonBreakablePartEnd = text.end - 1;
                float nonBreakablePartFullWidth = 0;
                float nonBreakablePartWidthWhichDoesNotExceedAllowedWidth = 0;
                float nonBreakablePartMaxAscender = 0;
                float nonBreakablePartMaxDescender = 0;
                float nonBreakablePartMaxHeight = 0;
                int firstCharacterWhichExceedsAllowedWidth = -1;
                float nonBreakingHyphenRelatedChunkWidth = 0;
                int nonBreakingHyphenRelatedChunkStart = -1;
                float beforeNonBreakingHyphenRelatedChunkMaxAscender = 0;
                float beforeNonBreakingHyphenRelatedChunkMaxDescender = 0;
                for (int ind = currentTextPos; ind < text.end; ind++) {
                    if (iText.IO.Util.TextUtil.IsNewLine(text.Get(ind))) {
                        wordBreakGlyphAtLineEnding = text.Get(ind);
                        isSplitForcedByNewLine = true;
                        firstCharacterWhichExceedsAllowedWidth = ind + 1;
                        if (ind != firstPrintPos) {
                            ignoreNewLineSymbol = true;
                        }
                        else {
                            // Notice that in that case we do not need to ignore the new line symbol ('\n')
                            forcePartialSplitOnFirstChar = true;
                        }
                        if (line.start == -1) {
                            line.start = currentTextPos;
                        }
                        crlf = iText.IO.Util.TextUtil.IsCarriageReturnFollowedByLineFeed(text, currentTextPos);
                        if (crlf) {
                            currentTextPos++;
                        }
                        line.end = Math.Max(line.end, firstCharacterWhichExceedsAllowedWidth - 1);
                        break;
                    }
                    Glyph currentGlyph = text.Get(ind);
                    if (NoPrint(currentGlyph)) {
                        if (ind + 1 == text.end || splitCharacters.IsSplitCharacter(text, ind + 1) && iText.IO.Util.TextUtil.IsSpaceOrWhitespace
                            (text.Get(ind + 1))) {
                            nonBreakablePartEnd = ind;
                            break;
                        }
                        continue;
                    }
                    if (tabAnchorCharacter != null && tabAnchorCharacter == text.Get(ind).GetUnicode()) {
                        tabAnchorCharacterPosition = currentLineWidth + nonBreakablePartFullWidth;
                        tabAnchorCharacter = null;
                    }
                    float glyphWidth = GetCharWidth(currentGlyph, fontSize.GetValue(), hScale, characterSpacing, wordSpacing) 
                        / TEXT_SPACE_COEFF;
                    float xAdvance = previousCharPos != -1 ? text.Get(previousCharPos).GetXAdvance() : 0;
                    if (xAdvance != 0) {
                        xAdvance = ScaleXAdvance(xAdvance, fontSize.GetValue(), hScale) / TEXT_SPACE_COEFF;
                    }
                    if (!noSoftWrap && (nonBreakablePartFullWidth + glyphWidth + xAdvance + italicSkewAddition + boldSimulationAddition
                        ) > layoutBox.GetWidth() - currentLineWidth && firstCharacterWhichExceedsAllowedWidth == -1) {
                        firstCharacterWhichExceedsAllowedWidth = ind;
                        if (iText.IO.Util.TextUtil.IsSpaceOrWhitespace(text.Get(ind))) {
                            wordBreakGlyphAtLineEnding = currentGlyph;
                            if (ind == firstPrintPos) {
                                forcePartialSplitOnFirstChar = true;
                                firstCharacterWhichExceedsAllowedWidth = ind + 1;
                                break;
                            }
                        }
                    }
                    if (null != hyphenationConfig) {
                        if (GlyphBelongsToNonBreakingHyphenRelatedChunk(text, ind)) {
                            if (-1 == nonBreakingHyphenRelatedChunkStart) {
                                beforeNonBreakingHyphenRelatedChunkMaxAscender = nonBreakablePartMaxAscender;
                                beforeNonBreakingHyphenRelatedChunkMaxDescender = nonBreakablePartMaxDescender;
                                nonBreakingHyphenRelatedChunkStart = ind;
                            }
                            nonBreakingHyphenRelatedChunkWidth += glyphWidth + xAdvance;
                        }
                        else {
                            nonBreakingHyphenRelatedChunkStart = -1;
                            nonBreakingHyphenRelatedChunkWidth = 0;
                        }
                    }
                    if (firstCharacterWhichExceedsAllowedWidth == -1) {
                        nonBreakablePartWidthWhichDoesNotExceedAllowedWidth += glyphWidth + xAdvance;
                    }
                    nonBreakablePartFullWidth += glyphWidth + xAdvance;
                    nonBreakablePartMaxAscender = Math.Max(nonBreakablePartMaxAscender, ascender);
                    nonBreakablePartMaxDescender = Math.Min(nonBreakablePartMaxDescender, descender);
                    nonBreakablePartMaxHeight = (nonBreakablePartMaxAscender - nonBreakablePartMaxDescender) * fontSize.GetValue
                        () / TEXT_SPACE_COEFF + textRise;
                    previousCharPos = ind;
                    if (!noSoftWrap && nonBreakablePartFullWidth + italicSkewAddition + boldSimulationAddition > layoutBox.GetWidth
                        () && (0 == nonBreakingHyphenRelatedChunkWidth || ind + 1 == text.end || !GlyphBelongsToNonBreakingHyphenRelatedChunk
                        (text, ind + 1))) {
                        if (IsOverflowFit(overflowX)) {
                            // we have extracted all the information we wanted and we do not want to continue.
                            // we will have to split the word anyway.
                            break;
                        }
                    }
                    if (splitCharacters.IsSplitCharacter(text, ind) || ind + 1 == text.end || splitCharacters.IsSplitCharacter
                        (text, ind + 1) && iText.IO.Util.TextUtil.IsSpaceOrWhitespace(text.Get(ind + 1))) {
                        nonBreakablePartEnd = ind;
                        break;
                    }
                }
                if (firstCharacterWhichExceedsAllowedWidth == -1) {
                    // can fit the whole word in a line
                    if (line.start == -1) {
                        line.start = currentTextPos;
                    }
                    line.end = Math.Max(line.end, nonBreakablePartEnd + 1);
                    currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
                    currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender);
                    currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
                    currentTextPos = nonBreakablePartEnd + 1;
                    currentLineWidth += nonBreakablePartFullWidth;
                    widthHandler.UpdateMinChildWidth(nonBreakablePartWidthWhichDoesNotExceedAllowedWidth + italicSkewAddition 
                        + boldSimulationAddition);
                    widthHandler.UpdateMaxChildWidth(nonBreakablePartWidthWhichDoesNotExceedAllowedWidth + italicSkewAddition 
                        + boldSimulationAddition);
                    anythingPlaced = true;
                }
                else {
                    // check if line height exceeds the allowed height
                    if (Math.Max(currentLineHeight, nonBreakablePartMaxHeight) > layoutBox.GetHeight() && IsOverflowFit(overflowY
                        )) {
                        ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                        ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                        ApplyMargins(occupiedArea.GetBBox(), margins, true);
                        // Force to place what we can
                        if (line.start == -1) {
                            line.start = currentTextPos;
                        }
                        line.end = Math.Max(line.end, firstCharacterWhichExceedsAllowedWidth - 1);
                        // the line does not fit because of height - full overflow
                        iText.Layout.Renderer.TextRenderer[] splitResult = Split(initialLineTextPos);
                        return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, splitResult[0], splitResult[1], this);
                    }
                    else {
                        // cannot fit a word as a whole
                        bool wordSplit = false;
                        bool hyphenationApplied = false;
                        if (hyphenationConfig != null) {
                            if (-1 == nonBreakingHyphenRelatedChunkStart) {
                                int[] wordBounds = GetWordBoundsForHyphenation(text, currentTextPos, text.end, Math.Max(currentTextPos, firstCharacterWhichExceedsAllowedWidth
                                     - 1));
                                if (wordBounds != null) {
                                    String word = text.ToUnicodeString(wordBounds[0], wordBounds[1]);
                                    iText.Layout.Hyphenation.Hyphenation hyph = hyphenationConfig.Hyphenate(word);
                                    if (hyph != null) {
                                        for (int i = hyph.Length() - 1; i >= 0; i--) {
                                            String pre = hyph.GetPreHyphenText(i);
                                            String pos = hyph.GetPostHyphenText(i);
                                            float currentHyphenationChoicePreTextWidth = GetGlyphLineWidth(ConvertToGlyphLine(text.ToUnicodeString(currentTextPos
                                                , wordBounds[0]) + pre + hyphenationConfig.GetHyphenSymbol()), fontSize.GetValue(), hScale, characterSpacing
                                                , wordSpacing);
                                            if (currentLineWidth + currentHyphenationChoicePreTextWidth + italicSkewAddition + boldSimulationAddition 
                                                <= layoutBox.GetWidth()) {
                                                hyphenationApplied = true;
                                                if (line.start == -1) {
                                                    line.start = currentTextPos;
                                                }
                                                line.end = Math.Max(line.end, wordBounds[0] + pre.Length);
                                                GlyphLine lineCopy = line.Copy(line.start, line.end);
                                                lineCopy.Add(font.GetGlyph(hyphenationConfig.GetHyphenSymbol()));
                                                lineCopy.end++;
                                                line = lineCopy;
                                                // TODO these values are based on whole word. recalculate properly based on hyphenated part
                                                currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
                                                currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender);
                                                currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
                                                currentLineWidth += currentHyphenationChoicePreTextWidth;
                                                widthHandler.UpdateMinChildWidth(currentHyphenationChoicePreTextWidth + italicSkewAddition + boldSimulationAddition
                                                    );
                                                widthHandler.UpdateMaxChildWidth(currentHyphenationChoicePreTextWidth + italicSkewAddition + boldSimulationAddition
                                                    );
                                                currentTextPos = wordBounds[0] + pre.Length;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                if (text.start == nonBreakingHyphenRelatedChunkStart) {
                                    nonBreakingHyphenRelatedChunkWidth = 0;
                                    firstCharacterWhichExceedsAllowedWidth = previousCharPos + 1;
                                }
                                else {
                                    firstCharacterWhichExceedsAllowedWidth = nonBreakingHyphenRelatedChunkStart;
                                    nonBreakablePartFullWidth -= nonBreakingHyphenRelatedChunkWidth;
                                    nonBreakablePartMaxAscender = beforeNonBreakingHyphenRelatedChunkMaxAscender;
                                    nonBreakablePartMaxDescender = beforeNonBreakingHyphenRelatedChunkMaxDescender;
                                }
                            }
                        }
                        if ((nonBreakablePartFullWidth > layoutBox.GetWidth() && !anythingPlaced && !hyphenationApplied) || forcePartialSplitOnFirstChar
                             || -1 != nonBreakingHyphenRelatedChunkStart) {
                            // if the word is too long for a single line we will have to split it
                            if (line.start == -1) {
                                line.start = currentTextPos;
                            }
                            if (!crlf) {
                                currentTextPos = (forcePartialSplitOnFirstChar || IsOverflowFit(overflowX)) ? firstCharacterWhichExceedsAllowedWidth
                                     : nonBreakablePartEnd + 1;
                            }
                            line.end = Math.Max(line.end, currentTextPos);
                            wordSplit = !forcePartialSplitOnFirstChar && (text.end != currentTextPos);
                            if (wordSplit || !(forcePartialSplitOnFirstChar || IsOverflowFit(overflowX))) {
                                currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
                                currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender);
                                currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
                                currentLineWidth += nonBreakablePartWidthWhichDoesNotExceedAllowedWidth;
                                widthHandler.UpdateMinChildWidth(nonBreakablePartWidthWhichDoesNotExceedAllowedWidth + italicSkewAddition 
                                    + boldSimulationAddition);
                                widthHandler.UpdateMaxChildWidth(nonBreakablePartWidthWhichDoesNotExceedAllowedWidth + italicSkewAddition 
                                    + boldSimulationAddition);
                            }
                            else {
                                // process empty line (e.g. '\n')
                                currentLineAscender = ascender;
                                currentLineDescender = descender;
                                currentLineHeight = (currentLineAscender - currentLineDescender) * fontSize.GetValue() / TEXT_SPACE_COEFF 
                                    + textRise;
                                currentLineWidth += GetCharWidth(line.Get(line.start), fontSize.GetValue(), hScale, characterSpacing, wordSpacing
                                    ) / TEXT_SPACE_COEFF;
                            }
                        }
                        if (line.end <= line.start) {
                            return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this);
                        }
                        else {
                            result = new TextLayoutResult(LayoutResult.PARTIAL, occupiedArea, null, null).SetWordHasBeenSplit(wordSplit
                                );
                        }
                        break;
                    }
                }
            }
            // indicates whether the placing is forced while the layout result is LayoutResult.NOTHING
            bool isPlacingForcedWhileNothing = false;
            if (currentLineHeight > layoutBox.GetHeight()) {
                if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) && IsOverflowFit(overflowY)) {
                    ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                    ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                    ApplyMargins(occupiedArea.GetBBox(), margins, true);
                    return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this);
                }
                else {
                    isPlacingForcedWhileNothing = true;
                }
            }
            yLineOffset = currentLineAscender * fontSize.GetValue() / TEXT_SPACE_COEFF;
            occupiedArea.GetBBox().MoveDown(currentLineHeight);
            occupiedArea.GetBBox().SetHeight(occupiedArea.GetBBox().GetHeight() + currentLineHeight);
            occupiedArea.GetBBox().SetWidth(Math.Max(occupiedArea.GetBBox().GetWidth(), currentLineWidth));
            layoutBox.SetHeight(area.GetBBox().GetHeight() - currentLineHeight);
            occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() + italicSkewAddition + boldSimulationAddition
                );
            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            ApplyMargins(occupiedArea.GetBBox(), margins, true);
            IncreaseYLineOffset(paddings, borders, margins);
            if (result == null) {
                result = new TextLayoutResult(LayoutResult.FULL, occupiedArea, null, null, isPlacingForcedWhileNothing ? this
                     : null);
            }
            else {
                iText.Layout.Renderer.TextRenderer[] split;
                if (ignoreNewLineSymbol || crlf) {
                    // ignore '\n'
                    split = SplitIgnoreFirstNewLine(currentTextPos);
                }
                else {
                    split = Split(currentTextPos);
                }
                result.SetSplitForcedByNewline(isSplitForcedByNewLine);
                result.SetSplitRenderer(split[0]);
                if (wordBreakGlyphAtLineEnding != null) {
                    split[0].SaveWordBreakIfNotYetSaved(wordBreakGlyphAtLineEnding);
                }
                // no sense to process empty renderer
                if (split[1].text.start != split[1].text.end) {
                    result.SetOverflowRenderer(split[1]);
                }
                else {
                    // LayoutResult with partial status should have non-null overflow renderer
                    result.SetStatus(LayoutResult.FULL);
                }
            }
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                if (result.GetStatus() == LayoutResult.FULL) {
                    if (occupiedArea.GetBBox().GetWidth() > 0) {
                        floatRendererAreas.Add(occupiedArea.GetBBox());
                    }
                }
                else {
                    if (result.GetStatus() == LayoutResult.PARTIAL) {
                        floatRendererAreas.Add(result.GetSplitRenderer().GetOccupiedArea().GetBBox());
                    }
                }
            }
            result.SetMinMaxWidth(countedMinMaxWidth);
            return result;
        }

        private void IncreaseYLineOffset(UnitValue[] paddings, Border[] borders, UnitValue[] margins) {
            yLineOffset += paddings[0] != null ? paddings[0].GetValue() : 0;
            yLineOffset += borders[0] != null ? borders[0].GetWidth() : 0;
            yLineOffset += margins[0] != null ? margins[0].GetValue() : 0;
        }

        public virtual void ApplyOtf() {
            UpdateFontAndText();
            UnicodeScript? script = this.GetProperty<UnicodeScript?>(Property.FONT_SCRIPT);
            if (!otfFeaturesApplied && TypographyUtils.IsPdfCalligraphAvailable() && text.start < text.end) {
                if (HasOtfFont()) {
                    Object typographyConfig = this.GetProperty<Object>(Property.TYPOGRAPHY_CONFIG);
                    ICollection<UnicodeScript> supportedScripts = null;
                    if (typographyConfig != null) {
                        supportedScripts = TypographyUtils.GetSupportedScripts(typographyConfig);
                    }
                    if (supportedScripts == null) {
                        supportedScripts = TypographyUtils.GetSupportedScripts();
                    }
                    IList<TextRenderer.ScriptRange> scriptsRanges = new List<TextRenderer.ScriptRange>();
                    if (script != null) {
                        scriptsRanges.Add(new TextRenderer.ScriptRange(script, text.end));
                    }
                    else {
                        // Try to autodetect script.
                        TextRenderer.ScriptRange currRange = new TextRenderer.ScriptRange(null, text.end);
                        scriptsRanges.Add(currRange);
                        for (int i = text.start; i < text.end; i++) {
                            int unicode = text.Get(i).GetUnicode();
                            if (unicode > -1) {
                                UnicodeScript glyphScript = UnicodeScriptUtil.Of(unicode);
                                if (UnicodeScript.COMMON.Equals(glyphScript) || UnicodeScript.UNKNOWN.Equals(glyphScript) || UnicodeScript
                                    .INHERITED.Equals(glyphScript)) {
                                    continue;
                                }
                                if (glyphScript != currRange.script) {
                                    if (currRange.script == null) {
                                        currRange.script = glyphScript;
                                    }
                                    else {
                                        currRange.rangeEnd = i;
                                        currRange = new TextRenderer.ScriptRange(glyphScript, text.end);
                                        scriptsRanges.Add(currRange);
                                    }
                                }
                            }
                        }
                    }
                    int delta = 0;
                    int origTextStart = text.start;
                    int origTextEnd = text.end;
                    int shapingRangeStart = text.start;
                    foreach (TextRenderer.ScriptRange scriptsRange in scriptsRanges) {
                        if (scriptsRange.script == null || !supportedScripts.Contains(EnumUtil.ThrowIfNull(scriptsRange.script))) {
                            continue;
                        }
                        scriptsRange.rangeEnd += delta;
                        text.start = shapingRangeStart;
                        text.end = scriptsRange.rangeEnd;
                        if ((scriptsRange.script == UnicodeScript.ARABIC || scriptsRange.script == UnicodeScript.HEBREW) && parent
                             is LineRenderer) {
                            // It's safe to set here BASE_DIRECTION to TextRenderer without additional checks, because
                            // by convention this property makes sense only if it's applied to LineRenderer or it's
                            // parents (Paragraph or above).
                            // Only if it's not found there first, LineRenderer tries to fetch autodetected BaseDirection
                            // from text renderers (see LineRenderer#applyOtf).
                            SetProperty(Property.BASE_DIRECTION, BaseDirection.DEFAULT_BIDI);
                        }
                        TypographyUtils.ApplyOtfScript(font.GetFontProgram(), text, scriptsRange.script, typographyConfig);
                        delta += text.end - scriptsRange.rangeEnd;
                        scriptsRange.rangeEnd = shapingRangeStart = text.end;
                    }
                    text.start = origTextStart;
                    text.end = origTextEnd + delta;
                }
                FontKerning fontKerning = (FontKerning)this.GetProperty<FontKerning?>(Property.FONT_KERNING, FontKerning.NO
                    );
                if (fontKerning == FontKerning.YES) {
                    TypographyUtils.ApplyKerning(font.GetFontProgram(), text);
                }
                otfFeaturesApplied = true;
            }
        }

        public override void Draw(DrawContext drawContext) {
            if (occupiedArea == null) {
                ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, 
                    "Drawing won't be performed."));
                return;
            }
            // Set up marked content before super.draw so that annotations are placed within marked content
            bool isTagged = drawContext.IsTaggingEnabled();
            LayoutTaggingHelper taggingHelper = null;
            bool isArtifact = false;
            TagTreePointer tagPointer = null;
            if (isTagged) {
                taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper == null) {
                    isArtifact = true;
                }
                else {
                    isArtifact = taggingHelper.IsArtifact(this);
                    if (!isArtifact) {
                        tagPointer = taggingHelper.UseAutoTaggingPointerAndRememberItsPosition(this);
                        if (taggingHelper.CreateTag(this, tagPointer)) {
                            tagPointer.GetProperties().AddAttributes(0, AccessibleAttributesApplier.GetLayoutAttributes(this, tagPointer
                                ));
                        }
                    }
                }
            }
            base.Draw(drawContext);
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            float leftBBoxX = GetInnerAreaBBox().GetX();
            if (line.end > line.start || savedWordBreakAtLineEnding != null) {
                UnitValue fontSize = this.GetPropertyAsUnitValue(Property.FONT_SIZE);
                if (!fontSize.IsPointValue()) {
                    ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                    logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Property
                        .FONT_SIZE));
                }
                TransparentColor fontColor = GetPropertyAsTransparentColor(Property.FONT_COLOR);
                int? textRenderingMode = this.GetProperty<int?>(Property.TEXT_RENDERING_MODE);
                float? textRise = this.GetPropertyAsFloat(Property.TEXT_RISE);
                float? characterSpacing = this.GetPropertyAsFloat(Property.CHARACTER_SPACING);
                float? wordSpacing = this.GetPropertyAsFloat(Property.WORD_SPACING);
                float? horizontalScaling = this.GetProperty<float?>(Property.HORIZONTAL_SCALING);
                float[] skew = this.GetProperty<float[]>(Property.SKEW);
                bool italicSimulation = true.Equals(GetPropertyAsBoolean(Property.ITALIC_SIMULATION));
                bool boldSimulation = true.Equals(GetPropertyAsBoolean(Property.BOLD_SIMULATION));
                float? strokeWidth = null;
                if (boldSimulation) {
                    textRenderingMode = PdfCanvasConstants.TextRenderingMode.FILL_STROKE;
                    strokeWidth = fontSize.GetValue() / 30;
                }
                PdfCanvas canvas = drawContext.GetCanvas();
                if (isTagged) {
                    if (isArtifact) {
                        canvas.OpenTag(new CanvasArtifact());
                    }
                    else {
                        canvas.OpenTag(tagPointer.GetTagReference());
                    }
                }
                BeginElementOpacityApplying(drawContext);
                canvas.SaveState().BeginText().SetFontAndSize(font, fontSize.GetValue());
                if (skew != null && skew.Length == 2) {
                    canvas.SetTextMatrix(1, skew[0], skew[1], 1, leftBBoxX, GetYLine());
                }
                else {
                    if (italicSimulation) {
                        canvas.SetTextMatrix(1, 0, ITALIC_ANGLE, 1, leftBBoxX, GetYLine());
                    }
                    else {
                        canvas.MoveText(leftBBoxX, GetYLine());
                    }
                }
                if (textRenderingMode != PdfCanvasConstants.TextRenderingMode.FILL) {
                    canvas.SetTextRenderingMode((int)textRenderingMode);
                }
                if (textRenderingMode == PdfCanvasConstants.TextRenderingMode.STROKE || textRenderingMode == PdfCanvasConstants.TextRenderingMode
                    .FILL_STROKE) {
                    if (strokeWidth == null) {
                        strokeWidth = this.GetPropertyAsFloat(Property.STROKE_WIDTH);
                    }
                    if (strokeWidth != null && strokeWidth != 1f) {
                        canvas.SetLineWidth((float)strokeWidth);
                    }
                    Color strokeColor = GetPropertyAsColor(Property.STROKE_COLOR);
                    if (strokeColor == null && fontColor != null) {
                        strokeColor = fontColor.GetColor();
                    }
                    if (strokeColor != null) {
                        canvas.SetStrokeColor(strokeColor);
                    }
                }
                if (fontColor != null) {
                    canvas.SetFillColor(fontColor.GetColor());
                    fontColor.ApplyFillTransparency(canvas);
                }
                if (textRise != null && textRise != 0) {
                    canvas.SetTextRise((float)textRise);
                }
                if (characterSpacing != null && characterSpacing != 0) {
                    canvas.SetCharacterSpacing((float)characterSpacing);
                }
                if (wordSpacing != null && wordSpacing != 0) {
                    if (font is PdfType0Font) {
                        // From the spec: Word spacing is applied to every occurrence of the single-byte character code 32 in
                        // a string when using a simple font or a composite font that defines code 32 as a single-byte code.
                        // It does not apply to occurrences of the byte value 32 in multiple-byte codes.
                        //
                        // For PdfType0Font we must add word manually with glyph offsets
                        for (int gInd = line.start; gInd < line.end; gInd++) {
                            if (iText.IO.Util.TextUtil.IsUni0020(line.Get(gInd))) {
                                short advance = (short)(iText.Layout.Renderer.TextRenderer.TEXT_SPACE_COEFF * (float)wordSpacing / fontSize
                                    .GetValue());
                                Glyph copy = new Glyph(line.Get(gInd));
                                copy.SetXAdvance(advance);
                                line.Set(gInd, copy);
                            }
                        }
                    }
                    else {
                        canvas.SetWordSpacing((float)wordSpacing);
                    }
                }
                if (horizontalScaling != null && horizontalScaling != 1) {
                    canvas.SetHorizontalScaling((float)horizontalScaling * 100);
                }
                GlyphLine.IGlyphLineFilter filter = new _IGlyphLineFilter_796();
                bool appearanceStreamLayout = true.Equals(GetPropertyAsBoolean(Property.APPEARANCE_STREAM_LAYOUT));
                if (GetReversedRanges() != null) {
                    bool writeReversedChars = !appearanceStreamLayout;
                    List<int> removedIds = new List<int>();
                    for (int i = line.start; i < line.end; i++) {
                        if (!filter.Accept(line.Get(i))) {
                            removedIds.Add(i);
                        }
                    }
                    foreach (int[] range in GetReversedRanges()) {
                        UpdateRangeBasedOnRemovedCharacters(removedIds, range);
                    }
                    line = line.Filter(filter);
                    if (writeReversedChars) {
                        canvas.ShowText(line, new TextRenderer.ReversedCharsIterator(reversedRanges, line).SetUseReversed(true));
                    }
                    else {
                        canvas.ShowText(line);
                    }
                }
                else {
                    if (appearanceStreamLayout) {
                        line.SetActualText(line.start, line.end, null);
                    }
                    canvas.ShowText(line.Filter(filter));
                }
                if (savedWordBreakAtLineEnding != null) {
                    canvas.ShowText(savedWordBreakAtLineEnding);
                }
                canvas.EndText().RestoreState();
                EndElementOpacityApplying(drawContext);
                Object underlines = this.GetProperty<Object>(Property.UNDERLINE);
                if (underlines is IList) {
                    foreach (Object underline in (IList)underlines) {
                        if (underline is Underline) {
                            DrawSingleUnderline((Underline)underline, fontColor, canvas, fontSize.GetValue(), italicSimulation ? ITALIC_ANGLE
                                 : 0);
                        }
                    }
                }
                else {
                    if (underlines is Underline) {
                        DrawSingleUnderline((Underline)underlines, fontColor, canvas, fontSize.GetValue(), italicSimulation ? ITALIC_ANGLE
                             : 0);
                    }
                }
                if (isTagged) {
                    canvas.CloseTag();
                }
            }
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            if (isTagged && !isArtifact) {
                if (isLastRendererForModelElement) {
                    taggingHelper.FinishTaggingHint(this);
                }
                taggingHelper.RestoreAutoTaggingPointerPosition(this);
            }
        }

        private sealed class _IGlyphLineFilter_796 : GlyphLine.IGlyphLineFilter {
            public _IGlyphLineFilter_796() {
            }

            public bool Accept(Glyph glyph) {
                return !iText.Layout.Renderer.TextRenderer.NoPrint(glyph);
            }
        }

        public override void DrawBackground(DrawContext drawContext) {
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            float? textRise = this.GetPropertyAsFloat(Property.TEXT_RISE);
            Rectangle bBox = GetOccupiedAreaBBox();
            Rectangle backgroundArea = ApplyMargins(bBox, false);
            float bottomBBoxY = backgroundArea.GetY();
            float leftBBoxX = backgroundArea.GetX();
            if (background != null) {
                bool isTagged = drawContext.IsTaggingEnabled();
                PdfCanvas canvas = drawContext.GetCanvas();
                if (isTagged) {
                    canvas.OpenTag(new CanvasArtifact());
                }
                bool backgroundAreaIsClipped = ClipBackgroundArea(drawContext, backgroundArea);
                canvas.SaveState().SetFillColor(background.GetColor());
                TransparentColor backgroundColor = new TransparentColor(background.GetColor(), background.GetOpacity());
                backgroundColor.ApplyFillTransparency(drawContext.GetCanvas());
                canvas.Rectangle(leftBBoxX - background.GetExtraLeft(), bottomBBoxY + (float)textRise - background.GetExtraBottom
                    (), backgroundArea.GetWidth() + background.GetExtraLeft() + background.GetExtraRight(), backgroundArea
                    .GetHeight() - (float)textRise + background.GetExtraTop() + background.GetExtraBottom());
                canvas.Fill().RestoreState();
                if (backgroundAreaIsClipped) {
                    drawContext.GetCanvas().RestoreState();
                }
                if (isTagged) {
                    canvas.CloseTag();
                }
            }
        }

        /// <summary>
        /// Trims any whitespace characters from the start of the
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// to be rendered.
        /// </summary>
        public virtual void TrimFirst() {
            UpdateFontAndText();
            if (text != null) {
                Glyph glyph;
                while (text.start < text.end && iText.IO.Util.TextUtil.IsWhitespace(glyph = text.Get(text.start)) && !iText.IO.Util.TextUtil
                    .IsNewLine(glyph)) {
                    text.start++;
                }
            }
        }

        internal virtual float TrimLast() {
            float trimmedSpace = 0;
            if (line.end <= 0) {
                return trimmedSpace;
            }
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Property
                    .FONT_SIZE));
            }
            float? characterSpacing = this.GetPropertyAsFloat(Property.CHARACTER_SPACING);
            float? wordSpacing = this.GetPropertyAsFloat(Property.WORD_SPACING);
            float hScale = (float)this.GetPropertyAsFloat(Property.HORIZONTAL_SCALING, 1f);
            int firstNonSpaceCharIndex = line.end - 1;
            while (firstNonSpaceCharIndex >= line.start) {
                Glyph currentGlyph = line.Get(firstNonSpaceCharIndex);
                if (!iText.IO.Util.TextUtil.IsWhitespace(currentGlyph)) {
                    break;
                }
                SaveWordBreakIfNotYetSaved(currentGlyph);
                float currentCharWidth = GetCharWidth(currentGlyph, fontSize.GetValue(), hScale, characterSpacing, wordSpacing
                    ) / TEXT_SPACE_COEFF;
                float xAdvance = firstNonSpaceCharIndex > line.start ? ScaleXAdvance(line.Get(firstNonSpaceCharIndex - 1).
                    GetXAdvance(), fontSize.GetValue(), hScale) / TEXT_SPACE_COEFF : 0;
                trimmedSpace += currentCharWidth - xAdvance;
                occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() - currentCharWidth);
                firstNonSpaceCharIndex--;
            }
            line.end = firstNonSpaceCharIndex + 1;
            return trimmedSpace;
        }

        /// <summary>Gets the maximum offset above the base line that this Text extends to.</summary>
        /// <returns>
        /// the upwards vertical offset of this
        /// <see cref="iText.Layout.Element.Text"/>
        /// </returns>
        public virtual float GetAscent() {
            return yLineOffset;
        }

        /// <summary>Gets the maximum offset below the base line that this Text extends to.</summary>
        /// <returns>
        /// the downwards vertical offset of this
        /// <see cref="iText.Layout.Element.Text"/>
        /// </returns>
        public virtual float GetDescent() {
            return -(GetOccupiedAreaBBox().GetHeight() - yLineOffset - (float)this.GetPropertyAsFloat(Property.TEXT_RISE
                ));
        }

        /// <summary>
        /// Gets the position on the canvas of the imaginary horizontal line upon which
        /// the
        /// <see cref="iText.Layout.Element.Text"/>
        /// 's contents will be written.
        /// </summary>
        /// <returns>
        /// the y position of this text on the
        /// <see cref="DrawContext"/>
        /// </returns>
        public virtual float GetYLine() {
            return occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight() - yLineOffset - (float)this.GetPropertyAsFloat
                (Property.TEXT_RISE);
        }

        /// <summary>Moves the vertical position to the parameter's value.</summary>
        /// <param name="y">the new vertical position of the Text</param>
        public virtual void MoveYLineTo(float y) {
            float curYLine = GetYLine();
            float delta = y - curYLine;
            occupiedArea.GetBBox().SetY(occupiedArea.GetBBox().GetY() + delta);
        }

        /// <summary>
        /// Manually sets the contents of the Text's representation on the canvas,
        /// regardless of the Text's own contents.
        /// </summary>
        /// <param name="text">the replacement text</param>
        public virtual void SetText(String text) {
            strToBeConverted = text;
            //strToBeConverted will be null after next method.
            UpdateFontAndText();
        }

        /// <summary>
        /// Manually sets a GlyphLine to be rendered with a specific start and end
        /// point.
        /// </summary>
        /// <param name="text">
        /// a
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// </param>
        /// <param name="leftPos">the leftmost end of the GlyphLine</param>
        /// <param name="rightPos">the rightmost end of the GlyphLine</param>
        public virtual void SetText(GlyphLine text, int leftPos, int rightPos) {
            this.strToBeConverted = null;
            this.text = new GlyphLine(text);
            this.text.start = leftPos;
            this.text.end = rightPos;
            this.otfFeaturesApplied = false;
        }

        public virtual GlyphLine GetText() {
            UpdateFontAndText();
            return text;
        }

        /// <summary>The length of the whole text assigned to this renderer.</summary>
        /// <returns>the text length</returns>
        public virtual int Length() {
            return text == null ? 0 : text.end - text.start;
        }

        public override String ToString() {
            return line != null ? line.ToString() : null;
        }

        /// <summary>Gets char code at given position for the text belonging to this renderer.</summary>
        /// <param name="pos">the position in range [0; length())</param>
        /// <returns>Unicode char code</returns>
        public virtual int CharAt(int pos) {
            return text.Get(pos + text.start).GetUnicode();
        }

        public virtual float GetTabAnchorCharacterPosition() {
            return tabAnchorCharacterPosition;
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.TextRenderer((Text)modelElement);
        }

        internal virtual IList<int[]> GetReversedRanges() {
            return reversedRanges;
        }

        internal virtual IList<int[]> InitReversedRanges() {
            if (reversedRanges == null) {
                reversedRanges = new List<int[]>();
            }
            return reversedRanges;
        }

        internal virtual iText.Layout.Renderer.TextRenderer RemoveReversedRanges() {
            reversedRanges = null;
            return this;
        }

        internal static float[] CalculateAscenderDescender(PdfFont font) {
            return CalculateAscenderDescender(font, RenderingMode.DEFAULT_LAYOUT_MODE);
        }

        internal static float[] CalculateAscenderDescender(PdfFont font, RenderingMode? mode) {
            FontMetrics fontMetrics = font.GetFontProgram().GetFontMetrics();
            float ascender;
            float descender;
            float usedTypoAscenderScaleCoeff = TYPO_ASCENDER_SCALE_COEFF;
            if (RenderingMode.HTML_MODE.Equals(mode) && !(font is PdfType1Font)) {
                usedTypoAscenderScaleCoeff = 1;
            }
            if (fontMetrics.GetWinAscender() == 0 || fontMetrics.GetWinDescender() == 0 || fontMetrics.GetTypoAscender
                () == fontMetrics.GetWinAscender() && fontMetrics.GetTypoDescender() == fontMetrics.GetWinDescender()) {
                ascender = fontMetrics.GetTypoAscender() * usedTypoAscenderScaleCoeff;
                descender = fontMetrics.GetTypoDescender() * usedTypoAscenderScaleCoeff;
            }
            else {
                ascender = fontMetrics.GetWinAscender();
                descender = fontMetrics.GetWinDescender();
            }
            return new float[] { ascender, descender };
        }

        private iText.Layout.Renderer.TextRenderer[] SplitIgnoreFirstNewLine(int currentTextPos) {
            if (iText.IO.Util.TextUtil.IsCarriageReturnFollowedByLineFeed(text, currentTextPos)) {
                return Split(currentTextPos + 2);
            }
            else {
                return Split(currentTextPos + 1);
            }
        }

        private GlyphLine ConvertToGlyphLine(String text) {
            return font.CreateGlyphLine(text);
        }

        private bool HasOtfFont() {
            return font is PdfType0Font && font.GetFontProgram() is TrueTypeFont;
        }

        protected internal override float? GetFirstYLineRecursively() {
            return GetYLine();
        }

        protected internal override float? GetLastYLineRecursively() {
            return GetYLine();
        }

        /// <summary>
        /// Returns the length of the
        /// <see cref="line">line</see>
        /// which is the result of the layout call.
        /// </summary>
        /// <returns>the length of the line</returns>
        protected internal virtual int LineLength() {
            return line.end > 0 ? line.end - line.start : 0;
        }

        protected internal virtual int BaseCharactersCount() {
            int count = 0;
            for (int i = line.start; i < line.end; i++) {
                Glyph glyph = line.Get(i);
                if (!glyph.HasPlacement()) {
                    count++;
                }
            }
            return count;
        }

        public override MinMaxWidth GetMinMaxWidth() {
            TextLayoutResult result = (TextLayoutResult)Layout(new LayoutContext(new LayoutArea(1, new Rectangle(MinMaxWidthUtils
                .GetInfWidth(), AbstractRenderer.INF))));
            return result.GetMinMaxWidth();
        }

        protected internal virtual int GetNumberOfSpaces() {
            if (line.end <= 0) {
                return 0;
            }
            int spaces = 0;
            for (int i = line.start; i < line.end; i++) {
                Glyph currentGlyph = line.Get(i);
                if (currentGlyph.GetUnicode() == ' ') {
                    spaces++;
                }
            }
            return spaces;
        }

        protected internal virtual iText.Layout.Renderer.TextRenderer CreateSplitRenderer() {
            return (iText.Layout.Renderer.TextRenderer)GetNextRenderer();
        }

        protected internal virtual iText.Layout.Renderer.TextRenderer CreateOverflowRenderer() {
            return (iText.Layout.Renderer.TextRenderer)GetNextRenderer();
        }

        protected internal virtual iText.Layout.Renderer.TextRenderer[] Split(int initialOverflowTextPos) {
            iText.Layout.Renderer.TextRenderer splitRenderer = CreateSplitRenderer();
            splitRenderer.SetText(text, text.start, initialOverflowTextPos);
            splitRenderer.font = font;
            splitRenderer.line = line;
            splitRenderer.occupiedArea = occupiedArea.Clone();
            splitRenderer.parent = parent;
            splitRenderer.yLineOffset = yLineOffset;
            splitRenderer.otfFeaturesApplied = otfFeaturesApplied;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.AddAllProperties(GetOwnProperties());
            iText.Layout.Renderer.TextRenderer overflowRenderer = CreateOverflowRenderer();
            overflowRenderer.SetText(text, initialOverflowTextPos, text.end);
            overflowRenderer.font = font;
            overflowRenderer.otfFeaturesApplied = otfFeaturesApplied;
            overflowRenderer.parent = parent;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            return new iText.Layout.Renderer.TextRenderer[] { splitRenderer, overflowRenderer };
        }

        protected internal virtual void DrawSingleUnderline(Underline underline, TransparentColor fontStrokeColor, 
            PdfCanvas canvas, float fontSize, float italicAngleTan) {
            TransparentColor underlineColor = underline.GetColor() != null ? new TransparentColor(underline.GetColor()
                , underline.GetOpacity()) : fontStrokeColor;
            canvas.SaveState();
            if (underlineColor != null) {
                canvas.SetStrokeColor(underlineColor.GetColor());
                underlineColor.ApplyStrokeTransparency(canvas);
            }
            canvas.SetLineCapStyle(underline.GetLineCapStyle());
            float underlineThickness = underline.GetThickness(fontSize);
            if (underlineThickness != 0) {
                canvas.SetLineWidth(underlineThickness);
                float yLine = GetYLine();
                float underlineYPosition = underline.GetYPosition(fontSize) + yLine;
                float italicWidthSubstraction = .5f * fontSize * italicAngleTan;
                Rectangle innerAreaBbox = GetInnerAreaBBox();
                canvas.MoveTo(innerAreaBbox.GetX(), underlineYPosition).LineTo(innerAreaBbox.GetX() + innerAreaBbox.GetWidth
                    () - italicWidthSubstraction, underlineYPosition).Stroke();
            }
            canvas.RestoreState();
        }

        protected internal virtual float CalculateLineWidth() {
            UnitValue fontSize = this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Property
                    .FONT_SIZE));
            }
            return GetGlyphLineWidth(line, fontSize.GetValue(), (float)this.GetPropertyAsFloat(Property.HORIZONTAL_SCALING
                , 1f), this.GetPropertyAsFloat(Property.CHARACTER_SPACING), this.GetPropertyAsFloat(Property.WORD_SPACING
                ));
        }

        /// <summary>
        /// Resolve
        /// <see cref="iText.Layout.Properties.Property.FONT"/>
        /// String[] value.
        /// </summary>
        /// <param name="addTo">add all processed renderers to.</param>
        /// <returns>
        /// true, if new
        /// <see cref="TextRenderer"/>
        /// has been created.
        /// </returns>
        protected internal virtual bool ResolveFonts(IList<IRenderer> addTo) {
            Object font = this.GetProperty<Object>(Property.FONT);
            if (font is PdfFont) {
                addTo.Add(this);
                return false;
            }
            else {
                if (font is String || font is String[]) {
                    if (font is String) {
                        // TODO remove this if-clause before 7.2
                        ILog logger = LogManager.GetLogger(typeof(AbstractRenderer));
                        logger.Warn(iText.IO.LogMessageConstant.FONT_PROPERTY_OF_STRING_TYPE_IS_DEPRECATED_USE_STRINGS_ARRAY_INSTEAD
                            );
                        IList<String> splitFontFamily = FontFamilySplitter.SplitFontFamily((String)font);
                        font = splitFontFamily.ToArray(new String[splitFontFamily.Count]);
                    }
                    FontProvider provider = this.GetProperty<FontProvider>(Property.FONT_PROVIDER);
                    FontSet fontSet = this.GetProperty<FontSet>(Property.FONT_SET);
                    if (provider.GetFontSet().IsEmpty() && (fontSet == null || fontSet.IsEmpty())) {
                        throw new InvalidOperationException(PdfException.FontProviderNotSetFontFamilyNotResolved);
                    }
                    FontCharacteristics fc = CreateFontCharacteristics();
                    FontSelectorStrategy strategy = provider.GetStrategy(strToBeConverted, JavaUtil.ArraysAsList((String[])font
                        ), fc, fontSet);
                    // process empty renderers because they can have borders or paddings with background to be drawn
                    if (null == strToBeConverted || String.IsNullOrEmpty(strToBeConverted)) {
                        addTo.Add(this);
                    }
                    else {
                        while (!strategy.EndOfText()) {
                            GlyphLine nextGlyphs = new GlyphLine(strategy.NextGlyphs());
                            PdfFont currentFont = strategy.GetCurrentFont();
                            iText.Layout.Renderer.TextRenderer textRenderer = CreateCopy(ReplaceSpecialWhitespaceGlyphs(nextGlyphs, currentFont
                                ), currentFont);
                            addTo.Add(textRenderer);
                        }
                    }
                    return true;
                }
                else {
                    throw new InvalidOperationException("Invalid FONT property value type.");
                }
            }
        }

        protected internal virtual void SetGlyphLineAndFont(GlyphLine gl, PdfFont font) {
            this.text = gl;
            this.font = font;
            this.otfFeaturesApplied = false;
            this.strToBeConverted = null;
            SetProperty(Property.FONT, font);
        }

        protected internal virtual iText.Layout.Renderer.TextRenderer CreateCopy(GlyphLine gl, PdfFont font) {
            iText.Layout.Renderer.TextRenderer copy = new iText.Layout.Renderer.TextRenderer(this);
            copy.SetGlyphLineAndFont(gl, font);
            return copy;
        }

        internal static void UpdateRangeBasedOnRemovedCharacters(List<int> removedIds, int[] range) {
            int shift = NumberOfElementsLessThan(removedIds, range[0]);
            range[0] -= shift;
            shift = NumberOfElementsLessThanOrEqual(removedIds, range[1]);
            range[1] -= shift;
        }

        internal override PdfFont ResolveFirstPdfFont(String[] font, FontProvider provider, FontCharacteristics fc
            , FontSet additionalFonts) {
            FontSelectorStrategy strategy = provider.GetStrategy(strToBeConverted, JavaUtil.ArraysAsList(font), fc, additionalFonts
                );
            IList<Glyph> resolvedGlyphs;
            PdfFont currentFont;
            //try to find first font that can render at least one glyph.
            while (!strategy.EndOfText()) {
                resolvedGlyphs = strategy.NextGlyphs();
                currentFont = strategy.GetCurrentFont();
                foreach (Glyph glyph in resolvedGlyphs) {
                    if (currentFont.ContainsGlyph(glyph.GetUnicode())) {
                        return currentFont;
                    }
                }
            }
            return base.ResolveFirstPdfFont(font, provider, fc, additionalFonts);
        }

        private static int NumberOfElementsLessThan(List<int> numbers, int n) {
            int x = numbers.BinarySearch(n);
            if (x >= 0) {
                return x;
            }
            else {
                return -x - 1;
            }
        }

        private static int NumberOfElementsLessThanOrEqual(List<int> numbers, int n) {
            int x = numbers.BinarySearch(n);
            if (x >= 0) {
                return x + 1;
            }
            else {
                return -x - 1;
            }
        }

        private static bool NoPrint(Glyph g) {
            if (!g.HasValidUnicode()) {
                return false;
            }
            int c = g.GetUnicode();
            return iText.IO.Util.TextUtil.IsNonPrintable(c);
        }

        private static bool GlyphBelongsToNonBreakingHyphenRelatedChunk(GlyphLine text, int ind) {
            return iText.IO.Util.TextUtil.IsNonBreakingHyphen(text.Get(ind)) || (ind + 1 < text.end && iText.IO.Util.TextUtil
                .IsNonBreakingHyphen(text.Get(ind + 1))) || ind - 1 >= text.start && iText.IO.Util.TextUtil.IsNonBreakingHyphen
                (text.Get(ind - 1));
        }

        private float GetCharWidth(Glyph g, float fontSize, float? hScale, float? characterSpacing, float? wordSpacing
            ) {
            if (hScale == null) {
                hScale = 1f;
            }
            float resultWidth = g.GetWidth() * fontSize * (float)hScale;
            if (characterSpacing != null) {
                resultWidth += (float)characterSpacing * (float)hScale * TEXT_SPACE_COEFF;
            }
            if (wordSpacing != null && g.GetUnicode() == ' ') {
                resultWidth += (float)wordSpacing * (float)hScale * TEXT_SPACE_COEFF;
            }
            return resultWidth;
        }

        private float ScaleXAdvance(float xAdvance, float fontSize, float? hScale) {
            return xAdvance * fontSize * (float)hScale;
        }

        private float GetGlyphLineWidth(GlyphLine glyphLine, float fontSize, float hScale, float? characterSpacing
            , float? wordSpacing) {
            float width = 0;
            for (int i = glyphLine.start; i < glyphLine.end; i++) {
                if (!NoPrint(glyphLine.Get(i))) {
                    float charWidth = GetCharWidth(glyphLine.Get(i), fontSize, hScale, characterSpacing, wordSpacing);
                    width += charWidth;
                    float xAdvance = (i != glyphLine.start) ? ScaleXAdvance(glyphLine.Get(i - 1).GetXAdvance(), fontSize, hScale
                        ) : 0;
                    width += xAdvance;
                }
            }
            return width / TEXT_SPACE_COEFF;
        }

        private int[] GetWordBoundsForHyphenation(GlyphLine text, int leftTextPos, int rightTextPos, int wordMiddleCharPos
            ) {
            while (wordMiddleCharPos >= leftTextPos && !IsGlyphPartOfWordForHyphenation(text.Get(wordMiddleCharPos)) &&
                 !iText.IO.Util.TextUtil.IsUni0020(text.Get(wordMiddleCharPos))) {
                wordMiddleCharPos--;
            }
            if (wordMiddleCharPos >= leftTextPos) {
                int left = wordMiddleCharPos;
                while (left >= leftTextPos && IsGlyphPartOfWordForHyphenation(text.Get(left))) {
                    left--;
                }
                int right = wordMiddleCharPos;
                while (right < rightTextPos && IsGlyphPartOfWordForHyphenation(text.Get(right))) {
                    right++;
                }
                return new int[] { left + 1, right };
            }
            else {
                return null;
            }
        }

        private bool IsGlyphPartOfWordForHyphenation(Glyph g) {
            return char.IsLetter((char)g.GetUnicode()) || 
                        // soft hyphen
                        '\u00ad' == g.GetUnicode();
        }

        private void UpdateFontAndText() {
            if (strToBeConverted != null) {
                try {
                    font = GetPropertyAsFont(Property.FONT);
                }
                catch (InvalidCastException) {
                    font = ResolveFirstPdfFont();
                    if (!String.IsNullOrEmpty(strToBeConverted)) {
                        ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                        logger.Error(iText.IO.LogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT);
                    }
                }
                text = ConvertToGlyphLine(strToBeConverted);
                otfFeaturesApplied = false;
                strToBeConverted = null;
            }
        }

        private void SaveWordBreakIfNotYetSaved(Glyph wordBreak) {
            if (savedWordBreakAtLineEnding == null) {
                if (iText.IO.Util.TextUtil.IsNewLine(wordBreak)) {
                    // we don't want to print '\n' in content stream
                    wordBreak = font.GetGlyph('\u0020');
                }
                // it's word-break character at the end of the line, which we want to save after trimming
                savedWordBreakAtLineEnding = new GlyphLine(JavaCollectionsUtil.SingletonList<Glyph>(wordBreak));
            }
        }

        private static GlyphLine ReplaceSpecialWhitespaceGlyphs(GlyphLine line, PdfFont font) {
            if (null != line) {
                Glyph space = font.GetGlyph('\u0020');
                Glyph glyph;
                for (int i = 0; i < line.Size(); i++) {
                    glyph = line.Get(i);
                    int? xAdvance = GetSpecialWhitespaceXAdvance(glyph, space, font.GetFontProgram().GetFontMetrics().IsFixedPitch
                        ());
                    if (xAdvance != null) {
                        Glyph newGlyph = new Glyph(space, glyph.GetUnicode());
                        System.Diagnostics.Debug.Assert(xAdvance <= short.MaxValue && xAdvance >= short.MinValue);
                        newGlyph.SetXAdvance((short)(int)xAdvance);
                        line.Set(i, newGlyph);
                    }
                }
            }
            return line;
        }

        private static int? GetSpecialWhitespaceXAdvance(Glyph glyph, Glyph spaceGlyph, bool isMonospaceFont) {
            if (glyph.GetCode() > 0) {
                return null;
            }
            switch (glyph.GetUnicode()) {
                // ensp
                case '\u2002': {
                    return isMonospaceFont ? 0 : 500 - spaceGlyph.GetWidth();
                }

                // emsp
                case '\u2003': {
                    return isMonospaceFont ? 0 : 1000 - spaceGlyph.GetWidth();
                }

                // thinsp
                case '\u2009': {
                    return isMonospaceFont ? 0 : 200 - spaceGlyph.GetWidth();
                }

                case '\t': {
                    return 3 * spaceGlyph.GetWidth();
                }
            }
            return null;
        }

        private class ReversedCharsIterator : IEnumerator<GlyphLine.GlyphLinePart> {
            private IList<int> outStart;

            private IList<int> outEnd;

            private IList<bool> reversed;

            private int currentInd = 0;

            private bool useReversed;

            public ReversedCharsIterator(IList<int[]> reversedRange, GlyphLine line) {
                outStart = new List<int>();
                outEnd = new List<int>();
                reversed = new List<bool>();
                if (reversedRange != null) {
                    if (reversedRange[0][0] > 0) {
                        outStart.Add(0);
                        outEnd.Add(reversedRange[0][0]);
                        reversed.Add(false);
                    }
                    for (int i = 0; i < reversedRange.Count; i++) {
                        int[] range = reversedRange[i];
                        outStart.Add(range[0]);
                        outEnd.Add(range[1] + 1);
                        reversed.Add(true);
                        if (i != reversedRange.Count - 1) {
                            outStart.Add(range[1] + 1);
                            outEnd.Add(reversedRange[i + 1][0]);
                            reversed.Add(false);
                        }
                    }
                    int lastIndex = reversedRange[reversedRange.Count - 1][1];
                    if (lastIndex < line.Size() - 1) {
                        outStart.Add(lastIndex + 1);
                        outEnd.Add(line.Size());
                        reversed.Add(false);
                    }
                }
                else {
                    outStart.Add(line.start);
                    outEnd.Add(line.end);
                    reversed.Add(false);
                }
            }

            public virtual TextRenderer.ReversedCharsIterator SetUseReversed(bool useReversed) {
                this.useReversed = useReversed;
                return this;
            }

            public virtual bool HasNext() {
                return currentInd < outStart.Count;
            }

            public virtual GlyphLine.GlyphLinePart Next() {
                GlyphLine.GlyphLinePart part = new GlyphLine.GlyphLinePart(outStart[currentInd], outEnd[currentInd]).SetReversed
                    (useReversed && reversed[currentInd]);
                currentInd++;
                return part;
            }

            public virtual void Remove() {
                throw new InvalidOperationException("Operation not supported");
            }

            public void Dispose() {
                
            }
            
            public bool MoveNext() {
                if (!HasNext()) {
                    return false;
                }
                
                Current = Next();
                return true;
            }
            
            public void Reset() {
                throw new System.NotSupportedException();
            }
            
            public GlyphLine.GlyphLinePart Current { get; private set; }
            
            object IEnumerator.Current {
                get { return Current; }
            }
        }

        private class ScriptRange {
            internal UnicodeScript? script;

            internal int rangeEnd;

            internal ScriptRange(UnicodeScript? script, int rangeEnd) {
                this.script = script;
                this.rangeEnd = rangeEnd;
            }
        }
    }
}
