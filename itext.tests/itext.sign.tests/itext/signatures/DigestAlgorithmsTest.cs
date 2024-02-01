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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Signatures.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class DigestAlgorithmsTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static readonly bool FIPS_MODE = "BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName());

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStringOidGetDigestTest() {
            String oid = "";
            NUnit.Framework.Assert.AreEqual(oid, DigestAlgorithms.GetDigest(oid));
        }

        [NUnit.Framework.Test]
        public virtual void NonExistingOidGetDigestTest() {
            String oid = "non_existing_oid";
            NUnit.Framework.Assert.AreEqual(oid, DigestAlgorithms.GetDigest(oid));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStringNameGetAllowedDigestTest() {
            NUnit.Framework.Assert.IsNull(DigestAlgorithms.GetAllowedDigest(""));
        }

        [NUnit.Framework.Test]
        public virtual void NonExistingNameGetAllowedDigestTest() {
            NUnit.Framework.Assert.IsNull(DigestAlgorithms.GetAllowedDigest("non_existing_oid"));
        }

        [NUnit.Framework.Test]
        public virtual void NullNameGetAllowedDigestTest() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => DigestAlgorithms.GetAllowedDigest(null));
        }

        [LogMessage(SignLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        [NUnit.Framework.Test]
        public virtual void NotAllowedOidGetDigestTest() {
            String name = "SM3";
            String oid = "1.2.156.10197.1.401";
            NUnit.Framework.Assert.AreEqual(FIPS_MODE ? oid : name, DigestAlgorithms.GetDigest(oid));
        }

        [LogMessage(SignLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        [NUnit.Framework.Test]
        public virtual void NotAllowedNameGetAllowedDigestTest() {
            String name = "SM3";
            String oid = "1.2.156.10197.1.401";
            NUnit.Framework.Assert.AreEqual(FIPS_MODE ? null : oid, DigestAlgorithms.GetAllowedDigest(name));
        }
    }
}
