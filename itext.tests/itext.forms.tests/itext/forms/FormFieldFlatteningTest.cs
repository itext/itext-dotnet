/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class FormFieldFlatteningTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FormFieldFlatteningTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FormFieldFlatteningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFlatteningTest01() {
            String srcFilename = "formFlatteningSource.pdf";
            String filename = "formFlatteningTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFlatteningChoiceFieldTest01() {
            String srcFilename = "formFlatteningSourceChoiceField.pdf";
            String filename = "formFlatteningChoiceFieldTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultiLineFormFieldClippingTest() {
            String src = sourceFolder + "multiLineFormFieldClippingTest.pdf";
            String dest = destinationFolder + "multiLineFormFieldClippingTest_flattened.pdf";
            String cmp = sourceFolder + "cmp_multiLineFormFieldClippingTest_flattened.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            form.GetField("Text1").SetValue("Tall letters: T I J L R E F");
            form.FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RotatedFieldAppearanceTest01() {
            String srcFilename = "src_rotatedFieldAppearanceTest01.pdf";
            String filename = "rotatedFieldAppearanceTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RotatedFieldAppearanceTest02() {
            String srcFilename = "src_rotatedFieldAppearanceTest02.pdf";
            String filename = "rotatedFieldAppearanceTest02.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DegeneratedRectTest01() {
            String srcFilename = "src_degeneratedRectTest01.pdf";
            String filename = "degeneratedRectTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DegeneratedRectTest02() {
            String srcFilename = "src_degeneratedRectTest02.pdf";
            String filename = "degeneratedRectTest02.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ScaledRectTest01() {
            String srcFilename = "src_scaledRectTest01.pdf";
            String filename = "scaledRectTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private static void FlattenFieldsAndCompare(String srcFile, String outFile) {
            PdfReader reader = new PdfReader(sourceFolder + srcFile);
            PdfWriter writer = new PdfWriter(destinationFolder + outFile);
            PdfDocument document = new PdfDocument(reader, writer);
            PdfAcroForm.GetAcroForm(document, false).FlattenFields();
            document.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + outFile, sourceFolder + "cmp_" + outFile
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
