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
