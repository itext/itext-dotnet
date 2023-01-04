/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class BackgroundSizeCalculationUtilUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BackgroundImageTest/";

        private const float delta = 0.0001f;

        [NUnit.Framework.Test]
        public virtual void CalculateImageSizeTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).Build();
            float[] widthAndHeight = BackgroundSizeCalculationUtil.CalculateBackgroundImageSize(backgroundImage, 200f, 
                300f);
            iText.Test.TestUtil.AreEqual(new float[] { 45f, 45f }, widthAndHeight, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateImageSizeWithCoverPropertyTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToCover();
            float[] widthAndHeight = BackgroundSizeCalculationUtil.CalculateBackgroundImageSize(backgroundImage, 200f, 
                300f);
            iText.Test.TestUtil.AreEqual(new float[] { 300f, 300f }, widthAndHeight, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateSizeWithContainPropertyTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "pattern-grg-rrg-rgg.png"
                ));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToContain();
            float[] widthAndHeight = BackgroundSizeCalculationUtil.CalculateBackgroundImageSize(backgroundImage, 200f, 
                300f);
            iText.Test.TestUtil.AreEqual(new float[] { 200f, 200.000015f }, widthAndHeight, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateSizeWithContainAndImageWeightMoreThatHeightTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToContain();
            float[] widthAndHeight = BackgroundSizeCalculationUtil.CalculateBackgroundImageSize(backgroundImage, 200f, 
                300f);
            iText.Test.TestUtil.AreEqual(new float[] { 200f, 112.5f }, widthAndHeight, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateSizeWithCoverAndImageWeightMoreThatHeightTest() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
            BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).Build();
            backgroundImage.GetBackgroundSize().SetBackgroundSizeToCover();
            float[] widthAndHeight = BackgroundSizeCalculationUtil.CalculateBackgroundImageSize(backgroundImage, 200f, 
                300f);
            iText.Test.TestUtil.AreEqual(new float[] { 533.3333f, 300f }, widthAndHeight, delta);
        }
    }
}
