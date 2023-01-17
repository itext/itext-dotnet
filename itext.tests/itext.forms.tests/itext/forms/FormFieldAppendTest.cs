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
using System.IO;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FormFieldAppendTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FormFieldAppendTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FormFieldAppendTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FormFillingAppend_form_empty_Test() {
            String srcFilename = sourceFolder + "Form_Empty.pdf";
            String temp = destinationFolder + "temp_empty.pdf";
            String filename = destinationFolder + "formFillingAppend_form_empty.pdf";
            StampingProperties props = new StampingProperties();
            props.UseAppendMode();
            PdfDocument doc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(temp), props);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            foreach (PdfFormField field in form.GetAllFormFields().Values) {
                field.SetValue("Test");
            }
            doc.Close();
            Flatten(temp, filename);
            FileInfo toDelete = new FileInfo(temp);
            toDelete.Delete();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFillingAppend_form_empty.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FormFillingAppend_form_filled_Test() {
            String srcFilename = sourceFolder + "Form_Empty.pdf";
            String temp = destinationFolder + "temp_filled.pdf";
            String filename = destinationFolder + "formFillingAppend_form_filled.pdf";
            StampingProperties props = new StampingProperties();
            props.UseAppendMode();
            PdfDocument doc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(temp), props);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            foreach (PdfFormField field in form.GetAllFormFields().Values) {
                field.SetValue("Different");
            }
            doc.Close();
            Flatten(temp, filename);
            new FileInfo(temp).Delete();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFillingAppend_form_filled.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        private void Flatten(String src, String dest) {
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            form.FlattenFields();
            doc.Close();
        }
    }
}
