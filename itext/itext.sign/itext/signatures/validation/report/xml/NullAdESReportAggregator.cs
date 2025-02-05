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

namespace iText.Signatures.Validation.Report.Xml {
    /// <summary>Use this implementation when no xml report has to be created</summary>
    public class NullAdESReportAggregator : AdESReportAggregator {
        /// <summary>Creates a new instance of NullAdESReportAggregator.</summary>
        public NullAdESReportAggregator() {
        }

        // Declaring default constructor explicitly to avoid removing it unintentionally
        public virtual void StartSignatureValidation(byte[] signature, String name, DateTime signingDate) {
        }

        // No action required
        public virtual void ProofOfExistenceFound(byte[] timeStampSignature, bool document) {
        }

        // No action required
        public virtual void ReportSignatureValidationSuccess() {
        }

        // No action required
        public virtual void ReportSignatureValidationFailure(bool isInconclusive, String reason) {
        }

        // No action required
        //  code for future use commented out for code coverage
        //    @Override
        //    public void reportCertificateChainValidationSuccess(X509Certificate certificate) {
        //        // No action required
        //    }
        //
        //    @Override
        //    public void reportCertificateChainValidationFailure(X509Certificate certificate, boolean isInconclusive,
        //            String reason) {
        //        // No action required
        //    }
        //
        //    @Override
        //    public void reportCRLValidationSuccess(X509Certificate certificate, CRL crl) {
        //        // No action required
        //    }
        //
        //    @Override
        //    public void reportCRLValidationFailure(X509Certificate certificate, CRL crl, boolean isInconclusive,
        //            String reason) {
        //        // No action required
        //    }
        //
        //    @Override
        //    public void reportOCSPValidationSuccess(X509Certificate certificate, IBasicOCSPResp ocsp) {
        //        // No action required
        //    }
        //
        //    @Override
        //    public void reportOCSPValidationFailure(X509Certificate certificate, IBasicOCSPResp ocsp, boolean isInconclusive,
        //            String reason) {
        //        // No action required
        //    }
        public virtual PadesValidationReport GetReport() {
            throw new NotSupportedException("Use another implementation of AdESReportAggregator to create an actual report"
                );
        }
    }
}
