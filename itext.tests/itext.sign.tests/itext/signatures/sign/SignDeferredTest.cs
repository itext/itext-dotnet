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
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iText.Forms;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    public class SignDeferredTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SignDeferredTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/SignDeferredTest/";

        private static readonly char[] password = "testpass".ToCharArray();

        private const String HASH_ALGORITHM = DigestAlgorithms.SHA256;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareDocForSignDeferredTest() {
            String input = sourceFolder + "helloWorldDoc.pdf";
            String output = destinationFolder + "newTemplateForSignDeferred.pdf";
            String sigFieldName = "DeferredSignature1";
            PdfName filter = PdfName.Adobe_PPKLite;
            PdfName subFilter = PdfName.Adbe_pkcs7_detached;
            int estimatedSize = 8192;
            PdfReader reader = new PdfReader(input);
            PdfSigner signer = new PdfSigner(reader, new FileStream(output, FileMode.Create), new StampingProperties()
                );
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetLayer2Text("Signature field which signing is deferred.").SetPageRect(new Rectangle(36, 600, 
                200, 100)).SetPageNumber(1);
            signer.SetFieldName(sigFieldName);
            IExternalSignatureContainer external = new ExternalBlankSignatureContainer(filter, subFilter);
            signer.SignExternalContainer(external, estimatedSize);
            // validate result
            ValidateTemplateForSignedDeferredResult(output, sigFieldName, filter, subFilter, estimatedSize);
        }

        [NUnit.Framework.Test]
        public virtual void DeferredHashCalcAndSignTest01() {
            String srcFileName = sourceFolder + "templateForSignCMSDeferred.pdf";
            String outFileName = destinationFolder + "deferredHashCalcAndSignTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_deferredHashCalcAndSignTest01.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.p12";
            X509Certificate[] signChain = Pkcs12FileHelper.ReadFirstChain(signCertFileName, password);
            ICipherParameters signPrivateKey = Pkcs12FileHelper.ReadFirstKey(signCertFileName, password, password);
            IExternalSignatureContainer extSigContainer = new SignDeferredTest.CmsDeferredSigner(signPrivateKey, signChain
                );
            String sigFieldName = "DeferredSignature1";
            PdfDocument docToSign = new PdfDocument(new PdfReader(srcFileName));
            FileStream outStream = new FileStream(outFileName, FileMode.Create);
            PdfSigner.SignDeferred(docToSign, sigFieldName, outStream, extSigContainer);
            docToSign.Close();
            outStream.Dispose();
            // validate result
            PadesSigTest.BasicCheckSignedDoc(outFileName, sigFieldName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outFileName, cmpFileName, destinationFolder
                , null));
        }

        [NUnit.Framework.Test]
        public virtual void CalcHashOnDocCreationThenDeferredSignTest01() {
            String input = sourceFolder + "helloWorldDoc.pdf";
            String outFileName = destinationFolder + "calcHashOnDocCreationThenDeferredSignTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_calcHashOnDocCreationThenDeferredSignTest01.pdf";
            // pre-calculate hash on creating pre-signed PDF
            String sigFieldName = "DeferredSignature1";
            PdfName filter = PdfName.Adobe_PPKLite;
            PdfName subFilter = PdfName.Adbe_pkcs7_detached;
            int estimatedSize = 8192;
            PdfReader reader = new PdfReader(input);
            MemoryStream baos = new MemoryStream();
            PdfSigner signer = new PdfSigner(reader, baos, new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.CERTIFIED_NO_CHANGES_ALLOWED);
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetLayer2Text("Signature field which signing is deferred.").SetPageRect(new Rectangle(36, 600, 
                200, 100)).SetPageNumber(1);
            signer.SetFieldName(sigFieldName);
            SignDeferredTest.DigestCalcBlankSigner external = new SignDeferredTest.DigestCalcBlankSigner(filter, subFilter
                );
            signer.SignExternalContainer(external, estimatedSize);
            byte[] docBytesHash = external.GetDocBytesHash();
            byte[] preSignedBytes = baos.ToArray();
            // sign the hash
            String signCertFileName = certsSrc + "signCertRsa01.p12";
            X509Certificate[] signChain = Pkcs12FileHelper.ReadFirstChain(signCertFileName, password);
            ICipherParameters signPrivateKey = Pkcs12FileHelper.ReadFirstKey(signCertFileName, password, password);
            byte[] cmsSignature = SignDocBytesHash(docBytesHash, signPrivateKey, signChain);
            // fill the signature to the presigned document
            SignDeferredTest.ReadySignatureSigner extSigContainer = new SignDeferredTest.ReadySignatureSigner(cmsSignature
                );
            PdfDocument docToSign = new PdfDocument(new PdfReader(new MemoryStream(preSignedBytes)));
            FileStream outStream = new FileStream(outFileName, FileMode.Create);
            PdfSigner.SignDeferred(docToSign, sigFieldName, outStream, extSigContainer);
            docToSign.Close();
            outStream.Dispose();
            // validate result
            PadesSigTest.BasicCheckSignedDoc(outFileName, sigFieldName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outFileName, cmpFileName, destinationFolder
                , null));
        }

        internal static void ValidateTemplateForSignedDeferredResult(String output, String sigFieldName, PdfName filter
            , PdfName subFilter, int estimatedSize) {
            PdfDocument outDocument = new PdfDocument(new PdfReader(output));
            PdfObject outSigDictObj = PdfAcroForm.GetAcroForm(outDocument, false).GetField(sigFieldName).GetValue();
            NUnit.Framework.Assert.IsTrue(outSigDictObj.IsDictionary());
            PdfDictionary outSigDict = (PdfDictionary)outSigDictObj;
            PdfArray byteRange = outSigDict.GetAsArray(PdfName.ByteRange);
            NUnit.Framework.Assert.IsNotNull(byteRange);
            NUnit.Framework.Assert.IsTrue(byteRange.Size() == 4);
            NUnit.Framework.Assert.AreEqual(filter, outSigDict.GetAsName(PdfName.Filter));
            NUnit.Framework.Assert.AreEqual(subFilter, outSigDict.GetAsName(PdfName.SubFilter));
            PdfString outSigContents = outSigDict.GetAsString(PdfName.Contents);
            NUnit.Framework.Assert.IsTrue(outSigContents.IsHexWriting());
            NUnit.Framework.Assert.AreEqual(new byte[estimatedSize], outSigContents.GetValueBytes());
        }

        internal static byte[] CalcDocBytesHash(Stream docBytes) {
            byte[] docBytesHash = null;
            try {
                docBytesHash = DigestAlgorithms.Digest(docBytes, SignTestPortUtil.GetMessageDigest(HASH_ALGORITHM));
            }
            catch (Exception) {
            }
            // dummy catch clause
            return docBytesHash;
        }

        internal static byte[] SignDocBytesHash(byte[] docBytesHash, ICipherParameters pk, X509Certificate[] chain
            ) {
            if (pk == null || chain == null) {
                return null;
            }
            byte[] signatureContent = null;
            try {
                PdfPKCS7 pkcs7 = new PdfPKCS7(null, chain, HASH_ALGORITHM, false);
                byte[] attributes = pkcs7.GetAuthenticatedAttributeBytes(docBytesHash, null, null, PdfSigner.CryptoStandard
                    .CMS);
                PrivateKeySignature signature = new PrivateKeySignature(pk, HASH_ALGORITHM);
                byte[] attrSign = signature.Sign(attributes);
                pkcs7.SetExternalDigest(attrSign, null, signature.GetEncryptionAlgorithm());
                signatureContent = pkcs7.GetEncodedPKCS7(docBytesHash, null, null, null, PdfSigner.CryptoStandard.CMS);
            }
            catch (GeneralSecurityException) {
            }
            // dummy catch clause
            return signatureContent;
        }

        internal class CmsDeferredSigner : IExternalSignatureContainer {
            private ICipherParameters pk;

            private X509Certificate[] chain;

            public CmsDeferredSigner(ICipherParameters pk, X509Certificate[] chain) {
                this.pk = pk;
                this.chain = chain;
            }

            public virtual byte[] Sign(Stream docBytes) {
                byte[] docBytesHash = CalcDocBytesHash(docBytes);
                byte[] signatureContent = null;
                if (docBytesHash != null) {
                    // sign the hash and create PKCS7 CMS signature
                    signatureContent = SignDocBytesHash(docBytesHash, pk, chain);
                }
                if (signatureContent == null) {
                    signatureContent = new byte[0];
                }
                return signatureContent;
            }

            public virtual void ModifySigningDictionary(PdfDictionary signDic) {
            }
        }

        internal class DigestCalcBlankSigner : IExternalSignatureContainer {
            private readonly PdfName filter;

            private readonly PdfName subFilter;

            private byte[] docBytesHash;

            public DigestCalcBlankSigner(PdfName filter, PdfName subFilter) {
                this.filter = filter;
                this.subFilter = subFilter;
            }

            public virtual byte[] GetDocBytesHash() {
                return docBytesHash;
            }

            public virtual byte[] Sign(Stream docBytes) {
                docBytesHash = CalcDocBytesHash(docBytes);
                return new byte[0];
            }

            public virtual void ModifySigningDictionary(PdfDictionary signDic) {
                signDic.Put(PdfName.Filter, filter);
                signDic.Put(PdfName.SubFilter, subFilter);
            }
        }

        internal class ReadySignatureSigner : IExternalSignatureContainer {
            private byte[] cmsSignatureContents;

            public ReadySignatureSigner(byte[] cmsSignatureContents) {
                this.cmsSignatureContents = cmsSignatureContents;
            }

            public virtual byte[] Sign(Stream docBytes) {
                return cmsSignatureContents;
            }

            public virtual void ModifySigningDictionary(PdfDictionary signDic) {
            }
        }
    }
}
