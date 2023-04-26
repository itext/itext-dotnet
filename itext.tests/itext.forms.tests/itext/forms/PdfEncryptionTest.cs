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
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfEncryptionTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfEncryptionTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfEncryptionTest/";

        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        // Custom entry in Info dictionary is used because standard entried are gone into metadata in PDF 2.0
        internal const String customInfoEntryKey = "Custom";

        internal const String customInfoEntryValue = "String";

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptedDocumentWithFormFields() {
            PdfReader reader = new PdfReader(sourceFolder + "encryptedDocumentWithFormFields.pdf", new ReaderProperties
                ().SetPassword("12345".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1)));
            PdfDocument pdfDocument = new PdfDocument(reader);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            acroForm.GetField("personal.name").GetPdfObject();
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256Pdf2PermissionsTest01() {
            String filename = "encryptAes256Pdf2PermissionsTest01.pdf";
            int permissions = EncryptionConstants.ALLOW_FILL_IN | EncryptionConstants.ALLOW_SCREENREADERS | EncryptionConstants
                .ALLOW_DEGRADED_PRINTING;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0).SetStandardEncryption(USER, OWNER, permissions, EncryptionConstants.ENCRYPTION_AES_256
                )));
            pdfDoc.GetDocumentInfo().SetMoreInfo(customInfoEntryKey, customInfoEntryValue);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField textField1 = new TextFormFieldBuilder(pdfDoc, "Name").SetWidgetRectangle(new Rectangle(100
                , 600, 200, 30)).CreateText();
            textField1.SetValue("Enter your name");
            form.AddField(textField1);
            PdfTextFormField textField2 = new TextFormFieldBuilder(pdfDoc, "Surname").SetWidgetRectangle(new Rectangle
                (100, 550, 200, 30)).CreateText();
            textField2.SetValue("Enter your surname");
            form.AddField(textField2);
            String sexFormFieldName = "Sex";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, sexFormFieldName);
            PdfButtonFormField group = builder.CreateRadioGroup();
            group.SetValue("Male");
            PdfFormAnnotation radio1 = builder.CreateRadioButton("Male", new Rectangle(100, 530, 10, 10));
            PdfFormAnnotation radio2 = builder.CreateRadioButton("Female", new Rectangle(120, 530, 10, 10));
            group.AddKid(radio1);
            group.AddKid(radio2);
            form.AddField(group);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_", USER, USER);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256Pdf2PermissionsTest02() {
            String filename = "encryptAes256Pdf2PermissionsTest02.pdf";
            // This test differs from the previous one (encryptAes256Pdf2PermissionsTest01) only in permissions.
            // Here we do not allow to fill the form in.
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS | EncryptionConstants.ALLOW_DEGRADED_PRINTING;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0).SetStandardEncryption(USER, OWNER, permissions, EncryptionConstants.ENCRYPTION_AES_256
                )));
            pdfDoc.GetDocumentInfo().SetMoreInfo(customInfoEntryKey, customInfoEntryValue);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField textField1 = new TextFormFieldBuilder(pdfDoc, "Name").SetWidgetRectangle(new Rectangle(100
                , 600, 200, 30)).CreateText();
            textField1.SetValue("Enter your name");
            form.AddField(textField1);
            PdfTextFormField textField2 = new TextFormFieldBuilder(pdfDoc, "Surname").SetWidgetRectangle(new Rectangle
                (100, 550, 200, 30)).CreateText();
            textField2.SetValue("Enter your surname");
            form.AddField(textField2);
            String sexFormFieldName = "Sex";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, sexFormFieldName);
            PdfButtonFormField group = builder.CreateRadioGroup();
            group.SetValue("Male");
            PdfFormAnnotation radio1 = new RadioFormFieldBuilder(pdfDoc, sexFormFieldName).CreateRadioButton("Male", new 
                Rectangle(100, 530, 10, 10));
            PdfFormAnnotation radio2 = new RadioFormFieldBuilder(pdfDoc, sexFormFieldName).CreateRadioButton("Female", 
                new Rectangle(120, 530, 10, 10));
            group.AddKid(radio1);
            group.AddKid(radio2);
            form.AddField(group);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_", USER, USER);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
