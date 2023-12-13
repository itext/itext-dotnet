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
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new SignaturePolicyInfo(null, 
                POLICY_HASH, POLICY_DIGEST_ALGORITHM, POLICY_URI));
            NUnit.Framework.Assert.AreEqual("Policy identifier cannot be null", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyIdentifierIsNotAllowedTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new SignaturePolicyInfo("", POLICY_HASH
                , POLICY_DIGEST_ALGORITHM, POLICY_URI));
            NUnit.Framework.Assert.AreEqual("Policy identifier cannot be null", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NullPolicyHashIsNotAllowedTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new SignaturePolicyInfo(POLICY_IDENTIFIER
                , (byte[])null, POLICY_DIGEST_ALGORITHM, POLICY_URI));
            NUnit.Framework.Assert.AreEqual("Policy hash cannot be null", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NullEncodedPolicyHashIsNotAllowedTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new SignaturePolicyInfo(POLICY_IDENTIFIER
                , (String)null, POLICY_DIGEST_ALGORITHM, POLICY_URI));
            NUnit.Framework.Assert.AreEqual("Policy hash cannot be null", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NullDigestAlgorithmIsNotAllowedTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new SignaturePolicyInfo(POLICY_IDENTIFIER
                , POLICY_HASH, null, POLICY_URI));
            NUnit.Framework.Assert.AreEqual("Policy digest algorithm cannot be null", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyDigestAlgorithmIsNotAllowedTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new SignaturePolicyInfo(POLICY_IDENTIFIER
                , POLICY_HASH, "", POLICY_URI));
            NUnit.Framework.Assert.AreEqual("Policy digest algorithm cannot be null", e.Message);
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
            DerObjectIdentifier objectIdentifier = new DerObjectIdentifier(POLICY_IDENTIFIER);
            DerObjectIdentifier objectIdentifierInstance = DerObjectIdentifier.GetInstance(objectIdentifier);
            SignaturePolicyId signaturePolicyId = new SignaturePolicyId(objectIdentifierInstance, otherHashAlgAndValue
                , SignUtils.CreateSigPolicyQualifiers(sigPolicyQualifierInfo));
            SignaturePolicyIdentifier expected = new SignaturePolicyIdentifier(signaturePolicyId);
            NUnit.Framework.Assert.AreEqual(expected.ToAsn1Object(), actual.ToAsn1Object());
        }

        [NUnit.Framework.Test]
        public virtual void ToSignaturePolicyIdentifierUnexpectedAlgorithmTest() {
            SignaturePolicyInfo info = new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH, "SHA-12345", POLICY_URI
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => info.ToSignaturePolicyIdentifier
                ());
            NUnit.Framework.Assert.AreEqual("Invalid policy hash algorithm", e.Message);
        }
    }
}
