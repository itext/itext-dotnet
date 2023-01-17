/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;
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

        // TODO generalize
        public virtual IX509Certificate BuildAuthorizedOCSPResponderCert() {        
            IX500Name subjectDnName = FACTORY.CreateX500Name(subjectDN);
            IBigInteger certSerialNumber = FACTORY.CreateBigInteger(Convert.ToString(SystemUtil.GetTimeBasedSeed())); // Using the current timestamp as the certificate serial number
            IContentSigner contentSigner = FACTORY.CreateContentSigner(signatureAlgorithm, signingKey);
            IJcaX509v3CertificateBuilder certBuilder = FACTORY.CreateJcaX509v3CertificateBuilder(signingCert,
                certSerialNumber, startDate, endDate, subjectDnName, publicKey);

            // TODO generalize extensions setting
            // Extensions --------------------------
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

        private static void AddExtension(IASN1ObjectIdentifier extensionOID, bool critical, IASN1Encodable extensionValue, 
            IJcaX509v3CertificateBuilder certBuilder) {
            certBuilder.AddExtension(extensionOID, critical, extensionValue);
        }
    }
}
