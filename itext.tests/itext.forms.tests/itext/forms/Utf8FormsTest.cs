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
