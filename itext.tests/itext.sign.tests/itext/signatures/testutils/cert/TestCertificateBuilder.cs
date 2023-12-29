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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Signatures.Testutils.Cert {
    public class TestCertificateBuilder {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        private const String signatureAlgorithm = "SHA256WithRSA";

        private IPublicKey publicKey;

        private IX509Certificate signingCert;

        private IPrivateKey signingKey;

        private String subjectDN;

        private DateTime startDate;

        private DateTime endDate;

        public TestCertificateBuilder(IPublicKey publicKey, IX509Certificate signingCert, IPrivateKey
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
        
        public virtual IX509Certificate BuildAuthorizedOCSPResponderCert() {        
            IX500Name subjectDnName = FACTORY.CreateX500Name(subjectDN);
            IBigInteger certSerialNumber = FACTORY.CreateBigInteger(Convert.ToString(SystemUtil.GetTimeBasedSeed())); // Using the current timestamp as the certificate serial number
            IContentSigner contentSigner = FACTORY.CreateContentSigner(signatureAlgorithm, signingKey);
            IX509V3CertificateGenerator certBuilder = FACTORY.CreateJcaX509v3CertificateBuilder(signingCert,
                certSerialNumber, startDate, endDate, subjectDnName, publicKey);
            
            bool ca = true;
            AddExtension(FACTORY.CreateExtensions().GetBasicConstraints(), true, FACTORY.CreateBasicConstraints(ca), 
                certBuilder);
            
            AddExtension(FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspNoCheck(), false, FACTORY.CreateDERNull(), 
                certBuilder);
            
            AddExtension(FACTORY.CreateExtensions().GetKeyUsage(), false, FACTORY.CreateKeyUsage(
                    FACTORY.CreateKeyUsage().GetDigitalSignature() | FACTORY.CreateKeyUsage().GetNonRepudiation()),
                certBuilder);
            
            AddExtension(FACTORY.CreateExtensions().GetExtendedKeyUsage(), false, FACTORY.CreateExtendedKeyUsage
                    (FACTORY.CreateKeyPurposeId().GetIdKpOCSPSigning()), 
                certBuilder);
            
            ISubjectPublicKeyInfo issuerPublicKeyInfo = FACTORY.CreateSubjectPublicKeyInfo(signingCert.GetPublicKey());
            IAuthorityKeyIdentifier authKeyIdentifier = FACTORY.CreateAuthorityKeyIdentifier(issuerPublicKeyInfo);
            AddExtension(FACTORY.CreateExtensions().GetAuthorityKeyIdentifier(), false, authKeyIdentifier, certBuilder);

            ISubjectPublicKeyInfo subjectPublicKeyInfo = FACTORY.CreateSubjectPublicKeyInfo(publicKey);
            ISubjectKeyIdentifier subjectKeyIdentifier = FACTORY.CreateSubjectKeyIdentifier(subjectPublicKeyInfo);
            AddExtension(FACTORY.CreateExtensions().GetSubjectKeyIdentifier(), false, subjectKeyIdentifier, certBuilder);
            // -------------------------------------
            return certBuilder.Build(contentSigner);
        }

        private static void AddExtension(IDerObjectIdentifier extensionOID, bool critical, IAsn1Encodable extensionValue, 
            IX509V3CertificateGenerator certBuilder) {
            certBuilder.AddExtension(extensionOID, critical, extensionValue);
        }
    }
}
