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
using iText.Kernel.Pdf;
using iText.Signatures.Validation.V1;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1.Mocks {
    public class MockDocumentRevisionsValidator : DocumentRevisionsValidator {
        private ReportItem.ReportItemStatus reportItemStatus = ReportItem.ReportItemStatus.INFO;

        public MockDocumentRevisionsValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override ValidationReport ValidateAllDocumentRevisions(ValidationContext context, PdfDocument document
            ) {
            ValidationReport report = new ValidationReport();
            if (reportItemStatus != ReportItem.ReportItemStatus.INFO) {
                report.AddReportItem(new ReportItem("test", "test", reportItemStatus));
            }
            return report;
        }

        public virtual void SetReportItemStatus(ReportItem.ReportItemStatus reportItemStatus) {
            this.reportItemStatus = reportItemStatus;
        }
    }
}
