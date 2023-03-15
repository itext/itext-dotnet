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
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FlatteningRotatedTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FlatteningRotatedTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FlatteningRotatedTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FormFlatteningTest_DefaultAppearanceGeneration_Rot() {
            String srcFilePatternPattern = "FormFlatteningDefaultAppearance_{0}_";
            String destPatternPattern = "FormFlatteningDefaultAppearance_{0}_";
            String[] rotAngle = new String[] { "0", "90", "180", "270" };
            foreach (String angle in rotAngle) {
                String srcFilePattern = MessageFormatUtil.Format(srcFilePatternPattern, angle);
                String destPattern = MessageFormatUtil.Format(destPatternPattern, angle);
                for (int i = 0; i < 360; i += 90) {
                    String src = sourceFolder + srcFilePattern + i + ".pdf";
                    String dest = destinationFolder + destPattern + i + "_flattened.pdf";
                    String cmp = sourceFolder + "cmp_" + srcFilePattern + i + ".pdf";
                    PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
                    foreach (PdfFormField field in form.GetAllFormFields().Values) {
                        field.SetValue("Test");
                    }
                    form.FlattenFields();
                    doc.Close();
                    NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
                }
            }
        }
    }
}
