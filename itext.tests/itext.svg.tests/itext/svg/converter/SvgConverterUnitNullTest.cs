/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Converter {
    /// <summary>
    /// These tests will make sure that a NullPointerException is never thrown: if a
    /// null check is made, an SVG-specific exception should tell the user where the
    /// null check failed.
    /// </summary>
    /// <remarks>
    /// These tests will make sure that a NullPointerException is never thrown: if a
    /// null check is made, an SVG-specific exception should tell the user where the
    /// null check failed.
    /// If the (optional)
    /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
    /// parameter is null, this
    /// should NOT throw an exception as this is caught in the library.
    /// </remarks>
    [NUnit.Framework.Category("UnitTest")]
    public class SvgConverterUnitNullTest : ExtendedITextTest {
        // we cannot easily mock the PdfDocument, so we make do with as close to unit testing as we can
        private readonly String content = "<svg width=\"10\" height=\"10\"/>";

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentStringNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((String)null
                    , doc, 1));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentInputStreamNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((Stream)null
                    , doc, 1));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentDocNullTest() {
            Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument(@is, null, 
                1));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentAllNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((String)null
                , null, 1));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentAllNullTest2() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((Stream)null
                , null, 1));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentStringPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                SvgConverter.DrawOnDocument(content, doc, 1, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentInputStreamPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
                SvgConverter.DrawOnDocument(@is, doc, 1, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageStringNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfPage page = doc.AddNewPage();
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnPage((String)null, page
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageInputStreamNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfPage page = doc.AddNewPage();
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnPage((Stream)null, page
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageDocNullTest() {
            Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnPage(@is, null));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageAllNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnPage((String)null, null
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageAllNullTest2() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnPage((Stream)null, null
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageStringPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfPage page = doc.AddNewPage();
                SvgConverter.DrawOnPage(content, page, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageInputStreamPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfPage page = doc.AddNewPage();
                Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
                SvgConverter.DrawOnPage(@is, page, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasStringNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((String)null
                    , doc, 1));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasInputStreamNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnCanvas((Stream)null, 
                    canvas));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasDocNullTest() {
            Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnCanvas(@is, null));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasAllNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnCanvas((String)null, 
                null));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasAllNullTest2() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnCanvas((Stream)null, 
                null));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasStringPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                SvgConverter.DrawOnCanvas(content, canvas, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasInputStreamPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
                SvgConverter.DrawOnCanvas(@is, canvas, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectStringNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((String)null
                    , doc));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectInputStreamNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((Stream)null
                    , doc));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectRendererNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((ISvgNodeRenderer
                    )null, doc));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectDocWithStringNullTest() {
            Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject(@is, null
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectDocWithStreamNullTest() {
            Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject(@is, null
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectDocWithRendererNullTest() {
            Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
            ISvgNodeRenderer renderer = SvgConverter.Process(SvgConverter.Parse(@is), null).GetRootRenderer();
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject(renderer, 
                null));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectAllWithStringNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((String)null
                , null));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectAllWithStreamNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((Stream)null
                , null));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectAllWithRendererNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((ISvgNodeRenderer
                )null, null));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectStringPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                SvgConverter.ConvertToXObject(content, doc, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectInputStreamPropsNullTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
                SvgConverter.ConvertToXObject(@is, doc, null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ParseStringNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.Parse((String)null));
        }

        [NUnit.Framework.Test]
        public virtual void ParseStreamNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.Parse((Stream)null));
        }

        [NUnit.Framework.Test]
        public virtual void ParseStreamPropsNullTest() {
            Stream @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
            SvgConverter.Parse(@is, null);
        }

        [NUnit.Framework.Test]
        public virtual void ParseStringPropsNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.Parse(null, null));
        }

        [NUnit.Framework.Test]
        public virtual void ProcessAllNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.Process(null, null));
        }

        [NUnit.Framework.Test]
        public virtual void ProcessPropsNullTest() {
            INode svg = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), ""));
            SvgConverter.Process(svg, null);
        }
    }
}
