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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.Math;
using iText.Bouncycastle.Security;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.X509.X509Certificate"/>.
    /// </summary>
    public class X509CertificateBC : IX509Certificate {
        private readonly X509Certificate certificate;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.X509.X509Certificate"/>.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="Org.BouncyCastle.X509.X509Certificate"/> to be wrapped
        /// </param>
        public X509CertificateBC(X509Certificate certificate) {
            this.certificate = certificate;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.X509.X509Certificate"/>.
        /// </returns>
        public virtual X509Certificate GetCertificate() {
            return certificate;
        }
        
        /// <summary><inheritDoc/></summary>
        public IX500Name GetIssuerDN() {
            return new X509NameBC(certificate.IssuerDN);
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger GetSerialNumber() {
            return new BigIntegerBC(certificate.SerialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public IPublicKey GetPublicKey() {
            return new PublicKeyBC(certificate.GetPublicKey());
        }

        /// <summary><inheritDoc/></summary>
        public string GetSigAlgOID() => this.certificate.SigAlgOid;

        /// <summary><inheritDoc/></summary>
        public byte[] GetSigAlgParams() => this.certificate.GetSigAlgParams();

        /// <summary><inheritDoc/></summary>
        public byte[] GetEncoded() {
            return certificate.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GetTbsCertificate() {
            return certificate.GetTbsCertificate();
        }

        /// <summary><inheritDoc/></summary>
        public IAsn1OctetString GetExtensionValue(string oid) {
            return new Asn1OctetStringBC(certificate.GetExtensionValue(new DerObjectIdentifier(oid)));
        }

        /// <summary><inheritDoc/></summary>
        public void Verify(IPublicKey issuerPublicKey) {
            try {
                certificate.Verify(((PublicKeyBC)issuerPublicKey).GetPublicKey());
            }
            catch (GeneralSecurityException e) {
                throw new GeneralSecurityExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public ISet<string> GetCriticalExtensionOids() {
            if (certificate.GetCriticalExtensionOids() == null) {
                return null;
            }
            ISet<string> set = new HashSet<string>();
            foreach (string oid in certificate.GetCriticalExtensionOids()) {
                set.Add(oid);
            }
            return set;
        }

        /// <summary><inheritDoc/></summary>
        public void CheckValidity(DateTime time) {
            try {
                certificate.CheckValidity(time);
            } catch (CertificateExpiredException e) {
                throw new CertificateExpiredExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name GetSubjectDN() {
            return new X509NameBC(certificate.SubjectDN);
        }

        /// <summary><inheritDoc/></summary>
        public string GetEndDateTime() {
            return certificate.CertificateStructure.EndDate.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public DateTime GetNotBefore() {
            return certificate.NotBefore;
        }

        /// <summary><inheritDoc/></summary>
        public DateTime GetNotAfter() {
            return certificate.NotAfter;
        }

        /// <summary><inheritDoc/></summary>
        public IList GetExtendedKeyUsage() {
            return certificate.GetExtendedKeyUsage()?.Select(ku=> ku.Id).ToList();
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            X509CertificateBC that = (X509CertificateBC)o;
            return Object.Equals(certificate, that.certificate);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificate);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificate.ToString();
        }
    }
}
