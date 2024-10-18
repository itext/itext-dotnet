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
using iText.Signatures.Cms;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation.Report.Xml {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PadesValidationReportTest : ExtendedITextTest {
        private static CMSContainer signature;

        private readonly ValidationObjects validationObjects = new ValidationObjects();

        [NUnit.Framework.OneTimeSetUp]
        public static void SetupFixture() {
            signature = new CMSContainer(Convert.FromBase64String(XmlReportTestHelper.SIGNATURE1_BASE64));
        }

        [NUnit.Framework.Test]
        public virtual void TestCreation() {
            PadesValidationReport sut = new PadesValidationReport(validationObjects);
            NUnit.Framework.Assert.IsNotNull(sut);
        }

        [NUnit.Framework.Test]
        public virtual void TestAddSignatureValidationReport() {
            PadesValidationReport sut = new PadesValidationReport(validationObjects);
            SignatureValidationReport signatureValidationReport = new SignatureValidationReport(validationObjects, signature
                , "signatureName", TimeTestUtil.TEST_DATE_TIME);
            sut.AddSignatureValidationReport(signatureValidationReport);
            // Collection should be returned
            NUnit.Framework.Assert.IsNotNull(sut.GetSignatureValidationReports());
            // Collection should contain at least one element
            NUnit.Framework.Assert.IsFalse(sut.GetSignatureValidationReports().IsEmpty());
            // The added signature validation report should be in the collection
            NUnit.Framework.Assert.IsTrue(sut.GetSignatureValidationReports().Contains(signatureValidationReport));
        }

        [NUnit.Framework.Test]
        public virtual void TestAddMultipleSignatureValidationReports() {
            PadesValidationReport sut = new PadesValidationReport(validationObjects);
            SignatureValidationReport signatureValidationReport1 = new SignatureValidationReport(validationObjects, signature
                , "signatureName", TimeTestUtil.TEST_DATE_TIME);
            SignatureValidationReport signatureValidationReport2 = new SignatureValidationReport(validationObjects, signature
                , "signatureName", TimeTestUtil.TEST_DATE_TIME);
            sut.AddSignatureValidationReport(signatureValidationReport1);
            sut.AddSignatureValidationReport(signatureValidationReport2);
            NUnit.Framework.Assert.IsTrue(sut.GetSignatureValidationReports().Contains(signatureValidationReport1));
            NUnit.Framework.Assert.IsTrue(sut.GetSignatureValidationReports().Contains(signatureValidationReport2));
        }
    }
}
