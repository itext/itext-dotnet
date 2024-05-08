using System;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    public class MockRevocationDataValidator : RevocationDataValidator {
        public IList<ICrlClient> crlClientsAdded = new List<ICrlClient>();

        public IList<IOcspClient> ocspClientsAdded = new List<IOcspClient>();

        public IList<MockRevocationDataValidator.RevocationDataValidatorCall> calls = new List<MockRevocationDataValidator.RevocationDataValidatorCall
            >();

        /// <summary>
        /// Creates new
        /// <see cref="RevocationDataValidator"/>
        /// instance to validate certificate revocation data.
        /// </summary>
        internal MockRevocationDataValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override RevocationDataValidator AddCrlClient(ICrlClient crlClient) {
            crlClientsAdded.Add(crlClient);
            return this;
        }

        public override RevocationDataValidator AddOcspClient(IOcspClient ocspClient) {
            ocspClientsAdded.Add(ocspClient);
            return this;
        }

        public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate) {
            calls.Add(new MockRevocationDataValidator.RevocationDataValidatorCall(report, context, certificate, validationDate
                ));
        }

        public sealed class RevocationDataValidatorCall {
            public readonly ValidationReport report;

            public readonly ValidationContext context;

            public readonly IX509Certificate certificate;

            public readonly DateTime validationDate;

            public RevocationDataValidatorCall(ValidationReport report, ValidationContext context, IX509Certificate certificate
                , DateTime validationDate) {
                this.report = report;
                this.context = context;
                this.certificate = certificate;
                this.validationDate = validationDate;
            }
        }
    }
}
