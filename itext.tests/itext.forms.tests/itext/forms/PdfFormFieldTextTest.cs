/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFormFieldTextTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldTextTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldTextTest/";

        private const String TEXT = "Some text in Russian \u0442\u0435\u043A\u0441\u0442 (text)";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FillFormWithAutosizeTest() {
            String outPdf = destinationFolder + "fillFormWithAutosizeTest.pdf";
            String inPdf = sourceFolder + "fillFormWithAutosizeSource.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithAutosizeTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            fields.Get("First field").SetValue("name name name ");
            fields.Get("Second field").SetValue("surname surname surname surname surname surname");
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAppearanceExtractionForNotMergedFieldsTest() {
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "sourceDAExtractionTest.pdf"), new PdfWriter
                (destinationFolder + "defaultAppearanceExtractionTest.pdf"));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, false);
            form.GetField("First field").SetValue("Your name");
            form.GetField("Text1").SetValue("Your surname");
            doc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + "defaultAppearanceExtractionTest.pdf"
                , sourceFolder + "cmp_defaultAppearanceExtractionTest.pdf", destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FontsResourcesHelvFontTest() {
            String filename = "fontsResourcesHelvFontTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "drWithHelv.pdf"), new PdfWriter(destinationFolder
                 + filename));
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSans-Regular.ttf", PdfEncodings.IDENTITY_H);
            font.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            form.GetField("description").SetValue(TEXT, font, 12f);
            pdfDoc.Close();
            PdfDocument document = new PdfDocument(new PdfReader(destinationFolder + filename));
            PdfDictionary actualDocumentFonts = PdfAcroForm.GetAcroForm(document, false).GetPdfObject().GetAsDictionary
                (PdfName.DR).GetAsDictionary(PdfName.Font);
            // Note that we know the structure of the expected pdf file
            PdfString expectedFieldsDAFont = new PdfString("/F2 12 Tf");
            PdfObject actualFieldDAFont = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).GetAsArray
                (PdfName.Fields).GetAsDictionary(0).Get(PdfName.DA);
            NUnit.Framework.Assert.AreEqual(new PdfName("Helvetica"), actualDocumentFonts.GetAsDictionary(new PdfName(
                "F1")).Get(PdfName.BaseFont), "There is no Helvetica font within DR key");
            NUnit.Framework.Assert.AreEqual(new PdfName("NotoSans"), actualDocumentFonts.GetAsDictionary(new PdfName("F2"
                )).Get(PdfName.BaseFont), "There is no NotoSans font within DR key.");
            NUnit.Framework.Assert.AreEqual(expectedFieldsDAFont, actualFieldDAFont, "There is no NotoSans(/F2) font within Fields DA key"
                );
            document.Close();
            ExtendedITextTest.PrintOutputPdfNameAndDir(destinationFolder + filename);
        }

        [NUnit.Framework.Test]
        public virtual void FontsResourcesHelvCourierNotoFontTest() {
            String filename = "fontsResourcesHelvCourierNotoFontTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "drWithHelvAndCourier.pdf"), new PdfWriter
                (destinationFolder + filename));
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSans-Regular.ttf", PdfEncodings.IDENTITY_H);
            font.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            form.GetField("description").SetFont(font);
            form.GetField("description").SetValue(TEXT);
            pdfDoc.Close();
            PdfDocument document = new PdfDocument(new PdfReader(destinationFolder + filename));
            // Note that we know the structure of the expected pdf file
            PdfString expectedAcroformDAFont = new PdfString("/F1 0 Tf 0 g ");
            PdfString expectedFieldsDAFont = new PdfString("/F3 12 Tf");
            PdfObject actualAcroFormDAFont = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).Get
                (PdfName.DA);
            PdfDictionary actualDocumentFonts = PdfAcroForm.GetAcroForm(document, false).GetPdfObject().GetAsDictionary
                (PdfName.DR).GetAsDictionary(PdfName.Font);
            PdfObject actualFieldDAFont = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).GetAsArray
                (PdfName.Fields).GetAsDictionary(0).Get(PdfName.DA);
            NUnit.Framework.Assert.AreEqual(new PdfName("Helvetica"), actualDocumentFonts.GetAsDictionary(new PdfName(
                "F1")).Get(PdfName.BaseFont), "There is no Helvetica font within DR key");
            NUnit.Framework.Assert.AreEqual(new PdfName("Courier"), actualDocumentFonts.GetAsDictionary(new PdfName("F2"
                )).Get(PdfName.BaseFont), "There is no Courier font within DR key.");
            NUnit.Framework.Assert.AreEqual(new PdfName("NotoSans"), actualDocumentFonts.GetAsDictionary(new PdfName("F3"
                )).Get(PdfName.BaseFont), "There is no NotoSans font within DR key.");
            NUnit.Framework.Assert.AreEqual(expectedAcroformDAFont, actualAcroFormDAFont, "There is no Helvetica(/F1) font within AcroForm DA key"
                );
            NUnit.Framework.Assert.AreEqual(expectedFieldsDAFont, actualFieldDAFont, "There is no NotoSans(/F3) font within Fields DA key"
                );
            document.Close();
            ExtendedITextTest.PrintOutputPdfNameAndDir(destinationFolder + filename);
        }
    }
}
