/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
