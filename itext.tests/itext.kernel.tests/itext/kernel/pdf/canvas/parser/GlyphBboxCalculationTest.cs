/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public class GlyphBboxCalculationTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/parser/GlyphBboxCalculationTest/";

        [NUnit.Framework.Test]
        public virtual void CheckBboxCalculationForType3FontsWithFontMatrix01() {
            String inputPdf = sourceFolder + "checkBboxCalculationForType3FontsWithFontMatrix01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputPdf));
            GlyphBboxCalculationTest.CharacterPositionEventListener listener = new GlyphBboxCalculationTest.CharacterPositionEventListener
                ();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            // font size (36) * |fontMatrix| (0.001) * glyph width (600) = 21.6
            NUnit.Framework.Assert.AreEqual(21.6, listener.glyphWith, 1e-5);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-3343")]
        public virtual void CheckBboxCalculationForType3FontsWithFontMatrix02() {
            String inputPdf = sourceFolder + "checkBboxCalculationForType3FontsWithFontMatrix02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputPdf));
            GlyphBboxCalculationTest.CharacterPositionEventListener listener = new GlyphBboxCalculationTest.CharacterPositionEventListener
                ();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            // font size (36) * |fontMatrix| (1) * glyph width (0.6) = 21.6
            NUnit.Framework.Assert.AreEqual(21.6, listener.glyphWith, 1e-5);
        }

        private class CharacterPositionEventListener : ITextExtractionStrategy {
            internal float glyphWith;

            public virtual String GetResultantText() {
                return null;
            }

            public virtual void EventOccurred(IEventData data, EventType type) {
                if (type.Equals(EventType.RENDER_TEXT)) {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    IList<TextRenderInfo> subs = renderInfo.GetCharacterRenderInfos();
                    for (int i = 0; i < subs.Count; i++) {
                        TextRenderInfo charInfo = subs[i];
                        glyphWith = charInfo.GetBaseline().GetLength();
                    }
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_TEXT));
            }
        }
    }
}
