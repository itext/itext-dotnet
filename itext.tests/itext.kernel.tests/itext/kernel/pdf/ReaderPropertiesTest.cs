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
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class ReaderPropertiesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CopyConstructorTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxXObjectsSizePerPage(10);
            ReaderProperties properties = new ReaderProperties().SetPassword("123".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                )).SetMemoryLimitsAwareHandler(handler);
            ReaderProperties copy = new ReaderProperties(properties);
            NUnit.Framework.Assert.AreEqual(copy.password, properties.password);
            NUnit.Framework.Assert.AreNotEqual(copy.memoryLimitsAwareHandler, properties.memoryLimitsAwareHandler);
            NUnit.Framework.Assert.AreEqual(copy.memoryLimitsAwareHandler.GetMaxXObjectsSizePerPage(), properties.memoryLimitsAwareHandler
                .GetMaxXObjectsSizePerPage());
            NUnit.Framework.Assert.AreEqual(10, copy.memoryLimitsAwareHandler.GetMaxXObjectsSizePerPage());
        }
    }
}
