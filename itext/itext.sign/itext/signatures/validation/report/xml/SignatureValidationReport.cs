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
using iText.Commons.Utils;
using iText.Signatures.Cms;

namespace iText.Signatures.Validation.Report.Xml {
//\cond DO_NOT_DOCUMENT
    internal class SignatureValidationReport : AbstractIdentifiableObject, SubValidationReport {
        private readonly SignatureIdentifier signatureIdentifier;

        private SignatureValidationStatus status;

        public SignatureValidationReport(ValidationObjects validationObjects, CMSContainer signature, String signatureName
            , DateTime signingDate)
            : base("S") {
            signatureIdentifier = new SignatureIdentifier(validationObjects, signature, signatureName, signingDate);
        }

        public virtual SignatureIdentifier GetSignatureIdentifier() {
            return signatureIdentifier;
        }

        public virtual void SetSignatureValidationStatus(SignatureValidationStatus status) {
            this.status = status;
        }

        public virtual SignatureValidationStatus GetSignatureValidationStatus() {
            return status;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Signatures.Validation.Report.Xml.SignatureValidationReport that = (iText.Signatures.Validation.Report.Xml.SignatureValidationReport
                )o;
            return signatureIdentifier.Equals(that.signatureIdentifier) && (status == null || status.Equals(that.status
                ));
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)signatureIdentifier, status);
        }
    }
//\endcond
}
