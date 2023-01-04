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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ClipPathSvgNodeRendererLowLevelIntegrationTest : SvgIntegrationTest {
        private PdfCanvas cv;

        private SvgDrawContext sdc;

        [NUnit.Framework.SetUp]
        public virtual void SetupDrawContextAndCanvas() {
            sdc = new SvgDrawContext(new ResourceResolver(""), new FontProvider());
            // set compression to none, in case you want to write to disk and inspect the created document
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel(0));
            PdfDocument doc = new PdfDocument(writer);
            cv = new PdfCanvas(doc.AddNewPage());
            sdc.PushCanvas(cv);
        }

        [NUnit.Framework.TearDown]
        public virtual void Close() {
            cv.GetDocument().Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyClipPathRendererNotDrawn() {
            ClipPathSvgNodeRenderer clipRenderer = new ClipPathSvgNodeRenderer();
            clipRenderer.SetAttributesAndStyles(new Dictionary<String, String>());
            clipRenderer.Draw(sdc);
            NUnit.Framework.Assert.AreEqual(0, cv.GetContentStream().GetBytes().Length);
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyEoClipPathRendererNotDrawn() {
            ClipPathSvgNodeRenderer clipRenderer = new ClipPathSvgNodeRenderer();
            clipRenderer.SetAttribute(SvgConstants.Attributes.CLIP_RULE, SvgConstants.Values.FILL_RULE_EVEN_ODD);
            clipRenderer.Draw(sdc);
            NUnit.Framework.Assert.AreEqual(0, cv.GetContentStream().GetBytes().Length);
        }

        [NUnit.Framework.Test]
        public virtual void TestRectClipPathRenderer() {
            ClipPathSvgNodeRenderer clipRenderer = new ClipPathSvgNodeRenderer();
            clipRenderer.SetAttributesAndStyles(new Dictionary<String, String>());
            RectangleSvgNodeRenderer rectRenderer = new RectangleSvgNodeRenderer();
            rectRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, "400");
            rectRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, "400");
            clipRenderer.AddChild(rectRenderer);
            clipRenderer.Draw(sdc);
            NUnit.Framework.Assert.AreEqual("q\n% rect\n0 0 300 300 re\nW\nn\nQ\n", iText.Commons.Utils.JavaUtil.GetStringForBytes
                (cv.GetContentStream().GetBytes()));
        }

        [NUnit.Framework.Test]
        public virtual void TestRectClipPathEoRendererNoChange() {
            ClipPathSvgNodeRenderer clipRenderer = new ClipPathSvgNodeRenderer();
            // clip-rule can only be set on elements in a clipPath, and must not have any influence on drawing
            clipRenderer.SetAttribute(SvgConstants.Attributes.CLIP_RULE, SvgConstants.Values.FILL_RULE_EVEN_ODD);
            RectangleSvgNodeRenderer rectRenderer = new RectangleSvgNodeRenderer();
            rectRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, "400");
            rectRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, "400");
            clipRenderer.AddChild(rectRenderer);
            clipRenderer.Draw(sdc);
            NUnit.Framework.Assert.AreEqual("q\n% rect\n0 0 300 300 re\nW\nn\nQ\n", iText.Commons.Utils.JavaUtil.GetStringForBytes
                (cv.GetContentStream().GetBytes()));
        }

        [NUnit.Framework.Test]
        public virtual void TestRectEoClipPathRenderer() {
            ClipPathSvgNodeRenderer clipRenderer = new ClipPathSvgNodeRenderer();
            clipRenderer.SetAttributesAndStyles(new Dictionary<String, String>());
            RectangleSvgNodeRenderer rectRenderer = new RectangleSvgNodeRenderer();
            rectRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, "400");
            rectRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, "400");
            rectRenderer.SetAttribute(SvgConstants.Attributes.CLIP_RULE, SvgConstants.Values.FILL_RULE_EVEN_ODD);
            clipRenderer.AddChild(rectRenderer);
            clipRenderer.Draw(sdc);
            NUnit.Framework.Assert.AreEqual("q\n% rect\n0 0 300 300 re\nW*\nn\nQ\n", iText.Commons.Utils.JavaUtil.GetStringForBytes
                (cv.GetContentStream().GetBytes()));
        }

        [NUnit.Framework.Test]
        public virtual void TestAppliedClipPathRenderer() {
            AbstractBranchSvgNodeRenderer clipPathRenderer = new ClipPathSvgNodeRenderer();
            clipPathRenderer.SetAttribute(SvgConstants.Attributes.ID, "randomString");
            ISvgNodeRenderer clippedRenderer = new RectangleSvgNodeRenderer();
            clippedRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, "80");
            clippedRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, "80");
            clipPathRenderer.AddChild(clippedRenderer);
            sdc.AddNamedObject("randomString", clipPathRenderer);
            ISvgNodeRenderer drawnRenderer = new CircleSvgNodeRenderer();
            drawnRenderer.SetAttribute(SvgConstants.Attributes.R, "84");
            drawnRenderer.SetAttribute(SvgConstants.Attributes.CLIP_PATH, "url(#randomString)");
            drawnRenderer.Draw(sdc);
            String expected = "q\n" + "% rect\n" + "0 0 60 60 re\n" + "W\n" + "n\n" + "0 0 0 rg\n" + "% ellipse\n" + "63 0 m\n"
                 + "63 34.79 34.79 63 0 63 c\n" + "-34.79 63 -63 34.79 -63 0 c\n" + "-63 -34.79 -34.79 -63 0 -63 c\n" 
                + "34.79 -63 63 -34.79 63 0 c\n" + "f\n" + "Q\n";
            NUnit.Framework.Assert.AreEqual(expected, iText.Commons.Utils.JavaUtil.GetStringForBytes(cv.GetContentStream
                ().GetBytes()));
        }

        [NUnit.Framework.Test]
        public virtual void TestAppliedGroupClipPathRenderer() {
            AbstractBranchSvgNodeRenderer clipPathRenderer = new ClipPathSvgNodeRenderer();
            clipPathRenderer.SetAttribute(SvgConstants.Attributes.ID, "randomString");
            ISvgNodeRenderer clippedRenderer = new RectangleSvgNodeRenderer();
            clippedRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, "80");
            clippedRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, "80");
            clipPathRenderer.AddChild(clippedRenderer);
            sdc.AddNamedObject("randomString", clipPathRenderer);
            AbstractBranchSvgNodeRenderer groupRenderer = new GroupSvgNodeRenderer();
            groupRenderer.SetAttributesAndStyles(new Dictionary<String, String>());
            ISvgNodeRenderer drawnRenderer = new CircleSvgNodeRenderer();
            drawnRenderer.SetAttribute(SvgConstants.Attributes.R, "84");
            drawnRenderer.SetAttribute(SvgConstants.Attributes.CLIP_PATH, "url(#randomString)");
            groupRenderer.AddChild(drawnRenderer);
            groupRenderer.Draw(sdc);
            String expected = "0 0 0 rg\n" + "q\n" + "q\n" + "% rect\n" + "0 0 60 60 re\n" + "W\n" + "n\n" + "% ellipse\n"
                 + "63 0 m\n" + "63 34.79 34.79 63 0 63 c\n" + "-34.79 63 -63 34.79 -63 0 c\n" + "-63 -34.79 -34.79 -63 0 -63 c\n"
                 + "34.79 -63 63 -34.79 63 0 c\n" + "f\n" + "Q\n" + "Q\n";
            NUnit.Framework.Assert.AreEqual(expected, iText.Commons.Utils.JavaUtil.GetStringForBytes(cv.GetContentStream
                ().GetBytes()));
        }

        [NUnit.Framework.Test]
        public virtual void TestEoAppliedGroupClipPathRenderer() {
            AbstractBranchSvgNodeRenderer clipPathRenderer = new ClipPathSvgNodeRenderer();
            clipPathRenderer.SetAttribute(SvgConstants.Attributes.ID, "randomString");
            ISvgNodeRenderer clippedRenderer = new RectangleSvgNodeRenderer();
            clippedRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, "80");
            clippedRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, "80");
            clippedRenderer.SetAttribute(SvgConstants.Attributes.CLIP_RULE, SvgConstants.Values.FILL_RULE_EVEN_ODD);
            ISvgNodeRenderer clippedRenderer2 = new RectangleSvgNodeRenderer();
            clippedRenderer2.SetAttribute(SvgConstants.Attributes.WIDTH, "80");
            clippedRenderer2.SetAttribute(SvgConstants.Attributes.HEIGHT, "80");
            clipPathRenderer.AddChild(clippedRenderer);
            clipPathRenderer.AddChild(clippedRenderer2);
            sdc.AddNamedObject("randomString", clipPathRenderer);
            AbstractBranchSvgNodeRenderer groupRenderer = new GroupSvgNodeRenderer();
            groupRenderer.SetAttributesAndStyles(new Dictionary<String, String>());
            ISvgNodeRenderer drawnRenderer = new CircleSvgNodeRenderer();
            drawnRenderer.SetAttribute(SvgConstants.Attributes.R, "84");
            drawnRenderer.SetAttribute(SvgConstants.Attributes.CLIP_PATH, "url(#randomString)");
            groupRenderer.AddChild(drawnRenderer);
            groupRenderer.Draw(sdc);
            String expected = "0 0 0 rg\n" + "q\n" + "q\n" + "% rect\n" + "0 0 60 60 re\n" + "W*\n" + "n\n" + "% ellipse\n"
                 + "63 0 m\n" + "63 34.79 34.79 63 0 63 c\n" + "-34.79 63 -63 34.79 -63 0 c\n" + "-63 -34.79 -34.79 -63 0 -63 c\n"
                 + "34.79 -63 63 -34.79 63 0 c\n" + "f\n" + "Q\n" + "q\n" + "% rect\n" + "0 0 60 60 re\n" + "W\n" + "n\n"
                 + "% ellipse\n" + "63 0 m\n" + "63 34.79 34.79 63 0 63 c\n" + "-34.79 63 -63 34.79 -63 0 c\n" + "-63 -34.79 -34.79 -63 0 -63 c\n"
                 + "34.79 -63 63 -34.79 63 0 c\n" + "f\n" + "Q\n" + "Q\n";
            NUnit.Framework.Assert.AreEqual(expected, iText.Commons.Utils.JavaUtil.GetStringForBytes(cv.GetContentStream
                ().GetBytes()));
        }
    }
}
