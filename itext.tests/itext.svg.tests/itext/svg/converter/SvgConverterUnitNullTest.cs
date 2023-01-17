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

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentStringNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((String)null
                , doc, 1));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentInputStreamNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((Stream)null
                , doc, 1));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentDocNullTest() {
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
            SvgConverter.DrawOnDocument(content, doc, 1, null);
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentInputStreamPropsNullTest() {
            SvgConverter.DrawOnDocument(@is, doc, 1, null);
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageStringNullTest() {
            PdfPage page = doc.GetFirstPage();
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnPage((String)null, page
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageInputStreamNullTest() {
            PdfPage page = doc.GetFirstPage();
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnPage((Stream)null, page
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageDocNullTest() {
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
            PdfPage page = doc.GetFirstPage();
            SvgConverter.DrawOnPage(content, page, null);
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageInputStreamPropsNullTest() {
            PdfPage page = doc.GetFirstPage();
            SvgConverter.DrawOnPage(@is, page, null);
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasStringNullTest() {
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnDocument((String)null
                , doc, 1));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasInputStreamNullTest() {
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.DrawOnCanvas((Stream)null, 
                canvas));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasDocNullTest() {
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
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            SvgConverter.DrawOnCanvas(content, canvas, null);
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasInputStreamPropsNullTest() {
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            SvgConverter.DrawOnCanvas(@is, canvas, null);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectStringNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((String)null
                , doc));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectInputStreamNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((Stream)null
                , doc));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectRendererNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject((ISvgNodeRenderer
                )null, doc));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectDocWithStringNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject(@is, null
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectDocWithStreamNullTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ConvertToXObject(@is, null
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectDocWithRendererNullTest() {
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
            SvgConverter.ConvertToXObject(content, doc, null);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectInputStreamPropsNullTest() {
            SvgConverter.ConvertToXObject(@is, doc, null);
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
