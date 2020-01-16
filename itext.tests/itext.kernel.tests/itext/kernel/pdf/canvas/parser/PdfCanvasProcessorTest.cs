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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
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
    }
}
