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
using System.Collections.Generic;

namespace iText.Signatures.Validation.Report.Xml {
    /// <summary>This class holds all parts needed to create an xml AdES report.</summary>
    public class PadesValidationReport {
        private readonly ICollection<SignatureValidationReport> signatureValidationReports = new List<SignatureValidationReport
            >();

        private readonly ValidationObjects validationObjects;

//\cond DO_NOT_DOCUMENT
        internal PadesValidationReport(ValidationObjects validationObjects) {
            this.validationObjects = validationObjects;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddSignatureValidationReport(SignatureValidationReport signatureValidationReport) {
            signatureValidationReports.Add(signatureValidationReport);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<SignatureValidationReport> GetSignatureValidationReports() {
            return signatureValidationReports;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<CollectableObject> GetValidationObjects() {
            return validationObjects.GetObjects();
        }
//\endcond
    }
}
