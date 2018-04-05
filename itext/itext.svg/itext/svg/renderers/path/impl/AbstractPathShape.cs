using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Renderers.Path;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>This class handles common behaviour in IPathShape implementations</summary>
    public abstract class AbstractPathShape : IPathShape {
        public virtual float GetCoordinate(IDictionary<String, String> attributes, String key) {
            String value = "";
            if (attributes != null) {
                value = attributes.Get(key);
            }
            if (value != null && !String.IsNullOrEmpty(value)) {
                return CssUtils.ParseAbsoluteLength(attributes.Get(key));
            }
            return 0;
        }

        public abstract void Draw(PdfCanvas arg1);

        public abstract void SetCoordinates(String[] arg1);

        public abstract void SetProperties(IDictionary<String, String> arg1);
    }
}
