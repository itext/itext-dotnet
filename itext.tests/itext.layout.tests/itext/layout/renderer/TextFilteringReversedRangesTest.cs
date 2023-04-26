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
using System.Collections.Generic;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class TextFilteringReversedRangesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Test01() {
            List<int> removedIds = new List<int>();
            removedIds.Add(0);
            int[] range = new int[] { 0, 1 };
            TextRenderer.UpdateRangeBasedOnRemovedCharacters(removedIds, range);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 0 }, range);
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            List<int> removedIds = new List<int>();
            removedIds.Add(10);
            int[] range = new int[] { 0, 5 };
            TextRenderer.UpdateRangeBasedOnRemovedCharacters(removedIds, range);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 5 }, range);
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            List<int> removedIds = new List<int>();
            removedIds.Add(0);
            removedIds.Add(3);
            removedIds.Add(10);
            int[] range = new int[] { 0, 5 };
            TextRenderer.UpdateRangeBasedOnRemovedCharacters(removedIds, range);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 3 }, range);
        }

        [NUnit.Framework.Test]
        public virtual void Test04() {
            List<int> removedIds = new List<int>();
            removedIds.Add(1);
            int[] range = new int[] { 0, 1 };
            TextRenderer.UpdateRangeBasedOnRemovedCharacters(removedIds, range);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 0 }, range);
        }
    }
}
