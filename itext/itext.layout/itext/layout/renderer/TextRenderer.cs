/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.IO.Log;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Hyphenation;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Splitting;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Text"/>
    /// object. It will draw the glyphs of the textual content on the
    /// <see cref="DrawContext"/>
    /// .
    /// </summary>
    public class TextRenderer : AbstractRenderer {
        protected internal const float TEXT_SPACE_COEFF = FontProgram.UNITS_NORMALIZATION;

        private const float ITALIC_ANGLE = 0.21256f;

        private const float BOLD_SIMULATION_STROKE_COEFF = 1 / 30f;

        private const float TYPO_ASCENDER_SCALE_COEFF = 1.2f;

        protected internal float yLineOffset;

        private PdfFont font;

        protected internal GlyphLine text;

        protected internal GlyphLine line;

        protected internal String strToBeConverted;

        protected internal bool otfFeaturesApplied = false;

        protected internal float tabAnchorCharacterPosition = -1;

        protected internal IList<int[]> reversedRanges;

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
        /// <see cref="iText.Layout.Element.Text"/>
        /// .
        /// </summary>
        /// <param name="textElement">
        /// the
        /// <see cref="iText.Layout.Element.Text"/>
        /// which this object should manage
        /// </param>
        /// <param name="text">the replacement text</param>
        public TextRenderer(Text textElement, String text)
            : base(textElement) {
            //font shall be stored only during converting original string to GlyphLine
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
            LayoutArea area = layoutContext.GetArea();
            float[] margins = GetMargins();
            Rectangle layoutBox = ApplyMargins(area.GetBBox().Clone(), margins, false);
            Border[] borders = GetBorders();
            ApplyBorderBox(layoutBox, borders, false);
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), 0, 0));
            bool anythingPlaced = false;
            int currentTextPos = text.start;
            float fontSize = (float)this.GetPropertyAsFloat(Property.FONT_SIZE);
            float textRise = (float)this.GetPropertyAsFloat(Property.TEXT_RISE);
            float? characterSpacing = this.GetPropertyAsFloat(Property.CHARACTER_SPACING);
            float? wordSpacing = this.GetPropertyAsFloat(Property.WORD_SPACING);
            float hScale = (float)this.GetProperty(Property.HORIZONTAL_SCALING, (float?)1f);
            ISplitCharacters splitCharacters = this.GetProperty<ISplitCharacters>(Property.SPLIT_CHARACTERS);
            float italicSkewAddition = true.Equals(GetPropertyAsBoolean(Property.ITALIC_SIMULATION)) ? ITALIC_ANGLE * 
                fontSize : 0;
            float boldSimulationAddition = true.Equals(GetPropertyAsBoolean(Property.BOLD_SIMULATION)) ? BOLD_SIMULATION_STROKE_COEFF
                 * fontSize : 0;
            line = new GlyphLine(text);
            line.start = line.end = -1;
            float[] ascenderDescender = CalculateAscenderDescender(font);
            float ascender = ascenderDescender[0];
            float descender = ascenderDescender[1];
            float currentLineAscender = 0;
            float currentLineDescender = 0;
            float currentLineHeight = 0;
            int initialLineTextPos = currentTextPos;
            float currentLineWidth = 0;
            int previousCharPos = -1;
            char? tabAnchorCharacter = this.GetProperty<char?>(Property.TAB_ANCHOR);
            TextLayoutResult result = null;
            // true in situations like "\nHello World"
            bool isSplitForcedByImmediateNewLine = false;
            // true in situations like "Hello\nWorld"
            bool isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol = false;
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
                for (int ind = currentTextPos; ind < text.end; ind++) {
                    if (TextUtil.IsNewLine(text.Get(ind))) {
                        isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol = true;
                        firstCharacterWhichExceedsAllowedWidth = ind + 1;
                        if (currentTextPos == firstPrintPos) {
                            isSplitForcedByImmediateNewLine = true;
                            // Notice that in that case we do not need to ignore the new line symbol ('\n')
                            isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol = false;
                        }
                        break;
                    }
                    Glyph currentGlyph = text.Get(ind);
                    if (NoPrint(currentGlyph)) {
                        continue;
                    }
                    if (tabAnchorCharacter != null && tabAnchorCharacter == text.Get(ind).GetUnicode()) {
                        tabAnchorCharacterPosition = currentLineWidth + nonBreakablePartFullWidth;
                        tabAnchorCharacter = null;
                    }
                    float glyphWidth = GetCharWidth(currentGlyph, fontSize, hScale, characterSpacing, wordSpacing) / TEXT_SPACE_COEFF;
                    float xAdvance = previousCharPos != -1 ? text.Get(previousCharPos).GetXAdvance() : 0;
                    if (xAdvance != 0) {
                        xAdvance = ScaleXAdvance(xAdvance, fontSize, hScale) / TEXT_SPACE_COEFF;
                    }
                    if ((nonBreakablePartFullWidth + glyphWidth + xAdvance + italicSkewAddition + boldSimulationAddition) > layoutBox
                        .GetWidth() - currentLineWidth && firstCharacterWhichExceedsAllowedWidth == -1) {
                        firstCharacterWhichExceedsAllowedWidth = ind;
                    }
                    if (firstCharacterWhichExceedsAllowedWidth == -1) {
                        nonBreakablePartWidthWhichDoesNotExceedAllowedWidth += glyphWidth + xAdvance;
                    }
                    nonBreakablePartFullWidth += glyphWidth + xAdvance;
                    nonBreakablePartMaxAscender = Math.Max(nonBreakablePartMaxAscender, ascender);
                    nonBreakablePartMaxDescender = Math.Min(nonBreakablePartMaxDescender, descender);
                    nonBreakablePartMaxHeight = (nonBreakablePartMaxAscender - nonBreakablePartMaxDescender) * fontSize / TEXT_SPACE_COEFF
                         + textRise;
                    previousCharPos = ind;
                    if (nonBreakablePartFullWidth + italicSkewAddition + boldSimulationAddition > layoutBox.GetWidth()) {
                        // we have extracted all the information we wanted and we do not want to continue.
                        // we will have to split the word anyway.
                        break;
                    }
                    if (splitCharacters.IsSplitCharacter(text, ind) || ind + 1 == text.end || splitCharacters.IsSplitCharacter
                        (text, ind + 1) && TextUtil.IsSpaceOrWhitespace(text.Get(ind + 1))) {
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
                    anythingPlaced = true;
                }
                else {
                    // check if line height exceeds the allowed height
                    if (Math.Max(currentLineHeight, nonBreakablePartMaxHeight) > layoutBox.GetHeight()) {
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
                        HyphenationConfig hyphenationConfig = this.GetProperty<HyphenationConfig>(Property.HYPHENATION);
                        if (hyphenationConfig != null) {
                            int[] wordBounds = GetWordBoundsForHyphenation(text, currentTextPos, text.end, Math.Max(currentTextPos, firstCharacterWhichExceedsAllowedWidth
                                 - 1));
                            if (wordBounds != null) {
                                String word = text.ToUnicodeString(wordBounds[0], wordBounds[1]);
                                iText.Layout.Hyphenation.Hyphenation hyph = hyphenationConfig.Hyphenate(word);
                                if (hyph != null) {
                                    for (int i = hyph.Length() - 1; i >= 0; i--) {
                                        String pre = hyph.GetPreHyphenText(i);
                                        String pos = hyph.GetPostHyphenText(i);
                                        float currentHyphenationChoicePreTextWidth = GetGlyphLineWidth(ConvertToGlyphLine(pre + hyphenationConfig.
                                            GetHyphenSymbol()), fontSize, hScale, characterSpacing, wordSpacing);
                                        if (currentLineWidth + currentHyphenationChoicePreTextWidth + italicSkewAddition + boldSimulationAddition 
                                            <= layoutBox.GetWidth()) {
                                            hyphenationApplied = true;
                                            if (line.start == -1) {
                                                line.start = currentTextPos;
                                            }
                                            line.end = Math.Max(line.end, currentTextPos + pre.Length);
                                            GlyphLine lineCopy = line.Copy(line.start, line.end);
                                            lineCopy.Add(font.GetGlyph(hyphenationConfig.GetHyphenSymbol()));
                                            lineCopy.end++;
                                            line = lineCopy;
                                            // TODO these values are based on whole word. recalculate properly based on hyphenated part
                                            currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
                                            currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender);
                                            currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
                                            currentLineWidth += currentHyphenationChoicePreTextWidth;
                                            currentTextPos += pre.Length;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if ((nonBreakablePartFullWidth > layoutBox.GetWidth() && !anythingPlaced && !hyphenationApplied) || (isSplitForcedByImmediateNewLine
                            )) {
                            // if the word is too long for a single line we will have to split it
                            wordSplit = true;
                            if (line.start == -1) {
                                line.start = currentTextPos;
                            }
                            currentTextPos = firstCharacterWhichExceedsAllowedWidth;
                            line.end = Math.Max(line.end, firstCharacterWhichExceedsAllowedWidth);
                            if (nonBreakablePartFullWidth > layoutBox.GetWidth() && !anythingPlaced && !hyphenationApplied) {
                                currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
                                currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender);
                                currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
                                currentLineWidth += nonBreakablePartWidthWhichDoesNotExceedAllowedWidth;
                            }
                            else {
                                // process empty line (e.g. '\n')
                                currentLineAscender = ascender;
                                currentLineDescender = descender;
                                currentLineHeight = (currentLineAscender - currentLineDescender) * fontSize / TEXT_SPACE_COEFF + textRise;
                                currentLineWidth += GetCharWidth(line.Get(0), fontSize, hScale, characterSpacing, wordSpacing) / TEXT_SPACE_COEFF;
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
                if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                    ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                    ApplyMargins(occupiedArea.GetBBox(), margins, true);
                    return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this);
                }
                else {
                    isPlacingForcedWhileNothing = true;
                }
            }
            yLineOffset = currentLineAscender * fontSize / TEXT_SPACE_COEFF;
            occupiedArea.GetBBox().MoveDown(currentLineHeight);
            occupiedArea.GetBBox().SetHeight(occupiedArea.GetBBox().GetHeight() + currentLineHeight);
            occupiedArea.GetBBox().SetWidth(Math.Max(occupiedArea.GetBBox().GetWidth(), currentLineWidth));
            layoutBox.SetHeight(area.GetBBox().GetHeight() - currentLineHeight);
            occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() + italicSkewAddition + boldSimulationAddition
                );
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            ApplyMargins(occupiedArea.GetBBox(), margins, true);
            if (result == null) {
                result = new TextLayoutResult(LayoutResult.FULL, occupiedArea, null, null, isPlacingForcedWhileNothing ? this
                     : null);
            }
            else {
                iText.Layout.Renderer.TextRenderer[] split;
                if (isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol) {
                    // ignore '\n'
                    split = SplitIgnoreFirstNewLine(currentTextPos);
                }
                else {
                    split = Split(currentTextPos);
                }
                result.SetSplitForcedByNewline(isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol || isSplitForcedByImmediateNewLine
                    );
                result.SetSplitRenderer(split[0]);
                // no sense to process empty renderer
                if (split[1].text.start != split[1].text.end) {
                    result.SetOverflowRenderer(split[1]);
                }
                else {
                    // LayoutResult with partial status should have non-null overflow renderer
                    result.SetStatus(LayoutResult.FULL);
                }
            }
            return result;
        }

        public virtual void ApplyOtf() {
            UpdateFontAndText();
            UnicodeScript? script = this.GetProperty<UnicodeScript?>(Property.FONT_SCRIPT);
            if (!otfFeaturesApplied) {
                if (script == null && TypographyUtils.IsTypographyModuleInitialized()) {
                    // Try to autodetect complex script.
                    ICollection<UnicodeScript> supportedScripts = TypographyUtils.GetSupportedScripts();
                    IDictionary<UnicodeScript, int?> scriptFrequency = new Dictionary<UnicodeScript, int?>();
                    for (int i = text.start; i < text.end; i++) {
                        int unicode = text.Get(i).GetUnicode();
                        if (unicode > -1) {
                            UnicodeScript glyphScript = iText.IO.Util.UnicodeScriptUtil.Of(unicode);
                            if (scriptFrequency.ContainsKey(glyphScript)) {
                                scriptFrequency[glyphScript] = scriptFrequency.Get(glyphScript) + 1;
                            }
                            else {
                                scriptFrequency[glyphScript] = 1;
                            }
                        }
                    }
                    int? max = 0;
                    KeyValuePair<UnicodeScript, int?>? selectedEntry = null;
                    foreach (KeyValuePair<UnicodeScript, int?> entry in scriptFrequency) {
                        UnicodeScript? entryScript = entry.Key;
                        if (entry.Value > max && !UnicodeScript.COMMON.Equals(entryScript) && !UnicodeScript.UNKNOWN.Equals(entryScript
                            ) && !UnicodeScript.INHERITED.Equals(entryScript)) {
                            max = entry.Value;
                            selectedEntry = entry;
                        }
                    }
                    if (selectedEntry != null) {
                        UnicodeScript selectScript = ((KeyValuePair<UnicodeScript, int?>)selectedEntry).Key;
                        if ((selectScript == UnicodeScript.ARABIC || selectScript == UnicodeScript.HEBREW) && parent is LineRenderer
                            ) {
                            SetProperty(Property.BASE_DIRECTION, BaseDirection.DEFAULT_BIDI);
                        }
                        if (supportedScripts != null && supportedScripts.Contains(selectScript)) {
                            script = selectScript;
                        }
                    }
                }
                if (HasOtfFont() && script != null) {
                    TypographyUtils.ApplyOtfScript(font.GetFontProgram(), text, script);
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
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TextRenderer));
                logger.Error(iText.IO.LogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED);
                return;
            }
            base.Draw(drawContext);
            PdfDocument document = drawContext.GetDocument();
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            bool isArtifact = false;
            TagTreePointer tagPointer = null;
            IAccessibleElement accessibleElement = null;
            if (isTagged) {
                accessibleElement = (IAccessibleElement)GetModelElement();
                PdfName role = accessibleElement.GetRole();
                if (role != null && !PdfName.Artifact.Equals(role)) {
                    tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                    if (!tagPointer.IsElementConnectedToTag(accessibleElement)) {
                        AccessibleAttributesApplier.ApplyLayoutAttributes(accessibleElement.GetRole(), this, document);
                    }
                    tagPointer.AddTag(accessibleElement, true);
                }
                else {
                    isTagged = false;
                    if (PdfName.Artifact.Equals(role)) {
                        isArtifact = true;
                    }
                }
            }
            ApplyMargins(occupiedArea.GetBBox(), GetMargins(), false);
            ApplyBorderBox(occupiedArea.GetBBox(), false);
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            float leftBBoxX = occupiedArea.GetBBox().GetX();
            if (line.end > line.start) {
                float fontSize = (float)this.GetPropertyAsFloat(Property.FONT_SIZE);
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
                    strokeWidth = fontSize / 30;
                }
                PdfCanvas canvas = drawContext.GetCanvas();
                if (isTagged) {
                    canvas.OpenTag(tagPointer.GetTagReference());
                }
                else {
                    if (isArtifact) {
                        canvas.OpenTag(new CanvasArtifact());
                    }
                }
                BeginElementOpacityApplying(drawContext);
                canvas.SaveState().BeginText().SetFontAndSize(font, fontSize);
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
                    canvas.SetWordSpacing((float)wordSpacing);
                }
                if (horizontalScaling != null && horizontalScaling != 1) {
                    canvas.SetHorizontalScaling((float)horizontalScaling * 100);
                }
                GlyphLine.IGlyphLineFilter filter = new _IGlyphLineFilter_584();
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
                canvas.EndText().RestoreState();
                EndElementOpacityApplying(drawContext);
                if (isTagged || isArtifact) {
                    canvas.CloseTag();
                }
                Object underlines = this.GetProperty<Object>(Property.UNDERLINE);
                if (underlines is IList) {
                    foreach (Object underline in (IList)underlines) {
                        if (underline is Underline) {
                            DrawSingleUnderline((Underline)underline, fontColor, canvas, fontSize, italicSimulation ? ITALIC_ANGLE : 0
                                );
                        }
                    }
                }
                else {
                    if (underlines is Underline) {
                        DrawSingleUnderline((Underline)underlines, fontColor, canvas, fontSize, italicSimulation ? ITALIC_ANGLE : 
                            0);
                    }
                }
            }
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            ApplyBorderBox(occupiedArea.GetBBox(), true);
            ApplyMargins(occupiedArea.GetBBox(), GetMargins(), true);
            if (isTagged) {
                tagPointer.MoveToParent();
                if (isLastRendererForModelElement) {
                    tagPointer.RemoveElementConnectionToTag(accessibleElement);
                }
            }
        }

        private sealed class _IGlyphLineFilter_584 : GlyphLine.IGlyphLineFilter {
            public _IGlyphLineFilter_584() {
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
                bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
                PdfCanvas canvas = drawContext.GetCanvas();
                if (isTagged) {
                    canvas.OpenTag(new CanvasArtifact());
                }
                canvas.SaveState().SetFillColor(background.GetColor());
                canvas.Rectangle(leftBBoxX - background.GetExtraLeft(), bottomBBoxY + (float)textRise - background.GetExtraBottom
                    (), backgroundArea.GetWidth() + background.GetExtraLeft() + background.GetExtraRight(), backgroundArea
                    .GetHeight() - (float)textRise + background.GetExtraTop() + background.GetExtraBottom());
                canvas.Fill().RestoreState();
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
                while (text.start < text.end && TextUtil.IsSpaceOrWhitespace(glyph = text.Get(text.start)) && !TextUtil.IsNewLine
                    (glyph)) {
                    text.start++;
                }
            }
        }

        /// <summary>
        /// Trims any whitespace characters from the end of the rendered
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// .
        /// </summary>
        /// <returns>the amount of space in points which the text was trimmed by</returns>
        [System.ObsoleteAttribute(@"visibility will be changed to package.")]
        public virtual float TrimLast() {
            float trimmedSpace = 0;
            if (line.end <= 0) {
                return trimmedSpace;
            }
            float fontSize = (float)this.GetPropertyAsFloat(Property.FONT_SIZE);
            float? characterSpacing = this.GetPropertyAsFloat(Property.CHARACTER_SPACING);
            float? wordSpacing = this.GetPropertyAsFloat(Property.WORD_SPACING);
            float hScale = (float)this.GetPropertyAsFloat(Property.HORIZONTAL_SCALING, 1f);
            int firstNonSpaceCharIndex = line.end - 1;
            while (firstNonSpaceCharIndex >= line.start) {
                Glyph currentGlyph = line.Get(firstNonSpaceCharIndex);
                if (!TextUtil.IsSpaceOrWhitespace(currentGlyph)) {
                    break;
                }
                float currentCharWidth = GetCharWidth(currentGlyph, fontSize, hScale, characterSpacing, wordSpacing) / TEXT_SPACE_COEFF;
                float xAdvance = firstNonSpaceCharIndex > line.start ? ScaleXAdvance(line.Get(firstNonSpaceCharIndex - 1).
                    GetXAdvance(), fontSize, hScale) / TEXT_SPACE_COEFF : 0;
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
            return -(occupiedArea.GetBBox().GetHeight() - yLineOffset - (float)this.GetPropertyAsFloat(Property.TEXT_RISE
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
            return new iText.Layout.Renderer.TextRenderer((Text)modelElement, null);
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

        [System.ObsoleteAttribute(@"Use iText.IO.Util.TextUtil.IsNewLine(iText.IO.Font.Otf.Glyph) instead.")]
        protected internal static bool IsNewLine(GlyphLine text, int ind) {
            return TextUtil.IsNewLine(text.Get(ind));
        }

        internal static float[] CalculateAscenderDescender(PdfFont font) {
            FontMetrics fontMetrics = font.GetFontProgram().GetFontMetrics();
            float ascender;
            float descender;
            if (fontMetrics.GetWinAscender() == 0 || fontMetrics.GetWinDescender() == 0 || fontMetrics.GetTypoAscender
                () == fontMetrics.GetWinAscender() && fontMetrics.GetTypoDescender() == fontMetrics.GetWinDescender()) {
                ascender = fontMetrics.GetTypoAscender() * TYPO_ASCENDER_SCALE_COEFF;
                descender = fontMetrics.GetTypoDescender() * TYPO_ASCENDER_SCALE_COEFF;
            }
            else {
                ascender = fontMetrics.GetWinAscender();
                descender = fontMetrics.GetWinDescender();
            }
            return new float[] { ascender, descender };
        }

        private iText.Layout.Renderer.TextRenderer[] SplitIgnoreFirstNewLine(int currentTextPos) {
            if (text.Get(currentTextPos).GetUnicode() == '\r') {
                int next = currentTextPos + 1 < text.end ? text.Get(currentTextPos + 1).GetUnicode() : -1;
                if (next == '\n') {
                    return Split(currentTextPos + 2);
                }
                else {
                    return Split(currentTextPos + 1);
                }
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

        [Obsolete]
        protected internal virtual void DrawSingleUnderline(Underline underline, Color fontStrokeColor, PdfCanvas 
            canvas, float fontSize, float italicAngleTan) {
            DrawSingleUnderline(underline, new TransparentColor(fontStrokeColor), canvas, fontSize, italicAngleTan);
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
                canvas.MoveTo(occupiedArea.GetBBox().GetX(), underlineYPosition).LineTo(occupiedArea.GetBBox().GetX() + occupiedArea
                    .GetBBox().GetWidth() - italicWidthSubstraction, underlineYPosition).Stroke();
            }
            canvas.RestoreState();
        }

        protected internal virtual float CalculateLineWidth() {
            return GetGlyphLineWidth(line, (float)this.GetPropertyAsFloat(Property.FONT_SIZE), (float)this.GetPropertyAsFloat
                (Property.HORIZONTAL_SCALING, 1f), this.GetPropertyAsFloat(Property.CHARACTER_SPACING), this.GetPropertyAsFloat
                (Property.WORD_SPACING));
        }

        protected internal virtual IList<iText.Layout.Renderer.TextRenderer> ResolveFonts() {
            Object font = this.GetProperty<Object>(Property.FONT);
            if (font is PdfFont) {
                return JavaCollectionsUtil.SingletonList<iText.Layout.Renderer.TextRenderer>(this);
            }
            else {
                if (font is String) {
                    FontProvider provider = this.GetProperty<FontProvider>(Property.FONT_PROVIDER);
                    if (provider == null) {
                        throw new InvalidOperationException("Invalid font type. FontProvider expected. Cannot resolve font with string value"
                            );
                    }
                    IList<iText.Layout.Renderer.TextRenderer> renderers = new List<iText.Layout.Renderer.TextRenderer>();
                    FontCharacteristics fc = CreateFontCharacteristics();
                    FontSelectorStrategy strategy = provider.GetStrategy(strToBeConverted, FontFamilySplitter.SplitFontFamily(
                        (String)font), fc);
                    while (!strategy.EndOfText()) {
                        iText.Layout.Renderer.TextRenderer textRenderer = new iText.Layout.Renderer.TextRenderer(this);
                        textRenderer.SetGlyphLineAndFont(strategy.NextGlyphs(), strategy.GetCurrentFont());
                        renderers.Add(textRenderer);
                    }
                    return renderers;
                }
                else {
                    throw new InvalidOperationException("Invalid font type.");
                }
            }
        }

        internal static void UpdateRangeBasedOnRemovedCharacters(List<int> removedIds, int[] range) {
            int shift = NumberOfElementsLessThan(removedIds, range[0]);
            range[0] -= shift;
            shift = NumberOfElementsLessThanOrEqual(removedIds, range[1] - 1);
            range[1] -= shift;
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
            return TextUtil.IsNonPrintable(c);
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
                 !TextUtil.IsUni0020(text.Get(wordMiddleCharPos))) {
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
            return char.IsLetter((char)g.GetUnicode()) || char.IsDigit((char)g.GetUnicode()) || '\u00ad' == g.GetUnicode
                ();
        }

        private void UpdateFontAndText() {
            if (strToBeConverted != null) {
                font = GetPropertyAsFont(Property.FONT);
                text = ConvertToGlyphLine(strToBeConverted);
                otfFeaturesApplied = false;
                strToBeConverted = null;
            }
        }

        private void SetGlyphLineAndFont(IList<Glyph> glyphs, PdfFont font) {
            this.text = new GlyphLine(glyphs);
            this.font = font;
            this.otfFeaturesApplied = false;
            this.strToBeConverted = null;
            SetProperty(Property.FONT, font);
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
    }
}
