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
