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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EmptyNestedTableTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/EmptyNestedTableTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/EmptyNestedTableTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildEmptyTable() {
            //TODO DEVSIX-6255 Implementation of the application border attributes for cells
            String outFileName = destinationFolder + "emptyNestedTableTest.pdf";
            String cmpFileName = sourceFolder + "cmp_emptyNestedTableTest.pdf";
            // setup document
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document layoutDocument = new Document(pdfDocument);
            // add table to document
            Table x = new Table(new float[] { 1f }).AddCell(new Cell().Add(new Table(new float[] { 1f })));
            layoutDocument.Add(x);
            // close document
            layoutDocument.Close();
            // compare
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
