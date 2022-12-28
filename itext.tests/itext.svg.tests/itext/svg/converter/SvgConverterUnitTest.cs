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
using System.IO;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Font;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Dummy.Processors.Impl;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Converter {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgConverterUnitTest : ExtendedITextTest {
        // we cannot easily mock the PdfDocument, so we make do with as close to unit testing as we can
        private PdfDocument doc;

        private readonly String content = "<svg width=\"10\" height=\"10\"/>";

        private Stream @is;

        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            @is = new MemoryStream(content.GetBytes(System.Text.Encoding.UTF8));
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

        [NUnit.Framework.Test]
        public virtual void DrawStreamOnDocumentCreatesResourceTest() {
            SvgConverter.DrawOnDocument(@is, doc, 1);
            TestResourceCreated(doc, 1);
        }

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

        [NUnit.Framework.Test]
        public virtual void DrawStreamOnPageCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            SvgConverter.DrawOnPage(@is, page);
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

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

        [NUnit.Framework.Test]
        public virtual void DrawStreamOnCanvasCreatesResourceTest() {
            PdfPage page = doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            SvgConverter.DrawOnCanvas(@is, canvas);
            NUnit.Framework.Assert.AreEqual(0, doc.GetFirstPage().GetResources().GetPdfObject().Size());
            TestResourceCreated(doc, 2);
        }

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

        [NUnit.Framework.Test]
        public virtual void ConvertStreamToXObjectCreatesNoResourceTest() {
            SvgConverter.ConvertToXObject(@is, doc);
            NUnit.Framework.Assert.AreEqual(0, doc.GetLastPage().GetResources().GetPdfObject().Size());
        }

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
            IBranchSvgNodeRenderer node = (IBranchSvgNodeRenderer)SvgConverter.Process(svg, props).GetRootRenderer();
            NUnit.Framework.Assert.IsTrue(node is DummySvgNodeRenderer);
            NUnit.Framework.Assert.AreEqual(0, node.GetChildren().Count);
            NUnit.Framework.Assert.IsNull(node.GetParent());
        }

        [NUnit.Framework.Test]
        public virtual void ProcessNode() {
            INode svg = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), ""));
            IBranchSvgNodeRenderer node = (IBranchSvgNodeRenderer)SvgConverter.Process(svg, null).GetRootRenderer();
            NUnit.Framework.Assert.IsTrue(node is SvgTagSvgNodeRenderer);
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

        [NUnit.Framework.Test]
        public virtual void ParseStream() {
            INode actual = SvgConverter.Parse(@is);
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            JsoupElementNode node = (JsoupElementNode)actual.ChildNodes()[0];
            NUnit.Framework.Assert.AreEqual("svg", node.Name());
            NUnit.Framework.Assert.AreEqual(0, node.ChildNodes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ParseStreamWithProps() {
            INode actual = SvgConverter.Parse(@is, new DummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            JsoupElementNode node = (JsoupElementNode)actual.ChildNodes()[0];
            NUnit.Framework.Assert.AreEqual("svg", node.Name());
            NUnit.Framework.Assert.AreEqual(0, node.ChildNodes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ParseStreamErrorEncodingTooBig() {
            @is = new MemoryStream(content.GetBytes(System.Text.Encoding.Unicode));
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

        [NUnit.Framework.Test]
        public virtual void ParseStreamWithOtherEncoding() {
            @is = new MemoryStream(content.GetBytes(System.Text.Encoding.Unicode));
            INode actual = SvgConverter.Parse(@is, new SvgConverterUnitTest.OtherCharsetDummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            JsoupElementNode node = (JsoupElementNode)actual.ChildNodes()[0];
            NUnit.Framework.Assert.AreEqual("svg", node.Name());
            NUnit.Framework.Assert.AreEqual(0, node.ChildNodes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ParseStreamErrorOtherCharset() {
            INode actual = SvgConverter.Parse(@is, new SvgConverterUnitTest.OtherCharsetDummySvgConverterProperties());
            NUnit.Framework.Assert.AreEqual(1, actual.ChildNodes().Count);
            // Does not throw an exception, but produces gibberish output that gets fed into a Text element, which is not a JsoupElementNode
            NUnit.Framework.Assert.IsFalse(actual.ChildNodes()[0] is JsoupElementNode);
        }

        [NUnit.Framework.Test]
        public virtual void CheckNullTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument
                ("test", null, 1));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.PARAMETER_CANNOT_BE_NULL, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ResourceResolverInstanceTest() {
            DummySvgConverterProperties properties = new DummySvgConverterProperties();
            SvgProcessorContext context = new SvgProcessorContext(properties);
            ResourceResolver initialResolver = context.GetResourceResolver();
            SvgProcessorResult svgProcessorResult = new SvgProcessorResult(new Dictionary<String, ISvgNodeRenderer>(), 
                new SvgTagSvgNodeRenderer(), context);
            ResourceResolver currentResolver = SvgConverter.GetResourceResolver(svgProcessorResult, properties);
            NUnit.Framework.Assert.AreEqual(initialResolver, currentResolver);
        }

        [NUnit.Framework.Test]
        public virtual void CreateResourceResolverWithoutProcessorResultTest() {
            ISvgConverterProperties props = new SvgConverterProperties();
            NUnit.Framework.Assert.IsNotNull(SvgConverter.GetResourceResolver(null, props));
        }

        [NUnit.Framework.Test]
        public virtual void ResourceResolverInstanceCustomResolverTest() {
            DummySvgConverterProperties properties = new DummySvgConverterProperties();
            SvgConverterUnitTest.TestSvgProcessorResult testSvgProcessorResult = new SvgConverterUnitTest.TestSvgProcessorResult
                ();
            ResourceResolver currentResolver = SvgConverter.GetResourceResolver(testSvgProcessorResult, properties);
            NUnit.Framework.Assert.IsNotNull(currentResolver);
        }

        [NUnit.Framework.Test]
        public virtual void ResourceResolverInstanceCustomResolverNullPropsTest() {
            SvgConverterUnitTest.TestSvgProcessorResult testSvgProcessorResult = new SvgConverterUnitTest.TestSvgProcessorResult
                ();
            ResourceResolver currentResolver = SvgConverter.GetResourceResolver(testSvgProcessorResult, null);
            NUnit.Framework.Assert.IsNotNull(currentResolver);
        }

        [NUnit.Framework.Test]
        public virtual void NullBBoxInDrawTest() {
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                PdfFormXObject @object = SvgConverter.ConvertToXObject(content, doc);
                ((PdfDictionary)@object.GetPdfObject()).Remove(PdfName.BBox);
                SvgConverter.Draw(@object, new PdfCanvas(doc, 1), 0, 0);
            }
            );
        }

        private class TestSvgProcessorResult : ISvgProcessorResult {
            public TestSvgProcessorResult() {
            }

            public virtual IDictionary<String, ISvgNodeRenderer> GetNamedObjects() {
                return null;
            }

            public virtual ISvgNodeRenderer GetRootRenderer() {
                return null;
            }

            public virtual FontProvider GetFontProvider() {
                return null;
            }

            public virtual FontSet GetTempFonts() {
                return null;
            }
        }
    }
}
