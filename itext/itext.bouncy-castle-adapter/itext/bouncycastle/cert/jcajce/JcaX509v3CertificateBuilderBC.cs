/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using Org.BouncyCastle.Cert.Jcajce;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>.
    /// </summary>
    public class JcaX509v3CertificateBuilderBC : IJcaX509v3CertificateBuilder {
        private readonly JcaX509v3CertificateBuilder certificateBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>.
        /// </summary>
        /// <param name="certificateBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaX509v3CertificateBuilderBC(JcaX509v3CertificateBuilder certificateBuilder) {
            this.certificateBuilder = certificateBuilder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>.
        /// </summary>
        /// <param name="signingCert">
        /// X509Certificate to create
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>
        /// </param>
        /// <param name="certSerialNumber">
        /// BigInteger to create
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>
        /// </param>
        /// <param name="startDate">
        /// start date to create
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>
        /// </param>
        /// <param name="endDate">
        /// end date to create
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>
        /// </param>
        /// <param name="subjectDnName">
        /// X500Name wrapper to create
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>
        /// </param>
        /// <param name="publicKey">
        /// PublicKey to create
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>
        /// </param>
        public JcaX509v3CertificateBuilderBC(X509Certificate signingCert, BigInteger certSerialNumber, DateTime startDate
            , DateTime endDate, IX500Name subjectDnName, AsymmetricKeyParameter publicKey)
            : this(new JcaX509v3CertificateBuilder(signingCert, certSerialNumber, startDate, endDate, ((X500NameBC)subjectDnName
                ).GetX500Name(), publicKey)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509v3CertificateBuilder"/>.
        /// </returns>
        public virtual JcaX509v3CertificateBuilder GetCertificateBuilder() {
            return certificateBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509CertificateHolder Build(IContentSigner contentSigner) {
            return new X509CertificateHolderBC(certificateBuilder.Build(((ContentSignerBC)contentSigner).GetContentSigner
                ()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJcaX509v3CertificateBuilder AddExtension(IASN1ObjectIdentifier extensionOID, bool critical
            , IASN1Encodable extensionValue) {
            try {
                certificateBuilder.AddExtension(((ASN1ObjectIdentifierBC)extensionOID).GetASN1ObjectIdentifier(), critical
                    , ((ASN1EncodableBC)extensionValue).GetEncodable());
                return this;
            }
            catch (CertificateEncodingException e) {
                throw new CertIOExceptionBC(e);
            }
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
            iText.Bouncycastle.Cert.Jcajce.JcaX509v3CertificateBuilderBC that = (iText.Bouncycastle.Cert.Jcajce.JcaX509v3CertificateBuilderBC
                )o;
            return Object.Equals(certificateBuilder, that.certificateBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateBuilder.ToString();
        }
    }
}
