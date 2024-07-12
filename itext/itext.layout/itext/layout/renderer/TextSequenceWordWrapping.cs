/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using System.Text;
using iText.IO.Font.Otf;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal sealed class TextSequenceWordWrapping {
        private const float OCCUPIED_AREA_RELAYOUT_EPS = 0.0001F;

        private TextSequenceWordWrapping() {
        }

        public static bool IsTextRendererAndRequiresSpecialScriptPreLayoutProcessing(IRenderer childRenderer) {
            return childRenderer is TextRenderer && ((TextRenderer)childRenderer).GetSpecialScriptsWordBreakPoints() ==
                 null && ((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs(false) && !LineRenderer.IsChildFloating
                (childRenderer);
        }

        /// <summary>
        /// Preprocess a continuous sequence of TextRenderer containing special scripts
        /// prior to layouting the first TextRenderer in the sequence.
        /// </summary>
        /// <remarks>
        /// Preprocess a continuous sequence of TextRenderer containing special scripts
        /// prior to layouting the first TextRenderer in the sequence.
        /// <para />
        /// In this method we preprocess a sequence containing special scripts only,
        /// skipping floating renderers as they're not part of a regular layout flow,
        /// and breaking the prelayout processing once a non-special script containing renderer occurs.
        /// Note! Even though floats are skipped during calculating correct word boundaries,
        /// floats themselves are considered as soft-wrap opportunities.
        /// <para />
        /// Prelayout processing includes the following steps:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="GetSpecialScriptsContainingTextRendererSequenceInfo(LineRenderer, int)"/>
        /// : determine boundaries of the
        /// sequence
        /// and concatenate its TextRenderer#text fields converted to a String representation;
        /// </description></item>
        /// <item><description>get the String analyzed with WordWrapper#getPossibleBreaks and
        /// receive a zero-based array of points where the String is allowed to got broken in lines;
        /// </description></item>
        /// <item><description>
        /// <see cref="DistributePossibleBreakPointsOverSequentialTextRenderers(LineRenderer, int, int, System.Collections.Generic.IList{E}, System.Collections.Generic.IList{E})
        ///     "/>
        /// :
        /// distribute the list over the TextRenderer#specialScriptsWordBreakPoints, preliminarily having the points
        /// shifted,
        /// so that each TextRenderer#specialScriptsWordBreakPoints is based on the first element of TextRenderer#text.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="lineRenderer">line renderer containing text sequence to process</param>
        /// <param name="childPos">
        /// index of the childRenderer in LineRenderer#childRenderers
        /// from which the a continuous sequence of TextRenderer containing special scripts starts
        /// </param>
        public static void ProcessSpecialScriptPreLayout(LineRenderer lineRenderer, int childPos) {
            TextSequenceWordWrapping.SpecialScriptsContainingTextRendererSequenceInfo info = GetSpecialScriptsContainingTextRendererSequenceInfo
                (lineRenderer, childPos);
            int numberOfSequentialTextRenderers = info.numberOfSequentialTextRenderers;
            String sequentialTextContent = info.sequentialTextContent;
            IList<int> indicesOfFloating = info.indicesOfFloating;
            IList<int> possibleBreakPointsGlobal = TypographyUtils.GetPossibleBreaks(sequentialTextContent);
            DistributePossibleBreakPointsOverSequentialTextRenderers(lineRenderer, childPos, numberOfSequentialTextRenderers
                , possibleBreakPointsGlobal, indicesOfFloating);
        }

        public static void UpdateTextSequenceLayoutResults(IDictionary<int, LayoutResult> textRendererLayoutResults
            , bool specialScripts, IRenderer childRenderer, int childPos, LayoutResult childResult) {
            if (childRenderer is TextRenderer && ((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs(true) ==
                 specialScripts) {
                textRendererLayoutResults.Put(childPos, childResult);
            }
        }

        public static void ResetTextSequenceIfItEnded(IDictionary<int, LayoutResult> textRendererLayoutResults, bool
             specialScripts, IRenderer childRenderer, int childPos, TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper
             minMaxWidthOfTextRendererSequenceHelper, bool noSoftWrap, AbstractWidthHandler widthHandler) {
            if (childRenderer is TextRenderer && ((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs(true) ==
                 specialScripts && !LineRenderer.IsChildFloating(childRenderer)) {
                return;
            }
            if (!textRendererLayoutResults.IsEmpty()) {
                int lastChildInTextSequence = childPos;
                while (lastChildInTextSequence >= 0) {
                    if (textRendererLayoutResults.Get(lastChildInTextSequence) != null) {
                        break;
                    }
                    else {
                        lastChildInTextSequence--;
                    }
                }
                LayoutResult childResult = textRendererLayoutResults.Get(lastChildInTextSequence);
                UpdateMinMaxWidthOfLineRendererAfterTextRendererSequenceProcessing(noSoftWrap, lastChildInTextSequence, childResult
                    , widthHandler, minMaxWidthOfTextRendererSequenceHelper, textRendererLayoutResults);
                textRendererLayoutResults.Clear();
            }
        }

        public static LineRenderer.LineAscentDescentState UpdateTextRendererSequenceAscentDescent(LineRenderer lineRenderer
            , IDictionary<int, float[]> textRendererSequenceAscentDescent, int childPos, float[] childAscentDescent
            , LineRenderer.LineAscentDescentState preTextSequenceAscentDescent) {
            IRenderer childRenderer = lineRenderer.childRenderers[childPos];
            if (childRenderer is TextRenderer && !((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs(true)) {
                if (textRendererSequenceAscentDescent.IsEmpty()) {
                    preTextSequenceAscentDescent = new LineRenderer.LineAscentDescentState(lineRenderer.maxAscent, lineRenderer
                        .maxDescent, lineRenderer.maxTextAscent, lineRenderer.maxTextDescent);
                }
                textRendererSequenceAscentDescent.Put(childPos, childAscentDescent);
            }
            else {
                if (!textRendererSequenceAscentDescent.IsEmpty()) {
                    textRendererSequenceAscentDescent.Clear();
                    preTextSequenceAscentDescent = null;
                }
            }
            return preTextSequenceAscentDescent;
        }

        public static TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper UpdateTextRendererSequenceMinMaxWidth
            (LineRenderer lineRenderer, AbstractWidthHandler widthHandler, int childPos, TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper
             minMaxWidthOfTextRendererSequenceHelper, bool anythingPlaced, IDictionary<int, LayoutResult> textRendererLayoutResults
            , IDictionary<int, LayoutResult> specialScriptLayoutResults, float textIndent) {
            IRenderer childRenderer = lineRenderer.childRenderers[childPos];
            if (childRenderer is TextRenderer) {
                bool firstTextRendererWithSpecialScripts = ((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs(true
                    ) && specialScriptLayoutResults.Count == 1;
                bool firstTextRendererWithoutSpecialScripts = !((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs
                    (true) && textRendererLayoutResults.Count == 1;
                if (firstTextRendererWithoutSpecialScripts || firstTextRendererWithSpecialScripts) {
                    minMaxWidthOfTextRendererSequenceHelper = new TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper
                        (widthHandler.minMaxWidth.GetChildrenMinWidth(), textIndent, anythingPlaced);
                }
                return minMaxWidthOfTextRendererSequenceHelper;
            }
            else {
                return null;
            }
        }

        public static TextSequenceWordWrapping.LastFittingChildRendererData GetIndexAndLayoutResultOfTheLastTextRendererWithNoSpecialScripts
            (LineRenderer lineRenderer, int childPos, IDictionary<int, LayoutResult> textSequenceLayoutResults, bool
             wasParentsHeightClipped, bool isOverflowFit, bool floatsPlaced) {
            LayoutResult lastAnalyzedTextLayoutResult = textSequenceLayoutResults.Get(childPos);
            if (lastAnalyzedTextLayoutResult.GetStatus() == LayoutResult.PARTIAL && !((TextLayoutResult)lastAnalyzedTextLayoutResult
                ).IsWordHasBeenSplit()) {
                // line break has already happened based on ISplitCharacters
                return new TextSequenceWordWrapping.LastFittingChildRendererData(childPos, textSequenceLayoutResults.Get(childPos
                    ));
            }
            lastAnalyzedTextLayoutResult = null;
            int lastAnalyzedTextRenderer = childPos;
            for (int i = childPos; i >= 0; i--) {
                if (lineRenderer.childRenderers[i] is TextRenderer && !LineRenderer.IsChildFloating(lineRenderer.childRenderers
                    [i])) {
                    TextRenderer textRenderer = (TextRenderer)lineRenderer.childRenderers[i];
                    if (!textRenderer.TextContainsSpecialScriptGlyphs(true)) {
                        TextLayoutResult textLayoutResult = (TextLayoutResult)textSequenceLayoutResults.Get(i);
                        TextLayoutResult previousTextLayoutResult = (TextLayoutResult)textSequenceLayoutResults.Get(lastAnalyzedTextRenderer
                            );
                        if (i != lastAnalyzedTextRenderer && (textLayoutResult.GetStatus() == LayoutResult.FULL && (previousTextLayoutResult
                            .IsStartsWithSplitCharacterWhiteSpace() || textLayoutResult.IsEndsWithSplitCharacter()))) {
                            lastAnalyzedTextLayoutResult = previousTextLayoutResult.GetStatus() == LayoutResult.NOTHING ? previousTextLayoutResult
                                 : new TextLayoutResult(LayoutResult.NOTHING, null, null, lineRenderer.childRenderers[lastAnalyzedTextRenderer
                                ]);
                            break;
                        }
                        if (textLayoutResult.IsContainsPossibleBreak() && textLayoutResult.GetStatus() != LayoutResult.NOTHING) {
                            textRenderer.SetIndexOfFirstCharacterToBeForcedToOverflow(textRenderer.line.end);
                            LayoutArea layoutArea = textRenderer.GetOccupiedArea().Clone();
                            layoutArea.GetBBox().IncreaseHeight(OCCUPIED_AREA_RELAYOUT_EPS).IncreaseWidth(OCCUPIED_AREA_RELAYOUT_EPS);
                            // Here we relayout the child with the possible break using its own occupied area as
                            // available layout box. It's expected to always work, because since during relayout
                            // we try to achieve partial result of the original layout, the resultant occupied area
                            // will be smaller. More right approach would be to reuse the same layout box which was
                            // used for the original layouting, however it seems to be an overkill to preserve them all.
                            LayoutResult newChildLayoutResult = textRenderer.Layout(new LayoutContext(layoutArea, wasParentsHeightClipped
                                ));
                            textRenderer.SetIndexOfFirstCharacterToBeForcedToOverflow(TextRenderer.UNDEFINED_FIRST_CHAR_TO_FORCE_OVERFLOW
                                );
                            if (newChildLayoutResult.GetStatus() == LayoutResult.FULL) {
                                lastAnalyzedTextLayoutResult = new TextLayoutResult(LayoutResult.NOTHING, null, null, lineRenderer.childRenderers
                                    [lastAnalyzedTextRenderer]);
                            }
                            else {
                                lastAnalyzedTextLayoutResult = newChildLayoutResult;
                                lastAnalyzedTextRenderer = i;
                            }
                            break;
                        }
                        lastAnalyzedTextRenderer = i;
                    }
                    else {
                        lastAnalyzedTextLayoutResult = new TextLayoutResult(LayoutResult.NOTHING, null, null, lineRenderer.childRenderers
                            [lastAnalyzedTextRenderer]);
                        break;
                    }
                }
                else {
                    if (LineRenderer.IsChildFloating(lineRenderer.childRenderers[i]) || lineRenderer.childRenderers[i] is ImageRenderer
                         || LineRenderer.IsInlineBlockChild(lineRenderer.childRenderers[i])) {
                        lastAnalyzedTextLayoutResult = new TextLayoutResult(LayoutResult.NOTHING, null, null, lineRenderer.childRenderers
                            [lastAnalyzedTextRenderer]);
                        break;
                    }
                    else {
                        break;
                    }
                }
            }
            if (lastAnalyzedTextLayoutResult == null) {
                OverflowWrapPropertyValue? overflowWrapValue = lineRenderer.childRenderers[childPos].GetProperty<OverflowWrapPropertyValue?
                    >(Property.OVERFLOW_WRAP);
                bool overflowWrapNotNormal = overflowWrapValue == OverflowWrapPropertyValue.ANYWHERE || overflowWrapValue 
                    == OverflowWrapPropertyValue.BREAK_WORD;
                if (overflowWrapNotNormal && textSequenceLayoutResults.Get(lastAnalyzedTextRenderer).GetStatus() != LayoutResult
                    .NOTHING || isOverflowFit) {
                    lastAnalyzedTextRenderer = childPos;
                    lastAnalyzedTextLayoutResult = textSequenceLayoutResults.Get(lastAnalyzedTextRenderer);
                }
                else {
                    if (floatsPlaced) {
                        lastAnalyzedTextLayoutResult = new TextLayoutResult(LayoutResult.NOTHING, null, null, lineRenderer.childRenderers
                            [lastAnalyzedTextRenderer]);
                    }
                    else {
                        return null;
                    }
                }
            }
            if (lastAnalyzedTextLayoutResult != null) {
                return new TextSequenceWordWrapping.LastFittingChildRendererData(lastAnalyzedTextRenderer, lastAnalyzedTextLayoutResult
                    );
            }
            else {
                return null;
            }
        }

        public static TextSequenceWordWrapping.LastFittingChildRendererData GetIndexAndLayoutResultOfTheLastTextRendererContainingSpecialScripts
            (LineRenderer lineRenderer, int childPos, IDictionary<int, LayoutResult> specialScriptLayoutResults, bool
             wasParentsHeightClipped, bool isOverflowFit) {
            int indexOfRendererContainingLastFullyFittingWord = childPos;
            int splitPosition = 0;
            bool needToSplitRendererContainingLastFullyFittingWord = false;
            int fittingLengthWithTrailingRightSideSpaces = 0;
            int amountOfTrailingRightSideSpaces = 0;
            LayoutResult childPosLayoutResult = specialScriptLayoutResults.Get(childPos);
            LayoutResult returnLayoutResult = null;
            for (int analyzedTextRendererIndex = childPos; analyzedTextRendererIndex >= 0; analyzedTextRendererIndex--
                ) {
                // get the number of fitting glyphs in the renderer being analyzed
                TextRenderer textRenderer = (TextRenderer)lineRenderer.childRenderers[analyzedTextRendererIndex];
                if (analyzedTextRendererIndex != childPos) {
                    fittingLengthWithTrailingRightSideSpaces = textRenderer.Length();
                }
                else {
                    if (childPosLayoutResult.GetSplitRenderer() != null) {
                        TextRenderer splitTextRenderer = (TextRenderer)childPosLayoutResult.GetSplitRenderer();
                        GlyphLine splitText = splitTextRenderer.text;
                        if (splitTextRenderer.Length() > 0) {
                            fittingLengthWithTrailingRightSideSpaces = splitTextRenderer.Length();
                            while (splitText.end + amountOfTrailingRightSideSpaces < splitText.Size() && iText.IO.Util.TextUtil.IsWhitespace
                                (splitText.Get(splitText.end + amountOfTrailingRightSideSpaces))) {
                                fittingLengthWithTrailingRightSideSpaces++;
                                amountOfTrailingRightSideSpaces++;
                            }
                        }
                    }
                }
                // check if line break can happen in this renderer relying on its specialScriptsWordBreakPoints list
                if (fittingLengthWithTrailingRightSideSpaces > 0) {
                    IList<int> breakPoints = textRenderer.GetSpecialScriptsWordBreakPoints();
                    if (breakPoints != null && breakPoints.Count > 0 && breakPoints[0] != -1) {
                        int possibleBreakPointPosition = TextRenderer.FindPossibleBreaksSplitPosition(textRenderer.GetSpecialScriptsWordBreakPoints
                            (), fittingLengthWithTrailingRightSideSpaces + textRenderer.text.start, false);
                        if (possibleBreakPointPosition > -1) {
                            splitPosition = breakPoints[possibleBreakPointPosition] - amountOfTrailingRightSideSpaces;
                            needToSplitRendererContainingLastFullyFittingWord = splitPosition != textRenderer.text.end;
                            if (!needToSplitRendererContainingLastFullyFittingWord) {
                                analyzedTextRendererIndex++;
                            }
                            indexOfRendererContainingLastFullyFittingWord = analyzedTextRendererIndex;
                            break;
                        }
                    }
                }
                TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus status = GetSpecialScriptsContainingSequenceStatus
                    (lineRenderer, analyzedTextRendererIndex);
                // possible breaks haven't been found, can't move back:
                // forced split on the latter renderer having either Full or Partial result
                // if either OVERFLOW_X is FIT or OVERFLOW_WRAP is either ANYWHERE or BREAK_WORD,
                // otherwise return null as a flag to move forward across this.childRenderers
                // till the end of the unbreakable word
                if (status == TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.FORCED_SPLIT) {
                    OverflowWrapPropertyValue? overflowWrapValue = lineRenderer.childRenderers[childPos].GetProperty<OverflowWrapPropertyValue?
                        >(Property.OVERFLOW_WRAP);
                    bool overflowWrapNotNormal = overflowWrapValue == OverflowWrapPropertyValue.ANYWHERE || overflowWrapValue 
                        == OverflowWrapPropertyValue.BREAK_WORD;
                    if (overflowWrapNotNormal && childPosLayoutResult.GetStatus() != LayoutResult.NOTHING || isOverflowFit) {
                        if (childPosLayoutResult.GetStatus() != LayoutResult.NOTHING) {
                            returnLayoutResult = childPosLayoutResult;
                        }
                        indexOfRendererContainingLastFullyFittingWord = childPos;
                        break;
                    }
                    else {
                        return null;
                    }
                }
                // possible breaks haven't been found, can't move back
                // move the entire renderer on the next line
                if (status == TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.MOVE_SEQUENCE_CONTAINING_SPECIAL_SCRIPTS_ON_NEXT_LINE
                    ) {
                    indexOfRendererContainingLastFullyFittingWord = analyzedTextRendererIndex;
                    break;
                }
            }
            if (returnLayoutResult == null) {
                returnLayoutResult = childPosLayoutResult;
                TextRenderer childRenderer = (TextRenderer)lineRenderer.childRenderers[indexOfRendererContainingLastFullyFittingWord
                    ];
                if (needToSplitRendererContainingLastFullyFittingWord) {
                    int amountOfFitOnTheFirstLayout = fittingLengthWithTrailingRightSideSpaces - amountOfTrailingRightSideSpaces
                         + childRenderer.text.start;
                    if (amountOfFitOnTheFirstLayout != splitPosition) {
                        LayoutArea layoutArea = childRenderer.GetOccupiedArea().Clone();
                        layoutArea.GetBBox().IncreaseHeight(OCCUPIED_AREA_RELAYOUT_EPS).IncreaseWidth(OCCUPIED_AREA_RELAYOUT_EPS);
                        childRenderer.SetSpecialScriptFirstNotFittingIndex(splitPosition);
                        // Here we relayout the child with the possible break using its own occupied area as
                        // available layout box. It's expected to always work, because since during relayout
                        // we try to achieve partial result of the original layout, the resultant occupied area
                        // will be smaller. More right approach would be to reuse the same layout box which was
                        // used for the original layouting, however it seems to be an overkill to preserve them all.
                        returnLayoutResult = childRenderer.Layout(new LayoutContext(layoutArea, wasParentsHeightClipped));
                        childRenderer.SetSpecialScriptFirstNotFittingIndex(-1);
                    }
                }
                else {
                    returnLayoutResult = new TextLayoutResult(LayoutResult.NOTHING, null, null, childRenderer);
                }
            }
            return new TextSequenceWordWrapping.LastFittingChildRendererData(indexOfRendererContainingLastFullyFittingWord
                , returnLayoutResult);
        }

        /// <summary>
        /// Performs some settings on
        /// <see cref="LineRenderer"/>
        /// and its child prior to layouting the child
        /// to be overflowed beyond the available area.
        /// </summary>
        /// <param name="lineRenderer">line renderer containing text sequence to process</param>
        /// <param name="textSequenceOverflowXProcessing">
        /// true if it is
        /// <see cref="TextRenderer"/>
        /// sequence processing in overflowX mode
        /// </param>
        /// <param name="childRenderer">
        /// the
        /// <see cref="LineRenderer"/>
        /// 's child to be preprocessed
        /// </param>
        /// <param name="wasXOverflowChanged">
        /// true if value of
        /// <see cref="iText.Layout.Properties.Property.OVERFLOW_X"/>
        /// has been changed during
        /// layouting
        /// </param>
        /// <param name="oldXOverflow">
        /// the value of
        /// <see cref="iText.Layout.Properties.Property.OVERFLOW_X"/>
        /// before it's been changed
        /// during layouting of
        /// <see cref="LineRenderer"/>
        /// or null if
        /// <see cref="iText.Layout.Properties.Property.OVERFLOW_X"/>
        /// hasn't been changed
        /// </param>
        public static void PreprocessTextSequenceOverflowX(LineRenderer lineRenderer, bool textSequenceOverflowXProcessing
            , IRenderer childRenderer, bool wasXOverflowChanged, OverflowPropertyValue? oldXOverflow) {
            bool specialScripts = childRenderer is TextRenderer && ((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs
                (true);
            if (textSequenceOverflowXProcessing && specialScripts) {
                int firstPossibleBreakWithinTheRenderer = ((TextRenderer)childRenderer).GetSpecialScriptsWordBreakPoints()
                    [0];
                if (firstPossibleBreakWithinTheRenderer != -1) {
                    ((TextRenderer)childRenderer).SetSpecialScriptFirstNotFittingIndex(firstPossibleBreakWithinTheRenderer);
                }
                if (wasXOverflowChanged) {
                    lineRenderer.SetProperty(Property.OVERFLOW_X, oldXOverflow);
                }
            }
            if (textSequenceOverflowXProcessing && !specialScripts && wasXOverflowChanged) {
                lineRenderer.SetProperty(Property.OVERFLOW_X, oldXOverflow);
            }
        }

        /// <summary>
        /// Checks if the layouting should be stopped on current child and resets configurations set on
        /// <see cref="PreprocessTextSequenceOverflowX(LineRenderer, bool, IRenderer, bool, iText.Layout.Properties.OverflowPropertyValue?)
        ///     "/>.
        /// </summary>
        /// <param name="lineRenderer">line renderer containing text sequence to process</param>
        /// <param name="textSequenceOverflowXProcessing">
        /// true if it is
        /// <see cref="TextRenderer"/>
        /// sequence processing in overflowX mode
        /// </param>
        /// <param name="childRenderer">
        /// the
        /// <see cref="LineRenderer"/>
        /// 's child to be preprocessed
        /// </param>
        /// <param name="wasXOverflowChanged">
        /// true if value of
        /// <see cref="iText.Layout.Properties.Property.OVERFLOW_X"/>
        /// has been changed during
        /// layouting
        /// </param>
        public static bool PostprocessTextSequenceOverflowX(LineRenderer lineRenderer, bool textSequenceOverflowXProcessing
            , int childPos, IRenderer childRenderer, LayoutResult childResult, bool wasXOverflowChanged) {
            bool specialScripts = childRenderer is TextRenderer && ((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs
                (true);
            bool shouldBreakLayouting = false;
            bool lastElemOfTextSequence = childPos + 1 == lineRenderer.childRenderers.Count || LineRenderer.IsChildFloating
                (lineRenderer.childRenderers[childPos + 1]) || !(lineRenderer.childRenderers[childPos + 1] is TextRenderer
                );
            if (textSequenceOverflowXProcessing && specialScripts) {
                if (((TextRenderer)childRenderer).GetSpecialScriptFirstNotFittingIndex() > 0 || lastElemOfTextSequence) {
                    shouldBreakLayouting = true;
                }
                ((TextRenderer)childRenderer).SetSpecialScriptFirstNotFittingIndex(-1);
                if (wasXOverflowChanged) {
                    lineRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                }
            }
            if (textSequenceOverflowXProcessing && !specialScripts) {
                if ((childResult is TextLayoutResult && ((TextLayoutResult)childResult).IsContainsPossibleBreak()) || lastElemOfTextSequence
                    ) {
                    shouldBreakLayouting = true;
                }
                if (wasXOverflowChanged) {
                    lineRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                }
            }
            return shouldBreakLayouting;
        }

//\cond DO_NOT_DOCUMENT
        internal static TextSequenceWordWrapping.SpecialScriptsContainingTextRendererSequenceInfo GetSpecialScriptsContainingTextRendererSequenceInfo
            (LineRenderer lineRenderer, int childPos) {
            StringBuilder sequentialTextContentBuilder = new StringBuilder();
            int numberOfSequentialTextRenderers = 0;
            IList<int> indicesOfFloating = new List<int>();
            for (int i = childPos; i < lineRenderer.childRenderers.Count; i++) {
                if (LineRenderer.IsChildFloating(lineRenderer.childRenderers[i])) {
                    numberOfSequentialTextRenderers++;
                    indicesOfFloating.Add(i);
                }
                else {
                    if (lineRenderer.childRenderers[i] is TextRenderer && ((TextRenderer)lineRenderer.childRenderers[i]).TextContainsSpecialScriptGlyphs
                        (false)) {
                        sequentialTextContentBuilder.Append(((TextRenderer)lineRenderer.childRenderers[i]).text.ToString());
                        numberOfSequentialTextRenderers++;
                    }
                    else {
                        break;
                    }
                }
            }
            return new TextSequenceWordWrapping.SpecialScriptsContainingTextRendererSequenceInfo(numberOfSequentialTextRenderers
                , sequentialTextContentBuilder.ToString(), indicesOfFloating);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void DistributePossibleBreakPointsOverSequentialTextRenderers(LineRenderer lineRenderer, int
             childPos, int numberOfSequentialTextRenderers, IList<int> possibleBreakPointsGlobal, IList<int> indicesOfFloating
            ) {
            int alreadyProcessedNumberOfCharsWithinGlyphLines = 0;
            int indexToBeginWith = 0;
            for (int i = 0; i < numberOfSequentialTextRenderers; i++) {
                if (!indicesOfFloating.Contains(i)) {
                    TextRenderer childTextRenderer = (TextRenderer)lineRenderer.childRenderers[childPos + i];
                    IList<int> amountOfCharsBetweenTextStartAndActualTextChunk = new List<int>();
                    IList<int> glyphLineBasedIndicesOfActualTextChunkEnds = new List<int>();
                    FillActualTextChunkRelatedLists(childTextRenderer.GetText(), amountOfCharsBetweenTextStartAndActualTextChunk
                        , glyphLineBasedIndicesOfActualTextChunkEnds);
                    IList<int> possibleBreakPoints = new List<int>();
                    for (int j = indexToBeginWith; j < possibleBreakPointsGlobal.Count; j++) {
                        int shiftedBreakPoint = possibleBreakPointsGlobal[j] - alreadyProcessedNumberOfCharsWithinGlyphLines;
                        int amountOfCharsBetweenTextStartAndTextEnd = amountOfCharsBetweenTextStartAndActualTextChunk[amountOfCharsBetweenTextStartAndActualTextChunk
                            .Count - 1];
                        if (shiftedBreakPoint > amountOfCharsBetweenTextStartAndTextEnd) {
                            indexToBeginWith = j;
                            alreadyProcessedNumberOfCharsWithinGlyphLines += amountOfCharsBetweenTextStartAndTextEnd;
                            break;
                        }
                        possibleBreakPoints.Add(shiftedBreakPoint);
                    }
                    IList<int> glyphLineBasedPossibleBreakPoints = ConvertPossibleBreakPointsToGlyphLineBased(possibleBreakPoints
                        , amountOfCharsBetweenTextStartAndActualTextChunk, glyphLineBasedIndicesOfActualTextChunkEnds);
                    childTextRenderer.SetSpecialScriptsWordBreakPoints(glyphLineBasedPossibleBreakPoints);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// This method defines how to proceed with a
        /// <see cref="TextRenderer"/>
        /// within which possible breaks haven't been found.
        /// </summary>
        /// <remarks>
        /// This method defines how to proceed with a
        /// <see cref="TextRenderer"/>
        /// within which possible breaks haven't been found.
        /// Possible scenarios are:
        /// - Preceding renderer is also an instance of
        /// <see cref="TextRenderer"/>
        /// and does contain special scripts:
        /// <see cref="GetIndexAndLayoutResultOfTheLastTextRendererContainingSpecialScripts(LineRenderer, int, System.Collections.Generic.IDictionary{K, V}, bool, bool)
        ///     "/>
        /// will proceed to analyze the preceding
        /// <see cref="TextRenderer"/>
        /// on the subject of possible breaks;
        /// - Preceding renderer is either an instance of
        /// <see cref="TextRenderer"/>
        /// which does not contain special scripts,
        /// or an instance of
        /// <see cref="ImageRenderer"/>
        /// or is an inlineBlock child: in this case the entire subsequence of
        /// <see cref="TextRenderer"/>
        /// -s containing special scripts is to be moved to the next line;
        /// - Otherwise a forced split is to happen.
        /// </remarks>
        /// <param name="lineRenderer">line renderer containing text sequence to process</param>
        /// <param name="analyzedTextRendererIndex">
        /// index of the latter child
        /// that has been analyzed on the subject of possible breaks
        /// </param>
        /// <returns>
        /// 
        /// <see cref="SpecialScriptsContainingSequenceStatus"/>
        /// instance standing for the strategy to proceed with.
        /// </returns>
        internal static TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus GetSpecialScriptsContainingSequenceStatus
            (LineRenderer lineRenderer, int analyzedTextRendererIndex) {
            bool moveSequenceContainingSpecialScriptsOnNextLine = false;
            bool moveToPreviousTextRendererContainingSpecialScripts = false;
            if (analyzedTextRendererIndex > 0) {
                IRenderer prevChildRenderer = lineRenderer.childRenderers[analyzedTextRendererIndex - 1];
                if (prevChildRenderer is TextRenderer && !LineRenderer.IsChildFloating(prevChildRenderer)) {
                    if (((TextRenderer)prevChildRenderer).TextContainsSpecialScriptGlyphs(true)) {
                        moveToPreviousTextRendererContainingSpecialScripts = true;
                    }
                    else {
                        moveSequenceContainingSpecialScriptsOnNextLine = true;
                    }
                }
                else {
                    if (LineRenderer.IsChildFloating(prevChildRenderer) || prevChildRenderer is ImageRenderer || LineRenderer.
                        IsInlineBlockChild(prevChildRenderer)) {
                        moveSequenceContainingSpecialScriptsOnNextLine = true;
                    }
                }
            }
            bool forcedSplit = !moveToPreviousTextRendererContainingSpecialScripts && !moveSequenceContainingSpecialScriptsOnNextLine;
            if (moveSequenceContainingSpecialScriptsOnNextLine) {
                return TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.MOVE_SEQUENCE_CONTAINING_SPECIAL_SCRIPTS_ON_NEXT_LINE;
            }
            else {
                if (forcedSplit) {
                    return TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.FORCED_SPLIT;
                }
                else {
                    return TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.MOVE_TO_PREVIOUS_TEXT_RENDERER_CONTAINING_SPECIAL_SCRIPTS;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float GetCurWidthRelayoutedTextSequenceDecrement(int childPos, int newChildPos, IDictionary
            <int, LayoutResult> textRendererLayoutResults) {
            float decrement = 0.0f;
            // if childPos == newChildPos, curWidth doesn't include width of the current childRenderer yet,
            // so no decrement is needed
            if (childPos != newChildPos) {
                for (int i = childPos - 1; i >= newChildPos; i--) {
                    if (textRendererLayoutResults.Get(i) != null) {
                        decrement += textRendererLayoutResults.Get(i).GetOccupiedArea().GetBBox().GetWidth();
                    }
                }
            }
            return decrement;
        }
//\endcond

        private static void UpdateMinMaxWidthOfLineRendererAfterTextRendererSequenceProcessing(bool noSoftWrap, int
             childPos, LayoutResult layoutResult, AbstractWidthHandler widthHandler, TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper
             minMaxWidthOfTextRendererSequenceHelper, IDictionary<int, LayoutResult> textRendererLayoutResults) {
            if (noSoftWrap) {
                return;
            }
            TextLayoutResult currLayoutResult = (TextLayoutResult)layoutResult;
            float leftMinWidthCurrRenderer = currLayoutResult.GetLeftMinWidth();
            float generalMinWidthCurrRenderer = currLayoutResult.GetMinMaxWidth().GetMinWidth();
            float widthOfUnbreakableChunkSplitAcrossRenderers = leftMinWidthCurrRenderer;
            float minWidthOfTextRendererSequence = generalMinWidthCurrRenderer;
            for (int prevRendererIndex = childPos - 1; prevRendererIndex >= 0; prevRendererIndex--) {
                if (textRendererLayoutResults.Get(prevRendererIndex) != null) {
                    TextLayoutResult prevLayoutResult = (TextLayoutResult)textRendererLayoutResults.Get(prevRendererIndex);
                    float leftMinWidthPrevRenderer = prevLayoutResult.GetLeftMinWidth();
                    float generalMinWidthPrevRenderer = prevLayoutResult.GetMinMaxWidth().GetMinWidth();
                    float rightMinWidthPrevRenderer = prevLayoutResult.GetRightMinWidth();
                    minWidthOfTextRendererSequence = Math.Max(minWidthOfTextRendererSequence, generalMinWidthPrevRenderer);
                    if (!prevLayoutResult.IsEndsWithSplitCharacter() && !currLayoutResult.IsStartsWithSplitCharacterWhiteSpace
                        ()) {
                        if (rightMinWidthPrevRenderer > -1f) {
                            widthOfUnbreakableChunkSplitAcrossRenderers += rightMinWidthPrevRenderer;
                        }
                        else {
                            widthOfUnbreakableChunkSplitAcrossRenderers += leftMinWidthPrevRenderer;
                        }
                        minWidthOfTextRendererSequence = Math.Max(minWidthOfTextRendererSequence, widthOfUnbreakableChunkSplitAcrossRenderers
                            );
                        if (rightMinWidthPrevRenderer > -1f) {
                            widthOfUnbreakableChunkSplitAcrossRenderers = leftMinWidthPrevRenderer;
                        }
                    }
                    else {
                        widthOfUnbreakableChunkSplitAcrossRenderers = leftMinWidthPrevRenderer;
                    }
                    currLayoutResult = prevLayoutResult;
                }
            }
            if (!minMaxWidthOfTextRendererSequenceHelper.anythingPlacedBeforeTextRendererSequence) {
                widthOfUnbreakableChunkSplitAcrossRenderers += minMaxWidthOfTextRendererSequenceHelper.textIndent;
                minWidthOfTextRendererSequence = Math.Max(minWidthOfTextRendererSequence, widthOfUnbreakableChunkSplitAcrossRenderers
                    );
            }
            float lineMinWidth = Math.Max(minWidthOfTextRendererSequence, minMaxWidthOfTextRendererSequenceHelper.minWidthPreSequence
                );
            widthHandler.minMaxWidth.SetChildrenMinWidth(lineMinWidth);
        }

        private static IList<int> ConvertPossibleBreakPointsToGlyphLineBased(IList<int> possibleBreakPoints, IList
            <int> amountOfChars, IList<int> indices) {
            if (possibleBreakPoints.IsEmpty()) {
                possibleBreakPoints.Add(-1);
                return possibleBreakPoints;
            }
            else {
                IList<int> glyphLineBased = new List<int>();
                foreach (int j in possibleBreakPoints) {
                    int found = TextRenderer.FindPossibleBreaksSplitPosition(amountOfChars, j, true);
                    if (found >= 0) {
                        glyphLineBased.Add(indices[found]);
                    }
                }
                return glyphLineBased;
            }
        }

        private static void FillActualTextChunkRelatedLists(GlyphLine glyphLine, IList<int> amountOfCharsBetweenTextStartAndActualTextChunk
            , IList<int> glyphLineBasedIndicesOfActualTextChunkEnds) {
            // ActualTextChunk is either an ActualText or a single independent glyph
            ActualTextIterator actualTextIterator = new ActualTextIterator(glyphLine);
            int amountOfCharsBetweenTextStartAndCurrentActualTextStartOrGlyph = 0;
            while (actualTextIterator.HasNext()) {
                GlyphLine.GlyphLinePart part = actualTextIterator.Next();
                int amountOfCharsWithinCurrentActualTextOrGlyph = 0;
                if (part.actualText != null) {
                    amountOfCharsWithinCurrentActualTextOrGlyph = part.actualText.Length;
                    int nextAmountOfChars = amountOfCharsWithinCurrentActualTextOrGlyph + amountOfCharsBetweenTextStartAndCurrentActualTextStartOrGlyph;
                    amountOfCharsBetweenTextStartAndActualTextChunk.Add(nextAmountOfChars);
                    glyphLineBasedIndicesOfActualTextChunkEnds.Add(part.end);
                    amountOfCharsBetweenTextStartAndCurrentActualTextStartOrGlyph = nextAmountOfChars;
                }
                else {
                    for (int j = part.start; j < part.end; j++) {
                        char[] chars = glyphLine.Get(j).GetChars();
                        amountOfCharsWithinCurrentActualTextOrGlyph = chars != null ? chars.Length : 0;
                        int nextAmountOfChars = amountOfCharsWithinCurrentActualTextOrGlyph + amountOfCharsBetweenTextStartAndCurrentActualTextStartOrGlyph;
                        amountOfCharsBetweenTextStartAndActualTextChunk.Add(nextAmountOfChars);
                        glyphLineBasedIndicesOfActualTextChunkEnds.Add(j + 1);
                        amountOfCharsBetweenTextStartAndCurrentActualTextStartOrGlyph = nextAmountOfChars;
                    }
                }
            }
        }

        internal enum SpecialScriptsContainingSequenceStatus {
            MOVE_SEQUENCE_CONTAINING_SPECIAL_SCRIPTS_ON_NEXT_LINE,
            MOVE_TO_PREVIOUS_TEXT_RENDERER_CONTAINING_SPECIAL_SCRIPTS,
            FORCED_SPLIT
        }

//\cond DO_NOT_DOCUMENT
        internal class MinMaxWidthOfTextRendererSequenceHelper {
            public float minWidthPreSequence;

            public float textIndent;

            public bool anythingPlacedBeforeTextRendererSequence;

            public MinMaxWidthOfTextRendererSequenceHelper(float minWidthPreSequence, float textIndent, bool anythingPlacedBeforeTextRendererSequence
                ) {
                this.minWidthPreSequence = minWidthPreSequence;
                this.textIndent = textIndent;
                this.anythingPlacedBeforeTextRendererSequence = anythingPlacedBeforeTextRendererSequence;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class LastFittingChildRendererData {
            public int childIndex;

            public LayoutResult childLayoutResult;

            public LastFittingChildRendererData(int childIndex, LayoutResult childLayoutResult) {
                this.childIndex = childIndex;
                this.childLayoutResult = childLayoutResult;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // numberOfSequentialTextRenderers - number of sequential TextRenderers containing special scripts,
        // plus number of ignored floating renderers occurring amidst the sequence;
        // sequentialTextContent - converted to String and concatenated TextRenderer#text-s;
        // indicesOfFloating - indices of ignored floating child renderers of this LineRenderer
        internal class SpecialScriptsContainingTextRendererSequenceInfo {
            public int numberOfSequentialTextRenderers;

            public String sequentialTextContent;

//\cond DO_NOT_DOCUMENT
            internal IList<int> indicesOfFloating;
//\endcond

            public SpecialScriptsContainingTextRendererSequenceInfo(int numberOfSequentialTextRenderers, String sequentialTextContent
                , IList<int> indicesOfFloating) {
                this.numberOfSequentialTextRenderers = numberOfSequentialTextRenderers;
                this.sequentialTextContent = sequentialTextContent;
                this.indicesOfFloating = indicesOfFloating;
            }
        }
//\endcond
    }
//\endcond
}
