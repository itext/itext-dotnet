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
using System.Linq;
using iText.Commons.Utils;

namespace iText.Signatures.Validation {
    /// <summary>Validation report, which contains detailed validation results.</summary>
    public class ValidationReport {
        private readonly IList<ReportItem> reportItems = new List<ReportItem>();

        /// <summary>
        /// Create new instance of
        /// <see cref="ValidationReport"/>.
        /// </summary>
        public ValidationReport() {
        }

        // Empty constructor.
        /// <summary>Get the result of a validation process.</summary>
        /// <returns>
        /// 
        /// <see cref="ValidationResult"/>
        /// , which represents the result of a validation
        /// </returns>
        public virtual ValidationReport.ValidationResult GetValidationResult() {
            if (reportItems.Any((reportItem) => reportItem.result == ValidationReport.ValidationResult.INVALID)) {
                return ValidationReport.ValidationResult.INVALID;
            }
            if (reportItems.Any((reportItem) => reportItem.result == ValidationReport.ValidationResult.INDETERMINATE)) {
                return ValidationReport.ValidationResult.INDETERMINATE;
            }
            return ValidationReport.ValidationResult.VALID;
        }

        /// <summary>Get all failures recognized during a validation process.</summary>
        /// <returns>
        /// report items
        /// <see cref="System.Collections.IList{E}"/>
        /// , which contains all recognized failures
        /// </returns>
        public virtual IList<ReportItem> GetFailures() {
            return reportItems.Where((item) => item.result != ValidationReport.ValidationResult.VALID).ToList();
        }

        /// <summary>Get list of failures, which are related to certificate validation.</summary>
        /// <returns>
        /// report items
        /// <see cref="System.Collections.IList{E}"/>
        /// , which contains only
        /// <see cref="CertificateReportItem"/>
        /// failures
        /// </returns>
        public virtual IList<CertificateReportItem> GetCertificateFailures() {
            return GetFailures().Where((item) => item is CertificateReportItem).Select((item) => (CertificateReportItem
                )item).ToList();
        }

        /// <summary>Get all log messages reported during a validation process.</summary>
        /// <returns>
        /// report items
        /// <see cref="System.Collections.IList{E}"/>
        /// , which contains all reported log messages, related to validation
        /// </returns>
        public virtual IList<ReportItem> GetLogs() {
            return JavaCollectionsUtil.UnmodifiableList(reportItems);
        }

        /// <summary>Get list of log messages, which are related to certificate validation.</summary>
        /// <returns>
        /// report items
        /// <see cref="System.Collections.IList{E}"/>
        /// , which contains only
        /// <see cref="CertificateReportItem"/>
        /// log messages
        /// </returns>
        public virtual IList<CertificateReportItem> GetCertificateLogs() {
            return reportItems.Where((item) => item is CertificateReportItem).Select((item) => (CertificateReportItem)
                item).ToList();
        }

        /// <summary>Add new report item to the overall validation result.</summary>
        /// <param name="item">
        /// 
        /// <see cref="ReportItem"/>
        /// to be added
        /// </param>
        public virtual void AddReportItem(ReportItem item) {
            reportItems.Add(item);
        }

        /// <summary>Enum representing possible validation results.</summary>
        public enum ValidationResult {
            /// <summary>Result for valid certificate.</summary>
            VALID,
            /// <summary>Result for invalid certificate.</summary>
            INVALID,
            /// <summary>Result for certificate, which status is indeterminate.</summary>
            INDETERMINATE
        }
    }
}
