using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Element {
    [NUnit.Framework.Category("UnitTest")]
    public class VerticalParagraphTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, LogLevel = LogLevelConstants.WARN, Count = 2)]
        public virtual void SettingUnsupportedPropertiesMustLogWarning() {
            VerticalParagraph verticalParagraph = new VerticalParagraph();
            Leading original = verticalParagraph.GetProperty<Leading>(Property.LEADING);
            verticalParagraph.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            verticalParagraph.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 3f));
            //keep sonar happy; the real test is through the log messages, so we just assert true here
            NUnit.Framework.Assert.AreEqual(original, verticalParagraph.GetProperty<Leading>(Property.LEADING));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, LogLevel = LogLevelConstants.WARN, Count = 1)]
        public virtual void UnsupportedInheritedPropertiesMustLogWarning() {
            VerticalParagraph verticalParagraph = new VerticalParagraph();
            VerticalParagraphTest.TestRenderer parentRenderer = new VerticalParagraphTest.TestRenderer();
            parentRenderer.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 3f));
            parentRenderer.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            IRenderer verticalParagraphRenderer = verticalParagraph.GetRenderer();
            verticalParagraphRenderer.SetParent(parentRenderer);
            NUnit.Framework.Assert.IsNull(verticalParagraphRenderer.GetProperty<Leading>(Property.LEADING));
        }

        private class TestRenderer : AbstractRenderer {
            public TestRenderer()
                : base() {
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                return null;
            }

            public override IRenderer GetNextRenderer() {
                return null;
            }
        }
    }
}
