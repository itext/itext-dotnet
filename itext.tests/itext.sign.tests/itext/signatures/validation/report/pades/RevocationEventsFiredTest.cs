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
using System.Collections.Generic;
using iText.Bouncycastleconnector;
using iText.Commons.Actions;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Dataorigin;
using iText.Signatures.Validation.Events;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Report.Pades {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class RevocationEventsFiredTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly ValidationContext VALIDATION_CONTEXT = new ValidationContext(ValidatorContext.REVOCATION_DATA_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static readonly DateTime CURRENT_DATE = DateTimeUtil.GetCurrentUtcTime();

        private RevocationEventsFiredTest.CustomReportGenerator customReportGenerator;

        private ValidatorChainBuilder builder;

        private IX509Certificate dummyCertificate;

        private IX509Certificate parentCert;

        private IPrivateKey privateKey;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = new ValidatorChainBuilder();
            customReportGenerator = new RevocationEventsFiredTest.CustomReportGenerator();
            builder.WithPAdESLevelReportGenerator(customReportGenerator);
            dummyCertificate = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem")[0];
            parentCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem")[1];
            privateKey = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.pem", PASSWORD);
        }

        [NUnit.Framework.Test]
        public virtual void ZeroApplicableResponsesFireTwoEventsTest() {
            RevocationDataValidator revocationDataValidator = builder.BuildRevocationDataValidator();
            revocationDataValidator.Validate(new ValidationReport(), VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE
                );
            NUnit.Framework.Assert.AreEqual(2, customReportGenerator.firedEvents.Count);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[0] is DssNotTimestampedEvent);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[1] is RevocationNotFromDssEvent);
        }

        [NUnit.Framework.Test]
        public virtual void NotTimestampedResponsesFireOneEventTest() {
            SetUpOcspClient(RevocationDataOrigin.LATEST_DSS, TimeBasedContext.PRESENT);
            RevocationDataValidator revocationDataValidator = builder.BuildRevocationDataValidator();
            revocationDataValidator.Validate(new ValidationReport(), VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE
                );
            NUnit.Framework.Assert.AreEqual(1, customReportGenerator.firedEvents.Count);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[0] is DssNotTimestampedEvent);
        }

        [NUnit.Framework.Test]
        public virtual void ResponsesNotFromLatestDssFireTwoEventsTest() {
            SetUpOcspClient(RevocationDataOrigin.HISTORICAL_DSS, TimeBasedContext.HISTORICAL);
            RevocationDataValidator revocationDataValidator = builder.BuildRevocationDataValidator();
            revocationDataValidator.Validate(new ValidationReport(), VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE
                );
            NUnit.Framework.Assert.AreEqual(2, customReportGenerator.firedEvents.Count);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[0] is DssNotTimestampedEvent);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[1] is RevocationNotFromDssEvent);
        }

        [NUnit.Framework.Test]
        public virtual void ResponsesFromSignatureFireTwoEventsTest() {
            SetUpOcspClient(RevocationDataOrigin.SIGNATURE, TimeBasedContext.HISTORICAL);
            RevocationDataValidator revocationDataValidator = builder.BuildRevocationDataValidator();
            revocationDataValidator.Validate(new ValidationReport(), VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE
                );
            NUnit.Framework.Assert.AreEqual(2, customReportGenerator.firedEvents.Count);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[0] is DssNotTimestampedEvent);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[1] is RevocationNotFromDssEvent);
        }

        [NUnit.Framework.Test]
        public virtual void ResponsesFromTimestampedDssDontFireEventsTest() {
            SetUpOcspClient(RevocationDataOrigin.LATEST_DSS, TimeBasedContext.HISTORICAL);
            RevocationDataValidator revocationDataValidator = builder.BuildRevocationDataValidator();
            revocationDataValidator.Validate(new ValidationReport(), VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE
                );
            NUnit.Framework.Assert.AreEqual(0, customReportGenerator.firedEvents.Count);
        }

        private void SetUpOcspClient(RevocationDataOrigin? responseOrigin, TimeBasedContext timeBasedContext) {
            TestOcspClient testOcspClient = new TestOcspClient().AddBuilderForCertIssuer(parentCert, privateKey);
            SignatureValidationProperties validationProperties = new SignatureValidationProperties();
            validationProperties.AddOcspClient(new _ValidationOcspClient_143(this, testOcspClient, timeBasedContext, responseOrigin
                ));
            builder.WithSignatureValidationProperties(validationProperties);
            builder.WithOCSPValidatorFactory(() => new _OCSPValidator_156(builder));
        }

        private sealed class _ValidationOcspClient_143 : ValidationOcspClient {
            public _ValidationOcspClient_143(RevocationEventsFiredTest _enclosing, TestOcspClient testOcspClient, TimeBasedContext
                 timeBasedContext, RevocationDataOrigin? responseOrigin) {
                this._enclosing = _enclosing;
                this.testOcspClient = testOcspClient;
                this.timeBasedContext = timeBasedContext;
                this.responseOrigin = responseOrigin;
            }

            public override IDictionary<IBasicOcspResponse, RevocationDataValidator.OcspResponseValidationInfo> GetResponses
                () {
                IBasicOcspResponse basicOCSPResp = RevocationEventsFiredTest.BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse
                    (testOcspClient.GetEncoded(this._enclosing.dummyCertificate, this._enclosing.parentCert, null));
                IDictionary<IBasicOcspResponse, RevocationDataValidator.OcspResponseValidationInfo> dummyResponses = new Dictionary
                    <IBasicOcspResponse, RevocationDataValidator.OcspResponseValidationInfo>();
                dummyResponses.Put(basicOCSPResp, new RevocationDataValidator.OcspResponseValidationInfo(basicOCSPResp.GetResponses
                    ()[0], basicOCSPResp, RevocationEventsFiredTest.CURRENT_DATE, timeBasedContext, responseOrigin));
                return dummyResponses;
            }

            private readonly RevocationEventsFiredTest _enclosing;

            private readonly TestOcspClient testOcspClient;

            private readonly TimeBasedContext timeBasedContext;

            private readonly RevocationDataOrigin? responseOrigin;
        }

        private sealed class _OCSPValidator_156 : OCSPValidator {
            public _OCSPValidator_156(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
                , ISingleResponse singleResp, IBasicOcspResponse ocspResp, DateTime validationDate, DateTime responseGenerationDate
                ) {
            }
        }

        public class CustomReportGenerator : PAdESLevelReportGenerator {
//\cond DO_NOT_DOCUMENT
            internal IList<IEvent> firedEvents = new List<IEvent>();
//\endcond

            public override void OnEvent(IEvent rawEvent) {
                firedEvents.Add(rawEvent);
            }
        }
    }
}
