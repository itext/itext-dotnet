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
using iText.Kernel.Pdf;
using iText.Kernel.Validation.Context;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfADocumentTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        [NUnit.Framework.Test]
        public virtual void CheckCadesSignatureTypeIsoConformance() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfADocument document = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            document.CheckIsoConformance(new SignTypeValidationContext(true));
        }

        [NUnit.Framework.Test]
        public virtual void CheckCMSSignatureTypeIsoConformance() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfADocument document = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => document.CheckIsoConformance
                (new SignTypeValidationContext(false)));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.SIGNATURE_SHALL_CONFORM_TO_ONE_OF_THE_PADES_PROFILE
                , e.Message);
        }
    }
}
