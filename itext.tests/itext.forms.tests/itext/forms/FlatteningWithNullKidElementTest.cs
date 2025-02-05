/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FlatteningWithNullKidElementTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FlatteningWithNullKidElementTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FlatteningWithNullKidElementTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FormFlatteningTestWithNullKidElement() {
            String filename = "Form_NullKidElement";
            String src = sourceFolder + filename + ".pdf";
            String temp = destinationFolder + "temp.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(temp));
            bool fail = false;
            try {
                PdfFormCreator.GetAcroForm(doc, true).FlattenFields();
                doc.Close();
            }
            catch (Exception) {
                fail = true;
            }
            NUnit.Framework.Assert.IsFalse(fail);
        }
    }
}
