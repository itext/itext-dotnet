using System;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    public class MockOCSPValidator : OCSPValidator {
        public readonly IList<MockOCSPValidator.OCSPValidatorCall> calls = new List<MockOCSPValidator.OCSPValidatorCall
            >();

        private Action<MockOCSPValidator.OCSPValidatorCall> onCallHandler;

        /// <summary>
        /// Creates new
        /// <see cref="OCSPValidator"/>
        /// instance.
        /// </summary>
        public MockOCSPValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , ISingleResponse singleResp, IBasicOcspResponse ocspResp, DateTime validationDate) {
            MockOCSPValidator.OCSPValidatorCall call = new MockOCSPValidator.OCSPValidatorCall(report, context, certificate
                , singleResp, ocspResp, validationDate);
            calls.Add(call);
            if (onCallHandler != null) {
                onCallHandler(call);
            }
        }

        public virtual void OnCallDo(Action<MockOCSPValidator.OCSPValidatorCall> c) {
            onCallHandler = c;
        }

        public sealed class OCSPValidatorCall {
            public readonly DateTime timeStamp = DateTimeUtil.GetCurrentUtcTime();

            public readonly ValidationReport report;

            public readonly ValidationContext context;

            public readonly IX509Certificate certificate;

            public readonly ISingleResponse singleResp;

            public readonly IBasicOcspResponse ocspResp;

            public readonly DateTime validationDate;

            public OCSPValidatorCall(ValidationReport report, ValidationContext context, IX509Certificate certificate, 
                ISingleResponse singleResp, IBasicOcspResponse ocspResp, DateTime validationDate) {
                this.report = report;
                this.context = context;
                this.certificate = certificate;
                this.singleResp = singleResp;
                this.ocspResp = ocspResp;
                this.validationDate = validationDate;
            }
        }
    }
}
