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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using iText.Signatures.Validation.V1;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1.Mocks {
    public class MockOCSPValidator : OCSPValidator {
        public readonly IList<MockOCSPValidator.OCSPValidatorCall> calls = new List<MockOCSPValidator.OCSPValidatorCall
            >();

        private Action<MockOCSPValidator.OCSPValidatorCall> onCallHandler;

        /// <summary>
        /// Creates new
        /// <see cref="iText.Signatures.Validation.V1.OCSPValidator"/>
        /// instance.
        /// </summary>
        public MockOCSPValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , ISingleResponse singleResp, IBasicOcspResponse ocspResp, DateTime validationDate, DateTime responseGenerationDate
            ) {
            MockOCSPValidator.OCSPValidatorCall call = new MockOCSPValidator.OCSPValidatorCall(report, context, certificate
                , singleResp, ocspResp, validationDate, responseGenerationDate);
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

            public readonly DateTime responseGenerationDate;

            public OCSPValidatorCall(ValidationReport report, ValidationContext context, IX509Certificate certificate, 
                ISingleResponse singleResp, IBasicOcspResponse ocspResp, DateTime validationDate, DateTime responseGenerationDate
                ) {
                this.report = report;
                this.context = context;
                this.certificate = certificate;
                this.singleResp = singleResp;
                this.ocspResp = ocspResp;
                this.validationDate = validationDate;
                this.responseGenerationDate = responseGenerationDate;
            }
        }
    }
}
