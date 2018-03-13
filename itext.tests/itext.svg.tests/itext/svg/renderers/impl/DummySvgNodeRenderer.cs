using System;
using System.Collections.Generic;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// A dummy implementation of
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// for testing purposes
    /// </summary>
    public class DummySvgNodeRenderer : ISvgNodeRenderer {
        internal ISvgNodeRenderer parent;

        internal IList<ISvgNodeRenderer> children;

        internal String name;

        public DummySvgNodeRenderer(String name, ISvgNodeRenderer parent) {
            this.name = name;
            this.parent = parent;
            this.children = new List<ISvgNodeRenderer>();
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

        public override String ToString() {
            return name;
        }
    }
}
