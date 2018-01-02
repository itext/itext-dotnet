/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    public class PadesSigTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesSigTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PadesSigTest/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="Org.BouncyCastle.Tsp.TSPException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        [NUnit.Framework.Test]
        public virtual void PadesRsaSigTest01() {
            SignApproval(certsSrc + "signCertRsa01.p12", destinationFolder + "padesRsaSigTest01.pdf");
            BasicCheckSignedDoc(destinationFolder + "padesRsaSigTest01.pdf", "Signature1");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="Org.BouncyCastle.Tsp.TSPException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1620: For some reason signatures created with the given cert (either by iText or acrobat) are considered invalid"
            )]
        public virtual void PadesDsaSigTest01() {
            SignApproval(certsSrc + "signCertDsa01.p12", destinationFolder + "padesDsaSigTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="Org.BouncyCastle.Tsp.TSPException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        [NUnit.Framework.Test]
        public virtual void PadesEccSigTest01() {
            SignApproval(certsSrc + "signCertEcc01.p12", destinationFolder + "padesEccSigTest01.pdf");
            // TODO ECDSA encryption algorithms verification is not supported
            BasicCheckSignedDoc(destinationFolder + "padesEccSigTest01.pdf", "Signature1");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void PadesEpesProfileTest01() {
            String notExistingSignaturePolicyOid = "2.16.724.631.3.1.124.2.29.9";
            DerObjectIdentifier asn1PolicyOid = DerObjectIdentifier.GetInstance(new DerObjectIdentifier(notExistingSignaturePolicyOid
                ));
            AlgorithmIdentifier hashAlg = new AlgorithmIdentifier(new DerObjectIdentifier(DigestAlgorithms.GetAllowedDigest
                ("SHA1")));
            // indicate that the policy hash value is not known; see ETSI TS 101 733 V2.2.1, 5.8.1
            byte[] zeroSigPolicyHash = new byte[] { 0 };
            DerOctetString hash = new DerOctetString(zeroSigPolicyHash);
            SignaturePolicyId signaturePolicyId = new SignaturePolicyId(asn1PolicyOid, new OtherHashAlgAndValue(hashAlg
                , hash));
            SignaturePolicyIdentifier sigPolicyIdentifier = new SignaturePolicyIdentifier(signaturePolicyId);
            SignApproval(certsSrc + "signCertRsa01.p12", destinationFolder + "padesEpesProfileTest01.pdf", sigPolicyIdentifier
                );
            BasicCheckSignedDoc(destinationFolder + "padesEpesProfileTest01.pdf", "Signature1");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        private void SignApproval(String signCertFileName, String outFileName) {
            SignApproval(signCertFileName, outFileName, null);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        private void SignApproval(String signCertFileName, String outFileName, SignaturePolicyIdentifier sigPolicyInfo
            ) {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            X509Certificate[] signChain = Pkcs12FileHelper.ReadFirstChain(signCertFileName, password);
            ICipherParameters signPrivateKey = Pkcs12FileHelper.ReadFirstKey(signCertFileName, password, password);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                false);
            signer.SetFieldName("Signature1");
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText7.");
            if (sigPolicyInfo == null) {
                signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            }
            else {
                signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES, sigPolicyInfo);
            }
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        internal static void BasicCheckSignedDoc(String filePath, String signatureName) {
            PdfDocument outDocument = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(outDocument);
            PdfPKCS7 pdfPKCS7 = sigUtil.VerifySignature(signatureName);
            NUnit.Framework.Assert.IsTrue(pdfPKCS7.Verify());
            outDocument.Close();
        }
    }
}
