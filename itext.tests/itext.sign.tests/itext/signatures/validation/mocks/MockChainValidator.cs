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
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Mocks {
    public class MockChainValidator : CertificateChainValidator {
        public IList<MockChainValidator.ValidationCallBack> verificationCalls = new List<MockChainValidator.ValidationCallBack
            >();

        private Action<MockChainValidator.ValidationCallBack> onCallHandler;

        public MockChainValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override ValidationReport Validate(ValidationReport result, ValidationContext context, IX509Certificate
             certificate, DateTime verificationDate) {
            MockChainValidator.ValidationCallBack call = new MockChainValidator.ValidationCallBack(certificate, context
                , result, verificationDate);
            if (onCallHandler != null) {
                onCallHandler(call);
            }
            verificationCalls.Add(call);
            return result;
        }

        public virtual void OnCallDo(Action<MockChainValidator.ValidationCallBack> c) {
            onCallHandler = c;
        }

        public sealed class ValidationCallBack {
            public readonly IX509Certificate certificate;

            public readonly ValidationContext context;

            public readonly ValidationReport report;

            public readonly DateTime checkDate;

            public ValidationCallBack(IX509Certificate certificate, ValidationContext context, ValidationReport report
                , DateTime checkDate) {
                this.certificate = certificate;
                this.context = context;
                this.report = report;
                this.checkDate = checkDate;
            }
        }
    }
}
