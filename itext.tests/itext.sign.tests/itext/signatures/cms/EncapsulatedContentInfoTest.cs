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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Test;

namespace iText.Signatures.Cms {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class EncapsulatedContentInfoTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const String ENCODED_WITH_CONTENT_B64 = "MH0GCyqGSIb3DQEJEAEEoG4EbDBqAgEBBglghkgBhv1sBwEwMTANBglg"
             + "hkgBZQMEAgEFAAQgSbIIRqXY9+m1GfDgEnVFrQw//OObVVmEk4sLQ4uirygCEHHvE6CzVVvOraJrAlIXOO8YDzIwMjMxMDMxMDU1ODQ"
             + "5WgIEwrIa7w==";

        [NUnit.Framework.Test]
        public virtual void TestDeserializationWithoutContent() {
            IAsn1EncodableVector v = FACTORY.CreateASN1EncodableVector();
            v.Add(FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_DATA));
            IAsn1Sequence testData = FACTORY.CreateDERSequence(v);
            EncapsulatedContentInfo sut = new EncapsulatedContentInfo(testData);
            NUnit.Framework.Assert.AreEqual(SecurityIDs.ID_PKCS7_DATA, sut.GetContentType());
            NUnit.Framework.Assert.IsNull(sut.GetContent());
        }

        [NUnit.Framework.Test]
        public virtual void TestDeserializationWithContent() {
            IAsn1Sequence testData = FACTORY.CreateASN1Sequence(Convert.FromBase64String(ENCODED_WITH_CONTENT_B64));
            EncapsulatedContentInfo sut = new EncapsulatedContentInfo(testData);
            NUnit.Framework.Assert.AreEqual("1.2.840.113549.1.9.16.1.4", sut.GetContentType());
            NUnit.Framework.Assert.IsNotNull(sut.GetContent());
        }

        [NUnit.Framework.Test]
        public virtual void TestCreation() {
            EncapsulatedContentInfo sut = new EncapsulatedContentInfo(SecurityIDs.ID_PKCS7_DATA);
            NUnit.Framework.Assert.AreEqual(SecurityIDs.ID_PKCS7_DATA, sut.GetContentType());
            NUnit.Framework.Assert.IsNull(sut.GetContent());
        }

        [NUnit.Framework.Test]
        public virtual void TestCreationWithContent() {
            EncapsulatedContentInfo sut = new EncapsulatedContentInfo(SecurityIDs.ID_PKCS7_DATA, FACTORY.CreateDEROctetString
                (new byte[20]));
            NUnit.Framework.Assert.AreEqual(SecurityIDs.ID_PKCS7_DATA, sut.GetContentType());
            NUnit.Framework.Assert.IsNotNull(sut.GetContent());
        }
    }
}
