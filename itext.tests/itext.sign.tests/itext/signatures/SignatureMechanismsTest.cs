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
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class SignatureMechanismsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetAlgorithmRSATest() {
            NUnit.Framework.Assert.AreEqual("RSA", SignatureMechanisms.GetAlgorithm("1.2.840.113549.1.1.1"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAlgorithmECDSATest() {
            NUnit.Framework.Assert.AreEqual("ECDSA", SignatureMechanisms.GetAlgorithm("1.2.840.10045.2.1"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAlgorithmEmptyTest() {
            NUnit.Framework.Assert.AreEqual("", SignatureMechanisms.GetAlgorithm(""));
        }

        [NUnit.Framework.Test]
        public virtual void GetAlgorithmEmptySpaceTest() {
            NUnit.Framework.Assert.AreEqual(" ", SignatureMechanisms.GetAlgorithm(" "));
        }

        [NUnit.Framework.Test]
        public virtual void GetAlgorithmUndefinedTest() {
            NUnit.Framework.Assert.AreEqual("undefined", SignatureMechanisms.GetAlgorithm("undefined"));
        }
    }
}
