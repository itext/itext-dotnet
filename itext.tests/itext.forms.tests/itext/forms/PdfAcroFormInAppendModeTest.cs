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
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAcroFormInAppendModeTest : ExtendedITextTest {
        private const String TEST_NAME = "PdfAcroFormInAppendModeTest/";

        private static readonly String DESTINATION_DIR = NUnit.Framework.TestContext.CurrentContext.TestDirectory 
            + "/test/itext/forms/" + TEST_NAME;

        private static readonly String SOURCE_DIR = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/" + TEST_NAME;

        private static readonly String INPUT_FILE_WITH_TWO_FORM_FIELDS = SOURCE_DIR + "inputFileWithTwoFormFields.pdf";

        private static readonly String INPUT_FILE_WITH_INDIRECT_FIELDS_ARRAY = SOURCE_DIR + "inputFileWithIndirectFieldsArray.pdf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_DIR);
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldTest() {
            String outputFile = "addFieldTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(INPUT_FILE_WITH_TWO_FORM_FIELDS), new PdfWriter(DESTINATION_DIR
                 + outputFile), new StampingProperties().UseAppendMode());
            PdfFormField field = new CheckBoxFormFieldBuilder(outputDoc, "checkboxname").SetWidgetRectangle(new Rectangle
                (10, 10, 24, 24)).CreateCheckBox().SetCheckType(CheckBoxType.CHECK).SetValue("On");
            PdfAcroForm.GetAcroForm(outputDoc, true).AddField(field);
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldTest() {
            String outputFile = "removeFieldTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(INPUT_FILE_WITH_TWO_FORM_FIELDS), new PdfWriter(DESTINATION_DIR
                 + outputFile), new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("textfield2");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveKidTest() {
            // Creating input document
            String inputFile = "in_removeKidTest.pdf";
            PdfDocument inDoc = new PdfDocument(new PdfWriter(DESTINATION_DIR + inputFile));
            inDoc.AddNewPage();
            PdfFormField root = new NonTerminalFormFieldBuilder(inDoc, "root").CreateNonTerminalFormField();
            PdfFormField child = new NonTerminalFormFieldBuilder(inDoc, "child").CreateNonTerminalFormField();
            root.AddKid(child);
            PdfAcroForm.GetAcroForm(inDoc, true).AddField(root);
            inDoc.Close();
            // Creating stamping document
            String outputFile = "removeKidTest.pdf";
            PdfReader reader = new PdfReader(DESTINATION_DIR + inputFile);
            PdfWriter writer = new PdfWriter(DESTINATION_DIR + outputFile);
            PdfDocument outputDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("root.child");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceFieldTest() {
            String outputFile = "replaceFieldTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(INPUT_FILE_WITH_TWO_FORM_FIELDS), new PdfWriter(DESTINATION_DIR
                 + outputFile), new StampingProperties().UseAppendMode());
            PdfFormField newField = new TextFormFieldBuilder(outputDoc, "newfield").SetWidgetRectangle(new Rectangle(20
                , 160, 100, 20)).CreateText().SetValue("new field");
            PdfAcroForm.GetAcroForm(outputDoc, true).ReplaceField("textfield1", newField);
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldToIndirectFieldsArrayTest() {
            String outputFile = "addFieldToIndirectFieldsArrayTest.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(INPUT_FILE_WITH_INDIRECT_FIELDS_ARRAY), new PdfWriter
                (DESTINATION_DIR + outputFile), new StampingProperties().UseAppendMode());
            PdfFormField field = new CheckBoxFormFieldBuilder(document, "checkboxname").SetWidgetRectangle(new Rectangle
                (10, 10, 24, 24)).CreateCheckBox().SetCheckType(CheckBoxType.CHECK).SetValue("On");
            // Get an existing acroform and add new field to it
            PdfAcroForm.GetAcroForm(document, false).AddField(field);
            document.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldFromIndirectFieldsArrayTest() {
            String outputFile = "removeFieldFromIndirectFieldsArrayTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(INPUT_FILE_WITH_INDIRECT_FIELDS_ARRAY), new PdfWriter
                (DESTINATION_DIR + outputFile), new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("textfield2");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveKidFromIndirectKidsArrayTest() {
            String inputFile = "in_removeKidFromIndirectKidsArrayTest.pdf";
            String outputFile = "removeKidFromIndirectKidsArrayTest.pdf";
            // Creating input document
            PdfDocument inDoc = new PdfDocument(new PdfWriter(DESTINATION_DIR + inputFile));
            inDoc.AddNewPage();
            PdfFormField root = new NonTerminalFormFieldBuilder(inDoc, "root").CreateNonTerminalFormField();
            PdfFormField child = new NonTerminalFormFieldBuilder(inDoc, "child").CreateNonTerminalFormField();
            root.AddKid(child);
            PdfAcroForm.GetAcroForm(inDoc, true).AddField(root);
            // make kids array indirect
            PdfAcroForm.GetAcroForm(inDoc, true).GetField("root").GetKids().MakeIndirect(inDoc);
            inDoc.Close();
            // Creating stamping document
            PdfReader reader = new PdfReader(DESTINATION_DIR + inputFile);
            PdfWriter writer = new PdfWriter(DESTINATION_DIR + outputFile);
            PdfDocument outputDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            PdfAcroForm.GetAcroForm(outputDoc, true).RemoveField("root.child");
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldToDirectAcroFormTest() {
            String inputFile = SOURCE_DIR + "inputFileWithDirectAcroForm.pdf";
            String outputFile = "addFieldToDirectAcroFormTest.pdf";
            PdfDocument outputDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(DESTINATION_DIR + outputFile
                ), new StampingProperties().UseAppendMode());
            PdfFormField field = new CheckBoxFormFieldBuilder(outputDoc, "checkboxname").SetWidgetRectangle(new Rectangle
                (10, 10, 24, 24)).CreateCheckBox().SetCheckType(CheckBoxType.CHECK).SetValue("On");
            PdfAcroForm.GetAcroForm(outputDoc, true).AddField(field);
            outputDoc.Close();
            CompareWithCmp(outputFile);
        }

        private static void CompareWithCmp(String outputFile) {
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_DIR + outputFile, SOURCE_DIR 
                + "cmp_" + outputFile, DESTINATION_DIR, "diff_"));
        }
    }
}
