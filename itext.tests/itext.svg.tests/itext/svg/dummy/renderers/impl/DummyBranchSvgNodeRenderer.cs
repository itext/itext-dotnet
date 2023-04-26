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
