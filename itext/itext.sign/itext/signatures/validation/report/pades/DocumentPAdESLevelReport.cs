using System;
using System.Collections.Generic;
using System.Text;

namespace iText.Signatures.Validation.Report.Pades {
    /// <summary>
    /// This report gathers PAdES level information about all signatures in a document
    /// as well as an overall PAdES level.
    /// </summary>
    public class DocumentPAdESLevelReport {
        private readonly IDictionary<String, PAdESLevelReport> signatureReports = new Dictionary<String, PAdESLevelReport
            >();

        /// <summary>Creates a new instance.</summary>
        public DocumentPAdESLevelReport() {
        }

        // Empty constructor
        /// <summary>Adds a signature validation report.</summary>
        /// <param name="report">a signature validation report</param>
        public virtual void AddPAdESReport(PAdESLevelReport report) {
            signatureReports.Put(report.GetSignatureName(), report);
        }

        /// <summary>Returns the overall document PAdES level, the lowest level off all signatures.</summary>
        /// <returns>the overall document PAdES level</returns>
        public virtual PAdESLevel GetDocumentLevel() {
            if (signatureReports.IsEmpty()) {
                return PAdESLevel.NONE;
            }
            PAdESLevel result = PAdESLevel.B_LTA;
            foreach (PAdESLevelReport rep in signatureReports.Values) {
                if (rep.GetLevel().CompareTo(result) < 0) {
                    result = rep.GetLevel();
                }
            }
            return result;
        }

        /// <summary>Returns the individual PAdES level report for a signature by name.</summary>
        /// <param name="signatureName">the signature name to retrieve the report for</param>
        /// <returns>the individual PAdES level report for the signature</returns>
        public virtual PAdESLevelReport GetSignatureReport(String signatureName) {
            return signatureReports.Get(signatureName);
        }

        /// <summary>Returns a map for all signatures PAdES reports.</summary>
        /// <returns>a map for all signatures PAdES reports</returns>
        public virtual IDictionary<String, PAdESLevelReport> GetSignatureReports() {
            return signatureReports;
        }

        public override String ToString() {
            StringBuilder sb = new StringBuilder(70);
            sb.Append("Document highest PAdES level: ").Append(GetDocumentLevel()).Append('\n');
            foreach (PAdESLevelReport report in this.signatureReports.Values) {
                sb.Append(report).Append('\n');
            }
            return sb.ToString();
        }
    }
}
