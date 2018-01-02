/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
                PdfCanvasProcessor processor = new PdfCanvasProcessor(new _IEventListener_82());
                processor.ProcessPageContent(page);
            }
        }

        private sealed class _IEventListener_82 : IEventListener {
            public _IEventListener_82() {
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
