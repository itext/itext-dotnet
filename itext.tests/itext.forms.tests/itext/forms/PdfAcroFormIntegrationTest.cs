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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAcroFormIntegrationTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfAcroFormIntegrationTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfAcroFormIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void OrphanedNamelessFormFieldTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "orphanedFormField.pdf"))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
                NUnit.Framework.Assert.AreEqual(3, form.GetRootFormFields().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MergeMergedFieldsWithTheSameNamesTest() {
            String srcFileName = SOURCE_FOLDER + "fieldMergedWithWidget.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_mergeMergedFieldsWithTheSameNames.pdf";
            String outFileName = DESTINATION_FOLDER + "mergeMergedFieldsWithTheSameNames.pdf";
            using (PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(outFileName))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(sourceDoc, true);
                NUnit.Framework.Assert.AreEqual(1, acroForm.GetFields().Size());
                NUnit.Framework.Assert.IsNull(acroForm.GetField("Field").GetKids());
                PdfFormField field = acroForm.CopyField("Field");
                field.GetPdfObject().Put(PdfName.Rect, new PdfArray(new Rectangle(210, 490, 150, 22)));
                acroForm.AddField(field);
                NUnit.Framework.Assert.AreEqual(1, acroForm.GetFields().Size());
                NUnit.Framework.Assert.AreEqual(2, acroForm.GetField("Field").GetKids().Size());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.CANNOT_MERGE_FORMFIELDS)]
        public virtual void AllowAddingFieldsWithTheSameNamesButDifferentValuesTest() {
            String cmpFileName = SOURCE_FOLDER + "cmp_fieldsWithTheSameNamesButDifferentValues.pdf";
            String outFileName = DESTINATION_FOLDER + "fieldsWithTheSameNamesButDifferentValues.pdf";
            using (PdfDocument outputDoc = new PdfDocument(new PdfWriter(outFileName))) {
                outputDoc.AddNewPage();
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(outputDoc, true);
                PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").CreateText();
                PdfFormField firstField = new TextFormFieldBuilder(outputDoc, "field").CreateText().SetValue("first");
                PdfFormField secondField = new TextFormFieldBuilder(outputDoc, "field").CreateText().SetValue("second");
                acroForm.AddField(root);
                root.AddKid(firstField);
                root.AddKid(secondField, false);
                NUnit.Framework.Assert.AreEqual(1, acroForm.GetFields().Size());
                NUnit.Framework.Assert.AreEqual(2, root.GetKids().Size());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ProcessFieldsWithTheSameNamesButDifferentValuesInReadingModeTest() {
            String srcFileName = SOURCE_FOLDER + "cmp_fieldsWithTheSameNamesButDifferentValues.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(srcFileName))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(document, true);
                NUnit.Framework.Assert.AreEqual(1, acroForm.GetFields().Size());
                PdfFormField root = acroForm.GetField("root");
                NUnit.Framework.Assert.AreEqual(2, root.GetKids().Size());
                root.GetChildField("field").SetValue("field");
                PdfFormCreator.GetAcroForm(document, true);
                // Check that fields weren't merged
                NUnit.Framework.Assert.AreEqual(2, root.GetKids().Size());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.CANNOT_MERGE_FORMFIELDS)]
        public virtual void ProcessFieldsWithTheSameNamesInWritingModeTest() {
            String srcFileName = SOURCE_FOLDER + "cmp_fieldsWithTheSameNamesButDifferentValues.pdf";
            String outFileName = DESTINATION_FOLDER + "processFieldsWithTheSameNamesInWritingMode.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(outFileName))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(document, true);
                NUnit.Framework.Assert.AreEqual(1, acroForm.GetFields().Size());
                PdfFormField root = acroForm.GetField("root");
                NUnit.Framework.Assert.AreEqual(2, root.GetKids().Size());
                root.GetChildField("field").SetValue("field");
                PdfFormCreator.GetAcroForm(document, true);
                // Check that fields were merged
                NUnit.Framework.Assert.AreEqual(1, root.GetKids().Size());
            }
        }
    }
}
