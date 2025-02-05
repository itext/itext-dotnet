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
using System.Collections;
using System.Collections.Generic;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Bouncycastlefips.Crypto;
using iText.Bouncycastlefips.Math;
using iText.Bouncycastlefips.Security;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastlefips.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.X509Certificate"/>.
    /// </summary>
    public class X509CertificateBCFips : IX509Certificate {
        private readonly X509Certificate certificate;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Cert.X509Certificate"/>.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.X509Certificate"/> to be wrapped
        /// </param>
        public X509CertificateBCFips(X509Certificate certificate) {
            this.certificate = certificate;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Cert.X509Certificate"/>.
        /// </returns>
        public virtual X509Certificate GetCertificate() {
            return certificate;
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name GetIssuerDN() {
            return new X500NameBCFips(certificate.IssuerDN);
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger GetSerialNumber() {
            return new BigIntegerBCFips(certificate.SerialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public IPublicKey GetPublicKey() {
            return new PublicKeyBCFips(certificate.GetPublicKey());
        }

        /// <summary><inheritDoc/></summary>
        public string GetSigAlgOID() => certificate.SigAlgOid.Id;

        /// <summary><inheritDoc/></summary>
        public byte[] GetSigAlgParams() => certificate.GetSigAlgParams();

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
            byte[] extensionValue = certificate.GetExtensionValue(new DerObjectIdentifier(oid));
            if (extensionValue == null) {
                return new Asn1OctetStringBCFips(null);
            }
            return new Asn1OctetStringBCFips(new DerOctetString(extensionValue));
        }

        /// <summary><inheritDoc/></summary>
        public void Verify(IPublicKey issuerPublicKey) {
            PkixVerifierFactoryProvider factoryProvider = new PkixVerifierFactoryProvider(
                ((PublicKeyBCFips) issuerPublicKey).GetPublicKey());
            try {
                certificate.Verify(factoryProvider);
            }
            catch (GeneralSecurityException e) {
                throw new GeneralSecurityExceptionBCFips(e);
            }        
        }
        
        /// <summary><inheritDoc/></summary>
        public ISet<string> GetCriticalExtensionOids() {
            if (certificate.GetCriticalExtensionOids() == null) {
                return null;
            }
            ISet<string> set = new HashSet<string>();
            foreach (DerObjectIdentifier oid in certificate.GetCriticalExtensionOids()) {
                set.Add(oid.Id);
            }
            return set;
        }

        /// <summary><inheritDoc/></summary>
        public void CheckValidity(DateTime time) {
            try {
                certificate.CheckValidity(time);
            } catch (CertificateExpiredException e) {
                throw new CertificateExpiredExceptionBCFips(e);
            } catch (CertificateNotYetValidException e) {
                throw new CertificateNotYetValidExceptionBCFips(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name GetSubjectDN() {
            return new X500NameBCFips(certificate.SubjectDN);
        }

        /// <summary><inheritDoc/></summary>
        public string GetEndDateTime() {
            return certificate.ToAsn1Structure().EndDate.GetTime();
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
            IList list = new ArrayList();
            IList oids = certificate.GetExtendedKeyUsage();
            if (oids == null) {
                return null;
            }
            foreach (DerObjectIdentifier oid in certificate.GetExtendedKeyUsage()) {
                list.Add(oid.Id);
            }
            return list;
        }
        
        /// <summary><inheritDoc/></summary>
        public bool[] GetKeyUsage() {
            return certificate.GetKeyUsage();
        }
        
        /// <summary><inheritDoc/></summary>
        public int GetBasicConstraints() {
            return certificate.GetBasicConstraints();
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.X509CertificateBCFips that = (iText.Bouncycastlefips.Cert.X509CertificateBCFips
                )o;
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
