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
    public class PolyNodeTest : ExtendedITextTest {

        [NUnit.Framework.Test]
        public virtual void AddAndGetChildTest() {
            PolyNode node = new PolyNode();
            PolyNode child = new PolyNode();
            node.AddChild(child);
            NUnit.Framework.Assert.AreSame(child, node.Childs[0]);
            NUnit.Framework.Assert.AreEqual(1, node.Childs.Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetChildCountTest() {
            PolyNode node = new PolyNode();
            node.AddChild(new PolyNode());
            NUnit.Framework.Assert.AreEqual(1, node.ChildCount);
        }

        [NUnit.Framework.Test]
        public virtual void GetContourAndPolygonTest() {
            PolyNode node = new PolyNode();
            NUnit.Framework.Assert.IsTrue(node.Contour is List<IntPoint>);
            NUnit.Framework.Assert.AreSame(node.Contour, node.m_polygon);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetEndTypeTest() {
            PolyNode node = new PolyNode();
            node.m_endtype = EndType.CLOSED_POLYGON;
            NUnit.Framework.Assert.AreEqual(EndType.CLOSED_POLYGON, node.m_endtype);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetJoinTypeTest() {
            PolyNode node = new PolyNode();
            node.m_jointype = JoinType.ROUND;
            NUnit.Framework.Assert.AreEqual(JoinType.ROUND, node.m_jointype);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndIsOpenTest() {
            PolyNode node = new PolyNode();
            node.isOpen = true;
            NUnit.Framework.Assert.IsTrue(node.isOpen);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetParentTest() {
            PolyNode parentNode = new PolyNode();
            PolyNode child = new PolyNode();
            parentNode.AddChild(child);
            NUnit.Framework.Assert.AreSame(parentNode, child.Parent);
        }

        [NUnit.Framework.Test]
        public virtual void GetNextPolyNodeNotEmptyTest() {
            PolyNode node = new PolyNode();
            node.AddChild(new PolyNode());
            node.AddChild(new PolyNode());
            NUnit.Framework.Assert.AreSame(node.Childs[0], node.GetNext());
        }

        [NUnit.Framework.Test]
        public virtual void GetNextNoChildsTest() {
            PolyNode node = new PolyNode();
            NUnit.Framework.Assert.IsNull(node.GetNext());
        }

        [NUnit.Framework.Test]
        public virtual void GetNextPolyNodeWithSiblingTest() {
            PolyNode node = new PolyNode();
            PolyNode child1 = new PolyNode();
            PolyNode child2 = new PolyNode();
            node.AddChild(child1);
            node.AddChild(child2);
            NUnit.Framework.Assert.AreSame(child2, child1.GetNext());
        }

        [NUnit.Framework.Test]
        public virtual void IsHoleTest() {
            PolyNode node = new PolyNode();
            NUnit.Framework.Assert.IsTrue(node.IsHole);
        }

        [NUnit.Framework.Test]
        public virtual void IsNotHoleTest() {
            PolyNode parent = new PolyNode();
            PolyNode child = new PolyNode();
            parent.AddChild(child);
            NUnit.Framework.Assert.IsFalse(child.IsHole);
        }
    }
}
