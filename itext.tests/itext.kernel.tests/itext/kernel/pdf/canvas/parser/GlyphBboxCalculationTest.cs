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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public class GlyphBboxCalculationTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/parser/GlyphBboxCalculationTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/parser/GlyphBboxCalculationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CheckBboxCalculationForType3FontsWithFontMatrix01() {
            String inputPdf = sourceFolder + "checkBboxCalculationForType3FontsWithFontMatrix01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputPdf));
            GlyphBboxCalculationTest.CharacterPositionEventListener listener = new GlyphBboxCalculationTest.CharacterPositionEventListener
                ();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            // font size (36) * |fontMatrix| (0.001) * glyph width (600) = 21.6
            NUnit.Framework.Assert.AreEqual(21.6, listener.glyphWidth, 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void CheckBboxCalculationForType3FontsWithFontMatrix02() {
            String inputPdf = sourceFolder + "checkBboxCalculationForType3FontsWithFontMatrix02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputPdf));
            GlyphBboxCalculationTest.CharacterPositionEventListener listener = new GlyphBboxCalculationTest.CharacterPositionEventListener
                ();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            // font size (36) * |fontMatrix| (1) * glyph width (0.6) = 21.6
            NUnit.Framework.Assert.AreEqual(21.6, listener.glyphWidth, 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void Type3FontsWithIdentityFontMatrixAndMultiplier() {
            String inputPdf = sourceFolder + "type3FontsWithIdentityFontMatrixAndMultiplier.pdf";
            String outputPdf = destinationFolder + "type3FontsWithIdentityFontMatrixAndMultiplier.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputPdf), new PdfWriter(outputPdf));
            GlyphBboxCalculationTest.CharacterPositionEventListener listener = new GlyphBboxCalculationTest.CharacterPositionEventListener
                ();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            PdfPage page = pdfDocument.GetPage(1);
            Rectangle pageSize = page.GetPageSize();
            PdfCanvas pdfCanvas = new PdfCanvas(page);
            pdfCanvas.BeginText().SetFontAndSize(processor.GetGraphicsState().GetFont(), processor.GetGraphicsState().
                GetFontSize()).MoveText(pageSize.GetWidth() / 2 - 24, pageSize.GetHeight() / 2).ShowText("A").EndText(
                );
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputPdf, sourceFolder + "cmp_type3FontsWithIdentityFontMatrixAndMultiplier.pdf"
                , destinationFolder, "diff_"));
        }

        private class CharacterPositionEventListener : ITextExtractionStrategy {
            internal float glyphWidth;

            public virtual String GetResultantText() {
                return null;
            }

            public virtual void EventOccurred(IEventData data, EventType type) {
                if (type.Equals(EventType.RENDER_TEXT)) {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    IList<TextRenderInfo> subs = renderInfo.GetCharacterRenderInfos();
                    for (int i = 0; i < subs.Count; i++) {
                        TextRenderInfo charInfo = subs[i];
                        glyphWidth = charInfo.GetBaseline().GetLength();
                    }
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_TEXT));
            }
        }
    }
}
