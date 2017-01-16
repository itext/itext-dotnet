using System;
using System.Collections.Generic;
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

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR, Count = 1)]
        public virtual void ContentStreamProcessorTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "yaxiststar.pdf"), new PdfWriter(new ByteArrayOutputStream
                ()));
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new _IEventListener_40());
                processor.ProcessPageContent(page);
            }
        }

        private sealed class _IEventListener_40 : IEventListener {
            public _IEventListener_40() {
            }

            public void EventOccurred(IEventData data, EventType type) {
                switch (type) {
                    case EventType.BEGIN_TEXT: {
                        System.Console.Out.WriteLine("-------- BEGIN TEXT CALLED ---------");
                        System.Console.Out.WriteLine("------------------------------------");
                        break;
                    }

                    case EventType.RENDER_TEXT: {
                        System.Console.Out.WriteLine("-------- RENDER TEXT CALLED --------");
                        TextRenderInfo renderInfo = (TextRenderInfo)data;
                        System.Console.Out.WriteLine("String: " + renderInfo.GetPdfString());
                        System.Console.Out.WriteLine("------------------------------------");
                        break;
                    }

                    case EventType.END_TEXT: {
                        System.Console.Out.WriteLine("-------- END TEXT CALLED -----------");
                        System.Console.Out.WriteLine("------------------------------------");
                        break;
                    }

                    case EventType.RENDER_IMAGE: {
                        System.Console.Out.WriteLine("-------- RENDER IMAGE CALLED---------");
                        ImageRenderInfo renderInfo1 = (ImageRenderInfo)data;
                        System.Console.Out.WriteLine("Image: " + renderInfo1.GetImage().GetPdfObject());
                        System.Console.Out.WriteLine("------------------------------------");
                        break;
                    }

                    case EventType.RENDER_PATH: {
                        System.Console.Out.WriteLine("-------- RENDER PATH CALLED --------");
                        PathRenderInfo renderinfo2 = (PathRenderInfo)data;
                        System.Console.Out.WriteLine("Path: " + renderinfo2.GetPath());
                        System.Console.Out.WriteLine("------------------------------------");
                        break;
                    }

                    case EventType.CLIP_PATH_CHANGED: {
                        System.Console.Out.WriteLine("-------- CLIPPING PATH CALLED-------");
                        ClippingPathInfo renderinfo3 = (ClippingPathInfo)data;
                        System.Console.Out.WriteLine("Clipping path: " + renderinfo3.GetClippingPath());
                        System.Console.Out.WriteLine("------------------------------------");
                        break;
                    }
                }
            }

            public ICollection<EventType> GetSupportedEvents() {
                return null;
            }
        }
    }
}
