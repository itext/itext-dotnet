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
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class WordWrapUnitTest : ExtendedITextTest {
        public static readonly String THAI_FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/NotoSansThai-Regular.ttf";

        public static readonly String REGULAR_FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/NotoSans-Regular.ttf";

        public static readonly String KHMER_FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/KhmerOS.ttf";

        // หากอากาศดีในวันพรุ่งนี้เราจะไปปิกนิก - one sentence, multiple words.
        public const String THAI_TEXT = "\u0E2B\u0E32\u0E01\u0E2D\u0E32\u0E01\u0E32\u0E28\u0E14\u0E35" + "\u0E43\u0E19\u0E27\u0E31\u0E19\u0E1E\u0E23\u0E38\u0E48\u0E07\u0E19\u0E35\u0E49"
             + "\u0E40\u0E23\u0E32\u0E08\u0E30\u0E44\u0E1B\u0E1B\u0E34\u0E01\u0E19\u0E34\u0E01";

        // อากาศ - one word
        public const String THAI_WORD = "\u0E2D\u0E32\u0E01\u0E32\u0E28";

        [NUnit.Framework.Test]
        public virtual void IsTextRendererAndRequiresSpecialScriptPreLayoutProcessingTest() {
            TextRenderer textRenderer = new TextRenderer(new Text(THAI_TEXT));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            textRenderer.SetText(THAI_TEXT);
            NUnit.Framework.Assert.IsTrue(TextSequenceWordWrapping.IsTextRendererAndRequiresSpecialScriptPreLayoutProcessing
                (textRenderer));
        }

        [NUnit.Framework.Test]
        public virtual void IsTextRendererAndDoesNotRequireSpecialScriptPreLayoutProcessingTest() {
            TextRenderer textRenderer = new TextRenderer(new Text(THAI_TEXT));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            textRenderer.SetText(THAI_TEXT);
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>());
            NUnit.Framework.Assert.IsFalse(TextSequenceWordWrapping.IsTextRendererAndRequiresSpecialScriptPreLayoutProcessing
                (textRenderer));
        }

        [NUnit.Framework.Test]
        public virtual void IsNotTextRenderer() {
            TabRenderer tabRenderer = new TabRenderer(new Tab());
            NUnit.Framework.Assert.IsFalse(TextSequenceWordWrapping.IsTextRendererAndRequiresSpecialScriptPreLayoutProcessing
                (tabRenderer));
        }

        [NUnit.Framework.Test]
        public virtual void SplitAndOverflowInheritSpecialScriptsWordBreakPoints() {
            String nonSpecialScriptText = "Some non-special script";
            TextRenderer textRenderer = new TextRenderer(new Text(nonSpecialScriptText));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(REGULAR_FONT, PdfEncodings.IDENTITY_H));
            textRenderer.SetText(nonSpecialScriptText);
            NUnit.Framework.Assert.IsNull(textRenderer.GetSpecialScriptsWordBreakPoints());
            TextSequenceWordWrapping.IsTextRendererAndRequiresSpecialScriptPreLayoutProcessing(textRenderer);
            NUnit.Framework.Assert.IsNotNull(textRenderer.GetSpecialScriptsWordBreakPoints());
            NUnit.Framework.Assert.IsTrue(textRenderer.GetSpecialScriptsWordBreakPoints().IsEmpty());
            // layout is needed prior to calling #split() in order to fill TextRenderer fields required to be non-null
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            textRenderer.SetParent(document.GetRenderer());
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(MinMaxWidthUtils.GetInfWidth(), AbstractRenderer.INF
                ));
            textRenderer.Layout(new LayoutContext(layoutArea));
            TextRenderer[] splitRenderers = textRenderer.Split(nonSpecialScriptText.Length / 2);
            foreach (TextRenderer split in splitRenderers) {
                NUnit.Framework.Assert.IsNotNull(split.GetSpecialScriptsWordBreakPoints());
                NUnit.Framework.Assert.IsTrue(split.GetSpecialScriptsWordBreakPoints().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoNeedToSplitTextRendererOnLineSplit() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            PdfFont pdfFont = PdfFontFactory.CreateFont(REGULAR_FONT, PdfEncodings.IDENTITY_H);
            // หากอากาศอากาศ - first 3 glyphs are an unbreakable placeholder in the first renderer so that text.start != 0;
            // the next 5 glyphs are an unbreakable part of first renderer that're supposed to fully fit on the first line,
            // the last 5 glyphs are an unbreakable part of the second renderer that could fit only partially, hence fully overflowed
            String thai = "\u0E2B\u0E32\u0E01" + THAI_WORD + THAI_WORD;
            TextRenderer textRendererFirst = new TextRenderer(new Text(""));
            textRendererFirst.SetProperty(Property.FONT, pdfFont);
            textRendererFirst.SetText(thai.JSubstring(0, 8));
            textRendererFirst.text.SetStart(3);
            textRendererFirst.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(3, 8)));
            textRendererFirst.SetParent(document.GetRenderer());
            float longestWordLength = textRendererFirst.GetMinMaxWidth().GetMaxWidth();
            TextRenderer textRendererSecond = new TextRenderer(new Text(thai.Substring(8)));
            textRendererSecond.SetProperty(Property.FONT, pdfFont);
            textRendererSecond.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(5)));
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(longestWordLength * 1.5f, AbstractRenderer.INF));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRendererFirst);
            lineRenderer.AddChild(textRendererSecond);
            LayoutResult result = lineRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, result.GetStatus());
            IRenderer splitRenderer = result.GetSplitRenderer();
            NUnit.Framework.Assert.IsNotNull(splitRenderer);
            IList<IRenderer> splitChildren = splitRenderer.GetChildRenderers();
            NUnit.Framework.Assert.IsNotNull(splitChildren);
            NUnit.Framework.Assert.AreEqual(1, splitChildren.Count);
            IRenderer overflowRenderer = result.GetOverflowRenderer();
            NUnit.Framework.Assert.IsNotNull(overflowRenderer);
            IList<IRenderer> overflowChildren = overflowRenderer.GetChildRenderers();
            NUnit.Framework.Assert.IsNotNull(overflowChildren);
            NUnit.Framework.Assert.AreEqual(1, overflowChildren.Count);
            TextRenderer splitChild = (TextRenderer)splitChildren[0];
            TextRenderer overflowChild = (TextRenderer)overflowChildren[0];
            NUnit.Framework.Assert.AreEqual(splitChild.text, overflowChild.text);
        }

        [NUnit.Framework.Test]
        public virtual void SpecialScriptPreLayoutProcessing() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            int thaiTextSplitPosition = THAI_TEXT.Length / 2;
            PdfFont font = PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H);
            TextRenderer textRendererFirstPart = new TextRenderer(new Text(THAI_TEXT.JSubstring(0, thaiTextSplitPosition
                )));
            textRendererFirstPart.SetProperty(Property.FONT, font);
            textRendererFirstPart.SetText(THAI_TEXT.JSubstring(0, thaiTextSplitPosition));
            TextRenderer textRendererSecondPart = new TextRenderer(new Text(THAI_TEXT.Substring(thaiTextSplitPosition)
                ));
            textRendererSecondPart.SetProperty(Property.FONT, font);
            textRendererSecondPart.SetText(THAI_TEXT.Substring(thaiTextSplitPosition));
            TableRenderer floatingNonTextRenderer = new TableRenderer(new Table(3));
            floatingNonTextRenderer.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            TableRenderer regularNonTextRenderer = new TableRenderer(new Table(3));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRendererFirstPart);
            lineRenderer.AddChild(floatingNonTextRenderer);
            lineRenderer.AddChild(textRendererSecondPart);
            lineRenderer.AddChild(regularNonTextRenderer);
            TextSequenceWordWrapping.SpecialScriptsContainingTextRendererSequenceInfo info = TextSequenceWordWrapping.
                GetSpecialScriptsContainingTextRendererSequenceInfo(lineRenderer, 0);
            int numberOfSequentialTextRenderers = info.numberOfSequentialTextRenderers;
            String sequentialTextContent = info.sequentialTextContent;
            IList<int> indicesOfFloating = info.indicesOfFloating;
            NUnit.Framework.Assert.AreEqual(3, numberOfSequentialTextRenderers);
            NUnit.Framework.Assert.AreEqual(THAI_TEXT, sequentialTextContent);
            NUnit.Framework.Assert.AreEqual(1, indicesOfFloating.Count);
            NUnit.Framework.Assert.AreEqual(1, (int)indicesOfFloating[0]);
            IList<int> possibleBreaks = new List<int>(JavaUtil.ArraysAsList(3, 8, 10, 12, 15, 20, 23, 26, 28, 30, 36));
            TextSequenceWordWrapping.DistributePossibleBreakPointsOverSequentialTextRenderers(lineRenderer, 0, numberOfSequentialTextRenderers
                , possibleBreaks, indicesOfFloating);
            IList<int> possibleBreaksFirstPart = textRendererFirstPart.GetSpecialScriptsWordBreakPoints();
            NUnit.Framework.Assert.IsNotNull(possibleBreaksFirstPart);
            IList<int> possibleBreaksSecondPart = textRendererSecondPart.GetSpecialScriptsWordBreakPoints();
            NUnit.Framework.Assert.IsNotNull(possibleBreaksSecondPart);
            int indexOfLastPossibleBreakInTheFirstRenderer = 4;
            IList<int> expectedPossibleBreaksFirstPart = possibleBreaks.SubList(0, indexOfLastPossibleBreakInTheFirstRenderer
                 + 1);
            IList<int> expectedPossibleBreaksSecondPart = possibleBreaks.SubList(indexOfLastPossibleBreakInTheFirstRenderer
                 + 1, possibleBreaks.Count);
            NUnit.Framework.Assert.AreEqual(expectedPossibleBreaksFirstPart, possibleBreaksFirstPart);
            for (int i = 0; i < expectedPossibleBreaksSecondPart.Count; i++) {
                expectedPossibleBreaksSecondPart[i] = expectedPossibleBreaksSecondPart[i] - thaiTextSplitPosition;
            }
            NUnit.Framework.Assert.AreEqual(expectedPossibleBreaksSecondPart, possibleBreaksSecondPart);
        }

        [NUnit.Framework.Test]
        public virtual void SpecialScriptRendererFollowedByRegularTextRendererGetSequenceInfo() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer specialScriptRenderer = new TextRenderer(new Text(THAI_TEXT));
            specialScriptRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H
                ));
            specialScriptRenderer.SetText(THAI_TEXT);
            TextRenderer nonSpecialScriptRenderer = new TextRenderer(new Text("non special"));
            nonSpecialScriptRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(REGULAR_FONT, PdfEncodings.IDENTITY_H
                ));
            nonSpecialScriptRenderer.SetText("non special");
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(specialScriptRenderer);
            lineRenderer.AddChild(nonSpecialScriptRenderer);
            TextSequenceWordWrapping.SpecialScriptsContainingTextRendererSequenceInfo info = TextSequenceWordWrapping.
                GetSpecialScriptsContainingTextRendererSequenceInfo(lineRenderer, 0);
            NUnit.Framework.Assert.AreEqual(1, info.numberOfSequentialTextRenderers);
            NUnit.Framework.Assert.AreEqual(THAI_TEXT, info.sequentialTextContent);
            NUnit.Framework.Assert.IsTrue(info.indicesOfFloating.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void OneThaiWordSplitAcrossMultipleRenderersDistributePossibleBreakPoints() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            PdfFont font = PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H);
            for (int i = 0; i < THAI_WORD.Length; i++) {
                TextRenderer textRenderer = new TextRenderer(new Text(""));
                textRenderer.SetProperty(Property.FONT, font);
                textRenderer.SetText(new String(new char[] { THAI_WORD[i] }));
                lineRenderer.AddChild(textRenderer);
            }
            IList<int> possibleBreaks = new List<int>(1);
            possibleBreaks.Add(THAI_WORD.Length);
            TextSequenceWordWrapping.DistributePossibleBreakPointsOverSequentialTextRenderers(lineRenderer, 0, THAI_WORD
                .Length, possibleBreaks, new List<int>());
            IList<IRenderer> childRenderers = lineRenderer.GetChildRenderers();
            for (int i = 0; i < THAI_WORD.Length; i++) {
                IList<int> possibleBreaksPerRenderer = ((TextRenderer)childRenderers[i]).GetSpecialScriptsWordBreakPoints(
                    );
                NUnit.Framework.Assert.IsNotNull(possibleBreaksPerRenderer);
                NUnit.Framework.Assert.AreEqual(1, possibleBreaksPerRenderer.Count);
                int breakPoint = possibleBreaksPerRenderer[0];
                if (i != THAI_WORD.Length - 1) {
                    NUnit.Framework.Assert.AreEqual(-1, breakPoint);
                }
                else {
                    NUnit.Framework.Assert.AreEqual(((TextRenderer)childRenderers[i]).Length(), breakPoint);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void OneThaiWordSplitAcrossMultipleRenderersGetIndexAndLayoutResult() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            String twoWords = THAI_WORD + "\u0E14\u0E35";
            PdfFont font = PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H);
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            for (int i = 0; i < twoWords.Length; i++) {
                TextRenderer textRenderer = new TextRenderer(new Text(""));
                textRenderer.SetProperty(Property.FONT, font);
                textRenderer.SetText(new String(new char[] { twoWords[i] }));
                if (i == THAI_WORD.Length - 1 || i == twoWords.Length - 1) {
                    textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(1)));
                }
                else {
                    textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(-1)));
                }
                lineRenderer.AddChild(textRenderer);
                LayoutArea layoutArea = new LayoutArea(1, new Rectangle(0, 0, i * 100, 100));
                if (i == twoWords.Length - 1) {
                    specialScriptLayoutResults.Put(i, new LayoutResult(LayoutResult.NOTHING, layoutArea, null, null));
                }
                else {
                    specialScriptLayoutResults.Put(i, new LayoutResult(LayoutResult.FULL, layoutArea, null, null));
                }
            }
            TextSequenceWordWrapping.LastFittingChildRendererData lastFittingChildRendererData = TextSequenceWordWrapping
                .GetIndexAndLayoutResultOfTheLastTextRendererContainingSpecialScripts(lineRenderer, THAI_WORD.Length +
                 1, specialScriptLayoutResults, false, true);
            NUnit.Framework.Assert.AreEqual(5, lastFittingChildRendererData.childIndex);
            NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, lastFittingChildRendererData.childLayoutResult.GetStatus
                ());
            NUnit.Framework.Assert.IsNull(lastFittingChildRendererData.childLayoutResult.GetOccupiedArea());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFloatsFollowedByUnfittingThaiRenderer() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            PdfFont font = PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H);
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            int indexOfThaiRenderer = 3;
            for (int i = 0; i < indexOfThaiRenderer; i++) {
                TableRenderer tableRenderer = new TableRenderer(new Table(3));
                tableRenderer.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
                lineRenderer.AddChild(tableRenderer);
            }
            TextRenderer textRenderer = new TextRenderer(new Text(""));
            textRenderer.SetProperty(Property.FONT, font);
            textRenderer.SetText(THAI_WORD);
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(THAI_WORD.Length)));
            lineRenderer.AddChild(textRenderer);
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(0, 0, 0, 100));
            specialScriptLayoutResults.Put(indexOfThaiRenderer, new LayoutResult(LayoutResult.NOTHING, layoutArea, null
                , null));
            TextSequenceWordWrapping.LastFittingChildRendererData lastFittingChildRendererData = TextSequenceWordWrapping
                .GetIndexAndLayoutResultOfTheLastTextRendererContainingSpecialScripts(lineRenderer, indexOfThaiRenderer
                , specialScriptLayoutResults, false, true);
            NUnit.Framework.Assert.AreEqual(indexOfThaiRenderer, lastFittingChildRendererData.childIndex);
            NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, lastFittingChildRendererData.childLayoutResult.GetStatus
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TrailingRightSideSpacesGetIndexAndLayoutResult() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            PdfFont font = PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H);
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            for (int i = 0; i < THAI_WORD.Length; i++) {
                TextRenderer textRenderer = new TextRenderer(new Text(""));
                textRenderer.SetProperty(Property.FONT, font);
                if (i == THAI_WORD.Length - 1) {
                    textRenderer.SetText(new String(new char[] { THAI_WORD[i], ' ', ' ', ' ' }));
                    textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(4)));
                }
                else {
                    textRenderer.SetText(new String(new char[] { THAI_WORD[i] }));
                    textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(-1)));
                }
                lineRenderer.AddChild(textRenderer);
                LayoutArea layoutArea = new LayoutArea(1, new Rectangle(0, 0, i * 100, 100));
                if (i == THAI_WORD.Length - 1) {
                    textRenderer.occupiedArea = layoutArea;
                    TextRenderer[] split = textRenderer.Split(1);
                    specialScriptLayoutResults.Put(i, new LayoutResult(LayoutResult.PARTIAL, layoutArea, split[0], split[1]));
                }
                else {
                    specialScriptLayoutResults.Put(i, new LayoutResult(LayoutResult.FULL, layoutArea, null, null));
                }
            }
            TextSequenceWordWrapping.LastFittingChildRendererData lastFittingChildRendererData = TextSequenceWordWrapping
                .GetIndexAndLayoutResultOfTheLastTextRendererContainingSpecialScripts(lineRenderer, THAI_WORD.Length -
                 1, specialScriptLayoutResults, false, true);
            NUnit.Framework.Assert.AreEqual(THAI_WORD.Length - 1, lastFittingChildRendererData.childIndex);
            NUnit.Framework.Assert.AreEqual(specialScriptLayoutResults.Get(THAI_WORD.Length - 1), lastFittingChildRendererData
                .childLayoutResult);
        }

        [NUnit.Framework.Test]
        public virtual void MinMaxWidthWithOneRenderer() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRenderer = new TextRenderer(new Text(""));
            textRenderer.SetParent(document.GetRenderer());
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            textRenderer.SetText(THAI_TEXT);
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(3, 8, 10, 12, 15, 20, 23
                , 26, 28, 30, 36)));
            MinMaxWidth minMaxWidth = textRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.IsTrue(minMaxWidth.GetMinWidth() < minMaxWidth.GetMaxWidth());
        }

        [NUnit.Framework.Test]
        public virtual void SpecialScriptsWordBreakPointsSplit() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRenderer = new TextRenderer(new Text(""));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            textRenderer.SetText(THAI_TEXT);
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(3, 8, 10, 12, 15, 20, 23
                , 26, 28, 30, 36)));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRenderer);
            MinMaxWidth minMaxWidth = lineRenderer.GetMinMaxWidth();
            float width = minMaxWidth.GetMinWidth() + minMaxWidth.GetMaxWidth() / 2;
            LayoutResult layoutResult = lineRenderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, 500
                ))));
            IRenderer lineSplitRenderer = layoutResult.GetSplitRenderer();
            NUnit.Framework.Assert.IsNotNull(lineSplitRenderer);
            NUnit.Framework.Assert.IsNotNull(lineSplitRenderer.GetChildRenderers());
            NUnit.Framework.Assert.IsTrue(lineSplitRenderer.GetChildRenderers()[0] is TextRenderer);
            TextRenderer textSplitRenderer = (TextRenderer)lineSplitRenderer.GetChildRenderers()[0];
            NUnit.Framework.Assert.IsNotNull(textSplitRenderer.GetSpecialScriptsWordBreakPoints());
            IRenderer lineOverflowRenderer = layoutResult.GetOverflowRenderer();
            NUnit.Framework.Assert.IsNotNull(lineOverflowRenderer);
            NUnit.Framework.Assert.IsNotNull(lineOverflowRenderer.GetChildRenderers());
            NUnit.Framework.Assert.IsTrue(lineOverflowRenderer.GetChildRenderers()[0] is TextRenderer);
            TextRenderer textOverflowRenderer = (TextRenderer)lineOverflowRenderer.GetChildRenderers()[0];
            NUnit.Framework.Assert.IsNotNull(textOverflowRenderer.GetSpecialScriptsWordBreakPoints());
            int textSplitRendererTextLength = textSplitRenderer.text.ToString().Length;
            foreach (int specialScriptsWordBreakPoint in textSplitRenderer.GetSpecialScriptsWordBreakPoints()) {
                NUnit.Framework.Assert.IsTrue(specialScriptsWordBreakPoint <= textSplitRendererTextLength);
            }
            foreach (int specialScriptsWordBreakPoint in textOverflowRenderer.GetSpecialScriptsWordBreakPoints()) {
                NUnit.Framework.Assert.IsTrue(specialScriptsWordBreakPoint > textSplitRendererTextLength && specialScriptsWordBreakPoint
                     <= textOverflowRenderer.text.Size());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ForcedSplitOnTooNarrowArea() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRenderer = new TextRenderer(new Text(THAI_WORD));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(5)));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRenderer);
            float minWidth = lineRenderer.GetMinMaxWidth().GetMinWidth();
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(minWidth / 2, 100));
            LayoutResult layoutResult = lineRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, layoutResult.GetStatus());
        }

        [NUnit.Framework.Test]
        public virtual void MidWordSplitPartialLayoutResult() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRenderer = new TextRenderer(new Text(THAI_WORD));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            List<int> specialScriptsWordBreakPoints = new List<int>();
            specialScriptsWordBreakPoints.Add(5);
            textRenderer.SetSpecialScriptsWordBreakPoints(specialScriptsWordBreakPoints);
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRenderer);
            float minWidth = lineRenderer.GetMinMaxWidth().GetMinWidth();
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(minWidth / 2, 100));
            LayoutResult layoutResult = lineRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, layoutResult.GetStatus());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRenderers() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            document.SetFont(PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            List<int> possibleBreaks = new List<int>(JavaUtil.ArraysAsList(3, 8, 10, 12, 15, 20, 23, 26, 28, 30, 36));
            TextRenderer textRenderer = new TextRenderer(new Text(THAI_TEXT));
            textRenderer.SetSpecialScriptsWordBreakPoints(possibleBreaks);
            LineRenderer lineRendererWithOneChild = new LineRenderer();
            lineRendererWithOneChild.SetParent(document.GetRenderer());
            lineRendererWithOneChild.AddChild(textRenderer);
            float maxWidth = lineRendererWithOneChild.GetMinMaxWidth().GetMaxWidth();
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(maxWidth / 2, 100));
            LayoutResult layoutResultSingleTextRenderer = lineRendererWithOneChild.Layout(new LayoutContext(layoutArea
                ));
            IRenderer splitRendererOneChild = layoutResultSingleTextRenderer.GetSplitRenderer();
            NUnit.Framework.Assert.IsNotNull(splitRendererOneChild);
            NUnit.Framework.Assert.AreEqual(1, splitRendererOneChild.GetChildRenderers().Count);
            String splitTextOneChild = ((TextRenderer)splitRendererOneChild.GetChildRenderers()[0]).text.ToString();
            IRenderer overflowRendererOneChild = layoutResultSingleTextRenderer.GetOverflowRenderer();
            NUnit.Framework.Assert.IsNotNull(overflowRendererOneChild);
            NUnit.Framework.Assert.AreEqual(1, overflowRendererOneChild.GetChildRenderers().Count);
            String overflowTextOneChild = ((TextRenderer)overflowRendererOneChild.GetChildRenderers()[0]).text.ToString
                ();
            LineRenderer lineRendererMultipleChildren = new LineRenderer();
            lineRendererMultipleChildren.SetParent(document.GetRenderer());
            for (int i = 0; i < THAI_TEXT.Length; i++) {
                TextRenderer oneGlyphRenderer = new TextRenderer(new Text(new String(new char[] { THAI_TEXT[i] })));
                IList<int> specialScriptsWordBreakPoints = new List<int>();
                if (possibleBreaks.Contains(i)) {
                    specialScriptsWordBreakPoints.Add(i);
                }
                oneGlyphRenderer.SetSpecialScriptsWordBreakPoints(specialScriptsWordBreakPoints);
                lineRendererMultipleChildren.AddChild(oneGlyphRenderer);
            }
            LayoutResult layoutResultMultipleTextRenderers = lineRendererMultipleChildren.Layout(new LayoutContext(layoutArea
                ));
            IRenderer splitRendererMultipleChildren = layoutResultMultipleTextRenderers.GetSplitRenderer();
            NUnit.Framework.Assert.IsNotNull(splitRendererMultipleChildren);
            NUnit.Framework.Assert.IsTrue(splitRendererMultipleChildren.GetChildRenderers().Count > 0);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (IRenderer childRenderer in splitRendererMultipleChildren.GetChildRenderers()) {
                stringBuilder.Append(((TextRenderer)childRenderer).text.ToString());
            }
            String splitTextMultipleChildren = stringBuilder.ToString();
            IRenderer overflowRendererMultipleChildren = layoutResultMultipleTextRenderers.GetOverflowRenderer();
            NUnit.Framework.Assert.IsNotNull(overflowRendererMultipleChildren);
            NUnit.Framework.Assert.IsTrue(overflowRendererMultipleChildren.GetChildRenderers().Count > 0);
            stringBuilder.Length = 0;
            foreach (IRenderer childRenderer in overflowRendererMultipleChildren.GetChildRenderers()) {
                stringBuilder.Append(((TextRenderer)childRenderer).text.ToString());
            }
            String overflowTextMultipleChildren = stringBuilder.ToString();
            NUnit.Framework.Assert.AreEqual(splitTextOneChild, splitTextMultipleChildren);
            NUnit.Framework.Assert.AreEqual(overflowTextOneChild, overflowTextMultipleChildren);
        }

        [NUnit.Framework.Test]
        public virtual void WordWrappingUnavailableWithNoCalligraph() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text(THAI_TEXT));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRenderer);
            float maxWidth = lineRenderer.GetMinMaxWidth().GetMaxWidth();
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(maxWidth / 2, 100));
            lineRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.IsNull(((TextRenderer)lineRenderer.GetChildRenderers()[0]).GetSpecialScriptsWordBreakPoints
                ());
        }

        [NUnit.Framework.Test]
        public virtual void NothingLayoutResult() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text(THAI_TEXT));
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(3, 8, 10, 12, 15, 20, 23
                , 26, 28, 30, 36)));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRenderer);
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(MinMaxWidthUtils.GetInfWidth(), 10000));
            Rectangle occupiedArea = lineRenderer.Layout(new LayoutContext(layoutArea)).GetOccupiedArea().GetBBox();
            LayoutArea decreasedHeightLayoutArea = new LayoutArea(1, new Rectangle(occupiedArea.GetWidth(), occupiedArea
                .GetHeight() - 1));
            LayoutResult nothingExpected = lineRenderer.Layout(new LayoutContext(decreasedHeightLayoutArea));
            NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, nothingExpected.GetStatus());
        }

        [NUnit.Framework.Test]
        public virtual void ResetTextSequenceLayoutResultsBecauseOfNonTextRenderer() {
            IDictionary<int, LayoutResult> textRendererLayoutResults = new Dictionary<int, LayoutResult>();
            TextLayoutResult res = new TextLayoutResult(LayoutResult.NOTHING, new LayoutArea(0, new Rectangle(0, 0, 10
                , 10)), null, null, null);
            textRendererLayoutResults.Put(0, res);
            TabRenderer tabRenderer = new TabRenderer(new Tab());
            TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper minMaxWidthOfTextRendererSequenceHelper = 
                new TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper(0f, 0f, false);
            AbstractWidthHandler widthHandler = new MaxSumWidthHandler(new MinMaxWidth());
            TextSequenceWordWrapping.ResetTextSequenceIfItEnded(textRendererLayoutResults, false, tabRenderer, 1, minMaxWidthOfTextRendererSequenceHelper
                , false, widthHandler);
            NUnit.Framework.Assert.IsTrue(textRendererLayoutResults.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void ResetTextSequenceLayoutResultsBecauseOfFloatingRenderer() {
            IDictionary<int, LayoutResult> textRendererLayoutResults = new Dictionary<int, LayoutResult>();
            TextLayoutResult res = new TextLayoutResult(LayoutResult.NOTHING, new LayoutArea(0, new Rectangle(0, 0, 10
                , 10)), null, null, null);
            int childPosAlreadyAdded = 0;
            textRendererLayoutResults.Put(childPosAlreadyAdded, res);
            iText.Layout.Element.Text text = new iText.Layout.Element.Text("float");
            text.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            TextRenderer tabRenderer = new TextRenderer(text);
            TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper minMaxWidthOfTextRendererSequenceHelper = 
                new TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper(0f, 0f, false);
            AbstractWidthHandler widthHandler = new MaxSumWidthHandler(new MinMaxWidth());
            int childPosDuringResetAttempt = 1;
            TextSequenceWordWrapping.ResetTextSequenceIfItEnded(textRendererLayoutResults, false, tabRenderer, childPosDuringResetAttempt
                , minMaxWidthOfTextRendererSequenceHelper, true, widthHandler);
            NUnit.Framework.Assert.IsTrue(textRendererLayoutResults.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void UpdateSpecialScriptLayoutResultsNonTextRenderer() {
            IDictionary<int, LayoutResult> textRendererLayoutResults = new Dictionary<int, LayoutResult>();
            Tab tab = new Tab();
            TabRenderer tabRenderer = new TabRenderer(tab);
            int childPosNotToBeAdded = 1;
            TextSequenceWordWrapping.UpdateTextSequenceLayoutResults(textRendererLayoutResults, true, tabRenderer, childPosNotToBeAdded
                , new LayoutResult(LayoutResult.FULL, new LayoutArea(1, new Rectangle(10, 10)), null, null, null));
            NUnit.Framework.Assert.IsTrue(textRendererLayoutResults.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void ResetSpecialScriptTextSequenceBecauseOfTextRendererWithNoSpecialScripts() {
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            LayoutResult res = new LayoutResult(LayoutResult.NOTHING, new LayoutArea(0, new Rectangle(0, 0, 10, 10)), 
                null, null);
            specialScriptLayoutResults.Put(0, res);
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text("whatever"));
            TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper minMaxWidthOfTextRendererSequenceHelper = 
                new TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper(0f, 0f, false);
            AbstractWidthHandler widthHandler = new MaxSumWidthHandler(new MinMaxWidth());
            TextSequenceWordWrapping.ResetTextSequenceIfItEnded(specialScriptLayoutResults, true, textRenderer, 1, minMaxWidthOfTextRendererSequenceHelper
                , true, widthHandler);
            NUnit.Framework.Assert.IsTrue(specialScriptLayoutResults.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void UpdateSpecialScriptLayoutResultsTextRendererWithNoSpecialScripts() {
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text("whatever"));
            LayoutResult res = new LayoutResult(LayoutResult.NOTHING, new LayoutArea(0, new Rectangle(0, 0, 10, 10)), 
                null, null);
            TextSequenceWordWrapping.UpdateTextSequenceLayoutResults(specialScriptLayoutResults, true, textRenderer, 1
                , res);
            NUnit.Framework.Assert.IsTrue(specialScriptLayoutResults.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void NotResetSpecialScriptTextSequenceBecauseOfTextRendererWithSpecialScripts() {
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            LayoutResult res = new LayoutResult(LayoutResult.NOTHING, new LayoutArea(0, new Rectangle(0, 0, 10, 10)), 
                null, null);
            int firstKey = 0;
            specialScriptLayoutResults.Put(firstKey, res);
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text("whatever"));
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaCollectionsUtil.SingletonList(-1)));
            TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper minMaxWidthOfTextRendererSequenceHelper = 
                new TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper(0f, 0f, false);
            AbstractWidthHandler widthHandler = new MaxSumWidthHandler(new MinMaxWidth());
            int secondKey = firstKey + 1;
            TextSequenceWordWrapping.ResetTextSequenceIfItEnded(specialScriptLayoutResults, true, textRenderer, secondKey
                , minMaxWidthOfTextRendererSequenceHelper, true, widthHandler);
            NUnit.Framework.Assert.AreEqual(1, specialScriptLayoutResults.Count);
            NUnit.Framework.Assert.IsTrue(specialScriptLayoutResults.ContainsKey(firstKey));
        }

        [NUnit.Framework.Test]
        public virtual void UpdateSpecialScriptLayoutResultsTextRendererWithSpecialScripts() {
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            LayoutResult res = new LayoutResult(LayoutResult.NOTHING, new LayoutArea(0, new Rectangle(0, 0, 10, 10)), 
                null, null);
            int firstKey = 0;
            specialScriptLayoutResults.Put(firstKey, res);
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text("whatever"));
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaCollectionsUtil.SingletonList(-1)));
            int secondKey = firstKey + 1;
            TextSequenceWordWrapping.UpdateTextSequenceLayoutResults(specialScriptLayoutResults, true, textRenderer, secondKey
                , res);
            NUnit.Framework.Assert.IsTrue(specialScriptLayoutResults.ContainsKey(firstKey));
            NUnit.Framework.Assert.IsTrue(specialScriptLayoutResults.ContainsKey(secondKey));
            NUnit.Framework.Assert.AreEqual(2, specialScriptLayoutResults.Count);
        }

        [NUnit.Framework.Test]
        public virtual void CurWidthZeroDecrement() {
            int oldNewChildPos = 1;
            float decrement = TextSequenceWordWrapping.GetCurWidthRelayoutedTextSequenceDecrement(oldNewChildPos, oldNewChildPos
                , new Dictionary<int, LayoutResult>());
            NUnit.Framework.Assert.AreEqual(0.0f, decrement, 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void CurWidthLayoutResultNothing() {
            float widthOfNewNothingResult = 500;
            LayoutArea occupiedArea = new LayoutArea(1, new Rectangle(0, 0, widthOfNewNothingResult, 0));
            LayoutResult oldResult = new LayoutResult(LayoutResult.FULL, occupiedArea, null, null);
            float simpleWidth = 200;
            LayoutResult simpleDecrement = new LayoutResult(LayoutResult.FULL, new LayoutArea(1, new Rectangle(0, 0, simpleWidth
                , 0)), null, null);
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            specialScriptLayoutResults.Put(0, oldResult);
            // leave specialScriptLayoutResults.get(1) null, as if childRenderers.get(1) is floating
            specialScriptLayoutResults.Put(2, simpleDecrement);
            float decrement = TextSequenceWordWrapping.GetCurWidthRelayoutedTextSequenceDecrement(3, 0, specialScriptLayoutResults
                );
            NUnit.Framework.Assert.AreEqual(widthOfNewNothingResult + simpleWidth, decrement, 0.00001);
        }

        [NUnit.Framework.Test]
        public virtual void CurWidthLayoutResultPartial() {
            float widthOfNewPartialResult = 500;
            LayoutArea oldOccupiedArea = new LayoutArea(1, new Rectangle(0, 0, widthOfNewPartialResult, 0));
            LayoutResult oldResult = new LayoutResult(LayoutResult.FULL, oldOccupiedArea, null, null);
            float simpleWidth = 200;
            LayoutResult simpleDecrement = new LayoutResult(LayoutResult.FULL, new LayoutArea(1, new Rectangle(0, 0, simpleWidth
                , 0)), null, null);
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            specialScriptLayoutResults.Put(0, oldResult);
            // leave specialScriptLayoutResults.get(1) null, as if childRenderers.get(1) is floating
            specialScriptLayoutResults.Put(2, simpleDecrement);
            float decrement = TextSequenceWordWrapping.GetCurWidthRelayoutedTextSequenceDecrement(3, 0, specialScriptLayoutResults
                );
            NUnit.Framework.Assert.AreEqual(widthOfNewPartialResult + simpleWidth, decrement, 0.00001);
        }

        [NUnit.Framework.Test]
        public virtual void PossibleBreakWithinActualText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text(""));
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(new Glyph(629, 378, new char[] { '\u17c3' }));
            glyphs.Add(new Glyph(578, 756, new char[] { '\u1790' }));
            glyphs.Add(new Glyph(386, 0, new char[] { '\u17d2', '\u1784' }));
            glyphs.Add(new Glyph(627, 378, new char[] { '\u17c1' }));
            glyphs.Add(new Glyph(581, 756, new char[] { '\u1793' }));
            glyphs.Add(new Glyph(633, 512, new char[] { '\u17c7' }));
            GlyphLine glyphLine = new GlyphLine(glyphs);
            glyphLine.SetActualText(0, 3, "\u1790\u17d2\u1784\u17c3");
            glyphLine.SetActualText(3, 6, "\u1793\u17c1\u17c7");
            textRenderer.SetText(glyphLine, PdfFontFactory.CreateFont(KHMER_FONT, PdfEncodings.IDENTITY_H));
            lineRenderer.AddChild(textRenderer);
            IList<int> possibleBreakPoints = new List<int>(JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6, 7));
            TextSequenceWordWrapping.DistributePossibleBreakPointsOverSequentialTextRenderers(lineRenderer, 0, 1, possibleBreakPoints
                , new List<int>());
            IList<int> distributed = ((TextRenderer)lineRenderer.GetChildRenderers()[0]).GetSpecialScriptsWordBreakPoints
                ();
            NUnit.Framework.Assert.AreEqual(new List<int>(JavaUtil.ArraysAsList(3, 6)), distributed);
        }

        [NUnit.Framework.Test]
        public virtual void TrimFirstOnePossibleBreak() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            PdfFont pdfFont = PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H);
            // " อากาศ"
            String thai = "\u0020" + THAI_WORD;
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text(""));
            textRenderer.SetProperty(Property.FONT, pdfFont);
            textRenderer.SetText(thai);
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(1)));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRenderer);
            lineRenderer.TrimFirst();
            TextRenderer childTextRenderer = (TextRenderer)lineRenderer.GetChildRenderers()[0];
            NUnit.Framework.Assert.IsNotNull(childTextRenderer.GetSpecialScriptsWordBreakPoints());
            NUnit.Framework.Assert.AreEqual(1, childTextRenderer.GetSpecialScriptsWordBreakPoints().Count);
            NUnit.Framework.Assert.AreEqual(-1, (int)childTextRenderer.GetSpecialScriptsWordBreakPoints()[0]);
        }

        [NUnit.Framework.Test]
        public virtual void UnfittingSequenceWithPrecedingTextRendererContainingNoSpecialScripts() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer thaiTextRenderer = new TextRenderer(new iText.Layout.Element.Text(""));
            thaiTextRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            thaiTextRenderer.SetText(THAI_WORD);
            thaiTextRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(5)));
            TextRenderer nonThaiTextRenderer = new TextRenderer(new iText.Layout.Element.Text("."));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(nonThaiTextRenderer);
            lineRenderer.AddChild(thaiTextRenderer);
            TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus status = TextSequenceWordWrapping.GetSpecialScriptsContainingSequenceStatus
                (lineRenderer, 1);
            NUnit.Framework.Assert.AreEqual(TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.MOVE_SEQUENCE_CONTAINING_SPECIAL_SCRIPTS_ON_NEXT_LINE
                , status);
        }

        [NUnit.Framework.Test]
        public virtual void UnfittingSequenceWithPrecedingInlineBlockRenderer() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer thaiTextRenderer = new TextRenderer(new iText.Layout.Element.Text(""));
            thaiTextRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            thaiTextRenderer.SetText(THAI_WORD);
            thaiTextRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(5)));
            TableRenderer inlineBlock = new TableRenderer(new Table(3));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(inlineBlock);
            lineRenderer.AddChild(thaiTextRenderer);
            TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus status = TextSequenceWordWrapping.GetSpecialScriptsContainingSequenceStatus
                (lineRenderer, 1);
            NUnit.Framework.Assert.AreEqual(TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.MOVE_SEQUENCE_CONTAINING_SPECIAL_SCRIPTS_ON_NEXT_LINE
                , status);
        }

        [NUnit.Framework.Test]
        public virtual void UnfittingSingleTextRendererContainingSpecialScripts() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer thaiTextRenderer = new TextRenderer(new iText.Layout.Element.Text(""));
            thaiTextRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            thaiTextRenderer.SetText(THAI_WORD);
            thaiTextRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(5)));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(thaiTextRenderer);
            TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus status = TextSequenceWordWrapping.GetSpecialScriptsContainingSequenceStatus
                (lineRenderer, 0);
            NUnit.Framework.Assert.AreEqual(TextSequenceWordWrapping.SpecialScriptsContainingSequenceStatus.FORCED_SPLIT
                , status);
        }

        [NUnit.Framework.Test]
        public virtual void OverflowXSingleWordSingleRenderer() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRenderer = new TextRenderer(new iText.Layout.Element.Text(""));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
            textRenderer.SetText(THAI_WORD);
            textRenderer.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(5)));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.AddChild(textRenderer);
            float minWidth = lineRenderer.GetMinMaxWidth().GetMinWidth();
            lineRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(minWidth / 2, 100));
            LayoutResult layoutResult = lineRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, layoutResult.GetStatus());
        }

        [NUnit.Framework.Test]
        public virtual void OverflowXSingleWordOneGlyphPerTextRenderer() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            TextRenderer textRendererForMinMaxWidth = new TextRenderer(new iText.Layout.Element.Text(THAI_WORD));
            textRendererForMinMaxWidth.SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H
                ));
            textRendererForMinMaxWidth.SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(5)));
            textRendererForMinMaxWidth.SetParent(document.GetRenderer());
            float minWidth = textRendererForMinMaxWidth.GetMinMaxWidth().GetMinWidth();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            TextRenderer[] textRenderers = new TextRenderer[THAI_WORD.Length];
            for (int i = 0; i < textRenderers.Length; i++) {
                textRenderers[i] = new TextRenderer(new iText.Layout.Element.Text(""));
                textRenderers[i].SetProperty(Property.FONT, PdfFontFactory.CreateFont(THAI_FONT, PdfEncodings.IDENTITY_H));
                textRenderers[i].SetText(new String(new char[] { THAI_WORD[i] }));
                textRenderers[i].SetSpecialScriptsWordBreakPoints(new List<int>(JavaUtil.ArraysAsList(i + 1 != textRenderers
                    .Length ? -1 : 1)));
                lineRenderer.AddChild(textRenderers[i]);
            }
            lineRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(minWidth / 2, 100));
            LayoutResult layoutResult = lineRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, layoutResult.GetStatus());
        }
    }
}
