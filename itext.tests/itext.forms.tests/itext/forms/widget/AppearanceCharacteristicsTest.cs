/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Collections.Generic;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Widget {
    public class AppearanceCharacteristicsTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/widget/AppearanceCharacteristicsTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/widget/AppearanceCharacteristicsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FillFormWithRotatedFieldAndPageTest() {
            String outPdf = destinationFolder + "fillFormWithRotatedFieldAndPageTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "pdfWithRotatedField.pdf"), new PdfWriter(outPdf
                ));
            PdfAcroForm form1 = PdfAcroForm.GetAcroForm(doc, false);
            form1.GetField("First field").SetValue("We filled this field").SetBorderColor(ColorConstants.BLACK);
            doc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, sourceFolder + "cmp_fillFormWithRotatedFieldAndPageTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BorderStyleInCreatedFormFieldsTest() {
            //TODO: update cmp file after fixing DEVSIX-836
            String outPdf = destinationFolder + "borderStyleInCreatedFormFields.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField formField1 = PdfTextFormField.CreateText(doc, new Rectangle(100, 600, 100, 50), "firstField", 
                "Hello, iText!");
            formField1.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_BEVELED);
            formField1.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
            PdfFormField formField2 = PdfTextFormField.CreateText(doc, new Rectangle(100, 500, 100, 50), "secondField"
                , "Hello, iText!");
            formField2.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_UNDERLINE);
            formField2.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
            PdfFormField formField3 = PdfTextFormField.CreateText(doc, new Rectangle(100, 400, 100, 50), "thirdField", 
                "Hello, iText!");
            formField3.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_INSET);
            formField3.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
            form.AddField(formField1);
            form.AddField(formField2);
            form.AddField(formField3);
            form.FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, sourceFolder + "cmp_borderStyleInCreatedFormFields.pdf"
                , destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void UpdatingBorderStyleInFormFieldsTest() {
            //TODO: update cmp file after fixing DEVSIX-836
            String inputPdf = sourceFolder + "borderStyleInCreatedFormFields.pdf";
            String outPdf = destinationFolder + "updatingBorderStyleInFormFields.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(inputPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, false);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            fields.Get("firstField").SetValue("New Value 1");
            fields.Get("secondField").SetValue("New Value 2");
            fields.Get("thirdField").SetValue("New Value 3");
            form.FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, sourceFolder + "cmp_updatingBorderStyleInFormFields.pdf"
                , destinationFolder));
        }
    }
}
