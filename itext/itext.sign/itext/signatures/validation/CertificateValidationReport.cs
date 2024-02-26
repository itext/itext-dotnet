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
using System.Linq;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Signatures.Validation {
    /// <summary>Certificate validation report, which contains detailed validation results.</summary>
    public class CertificateValidationReport {
        private readonly IX509Certificate investigatedCertificate;

        private readonly IList<CertificateValidationReport.ReportItem> failures = new List<CertificateValidationReport.ReportItem
            >();

        private readonly IList<CertificateValidationReport.ReportItem> logs = new List<CertificateValidationReport.ReportItem
            >();

        /// <summary>
        /// Create new instance of
        /// <see cref="CertificateValidationReport"/>.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// , against which validation process was started.
        /// </param>
        public CertificateValidationReport(IX509Certificate certificate) {
            investigatedCertificate = certificate;
        }

        /// <summary>Get the certificate, against which validation process was started.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// , against which validation process was started
        /// </returns>
        public virtual IX509Certificate GetInvestigatedCertificate() {
            return investigatedCertificate;
        }

        /// <summary>Get the result of a validation process.</summary>
        /// <returns>
        /// 
        /// <see cref="ValidationResult"/>
        /// , which represents the result of a validation
        /// </returns>
        public virtual CertificateValidationReport.ValidationResult GetValidationResult() {
            if (failures.Any((reportItem) => reportItem.result == CertificateValidationReport.ValidationResult.INVALID
                )) {
                return CertificateValidationReport.ValidationResult.INVALID;
            }
            if (failures.Any((reportItem) => reportItem.result == CertificateValidationReport.ValidationResult.INDETERMINATE
                )) {
                return CertificateValidationReport.ValidationResult.INDETERMINATE;
            }
            return CertificateValidationReport.ValidationResult.VALID;
        }

        /// <summary>Get all failures recognized during a validation process.</summary>
        /// <returns>
        /// report items
        /// <see cref="System.Collections.IList{E}"/>
        /// , which contains all recognized failures
        /// </returns>
        public virtual IList<CertificateValidationReport.ReportItem> GetFailures() {
            return JavaCollectionsUtil.UnmodifiableList(failures);
        }

        /// <summary>Get all log messages reported during a validation process.</summary>
        /// <returns>
        /// report items
        /// <see cref="System.Collections.IList{E}"/>
        /// , which contains all reported log messages, related to validation
        /// </returns>
        public virtual IList<CertificateValidationReport.ReportItem> GetLogs() {
            return JavaCollectionsUtil.UnmodifiableList(logs);
        }

        /// <summary>Add new failure message to the overall validation result.</summary>
        /// <param name="failingCertificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// processing which failure occurred
        /// </param>
        /// <param name="check">
        /// 
        /// <see cref="System.String"/>
        /// , which represents a check name during which failure occurred
        /// </param>
        /// <param name="message">
        /// 
        /// <see cref="System.String"/>
        /// with the exact failure message
        /// </param>
        /// <param name="e">
        /// 
        /// <see cref="System.Exception"/>
        /// , which is the root cause of this failure
        /// </param>
        /// <param name="result">
        /// 
        /// <see cref="ValidationResult"/>
        /// , which shall be used as a validation result because of this failure
        /// </param>
        public virtual void AddFailure(IX509Certificate failingCertificate, String check, String message, Exception
             e, CertificateValidationReport.ValidationResult result) {
            CertificateValidationReport.ReportItem item = new CertificateValidationReport.ReportItem(failingCertificate
                , check, message, e, result);
            failures.Add(item);
            logs.Add(item);
        }

        /// <summary>Add new failure message to the overall validation result.</summary>
        /// <param name="failingCertificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// processing which failure occurred
        /// </param>
        /// <param name="check">
        /// 
        /// <see cref="System.String"/>
        /// , which represents a check name during which failure occurred
        /// </param>
        /// <param name="message">
        /// 
        /// <see cref="System.String"/>
        /// with the exact failure message
        /// </param>
        /// <param name="e">
        /// 
        /// <see cref="System.Exception"/>
        /// , which is the root cause of this failure
        /// </param>
        public virtual void AddFailure(IX509Certificate failingCertificate, String check, String message, Exception
             e) {
            AddFailure(failingCertificate, check, message, e, CertificateValidationReport.ValidationResult.INVALID);
        }

        /// <summary>Add new failure message to the overall validation result.</summary>
        /// <param name="failingCertificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// processing which failure occurred
        /// </param>
        /// <param name="check">
        /// 
        /// <see cref="System.String"/>
        /// , which represents a check name during which failure occurred
        /// </param>
        /// <param name="message">
        /// 
        /// <see cref="System.String"/>
        /// with the exact failure message
        /// </param>
        /// <param name="result">
        /// 
        /// <see cref="ValidationResult"/>
        /// , which shall be used as a validation result because of this failure
        /// </param>
        public virtual void AddFailure(IX509Certificate failingCertificate, String check, String message, CertificateValidationReport.ValidationResult
             result) {
            AddFailure(failingCertificate, check, message, null, result);
        }

        /// <summary>Add new failure message to the overall validation result.</summary>
        /// <param name="failingCertificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// processing which failure occurred
        /// </param>
        /// <param name="check">
        /// 
        /// <see cref="System.String"/>
        /// , which represents a check name during which failure occurred
        /// </param>
        /// <param name="message">
        /// 
        /// <see cref="System.String"/>
        /// with the exact failure message
        /// </param>
        public virtual void AddFailure(IX509Certificate failingCertificate, String check, String message) {
            AddFailure(failingCertificate, check, message, (Exception)null);
        }

        /// <summary>Add new log message to the overall validation result.</summary>
        /// <param name="currentCertificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// processing which log message occurred
        /// </param>
        /// <param name="check">
        /// 
        /// <see cref="System.String"/>
        /// , which represents a check name during which log message occurred
        /// </param>
        /// <param name="message">
        /// 
        /// <see cref="System.String"/>
        /// with the exact log message
        /// </param>
        public virtual void AddLog(IX509Certificate currentCertificate, String check, String message) {
            logs.Add(new CertificateValidationReport.ReportItem(currentCertificate, check, message, CertificateValidationReport.ValidationResult
                .VALID));
        }

        /// <summary>Report item to be used for single failure or log message.</summary>
        public class ReportItem {
            private readonly IX509Certificate certificate;

            private readonly String checkName;

            private readonly String message;

            private readonly Exception cause;

            internal readonly CertificateValidationReport.ValidationResult result;

            /// <summary>
            /// Create
            /// <see cref="ReportItem"/>
            /// instance.
            /// </summary>
            /// <param name="certificate">
            /// 
            /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
            /// processing which report item occurred
            /// </param>
            /// <param name="checkName">
            /// 
            /// <see cref="System.String"/>
            /// , which represents a check name during which report item occurred
            /// </param>
            /// <param name="message">
            /// 
            /// <see cref="System.String"/>
            /// with the exact report item message
            /// </param>
            /// <param name="result">
            /// 
            /// <see cref="ValidationResult"/>
            /// , which this report item leads to
            /// </param>
            public ReportItem(IX509Certificate certificate, String checkName, String message, CertificateValidationReport.ValidationResult
                 result)
                : this(certificate, checkName, message, null, result) {
            }

            /// <summary>
            /// Create
            /// <see cref="ReportItem"/>
            /// instance.
            /// </summary>
            /// <param name="certificate">
            /// 
            /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
            /// processing which report item occurred
            /// </param>
            /// <param name="checkName">
            /// 
            /// <see cref="System.String"/>
            /// , which represents a check name during which report item occurred
            /// </param>
            /// <param name="message">
            /// 
            /// <see cref="System.String"/>
            /// with the exact report item message
            /// </param>
            /// <param name="cause">
            /// 
            /// <see cref="System.Exception"/>
            /// , which caused this report item
            /// </param>
            /// <param name="result">
            /// 
            /// <see cref="ValidationResult"/>
            /// , which this report item leads to
            /// </param>
            public ReportItem(IX509Certificate certificate, String checkName, String message, Exception cause, CertificateValidationReport.ValidationResult
                 result) {
                this.certificate = certificate;
                this.checkName = checkName;
                this.message = message;
                this.cause = cause;
                this.result = result;
            }

            /// <summary>Get the certificate related to this report item.</summary>
            /// <returns>
            /// 
            /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
            /// related to this report item
            /// </returns>
            public virtual IX509Certificate GetCertificate() {
                return certificate;
            }

            /// <summary>Get the check name related to this report item.</summary>
            /// <returns>
            /// 
            /// <see cref="System.String"/>
            /// check name related to this report item
            /// </returns>
            public virtual String GetCheckName() {
                return checkName;
            }

            /// <summary>Get the message related to this report item.</summary>
            /// <returns>
            /// 
            /// <see cref="System.String"/>
            /// message related to this report item
            /// </returns>
            public virtual String GetMessage() {
                return message;
            }

            /// <summary>Get the exception, which caused this report item.</summary>
            /// <returns>
            /// 
            /// <see cref="System.Exception"/>
            /// , which cause this report item
            /// </returns>
            public virtual Exception GetExceptionCause() {
                return cause;
            }

            /// <summary>Get validation result this report item leads to.</summary>
            /// <returns>
            /// 
            /// <see cref="ValidationResult"/>
            /// this report item leads to
            /// </returns>
            public virtual CertificateValidationReport.ValidationResult GetValidationResult() {
                return result;
            }
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
