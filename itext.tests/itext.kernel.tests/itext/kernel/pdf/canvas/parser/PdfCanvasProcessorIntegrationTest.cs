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
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCanvasProcessorIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfCanvasProcessorTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/parser/PdfCanvasProcessorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ContentStreamProcessorTest() {
            PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "tableWithImageAndText.pdf"), new PdfWriter
                (new ByteArrayOutputStream()));
            StringBuilder pageEventsLog = new StringBuilder();
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorIntegrationTest.RecordEveryHighLevelEventListener
                    (pageEventsLog));
                processor.ProcessPageContent(page);
            }
            byte[] logBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "contentStreamProcessorTest_events_log.dat"
                ));
            String expectedPageEventsLog = iText.Commons.Utils.JavaUtil.GetStringForBytes(logBytes, System.Text.Encoding
                .UTF8);
            NUnit.Framework.Assert.AreEqual(expectedPageEventsLog, pageEventsLog.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ProcessGraphicsStateResourceOperatorFillOpacityTest() {
            PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "transparentText.pdf"));
            float? expOpacity = 0.5f;
            IDictionary<String, Object> textRenderInfo = new Dictionary<String, Object>();
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorIntegrationTest.RecordEveryTextRenderEvent
                    (textRenderInfo));
                processor.ProcessPageContent(page);
            }
            NUnit.Framework.Assert.AreEqual(expOpacity, textRenderInfo.Get("FillOpacity"), "Expected fill opacity not found"
                );
        }

        [NUnit.Framework.Test]
        public virtual void ProcessGraphicsStateResourceOperatorStrokeOpacityTest() {
            PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hiddenText.pdf"));
            float? expOpacity = 0.0f;
            IDictionary<String, Object> textRenderInfo = new Dictionary<String, Object>();
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorIntegrationTest.RecordEveryTextRenderEvent
                    (textRenderInfo));
                processor.ProcessPageContent(page);
            }
            NUnit.Framework.Assert.AreEqual(expOpacity, textRenderInfo.Get("StrokeOpacity"), "Expected stroke opacity not found"
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestClosingEmptyPath() {
            String fileName = "closingEmptyPath.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + fileName));
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorIntegrationTest.NoOpEventListener
                ());
            // Assert than no exception is thrown when an empty path is handled
            NUnit.Framework.Assert.DoesNotThrow(() => processor.ProcessPageContent(document.GetPage(1)));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FAILED_TO_PROCESS_A_TRANSFORMATION_MATRIX, Count = 1)]
        public virtual void TestNoninvertibleMatrix() {
            String fileName = "noninvertibleMatrix.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + fileName));
            LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            PdfPage page = pdfDocument.GetFirstPage();
            processor.ProcessPageContent(page);
            String resultantText = strategy.GetResultantText();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual("Hello World!\nHello World!\nHello World!\nHello World! Hello World! Hello World!"
                , resultantText);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-3608: this test currently throws StackOverflowError, which cannot be caught in .NET"
            )]
        public virtual void ParseCircularReferencesInResourcesTest() {
            String fileName = "circularReferencesInResources.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + fileName))) {
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorIntegrationTest.NoOpEventListener
                    ());
                PdfPage page = pdfDocument.GetFirstPage();
                NUnit.Framework.Assert.Catch(typeof(OutOfMemoryException), () => processor.ProcessPageContent(page));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNABLE_TO_PARSE_COLOR_WITHIN_COLORSPACE)]
        public virtual void PatternColorParsingNotValidPdfTest() {
            String inputFile = SOURCE_FOLDER + "patternColorParsingNotValidPdfTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFile));
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i) {
                PdfPage page = pdfDocument.GetPage(i);
                PdfCanvasProcessorIntegrationTest.ColorParsingEventListener colorParsingEventListener = new PdfCanvasProcessorIntegrationTest.ColorParsingEventListener
                    ();
                PdfCanvasProcessor processor = new PdfCanvasProcessor(colorParsingEventListener);
                processor.ProcessPageContent(page);
                Color renderInfo = colorParsingEventListener.GetEncounteredPath().GetFillColor();
                NUnit.Framework.Assert.IsNull(renderInfo);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorParsingValidPdfTest() {
            String inputFile = SOURCE_FOLDER + "patternColorParsingValidPdfTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFile));
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i) {
                PdfPage page = pdfDocument.GetPage(i);
                PdfCanvasProcessorIntegrationTest.ColorParsingEventListener colorParsingEventListener = new PdfCanvasProcessorIntegrationTest.ColorParsingEventListener
                    ();
                PdfCanvasProcessor processor = new PdfCanvasProcessor(colorParsingEventListener);
                processor.ProcessPageContent(page);
                PathRenderInfo renderInfo = colorParsingEventListener.GetEncounteredPath();
                PdfColorSpace colorSpace = renderInfo.GetGraphicsState().GetFillColor().GetColorSpace();
                NUnit.Framework.Assert.IsTrue(colorSpace is PdfSpecialCs.Pattern);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckImageRenderInfoProcessorTest() {
            PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "tableWithImageAndText.pdf"), new PdfWriter
                (new ByteArrayOutputStream()));
            PdfPage page = document.GetPage(1);
            PdfCanvasProcessorIntegrationTest.RecordFirstImageEventListener eventListener = new PdfCanvasProcessorIntegrationTest.RecordFirstImageEventListener
                ();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(eventListener);
            processor.ProcessPageContent(page);
            // Check caught image's ImageRenderInfo
            ImageRenderInfo imageRenderInfo = eventListener.GetImageRenderInfo();
            float EPS = 0.001f;
            NUnit.Framework.Assert.IsFalse(imageRenderInfo.IsInline());
            NUnit.Framework.Assert.AreEqual(1024, imageRenderInfo.GetImage().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(768, imageRenderInfo.GetImage().GetHeight(), EPS);
            NUnit.Framework.Assert.AreEqual("/Im1", imageRenderInfo.GetImageResourceName().ToString());
            NUnit.Framework.Assert.AreEqual(new Vector(212.67f, 676.25f, 1), imageRenderInfo.GetStartPoint());
            NUnit.Framework.Assert.AreEqual(new Matrix(169.67f, 0, 0, 0, 127.25f, 0, 212.67f, 676.25f, 1), imageRenderInfo
                .GetImageCtm());
            NUnit.Framework.Assert.AreEqual(21590.508, imageRenderInfo.GetArea(), EPS);
            NUnit.Framework.Assert.IsNull(imageRenderInfo.GetColorSpaceDictionary());
            NUnit.Framework.Assert.AreEqual(1, imageRenderInfo.GetCanvasTagHierarchy().Count);
            NUnit.Framework.Assert.IsTrue(imageRenderInfo.HasMcid(5, true));
            NUnit.Framework.Assert.IsTrue(imageRenderInfo.HasMcid(5));
            NUnit.Framework.Assert.IsFalse(imageRenderInfo.HasMcid(1));
            NUnit.Framework.Assert.AreEqual(5, imageRenderInfo.GetMcid());
        }

        private class ColorParsingEventListener : IEventListener {
            private IList<IEventData> content = new List<IEventData>();

            private const String pathDataExpected = "Path data expected.";

            public virtual void EventOccurred(IEventData data, EventType type) {
                if (type.Equals(EventType.RENDER_PATH)) {
                    PathRenderInfo pathRenderInfo = (PathRenderInfo)data;
                    pathRenderInfo.PreserveGraphicsState();
                    content.Add(data);
                }
            }

//\cond DO_NOT_DOCUMENT
            /// <summary>Get the last encountered PathRenderInfo, then clears the internal buffer</summary>
            /// <returns>the PathRenderInfo object that was encountered when processing the last path rendering operation</returns>
            internal virtual PathRenderInfo GetEncounteredPath() {
                if (content.Count == 0) {
                    return null;
                }
                IEventData eventData = content[0];
                if (!(eventData is PathRenderInfo)) {
                    throw new PdfException(pathDataExpected);
                }
                content.Clear();
                return (PathRenderInfo)eventData;
            }
//\endcond

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }
        }

        private class NoOpEventListener : IEventListener {
            public virtual void EventOccurred(IEventData data, EventType type) {
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }
        }

        private class RecordFirstImageEventListener : IEventListener {
            private ImageRenderInfo imageRenderInfo = null;

//\cond DO_NOT_DOCUMENT
            internal RecordFirstImageEventListener() {
            }
//\endcond

            public virtual void EventOccurred(IEventData data, EventType type) {
                switch (type) {
                    case EventType.RENDER_IMAGE: {
                        if (imageRenderInfo == null) {
                            imageRenderInfo = (ImageRenderInfo)data;
                        }
                        break;
                    }
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }

            public virtual ImageRenderInfo GetImageRenderInfo() {
                return imageRenderInfo;
            }
        }

        private class RecordEveryHighLevelEventListener : IEventListener {
            private const String END_EVENT_OCCURRENCE = "------------------------------------";

            private StringBuilder sb;

//\cond DO_NOT_DOCUMENT
            internal RecordEveryHighLevelEventListener(StringBuilder outStream) {
                this.sb = outStream;
            }
//\endcond

            public virtual void EventOccurred(IEventData data, EventType type) {
                switch (type) {
                    case EventType.BEGIN_TEXT: {
                        sb.Append("-------- BEGIN TEXT ---------").Append("\n");
                        sb.Append(END_EVENT_OCCURRENCE).Append("\n");
                        break;
                    }

                    case EventType.RENDER_TEXT: {
                        sb.Append("-------- RENDER TEXT --------").Append("\n");
                        TextRenderInfo renderInfo = (TextRenderInfo)data;
                        sb.Append("String: ").Append(renderInfo.GetPdfString().ToUnicodeString()).Append("\n");
                        sb.Append(END_EVENT_OCCURRENCE).Append("\n");
                        break;
                    }

                    case EventType.END_TEXT: {
                        sb.Append("-------- END TEXT -----------").Append("\n");
                        sb.Append(END_EVENT_OCCURRENCE).Append("\n");
                        break;
                    }

                    case EventType.RENDER_IMAGE: {
                        sb.Append("-------- RENDER IMAGE ---------").Append("\n");
                        ImageRenderInfo imageRenderInfo = (ImageRenderInfo)data;
                        sb.Append("Image: ").Append(imageRenderInfo.GetImageResourceName()).Append("\n");
                        sb.Append(END_EVENT_OCCURRENCE).Append("\n");
                        break;
                    }

                    case EventType.RENDER_PATH: {
                        sb.Append("-------- RENDER PATH --------").Append("\n");
                        PathRenderInfo pathRenderInfo = (PathRenderInfo)data;
                        sb.Append("Operation type: ").Append(pathRenderInfo.GetOperation()).Append("\n");
                        sb.Append("Num of subpaths: ").Append(pathRenderInfo.GetPath().GetSubpaths().Count).Append("\n");
                        sb.Append(END_EVENT_OCCURRENCE).Append("\n");
                        break;
                    }

                    case EventType.CLIP_PATH_CHANGED: {
                        sb.Append("-------- CLIPPING PATH ------").Append("\n");
                        ClippingPathInfo clippingPathRenderInfo = (ClippingPathInfo)data;
                        sb.Append("Num of subpaths: ").Append(clippingPathRenderInfo.GetClippingPath().GetSubpaths().Count).Append
                            ("\n");
                        sb.Append(END_EVENT_OCCURRENCE).Append("\n");
                        break;
                    }
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }
        }

        private class RecordEveryTextRenderEvent : IEventListener {
            private IDictionary<String, Object> map;

//\cond DO_NOT_DOCUMENT
            internal RecordEveryTextRenderEvent(IDictionary<String, Object> map) {
                this.map = map;
            }
//\endcond

            public virtual void EventOccurred(IEventData data, EventType type) {
                if (data is TextRenderInfo) {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    map.Put("String", renderInfo.GetPdfString().ToUnicodeString());
                    map.Put("FillOpacity", renderInfo.GetGraphicsState().GetFillOpacity());
                    map.Put("StrokeOpacity", renderInfo.GetGraphicsState().GetStrokeOpacity());
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }
        }
    }
}
