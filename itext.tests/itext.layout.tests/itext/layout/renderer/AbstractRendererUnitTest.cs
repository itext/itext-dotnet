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
using System.Collections.Generic;
using System.IO;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    public class AbstractRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreateXObjectTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_LEFT).AddColorStop(new GradientColorStop
                (ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE));
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfXObject pdfXObject = AbstractRenderer.CreateXObject(gradientBuilder, new Rectangle(0, 0, 20, 20), pdfDocument
                );
            NUnit.Framework.Assert.IsNotNull(pdfXObject.GetPdfObject().Get(PdfName.Resources));
        }

        [NUnit.Framework.Test]
        public virtual void CreateXObjectWithNullLinearGradientTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfXObject pdfXObject = AbstractRenderer.CreateXObject(null, new Rectangle(0, 0, 20, 20), pdfDocument);
            NUnit.Framework.Assert.IsNull(pdfXObject.GetPdfObject().Get(PdfName.Resources));
        }

        [NUnit.Framework.Test]
        public virtual void CreateXObjectWithInvalidColorTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfXObject pdfXObject = AbstractRenderer.CreateXObject(gradientBuilder, new Rectangle(0, 0, 20, 20), pdfDocument
                );
            NUnit.Framework.Assert.IsNull(pdfXObject.GetPdfObject().Get(PdfName.Resources));
        }

        [NUnit.Framework.Test]
        public virtual void DrawBackgroundImageTest() {
            AbstractRenderer renderer = new _DivRenderer_118(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_128(counter, bytes, document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, new BackgroundImage.Builder().SetImage(new _PdfImageXObject_145
                (ImageDataFactory.CreateRawImage(bytes))).Build());
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(50, counter[0]);
        }

        private sealed class _DivRenderer_118 : DivRenderer {
            public _DivRenderer_118(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(99.0f, 49.0f);
            }
        }

        private sealed class _PdfCanvas_128 : PdfCanvas {
            public _PdfCanvas_128(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                ++counter[0];
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfImageXObject);
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysToString(((PdfImageXObject)xObject).GetImageBytes(false)), 
                    JavaUtil.ArraysToString(bytes));
                return null;
            }

            private readonly int[] counter;

            private readonly byte[] bytes;
        }

        private sealed class _PdfImageXObject_145 : PdfImageXObject {
            public _PdfImageXObject_145(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawBackgroundImageWithNoRepeatXTest() {
            AbstractRenderer renderer = new _DivRenderer_162(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_172(counter, bytes, document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, new BackgroundImage.Builder().SetImage(new _PdfImageXObject_189
                (ImageDataFactory.CreateRawImage(bytes))).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue
                .NO_REPEAT, BackgroundRepeat.BackgroundRepeatValue.REPEAT)).Build());
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(5, counter[0]);
        }

        private sealed class _DivRenderer_162 : DivRenderer {
            public _DivRenderer_162(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 49.0f);
            }
        }

        private sealed class _PdfCanvas_172 : PdfCanvas {
            public _PdfCanvas_172(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                ++counter[0];
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfImageXObject);
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysToString(((PdfImageXObject)xObject).GetImageBytes(false)), 
                    JavaUtil.ArraysToString(bytes));
                return null;
            }

            private readonly int[] counter;

            private readonly byte[] bytes;
        }

        private sealed class _PdfImageXObject_189 : PdfImageXObject {
            public _PdfImageXObject_189(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawBackgroundImageWithNoRepeatYTest() {
            AbstractRenderer renderer = new _DivRenderer_206(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_216(counter, bytes, document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, new BackgroundImage.Builder().SetImage(new _PdfImageXObject_233
                (ImageDataFactory.CreateRawImage(bytes))).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue
                .REPEAT, BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).Build());
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(10, counter[0]);
        }

        private sealed class _DivRenderer_206 : DivRenderer {
            public _DivRenderer_206(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(99.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_216 : PdfCanvas {
            public _PdfCanvas_216(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                ++counter[0];
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfImageXObject);
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysToString(((PdfImageXObject)xObject).GetImageBytes(false)), 
                    JavaUtil.ArraysToString(bytes));
                return null;
            }

            private readonly int[] counter;

            private readonly byte[] bytes;
        }

        private sealed class _PdfImageXObject_233 : PdfImageXObject {
            public _PdfImageXObject_233(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawBackgroundImageWithNoRepeatTest() {
            AbstractRenderer renderer = new _DivRenderer_250(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_260(counter, bytes, document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, new BackgroundImage.Builder().SetImage(new _PdfImageXObject_277
                (ImageDataFactory.CreateRawImage(bytes))).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue
                .NO_REPEAT)).Build());
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(1, counter[0]);
        }

        private sealed class _DivRenderer_250 : DivRenderer {
            public _DivRenderer_250(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_260 : PdfCanvas {
            public _PdfCanvas_260(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                ++counter[0];
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfImageXObject);
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysToString(((PdfImageXObject)xObject).GetImageBytes(false)), 
                    JavaUtil.ArraysToString(bytes));
                return null;
            }

            private readonly int[] counter;

            private readonly byte[] bytes;
        }

        private sealed class _PdfImageXObject_277 : PdfImageXObject {
            public _PdfImageXObject_277(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawBackgroundImageWithPositionTest() {
            AbstractRenderer renderer = new _DivRenderer_295(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_304(bytes, document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, new BackgroundImage.Builder().SetImage(new _PdfImageXObject_322
                (ImageDataFactory.CreateRawImage(bytes))).SetBackgroundPosition(new BackgroundPosition().SetXShift(new 
                UnitValue(UnitValue.PERCENT, 30))).Build());
            renderer.DrawBackground(context);
        }

        private sealed class _DivRenderer_295 : DivRenderer {
            public _DivRenderer_295(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_304 : PdfCanvas {
            public _PdfCanvas_304(byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.bytes = bytes;
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfImageXObject);
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysToString(((PdfImageXObject)xObject).GetImageBytes(false)), 
                    JavaUtil.ArraysToString(bytes));
                NUnit.Framework.Assert.AreEqual(27, (int)rect.GetX());
                NUnit.Framework.Assert.AreEqual(40, (int)rect.GetY());
                return null;
            }

            private readonly byte[] bytes;
        }

        private sealed class _PdfImageXObject_322 : PdfImageXObject {
            public _PdfImageXObject_322(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawGradientWithPositionTest() {
            AbstractRenderer renderer = new _DivRenderer_338(new Div());
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_346(document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, new BackgroundImage.Builder().SetLinearGradientBuilder(new 
                StrategyBasedLinearGradientBuilder().AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue
                ())).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue()))).SetBackgroundPosition(
                new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY(BackgroundPosition.PositionY
                .BOTTOM).SetYShift(UnitValue.CreatePointValue(100)).SetXShift(UnitValue.CreatePointValue(30))).Build()
                );
            renderer.DrawBackground(context);
        }

        private sealed class _DivRenderer_338 : DivRenderer {
            public _DivRenderer_338(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_346 : PdfCanvas {
            public _PdfCanvas_346(PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObject(PdfXObject xObject, Rectangle rect) {
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfFormXObject);
                NUnit.Framework.Assert.AreEqual(-30, (int)rect.GetX());
                NUnit.Framework.Assert.AreEqual(100, (int)rect.GetY());
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawGradientWithPercentagePositionTest() {
            AbstractRenderer renderer = new _DivRenderer_370(new Div());
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_378(document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, new BackgroundImage.Builder().SetLinearGradientBuilder(new 
                StrategyBasedLinearGradientBuilder().AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue
                ())).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue()))).SetBackgroundPosition(
                new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY(BackgroundPosition.PositionY
                .BOTTOM).SetYShift(UnitValue.CreatePercentValue(70)).SetXShift(UnitValue.CreatePercentValue(33))).Build
                ());
            renderer.DrawBackground(context);
        }

        private sealed class _DivRenderer_370 : DivRenderer {
            public _DivRenderer_370(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_378 : PdfCanvas {
            public _PdfCanvas_378(PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObject(PdfXObject xObject, Rectangle rect) {
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfFormXObject);
                NUnit.Framework.Assert.AreEqual(0, (int)rect.GetX());
                NUnit.Framework.Assert.AreEqual(0, (int)rect.GetY());
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawBackgroundImagesTest() {
            AbstractRenderer renderer = new _DivRenderer_402(new Div());
            IList<byte[]> listBytes = JavaUtil.ArraysAsList(new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 }
                , new byte[] { 4, 15, 41, 23, 3, 2, 7, 14, 55, 27, 46, 12, 14, 14, 7, 7, 24, 25 });
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_414(listBytes, counter, document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, JavaUtil.ArraysAsList((BackgroundImage)new BackgroundImage.Builder
                ().SetImage(new _PdfImageXObject_430(ImageDataFactory.CreateRawImage(listBytes[1]))).Build(), (BackgroundImage
                )new BackgroundImage.Builder().SetImage(new _PdfImageXObject_441(ImageDataFactory.CreateRawImage(listBytes
                [0]))).Build()));
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(listBytes.Count, counter[0]);
        }

        private sealed class _DivRenderer_402 : DivRenderer {
            public _DivRenderer_402(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_414 : PdfCanvas {
            public _PdfCanvas_414(IList<byte[]> listBytes, int[] counter, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.listBytes = listBytes;
                this.counter = counter;
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfImageXObject);
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysToString(((PdfImageXObject)xObject).GetImageBytes(false)), 
                    JavaUtil.ArraysToString(listBytes[counter[0]++]));
                return null;
            }

            private readonly IList<byte[]> listBytes;

            private readonly int[] counter;
        }

        private sealed class _PdfImageXObject_430 : PdfImageXObject {
            public _PdfImageXObject_430(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        private sealed class _PdfImageXObject_441 : PdfImageXObject {
            public _PdfImageXObject_441(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DrawBackgroundImagesWithPositionsTest() {
            AbstractRenderer renderer = new _DivRenderer_458(new Div());
            IList<byte[]> listBytes = JavaUtil.ArraysAsList(new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 }
                , new byte[] { 4, 15, 41, 23, 3, 2, 7, 14, 55, 27, 46, 12, 14, 14, 7, 7, 24, 25 });
            float widthHeight = 10.0f;
            IList<Rectangle> listRectangles = JavaUtil.ArraysAsList(new Rectangle(81, 20, widthHeight, widthHeight), new 
                Rectangle(0, 40, widthHeight, widthHeight));
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_475(listBytes, counter, listRectangles, document
                , 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, JavaUtil.ArraysAsList((BackgroundImage)new BackgroundImage.Builder
                ().SetImage(new _PdfImageXObject_493(widthHeight, ImageDataFactory.CreateRawImage(listBytes[1]))).Build
                (), (BackgroundImage)new BackgroundImage.Builder().SetImage(new _PdfImageXObject_504(widthHeight, ImageDataFactory
                .CreateRawImage(listBytes[0]))).SetBackgroundPosition(new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX
                .RIGHT).SetPositionY(BackgroundPosition.PositionY.CENTER).SetXShift(new UnitValue(UnitValue.PERCENT, 10
                ))).Build()));
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(listBytes.Count, counter[0]);
        }

        private sealed class _DivRenderer_458 : DivRenderer {
            public _DivRenderer_458(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_475 : PdfCanvas {
            public _PdfCanvas_475(IList<byte[]> listBytes, int[] counter, IList<Rectangle> listRectangles, PdfDocument
                 baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.listBytes = listBytes;
                this.counter = counter;
                this.listRectangles = listRectangles;
                this.@object = null;
            }

            internal PdfXObject @object;

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                if (this.@object == xObject) {
                    return null;
                }
                this.@object = xObject;
                NUnit.Framework.Assert.IsTrue(xObject is PdfImageXObject);
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysToString(((PdfImageXObject)xObject).GetImageBytes(false)), 
                    JavaUtil.ArraysToString(listBytes[counter[0]]));
                NUnit.Framework.Assert.AreEqual((int)listRectangles[counter[0]].GetX(), (int)rect.GetX());
                NUnit.Framework.Assert.AreEqual((int)listRectangles[counter[0]++].GetY(), (int)rect.GetY());
                return null;
            }

            private readonly IList<byte[]> listBytes;

            private readonly int[] counter;

            private readonly IList<Rectangle> listRectangles;
        }

        private sealed class _PdfImageXObject_493 : PdfImageXObject {
            public _PdfImageXObject_493(float widthHeight, ImageData baseArg1)
                : base(baseArg1) {
                this.widthHeight = widthHeight;
            }

            public override float GetWidth() {
                return widthHeight;
            }

            public override float GetHeight() {
                return widthHeight;
            }

            private readonly float widthHeight;
        }

        private sealed class _PdfImageXObject_504 : PdfImageXObject {
            public _PdfImageXObject_504(float widthHeight, ImageData baseArg1)
                : base(baseArg1) {
                this.widthHeight = widthHeight;
            }

            public override float GetWidth() {
                return widthHeight;
            }

            public override float GetHeight() {
                return widthHeight;
            }

            private readonly float widthHeight;
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundColorClipTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfCanvas pdfCanvas = new _PdfCanvas_523(pdfDocument.AddNewPage());
            DrawContext drawContext = new DrawContext(pdfDocument, pdfCanvas);
            AbstractRenderer renderer = new DivRenderer(new Div().SetPadding(20).SetBorder(new DashedBorder(10)));
            renderer.occupiedArea = new LayoutArea(1, new Rectangle(100f, 200f, 300f, 400f));
            renderer.SetProperty(Property.BACKGROUND, new Background(new DeviceRgb(), 1, BackgroundBox.CONTENT_BOX));
            renderer.DrawBackground(drawContext);
        }

        private sealed class _PdfCanvas_523 : PdfCanvas {
            public _PdfCanvas_523(PdfPage baseArg1)
                : base(baseArg1) {
            }

            public override PdfCanvas Rectangle(double x, double y, double width, double height) {
                NUnit.Framework.Assert.AreEqual(130.0, x, 0);
                NUnit.Framework.Assert.AreEqual(230.0, y, 0);
                NUnit.Framework.Assert.AreEqual(240.0, width, 0);
                NUnit.Framework.Assert.AreEqual(340.0, height, 0);
                return this;
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageClipOriginNoRepeatTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            PdfXObject rawImage = new _PdfImageXObject_544(ImageDataFactory.CreateRawImage(bytes));
            PdfCanvas pdfCanvas = new _PdfCanvas_555(rawImage, pdfDocument.AddNewPage());
            DrawContext drawContext = new DrawContext(pdfDocument, pdfCanvas);
            AbstractRenderer renderer = new DivRenderer(new Div().SetPadding(20).SetBorder(new DashedBorder(10)));
            renderer.occupiedArea = new LayoutArea(1, new Rectangle(100f, 200f, 300f, 400f));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(rawImage).SetBackgroundRepeat(new 
                BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).SetBackgroundClip(BackgroundBox.CONTENT_BOX
                ).SetBackgroundOrigin(BackgroundBox.BORDER_BOX).Build();
            renderer.SetProperty(Property.BACKGROUND_IMAGE, backgroundImage);
            renderer.DrawBackground(drawContext);
        }

        private sealed class _PdfImageXObject_544 : PdfImageXObject {
            public _PdfImageXObject_544(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 50f;
            }

            public override float GetHeight() {
                return 50f;
            }
        }

        private sealed class _PdfCanvas_555 : PdfCanvas {
            public _PdfCanvas_555(PdfXObject rawImage, PdfPage baseArg1)
                : base(baseArg1) {
                this.rawImage = rawImage;
            }

            public override PdfCanvas Rectangle(double x, double y, double width, double height) {
                NUnit.Framework.Assert.AreEqual(130.0, x, 0);
                NUnit.Framework.Assert.AreEqual(230.0, y, 0);
                NUnit.Framework.Assert.AreEqual(240.0, width, 0);
                NUnit.Framework.Assert.AreEqual(340.0, height, 0);
                return this;
            }

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                NUnit.Framework.Assert.AreEqual(rawImage, xObject);
                NUnit.Framework.Assert.AreEqual(100f, rect.GetX(), 0);
                NUnit.Framework.Assert.AreEqual(550f, rect.GetY(), 0);
                NUnit.Framework.Assert.AreEqual(50f, rect.GetWidth(), 0);
                NUnit.Framework.Assert.AreEqual(50f, rect.GetHeight(), 0);
                return this;
            }

            private readonly PdfXObject rawImage;
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundLinearGradientClipOriginNoRepeatTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            PdfCanvas pdfCanvas = new _PdfCanvas_589(pdfDocument.AddNewPage());
            DrawContext drawContext = new DrawContext(pdfDocument, pdfCanvas);
            AbstractRenderer renderer = new DivRenderer(new Div().SetPadding(20).SetBorder(new DashedBorder(10)));
            renderer.occupiedArea = new LayoutArea(1, new Rectangle(100f, 200f, 300f, 400f));
            Rectangle targetBoundingBox = new Rectangle(50f, 150f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder(gradientBuilder).
                SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).SetBackgroundClip
                (BackgroundBox.CONTENT_BOX).SetBackgroundOrigin(BackgroundBox.BORDER_BOX).Build();
            renderer.SetProperty(Property.BACKGROUND_IMAGE, backgroundImage);
            renderer.DrawBackground(drawContext);
        }

        private sealed class _PdfCanvas_589 : PdfCanvas {
            public _PdfCanvas_589(PdfPage baseArg1)
                : base(baseArg1) {
            }

            public override PdfCanvas Rectangle(double x, double y, double width, double height) {
                NUnit.Framework.Assert.AreEqual(130.0, x, 0);
                NUnit.Framework.Assert.AreEqual(230.0, y, 0);
                NUnit.Framework.Assert.AreEqual(240.0, width, 0);
                NUnit.Framework.Assert.AreEqual(340.0, height, 0);
                return this;
            }

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
                NUnit.Framework.Assert.AreEqual(100f, rect.GetX(), 0);
                NUnit.Framework.Assert.AreEqual(200f, rect.GetY(), 0);
                NUnit.Framework.Assert.AreEqual(300f, rect.GetWidth(), 0);
                NUnit.Framework.Assert.AreEqual(400f, rect.GetHeight(), 0);
                return this;
            }
        }
    }
}
