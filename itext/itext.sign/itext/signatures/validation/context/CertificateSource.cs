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
namespace iText.Signatures.Validation.Context {
    /// <summary>This enum lists all possible contexts related to the certificate origin in which a validation may take place
    ///     </summary>
    public enum CertificateSource {
        /// <summary>The context while validating a CRL issuer certificate.</summary>
        CRL_ISSUER,
        /// <summary>The context while validating a OCSP issuer certificate that is neither trusted nor CA.</summary>
        OCSP_ISSUER,
        /// <summary>The context while validating a certificate issuer certificate.</summary>
        CERT_ISSUER,
        /// <summary>The context while validating a signer certificate.</summary>
        SIGNER_CERT,
        /// <summary>A certificate that is on a trusted list.</summary>
        TRUSTED,
        /// <summary>The context while validating a timestamp issuer certificate.</summary>
        TIMESTAMP
    }
}
