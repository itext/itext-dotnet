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
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Lotl;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    internal class NullQualifiedValidator : QualifiedValidator {
//\cond DO_NOT_DOCUMENT
        internal NullQualifiedValidator() {
        }
//\endcond

        // Empty constructor.
        public override QualifiedValidator.QualificationValidationData ObtainQualificationValidationResultForSignature
            (String signatureName) {
            return null;
        }

        public override IDictionary<String, QualifiedValidator.QualificationValidationData> ObtainAllSignaturesValidationResults
            () {
            return new Dictionary<String, QualifiedValidator.QualificationValidationData>();
        }

        public override void StartSignatureValidation(String signatureName) {
        }

        // Do nothing.
        public override void EnsureValidatorIsEmpty() {
        }

        // Do nothing.
        protected internal override void CheckSignatureQualification(IList<IX509Certificate> previousCertificates, 
            CountryServiceContext currentContext, IX509Certificate trustedCertificate, DateTime validationDate, ValidationContext
             context) {
        }
        // Do nothing.
    }
//\endcond
}
