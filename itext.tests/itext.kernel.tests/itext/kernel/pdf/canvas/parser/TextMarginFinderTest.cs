/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextMarginFinderTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/TextMarginFinderTest/";

        [NUnit.Framework.Test]
        public virtual void Test() {
            TextMarginFinder finder = new TextMarginFinder();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "in.pdf"));
            new PdfCanvasProcessor(finder).ProcessPageContent(pdfDocument.GetPage(1));
            Rectangle textRect = finder.GetTextRectangle();
            NUnit.Framework.Assert.AreEqual(1.42f * 72f, textRect.GetX(), 0.01f);
            NUnit.Framework.Assert.AreEqual(7.42f * 72f, textRect.GetX() + textRect.GetWidth(), 0.01f);
            NUnit.Framework.Assert.AreEqual(2.42f * 72f, textRect.GetY(), 0.01f);
            NUnit.Framework.Assert.AreEqual(10.42f * 72f, textRect.GetY() + textRect.GetHeight(), 0.01f);
        }
    }
}
