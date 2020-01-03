/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class PdfAcroFormInAppendModeTest : ExtendedITextTest {
        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfAcroFormInAppendModeTest/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfAcroFormInAppendModeTest/";

        private static readonly String inputFile = destinationFolder + "inputFile.pdf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
            CreateInputFile();
        }

        [NUnit.Framework.Test]
        public virtual void CreateFieldInAppendModeTest() {
            String outputFile = "createFieldInAppendModeTest.pdf";
            PdfDocument outputDoc = CreateDocInAppendMode(destinationFolder + outputFile);
            PdfFormField field = PdfFormField.CreateCheckBox(outputDoc, new Rectangle(10, 10, 24, 24), "checkboxname", 
                "On", PdfFormField.TYPE_CHECK);
            PdfAcroForm.GetAcroForm(outputDoc, true).AddField(field);
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldInAppendModeTest() {
            String outputFile = "removeFieldInAppendModeTest.pdf";
            PdfDocument outputDoc = CreateDocInAppendMode(destinationFolder + outputFile);
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("textfield2");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldWithParentInAppendModeTest() {
            // Creating input document
            String inputFile = "inputRemoveFieldWithParentInAppendModeTest.pdf";
            PdfDocument inDoc = new PdfDocument(new PdfWriter(destinationFolder + inputFile));
            inDoc.AddNewPage();
            PdfFormField root = PdfFormField.CreateEmptyField(inDoc);
            root.SetFieldName("root");
            PdfFormField child = PdfFormField.CreateEmptyField(inDoc);
            child.SetFieldName("child");
            root.AddKid(child);
            PdfAcroForm.GetAcroForm(inDoc, true).AddField(root);
            inDoc.Close();
            // Creating stamping document
            String outputFile = "removeFieldWithParentInAppendModeTest.pdf";
            PdfReader reader = new PdfReader(destinationFolder + inputFile);
            PdfWriter writer = new PdfWriter(destinationFolder + outputFile);
            PdfDocument outputDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("root.child");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceFieldInAppendModeTest() {
            String outputFile = "replaceFieldInAppendModeTest.pdf";
            PdfDocument outputDoc = CreateDocInAppendMode(destinationFolder + outputFile);
            PdfFormField newField = PdfFormField.CreateText(outputDoc, new Rectangle(20, 160, 100, 20), "newfield", "new field"
                );
            PdfAcroForm.GetAcroForm(outputDoc, true).ReplaceField("textfield1", newField);
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        private static void CreateInputFile() {
            PdfDocument document = new PdfDocument(new PdfWriter(inputFile));
            document.AddNewPage();
            PdfAcroForm.GetAcroForm(document, true).AddField(PdfFormField.CreateText(document, new Rectangle(20, 160, 
                100, 20), "textfield1", "text1"));
            PdfAcroForm.GetAcroForm(document, true).AddField(PdfFormField.CreateText(document, new Rectangle(20, 130, 
                100, 20), "textfield2", "text2"));
            document.Close();
        }

        private static PdfDocument CreateDocInAppendMode(String outFile) {
            PdfReader reader = new PdfReader(inputFile);
            PdfWriter writer = new PdfWriter(outFile);
            return new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
        }

        private static void CompareWithCmp(String outputFile) {
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + outputFile, sourceFolder
                 + "cmp_" + outputFile, destinationFolder, "diff_"));
        }
    }
}
