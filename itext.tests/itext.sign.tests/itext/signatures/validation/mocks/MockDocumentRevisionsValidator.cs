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
using iText.Kernel.Pdf;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Mocks {
    public class MockDocumentRevisionsValidator : DocumentRevisionsValidator {
        public Action<MockDocumentRevisionsValidator.RevisionsValidatorCall> onCallHandler;

        private ReportItem.ReportItemStatus reportItemStatus = ReportItem.ReportItemStatus.INFO;

        private IList<MockDocumentRevisionsValidator.RevisionsValidatorCall> calls = new List<MockDocumentRevisionsValidator.RevisionsValidatorCall
            >();

        public MockDocumentRevisionsValidator()
            : base(new ValidatorChainBuilder()) {
        }

        public override ValidationReport ValidateAllDocumentRevisions(ValidationContext context, PdfDocument document
            ) {
            MockDocumentRevisionsValidator.RevisionsValidatorCall call = new MockDocumentRevisionsValidator.RevisionsValidatorCall
                (context, document);
            calls.Add(call);
            if (onCallHandler != null) {
                onCallHandler(call);
            }
            ValidationReport report = new ValidationReport();
            if (reportItemStatus != ReportItem.ReportItemStatus.INFO) {
                report.AddReportItem(new ReportItem("test", "test", reportItemStatus));
            }
            return report;
        }

        public virtual void SetReportItemStatus(ReportItem.ReportItemStatus reportItemStatus) {
            this.reportItemStatus = reportItemStatus;
        }

        public virtual void OnCallDo(Action<MockDocumentRevisionsValidator.RevisionsValidatorCall> callback) {
            onCallHandler = callback;
        }

        public class RevisionsValidatorCall {
            public readonly ValidationContext context;

            public readonly PdfDocument document;

            public RevisionsValidatorCall(ValidationContext context, PdfDocument document) {
                this.context = context;
                this.document = document;
            }
        }
    }
}
