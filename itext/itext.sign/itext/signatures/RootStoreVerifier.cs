/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
