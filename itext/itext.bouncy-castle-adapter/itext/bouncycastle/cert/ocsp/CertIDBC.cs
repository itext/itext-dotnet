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
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Math;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
    /// </summary>
    public class CertIDBC : ICertID {
        private static readonly iText.Bouncycastle.Cert.Ocsp.CertIDBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.CertIDBC
            (null);

        private static readonly string HASH_SHA1 = CertificateID.HashSha1;

        private readonly CertID certificateID;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
        /// </summary>
        /// <param name="certificateID">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// to be wrapped
        /// </param>
        public CertIDBC(CertID certificateID) {
            this.certificateID = certificateID;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
        /// </summary>
        /// <param name="hashAlgorithm">
        /// hash algorithm
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// </param>
        /// <param name="issuerCert">
        /// X509Certificate wrapper
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// </param>
        /// <param name="serialNumber">
        /// serial number
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// </param>
        public CertIDBC(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) 
            : this(new AlgorithmIdentifier(new DerObjectIdentifier(hashAlgorithm), DerNull.Instance), issuerCert, serialNumber) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
        /// </summary>
        /// <param name="hashAlgId">
        /// hash algorithm indentifier
        /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>
        /// </param>
        /// <param name="issuerCert">
        /// X509Certificate wrapper
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// </param>
        /// <param name="serialNumber">
        /// serial number
        /// </param>
        public CertIDBC(AlgorithmIdentifier hashAlgId, IX509Certificate issuerCert, IBigInteger serialNumber) {
            X509Name issuerName = PrincipalUtilities.GetSubjectX509Principal(((X509CertificateBC)issuerCert).GetCertificate());
            string hashAlgorithm = hashAlgId.Algorithm.Id;
            byte[] issuerNameHash = DigestUtilities.CalculateDigest(hashAlgorithm, issuerName.GetEncoded());

            AsymmetricKeyParameter issuerKey = ((X509CertificateBC)issuerCert).GetCertificate().GetPublicKey();
            SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKey);
            byte[] issuerKeyHash = DigestUtilities.CalculateDigest(hashAlgorithm, info.PublicKeyData.GetBytes());

            this.certificateID = new CertID(hashAlgId, new DerOctetString(issuerNameHash),
                new DerOctetString(issuerKeyHash), new DerInteger(((BigIntegerBC)serialNumber).GetBigInteger()));
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="CertIDBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Cert.Ocsp.CertIDBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
        /// </returns>
        public virtual CertID GetCertID() {
            return certificateID;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetHashAlgOID() {
            return new DerObjectIdentifierBC(certificateID.HashAlgorithm.Algorithm.Id);
        }

        /// <summary><inheritDoc/></summary>
        public virtual string GetHashSha1() {
            return HASH_SHA1;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool MatchesIssuer(IX509Certificate issuerCert) {
            return new CertIDBC(certificateID.HashAlgorithm, issuerCert, new BigIntegerBC(
                certificateID.SerialNumber.Value)).GetCertID().Equals(certificateID);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBigInteger GetSerialNumber() {
            return new BigIntegerBC(certificateID.SerialNumber.Value);
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.CertIDBC that = (iText.Bouncycastle.Cert.Ocsp.CertIDBC)o;
            return Object.Equals(certificateID, that.certificateID);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateID);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateID.ToString();
        }
    }
}
