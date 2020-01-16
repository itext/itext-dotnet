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
        private const String testFolder = "PdfAcroFormInAppendModeTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/" + testFolder;

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/" + testFolder;

        private static readonly String inputFile = destinationFolder + "inputFile.pdf";

        private static readonly String inputFileWithIndirectFieldsArray = destinationFolder + "inputFileWithIndirectFieldsArray.pdf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
            CreateInputFile();
            CreateInputFileWithIndirectFieldsArray();
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldTest() {
            String outputFile = "addFieldTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(destinationFolder + outputFile
                ), new StampingProperties().UseAppendMode());
            PdfFormField field = PdfFormField.CreateCheckBox(outputDoc, new Rectangle(10, 10, 24, 24), "checkboxname", 
                "On", PdfFormField.TYPE_CHECK);
            PdfAcroForm.GetAcroForm(outputDoc, true).AddField(field);
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldTest() {
            String outputFile = "removeFieldTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(destinationFolder + outputFile
                ), new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("textfield2");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveKidTest() {
            // Creating input document
            String inputFile = "in_removeKidTest.pdf";
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
            String outputFile = "removeKidTest.pdf";
            PdfReader reader = new PdfReader(destinationFolder + inputFile);
            PdfWriter writer = new PdfWriter(destinationFolder + outputFile);
            PdfDocument outputDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("root.child");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceFieldTest() {
            String outputFile = "replaceFieldTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(destinationFolder + outputFile
                ), new StampingProperties().UseAppendMode());
            PdfFormField newField = PdfFormField.CreateText(outputDoc, new Rectangle(20, 160, 100, 20), "newfield", "new field"
                );
            PdfAcroForm.GetAcroForm(outputDoc, true).ReplaceField("textfield1", newField);
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldToIndirectFieldsArrayTest() {
            String outputFile = "addFieldToIndirectFieldsArrayTest.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(inputFileWithIndirectFieldsArray), new PdfWriter(destinationFolder
                 + outputFile), new StampingProperties().UseAppendMode());
            PdfFormField field = PdfFormField.CreateCheckBox(document, new Rectangle(10, 10, 24, 24), "checkboxname", 
                "On", PdfFormField.TYPE_CHECK);
            // Get an existing acroform and add new field to it
            PdfAcroForm.GetAcroForm(document, false).AddField(field);
            document.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldFromIndirectFieldsArrayTest() {
            String outputFile = "removeFieldFromIndirectFieldsArrayTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(inputFileWithIndirectFieldsArray), new PdfWriter(destinationFolder
                 + outputFile), new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("textfield2");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveKidFromIndirectKidsArrayTest() {
            String inputFile = "in_removeKidFromIndirectKidsArrayTest.pdf";
            String outputFile = "removeKidFromIndirectKidsArrayTest.pdf";
            // Creating input document
            PdfDocument inDoc = new PdfDocument(new PdfWriter(destinationFolder + inputFile));
            inDoc.AddNewPage();
            PdfFormField root = PdfFormField.CreateEmptyField(inDoc);
            root.SetFieldName("root");
            PdfFormField child = PdfFormField.CreateEmptyField(inDoc);
            child.SetFieldName("child");
            root.AddKid(child);
            PdfAcroForm.GetAcroForm(inDoc, true).AddField(root);
            // make kids array indirect
            PdfAcroForm.GetAcroForm(inDoc, true).GetField("root").GetKids().MakeIndirect(inDoc);
            inDoc.Close();
            // Creating stamping document
            PdfReader reader = new PdfReader(destinationFolder + inputFile);
            PdfWriter writer = new PdfWriter(destinationFolder + outputFile);
            PdfDocument outputDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("root.child");
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

        private static void CreateInputFileWithIndirectFieldsArray() {
            PdfDocument document = new PdfDocument(new PdfWriter(inputFileWithIndirectFieldsArray));
            document.AddNewPage();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            acroForm.GetFields().MakeIndirect(document);
            acroForm.AddField(PdfFormField.CreateText(document, new Rectangle(20, 160, 100, 20), "textfield1", "text1"
                ));
            acroForm.AddField(PdfFormField.CreateText(document, new Rectangle(20, 130, 100, 20), "textfield2", "text2"
                ));
            document.Close();
        }

        private static void CompareWithCmp(String outputFile) {
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + outputFile, sourceFolder
                 + "cmp_" + outputFile, destinationFolder, "diff_"));
        }
    }
}
