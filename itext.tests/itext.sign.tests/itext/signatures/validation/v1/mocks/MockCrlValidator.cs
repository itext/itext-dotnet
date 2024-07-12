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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Validation.V1;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1.Mocks {
    public class MockCrlValidator : CRLValidator {
        public readonly IList<MockCrlValidator.CRLValidateCall> calls = new List<MockCrlValidator.CRLValidateCall>
            ();

        private Action<MockCrlValidator.CRLValidateCall> onCallHandler;

        /// <summary>
        /// Creates new
        /// <see cref="iText.Signatures.Validation.V1.CRLValidator"/>
        /// instance.
        /// </summary>
        public MockCrlValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , IX509Crl crl, DateTime validationDate, DateTime responseGenerationDate) {
            MockCrlValidator.CRLValidateCall call = new MockCrlValidator.CRLValidateCall(report, context, certificate, 
                crl, validationDate, responseGenerationDate);
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

            public readonly DateTime responseGenerationDate;

            public CRLValidateCall(ValidationReport report, ValidationContext context, IX509Certificate certificate, IX509Crl
                 crl, DateTime validationDate, DateTime responseGenerationDate) {
                this.report = report;
                this.context = context;
                this.certificate = certificate;
                this.crl = crl;
                this.validationDate = validationDate;
                this.responseGenerationDate = responseGenerationDate;
            }
        }
    }
}
