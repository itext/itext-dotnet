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
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TransparencyTest : ExtendedITextTest {
        private static readonly PdfName DEFAULT_RESOURCE_NAME = new PdfName("Gs1");

        private static readonly PdfName FILL_OPAC = new PdfName("ca");

        private static readonly PdfName STROKE_OPAC = new PdfName("CA");

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
        public virtual void NoOpacitySet() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "blue");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithStroke() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "blue");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithoutStroke() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithFill() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithNoneStroke() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithFill() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithoutFill() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithNoneFill() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithStroke() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "blue");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillAndStrokeOpacitySetWithStrokeAndFill() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "green");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(2, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(100,20,80)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithStrokeRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(100,20,80)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithoutStrokeRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithFillRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgb(100,20,80)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithNoneStrokeRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithFillRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgb(100,20,80)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithoutFillRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithNoneFillRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithStrokeRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(100,20,80)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillAndStrokeOpacitySetWithStrokeAndFillRGB() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgb(100,20,80)");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(60,90,180)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(2, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80, .75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithStrokeRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.5625));
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithoutStrokeRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithNoneStrokeRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.5625));
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithoutFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithNoneFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithStrokeRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(2, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void FillAndStrokeOpacitySetWithStrokeAndFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(60,90,180,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(2, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.5625));
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.5625));
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithStrokeRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithoutStrokeRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(1, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithNoneStrokeRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithNoneFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void NoAndStrokeOpacitySetWithStrokeAndFillRGBA() {
            AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
            renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(60,90,180,.75)");
            renderer.Draw(sdc);
            PdfResources resources = cv.GetResources();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
            PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                );
            NUnit.Framework.Assert.AreEqual(2, resDic.Size());
            NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
        }
    }
}
