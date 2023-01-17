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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Security;
using iText.Forms;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignDeferredTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SignDeferredTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/SignDeferredTest/";

        private static readonly char[] password = "testpassphrase".ToCharArray();

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
        public virtual void PrepareDocForSignDeferredNotEnoughSizeTest() {
            String input = sourceFolder + "helloWorldDoc.pdf";
            String sigFieldName = "DeferredSignature1";
            PdfName filter = PdfName.Adobe_PPKLite;
            PdfName subFilter = PdfName.Adbe_pkcs7_detached;
            PdfReader reader = new PdfReader(input);
            PdfSigner signer = new PdfSigner(reader, new MemoryStream(), new StampingProperties());
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetLayer2Text("Signature field which signing is deferred.").SetPageRect(new Rectangle(36, 600, 
                200, 100)).SetPageNumber(1);
            signer.SetFieldName(sigFieldName);
            IExternalSignatureContainer external = new ExternalBlankSignatureContainer(filter, subFilter);
            // This size is definitely not enough
            int estimatedSize = -1;
            Exception e = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => signer.SignExternalContainer
                (external, estimatedSize));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.NOT_ENOUGH_SPACE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareDocForSignDeferredLittleSpaceTest() {
            String input = sourceFolder + "helloWorldDoc.pdf";
            String sigFieldName = "DeferredSignature1";
            PdfName filter = PdfName.Adobe_PPKLite;
            PdfName subFilter = PdfName.Adbe_pkcs7_detached;
            PdfReader reader = new PdfReader(input);
            PdfSigner signer = new PdfSigner(reader, new MemoryStream(), new StampingProperties());
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetLayer2Text("Signature field which signing is deferred.").SetPageRect(new Rectangle(36, 600, 
                200, 100)).SetPageNumber(1);
            signer.SetFieldName(sigFieldName);
            IExternalSignatureContainer external = new ExternalBlankSignatureContainer(filter, subFilter);
            // This size is definitely not enough, however, the size check will pass.
            // The test will fail lately on an invalid key
            int estimatedSize = 0;
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => signer.SignExternalContainer(external
                , estimatedSize));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.TOO_BIG_KEY, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeferredHashCalcAndSignTest01() {
            String srcFileName = sourceFolder + "templateForSignCMSDeferred.pdf";
            String outFileName = destinationFolder + "deferredHashCalcAndSignTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_deferredHashCalcAndSignTest01.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
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
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
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
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
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
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
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

        internal static byte[] SignDocBytesHash(byte[] docBytesHash, IPrivateKey pk, IX509Certificate[] chain) {
            if (pk == null || chain == null) {
                return null;
            }
            byte[] signatureContent = null;
            try {
                PdfPKCS7 pkcs7 = new PdfPKCS7(null, chain, HASH_ALGORITHM, false);
                byte[] attributes = pkcs7.GetAuthenticatedAttributeBytes(docBytesHash, PdfSigner.CryptoStandard.CMS, null, 
                    null);
                PrivateKeySignature signature = new PrivateKeySignature(pk, HASH_ALGORITHM);
                byte[] attrSign = signature.Sign(attributes);
                pkcs7.SetExternalSignatureValue(attrSign, null, signature.GetSignatureAlgorithmName());
                signatureContent = pkcs7.GetEncodedPKCS7(docBytesHash);
            }
            catch (AbstractGeneralSecurityException) {
            }
            // dummy catch clause
            return signatureContent;
        }

        internal class CmsDeferredSigner : IExternalSignatureContainer {
            private IPrivateKey pk;

            private IX509Certificate[] chain;

            public CmsDeferredSigner(IPrivateKey pk, IX509Certificate[] chain) {
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
