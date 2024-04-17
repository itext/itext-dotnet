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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.V1.Report {
    /// <summary>Report item to be used for single certificate related failure or log message.</summary>
    public class CertificateReportItem : ReportItem {
        private readonly IX509Certificate certificate;

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
        /// <param name="status">
        /// 
        /// <see cref="ReportItemStatus"/>
        /// report item status that determines validation result
        /// </param>
        public CertificateReportItem(IX509Certificate certificate, String checkName, String message, ReportItem.ReportItemStatus
             status)
            : this(certificate, checkName, message, null, status) {
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
        /// <param name="status">
        /// 
        /// <see cref="ReportItemStatus"/>
        /// report item status that determines validation result
        /// </param>
        public CertificateReportItem(IX509Certificate certificate, String checkName, String message, Exception cause
            , ReportItem.ReportItemStatus status)
            : base(checkName, message, cause, status) {
            this.certificate = certificate;
        }

        /// <summary>Get the certificate related to this report item.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// related to this report item.
        /// </returns>
        public virtual IX509Certificate GetCertificate() {
            return certificate;
        }

        public override String ToString() {
            return "\nCertificateReportItem{" + "baseclass=" + base.ToString() + "\ncertificate=" + certificate.GetSubjectDN
                () + '}';
        }
    }
}
