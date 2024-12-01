using System;
using System.Collections.Generic;
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class CircleSvgNodeRendererUnitTest {
        [NUnit.Framework.Test]
        public virtual void NoViewPortTest() {
            CircleSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            SvgDrawContext context = new SvgDrawContext(new ResourceResolver(""), new FontProvider());
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("r", "50%");
            renderer.SetAttributesAndStyles(styles);
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => renderer.SetParameters(context
                ));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.ILLEGAL_RELATIVE_VALUE_NO_VIEWPORT_IS_SET, e.Message
                );
        }
    }
}
