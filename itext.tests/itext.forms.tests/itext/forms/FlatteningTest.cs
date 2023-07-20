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
using iText.Forms.Fields;
using iText.Forms.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FlatteningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FlatteningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FlatteningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FlatteningFormFieldNoSubtypeInAPTest() {
            String src = sourceFolder + "formFieldNoSubtypeInAPTest.pdf";
            String dest = destinationFolder + "flatteningFormFieldNoSubtypeInAPTest.pdf";
            String cmp = sourceFolder + "cmp_flatteningFormFieldNoSubtypeInAPTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfFormCreator.GetAcroForm(doc, false).FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.N_ENTRY_IS_REQUIRED_FOR_APPEARANCE_DICTIONARY)]
        public virtual void FormFlatteningTestWithoutNEntry() {
            String filename = "formFlatteningTestWithoutNEntry";
            String src = sourceFolder + filename + ".pdf";
            String dest = destinationFolder + filename + "_flattened.pdf";
            String cmp = sourceFolder + "cmp_" + filename + "_flattened.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfFormCreator.GetAcroForm(doc, false).FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void HiddenFieldsFlatten() {
            //TODO: Adapt assertion after DEVSIX-3079 is fixed
            String filename = "hiddenField";
            String src = sourceFolder + filename + ".pdf";
            String dest = destinationFolder + filename + "_flattened.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(document, true);
            acroForm.GetField("hiddenField").GetPdfObject().Put(PdfName.F, new PdfNumber(2));
            acroForm.FlattenFields();
            String textAfterFlatten = PdfTextExtractor.GetTextFromPage(document.GetPage(1));
            document.Close();
            NUnit.Framework.Assert.IsTrue(textAfterFlatten.Contains("hiddenFieldValue"), "Pdf does not contain the expected text"
                );
        }
    }
}
