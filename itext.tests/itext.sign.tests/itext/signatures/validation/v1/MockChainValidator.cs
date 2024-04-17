using System;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    internal class MockChainValidator : CertificateChainValidator {
        public IList<MockChainValidator.ValidationCallBack> verificationCalls = new List<MockChainValidator.ValidationCallBack
            >();

        internal MockChainValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override ValidationReport Validate(ValidationReport result, ValidationContext context, IX509Certificate
             certificate, DateTime verificationDate) {
            verificationCalls.Add(new MockChainValidator.ValidationCallBack(certificate, verificationDate));
            return result;
        }

        public class ValidationCallBack {
            public IX509Certificate certificate;

            public DateTime checkDate;

            public ValidationCallBack(IX509Certificate certificate, DateTime checkDate) {
                this.certificate = certificate;
                this.checkDate = checkDate;
            }
        }
    }
}
