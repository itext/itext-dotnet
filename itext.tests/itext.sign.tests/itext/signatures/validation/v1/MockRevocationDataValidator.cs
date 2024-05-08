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
