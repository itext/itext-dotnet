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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4MiscCheckTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String CMP_FOLDER = SOURCE_FOLDER + "cmp/PdfA4MiscCheckTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4MiscCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4CheckThatAsKeyIsAllowedTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4CheckThatAsKeyIsAllowedTest.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4CheckThatAsKeyIsAllowedTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent())) {
                doc.AddNewPage();
                PdfArray configs = new PdfArray();
                PdfDictionary config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName"));
                PdfDictionary usageAppDict = new PdfDictionary();
                usageAppDict.Put(PdfName.Event, PdfName.View);
                PdfArray categoryArray = new PdfArray();
                categoryArray.Add(PdfName.Zoom);
                usageAppDict.Put(PdfName.Category, categoryArray);
                config.Put(PdfName.AS, usageAppDict);
                configs.Add(config);
                PdfDictionary ocProperties = new PdfDictionary();
                ocProperties.Put(PdfName.Configs, configs);
                doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
            }
            CompareResult(outPdf, cmpPdf);
        }

        private PdfOutputIntent CreateOutputIntent() {
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "sRGB Color Space Profile.icm"));
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
