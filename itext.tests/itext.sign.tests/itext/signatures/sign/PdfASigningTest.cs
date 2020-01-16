/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
            PdfFont font = PdfFontFactory.CreateFont(FONT, "WinAnsi", true);
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
