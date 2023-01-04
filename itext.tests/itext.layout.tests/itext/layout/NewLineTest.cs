/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
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
            PdfDocument pdf = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create), new WriterProperties
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
