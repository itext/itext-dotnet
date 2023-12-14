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
using System.Collections;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security.Certificates;

namespace iText.Signatures.Testutils.Cert {
    public class TestCertificateBuilder {
        private const String signatureAlgorithm = "SHA256WithRSA";

        private AsymmetricKeyParameter publicKey;

        private X509Certificate signingCert;

        private ICipherParameters signingKey;

        private String subjectDN;

        private DateTime startDate;

        private DateTime endDate;

        public TestCertificateBuilder(AsymmetricKeyParameter publicKey, X509Certificate signingCert, ICipherParameters
             signingKey, String subjectDN) {
            // requires corresponding key pairs to be used in this class
            this.publicKey = publicKey;
            this.signingCert = signingCert;
            this.signingKey = signingKey;
            this.subjectDN = subjectDN;
            this.startDate = DateTimeUtil.GetCurrentUtcTime();
            this.endDate = startDate.AddDays(365 * 100);
        }

        public virtual void SetStartDate(DateTime startDate) {
            this.startDate = startDate;
        }

        public virtual void SetEndDate(DateTime endDate) {
            this.endDate = endDate;
        }

        // TODO generalize
        public virtual X509Certificate BuildAuthorizedOCSPResponderCert() {        
            X509Name subjectDnName = new X509Name(subjectDN);
            BigInteger certSerialNumber = new BigInteger(Convert.ToString(SystemUtil.GetTimeBasedSeed())); // Using the current timestamp as the certificate serial number
            ISignatureFactory contentSigner = new Asn1SignatureFactory(signatureAlgorithm, (AsymmetricKeyParameter) signingKey);
            X509V3CertificateGenerator certBuilder = new X509V3CertificateGenerator();
            certBuilder.SetIssuerDN(signingCert.SubjectDN);
            certBuilder.SetSerialNumber(certSerialNumber);
            certBuilder.SetNotBefore(startDate);
            certBuilder.SetNotAfter(endDate);
            certBuilder.SetSubjectDN(subjectDnName);
            certBuilder.SetPublicKey(publicKey);
            
            // TODO generalize extensions setting
            // Extensions --------------------------
            bool ca = true;
            AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(ca), 
                certBuilder);
            
            AddExtension(OcspObjectIdentifiers.PkixOcspNocheck, false, Org.BouncyCastle.Asn1.DerNull.Instance, 
                certBuilder);
            
            AddExtension(X509Extensions.KeyUsage, false, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.NonRepudiation),
                certBuilder);
            
            AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(KeyPurposeID.IdKPOcspSigning), 
                certBuilder);
            
            SubjectPublicKeyInfo issuerPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(signingCert.GetPublicKey());
            AuthorityKeyIdentifier authKeyIdentifier = new AuthorityKeyIdentifier(issuerPublicKeyInfo);
            AddExtension(X509Extensions.AuthorityKeyIdentifier, false, authKeyIdentifier, certBuilder);

            SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            SubjectKeyIdentifier subjectKeyIdentifier = new SubjectKeyIdentifier(subjectPublicKeyInfo);
            AddExtension(X509Extensions.SubjectKeyIdentifier, false, subjectKeyIdentifier, certBuilder);
            // -------------------------------------
            return certBuilder.Generate(contentSigner);
        }

        private static void AddExtension(DerObjectIdentifier extensionOID, bool critical, Asn1Encodable extensionValue, 
            X509V3CertificateGenerator certBuilder) {
            certBuilder.AddExtension(extensionOID, critical, extensionValue);
        }
    }
}
