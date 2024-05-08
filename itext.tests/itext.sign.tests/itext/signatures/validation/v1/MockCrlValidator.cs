using System;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    public class MockCrlValidator : CRLValidator {
        public readonly IList<MockCrlValidator.CRLValidateCall> calls = new List<MockCrlValidator.CRLValidateCall>
            ();

        private Action<MockCrlValidator.CRLValidateCall> onCallHandler;

        /// <summary>
        /// Creates new
        /// <see cref="CRLValidator"/>
        /// instance.
        /// </summary>
        public MockCrlValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , IX509Crl crl, DateTime validationDate) {
            MockCrlValidator.CRLValidateCall call = new MockCrlValidator.CRLValidateCall(report, context, certificate, 
                crl, validationDate);
            calls.Add(call);
            if (onCallHandler != null) {
                onCallHandler(calls[calls.Count - 1]);
            }
        }

        public virtual void OnCallDo(Action<MockCrlValidator.CRLValidateCall> c) {
            onCallHandler = c;
        }

        public sealed class CRLValidateCall {
            public readonly DateTime timeStamp = DateTimeUtil.GetCurrentUtcTime();

            public readonly ValidationReport report;

            public readonly ValidationContext context;

            public readonly IX509Certificate certificate;

            public readonly IX509Crl crl;

            public readonly DateTime validationDate;

            public CRLValidateCall(ValidationReport report, ValidationContext context, IX509Certificate certificate, IX509Crl
                 crl, DateTime validationDate) {
                this.report = report;
                this.context = context;
                this.certificate = certificate;
                this.crl = crl;
                this.validationDate = validationDate;
            }
        }
    }
}
