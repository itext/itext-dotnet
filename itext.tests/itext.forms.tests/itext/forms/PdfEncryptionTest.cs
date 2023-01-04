/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            PdfButtonFormField group = new RadioFormFieldBuilder(pdfDoc, "Sex").CreateRadioGroup();
            group.SetValue("Male");
            new RadioFormFieldBuilder(pdfDoc).SetWidgetRectangle(new Rectangle(100, 530, 10, 10)).CreateRadioButton(group
                , "Male");
            new RadioFormFieldBuilder(pdfDoc).SetWidgetRectangle(new Rectangle(120, 530, 10, 10)).CreateRadioButton(group
                , "Female");
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
            PdfButtonFormField group = new RadioFormFieldBuilder(pdfDoc, "Sex").CreateRadioGroup();
            group.SetValue("Male");
            new RadioFormFieldBuilder(pdfDoc).SetWidgetRectangle(new Rectangle(100, 530, 10, 10)).CreateRadioButton(group
                , "Male");
            new RadioFormFieldBuilder(pdfDoc).SetWidgetRectangle(new Rectangle(120, 530, 10, 10)).CreateRadioButton(group
                , "Female");
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
