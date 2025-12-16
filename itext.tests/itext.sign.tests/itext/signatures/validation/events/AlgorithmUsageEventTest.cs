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
using iText.Kernel.Crypto;
using iText.Test;

namespace iText.Signatures.Validation.Events {
    [NUnit.Framework.Category("UnitTest")]
    public class AlgorithmUsageEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreationTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("Name", "OID", "Location");
            NUnit.Framework.Assert.AreEqual("Name", sut.GetName());
            NUnit.Framework.Assert.AreEqual("OID", sut.GetOid());
            NUnit.Framework.Assert.AreEqual("Location", sut.GetUsageLocation());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToAdESByOidNegativeTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("MD5", OID.MD5, "Location");
            NUnit.Framework.Assert.IsFalse(sut.IsAllowedAccordingToAdES());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToAdESByNameNegativeTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("MD5", null, "Location");
            NUnit.Framework.Assert.IsFalse(sut.IsAllowedAccordingToAdES());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToAdESByOidPositiveTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("SHA_256", OID.SHA_256, "Location");
            NUnit.Framework.Assert.IsTrue(sut.IsAllowedAccordingToAdES());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToAdESByNamePositiveTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("SHA_256", null, "Location");
            NUnit.Framework.Assert.IsTrue(sut.IsAllowedAccordingToAdES());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToEtsiTs119_312ByOidNegativeTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("SHA", "1.3.14.3.2.26", "Location");
            NUnit.Framework.Assert.IsFalse(sut.IsAllowedAccordingToEtsiTs119_312());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToEtsiTs119_312ByNameNegativeTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("SHA", null, "Location");
            NUnit.Framework.Assert.IsFalse(sut.IsAllowedAccordingToEtsiTs119_312());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToEtsiTs119_312ByOidPositiveTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("sha256WithRsaEncryption", "1.2.840.113549.1.1.11", "Location"
                );
            NUnit.Framework.Assert.IsTrue(sut.IsAllowedAccordingToEtsiTs119_312());
        }

        [NUnit.Framework.Test]
        public virtual void IsAllowedAccordingToEtsiTs119_312ByNamePositiveTest() {
            AlgorithmUsageEvent sut = new AlgorithmUsageEvent("sha256WithRsaEncryption", null, "Location");
            NUnit.Framework.Assert.IsTrue(sut.IsAllowedAccordingToEtsiTs119_312());
        }
    }
}
