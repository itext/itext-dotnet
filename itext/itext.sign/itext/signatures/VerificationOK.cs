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
