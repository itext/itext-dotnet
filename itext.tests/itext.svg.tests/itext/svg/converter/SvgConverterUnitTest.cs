using System;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Dummy.Processors.Impl;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Converter {
    public class SvgConverterUnitTest {
        private PdfDocument doc;

        private readonly String content = "<svg width=\"10\" height=\"10\"/>";

        private Stream @is;

        // we cannot easily mock the PdfDocument, so we make do with as close to unit testing as we can
        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            @is = new MemoryStream(content.GetBytes(Encoding.UTF8));
        }

        [NUnit.Framework.TearDown]
        public virtual void Teardown() {
            doc.Close();
        }

        private void TestResourceCreated(PdfDocument doc, int pageNo) {
            PdfResources res = doc.GetPage(pageNo).GetResources();
            NUnit.Framework.Assert.AreEqual(1, res.GetPdfObject().Size());
            foreach (PdfName name in res.GetResourceNames()) {
                PdfObject obj = res.GetResourceObject(PdfName.XObject, name);
                NUnit.Framework.Assert.IsTrue(obj.IsStream());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawStringOnDocumentCreatesResourceTest() {
            SvgConverter.DrawOnDocument(content, doc, 1);
            TestResourceCreated(doc, 1);
        }

        [NUnit.Framework.Test]
        public virtual void DrawStringOnDocumentWithPropsCreatesResourceTest() {
            SvgConverter.DrawOnDocument(content, doc, 1, new DummySvgConverterProperties());
            TestResourceCreated(doc, 1);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DrawStreamOnDocumentCreatesResourceTest() {
            SvgConverter.DrawOnDocument(@is, doc, 1);
            TestResourceCreated(doc, 1);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DrawStreamOnDocumentWithPropsCreatesResourceTest() {
            SvgConverter.DrawOnDocument(@is, doc, 1, new DummySvgConverterProperties());
            TestResourceCreated(doc, 1);
        }

        [NUnit.Framework.Test]
        public virtual void DrawStringOnPageCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            SvgConverter.DrawOnPage(content, page);
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        [NUnit.Framework.Test]
        public virtual void DrawStringOnPageWithPropsCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            SvgConverter.DrawOnPage(content, page, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DrawStreamOnPageCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            SvgConverter.DrawOnPage(@is, page);
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DrawStreamOnPageWithPropsCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            SvgConverter.DrawOnPage(@is, page, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        [NUnit.Framework.Test]
        public virtual void DrawStringOnCanvasCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            SvgConverter.DrawOnCanvas(content, canvas);
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        [NUnit.Framework.Test]
        public virtual void DrawStringOnCanvasWithPropsCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            SvgConverter.DrawOnCanvas(content, canvas, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DrawStreamOnCanvasCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            SvgConverter.DrawOnCanvas(@is, canvas);
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DrawStreamOnCanvasWithPropsCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            SvgConverter.DrawOnCanvas(@is, canvas, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertStringToXObjectCreatesNoResourceTest() {
            SvgConverter.ConvertToXObject(content, doc);
            NUnit.Framework.Assert.AreEqual(0, doc.GetLastPage().GetResources().GetPdfObject().Size());
        }

        [NUnit.Framework.Test]
        public virtual void ConvertStringToXObjectWithPropsCreatesNoResourceTest() {
            SvgConverter.ConvertToXObject(content, doc, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(0, doc.GetLastPage().GetResources().GetPdfObject().Size());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ConvertStreamToXObjectCreatesNoResourceTest() {
            SvgConverter.ConvertToXObject(@is, doc);
            NUnit.Framework.Assert.AreEqual(0, doc.GetLastPage().GetResources().GetPdfObject().Size());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ConvertStreamToXObjectWithPropsCreatesNoResourceTest() {
            SvgConverter.ConvertToXObject(@is, doc, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(0, doc.GetLastPage().GetResources().GetPdfObject().Size());
        }

        [NUnit.Framework.Test]
        public virtual void ProcessNodeWithCustomFactory() {
            INode svg = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), ""));
            DummySvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer node = SvgConverter.Process(svg, props);
            NUnit.Framework.Assert.IsTrue(node is DummySvgNodeRenderer);
            NUnit.Framework.Assert.AreEqual(0, node.GetChildren().Count);
            NUnit.Framework.Assert.IsNull(node.GetParent());
        }

        [NUnit.Framework.Test]
        public virtual void ProcessNode() {
            INode svg = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), ""));
            ISvgNodeRenderer node = SvgConverter.Process(svg);
            NUnit.Framework.Assert.IsTrue(node is SvgSvgNodeRenderer);
            NUnit.Framework.Assert.AreEqual(0, node.GetChildren().Count);
            NUnit.Framework.Assert.IsNull(node.GetParent());
        }

        [NUnit.Framework.Test]
        public virtual void ParseString() {
            INode actual = SvgConverter.Parse(content);
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            JsoupElementNode node = (JsoupElementNode)actual.ChildNodes()[0];
            NUnit.Framework.Assert.AreEqual("svg", node.Name());
            NUnit.Framework.Assert.AreEqual(0, node.ChildNodes().Count);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ParseStream() {
            INode actual = SvgConverter.Parse(@is);
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            JsoupElementNode node = (JsoupElementNode)actual.ChildNodes()[0];
            NUnit.Framework.Assert.AreEqual("svg", node.Name());
            NUnit.Framework.Assert.AreEqual(0, node.ChildNodes().Count);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ParseStreamWithProps() {
            INode actual = SvgConverter.Parse(@is, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            JsoupElementNode node = (JsoupElementNode)actual.ChildNodes()[0];
            NUnit.Framework.Assert.AreEqual("svg", node.Name());
            NUnit.Framework.Assert.AreEqual(0, node.ChildNodes().Count);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ParseStreamErrorEncodingTooBig() {
            @is = new MemoryStream(content.GetBytes(Encoding.Unicode));
            INode actual = SvgConverter.Parse(@is, new DummySvgConverterProperties());
            // defaults to UTF-8
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            // Does not throw an exception, but produces gibberish output that gets fed into a Text element, which is not a JsoupElementNode
            NUnit.Framework.Assert.IsFalse(actual.ChildNodes()[0] is JsoupElementNode);
        }

        private class OtherCharsetDummySvgConverterProperties : DummySvgConverterProperties {
            public override String GetCharset() {
                return "UTF-16LE";
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ParseStreamWithOtherEncoding() {
            @is = new MemoryStream(content.GetBytes(Encoding.Unicode));
            INode actual = SvgConverter.Parse(@is, new SvgConverterUnitTest.OtherCharsetDummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            JsoupElementNode node = (JsoupElementNode)actual.ChildNodes()[0];
            NUnit.Framework.Assert.AreEqual("svg", node.Name());
            NUnit.Framework.Assert.AreEqual(0, node.ChildNodes().Count);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ParseStreamErrorOtherCharset() {
            INode actual = SvgConverter.Parse(@is, new SvgConverterUnitTest.OtherCharsetDummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            // Does not throw an exception, but produces gibberish output that gets fed into a Text element, which is not a JsoupElementNode
            NUnit.Framework.Assert.IsFalse(actual.ChildNodes()[0] is JsoupElementNode);
        }
    }
}
