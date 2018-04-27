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
    }
}
