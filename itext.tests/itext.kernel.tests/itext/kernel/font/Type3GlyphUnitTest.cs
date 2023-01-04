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
using System.IO;
using iText.IO.Image;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class Type3GlyphUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/Type3GlyphUnitTest/";

        [NUnit.Framework.Test]
        public virtual void AddImageWithoutMaskTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Type3Glyph glyph = new Type3Glyph(new PdfStream(), pdfDoc);
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "imageTest.png");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => glyph.AddImageWithTransformationMatrix
                (img, 100, 0, 0, 100, 0, 0, false));
            NUnit.Framework.Assert.AreEqual("Not colorized type3 fonts accept only mask images.", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AddInlineImageMaskTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Type3Glyph glyph = new Type3Glyph(new PdfStream(), pdfDoc);
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "imageTest.png");
            img.MakeMask();
            NUnit.Framework.Assert.IsNull(glyph.AddImageWithTransformationMatrix(img, 100, 0, 0, 100, 0, 0, true));
        }

        [NUnit.Framework.Test]
        public virtual void AddImageMaskAsNotInlineTest() {
            //TODO DEVSIX-5764 Display message error for non-inline images in type 3 glyph
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Type3Glyph glyph = new Type3Glyph(new PdfStream(), pdfDoc);
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "imageTest.png");
            img.MakeMask();
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => glyph.AddImageWithTransformationMatrix(
                img, 100, 0, 0, 100, 0, 0, false));
        }
    }
}
