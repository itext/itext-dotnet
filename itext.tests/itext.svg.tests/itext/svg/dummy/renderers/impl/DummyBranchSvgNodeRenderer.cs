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
using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Svg.Renderers;

namespace iText.Svg.Dummy.Renderers.Impl {
    public class DummyBranchSvgNodeRenderer : DummySvgNodeRenderer, IBranchSvgNodeRenderer {
        internal IList<ISvgNodeRenderer> children = new List<ISvgNodeRenderer>();

        public DummyBranchSvgNodeRenderer(String name)
            : base(name) {
        }

        public virtual void AddChild(ISvgNodeRenderer child) {
            children.Add(child);
        }

        public virtual IList<ISvgNodeRenderer> GetChildren() {
            return children;
        }

        public override void Draw(SvgDrawContext context) {
            System.Console.Out.WriteLine(name + ": Drawing in dummy node, children left: " + children.Count);
        }

        public override bool Equals(Object o) {
            if (!(o is iText.Svg.Dummy.Renderers.Impl.DummyBranchSvgNodeRenderer)) {
                return false;
            }
            //Name
            iText.Svg.Dummy.Renderers.Impl.DummyBranchSvgNodeRenderer otherDummy = (iText.Svg.Dummy.Renderers.Impl.DummyBranchSvgNodeRenderer
                )o;
            if (!this.name.Equals(otherDummy.name)) {
                return false;
            }
            //children
            if (!(this.children.IsEmpty() && otherDummy.children.IsEmpty())) {
                if (this.children.Count != otherDummy.children.Count) {
                    return false;
                }
                bool iterationResult = true;
                for (int i = 0; i < this.children.Count; i++) {
                    iterationResult &= this.children[i].Equals(otherDummy.GetChildren()[i]);
                }
                if (!iterationResult) {
                    return false;
                }
            }
            return true;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }
    }
}
