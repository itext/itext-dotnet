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
