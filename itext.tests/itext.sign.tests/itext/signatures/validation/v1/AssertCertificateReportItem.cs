using System;
using System.Text;
using NUnit.Framework;
using iText.Commons.Utils;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    public class AssertCertificateReportItem {
        private readonly CertificateReportItem item;

        private readonly AssertCertificateReportItem.CheckChain chain = new AssertCertificateReportItem.StartOfChain
            ();

        public AssertCertificateReportItem(CertificateReportItem item) {
            this.item = item;
        }

        public virtual iText.Signatures.Validation.V1.AssertCertificateReportItem HasMessage(String template, params 
            Object[] arguments) {
            chain.SetNext(new AssertCertificateReportItem.MessageChecker(MessageFormatUtil.Format(template, arguments)
                ));
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertCertificateReportItem HasCheckName(String checkName) {
            chain.SetNext(new AssertCertificateReportItem.CheckNameChecker(checkName));
            return this;
        }

        public virtual iText.Signatures.Validation.V1.AssertCertificateReportItem HasResult(ReportItem.ReportItemStatus
             reportItemStatus) {
            chain.SetNext(new AssertCertificateReportItem.StatusChecker(reportItemStatus));
            return this;
        }

        public virtual void DoAssert() {
            AssertCertificateReportItem.CheckResult result = new AssertCertificateReportItem.CheckResult();
            chain.Run(item, result);
            if (!result.success) {
                result.messageBuilder.Append("\n For item: ").Append(item);
                throw new AssertionException(result.messageBuilder.ToString());
            }
        }

        private class CheckResult {
            public StringBuilder messageBuilder = new StringBuilder("\n");

            public bool success = true;
        }

        private abstract class CheckChain {
            private AssertCertificateReportItem.CheckChain next;

            protected internal abstract void Check(CertificateReportItem item, AssertCertificateReportItem.CheckResult
                 result);

            public virtual void Run(CertificateReportItem item, AssertCertificateReportItem.CheckResult result) {
                Check(item, result);
                next = GetNext();
                if (next == null) {
                    return;
                }
                next.Run(item, result);
            }

            public virtual AssertCertificateReportItem.CheckChain GetNext() {
                return next;
            }

            public virtual void SetNext(AssertCertificateReportItem.CheckChain next) {
                if (this.next == null) {
                    this.next = next;
                }
                else {
                    this.next.SetNext(next);
                }
            }
        }

        private class StartOfChain : AssertCertificateReportItem.CheckChain {
            protected internal override void Check(CertificateReportItem item, AssertCertificateReportItem.CheckResult
                 result) {
            }
        }

        private class MessageChecker : AssertCertificateReportItem.CheckChain {
            private readonly String message;

            public MessageChecker(String message)
                : base() {
                this.message = message;
            }

            protected internal override void Check(CertificateReportItem item, AssertCertificateReportItem.CheckResult
                 result) {
                if (!message.Equals(item.GetMessage())) {
                    result.success = false;
                    result.messageBuilder.Append("Expected message '").Append(message).Append("' but found '").Append(item.GetMessage
                        ()).Append("'.\n");
                }
            }
        }

        private class CheckNameChecker : AssertCertificateReportItem.CheckChain {
            private readonly String checkName;

            public CheckNameChecker(String checkName)
                : base() {
                this.checkName = checkName;
            }

            protected internal override void Check(CertificateReportItem item, AssertCertificateReportItem.CheckResult
                 result) {
                if (!checkName.Equals(item.GetCheckName())) {
                    result.success = false;
                    result.messageBuilder.Append("Expected check name '").Append(checkName).Append("' but found '").Append(item
                        .GetCheckName()).Append("'.\n");
                }
            }
        }

        private class StatusChecker : AssertCertificateReportItem.CheckChain {
            private readonly ReportItem.ReportItemStatus reportItemStatus;

            public StatusChecker(ReportItem.ReportItemStatus reportItemStatus)
                : base() {
                this.reportItemStatus = reportItemStatus;
            }

            protected internal override void Check(CertificateReportItem item, AssertCertificateReportItem.CheckResult
                 result) {
                if (!reportItemStatus.Equals(item.GetStatus())) {
                    result.success = false;
                    result.messageBuilder.Append("Expected check name '").Append(reportItemStatus).Append("' but found '").Append
                        (item.GetStatus()).Append("'.\n");
                }
            }
        }
    }
}
