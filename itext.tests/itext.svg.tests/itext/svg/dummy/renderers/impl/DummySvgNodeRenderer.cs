using System;
using System.Collections.Generic;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Dummy.Renderers.Impl {
    /// <summary>
    /// A dummy implementation of
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// for testing purposes
    /// </summary>
    public class DummySvgNodeRenderer : ISvgNodeRenderer {
        internal ISvgNodeRenderer parent;

        internal IList<ISvgNodeRenderer> children;

        internal String name;

        public DummySvgNodeRenderer()
            : this("dummy") {
        }

        public DummySvgNodeRenderer(String name) {
            this.name = name;
            this.children = new List<ISvgNodeRenderer>();
        }

        public virtual void SetParent(ISvgNodeRenderer parent) {
            this.parent = parent;
        }

        public virtual ISvgNodeRenderer GetParent() {
            return parent;
        }

        public virtual void Draw(SvgDrawContext context) {
            System.Console.Out.WriteLine(name + ": Drawing in dummy node, children left: " + children.Count);
        }

        public virtual void AddChild(ISvgNodeRenderer child) {
            children.Add(child);
        }

        public virtual IList<ISvgNodeRenderer> GetChildren() {
            return children;
        }

        public virtual void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles) {
        }

        public virtual String GetAttribute(String key) {
            if (SvgTagConstants.WIDTH.EqualsIgnoreCase(key) || SvgTagConstants.HEIGHT.EqualsIgnoreCase(key)) {
                return "10";
            }
            return "";
        }

        public override String ToString() {
            return name;
        }

        public override bool Equals(Object o) {
            if (!(o is iText.Svg.Dummy.Renderers.Impl.DummySvgNodeRenderer)) {
                return false;
            }
            //Name
            iText.Svg.Dummy.Renderers.Impl.DummySvgNodeRenderer otherDummy = (iText.Svg.Dummy.Renderers.Impl.DummySvgNodeRenderer
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
    }
}
