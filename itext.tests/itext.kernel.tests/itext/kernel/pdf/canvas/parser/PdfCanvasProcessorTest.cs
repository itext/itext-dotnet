/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.IO;
using System.Text;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public class PdfCanvasProcessorTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfCanvasProcessorTest/";

        [NUnit.Framework.Test]
        public virtual void ContentStreamProcessorTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "tableWithImageAndText.pdf"), new PdfWriter
                (new ByteArrayOutputStream()));
            StringBuilder pageEventsLog = new StringBuilder();
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorTest.RecordEveryHighLevelEventListener
                    (pageEventsLog));
                processor.ProcessPageContent(page);
            }
            byte[] logBytes = File.ReadAllBytes(Path.Combine(sourceFolder + "contentStreamProcessorTest_events_log.dat"
                ));
            String expectedPageEventsLog = iText.IO.Util.JavaUtil.GetStringForBytes(logBytes, System.Text.Encoding.UTF8
                );
            NUnit.Framework.Assert.AreEqual(expectedPageEventsLog, pageEventsLog.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ProcessGraphicsStateResourceOperatorFillOpacityTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "transparentText.pdf"));
            float? expOpacity = 0.5f;
            IDictionary<String, Object> textRenderInfo = new Dictionary<String, Object>();
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorTest.RecordEveryTextRenderEvent
                    (textRenderInfo));
                processor.ProcessPageContent(page);
            }
            NUnit.Framework.Assert.AreEqual(expOpacity, textRenderInfo.Get("FillOpacity"), "Expected fill opacity not found"
                );
        }

        [NUnit.Framework.Test]
        public virtual void ProcessGraphicsStateResourceOperatorStrokeOpacityTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "hiddenText.pdf"));
            float? expOpacity = 0.0f;
            IDictionary<String, Object> textRenderInfo = new Dictionary<String, Object>();
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorTest.RecordEveryTextRenderEvent
                    (textRenderInfo));
                processor.ProcessPageContent(page);
            }
            NUnit.Framework.Assert.AreEqual(expOpacity, textRenderInfo.Get("StrokeOpacity"), "Expected stroke opacity not found"
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestClosingEmptyPath() {
            String fileName = "closingEmptyPath.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + fileName));
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorTest.NoOpEventListener());
            // Assert than no exception is thrown when an empty path is handled
            processor.ProcessPageContent(document.GetPage(1));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FAILED_TO_PROCESS_A_TRANSFORMATION_MATRIX, Count = 1)]
        public virtual void TestNoninvertibleMatrix() {
            String fileName = "noninvertibleMatrix.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + fileName));
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
            NUnit.Framework.Assert.That(() =>  {
                String fileName = "circularReferencesInResources.pdf";
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + fileName));
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfCanvasProcessorTest.NoOpEventListener());
                PdfPage page = pdfDocument.GetFirstPage();
                processor.ProcessPageContent(page);
                pdfDocument.Close();
            }
            , NUnit.Framework.Throws.InstanceOf<OutOfMemoryException>())
;
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNABLE_TO_PARSE_COLOR_WITHIN_COLORSPACE)]
        public virtual void PatternColorParsingNotValidPdfTest() {
            String inputFile = sourceFolder + "patternColorParsingNotValidPdfTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFile));
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i) {
                PdfPage page = pdfDocument.GetPage(i);
                PdfCanvasProcessorTest.ColorParsingEventListener colorParsingEventListener = new PdfCanvasProcessorTest.ColorParsingEventListener
                    ();
                PdfCanvasProcessor processor = new PdfCanvasProcessor(colorParsingEventListener);
                processor.ProcessPageContent(page);
                Color renderInfo = colorParsingEventListener.GetEncounteredPath().GetFillColor();
                NUnit.Framework.Assert.IsNull(renderInfo);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorParsingValidPdfTest() {
            String inputFile = sourceFolder + "patternColorParsingValidPdfTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFile));
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i) {
                PdfPage page = pdfDocument.GetPage(i);
                PdfCanvasProcessorTest.ColorParsingEventListener colorParsingEventListener = new PdfCanvasProcessorTest.ColorParsingEventListener
                    ();
                PdfCanvasProcessor processor = new PdfCanvasProcessor(colorParsingEventListener);
                processor.ProcessPageContent(page);
                PathRenderInfo renderInfo = colorParsingEventListener.GetEncounteredPath();
                PdfColorSpace colorSpace = renderInfo.GetGraphicsState().GetFillColor().GetColorSpace();
                NUnit.Framework.Assert.IsTrue(colorSpace is PdfSpecialCs.Pattern);
            }
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

        private class RecordEveryHighLevelEventListener : IEventListener {
            private const String END_EVENT_OCCURRENCE = "------------------------------------";

            private StringBuilder sb;

            internal RecordEveryHighLevelEventListener(StringBuilder outStream) {
                this.sb = outStream;
            }

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

            internal RecordEveryTextRenderEvent(IDictionary<String, Object> map) {
                this.map = map;
            }

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
