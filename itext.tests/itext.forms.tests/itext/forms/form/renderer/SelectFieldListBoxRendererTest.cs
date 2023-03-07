using iText.Forms.Form.Element;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Forms.Form.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class SelectFieldListBoxRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetNextRendererTest() {
            SelectFieldListBoxRenderer listBoxRenderer = new SelectFieldListBoxRenderer(new ListBoxField("", 0, false)
                );
            IRenderer nextRenderer = listBoxRenderer.GetNextRenderer();
            NUnit.Framework.Assert.IsTrue(nextRenderer is SelectFieldListBoxRenderer);
        }

        [NUnit.Framework.Test]
        public virtual void AllowLastYLineRecursiveExtractionTest() {
            SelectFieldListBoxRendererTest.CustomSelectFieldListBoxRenderer listBoxRenderer = new SelectFieldListBoxRendererTest.CustomSelectFieldListBoxRenderer
                (new ListBoxField("", 0, false));
            bool lastY = listBoxRenderer.CallAllowLastYLineRecursiveExtraction();
            NUnit.Framework.Assert.IsFalse(lastY);
        }

        private class CustomSelectFieldListBoxRenderer : SelectFieldListBoxRenderer {
            public CustomSelectFieldListBoxRenderer(AbstractSelectField modelElement)
                : base(modelElement) {
            }

            public virtual bool CallAllowLastYLineRecursiveExtraction() {
                return this.AllowLastYLineRecursiveExtraction();
            }
        }
    }
}
