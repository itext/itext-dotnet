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

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class StampingPropertiesUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BasicStampingPropertiesTest() {
            StampingProperties stampingProperties = new StampingProperties();
            NUnit.Framework.Assert.IsFalse(stampingProperties.appendMode);
            NUnit.Framework.Assert.IsFalse(stampingProperties.disableMac);
            NUnit.Framework.Assert.IsFalse(stampingProperties.preserveEncryption);
            stampingProperties.UseAppendMode();
            stampingProperties.DisableMac();
            stampingProperties.PreserveEncryption();
            NUnit.Framework.Assert.IsTrue(stampingProperties.appendMode);
            NUnit.Framework.Assert.IsTrue(stampingProperties.disableMac);
            NUnit.Framework.Assert.IsTrue(stampingProperties.preserveEncryption);
        }

        [NUnit.Framework.Test]
        public virtual void CopiedStampingPropertiesTest() {
            StampingProperties stampingProperties = new StampingProperties();
            stampingProperties.UseAppendMode();
            stampingProperties.DisableMac();
            stampingProperties.PreserveEncryption();
            StampingProperties copiedProperties = new StampingProperties(stampingProperties);
            NUnit.Framework.Assert.IsTrue(copiedProperties.appendMode);
            NUnit.Framework.Assert.IsTrue(copiedProperties.disableMac);
            NUnit.Framework.Assert.IsTrue(copiedProperties.preserveEncryption);
        }
    }
}
