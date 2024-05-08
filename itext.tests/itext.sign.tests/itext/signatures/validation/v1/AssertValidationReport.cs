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
using System.Text;
using NUnit.Framework;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    public class AssertValidationReport : IDisposable {
        private readonly ValidationReport report;

        private readonly AssertValidationReport.CheckChain chain = new AssertValidationReport.StartOfChain();

        private bool asserted = false;

        private AssertValidationReport(ValidationReport report) {
            this.report = report;
        }

        public static void AssertThat(ValidationReport report, Action<iText.Signatures.Validation.V1.AssertValidationReport
            > c) {
            iText.Signatures.Validation.V1.AssertValidationReport assertion = new iText.Signatures.Validation.V1.AssertValidationReport
                (report);
            c(assertion);
            assertion.DoAssert();
        }

        private void DoAssert() {
            asserted = true;
            AssertValidationReport.CheckResult result = new AssertValidationReport.CheckResult();
            chain.Run(report, result);
            if (!result.success) {
                result.messageBuilder.Append("\n For report: ").Append(report);
                throw new AssertionException(result.messageBuilder.ToString());
            }
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasNumberOfFailures(int i) {
            chain.SetNext(new AssertValidationReport.FailureCountCheck(i));
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasNumberOfLogs(int i) {
            chain.SetNext(new AssertValidationReport.LogCountCheck(i));
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasLogItem(ReportItem logItem) {
            chain.SetNext(new AssertValidationReport.LogItemCheck(logItem));
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasLogItem(Action<AssertValidationReport.AssertValidationReportLogItem
            > c) {
            AssertValidationReport.AssertValidationReportLogItem asserter = new AssertValidationReport.AssertValidationReportLogItem
                (1, 1);
            c(asserter);
            asserter.AddToChain(this);
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasLogItems(int minCount, int maxCount
            , Action<AssertValidationReport.AssertValidationReportLogItem> c) {
            AssertValidationReport.AssertValidationReportLogItem asserter = new AssertValidationReport.AssertValidationReportLogItem
                (minCount, maxCount);
            c(asserter);
            asserter.AddToChain(this);
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasStatus(ValidationReport.ValidationResult
             expectedStatus) {
            chain.SetNext((new AssertValidationReport.StatusCheck(expectedStatus)));
            return this;
        }

        public virtual void Close() {
            if (!asserted) {
                throw new InvalidOperationException("AssertValidationReport not asserted!");
            }
        }

        public class AssertValidationReportLogItem {
            private readonly AssertValidationReport.ValidationReportLogItemCheck check;

            public AssertValidationReportLogItem(int minCount, int maxCount) {
                this.check = new AssertValidationReport.ValidationReportLogItemCheck(minCount, maxCount);
            }

            public virtual AssertValidationReport.AssertValidationReportLogItem WithCheckName(String checkName) {
                check.WithCheckName(checkName);
                return this;
            }

            public AssertValidationReport.AssertValidationReportLogItem WithMessage(String message, params Func<ReportItem
                , Object>[] @params) {
                check.WithMessage(message, @params);
                return this;
            }

            public virtual AssertValidationReport.AssertValidationReportLogItem WithStatus(ReportItem.ReportItemStatus
                 status) {
                check.WithStatus(status);
                return this;
            }

            public virtual AssertValidationReport.AssertValidationReportLogItem WithCertificate(IX509Certificate certificate
                ) {
                check.WithCertificate(certificate);
                return this;
            }

            public virtual AssertValidationReport.AssertValidationReportLogItem WithExceptionCauseType(Type exceptionType
                ) {
                check.WithExceptionCauseType(exceptionType);
                return this;
            }

            public virtual void AddToChain(AssertValidationReport asserter) {
                asserter.chain.SetNext(check);
            }
        }

        private class CheckResult {
            public StringBuilder messageBuilder = new StringBuilder("\n");

            public bool success = true;
        }

        private abstract class CheckChain {
            private AssertValidationReport.CheckChain next;

            protected internal abstract void Check(ValidationReport report, AssertValidationReport.CheckResult result);

            public virtual void Run(ValidationReport report, AssertValidationReport.CheckResult result) {
                Check(report, result);
                next = GetNext();
                if (next == null) {
                    return;
                }
                next.Run(report, result);
            }

            public virtual AssertValidationReport.CheckChain GetNext() {
                return next;
            }

            public virtual void SetNext(AssertValidationReport.CheckChain next) {
                if (this.next == null) {
                    this.next = next;
                }
                else {
                    this.next.SetNext(next);
                }
            }
        }

        private class StartOfChain : AssertValidationReport.CheckChain {
            protected internal override void Check(ValidationReport report, AssertValidationReport.CheckResult result) {
            }
        }

        private class FailureCountCheck : AssertValidationReport.CheckChain {
            private readonly int expected;

            public FailureCountCheck(int expected)
                : base() {
                this.expected = expected;
            }

            protected internal override void Check(ValidationReport report, AssertValidationReport.CheckResult result) {
                if (report.GetFailures().Count != expected) {
                    result.success = false;
                    result.messageBuilder.Append("\nExpected ").Append(expected).Append(" failures but found ").Append(report.
                        GetFailures().Count);
                }
            }
        }

        private class ValidationReportLogItemCheck : AssertValidationReport.CheckChain {
            private readonly int minCount;

            private readonly int maxCount;

            private readonly IList<Func<ReportItem, Object>> messageParams = new List<Func<ReportItem, Object>>();

            private readonly StringBuilder errorMessage = new StringBuilder();

            private String checkName;

            private String message;

            private ReportItem.ReportItemStatus status;

            private bool checkStatus = false;

            private IX509Certificate certificate;

            private Type exceptionType;

            public ValidationReportLogItemCheck(int minCount, int maxCount) {
                this.minCount = minCount;
                this.maxCount = maxCount;
                errorMessage.Append("\nExpected between ").Append(minCount).Append(" and ").Append(maxCount).Append(" message with "
                    );
            }

            public virtual void WithCheckName(String checkName) {
                this.checkName = checkName;
                errorMessage.Append(" check name '").Append(checkName).Append("'");
            }

            public virtual void WithMessage(String message, params Func<ReportItem, Object>[] @params) {
                this.message = message;
                messageParams.AddAll(@params);
                errorMessage.Append(" message '").Append(message).Append("'");
            }

            public virtual void WithStatus(ReportItem.ReportItemStatus status) {
                this.status = status;
                checkStatus = true;
                errorMessage.Append(" status '").Append(status).Append("'");
            }

            public virtual void WithCertificate(IX509Certificate certificate) {
                this.certificate = certificate;
                errorMessage.Append(" certificate '").Append(certificate.GetSubjectDN()).Append("'");
            }

            public virtual void WithExceptionCauseType(Type exceptionType) {
                this.exceptionType = exceptionType;
                errorMessage.Append(" with exception cause '").Append(exceptionType.FullName).Append("'");
            }

            protected internal override void Check(ValidationReport report, AssertValidationReport.CheckResult result) {
                errorMessage.Append("\n");
                IList<ReportItem> prefiltered;
                if (message != null) {
                    prefiltered = report.GetLogs().Where((i) => {
                        Object[] @params = new Object[messageParams.Count];
                        for (int p = 0; p < messageParams.Count; p++) {
                            @params[p] = messageParams[p].Invoke(i);
                        }
                        return i.GetMessage().Equals(MessageFormatUtil.Format(message, @params));
                    }
                    ).ToList();
                    errorMessage.Append("found ").Append(prefiltered.Count).Append(" matches after message filter\n");
                }
                else {
                    prefiltered = report.GetLogs();
                }
                if (checkName != null) {
                    prefiltered = prefiltered.Where((i) => (checkName.Equals(i.GetCheckName()))).ToList();
                    errorMessage.Append("found ").Append(prefiltered.Count).Append(" matches after check name filter\n");
                }
                if (checkStatus) {
                    prefiltered = prefiltered.Where((i) => (status.Equals(i.GetStatus()))).ToList();
                    errorMessage.Append("found ").Append(prefiltered.Count).Append(" matches after status filter\n");
                }
                if (certificate != null) {
                    prefiltered = prefiltered.Where((i) => certificate.Equals(((CertificateReportItem)i).GetCertificate())).ToList
                        ();
                    errorMessage.Append("found ").Append(prefiltered.Count).Append(" matches after certificate filter\n");
                }
                if (exceptionType != null) {
                    prefiltered = prefiltered.Where((i) => i.GetExceptionCause() != null && exceptionType.IsAssignableFrom(i.GetExceptionCause
                        ().GetType())).ToList();
                    errorMessage.Append("found ").Append(prefiltered.Count).Append(" matches after exception cause filter\n");
                }
                long foundCount = prefiltered.Count;
                if (foundCount < minCount || foundCount > maxCount) {
                    result.success = false;
                    result.messageBuilder.Append(errorMessage);
                }
            }

            public override String ToString() {
                return "checkName='" + checkName + '\'' + ", message='" + message + '\'' + ", status=" + status + ", certificate="
                     + (certificate == null ? "null" : certificate.GetSubjectDN().ToString()) + ", exceptionType=" + (exceptionType
                     == null ? "null" : exceptionType.FullName);
            }
        }

        private class LogCountCheck : AssertValidationReport.CheckChain {
            private readonly int expected;

            public LogCountCheck(int expected)
                : base() {
                this.expected = expected;
            }

            protected internal override void Check(ValidationReport report, AssertValidationReport.CheckResult result) {
                if (report.GetLogs().Count != expected) {
                    result.success = false;
                    result.messageBuilder.Append("\nExpected ").Append(expected).Append(" logs but found ").Append(report.GetLogs
                        ().Count);
                }
            }
        }

        private class LogItemCheck : AssertValidationReport.CheckChain {
            private readonly ReportItem expectedItem;

            public LogItemCheck(ReportItem expectedItem)
                : base() {
                this.expectedItem = expectedItem;
            }

            protected internal override void Check(ValidationReport report, AssertValidationReport.CheckResult result) {
                if (!report.GetLogs().Contains(expectedItem)) {
                    result.success = false;
                    result.messageBuilder.Append("\nExpected report item not found:").Append(expectedItem);
                }
            }
        }

        private class StatusCheck : AssertValidationReport.CheckChain {
            private readonly ValidationReport.ValidationResult expectedStatus;

            public StatusCheck(ValidationReport.ValidationResult expectedStatus)
                : base() {
                this.expectedStatus = expectedStatus;
            }

            protected internal override void Check(ValidationReport report, AssertValidationReport.CheckResult result) {
                if (!expectedStatus.Equals(report.GetValidationResult())) {
                    result.success = false;
                    result.messageBuilder.Append("\nExpetected validationResult of ").Append(expectedStatus).Append(" but found "
                        ).Append(report.GetValidationResult());
                }
            }
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
