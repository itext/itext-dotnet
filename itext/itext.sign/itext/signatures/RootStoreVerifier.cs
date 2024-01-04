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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;

namespace iText.Signatures {
    /// <summary>
    /// Verifies a certificate against a <c>KeyStore</c>
    /// containing trusted anchors.
    /// </summary>
    public class RootStoreVerifier : CertificateVerifier {
        /// <summary>A key store against which certificates can be verified.</summary>
        protected internal List<IX509Certificate> rootStore = null;

        /// <summary>Creates a RootStoreVerifier in a chain of verifiers.</summary>
        /// <param name="verifier">the next verifier in the chain</param>
        public RootStoreVerifier(CertificateVerifier verifier)
            : base(verifier) {
        }

        /// <summary>Sets the Key Store against which a certificate can be checked.</summary>
        /// <param name="keyStore">a root store</param>
        public virtual void SetRootStore(List<IX509Certificate> keyStore) {
            this.rootStore = keyStore;
        }

        /// <summary>Verifies a single certificate against a key store (if present).</summary>
        /// <param name="signCert">the certificate to verify</param>
        /// <param name="issuerCert">the issuer certificate</param>
        /// <param name="signDate">the date the certificate needs to be valid</param>
        /// <returns>
        /// a list of <c>VerificationOK</c> objects.
        /// The list will be empty if the certificate couldn't be verified.
        /// </returns>
        public override IList<VerificationOK> Verify(IX509Certificate signCert, IX509Certificate issuerCert, DateTime
             signDate) {
            // verify using the CertificateVerifier if root store is missing
            if (rootStore == null) {
                return base.Verify(signCert, issuerCert, signDate);
            }
            try {
                IList<VerificationOK> result = new List<VerificationOK>();
                // loop over the trusted anchors in the root store
                foreach (IX509Certificate anchor in SignUtils.GetCertificates(rootStore)) {
                    try {
                        signCert.Verify(anchor.GetPublicKey());
                        result.Add(new VerificationOK(signCert, this.GetType(), "Certificate verified against root store."));
                        result.AddAll(base.Verify(signCert, issuerCert, signDate));
                        return result;
                    }
                    catch (AbstractGeneralSecurityException) {
                    }
                }
                // do nothing and continue
                result.AddAll(base.Verify(signCert, issuerCert, signDate));
                return result;
            }
            catch (AbstractGeneralSecurityException) {
                return base.Verify(signCert, issuerCert, signDate);
            }
        }
    }
}
