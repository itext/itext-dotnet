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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Bouncycastlefips.Security;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class RSASSAPSSTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(
            NUnit.Framework.TestContext.CurrentContext.TestDirectory) 
                                                       + "/resources/itext/signatures/sign/RSASSAPSSTest/";

        private static readonly String SOURCE_FILE = SOURCE_FOLDER + "helloWorldDoc.pdf";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/RSASSAPSSTest/";

        private const String SIGNATURE_FIELD = "Signature";

        private static readonly char[] SAMPLE_KEY_PASSPHRASE = "pdfpdfpdfsecretsecret".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaPssTest()
        {
            String digestName = "SHA256";
            String outFileName = "simplePssSignature.pdf";
            String cmpFileName = "cmp_simplePssSignature.pdf";
            if ("BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName()))
            {
                // Signer RSASSA-PSS not recognised in BC mode
                NUnit.Framework.Assert.Catch(typeof(GeneralSecurityExceptionBCFips), () =>
                {
                    DoRoundTrip(digestName, "RSASSA-PSS", outFileName, 
                        RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName));
                });
            }
            else
            {
                DoRoundTrip(digestName, "RSASSA-PSS", outFileName, 
                    RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName));
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(System.IO.Path.Combine(
                    DESTINATION_FOLDER
                    , outFileName).ToString(), System.IO.Path.Combine(SOURCE_FOLDER, cmpFileName).ToString()));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaPssAlternativeNomenclatureTest()
        {
            String digestName = "SHA256";
            String outFileName = "simplePssAlternativeNomenclatureSignature.pdf";
            String cmpFileName = "cmp_simplePssSignature.pdf";

            if ("BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName()))
            {
                // Signer RSASSA-PSS not recognised in BC mode
                NUnit.Framework.Assert.Catch(typeof(GeneralSecurityExceptionBCFips), () =>
                {
                    DoRoundTrip(digestName,
                        //we should accept the "<digest>withRSA/PSS" convention as well
                        "RSA/PSS", outFileName, RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName));
                });
            }
            else
            {
                DoRoundTrip(digestName,
                    //we should accept the "<digest>withRSA/PSS" convention as well
                    "RSA/PSS", outFileName, RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName));
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(System.IO.Path.Combine(
                    DESTINATION_FOLDER
                    , outFileName).ToString(), System.IO.Path.Combine(SOURCE_FOLDER, cmpFileName).ToString()));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaSha384PssTest() {
            String digestName = "SHA384";
            String outFileName = "simplePssSignatureSha384.pdf";
            if ("BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName()))
            {
                // Signer RSASSA-PSS not recognised in BC mode
                NUnit.Framework.Assert.Catch(typeof(GeneralSecurityExceptionBCFips), () =>
                {
                    DoRoundTrip(digestName, "RSASSA-PSS", outFileName,
                        RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName));
                });
            }
            else
            {
                DoRoundTrip(digestName, "RSASSA-PSS", outFileName,
                    RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaCustomSaltLengthTest() {
            String digestName = "SHA256";
            String outFileName = "customSaltLength.pdf";
            String cmpFileName = "cmp_simplePssSignature.pdf";
            
            if ("BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName())) {
                // Signer RSASSA-PSS not recognised in BC mode
                NUnit.Framework.Assert.Catch(typeof(GeneralSecurityExceptionBCFips), () =>
                {
                    DoRoundTrip(digestName, "RSASSA-PSS", outFileName, new RSASSAPSSMechanismParams(
                        BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier
                            (DigestAlgorithms.GetAllowedDigest(digestName)), 40, 1));
                });
            }
            else
            {
                DoRoundTrip(digestName, "RSASSA-PSS", outFileName, 
                    new RSASSAPSSMechanismParams(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier
                        (DigestAlgorithms.GetAllowedDigest(digestName)), 40, 1));
                String cmpOut = SignaturesCompareTool.CompareSignatures(System.IO.Path.Combine(DESTINATION_FOLDER, 
                    outFileName).ToString(), System.IO.Path.Combine(SOURCE_FOLDER, cmpFileName).ToString());
                NUnit.Framework.Assert.IsTrue(cmpOut.Contains("out: Integer(40)"));
                NUnit.Framework.Assert.IsTrue(cmpOut.Contains("cmp: Integer(32)"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void RejectMgfDigestDiscrepancy() {
            // mgf digest function param is not the same as signature digest function
            String inFileName = "mgfDiscrepancy.pdf";
            using (PdfReader r = new PdfReader(System.IO.Path.Combine(SOURCE_FOLDER, inFileName).ToString())) {
                using (PdfDocument pdfDoc = new PdfDocument(r)) {
                    SignatureUtil u = new SignatureUtil(pdfDoc);
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => u.ReadSignatureData(SIGNATURE_FIELD));
                }
            }
        }

        private void DoRoundTrip(String digestAlgo, String signatureAlgo, String outFileName, IApplicableSignatureParams
             @params) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, outFileName).ToString();
            DoSign(digestAlgo, signatureAlgo, outFile, @params);
            DoVerify(outFile);
        }

        private void DoSign(String digestAlgo, String signatureAlgo, String outFile, IApplicableSignatureParams @params
            ) {
            // write to a file for easier inspection when debugging
            using (FileStream fos = new FileStream(outFile, FileMode.Create)) {
                IX509Certificate root = ReadCertificate(System.IO.Path.Combine(SOURCE_FOLDER, "ca.crt"));
                IX509Certificate signerCert = ReadCertificate(System.IO.Path.Combine(SOURCE_FOLDER, "rsa.crt"));
                IX509Certificate[] signChain = new IX509Certificate[] { signerCert, root };
                IPrivateKey signPrivateKey = ReadPrivateKey(System.IO.Path.Combine(SOURCE_FOLDER, "rsa.key.pem"));
                IExternalSignature pks = new PrivateKeySignature(signPrivateKey, digestAlgo, signatureAlgo, @params);
                //IExternalSignature pks = new PrivateKeySignature(signPrivateKey, digestAlgo, @params);
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), fos, new StampingProperties());
                signer.SetFieldName(SIGNATURE_FIELD);
                signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
            }
        }

        private void DoVerify(String fileName) {
            using (PdfReader r = new PdfReader(fileName)) {
                using (PdfDocument pdfDoc = new PdfDocument(r)) {
                    SignatureUtil u = new SignatureUtil(pdfDoc);
                    PdfPKCS7 data = u.ReadSignatureData(SIGNATURE_FIELD);
                    NUnit.Framework.Assert.AreEqual(SecurityIDs.ID_RSASSA_PSS, data.GetSignatureMechanismOid());
                    NUnit.Framework.Assert.IsTrue(data.VerifySignatureIntegrityAndAuthenticity());
                }
            }
        }

        private IPrivateKey ReadPrivateKey(string path) {
            try {
                return PemFileHelper.ReadFirstKey(path, SAMPLE_KEY_PASSPHRASE);
            }
            catch (Exception e) {
                throw new Exception("Reading key failed", e);
            }
        }

        private IX509Certificate ReadCertificate(string  path) {
            byte[] content = System.IO.File.ReadAllBytes(path);
            return BOUNCY_CASTLE_FACTORY.CreateX509Certificate(content);
        }
    }
}
