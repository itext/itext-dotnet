/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Forms.Fields;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Fields.Merging {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnDuplicateFormFieldNameStrategyTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/merging/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/merging/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AlwaysThrowExceptionOnDuplicateFormFieldName01() {
            MemoryStream baos = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true, new AlwaysThrowExceptionStrategy());
            PdfButtonFormField field1 = new CheckBoxFormFieldBuilder(pdfDocument, "test").CreateCheckBox();
            form.AddField(field1);
            PdfButtonFormField field2 = new CheckBoxFormFieldBuilder(pdfDocument, "test").CreateCheckBox();
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => form.AddField(field2));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AlwaysThrowExceptionOnDuplicateFormFieldName02() {
            MemoryStream baos = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true, new AlwaysThrowExceptionStrategy());
            form.AddField(new CheckBoxFormFieldBuilder(pdfDocument, "test").CreateCheckBox());
            form.AddField(new CheckBoxFormFieldBuilder(pdfDocument, "test1").CreateCheckBox());
            NUnit.Framework.Assert.IsNotNull(form.GetField("test"));
            NUnit.Framework.Assert.IsNotNull(form.GetField("test1"));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IncrementFieldNameEven() {
            String destination = DESTINATION_FOLDER + "incrementFieldNameEven.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destination))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true, new AddIndexStrategy());
                for (int i = 1; i < 3; i++) {
                    Rectangle rect = new Rectangle(20, 20);
                    rect.SetY(100 * i);
                    rect.SetX(100);
                    PdfButtonFormField field1 = new CheckBoxFormFieldBuilder(pdfDocument, "test").SetWidgetRectangle(rect).CreateCheckBox
                        ();
                    form.AddField(field1);
                    Rectangle rect2 = new Rectangle(20, 20);
                    rect2.SetY(100 * i);
                    rect2.SetX(200);
                    PdfButtonFormField field2 = new CheckBoxFormFieldBuilder(pdfDocument, "bingbong").SetWidgetRectangle(rect2
                        ).CreateCheckBox();
                    form.AddField(field2);
                }
                PdfFormField field1_1 = form.GetField("test");
                PdfFormField field2_1 = form.GetField("bingbong");
                PdfFormField field3 = form.GetField("test_1");
                PdfFormField field4 = form.GetField("bingbong_1");
                NUnit.Framework.Assert.IsNotNull(field1_1);
                NUnit.Framework.Assert.IsNotNull(field2_1);
                NUnit.Framework.Assert.IsNotNull(field3);
                NUnit.Framework.Assert.IsNotNull(field4);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destination, SOURCE_FOLDER + "cmp_incrementalFieldNameEven.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAddFormFieldWithoutConfiguration() {
            String destination = DESTINATION_FOLDER + "testAddFormFieldWithoutConfiguration.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destination))) {
                Rectangle rect = new Rectangle(20, 20);
                rect.SetY(100);
                rect.SetX(100);
                PdfFormField field1 = new TextFormFieldBuilder(pdfDocument, "parent").SetWidgetRectangle(rect).CreateText(
                    );
                Rectangle rect2 = new Rectangle(20, 20);
                rect2.SetY(100);
                rect2.SetX(200);
                PdfFormField child1 = new TextFormFieldBuilder(pdfDocument, "child").SetWidgetRectangle(rect2).CreateText(
                    );
                Rectangle rect3 = new Rectangle(20, 20);
                rect3.SetY(100);
                rect3.SetX(300);
                PdfFormField child2 = new TextFormFieldBuilder(pdfDocument, "child").SetWidgetRectangle(rect3).CreateText(
                    );
                field1.AddKid(child1);
                field1.AddKid(child2);
                PdfAcroForm.GetAcroForm(pdfDocument, true).AddField(field1);
                NUnit.Framework.Assert.AreEqual(2, field1.GetKids().Size());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destination, SOURCE_FOLDER + "cmp_testAddFormFieldWithoutConfiguration.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void IncrementFieldNameUnEven() {
            String destination = DESTINATION_FOLDER + "incrementFieldNameUnEven.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destination));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true, new AddIndexStrategy());
            for (int i = 1; i < 4; i++) {
                Rectangle rect = new Rectangle(20, 20);
                rect.SetY(100 * i);
                rect.SetX(100);
                PdfButtonFormField field1 = new CheckBoxFormFieldBuilder(pdfDocument, "test").SetWidgetRectangle(rect).CreateCheckBox
                    ();
                form.AddField(field1);
                Rectangle rect2 = new Rectangle(20, 20);
                rect2.SetY(100 * i);
                rect2.SetX(200);
                PdfButtonFormField field2 = new CheckBoxFormFieldBuilder(pdfDocument, "bingbong").SetWidgetRectangle(rect2
                    ).CreateCheckBox();
                form.AddField(field2);
            }
            PdfFormField field1_1 = form.GetField("test");
            PdfFormField field2_1 = form.GetField("bingbong");
            PdfFormField field3 = form.GetField("test_1");
            PdfFormField field4 = form.GetField("bingbong_1");
            PdfFormField field5 = form.GetField("test_2");
            PdfFormField field6 = form.GetField("bingbong_2");
            NUnit.Framework.Assert.IsNotNull(field1_1);
            NUnit.Framework.Assert.IsNotNull(field2_1);
            NUnit.Framework.Assert.IsNotNull(field3);
            NUnit.Framework.Assert.IsNotNull(field4);
            NUnit.Framework.Assert.IsNotNull(field5);
            NUnit.Framework.Assert.IsNotNull(field6);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destination, SOURCE_FOLDER + "cmp_incrementFieldNameUnEven.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddIndexDotOperatorThrowsException() {
            MemoryStream baos = new MemoryStream();
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true, new AddIndexStrategy("."));
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddIndexNullOperatorThrowsException() {
            MemoryStream baos = new MemoryStream();
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true, new AddIndexStrategy(null));
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void InvalidParamsToExecuteNull() {
            NUnit.Framework.Assert.IsFalse(new AddIndexStrategy().Execute(null, null, false));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 4)]
        public virtual void FlattenReadOnlyAddIndexTo() {
            String destination = DESTINATION_FOLDER + "flattenReadOnlyAddIndexTo.pdf";
            PdfWriter writer = new PdfWriter(destination);
            PdfDocument pdfDoc = new PdfDocument(writer);
            String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext.CurrentContext
                .TestDirectory) + "/resources/itext/forms/FormFieldFlatteningTest/";
            using (PdfDocument pdfInnerDoc = new PdfDocument(new PdfReader(sourceFolder + "readOnlyForm.pdf"))) {
                pdfInnerDoc.CopyPagesTo(1, pdfInnerDoc.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            }
            using (PdfDocument pdfInnerDoc_1 = new PdfDocument(new PdfReader(sourceFolder + "readOnlyForm.pdf"))) {
                pdfInnerDoc_1.CopyPagesTo(1, pdfInnerDoc_1.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            }
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false, new AddIndexStrategy());
            bool isReadOnly = true;
            foreach (PdfFormField field in form.GetAllFormFields().Values) {
                isReadOnly = (isReadOnly && field.IsReadOnly());
            }
            int amount = form.GetAllFormFields().Count;
            pdfDoc.Close();
            NUnit.Framework.Assert.IsTrue(isReadOnly);
            NUnit.Framework.Assert.AreEqual(4, amount);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destination, SOURCE_FOLDER + "cmp_flattenReadOnlyAddIndexTo.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddIndexStrategySeparatesTheFields() {
            try {
                PdfFormCreator.SetFactory(new _PdfFormFactory_269());
                using (PdfDocument pdfInnerDoc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + "add_index.pdf"))) {
                    Document doc = new Document(pdfInnerDoc);
                    doc.Add(new CheckBox("test1").SetBorder(new SolidBorder(ColorConstants.RED, 1)));
                    doc.Add(new CheckBox("test1").SetBorder(new SolidBorder(ColorConstants.RED, 1)));
                    doc.Add(new CheckBox("test").SetInteractive(true));
                    doc.Add(new CheckBox("test").SetInteractive(true));
                }
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "add_index.pdf", SOURCE_FOLDER
                     + "cmp_add_index.pdf", DESTINATION_FOLDER, "diff_"));
            }
            finally {
                PdfFormCreator.SetFactory(new PdfFormFactory());
            }
        }

        private sealed class _PdfFormFactory_269 : PdfFormFactory {
            public _PdfFormFactory_269() {
            }

            public override PdfAcroForm GetAcroForm(PdfDocument document, bool createIfNotExist) {
                return PdfAcroForm.GetAcroForm(document, createIfNotExist, new AddIndexStrategy());
            }
        }
    }
}
