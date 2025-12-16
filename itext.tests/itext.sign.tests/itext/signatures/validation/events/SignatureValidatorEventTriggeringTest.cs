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
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Kernel.Pdf;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Mocks;
using iText.Test;

namespace iText.Signatures.Validation.Events {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class SignatureValidatorEventTriggeringTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/SignatureValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        private ValidatorChainBuilder CreateValidatorChainBuilder(MockIssuingCertificateRetriever mockCertificateRetriever
            , SignatureValidationProperties parameters, MockChainValidator mockCertificateChainValidator, MockDocumentRevisionsValidator
             mockDocumentRevisionsValidator) {
            return new ValidatorChainBuilder().WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever).
                WithSignatureValidationProperties(parameters).WithCertificateChainValidatorFactory(() => mockCertificateChainValidator
                ).WithRevocationDataValidatorFactory(() => new MockRevocationDataValidator()).WithDocumentRevisionsValidatorFactory
                (() => mockDocumentRevisionsValidator);
        }

        [NUnit.Framework.Test]
        public virtual void AlgorithmReportingTest() {
            MockChainValidator mockCertificateChainValidator = new MockChainValidator();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever();
            MockDocumentRevisionsValidator mockDocumentRevisionsValidator = new MockDocumentRevisionsValidator();
            ValidatorChainBuilder builder = CreateValidatorChainBuilder(mockCertificateRetriever, parameters, mockCertificateChainValidator
                , mockDocumentRevisionsValidator);
            MockEventListener testEventHandler = new MockEventListener();
            builder.GetEventManager().Register(testEventHandler);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithDss.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                signatureValidator.ValidateSignatures();
            }
            NUnit.Framework.Assert.AreEqual(2, testEventHandler.GetEvents().Where((e) => e is AlgorithmUsageEvent).Count
                ());
            NUnit.Framework.Assert.AreEqual(2, testEventHandler.GetEvents().Where((e) => e is AlgorithmUsageEvent && "Signature verification check."
                .Equals(((AlgorithmUsageEvent)e).GetUsageLocation())).Count());
        }
    }
}
