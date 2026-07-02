/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class AbstractRendererUnitTest : ExtendedITextTest {
        public static IEnumerable<Object[]> ProvideZeroWidthAndHeightRectangleSizes() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { 0.0f, 40.0f }, new Object[] { 100.0f, 0.0f }, 
                new Object[] { 0.0f, 0.0f }, new Object[] { 0.00001f, 50.0f }, new Object[] { 90.0f, 0.00001f }, new Object
                [] { 0.00001f, 0.00001f } });
        }

        [NUnit.Framework.Test]
        public virtual void CreateXObjectTest() {
            StrategyBasedLinearGradientBuilder gradientBuilder = (StrategyBasedLinearGradientBuilder)new StrategyBasedLinearGradientBuilder
                ().SetGradientDirectionAsStrategy(StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_LEFT).
                AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE));
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
            AbstractRenderer renderer = new _DivRenderer_123(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_133(counter, bytes, document, 1));
            IList<BackgroundImage> images = new List<BackgroundImage>();
            images.Add(new BackgroundImage.Builder().SetImage(new _PdfImageXObject_151(ImageDataFactory.CreateRawImage
                (bytes))).Build());
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(50, counter[0]);
        }

        private sealed class _DivRenderer_123 : DivRenderer {
            public _DivRenderer_123(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(99.0f, 49.0f);
            }
        }

        private sealed class _PdfCanvas_133 : PdfCanvas {
            public _PdfCanvas_133(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

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

        private sealed class _PdfImageXObject_151 : PdfImageXObject {
            public _PdfImageXObject_151(ImageData baseArg1)
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
            AbstractRenderer renderer = new _DivRenderer_169(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_179(counter, bytes, document, 1));
            IList<BackgroundImage> images = new List<BackgroundImage>();
            images.Add(new BackgroundImage.Builder().SetImage(new _PdfImageXObject_197(ImageDataFactory.CreateRawImage
                (bytes))).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .REPEAT)).Build());
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(5, counter[0]);
        }

        private sealed class _DivRenderer_169 : DivRenderer {
            public _DivRenderer_169(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 49.0f);
            }
        }

        private sealed class _PdfCanvas_179 : PdfCanvas {
            public _PdfCanvas_179(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

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

        private sealed class _PdfImageXObject_197 : PdfImageXObject {
            public _PdfImageXObject_197(ImageData baseArg1)
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
            AbstractRenderer renderer = new _DivRenderer_217(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_227(counter, bytes, document, 1));
            IList<BackgroundImage> images = new List<BackgroundImage>();
            images.Add(new BackgroundImage.Builder().SetImage(new _PdfImageXObject_245(ImageDataFactory.CreateRawImage
                (bytes))).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .NO_REPEAT)).Build());
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(10, counter[0]);
        }

        private sealed class _DivRenderer_217 : DivRenderer {
            public _DivRenderer_217(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(99.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_227 : PdfCanvas {
            public _PdfCanvas_227(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

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

        private sealed class _PdfImageXObject_245 : PdfImageXObject {
            public _PdfImageXObject_245(ImageData baseArg1)
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
            AbstractRenderer renderer = new _DivRenderer_265(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_275(counter, bytes, document, 1));
            IList<BackgroundImage> images = new List<BackgroundImage>();
            images.Add(new BackgroundImage.Builder().SetImage(new _PdfImageXObject_293(ImageDataFactory.CreateRawImage
                (bytes))).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).
                Build());
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(1, counter[0]);
        }

        private sealed class _DivRenderer_265 : DivRenderer {
            public _DivRenderer_265(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_275 : PdfCanvas {
            public _PdfCanvas_275(int[] counter, byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.counter = counter;
                this.bytes = bytes;
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

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

        private sealed class _PdfImageXObject_293 : PdfImageXObject {
            public _PdfImageXObject_293(ImageData baseArg1)
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
            AbstractRenderer renderer = new _DivRenderer_313(new Div());
            byte[] bytes = new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_322(bytes, document, 1));
            IList<BackgroundImage> images = new List<BackgroundImage>();
            images.Add(new BackgroundImage.Builder().SetImage(new _PdfImageXObject_341(ImageDataFactory.CreateRawImage
                (bytes))).SetBackgroundPosition(new BackgroundPosition().SetXShift(new UnitValue(UnitValue.PERCENT, 30
                ))).Build());
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(context);
        }

        private sealed class _DivRenderer_313 : DivRenderer {
            public _DivRenderer_313(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_322 : PdfCanvas {
            public _PdfCanvas_322(byte[] bytes, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.bytes = bytes;
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

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

        private sealed class _PdfImageXObject_341 : PdfImageXObject {
            public _PdfImageXObject_341(ImageData baseArg1)
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
            AbstractRenderer renderer = new _DivRenderer_360(new Div());
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_368(document, 1));
            IList<BackgroundImage> images = new List<BackgroundImage>();
            StrategyBasedLinearGradientBuilder linearGradientBuilder = (StrategyBasedLinearGradientBuilder)new StrategyBasedLinearGradientBuilder
                ().AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue())).AddStopColor(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()));
            images.Add(new BackgroundImage.Builder().SetLinearGradientBuilder(linearGradientBuilder).SetBackgroundPosition
                (new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY(BackgroundPosition.PositionY
                .BOTTOM).SetYShift(UnitValue.CreatePointValue(100)).SetXShift(UnitValue.CreatePointValue(30))).Build()
                );
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(context);
        }

        private sealed class _DivRenderer_360 : DivRenderer {
            public _DivRenderer_360(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_368 : PdfCanvas {
            public _PdfCanvas_368(PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
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
            AbstractRenderer renderer = new _DivRenderer_399(new Div());
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_407(document, 1));
            IList<BackgroundImage> images = new List<BackgroundImage>();
            StrategyBasedLinearGradientBuilder linearGradientBuilder = (StrategyBasedLinearGradientBuilder)new StrategyBasedLinearGradientBuilder
                ().AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue())).AddStopColor(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()));
            images.Add(new BackgroundImage.Builder().SetLinearGradientBuilder(linearGradientBuilder).SetBackgroundPosition
                (new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY(BackgroundPosition.PositionY
                .BOTTOM).SetYShift(UnitValue.CreatePercentValue(70)).SetXShift(UnitValue.CreatePercentValue(33))).Build
                ());
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(context);
        }

        private sealed class _DivRenderer_399 : DivRenderer {
            public _DivRenderer_399(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_407 : PdfCanvas {
            public _PdfCanvas_407(PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

            public override PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, Rectangle rect) {
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
            AbstractRenderer renderer = new _DivRenderer_438(new Div());
            IList<byte[]> listBytes = JavaUtil.ArraysAsList(new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 }
                , new byte[] { 4, 15, 41, 23, 3, 2, 7, 14, 55, 27, 46, 12, 14, 14, 7, 7, 24, 25 });
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_450(listBytes, counter, document, 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, JavaUtil.ArraysAsList((BackgroundImage)new BackgroundImage.Builder
                ().SetImage(new _PdfImageXObject_467(ImageDataFactory.CreateRawImage(listBytes[1]))).Build(), (BackgroundImage
                )new BackgroundImage.Builder().SetImage(new _PdfImageXObject_479(ImageDataFactory.CreateRawImage(listBytes
                [0]))).Build()));
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(listBytes.Count, counter[0]);
        }

        private sealed class _DivRenderer_438 : DivRenderer {
            public _DivRenderer_438(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_450 : PdfCanvas {
            public _PdfCanvas_450(IList<byte[]> listBytes, int[] counter, PdfDocument baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.listBytes = listBytes;
                this.counter = counter;
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

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

        private sealed class _PdfImageXObject_467 : PdfImageXObject {
            public _PdfImageXObject_467(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 10.0f;
            }

            public override float GetHeight() {
                return 10.0f;
            }
        }

        private sealed class _PdfImageXObject_479 : PdfImageXObject {
            public _PdfImageXObject_479(ImageData baseArg1)
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
            AbstractRenderer renderer = new _DivRenderer_496(new Div());
            IList<byte[]> listBytes = JavaUtil.ArraysAsList(new byte[] { 54, 25, 47, 15, 2, 2, 2, 44, 55, 77, 86, 24 }
                , new byte[] { 4, 15, 41, 23, 3, 2, 7, 14, 55, 27, 46, 12, 14, 14, 7, 7, 24, 25 });
            float widthHeight = 10.0f;
            IList<Rectangle> listRectangles = JavaUtil.ArraysAsList(new Rectangle(81, 20, widthHeight, widthHeight), new 
                Rectangle(0, 40, widthHeight, widthHeight));
            int[] counter = new int[] { 0 };
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            DrawContext context = new DrawContext(document, new _PdfCanvas_513(listBytes, counter, listRectangles, document
                , 1));
            renderer.SetProperty(Property.BACKGROUND_IMAGE, JavaUtil.ArraysAsList((BackgroundImage)new BackgroundImage.Builder
                ().SetImage(new _PdfImageXObject_532(widthHeight, ImageDataFactory.CreateRawImage(listBytes[1]))).Build
                (), (BackgroundImage)new BackgroundImage.Builder().SetImage(new _PdfImageXObject_544(widthHeight, ImageDataFactory
                .CreateRawImage(listBytes[0]))).SetBackgroundPosition(new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX
                .RIGHT).SetPositionY(BackgroundPosition.PositionY.CENTER).SetXShift(new UnitValue(UnitValue.PERCENT, 10
                ))).Build()));
            renderer.DrawBackground(context);
            NUnit.Framework.Assert.AreEqual(listBytes.Count, counter[0]);
        }

        private sealed class _DivRenderer_496 : DivRenderer {
            public _DivRenderer_496(Div baseArg1)
                : base(baseArg1) {
            }

            public override Rectangle GetOccupiedAreaBBox() {
                return new Rectangle(100.0f, 50.0f);
            }
        }

        private sealed class _PdfCanvas_513 : PdfCanvas {
            public _PdfCanvas_513(IList<byte[]> listBytes, int[] counter, IList<Rectangle> listRectangles, PdfDocument
                 baseArg1, int baseArg2)
                : base(baseArg1, baseArg2) {
                this.listBytes = listBytes;
                this.counter = counter;
                this.listRectangles = listRectangles;
                this.@object = null;
            }

//\cond DO_NOT_DOCUMENT
            internal PdfXObject @object;
//\endcond

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

        private sealed class _PdfImageXObject_532 : PdfImageXObject {
            public _PdfImageXObject_532(float widthHeight, ImageData baseArg1)
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

        private sealed class _PdfImageXObject_544 : PdfImageXObject {
            public _PdfImageXObject_544(float widthHeight, ImageData baseArg1)
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
            PdfCanvas pdfCanvas = new _PdfCanvas_565(pdfDocument.AddNewPage());
            DrawContext drawContext = new DrawContext(pdfDocument, pdfCanvas);
            AbstractRenderer renderer = new DivRenderer(new Div().SetPadding(20).SetBorder(new DashedBorder(10)));
            renderer.occupiedArea = new LayoutArea(1, new Rectangle(100f, 200f, 300f, 400f));
            renderer.SetProperty(Property.BACKGROUND, new Background(new DeviceRgb(), 1, BackgroundBox.CONTENT_BOX));
            renderer.DrawBackground(drawContext);
        }

        private sealed class _PdfCanvas_565 : PdfCanvas {
            public _PdfCanvas_565(PdfPage baseArg1)
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
            PdfXObject rawImage = new _PdfImageXObject_586(ImageDataFactory.CreateRawImage(bytes));
            PdfCanvas pdfCanvas = new _PdfCanvas_597(rawImage, pdfDocument.AddNewPage());
            DrawContext drawContext = new DrawContext(pdfDocument, pdfCanvas);
            AbstractRenderer renderer = new DivRenderer(new Div().SetPadding(20).SetBorder(new DashedBorder(10)));
            renderer.occupiedArea = new LayoutArea(1, new Rectangle(100f, 200f, 300f, 400f));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(rawImage).SetBackgroundRepeat(new 
                BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).SetBackgroundClip(BackgroundBox.CONTENT_BOX
                ).SetBackgroundOrigin(BackgroundBox.BORDER_BOX).Build();
            IList<BackgroundImage> images = new List<BackgroundImage>();
            images.Add(backgroundImage);
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(drawContext);
        }

        private sealed class _PdfImageXObject_586 : PdfImageXObject {
            public _PdfImageXObject_586(ImageData baseArg1)
                : base(baseArg1) {
            }

            public override float GetWidth() {
                return 50f;
            }

            public override float GetHeight() {
                return 50f;
            }
        }

        private sealed class _PdfCanvas_597 : PdfCanvas {
            public _PdfCanvas_597(PdfXObject rawImage, PdfPage baseArg1)
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
            PdfCanvas pdfCanvas = new _PdfCanvas_635(pdfDocument.AddNewPage());
            DrawContext drawContext = new DrawContext(pdfDocument, pdfCanvas);
            AbstractRenderer renderer = new DivRenderer(new Div().SetPadding(20).SetBorder(new DashedBorder(10)));
            renderer.occupiedArea = new LayoutArea(1, new Rectangle(100f, 200f, 300f, 400f));
            Rectangle targetBoundingBox = new Rectangle(50f, 150f, 300f, 300f);
            LinearGradientBuilder gradientBuilder = (LinearGradientBuilder)new LinearGradientBuilder().SetGradientVector
                (targetBoundingBox.GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(
                ) - 100f, targetBoundingBox.GetTop() - 100f).SetSpread(GradientSpreadMethod.PAD).AddStopColor(new GradientColorStop
                (ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddStopColor(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder(gradientBuilder).
                SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).SetBackgroundClip
                (BackgroundBox.CONTENT_BOX).SetBackgroundOrigin(BackgroundBox.BORDER_BOX).Build();
            IList<BackgroundImage> images = new List<BackgroundImage>();
            images.Add(backgroundImage);
            renderer.SetProperty(Property.BACKGROUND_IMAGE, images);
            renderer.DrawBackground(drawContext);
        }

        private sealed class _PdfCanvas_635 : PdfCanvas {
            public _PdfCanvas_635(PdfPage baseArg1)
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

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PAGE_WAS_FLUSHED_ACTION_WILL_NOT_BE_PERFORMED)]
        public virtual void ApplyLinkAnnotationFlushedPageTest() {
            AbstractRenderer abstractRenderer = new DivRenderer(new Div());
            abstractRenderer.occupiedArea = new LayoutArea(1, new Rectangle(100, 100));
            abstractRenderer.SetProperty(Property.LINK_ANNOTATION, new PdfLinkAnnotation(new Rectangle(0, 0, 0, 0)));
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDocument.AddNewPage();
            pdfDocument.GetPage(1).Flush();
            abstractRenderer.ApplyLinkAnnotation(pdfDocument);
            // This test checks that there is log message and there is no NPE so assertions are not required
            NUnit.Framework.Assert.IsTrue(true);
        }

        [NUnit.Framework.Test]
        public virtual void NullChildTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDocument.AddNewPage();
            using (Document doc = new Document(pdfDocument)) {
                DocumentRenderer renderer = new DocumentRenderer(doc);
                DivRenderer divRenderer = new DivRenderer(new Div());
                divRenderer.childRenderers.Add(null);
                NUnit.Framework.Assert.DoesNotThrow(() => renderer.LinkRenderToDocument(divRenderer, doc.GetPdfDocument())
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void ObtainingMarginsErrorTest() {
            //TODO DEVSIX-6372 Obtaining DocumentRenderer's margins results in a ClassCastException
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Document document = new Document(doc);
            RootRenderer renderer = document.GetRenderer();
            Rectangle rect = new Rectangle(0, 0);
            NUnit.Framework.Assert.Catch(typeof(InvalidCastException), () => renderer.ApplyMargins(rect, false));
        }

        [NUnit.Framework.TestCaseSource("ProvideZeroWidthAndHeightRectangleSizes")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES, LogLevel = LogLevelConstants
            .INFO)]
        public virtual void DrawColorBackgroundWithWidthOrHeightClippedToZeroTest(float width, float height) {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                document.AddNewPage();
                DrawContext context = new DrawContext(document, new PdfCanvas(document, 1));
                float unclippedRectangleWidth = 100.0f;
                float unclippedRectangleHeight = 50.0f;
                Div div = new Div();
                div.SetWidth(unclippedRectangleWidth);
                div.SetHeight(unclippedRectangleHeight);
                div.SetPaddingRight(unclippedRectangleWidth - width);
                div.SetPaddingBottom(unclippedRectangleHeight - height);
                AbstractRenderer renderer = (AbstractRenderer)div.GetRenderer();
                Background background = new Background(ColorConstants.RED, 1, BackgroundBox.CONTENT_BOX);
                renderer.SetProperty(Property.BACKGROUND, background);
                renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(unclippedRectangleWidth, unclippedRectangleHeight
                    ))));
                renderer.DrawBackground(context);
                NUnit.Framework.Assert.AreEqual(0, document.GetPage(1).GetContentStream(0).GetBytes().Length);
            }
        }
    }
}
