using System;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Test;

namespace iText.Signatures.Sign {
    public class PdfSignatureTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureTest/";

        [NUnit.Framework.Test]
        public virtual void SetByteRangeTest() {
            using (PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "simpleSignature.pdf"))) {
                SignatureUtil sigUtil = new SignatureUtil(doc);
                PdfSignature signature = sigUtil.GetSignature("Signature1");
                int[] byteRange = new int[] { 0, 141, 16526, 2494 };
                signature.SetByteRange(byteRange);
                PdfArray expected = new PdfArray((new int[] { 0, 141, 16526, 2494 }));
                NUnit.Framework.Assert.AreEqual(expected.ToIntArray(), signature.GetByteRange().ToIntArray());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetContentsTest() {
            using (PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "simpleSignature.pdf"))) {
                SignatureUtil sigUtil = new SignatureUtil(doc);
                PdfSignature signature = sigUtil.GetSignature("Signature1");
                byte[] newContents = new PdfString("new iText signature").GetValueBytes();
                signature.SetContents(newContents);
                NUnit.Framework.Assert.AreEqual("new iText signature", signature.GetContents().GetValue());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetCertTest() {
            using (PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "adbe.x509.rsa_sha1_signature.pdf"))
                ) {
                SignatureUtil sigUtil = new SignatureUtil(doc);
                PdfSignature signature = sigUtil.GetSignature("Signature1");
                byte[] certChain = new PdfString("Hello, iText!!").GetValueBytes();
                signature.SetCert(certChain);
                NUnit.Framework.Assert.AreEqual("Hello, iText!!", signature.GetCertObject().ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetCertObjectTest() {
            using (PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "adbe.x509.rsa_sha1_signature.pdf"))
                ) {
                SignatureUtil sigUtil = new SignatureUtil(doc);
                PdfSignature signature = sigUtil.GetSignature("Signature1");
                NUnit.Framework.Assert.IsTrue(signature.GetCertObject().IsArray());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetNameTest() {
            using (PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "simpleSignature.pdf"))) {
                SignatureUtil sigUtil = new SignatureUtil(doc);
                PdfSignature signature = sigUtil.GetSignature("Signature1");
                NUnit.Framework.Assert.IsNull(signature.GetName());
                String name = "iText person";
                signature.SetName(name);
                NUnit.Framework.Assert.AreEqual(name, signature.GetName());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetSignatureCreatorTest() {
            using (PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "noPropBuilds.pdf"))) {
                SignatureUtil sigUtil = new SignatureUtil(doc);
                PdfSignature signature = sigUtil.GetSignature("Signature1");
                NUnit.Framework.Assert.IsNull(signature.GetPdfObject().GetAsDictionary(PdfName.Prop_Build));
                signature.SetSignatureCreator("iText.Name");
                String propBuild = signature.GetPdfObject().GetAsDictionary(PdfName.Prop_Build).GetAsDictionary(PdfName.App
                    ).GetAsName(PdfName.Name).GetValue();
                NUnit.Framework.Assert.AreEqual("iText.Name", propBuild);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PdfSignatureAppDefaultConstructorTest() {
            PdfSignatureApp signatureApp = new PdfSignatureApp();
            NUnit.Framework.Assert.IsTrue(signatureApp.GetPdfObject().IsDictionary());
        }
    }
}
