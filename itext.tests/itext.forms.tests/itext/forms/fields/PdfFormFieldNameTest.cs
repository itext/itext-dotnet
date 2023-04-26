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
using System.IO;
using iText.Forms;
using iText.Forms.Logs;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfFormFieldNameTest : ExtendedITextTest {
        private PdfDocument outputDoc;

        private PdfAcroForm acroForm;

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            outputDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            outputDoc.AddNewPage();
            acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
        }

        [NUnit.Framework.TearDown]
        public virtual void Shutdown() {
            outputDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithNormalNames() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child1");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "child2");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            PdfFormField child1RecoveredField = acroForm.GetField("root.child1");
            PdfFormField child2RecoveredField = acroForm.GetField("root.child1.child2");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("root", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual("root.child1", child1RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child2, child2RecoveredField);
            NUnit.Framework.Assert.AreEqual("root.child1.child2", child2RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithNormalNamesRootIsEmpty() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child1");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "child2");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("");
            PdfFormField child1RecoveredField = acroForm.GetField(".child1");
            PdfFormField child2RecoveredField = acroForm.GetField(".child1.child2");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual(".child1", child1RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child2, child2RecoveredField);
            NUnit.Framework.Assert.AreEqual(".child1.child2", child2RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithWhiteSpaceInNames() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "ro\tot");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child 1");
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("ro\tot");
            PdfFormField child1RecoveredField = acroForm.GetField("ro\tot.child 1");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("ro\tot", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual("ro\tot.child 1", child1RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithEmptyStringAsName() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "child2");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            PdfFormField child1RecoveredField = acroForm.GetField("root.");
            PdfFormField child2RecoveredField = acroForm.GetField("root..child2");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("root", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual("root.", child1RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child2, child2RecoveredField);
            NUnit.Framework.Assert.AreEqual("root..child2", child2RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithTwoEmptyStringAsName() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            PdfFormField child1RecoveredField = acroForm.GetField("root.");
            PdfFormField child2RecoveredField = acroForm.GetField("root..");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("root", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual("root.", child1RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child2, child2RecoveredField);
            NUnit.Framework.Assert.AreEqual("root..", child2RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithTwoEmptyStringAsNameFollowedByActualName() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child3 = AddDefaultTextFormField(outputDoc, "child3");
            child2.AddKid(child3);
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            PdfFormField child1RecoveredField = acroForm.GetField("root.");
            PdfFormField child2RecoveredField = acroForm.GetField("root..");
            PdfFormField child3RecoveredField = acroForm.GetField("root...child3");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("root", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual("root.", child1RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child2, child2RecoveredField);
            NUnit.Framework.Assert.AreEqual("root..", child2RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child3, child3RecoveredField);
            NUnit.Framework.Assert.AreEqual("root...child3", child3RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithAlternatingFilledInStartWithRootFilledIn() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "child2");
            PdfFormField child3 = AddDefaultTextFormField(outputDoc, "");
            child2.AddKid(child3);
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            PdfFormField child1RecoveredField = acroForm.GetField("root.");
            PdfFormField child2RecoveredField = acroForm.GetField("root..child2");
            PdfFormField child3RecoveredField = acroForm.GetField("root..child2.");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("root", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual("root.", child1RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child2, child2RecoveredField);
            NUnit.Framework.Assert.AreEqual("root..child2", child2RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child3, child3RecoveredField);
            NUnit.Framework.Assert.AreEqual("root..child2.", child3RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithAlternatingFilledInStartWithRootEmpty() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child1");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child3 = AddDefaultTextFormField(outputDoc, "child3");
            child2.AddKid(child3);
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            PdfFormField rootRecoveredField = acroForm.GetField("");
            PdfFormField child1RecoveredField = acroForm.GetField(".child1");
            PdfFormField child2RecoveredField = acroForm.GetField(".child1.");
            PdfFormField child3RecoveredField = acroForm.GetField(".child1..child3");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(root, rootRecoveredField);
            NUnit.Framework.Assert.AreEqual("", rootRecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child1, child1RecoveredField);
            NUnit.Framework.Assert.AreEqual(".child1", child1RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child2, child2RecoveredField);
            NUnit.Framework.Assert.AreEqual(".child1.", child2RecoveredField.GetFieldName().ToString());
            NUnit.Framework.Assert.AreEqual(child3, child3RecoveredField);
            NUnit.Framework.Assert.AreEqual(".child1..child3", child3RecoveredField.GetFieldName().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldWithEmptyNameCorrectlyRemoved() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "child2");
            root.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RemoveField("root.");
            //ASSERT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            NUnit.Framework.Assert.IsFalse(rootRecoveredField.GetChildFields().Contains(child1));
            NUnit.Framework.Assert.IsTrue(rootRecoveredField.GetChildFields().Contains(child2));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldWithEmptyName2DeepCorrectlyRemoved() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child1");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            root.RemoveChild(child2);
            acroForm.RemoveField("root.child1.");
            //ASSERT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            NUnit.Framework.Assert.IsTrue(rootRecoveredField.GetChildFields().Contains(child1));
            NUnit.Framework.Assert.IsFalse(rootRecoveredField.GetChildFields().Contains(child2));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFieldWith2EmptyNamesCorrectlyRemoved() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            root.RemoveChild(child2);
            acroForm.RemoveField("root..");
            //ASSERT
            PdfFormField rootRecoveredField = acroForm.GetField("root");
            NUnit.Framework.Assert.IsTrue(rootRecoveredField.GetChildFields().Contains(child1));
            NUnit.Framework.Assert.IsFalse(rootRecoveredField.GetChildFields().Contains(child2));
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithAllEmptyNamesCorrectlyRemoved() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            root.RemoveChild(child2);
            int sizeBeforeRemoval = acroForm.GetAllFormFields().Count;
            acroForm.RemoveField("..");
            //ASSERT
            PdfFormField rootRecoveredField = acroForm.GetField("");
            NUnit.Framework.Assert.IsTrue(rootRecoveredField.GetChildFields().Contains(child1));
            NUnit.Framework.Assert.IsFalse(rootRecoveredField.GetChildFields().Contains(child2));
            NUnit.Framework.Assert.AreEqual(sizeBeforeRemoval - 1, acroForm.GetAllFormFields().Count);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFormFieldRemoveEmptyRootNoFieldsAnyMore() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            root.RemoveChild(child2);
            acroForm.RemoveField("");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(0, acroForm.GetAllFormFields().Count);
        }

        [NUnit.Framework.Test]
        public virtual void RenameFieldToEmptyNamesGetsRenamed() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child1");
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RenameField("root.child1", "");
            //ASSERT
            NUnit.Framework.Assert.AreEqual("root.", child1.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void RenameFieldWithEmptyNameGetsRenamed() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "newName");
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RenameField("root", "");
            acroForm.RenameField(".newName", "");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(".", child1.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.FIELDNAME_NOT_FOUND_OPERATION_CAN_NOT_BE_COMPLETED, Count = 1)]
        public virtual void RenameFieldWithNotFoundNameLogsWarning() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "newName");
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RenameField("root.", "");
            NUnit.Framework.Assert.AreEqual("root.newName", child1.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.FIELDNAME_NOT_FOUND_OPERATION_CAN_NOT_BE_COMPLETED, Count = 1)]
        public virtual void RenameFieldWithNotFoundNameLogsError() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "newName");
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RenameField("root", "");
            acroForm.RenameField("root.newName", "");
            //ASSERT
            NUnit.Framework.Assert.AreEqual(".newName", child1.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void RenameFieldWithEmptyName2DeepGetRenamed() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RenameField("root..", "newName");
            //ASSERT
            NUnit.Framework.Assert.AreEqual("root..newName", child2.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void RenameFieldWithEmptyNameRootDeepGetRenamed() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RenameField("..", "newName");
            //ASSERT
            NUnit.Framework.Assert.AreEqual("..newName", child2.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void RenameFieldRenameAllFromEmpty() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            acroForm.RenameField("", "newName");
            NUnit.Framework.Assert.IsNull(acroForm.GetField(""));
            NUnit.Framework.Assert.IsNotNull(acroForm.GetField("newName"));
            acroForm.RenameField("newName.", "newName");
            NUnit.Framework.Assert.AreEqual("newName.newName", child1.GetFieldName().ToUnicodeString());
            //ASSERT
            acroForm.RenameField("newName.newName.", "newName");
            NUnit.Framework.Assert.AreEqual("newName.newName.newName", child2.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void RenameMultipleTimesInLoop() {
            //ARRANGE
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "-1");
            root.AddKid(child1);
            acroForm.AddField(root);
            //ACT
            for (int i = 0; i < 100; i++) {
                acroForm.RenameField("root." + (i - 1), "" + i);
                NUnit.Framework.Assert.AreEqual("root." + i, child1.GetFieldName().ToUnicodeString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldWithEmptyNamesWork() {
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            root.AddKid(child1);
            acroForm.AddField(root);
            PdfFormField copy = acroForm.CopyField("root.");
            NUnit.Framework.Assert.IsNotNull(copy);
            NUnit.Framework.Assert.AreEqual("root.", copy.GetFieldName().ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldWithEmptyNames2DeepWork() {
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            child1.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            PdfFormField copy = acroForm.CopyField("root..");
            NUnit.Framework.Assert.IsNotNull(copy);
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceFieldReplacesItInTheChild() {
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child");
            root.AddKid(child1);
            acroForm.AddField(root);
            PdfFormField toReplace = AddDefaultTextFormField(outputDoc, "toReplace");
            acroForm.ReplaceField("root.child", toReplace);
            NUnit.Framework.Assert.IsNull(acroForm.GetField("toReplace"));
            NUnit.Framework.Assert.IsNotNull(acroForm.GetField("root.toReplace"));
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceFieldReplacesItInTheChildWithChildNameEmpty() {
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            root.AddKid(child1);
            acroForm.AddField(root);
            PdfFormField toReplace = AddDefaultTextFormField(outputDoc, "toReplace");
            acroForm.ReplaceField("root.", toReplace);
            NUnit.Framework.Assert.IsNull(acroForm.GetField("toReplace"));
            NUnit.Framework.Assert.IsNotNull(acroForm.GetField("root.toReplace"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFormFieldWithoutName() {
            MemoryStream f = new MemoryStream();
            PdfDocument originalDoc = new PdfDocument(new PdfWriter(f));
            originalDoc.AddNewPage();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            root.AddKid(child1);
            acroForm.AddField(root);
            originalDoc.Close();
            Stream i = new MemoryStream(f.ToArray());
            PdfDocument loaded = new PdfDocument(new PdfReader(i));
            using (PdfDocument newDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
                loaded.CopyPagesTo(1, 1, newDoc, pdfPageFormCopier);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(outputDoc, false);
                NUnit.Framework.Assert.IsNotNull(form);
                NUnit.Framework.Assert.AreEqual(2, form.GetAllFormFields().Count);
                NUnit.Framework.Assert.IsNotNull(form.GetField("root."));
                NUnit.Framework.Assert.IsNotNull(form.GetField("root"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyFormFieldWithoutRootName() {
            MemoryStream f = new MemoryStream();
            PdfDocument originalDoc = new PdfDocument(new PdfWriter(f));
            originalDoc.AddNewPage();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            PdfFormField root = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            root.AddKid(child1);
            acroForm.AddField(root);
            originalDoc.Close();
            Stream i = new MemoryStream(f.ToArray());
            PdfDocument loaded = new PdfDocument(new PdfReader(i));
            using (PdfDocument newDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
                loaded.CopyPagesTo(1, 1, newDoc, pdfPageFormCopier);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(outputDoc, false);
                NUnit.Framework.Assert.IsNotNull(form);
                NUnit.Framework.Assert.AreEqual(2, form.GetAllFormFields().Count);
                NUnit.Framework.Assert.IsNotNull(form.GetField("."));
                NUnit.Framework.Assert.IsNotNull(form.GetField(""));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyFormFieldWithoutNameAdded2timesOverwritesTheFirst() {
            MemoryStream f = new MemoryStream();
            PdfDocument originalDoc = new PdfDocument(new PdfWriter(f));
            originalDoc.AddNewPage();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            root.AddKid(child2);
            root.AddKid(child1);
            acroForm.AddField(root);
            originalDoc.Close();
            Stream i = new MemoryStream(f.ToArray());
            PdfDocument loaded = new PdfDocument(new PdfReader(i));
            using (PdfDocument newDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
                loaded.CopyPagesTo(1, 1, newDoc, pdfPageFormCopier);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(outputDoc, false);
                NUnit.Framework.Assert.IsNotNull(form);
                NUnit.Framework.Assert.AreEqual(2, form.GetAllFormFields().Count);
                NUnit.Framework.Assert.IsNotNull(form.GetField("root."));
                NUnit.Framework.Assert.IsNotNull(form.GetField("root"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddingSiblingsSameNameMergesFieldsTogether() {
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "child1");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "child1");
            root.AddKid(child1);
            root.AddKid(child2);
            acroForm.AddField(root);
            NUnit.Framework.Assert.AreEqual(2, root.GetChildFields().Count);
        }

        [NUnit.Framework.Test]
        public virtual void AddingSiblingsSameEmptyNamesMergesFieldsTogether() {
            PdfFormField root = AddDefaultTextFormField(outputDoc, "root");
            PdfFormField child1 = AddDefaultTextFormField(outputDoc, "");
            PdfFormField child2 = AddDefaultTextFormField(outputDoc, "");
            root.AddKid(child1);
            root.AddKid(child2);
            acroForm.AddField(root);
            NUnit.Framework.Assert.AreEqual(2, root.GetChildFields().Count);
        }

        private static PdfFormField AddDefaultTextFormField(PdfDocument doc, String name) {
            return new TextFormFieldBuilder(doc, name).SetWidgetRectangle(new Rectangle(100, 100, 100, 100)).CreateText
                ();
        }
    }
}
