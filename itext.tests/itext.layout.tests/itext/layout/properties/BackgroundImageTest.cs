/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Reflection;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BackgroundImageTest : ExtendedITextTest {
        private const float DELTA = 0.0001f;

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BackgroundImageTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/BackgroundImageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImage() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                .GetXAxisRepeat());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                .GetYAxisRepeat());
            BackgroundImageGenericTest("backgroundImage", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void CopyConstructorTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage image = new BackgroundImage.Builder().SetImage(xObject).Build();
            FieldInfo[] imageFields = image.GetType().GetFields();
            iText.Layout.Properties.BackgroundImage copyImage = new iText.Layout.Properties.BackgroundImage(image);
            FieldInfo[] copyImageFields = copyImage.GetType().GetFields();
            NUnit.Framework.Assert.AreEqual(imageFields.Length, copyImageFields.Length);
            for (int i = 0; i < imageFields.Length; i++) {
                FieldInfo imageField = imageFields[i];
                FieldInfo copyImageField = copyImageFields[i];
                NUnit.Framework.Assert.AreEqual(imageField, copyImageField);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageClipOriginDefaultsTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            NUnit.Framework.Assert.AreEqual(BackgroundBox.BORDER_BOX, backgroundImage.GetBackgroundClip());
            NUnit.Framework.Assert.AreEqual(BackgroundBox.PADDING_BOX, backgroundImage.GetBackgroundOrigin());
            BackgroundImageGenericTest("backgroundImage", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageClipOriginTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                SetBackgroundClip(BackgroundBox.CONTENT_BOX).SetBackgroundOrigin(BackgroundBox.CONTENT_BOX).Build();
            NUnit.Framework.Assert.AreEqual(BackgroundBox.CONTENT_BOX, backgroundImage.GetBackgroundClip());
            NUnit.Framework.Assert.AreEqual(BackgroundBox.CONTENT_BOX, backgroundImage.GetBackgroundOrigin());
            BackgroundImageGenericTest("backgroundImage", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundMultipleImagesTest() {
            IList<iText.Layout.Properties.BackgroundImage> images = JavaUtil.ArraysAsList(new BackgroundImage.Builder(
                ).SetImage(new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "rock_texture.jpg"))).SetBackgroundRepeat
                (new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .REPEAT)).SetBackgroundPosition(new BackgroundPosition()).Build(), new BackgroundImage.Builder().SetImage
                (new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"))).SetBackgroundRepeat(new BackgroundRepeat
                (BackgroundRepeat.BackgroundRepeatValue.REPEAT, BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).Build
                ());
            BackgroundImageGenericTest("backgroundMultipleImages", images);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue()));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder
                (gradientBuilder).Build();
            BackgroundImageGenericTest("backgroundImageWithLinearGradient", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndPositionTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue()));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder
                (gradientBuilder).SetBackgroundPosition(new BackgroundPosition().SetYShift(UnitValue.CreatePointValue(
                30)).SetXShift(UnitValue.CreatePointValue(50))).Build();
            BackgroundImageGenericTest("backgroundImageWithLinearGradientAndPosition", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndRepeatTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue()));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder
                (gradientBuilder).SetBackgroundRepeat(new BackgroundRepeat()).Build();
            BackgroundImageGenericTest("backgroundImageWithLinearGradientAndRepeat", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndPositionAndRepeatTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue()));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder
                (gradientBuilder).SetBackgroundRepeat(new BackgroundRepeat()).SetBackgroundPosition(new BackgroundPosition
                ().SetYShift(UnitValue.CreatePointValue(30)).SetXShift(UnitValue.CreatePointValue(50))).Build();
            BackgroundImageGenericTest("backgroundImageWithLinearGradientAndPositionAndRepeat", backgroundImage);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, LogLevel = LogLevelConstants.WARN)]
        public virtual void BackgroundImageWithLinearGradientAndTransformTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue()));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder
                (gradientBuilder).Build();
            BackgroundImageGenericTest("backgroundImageWithLinearGradientAndTransform", backgroundImage, Math.PI / 4);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageForText() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                .GetXAxisRepeat());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                .GetYAxisRepeat());
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsBackgroundSpecified());
            String outFileName = DESTINATION_FOLDER + "backgroundImageForText.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageForText.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetBackgroundImage(backgroundImage);
                textElement.SetFontSize(50);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithPercentWidth() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            String outFileName = DESTINATION_FOLDER + "backgroundImageWithPercentWidth.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageWithPercentWidth.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetFontSize(50);
                backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(UnitValue.CreatePercentValue(30), null);
                textElement.SetBackgroundImage(backgroundImage);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithPercentHeight() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            String outFileName = DESTINATION_FOLDER + "backgroundImageWithPercentHeight.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageWithPercentHeight.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetFontSize(50);
                backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(null, UnitValue.CreatePercentValue(30));
                textElement.SetBackgroundImage(backgroundImage);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithPercentHeightAndWidth() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            String outFileName = DESTINATION_FOLDER + "backgroundImageWithPercentHeightAndWidth.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageWithPercentHeightAndWidth.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetFontSize(50);
                backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(UnitValue.CreatePercentValue(20), UnitValue.
                    CreatePercentValue(20));
                textElement.SetBackgroundImage(backgroundImage);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithPointWidth() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            String outFileName = DESTINATION_FOLDER + "backgroundImageWithPointWidth.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageWithPointWidth.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetFontSize(50);
                backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(UnitValue.CreatePointValue(15), null);
                textElement.SetBackgroundImage(backgroundImage);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithPointHeight() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            String outFileName = DESTINATION_FOLDER + "backgroundImageWithPointHeight.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageWithPointHeight.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetFontSize(50);
                backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(null, UnitValue.CreatePointValue(20));
                textElement.SetBackgroundImage(backgroundImage);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithPointHeightAndWidth() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            String outFileName = DESTINATION_FOLDER + "backgroundImageWithPointHeightAndWidth.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageWithPointHeightAndWidth.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetFontSize(50);
                backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(UnitValue.CreatePointValue(50), UnitValue.CreatePointValue
                    (100));
                textElement.SetBackgroundImage(backgroundImage);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLowWidthAndHeight() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            String outFileName = DESTINATION_FOLDER + "backgroundImageWithLowWidthAndHeight.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_backgroundImageWithLowWidthAndHeight.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                Text textElement = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                    );
                textElement.SetFontSize(50);
                backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(UnitValue.CreatePointValue(-1), UnitValue.CreatePointValue
                    (-1));
                textElement.SetBackgroundImage(backgroundImage);
                doc.Add(new Paragraph(textElement));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithoutRepeatXTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .REPEAT)).Build();
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                ().GetXAxisRepeat());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                .GetYAxisRepeat());
            BackgroundImageGenericTest("backgroundImageWithoutRepeatX", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithoutRepeatYTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .NO_REPEAT)).Build();
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                .GetXAxisRepeat());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                ().GetYAxisRepeat());
            BackgroundImageGenericTest("backgroundImageWithoutRepeatY", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithoutRepeatXYTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).Build();
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                ().GetXAxisRepeat());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                ().GetYAxisRepeat());
            BackgroundImageGenericTest("backgroundImageWithoutRepeatXY", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithPositionTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).SetBackgroundPosition
                (new BackgroundPosition().SetXShift(new UnitValue(UnitValue.PERCENT, 80)).SetYShift(new UnitValue(UnitValue
                .POINT, 55))).Build();
            BackgroundImageGenericTest("backgroundImageWithPosition", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImagesWithPositionTest() {
            IList<iText.Layout.Properties.BackgroundImage> images = JavaUtil.ArraysAsList(new BackgroundImage.Builder(
                ).SetImage(new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "rock_texture.jpg"))).SetBackgroundRepeat
                (new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).SetBackgroundPosition(new BackgroundPosition
                ().SetXShift(new UnitValue(UnitValue.PERCENT, 100)).SetYShift(new UnitValue(UnitValue.PERCENT, 100))).
                Build(), new BackgroundImage.Builder().SetImage(new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER
                 + "itis.jpg"))).SetBackgroundPosition(new BackgroundPosition().SetXShift(new UnitValue(UnitValue.PERCENT
                , 0)).SetYShift(new UnitValue(UnitValue.PERCENT, 100))).Build());
            BackgroundImageGenericTest("backgroundImagesWithPosition", images);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObject() {
            String filename = "backgroundXObject";
            String fileName = filename + ".pdf";
            String outFileName = DESTINATION_FOLDER + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(CreateFormXObject
                    (pdfDocument, "itis.jpg")).Build();
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                    .GetXAxisRepeat());
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                    .GetYAxisRepeat());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectWithoutRepeatX() {
            String filename = "backgroundXObjectWithoutRepeatX";
            String fileName = filename + ".pdf";
            String outFileName = DESTINATION_FOLDER + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(CreateFormXObject
                    (pdfDocument, "itis.jpg")).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue
                    .NO_REPEAT, BackgroundRepeat.BackgroundRepeatValue.REPEAT)).Build();
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                    ().GetXAxisRepeat());
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                    .GetYAxisRepeat());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectWithoutRepeatY() {
            String filename = "backgroundXObjectWithoutRepeatY";
            String fileName = filename + ".pdf";
            String outFileName = DESTINATION_FOLDER + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(CreateFormXObject
                    (pdfDocument, "itis.jpg")).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue
                    .REPEAT, BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).Build();
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundImage.GetRepeat()
                    .GetXAxisRepeat());
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                    ().GetYAxisRepeat());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectWithoutRepeatXY() {
            String filename = "backgroundXObjectWithoutRepeatXY";
            String fileName = filename + ".pdf";
            String outFileName = DESTINATION_FOLDER + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(CreateFormXObject
                    (pdfDocument, "itis.jpg")).SetBackgroundRepeat(new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue
                    .NO_REPEAT)).Build();
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                    ().GetXAxisRepeat());
                NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, backgroundImage.GetRepeat
                    ().GetYAxisRepeat());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectAndImageTest() {
            String filename = "backgroundXObjectAndImageTest";
            String fileName = filename + ".pdf";
            String outFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + filename + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                     + "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " + "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit "
                     + "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui "
                     + "officia deserunt mollit anim id est laborum. ";
                Div div = new Div().Add(new Paragraph(text + text + text));
                PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
                iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(imageXObject
                    ).Build();
                div.SetBackgroundImage(backgroundImage);
                doc.Add(div);
                iText.Layout.Properties.BackgroundImage backgroundFormXObject = new BackgroundImage.Builder().SetImage(CreateFormXObject
                    (pdfDocument, "itis.jpg")).Build();
                div = new Div().Add(new Paragraph(text + text + text));
                div.SetBackgroundImage(backgroundFormXObject);
                doc.Add(div);
                pdfDocument.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                    , "diff"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXFormObjectWithBboxTest() {
            // There shall be rock texture picture at the left top corner with 30pt width and 60pt height
            String filename = "backgroundComplicatedXFormObjectTest";
            String fileName = filename + ".pdf";
            String outFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + filename + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)))
                ) {
                Document doc = new Document(pdfDocument);
                String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                     + "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " + "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit "
                     + "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui "
                     + "officia deserunt mollit anim id est laborum. ";
                iText.Layout.Properties.BackgroundImage backgroundFormXObject = new BackgroundImage.Builder().SetBackgroundRepeat
                    (new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).SetImage(CreateFormXObject(pdfDocument
                    , "rock_texture.jpg").SetBBox(new PdfArray(new Rectangle(70, -15, 50, 75)))).Build();
                Div div = new Div().Add(new Paragraph(text + text + text));
                div.SetBackgroundImage(backgroundFormXObject);
                doc.Add(div);
                pdfDocument.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                    , "diff"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithBboxTest() {
            // There shall be default rock texture picture with 100pt width and height at the left top corner. BBox shall not do any differences.
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "rock_texture.jpg"))
                .Put(PdfName.BBox, new PdfArray(new Rectangle(70, -15, 500, 750)));
            iText.Layout.Properties.BackgroundImage image = new BackgroundImage.Builder().SetImage(xObject).SetBackgroundRepeat
                (new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT)).Build();
            BackgroundImageGenericTest("backgroundImageWithBbox", image);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndNormalBlendModeTest() {
            BlendModeTest(BlendMode.NORMAL);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndMultiplyBlendModeTest() {
            BlendModeTest(BlendMode.MULTIPLY);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndScreenBlendModeTest() {
            BlendModeTest(BlendMode.SCREEN);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndOverlayBlendModeTest() {
            BlendModeTest(BlendMode.OVERLAY);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndDarkenBlendModeTest() {
            BlendModeTest(BlendMode.DARKEN);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndLightenBlendModeTest() {
            BlendModeTest(BlendMode.LIGHTEN);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndColorDodgeBlendModeTest() {
            BlendModeTest(BlendMode.COLOR_DODGE);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndColorBurnBlendModeTest() {
            BlendModeTest(BlendMode.COLOR_BURN);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndHardLightBlendModeTest() {
            BlendModeTest(BlendMode.HARD_LIGHT);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndSoftLightBlendModeTest() {
            BlendModeTest(BlendMode.SOFT_LIGHT);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndDifferenceBlendModeTest() {
            BlendModeTest(BlendMode.DIFFERENCE);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndExclusionBlendModeTest() {
            BlendModeTest(BlendMode.EXCLUSION);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndHueBlendModeTest() {
            BlendModeTest(BlendMode.HUE);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndSaturationBlendModeTest() {
            BlendModeTest(BlendMode.SATURATION);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndColorBlendModeTest() {
            BlendModeTest(BlendMode.COLOR);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithLinearGradientAndLuminosityBlendModeTest() {
            BlendModeTest(BlendMode.LUMINOSITY);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateImageSizeTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            float[] widthAndHeight = backgroundImage.CalculateBackgroundImageSize(200f, 300f);
            iText.Test.TestUtil.AreEqual(new float[] { 45f, 45f }, widthAndHeight, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateImageSizeWithCoverPropertyTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToCover();
            float[] widthAndHeight = backgroundImage.CalculateBackgroundImageSize(200f, 300f);
            iText.Test.TestUtil.AreEqual(new float[] { 300f, 300f }, widthAndHeight, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateSizeWithContainPropertyTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToContain();
            float[] widthAndHeight = backgroundImage.CalculateBackgroundImageSize(200f, 300f);
            iText.Test.TestUtil.AreEqual(new float[] { 200f, 200.000015f }, widthAndHeight, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateSizeWithContainAndImageWeightMoreThatHeightTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToContain();
            float[] widthAndHeight = backgroundImage.CalculateBackgroundImageSize(200f, 300f);
            iText.Test.TestUtil.AreEqual(new float[] { 200f, 112.5f }, widthAndHeight, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateSizeWithCoverAndImageWeightMoreThatHeightTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).
                Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToCover();
            float[] widthAndHeight = backgroundImage.CalculateBackgroundImageSize(200f, 300f);
            iText.Test.TestUtil.AreEqual(new float[] { 533.3333f, 300f }, widthAndHeight, DELTA);
        }

        private void BlendModeTest(BlendMode blendMode) {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().AddColorStop(new 
                GradientColorStop(ColorConstants.BLACK.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants
                .WHITE.GetColorValue()));
            iText.Layout.Properties.BackgroundImage backgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder
                (gradientBuilder).Build();
            AbstractLinearGradientBuilder topGradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_RIGHT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue())).AddColorStop
                (new GradientColorStop(ColorConstants.BLUE.GetColorValue()));
            iText.Layout.Properties.BackgroundImage topBackgroundImage = new BackgroundImage.Builder().SetLinearGradientBuilder
                (topGradientBuilder).SetBackgroundBlendMode(blendMode).Build();
            BackgroundImageGenericTest("backgroundImageWithLinearGradientAndBlendMode_" + blendMode.GetPdfRepresentation
                ().GetValue(), JavaUtil.ArraysAsList(topBackgroundImage, backgroundImage));
        }

        private PdfFormXObject CreateFormXObject(PdfDocument pdfDocument, String pictureName) {
            ImageData image = ImageDataFactory.Create(SOURCE_FOLDER + pictureName);
            PdfFormXObject template = new PdfFormXObject(new Rectangle(image.GetWidth(), image.GetHeight()));
            PdfCanvas canvas = new PdfCanvas(template, pdfDocument);
            canvas.AddImageFittedIntoRectangle(image, new Rectangle(0, 0, image.GetWidth(), image.GetHeight()), false)
                .Flush();
            canvas.Release();
            template.Flush();
            return template;
        }

        private void BackgroundImageGenericTest(String filename, Object backgroundImage) {
            BackgroundImageGenericTest(filename, backgroundImage, null);
        }

        private void BackgroundImageGenericTest(String filename, Object backgroundImage, double? angle) {
            if (backgroundImage is iText.Layout.Properties.BackgroundImage) {
                NUnit.Framework.Assert.IsTrue(((iText.Layout.Properties.BackgroundImage)backgroundImage).IsBackgroundSpecified
                    ());
            }
            else {
                foreach (iText.Layout.Properties.BackgroundImage image in (IList<iText.Layout.Properties.BackgroundImage>)
                    backgroundImage) {
                    NUnit.Framework.Assert.IsTrue((image).IsBackgroundSpecified());
                }
            }
            String outFileName = DESTINATION_FOLDER + filename + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + filename + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName)));
            Document doc = new Document(pdfDocument);
            String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                 + "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " + "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit "
                 + "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui "
                 + "officia deserunt mollit anim id est laborum. ";
            Div div = new Div().Add(new Paragraph(text + text + text));
            if (angle != null) {
                div.SetRotationAngle(angle.Value);
            }
            if (backgroundImage is iText.Layout.Properties.BackgroundImage) {
                div.SetBackgroundImage((iText.Layout.Properties.BackgroundImage)backgroundImage);
            }
            else {
                div.SetBackgroundImage((IList<iText.Layout.Properties.BackgroundImage>)backgroundImage);
            }
            doc.Add(div);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private void BackgroundXObjectGenericTest(String filename, iText.Layout.Properties.BackgroundImage backgroundImage
            , PdfDocument pdfDocument) {
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsBackgroundSpecified());
            String outFileName = DESTINATION_FOLDER + filename + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + filename + ".pdf";
            Document doc = new Document(pdfDocument);
            String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                 + "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " + "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit "
                 + "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui "
                 + "officia deserunt mollit anim id est laborum. ";
            Div div = new Div().Add(new Paragraph(text + text + text));
            div.SetBackgroundImage(backgroundImage);
            doc.Add(div);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }
    }
}
