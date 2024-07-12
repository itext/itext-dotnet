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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class NewLineTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/NewLineTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/NewLineTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
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

        private void Test(String newlineCharacters, String fileName) {
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String diffPrefix = "diff_" + fileName + "_";
            PdfDocument pdf = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(outFileName), new WriterProperties
                ().SetCompressionLevel(0)));
            Document document = new Document(pdf);
            Paragraph paragraph = new Paragraph().Add("This line is before." + newlineCharacters + "This line is after."
                );
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }
    }
}
