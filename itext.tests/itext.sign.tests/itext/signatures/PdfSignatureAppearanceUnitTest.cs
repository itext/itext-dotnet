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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures {
    /// <summary>
    /// The idea of this test is to check the
    /// <see cref="PdfSignatureAppearance"/>
    /// 's getters.
    /// </summary>
    /// <remarks>
    /// The idea of this test is to check the
    /// <see cref="PdfSignatureAppearance"/>
    /// 's getters.
    /// For actual result of setters invocations one should check the integration test for this class.
    /// </remarks>
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PdfSignatureAppearanceUnitTest : ExtendedITextTest {
        // The source folder points to the integration test, so that the resources are nor duplicated
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureAppearanceTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfSignatureAppearanceUnitTest/";

        public static readonly String KEYSTORE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureAppearanceTest/test.pem";

        public static readonly char[] PASSWORD = "kspass".ToCharArray();

        private static IX509Certificate[] chain;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
            chain = PemFileHelper.ReadFirstChain(KEYSTORE_PATH);
        }

        [NUnit.Framework.Test]
        public virtual void ReasonCaptionTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            String newReasonCaption = "Hello World";
            NUnit.Framework.Assert.IsNull(signatureAppearance.GetLayer2Text());
            String layer2Text = signatureAppearance.GetModelElement().GetDescription(true);
            // There is no text from new reason caption in the default layer 2 text
            NUnit.Framework.Assert.IsFalse(layer2Text.Contains(newReasonCaption));
            signatureAppearance.SetReasonCaption(newReasonCaption);
            layer2Text = signatureAppearance.GetModelElement().GetDescription(true);
            // Now layer 2 text contains text from new reason caption
            NUnit.Framework.Assert.IsTrue(layer2Text.Contains(newReasonCaption));
        }

        [NUnit.Framework.Test]
        public virtual void LocationCaptionTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            String newLocationCaption = "Hello World";
            NUnit.Framework.Assert.IsNull(signatureAppearance.GetLayer2Text());
            String layer2Text = signatureAppearance.GetModelElement().GetDescription(true);
            // There is no text from new location caption in the default layer 2 text
            NUnit.Framework.Assert.IsFalse(layer2Text.Contains(newLocationCaption));
            signatureAppearance.SetLocationCaption(newLocationCaption);
            layer2Text = signatureAppearance.GetModelElement().GetDescription(true);
            // Now layer 2 text contains text from new location caption
            NUnit.Framework.Assert.IsTrue(layer2Text.Contains(newLocationCaption));
        }

        [NUnit.Framework.Test]
        public virtual void RenderingModeSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            PdfSignatureAppearance.RenderingMode defaultMode = signatureAppearance.GetRenderingMode();
            NUnit.Framework.Assert.AreEqual(PdfSignatureAppearance.RenderingMode.DESCRIPTION, defaultMode);
            PdfSignatureAppearance.RenderingMode testRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
            signatureAppearance.SetRenderingMode(testRenderingMode);
            NUnit.Framework.Assert.AreEqual(testRenderingMode, signatureAppearance.GetRenderingMode());
        }

        [NUnit.Framework.Test]
        public virtual void SignatureCreatorSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.AreEqual("", signatureAppearance.GetSignatureCreator());
            String signatureCreator = "Hello World";
            signatureAppearance.SetSignatureCreator(signatureCreator);
            NUnit.Framework.Assert.AreEqual(signatureCreator, signatureAppearance.GetSignatureCreator());
        }

        [NUnit.Framework.Test]
        public virtual void ContactSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.AreEqual("", signatureAppearance.GetContact());
            String contact = "Hello World";
            signatureAppearance.SetContact(contact);
            NUnit.Framework.Assert.AreEqual(contact, signatureAppearance.GetContact());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.IsNull(signatureAppearance.GetCertificate());
            IX509Certificate testCertificate = chain[0];
            signatureAppearance.SetCertificate(testCertificate);
            NUnit.Framework.Assert.AreEqual(testCertificate, signatureAppearance.GetCertificate());
        }

        [NUnit.Framework.Test]
        public virtual void SignatureGraphicSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.IsNull(signatureAppearance.GetSignatureGraphic());
            ImageData testImageData = ImageDataFactory.Create(SOURCE_FOLDER + "itext.png");
            signatureAppearance.SetSignatureGraphic(testImageData);
            NUnit.Framework.Assert.AreEqual(testImageData, signatureAppearance.GetSignatureGraphic());
        }

        [NUnit.Framework.Test]
        public virtual void ImageSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.IsNull(signatureAppearance.GetImage());
            ImageData testImageData = ImageDataFactory.Create(SOURCE_FOLDER + "itext.png");
            signatureAppearance.SetImage(testImageData);
            NUnit.Framework.Assert.AreEqual(testImageData, signatureAppearance.GetImage());
        }

        [NUnit.Framework.Test]
        public virtual void ImageScalingSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.AreEqual(0, signatureAppearance.GetImageScale(), 0.0001);
            float newScale = 1F;
            signatureAppearance.SetImageScale(newScale);
            NUnit.Framework.Assert.AreEqual(newScale, signatureAppearance.GetImageScale(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void Layer2FontSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.IsNull(signatureAppearance.GetLayer2Font());
            PdfFont newFont = PdfFontFactory.CreateFont();
            signatureAppearance.SetLayer2Font(newFont);
            NUnit.Framework.Assert.AreEqual(newFont, signatureAppearance.GetLayer2Font());
        }

        [NUnit.Framework.Test]
        public virtual void Layer2FontSizeSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.AreEqual(0, signatureAppearance.GetLayer2FontSize(), 0.0001);
            float newSize = 12F;
            signatureAppearance.SetLayer2FontSize(newSize);
            NUnit.Framework.Assert.AreEqual(newSize, signatureAppearance.GetLayer2FontSize(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void Layer2FontColorSetGetTest() {
            PdfSignatureAppearance signatureAppearance = GetTestSignatureAppearance();
            NUnit.Framework.Assert.IsNull(signatureAppearance.GetLayer2FontColor());
            Color newColor = ColorConstants.RED;
            signatureAppearance.SetLayer2FontColor(newColor);
            NUnit.Framework.Assert.AreEqual(newColor, signatureAppearance.GetLayer2FontColor());
        }

        [NUnit.Framework.Test]
        public virtual void GetAppearanceInvisibleTest() {
            PdfSignatureAppearance appearance = new PdfSignatureAppearance(new PdfDocument(new PdfWriter(new MemoryStream
                ())), new Rectangle(0, 100), 1);
            PdfFormXObject xObject = appearance.GetAppearance();
            NUnit.Framework.Assert.IsTrue(new Rectangle(0, 0).EqualsWithEpsilon(xObject.GetBBox().ToRectangle()));
        }

        [NUnit.Framework.Test]
        public virtual void GetSignDateTest() {
            PdfSignatureAppearance appearance = new PdfSignatureAppearance(null, new Rectangle(100, 100), 1);
            DateTime current = DateTimeUtil.GetCurrentTime();
            appearance.SetSignDate(current);
            NUnit.Framework.Assert.AreEqual(current, appearance.GetSignDate());
        }

        private static PdfSignatureAppearance GetTestSignatureAppearance() {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), new MemoryStream(), new StampingProperties());
            return signer.GetSignatureAppearance();
        }
    }
}
