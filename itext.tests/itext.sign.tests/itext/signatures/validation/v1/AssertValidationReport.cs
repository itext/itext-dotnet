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
using System.Linq;
using System.Text;
using NUnit.Framework;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    public class AssertValidationReport {
        private readonly ValidationReport report;

        private readonly AssertValidationReport.CheckChain chain = new AssertValidationReport.StartOfChain();

        public AssertValidationReport(ValidationReport report) {
            this.report = report;
        }

        public virtual void DoAssert() {
            AssertValidationReport.CheckResult result = new AssertValidationReport.CheckResult();
            chain.Run(report, result);
            if (!result.success) {
                result.messageBuilder.Append("\n For item: ").Append(report);
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

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasLogItem(Func<ReportItem, bool> check
            , String itemDescription) {
            chain.SetNext(new AssertValidationReport.ItemCheck(check, 1, itemDescription));
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasLogItems(Func<ReportItem, bool> check
            , int count, String itemDescription) {
            chain.SetNext(new AssertValidationReport.ItemCheck(check, count, itemDescription));
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertValidationReport HasStatus(ValidationReport.ValidationResult
             expectedStatus) {
            chain.SetNext((new AssertValidationReport.StatusCheck(expectedStatus)));
            return this;
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

        private class ItemCheck : AssertValidationReport.CheckChain {
            private readonly Func<ReportItem, bool> check;

            private readonly String message;

            private readonly int expectedCount;

            public ItemCheck(Func<ReportItem, bool> check, int count, String itemDescription)
                : base() {
                this.check = check;
                this.expectedCount = count;
                this.message = itemDescription;
            }

            protected internal override void Check(ValidationReport report, AssertValidationReport.CheckResult result) {
                long foundCount = report.GetLogs().Where((i) => check.Invoke(i)).Count();
                if (foundCount != expectedCount) {
                    result.success = false;
                    result.messageBuilder.Append("\nExpected ").Append(expectedCount).Append(" report logs like '").Append(message
                        ).Append("' but found ").Append(foundCount);
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
    }
}
