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
using System.Collections.Generic;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.ClipperLib {
    public class PolyTreeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ClearPolyTreeTest() {
            PolyTree tree = new PolyTree();
            tree.AddChild(new PolyNode());
            IList<PolyNode> allPolys = tree.m_AllPolys;
            allPolys.Add(new PolyNode());
            NUnit.Framework.Assert.IsFalse(allPolys.IsEmpty());
            NUnit.Framework.Assert.AreEqual(1, tree.ChildCount);
            tree.Clear();
            NUnit.Framework.Assert.IsTrue(allPolys.IsEmpty());
            NUnit.Framework.Assert.AreEqual(0, tree.ChildCount);
        }

        [NUnit.Framework.Test]
        public virtual void GetFistChildInPolyTreeTest() {
            PolyTree tree = new PolyTree();
            PolyNode firstChild = new PolyNode();
            PolyNode secondChild = new PolyNode();
            tree.AddChild(firstChild);
            tree.AddChild(secondChild);
            NUnit.Framework.Assert.AreSame(firstChild, tree.GetFirst());
        }

        [NUnit.Framework.Test]
        public virtual void GetFistChildInEmptyPolyTreeTest() {
            PolyTree tree = new PolyTree();
            NUnit.Framework.Assert.IsNull(tree.GetFirst());
        }

        [NUnit.Framework.Test]
        public virtual void GetTotalSizePolyTreeEmptyTest() {
            PolyTree tree = new PolyTree();
            NUnit.Framework.Assert.AreEqual(0, tree.Total);
        }

        [NUnit.Framework.Test]
        public virtual void GetTotalSizeDifferentPolyNodeTest() {
            PolyTree tree = new PolyTree();
            IList<PolyNode> allPolys = tree.m_AllPolys;
            allPolys.Add(new PolyNode());
            allPolys.Add(new PolyNode());
            tree.AddChild(new PolyNode());
            NUnit.Framework.Assert.AreEqual(1, tree.Total);
        }

        [NUnit.Framework.Test]
        public virtual void GetTotalSizeSamePolyNodeTest() {
            PolyTree tree = new PolyTree();
            PolyNode node = new PolyNode();
            IList<PolyNode> allPolys = tree.m_AllPolys;
            allPolys.Add(node);
            allPolys.Add(new PolyNode());
            tree.AddChild(node);
            NUnit.Framework.Assert.AreEqual(2, tree.Total);
        }
    }
}
