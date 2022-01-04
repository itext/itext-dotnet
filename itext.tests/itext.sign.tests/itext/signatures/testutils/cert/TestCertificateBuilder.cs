/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections;
using iText.Commons.Utils;
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
