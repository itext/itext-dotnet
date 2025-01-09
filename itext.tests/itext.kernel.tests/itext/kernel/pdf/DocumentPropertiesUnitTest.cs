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
using iText.Commons.Actions.Contexts;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class DocumentPropertiesUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetEventCountingMetaInfoTest() {
            DocumentProperties documentProperties = new DocumentProperties();
            documentProperties.SetEventCountingMetaInfo(new DocumentPropertiesUnitTest.TestMetaInfo());
            NUnit.Framework.Assert.IsTrue(documentProperties.IsEventCountingMetaInfoSet());
        }

        [NUnit.Framework.Test]
        public virtual void MetaInfoIsNotSetTest() {
            DocumentProperties documentProperties = new DocumentProperties();
            NUnit.Framework.Assert.IsFalse(documentProperties.IsEventCountingMetaInfoSet());
        }

        private class TestMetaInfo : IMetaInfo {
        }
    }
}
