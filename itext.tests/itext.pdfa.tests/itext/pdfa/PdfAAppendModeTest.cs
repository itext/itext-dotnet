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
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAAppendModeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public const String testDirName = "PdfAAppendModeTest/";

        public static readonly String cmpFolder = sourceFolder + "cmp/" + testDirName;

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/" + testDirName;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddPageInAppendModeTest() {
            String inputFile = destinationFolder + "in_addPageInAppendModeTest.pdf";
            String outputFile = destinationFolder + "out_addPageInAppendModeTest.pdf";
            String cmpFile = cmpFolder + "cmp_addPageInAppendModeTest.pdf";
            CreateInputPdfADocument(inputFile);
            PdfDocument pdfADocument = new PdfADocument(new PdfReader(inputFile), new PdfWriter(outputFile), new StampingProperties
                ().UseAppendMode());
            PdfCanvas canvas = new PdfCanvas(pdfADocument.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf"
                , PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED), 16).ShowText("This page 2").EndText().RestoreState
                ();
            canvas.Release();
            pdfADocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(inputFile));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outputFile));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        private static void CreateInputPdfADocument(String docName) {
            PdfWriter writer = new PdfWriter(docName);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(sourceFolder + "sRGB Color Space Profile.icm"
                , FileMode.Open, FileAccess.Read)));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf"
                , PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED), 16).ShowText("This page 1").EndText().RestoreState
                ();
            canvas.Release();
            pdfDoc.Close();
        }
    }
}
