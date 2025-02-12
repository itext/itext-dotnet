/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AnonymousBoxTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/AnonymousBoxTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/AnonymousBoxTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void RelativeHeightTest() {
            String outFileName = DESTINATION_FOLDER + "relativeHeightTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_relativeHeightTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                Document doc = new Document(pdfDocument);
                Div div = new Div();
                div.SetHeight(500);
                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
                image.SetHeight(UnitValue.CreatePercentValue(50));
                AnonymousBox ab = new AnonymousBox();
                ab.Add(image);
                div.Add(ab);
                doc.Add(div);
                doc.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }
    }
}
