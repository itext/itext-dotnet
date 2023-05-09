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
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAStampingModeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAStampingModeTest/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfAStampingModeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1FieldStampingModeTest01() {
            String fileName = "pdfA1FieldStampingModeTest01.pdf";
            PdfADocument pdfDoc = new PdfADocument(new PdfReader(sourceFolder + "pdfs/pdfA1DocumentWithPdfA1Fields01.pdf"
                ), new PdfWriter(destinationFolder + fileName));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, false);
            form.GetField("checkBox").SetValue("0");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(destinationFolder + fileName, cmpFolder + "cmp_"
                 + fileName, destinationFolder, "diff_"));
            NUnit.Framework.Assert.IsNull(compareTool.CompareXmp(destinationFolder + fileName, cmpFolder + "cmp_" + fileName
                , true));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(destinationFolder + fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA2FieldStampingModeTest01() {
            String fileName = "pdfA2FieldStampingModeTest01.pdf";
            PdfADocument pdfDoc = new PdfADocument(new PdfReader(sourceFolder + "pdfs/pdfA2DocumentWithPdfA2Fields01.pdf"
                ), new PdfWriter(destinationFolder + fileName));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, false);
            form.GetField("checkBox").SetValue("0");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(destinationFolder + fileName, cmpFolder + "cmp_"
                 + fileName, destinationFolder, "diff_"));
            NUnit.Framework.Assert.IsNull(compareTool.CompareXmp(destinationFolder + fileName, cmpFolder + "cmp_" + fileName
                , true));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(destinationFolder + fileName));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    }
}
