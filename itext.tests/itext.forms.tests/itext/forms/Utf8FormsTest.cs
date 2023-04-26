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
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("UnitTest")]
    public class Utf8FormsTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/Utf8FormsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/Utf8FormsTest/";

        public static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/Utf8FormsTest/NotoSansCJKsc-Regular.otf";

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ReadUtf8FieldName() {
            String filename = sourceFolder + "utf-8-field-name.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            pdfDoc.Close();
            foreach (String fldName in fields.Keys) {
                //  لا
                NUnit.Framework.Assert.AreEqual("\u0644\u0627", fldName);
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ReadUtf8TextAnnot() {
            String filename = sourceFolder + "utf-8-text-annot.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            pdfDoc.Close();
            foreach (String fldName in fields.Keys) {
                //  福昕 福昕UTF8
                NUnit.Framework.Assert.AreEqual("\u798F\u6615 \u798F\u6615UTF8", fields.Get(fldName).GetValueAsString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteUtf8FieldNameAndValue() {
            //TODO DEVSIX-2798
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "writeUtf8FieldNameAndValue.pdf"));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "").SetWidgetRectangle(new Rectangle(99, 753, 425
                , 15)).CreateText();
            field.SetValue("");
            field.SetFont(PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H));
            //  لا
            field.Put(PdfName.T, new PdfString("\u0644\u0627", PdfEncodings.UTF8));
            //  福昕 福昕UTF8
            field.Put(PdfName.V, new PdfString("\u798F\u6615 \u798F\u6615UTF8", PdfEncodings.UTF8));
            field.RegenerateField();
            form.AddField(field);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "writeUtf8FieldNameAndValue.pdf"
                , sourceFolder + "cmp_writeUtf8FieldNameAndValue.pdf", destinationFolder, "diffFieldNameAndValue_"));
        }
    }
}
