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
using iText.Signatures.Cms;

namespace iText.Signatures.Validation.Report.Xml {
//\cond DO_NOT_DOCUMENT
    internal class POEValidationReport : AbstractCollectableObject, SubValidationReport {
        public POEValidationReport(ValidationObjects validationObjects, CMSContainer cmsContainer, bool document)
            : base("T") {
        }

        // Will be completed later
        public virtual void SetSignatureValidationStatus(SignatureValidationStatus status) {
        }

        // Will be completed later
        public override void Accept(CollectableObjectVisitor visitor) {
            visitor.Visit(this);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Signatures.Validation.Report.Xml.POEValidationReport that = (iText.Signatures.Validation.Report.Xml.POEValidationReport
                )o;
            // Will be completed later
            return true;
        }

        public override int GetHashCode() {
            // Will be completed later
            return 0;
        }
    }
//\endcond
}
