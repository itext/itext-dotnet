/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextRenderInfoTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/TextRenderInfoTest/";

        public const int FIRST_PAGE = 1;

        public const int FIRST_ELEMENT_INDEX = 0;

        [NUnit.Framework.Test]
        public virtual void TestCharacterRenderInfos() {
            PdfCanvasProcessor parser = new PdfCanvasProcessor(new TextRenderInfoTest.CharacterPositionEventListener()
                );
            parser.ProcessPageContent(new PdfDocument(new PdfReader(sourceFolder + "simple_text.pdf")).GetPage(FIRST_PAGE
                ));
        }

        /// <summary>
        /// Test introduced to exclude a bug related to a Unicode quirk for
        /// Japanese.
        /// </summary>
        /// <remarks>
        /// Test introduced to exclude a bug related to a Unicode quirk for
        /// Japanese. TextRenderInfo threw an AIOOBE for some characters.
        /// </remarks>
        [NUnit.Framework.Test]
        public virtual void TestUnicodeEmptyString() {
            StringBuilder sb = new StringBuilder();
            String inFile = "japanese_text.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + inFile));
            ITextExtractionStrategy start = new SimpleTextExtractionStrategy();
            sb.Append(PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(FIRST_PAGE), start));
            String result = sb.JSubstring(0, sb.ToString().IndexOf("\n", StringComparison.Ordinal));
            String origText = "\u76f4\u8fd1\u306e\u0053\uff06\u0050\u0035\u0030\u0030" + "\u914d\u5f53\u8cb4\u65cf\u6307\u6570\u306e\u30d1\u30d5"
                 + "\u30a9\u30fc\u30de\u30f3\u30b9\u306f\u0053\uff06\u0050" + "\u0035\u0030\u0030\u6307\u6570\u3092\u4e0a\u56de\u308b";
            NUnit.Framework.Assert.AreEqual(origText, result);
        }

        [NUnit.Framework.Test]
        public virtual void TestType3FontWidth() {
            String inFile = "type3font_text.pdf";
            LineSegment origLineSegment = new LineSegment(new Vector(20.3246f, 769.4974f, 1.0f), new Vector(151.22923f
                , 769.4974f, 1.0f));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + inFile));
            TextRenderInfoTest.TextPositionEventListener renderListener = new TextRenderInfoTest.TextPositionEventListener
                ();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(renderListener);
            processor.ProcessPageContent(pdfDocument.GetPage(FIRST_PAGE));
            NUnit.Framework.Assert.AreEqual(renderListener.GetLineSegments()[FIRST_ELEMENT_INDEX].GetStartPoint().Get(
                FIRST_ELEMENT_INDEX), origLineSegment.GetStartPoint().Get(FIRST_ELEMENT_INDEX), 1 / 2f);
            NUnit.Framework.Assert.AreEqual(renderListener.GetLineSegments()[FIRST_ELEMENT_INDEX].GetEndPoint().Get(FIRST_ELEMENT_INDEX
                ), origLineSegment.GetEndPoint().Get(FIRST_ELEMENT_INDEX), 1 / 2f);
        }

        private class TextPositionEventListener : IEventListener {
            internal IList<LineSegment> lineSegments = new List<LineSegment>();

            public virtual void EventOccurred(IEventData data, EventType type) {
                if (type.Equals(EventType.RENDER_TEXT)) {
                    lineSegments.Add(((TextRenderInfo)data).GetBaseline());
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_TEXT));
            }

            public virtual IList<LineSegment> GetLineSegments() {
                return lineSegments;
            }
        }

        private class CharacterPositionEventListener : ITextExtractionStrategy {
            public virtual String GetResultantText() {
                return null;
            }

            public virtual void EventOccurred(IEventData data, EventType type) {
                if (type.Equals(EventType.RENDER_TEXT)) {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    IList<TextRenderInfo> subs = renderInfo.GetCharacterRenderInfos();
                    TextRenderInfo previousCharInfo = subs[0];
                    for (int i = 1; i < subs.Count; i++) {
                        TextRenderInfo charInfo = subs[i];
                        Vector previousEndPoint = previousCharInfo.GetBaseline().GetEndPoint();
                        Vector currentStartPoint = charInfo.GetBaseline().GetStartPoint();
                        AssertVectorsEqual(charInfo.GetText(), previousEndPoint, currentStartPoint);
                        previousCharInfo = charInfo;
                    }
                }
            }

            private void AssertVectorsEqual(String message, Vector v1, Vector v2) {
                NUnit.Framework.Assert.AreEqual(v1.Get(0), v2.Get(0), 1 / 72f, message);
                NUnit.Framework.Assert.AreEqual(v1.Get(1), v2.Get(1), 1 / 72f, message);
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_TEXT));
            }
        }
    }
}
