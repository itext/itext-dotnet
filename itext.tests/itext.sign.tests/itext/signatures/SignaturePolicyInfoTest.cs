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
using Org.BouncyCastle;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.X509;
using iText.IO.Codec;
using iText.Test;

namespace iText.Signatures {
    public class SignaturePolicyInfoTest : ExtendedITextTest {
        private const String POLICY_IDENTIFIER = "2.16.724.1.3.1.1.2.1.9";

        private const String POLICY_HASH_BASE64 = "G7roucf600+f03r/o0bAOQ6WAs0=";

        private static readonly byte[] POLICY_HASH = Convert.FromBase64String(POLICY_HASH_BASE64);

        private const String POLICY_DIGEST_ALGORITHM = "SHA-256";

        private const String POLICY_URI = "https://sede.060.gob.es/politica_de_firma_anexo_1.pdf";

        [NUnit.Framework.Test]
        public virtual void CheckConstructorTest() {
            SignaturePolicyInfo info = new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH, POLICY_DIGEST_ALGORITHM
                , POLICY_URI);
            NUnit.Framework.Assert.AreEqual(POLICY_IDENTIFIER, info.GetPolicyIdentifier());
            NUnit.Framework.Assert.AreEqual(POLICY_HASH, info.GetPolicyHash());
            NUnit.Framework.Assert.AreEqual(POLICY_DIGEST_ALGORITHM, info.GetPolicyDigestAlgorithm());
            NUnit.Framework.Assert.AreEqual(POLICY_URI, info.GetPolicyUri());
        }

        [NUnit.Framework.Test]
        public virtual void CheckConstructorWithEncodedHashTest() {
            SignaturePolicyInfo info = new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH_BASE64, POLICY_DIGEST_ALGORITHM
                , POLICY_URI);
            NUnit.Framework.Assert.AreEqual(POLICY_IDENTIFIER, info.GetPolicyIdentifier());
            NUnit.Framework.Assert.AreEqual(POLICY_HASH, info.GetPolicyHash());
            NUnit.Framework.Assert.AreEqual(POLICY_DIGEST_ALGORITHM, info.GetPolicyDigestAlgorithm());
            NUnit.Framework.Assert.AreEqual(POLICY_URI, info.GetPolicyUri());
        }

        [NUnit.Framework.Test]
        public virtual void NullIdentifierIsNotAllowedTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SignaturePolicyInfo(null, POLICY_HASH, POLICY_DIGEST_ALGORITHM, POLICY_URI);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Policy identifier cannot be null"))
;
        }

        [NUnit.Framework.Test]
        public virtual void EmptyIdentifierIsNotAllowedTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SignaturePolicyInfo("", POLICY_HASH, POLICY_DIGEST_ALGORITHM, POLICY_URI);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Policy identifier cannot be null"))
;
        }

        [NUnit.Framework.Test]
        public virtual void NullPolicyHashIsNotAllowedTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SignaturePolicyInfo(POLICY_IDENTIFIER, (byte[])null, POLICY_DIGEST_ALGORITHM, POLICY_URI);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Policy hash cannot be null"))
;
        }

        [NUnit.Framework.Test]
        public virtual void NullEncodedPolicyHashIsNotAllowedTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SignaturePolicyInfo(POLICY_IDENTIFIER, (String)null, POLICY_DIGEST_ALGORITHM, POLICY_URI);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Policy hash cannot be null"))
;
        }

        [NUnit.Framework.Test]
        public virtual void NullDigestAlgorithmIsNotAllowedTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH, null, POLICY_URI);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Policy digest algorithm cannot be null"))
;
        }

        [NUnit.Framework.Test]
        public virtual void EmptyDigestAlgorithmIsNotAllowedTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH, "", POLICY_URI);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Policy digest algorithm cannot be null"))
;
        }

        [NUnit.Framework.Test]
        public virtual void ToSignaturePolicyIdentifierTest() {
            SignaturePolicyIdentifier actual = new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH, POLICY_DIGEST_ALGORITHM
                , POLICY_URI).ToSignaturePolicyIdentifier();
            DerIA5String deria5String = new DerIA5String(POLICY_URI);
            SigPolicyQualifierInfo sigPolicyQualifierInfo = new SigPolicyQualifierInfo(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdSpqEtsUri
                , deria5String);
            DerOctetString derOctetString = new DerOctetString(POLICY_HASH);
            String algId = DigestAlgorithms.GetAllowedDigest(POLICY_DIGEST_ALGORITHM);
            DerObjectIdentifier asn1ObjectIdentifier = new DerObjectIdentifier(algId);
            AlgorithmIdentifier algorithmIdentifier = new AlgorithmIdentifier(asn1ObjectIdentifier);
            OtherHashAlgAndValue otherHashAlgAndValue = new OtherHashAlgAndValue(algorithmIdentifier, derOctetString);
            DerObjectIdentifier derObjectIdentifier = new DerObjectIdentifier(POLICY_IDENTIFIER);
            DerObjectIdentifier derObjectIdentifierInstance = DerObjectIdentifier.GetInstance(derObjectIdentifier);
            SignaturePolicyId signaturePolicyId = new SignaturePolicyId(derObjectIdentifierInstance, otherHashAlgAndValue
                , SignUtils.CreateSigPolicyQualifiers(sigPolicyQualifierInfo));
            SignaturePolicyIdentifier expected = new SignaturePolicyIdentifier(signaturePolicyId);
            NUnit.Framework.Assert.AreEqual(expected.ToAsn1Object(), actual.ToAsn1Object());
        }

        [NUnit.Framework.Test]
        public virtual void ToSignaturePolicyIdentifierUnexpectedAlgorithmTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH, "SHA-12345", POLICY_URI).ToSignaturePolicyIdentifier
                    ();
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Invalid policy hash algorithm"))
;
        }
    }
}
