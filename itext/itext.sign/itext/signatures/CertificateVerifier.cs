/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures {
    /// <summary>
    /// Superclass for a series of certificate verifiers that will typically
    /// be used in a chain.
    /// </summary>
    /// <remarks>
    /// Superclass for a series of certificate verifiers that will typically
    /// be used in a chain. It wraps another <c>CertificateVerifier</c>
    /// that is the next element in the chain of which the <c>verify()</c>
    /// method will be called.
    /// </remarks>
    public class CertificateVerifier {
        /// <summary>The previous CertificateVerifier in the chain of verifiers.</summary>
        protected internal iText.Signatures.CertificateVerifier verifier;

        /// <summary>Indicates if going online to verify a certificate is allowed.</summary>
        protected internal bool onlineCheckingAllowed = true;

        /// <summary>Creates the final CertificateVerifier in a chain of verifiers.</summary>
        /// <param name="verifier">the previous verifier in the chain</param>
        public CertificateVerifier(iText.Signatures.CertificateVerifier verifier) {
            this.verifier = verifier;
        }

        /// <summary>Decide whether or not online checking is allowed.</summary>
        /// <param name="onlineCheckingAllowed">a boolean indicating whether the certificate can be verified using online verification results.
        ///     </param>
        public virtual void SetOnlineCheckingAllowed(bool onlineCheckingAllowed) {
            this.onlineCheckingAllowed = onlineCheckingAllowed;
        }

        /// <summary>
        /// Checks the validity of the certificate, and calls the next
        /// verifier in the chain, if any.
        /// </summary>
        /// <param name="signCert">the certificate that needs to be checked</param>
        /// <param name="issuerCert">its issuer</param>
        /// <param name="signDate">the date the certificate needs to be valid</param>
        /// <returns>a list of <c>VerificationOK</c> objects. The list will be empty if the certificate couldn't be verified.
        ///     </returns>
        public virtual IList<VerificationOK> Verify(IX509Certificate signCert, IX509Certificate issuerCert, DateTime
             signDate) {
            // Check if the certificate is valid on the signDate
            if (signDate != null) {
                signCert.CheckValidity(signDate);
            }
            // Check if the signature is valid
            if (issuerCert != null) {
                signCert.Verify(issuerCert.GetPublicKey());
            }
            else {
                // Also in case, the certificate is self-signed
                signCert.Verify(signCert.GetPublicKey());
            }
            IList<VerificationOK> result = new List<VerificationOK>();
            if (verifier != null) {
                result.AddAll(verifier.Verify(signCert, issuerCert, signDate));
            }
            return result;
        }
    }
}
