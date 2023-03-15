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
using System.Text;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures {
    /// <summary>
    /// Class that informs you that the verification of a Certificate
    /// succeeded using a specific CertificateVerifier and for a specific
    /// reason.
    /// </summary>
    public class VerificationOK {
        /// <summary>The certificate that was verified successfully.</summary>
        protected internal IX509Certificate certificate;

        /// <summary>The CertificateVerifier that was used for verifying.</summary>
        protected internal Type verifierClass;

        /// <summary>The reason why the certificate verified successfully.</summary>
        protected internal String message;

        /// <summary>Creates a VerificationOK object</summary>
        /// <param name="certificate">the certificate that was successfully verified</param>
        /// <param name="verifierClass">the class that was used for verification</param>
        /// <param name="message">the reason why the certificate could be verified</param>
        public VerificationOK(IX509Certificate certificate, Type verifierClass, String message) {
            this.certificate = certificate;
            this.verifierClass = verifierClass;
            this.message = message;
        }

        /// <summary>Return a single String explaining which certificate was verified, how and why.</summary>
        /// <seealso cref="System.Object.ToString()"/>
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            if (certificate != null) {
                sb.Append(certificate.GetSubjectDN().ToString());
                sb.Append(" verified with ");
            }
            sb.Append(verifierClass.FullName);
            sb.Append(": ");
            sb.Append(message);
            return sb.ToString();
        }
    }
}
