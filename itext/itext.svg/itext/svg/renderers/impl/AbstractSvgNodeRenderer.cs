using System.Collections.Generic;
using iText.IO.Util;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary><inheritDoc/></summary>
    public abstract class AbstractSvgNodeRenderer : ISvgNodeRenderer {
        private ISvgNodeRenderer parent;

        private readonly IList<ISvgNodeRenderer> children = new List<ISvgNodeRenderer>();

        public virtual void SetParent(ISvgNodeRenderer parent) {
            this.parent = parent;
        }

        public virtual ISvgNodeRenderer GetParent() {
            return parent;
        }

        public void AddChild(ISvgNodeRenderer child) {
            // final method, in order to disallow adding null
            if (child != null) {
                children.Add(child);
            }
        }

        public IList<ISvgNodeRenderer> GetChildren() {
            // final method, in order to disallow modifying the List
            return JavaCollectionsUtil.UnmodifiableList(children);
        }

        public abstract void Draw(SvgDrawContext arg1);
    }
}
