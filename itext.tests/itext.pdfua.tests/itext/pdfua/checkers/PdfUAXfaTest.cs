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
using iText.Kernel.Pdf;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;
using iText.Test.Utils;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAXfaTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAXfaTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAXfaTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void DynamicRenderRequiredValueTest() {
            String input = SOURCE_FOLDER + "dynamicRequired.pdf";
            String output = DESTINATION_FOLDER + "dynamicRequired_reopen.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfReader(input), new PdfWriter(output));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.DYNAMIC_XFA_FORMS_SHALL_NOT_BE_USED, e.Message
                );
            FileUtil.Copy(input, output);
            String result = new VeraPdfValidator().Validate(output);
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            System.Console.Out.WriteLine(result);
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            // VeraPdf also complains only about the dynamic XFA forms
            NUnit.Framework.Assert.IsNotNull(result);
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void DynamicRenderForbiddenValueTest() {
            String input = SOURCE_FOLDER + "dynamicForbidden.pdf";
            String output = DESTINATION_FOLDER + "dynamicForbidden_reopen.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfReader(input), new PdfWriter(output));
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            String result = new VeraPdfValidator().Validate(output);
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(result);
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    }
}
