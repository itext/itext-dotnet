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
using iText.Kernel.Geom;
using iText.Test;

namespace iText.Layout.Layout {
    [NUnit.Framework.Category("UnitTest")]
    public class LayoutAreaTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CloneTest() {
            RootLayoutArea originalRootLayoutArea = new RootLayoutArea(1, new Rectangle(5, 10, 15, 20));
            originalRootLayoutArea.emptyArea = false;
            LayoutArea cloneAsLayoutArea = ((LayoutArea)originalRootLayoutArea).Clone();
            RootLayoutArea cloneAsRootLayoutArea = (RootLayoutArea)originalRootLayoutArea.Clone();
            NUnit.Framework.Assert.IsTrue((originalRootLayoutArea).GetBBox() != cloneAsLayoutArea.GetBBox());
            NUnit.Framework.Assert.AreEqual(typeof(RootLayoutArea), cloneAsRootLayoutArea.GetType());
            NUnit.Framework.Assert.AreEqual(typeof(RootLayoutArea), cloneAsLayoutArea.GetType());
            NUnit.Framework.Assert.IsFalse(((RootLayoutArea)cloneAsLayoutArea).IsEmptyArea());
        }
    }
}
