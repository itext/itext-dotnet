using System;
using System.Collections.Generic;
using iText.Svg.Processors;
using iText.Svg.Renderers;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// A wrapper class that encapsulates processing results of
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
    /// objects.
    /// </summary>
    public class DefaultSvgProcessorResult : ISvgProcessorResult {
        private IDictionary<String, ISvgNodeRenderer> namedObjects;

        private ISvgNodeRenderer root;

        public DefaultSvgProcessorResult(IDictionary<String, ISvgNodeRenderer> namedObjects, ISvgNodeRenderer root
            ) {
            this.namedObjects = namedObjects;
            this.root = root;
        }

        public virtual IDictionary<String, ISvgNodeRenderer> GetNamedObjects() {
            return namedObjects;
        }

        public virtual ISvgNodeRenderer GetRootRenderer() {
            return root;
        }

        public override bool Equals(Object o) {
            if (o == null || (!o.GetType().Equals(this.GetType()))) {
                return false;
            }
            iText.Svg.Processors.Impl.DefaultSvgProcessorResult otherResult = (iText.Svg.Processors.Impl.DefaultSvgProcessorResult
                )o;
            return otherResult.GetNamedObjects().Equals(this.GetNamedObjects()) && otherResult.GetRootRenderer().Equals
                (this.GetRootRenderer());
        }

        public override int GetHashCode() {
            int hash = GetNamedObjects().GetHashCode() + 42 * GetRootRenderer().GetHashCode();
            return hash;
        }
    }
}
