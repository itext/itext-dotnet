using System;

namespace iText.Test.Pdfa {
    internal class VeraPdfReportResult {
        public String VeraPdfLogs { get; set; }
        public int NonCompliantPdfaCount { get; set; }
        public String MessageResult { get; set; }
    }
}
