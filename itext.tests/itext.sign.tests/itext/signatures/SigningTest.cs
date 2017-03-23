/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2017 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using Org.BouncyCastle.Pkcs;

namespace iText.Signatures {
    public class SigningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext.CurrentContext
                                                         .TestDirectory) + "/resources/itext/signatures/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
                                                              .TestDirectory + "/test/itext/signatures/";

        public static readonly String keystorePath = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext.CurrentContext
                                                         .TestDirectory) + "/resources/itext/signatures/ks";

        public static readonly char[] password = "password".ToCharArray();

        private X509Certificate[] chain;

        private ICipherParameters pk;

        //TODO: add some validation of results in future
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.SecurityUtilityException"/>
        [NUnit.Framework.OneTimeSetUp]
        public virtual void Init() {
            CreateOrClearDestinationFolder(destinationFolder);

            string alias = null;
            Pkcs12Store pk12;

            pk12 = new Pkcs12Store(new FileStream(keystorePath, FileMode.Open, FileAccess.Read), password);

            foreach (var a in pk12.Aliases) {
                alias = ((string) a);
                if (pk12.IsKeyEntry(alias))
                    break;
            }
            pk = pk12.GetKey(alias).Key;
            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            chain = new X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
                chain[k] = ce[k].Certificate;
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleSigningTest() {
            String src = sourceFolder + "simpleDocument.pdf";
            String fileName = "simpleSignature.pdf";
            String dest = destinationFolder + fileName;
            int x = 36;
            int y = 648;
            int w = 200;
            int h = 100;
            Rectangle rect = new Rectangle(x, y, w, h);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Test 1", "TestCity", rect, false, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder
                                                                                  + "cmp_" + fileName, destinationFolder,
                "diff_", GetTestMap(new Rectangle(67, 690, 155, 15))));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SigningIntoExistingFieldTest01() {
            String src = sourceFolder + "emptySignature01.pdf";
            //field is merged with widget and has /P key
            String fileName = "filledSignatureFields01.pdf";
            String dest = destinationFolder + fileName;
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Test 1", "TestCity", null, false, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder
                                                                                  + "cmp_" + fileName, destinationFolder,
                "diff_", GetTestMap(new Rectangle(67, 725, 160, 15))));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SigningIntoExistingFieldTest02() {
            String src = sourceFolder + "emptySignature02.pdf";
            //field is merged with widget and widget doesn't have /P key
            String fileName = "filledSignatureFields02.pdf";
            String dest = destinationFolder + fileName;
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Test 1", "TestCity", null, false, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder
                                                                                  + "cmp_" + fileName, destinationFolder,
                "diff_", GetTestMap(new Rectangle(67, 725, 160, 15))));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void SigningIntoExistingReuseAppearanceTest() {
            String src = sourceFolder + "emptySigWithAppearance.pdf";
            String dest = destinationFolder + "filledSignatureReuseAppearanceFields.pdf";
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Test 1", "TestCity", null, true, false);
        }

        [NUnit.Framework.Test]
        public virtual void SigningTaggedDocument() {
            String src = sourceFolder + "simpleTaggedDocument.pdf";
            String dest = destinationFolder + "signedTaggedDocument.pdf";

            Rectangle rect = new Rectangle(36, 648, 200, 100);

            String fieldName = "Signature1";

            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Test 1", "TestCity", rect, false, false);
//            Sign(src, fieldName, dest, chain, pk,
//                DigestAlgorithms.SHA256, provider.getName(),
//                PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity", rect, false, false);
        }

        [NUnit.Framework.Test]
        public virtual void SigningTaggedDocumentAppendMode() {
            String src = sourceFolder + "simpleTaggedDocument.pdf";
            String dest = destinationFolder + "signedTaggedDocumentAppendMode.pdf";

            Rectangle rect = new Rectangle(36, 648, 200, 100);

            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Test 1", "TestCity", rect, false, true);
        }

        [NUnit.Framework.Test]
        public virtual void SigningDocumentAppendModeIndirectPageAnnots() {
            String file =  "AnnotsIndirect.pdf";
            String src = sourceFolder + file;
            String dest = destinationFolder + file;

            Rectangle rect = new Rectangle(30, 200, 200, 100);

            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Test 1", "TestCity", rect, false, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + file, destinationFolder,
                "diff_", GetTestMap(new Rectangle(30, 245, 200, 12))));
        }


        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        protected internal virtual void Sign(String src, String name, String dest, X509Certificate
            [] chain, ICipherParameters pk, String digestAlgorithm, PdfSigner.CryptoStandard
                subfilter, String reason, String location, Rectangle rectangleForNewField, bool
                    setReuseAppearance, bool isAppendMode) {
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), isAppendMode
                );
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance().SetReason(reason
                ).SetLocation(location).SetReuseAppearance(setReuseAppearance);
            if (rectangleForNewField != null) {
                appearance.SetPageRect(rectangleForNewField);
            }
            signer.SetFieldName(name);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
        }

        private static Dictionary<int, IList<Rectangle>> GetTestMap(Rectangle ignoredArea) {
            return new Dictionary<int, IList<Rectangle>> {
                {1, IO.Util.JavaUtil.ArraysAsList(ignoredArea)}
            };
        }
    }
}
