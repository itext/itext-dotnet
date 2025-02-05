/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Digest;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class BouncyCastleDigestUnitTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        private static readonly bool FIPS_MODE = FIPS_MODE = "BCFIPS".Equals(FACTORY.GetProviderName());

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestMD2Test() {
            if (FIPS_MODE) {
                NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => 
                    new BouncyCastleDigest().GetMessageDigest("MD2"));
            }
            else {
                GetMessageDigestTest("MD2", "MD2");
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestMD5Test() {
            GetMessageDigestTest("MD5", "MD5");
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestSHA1Test() {
            GetMessageDigestTest("SHA1", "SHA-1");
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestSHA224Test() {
            if (FIPS_MODE) {
                NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => 
                    new BouncyCastleDigest().GetMessageDigest("SHA-224"));
            }
            else {
                GetMessageDigestTest("SHA224", "SHA-224");
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestSHA256Test() {
            GetMessageDigestTest("SHA256", "SHA-256");
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestSHA384Test() {
            GetMessageDigestTest("SHA384", "SHA-384");
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestSHA512Test() {
            GetMessageDigestTest("SHA512", "SHA-512");
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestRIPEMD128Test() {
            if (FIPS_MODE) {
                NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => 
                    new BouncyCastleDigest().GetMessageDigest("RIPEMD128"));
            }
            else {
                GetMessageDigestTest("RIPEMD128", "RIPEMD128");
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestRIPEMD160Test() {
            if (FIPS_MODE) {
                NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => 
                    new BouncyCastleDigest().GetMessageDigest("RIPEMD160"));
            }
            else {
                GetMessageDigestTest("RIPEMD160", "RIPEMD160");
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestRIPEMD256Test() {
            if (FIPS_MODE) {
                NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => 
                    new BouncyCastleDigest().GetMessageDigest("RIPEMD256"));
            }
            else {
                GetMessageDigestTest("RIPEMD256", "RIPEMD256");
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestGOST3411Test() {
            if (FIPS_MODE) {
                NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => 
                    new BouncyCastleDigest().GetMessageDigest("Gost3411"));
            }
            else {
                GetMessageDigestTest("Gost3411", "Gost3411");
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestNullTest() {
            IExternalDigest digest = new BouncyCastleDigest();
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => digest.GetMessageDigest(null));
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestUnknownTest() {
            IExternalDigest digest = new BouncyCastleDigest();
            if (FIPS_MODE) {
                NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => digest.GetMessageDigest("unknown"));
            }
            else {
                NUnit.Framework.Assert.Catch(typeof(AbstractSecurityUtilityException),
                    () => digest.GetMessageDigest("unknown"));
            }
        }

        private static void GetMessageDigestTest(String hashAlgorithm, String expectedDigestAlgorithm) {
            IMessageDigest digest = new BouncyCastleDigest().GetMessageDigest(hashAlgorithm);
            NUnit.Framework.Assert.IsNotNull(digest);
            NUnit.Framework.Assert.AreEqual(expectedDigestAlgorithm, digest.GetAlgorithmName());
        }
    }
}
