/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.Kernel.Crypto.Securityhandler {
    [NUnit.Framework.Category("UnitTest")]
    public class StandardSecurityHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void EqualsArrayTest() {
            NUnit.Framework.Assert.IsTrue(StandardSecurityHandler.EqualsArray(new byte[] { 1, 2, 3, 4, 5 }, new byte[]
                 { 1, 2, 3, 4, 5 }, 5));
            NUnit.Framework.Assert.IsTrue(StandardSecurityHandler.EqualsArray(new byte[] { 1, 2, 3, 4, 5 }, new byte[]
                 { 1, 2, 3, 9, 0 }, 3));
            NUnit.Framework.Assert.IsFalse(StandardSecurityHandler.EqualsArray(new byte[] { 1, 2, 3, 4, 5 }, new byte[
                ] { 1, 2, 3, 9, 0 }, 4));
            NUnit.Framework.Assert.IsFalse(StandardSecurityHandler.EqualsArray(new byte[] { 1, 2 }, new byte[] { 1, 2 }
                , 3));
            NUnit.Framework.Assert.IsFalse(StandardSecurityHandler.EqualsArray(new byte[] { 1, 2, 3 }, new byte[] { 1, 
                2 }, 3));
        }
    }
}
