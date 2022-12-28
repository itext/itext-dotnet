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
