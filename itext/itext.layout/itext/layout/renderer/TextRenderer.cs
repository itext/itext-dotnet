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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Exceptions;
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

        internal const int UNDEFINED_FIRST_CHAR_TO_FORCE_OVERFLOW = int.MaxValue;

        private const float ITALIC_ANGLE = 0.21256f;

        private const float BOLD_SIMULATION_STROKE_COEFF = 1 / 30f;

        protected internal float yLineOffset;

        private PdfFont font;

        protected internal GlyphLine text;

        protected internal GlyphLine line;

        protected internal String strToBeConverted;

        protected internal bool otfFeaturesApplied = false;

        protected internal float tabAnchorCharacterPosition = -1;

        protected internal IList<int[]> reversedRanges;

        protected internal GlyphLine savedWordBreakAtLineEnding;

        // if list is null, presence of special scripts in the TextRenderer#text hasn't been checked yet
        // if list is empty, TextRenderer#text has been analyzed and no special scripts have been detected
        // if list contains -1, TextRenderer#text contains special scripts, but no word break is possible within it
        // Must remain ArrayList: once an instance is formed and filled prior to layouting on split of this TextRenderer,
        // it's used to get element by index or passed to List.subList()
        private IList<int> specialScriptsWordBreakPoints;

        private int specialScriptFirstNotFittingIndex = -1;

        private int indexOfFirstCharacterToBeForcedToOverflow = UNDEFINED_FIRST_CHAR_TO_FORCE_OVERFLOW;

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
            this.specialScriptsWordBreakPoints = other.specialScriptsWordBreakPoints;
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            UpdateFontAndText();
            LayoutArea area = layoutContext.GetArea();
            Rectangle layoutBox = area.GetBBox().Clone();
            bool noSoftWrap = true.Equals(this.parent.GetOwnProperty<bool?>(Property.NO_SOFT_WRAP_INLINE));
            OverflowPropertyValue? overflowX = this.parent.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
            OverflowWrapPropertyValue? overflowWrap = this.GetProperty<OverflowWrapPropertyValue?>(Property.OVERFLOW_WRAP
                );
            bool overflowWrapNotNormal = overflowWrap == OverflowWrapPropertyValue.ANYWHERE || overflowWrap == OverflowWrapPropertyValue
                .BREAK_WORD;
            if (overflowWrapNotNormal) {
                overflowX = OverflowPropertyValue.FIT;
            }
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            FloatPropertyValue? floatPropertyValue = this.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                FloatingHelper.AdjustFloatedBlockLayoutBox(this, layoutBox, null, floatRendererAreas, floatPropertyValue, 
                    overflowX);
            }
            float preMarginBorderPaddingWidth = layoutBox.GetWidth();
            UnitValue[] margins = GetMargins();
            ApplyMargins(layoutBox, margins, false);
            Border[] borders = GetBorders();
            ApplyBorderBox(layoutBox, borders, false);
            UnitValue[] paddings = GetPaddings();
            ApplyPaddings(layoutBox, paddings, false);
            MinMaxWidth countedMinMaxWidth = new MinMaxWidth(preMarginBorderPaddingWidth - layoutBox.GetWidth());
            AbstractWidthHandler widthHandler;
            if (noSoftWrap) {
                widthHandler = new SumSumWidthHandler(countedMinMaxWidth);
            }
            else {
                widthHandler = new MaxSumWidthHandler(countedMinMaxWidth);
            }
            float leftMinWidth = -1f;
            float[] leftMarginBorderPadding = new float[] { margins[LEFT_SIDE].GetValue(), borders[LEFT_SIDE] == null ? 
                0.0f : borders[LEFT_SIDE].GetWidth(), paddings[LEFT_SIDE].GetValue() };
            float rightMinWidth = -1f;
            float[] rightMarginBorderPadding = new float[] { margins[RIGHT_SIDE].GetValue(), borders[RIGHT_SIDE] == null
                 ? 0.0f : borders[RIGHT_SIDE].GetWidth(), paddings[RIGHT_SIDE].GetValue() };
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), 0, 0));
            TargetCounterHandler.AddPageByID(this);
            bool anythingPlaced = false;
            int currentTextPos = text.start;
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
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
                currentLineHeight = (currentLineAscender - currentLineDescender) * FontProgram.ConvertTextSpaceToGlyphSpace
                    (fontSize.GetValue()) + textRise;
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
            bool containsPossibleBreak = false;
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
                        containsPossibleBreak = true;
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
                        bool nextGlyphIsSpaceOrWhiteSpace = ind + 1 < text.end && (splitCharacters.IsSplitCharacter(text, ind + 1)
                             && iText.IO.Util.TextUtil.IsSpaceOrWhitespace(text.Get(ind + 1)));
                        if (nextGlyphIsSpaceOrWhiteSpace && firstCharacterWhichExceedsAllowedWidth == -1) {
                            containsPossibleBreak = true;
                        }
                        if (ind + 1 == text.end || nextGlyphIsSpaceOrWhiteSpace || (ind + 1 >= indexOfFirstCharacterToBeForcedToOverflow
                            )) {
                            if (ind + 1 >= indexOfFirstCharacterToBeForcedToOverflow) {
                                firstCharacterWhichExceedsAllowedWidth = currentTextPos;
                                break;
                            }
                            else {
                                nonBreakablePartEnd = ind;
                                break;
                            }
                        }
                        continue;
                    }
                    if (tabAnchorCharacter != null && tabAnchorCharacter == text.Get(ind).GetUnicode()) {
                        tabAnchorCharacterPosition = currentLineWidth + nonBreakablePartFullWidth;
                        tabAnchorCharacter = null;
                    }
                    float glyphWidth = FontProgram.ConvertTextSpaceToGlyphSpace(GetCharWidth(currentGlyph, fontSize.GetValue()
                        , hScale, characterSpacing, wordSpacing));
                    float xAdvance = previousCharPos != -1 ? text.Get(previousCharPos).GetXAdvance() : 0;
                    if (xAdvance != 0) {
                        xAdvance = FontProgram.ConvertTextSpaceToGlyphSpace(ScaleXAdvance(xAdvance, fontSize.GetValue(), hScale));
                    }
                    float potentialWidth = nonBreakablePartFullWidth + glyphWidth + xAdvance + italicSkewAddition + boldSimulationAddition;
                    bool symbolNotFitOnLine = potentialWidth > layoutBox.GetWidth() - currentLineWidth + EPS;
                    if ((!noSoftWrap && symbolNotFitOnLine && firstCharacterWhichExceedsAllowedWidth == -1) || ind == specialScriptFirstNotFittingIndex
                        ) {
                        firstCharacterWhichExceedsAllowedWidth = ind;
                        bool spaceOrWhitespace = iText.IO.Util.TextUtil.IsSpaceOrWhitespace(text.Get(ind));
                        OverflowPropertyValue? parentOverflowX = parent.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
                        if (spaceOrWhitespace || overflowWrapNotNormal && !IsOverflowFit(parentOverflowX)) {
                            if (spaceOrWhitespace) {
                                wordBreakGlyphAtLineEnding = currentGlyph;
                            }
                            if (ind == firstPrintPos) {
                                containsPossibleBreak = true;
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
                    if (firstCharacterWhichExceedsAllowedWidth == -1 || !IsOverflowFit(overflowX)) {
                        nonBreakablePartWidthWhichDoesNotExceedAllowedWidth += glyphWidth + xAdvance;
                    }
                    nonBreakablePartFullWidth += glyphWidth + xAdvance;
                    nonBreakablePartMaxAscender = Math.Max(nonBreakablePartMaxAscender, ascender);
                    nonBreakablePartMaxDescender = Math.Min(nonBreakablePartMaxDescender, descender);
                    nonBreakablePartMaxHeight = FontProgram.ConvertTextSpaceToGlyphSpace((nonBreakablePartMaxAscender - nonBreakablePartMaxDescender
                        ) * fontSize.GetValue()) + textRise;
                    previousCharPos = ind;
                    if (!noSoftWrap && symbolNotFitOnLine && (0 == nonBreakingHyphenRelatedChunkWidth || ind + 1 == text.end ||
                         !GlyphBelongsToNonBreakingHyphenRelatedChunk(text, ind + 1))) {
                        if (IsOverflowFit(overflowX)) {
                            // we have extracted all the information we wanted and we do not want to continue.
                            // we will have to split the word anyway.
                            break;
                        }
                    }
                    if (OverflowWrapPropertyValue.ANYWHERE == overflowWrap) {
                        float childMinWidth = (float)((double)glyphWidth + (double)xAdvance + (double)italicSkewAddition + (double
                            )boldSimulationAddition);
                        if (leftMinWidth == -1f) {
                            leftMinWidth = childMinWidth;
                        }
                        else {
                            rightMinWidth = childMinWidth;
                        }
                        widthHandler.UpdateMinChildWidth(childMinWidth);
                        widthHandler.UpdateMaxChildWidth((float)((double)glyphWidth + (double)xAdvance));
                    }
                    bool endOfWordBelongingToSpecialScripts = TextContainsSpecialScriptGlyphs(true) && FindPossibleBreaksSplitPosition
                        (specialScriptsWordBreakPoints, ind + 1, true) >= 0;
                    bool endOfNonBreakablePartCausedBySplitCharacter = splitCharacters.IsSplitCharacter(text, ind) || (ind + 1
                         < text.end && (splitCharacters.IsSplitCharacter(text, ind + 1) && iText.IO.Util.TextUtil.IsSpaceOrWhitespace
                        (text.Get(ind + 1))));
                    if (endOfNonBreakablePartCausedBySplitCharacter && firstCharacterWhichExceedsAllowedWidth == -1) {
                        containsPossibleBreak = true;
                    }
                    if (ind + 1 == text.end || endOfNonBreakablePartCausedBySplitCharacter || endOfWordBelongingToSpecialScripts
                         || (ind + 1 >= indexOfFirstCharacterToBeForcedToOverflow)) {
                        if (ind + 1 >= indexOfFirstCharacterToBeForcedToOverflow && !endOfNonBreakablePartCausedBySplitCharacter) {
                            firstCharacterWhichExceedsAllowedWidth = currentTextPos;
                        }
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
                    if (OverflowWrapPropertyValue.ANYWHERE == overflowWrap) {
                        widthHandler.UpdateMaxChildWidth((float)((double)italicSkewAddition + (double)boldSimulationAddition));
                    }
                    else {
                        float childMinWidth = (float)((double)nonBreakablePartWidthWhichDoesNotExceedAllowedWidth + (double)italicSkewAddition
                             + (double)boldSimulationAddition);
                        if (leftMinWidth == -1f) {
                            leftMinWidth = childMinWidth;
                        }
                        else {
                            rightMinWidth = childMinWidth;
                        }
                        widthHandler.UpdateMinChildWidth(childMinWidth);
                        widthHandler.UpdateMaxChildWidth(childMinWidth);
                    }
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
                        line.end = Math.Max(line.end, firstCharacterWhichExceedsAllowedWidth);
                        // the line does not fit because of height - full overflow
                        iText.Layout.Renderer.TextRenderer[] splitResult = Split(initialLineTextPos);
                        bool[] startsEnds = IsStartsWithSplitCharWhiteSpaceAndEndsWithSplitChar(splitCharacters);
                        return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, splitResult[0], splitResult[1], this).SetContainsPossibleBreak
                            (containsPossibleBreak).SetStartsWithSplitCharacterWhiteSpace(startsEnds[0]).SetEndsWithSplitCharacter
                            (startsEnds[1]);
                    }
                    else {
                        // cannot fit a word as a whole
                        bool wordSplit = false;
                        bool hyphenationApplied = false;
                        if (hyphenationConfig != null && indexOfFirstCharacterToBeForcedToOverflow == UNDEFINED_FIRST_CHAR_TO_FORCE_OVERFLOW
                            ) {
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
                                                // TODO DEVSIX-7010 recalculate line properties in case of word hyphenation.
                                                // These values are based on whole word. Recalculate properly based on hyphenated part.
                                                currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
                                                currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender);
                                                currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
                                                currentLineWidth += currentHyphenationChoicePreTextWidth;
                                                if (OverflowWrapPropertyValue.ANYWHERE == overflowWrap) {
                                                    widthHandler.UpdateMaxChildWidth((float)((double)italicSkewAddition + (double)boldSimulationAddition));
                                                }
                                                else {
                                                    widthHandler.UpdateMinChildWidth((float)((double)currentHyphenationChoicePreTextWidth + (double)italicSkewAddition
                                                         + (double)boldSimulationAddition));
                                                    widthHandler.UpdateMaxChildWidth((float)((double)currentHyphenationChoicePreTextWidth + (double)italicSkewAddition
                                                         + (double)boldSimulationAddition));
                                                }
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
                        bool specialScriptWordSplit = TextContainsSpecialScriptGlyphs(true) && !isSplitForcedByNewLine && IsOverflowFit
                            (overflowX);
                        if ((nonBreakablePartFullWidth > layoutBox.GetWidth() && !anythingPlaced && !hyphenationApplied) || forcePartialSplitOnFirstChar
                             || -1 != nonBreakingHyphenRelatedChunkStart || specialScriptWordSplit) {
                            // if the word is too long for a single line we will have to split it
                            // we also need to split the word here if text contains glyphs from scripts
                            // which require word wrapping for further processing in LineRenderer
                            if (line.start == -1) {
                                line.start = currentTextPos;
                            }
                            if (!crlf) {
                                currentTextPos = (forcePartialSplitOnFirstChar || IsOverflowFit(overflowX) || specialScriptWordSplit) ? firstCharacterWhichExceedsAllowedWidth
                                     : nonBreakablePartEnd + 1;
                            }
                            line.end = Math.Max(line.end, currentTextPos);
                            wordSplit = !forcePartialSplitOnFirstChar && (text.end != currentTextPos);
                            if (wordSplit || !(forcePartialSplitOnFirstChar || IsOverflowFit(overflowX))) {
                                currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
                                currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender);
                                currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
                                currentLineWidth += nonBreakablePartWidthWhichDoesNotExceedAllowedWidth;
                                if (OverflowWrapPropertyValue.ANYWHERE == overflowWrap) {
                                    widthHandler.UpdateMaxChildWidth((float)((double)italicSkewAddition + (double)boldSimulationAddition));
                                }
                                else {
                                    float childMinWidth = (float)((double)nonBreakablePartWidthWhichDoesNotExceedAllowedWidth + (double)italicSkewAddition
                                         + (double)boldSimulationAddition);
                                    if (leftMinWidth == -1f) {
                                        leftMinWidth = childMinWidth;
                                    }
                                    else {
                                        rightMinWidth = childMinWidth;
                                    }
                                    widthHandler.UpdateMinChildWidth(childMinWidth);
                                    widthHandler.UpdateMaxChildWidth(childMinWidth);
                                }
                            }
                            else {
                                // process empty line (e.g. '\n')
                                currentLineAscender = ascender;
                                currentLineDescender = descender;
                                currentLineHeight = FontProgram.ConvertTextSpaceToGlyphSpace((currentLineAscender - currentLineDescender) 
                                    * fontSize.GetValue()) + textRise;
                                currentLineWidth += FontProgram.ConvertTextSpaceToGlyphSpace(GetCharWidth(line.Get(line.start), fontSize.GetValue
                                    (), hScale, characterSpacing, wordSpacing));
                            }
                        }
                        if (line.end <= line.start) {
                            bool[] startsEnds = IsStartsWithSplitCharWhiteSpaceAndEndsWithSplitChar(splitCharacters);
                            return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this).SetContainsPossibleBreak
                                (containsPossibleBreak).SetStartsWithSplitCharacterWhiteSpace(startsEnds[0]).SetEndsWithSplitCharacter
                                (startsEnds[1]);
                        }
                        else {
                            result = new TextLayoutResult(LayoutResult.PARTIAL, occupiedArea, null, null).SetWordHasBeenSplit(wordSplit
                                ).SetContainsPossibleBreak(containsPossibleBreak);
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
                    bool[] startsEnds = IsStartsWithSplitCharWhiteSpaceAndEndsWithSplitChar(splitCharacters);
                    return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this).SetContainsPossibleBreak
                        (containsPossibleBreak).SetStartsWithSplitCharacterWhiteSpace(startsEnds[0]).SetEndsWithSplitCharacter
                        (startsEnds[1]);
                }
                else {
                    isPlacingForcedWhileNothing = true;
                }
            }
            yLineOffset = FontProgram.ConvertTextSpaceToGlyphSpace(currentLineAscender * fontSize.GetValue());
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
                     : null).SetContainsPossibleBreak(containsPossibleBreak);
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
            if (!noSoftWrap) {
                foreach (float dimension in leftMarginBorderPadding) {
                    leftMinWidth += dimension;
                }
                foreach (float dimension in rightMarginBorderPadding) {
                    if (rightMinWidth < 0) {
                        leftMinWidth += dimension;
                    }
                    else {
                        rightMinWidth += dimension;
                    }
                }
                result.SetLeftMinWidth(leftMinWidth);
                result.SetRightMinWidth(rightMinWidth);
            }
            else {
                result.SetLeftMinWidth(countedMinMaxWidth.GetMinWidth());
                result.SetRightMinWidth(-1f);
            }
            bool[] startsEnds_1 = IsStartsWithSplitCharWhiteSpaceAndEndsWithSplitChar(splitCharacters);
            result.SetStartsWithSplitCharacterWhiteSpace(startsEnds_1[0]).SetEndsWithSplitCharacter(startsEnds_1[1]);
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
                PdfDocument pdfDocument = GetPdfDocument();
                SequenceId sequenceId = pdfDocument == null ? null : pdfDocument.GetDocumentIdWrapper();
                MetaInfoContainer metaInfoContainer = this.GetProperty<MetaInfoContainer>(Property.META_INFO);
                IMetaInfo metaInfo = metaInfoContainer == null ? null : metaInfoContainer.GetMetaInfo();
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
                        TypographyUtils.ApplyOtfScript(font.GetFontProgram(), text, scriptsRange.script, typographyConfig, sequenceId
                            , metaInfo);
                        delta += text.end - scriptsRange.rangeEnd;
                        scriptsRange.rangeEnd = shapingRangeStart = text.end;
                    }
                    text.start = origTextStart;
                    text.end = origTextEnd + delta;
                }
                FontKerning fontKerning = (FontKerning)this.GetProperty<FontKerning?>(Property.FONT_KERNING, FontKerning.NO
                    );
                if (fontKerning == FontKerning.YES) {
                    TypographyUtils.ApplyKerning(font.GetFontProgram(), text, sequenceId, metaInfo);
                }
                otfFeaturesApplied = true;
            }
        }

        public override void Draw(DrawContext drawContext) {
            if (occupiedArea == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Drawing won't be performed."));
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
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.FONT_SIZE));
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
                                short advance = (short)(FontProgram.ConvertGlyphSpaceToTextSpace((float)wordSpacing) / fontSize.GetValue()
                                    );
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
                GlyphLine.IGlyphLineFilter filter = new TextRenderer.CustomGlyphLineFilter();
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
            /*  Between two sentences separated by one or more whitespaces,
            icu allows to break right after the last whitespace.
            Therefore we need to carefully edit specialScriptsWordBreakPoints list after trimming:
            if a break is allowed to happen right before the first glyph of an already trimmed text,
            we need to remove this point from the list
            (or replace it with -1 thus marking that text contains special scripts,
            in case if the removed break point was the only possible break point).
            */
            if (TextContainsSpecialScriptGlyphs(true) && specialScriptsWordBreakPoints[0] == text.start) {
                if (specialScriptsWordBreakPoints.Count == 1) {
                    specialScriptsWordBreakPoints[0] = -1;
                }
                else {
                    specialScriptsWordBreakPoints.JRemoveAt(0);
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
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
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
                float currentCharWidth = FontProgram.ConvertTextSpaceToGlyphSpace(GetCharWidth(currentGlyph, fontSize.GetValue
                    (), hScale, characterSpacing, wordSpacing));
                float xAdvance = firstNonSpaceCharIndex > line.start ? FontProgram.ConvertTextSpaceToGlyphSpace(ScaleXAdvance
                    (line.Get(firstNonSpaceCharIndex - 1).GetXAdvance(), fontSize.GetValue(), hScale)) : 0;
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

        /// <summary>Manually set a GlyphLine and PdfFont for rendering.</summary>
        /// <param name="text">
        /// the
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// </param>
        /// <param name="font">the font</param>
        public virtual void SetText(GlyphLine text, PdfFont font) {
            GlyphLine newText = new GlyphLine(text);
            newText = TextPreprocessingUtil.ReplaceSpecialWhitespaceGlyphs(newText, font);
            SetProcessedGlyphLineAndFont(newText, font);
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

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <remarks>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// <para />
        /// If
        /// <see cref="TextRenderer"/>
        /// overflows to the next line, iText uses this method to create a renderer
        /// for the overflow part. So if one wants to extend
        /// <see cref="TextRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created. Another method that should be overridden in case of
        /// <see cref="TextRenderer"/>
        /// 's extension is
        /// <see cref="CreateCopy(iText.IO.Font.Otf.GlyphLine, iText.Kernel.Font.PdfFont)"/>
        /// . This method is responsible
        /// for creation of
        /// <see cref="TextRenderer"/>
        /// 's copies, which represent its parts of specific font.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.TextRenderer), this.GetType());
            return new iText.Layout.Renderer.TextRenderer((Text)modelElement);
        }

        /// <summary>Get ascender and descender from font metrics.</summary>
        /// <remarks>
        /// Get ascender and descender from font metrics.
        /// If these values are obtained from typo metrics they are normalized with a scale coefficient.
        /// </remarks>
        /// <param name="font">from which metrics will be extracted</param>
        /// <returns>array in which the first element is an ascender and the second is a descender</returns>
        public static float[] CalculateAscenderDescender(PdfFont font) {
            return CalculateAscenderDescender(font, RenderingMode.DEFAULT_LAYOUT_MODE);
        }

        /// <summary>Get ascender and descender from font metrics.</summary>
        /// <remarks>
        /// Get ascender and descender from font metrics.
        /// In RenderingMode.DEFAULT_LAYOUT_MODE if these values are obtained from typo metrics they are normalized with a scale coefficient.
        /// </remarks>
        /// <param name="font">from which metrics will be extracted</param>
        /// <param name="mode">mode in which metrics will be obtained. Impact on the use of scale coefficient</param>
        /// <returns>array in which the first element is an ascender and the second is a descender</returns>
        public static float[] CalculateAscenderDescender(PdfFont font, RenderingMode? mode) {
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

        /// <summary>
        /// Analyzes/checks whether
        /// <see cref="text"/>
        /// , bounded by start and end,
        /// contains glyphs belonging to special script.
        /// </summary>
        /// <remarks>
        /// Analyzes/checks whether
        /// <see cref="text"/>
        /// , bounded by start and end,
        /// contains glyphs belonging to special script.
        /// Mind that the behavior of this method depends on the analyzeSpecialScriptsWordBreakPointsOnly parameter:
        /// - pass
        /// <see langword="false"/>
        /// if you need to analyze the
        /// <see cref="text"/>
        /// by checking each of its glyphs
        /// AND to fill
        /// <see cref="specialScriptsWordBreakPoints"/>
        /// list afterwards,
        /// i.e. when analyzing a sequence of TextRenderers prior to layouting;
        /// - pass
        /// <see langword="true"/>
        /// if you want to check if text contains glyphs belonging to special scripts,
        /// according to the already filled
        /// <see cref="specialScriptsWordBreakPoints"/>
        /// list.
        /// </remarks>
        /// <param name="analyzeSpecialScriptsWordBreakPointsOnly">
        /// false if analysis of each glyph is required,
        /// true if analysis has already been performed earlier
        /// and the results are stored in
        /// <see cref="specialScriptsWordBreakPoints"/>
        /// </param>
        /// <returns>
        /// true if
        /// <see cref="text"/>
        /// , bounded by start and end, contains glyphs belonging to special script, otherwise false
        /// </returns>
        /// <seealso cref="specialScriptsWordBreakPoints"/>
        internal virtual bool TextContainsSpecialScriptGlyphs(bool analyzeSpecialScriptsWordBreakPointsOnly) {
            if (specialScriptsWordBreakPoints != null) {
                return !specialScriptsWordBreakPoints.IsEmpty();
            }
            if (analyzeSpecialScriptsWordBreakPointsOnly) {
                return false;
            }
            ISplitCharacters splitCharacters = this.GetProperty<ISplitCharacters>(Property.SPLIT_CHARACTERS);
            if (splitCharacters is BreakAllSplitCharacters) {
                specialScriptsWordBreakPoints = new List<int>();
            }
            for (int i = text.start; i < text.end; i++) {
                int unicode = text.Get(i).GetUnicode();
                if (unicode > -1) {
                    if (CodePointIsOfSpecialScript(unicode)) {
                        return true;
                    }
                }
                else {
                    char[] chars = text.Get(i).GetChars();
                    if (chars != null) {
                        foreach (char ch in chars) {
                            if (CodePointIsOfSpecialScript(ch)) {
                                return true;
                            }
                        }
                    }
                }
            }
            // if we've reached this point, it means we've analyzed the entire TextRenderer#text
            // and haven't found special scripts, therefore we define specialScriptsWordBreakPoints
            // as an empty list to mark, it's already been analyzed
            specialScriptsWordBreakPoints = new List<int>();
            return false;
        }

        internal virtual void SetSpecialScriptsWordBreakPoints(IList<int> specialScriptsWordBreakPoints) {
            this.specialScriptsWordBreakPoints = specialScriptsWordBreakPoints;
        }

        internal virtual IList<int> GetSpecialScriptsWordBreakPoints() {
            return this.specialScriptsWordBreakPoints;
        }

        internal virtual void SetSpecialScriptFirstNotFittingIndex(int lastFittingIndex) {
            this.specialScriptFirstNotFittingIndex = lastFittingIndex;
        }

        internal virtual int GetSpecialScriptFirstNotFittingIndex() {
            return specialScriptFirstNotFittingIndex;
        }

        internal virtual void SetIndexOfFirstCharacterToBeForcedToOverflow(int indexOfFirstCharacterToBeForcedToOverflow
            ) {
            this.indexOfFirstCharacterToBeForcedToOverflow = indexOfFirstCharacterToBeForcedToOverflow;
        }

        protected internal override Rectangle GetBackgroundArea(Rectangle occupiedAreaWithMargins) {
            float textRise = (float)this.GetPropertyAsFloat(Property.TEXT_RISE);
            return occupiedAreaWithMargins.MoveUp(textRise).DecreaseHeight(textRise);
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
            GlyphLine newText = new GlyphLine(text);
            newText.start = text.start;
            newText.end = initialOverflowTextPos;
            splitRenderer.SetProcessedGlyphLineAndFont(newText, font);
            splitRenderer.line = line;
            splitRenderer.occupiedArea = occupiedArea.Clone();
            splitRenderer.parent = parent;
            splitRenderer.yLineOffset = yLineOffset;
            splitRenderer.otfFeaturesApplied = otfFeaturesApplied;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.AddAllProperties(GetOwnProperties());
            iText.Layout.Renderer.TextRenderer overflowRenderer = CreateOverflowRenderer();
            newText = new GlyphLine(text);
            newText.start = initialOverflowTextPos;
            newText.end = text.end;
            overflowRenderer.SetProcessedGlyphLineAndFont(newText, font);
            overflowRenderer.otfFeaturesApplied = otfFeaturesApplied;
            overflowRenderer.parent = parent;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            if (specialScriptsWordBreakPoints != null) {
                if (specialScriptsWordBreakPoints.IsEmpty()) {
                    splitRenderer.SetSpecialScriptsWordBreakPoints(new List<int>());
                    overflowRenderer.SetSpecialScriptsWordBreakPoints(new List<int>());
                }
                else {
                    if (specialScriptsWordBreakPoints[0] == -1) {
                        IList<int> split = new List<int>(1);
                        split.Add(-1);
                        splitRenderer.SetSpecialScriptsWordBreakPoints(split);
                        IList<int> overflow = new List<int>(1);
                        overflow.Add(-1);
                        overflowRenderer.SetSpecialScriptsWordBreakPoints(overflow);
                    }
                    else {
                        int splitIndex = FindPossibleBreaksSplitPosition(specialScriptsWordBreakPoints, initialOverflowTextPos, false
                            );
                        if (splitIndex > -1) {
                            splitRenderer.SetSpecialScriptsWordBreakPoints(specialScriptsWordBreakPoints.SubList(0, splitIndex + 1));
                        }
                        else {
                            IList<int> split = new List<int>(1);
                            split.Add(-1);
                            splitRenderer.SetSpecialScriptsWordBreakPoints(split);
                        }
                        if (splitIndex + 1 < specialScriptsWordBreakPoints.Count) {
                            overflowRenderer.SetSpecialScriptsWordBreakPoints(specialScriptsWordBreakPoints.SubList(splitIndex + 1, specialScriptsWordBreakPoints
                                .Count));
                        }
                        else {
                            IList<int> split = new List<int>(1);
                            split.Add(-1);
                            overflowRenderer.SetSpecialScriptsWordBreakPoints(split);
                        }
                    }
                }
            }
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
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
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
                if (font is String[]) {
                    FontProvider provider = this.GetProperty<FontProvider>(Property.FONT_PROVIDER);
                    FontSet fontSet = this.GetProperty<FontSet>(Property.FONT_SET);
                    if (provider.GetFontSet().IsEmpty() && (fontSet == null || fontSet.IsEmpty())) {
                        throw new InvalidOperationException(LayoutExceptionMessageConstant.FONT_PROVIDER_NOT_SET_FONT_FAMILY_NOT_RESOLVED
                            );
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
                            GlyphLine newGlyphs = TextPreprocessingUtil.ReplaceSpecialWhitespaceGlyphs(nextGlyphs, currentFont);
                            iText.Layout.Renderer.TextRenderer textRenderer = CreateCopy(newGlyphs, currentFont);
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

        protected internal virtual void SetProcessedGlyphLineAndFont(GlyphLine gl, PdfFont font) {
            this.text = gl;
            this.font = font;
            this.otfFeaturesApplied = false;
            this.strToBeConverted = null;
            this.specialScriptsWordBreakPoints = null;
            SetProperty(Property.FONT, font);
        }

        /// <summary>
        /// Creates a copy of this
        /// <see cref="TextRenderer"/>
        /// , which corresponds to the passed
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// with
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </summary>
        /// <remarks>
        /// Creates a copy of this
        /// <see cref="TextRenderer"/>
        /// , which corresponds to the passed
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// with
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// <para />
        /// While processing
        /// <see cref="TextRenderer"/>
        /// , iText uses this method to create
        /// <see cref="iText.IO.Font.Otf.GlyphLine">glyph lines</see>
        /// of specific
        /// <see cref="iText.Kernel.Font.PdfFont">fonts</see>
        /// , which represent the
        /// <see cref="TextRenderer"/>
        /// 's parts. If one extends
        /// <see cref="TextRenderer"/>
        /// , one should override this method, otherwise if
        /// <see cref="iText.Layout.Font.FontSelector"/>
        /// related logic is triggered, copies of this
        /// <see cref="TextRenderer"/>
        /// will have the default behavior rather than
        /// the custom one.
        /// </remarks>
        /// <param name="gl">
        /// a
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// which represents some of this
        /// <see cref="TextRenderer"/>
        /// 's content
        /// </param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// for this part of the
        /// <see cref="TextRenderer"/>
        /// 's content
        /// </param>
        /// <returns>
        /// copy of this
        /// <see cref="TextRenderer"/>
        /// , which correspond to the passed
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// with
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </returns>
        protected internal virtual iText.Layout.Renderer.TextRenderer CreateCopy(GlyphLine gl, PdfFont font) {
            if (typeof(iText.Layout.Renderer.TextRenderer) != this.GetType()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.CREATE_COPY_SHOULD_BE_OVERRIDDEN
                    ));
            }
            iText.Layout.Renderer.TextRenderer copy = new iText.Layout.Renderer.TextRenderer(this);
            copy.SetProcessedGlyphLineAndFont(gl, font);
            return copy;
        }

        internal static void UpdateRangeBasedOnRemovedCharacters(List<int> removedIds, int[] range) {
            int shift = NumberOfElementsLessThan(removedIds, range[0]);
            range[0] -= shift;
            shift = NumberOfElementsLessThanOrEqual(removedIds, range[1]);
            range[1] -= shift;
        }

        // if amongPresentOnly is true,
        // returns the index of lists's element which equals textStartBasedInitialOverflowTextPos
        // or -1 if textStartBasedInitialOverflowTextPos wasn't found in the list.
        // if amongPresentOnly is false, returns the index of list's element
        // that is not greater than textStartBasedInitialOverflowTextPos
        // if there's no such element in the list, -1 is returned
        internal static int FindPossibleBreaksSplitPosition(IList<int> list, int textStartBasedInitialOverflowTextPos
            , bool amongPresentOnly) {
            int low = 0;
            int high = list.Count - 1;
            while (low <= high) {
                int middle = (int)(((uint)(low + high)) >> 1);
                if (list[middle].CompareTo(textStartBasedInitialOverflowTextPos) < 0) {
                    low = middle + 1;
                }
                else {
                    if (list[middle].CompareTo(textStartBasedInitialOverflowTextPos) > 0) {
                        high = middle - 1;
                    }
                    else {
                        return middle;
                    }
                }
            }
            if (!amongPresentOnly && low > 0) {
                return low - 1;
            }
            return -1;
        }

        internal static bool CodePointIsOfSpecialScript(int codePoint) {
            UnicodeScript? glyphScript = UnicodeScriptUtil.Of(codePoint);
            return UnicodeScript.THAI == glyphScript || UnicodeScript.KHMER == glyphScript || UnicodeScript.LAO == glyphScript
                 || UnicodeScript.MYANMAR == glyphScript;
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

        /// <summary>Identifies two properties for the layouted text renderer text: start and end break possibilities.
        ///     </summary>
        /// <remarks>
        /// Identifies two properties for the layouted text renderer text: start and end break possibilities.
        /// First - if it ends with split character, second - if it starts with the split character
        /// which is at the same time is a whitespace character. These properties will later be used for identifying
        /// if we can consider this and previous/next text renderers chunks to be a part of a single word spanning across
        /// the text renderers boundaries. In the start of the text renderer we only care about split characters, which are
        /// white spaces, because only such will allow soft-breaks before them: normally split characters allow breaks only
        /// after them.
        /// </remarks>
        /// <param name="splitCharacters">
        /// current renderer
        /// <see cref="iText.Layout.Splitting.ISplitCharacters"/>
        /// property value
        /// </param>
        /// <returns>
        /// a boolean array of two elements, where first element identifies start break possibility, and second - end
        /// break possibility.
        /// </returns>
        internal virtual bool[] IsStartsWithSplitCharWhiteSpaceAndEndsWithSplitChar(ISplitCharacters splitCharacters
            ) {
            bool startsWithBreak = line.start < line.end && splitCharacters.IsSplitCharacter(text, line.start) && iText.IO.Util.TextUtil
                .IsSpaceOrWhitespace(text.Get(line.start));
            bool endsWithBreak = line.start < line.end && splitCharacters.IsSplitCharacter(text, line.end - 1);
            if (specialScriptsWordBreakPoints == null || specialScriptsWordBreakPoints.IsEmpty()) {
                return new bool[] { startsWithBreak, endsWithBreak };
            }
            else {
                if (!endsWithBreak) {
                    endsWithBreak = specialScriptsWordBreakPoints.Contains(line.end);
                }
                return new bool[] { startsWithBreak, endsWithBreak };
            }
        }

        private float GetCharWidth(Glyph g, float fontSize, float? hScale, float? characterSpacing, float? wordSpacing
            ) {
            if (hScale == null) {
                hScale = 1f;
            }
            float resultWidth = g.GetWidth() * fontSize * (float)hScale;
            if (characterSpacing != null) {
                resultWidth += FontProgram.ConvertGlyphSpaceToTextSpace((float)characterSpacing * (float)hScale);
            }
            if (wordSpacing != null && g.GetUnicode() == ' ') {
                resultWidth += FontProgram.ConvertGlyphSpaceToTextSpace((float)wordSpacing * (float)hScale);
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
            return FontProgram.ConvertTextSpaceToGlyphSpace(width);
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
                PdfFont newFont;
                try {
                    newFont = GetPropertyAsFont(Property.FONT);
                }
                catch (InvalidCastException) {
                    newFont = ResolveFirstPdfFont();
                    if (!String.IsNullOrEmpty(strToBeConverted)) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                        logger.LogError(iText.IO.Logs.IoLogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT);
                    }
                }
                GlyphLine newText = newFont.CreateGlyphLine(strToBeConverted);
                newText = TextPreprocessingUtil.ReplaceSpecialWhitespaceGlyphs(newText, newFont);
                SetProcessedGlyphLineAndFont(newText, newFont);
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

        private sealed class CustomGlyphLineFilter : GlyphLine.IGlyphLineFilter {
            public bool Accept(Glyph glyph) {
                return !NoPrint(glyph);
            }
        }
    }
}
