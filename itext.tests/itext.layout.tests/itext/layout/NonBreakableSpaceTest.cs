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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class NonBreakableSpaceTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/NonBreakableSpaceTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/NonBreakableSpaceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleParagraphTest() {
            String outFileName = destinationFolder + "simpleParagraphTest.pdf";
            String cmpFileName = sourceFolder + "cmp_simpleParagraphTest.pdf";
            String diffPrefix = "diff_simpleParagraphTest_";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            document.Add(new Paragraph("aaa bbb\u00a0ccccccccccc").SetWidth(100).SetBorder(new SolidBorder(ColorConstants
                .RED, 10)));
            document.Add(new Paragraph("aaa bbb ccccccccccc").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 10)));
            document.Add(new Paragraph("aaaaaaa\u00a0bbbbbbbbbbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 10)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }

        [NUnit.Framework.Test]
        public virtual void ConsecutiveSpacesTest() {
            String outFileName = destinationFolder + "consecutiveSpacesTest.pdf";
            String cmpFileName = sourceFolder + "cmp_consecutiveSpacesTest.pdf";
            String diffPrefix = "diff_consecutiveSpacesTest_";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            document.Add(new Paragraph("aaa\u00a0\u00a0\u00a0bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants
                .RED, 10)));
            document.Add(new Paragraph("aaa\u00a0bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.GREEN, 10
                )));
            document.Add(new Paragraph("aaa   bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 10)));
            document.Add(new Paragraph("aaa bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLACK, 10)));
            Paragraph p = new Paragraph();
            p.Add("aaa\u00a0\u00a0\u00a0bbb").Add("ccc   ddd");
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }
    }
}
