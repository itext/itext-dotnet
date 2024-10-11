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
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfARadiofieldTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfARadiofieldTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfARadiofieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1aRadioFieldOffAppearanceTest() {
            String name = "pdfA1a_radioFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(doc, true);
            String formFieldName = "group";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(doc, formFieldName);
            PdfButtonFormField group = builder.CreateRadioGroup();
            group.SetValue("1", true);
            group.SetReadOnly(true);
            Rectangle rect1 = new Rectangle(36, 700, 20, 20);
            Rectangle rect2 = new Rectangle(36, 680, 20, 20);
            PdfFormAnnotation radio1 = builder.CreateRadioButton("1", rect1).SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            PdfFormAnnotation radio2 = builder.CreateRadioButton("2", rect2).SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            group.AddKid(radio1);
            group.AddKid(radio2);
            form.AddField(group);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }
    }
}
