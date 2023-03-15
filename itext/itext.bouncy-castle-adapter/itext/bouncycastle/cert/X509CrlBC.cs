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
using System.Security.Cryptography.X509Certificates;
using iText.Bouncycastle.Asn1.X500;
using iText.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Cert {
    public class X509CrlBC : IX509Crl {
        /// <summary>
        /// Wrapper class for
        /// <see cref="Org.BouncyCastle.Cert.X509Crl"/>.
        /// </summary>
        private readonly X509Crl x509Crl;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Cert.X509Crl"/>.
        /// </summary>
        /// <param name="x509Crl">
        /// <see cref="Org.BouncyCastle.Cert.X509Crl"/>
        /// to be wrapped
        /// </param>
        public X509CrlBC(X509Crl x509Crl) {
            this.x509Crl = x509Crl;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Cert.X509Crl"/>.
        /// </returns>
        public virtual X509Crl GetX509Crl() {
            return x509Crl;
        }

        /// <summary><inheritDoc/></summary>
        public bool IsRevoked(IX509Certificate cert) {
            return x509Crl.IsRevoked(((X509CertificateBC)cert).GetCertificate());
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name GetIssuerDN() {
            return new X500NameBC(x509Crl.IssuerDN);
        }

        /// <summary><inheritDoc/></summary>
        public DateTime GetNextUpdate() {
            return x509Crl.NextUpdate.Value;
        }

        /// <summary><inheritDoc/></summary>
        public void Verify(IPublicKey publicKey) {
            x509Crl.Verify(((PublicKeyBC) publicKey).GetPublicKey());
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GetEncoded() {
            return x509Crl.GetEncoded();
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            X509CrlBC that = (X509CrlBC)o;
            return Object.Equals(x509Crl, that.x509Crl);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(x509Crl);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return x509Crl.ToString();
        }
    }
}
