using System;
using System.Collections.Generic;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Dummy.Factories {
    public class DummySvgNodeMapper : ISvgNodeRendererMapper {
        public virtual IDictionary<String, Type> GetMapping() {
            IDictionary<String, Type> result = new Dictionary<String, Type>();
            result.Put("dummy", typeof(DummySvgNodeRenderer));
            result.Put("processable", typeof(DummyProcessableSvgNodeRenderer));
            result.Put("protected", typeof(DummyProtectedSvgNodeRenderer));
            result.Put("argumented", typeof(DummyArgumentedConstructorSvgNodeRenderer));
            return result;
        }

        public virtual IList<String> GetIgnoredTags() {
            return new List<String>();
        }
    }
}
