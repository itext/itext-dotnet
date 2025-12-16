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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Events {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateChainValidatorEventTriggeringTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/CertificateChainValidatorTest/";

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private ValidatorChainBuilder SetUpValidatorChain(IssuingCertificateRetriever certificateRetriever, SignatureValidationProperties
             properties, MockRevocationDataValidator mockRevocationDataValidator) {
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => certificateRetriever).WithSignatureValidationProperties
                (properties).WithRevocationDataValidatorFactory(() => mockRevocationDataValidator);
            return validatorChainBuilder;
        }

        [NUnit.Framework.Test]
        public virtual void AlgoritmEventTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(chainName)[0];
            ValidatorChainBuilder validatorChainBuilder = SetUpValidatorChain(certificateRetriever, properties, mockRevocationDataValidator
                );
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            MockEventListener testEventListener = new MockEventListener();
            validatorChainBuilder.GetEventManager().Register(testEventListener);
            ValidationReport report = validator.ValidateCertificate(baseContext, rootCert, TimeTestUtil.TEST_DATE_TIME
                );
            NUnit.Framework.Assert.IsTrue(testEventListener.GetEvents().Any((e) => e is AlgorithmUsageEvent));
            NUnit.Framework.Assert.IsTrue(testEventListener.GetEvents().Any((e) => "Certificate check.".Equals(((AlgorithmUsageEvent
                )e).GetUsageLocation())));
        }
    }
}
