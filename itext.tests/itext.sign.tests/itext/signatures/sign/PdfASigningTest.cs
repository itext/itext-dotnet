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
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    public class PdfASigningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfASigningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfASigningTest/";

        public static readonly String keystorePath = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/signCertRsa01.p12";

        public static readonly char[] password = "testpass".ToCharArray();

        public static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/font/FreeSans.ttf";

        private X509Certificate[] chain;

        private ICipherParameters pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = Pkcs12FileHelper.ReadFirstKey(keystorePath, password, password);
            chain = Pkcs12FileHelper.ReadFirstChain(keystorePath, password);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSigningTest() {
            String src = sourceFolder + "simplePdfADocument.pdf";
            String fileName = "simpleSignature.pdf";
            String dest = destinationFolder + fileName;
            int x = 36;
            int y = 548;
            int w = 200;
            int h = 100;
            Rectangle rect = new Rectangle(x, y, w, h);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , rect, false, false, PdfSigner.NOT_CERTIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                , "diff_", GetTestMap(new Rectangle(67, 575, 155, 15))));
        }

        protected internal virtual void Sign(String src, String name, String dest, X509Certificate[] chain, ICipherParameters
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, String reason, String location, Rectangle
             rectangleForNewField, bool setReuseAppearance, bool isAppendMode) {
            Sign(src, name, dest, chain, pk, digestAlgorithm, subfilter, reason, location, rectangleForNewField, setReuseAppearance
                , isAppendMode, PdfSigner.NOT_CERTIFIED, null);
        }

        protected internal virtual void Sign(String src, String name, String dest, X509Certificate[] chain, ICipherParameters
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, String reason, String location, Rectangle
             rectangleForNewField, bool setReuseAppearance, bool isAppendMode, int certificationLevel, float? fontSize
            ) {
            PdfReader reader = new PdfReader(src);
            StampingProperties properties = new StampingProperties();
            if (isAppendMode) {
                properties.UseAppendMode();
            }
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), properties);
            signer.SetCertificationLevel(certificationLevel);
            PdfFont font = PdfFontFactory.CreateFont(FONT, "WinAnsi", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance().SetReason(reason).SetLocation(location
                ).SetLayer2Font(font).SetReuseAppearance(setReuseAppearance);
            if (rectangleForNewField != null) {
                appearance.SetPageRect(rectangleForNewField);
            }
            if (fontSize != null) {
                appearance.SetLayer2FontSize((float)fontSize);
            }
            signer.SetFieldName(name);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
        }

        private static IDictionary<int, IList<Rectangle>> GetTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaUtil.ArraysAsList(ignoredArea));
            return result;
        }
    }
}
