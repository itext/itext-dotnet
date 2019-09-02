using iText.IO.Util;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    public class LineRendererUnitTest : AbstractRendererUnitTest {
        private const double EPS = 1e-5;

        [NUnit.Framework.Test]
        public virtual void AdjustChildPositionsAfterReorderingSimpleTest01() {
            Document dummyDocument = CreateDocument();
            IRenderer dummy1 = CreateLayoutedTextRenderer("Hello", dummyDocument);
            IRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            IRenderer dummyImage = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            NUnit.Framework.Assert.AreEqual(0, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            LineRenderer.AdjustChildPositionsAfterReordering(JavaUtil.ArraysAsList(dummy1, dummyImage, dummy2), 10);
            NUnit.Framework.Assert.AreEqual(10, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(37.3359985, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(137.3359985, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 4)]
        public virtual void AdjustChildPositionsAfterReorderingTestWithPercentMargins01() {
            Document dummyDocument = CreateDocument();
            IRenderer dummy1 = CreateLayoutedTextRenderer("Hello", dummyDocument);
            dummy1.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePercentValue(10));
            dummy1.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePercentValue(10));
            dummy1.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePercentValue(10));
            dummy1.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePercentValue(10));
            IRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            NUnit.Framework.Assert.AreEqual(0, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
            LineRenderer.AdjustChildPositionsAfterReordering(JavaUtil.ArraysAsList(dummy1, dummy2), 10);
            NUnit.Framework.Assert.AreEqual(10, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            // If margins and paddings are specified in percents, we treat them as point values for now
            NUnit.Framework.Assert.AreEqual(77.3359985, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void AdjustChildPositionsAfterReorderingTestWithFloats01() {
            Document dummyDocument = CreateDocument();
            IRenderer dummy1 = CreateLayoutedTextRenderer("Hello", dummyDocument);
            IRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            IRenderer dummyImage = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            dummyImage.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            NUnit.Framework.Assert.AreEqual(0, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            LineRenderer.AdjustChildPositionsAfterReordering(JavaUtil.ArraysAsList(dummy1, dummyImage, dummy2), 10);
            NUnit.Framework.Assert.AreEqual(10, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            // Floating renderer is not repositioned
            NUnit.Framework.Assert.AreEqual(0, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(37.3359985, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED)]
        public virtual void InlineBlockWithBigMinWidth01() {
            Document dummyDocument = CreateDocument();
            LineRenderer lineRenderer = (LineRenderer)new LineRenderer().SetParent(dummyDocument.GetRenderer());
            Div div = new Div().SetMinWidth(2000).SetHeight(100);
            DivRenderer inlineBlockRenderer = (DivRenderer)div.CreateRendererSubTree();
            lineRenderer.AddChild(inlineBlockRenderer);
            LayoutResult result = lineRenderer.Layout(new LayoutContext(CreateLayoutArea(1000, 1000)));
            // In case there is an inline block child with large min-width, the inline block child will be force placed (not layouted properly)
            NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, result.GetStatus());
            NUnit.Framework.Assert.AreEqual(0, result.GetOccupiedArea().GetBBox().GetHeight(), EPS);
            NUnit.Framework.Assert.AreEqual(true, inlineBlockRenderer.GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
        }
    }
}
