/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Pdfua.Exceptions;
using iText.Signatures;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUASignerTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUASignerTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(PdfUASignerTest));

        public static readonly String CERTIFICATE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/certificates/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void InvisibleSignatureWithNoTU() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                GenerateSignature(inPdf, "invisibleSignatureWithNoTU", (signer) => {
                }
                );
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvisibleSignatureWithTU() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            String outPdf = GenerateSignature(inPdf, "invisibleSignatureWithTU", (signer) => {
                signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Some alternate description");
                signer.GetSignerProperties().SetSignatureAppearance(appearance);
            }
            );
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void VisibleSignatureWithTUButNotAFont() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            String outPdf = GenerateSignature(inPdf, "visibleSignatureWithTUButNotAFont", (signer) => {
                signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Some alternate description");
                try {
                    appearance.SetFont(PdfFontFactory.CreateFont(FONT));
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                appearance.SetContent("Some signature content");
                signer.GetSignerProperties().SetPageNumber(1).SetPageRect(new Rectangle(36, 648, 200, 100)).SetSignatureAppearance
                    (appearance);
            }
            );
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void VisibleSignatureWithoutTUFont() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                GenerateSignature(inPdf, "visibleSignatureWithoutTUFont", (signer) => {
                    signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                    SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                    appearance.SetContent(new SignedAppearanceText().SetLocationLine("Dummy location").SetReasonLine("Dummy reason"
                        ).SetSignedBy("Dummy"));
                    try {
                        appearance.SetFont(PdfFontFactory.CreateFont(FONT));
                    }
                    catch (System.IO.IOException) {
                        throw new Exception();
                    }
                    signer.GetSignerProperties().SetPageNumber(1).SetPageRect(new Rectangle(36, 648, 200, 100)).SetSignatureAppearance
                        (appearance);
                }
                );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void VisibleSignatureWithNoFontSelected() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                GenerateSignature(inPdf, "visibleSignatureWithNoFontSelected", (signer) => {
                    signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                    SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                    appearance.SetContent("Some signature content");
                    signer.GetSignerProperties().SetPageNumber(1).SetPageRect(new Rectangle(36, 648, 200, 100));
                    appearance.GetAccessibilityProperties().SetAlternateDescription("Some alternate description");
                    appearance.SetContent(new SignedAppearanceText().SetSignedBy("Dummy").SetReasonLine("Dummy reason").SetLocationLine
                        ("Dummy location"));
                    signer.GetSignerProperties().SetSignatureAppearance(appearance);
                }
                );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void NormalPdfSignerInvisibleSignatureWithTU() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            String outPdf = GenerateSignatureNormal(inPdf, "normalPdfSignerInvisibleSignatureWithTU", (signer) => {
                signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Some alternate description");
                signer.GetSignerProperties().SetSignatureAppearance(appearance);
            }
            );
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void NormalPdfSignerInvisibleSignatureWithoutTU() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            String outPdf = GenerateSignatureNormal(inPdf, "normalPdfSignerInvisibleSignatureWithoutTU", (signer) => {
                signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                signer.GetSignerProperties().SetSignatureAppearance(appearance);
            }
            );
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void NormalPdfSignerVisibleSignatureWithoutFont() {
            // TODO DEVSIX-8623 Spike: Get rid of PdfADocument, PdfUADocument, PdfAAgnosticDocument in favour of one PdfDocument
            //This test should fail with the appropriate exception
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            String outPdf = GenerateSignatureNormal(inPdf, "normalPdfSignerVisibleSignatureWithoutFont", (signer) => {
                signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Some alternate description");
                appearance.SetContent(new SignedAppearanceText().SetLocationLine("Dummy location").SetReasonLine("Dummy reason"
                    ).SetSignedBy("Dummy"));
                signer.GetSignerProperties().SetPageNumber(1).SetPageRect(new Rectangle(36, 648, 200, 100)).SetSignatureAppearance
                    (appearance);
            }
            );
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void NormalPdfSignerVisibleSignatureWithFont() {
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            String outPdf = GenerateSignatureNormal(inPdf, "normalPdfSignerVisibleSignatureWithFont", (signer) => {
                signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Some alternate description");
                appearance.SetContent(new SignedAppearanceText().SetLocationLine("Dummy location").SetReasonLine("Dummy reason"
                    ).SetSignedBy("Dummy"));
                appearance.SetFont(font);
                signer.GetSignerProperties().SetPageNumber(1).SetPageRect(new Rectangle(36, 648, 200, 100)).SetSignatureAppearance
                    (appearance);
            }
            );
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void NormalPdfSignerVisibleSignatureWithFontEmptyTU() {
            // TODO DEVSIX-8623 Spike: Get rid of PdfADocument, PdfUADocument, PdfAAgnosticDocument in favour of one PdfDocument
            //Should throw the correct exception if the font is not set
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            String outPdf = GenerateSignatureNormal(inPdf, "normalPdfSignerVisibleSignatureWithFontEmptyTU", (signer) => {
                signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                appearance.GetAccessibilityProperties().SetAlternateDescription("");
                appearance.SetContent(new SignedAppearanceText().SetLocationLine("Dummy location").SetReasonLine("Dummy reason"
                    ).SetSignedBy("Dummy"));
                appearance.SetFont(font);
                signer.GetSignerProperties().SetPageNumber(1).SetPageRect(new Rectangle(36, 648, 200, 100)).SetSignatureAppearance
                    (appearance);
            }
            );
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfSignerVisibleSignatureWithFontEmptyTU() {
            //Should throw the correct exception if the font is not set
            MemoryStream inPdf = GenerateSimplePdfUA1Document();
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                GenerateSignature(inPdf, "pdfSignerVisibleSignatureWithFontEmptyTU", (signer) => {
                    signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature12"));
                    SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID);
                    appearance.GetAccessibilityProperties().SetAlternateDescription("");
                    appearance.SetContent(new SignedAppearanceText().SetLocationLine("Dummy location").SetReasonLine("Dummy reason"
                        ).SetSignedBy("Dummy"));
                    appearance.SetFont(font);
                    signer.GetSignerProperties().SetPageNumber(1).SetPageRect(new Rectangle(36, 648, 200, 100)).SetSignatureAppearance
                        (appearance);
                }
                );
            }
            );
        }

        private MemoryStream GenerateSimplePdfUA1Document() {
            MemoryStream @out = new MemoryStream();
            PdfUADocument pdfUADocument = new PdfUADocument(new PdfWriter(@out), new PdfUAConfig(PdfUAConformance.PDF_UA_1
                , "Title", "en-US"));
            pdfUADocument.AddNewPage();
            pdfUADocument.Close();
            return new MemoryStream(@out.ToArray());
        }

        private String GenerateSignature(MemoryStream inPdf, String name, Action<PdfSigner> signingAction) {
            String certFileName = CERTIFICATE_FOLDER + "sign.pem";
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certFileName, PASSWORD);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certFileName);
            String outPdf = DESTINATION_FOLDER + name + ".pdf";
            PdfSigner signer = new PdfUASignerTest.PdfUaSigner(new PdfReader(inPdf), FileUtil.GetFileOutputStream(outPdf
                ), new StampingProperties());
            signingAction(signer);
            signer.SignDetached(new BouncyCastleDigest(), pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard
                .CADES);
            logger.LogInformation("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outPdf));
            return outPdf;
        }

        private String GenerateSignatureNormal(MemoryStream inPdf, String name, Action<PdfSigner> signingAction) {
            String certFileName = CERTIFICATE_FOLDER + "sign.pem";
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certFileName, PASSWORD);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certFileName);
            String outPdf = DESTINATION_FOLDER + name + ".pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(inPdf), FileUtil.GetFileOutputStream(outPdf), new StampingProperties
                ());
            signingAction(signer);
            signer.SignDetached(new BouncyCastleDigest(), pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard
                .CADES);
            logger.LogInformation("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outPdf));
            return outPdf;
        }

//\cond DO_NOT_DOCUMENT
        internal class PdfUaSigner : PdfSigner {
            public PdfUaSigner(PdfReader reader, Stream outputStream, StampingProperties properties)
                : base(reader, outputStream, properties) {
            }

            public PdfUaSigner(PdfReader reader, Stream outputStream, String path, StampingProperties stampingProperties
                , SignerProperties signerProperties)
                : base(reader, outputStream, path, stampingProperties, signerProperties) {
            }

            public PdfUaSigner(PdfReader reader, Stream outputStream, String path, StampingProperties properties)
                : base(reader, outputStream, path, properties) {
            }

            protected override PdfDocument InitDocument(PdfReader reader, PdfWriter writer, StampingProperties properties
                ) {
                return new PdfUADocument(reader, writer, new PdfUAConfig(PdfUAConformance.PDF_UA_1, "Title", "en-US"));
            }
        }
//\endcond
    }
}
