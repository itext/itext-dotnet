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
using iText.Kernel.Exceptions;

namespace iText.Signatures {
    // The behavior is different on .NET
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PdfPKCS7ManuallyPortedTest : PdfPKCS7BasicTest {
        [NUnit.Framework.Test]
        public virtual void VerifyEd25519SignatureTest() {
            if ("BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName())) {
                // ED25519 is not available in FIPS approved mode
                if (BOUNCY_CASTLE_FACTORY.IsInApprovedOnlyMode()) {
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => VerifyIsoExtensionExample("Ed25519", "sample-ed25519-sha512.pdf"
                        ));
                } else {
                    // algorithm identifier in key not recognised
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => VerifyIsoExtensionExample("Ed25519", "sample-ed25519-sha512.pdf"
                        ));
                }
            } else {
                VerifyIsoExtensionExample("Ed25519", "sample-ed25519-sha512.pdf");
            }
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNistECDSASha3SignatureTest() {
            if ("BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName())) {
                VerifyIsoExtensionExample("SHA3-256withECDSA", "sample-nistp256-sha3_256.pdf");
            } else {
                // Signer SHA3-256WITHECDSA not recognised in BC mode
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => VerifyIsoExtensionExample("SHA3-256withECDSA", "sample-nistp256-sha3_256.pdf"
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void VerifyBrainpoolSha3SignatureTest() {
            if ("BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName())) {
                VerifyIsoExtensionExample("SHA3-384withECDSA", "sample-brainpoolP384r1-sha3_384.pdf");
            } else {
                // Signer SHA3-384WITHECDSA not recognised in BC mode
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => VerifyIsoExtensionExample("SHA3-384withECDSA", "sample-brainpoolP384r1-sha3_384.pdf"
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void VerifyRsaSha3SignatureTest() {
            // VerifySignatureIntegrityAndAuthenticity fails in BCFIPS mode
            if ("BC".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName())) {
                VerifyIsoExtensionExample("SHA3-256withRSA", "sample-rsa-sha3_256.pdf");
            }
        }

        [NUnit.Framework.Test]
        public virtual void VerifyRsaPssSha3SignatureTest()
        {
            if ("BC".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName())) {
                VerifyIsoExtensionExample("RSASSA-PSS", "sample-pss-sha3_256.pdf");
            } else {
                // Signer "RSASSA-PSS not recognised in BCFIPS mode
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => VerifyIsoExtensionExample("RSASSA-PSS", "sample-pss-sha3_256.pdf"));
            }
        }
    }
}
