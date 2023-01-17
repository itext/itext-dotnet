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
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfAcroFormTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfAcroFormTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfAcroFormTest/";

        [NUnit.Framework.Test]
        public virtual void SetSignatureFlagsTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                acroForm.SetSignatureFlags(65);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject sigFlags = acroForm.GetPdfObject().Get(PdfName.SigFlags);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(new PdfNumber(65), sigFlags);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddChildToFormFieldTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfFormField field = new TextFormFieldBuilder(outputDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                    , 200, 20)).CreateText();
                acroForm.AddField(field);
                PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").SetWidgetRectangle(new Rectangle(100, 600, 
                    200, 20)).CreateText();
                PdfFormField child = new TextFormFieldBuilder(outputDoc, "child").SetWidgetRectangle(new Rectangle(100, 300
                    , 200, 20)).CreateText();
                root.AddKid(child);
                acroForm.AddField(root);
                NUnit.Framework.Assert.AreEqual(2, acroForm.fields.Count);
                PdfArray fieldKids = root.GetKids();
                NUnit.Framework.Assert.AreEqual(2, fieldKids.Size());
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddChildToWidgetTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfArray options = new PdfArray();
                options.Add(new PdfString("1"));
                options.Add(new PdfString("2"));
                PdfTextFormField text = new TextFormFieldBuilder(outputDoc, "text").SetWidgetRectangle(new Rectangle(36, 696
                    , 20, 20)).CreateText();
                PdfTextFormField childText = new TextFormFieldBuilder(outputDoc, "childText").SetWidgetRectangle(new Rectangle
                    (36, 696, 20, 20)).CreateText();
                text.AddKid(childText);
                acroForm.AddField(text);
                NUnit.Framework.Assert.AreEqual(1, acroForm.fields.Count);
                IList<PdfFormField> fieldKids = text.GetChildFields();
                NUnit.Framework.Assert.AreEqual(2, fieldKids.Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldChildTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfFormField field = new TextFormFieldBuilder(outputDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                    , 200, 20)).CreateText();
                acroForm.AddField(field);
                PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").SetWidgetRectangle(new Rectangle(100, 600, 
                    200, 20)).CreateText();
                PdfFormField child = new TextFormFieldBuilder(outputDoc, "child").SetWidgetRectangle(new Rectangle(100, 600
                    , 200, 20)).CreateText();
                root.AddKid(child);
                acroForm.AddField(root);
                PdfFormField childField = acroForm.GetField("root.child");
                NUnit.Framework.Assert.AreEqual("root.child", childField.GetFieldName().ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldWithEqualChildNamesTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfFormField field = new TextFormFieldBuilder(outputDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                    , 200, 20)).CreateText();
                acroForm.AddField(field);
                PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").SetWidgetRectangle(new Rectangle(100, 600, 
                    200, 20)).CreateText();
                PdfFormField child = new TextFormFieldBuilder(outputDoc, "field").SetWidgetRectangle(new Rectangle(100, 300
                    , 200, 20)).CreateText();
                PdfFormField child1 = new TextFormFieldBuilder(outputDoc, "field").SetWidgetRectangle(new Rectangle(100, 300
                    , 200, 20)).CreateText();
                PdfFormField child2 = new TextFormFieldBuilder(outputDoc, "another_name").SetWidgetRectangle(new Rectangle
                    (100, 300, 200, 20)).CreateText();
                child1.AddKid(child2);
                child.AddKid(child1);
                root.AddKid(child);
                acroForm.AddField(root);
                PdfFormField childField = acroForm.GetField("root.field.field.another_name");
                NUnit.Framework.Assert.AreEqual("root.field.field.another_name", childField.GetFieldName().ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ChangeFieldNameTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfFormField field = new TextFormFieldBuilder(outputDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                    , 200, 20)).CreateText();
                acroForm.AddField(field);
                PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").SetWidgetRectangle(new Rectangle(100, 600, 
                    200, 20)).CreateText();
                PdfFormField child = new TextFormFieldBuilder(outputDoc, "child").SetWidgetRectangle(new Rectangle(100, 300
                    , 200, 20)).CreateText();
                root.AddKid(child);
                acroForm.AddField(root);
                acroForm.GetField("root").SetFieldName("diff");
                PdfFormField childField = PdfAcroForm.GetAcroForm(outputDoc, true).GetField("diff.child");
                NUnit.Framework.Assert.AreEqual("diff.child", childField.GetFieldName().ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemoveChildFromFormFieldTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                outputDoc.AddNewPage();
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfFormField field = new TextFormFieldBuilder(outputDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                    , 200, 20)).CreateText().SetValue("text1");
                acroForm.AddField(field);
                PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").SetWidgetRectangle(new Rectangle(100, 600, 
                    200, 20)).CreateText().SetValue("root");
                PdfFormField child = new TextFormFieldBuilder(outputDoc, "child").SetWidgetRectangle(new Rectangle(100, 300
                    , 200, 20)).CreateText().SetValue("child");
                PdfFormField child1 = new TextFormFieldBuilder(outputDoc, "aaaaa").SetWidgetRectangle(new Rectangle(100, 400
                    , 200, 20)).CreateText().SetValue("aaaaa");
                PdfFormField child2 = new TextFormFieldBuilder(outputDoc, "bbbbb").SetWidgetRectangle(new Rectangle(100, 500
                    , 200, 20)).CreateText().SetValue("bbbbb");
                child1.AddKid(child2);
                child.AddKid(child1);
                root.AddKid(child);
                acroForm.AddField(root);
                acroForm.RemoveField("root.child.aaaaa");
                NUnit.Framework.Assert.AreEqual(2, acroForm.fields.Count);
                NUnit.Framework.Assert.AreEqual(2, root.GetKids().Size());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetChildFromFormFieldWithDifferentAmountOfChildrenTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                outputDoc.AddNewPage();
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfFormField field = new TextFormFieldBuilder(outputDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                    , 200, 20)).CreateText().SetValue("text1");
                acroForm.AddField(field);
                PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").SetWidgetRectangle(new Rectangle(100, 600, 
                    200, 20)).CreateText().SetValue("root");
                PdfFormField child = new TextFormFieldBuilder(outputDoc, "child").SetWidgetRectangle(new Rectangle(100, 300
                    , 200, 20)).CreateText().SetValue("child");
                PdfFormField child1 = new TextFormFieldBuilder(outputDoc, "aaaaa").SetWidgetRectangle(new Rectangle(100, 400
                    , 200, 20)).CreateText().SetValue("aaaaa");
                PdfFormField child2 = new TextFormFieldBuilder(outputDoc, "bbbbb").SetWidgetRectangle(new Rectangle(100, 500
                    , 200, 20)).CreateText().SetValue("bbbbb");
                PdfFormField child3 = new TextFormFieldBuilder(outputDoc, "child1").SetWidgetRectangle(new Rectangle(100, 
                    500, 200, 20)).CreateText().SetValue("child1");
                PdfFormField child4 = new TextFormFieldBuilder(outputDoc, "child2").SetWidgetRectangle(new Rectangle(100, 
                    500, 200, 20)).CreateText().SetValue("child2");
                PdfFormField child5 = new TextFormFieldBuilder(outputDoc, "child2").SetWidgetRectangle(new Rectangle(100, 
                    500, 200, 20)).CreateText().SetValue("child2");
                child1.AddKid(child2);
                child1.AddKid(child3);
                child1.AddKid(child4);
                child4.AddKid(child5);
                child.AddKid(child1);
                root.AddKid(child);
                acroForm.AddField(root);
                PdfFormField childField = acroForm.GetField("root.child.aaaaa.child2");
                NUnit.Framework.Assert.AreEqual("root.child.aaaaa.child2", childField.GetFieldName().ToString());
                NUnit.Framework.Assert.AreEqual(2, acroForm.fields.Count);
                NUnit.Framework.Assert.AreEqual(2, root.GetKids().Size());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetCalculationOrderTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfArray calculationOrderArray = new PdfArray(new int[] { 1, 0 });
                acroForm.SetCalculationOrder(calculationOrderArray);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject calculationOrder = acroForm.GetPdfObject().Get(PdfName.CO);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(calculationOrderArray, calculationOrder);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultAppearanceTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                acroForm.SetDefaultAppearance("default appearance");
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject calculationOrder = acroForm.GetPdfObject().Get(PdfName.DA);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(new PdfString("default appearance"), calculationOrder);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultJustificationTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                acroForm.SetDefaultJustification(14);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject defaultJustification = acroForm.GetPdfObject().Get(PdfName.Q);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(new PdfNumber(14), defaultJustification);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultResourcesTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfDictionary dictionary = new PdfDictionary();
                PdfAcroForm.GetAcroForm(outputDoc, true).SetDefaultResources(dictionary);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject defaultResourcesDict = acroForm.GetPdfObject().Get(PdfName.DR);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(dictionary, defaultResourcesDict);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetNeedAppearancesTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                acroForm.SetNeedAppearances(false);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject needAppearance = acroForm.GetPdfObject().Get(PdfName.NeedAppearances);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(new PdfBoolean(false), needAppearance);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage("NeedAppearances has been deprecated in PDF 2.0. Appearance streams are required in PDF 2.0.")]
        public virtual void SetNeedAppearancesInPdf2Test() {
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)));
            outputDoc.AddNewPage();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            acroForm.SetNeedAppearances(false);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject needAppearance = acroForm.GetPdfObject().Get(PdfName.NeedAppearances);
            outputDoc.Close();
            NUnit.Framework.Assert.IsNull(needAppearance);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetGenerateAppearanceTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                acroForm.SetNeedAppearances(false);
                acroForm.SetGenerateAppearance(true);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                bool isGenerateAppearance = acroForm.IsGenerateAppearance();
                Object needAppearances = acroForm.GetPdfObject().Get(PdfName.NeedAppearances);
                outputDoc.Close();
                NUnit.Framework.Assert.IsNull(needAppearances);
                NUnit.Framework.Assert.IsTrue(isGenerateAppearance);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetXFAResourcePdfArrayTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfArray array = new PdfArray();
                acroForm.SetXFAResource(array);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject xfaObject = acroForm.GetPdfObject().Get(PdfName.XFA);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(array, xfaObject);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetXFAResourcePdfStreamTest() {
            using (PdfDocument outputDoc = CreateDocument()) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                PdfStream stream = new PdfStream();
                acroForm.SetXFAResource(stream);
                bool isModified = acroForm.GetPdfObject().IsModified();
                bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
                PdfObject xfaObject = acroForm.GetPdfObject().Get(PdfName.XFA);
                outputDoc.Close();
                NUnit.Framework.Assert.AreEqual(stream, xfaObject);
                NUnit.Framework.Assert.IsTrue(isModified);
                NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
            }
        }

        private static PdfDocument CreateDocument() {
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            outputDoc.AddNewPage();
            return outputDoc;
        }
    }
}
