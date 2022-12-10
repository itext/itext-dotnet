/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
