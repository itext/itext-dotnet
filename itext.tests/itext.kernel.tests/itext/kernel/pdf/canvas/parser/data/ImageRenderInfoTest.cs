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
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    [NUnit.Framework.Category("UnitTest")]
    public class ImageRenderInfoTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/data/" + "ImageRenderInfoTest/";

        private readonly float EPS = 0.001f;

        [NUnit.Framework.Test]
        public virtual void CheckImageRenderInfoTest() {
            String source_image = SOURCE_FOLDER + "simple.tif";
            PdfImageXObject image = new PdfImageXObject(ImageDataFactory.Create(source_image));
            PdfStream imageStream = image.GetPdfObject();
            Matrix matrix = new Matrix(2, 0.5f, 0, 2, 0.5f, 0);
            Stack<CanvasTag> tagHierarchy = new Stack<CanvasTag>();
            tagHierarchy.Push(new CanvasTag(new PdfName("tag"), 2));
            ImageRenderInfo imageRenderInfo = new ImageRenderInfo(tagHierarchy, new ImageRenderInfoTest.TestGraphicsState
                (this), matrix, imageStream, new PdfName("Im1"), new PdfDictionary(), true);
            NUnit.Framework.Assert.IsTrue(imageRenderInfo.IsInline());
            NUnit.Framework.Assert.AreEqual(image.GetWidth(), imageRenderInfo.GetImage().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual("/Im1", imageRenderInfo.GetImageResourceName().ToString());
            NUnit.Framework.Assert.AreEqual(new Vector(0.5f, 0, 1), imageRenderInfo.GetStartPoint());
            NUnit.Framework.Assert.AreEqual(matrix, imageRenderInfo.GetImageCtm());
            NUnit.Framework.Assert.AreEqual(4, imageRenderInfo.GetArea(), EPS);
            NUnit.Framework.Assert.AreEqual(0, imageRenderInfo.GetColorSpaceDictionary().Size());
            NUnit.Framework.Assert.AreEqual(1, imageRenderInfo.GetCanvasTagHierarchy().Count);
            NUnit.Framework.Assert.IsTrue(imageRenderInfo.HasMcid(2, true));
            NUnit.Framework.Assert.IsTrue(imageRenderInfo.HasMcid(2));
            NUnit.Framework.Assert.IsFalse(imageRenderInfo.HasMcid(1));
            NUnit.Framework.Assert.AreEqual(2, imageRenderInfo.GetMcid());
        }

        private class TestGraphicsState : CanvasGraphicsState {
            protected internal TestGraphicsState(ImageRenderInfoTest _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly ImageRenderInfoTest _enclosing;
        }
    }
}
