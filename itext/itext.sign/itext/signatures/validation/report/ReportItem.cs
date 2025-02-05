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

namespace iText.Signatures.Validation.Report {
    /// <summary>Report item to be used for single failure or log message.</summary>
    public class ReportItem {
        private readonly String checkName;

        private readonly String message;

        private readonly Exception cause;

        private ReportItem.ReportItemStatus status;

        /// <summary>
        /// Create
        /// <see cref="ReportItem"/>
        /// instance.
        /// </summary>
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
        /// <param name="status">
        /// 
        /// <see cref="ReportItemStatus"/>
        /// report item status that determines validation result
        /// </param>
        public ReportItem(String checkName, String message, ReportItem.ReportItemStatus status)
            : this(checkName, message, null, status) {
        }

        /// <summary>
        /// Create
        /// <see cref="ReportItem"/>
        /// instance.
        /// </summary>
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
        /// <param name="status">
        /// 
        /// <see cref="ReportItemStatus"/>
        /// report item status that determines validation result
        /// </param>
        public ReportItem(String checkName, String message, Exception cause, ReportItem.ReportItemStatus status) {
            this.checkName = checkName;
            this.message = message;
            this.cause = cause;
            this.status = status;
        }

        /// <summary>Get the check name related to this report item.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// check name related to this report item.
        /// </returns>
        public virtual String GetCheckName() {
            return checkName;
        }

        /// <summary>Get the message related to this report item.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// message related to this report item.
        /// </returns>
        public virtual String GetMessage() {
            return message;
        }

        /// <summary>Get the exception, which caused this report item.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Exception"/>
        /// , which cause this report item.
        /// </returns>
        public virtual Exception GetExceptionCause() {
            return cause;
        }

        /// <summary>Get report item status that determines validation result this report item corresponds to.</summary>
        /// <returns>
        /// 
        /// <see cref="ReportItemStatus"/>
        /// report item status that determines validation result.
        /// </returns>
        public virtual ReportItem.ReportItemStatus GetStatus() {
            return status;
        }

        /// <summary>Set report item status that determines validation result this report item corresponds to.</summary>
        /// <param name="status">
        /// 
        /// <see cref="ReportItemStatus"/>
        /// report item status that determines validation result
        /// </param>
        /// <returns>
        /// this
        /// <see cref="ReportItem"/>
        /// instance.
        /// </returns>
        public virtual iText.Signatures.Validation.Report.ReportItem SetStatus(ReportItem.ReportItemStatus status) {
            this.status = status;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override String ToString() {
            return "\nReportItem{" + "checkName='" + checkName + '\'' + ", message='" + message + '\'' + ", cause=" + 
                cause + ", status=" + status + '}';
        }

        /// <summary>Enum representing possible report item statuses that determine validation result.</summary>
        public enum ReportItemStatus {
            /// <summary>Report item status for info messages.</summary>
            INFO,
            /// <summary>Report item status that leads to invalid validation result.</summary>
            INVALID,
            /// <summary>Report item status that leads to indeterminate validation result.</summary>
            INDETERMINATE
        }
    }
}
