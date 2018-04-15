/*
* To change this license header, choose License Headers in Project Properties.
* To change this template file, choose Tools | Templates
* and open the template in the editor.
*/
using System;
using System.Collections.Generic;
using iText.Svg.Renderers;

namespace iText.Svg.Dummy.Renderers.Impl {
    /// <author>BenoîtLagae</author>
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
        //*/
    }
}
