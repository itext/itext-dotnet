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
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class NewLineTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/NewLineTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/layout/NewLineTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void R() {
            Test("\r", "r.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void N() {
            Test("\n", "n.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void Rn() {
            Test("\r\n", "rn.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void Rrn() {
            Test("\r\r\n", "rrn.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void Nn() {
            Test("\n\n", "nn.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void Rnn() {
            Test("\r\n\n", "rnn.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void Rnrn() {
            Test("\r\n\r\n", "rnrn.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void DotAfterNTest() {
            Test("\n", "0123", ".com", "ndot.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void DotAfterRNTest() {
            Test("\r\n", "0123", ".com", "rndot.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void DotAfterNRTest() {
            Test("\n\r", "0123", ".com", "nrdot.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void DotAfterRTest() {
            Test("\r", "0123", ".com", "rdot.pdf");
        }

        private void Test(String newlineCharacters, String fileName) {
            Test(newlineCharacters, "This line is before.", "This line is after.", fileName);
        }

        private void Test(String newlineCharacters, String pre, String post, String fileName) {
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String diffPrefix = "diff_" + fileName + "_";
            PdfDocument pdf = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties().SetCompressionLevel
                (0)));
            Document document = new Document(pdf);
            Paragraph paragraph = new Paragraph().Add(new StringBuilder(pre).Append(newlineCharacters).Append(post).ToString
                ());
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }
    }
}
