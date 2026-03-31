/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Linq;
using System.Text;
using iText.Commons.Json;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;

namespace iText.Signatures.Validation.Report {
    /// <summary>Validation report, which contains detailed validation results.</summary>
    public class ValidationReport : IJsonSerializable {
        private const String JSON_KEY_REPORT_ITEMS = "reportItems";

        private readonly IList<ReportItem> reportItems = new List<ReportItem>();

        /// <summary>
        /// Create new instance of
        /// <see cref="ValidationReport"/>.
        /// </summary>
        public ValidationReport() {
        }

        // Declaring default constructor explicitly to avoid removing it unintentionally.
        /// <summary>Create a copy of another validation report.</summary>
        /// <param name="report">to be copied</param>
        public ValidationReport(iText.Signatures.Validation.Report.ValidationReport report) {
            foreach (ReportItem item in report.reportItems) {
                this.AddReportItem(new ReportItem(item));
            }
        }

        /// <summary>Get the result of a validation process.</summary>
        /// <returns>
        /// 
        /// <see cref="ValidationResult"/>
        /// , which represents the result of a validation
        /// </returns>
        public virtual ValidationReport.ValidationResult GetValidationResult() {
            if (reportItems.Any((reportItem) => reportItem.GetStatus() == ReportItem.ReportItemStatus.INVALID)) {
                return ValidationReport.ValidationResult.INVALID;
            }
            if (reportItems.Any((reportItem) => reportItem.GetStatus() == ReportItem.ReportItemStatus.INDETERMINATE)) {
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
            return reportItems.Where((item) => item.GetStatus() != ReportItem.ReportItemStatus.INFO).ToList();
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

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override String ToString() {
            StringBuilder sb = new StringBuilder("ValidationReport{validationResult=");
            sb.Append(GetValidationResult()).Append("\nreportItems=");
            foreach (ReportItem i in reportItems) {
                sb.Append(i).Append(", ");
            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Merge all
        /// <see cref="ReportItem"/>
        /// objects from sub report into this one.
        /// </summary>
        /// <param name="subReport">report from which items will be merged</param>
        /// <returns>
        /// 
        /// <see cref="ValidationReport"/>
        /// the same updated validation report instance.
        /// </returns>
        public virtual iText.Signatures.Validation.Report.ValidationReport Merge(iText.Signatures.Validation.Report.ValidationReport
             subReport) {
            if (subReport != null) {
                foreach (ReportItem item in subReport.GetLogs()) {
                    AddReportItem(item);
                }
            }
            return this;
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual JsonValue ToJson() {
            // Sort the items by check name to ensure consistent order
            IList<ReportItem> sortedItems = GetLogs().Sorted((item1, item2) => {
                if (item1.GetCheckName() == null && item2.GetCheckName() == null) {
                    return 0;
                }
                else {
                    if (item1.GetCheckName() == null) {
                        return -1;
                    }
                    else {
                        if (item2.GetCheckName() == null) {
                            return 1;
                        }
                        else {
                            return string.CompareOrdinal(item1.GetCheckName(), item2.GetCheckName());
                        }
                    }
                }
            }
            ).ToList();
            JsonArray reportItemsJson = new JsonArray();
            foreach (ReportItem reportItem in sortedItems) {
                reportItemsJson.Add(reportItem.ToJson());
            }
            JsonObject validationReportJson = new JsonObject();
            validationReportJson.Add(JSON_KEY_REPORT_ITEMS, reportItemsJson);
            return validationReportJson;
        }

        /// <summary>
        /// Deserializes
        /// <see cref="iText.Commons.Json.JsonValue"/>
        /// into
        /// <see cref="ValidationReport"/>.
        /// </summary>
        /// <param name="jsonValue">
        /// 
        /// <see cref="iText.Commons.Json.JsonValue"/>
        /// to deserialize
        /// </param>
        /// <returns>
        /// deserialized
        /// <see cref="ValidationReport"/>
        /// </returns>
        public static iText.Signatures.Validation.Report.ValidationReport FromJson(JsonValue jsonValue) {
            JsonObject validationReportJson = (JsonObject)jsonValue;
            JsonArray reportItemsJson = (JsonArray)validationReportJson.GetField(JSON_KEY_REPORT_ITEMS);
            iText.Signatures.Validation.Report.ValidationReport validationReportFromJson = new iText.Signatures.Validation.Report.ValidationReport
                ();
            foreach (ReportItem reportItem in reportItemsJson.GetValues().Select((reportItemJson) => ReportItem.FromJson
                (reportItemJson)).ToList()) {
                validationReportFromJson.AddReportItem(reportItem);
            }
            return validationReportFromJson;
        }

        /// <summary>
        /// Merge all
        /// <see cref="ReportItem"/>
        /// objects from sub report into this one with different status.
        /// </summary>
        /// <param name="subReport">report from which items will be merged</param>
        /// <param name="newStatus">
        /// 
        /// <see cref="ReportItemStatus"/>
        /// which will be used instead of provided ones
        /// </param>
        /// <returns>
        /// 
        /// <see cref="ValidationReport"/>
        /// the same updated validation report instance.
        /// </returns>
        public virtual iText.Signatures.Validation.Report.ValidationReport MergeWithDifferentStatus(iText.Signatures.Validation.Report.ValidationReport
             subReport, ReportItem.ReportItemStatus newStatus) {
            if (subReport != null) {
                foreach (ReportItem item in subReport.GetLogs()) {
                    AddReportItem(new ReportItem(item).SetStatus(newStatus));
                }
            }
            return this;
        }

        /// <summary>Enum representing possible validation results.</summary>
        public enum ValidationResult {
            /// <summary>Valid validation result.</summary>
            VALID,
            /// <summary>Invalid validation result.</summary>
            INVALID,
            /// <summary>Indeterminate validation result.</summary>
            INDETERMINATE
        }
    }
}
